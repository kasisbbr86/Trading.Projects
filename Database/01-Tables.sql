/****** Object:  Table [dbo].[Shipping]    Script Date: 12/2/2019 3:34:58 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[DocumentInstruction]    Script Date: 12/2/2019 3:34:56 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ShippingModel]    Script Date: 12/2/2019 3:34:58 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[TradeImportLog]    Script Date: 05-12-2019 11:32:43 ******/
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