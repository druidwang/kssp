
/****** Object:  StoredProcedure [dbo].[USP_Busi_GetDeferredFeedOrderBomDet]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_GetDeferredFeedOrderBomDet')
	DROP PROCEDURE USP_Busi_GetDeferredFeedOrderBomDet
CREATE PROCEDURE [dbo].[USP_Busi_GetDeferredFeedOrderBomDet]
(
	@Flow varchar(50),
	@UserId int,
	@UserNm varchar(100)
)
AS
BEGIN
	--查询生产单的后续工序
	select distinct mstr.OrderNo, 
	mstr.Seq,
	MIN(op.Op) as NextOp
	into #NextOrderOp 
	from ORD_OrderMstr_4 as mstr
	inner join ORD_OrderOp as op on mstr.OrderNo = op.OrderNo and (mstr.CurtOp is null or mstr.CurtOp < op.Op)
	where mstr.Flow = @Flow and mstr.Status = 2 and mstr.IsPause = 0
	group by mstr.OrderNo, mstr.Seq	
	
	--查询后续没有回冲的工序
	--select distinct op.OrderNo, op.Op, nextOp.Seq into #NextNotBFOp from #NextOrderOp as nextOp
	--inner join ORD_OrderOp as op on nextOp.OrderNo = op.OrderNo and op.IsBackflush = 0
	
	--如果两张工单回冲的工序相同，代表前一张为插入的工单，插入工单后的所有工单都不要前进工位和扣减物料	
	--工序相同的工单取工序号最大，序号最小一条，在待回冲列表中删除工单序号大于该序号的所有工单
	--delete a from #NextOrderOp as a
	--inner join (select top 1 MIN(Seq) as MinSeq from #NextNotBFOp group by Op having COUNT(*) > 1 order by Op desc) as b
	--on a.Seq > b.MinSeq
	delete a from #NextOrderOp as a
	inner join (select top 1 MIN(Seq) as MinSeq from #NextOrderOp group by NextOp having COUNT(*) > 1 order by NextOp desc) as b
	on a.Seq > b.MinSeq
	
	--更新生产单的当前工序
	update ORD_OrderMstr_4 set CurtOp = nextOp.NextOp, 								
								LastModifyDate = GETDATE(), 
								LastModifyUser = @UserId, 
								LastModifyUserNm = @UserNm, 
								[Version] = [Version] + 1
	from ORD_OrderMstr_4 as mstr 
	inner join #NextOrderOp as nextOp on mstr.OrderNo = nextOp.OrderNo
	
	--更新后续工序的回冲标记
	update ORD_OrderOp set IsBackflush = 1,
							LastModifyDate = GETDATE(), 
							LastModifyUser = @UserId, 
							LastModifyUserNm = @UserNm, 
							[Version] = [Version] + 1
	from ORD_OrderOp as op 
	inner join #NextOrderOp as nextOp on op.OrderNo = nextOp.OrderNo and op.Op = nextOp.NextOp

	--返回待回冲的生产单Bom
	select bom.* from #NextOrderOp as op
	inner join ORD_OrderBomDet as bom on op.OrderNo = bom.OrderNo and op.NextOp = bom.Op	
	where bom.IsAutoFeed = 1    --自动投料
	and bom.FeedMethod = 1      --投料至工单成品
	and bom.BackFlushMethod = 1 --回冲工单在制品
END
GO
