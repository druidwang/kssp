/****** Object:  StoredProcedure [dbo].[USP_Busi_UpdateSeq4RestoreVanOrder]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_UpdateSeq4RestoreVanOrder')
	DROP PROCEDURE USP_Busi_UpdateSeq4RestoreVanOrder
Create PROCEDURE [dbo].[USP_Busi_UpdateSeq4RestoreVanOrder]
(
	@TraceCode varchar(50),
	@VanOrder varchar(50),
	@NewSeq int,
	@UserId int,
	@UserNm varchar(100)
)
AS
BEGIN
	--查询包含TraceCode的采购单（Kit单/排序单）
	select Flow, OrderNo, CONVERT(varchar(8), StartTime, 112) as StartTime into #temp1 from ORD_OrderMstr_1 where TraceCode = @TraceCode;
	--查询包含TraceCode的移库单（Kit单/排序单）
	select Flow, OrderNo, CONVERT(varchar(8), StartTime, 112) as StartTime into #temp2 from ORD_OrderMstr_2 where TraceCode = @TraceCode;
	--查询包含TraceCode的生产单（驾驶室/底盘）
	select Flow, OrderNo, CONVERT(varchar(8), StartTime, 112) as StartTime into #temp4 from ORD_OrderMstr_4 where TraceCode = @TraceCode and OrderNo <> @VanOrder;
	--查询包含TraceCode的计划协议单（Kit单/排序单）
	select Flow, OrderNo, CONVERT(varchar(8), StartTime, 112) as StartTime into #temp8 from ORD_OrderMstr_8 where TraceCode = @TraceCode;
	
begin tran
	update ORD_OrderMstr_1 set Seq = mstr.Seq + 1, Version = Version + 1, LastModifyDate = GETDATE(), LastModifyUser = @UserId, LastModifyUserNm = @UserNm
	from ORD_OrderMstr_1 as mstr inner join #temp1 as t on mstr.Flow = t.Flow and CONVERT(varchar(8), mstr.StartTime, 112) = t.StartTime
	where mstr.OrderNo <> t.OrderNo and mstr.Seq >= @NewSeq;
	
	update ORD_OrderMstr_2 set Seq = mstr.Seq + 1, Version = Version + 1, LastModifyDate = GETDATE(), LastModifyUser = @UserId, LastModifyUserNm = @UserNm
	from ORD_OrderMstr_2 as mstr inner join #temp2 as t on mstr.Flow = t.Flow and CONVERT(varchar(8), mstr.StartTime, 112) = t.StartTime
	where mstr.OrderNo <> t.OrderNo and mstr.Seq >= @NewSeq;
	
	update ORD_OrderMstr_4 set Seq = mstr.Seq + 1, Version = Version + 1, LastModifyDate = GETDATE(), LastModifyUser = @UserId, LastModifyUserNm = @UserNm
	from ORD_OrderMstr_4 as mstr inner join #temp4 as t on mstr.Flow = t.Flow and CONVERT(varchar(8), mstr.StartTime, 112) = t.StartTime
	where mstr.OrderNo <> t.OrderNo and mstr.Seq >= @NewSeq;
	
	update ORD_OrderMstr_8 set Seq = mstr.Seq + 1, Version = Version + 1, LastModifyDate = GETDATE(), LastModifyUser = @UserId, LastModifyUserNm = @UserNm
	from ORD_OrderMstr_8 as mstr inner join #temp8 as t on mstr.Flow = t.Flow and CONVERT(varchar(8), mstr.StartTime, 112) = t.StartTime
	where mstr.OrderNo <> t.OrderNo and mstr.Seq >= @NewSeq;
commit

END
GO