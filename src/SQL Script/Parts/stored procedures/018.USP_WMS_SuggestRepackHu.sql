SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_SuggestRepackHu')
BEGIN
	DROP PROCEDURE USP_WMS_SuggestRepackHu
END
GO

CREATE PROCEDURE dbo.USP_WMS_SuggestRepackHu
	@RepackTaskId int
AS
BEGIN
	set nocount on
	declare @ErrorMsg nvarchar(MAX)

	create table #tempMsg_018 
	(
		Id int identity(1, 1) primary key,
		Lvl tinyint,
		Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempBuffInv_018
	(
		RowId int identity(1, 1) primary key,
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Qty decimal(18, 8),
		SuggestQty decimal(18, 8)
	)

	begin try
		if @RepackTaskId is null
		begin
			insert into #tempMsg_018(Lvl, Msg) values(2, N'翻包任务不能为空。')
		end
		else
		begin
			declare @Item varchar(50)
			declare @Uom varchar(5)
			declare @UC decimal(18, 8)
			declare @Qty decimal(18, 8)
			declare @LastQty decimal(18, 8)
			declare @IsActive bit

			select @Qty = Qty - RepackQty, @IsActive = IsActive from WMS_RepackTask where Id = @RepackTaskId

			if @Qty is null
			begin
				insert into #tempMsg_018(Lvl, Msg) values(2, N'翻包任务[' + convert(varchar, @RepackTaskId) + N']不存在。')
			end

			if @IsActive = 0
			begin
				insert into #tempMsg_018(Lvl, Msg) values(2, N'翻包任务[' + convert(varchar, @RepackTaskId) + N']已经关闭。')
			end
			else
			if @Qty <= 0
			begin
				insert into #tempMsg_018(Lvl, Msg) values(2, N'翻包任务[' + convert(varchar, @RepackTaskId) + N']已经关闭。')
			end

			if not exists(select top 1 1 from #tempMsg_018)
			begin
				insert into #tempBuffInv_018(HuId, Qty)
				select HuId, Qty from WMS_BuffInv 
				where Item = @Item and Uom = @Uom and Qty > 0 and IsLock = 0 and HuId is not null and UC <> Qty
				order by UC, LotNo

				insert into #tempBuffInv_018(HuId, Qty)
				select HuId, Qty from WMS_BuffInv 
				where Item = @Item and Uom = @Uom and Qty > 0 and IsLock = 0 and HuId is not null and UC = Qty
				order by UC, LotNo

				update #tempBuffInv_018 set SuggestQty = @LastQty, @Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= Qty THEN Qty ELSE @Qty END
			end
		end
	end try
	begin catch
		set @ErrorMsg = N'推荐翻箱条码发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	select Lvl, Msg from #tempMsg_018
	select HuId from #tempBuffInv_018 where SuggestQty > 0

	drop table #tempMsg_018
	drop table #tempBuffInv_018
END
GO