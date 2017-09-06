/****** Object:  StoredProcedure [dbo].[USP_Busi_AdjVanOrderSeq]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_AdjVanOrderSeq')
	DROP PROCEDURE USP_Busi_AdjVanOrderSeq
CREATE PROCEDURE [dbo].[USP_Busi_AdjVanOrderSeq]
	@ProdLine varchar(50),  --SAP生产线代码
	@TraceCode varchar(50), --Van号
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
	Declare @OrgVanOrderNo varchar(50);   --原生产单号，需要调整顺序的生产单
	Declare @OrgVanOrderStatus tinyint;   --原生产单状态
	Declare @OrgVanOrderSeq bigint;		  --原生产单顺序号
	Declare @TargetVanOrderNo varchar(50);--目的生产单号，插入点的生产单
	Declare @TargetVanOrderStatus tinyint;--目的生产单状态
	Declare @TargetVanOrderSeq bigint;	  --目的生产单顺序号
	Declare @VanProdLine varchar(50);     --生产线代码
	
	--查找原生产单
	select @OrgVanOrderNo = OrderNo, @VanProdLine = Flow, @OrgVanOrderStatus = [Status], @OrgVanOrderSeq = Seq from ORD_OrderMstr_4 where Flow = @ProdLine and TraceCode = @TraceCode;
	
	If (@OrgVanOrderStatus is not null and @OrgVanOrderStatus <> 0) 
	begin
		--如果原生产单已经释放，不能调整顺序
		set @IsAdj = 0;
		return
	end
	
	If (@OrgVanOrderNo is null) 
	begin
		--原生产单还没有导入LES，在生产线映射表中查找生产线代码
		select @VanProdLine = ProdLine from CUST_ProductLineMap where SAPProdLine = @ProdLine;
	end
	
	--查找插入点的生产单
	select @TargetVanOrderNo = OrderNo, @TargetVanOrderStatus = [Status], @TargetVanOrderSeq = Seq from ORD_OrderMstr_4 where Flow = @VanProdLine and SapSeq = @NewSeq
	If (@TargetVanOrderStatus is not null and @TargetVanOrderStatus <> 0) 
	begin
		--如果插入点的生产单已经释放，不能调整顺序
		set @IsAdj = 0;
		return
	end
	
	--剩余情况都能够调整顺序
	set @IsAdj = 1;
	begin tran
		if (@OrgVanOrderNo is null and @TargetVanOrderNo is null)
		begin
			--如果原生产单和目的生产单都没有导入，可以调整
			return;
		end
		else if (@TargetVanOrderNo is null)			
		begin
			--如果目的生产单没有导入，相当于把原生产单调整至没有导入LES的生产单队列中
			--从当前生产队列中删除，调整原生产单队列顺序
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
