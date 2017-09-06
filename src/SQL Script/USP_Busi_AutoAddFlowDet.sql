
/****** Object:  StoredProcedure [dbo].[USP_Busi_AutoAddFlowDet]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_AutoAddFlowDet')
	DROP PROCEDURE USP_Busi_AutoAddFlowDet
CREATE PROCEDURE [dbo].[USP_Busi_AutoAddFlowDet]
	@OrderNo varchar(50),
	@UserId int,
	@UserNm varchar(50)
AS
BEGIN
	declare @DateTimeNow datetime = GetDate();
	
	select distinct Item, Location into #tempOrderBom from ORD_OrderBomDet where OrderNo = @OrderNo;
	
	select distinct det.Flow, det.Item, det.RefItemCode, det.Uom, det.BaseUom, det.UC, det.UCDesc, det.MinUC, 
	case when isnull(det.LocFrom, '') <> '' then det.LocFrom else mstr.LocFrom end as LocFrom, 
	case when isnull(det.LocTo, '') <> '' then det.LocTo else mstr.LocTo end as LocTo, 
	case when isnull(det.BillAddr, '') <> '' then det.BillAddr else mstr.BillAddr end as BillAddr, 
	case when isnull(det.PriceList, '') <> '' then det.PriceList else mstr.PriceList end as PriceList, 
	det.Container, det.ContainerDesc, det.IsInspect, det.IsRejInspect, det.RoundUpOpt, det.BillTerm
	into #tempFlowDet from SCM_FlowDet as det 
	inner join SCM_FlowMstr as mstr on det.Flow = mstr.Code
	--（新逻辑）只查看当前生产单的拉料路线
	inner join #tempOrderBom as bom on (bom.Location = det.LocTo or (det.LocTo is null and bom.Location = mstr.LocTo))
	--（旧逻辑）生产单的来源区域等于路线的目的区域
	--where mstr.PartyTo in (select PartyFrom from ORD_OrderMstr_4 where OrderNo = 'OrderNo')
	--路线明细在有效期内
	and (det.StartDate is null or det.StartDate <= @DateTimeNow) and (det.EndDate is null or det.EndDate >= @DateTimeNow)
	
	insert into SCM_FlowDet 
	(
	Flow,				--1
	Seq,				--2
	Item,				--3
	RefItemCode,		--4
	Uom,				--5
	BaseUom,			--6
	UC,					--7
	UCDesc,				--8
	MinUC,				--9
	StartDate,			--10
	LocFrom,			--11
	LocTo,				--12
	BillAddr,			--13
	PriceList,			--14
	Strategy,			--15
	Container,			--16
	ContainerDesc,		--17
	IsAutoCreate,		--18
	IsInspect,			--19
	IsRejInspect,		--20
	RoundUpOpt,			--21
	BillTerm,			--22
	MrpWeight,			--23
	MrpTotal,			--24
	MrpTotalAdj,		--25
	CreateUser,			--26
	CreateUserNm,		--27
	CreateDate,			--28
	LastModifyUser,		--29
	LastModifyUserNm,	--30
	LastModifyDate,		--31
	IsChangeUC			--32
	)
	select 
	det.Flow,			--1
	999,				--2
	det.Item,			--3
	det.RefItemCode,	--4
	det.Uom,			--5
	det.BaseUom,		--6
	det.UC,				--7
	det.UCDesc,			--8
	det.MinUC,			--9
	@DateTimeNow,		--10
	det.LocFrom,		--11
	a.Location as LocTo,--12	使用Bom中零件的消耗库位作为入库库位
	det.BillAddr,		--13
	det.PriceList,		--14
	3,					--15	添加的路线全部采用JIT拉动
	det.Container,		--16	
	det.ContainerDesc,	--17
	1,					--18	设置为自动拉动
	det.IsInspect,		--19
	det.IsRejInspect,	--20
	det.RoundUpOpt,		--21
	det.BillTerm,		--22
	0,					--23
	0,					--24
	0,					--25
	@UserId,			--26
	@UserNm,			--27
	@DateTimeNow,		--28
	@UserId,			--29
	@UserNm,			--30
	@DateTimeNow,		--31
	0					--32
	from #tempFlowDet as det 
	inner join 
	(
		--未找到配送路线的零件
		select bom.Item, bom.Location from #tempOrderBom as bom left join #tempFlowDet as det on bom.Item = det.Item and bom.Location = det.LocTo
		where det.Item is null
	) as a on det.Item = a.Item
	group by det.Flow, det.Item, det.RefItemCode, det.Uom, det.BaseUom, det.UC, det.UCDesc, det.MinUC, 
	det.LocFrom, a.Location, det.BillAddr, det.PriceList, det.Container, det.ContainerDesc, det.IsInspect, 
	det.IsRejInspect, det.RoundUpOpt, det.BillTerm
	--过滤掉同一个零件找到两条配送路线
	having COUNT(*) = 1
	
	drop table #tempOrderBom;
	drop table #tempFlowDet;
END
GO