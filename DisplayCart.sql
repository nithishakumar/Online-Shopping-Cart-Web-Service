USE [PartsWeb]
GO

/****** Object:  StoredProcedure [dbo].[DisplayCart]    Script Date: 07/25/2021 09:56:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DisplayCart]
@Username CHAR(10)
AS
CREATE TABLE temp
(
Fran Varchar(2),
Partno Varchar(28),
Part Varchar(28),
Brand Char(4),
BrdDesc Varchar(20),
SohCpd numeric(7, 0),
SohPrBr numeric(38, 0),
SohBrch numeric(38, 0),
Rate numeric(11, 3),
Mad numeric(10, 3),
Factor numeric(5, 2),
CrtnQty numeric(7, 0),
PartDesc Varchar(30),
"Weight" numeric (11, 3),
Volume numeric (11, 3),
"Length" numeric (9, 3),
Height numeric (9, 3),
Width numeric (9, 3),
Suppart VARCHAR(28),
SohFactor numeric(5,2),
Category varchar(6),
MainGroup varchar(4),
SubGroup varchar(4),
PmSitWk1 numeric(7,0),
PmSitWk2 numeric(7,0),
PmSitWk3 numeric(7,0),
PmSitWk4 numeric(7,0),
CSBASIS varchar(10),
DSFACT numeric(11,3),
VALIDGRP varchar(10),
DSPrcd varchar(4),
DSBrand varchar(4)
)

DECLARE @Brand Char(4), @PartNo Varchar(28)
DECLARE cart_cursor CURSOR FOR 
SELECT PCBRAND, PCPART
FROM PATCART WHERE PCUSER = @Username
order by pccartid
OPEN cart_cursor
FETCH NEXT FROM cart_cursor INTO
@Brand, @PartNo
WHILE @@FETCH_STATUS = 0
BEGIN
INSERT temp EXEC DTPartSelection 52, @PartNo, 2, 'RP', 'MAD', 'SOH', '9999'
SELECT Part, Suppart, BrdDesc, CrtnQty, Rate, SohCpd, SohBrch, Brand, PartDesc, PCCONFQTY, PCREMARKS FROM temp,PATCART WHERE temp.Part = PATCART.PCPART AND temp.Brand = PATCART.PCBRAND AND PCUSER = @Username
DELETE FROM temp
FETCH NEXT FROM cart_cursor
END
CLOSE cart_cursor
DEALLOCATE cart_cursor
DROP TABLE temp

GO



