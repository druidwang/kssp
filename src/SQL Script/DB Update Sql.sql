--INV_StockTakeMstr
ALTER TABLE INV_StockTakeMstr DROP COLUMN StartUser;
go
ALTER TABLE INV_StockTakeMstr DROP COLUMN StartUserNm;
go
ALTER TABLE INV_StockTakeMstr DROP COLUMN StartDate;
go
ALTER TABLE INV_StockTakeMstr DROP COLUMN EffStartDate;
go
ALTER TABLE INV_StockTakeMstr DROP COLUMN EffCompleteDate;
go
ALTER TABLE INV_StockTakeMstr ADD StockTakeDate datetime;
go

--INV_StockTakeResult
ALTER TABLE INV_StockTakeResult DROP CONSTRAINT FK_INV_STOC_REFERENCE_MD_UOM3;
go
ALTER TABLE INV_StockTakeResult DROP COLUMN Uom;
go
ALTER TABLE INV_StockTakeResult DROP COLUMN LotNo;
go
ALTER TABLE INV_StockTakeResult DROP COLUMN CompleteInvQty;
go
ALTER TABLE INV_StockTakeResult DROP COLUMN StartInvQty;
go
ALTER TABLE INV_StockTakeResult ADD InvQty decimal;
go


--INV_StockTakeDet
ALTER TABLE INV_StockTakeDet DROP CONSTRAINT FK_INV_STOC_REFERENCE_MD_UOM2;
go
ALTER TABLE INV_StockTakeDet DROP COLUMN Uom;
go
ALTER TABLE INV_StockTakeDet DROP COLUMN EffStartDate;
go
ALTER TABLE INV_StockTakeDet DROP COLUMN EffCompleteDate;
go
ALTER TABLE INV_StockTakeDet ADD StockTakeDate datetime;
go
ALTER TABLE INV_StockTakeDet ADD EffectDate datetime;
go

--包装描述
alter table MD_ItemPackage add Desc1 varchar(100);
go
update MD_ItemPackage set desc1 = '';
go
alter table MD_ItemPackage alter column Desc1 varchar(100) not null;
go


--节拍时间/节拍时间单位
alter table PRD_RoutingMstr add TaktTime float;
go
update PRD_RoutingMstr set TaktTime = 0;
go
alter table PRD_RoutingMstr alter column TaktTime float not null;
go
alter table PRD_RoutingMstr add TaktTimeUnit tinyint;
go
update PRD_RoutingMstr set TaktTimeUnit = 2;
go
alter table PRD_RoutingMstr alter column TaktTimeUnit tinyint not null;
go

--增加客户化零件追溯表
create table CUST_ItemTrace (
   Item                 varchar(50)          not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   constraint PK_CUST_ITEMTRACE primary key (Item)
)
go

alter table CUST_ItemTrace
   add constraint FK_CUST_ITE_REFERENCE_MD_ITEM foreign key (Item)
      references MD_Item (Code)
go


--OrderBomDetail的预计消耗时间不能为空，默认等于OrderMstr的StartTime。
alter table ORD_OrderBomDet alter column EstConsumeTime datetime not null;


--OrderBind添加字段
alter table ORD_OrderBinding add BindFlowStrategy tinyint not null;   --绑定订单的类型，为了知道是排序和看板拉动用。
go
alter table ORD_OrderBinding add BindOrderDet int;    --添加绑定订单明细，方便追踪排序/看板订单进行暂停。
go
alter table ORD_OrderBinding
   add constraint FK_ORD_ORDE_REFERENCE_ORD_ORDE8 foreign key (BindOrderDet)
      references ORD_OrderDet (Id)
go

--OrderMstr上增加FlowStrategy，维护FlowStrategy的时候反写
alter table SCM_FlowMstr add FlowStrategy tinyint;
go
update SCM_FlowMstr set FlowStrategy = 0;
go
alter table SCM_FlowMstr alter column FlowStrategy tinyint not null;
go

--FlowDetail.Container字段的外键建错了，建成和Customer表关联，现改正。
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('SCM_FlowDet') and o.name = 'FK_SCM_FLOW_REFERENCE_MD_CUSTO')
alter table SCM_FlowDet
   drop constraint FK_SCM_FLOW_REFERENCE_MD_CUSTO
go

alter table SCM_FlowDet
   add constraint FK_SCM_FLOW_REFERENCE_MD_CONTA foreign key (Container)
      references MD_Container (Code)
go



--CodeMstr和CodeDet定义进行调整
update sys_codemstr set type = 1 where code = 'InspectDefect'

delete from sys_codedet where code = 'LocationType'
delete from sys_codedet where code = 'FlowType'
delete from sys_codedet where code = 'StructureType'
delete from sys_codedet where code = 'Strategy'
delete from sys_codedet where code = 'BindCreate' and value = '4'
delete from sys_codedet where code = 'BindCreate' and value = '5'

delete from sys_codemstr where code = 'LocationType'
delete from sys_codemstr where code = 'FlowType'
delete from sys_codemstr where code = 'StructureType'
delete from sys_codemstr where code = 'Strategy'

insert into sys_codemstr values('BomStructureType', 'Bom结构类型', 0);
insert into sys_codemstr values('FlowStrategy', '路线自动化策略', 0);

insert into sys_codedet values('FlowStrategy', 0, 'CodeDetail_FlowStrategy_Manual', 1, 1);
insert into sys_codedet values('FlowStrategy', 1, 'CodeDetail_FlowStrategy_KB', 0, 2);
insert into sys_codedet values('FlowStrategy', 2, 'CodeDetail_FlowStrategy_JIT', 0, 3);
insert into sys_codedet values('FlowStrategy', 3, 'CodeDetail_FlowStrategy_SEQ', 0, 4);
insert into sys_codedet values('FlowStrategy', 4, 'CodeDetail_FlowStrategy_KIT', 0, 5);
insert into sys_codedet values('FlowStrategy', 5, 'CodeDetail_FlowStrategy_MRP', 0, 6);
insert into sys_codedet values('FlowStrategy', 6, 'CodeDetail_FlowStrategy_ANDON', 0, 7);
insert into sys_codedet values('BomStructureType', 0, 'CodeDetail_BomStructureType_Normal', 1, 1);
insert into sys_codedet values('BomStructureType', 1, 'CodeDetail_BomStructureType_Virtual', 0, 2);

--检验/不合格品库位相关定义
alter table MD_Location drop column Type;
alter table SCM_FlowMstr drop CONSTRAINT FK_SCM_FLOW_REFERENCE_MD_LOCAT4
alter table SCM_FlowMstr drop column InspLocFrom;
alter table SCM_FlowMstr drop CONSTRAINT FK_SCM_FLOW_REFERENCE_MD_LOCAT5
alter table SCM_FlowMstr drop column InspLocTo;
alter table SCM_FlowMstr drop CONSTRAINT FK_SCM_FLOW_REFERENCE_MD_LOCAT6
alter table SCM_FlowMstr drop column RejLocFrom;
alter table SCM_FlowMstr drop CONSTRAINT FK_SCM_FLOW_REFERENCE_MD_LOCAT7
alter table SCM_FlowMstr drop column RejLocTo;


alter table SCM_FlowDet drop CONSTRAINT FK_SCM_FLOW_REFERENCE_MD_LOCAT10
alter table SCM_FlowDet drop column InspLocFrom;
alter table SCM_FlowDet drop CONSTRAINT FK_SCM_FLOW_REFERENCE_MD_LOCAT11
alter table SCM_FlowDet drop column InspLocTo;
alter table SCM_FlowDet drop CONSTRAINT FK_SCM_FLOW_REFERENCE_MD_LOCAT12
alter table SCM_FlowDet drop column RejLocFrom;
alter table SCM_FlowDet drop CONSTRAINT FK_SCM_FLOW_REFERENCE_MD_LOCAT
alter table SCM_FlowDet drop column RejLocTo;


alter table ORD_OrderMstr drop CONSTRAINT FK_ORD_ORDE_REFERENCE_MD_LOCAT4
alter table ORD_OrderMstr drop column InspLoc;
alter table ORD_OrderMstr drop column InspLocNm;
alter table ORD_OrderMstr drop CONSTRAINT FK_ORD_ORDE_REFERENCE_MD_LOCAT5
alter table ORD_OrderMstr drop column RejLoc;
alter table ORD_OrderMstr drop column RejLocNm;


alter table ORD_OrderDet drop CONSTRAINT FK_ORD_ORDE_REFERENCE_MD_LOCAT8
alter table ORD_OrderDet drop column InspLoc;
alter table ORD_OrderDet drop column InspLocNm;
alter table ORD_OrderDet drop CONSTRAINT FK_ORD_ORDE_REFERENCE_MD_LOCAT9
alter table ORD_OrderDet drop column RejLoc;
alter table ORD_OrderDet drop column RejLocNm;


