USE [Sconit_20150106]
GO
/****** Object:  StoredProcedure [dbo].[USP_SAP_PP_ExportEXOrder]    Script Date: 01/07/2015 15:15:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
ALTER PROCEDURE [dbo].[USP_SAP_PP_ExportEXOrder]
(
	@BatchNo varchar(50),
	@StartTime datetime,
	@EndTime datetime
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @CurrDate datetime=GETDATE()
	Declare @ExecutionTime datetime = @CurrDate--varchar(20)=dbo.FormatDate(@CurrDate,'YYYYMMDDHHNNSS')
	Declare @WERKS varchar(50)
	Declare @ZMESGUID varchar(50)=dbo.FormatDate(GETDATE(),'YYYYMMDDHHNNSS0000')
	Select @WERKS=Value from SYS_EntityPreference where Id = 90107
	--select @ExecutionTime,@ExecutionTime,@StartTime,@EndTime 
	--?????一个问题是等所有PPMES_0001全部都收集好之后再更新UniqueCode还是每一个部分（炼胶，挤出，后加工）收集好就更新
	----正常收货
	Insert into SAP_Interface_PPMES0001(ZMESSC, ZMESLN, ZPTYPE, AUFART, WERKS, MATNR_H, GAMNG_H, GMEIN_H, GLTRP, GSTRP, BLDAT, BUDAT, LFSNR, BWART_H, 
		LGORT_H, ERFMG_H, ZComnum, LMNGA_H, ISM, BWART_I, MATNR_I, ERFMG_I, GMEIN_I, LGORT_I, ZMESGUID, ZCSRQSJ, BatchNo, UniqueCode, Status,OrderType,TailQty)
		Select Distinct OrdMstr.OrderNo As ZMESSC,OrdDet.Seq As ZMESLN,'BUSCO01' As ZPTYPE,'SY01' As AUFART,@WERKS As WERKS,OrdDet.Item As MATNR_H,
			Left(cast(OrdDet.OrderQty As varchar),13) As GAMNG_H,OrdDet.BaseUom As GMEIN_H,OrdMstr.WindowTime As GLTRP,
			OrdMstr.StartTime As GSTRP,OrdMstr.EffDate As BLDAT,
			RecMstr.CreateDate As BUDAT,RecMstr.RecNo As LFSNR,Case when RecDet.RecQty*RecDet.UnitQty >0 then '101' else '102' End As BWART_H ,
			OrdDet.LocTo As LGORT_H,Left(cast(abs(RecDet.RecQty*RecDet.UnitQty) As varchar),13) As ERFMG_H,RecMstr.RecNo+right('00'+CAST(RecDet.Seq As varchar(10)),2) As ZComnum,
			Left(cast(RecDet.RecQty*RecDet.UnitQty As varchar),13) As LMNGA_H,'' As ISM,Case when sum(BFDet.BFQty*BFDet.UnitQty) <0 then  '261' else '262' End As BWART_I,BFDet.Item As MATNR_I,
			Left(cast(abs(round(sum(BFDet.BFQty*BFDet.UnitQty),3)) as varchar),13) As ERFMG_I,BFDet.BaseUom As GMEIN_I,BFDet.LocFrom As LGORT_I,
			@ZMESGUID As ZMESGUID,@ExecutionTime As ZCSRQSJ ,@BatchNo As BatchNo , NULL As UniqueCode,'0' As Status,'EXOrder',
			Left(cast(sum(BFDet.BFQty*BFDet.UnitQty)-round(sum(BFDet.BFQty*BFDet.UnitQty),3) As varchar),50) As TailQty
			from ORD_RecDet_4 RecDet,ORD_RecMstr_4 RecMstr,ORD_OrderDet_4 OrdDet,ORD_OrderMstr_4 OrdMstr,ORD_OrderBackflushDet BFDet
			Where RecDet.OrderDetId = OrdDet.Id 
			and RecDet.RecNo = RecMstr.RecNo 
			and OrdDet.OrderNo = OrdMstr.OrderNo 
			and RecDet.Id = BFDet.RecDetId
			and OrdMstr.ResourceGroup = 20
			and OrdMstr.SubType= 0
			and BFDet.IsVoid=0
			and RecMstr.CreateDate>@StartTime AND RecMstr.CreateDate<=@EndTime
			and RecMstr.CreateUser<>3910
			Group BY OrdMstr.OrderNo ,OrdDet.Seq , OrdDet.Item ,
			Left(cast(OrdDet.OrderQty As varchar),13) ,OrdDet.BaseUom ,OrdMstr.WindowTime ,
			OrdMstr.StartTime ,OrdMstr.EffDate ,
			RecMstr.CreateDate ,RecMstr.RecNo ,Case when RecDet.RecQty*RecDet.UnitQty >0 then '101' else '102' End ,
			OrdDet.LocTo ,Left(cast(abs(RecDet.RecQty*RecDet.UnitQty) As varchar),13) ,
			RecMstr.RecNo+right('00'+CAST(RecDet.Seq As varchar(10)),2) ,
			Left(cast(RecDet.RecQty*RecDet.UnitQty As varchar),13) ,BFDet.Item ,
			BFDet.BaseUom ,BFDet.LocFrom 
			--and Not RecDet.LastModifyDate between @StartTime and @EndTime
--塞芯
		--塞芯投料
		Insert into SAP_Interface_PPMES0001(ZMESSC, ZMESLN, ZPTYPE, AUFART, WERKS, MATNR_H, GAMNG_H, GMEIN_H, GLTRP, GSTRP, BLDAT, BUDAT, LFSNR, BWART_H, 
		LGORT_H, ERFMG_H, ZComnum, LMNGA_H, ISM, BWART_I, MATNR_I, ERFMG_I, GMEIN_I, LGORT_I, ZMESGUID, ZCSRQSJ, BatchNo, UniqueCode, Status,OrderType,TailQty)
		Select Distinct OrdMstr.OrderNo As ZMESSC,OrdDet.Seq As ZMESLN,'BUSCO01' As ZPTYPE,'SY01' As AUFART,@WERKS As WERKS,OrdDet.Item As MATNR_H,
		Left(cast(OrdDet.OrderQty As varchar),13) As GAMNG_H,OrdDet.BaseUom As GMEIN_H,OrdMstr.WindowTime As GLTRP,
		OrdMstr.StartTime As GSTRP,OrdMstr.EffDate As BLDAT,
		BFDet.CreateDate As BUDAT,BFDet.Id As LFSNR,'101' As BWART_H ,
		OrdDet.LocTo As LGORT_H,'0' As ERFMG_H,BFDet.Id As ZComnum,
		'0' As LMNGA_H,'' As ISM,Case when sum(BFDet.BFQty*BFDet.UnitQty) <0 then  '261' else '262' End As BWART_I,BFDet.Item As MATNR_I,
		Left(cast(abs(round(sum(BFDet.BFQty*BFDet.UnitQty),3)) as varchar),13) As ERFMG_I,BFDet.BaseUom As GMEIN_I,BFDet.LocFrom As LGORT_I,
		@ZMESGUID As ZMESGUID,@ExecutionTime As ZCSRQSJ ,@BatchNo As BatchNo , NULL As UniqueCode,'0' As Status,'EXOrder',
		Left(cast(sum(BFDet.BFQty*BFDet.UnitQty)-round(sum(BFDet.BFQty*BFDet.UnitQty),3) As varchar),50) As TailQty
		from ORD_OrderDet_4 OrdDet,ORD_OrderMstr_4 OrdMstr,ORD_OrderBackflushDet BFDet
		Where OrdDet.OrderNo = OrdMstr.OrderNo 
		and OrdMstr.OrderNo = BFDet.OrderNo
		and OrdDet.Seq = BFDet.OrderDetSeq
		and OrdMstr.ResourceGroup = 20
		and OrdMstr.SubType= 0
		--and BFDet.IsVoid=0
		and BFDet.CreateDate>@StartTime AND BFDet.CreateDate<=@EndTime
		and BFDet.CreateUser<>3910
		and OrdMstr.ProdLineFact='EXV'
		Group by OrdMstr.OrderNo,OrdDet.Seq,OrdDet.Item,
		Left(cast(OrdDet.OrderQty As varchar),13),
		OrdDet.BaseUom,OrdMstr.WindowTime,OrdMstr.StartTime,
		OrdMstr.EffDate,BFDet.CreateDate,BFDet.Id,OrdDet.LocTo,
		BFDet.Item,BFDet.BaseUom,BFDet.LocFrom
		--塞芯收货(没有OrderBomDet)
		Insert into SAP_Interface_PPMES0001(ZMESSC, ZMESLN, ZPTYPE, AUFART, WERKS, MATNR_H, GAMNG_H, GMEIN_H, GLTRP, GSTRP, BLDAT, BUDAT, LFSNR, BWART_H, 
		LGORT_H, ERFMG_H, ZComnum, LMNGA_H, ISM, BWART_I, MATNR_I, ERFMG_I, GMEIN_I, LGORT_I, ZMESGUID, ZCSRQSJ, BatchNo, UniqueCode, Status,OrderType,TailQty)
		Select Distinct OrdMstr.OrderNo As ZMESSC,OrdDet.Seq As ZMESLN,'BUSCO01' As ZPTYPE,'SY01' As AUFART,@WERKS As WERKS,OrdDet.Item As MATNR_H,
		Left(cast(OrdDet.OrderQty As varchar),13) As GAMNG_H,OrdDet.BaseUom As GMEIN_H,OrdMstr.WindowTime As GLTRP,
		OrdMstr.StartTime As GSTRP,OrdMstr.EffDate As BLDAT,
		RecMstr.CreateDate As BUDAT,RecMstr.RecNo As LFSNR,Case when RecDet.RecQty*RecDet.UnitQty >0 then '101' else '102' End As BWART_H ,
		OrdDet.LocTo As LGORT_H,Left(cast(abs(RecDet.RecQty*RecDet.UnitQty) As varchar),13) As ERFMG_H,RecMstr.RecNo+right('00'+CAST(RecDet.Seq As varchar(10)),2) As ZComnum,
		Left(cast(RecDet.RecQty*RecDet.UnitQty As varchar),13) As LMNGA_H,'' As ISM,Case when sum(BFDet.BFQty*BFDet.UnitQty) <0 then  '261' else '262' End As BWART_I,'' As MATNR_I,
		'0' As ERFMG_I,'' As GMEIN_I,'' As LGORT_I,
		@ZMESGUID As ZMESGUID,@ExecutionTime As ZCSRQSJ ,@BatchNo As BatchNo , NULL As UniqueCode,'0' As Status,'EXOrder',
		'0' As TailQty
		from ORD_RecDet_4 RecDet inner join ORD_RecMstr_4 RecMstr on RecDet.RecNo = RecMstr.RecNo 
		inner join ORD_OrderDet_4 OrdDet on RecDet.OrderDetId = OrdDet.Id 
		inner join ORD_OrderMstr_4 OrdMstr on OrdDet.OrderNo = OrdMstr.OrderNo 
		left join ORD_OrderBackflushDet BFDet on RecDet.Id = BFDet.RecDetId
		Where OrdMstr.ResourceGroup = 20
		and OrdMstr.SubType= 0
		and RecMstr.CreateDate>@StartTime AND RecMstr.CreateDate<=@EndTime
		and BFDet.Id  is null
		and RecMstr.CreateUser<>3910
		Group BY OrdMstr.OrderNo ,OrdDet.Seq , OrdDet.Item ,
		Left(cast(OrdDet.OrderQty As varchar),13) ,OrdDet.BaseUom ,OrdMstr.WindowTime ,
		OrdMstr.StartTime ,OrdMstr.EffDate ,
		RecMstr.CreateDate ,RecMstr.RecNo ,Case when RecDet.RecQty*RecDet.UnitQty >0 then '101' else '102' End ,
		OrdDet.LocTo ,Left(cast(abs(RecDet.RecQty*RecDet.UnitQty) As varchar),13) ,
		RecMstr.RecNo+right('00'+CAST(RecDet.Seq As varchar(10)),2) ,
		Left(cast(RecDet.RecQty*RecDet.UnitQty As varchar),13) ,BFDet.Item ,
		BFDet.BaseUom ,BFDet.LocFrom
	----冲销
	Insert into SAP_Interface_PPMES0002(ZMESSC, ZMESLN, ZPTYPE, ZComnum, ZMESGUID, ZCSRQSJ, BatchNo, UniqueCode, Status,OrderType,CancelDate)
		Select Distinct OrdMstr.OrderNo As ZMESSC,OrdDet.Seq As ZMESLN,'BUSCX01' As ZPTYPE,
			RecMstr.RecNo+right('00'+CAST(RecDet.Seq As varchar(10)),2) As ZComnum,
			@ZMESGUID As ZMESGUID,@ExecutionTime As ZCSRQSJ,@BatchNo As BatchNo , 
			NULL As UniqueCode,'0' As Status,'EXOrder' As OrderType,RecMstr.LastModifyDate
			from ORD_RecDet_4 RecDet,ORD_RecMstr_4 RecMstr,ORD_OrderDet_4 OrdDet,ORD_OrderMstr_4 OrdMstr
			Where RecDet.OrderDetId = OrdDet.Id 
			and RecDet.RecNo = RecMstr.RecNo 
			and OrdDet.OrderNo = OrdMstr.OrderNo 
			and OrdMstr.ResourceGroup = 20
			and OrdMstr.SubType= 0
			and RecMstr.Status=1
			and RecMstr.LastModifyUser<>3910
			and RecMstr.LastModifyDate>@StartTime AND RecMstr.LastModifyDate<=@EndTime
	--更新批次号和订单数
	--EXEC dbo.USP_Interface_EX_ReceivedOrder
	EXEC USP_SAP_PP_GenEXSAPOrder @BatchNo,@StartTime ,@EndTime
	UPdate a
		Set a.GAMNG_H=Left(cast(b.OrderQty as varchar),13),a.ZMESSC=b.PlanNo/*,a.ISM =b.ISM*/,
			a.GSTRP=isnull(b.StartTime,a.GSTRP) ,a.GLTRP=isnull(b.WindowTime,a.GLTRP) from SAP_Interface_PPMES0001 a,SAP_EX001 b 
			where a.LFSNR=b.RecNo 
			and a.BatchNo=b.BatchNo
			and a.BatchNo =@BatchNo
			and a.OrderType ='EXOrder'
			
	UPdate a
		Set a.ZMESSC=b.ZMESSC from SAP_Interface_PPMES0002 a,SAP_Interface_PPMES0001 b 
			where a.ZComnum =b.ZComnum
			--and a.BatchNo=b.BatchNo
			and a.BatchNo=@BatchNo
			and a.OrderType ='EXOrder'
	--更新SAP 库位
	Update a
		Set a.LGORT_H=b.SAPLocation from SAP_Interface_PPMES0001 a, MD_Location b where a.LGORT_H=b.Code and a.ZCSRQSJ=@ExecutionTime
	Update a
		Set a.LGORT_I=b.SAPLocation from SAP_Interface_PPMES0001 a, MD_Location b where a.LGORT_I=b.Code and a.ZCSRQSJ=@ExecutionTime


		--Delete ,Update在测试阶段启用下面的代码，测试完后要把所有存储整合在一起，一起更新
		
		Update SAP_Interface_PPMES0001 Set MTSNR =LFSNR where BatchNo =@BatchNo 
		--扣料为0的不传
		Update SAP_Interface_PPMES0001
		Set Status ='1' from SAP_Interface_PPMES0001 
		where BatchNo =@BatchNo and  CAST(ERFMG_I As decimal(18,8))=0 
		and OrderType='EXOrder'and MATNR_I!=''
		--扣料为0的不传
		SELECT s1.ZComnum INTO #TEMP1 FROM SAP_Interface_PPMES0001 s1 
			INNER JOIN SAP_Interface_PPMES0002 s2 ON s1.ZComnum=s2.ZComnum
			WHERE s1.BatchNo=@BatchNo AND s2.BatchNo=@BatchNo
			And s1.ZPTYPE ='BUSCO01'
		
		IF @@ROWCOUNT>0
		BEGIN
			--如果同一个时间段内存在冲销都不传
			DELETE a FROM SAP_Interface_PPMES0001 a INNER JOIN #TEMP1 b ON a.ZComnum=b.ZComnum
			DELETE a FROM SAP_Interface_PPMES0002 a INNER JOIN #TEMP1 b ON a.ZComnum=b.ZComnum
		END
		DECLARE @UniqueCode varchar(50)
		WHILE EXISTS(SELECT * FROM SAP_Interface_PPMES0001 WHERE BatchNo=@BatchNo AND UniqueCode IS NULL)
		BEGIN
			DECLARE @LastOrderNo varchar(50)
			SET @UniqueCode=REPLACE(NEWID(),'-','')
			UPDATE TOP(5000) SAP_Interface_PPMES0001 Set UniqueCode=@UniqueCode,ZMESGUID =@ZMESGUID WHERE BatchNo=@BatchNo AND UniqueCode IS NULL
			SELECT TOP 1 @LastOrderNo=ZMESSC FROM SAP_Interface_PPMES0001 WHERE BatchNo=@BatchNo AND UniqueCode IS NULL
			PRINT @LastOrderNo
			IF EXISTS(SELECT 1 FROM SAP_Interface_PPMES0001 WHERE BatchNo=@BatchNo AND UniqueCode=@UniqueCode AND ZMESSC=@LastOrderNo )
			BEGIN
				UPDATE SAP_Interface_PPMES0001 Set UniqueCode=@UniqueCode,ZMESGUID =@ZMESGUID WHERE BatchNo=@BatchNo AND ZMESSC=@LastOrderNo
			END
		END
		WHILE EXISTS(SELECT * FROM SAP_Interface_PPMES0002 WHERE BatchNo=@BatchNo AND UniqueCode IS NULL)
		BEGIN
			SET @UniqueCode=REPLACE(NEWID(),'-','')
			UPDATE TOP(5000) SAP_Interface_PPMES0002 Set UniqueCode=@UniqueCode,ZMESGUID =@ZMESGUID WHERE BatchNo=@BatchNo AND UniqueCode IS NULL
			SELECT TOP 1 @LastOrderNo=ZMESSC FROM SAP_Interface_PPMES0002 WHERE BatchNo=@BatchNo AND UniqueCode IS NULL
			PRINT @LastOrderNo
			IF EXISTS(SELECT 1 FROM SAP_Interface_PPMES0002 WHERE BatchNo=@BatchNo AND UniqueCode=@UniqueCode AND ZMESSC=@LastOrderNo )
			BEGIN
				UPDATE SAP_Interface_PPMES0002 Set UniqueCode=@UniqueCode,ZMESGUID =@ZMESGUID WHERE BatchNo=@BatchNo AND ZMESSC=@LastOrderNo
			END
		END
		SELECT Status=1,BatchNo=@BatchNo,'生成挤出收货数据成功。'
END