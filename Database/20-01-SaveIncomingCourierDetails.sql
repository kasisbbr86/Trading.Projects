USE [ShippingTrade]
GO
/****** Object:  StoredProcedure [dbo].[SaveIncomingCourierDetails]    Script Date: 28-12-2019 10:07:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SaveIncomingCourierDetails]
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

	-- delete
	--DELETE subDetail
	--FROM [IncomingCourierDetails] subDetail LEFT OUTER JOIN @tvpIncomingCourierDetails masterDetail
	--	ON subDetail.DetailsID = masterDetail.DetailsID AND subDetail.IsSubDetail = 1
	--		AND subDetail.ParentDetailsID = @ParentDetailID
	--WHERE masterDetail.DetailsID IS NULL

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
	WHERE subDetail.DetailsID IS NULL

	SELECT @MasterID
END