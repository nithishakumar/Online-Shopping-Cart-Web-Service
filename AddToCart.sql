USE [PartsWeb]
GO

/****** Object:  StoredProcedure [dbo].[AddToCart]    Script Date: 07/25/2021 09:55:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddToCart](
@pcschema CHAR(10),
@Username CHAR(10),
@Part VARCHAR (10),
@InqPart VARCHAR (10),
@Rate NUMERIC(10,3),
@Brand VARCHAR(10),
@ConfQty INT,
@Remarks VARCHAR(100),
@CreatedDate DateTime,
@CreatedTime DateTime
)
AS 
BEGIN
SET NOCOUNT OFF
IF EXISTS (SELECT *FROM PATCART WHERE PCPART = @Part AND PCBRAND = @Brand AND PCUSER = @Username)
BEGIN
UPDATE PATCART SET PCCONFQTY = @ConfQty, PCREMARKS = @Remarks, PCCREATEDDATE = @CreatedDate, PCCREATEDTIME = @CreatedTime 
WHERE PCPART = @Part AND PCBRAND = @Brand AND PCUSER = @Username
END
ELSE 
BEGIN
INSERT INTO PATCART(PCAPPL, PCUSER, PCPART, PCINQPART, PCRATE, PCBRAND, PCCONFQTY, PCREMARKS, PCCREATEDDATE, PCCREATEDTIME) VALUES(
 @pcschema, @Username, @Part, @InqPart, @Rate, @Brand, @ConfQty, @Remarks, @CreatedDate, @CreatedTime)
END
END



GO
