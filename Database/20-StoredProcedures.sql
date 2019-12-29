USE [ShippingTrade]
GO
/****** Object:  StoredProcedure [dbo].[DeleteCourierInvoice]    Script Date: 29-12-2019 22:37:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[DeleteCourierInvoice]
(
	@InvoiceDetailId INT
)
AS 
BEGIN 
	SET NOCOUNT ON

	DELETE FROM [dbo].[IncomingCourierDetails]
	WHERE ParentDetailsID = @InvoiceDetailId;

	DELETE FROM [dbo].[IncomingCourierDetails]
	WHERE DetailsID = @InvoiceDetailId;

END 
GO
/****** Object:  StoredProcedure [dbo].[GetShippingTradeDetails]    Script Date: 29-12-2019 22:37:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetShippingTradeDetails]
(
@SINo INT
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @ShippingId INT = 0
	SELECT TOP 1 @ShippingId = [Id] FROM Shipping WHERE [SINo] = @SINo ORDER BY [Id] ASC

	SELECT [Id]
      ,[TradeSheetName]
      ,[SIDate]
      ,[SINo]
      ,[Vender]
      ,[SoldToParty]
      ,[ShipToParty]
      ,[BLConsignee]
      ,[PortOfDischarge]
      ,[FinalDestination]
      ,[Via]
      ,[Transportation]
      ,[PortOfLoading]
      ,[TradeTerms]
      ,[PaymentTerms]
      ,[LCNo]
      ,[LCIssuanceDate]
      ,[LCIssuingBank]
      ,[LCExpiryDate]
      ,[ShipmentExpiryDate]
      ,[RequiredBLDate]
      ,[Freight]
      ,[PartialShipment]
      ,[TransShipment]
	FROM [dbo].[Shipping]
	WHERE [Id] = @ShippingId

	SELECT [ShippingId]
      ,[Instruction]
	FROM [dbo].[DocumentInstruction]
	WHERE [ShippingId] = @ShippingId
	
	SELECT [ShippingId]
      ,[PONo]
      ,[ModelName]
      ,[Version]
      ,[Quantity]
      ,[BLModelName]
      ,[Description]
      ,[Remarks]
	FROM [dbo].[ShippingModel]
	WHERE [ShippingId] = @ShippingId
END
GO
/****** Object:  StoredProcedure [dbo].[SaveIncomingCourierDetails]    Script Date: 29-12-2019 22:37:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaveIncomingCourierDetails]
(
	@tvpIncomingCourierDetails dbo.IncomingCourierDetailsTableType READONLY
)
AS 
BEGIN

	DECLARE @ParentDetailID INT;
	DECLARE @MasterID INT;

	SELECT DISTINCT @MasterID = [MasterID] FROM @tvpIncomingCourierDetails;

	IF EXISTS (SELECT 1 FROM @tvpIncomingCourierDetails
		WHERE DetailsID = 0 AND [IsSubDetail] = 0 AND MasterID = @MasterID)
	BEGIN
		INSERT INTO [dbo].[IncomingCourierDetails]
			([CompanyID]
			   ,[MasterID]
			   ,[SINo]
			   ,[Date]
			   ,[DocDetail]
			   ,[RefNo]
			   ,[Qty]
			   ,[Remarks]
			   ,[FileName]
			   ,[FilePath]
			   ,[ParentDetailsID]
			   ,[ArraySubItem]
			   ,[IsSubDetail]
			   ,[CreatedBy]
			   ,[CreationDate]
			   ,[UpdatedBy]
			   ,[UpdatedDate])
			SELECT 
				[CompanyID]
			  ,[MasterID]
			  ,ISNULL([SINo], '')
			  ,ISNULL([Date], GETDATE())
			  ,ISNULL([DocDetail], '')
			  ,ISNULL([RefNo], '')
			  ,ISNULL([Qty], 0.0)
			  ,ISNULL([Remarks], '')
			  ,ISNULL([FileName], '')
			  ,ISNULL([FilePath], '')
			  ,ISNULL([ParentDetailsID], 0)
			  ,ISNULL([ArraySubItem], '')
			  ,[IsSubDetail]
			  ,ISNULL([CreatedBy], 1)
			  ,ISNULL([CreationDate], GETDATE())	
			  ,ISNULL([UpdatedBy], 1)
			  ,ISNULL([UpdatedDate], GETDATE())
		FROM @tvpIncomingCourierDetails
		WHERE DetailsID = 0 AND [IsSubDetail] = 0 AND MasterID = @MasterID

		SET @ParentDetailID = @@IDENTITY
	END
	ELSE
	BEGIN
		UPDATE detail
		SET detail.[SINo] = masterDetail.[SINo]
			,detail.[UpdatedBy] = ISNULL(masterDetail.[UpdatedBy], 1)
			,detail.[UpdatedDate] = ISNULL(masterDetail.[UpdatedDate], GETDATE())
		FROM [IncomingCourierDetails] detail INNER JOIN @tvpIncomingCourierDetails masterDetail
			ON detail.DetailsID = masterDetail.DetailsID AND detail.[IsSubDetail] = 0
				AND detail.MasterID = @MasterID

		SELECT @ParentDetailID = detail.DetailsID
		FROM [IncomingCourierDetails] detail INNER JOIN @tvpIncomingCourierDetails masterDetail
			ON detail.DetailsID = masterDetail.DetailsID AND detail.[IsSubDetail] = 0
				AND detail.MasterID = @MasterID
	END 

	--update
	UPDATE subDetail
	SET	subDetail.RefNo = ISNULL(masterDetail.RefNo, '') 
		,subDetail.DocDetail = masterDetail.DocDetail
		,subDetail.Qty = ISNULL(masterDetail.Qty, 0.0)
		,subDetail.Remarks = ISNULL(masterDetail.Remarks, '') 
		,subDetail.UpdatedBy = ISNULL(masterDetail.UpdatedBy, 1)
		,subDetail.UpdatedDate = ISNULL(masterDetail.UpdatedDate, GETDATE())
	FROM (SELECT * FROM @tvpIncomingCourierDetails WHERE IsSubDetail = 1)
				masterDetail LEFT OUTER JOIN [IncomingCourierDetails] subDetail
		ON subDetail.DetailsID = masterDetail.DetailsID
			AND subDetail.ParentDetailsID = @ParentDetailID
			AND subDetail.MasterID = @MasterID

	---- delete
	DELETE subDetail
	FROM [IncomingCourierDetails] subDetail LEFT OUTER JOIN
			(SELECT * FROM @tvpIncomingCourierDetails WHERE IsSubDetail = 1)
						masterDetail
		ON subDetail.DetailsID = masterDetail.DetailsID 
			AND masterDetail.IsSubDetail = masterDetail.IsSubDetail
			AND subDetail.MasterID = @MasterID			
	WHERE masterDetail.DetailsID IS NULL AND subDetail.IsSubDetail = 1

	-- insert
	INSERT INTO [dbo].[IncomingCourierDetails]
		([CompanyID]
           ,[MasterID]
           ,[SINo]
           ,[Date]
           ,[DocDetail]
           ,[RefNo]
           ,[Qty]
           ,[Remarks]
           ,[FileName]
           ,[FilePath]
           ,[ParentDetailsID]
           ,[ArraySubItem]
           ,[IsSubDetail]
           ,[CreatedBy]
           ,[CreationDate]
		   ,[UpdatedBy]
		   ,[UpdatedDate])
	SELECT 
			masterDetail.[CompanyID]
		  ,masterDetail.[MasterID]
		  ,ISNULL(masterDetail.[SINo], '')
		  ,ISNULL(masterDetail.[Date], GETDATE())
		  ,ISNULL(masterDetail.[DocDetail], '')
		  ,ISNULL(masterDetail.[RefNo], '')
		  ,ISNULL(masterDetail.[Qty], 0.0)
		  ,ISNULL(masterDetail.[Remarks], '')
		  ,ISNULL(masterDetail.[FileName], '')
		  ,ISNULL(masterDetail.[FilePath], '')
		  ,@ParentDetailID
		  ,ISNULL(masterDetail.[ArraySubItem], '')
		  ,masterDetail.[IsSubDetail]
		  ,ISNULL(masterDetail.[CreatedBy], 1)
		  ,ISNULL(masterDetail.[CreationDate], GETDATE())	
		  ,ISNULL(masterDetail.[UpdatedBy], 1)
		  ,ISNULL(masterDetail.[UpdatedDate], GETDATE())
	FROM (SELECT * FROM @tvpIncomingCourierDetails WHERE IsSubDetail = 1)
				masterDetail LEFT OUTER JOIN [IncomingCourierDetails] subDetail
		ON subDetail.DetailsID = masterDetail.DetailsID 
			AND masterDetail.IsSubDetail = masterDetail.IsSubDetail
			AND subDetail.MasterID = @MasterID
			AND subDetail.IsSubDetail = 1
	WHERE subDetail.DetailsID IS NULL
		
	SELECT @MasterID
END
GO
/****** Object:  StoredProcedure [dbo].[SaveIncomingCourierMaster]    Script Date: 29-12-2019 22:37:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SaveIncomingCourierMaster]
(
	@CompanyID uniqueidentifier,
	@MasterID INT,
	@AWBNo NVARCHAR(MAX),
	@CourierCompany INT,
	@ReceivedFrom INT,
	@CourierFor INT,
	@DocumentType INT,
	@ReceivedOn DATETIME,
	@HandedOverOn DATETIME,
	@CreatedBy INT,
	@CreationDate DATETIME,
	@UpdatedBy INT,
	@UpdatedDate DATETIME
 )
 AS
 BEGIN

	IF (@MasterID = 0)
	BEGIN
		INSERT INTO [dbo].[IncomingCourierMaster]
			   ([CompanyID]
			   ,[AWBNo]
			   ,[CourierCompany]
			   ,[ReceivedFrom]
			   ,[CourierFor]
			   ,[DocumentType]
			   ,[ReceivedOn]
			   ,[HandedOverOn]
			   ,[CreatedBy]
			   ,[CreationDate]
			   ,[UpdatedBy]
			   ,[UpdatedDate])
		 VALUES
			   (@CompanyID,
			   @AWBNo,
			   @CourierCompany,
			   @ReceivedFrom,
			   @CourierFor,
			   @DocumentType,
			   @ReceivedOn,
			   @HandedOverOn,
			   @CreatedBy,
			   @CreationDate,
			   @UpdatedBy,
			   @UpdatedDate)

		SET @MasterID = @@IDENTITY
	END
	ELSE
	BEGIN
		UPDATE [dbo].[IncomingCourierMaster]
		SET	[AWBNo] = @AWBNo
			,[CourierCompany] = @CourierCompany
			,[ReceivedFrom] = @ReceivedFrom
			,[CourierFor] = @CourierFor
			,[DocumentType] = @DocumentType
			,[ReceivedOn] = @ReceivedOn
			,[HandedOverOn] = @HandedOverOn
			,[UpdatedBy] = @UpdatedBy
			,[UpdatedDate] = @UpdatedDate
		WHERE MasterID = @MasterID --AND [CompanyID] = @CompanyID
	END

	SELECT @MasterID AS MasterID
 END



GO
/****** Object:  StoredProcedure [dbo].[SaveShippingTradeDetails]    Script Date: 29-12-2019 22:37:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaveShippingTradeDetails]
(
	@TradeSheetName NVARCHAR(50),
	@SIDate NVARCHAR(50),
	@SINo NVARCHAR(50),
	@Vender NVARCHAR(50),
	@SoldToParty NVARCHAR(50),
	@ShipToParty NVARCHAR(50),
	@BLConsignee NVARCHAR(50),
	@PortOfDischarge NVARCHAR(50),
	@FinalDestination NVARCHAR(50),
	@Via NVARCHAR(50),
	@Transportation NVARCHAR(50),
	@PortOfLoading NVARCHAR(50),
	@TradeTerms NVARCHAR(50),
	@PaymentTerms NVARCHAR(50),
	@LCNo NVARCHAR(50),
	@LCIssuanceDate NVARCHAR(50),
	@LCIssuingBank NVARCHAR(50),
	@LCExpiryDate NVARCHAR(50),
	@ShipmentExpiryDate NVARCHAR(50),
	@RequiredBLDate NVARCHAR(50),
	@Freight NVARCHAR(50),
	@PartialShipment NVARCHAR(50),
	@TransShipment NVARCHAR(50),
	@tvpDocumentInstructions dbo.DocumentInstructionTableType READONLY,
	@tvpShippingModels dbo.ShippingModelTableType READONLY
) 
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @ShippingId INT = 0;

	SELECT TOP 1 @ShippingId = [Id] FROM Shipping WHERE [SINo] = @SINo ORDER BY [Id] ASC

	IF (@ShippingId = 0)
	BEGIN
		INSERT INTO [dbo].[Shipping]
			   ([TradeSheetName]
			   ,[SIDate]
			   ,[SINo]
			   ,[Vender]
			   ,[SoldToParty]
			   ,[ShipToParty]
			   ,[BLConsignee]
			   ,[PortOfDischarge]
			   ,[FinalDestination]
			   ,[Via]
			   ,[Transportation]
			   ,[PortOfLoading]
			   ,[TradeTerms]
			   ,[PaymentTerms]
			   ,[LCNo]
			   ,[LCIssuanceDate]
			   ,[LCIssuingBank]
			   ,[LCExpiryDate]
			   ,[ShipmentExpiryDate]
			   ,[RequiredBLDate]
			   ,[Freight]
			   ,[PartialShipment]
			   ,[TransShipment])
		 VALUES
			   (@TradeSheetName,
			   @SIDate,
			   @SINo,
			   @Vender,
			   @SoldToParty,
			   @ShipToParty,
			   @BLConsignee,
			   @PortOfDischarge,
			   @FinalDestination,
			   @Via,
			   @Transportation,
			   @PortOfLoading,
			   @TradeTerms,
			   @PaymentTerms,
			   @LCNo,
			   @LCIssuanceDate,
			   @LCIssuingBank,
			   @LCExpiryDate,
			   @ShipmentExpiryDate,
			   @RequiredBLDate,
			   @Freight,
			   @PartialShipment,
			   @TransShipment)

		SET @ShippingId = @@IDENTITY

		INSERT INTO [dbo].[DocumentInstruction]
		([ShippingId], [Instruction])
		SELECT 
			@ShippingId, Instruction					
		FROM @tvpDocumentInstructions

		INSERT INTO [dbo].[ShippingModel]
			   ([ShippingId]
			   ,[PONo]
			   ,[ModelName]
			   ,[Version]
			   ,[Quantity]
			   ,[BLModelName]
			   ,[Description]
			   ,[Remarks])
		 SELECT @ShippingId,
			   PONo,
			   ModelName,
			   [Version],
			   Quantity,
			   BLModelName,
			   [Description],
			   Remarks
			FROM @tvpShippingModels
	END
	SELECT @ShippingId AS ShippingId;

END
GO
/****** Object:  StoredProcedure [dbo].[WriteShippingImportLog]    Script Date: 29-12-2019 22:37:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[WriteShippingImportLog]
(
@ShippingId INT,
@WorkBookName nvarchar(250),
@TradeRequest nvarchar(max),
@ImportDate datetime,
@ImportStatus nvarchar(20),
@ExceptionMessage nvarchar(1000)
)
AS 
BEGIN 
	SET NOCOUNT ON

	INSERT INTO [dbo].[TradeImportLog]
           ([ShippingId]
           ,[WorkBookName]
           ,[TradeRequest]
           ,[ImportDate]
           ,[ImportStatus]
           ,[ExceptionMessage])
     VALUES
           (@ShippingId,
           @WorkBookName,
           @TradeRequest,
           @ImportDate,
           @ImportStatus,
           @ExceptionMessage)
END 
GO