--2011-12-20 dingxin
--订单明细表增加Currency字段，方便采购销售计算价格。
alter table ORD_OrderDet add Currency varchar(50);
go
alter table ORD_OrderDet
   add constraint FK_ORD_ORDE_REFERENCE_MD_CURRE2 foreign key (Currency)
      references MD_Currency (Code)
go

--2011-12-26 dingxin
--增加盘点库位表，支持多库位同时盘点
create table INV_StockTakeLoc (
   Id                   int                  identity,
   StNo                 varchar(50)          not null,
   Loc                  varchar(50)          not null,
   LocName              varchar(50)          not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   constraint PK_INV_STOCKTAKELOC primary key (Id)
)
go

alter table INV_StockTakeLoc
   add constraint FK_INV_STOC_REFERENCE_INV_STOC foreign key (StNo)
      references INV_StockTakeMstr (StNo)
go

alter table INV_StockTakeLoc
   add constraint FK_INV_STOC_REFERENCE_MD_LOCAT foreign key (Loc)
      references MD_Location (Code)
go

--2011-12-26 dingxin
--增加盘点库存生效开始/结束时间
alter table inv_stocktakemstr add EffStartDate datetime;
alter table inv_stocktakemstr add EffCompleteDate datetime;
alter table inv_stocktakedet add EffStartDate datetime;
alter table inv_stocktakedet add EffCompleteDate datetime;
alter table inv_stocktakedet drop column StartTime;
alter table inv_stocktakedet drop column EndTime;
go

--2011-12-26 dingxin
--增加Issue代码表
create table ISS_IssueNo (
   Code                 varchar(50)          not null,
   Desc1                varchar(100)         not null,
   IssueType            varchar(50)          not null,
   Seq                  int                  not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   constraint PK_ISS_ISSUENO primary key (Code)
)
go

alter table ISS_IssueNo
   add constraint FK_ISS_ISSU_REFERENCE_ISS_ISSU foreign key (IssueType)
      references ISS_IssueType (Code)
go

--2011-12-28 dingxin
--盘点结果表增加开始库存时间和结束库存时间
alter table INV_StockTakeResult drop column InvQty;
alter table INV_StockTakeResult add StartInvQty decimal(18,8) not null;
alter table INV_StockTakeResult add CompleteInvQty decimal(18,8) not null;

CREATE VIEW [dbo].[ViewLocationDet]
AS
SELECT     MAX(Id) AS Id, Location, Item, SUM(CASE WHEN IsCS = 1 THEN Qty ELSE 0 END) AS CsQty, SUM(CASE WHEN IsCS = 0 THEN Qty ELSE 0 END) 
                      AS NmlQty, SUM(Qty) AS Qty
FROM         dbo.Inv_LocationLotDet
WHERE Qty<>0
GROUP BY Location, Item
HAVING      (SUM(Qty) <> 0)
alter table INV_StockTakeResult add CompleteInvQty decimal(18,8) not null;


--2011-12-28 dingxin
--重建ISS_IssueTypeToRoleDet和ISS_IssueTypeToUserDet
Drop table ISS_IssueTypeToRoleDet
go

CREATE TABLE [dbo].[ISS_IssueTypeToRoleDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IssueTypeTo] [varchar](50) NOT NULL,
	[RoleId] [int] NOT NULL,
	[IsEmail] [bit] NOT NULL,
	[IsSMS] [bit] NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ISS_ISSUETYPETOROLEDET] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ISS_IssueTypeToRoleDet]  WITH CHECK ADD  CONSTRAINT [FK_ISS_ISSU_REFERENCE_ACC_ROLE] FOREIGN KEY([RoleId])
REFERENCES [dbo].[ACC_Role] ([Id])
GO

ALTER TABLE [dbo].[ISS_IssueTypeToRoleDet] CHECK CONSTRAINT [FK_ISS_ISSU_REFERENCE_ACC_ROLE]
GO

ALTER TABLE [dbo].[ISS_IssueTypeToRoleDet]  WITH CHECK ADD  CONSTRAINT [FK_ISS_ISSU_REFERENCE_ISS_ISSU6] FOREIGN KEY([IssueTypeTo])
REFERENCES [dbo].[ISS_IssueTypeToMstr] ([Code])
GO

ALTER TABLE [dbo].[ISS_IssueTypeToRoleDet] CHECK CONSTRAINT [FK_ISS_ISSU_REFERENCE_ISS_ISSU6]
GO


Drop table ISS_IssueTypeToUserDet
go
CREATE TABLE [dbo].[ISS_IssueTypeToUserDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IssueTypeTo] [varchar](50) NOT NULL,
	[UserId] [int] NOT NULL,
	[IsEmail] [bit] NOT NULL,
	[IsSMS] [bit] NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ISS_ISSUETYPETOUSERDET] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ISS_IssueTypeToUserDet]  WITH CHECK ADD  CONSTRAINT [FK_ISS_ISSU_REFERENCE_ACC_USER] FOREIGN KEY([UserId])
REFERENCES [dbo].[ACC_User] ([Id])
GO

ALTER TABLE [dbo].[ISS_IssueTypeToUserDet] CHECK CONSTRAINT [FK_ISS_ISSU_REFERENCE_ACC_USER]
GO

ALTER TABLE [dbo].[ISS_IssueTypeToUserDet]  WITH CHECK ADD  CONSTRAINT [FK_ISS_ISSU_REFERENCE_ISS_ISSU5] FOREIGN KEY([IssueTypeTo])
REFERENCES [dbo].[ISS_IssueTypeToMstr] ([Code])
GO

ALTER TABLE [dbo].[ISS_IssueTypeToUserDet] CHECK CONSTRAINT [FK_ISS_ISSU_REFERENCE_ISS_ISSU5]
GO

--2011-12-28 dingxin
--重建ISS_IssueTypeToMstr和ISS_IssueLevel表外键关系
ALTER TABLE [dbo].[ISS_IssueTypeToMstr] drop CONSTRAINT [FK_ISS_ISSU_REFERENCE_ISS_ISSU4]
go
ALTER TABLE [dbo].[ISS_IssueTypeToMstr]  WITH CHECK ADD  CONSTRAINT [FK_ISS_ISSU_REFERENCE_ISS_ISSU4] FOREIGN KEY([IssueLevel])
REFERENCES [dbo].[ISS_IssueLevel] ([Code])
GO

ALTER TABLE [dbo].[ISS_IssueTypeToMstr] CHECK CONSTRAINT [FK_ISS_ISSU_REFERENCE_ISS_ISSU4]
GO


--2011-12-29 dingxin
--订单明细增加OrderType字段为了分表使用
alter table ord_orderdet add OrderType tinyint;
go
update ord_orderdet set OrderType = 4;
go
alter table ord_orderdet alter column OrderType tinyint not null;
go

--2011-12-29 dingxin
--删除检验库位和不合格品库位
alter table MD_Region drop CONSTRAINT FK_MD_REGIO_REFERENCE_MD_LOCAT;
alter table MD_Region drop CONSTRAINT FK_MD_REGIO_REFERENCE_MD_LOCAT2;
alter table MD_Region drop column InspectLoc;
alter table MD_Region drop column RejectLoc;
go
alter table MD_Region add Plant varchar(50);
go
ALTER TABLE [dbo].[ORD_IpDet] DROP CONSTRAINT [FK_ORD_IPDE_REFERENCE_MD_LOCAT]
ALTER TABLE [dbo].[ORD_IpDet] DROP CONSTRAINT [FK_ORD_IPDE_REFERENCE_MD_LOCAT4]
alter table [dbo].[ORD_IpDet] drop column InspLoc;
alter table [dbo].[ORD_IpDet] drop column InspLocNm;
alter table [dbo].[ORD_IpDet] drop column RejLoc;
alter table [dbo].[ORD_IpDet] drop column RejLocNm;
go

--2011-12-29 dingxin
--字段名称拼错了
alter table ORD_IpMstr drop column IsAsnAuotClose;
alter table ORD_IpMstr add IsAsnAutoClose bit not null;
go

--2011-12-29 dingxin
--去掉IpMstr上的生效日期，加在明细上面。
alter table ORD_IpMstr drop column EffDate;
alter table ORD_IpDet add EffDate DateTime;
go


--2011-12-29 dingxin
--重建发货一系列表
drop table bil_billdet
drop table bil_actbill
drop table bil_billmstr
drop table inv_locationlotdet
drop table ORD_IpRec
drop table ORD_IpLocationDet
drop table bil_planbill
drop table ORD_IpDet
drop table ORD_RecDet
drop table ORD_RecMstr
drop table ORD_IpMstr

/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     2011/12/29 17:27:16                          */
/*==============================================================*/


