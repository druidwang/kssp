
 
/*==============================================================*/

/* Table: ISS_IssueAddr */

/*==============================================================*/

create table ISS_IssueAddr (

Code varchar(50) not null,

Desc1 varchar(100) not null,

ParentCode varchar(50) null,

Seq int not null,

CreateUser int not null,

CreateUserNm varchar(100) not null,

CreateDate datetime not null,

LastModifyUser int not null,

LastModifyUserNm varchar(100) not null,

LastModifyDate datetime not null,

constraint PK_ISS_ISSUEADDR primary key (Code)

)

go


/*==============================================================*/

/* Table: ISS_IssueDet */

/*==============================================================*/

create table ISS_IssueDet (

Id int IDENTITY(1,1)  not null,

IssueCode varchar(50) not null,

IssueTypeToUserDetId int null,

IssueTypeToRoleDetId int null,

Seq int not null,

IssueLevel varchar(50) not null,

IsSubmit bit not null,

IsInProcess bit not null,

IsDefault bit not null,

IsEmail bit not null,

IsSMS bit not null,

Priority tinyint not null,

UserId int not null,

Email varchar(50) null,

EmailStatus tinyint not null,

EmailCount int not null,

MobilePhone varchar(50) null,

SMSStatus tinyint not null,

SMSCount int not null,

IsActive bit not null,

CreateUser int not null,

CreateUserNm varchar(100) not null,

CreateDate datetime not null,

LastModifyUser int not null,

LastModifyUserNm varchar(100) not null,

LastModifyDate datetime not null,

constraint PK_ISS_ISSUEDET primary key (Id)

)

go


/*==============================================================*/

/* Table: ISS_IssueLevel */

/*==============================================================*/

create table ISS_IssueLevel (

Code varchar(50) not null,

Desc1 varchar(100) not null,

IsActive bit not null,

IsSubmit bit not null,

IsInProcess bit not null,

Seq int not null,

IsDefault bit not null,

CreateUser int not null,

CreateUserNm varchar(100) not null,

CreateDate datetime not null,

LastModifyUser int not null,

LastModifyUserNm varchar(100) not null,

LastModifyDate datetime not null,

constraint PK_ISS_ISSUELEVEL primary key (Code)

)

go


/*==============================================================*/

/* Table: ISS_IssueMstr */

/*==============================================================*/

create table ISS_IssueMstr (

Code varchar(50) not null,

IssueAddr varchar(50) not null,

IssueType varchar(50) not null,

[Type] tinyint not null,

IssueNo varchar(50) null,

IssueSubject varchar(50) null,

BackYards varchar(50) null,

Content varchar(100) null,

Solution varchar(256) null,

Status tinyint not null,

Priority tinyint not null,

FinishedUser varchar(50) null,

FinishedDate datetime null,

UserName varchar(100) null,

Email varchar(50) null,

MPhone varchar(50) null,

CreateUser int not null,

CreateUserNm varchar(100) not null,

CreateDate datetime not null,

LastModifyUser int not null,

LastModifyUserNm varchar(100) not null,

LastModifyDate datetime not null,

ReleaseDate datetime null,

ReleaseUser int null,

ReleaseUserNm varchar(100) null,

StartDate datetime null,

StartUser int null,

StartUserNm varchar(100) null,

CloseDate datetime null,

CloseUser int null,

CloseUserNm varchar(100) null,

CancelDate datetime null,

CancelUser int null,

CancelUserNm varchar(100) null,

CompleteDate datetime null,

CompleteUser int null,

CompleteUserNm varchar(100) null,

constraint PK_ISS_ISSUEMSTR primary key (Code)

)

go


/*==============================================================*/

/* Table: ISS_IssueType */

/*==============================================================*/

create table ISS_IssueType (

Code varchar(50) not null,

Desc1 varchar(100) not null,

InProcessWaitingTime decimal(18,8) null,

CompleteWaitingTime decimal(18,8) null,

IsActive bit not null,

CreateUser int not null,

CreateUserNm varchar(100) not null,

CreateDate datetime not null,

LastModifyUser int not null,

LastModifyUserNm varchar(100) not null,

LastModifyDate datetime not null,

constraint PK_ISS_ISSUETYPE primary key (Code)

)

go


/*==============================================================*/

/* Table: ISS_IssueTypeToMstr */

/*==============================================================*/

create table ISS_IssueTypeToMstr (

Code varchar(50) not null,

Desc1 varchar(100) not null,

IsActive bit not null,

IssueType varchar(50) not null,

IssueLevel varchar(50) not null,

CreateUser int not null,

CreateUserNm varchar(100) not null,

CreateDate datetime not null,

LastModifyUser int not null,

LastModifyUserNm varchar(100) not null,

LastModifyDate datetime not null,

constraint PK_ISS_ISSUETYPETOMSTR primary key (Code)

)

go


/*==============================================================*/

/* Table: ISS_IssueTypeToRoleDet */

/*==============================================================*/

create table ISS_IssueTypeToRoleDet (

Id int IDENTITY(1,1)  not null,

IssueTypeTo varchar(50) not null,

RoleId int not null,

IsEmail bit not null,

IsSMS bit not null,

CreateUser int not null,

CreateUserNm varchar(100) not null,

CreateDate datetime not null,

LastModifyUser int not null,

LastModifyUserNm varchar(100) not null,

LastModifyDate datetime not null,

constraint PK_ISS_ISSUETYPETOROLEDET primary key (Id)

)

