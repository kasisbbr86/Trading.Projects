USE [ShippingTrade]
GO
/****** Object:  UserDefinedTableType [dbo].[DocumentInstructionTableType]    Script Date: 29-12-2019 22:37:45 ******/
CREATE TYPE [dbo].[DocumentInstructionTableType] AS TABLE(
	[Instruction] [nvarchar](250) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[IncomingCourierDetailsTableType]    Script Date: 29-12-2019 22:37:45 ******/
CREATE TYPE [dbo].[IncomingCourierDetailsTableType] AS TABLE(
	[CompanyID] [uniqueidentifier] NULL,
	[DetailsID] [int] NULL,
	[MasterID] [int] NULL,
	[SINo] [nvarchar](50) NULL,
	[Date] [datetime] NULL,
	[DocDetail] [nvarchar](max) NULL,
	[RefNo] [nvarchar](max) NULL,
	[Qty] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](max) NULL,
	[FileName] [nvarchar](max) NULL,
	[FilePath] [nvarchar](max) NULL,
	[ParentDetailsID] [int] NULL,
	[ArraySubItem] [nvarchar](max) NULL,
	[IsSubDetail] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreationDate] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[ShippingModelTableType]    Script Date: 29-12-2019 22:37:45 ******/
CREATE TYPE [dbo].[ShippingModelTableType] AS TABLE(
	[PONo] [nvarchar](100) NULL,
	[ModelName] [nvarchar](50) NULL,
	[Version] [nvarchar](10) NULL,
	[Quantity] [nvarchar](10) NULL,
	[BLModelName] [nvarchar](100) NULL,
	[Description] [nvarchar](250) NULL,
	[Remarks] [nvarchar](250) NULL
)
GO
