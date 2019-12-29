USE [ShippingTrade]
GO

/****** Object:  StoredProcedure [dbo].[SaveIncomingCourierMaster]    Script Date: 28-12-2019 10:06:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[SaveIncomingCourierMaster]
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


