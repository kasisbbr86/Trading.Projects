ALTER PROCEDURE dbo.SaveShippingTradeDetails
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

ALTER PROCEDURE WriteShippingImportLog
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

ALTER PROCEDURE GetShippingTradeDetails
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