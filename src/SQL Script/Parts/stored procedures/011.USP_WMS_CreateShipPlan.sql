SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_CreateShipPlan')
BEGIN
	DROP PROCEDURE USP_WMS_CreateShipPlan
END
GO

CREATE PROCEDURE dbo.USP_WMS_CreateShipPlan
	@OrderNo varchar(50),
	@CreateUserId int,
	@CreateUserNm varchar(100)
AS
BEGIN
	set nocount on
	declare @DateTimeNow datetime
	declare @ErrorMsg nvarchar(MAX)
	
	set @DateTimeNow = GetDate()

	begin try
		begin try
			declare @OrderType tinyint
			declare @Status tinyint
			declare @IsAutoCreatePickList bit
			declare @BeginShipPlanId int
			declare @EndShipPlanId int
			declare @InsertShipPlanCount int

			if (ISNULL(@OrderNo, '') = '')
			begin
				set @ErrorMsg = N'要货单号不能为空。'
				RAISERROR(@ErrorMsg, 16, 1) 
			end

			select @OrderType = [Type], @Status = [Status], @IsAutoCreatePickList = IsCreatePL
			from ORD_OrderMstr_2 where OrderNo = @OrderNo

			if (@OrderType = null)
			begin
				select @OrderType = [Type], @Status = [Status], @IsAutoCreatePickList = IsCreatePL 
				from ORD_OrderMstr_3 where OrderNo = @OrderNo
			end

			if (@OrderType = null)
			begin
				set @ErrorMsg = N'要货单号' + @OrderNo + N'不存在。'
				RAISERROR(@ErrorMsg, 16, 1) 
			end

			if (@OrderType <> 2 and @OrderType <> 3)
			begin
				set @ErrorMsg = N'要货单' + @OrderNo + N'的类型不正确，不能创建发货任务。'
				RAISERROR(@ErrorMsg, 16, 1) 
			end

			if (@Status <> 1)
			begin
				set @ErrorMsg = N'要货单' + @OrderNo + N'不是释放状态，不能创建发货任务。'
				RAISERROR(@ErrorMsg, 16, 1) 
			end
		end try
		begin catch
			set @ErrorMsg = N'数据准备出现异常：' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch

		begin try
			declare @Trancount int
			set @Trancount = @@trancount

			if @Trancount = 0
			begin
				begin tran
			end

			if @OrderType = 2
			begin
				insert into WMS_ShipPlan(Flow, OrderNo, OrderSeq, OrderDetId, OrderType, StartTime, WindowTime, 
							Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, 
							OrderQty, ShipQty, [Priority], PartyFrom, PartyFromNm, PartyTo, PartyToNm, 
							ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
							ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, 
							LocFrom, LocFromNm, LocTo, LocToNm, Station, Dock, IsOccupyInv, IsActive, 
							CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
							[Version], IsShipScanHu, PickQty, PickedQty, LockQty, PackQty)
				select mstr.Flow, det.OrderNo, det.Seq, det.Id, mstr.[Type], @DateTimeNow, CASE WHEN StartTime >  @DateTimeNow THEN StartTime ELSE @DateTimeNow end as WindowTime,
				det.Item, det.ItemDesc, det.RefItemCode, det.Uom, det.BaseUom, det.UnitQty, det.UC, det.UCDesc,
				det.OrderQty, 0, mstr.[Priority], mstr.PartyFrom, mstr.PartyFromNm, mstr.PartyTo, mstr.PartyToNm, 
				mstr.ShipFrom, mstr.ShipFromAddr, mstr.ShipFromTel, mstr.ShipFromCell, mstr.ShipFromFax, mstr.ShipFromContact, 
				mstr.ShipTo, mstr.ShipToAddr, mstr.ShipToTel, mstr.ShipToCell, mstr.ShipToFax, mstr.ShipToContact,
				ISNULL(det.LocFrom, mstr.LocFrom), ISNULL(dl.Name, ml.Name), ISNULL(det.LocTo, mstr.LocTo), CASE WHEN det.LocTo is null THEN mstr.LocToNm ELSE det.LocToNm END, det.BinTo, mstr.Dock, 0, 1,
				@CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow,
				1, mstr.IsShipScanHu, 0, 0, 0, 0
				from ORD_OrderMstr_2 as mstr 
				inner join ORD_OrderDet_2 as det on mstr.OrderNo = det.OrderNo
				left join MD_Location as ml on mstr.LocFrom = ml.Code
				left join MD_Location as dl on det.LocFrom = dl.Code
				where mstr.OrderNo = @OrderNo 
				and ((det.LocFrom is null and ml.EnableAdvWM = 1) or (det.LocFrom is not null and dl.EnableAdvWM = 1))
			end
			else
			begin
				insert into WMS_ShipPlan(Flow, OrderNo, OrderSeq, OrderDetId, OrderType, StartTime, WindowTime, 
							Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, 
							OrderQty, ShipQty, [Priority], PartyFrom, PartyFromNm, PartyTo, PartyToNm, 
							ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
							ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, 
							LocFrom, LocFromNm, LocTo, LocToNm, Station, Dock, IsOccupyInv, IsActive, 
							CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
							[Version], IsShipScanHu, PickQty, PickedQty, LockQty, PackQty)
				select mstr.Flow, det.OrderNo, det.Seq, det.Id, mstr.[Type], @DateTimeNow, CASE WHEN StartTime >  @DateTimeNow THEN StartTime ELSE @DateTimeNow end as WindowTime,
				det.Item, det.ItemDesc, det.RefItemCode, det.Uom, det.BaseUom, det.UnitQty, det.UC, det.UCDesc,
				det.OrderQty, 0, mstr.[Priority], mstr.PartyFrom, mstr.PartyFromNm, mstr.PartyTo, mstr.PartyToNm, 
				mstr.ShipFrom, mstr.ShipFromAddr, mstr.ShipFromTel, mstr.ShipFromCell, mstr.ShipFromFax, mstr.ShipFromContact, 
				mstr.ShipTo, mstr.ShipToAddr, mstr.ShipToTel, mstr.ShipToCell, mstr.ShipToFax, mstr.ShipToContact,
				ISNULL(det.LocFrom, mstr.LocFrom), ISNULL(dl.Name, ml.Name), ISNULL(det.LocTo, mstr.LocTo), CASE WHEN det.LocTo is null THEN mstr.LocToNm ELSE det.LocToNm END, det.BinTo, mstr.Dock, 0, 1,
				@CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow,
				1, mstr.IsShipScanHu, 0, 0, 0, 0
				from ORD_OrderMstr_3 as mstr 
				inner join ORD_OrderDet_3 as det on mstr.OrderNo = det.OrderNo
				left join MD_Location as ml on mstr.LocFrom = ml.Code
				left join MD_Location as dl on det.LocFrom = dl.Code
				where mstr.OrderNo = @OrderNo 
				and ((det.LocFrom is null and ml.EnableAdvWM = 1) or (det.LocFrom is not null and dl.EnableAdvWM = 1))
			end

			select @EndShipPlanId = @@IDENTITY, @InsertShipPlanCount = @@ROWCOUNT
			set @BeginShipPlanId = @EndShipPlanId - @InsertShipPlanCount + 1

			if (@IsAutoCreatePickList = 1)
			begin
				declare @CreatePickTaskTable CreatePickTaskTableType
				insert into @CreatePickTaskTable(Id, PickQty)
				select Id, ShipQty from WMS_ShipPlan where Id between @BeginShipPlanId and @EndShipPlanId
				exec USP_WMS_CreatePickTask
			end
			
			if @Trancount = 0 
			begin  
				commit
			end
		end try
		begin catch
			if @Trancount = 0
			begin
				rollback
			end 

			set @ErrorMsg = N'数据更新出现异常：' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch
	end try
	begin catch
		set @ErrorMsg = N'创建发运计划发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch
END
GO