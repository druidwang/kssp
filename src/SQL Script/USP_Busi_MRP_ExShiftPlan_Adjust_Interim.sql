USE [Sconit]
GO
/****** Object:  StoredProcedure [dbo].[USP_Busi_MRP_ExShiftPlan_Adjust_Interim]    Script Date: 01/05/2015 16:21:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_Busi_MRP_ExShiftPlan_Adjust_Interim]
(
	@WeekIndex varchar(50),
	@PlanVersion datetime,
	@Para varchar(max),
	@Type varchar(50) 
)
AS
BEGIN
set nocount on
----00001***** 提示不可以移线的断面  20130926
----00002***** 早班开始时间根据Prd_ShiftDet里面去取，目前系统运算是以0点开始的，所以根据Prd_ShiftDet里面的开始时间整体减去时间差  20131027
----00003*****  物料比例去BOM里面临时抓		2013104
----00004***** 手动上传的调整版本          20131112
----00005*****  不计算废品率          20131119
----00006*****  根据 FlowDet 里面的Sequence更新 ItemPlan里面的Sequence.	20131125
----00007*****  换模时间考虑跨周.	20140103
----00008*****  加上Seq条件	20140312
----00009*****  Upper 线别  20140928
----00010*****	Mrp_ExPlan表的数据都是原始的数据，不能修改，这里用过渡表代替
----00011*****  出断面中包含的物料，如果在挤出生产线上不存在，那该物料就不进行排产。 20141209
--VB的string容纳大的字符串的时候会有一些换行符截断字符串，这里在开始对传进来的参数进行消除动作
Select @para= REPLACE(REPLACE(@para,CHAR(13),''),CHAR(10),'')/*,@PlanVersion =dbo.FormatDate(GetDate(),'YYYY-MM-DD HH:NN:SS')*/
Declare @CurDtae datetime =Getdate()
--select len(@para)-len(REPLACE(@para,'290010','')) 
--return
--Create table to show nonproductive time 
Create Table #UnproductiveCode(Code varchar(2),Desc1 varchar(20))
Insert into #UnproductiveCode
	select code,description from Mrp_producttype
--Insert into #UnproductiveCode values('A','批产01'),('B','工业化02'),('C','开发03'),('D','备模'),('E','工装'),('F','设备'),('G','材料'),
--	('H','工艺'),('J','移线'),('K','攻关'),('L','改进'),('M','保养'),('N','能源'),('P','停产'),('Q','工程更改'),('R','配件'),('Z','其他')
Create Table #ProductType(Code varchar(5),CType int)
Declare @int int=65
while @int<91
Begin
	Insert into #ProductType values(CHAR(@int),(@int-64)*10)
	Set @int=@int+1
End
--Select * from #UnproductiveCode
--Select * from #ProductType

