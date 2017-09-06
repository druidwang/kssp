/****** Object:  StoredProcedure [dbo].[USP_Split_RestartProductLine]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_RestartProductLine')
	DROP PROCEDURE USP_Split_RestartProductLine
Create PROCEDURE [dbo].[USP_Split_RestartProductLine]
(
	@ProductLineCode varchar(50),
	@UserId int,
	@UserNm varchar(100)
)
AS
BEGIN
	declare @DateTimeNow as datetime;
	set @DateTimeNow = GETDATE();
	
	--查询该生产线上所有未关闭生产单的Van号
	select TraceCode into #temp1 from ORD_OrderMstr_4 where Flow = @ProductLineCode and Status in (0, 1, 2);
	
	begin tran	
		--生产线恢复
		update SCM_FlowMstr set IsPause = 0, PauseTime = null, LastModifyDate = @DateTimeNow, LastModifyUser = @UserId, LastModifyUserNm = @UserNm where Code = @ProductLineCode;
		
		--根据Van号恢复采购单（Kit单/排序单）
		update ORD_OrderMstr_1 set IsPLPause = 0, PauseTime = null, Version = Version + 1, LastModifyDate = GETDATE(), LastModifyUser = @UserId, LastModifyUserNm = @UserNm
		from ORD_OrderMstr_1 as m1 inner join #temp1 as t on m1.TraceCode = t.TraceCode
		
		--根据Van号恢复移库单（Kit单/排序单）
		update ORD_OrderMstr_2 set IsPLPause = 0, PauseTime = null, Version = Version + 1, LastModifyDate = GETDATE(), LastModifyUser = @UserId, LastModifyUserNm = @UserNm
		from ORD_OrderMstr_2 as m1 inner join #temp1 as t on m1.TraceCode = t.TraceCode
		
		--根据Van号恢复生产单（驾驶室/底盘）
		update ORD_OrderMstr_4 set IsPLPause = 0, PauseTime = null, Version = Version + 1, LastModifyDate = GETDATE(), LastModifyUser = @UserId, LastModifyUserNm = @UserNm
		from ORD_OrderMstr_4 as m1 inner join #temp1 as t on m1.TraceCode = t.TraceCode
		
		--根据Van号恢复计划协议单（Kit单/排序单）
		update ORD_OrderMstr_8 set IsPLPause = 0, PauseTime = null, Version = Version + 1, LastModifyDate = GETDATE(), LastModifyUser = @UserId, LastModifyUserNm = @UserNm
		from ORD_OrderMstr_8 as m1 inner join #temp1 as t on m1.TraceCode = t.TraceCode
	commit

END
GO
