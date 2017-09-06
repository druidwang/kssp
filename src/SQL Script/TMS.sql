


/****** Object:  Table [dbo].[TMS_ADDRESS]    Script Date: 01/14/2015 14:09:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TMS_Address](
	[Code] [varchar](50) NOT NULL,
	[Country] [varchar](50) NOT NULL,
	[Province] [varchar](50) NOT NULL,
	[City] [varchar](50) NOT NULL,
	[District] [varchar](50) NOT NULL,
	[Street] [varchar](50) NOT NULL,
	[Address] [varchar] (255) NOT NULL,
	[Postcode] [varchar](50)  NULL,
	[Contacts] [varchar] NULL,
	[Phone] [varchar](50) NULL,
	[MobilePhone] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Remark] [varchar](255) NULL,
	[CreateUserNm] [varchar](50) NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUserNm] [varchar](50) NULL,
	[LastModifyUser][int] NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	
 CONSTRAINT [PK_TMS_ADDRESS] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




/****** Object:  Table [dbo].[MD_Carrier]    Script Date: 2015/6/5 13:30:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[MD_Carrier](
	[Code] [varchar](50) NOT NULL,
 CONSTRAINT [PK_MD_CARRIER] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[MD_Carrier]  WITH NOCHECK ADD  CONSTRAINT [FK_MD_CARRIER_REFERENCE_MD_PARTY] FOREIGN KEY([Code])
REFERENCES [dbo].[MD_Party] ([Code])
GO

ALTER TABLE [dbo].[MD_Carrier] CHECK CONSTRAINT [FK_MD_CARRIER_REFERENCE_MD_PARTY]
GO




/****** Object:  Table [dbo].[TMS_DRIVER]    Script Date: 01/14/2015 14:09:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TMS_DRIVER](
	[Code] [varchar](50) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Fax]  [varchar](50) NULL,
	[Address] [varchar] (255)  NULL,
	[PostCode] [varchar](50)  NULL,
	[Contacts] [varchar] NULL,
	[Phone] [varchar](50) NULL,
	[MobilePhone] [varchar](50) NOT NULL,
	[IdentityCard] [varchar](50) NOT NULL,
	[Email] [varchar](50) NULL,
	[CreateUserNm] [varchar](50) NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUserNm] [varchar](50) NULL,
	[LastModifyUser][int] NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	
 CONSTRAINT [PK_TMS_DRIVER] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



/****** Object:  Table [dbo].[TMS_Vehicle]    Script Date: 01/14/2015 14:09:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TMS_Vehicle](
	[Code] [varchar](50) NOT NULL,
	[Desc1] [varchar](50) NOT NULL,
	[Driver] [varchar](50) NULL,
	[DrivingNo] [varchar](50)  NULL,
	[Owner] [varchar](50) NOT  NULL,
	[MobilePhone] [varchar](50)  NULL,
	[Phone] [varchar](50) NULL,
	[VIN] [varchar] NULL,
	[EngineNo] [varchar] NULL,
	[Address] [varchar] (255)  NULL,
	[Fax]  [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Tonnage] [varchar](50)  NULL,
	[CreateUserNm] [varchar](50) NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUserNm] [varchar](50) NULL,
	[LastModifyUser][int] NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	
 CONSTRAINT [PK_TMS_Vehicle] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



----�˵�

insert into ACC_PermissionCategory values('TMS','�������',0,120)
insert into ACC_PermissionCategory values('Carrier','������',8,20)
insert into ACC_Permission (Code,Desc1,Category) values('Url_TransportAddress_Edit','����ص�༭','TMS')
insert into ACC_Permission (Code,Desc1,Category) values('Url_TransportAddress_View','����ص�鿴','TMS')
insert into ACC_Permission (Code,Desc1,Category) values('Url_Carrier_Edit','�����̱༭','MasterData')
insert into ACC_Permission (Code,Desc1,Category) values('Url_Carrier_View','�����̲鿴','MasterData')
insert into ACC_Permission (Code,Desc1,Category) values('Url_Driver_Edit','��ʻԱ�༭','TMS')
insert into ACC_Permission (Code,Desc1,Category) values('Url_Driver_View','��ʻԱ�鿴','TMS')
insert into ACC_Permission (Code,Desc1,Category) values('Url_Vehicle_Edit','���乤�߱༭','TMS')
insert into ACC_Permission (Code,Desc1,Category) values('Url_Vehicle_View','���乤�߲鿴','TMS')


insert into sys_menu values('Menu.TMS','�������',null,2000,'�������',null,'~/Content/Images/Nav/Default.png',1)
insert into sys_menu values('Menu.TMS.Trans','����','Menu.TMS',100,'����',null,'~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Menu.TMS.Info','��Ϣ','Menu.TMS',200,'��Ϣ',null,'~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Menu.TMS.Setup','����','Menu.TMS',300,'����',null,'~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Url_TransportAddress_View','����ص�','Menu.TMS.Setup',100,'����ص�','~/TransportAddress/Index','~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Url_Carrier_View','������','Menu.TMS.Setup',200,'������','~/Carrier/Index','~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Url_Driver_View','��ʻԱ','Menu.TMS.Setup',300,'��ʻԱ','~/Driver/Index','~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Url_Vehicle_View','���乤��','Menu.TMS.Setup',400,'���乤��','~/Vehicle/Index','~/Content/Images/Nav/Trans.png',1)