if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_ORD_ORDE')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_ORD_ORDE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_ORD_IPMS')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_ORD_IPMS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_ORD_RECM')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_ORD_RECM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_MD_ITEM')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_MD_ITEM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_MD_UOM')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_MD_UOM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_MD_ADDRE')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_MD_ADDRE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_MD_PARTY')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_MD_PARTY
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_BIL_PRIC')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_BIL_PRIC
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_MD_CURRE')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_MD_CURRE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_MD_LOCAT')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_MD_LOCAT
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_ActBill') and o.name = 'FK_BIL_ACTB_REFERENCE_MD_TAX')
alter table BIL_ActBill
   drop constraint FK_BIL_ACTB_REFERENCE_MD_TAX
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillDet') and o.name = 'FK_BIL_BILL_REFERENCE_BIL_BILL')
alter table BIL_BillDet
   drop constraint FK_BIL_BILL_REFERENCE_BIL_BILL
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillDet') and o.name = 'FK_BIL_BILL_REFERENCE_BIL_ACTB')
alter table BIL_BillDet
   drop constraint FK_BIL_BILL_REFERENCE_BIL_ACTB
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillDet') and o.name = 'FK_BIL_BILL_REFERENCE_BIL_PRIC')
alter table BIL_BillDet
   drop constraint FK_BIL_BILL_REFERENCE_BIL_PRIC
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillDet') and o.name = 'FK_BIL_BILL_REFERENCE_MD_ITEM')
alter table BIL_BillDet
   drop constraint FK_BIL_BILL_REFERENCE_MD_ITEM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillDet') and o.name = 'FK_BIL_BILL_REFERENCE_MD_UOM')
alter table BIL_BillDet
   drop constraint FK_BIL_BILL_REFERENCE_MD_UOM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillDet') and o.name = 'FK_BIL_BILL_REFERENCE_ORD_ORDE')
alter table BIL_BillDet
   drop constraint FK_BIL_BILL_REFERENCE_ORD_ORDE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillDet') and o.name = 'FK_BIL_BILL_REFERENCE_ORD_IPMS')
alter table BIL_BillDet
   drop constraint FK_BIL_BILL_REFERENCE_ORD_IPMS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillDet') and o.name = 'FK_BIL_BILL_REFERENCE_ORD_RECM')
alter table BIL_BillDet
   drop constraint FK_BIL_BILL_REFERENCE_ORD_RECM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillMstr') and o.name = 'FK_BIL_BILL_REFERENCE_MD_ADDRE')
alter table BIL_BillMstr
   drop constraint FK_BIL_BILL_REFERENCE_MD_ADDRE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillMstr') and o.name = 'FK_BIL_BILL_REFERENCE_MD_PARTY')
alter table BIL_BillMstr
   drop constraint FK_BIL_BILL_REFERENCE_MD_PARTY
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillMstr') and o.name = 'FK_BIL_BILL_REFERENCE_MD_CURRE')
alter table BIL_BillMstr
   drop constraint FK_BIL_BILL_REFERENCE_MD_CURRE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_BillMstr') and o.name = 'FK_BIL_BILL_REFERENCE_MD_TAX')
alter table BIL_BillMstr
   drop constraint FK_BIL_BILL_REFERENCE_MD_TAX
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_ORD_ORDE')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_ORD_ORDE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_ORD_IPMS')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_ORD_IPMS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_ORD_RECM')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_ORD_RECM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_MD_ITEM')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_MD_ITEM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_MD_UOM')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_MD_UOM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_MD_ADDRE')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_MD_ADDRE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_MD_PARTY')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_MD_PARTY
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_BIL_PRIC')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_BIL_PRIC
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_MD_CURRE')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_MD_CURRE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_INV_HU')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_INV_HU
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_MD_LOCAT')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_MD_LOCAT
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BIL_PlanBill') and o.name = 'FK_BIL_PLAN_REFERENCE_MD_TAX')
alter table BIL_PlanBill
   drop constraint FK_BIL_PLAN_REFERENCE_MD_TAX
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('INV_LocationLotDet') and o.name = 'FK_INV_LOCA_REFERENCE_BIL_PLAN')
alter table INV_LocationLotDet
   drop constraint FK_INV_LOCA_REFERENCE_BIL_PLAN
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('INV_LocationLotDet') and o.name = 'FK_INV_LOCA_REFERENCE_MD_LOCAT2')
alter table INV_LocationLotDet
   drop constraint FK_INV_LOCA_REFERENCE_MD_LOCAT2
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('INV_LocationLotDet') and o.name = 'FK_INV_LOCA_REFERENCE_MD_LOCAT')
alter table INV_LocationLotDet
   drop constraint FK_INV_LOCA_REFERENCE_MD_LOCAT
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('INV_LocationLotDet') and o.name = 'FK_INV_LOCA_REFERENCE_MD_ITEM')
alter table INV_LocationLotDet
   drop constraint FK_INV_LOCA_REFERENCE_MD_ITEM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('INV_LocationLotDet') and o.name = 'FK_INV_LOCA_REFERENCE_INV_HU')
alter table INV_LocationLotDet
   drop constraint FK_INV_LOCA_REFERENCE_INV_HU
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpDet') and o.name = 'FK_ORD_IPDE_REFERENCE_ORD_IPMS')
alter table ORD_IpDet
   drop constraint FK_ORD_IPDE_REFERENCE_ORD_IPMS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpDet') and o.name = 'FK_ORD_IPDE_REFERENCE_MD_ITEM')
alter table ORD_IpDet
   drop constraint FK_ORD_IPDE_REFERENCE_MD_ITEM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpDet') and o.name = 'FK_ORD_IPDE_REFERENCE_ORD_ORDE')
alter table ORD_IpDet
   drop constraint FK_ORD_IPDE_REFERENCE_ORD_ORDE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpDet') and o.name = 'FK_ORD_IPDE_REFERENCE_MD_UOM')
alter table ORD_IpDet
   drop constraint FK_ORD_IPDE_REFERENCE_MD_UOM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpDet') and o.name = 'FK_ORD_IPDE_REFERENCE_MD_LOCAT2')
alter table ORD_IpDet
   drop constraint FK_ORD_IPDE_REFERENCE_MD_LOCAT2
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpDet') and o.name = 'FK_ORD_IPDE_REFERENCE_MD_LOCAT3')
alter table ORD_IpDet
   drop constraint FK_ORD_IPDE_REFERENCE_MD_LOCAT3
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpDet') and o.name = 'FK_ORD_IPDE_REFERENCE_MD_ADDRE')
alter table ORD_IpDet
   drop constraint FK_ORD_IPDE_REFERENCE_MD_ADDRE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpDet') and o.name = 'FK_ORD_IPDE_REFERENCE_BIL_PRIC')
alter table ORD_IpDet
   drop constraint FK_ORD_IPDE_REFERENCE_BIL_PRIC
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpDet') and o.name = 'FK_ORD_IPDE_REFERENCE_MD_CURRE')
alter table ORD_IpDet
   drop constraint FK_ORD_IPDE_REFERENCE_MD_CURRE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpDet') and o.name = 'FK_ORD_IPDE_REFERENCE_MD_TAX')
alter table ORD_IpDet
   drop constraint FK_ORD_IPDE_REFERENCE_MD_TAX
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpLocationDet') and o.name = 'FK_ORD_IPLO_REFERENCE_INV_HU')
alter table ORD_IpLocationDet
   drop constraint FK_ORD_IPLO_REFERENCE_INV_HU
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpLocationDet') and o.name = 'FK_ORD_IPLO_REFERENCE_ORD_IPDE')
alter table ORD_IpLocationDet
   drop constraint FK_ORD_IPLO_REFERENCE_ORD_IPDE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpLocationDet') and o.name = 'FK_ORD_IPLO_REFERENCE_ORD_IPMS')
alter table ORD_IpLocationDet
   drop constraint FK_ORD_IPLO_REFERENCE_ORD_IPMS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpLocationDet') and o.name = 'FK_ORD_IPLO_REFERENCE_ORD_ORDE')
alter table ORD_IpLocationDet
   drop constraint FK_ORD_IPLO_REFERENCE_ORD_ORDE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpLocationDet') and o.name = 'FK_ORD_IPLO_REFERENCE_MD_ITEM')
alter table ORD_IpLocationDet
   drop constraint FK_ORD_IPLO_REFERENCE_MD_ITEM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpLocationDet') and o.name = 'FK_ORD_IPLO_REFERENCE_BIL_PLAN')
alter table ORD_IpLocationDet
   drop constraint FK_ORD_IPLO_REFERENCE_BIL_PLAN
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpMstr') and o.name = 'FK_ORD_IPMS_REFERENCE_MD_PARTY2')
alter table ORD_IpMstr
   drop constraint FK_ORD_IPMS_REFERENCE_MD_PARTY2
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpMstr') and o.name = 'FK_ORD_IPMS_REFERENCE_MD_PARTY')
alter table ORD_IpMstr
   drop constraint FK_ORD_IPMS_REFERENCE_MD_PARTY
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpMstr') and o.name = 'FK_ORD_IPMS_REFERENCE_MD_ADDRE2')
alter table ORD_IpMstr
   drop constraint FK_ORD_IPMS_REFERENCE_MD_ADDRE2
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpMstr') and o.name = 'FK_ORD_IPMS_REFERENCE_MD_ADDRE')
alter table ORD_IpMstr
   drop constraint FK_ORD_IPMS_REFERENCE_MD_ADDRE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpMstr') and o.name = 'FK_ORD_IPMS_REFERENCE_ORD_IPMS')