--
If @type='Get'
	Begin
		--USP_Busi_MRP_ExShiftPlan @WeekIndex,'2013/7/11 14:12:04',
		--'EX01=5;EX01=6;EX01=7;EX01=8;EX02=9;EX02=10;EX02=11;EX02=12;EX03=13;EX03=14;EX03=15;EX03=16;EX04=17;EX04=18;EX04=19;EX04=20;EX05=21;EX05=22;EX05=23;EX05=24;EX06=25;EX06=26;EX06=27;EX06=28;EX07=29;EX07=30;EX07=31;EX07=32;EX08=33;EX08=34;EX08=35;EX08=36;EX09=37;EX09=38;EX09=39;EX09=40;EX10=41;EX10=42;EX10=43;EX10=44;EX11=45;EX11=46;EX11=47;EX11=48;EX12=49;EX12=50;EX12=51;EX12=52;EX13=53;EX13=54;EX13=55;EX13=56;EX14=57;EX14=58;EX14=59;EX14=60;','Get'
		--Select * from dbo.[Func_SplitStr_ItemValue](@Para,',')
		--取得计划的周末日期
		Declare @WeekEnd datetime
		declare @tmpdate1 datetime
		Select @tmpdate1=dateadd(week,cast(substring(@WeekIndex,charindex('-',@WeekIndex,1)+1,2)as int),left(@WeekIndex,4)+'-01-01')
		Select  @WeekEnd= dateadd(day,-datepart(weekday,@tmpdate1)+2,@tmpdate1)
		--Select * into #SplitTable from dbo.[Func_SplitStr_ItemValue](@Para,';')
		--Select DiagItem,row_number()over(partition by DiagItem order by diagvalue) As NONO into #BaseJoinTable from #SplitTable
		----Weekdayindex 1 to 7 means monday to sunday , 0 number means the starttime is over the time span of this week
		Select top 0 productline,Section,/*b.desc1*/a.SectionDescription desc1,a.ProductType,cast(shiftqty as varchar)shiftqty,case when plandate>=@WeekEnd then 8 else datepart(weekday,plandate)-1 end as weekdayindex,
			row_number()over(partition by productline ,case when plandate>=@WeekEnd then 8 else datepart(weekday,plandate)-1 end  order by plandate) as seq
			into #weekshiftplan  from MRP_MrpExSectionPlan a with(nolock),MD_Item b with(nolock) 

		--------------------------------------------
		Create table #BaseJoinTable(diagitem varchar(20),NONO int)
		select productline,4 As linerows into #productline from MRP_ProdLineEx with(nolock) Group by ProductLine
		declare @InsertLine varchar(20),@InsertSeq int,@insertrows int=4
		while exists (select top 1 * from #productline)
			Begin
				select @InsertLine=productline,@insertrows=linerows from #productline order by productline
				----一个线别至少5行
				If @insertrows<4 Begin Set @insertrows=4 End
				while @insertrows>0
					Begin
						Insert into #BaseJoinTable
							Select @InsertLine,@insertrows
						Set @insertrows =@insertrows -1
					End
				delete #productline where productline=@InsertLine
			End
			--productline	Section	desc1	ProductType	shiftqty	weekdayindex	seq
			--EX13	299999	停产	P	P3	6	1
			Insert into #weekshiftplan
				select productline,'','','','','',1 from #productline
		
		Create index IX_B on #BaseJoinTable(diagitem);Create index IX_B on #weekshiftplan(productline)
		----如果一天都没有排产就设置计划为"保养" 3个班别
		--Declare @chkPlan varchar(100)=''
		select * into #ChkPlanTable from (select distinct productline from #weekshiftplan)a,
			(select 1 as weekdayindex union select 2 union select 3 union select 4 union select 5 union select 6 union select 7)b
		Insert into #weekshiftplan
			select distinct a.ProductLine ,'' As section ,'' As desc1,'' as remark,''As shiftQty,a.weekdayindex,1 As Seq 
				from #ChkPlanTable a left join #weekshiftplan b on a.ProductLine=b.ProductLine and a.weekdayindex=b.weekdayindex
				where b.ProductLine is null
		------
		select COUNT(*) As TotalRow from #BaseJoinTable
		Select a.diagitem As productline,isnull(b.section,'')aa,isnull(b.desc1,'')bb,isnull(b.shiftqty,'')cc
			 ,isnull(c.section,'')dd,isnull(c.desc1,'')ee,isnull(c.shiftqty,'')ff
			 ,isnull(d.section,'')gg,isnull(d.desc1,'')hh,isnull(d.shiftqty,'')ii
			 ,isnull(e.section,'')jj,isnull(e.desc1,'')kk,isnull(e.shiftqty,'')ll
			 ,isnull(f.section,'')mm,isnull(f.desc1,'')oo,isnull(f.shiftqty,'')pp
			 ,isnull(g.section,'')qq,isnull(g.desc1,'')rr,isnull(g.shiftqty,'')ss
			 ,isnull(h.section,'')tt,isnull(h.desc1,'')uu,isnull(h.shiftqty,'')vv
			 ,isnull(i.section,'')ww,isnull(i.desc1,'')xx,isnull(i.shiftqty,'')yy
			 from #BaseJoinTable a with(index=IX_B)
			 left join (Select * from #weekshiftplan with(index=IX_B) where weekdayindex =1) b on a.diagitem=b.productline and a.NONO=b.seq
			 left join (Select * from #weekshiftplan with(index=IX_B) where weekdayindex =2) c  on a.diagitem=c.productline and a.NONO=c.seq
			 left join (Select * from #weekshiftplan with(index=IX_B) where weekdayindex =3) d  on a.diagitem=d.productline and a.NONO=d.seq
			 left join (Select * from #weekshiftplan with(index=IX_B) where weekdayindex =4) e  on a.diagitem=e.productline and a.NONO=e.seq
			 left join (Select * from #weekshiftplan with(index=IX_B) where weekdayindex =5) f  on a.diagitem=f.productline and a.NONO=f.seq
			 left join (Select * from #weekshiftplan with(index=IX_B) where weekdayindex =6) g on a.diagitem=g.productline and a.NONO=g.seq
			 left join (Select * from #weekshiftplan with(index=IX_B) where weekdayindex =7) h  on a.diagitem=h.productline and a.NONO=h.seq
			 left join (Select * from #weekshiftplan with(index=IX_B) where weekdayindex =8) i  on a.diagitem=i.productline and a.NONO=i.seq
			 Order by a.diagitem,a.NONO 
	select dbo.FormatDate(dateadd(day,-7,@WeekEnd),'MM-DD'),dbo.FormatDate(dateadd(day,-6,@WeekEnd),'MM-DD'),dbo.FormatDate(dateadd(day,-5,@WeekEnd),'MM-DD'),dbo.FormatDate(dateadd(day,-4,@WeekEnd),'MM-DD'),
		dbo.FormatDate(dateadd(day,-3,@WeekEnd),'MM-DD'),dbo.FormatDate(dateadd(day,-2,@WeekEnd),'MM-DD'),dbo.FormatDate(dateadd(day,-1,@WeekEnd),'MM-DD')

	End
If @type='Update'
	Begin
			--declare @paraAll varchar(max)= 'prodLine=EX01,section=290010,Qty=2,seq=5,weekday=星期一;prodLine=EX03,section=290046,Qty=5.5,seq=13,weekday=星期一;prodLine=EX06,section=290101,Qty=3,seq=25,weekday=星期一;prodLine=EX10,section=290159,Qty=2,seq=41,weekday=星期一;prodLine=EX10,section=290156,Qty=2.25,seq=42,weekday=星期一;prodLine=EX01,section=290010,Qty=3,seq=5,weekday=星期二;prodLine=EX02,section=290039,Qty=3,seq=9,weekday=星期二;prodLine=EX03,section=290046,Qty=8,seq=13,weekday=星期二;prodLine=EX04,section=290068,Qty=2,seq=17,weekday=星期二;prodLine=EX05,section=290079,Qty=1.5,seq=21,weekday=星期二;prodLine=EX05,section=290087,Qty=2,seq=22,weekday=星期二;prodLine=EX06,section=290098,Qty=6,seq=25,weekday=星期二;prodLine=EX06,section=290102,Qty=2,seq=26,weekday=星期二;prodLine=EX06,section=290098,Qty=1.5,seq=27,weekday=星期二;prodLine=EX07,section=290109,Qty=1.5,seq=29,weekday=星期二;prodLine=EX08,section=290134,Qty=2,seq=33,weekday=星期二;prodLine=EX08,section=290132,Qty=2,seq=34,weekday=星期二;prodLine=EX09,section=290154,Qty=3,seq=37,weekday=星期二;prodLin
				--	e=EX10,section=290103,Qty=12,seq=41,weekday=星期二;prodLine=EX10,section=290103,Qty=15.5,seq=42,weekday=星期二;prodLine=EX11,section=290173,Qty=5,seq=45,weekday=星期二;prodLine=EX12,section=290175,Qty=1.5,seq=49,weekday=星期二;prodLine=EX12,section=290182,Qty=1.5,seq=50,weekday=星期二;prodLine=EX13,section=290030,Qty=4.25,seq=53,weekday=星期二;prodLine=EX14,section=290030,Qty=3,seq=57,weekday=星期二;prodLine=EX01,section=290019,Qty=1.5,seq=5,weekday=星期三;prodLine=EX01,section=290009,Qty=6.75,seq=6,weekday=星期三;prodLine=EX02,section=290013,Qty=3,seq=9,weekday=星期三;prodLine=EX03,section=290044,Qty=2.5,seq=13,weekday=星期三;prodLine=EX05,section=290088,Qty=8,seq=21,weekday=星期三;prodLine=EX06,section=290104,Qty=0.5,seq=25,weekday=星期三;prodLine=EX08,section=290133,Qty=3,seq=33,weekday=星期三;prodLine=EX09,section=290051,Qty=3,seq=37,weekday=星期三;prodLine=EX11,section=290173,Qty=2,seq=45,weekday=星期三;prodLine=EX12,section=290187,Qty=1.5,seq=49,weekday=星期三;prodLine=EX13,section=290196,Qty=1.5,seq=53,weekday=星期三;p
				--	rodLine=EX13,section=290197,Qty=3,seq=54,weekday=星期三;prodLine=EX02,section=290035,Qty=2,seq=9,weekday=星期四;prodLine=EX02,section=290039,Qty=1.25,seq=10,weekday=星期四;prodLine=EX03,section=290044,Qty=5,seq=13,weekday=星期四;prodLine=EX06,section=290104,Qty=5,seq=25,weekday=星期四;prodLine=EX09,section=290150,Qty=7.75,seq=37,weekday=星期四;prodLine=EX01,section=290014,Qty=1.5,seq=5,weekday=星期五;prodLine=EX05,section=290096,Qty=2,seq=21,weekday=星期五;prodLine=EX06,section=290103,Qty=7,seq=25,weekday=星期五;prodLine=EX01,section=290015,Qty=1.5,seq=5,weekday=星期六;prodLine=EX01,section=290013,Qty=3,seq=6,weekday=星期六;prodLine=EX03,section=290051,Qty=3,seq=13,weekday=星期六;prodLine=EX10,section=290160,Qty=6,seq=41,weekday=星期六;prodLine=EX01,section=290022,Qty=1.5,seq=5,weekday=星期日;prodLine=EX03,section=290048,Qty=2.5,seq=13,weekday=星期日;prodLine=EX10,section=290160,Qty=1.5,seq=41,weekday=星期日
			--'
 
			Declare @prodLine varchar(20),@section varchar(20),@Qty_1 varchar(100),@seq int,@Group int=0,@para_1 varchar(1000)
			Declare @weekday varchar(20),@sectiondesc varchar(50)
			Select * into #ParaListGroup from dbo.[Func_SplitStr](@para,';')
			Create table #ParaList(diagitem varchar(100),diagvalue varchar(100))
			Create table #SplitResult(prodLine varchar(100),section varchar(100),Qty varchar(100),[weekday] varchar(20),
				sectiondesc varchar(50),TypeCode varchar(5),seq int,[group] int)
			Declare @MatchTypeCode varchar(50)='[ABCDEFGHIJKLMNOPQRSTUVWXYZ]'
			while Exists (Select top 1* from #ParaListGroup)
				Begin
					Select top 1 @para_1=F1 from #ParaListGroup
					Insert into #ParaList 
					Select * from dbo.[Func_SplitStr_ItemValue](@para_1,',')
					Select @prodLine=replace(diagvalue,' ','') from #ParaList where diagitem='prodLine'
					Select @section=replace(diagvalue,' ','') from #ParaList where diagitem='section'
					Select @weekday=diagvalue from #ParaList where diagitem='weekday'
					Select @sectiondesc=diagvalue from #ParaList where diagitem='sectiondesc'
					----有人会不小心敲进了换行符或者是回车符
					--Update #ParaList
					--	Set diagvalue =SUBSTRING(LTRIM(rtrim(diagvalue)),1,20) from #ParaList 
					--	where SUBSTRING(LTRIM(rtrim(diagvalue)),1,1) like @MatchTypeCode and diagitem='Qty'
					Select @Qty_1=REPLACE(REPLACE(REPLACE(diagvalue,CHAR(10),''),CHAR(13),''),' ','') from #ParaList where diagitem='Qty'
					Select @seq=diagvalue from #ParaList where diagitem='seq'
					Set @group =@group+1
					Insert into #SplitResult Select @prodLine,@section,@Qty_1,@weekday,@sectiondesc,'',@seq,@group
					delete from #ParaListGroup where F1=@para_1
					delete #ParaList
				End
			Update #SplitResult Set TypeCode =SUBSTRING(LTRIM(rtrim(qty)),1,1),Qty =SUBSTRING(LTRIM(rtrim(qty)),2,20) where SUBSTRING(LTRIM(rtrim(qty)),1,1) like @MatchTypeCode
			--Update #SplitResult Set TypeCode='O' where TypeCode=''
			--已经没有枚举类型了
			--Update a
			--	Set a.TypeCode =b.CType from #SplitResult a,#ProductType b where a.TypeCode =b.Code 
			Update a
				Set a.Seq=b.seqnew from #SplitResult a,(Select *,row_number()over(partition by prodLine,weekday order by seq) As SeqNew  from #SplitResult) b
					Where a.prodLine=b.prodLine and a.section=b.section and a.weekday=b.weekday and a.qty=b.qty
					--0008
					and a.seq=b.seq
			 --Drop table #SplitResult_1
			 Declare @WeekStart datetime
			 Declare @TempWeekStart datetime
			 declare @tmpdate datetime
			 declare @DateIndex varchar(20)=@WeekIndex
			 Print '@DateIndex='+@DateIndex
			 Select @tmpdate=dateadd(week,cast(substring(@DateIndex,charindex('-',@DateIndex,1)+1,2)as int),left(@DateIndex,4)+'-01-01')
			 Select @WeekStart=dateadd(day,-datepart(weekday,@tmpdate)-5,@tmpdate)
			 Set @TempWeekStart=@WeekStart
			 ----0007
			 declare @LastDateIndex varchar(20)=''
			 declare @EndDateOfLastWeek datetime=dateadd(day,-datepart(weekday,@tmpdate)-6,@tmpdate)
			 select @LastDateIndex=MAX(DateIndex) from MRP_MrpExPlanMaster where DateIndex !=@WeekIndex
			 ----0007
			 Select *,GETDATE () As weekdatetime, case when ltrim(rtrim(weekday))='星期一' then 0 when ltrim(rtrim(weekday))='星期二' then 1 
				when ltrim(rtrim(weekday))='星期三' then 2 when ltrim(rtrim(weekday))='星期四' then 3 
				when ltrim(rtrim(weekday))='星期五' then 4 when ltrim(rtrim(weekday))='星期六' then 5 
				when ltrim(rtrim(weekday))='星期日' then 6 else 7 End  As weekindex ,'N' As NoSwitchTime,'N' As LastShift,GETDATE() As StartTime,GETDATE() As WindowTime  into #SplitResult_1 from #SplitResult
			Update #SplitResult_1 Set weekdatetime =dateadd(day,weekindex,@WeekStart)
			-----Check digital format
			Declare @Msg varchar(max)=''
			select prodLine,weekday into #WarningDigitalFormat from #SplitResult_1 where ISNUMERIC(qty)=0
			If @@ROWCOUNT !=0
				Begin
					--Declare @Msg varchar(max)=''
					select @Msg=@Msg+prodLine+','+weekday+';'from #WarningDigitalFormat
					select @Msg=@Msg+'----以上的线别对应日期数字输入格式错误'
					select 'NG' As ChkRst,@Msg As Msg
					return
				End
			-----Check 排班数是否溢出
			select prodLine,weekday ,SUM(cast(qty as float)) as SQTY into #Warning from #SplitResult_1 where weekday!='溢出' group by prodLine,weekday having SUM(cast(qty as float))>3
			If @@ROWCOUNT !=0
				Begin
					--Declare @Msg varchar(max)=''
						select @Msg=@Msg+prodLine+','+weekday+';'from #Warning
					select @Msg=@Msg+'----以上的线别对应日期生产班别溢出(Total>3)'
					select 'NG' As ChkRst,@Msg As Msg
					return
				End
			-----是否满班
			select prodLine,weekday ,SUM(cast(qty as float)) as SQTY into #Warning01 from #SplitResult_1 where weekday!='溢出' group by prodLine,weekday having SUM(cast(qty as float))<3
			If @@ROWCOUNT !=0
				Begin
					--Declare @Msg varchar(max)=''
						select @Msg=@Msg+prodLine+','+weekday+';'from #Warning01
					select @Msg=@Msg+'----以上的线别对应日期生产班别没有排满(Total<3),如果确实是空置的工时，请用P(停产)补充误差。'
					select 'NG' As ChkRst,@Msg As Msg
					return
				End
			-----Check if section is defined in system
			select * into #CheckSectionFormat from #SplitResult_1 
				where section not in( select Item from MRP_ProdLineEx with(nolock))
				and section !='299999'
			If @@ROWCOUNT !=0
				Begin
					select @Msg=@Msg+prodLine+','+weekday+section+';'from #CheckSectionFormat
					select @Msg=@Msg+'----以上的线别对应日期的断面格式错误!'
					select 'NG' As ChkRst,@Msg As Msg
					return
				End
			------------
				----0001****
				select * into #CheckSectionProperty from #SplitResult_1 a where a.TypeCode in ('A','B') 
					and not exists(select * from MRP_ProdLineEx b where a.section =b.Item and a.prodLine=b.ProductLine )
					if @@ROWCOUNT !=0
					Begin
						select @Msg=@Msg+prodLine+','+weekday+section+';'from #CheckSectionProperty
						select @Msg=@Msg+'----以上的断面不可以移线，请确认!'
						select 'NG' As ChkRst,@Msg As Msg
						return
					End
				select * into #CheckWrongSection from #SplitResult_1 a where a.section !='299999'
					and not exists(select * from MRP_ProdLineEx b where a.section =b.Item)
					if @@ROWCOUNT !=0
					Begin
						select @Msg=@Msg+prodLine+','+weekday+section+';'from #CheckWrongSection
						select @Msg=@Msg+'----以上的断面的书写不正确，请确认!'
						select 'NG' As ChkRst,@Msg As Msg
						return
					End
			------------
			-----*****FindOut不需要换模的断面Begin溢出部分也要排进去(Log --0001)******
			select prodline, MAX(seq) seq ,weekindex into #SplitResult_ChkSwitch
				from #SplitResult_1  where section !='P' /*0001*and weekindex !=7/*溢出不管*/*/ group by prodline, weekindex
			select b.* into #temp1 from #SplitResult_ChkSwitch a,#SplitResult_1 b where a.prodLine =b.prodLine and a.seq=b.seq and a.weekindex =b.weekindex 
			update #temp1 set weekindex = weekindex+1
			select b.* into #NoMinusSwitch from #SplitResult_1 a,#temp1 b where a.seq =1 and a.prodLine =b.prodLine and a.weekindex =b.weekindex and a.section=b.section 
			
			update #NoMinusSwitch set weekindex = weekindex-1
			Insert into #NoMinusSwitch
				select a.*from #SplitResult_1 a,#temp1 b where a.seq =1 and a.prodLine =b.prodLine and a.weekindex =b.weekindex and a.section=b.section
					--and a.weekindex !=7--0001
			----将连续的隔天生产的断面汇总
			select distinct * into #NoMinusSwitch_1 from #NoMinusSwitch
			----有可能一条线出现*不连续*的两次断面隔天连着生产，所以要再细化Group
			select *,DATEADD(DAY,-weekindex,weekdatetime) As TimeGroup into #NoMinusSwitch_2 from #NoMinusSwitch_1
			Delete a from (select *,ROW_NUMBER()over(partition by prodLine,section,TimeGroup order by weekindex asc) As NONO  from #NoMinusSwitch_2)a 
				where a.nono=1
			Update b Set NoSwitchTime='Y' from #NoMinusSwitch_2 a,#SplitResult_1 b where a.prodLine =b.prodLine and a.seq=b.seq  and a.weekindex =b.weekindex and a.section=b.section  
			--select section ,COUNT(distinct prodLine) from #SplitResult_1 group by  section
			--having COUNT(distinct prodLine)>1
			------断面的最后班By线别&断面罗列
			update a Set a.LastShift ='Y' from (select *,ROW_NUMBER()over(PARTITION by prodline,section order by weekindex desc,seq desc ) As NONO from #SplitResult_1 )a
				Where a.NONO=1
			----0007
			select   b.*,ROW_NUMBER()over(partition by/* b.section,*/b.productline order by b.starttime desc) As NONO 
				into #GetEndSectionOfLastWeek from MRP_MrpExPlanMaster a,MRP_MrpExSectionPlan b 
				where a.DateIndex =@LastDateIndex and IsActive =1 and a.DateIndex =b.DateIndex
				and a.PlanVersion=b.PlanVersion and a.PlanDate =b.PlanDate and 
				a.ProductLine=b.ProductLine and a.PlanDate =@EndDateOfLastWeek
				and Section !='299999'  
			Delete #GetEndSectionOfLastWeek where NONO !=1
			Update a
				Set NoSwitchTime='Y' from (select *,ROW_NUMBER()over(partition by prodLine order by seq) As NONO 
				from #SplitResult_1 where weekindex=0) a where exists(select 0 from #GetEndSectionOfLastWeek b 
				where a.section=b.Section and a.prodLine=b.ProductLine) and a.NONO =1
			----0007
			-----*****FindOut不需要换模的断面End
			
			--Select * from #SplitResult_1 order by prodLine ,weekdatetime 
			--Return
			--SP_help 'MRP_prodlineex'
			----By Line & Weekday process one by one
			----Get all sections' shift quato Qty :MrpSpeed =9 means 1 min has 9 meter output 
			Select Item As Section ,Mrpspeed*24*60/ShiftType As ShiftQuatoQty into #MRP_prodlineex from MRP_prodlineex with(nolock)
			----00009Begin
			Select top 0 * into #MRP_MrpExPlan_Temp from MRP_MrpExPlan where 1=2
			----00009End
			----Get all sections' weekly request
			----Select * from mrp_mrpExPlan
			----MRP_prodlineex:一个断面会在两条线投产，这样 Join出来的sectionQty是错的，所以这儿进行过滤
			select * into #MRP_prodlineex_1 from (Select *,ROW_NUMBER()Over (partition by item order by productline) AS NONO from MRP_prodlineex)a where a.NONO=1
			create index IX_M on #MRP_prodlineex_1(Item)
			Select a.section ,sum (a.sectionQty) as sectionQty,sum ((a.sectionQty)*(b.Correction-1)) as correctionQty into #Mrp_mrpExPlan 
				from #MRP_MrpExPlan_Temp a with(nolock),#MRP_prodlineex_1 b  with(nolock) where a.Section =b.Item and a.DateIndex =@WeekIndex
				and PlanVersion =@PlanVersion group by a.Section
			--Select * from #MRP_prodlineex
			--select * from #Mrp_mrpExPlan where section='290059'
			Select * from #SplitResult_1 order by prodline,weekdatetime
			----Get Begin and End Time for each shift plan***Begin
			Select Distinct prodLine,weekdatetime into #LinePlandateGroup from #SplitResult_1 order by prodline,weekdatetime
			Declare @prodLineForLoop varchar(20),@DateTimeForLoop datetime,@SeqForGroup int,@ShiftQtyForGroup float=0,@ShiftQtyForGroupInterim float=0
			while exists(select top 1 * from #LinePlandateGroup)
				Begin
					select @ShiftQtyForGroup =0,@ShiftQtyForGroupInterim =0,@SeqForGroup=1
					select @prodLineForLoop=prodLine,@DateTimeForLoop=weekdatetime from #LinePlandateGroup
					while exists(select top 1 * from #SplitResult_1 where prodLine=@prodLineForLoop and weekdatetime=@DateTimeForLoop and seq=@SeqForGroup)
						Begin
							select top 1 @prodLineForLoop=prodLine,@DateTimeForLoop=weekdatetime ,@ShiftQtyForGroupInterim=Qty
								from #SplitResult_1 where prodLine=@prodLineForLoop and weekdatetime=@DateTimeForLoop and seq=@SeqForGroup
							Set @ShiftQtyForGroup=@ShiftQtyForGroup+@ShiftQtyForGroupInterim
							Update #SplitResult_1 
								Set StartTime =dateadd(minute,(@ShiftQtyForGroup-@ShiftQtyForGroupInterim)*8*60,weekdatetime),
									WindowTime =dateadd(minute,@ShiftQtyForGroup*8*60,weekdatetime)
									Where prodLine=@prodLineForLoop and weekdatetime=@DateTimeForLoop and seq=@SeqForGroup
						    Set @SeqForGroup=@SeqForGroup+1
						End
					delete from #LinePlandateGroup where prodLine=@prodLineForLoop and weekdatetime=@DateTimeForLoop
				End
			--按照每个断面，每条线对应的班别数来平均在不同的线别生产的断面的需求量个修正量Begin
			--Drop table #TotalShiftForSection;Drop table #TotalShiftForSectionAndLine
			select section,SUM(cast(QTY As Float))As QTY into #TotalShiftForSection from #SplitResult_1 Group by section
			select Prodline,section,SUM(cast(QTY As Float))As QTY ,CAST(0 As float) As SectionTotal into #TotalShiftForSectionAndLine from #SplitResult_1 Group by Prodline,section
			Update b set b.SectionTotal=a.QTY  from #TotalShiftForSection a,#TotalShiftForSectionAndLine b where a.section=b.section 
			--select * from #Mrp_mrpExPlanByLine where section='290013'
			--select * from #Mrp_mrpExPlan where section='290013'
			select b.prodLine,b.section,b.QTY*a.sectionQty/b.SectionTotal As SectionQty,b.QTY*a.correctionQty/b.SectionTotal As correctionQty
				into #Mrp_mrpExPlanByLine from #Mrp_mrpExPlan a,#TotalShiftForSectionAndLine b where a.Section =b.section 
			----按照每个断面，每条线对应的班别数来平均在不同的线别生产的断面的需求量个修正量End
			
			Select * from #SplitResult_1 order by prodLine ,weekdatetime 
			--Return
			--return
			--Select * from #Mrp_mrpExPlan where section not in (Select section from #SplitResult)
			--Select * from MD_Item where desc1 like '%虚拟%'--299999 

			Delete _MRP_MrpExSectionPlan_

			--Select top 0 * into _MRP_MrpExSectionPlan_ from MRP_MrpExSectionPlan
			--Select top 1000 * from MRP_MrpExSectionPlan
		Declare @ProductLine varchar (50),/*@Section varchar (50),@DateIndex varchar (10),*/@PlanDate datetime,/*@PlanVersion datetime,*/@Sequence int,@Qty float,@CorrectionQty float 
		Declare @AdjustQty float,@Speed float,@ApsPriority int,@SwitchTime float,@SpeedTimes int,@Quota float,@MinLotSize float,@EconomicLotSize float,@MaxLotSize float
		Declare @TurnQty int,@Correction float,@ShiftType int,@TotalAps int,@TotalQuota float,@StartTime datetime2 (7),@WindowTime datetime2 (7),@UpTime float  
		Declare @PlanNo varchar (50),@ShiftQty float,@ProductType varchar(5),@LatestStartTime datetime2 (7),@Remark varchar (50),@Logs varchar (4000)
		Declare @IsEconomic bit,@Shift varchar (50),@CurrentSwitchTime float,@SectionDescription varchar (100),@IsOld bit,@LastModifyDate datetime 
		Declare @WeekSectionRequestQty float,@seqforweekday varchar(10),@Groupdel as int ,@NoSwitchTime char(1),@LastShift char(1)
		Declare @TempCorrectQty As float,@UnproductiveCode varchar(10),@UnproductiveCodeDesc varchar(50)
		While exists(Select top 100 * from #SplitResult_1)
			Begin
				----初始化变量
				Select @ProductLine =null,@PlanDate =null,@Sequence =0,@Qty =0,@CorrectionQty =0 ,@AdjustQty =0,@Speed =0,@ApsPriority =0,@SwitchTime =0,
				@SpeedTimes =0,@Quota =0,@MinLotSize =0,@EconomicLotSize =0,@MaxLotSize =0,@TurnQty =0,@Correction =0,@ShiftType =0,@TotalAps =0,
				@TotalQuota =0,@StartTime =null,@WindowTime =null,@UpTime =0,@PlanNo =null,@ShiftQty =0,@ProductType ='',@LatestStartTime =null,
				@Remark =null,@Logs =null,@IsEconomic =0,@Shift ='',@CurrentSwitchTime =0,@SectionDescription =null,@IsOld =null,@LastModifyDate=null,
				@WeekSectionRequestQty =0,@seqforweekday =null,@Groupdel =0 ,@NoSwitchTime=null,@LastShift =null,@TempCorrectQty =0,@UnproductiveCode=null,
				@UnproductiveCodeDesc=null
				Select @Qty=0,@AdjustQty=0,@CorrectionQty=0,@TempCorrectQty=0,@WeekSectionRequestQty=0,@NoSwitchTime='N',@LastShift='N',@NoSwitchTime=0
				----先减需求，再减修正，再调整
				Select top 1 @ProductLine=ProdLine,@Section=section,@ShiftQty=Qty,@SectionDescription=sectiondesc,@DateIndex=@WeekIndex,@PlanDate=weekdatetime
					,@productType=TypeCode,@seqforweekday=seq,@Groupdel=[group],@NoSwitchTime=NoSwitchTime,@LastShift=LastShift, @StartTime=StartTime 
					,@WindowTime=WindowTime from #SplitResult_1  order by ProdLine ,weekdatetime
				--每一天的NO.1计划序列为1
				If @seqforweekday='1'
					Begin
						Set @TempWeekStart=@PlanDate
					End
				----对非生产断面做特殊处理
				--If len(@section)=1
				--	Begin
				--		Set @UnproductiveCode=@section
				--		--select @UnproductiveCodeDesc=Desc1 from #UnproductiveCode where Code =@section 
				--		Set @section='299999'
				--	End
				--Select @PlanVersion_1=@PlanVersion
				Select @Qty=MrpSpeed*speedtimes*(60*24*@ShiftQty/shifttype-case when @NoSwitchTime ='N' then switchtime else 0 End),
					  @CurrentSwitchTime=case when @NoSwitchTime='N' then SwitchTime else 0 End from MRP_prodlineex with(nolock) 
					  where Item=@section and ProductLine like 
					  --试制的断面可以移线
					  case when @ProductType in('A','B') then @ProductLine else '' end +'%'
				--Select @CorrectionQty=@Qty*(correction-1) from MRP_prodlineex with(nolock) where Item=@section Make such update base on final @Qty at final step.
				/*Select @WeekSectionRequestQty=sectionQty,@TempCorrectQty=correctionQty from #Mrp_mrpExPlanByLine 
					where section=@section and prodLine=@ProductLine
				If @WeekSectionRequestQty>0
					Begin
						If @WeekSectionRequestQty>@Qty
							Begin
								if @LastShift ='N'
									Begin
										Select @AdjustQty=0,@CorrectionQty=0
										Set @WeekSectionRequestQty=@WeekSectionRequestQty-@Qty
									End
								Else
									Begin
										----班产QTY=实际PlanQty(受总需求的影响)+Correction+Adjust
										select @CorrectionQty=@TempCorrectQty,@AdjustQty=@Qty-(@TempCorrectQty+@WeekSectionRequestQty)
										select @Qty=@WeekSectionRequestQty
										select @WeekSectionRequestQty=0,@TempCorrectQty=0
										
									End
							End 
						Else If @WeekSectionRequestQty<@Qty
							Begin
								if @WeekSectionRequestQty+@TempCorrectQty<@Qty
									Begin
										select @CorrectionQty=@TempCorrectQty,@AdjustQty=@Qty-(@TempCorrectQty+@WeekSectionRequestQty)
										select @Qty=@WeekSectionRequestQty,@TempCorrectQty=0,@WeekSectionRequestQty=0
									End
								Else
									Begin
										If @LastShift ='N'
										Begin
										----班产QTY=实际PlanQty(受总需求的影响)+Correction+Adjust
										select @CorrectionQty=@Qty-@WeekSectionRequestQty,@AdjustQty=0
										select @Qty=@WeekSectionRequestQty,@TempCorrectQty=@TempCorrectQty-@CorrectionQty,@WeekSectionRequestQty=0
										End
										Else
										Begin
										select @CorrectionQty=@TempCorrectQty,@AdjustQty=@Qty-(@TempCorrectQty+@WeekSectionRequestQty)
										select @Qty=@WeekSectionRequestQty,@TempCorrectQty=0,@WeekSectionRequestQty=0
										End
									End
							End
					End
				----这是@WeekSectionRequestQty=0的被分配完的情况，	
				Else
					Begin
						If @TempCorrectQty>0
						Begin
							If @TempCorrectQty>@Qty 
							Begin
								If @LastShift='N'
								Begin
									Select @CorrectionQty=@Qty,@AdjustQty =0,@TempCorrectQty=@TempCorrectQty-@Qty,@Qty=0
								End
								Else
								Begin
									Select @CorrectionQty=@TempCorrectQty,@AdjustQty=@Qty-@TempCorrectQty,@Qty=0
								End
							End
							Else
							Begin
								If @LastShift='N'
								Begin
									Select @CorrectionQty=@TempCorrectQty,@AdjustQty=@Qty-@TempCorrectQty,@TempCorrectQty=0,@Qty=0
								End
								Else
								Begin
									Select @CorrectionQty=@TempCorrectQty,@AdjustQty=@Qty-@TempCorrectQty,@TempCorrectQty=0,@Qty=0
								End
							End
						End
						Else
							Begin
								Select @AdjustQty=-@Qty
							End
					End
				Update #Mrp_mrpExPlanByLine Set sectionQty=@WeekSectionRequestQty,correctionQty=@TempCorrectQty where section=@section and prodLine=@ProductLine
				*/Select @Speed=MrpSpeed,@ApsPriority=ApsPriority,@SwitchTime=SwitchTime,@SpeedTimes=SpeedTimes,@Quota=Quota,@MinLotSize=MinLotSize,
					@EconomicLotSize=EconomicLotSize,@MaxLotSize=MaxLotSize,@TurnQty=TurnQty,@Correction=Correction,@ShiftType=ShiftType
					/*@productType=productType*/ from MRP_prodlineex with(nolock) where Item =@section and ProductLine like 
					--试制的断面可以移线
					case when @ProductType in('A','B') then @ProductLine else '' end +'%'
				If @ShiftType is null
					Begin
						select @ShiftType=ShiftType from MRP_prodlineex  with(nolock) where Item =@section
						select @ShiftType=ISNULL(@ShiftType,3)
					End
				--totalquota/totalaps =(sum MRP_ProdLineExInstance)
				Select @TotalQuota=sum(quota),@TotalAps=sum(ApsPriority) from MRP_ProdLineExInstance where item=@section and dateindex=@dateindex
				--Select @StartTime=@TempWeekStart,@TempWeekStart=dateadd(minute,@shiftQty*8*60,@TempWeekStart)
				Set @Logs =''
				----
				Select top 1 @uptime=uptime from MRP_MrpExSectionPlan with(nolock) --where dateindex=@dateindex and productline=@ProductLine
				Select top 1 @isEconomic=isEconomic from MRP_MrpExSectionPlan with(nolock) --where dateindex=@dateindex --and productline=@@ProductLine
				Select top 1 @planNo=planNo from MRP_MrpExSectionPlan with(nolock) --where  dateindex=@dateindex and productline=@ProductLine and section=@section
				Select @planNo=ISNULL (@planNo,'')
				----
				--Set @remark= case when @ProductType =10 then 'SY01' when @ProductType=20 then 'SY02' else 'SY03' end ;
				--Select @remark=@UnproductiveCode where @UnproductiveCode is not null
				select @remark=a.Desc1 from #UnproductiveCode a where a.Code = @ProductType
				----
				Set @LatestStartTime='9999-12-31 23:59:59.9999999'--这个字段需要继续查看
				if @section!='299999'
					Begin
						Select @SectionDescription=Desc1 from MD_Item with(nolock) where Code =@section
					End
				--Sp_help 'MRP_MrpExSectionPlan'
				Insert into _MRP_MrpExSectionPlan_( ProductLine, Section, DateIndex, PlanDate, PlanVersion, Sequence, Qty, CorrectionQty, AdjustQty,
					Speed, ApsPriority, SwitchTime, SpeedTimes, Quota, MinLotSize, EconomicLotSize, MaxLotSize, TurnQty, Correction, ShiftType, TotalAps,
					TotalQuota, StartTime, WindowTime, UpTime, PlanNo, ShiftQty, ProductType, LatestStartTime, Remark, Logs, IsEconomic, Shift, 
					CurrentSwitchTime, SectionDescription,IsOld,LastModifyDate)
				
				Select @ProductLine,@Section,@DateIndex,@PlanDate,@PlanVersion,@Sequence,@Qty,0 as CorrectionQty,0 as AdjustQty,@Speed,@ApsPriority,@SwitchTime,
					@SpeedTimes,@Quota,@MinLotSize,@EconomicLotSize,@MaxLotSize,@TurnQty,@Correction,@ShiftType,isnull(@TotalAps,0),isnull(@TotalQuota,0),@StartTime,@WindowTime,
					@UpTime,@PlanNo,@ShiftQty,@ProductType,@LatestStartTime,@Remark,@Logs,@IsEconomic,@Shift,@CurrentSwitchTime,@SectionDescription,@IsOld,@LastModifyDate
				Delete top (1) from #SplitResult_1 where [group]=@GroupDel	
			End
			----如果是在生产进行中的情况下重新运行的版本
			If @PlanVersion  >@WeekStart 
				Begin
					Declare @BundaryTime datetime
					If dbo.FormatDate(@PlanVersion,'HHNNSS') between '000000'and '08000000'
						Begin
							Set @BundaryTime=LEFT(@PlanVersion,11)+'00:00:00'
						End
					Else If dbo.FormatDate(@PlanVersion,'HHNNSS') between '08000000'and '16000000'
						Begin
							Set @BundaryTime=LEFT(@PlanVersion,11)+'08:00:00'
						End
					Else If dbo.FormatDate(@PlanVersion,'HHNNSS') between '16000000'and '24000000'
						Begin
							Set @BundaryTime=LEFT(@PlanVersion,11)+'16:00:00'
						End
					select @BundaryTime as '@BundaryTime'
					----00004
					--select * from _MRP_MrpExSectionPlan_ where StartTime<@BundaryTime
					--Delete from _MRP_MrpExSectionPlan_ where StartTime<@BundaryTime
					----00004
				End
				else
				Begin
					Set @BundaryTime=dateadd(MINUTE,-1,@WeekStart)
				End
				--select @BundaryTime as '@BundaryTime'
				--return
			----Calculate correctionQty&Sequence
			Update a 
				Set /*a.CorrectionQty =b.CQTY ,*/a.Sequence =b.NONO from _MRP_MrpExSectionPlan_ a,(Select */*,Qty*(Correction-1) As CQTY*/,ROW_NUMBER()over(PARTITION by productline,dateindex,planversion order by starttime)*10 as NONO
							   from _MRP_MrpExSectionPlan_ ) b where a.ProductLine=b.ProductLine and a.Section=b.Section and a.DateIndex=b.DateIndex and a.PlanVersion=b.PlanVersion and a.StartTime=b.StartTime 
			----00004
			--Delete from MRP_MrpExSectionPlan where PlanVersion =@PlanVersion and @DateIndex =@WeekIndex and StartTime>@BundaryTime
			----00004
			----Update IsOld&LastModifyDate
			Update _MRP_MrpExSectionPlan_ Set isold='0',lastmodifydate=GETDATE()
			----将一些必要参数设置一定的值:
			Update _MRP_MrpExSectionPlan_ set Speed=1,SpeedTimes=1,Quota =1,MinLotSize=1,EconomicLotSize=1,MaxLotSize=1,
				TurnQty =2,Correction =1.2,ShiftType =3,TotalAps=5,TotalQuota=100 where Section ='299999'
			----
			--select top 1000 * from Mrp_MrpExPlanItemRate where Item ='300645'
			----00003临时将没有维护断面物料比例的断面按物料1:1:1....的比例去产生比例数据
			Insert into Mrp_MrpExPlanItemRate(Section, Item, ItemRate, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate)
			select distinct Item as section,Bom as item ,1 as itemrate,''as createuser,'' as createusernm,GETDATE() as createdate,'','',GETDATE()  
				from PRD_BomDet a with(nolock),PRD_BomMstr b 
				where  a.Bom =b.Code 
				and  Item in (select Section from _MRP_MrpExSectionPlan_) 
				and Item not in (select Section from Mrp_MrpExPlanItemRate)
				----2014/01/24  Mrpoption  in(0,2) +有效期控制
				and b.IsActive =1
				and BomMrpOption in (0,2)
				and (StartDate <=GETDATE() or StartDate is null)
				and (EndDate >=GETDATE() or EndDate is null)
			--select * from Mrp_MrpExPlanItemRate
			----00003
			----00005
			select distinct a.Item,a.Bom,(a.RateQty/b.Qty)*(1/*+a.ScrapPct*/) as RateQty,c.UC as UnitCount,CAST(0 as float) as SectionItemRate ,CAST(0 as float) as SectionSumRate into #ItemRateQty from PRD_BomDet a with(nolock),
				PRD_BomMstr b with(nolock),MD_Item c with(nolock) where a.Item =b.Code and a.Bom =c.Code
				----00003
				--and b.Code in (select Section from Mrp_MrpExPlanItemRate)
				and a.Item in (select Section from _MRP_MrpExSectionPlan_) 
				----00003
				----2014/01/24  Mrpoption  in(0,2) +有效期控制
				and a.BomMrpOption in (0,2)
				and b.IsActive =1
				and (a.StartDate <=GETDATE() or a.StartDate is null)
				and (a.EndDate >=GETDATE() or a.EndDate is null)
			----每个断面对应的Section Qty 由itemrate和RateQty的乘积决定，（b.Qty*a.SectionItemRate/a.SectionSumRate-----算出每个物料对应的SectionQty）/a.RateQty------每个断面对应的SectionQTY/RateQty得到ItemQty
			select b.Section,SUM(b.itemrate*a.RateQty) SectionSumRate into #SectionSumRate from #ItemRateQty a ,Mrp_MrpExPlanItemRate b with(nolock) where a.Bom=b.item and a.Item =b.section
				group by Section
			select a.Bom, b.itemrate*a.RateQty SectionItemRate into #SectionItemRate from #ItemRateQty a ,Mrp_MrpExPlanItemRate b with(nolock) where a.Bom=b.item and a.Item =b.section

			update a Set a.SectionSumRate=b.SectionSumRate from #ItemRateQty a, #SectionSumRate b where a.Item=b.Section
			update a Set a.SectionItemRate=b.SectionItemRate from #ItemRateQty a, #SectionItemRate b where a.Bom=b.Bom
			--select * from #ItemRateQty
			select Section ,SUM(Qty)Qty into #SumSectionQty from _MRP_MrpExSectionPlan_ group by Section
			----00004
			delete #MRP_MrpExPlan_Temp where PlanVersion =@PlanVersion and DateIndex =@DateIndex 
			Insert into #MRP_MrpExPlan_Temp(DateIndex, PlanVersion, Section, SectionQty, Item, ItemQty, RateQty, InvQty, LatestStartTime, UnitCount, StartTime)
				select @DateIndex ,@PlanVersion  ,a.Item,b.Qty*a.SectionItemRate/a.SectionSumRate as sectionqty,a.Bom as item ,b.Qty*a.SectionItemRate/a.SectionSumRate/a.RateQty as Itemqty,
					a.RateQty as RateQty, 0 as InvQty,getdate(),a.UnitCount  as unitcount,getdate()
					from #ItemRateQty a,#SumSectionQty b where a.Item=b.Section  
			delete MRP_MrpExSectionPlan where PlanVersion =@PlanVersion and DateIndex =@DateIndex 
			----00004		
			Insert into MRP_MrpExSectionPlan(ProductLine, Section, DateIndex, PlanDate, PlanVersion, Sequence, Qty, CorrectionQty, AdjustQty,
					Speed, ApsPriority, SwitchTime, SpeedTimes, Quota, MinLotSize, EconomicLotSize, MaxLotSize, TurnQty, Correction, ShiftType, TotalAps,
					TotalQuota, StartTime, WindowTime, UpTime, PlanNo, ShiftQty, ProductType, LatestStartTime, Remark, Logs, IsEconomic,   
					CurrentSwitchTime, SectionDescription,IsOld,LastModifyDate)
				select ProductLine, Section, DateIndex, PlanDate, PlanVersion, Sequence, Qty, CorrectionQty, AdjustQty,
					Speed, ApsPriority, SwitchTime, SpeedTimes, Quota, MinLotSize, EconomicLotSize, MaxLotSize, TurnQty, Correction, ShiftType, TotalAps,
					TotalQuota, StartTime, WindowTime, UpTime, PlanNo, ShiftQty, ProductType, LatestStartTime, Remark, Logs, IsEconomic,   
					CurrentSwitchTime, SectionDescription,IsOld,LastModifyDate from _MRP_MrpExSectionPlan_
			----Update MRP_MrpExItemPlan
			Select DateIndex,PlanVersion,Section ,SUM(Qty) as TotalQty,SUM(correctionqty) as Totalcorrectionqty,SUM(AdjustQty) as TotalAdjustQty into #PlanRateQty  from MRP_MrpExSectionPlan 
				where DateIndex =@dateindex and PlanVersion = @planversion 
				and Section !='299999'
				and section is not null group by DateIndex,PlanVersion,Section
			Select * into #Mrp_mrpExPlan_1 from #MRP_MrpExPlan_Temp a where a.DateIndex =@dateindex and a.PlanVersion = @planversion
				and Section is not null
			Select b.*,a.TotalQty,a.Totalcorrectionqty,a.TotalAdjustQty into #Mrp_mrpExPlan_Insert from #PlanRateQty a,#Mrp_mrpExPlan_1 b where a.DateIndex =b.DateIndex and a.PlanVersion =b.PlanVersion and a.Section =b.Section
			--Truncate table _MRP_MrpExItemPlan  PlanQty-->同一个Section挤出件这一周所有所有的需求数(from MRP_MrpExItemPlan  where PlanVersion='2014-12-05 15:24:05.000' and DateIndex ='2014-50' and Item =300832)
			Delete from MRP_MrpExItemPlan where PlanVersion =@PlanVersion and DateIndex =@WeekIndex 
			Insert into MRP_MrpExItemPlan(Sequence, SectionId, Item, ProductLine, DateIndex, PlanVersion, Section, RateQty, PlanQty, Qty, CorrectionQty, AdjustQty, UnitCount, ItemDescription, UsedQty, PlanDate, IsOld, InvQty)
				Select b.Sequence,b.Id AS sectionId,a.item,b.ProductLine,b.DateIndex,b.PlanVersion,b.Section,a.rateqty,a.ItemQty As PlanQty,Qty*a.ItemQty/case when a.TotalQty=0 then 1 else TotalQty end,
					 b.correctionqty*a.ItemQty/case when a.TotalQty=0 then 1 else TotalQty end,b.AdjustQty*a.ItemQty/case when a.TotalQty=0 then 1 else TotalQty end
					 ,a.unitcount,''As Itemdesc,0 as usedQty,PlanDate,IsOld,0 as InvQty from #Mrp_mrpExPlan_Insert a ,MRP_MrpExSectionPlan b with(nolock)
					 where a.DateIndex =b.DateIndex and a.PlanVersion =b.PlanVersion and a.Section =b.Section 
					and a.DateIndex =@dateindex and a.PlanVersion = @planversion
			Update a set a.itemdescription=b.Desc1 from MRP_MrpExItemPlan a,MD_Item b with(nolock)  where a.item=b.Code 
				and a.DateIndex =@dateindex and a.PlanVersion = @planversion


			--00011根据SCM-FlowDet过滤
			 Select Distinct a.Flow,a.Item into #FlowDet from SCM_FlowDet a,SCM_FlowMstr b 
					where a.Flow =b.Code and b.ResourceGroup ='20' and 
					(a.StartDate<@CurDtae or a.StartDate is null) and (a.EndDate>@CurDtae or a.EndDate is null) 
			 Select a.*,CAST( 1 as decimal(18,8)) As ItemRate,
				CAST( 1 as decimal(18,8)) As SectionQtyRate,
				CAST( 1 as decimal(18,8)) As SectionQtySumRate,  /*挤出件占断面的总米数比率*/
				b.Qty As SectionQty into #_MRP_MrpExItemPlan 
				from MRP_MrpExItemPlan a ,MRP_MrpExSectionPlan b
				where a.PlanVersion =@PlanVersion
				and a.DateIndex=@DateIndex
				and	a.SectionId=b.Id
			 Delete a from #_MRP_MrpExItemPlan a
				where PlanVersion =@PlanVersion and DateIndex=@DateIndex
				and not exists(Select * from #FlowDet b where a.Item =b.Item and a.ProductLine=b.Flow)
			Update a
				Set a.ItemRate=b.ItemRate,SectionQtyRate=b.ItemRate*a.RateQty
					from #_MRP_MrpExItemPlan a,Mrp_MrpExPlanItemRate b where a.Item=b.Item
			Update a
				Set a.SectionQtySumRate=b.SectionQtySumRate from #_MRP_MrpExItemPlan a ,
				(Select SectionId,SUM(SectionQtyRate) As SectionQtySumRate from #_MRP_MrpExItemPlan Group by SectionId)b
				where a.SectionId=b.SectionId
			
			Update #_MRP_MrpExItemPlan Set Qty=SectionQtyRate/SectionQtySumRate*SectionQty/RateQty
			Update a
				Set a.PlanQty=b.PlanQty from #_MRP_MrpExItemPlan a,
				(Select Section,Item ,max(Qty) As PlanQty from #_MRP_MrpExItemPlan Group by Section,Item)b
				where a.Section=b.Section and a.Item =b.Item 
			 Delete a from MRP_MrpExItemPlan a
				where PlanVersion =@PlanVersion and DateIndex=@DateIndex
				and not exists(Select * from #FlowDet b where a.Item =b.Item and a.ProductLine=b.Flow)
			Update a
				Set a.PlanQty=b.PlanQty ,a.Qty=b.Qty from MRP_MrpExItemPlan a, #_MRP_MrpExItemPlan b where a.Id=b.Id
			--00011	End
			--select top 1000 * from MRP_MrpPlanMaster--public enum MessageType { Info = 0, Warning = 1, Error = 2 }
			--select top 1000 * from MRP_MrpExSectionPlan
			Declare @SnapTime Datetime
			select @SnapTime=MAX(SnapTime) from MRP_SnapMrpPlan with(nolock)
			if @SnapTime is null
			Begin
				set @SnapTime=dbo.FormatDate(GetDate(),'YYYY-MM-DD HH:NN:SS')
			end
			----00004
			--Insert into MRP_MrpPlanMaster
			--	Select @PlanVersion, @DateIndex ,@SnapTime,@PlanVersion,20,0,1 as IsRelease,2063,'超级用户',GETDATE(),GETDATE(),2063,'超级用户'
			----00004
			----00006
			Select a.Id As ItemPlanId,a.Section,a.Item,a.PlanDate,c.Seq As FlowDetSeq 
				Into #UpdateSeqOfMrpExItemPlan 
				From MRP_MrpExItemPlan a with(nolock)
				Left join SCM_FlowDet c with(nolock)
				On a.productline=c.flow and a.item=c.item
				Where a.DateIndex =@dateIndex and PlanVersion=@PlanVersion
			----没有匹配到FlowDet 里面 Seq 的 Item 排在最后面(Or 最前面???)
			Declare @NullSeqNo int 
			Select @NullSeqNo=MAX(FlowDetSeq)+1 from #UpdateSeqOfMrpExItemPlan
			Update #UpdateSeqOfMrpExItemPlan
				Set FlowDetSeq=@NullSeqNo from #UpdateSeqOfMrpExItemPlan where FlowDetSeq is null
			Select *,ROW_NUMBER()Over(partition by PlanDate,Section Order by FlowDetSeq,Item) as ItemPlanSeq 
				Into #ItemPlanSeq from #UpdateSeqOfMrpExItemPlan
			--Select * from #ItemPlanSeq a,MRP_MrpExItemPlan b with(nolock) where a.ItemPlanId=b.Id 
			Update b
				Set b.Sequence =a.ItemPlanSeq from #ItemPlanSeq a,MRP_MrpExItemPlan b with(nolock) where a.ItemPlanId=b.Id 
			----00006
			----00002
			Declare @TimeDiff int 
			select @TimeDiff=substring(shifttime,1,2)*60+substring(shifttime,4,2)*1 from PRD_ShiftDet where Shift like 'EX3-1%'
			Update MRP_MrpExSectionPlan set StartTime =DATEADD(MINUTE,@TimeDiff,StartTime),WindowTime =DATEADD(MINUTE,@TimeDiff,WindowTime),ProductType=UPPER(ProductType)
				where PlanVersion=@PlanVersion and DateIndex =@DateIndex
			----00002
			----000003
			Delete Mrp_MrpExPlanItemRate where CreateUserNm=''
			----00009
			Update MRP_MrpExSectionPlan
				Set ProductLine= UPPER(ProductLine ) from MRP_MrpExSectionPlan where PlanVersion =@PlanVersion
				 
			Update MRP_MrpExItemPlan
				Set ProductLine= UPPER(ProductLine ) from MRP_MrpExItemPlan where PlanVersion =@PlanVersion
				 

			--Select  * from _MRP_MrpExItemPlan  
			--Select  * from _MRP_MrpExSectionPlan_
			--Select top 3 * from MRP_prodlineex where item ='290029'
			--Select top 3 * from MRP_MrpExSectionPlan where section ='290029'order by dateindex desc
			--totalquota/totalaps =(sum MRP_ProdLineExInstance)
			--MRP_MrpExSectionPlan--uptime
			--remark ---productType(MRP_prodlineex) to string
			--planNo--MRP_MrpExSectionPlan--planNO
			--isEconomic---

	End
END
