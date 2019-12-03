create PROCEDURE dbo.SaveShippingTradeDetails
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
	@ NVARCHAR(50),
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
           Version,
           Quantity,
           BLModelName,
           Description,
           Remarks
		FROM @tvpShippingModels

END
GO