go


/*==============================================================*/

/* Table: ISS_IssueTypeToUserDet */

/*==============================================================*/

create table ISS_IssueTypeToUserDet (

Id int IDENTITY(1,1) NOT NULL,

IssueTypeTo varchar(50) not null,

UserId int not null,

IsEmail bit not null,

IsSMS bit not null,

CreateUser int not null,

CreateUserNm varchar(100) not null,

CreateDate datetime not null,

LastModifyUser int not null,

LastModifyUserNm varchar(100) not null,

LastModifyDate datetime not null,

constraint PK_ISS_ISSUETYPETOUSERDET primary key (Id)

)

go



/*==============================================================*/
 
/* Table: ISS_IssueNo                                           */
 
/*==============================================================*/
 
create table ISS_IssueNo(
 
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


/*==============================================================*/
 
/* Table: ISS_SMSStatus                                         */
 
/*==============================================================*/
 
CREATE TABLE [ISS_SMSStatus](
	[Id] [int] identity,
	[Issue] [varchar](50) NULL,
	[IssueDet] [int] NULL,
	[IssueLevel] [varchar](50) NULL,
	[MsgID] [varchar](255) NULL,
	[SeqID] [int] NULL,
	[SrcID] [varchar](255) NULL,
	[DestID] [varchar](255) NULL,
	[ServiceID] [varchar](255) NULL,
	[SrcTerminalId] [varchar](255) NULL,
	[SrcTerminalType] [int] NULL,
	[MsgFmt] [int] NULL,
	[MsgLength] [int] NULL,
	[Status] [varchar](255) NULL,
	[Content_] [varchar](255) NULL,
	[DoneDatetime] [datetime] NULL,
	[SubmitDatetime] [datetime] NULL,
	[EventHandler] [varchar](50) NULL,
	[CreateUser] int null,
	[CreateUserNm] varchar(100)  null,
	[CreateDate] datetime not null,
	[LastModifyUser] int  null,
	[LastModifyUserNm] varchar(100)  null,
	[LastModifyDate] datetime not null,
	constraint PK_ISS_SMSStatus primary key (Id)
)
go

/*==============================================================*/
 
/* Table: ISS_IssueLog                                          */
 
/*==============================================================*/
 
CREATE TABLE [ISS_IssueLog](
	[Id] [int] identity,
	[Issue] [varchar](50) NULL,
	[IssueDet] [int] NULL,
	[Level_] [varchar](50) NULL,
	[Content_] [varchar](255) NULL,
	[UserId] [varchar](50) NULL,
	[EmailStatus] [varchar](50) NULL,
	[SMSStatus] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[MPhone] [varchar](50) NULL,
	[IsEmail] [bit] NULL,
	[IsSMS] [bit] NULL,
	[CreateUser] int not null,
	[CreateUserNm] varchar(100) not null,
	[CreateDate] datetime not null,
	[LastModifyUser] int not null,
	[LastModifyUserNm] varchar(100) not null,
	[LastModifyDate] datetime not null,
	constraint PK_ISS_IssueLog primary key (Id)
)
go


alter table ISS_IssueAddr

add constraint FK_ISS_ISSU_REFERENCE_ISS_ISSU2 foreign key (ParentCode)

references ISS_IssueAddr (Code)

go


alter table ISS_IssueDet

add constraint FK_ISS_ISSU_REFERENCE_ISS_ISSU9 foreign key (IssueCode)

references ISS_IssueMstr (Code)

go


alter table ISS_IssueDet

add constraint FK_ISS_ISSU_REFERENCE_ISS_ISSU foreign key (IssueLevel)

references ISS_IssueLevel (Code)

go


alter table ISS_IssueMstr

add constraint FK_ISS_ISSU_REFERENCE_ISS_ISSU8 foreign key (IssueType)

references ISS_IssueType (Code)

go


alter table ISS_IssueTypeToMstr

add constraint FK_ISS_ISSU_REFERENCE_ISS_ISSU3 foreign key (IssueType)

references ISS_IssueType (Code)

go


alter table ISS_IssueTypeToMstr

add constraint FK_ISS_ISSU_REFERENCE_ISS_ISSU4 foreign key (IssueLevel)

references ISS_IssueLevel (Code)

go


alter table ISS_IssueTypeToRoleDet

add constraint FK_ISS_ISSU_REFERENCE_ISS_ISSU6 foreign key (IssueTypeTo)

references ISS_IssueTypeToMstr (Code)

go


alter table ISS_IssueTypeToRoleDet

add constraint FK_ISS_ISSU_REFERENCE_ACC_ROLE foreign key (RoleId)

references ACC_Role (Id)

go


alter table ISS_IssueTypeToUserDet

add constraint FK_ISS_ISSU_REFERENCE_ISS_ISSU5 foreign key (IssueTypeTo)

references ISS_IssueTypeToMstr (Code)

go


alter table ISS_IssueTypeToUserDet

add constraint FK_ISS_ISSU_REFERENCE_ACC_USER foreign key (UserId)

references ACC_User (Id)

go


 
alter table ISS_IssueNo
 
   add constraint FK_ISS_ISSU_REFERENCE_ISS_ISSU10 foreign key (IssueType)
 
      references ISS_IssueType (Code)
 
go
