
/****** Object:  StoredProcedure [dbo].[USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderNotExist]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderNotExist')
	DROP PROCEDURE USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderNotExist
CREATE PROCEDURE [dbo].[USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderNotExist]
	@VanProdLine varchar(50),     --�����ߴ���
	@OrgVanOrderNo varchar(50),   --ԭ�������ţ���Ҫ����˳���������
	@OldStartTime DateTime,
	@NewStartTime DateTime,
	@OldSeq bigint,
	@NewSeq bigint,
	@UserId int,
	@UserNm varchar(100)
AS
BEGIN
	begin tran
		if Exists(select top 1 OrderNo from ORD_OrderMstr_4 where StartTime between @NewStartTime and DATEADD(DAY,+1,@NewStartTime))
		begin
			Declare @maxSeq bigint;
			select @maxSeq = MAX(Seq) from ORD_OrderMstr_4 where StartTime between @NewStartTime and DATEADD(DAY,+1,@NewStartTime)
			--���������Ŀ�ʼ����������������������Ϊָ����@NewSeqΪ��������һ��û�и�˳���������
			update ORD_OrderMstr_4 set StartTime = @NewStartTime, WindowTime = @NewStartTime, Seq = @maxSeq + 1, SapSeq = @NewSeq, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where OrderNo = @OrgVanOrderNo
		end
		else
		begin
			--ɾ��ԭ������
			delete from ORD_OrderMstr_4 where OrderNo = @OrgVanOrderNo;			
		end
		update ORD_OrderMstr_4 set Seq = Seq - 1, SapSeq = SapSeq - 1, Version = Version + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where Flow = @VanProdLine and StartTime = @OldStartTime and SapSeq > @OldSeq
	commit
END
GO