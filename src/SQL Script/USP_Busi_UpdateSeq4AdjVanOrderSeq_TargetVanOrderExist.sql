
/****** Object:  StoredProcedure [dbo].[USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderExist]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderExist')
	DROP PROCEDURE USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderExist
CREATE PROCEDURE [dbo].[USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderExist]
	@VanProdLine varchar(50),     --�����ߴ���
	@OrgVanOrderNo varchar(50),   --ԭ�������ţ���Ҫ����˳���������
	@OldStartTime DateTime,
	@NewStartTime DateTime,
	@OldSeq bigint,               --SAPԭ������˳��
	@NewSeq bigint,				  --SAPĿ��������˳��
	@TargetVanOrderSeq bigint,       --Ŀ��������˳��ţ���SAP��˳���Ҫ���ֿ�����Ϊ�ͷź������������˳�򲻻�ı�SAPSeq��ֻ���޸�Seq
	@UserId int,
	@UserNm varchar(100)
AS
BEGIN
	begin tran
		 if (@OldStartTime = @NewStartTime)
		 begin
			--����ǰ�������Ŀ������ں�Ŀ���������Ŀ���������ͬһ��
			if (@OldSeq > @NewSeq)
			begin
				--��ǰ������NewSeq <= Seq < OldSeq��������Ŀ��������֮��
				update ORD_OrderMstr_4 set Seq = Seq + 1, SapSeq = SapSeq + 1, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where Flow = @VanProdLine and StartTime = @NewStartTime and SapSeq >= @NewSeq and SapSeq < @OldSeq
				update ORD_OrderMstr_4 set Seq = @TargetVanOrderSeq, SapSeq = @NewSeq, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where OrderNo = @OrgVanOrderNo
			end  
			else if (@OldSeq < @NewSeq)
			begin
				--���������OldSeq < Seq <= NewSeq��������Ŀ��������֮��
				update ORD_OrderMstr_4 set Seq = Seq - 1, SapSeq = SapSeq - 1, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where Flow = @VanProdLine and StartTime = @NewStartTime and SapSeq <= @NewSeq and SapSeq > @OldSeq
				update ORD_OrderMstr_4 set Seq = @TargetVanOrderSeq, SapSeq = @NewSeq, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where OrderNo = @OrgVanOrderNo
			end
			else
			begin
				return;
			end
		 end
		 else
		 begin
			--����ǰ�������Ŀ������ں�Ŀ���������Ŀ������ڲ���ͬһ�죬����ǰ�Ĺ�����ԭ�����е������¶�����
			update ORD_OrderMstr_4 set Seq = Seq + 1, SapSeq = SapSeq + 1, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where Flow = @VanProdLine and StartTime = @NewStartTime and SapSeq >= @NewSeq
			update ORD_OrderMstr_4 set Seq = Seq - 1, SapSeq = SapSeq - 1, Version = Version + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where Flow = @VanProdLine and StartTime = @OldStartTime and SapSeq > @OldSeq
			update ORD_OrderMstr_4 set StartTime = @NewStartTime, WindowTime = @NewStartTime, Seq = @TargetVanOrderSeq, SapSeq = @NewSeq, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where OrderNo = @OrgVanOrderNo
		 end
	commit
END
GO