alter table ORD_IpMstr
   drop constraint FK_ORD_IPMS_REFERENCE_ORD_IPMS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpRec') and o.name = 'FK_ORD_IPRE_REFERENCE_ORD_RECD')
alter table ORD_IpRec
   drop constraint FK_ORD_IPRE_REFERENCE_ORD_RECD
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_IpRec') and o.name = 'FK_ORD_IPRE_REFERENCE_ORD_IPDE')
alter table ORD_IpRec
   drop constraint FK_ORD_IPRE_REFERENCE_ORD_IPDE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecDet') and o.name = 'FK_ORD_RECD_REFERENCE_ORD_RECM')
alter table ORD_RecDet
   drop constraint FK_ORD_RECD_REFERENCE_ORD_RECM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecDet') and o.name = 'FK_ORD_RECD_REFERENCE_ORD_ORDE')
alter table ORD_RecDet
   drop constraint FK_ORD_RECD_REFERENCE_ORD_ORDE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecDet') and o.name = 'FK_ORD_RECD_REFERENCE_MD_ITEM')
alter table ORD_RecDet
   drop constraint FK_ORD_RECD_REFERENCE_MD_ITEM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecDet') and o.name = 'FK_ORD_RECD_REFERENCE_MD_UOM')
alter table ORD_RecDet
   drop constraint FK_ORD_RECD_REFERENCE_MD_UOM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecDet') and o.name = 'FK_ORD_RECD_REFERENCE_INV_HU')
alter table ORD_RecDet
   drop constraint FK_ORD_RECD_REFERENCE_INV_HU
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecDet') and o.name = 'FK_ORD_RECD_REFERENCE_MD_LOCAT2')
alter table ORD_RecDet
   drop constraint FK_ORD_RECD_REFERENCE_MD_LOCAT2
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecDet') and o.name = 'FK_ORD_RECD_REFERENCE_MD_LOCAT')
alter table ORD_RecDet
   drop constraint FK_ORD_RECD_REFERENCE_MD_LOCAT
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecMstr') and o.name = 'FK_ORD_RECM_REFERENCE_ORD_IPMS')
alter table ORD_RecMstr
   drop constraint FK_ORD_RECM_REFERENCE_ORD_IPMS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecMstr') and o.name = 'FK_ORD_RECM_REFERENCE_MD_PARTY2')
alter table ORD_RecMstr
   drop constraint FK_ORD_RECM_REFERENCE_MD_PARTY2
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecMstr') and o.name = 'FK_ORD_RECM_REFERENCE_MD_PARTY')
alter table ORD_RecMstr
   drop constraint FK_ORD_RECM_REFERENCE_MD_PARTY
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecMstr') and o.name = 'FK_ORD_RECM_REFERENCE_MD_ADDRE2')
alter table ORD_RecMstr
   drop constraint FK_ORD_RECM_REFERENCE_MD_ADDRE2
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_RecMstr') and o.name = 'FK_ORD_RECM_REFERENCE_MD_ADDRE')
alter table ORD_RecMstr
   drop constraint FK_ORD_RECM_REFERENCE_MD_ADDRE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('BIL_ActBill')
            and   type = 'U')
   drop table BIL_ActBill
go

if exists (select 1
            from  sysobjects
           where  id = object_id('BIL_BillDet')
            and   type = 'U')
   drop table BIL_BillDet
go

if exists (select 1
            from  sysobjects
           where  id = object_id('BIL_BillMstr')
            and   type = 'U')
   drop table BIL_BillMstr
go

if exists (select 1
            from  sysobjects
           where  id = object_id('BIL_PlanBill')
            and   type = 'U')
   drop table BIL_PlanBill
go

if exists (select 1
            from  sysobjects
           where  id = object_id('INV_LocationLotDet')
            and   type = 'U')
   drop table INV_LocationLotDet
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ORD_IpDet')
            and   type = 'U')
   drop table ORD_IpDet
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ORD_IpLocationDet')
            and   type = 'U')
   drop table ORD_IpLocationDet
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ORD_IpMstr')
            and   type = 'U')
   drop table ORD_IpMstr
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ORD_IpRec')
            and   type = 'U')
   drop table ORD_IpRec
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ORD_RecDet')
            and   type = 'U')
   drop table ORD_RecDet
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ORD_RecMstr')
            and   type = 'U')
   drop table ORD_RecMstr
go

/*==============================================================*/
/* Table: BIL_ActBill                                           */
/*==============================================================*/
create table BIL_ActBill (
   Id                   int                  identity,
   OrderNo              varchar(50)          not null,
   IpNo                 varchar(50)          null,
   ExtIpNo              varchar(50)          null,
   RecNo                varchar(50)          null,
   ExtRecNo             varchar(50)          null,
   Type                 tinyint              not null,
   Item                 varchar(50)          not null,
   ItemDesc             varchar(100)         not null,
   Uom                  varchar(5)           not null,
   UC                   decimal(18,8)        not null,
   BillAddr             varchar(50)          not null,
   BillAddrDesc         varchar(256)         null,
   Party                varchar(50)          not null,
   PartyNm              varchar(100)         null,
   PriceList            varchar(50)          not null,
   Currency             varchar(50)          not null,
   UnitPrice            decimal(18,8)        not null,
   IsProvEst            bit                  not null,
   Tax                  varchar(50)          null,
   IsIncludeTax         bit                  not null,
   BillAmount           decimal(18,8)        not null,
   BilledAmount         decimal(18,8)        not null,
   BillQty              decimal(18,8)        not null,
   BilledQty            decimal(18,8)        not null,
   UnitQty              decimal(18,8)        not null,
   LocFrom              varchar(50)          null,
   EffDate              datetime             not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   Version              int                  not null,
   constraint PK_BIL_ACTBILL primary key (Id)
)
go

/*==============================================================*/
/* Table: BIL_BillDet                                           */
/*==============================================================*/
create table BIL_BillDet (
   Id                   int                  not null,
   BillNo               varchar(50)          not null,
   ActBill              int                  not null,
   Item                 varchar(50)          not null,
   ItemDesc             varchar(100)         not null,
   Uom                  varchar(5)           not null,
   UC                   decimal(18,8)        not null,
   Qty                  decimal(18,8)        not null,
   PriceList            varchar(50)          not null,
   Amount               decimal(18,8)        not null,
   UnitPrice            decimal(18,8)        not null,
   OrderNo              varchar(50)          not null,
   IpNo                 varchar(50)          null,
   ExtIpNo              varchar(50)          null,
   RecNo                varchar(50)          null,
   ExtRecNo             varchar(50)          null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   Version              int                  not null,
   constraint PK_BIL_BILLDET primary key (Id)
)
go

/*==============================================================*/
/* Table: BIL_BillMstr                                          */
/*==============================================================*/
create table BIL_BillMstr (
   BillNo               varchar(50)          not null,
   ExtBillNo            varchar(50)          null,
   RefBillNo            varchar(50)          null,
   Type                 tinyint              not null,
   SubType              tinyint              not null,
   Status               tinyint              not null,
   BillAddr             varchar(50)          not null,
   BillAddrDesc         varchar(256)         null,
   Party                varchar(50)          not null,
   PartyNm              varchar(100)         null,
   Currency             varchar(50)          not null,
   IsIncludeTax         bit                  not null,
   Tax                  varchar(50)          null,
   EffDate              datetime             not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   Version              int                  not null,
   constraint PK_BIL_BILLMSTR primary key (BillNo)
)
go

/*==============================================================*/
/* Table: BIL_PlanBill                                          */
/*==============================================================*/
create table BIL_PlanBill (
   Id                   int                  identity,
   OrderNo              varchar(50)          not null,
   IpNo                 varchar(50)          null,
   ExtIpNo              varchar(50)          null,
   RecNo                varchar(50)          null,
   ExtRecNo             varchar(50)          null,
   Type                 tinyint              not null,
   Item                 varchar(50)          not null,
   ItemDesc             varchar(100)         not null,
   Uom                  varchar(5)           not null,
   UC                   decimal(18,8)        not null,
   BillTerm             tinyint              not null,
   BillAddr             varchar(50)          not null,
   BillAddrDesc         varchar(256)         null,
   Party                varchar(50)          not null,
   PartyNm              varchar(100)         null,
   PriceList            varchar(50)          not null,
   Currency             varchar(50)          not null,
   UnitPrice            decimal(18,8)        not null,
   IsProvEst            bit                  not null,
   Tax                  varchar(50)          null,
   IsIncludeTax         bit                  not null,
   PlanAmount           decimal(18,8)        not null,
   ActAmount            decimal(18,8)        not null,
   PlanQty              decimal(18,8)        not null,
   ActQty               decimal(18,8)        not null,
   UnitQty              decimal(18,8)        not null,
   HuId                 varchar(50)          null,
   LocFrom              varchar(50)          null,
   EffDate              datetime             not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   Version              int                  not null,
   constraint PK_BIL_PLANBILL primary key (Id)
)
go

