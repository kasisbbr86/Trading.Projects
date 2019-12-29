USE [ShippingTrade]
GO
/****** Object:  Table [dbo].[DocumentInstruction]    Script Date: 29-12-2019 22:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentInstruction](
	[ShippingId] [int] NOT NULL,
	[Instruction] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_DocumentInstruction] PRIMARY KEY CLUSTERED 
(
	[ShippingId] ASC,
	[Instruction] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IncomingCourierDetails]    Script Date: 29-12-2019 22:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IncomingCourierDetails](
	[CompanyID] [uniqueidentifier] NOT NULL,
	[DetailsID] [int] IDENTITY(1,1) NOT NULL,
	[MasterID] [int] NOT NULL,
	[SINo] [nvarchar](50) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[DocDetail] [nvarchar](max) NOT NULL,
	[RefNo] [nvarchar](max) NOT NULL,
	[Qty] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](max) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[FilePath] [nvarchar](max) NOT NULL,
	[ParentDetailsID] [int] NOT NULL,
	[ArraySubItem] [nvarchar](max) NOT NULL,
	[IsSubDetail] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_IncomingCourierDetails] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[DetailsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IncomingCourierMaster]    Script Date: 29-12-2019 22:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IncomingCourierMaster](
	[CompanyID] [uniqueidentifier] NOT NULL,
	[MasterID] [int] IDENTITY(1,1) NOT NULL,
	[AWBNo] [nvarchar](max) NOT NULL,
	[CourierCompany] [int] NOT NULL,
	[ReceivedFrom] [int] NOT NULL,
	[CourierFor] [int] NOT NULL,
	[DocumentType] [int] NOT NULL,
	[ReceivedOn] [datetime2](7) NOT NULL,
	[HandedOverOn] [datetime2](7) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_IncomingCourierMaster] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[MasterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Shipping]    Script Date: 29-12-2019 22:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shipping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TradeSheetName] [nvarchar](50) NOT NULL,
	[SIDate] [nvarchar](20) NULL,
	[SINo] [nvarchar](20) NULL,
	[Vender] [nvarchar](20) NULL,
	[SoldToParty] [nvarchar](20) NULL,
	[ShipToParty] [nvarchar](20) NULL,
	[BLConsignee] [nvarchar](50) NULL,
	[PortOfDischarge] [nvarchar](50) NULL,
	[FinalDestination] [nvarchar](50) NULL,
	[Via] [nvarchar](50) NULL,
	[Transportation] [nvarchar](50) NULL,
	[PortOfLoading] [nvarchar](50) NULL,
	[TradeTerms] [nvarchar](50) NULL,
	[PaymentTerms] [nvarchar](50) NULL,
	[LCNo] [nvarchar](20) NULL,
	[LCIssuanceDate] [nvarchar](20) NULL,
	[LCIssuingBank] [nvarchar](20) NULL,
	[LCExpiryDate] [nvarchar](20) NULL,
	[ShipmentExpiryDate] [nvarchar](20) NULL,
	[RequiredBLDate] [nvarchar](20) NULL,
	[Freight] [nvarchar](50) NULL,
	[PartialShipment] [nvarchar](50) NULL,
	[TransShipment] [nvarchar](50) NULL,
 CONSTRAINT [PK_Shipping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingAdvice]    Script Date: 29-12-2019 22:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingAdvice](
	[CompanyID] [uniqueidentifier] NOT NULL,
	[ShippingAdviceId] [int] IDENTITY(1,1) NOT NULL,
	[SCInvoiceNo] [nvarchar](max) NOT NULL,
	[InvoiceAmount] [decimal](18, 2) NOT NULL,
	[Consignee] [nvarchar](max) NOT NULL,
	[BLDate] [datetime] NOT NULL,
	[ReceivedDate] [datetime] NOT NULL,
	[Shiper] [nvarchar](max) NOT NULL,
	[BLNo] [nvarchar](max) NOT NULL,
	[Factory] [nvarchar](max) NOT NULL,
	[Department] [nvarchar](max) NOT NULL,
	[Material] [nvarchar](max) NOT NULL,
	[Quantity] [int] NOT NULL,
	[FOB] [decimal](18, 2) NOT NULL,
	[PurchaseDocumentNo] [int] NOT NULL,
	[Item1] [int] NOT NULL,
	[SAPSO] [nvarchar](max) NOT NULL,
	[Item2] [int] NOT NULL,
	[SAPDO] [nvarchar](max) NOT NULL,
	[PInt] [int] NOT NULL,
	[SLoc] [nvarchar](max) NOT NULL,
	[Temp1] [nchar](1) NOT NULL,
	[Seq] [int] NOT NULL,
	[Del] [nvarchar](max) NOT NULL,
	[Comp] [nchar](1) NOT NULL,
	[DeliveryDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ShippingAdvice] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[ShippingAdviceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingModel]    Script Date: 29-12-2019 22:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingModel](
	[ShippingId] [int] NOT NULL,
	[PONo] [nvarchar](100) NOT NULL,
	[ModelName] [nvarchar](50) NULL,
	[Version] [nvarchar](10) NULL,
	[Quantity] [nvarchar](10) NULL,
	[BLModelName] [nvarchar](100) NULL,
	[Description] [nvarchar](250) NULL,
	[Remarks] [nvarchar](250) NULL,
 CONSTRAINT [PK_ShippingModel] PRIMARY KEY CLUSTERED 
(
	[ShippingId] ASC,
	[PONo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TradeImportLog]    Script Date: 29-12-2019 22:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TradeImportLog](
	[ShippingId] [int] NOT NULL,
	[WorkBookName] [nvarchar](250) NOT NULL,
	[TradeRequest] [nvarchar](max) NULL,
	[ImportDate] [datetime] NULL,
	[ImportStatus] [nvarchar](20) NULL,
	[ExceptionMessage] [nvarchar](1000) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[IncomingCourierDetails] ADD  CONSTRAINT [DF_IncomingCourierDetails_ArraySubItem]  DEFAULT ('') FOR [ArraySubItem]
GO
ALTER TABLE [dbo].[IncomingCourierDetails]  WITH CHECK ADD  CONSTRAINT [FK_IncomingCourierDetails_IncomingCourierMaster] FOREIGN KEY([CompanyID], [MasterID])
REFERENCES [dbo].[IncomingCourierMaster] ([CompanyID], [MasterID])
GO
ALTER TABLE [dbo].[IncomingCourierDetails] CHECK CONSTRAINT [FK_IncomingCourierDetails_IncomingCourierMaster]
GO
