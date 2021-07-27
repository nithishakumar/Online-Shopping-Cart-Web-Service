CREATE DATABASE PARTSWEB
USE PARTSWEB

/****** Sample table queries ******/

CREATE TABLE PATPARTDATA(
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
"Weight" numeric(11, 3),
Volume numeric(11, 3),
"Length" numeric(9, 3),
Height numeric(9, 3),
Width numeric (9, 3),
Suppart Varchar(28),
SohFactor numeric(5, 2),
Category varchar(6),
MainGroup varchar(4),
SubGroup varchar(4),
PmSitWk1 numeric(7, 0),
PmSitWk2 numeric(7, 0),
PmSitWk3 numeric(7, 0),
PmSitWk4 numeric(7, 0),
CSBASIS varchar(10),
DSFact numeric(11, 3),
VALIDGRP varchar(10),
DSPrcd varchar(4),
DsBrand varchar(4)
)

CREATE TABLE PATCART(
PCCARTID INT IDENTITY NOT NULL,
PRIMARY KEY(PCCARTID),
PCAPPL CHAR(10),
PCUSER CHAR(10),
PCPART VARCHAR(10),
PCINQPART VARCHAR(10),
PCRATE NUMERIC(10, 3),
PCBRAND VARCHAR(10),
PCCONFQTY INT,
PCREMARKS VARCHAR(10),
PCCREATEDDATE DATETIME,
PCCREATEDTIME DATETIME
)

CREATE TABLE PATUSERFILES (
UFFILEID INT NOT NULL IDENTITY,
PRIMARY KEY(UFFILEID),
UFAPPL CHAR(10),
UFUSER CHAR(10),
UFNAME VARCHAR(50),
UFTYPE VARCHAR(50),
UFDATA VARBINARY(MAX)
)


/****** Sample records ******/

INSERT INTO PATPARTDATA VALUES ('52', '7P0407021', '7P0407021', 'VKO', 'VAIKOVEMO', 6, 0, 0, 378.950, 0, 0, 1, 
'TRACK CNTRL ARM FRT LH/RH AUDI', 0, 0, 0, 0, 0, 'V10-3484', 4, '0', '0', '1', 0, 0, 0, 0, '1', 10, '1', '3', '3')

INSERT INTO PATPARTDATA VALUES ('52', '7P0407021', '7P0407021', 'FBI', 'FEBI', 0, 0, 0, 346, 0, 0, 1, 
'CONTROL ARM FRT-VW/PORSCHE', 0, 0, 0, 0, 0, '106923', 4, '0', '0', '1', 0, 0, 0, 0, '1', 10, '1', '3', '3')

EXEC AddToCart 'PARTSWEB', 'ANTOZ02053', '7P0407021', '7P0407021', 378.950, 'VKO', 2, 'sample1', NULL, NULL
EXEC Addtocart 'PARTSWEB', 'ANTOZ02053', '7P0407021', '7P0407021', 346, 'FEBI', 3, 'sample2', NULL, NULL
EXEC Addtocart 'PARTSWEB', 'ANTOZ02022', '220702122w', '220702122w', 378.950, 'F2I', 3, 'sample3', NULL, NULL