/*==============================================================*/
/* Table: INV_LocationLotDet                                    */
/*==============================================================*/
create table INV_LocationLotDet (
   Id                   int                  not null,
   Location             varchar(50)          not null,
   Bin                  varchar(50)          null,
   Item                 varchar(50)          not null,
   HuId                 varchar(50)          null,
   Qty                  decimal(18,8)        not null,
   IsCS                 bit                  not null,
   PlanBill             int                  null,
   QualityType          tinyint              not null,
   IsFreeze             bit                  not null,
   IsATP                bit                  not null,
   IsOccupy             bit                  not null,
   OccupyType           tinyint              null,
   OccupyRefNo          varchar(50)          null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   Version              int                  not null,
   constraint PK_INV_LOCATIONLOTDET primary key (Id)
)
go

/*==============================================================*/
/* Table: ORD_IpDet                                             */
/*==============================================================*/
create table ORD_IpDet (
   Id                   int                  identity,
   IpNo                 varchar(50)          not null,
   OrderDetId           int                  null,
   Item                 varchar(50)          not null,
   ItemDesc             varchar(100)         not null,
   RefItemCode          varchar(50)          null,
   Uom                  varchar(5)           not null,
   UC                   decimal(18,8)        not null,
   QualityType          tinyint              not null,
   Qty                  decimal(18,8)        not null,
   RecQty               decimal(18,8)        not null,
   UnitQty              decimal(18,8)        not null,
   LocFrom              varchar(50)          null,
   LocFromNm            varchar(100)         null,
   LocTo                varchar(50)          null,
   LocToNm              varchar(100)         null,
   IsInspect            bit                  not null,
   BillAddr             varchar(50)          null,
   PriceList            varchar(50)          null,
   UnitPrice            decimal(18,8)        null,
   Currency             varchar(50)          null,
   IsProvEst            bit                  not null,
   Tax                  varchar(50)          null,
   IsIncludeTax         bit                  not null,
   BillTerm             int                  not null,
   IsClose              bit                  not null,
   EffDate              datetime             null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   Version              int                  not null,
   constraint PK_ORD_IPDET primary key (Id)
)
go

/*==============================================================*/
/* Table: ORD_IpLocationDet                                     */
/*==============================================================*/
create table ORD_IpLocationDet (
   Id                   int                  not null,
   IpNo                 varchar(50)          not null,
   IpDetId              int                  not null,
   OrderDetId           int                  null,
   Item                 varchar(50)          not null,
   HuId                 varchar(50)          null,
   LotNo                varchar(50)          null,
   IsCS                 bit                  not null,
   PlanBill             int                  null,
   QualityType          tinyint              not null,
   IsFreeze             bit                  not null,
   IsATP                bit                  not null,
   Qty                  decimal(18,8)        not null,
   RecQty               decimal(18,8)        not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   Version              int                  not null,
   constraint PK_ORD_IPLOCATIONDET primary key (Id)
)
go

/*==============================================================*/
/* Table: ORD_IpMstr                                            */
/*==============================================================*/
create table ORD_IpMstr (
   IpNo                 varchar(50)          not null,
   ExtIpNo              varchar(50)          null,
   GapIpNo              varchar(50)          null,
   Type                 tinyint              not null,
   OrderType            tinyint              not null,
   QualityType          tinyint              not null,
   Status               tinyint              not null,
   DepartTime           datetime             not null,
   ArriveTime           datetime             not null,
   PartyFrom            varchar(50)          not null,
   PartyFromNm          varchar(100)         not null,
   PartyTo              varchar(50)          not null,
   PartyToNm            varchar(100)         not null,
   ShipFrom             varchar(50)          not null,
   ShipFromAddr         varchar(256)         not null,
   ShipFromTel          varchar(50)          null,
   ShipFromCell         varchar(50)          null,
   ShipFromFax          varchar(50)          null,
   ShipFromContact      varchar(50)          null,
   ShipTo               varchar(50)          not null,
   ShipToAddr           varchar(256)         not null,
   ShipToTel            varchar(50)          null,
   ShipToCell           varchar(50)          null,
   ShipToFax            varchar(50)          null,
   ShipToContact        varchar(50)          null,
   Dock                 varchar(100)         null,
   IsAutoReceive        bit                  not null,
   IsRecScanHu          bit                  not null,
   IsPrintAsn           bit                  not null,
   IsAsnPrinted         bit                  not null,
   IsPrintRec           bit                  not null,
   IsRecExceed          bit                  not null,
   IsRecFulfillUC       bit                  not null,
   IsRecFifo            bit                  not null,
   IsAsnAutoClose       bit                  not null,
   IsAsnUniqueRec       bit                  not null,
   IsRecCreateHu        bit                  not null,
   IsCheckPartyFromAuth bit                  not null,
   IsCheckPartyToAuth   bit                  not null,
   RecGapTo             tinyint              not null,
   AsnTemplate          varbinary(100)       null,
   RecTemplate          varbinary(100)       null,
   HuTemplate           varbinary(100)       null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   CloseDate            datetime             null,
   CloseUser            int                  null,
   CloseUserNm          varchar(100)         null,
   CloseReason          varchar(256)         null,
   Version              int                  not null,
   constraint PK_ORD_IPMSTR primary key (IpNo)
)
go

/*==============================================================*/
/* Table: ORD_IpRec                                             */
/*==============================================================*/
create table ORD_IpRec (
   Id                   int                  not null,
   IpDetId              int                  not null,
   RecDetId             int                  not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   constraint PK_ORD_IPREC primary key (Id)
)
go

/*==============================================================*/
/* Table: ORD_RecDet                                            */
/*==============================================================*/
create table ORD_RecDet (
   Id                   int                  not null,
   RecNo                varchar(50)          not null,
   OrderDetId           int                  null,
   Item                 varchar(50)          not null,
   ItemDesc             varchar(100)         not null,
   RefItemCode          varchar(50)          null,
   Uom                  varchar(5)           not null,
   UC                   decimal(18,8)        not null,
   QualityType          tinyint              not null,
   HuId                 varchar(50)          null,
   RecQty               decimal(18,8)        not null,
   RejQty               decimal(18,8)        not null,
   ScrapQty             decimal(18,8)        not null,
   UnitQty              decimal(18,8)        not null,
   LocFrom              varchar(50)          null,
   LocFromNm            varchar(100)         null,
   LocTo                varchar(50)          null,
   LocToNm              varchar(100)         null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   constraint PK_ORD_RECDET primary key (Id)
)
go

/*==============================================================*/
/* Table: ORD_RecMstr                                           */
/*==============================================================*/
create table ORD_RecMstr (
   RecNo                varchar(50)          not null,
   ExtRecNo             varchar(50)          null,
   IpNo                 varchar(50)          null,
   Type                 tinyint              not null,
   OrderType            tinyint              not null,
   QualityType          tinyint              not null,
   PartyFrom            varchar(50)          not null,
   PartyFromNm          varchar(100)         not null,
   PartyTo              varchar(50)          not null,
   PartyToNm            varchar(100)         not null,
   ShipFrom             varchar(50)          not null,
   ShipFromAddr         varchar(256)         not null,
   ShipFromTel          varchar(50)          null,
   ShipFromCell         varchar(50)          null,
   ShipFromFax          varchar(50)          null,
   ShipFromContact      varchar(50)          null,
   ShipTo               varchar(50)          not null,
   ShipToAddr           varchar(256)         not null,
   ShipToTel            varchar(50)          null,
   ShipToCell           varchar(50)          null,
   ShipToFax            varchar(50)          null,
   ShipToContact        varchar(50)          null,
   Dock                 varchar(100)         null,
   EffDate              datetime             not null,
   IsPrintRec           bit                  not null,
   IsRecPrinted         bit                  not null,
   IsCheckPartyFromAuth bit                  not null,
   IsCheckPartyToAuth   bit                  not null,
   RecTemplate          varbinary(100)       null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   constraint PK_ORD_RECMSTR primary key (RecNo)
)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_ORD_ORDE foreign key (OrderNo)
      references ORD_OrderMstr (OrderNo)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_ORD_IPMS foreign key (IpNo)
      references ORD_IpMstr (IpNo)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_ORD_RECM foreign key (RecNo)
      references ORD_RecMstr (RecNo)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_MD_ITEM foreign key (Item)
      references MD_Item (Code)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_MD_UOM foreign key (Uom)
      references MD_Uom (Code)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_MD_ADDRE foreign key (BillAddr)
      references MD_Address (Code)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_MD_PARTY foreign key (Party)
      references MD_Party (Code)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_BIL_PRIC foreign key (PriceList)
      references BIL_PriceListMstr (Code)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_MD_CURRE foreign key (Currency)
      references MD_Currency (Code)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_MD_LOCAT foreign key (LocFrom)
      references MD_Location (Code)
