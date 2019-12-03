CREATE TYPE dbo.DocumentInstructionTableType AS TABLE 
    ( Instruction nvarchar(250) )
GO
DROP TYPE ShippingModelTableType
CREATE TYPE dbo.ShippingModelTableType AS TABLE 
    (  [PONo] [nvarchar](100) ,
       [ModelName] [nvarchar](50) ,
       [Version] [nvarchar](10) ,
       [Quantity] [nvarchar](10) ,
       [BLModelName] [nvarchar](100) ,
       [Description] [nvarchar](250) ,
       [Remarks] [nvarchar](250) )
GO