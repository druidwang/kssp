
/****** Object:  StoredProcedure [dbo].[USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderExist]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderExist')
	DROP PROCEDURE USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderExist
CREATE PROCEDURE [dbo].[USP_Busi_UpdateSeq4AdjVanOrderSeq_TargetVanOrderExist]
	@VanProdLine varchar(50),     --生产线代码
	@OrgVanOrderNo varchar(50),   --原生产单号，需要调整顺序的生产单
	@OldStartTime DateTime,
	@NewStartTime DateTime,
	@OldSeq bigint,               --SAP原生产单顺序
	@NewSeq bigint,				  --SAP目的生产单顺序
	@TargetVanOrderSeq bigint,       --目的生产单顺序号，和SAP的顺序号要区分开，因为释放后的生产单调整顺序不会改变SAPSeq，只会修改Seq
	@UserId int,
	@UserNm varchar(100)
AS
BEGIN
	begin tran
		 if (@OldStartTime = @NewStartTime)
		 begin
			--调整前生产单的开工日期和目的生产单的开工日期在同一天
			if (@OldSeq > @NewSeq)
			begin
				--往前调整，NewSeq <= Seq < OldSeq，调整至目的生产单之后
				update ORD_OrderMstr_4 set Seq = Seq + 1, SapSeq = SapSeq + 1, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where Flow = @VanProdLine and StartTime = @NewStartTime and SapSeq >= @NewSeq and SapSeq < @OldSeq
				update ORD_OrderMstr_4 set Seq = @TargetVanOrderSeq, SapSeq = @NewSeq, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where OrderNo = @OrgVanOrderNo
			end  
			else if (@OldSeq < @NewSeq)
			begin
				--往后调整，OldSeq < Seq <= NewSeq，调整至目的生产单之后
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
			--调整前生产单的开工日期和目的生产单的开工日期不在同一天，调整前的工单从原来队列调整至新队列中
			update ORD_OrderMstr_4 set Seq = Seq + 1, SapSeq = SapSeq + 1, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where Flow = @VanProdLine and StartTime = @NewStartTime and SapSeq >= @NewSeq
			update ORD_OrderMstr_4 set Seq = Seq - 1, SapSeq = SapSeq - 1, Version = Version + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where Flow = @VanProdLine and StartTime = @OldStartTime and SapSeq > @OldSeq
			update ORD_OrderMstr_4 set StartTime = @NewStartTime, WindowTime = @NewStartTime, Seq = @TargetVanOrderSeq, SapSeq = @NewSeq, [Version] = [Version] + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = GETDATE() where OrderNo = @OrgVanOrderNo
		 end
	commit
END
GO