go

alter table BIL_ActBill
   add constraint FK_BIL_ACTB_REFERENCE_MD_TAX foreign key (Tax)
      references MD_Tax (Code)
go

alter table BIL_BillDet
   add constraint FK_BIL_BILL_REFERENCE_BIL_BILL foreign key (BillNo)
      references BIL_BillMstr (BillNo)
go

alter table BIL_BillDet
   add constraint FK_BIL_BILL_REFERENCE_BIL_ACTB foreign key (ActBill)
      references BIL_ActBill (Id)
go

alter table BIL_BillDet
   add constraint FK_BIL_BILL_REFERENCE_BIL_PRIC foreign key (PriceList)
      references BIL_PriceListMstr (Code)
go

alter table BIL_BillDet
   add constraint FK_BIL_BILL_REFERENCE_MD_ITEM foreign key (Item)
      references MD_Item (Code)
go

alter table BIL_BillDet
   add constraint FK_BIL_BILL_REFERENCE_MD_UOM foreign key (Uom)
      references MD_Uom (Code)
go

alter table BIL_BillDet
   add constraint FK_BIL_BILL_REFERENCE_ORD_ORDE foreign key (OrderNo)
      references ORD_OrderMstr (OrderNo)
go

alter table BIL_BillDet
   add constraint FK_BIL_BILL_REFERENCE_ORD_IPMS foreign key (IpNo)
      references ORD_IpMstr (IpNo)
go

alter table BIL_BillDet
   add constraint FK_BIL_BILL_REFERENCE_ORD_RECM foreign key (RecNo)
      references ORD_RecMstr (RecNo)
go

alter table BIL_BillMstr
   add constraint FK_BIL_BILL_REFERENCE_MD_ADDRE foreign key (BillAddr)
      references MD_Address (Code)
go

alter table BIL_BillMstr
   add constraint FK_BIL_BILL_REFERENCE_MD_PARTY foreign key (Party)
      references MD_Party (Code)
go

alter table BIL_BillMstr
   add constraint FK_BIL_BILL_REFERENCE_MD_CURRE foreign key (Currency)
      references MD_Currency (Code)
go

alter table BIL_BillMstr
   add constraint FK_BIL_BILL_REFERENCE_MD_TAX foreign key (Tax)
      references MD_Tax (Code)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_ORD_ORDE foreign key (OrderNo)
      references ORD_OrderMstr (OrderNo)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_ORD_IPMS foreign key (IpNo)
      references ORD_IpMstr (IpNo)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_ORD_RECM foreign key (RecNo)
      references ORD_RecMstr (RecNo)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_MD_ITEM foreign key (Item)
      references MD_Item (Code)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_MD_UOM foreign key (Uom)
      references MD_Uom (Code)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_MD_ADDRE foreign key (BillAddr)
      references MD_Address (Code)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_MD_PARTY foreign key (Party)
      references MD_Party (Code)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_BIL_PRIC foreign key (PriceList)
      references BIL_PriceListMstr (Code)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_MD_CURRE foreign key (Currency)
      references MD_Currency (Code)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_INV_HU foreign key (HuId)
      references INV_Hu (HuId)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_MD_LOCAT foreign key (LocFrom)
      references MD_Location (Code)
go

alter table BIL_PlanBill
   add constraint FK_BIL_PLAN_REFERENCE_MD_TAX foreign key (Tax)
      references MD_Tax (Code)
go

alter table INV_LocationLotDet
   add constraint FK_INV_LOCA_REFERENCE_BIL_PLAN foreign key (PlanBill)
      references BIL_PlanBill (Id)
go

alter table INV_LocationLotDet
   add constraint FK_INV_LOCA_REFERENCE_MD_LOCAT2 foreign key (Location)
      references MD_Location (Code)
go

alter table INV_LocationLotDet
   add constraint FK_INV_LOCA_REFERENCE_MD_LOCAT foreign key (Bin)
      references MD_LocationBin (Code)
go

alter table INV_LocationLotDet
   add constraint FK_INV_LOCA_REFERENCE_MD_ITEM foreign key (Item)
      references MD_Item (Code)
go

alter table INV_LocationLotDet
   add constraint FK_INV_LOCA_REFERENCE_INV_HU foreign key (HuId)
      references INV_Hu (HuId)
go

alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_ORD_IPMS foreign key (IpNo)
      references ORD_IpMstr (IpNo)
go

alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_MD_ITEM foreign key (Item)
      references MD_Item (Code)
go

alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_ORD_ORDE foreign key (OrderDetId)
      references ORD_OrderDet (Id)
go

alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_MD_UOM foreign key (Uom)
      references MD_Uom (Code)
go

alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_MD_LOCAT2 foreign key (LocFrom)
      references MD_Location (Code)
go

alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_MD_LOCAT3 foreign key (LocTo)
      references MD_Location (Code)
go

alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_MD_ADDRE foreign key (BillAddr)
      references MD_Address (Code)
go

alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_BIL_PRIC foreign key (PriceList)
      references BIL_PriceListMstr (Code)
go

alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_MD_CURRE foreign key (Currency)
      references MD_Currency (Code)
go

alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_MD_TAX foreign key (Tax)
      references MD_Tax (Code)
go

alter table ORD_IpLocationDet
   add constraint FK_ORD_IPLO_REFERENCE_INV_HU foreign key (HuId)
      references INV_Hu (HuId)
go

alter table ORD_IpLocationDet
   add constraint FK_ORD_IPLO_REFERENCE_ORD_IPDE foreign key (IpDetId)
      references ORD_IpDet (Id)
go

alter table ORD_IpLocationDet
   add constraint FK_ORD_IPLO_REFERENCE_ORD_IPMS foreign key (IpNo)
      references ORD_IpMstr (IpNo)
go

alter table ORD_IpLocationDet
   add constraint FK_ORD_IPLO_REFERENCE_ORD_ORDE foreign key (OrderDetId)
      references ORD_OrderDet (Id)
go

alter table ORD_IpLocationDet
   add constraint FK_ORD_IPLO_REFERENCE_MD_ITEM foreign key (Item)
      references MD_Item (Code)
go

alter table ORD_IpLocationDet
   add constraint FK_ORD_IPLO_REFERENCE_BIL_PLAN foreign key (PlanBill)
      references BIL_PlanBill (Id)
go

alter table ORD_IpMstr
   add constraint FK_ORD_IPMS_REFERENCE_MD_PARTY2 foreign key (PartyFrom)
      references MD_Party (Code)
go

alter table ORD_IpMstr
   add constraint FK_ORD_IPMS_REFERENCE_MD_PARTY foreign key (PartyTo)
      references MD_Party (Code)
go

alter table ORD_IpMstr
   add constraint FK_ORD_IPMS_REFERENCE_MD_ADDRE2 foreign key (ShipFrom)
      references MD_Address (Code)
go

alter table ORD_IpMstr
   add constraint FK_ORD_IPMS_REFERENCE_MD_ADDRE foreign key (ShipTo)
      references MD_Address (Code)
go

alter table ORD_IpMstr
   add constraint FK_ORD_IPMS_REFERENCE_ORD_IPMS foreign key (GapIpNo)
      references ORD_IpMstr (IpNo)
go

alter table ORD_IpRec
   add constraint FK_ORD_IPRE_REFERENCE_ORD_RECD foreign key (RecDetId)
      references ORD_RecDet (Id)
go

alter table ORD_IpRec
   add constraint FK_ORD_IPRE_REFERENCE_ORD_IPDE foreign key (IpDetId)
      references ORD_IpDet (Id)
go

alter table ORD_RecDet
   add constraint FK_ORD_RECD_REFERENCE_ORD_RECM foreign key (RecNo)
      references ORD_RecMstr (RecNo)
go

alter table ORD_RecDet
   add constraint FK_ORD_RECD_REFERENCE_ORD_ORDE foreign key (OrderDetId)
      references ORD_OrderDet (Id)
go

alter table ORD_RecDet
   add constraint FK_ORD_RECD_REFERENCE_MD_ITEM foreign key (Item)
      references MD_Item (Code)
go

alter table ORD_RecDet
   add constraint FK_ORD_RECD_REFERENCE_MD_UOM foreign key (Uom)
      references MD_Uom (Code)
go

alter table ORD_RecDet
   add constraint FK_ORD_RECD_REFERENCE_INV_HU foreign key (HuId)
      references INV_Hu (HuId)
go

alter table ORD_RecDet
   add constraint FK_ORD_RECD_REFERENCE_MD_LOCAT2 foreign key (LocFrom)
      references MD_Location (Code)
go

alter table ORD_RecDet
   add constraint FK_ORD_RECD_REFERENCE_MD_LOCAT foreign key (LocTo)
      references MD_Location (Code)
go

alter table ORD_RecMstr
   add constraint FK_ORD_RECM_REFERENCE_ORD_IPMS foreign key (IpNo)
      references ORD_IpMstr (IpNo)
go

