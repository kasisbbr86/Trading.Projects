USE [ShippingTrade]
GO

/****** Object:  UserDefinedTableType [dbo].[IncomingCourierDetailsTableType]    Script Date: 28-12-2019 10:05:56 ******/
DROP TYPE [dbo].[IncomingCourierDetailsTableType]
GO

/****** Object:  UserDefinedTableType [dbo].[IncomingCourierDetailsTableType]    Script Date: 28-12-2019 10:05:56 ******/
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


