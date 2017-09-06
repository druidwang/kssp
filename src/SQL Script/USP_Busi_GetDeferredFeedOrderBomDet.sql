
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
	--��ѯ�������ĺ�������
	select distinct mstr.OrderNo, 
	mstr.Seq,
	MIN(op.Op) as NextOp
	into #NextOrderOp 
	from ORD_OrderMstr_4 as mstr
	inner join ORD_OrderOp as op on mstr.OrderNo = op.OrderNo and (mstr.CurtOp is null or mstr.CurtOp < op.Op)
	where mstr.Flow = @Flow and mstr.Status = 2 and mstr.IsPause = 0
	group by mstr.OrderNo, mstr.Seq	
	
	--��ѯ����û�лس�Ĺ���
	--select distinct op.OrderNo, op.Op, nextOp.Seq into #NextNotBFOp from #NextOrderOp as nextOp
	--inner join ORD_OrderOp as op on nextOp.OrderNo = op.OrderNo and op.IsBackflush = 0
	
	--������Ź����س�Ĺ�����ͬ������ǰһ��Ϊ����Ĺ��������빤��������й�������Ҫǰ����λ�Ϳۼ�����	
	--������ͬ�Ĺ���ȡ�������������Сһ�����ڴ��س��б���ɾ��������Ŵ��ڸ���ŵ����й���
	--delete a from #NextOrderOp as a
	--inner join (select top 1 MIN(Seq) as MinSeq from #NextNotBFOp group by Op having COUNT(*) > 1 order by Op desc) as b
	--on a.Seq > b.MinSeq
	delete a from #NextOrderOp as a
	inner join (select top 1 MIN(Seq) as MinSeq from #NextOrderOp group by NextOp having COUNT(*) > 1 order by NextOp desc) as b
	on a.Seq > b.MinSeq
	
	--�����������ĵ�ǰ����
	update ORD_OrderMstr_4 set CurtOp = nextOp.NextOp, 								
								LastModifyDate = GETDATE(), 
								LastModifyUser = @UserId, 
								LastModifyUserNm = @UserNm, 
								[Version] = [Version] + 1
	from ORD_OrderMstr_4 as mstr 
	inner join #NextOrderOp as nextOp on mstr.OrderNo = nextOp.OrderNo
	
	--���º�������Ļس���
	update ORD_OrderOp set IsBackflush = 1,
							LastModifyDate = GETDATE(), 
							LastModifyUser = @UserId, 
							LastModifyUserNm = @UserNm, 
							[Version] = [Version] + 1
	from ORD_OrderOp as op 
	inner join #NextOrderOp as nextOp on op.OrderNo = nextOp.OrderNo and op.Op = nextOp.NextOp

	--���ش��س��������Bom
	select bom.* from #NextOrderOp as op
	inner join ORD_OrderBomDet as bom on op.OrderNo = bom.OrderNo and op.NextOp = bom.Op	
	where bom.IsAutoFeed = 1    --�Զ�Ͷ��
	and bom.FeedMethod = 1      --Ͷ����������Ʒ
	and bom.BackFlushMethod = 1 --�س幤������Ʒ
END
GO