alter table ORD_RecMstr
   add constraint FK_ORD_RECM_REFERENCE_MD_PARTY2 foreign key (PartyFrom)
      references MD_Party (Code)
go

alter table ORD_RecMstr
   add constraint FK_ORD_RECM_REFERENCE_MD_PARTY foreign key (PartyTo)
      references MD_Party (Code)
go

alter table ORD_RecMstr
   add constraint FK_ORD_RECM_REFERENCE_MD_ADDRE2 foreign key (ShipFrom)
      references MD_Address (Code)
go

alter table ORD_RecMstr
   add constraint FK_ORD_RECM_REFERENCE_MD_ADDRE foreign key (ShipTo)
      references MD_Address (Code)
go

--2011-12-30 dingxin
--发货明细表增加行号
alter table ORD_IpDet add Seq int not null;

--2011-12-30 dingxin
--发货单上怎加IsShipScanHu
alter table ord_ipmstr add IsShipScanHu bit not null;

--2011-12-30 dingxin
--条码字段更新
alter table INV_Hu alter column ManufactureDate datetime not null;
ALTER TABLE [dbo].[INV_Hu] DROP CONSTRAINT FK_INV_HU_REFERENCE_INV_LOTN
ALTER TABLE [dbo].[INV_Hu] DROP CONSTRAINT FK_INV_HU_REFERENCE_MD_LOCAT
ALTER TABLE [dbo].[INV_Hu] DROP CONSTRAINT FK_INV_HU_REFERENCE_MD_LOCAT2
ALTER TABLE [dbo].INV_StockTakeDet DROP CONSTRAINT FK_INV_STOC_REFERENCE_INV_LOTN2
ALTER TABLE [dbo].INV_StockTakeResult DROP CONSTRAINT FK_INV_STOC_REFERENCE_INV_LOTN
drop table INV_LotNo
ALTER TABLE [dbo].[INV_Hu] DROP Column Location
ALTER TABLE [dbo].[INV_Hu] DROP Column Bin
ALTER TABLE [dbo].[INV_Hu] DROP Column Status
ALTER TABLE [dbo].[INV_Hu] DROP Column Type
ALTER TABLE [dbo].[INV_Hu] DROP Column Version
alter table inv_hu add RemindExpireDate datetime
go

--2011-12-30 dingxin
--发货/收货单字段更新
alter table ord_ipdet add OrderNo varchar(50);
alter table ord_ipdet add OrderDetSeq int;
ALTER TABLE [dbo].[ORD_IpDet]  WITH CHECK ADD  CONSTRAINT [FK_ORD_IpDet_ORD_OrderMstr] FOREIGN KEY([OrderNo])
REFERENCES [dbo].[ORD_OrderMstr] ([OrderNo])
alter table ORD_RecDet add OrderNo varchar(50);
alter table ORD_RecDet add OrderDetSeq int;
alter table ORD_RecDet add Seq int not null;
ALTER TABLE [dbo].[ORD_RecDet]  WITH CHECK ADD  CONSTRAINT [FK_ORD_RecDet_ORD_OrderMstr] FOREIGN KEY([OrderNo])
REFERENCES [dbo].[ORD_OrderMstr] ([OrderNo])
alter table ORD_RecDet add IpNo varchar(50);
alter table ORD_RecDet add IpDetId int;
alter table ORD_RecDet add IpDetSeq int;
alter table ORD_RecDet add LotNo varchar(50);
ALTER TABLE [dbo].[ORD_RecDet]  WITH CHECK ADD  CONSTRAINT [FK_ORD_RecDet_ORD_IpMstr] FOREIGN KEY([IpNo])
REFERENCES [dbo].[ORD_IpMstr] ([IpNo])
ALTER TABLE [dbo].[ORD_RecDet]  WITH CHECK ADD  CONSTRAINT [FK_ORD_RecDet_ORD_IpDet] FOREIGN KEY([IpDetId])
REFERENCES [dbo].[ORD_IpDet] ([Id])
go

--2011-12-30 dingxin
--创建库存事务表
create table INV_LocTrans (
   Id                  bigint               identity,
   OrderNo              varchar(50)          null,
   OrderType            tinyint              null,
   OrderDetSeq          int                  null,
   OrderDetId           int                  null,
   OrderBomDetId        int                  null,
   IpNo                 varchar(50)          null,
   IpDetSeq             int                  null,
   RecNo                varchar(50)          null,
   RecDetSeq            int                  null,
   Item                 varchar(50)          not null,
   ItemDesc             varchar(100)         null,
   Uom                  varchar(5)           not null,
   Qty                  decimal(18,8)        not null,
   QualityType          tinyint              not null,
   HuId                 varchar(50)          null,
   LotNo                varchar(50)          null,
   TransType            tinyint              not null,
   PartyFrom            varchar(50)          null,
   PartyFromName        varchar(100)         null,
   PartyTo              varchar(50)          null,
   PartyToName          varchar(100)         null,
   LocFrom              varchar(50)          null,
   LocFromName          varchar(100)         null,
   LocTo                varchar(50)          null,
   LocToName            varchar(100)         null,
   LocIOReason          varchar(50)          null,
   LocIOReasonDesc      varchar(100)         null,
   EffDate              datetime             not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   constraint PK_INV_LOCTRANS primary key (Id)
)
go

--2012-1-1 dingxin
--新增视图
if exists (select 1
            from  sysobjects
           where  id = object_id('VIEW_LocationDet')
            and   type = 'V')
   drop view VIEW_LocationDet
go

/*==============================================================*/
/* View: VIEW_LocationDet                                       */
/*==============================================================*/
create view VIEW_LocationDet as
SELECT     Location, Item, SUM(CASE WHEN IsCS = 1 THEN Qty ELSE 0 END) AS CsQty, SUM(CASE WHEN IsCS = 0 THEN Qty ELSE 0 END) 
                      AS NmlQty, SUM(Qty) AS Qty, Count_Big(*) as CountBig
FROM         INV_LocationLotDet
WHERE     (Qty <> 0)
GROUP BY Location, Item
go

ALTER VIEW VIEW_LocationDet
with SCHEMABINDING 
AS
SELECT     Location, Item, SUM(CASE WHEN IsCS = 1 THEN Qty ELSE 0 END) AS CsQty, SUM(CASE WHEN IsCS = 0 THEN Qty ELSE 0 END) 
                      AS NmlQty, SUM(Qty) AS Qty, Count_Big(*) as CountBig
FROM         dbo.INV_LocationLotDet
WHERE     (Qty <> 0)
GROUP BY Location, Item
go


