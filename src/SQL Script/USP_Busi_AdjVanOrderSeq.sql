/****** Object:  StoredProcedure [dbo].[USP_Busi_AdjVanOrderSeq]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_AdjVanOrderSeq')
	DROP PROCEDURE USP_Busi_AdjVanOrderSeq
CREATE PROCEDURE [dbo].[USP_Busi_AdjVanOrderSeq]
	@ProdLine varchar(50),  --SAP�����ߴ���
	@TraceCode varchar(50), --Van��
	@OldStartTime DateTime,
	@OldSeq bigint,
	@NewStartTime DateTime,
	@NewSeq bigint,
	@UserId int,
	@UserNm varchar(100),
	@IsAdj bit output
AS
BEGIN
	Declare @DateTimeNow datetime = GetDate();
	Declare @OrgVanOrderNo varchar(50);   --ԭ�������ţ���Ҫ����˳���������
	Declare @OrgVanOrderStatus tinyint;   --ԭ������״̬
	Declare @OrgVanOrderSeq bigint;		  --ԭ������˳���
	Declare @TargetVanOrderNo varchar(50);--Ŀ���������ţ�������������
	Declare @TargetVanOrderStatus tinyint;--Ŀ��������״̬
	Declare @TargetVanOrderSeq bigint;	  --Ŀ��������˳���
	Declare @VanProdLine varchar(50);     --�����ߴ���
	
	--����ԭ������
	select @OrgVanOrderNo = OrderNo, @VanProdLine = Flow, @OrgVanOrderStatus = [Status], @OrgVanOrderSeq = Seq from ORD_OrderMstr_4 where Flow = @ProdLine and TraceCode = @TraceCode;
	
	If (@OrgVanOrderStatus is not null and @OrgVanOrderStatus <> 0) 
	begin
		--���ԭ�������Ѿ��ͷţ����ܵ���˳��
		set @IsAdj = 0;
		return
	end
	
	If (@OrgVanOrderNo is null) 
	begin
		--ԭ��������û�е���LES����������ӳ����в��������ߴ���
		select @VanProdLine = ProdLine from CUST_ProductLineMap where SAPProdLine = @ProdLine;
	end
	
	--���Ҳ�����������
	select @TargetVanOrderNo = OrderNo, @TargetVanOrderStatus = [Status], @TargetVanOrderSeq = Seq from ORD_OrderMstr_4 where Flow = @VanProdLine and SapSeq = @NewSeq
	If (@TargetVanOrderStatus is not null and @TargetVanOrderStatus <> 0) 
	begin
		--����������������Ѿ��ͷţ����ܵ���˳��
		set @IsAdj = 0;
		return
	end
	
	--ʣ��������ܹ�����˳��
	set @IsAdj = 1;
	begin tran
		if (@OrgVanOrderNo is null and @TargetVanOrderNo is null)
		begin
			--���ԭ��������Ŀ����������û�е��룬���Ե���
			return;
		end
		else if (@TargetVanOrderNo is null)			
		begin
			--���Ŀ��������û�е��룬�൱�ڰ�ԭ������������û�е���LES��������������
			--�ӵ�ǰ����������ɾ��������ԭ����������˳��
			delete from ORD_OrderMstr_4 where OrderNo = @OrgVanOrderNo;
			update ORD_OrderMstr_4 set Seq = Seq - 1, SapSeq = SapSeq - 1, Version = Version + 1, LastModifyUser = @UserId, LastModifyUserNm = @UserNm, LastModifyDate = @DateTimeNow where Flow = @VanProdLine and StartTime = @OldStartTime and SapSeq > @OldSeq
		end
		else if (@OrgVanOrderNo is null)
		begin
			set @IsAdj = 1;
		end
		else
		begin
			set @IsAdj = 1;
		end
	commit
END
GO
