USE [PartsWeb]
GO

/****** Object:  StoredProcedure [dbo].[DTPartSelection]    Script Date: 07/25/2021 09:56:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[DTPartSelection](
   @Franc Varchar(2),
   @Part_no Varchar(130) ,
   @PrType  numeric(2,0),
   @PrValue varchar(10) ,
   @SlMADtype varchar(5),	
   @SlSOHtype varchar(5) ,
   @Cust varchar(10)	 
   ) AS
BEGIN
 ---Main SQL
 select * from (
 Select  Fran,  Partno,  Part,  Brand,   BrdDesc, SohCpd, 
  Coalesce(PRBR.PRBRQTY,0) SohPrBr,Coalesce(BR.BRCHQTY,0) SohBrch ,
 --SohBrch
 Rate, Mad,  Factor,CrtnQty,  PartDesc,   Weight,  Volume, 
 Length,  Height,  Width,  Suppart, SohFactor, Category, 
 MainGroup,  SubGroup,PmSitWk1,   PmSitWk2, PmSitWk3,  PmSitWk4 ,
 Coalesce(CSBASIS,' ') CSBASIS ,Coalesce(DSFACT,1) DSFACT  ,Coalesce(ValidGrp,' ') VALIDGRP,
  Coalesce(DSPrcd,' ') DSPrcd , Coalesce(DSBrand,' ') DSBrand
 from (  
 Select  Coalesce(Fran,' ') as Fran, Coalesce(PartNo,' ') as Partno,  Coalesce(Part,' ') as Part, 
 Coalesce(Brand,' ') as Brand,  Coalesce(Brddesc,' ') as BrdDesc, Coalesce(SohCpd,0) as SohCpd, 
 Coalesce(SohBrch,0) as SohBrch ,  Coalesce(Rate,0) as Rate,  Coalesce(Mad,0) as Mad, 
 Coalesce(E.SlFact,0) as Factor, Coalesce(IDCrtnQty,0) as CrtnQty,  
 Coalesce(IDPartDesc,' ') as PartDesc,  
 Coalesce(IDWeight,0) as Weight,  Coalesce(IDVolume,0) as Volume, 
 Coalesce(IDLength,0) as Length,  Coalesce(IDHeight,0) as Height,
 Coalesce(IDWidth,0) as Width,  Coalesce(IDSuppart,' ') as Suppart, 
 Coalesce(F.SlFact,0) as SohFactor, Coalesce(IDCategory,' ') as Category, 
 Coalesce(IDMainGroup,' ') as MainGroup,  Coalesce(IDSubGroup,' ') as SubGroup,
 COALESCE(PmSitWk1,0) AS PmSitWk1,  COALESCE(PmSitWk2,0) AS PmSitWk2,
 COALESCE(PmSitWk3,0) AS PmSitWk3,  COALESCE(PmSitWk4,0) AS PmSitWk4 
 From (Select IDFran  as Fran,IDDispPno as Partno,  IDSohCpd as SohCpd,
 IDSohBrch as SohBrch,  BDBrddesc as Brddesc,IDRate as Rate,IDMad as Mad,Brand,Part,
 IDCrtnQty,IDPartDesc,IDWeight,IDVolume,IDLength,IDHeight, IDWidth,
 IDSuppart,IDCategory,IDMainGroup,IDSubGroup, PmSitWk1 ,PmSitWk2,PmSitWk3,PmSitWk4
  From (Select A.Fran as IDFran, A.Partno as Part,A.Brand as Brand,
 Coalesce(B.PmPart,' ') as IDDisppno, Coalesce(B.PMSohCpd,0) as IDSohCPD,
 Coalesce(B.PMSohBrch,0) as IDSohBrch, Coalesce(B.PrRate,0) as IDRate,
 Coalesce(B.PmDomMad,0) as IDMad, Coalesce(B.PmPackQty,0) as IDCrtnQty,
 Coalesce(B.PMDesc,' ') as IDPartDesc, Coalesce(B.PMWeight,0) as IDWeight,
 Coalesce(B.PMVolume,0) as IDVolume, Coalesce(B.PMLength,0) as IDLength, 
 Coalesce(B.PMHeight,0) as IDHeight, Coalesce(B.PMWidth,0) as IDWidth,
 Coalesce(B.PMSuppart,' ') as IDSuppart, Coalesce(B.PMCatg,' ') as IDCategory,
 Coalesce(B.PMGroup,' ') as IDMainGroup, Coalesce(B.PMSubGroup,' ') as IDSubGroup,
 Coalesce(B.PmSitWk1,0) as PmSitWk1 ,  Coalesce(B.PmSitWk2,0) as PmSitWk2,
 Coalesce(B.PmSitWk3,0) as PmSitWk3,Coalesce(B.PmSitWk4,0) as PmSitWk4 
 From (Select PMFran as Fran, PMBrndPart as Partno, PMBrand as Brand
 From PartsWeb..Patparts Where PMFran = @Franc and PMBrndpart = @Part_no
 UNION 
 Select MFFran as Fran,MFPart as Partno, MFBrand as Brand From PartsWeb..PatMfgpart 
 Where MFFran = @Franc and MFPart= @Part_no
 UNION  
 Select PMFran as Fran, PMBrndpart as Partno, PMBrand as Brand 
 From PartsWeb..Patparts Where PMFran = @Franc
 And PMBrndpart In (Select MFPart From PartsWeb..PatMfgPart 
 Where MFFran=@Franc and MFMfgpart= @Part_no) 
 UNION 
 Select PMFran as Fran, PMBrndpart as Partno,
 PMBrand as Brand    From PartsWeb.. Patparts Where PMFran = @Franc And 
 PMBrndpart In  (Select MFPart as Partno From  PartsWeb.. PatMfgpart  
 Where MFFran = @Franc And MFmfgPart in           
 (Select MFmfgpart From  PartsWeb.. PatMfgpart
 Where MFfran = @Franc  And MFPart = @Part_no))) A  
 Left outer Join (Select PrFran,PMSohCpd,PMSohBrch,PRrate,PMPart,
 PMBrndpart,PMBrand,PMDomMad , PMPackqty,PMDesc,PMWeight,
 PMVolume,PMLength,PMHeight,PMWidth,
 PMSuppart,PMCatg,PMGroup,PMSubGroup,  PmSitWk1,PmSitWk2,
 PmSitWk3,PmSitWk4  From  PartsWeb..PatParts 
 Left Outer Join PartsWeb..PatRates On PRFran=PMFran And PRPart=PMPart
 Where PRFran=@Franc and PRType=@PrType and PRValue=@PrValue ) B  On A.Fran=B.PrFran And 
 A.Partno=B.PMBrndpart  And A.Brand=B.PMBrand) C  
 Left Outer Join  PartsWeb..PatBrands On C.IDFran=BDFran 
 and C.Brand=BDBrand) D  
 Left Outer Join PartsWeb..PatSalePar E On D.Fran=E.SLFran And E.Sltype=@SlMADtype
 And (E.SlFmValue <=Mad And E.SlToValue >= Mad) 
 Left Outer Join PartsWeb..PatSalePar F 
 On D.Fran=F.SLFran And  F.Sltype=  @SlSOHtype And 
 (F.SlFmValue <=Mad And F.SlToValue >= Mad)  
 Where Fran=@Franc And PartNo<>' ' ) xy
left outer join
(Select DSFRAN,DSCATG,DSGROUP,DSSUBGROUP,DSPrcd, DSBrand, CSBASIS, coalesce(DSFACT,0) as DSFACT 
 From PartsWeb..PATDISCGRP Inner Join PartsWeb..PATCUSTGRP
 On (DSFRAN=CSFRAN and DSBASIS=CSBASIS) 
 Where DSFran  = @Franc And Dsbaserate=@PrValue
 And CSCust=@Cust ) as YYY on 
 DSCATG in ('*',Category) and DSBrand In ('*', brand) and 
 DSGROUP in ('*',MainGroup) and DSSUBGROUP in ('*', SubGroup) 
  inner join 
 (Select PdCatg, PdGrp, PdSubGrp, coalesce(PDVALUE,' ') as ValidGrp From PartsWeb..PatPrdCust
 Where PdFran  = @Franc And PDCust=@Cust) as ZZZ
 on  PdCatg In ('*',Category) And PdGrp In ('*', MainGroup)
 And PdSubGrp In ('*', SubGroup)
  
  Left outer join 
--Primary Branch soh
(select BDPART, COALESCE(SUM(BDSOH),0) PRBRQTY from PartsWeb..patbrdystk where bdfran = @Franc
and bdbrch in (select cfvalue from PartsWeb..GEFCUFR where cffran = BDFRAN and cftype = 'PRBR' AND CFCUST = @Cust)
GROUP BY BDPART) PRBR
on PRBR.BDPART =Partno
 
Left outer join
--Other Branch soh
(select BDPART, COALESCE(SUM(BDSOH),0) BRCHQTY from PartsWeb..patbrdystk where bdfran = @Franc
and bdbrch not in (select cfvalue from PartsWeb..GEFCUFR where cffran = BDFRAN and cftype = 'PRBR' AND CFCUST = @Cust)
GROUP BY BDPART) BR
on BR.BDPART = Partno

 ) xx
   --End SQL
END

GO