CREATE UNIQUE CLUSTERED INDEX IX_LocationDet ON VIEW_LocationDet
(
	Location ASC,
	Item ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


if exists (select 1
            from  sysobjects
           where  id = object_id('VIEW_OrderMstr')
            and   type = 'V')
   drop view VIEW_OrderMstr
go

/*==============================================================*/
/* View: VIEW_OrderMstr                                         */
/*==============================================================*/
create view VIEW_OrderMstr as
SELECT     OrderNo, Flow, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPause, PauseTime, IsPLPause, EffDate, Priority, 
                      Status, Seq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, ShipToAddr, 
                      ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, Currency, Dock, 
                      Routing, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, IsPrintRec, 
                      IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPickFromBin, IsPLCreate, IsShipFifo, 
                      IsRecFifo, IsShipByOrder, IsAsnUniqueRec, IsAsnAutoClose, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, 
                      HuTemplate, BillTerm, CreateHuOpt, ReCalculatePriceOpt, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version
FROM         ORD_OrderMstr
go

ALTER VIEW VIEW_OrderMstr
with SCHEMABINDING 
AS
SELECT     OrderNo, Flow, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPause, PauseTime, IsPLPause, EffDate, Priority, 
                      Status, Seq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, ShipToAddr, 
                      ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, Currency, Dock, 
                      Routing, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, IsPrintRec, 
                      IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPickFromBin, IsPLCreate, IsShipFifo, 
                      IsRecFifo, IsShipByOrder, IsAsnUniqueRec, IsAsnAutoClose, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, 
                      HuTemplate, BillTerm, CreateHuOpt, ReCalculatePriceOpt, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version
FROM         dbo.ORD_OrderMstr
GO

CREATE UNIQUE CLUSTERED INDEX IX_OrderMstr ON VIEW_OrderMstr 
(
	[OrderNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

--存储过程,先进先出,返回最先的条码
CREATE PROCEDURE [dbo].[usp_GetFiHuId] 
( 
@ItemCode varchar (50),
@UnitCount decimal,
@QualityType int, 
@ManuFactureParty varchar (50),
@HuId varchar (50)
) 
AS 
select top 1 @HuId = hudet.huid from locationlotdet l left join hudet on hudet.huid = l.huid
where hudet.item =@ItemCode and hudet.uc =@UnitCount and l.qty>0 and l.QualityType=@QualityType
and l.ManuFactureParty =@ManuFactureParty
order by hudet.manufacturedate
GO

--2012-1-5 dingxin
--条码上增加字段
alter table INV_Hu add ManufactureParty varchar(50);
go
update INV_Hu set ManufactureParty = (select top 1 Code from MD_Party)
go
alter table INV_Hu alter column ManufactureParty varchar(50) not null;
go
alter table INV_Hu
   add constraint FK_INV_HU_REFERENCE_MD_PARTY foreign key (ManufactureParty)
      references MD_Party (Code)
go
alter table INV_Hu add ItemRefCode varchar(50);
go


--2012-1-8 dingxin
--订单上增加制造商字段
alter table ORD_OrderDet add ManufactureParty varchar(50);
alter table ORD_IpDet add ManufactureParty varchar(50);

go
alter table ORD_OrderDet
   add constraint FK_ORD_ORDE_REFERENCE_MD_PARTY0 foreign key (ManufactureParty)
      references MD_Party (Code)
go      
alter table ORD_IpDet
   add constraint FK_ORD_IPDE_REFERENCE_MD_PARTY foreign key (ManufactureParty)
      references MD_Party (Code)
go      

--2012-1-8 dingxin
--PlanBill和ActBill增加是否关闭字段
alter table BIL_PlanBill add IsClose bit not null;
alter table BIL_ActBill add IsClose bit not null;

--2012-1-8 dingxin
--库存明细的占用字段不允许为空
update INV_LocationLotDet set OccupyType = 0
alter table INV_LocationLotDet alter column OccupyType tinyint not null;



--2012-1-9 dingxin
--重新生成拣货单相关表
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_PickListDet') and o.name = 'FK_ORD_PICK_REFERENCE_ORD_PICK2')
alter table ORD_PickListDet
   drop constraint FK_ORD_PICK_REFERENCE_ORD_PICK2
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_PickListDet') and o.name = 'FK_ORD_PICK_REFERENCE_MD_PARTY')
alter table ORD_PickListDet
   drop constraint FK_ORD_PICK_REFERENCE_MD_PARTY
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_PickListResult') and o.name = 'FK_ORD_PICK_REFERENCE_ORD_PICK3')
alter table ORD_PickListResult
   drop constraint FK_ORD_PICK_REFERENCE_ORD_PICK3
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ORD_PickListResult') and o.name = 'FK_ORD_PICK_REFERENCE_ORD_PICK')
alter table ORD_PickListResult
   drop constraint FK_ORD_PICK_REFERENCE_ORD_PICK
go

alter table ORD_PickListDet
   drop constraint PK_ORD_PICKLISTDET
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ORD_PickListDet')
            and   type = 'U')
   drop table ORD_PickListDet
go

alter table ORD_PickListMstr
   drop constraint PK_ORD_PICKLISTMSTR
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ORD_PickListMstr')
            and   type = 'U')
   drop table ORD_PickListMstr
go

alter table ORD_PickListResult
   drop constraint PK_ORD_PICKLISTRESULT
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ORD_PickListResult')
            and   type = 'U')
   drop table ORD_PickListResult
go

/*==============================================================*/
/* Table: ORD_PickListDet                                       */
/*==============================================================*/
create table ORD_PickListDet (
   Id                   int                  identity,
   PLNo                 varchar(50)          not null,
   OrderNo              varchar(50)          not null,
   OrderDet             int                  not null,
   Item                 varchar(50)          not null,
   ItemDesc             varchar(100)         not null,
   RefItemCode          varchar(50)          null,
   ManufactureParty     varchar(50)          null,
   QualityStatus        tinyint              not null,
   Uom                  varchar(5)           not null,
   UC                   decimal(18,8)        not null,
   LocFrom              varchar(50)          null,
   LocFromNm            varchar(100)         null,
   Area                 varchar(50)          null,
   Bin                  varchar(50)          null,
   Qty                  decimal(18,8)        not null,
   PickedQty            decimal(18,8)        not null,
   HuId                 varchar(50)          null,
   LotNo                varchar(50)          null,
   LocTo                varchar(50)          null,
   LocToNm              varchar(100)         null,
   IsInspect            bit                  not null,
   Memo                 varchar(256)         null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   Version              int                  not null
)
go

alter table ORD_PickListDet
   add constraint PK_ORD_PICKLISTDET primary key (Id)
go

/*==============================================================*/
/* Table: ORD_PickListMstr                                      */
/*==============================================================*/
create table ORD_PickListMstr (
   PLNo                 varchar(50)          not null,
   Status               varchar(50)          not null,
   OrderType            tinyint              not null,
   StartTime            datetime             not null,
   WinTime              datetime             not null,
   PartyFrom            varchar(50)          not null,
   PartyFromNm          varchar(100)         not null,
   PartyTo              varchar(50)          not null,
   PartyToNm            varchar(100)         not null,
   ShipFrom             varchar(50)          not null,
   ShipFromAddr         varchar(256)         not null,
   ShipFromTel          varchar(50)          null,
   ShipFromCell         varchar(50)          null,
   ShipFromFax          varchar(50)          null,
   ShipFromContact      varchar(50)          null,
   ShipTo               varchar(50)          not null,
   ShipToAddr           varchar(256)         not null,
   ShipToTel            varchar(50)          null,
   ShipToCell           varchar(50)          null,
   ShipToFax            varchar(50)          null,
   ShipToContact        varchar(50)          null,
   Dock                 varchar(100)         null,
   IsAutoReceive        bit                  not null,
   IsRecScan            bit                  not null,
   IsPrintAsn           bit                  not null,
   IsPrintRec           bit                  not null,
   IsRecExceed          bit                  not null,
   IsRecFulfillUC       bit                  not null,
   IsRecFifo            bit                  not null,
   IsAsnAuotClose       bit                  not null,
   IsAsnUniqueRec       bit                  not null,
   IsCheckPartyFromAuth bit                  not null,
   IsCheckPartyToAuth   bit                  not null,
   CreateHuOpt          tinyint              not null,
   RecGapTo             tinyint              not null,
   AsnTemplate          varbinary(100)       null,
   RecTemplate          varbinary(100)       null,
   HuTemplate           varbinary(100)       null,
   EffDate              datetime             not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null,
   LastModifyUser       int                  not null,
   LastModifyUserNm     varchar(100)         not null,
   LastModifyDate       datetime             not null,
   StartDate            datetime             null,
   StartUser            int                  null,
   StartUserNm          varchar(100)         null,
   CompleteDate         datetime             null,
   CompleteUser         int                  null,
   CompleteUserNm       varchar(100)         null,
   CloseDate            datetime             null,
   CloseUser            int                  null,
   CloseUserNm          varchar(100)         null,
   CancelDate           datetime             null,
   CancelUser           int                  null,
   CancelUserNm         varchar(100)         null,
   CancelReason         varchar(256)         null,
   Version              int                  not null
)
go

alter table ORD_PickListMstr
   add constraint PK_ORD_PICKLISTMSTR primary key (PLNo)
go

/*==============================================================*/
/* Table: ORD_PickListResult                                    */
/*==============================================================*/
create table ORD_PickListResult (
   Id                   int                  identity,
   PLNo                 varchar(50)          not null,
   PLDet                int                  not null,
   Item                 varchar(50)          not null,
   ItemDesc             varchar(100)         not null,
   RefItemCode          varchar(50)          null,
   Uom                  varchar(5)           not null,
   UC                   decimal(18,8)        not null,
   HuId                 varchar(50)          null,
   LotNo                varchar(50)          null,
   IsCS                 bit                  not null,
   PlanBill             int                  null,
   QualityStatus        tinyint              not null,
   Qty                  decimal(18,8)        not null,
   CreateUser           int                  not null,
   CreateUserNm         varchar(100)         not null,
   CreateDate           datetime             not null
)
go

alter table ORD_PickListResult
   add constraint PK_ORD_PICKLISTRESULT primary key (Id)
go

alter table ORD_PickListDet
   add constraint FK_ORD_PICK_REFERENCE_ORD_PICK2 foreign key (PLNo)
      references ORD_PickListMstr (PLNo)
go

alter table ORD_PickListDet
   add constraint FK_ORD_PICK_REFERENCE_MD_PARTY foreign key (ManufactureParty)
      references MD_Party (Code)
go

alter table ORD_PickListResult
   add constraint FK_ORD_PICK_REFERENCE_ORD_PICK3 foreign key (PLDet)
      references ORD_PickListDet (Id)
go

alter table ORD_PickListResult
   add constraint FK_ORD_PICK_REFERENCE_ORD_PICK foreign key (PLNo)
      references ORD_PickListMstr (PLNo)
go


--2012-1-10 dingxin
--INV_LocTrans增加UnitQty
alter table INV_LocTrans add UnitQty decimal(18,8) not null;


ALTER TABLE INV_StockTakedet ADD IsCS bit;

ALTER TABLE INV_StockTakeresult ADD IsCS bit;

ALTER TABLE INV_StockTakeInv ADD IsCS bit;

ALTER TABLE INV_StockTakeLoc ADD Bins varchar(500);

ALTER TABLE INV_StockTakeMstr ADD Remark varchar(50);