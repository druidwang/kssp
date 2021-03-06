USE [Sconit]
GO
/****** Object:  StoredProcedure [dbo].[USP_SAP_MM_ExportTransOrder]    Script Date: 12/08/2014 10:31:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
ALTER PROCEDURE [dbo].[USP_SAP_MM_ExportTransOrder]
(
	@BatchNo varchar(50),
	@StartTime datetime,
	@EndTime datetime
)
AS
BEGIN
	----2014/06/22 Add UnitQty --0001
	----2014/06/25 有直接扫描移库路线移库的，所以没有路线信息，这里要考虑到 --0002
	----2014/07/14 Add 产品组信息 --0003
	----2014/07/29 成本中心发料9306库位不传 --0004
	----2014/09/24 没有SAP库位的对陈本中心发料不传 SAP --0005
	----2014/11/12  过账日期取创建时间 --0006
	SET NOCOUNT ON
		DECLARE @CurrDate datetime=GETDATE()
		DECLARE @EKORG varchar(50),@WERKS varchar(50),@BUKRS varchar(50)
		DECLARE @ChongQingSaleFlow varchar(1000)=''
		SELECT @ChongQingSaleFlow=Value FROM SYS_EntityPreference WHERE Id =90109
		---select * from SYS_EntityPreference
		SELECT @EKORG=Value FROM SYS_EntityPreference WHERE Id=90105
		SELECT @BUKRS=Value FROM SYS_EntityPreference WHERE Id=90106
		SELECT @WERKS=Value FROM SYS_EntityPreference WHERE Id=90107
		
		----视作销售路线的移库路线
		SELECT F1 As Flow INTO #TransferFlowEqualWithSaleFlow FROM dbo.Func_SplitStr(@ChongQingSaleFlow,'‖')
		SELECT * INTO #Temp1_STMES0001 FROM SAP_Interface_STMES0001 WHERE 1<>1
		
		SELECT * INTO #Temp2_STMES0001 FROM SAP_Interface_STMES0001 WHERE 1<>1
		/*--0005
		Select Code into #NONSAP from MD_Location where SAPLocation ='N/A' and Region not like 'S%'
		--插入成本中心发料接口--移到调整单接口
		INSERT INTO #Temp1_STMES0001(ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, 
			ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, DataType)
		SELECT mm.MiscOrderNo+mm.MoveType,md.Seq,MoveType,@WERKS AS WERKS,mm.EffDate,mm.CloseDate,
			l.SAPLocation AS LGORT,mm.CostCenter,mm.MiscOrderNo AS LIFNR,md.Item,CAST(md.Qty*md.UnitQty AS decimal(10,3)) AS Qty,
			md.BaseUom,'' AS MATNR_TH,'' AS UMLGO,mm.MiscOrderNo+'0',@CurrDate,0,@BatchNo, 3
		FROM ORD_MiscOrderMstr mm INNER JOIN ORD_MiscOrderDet md ON mm.MiscOrderNo=md.MiscOrderNo
		INNER JOIN MD_Location l ON l.Code=mm.Location
		WHERE mm.SubType in(0,26) AND mm.CloseDate>@StartTime AND mm.CloseDate<=@EndTime AND mm.IsCs=0
		--0004--0005
		AND mm.Location not in (Select Code from #NONSAP) AND mm.CreateUser<>3910
		
		INSERT INTO #Temp2_STMES0001(ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, 
			ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, DataType)
		SELECT mm.MiscOrderNo+mm.CancelMoveType,md.Seq,CancelMoveType,@WERKS AS WERKS,mm.EffDate,mm.CancelDate,
			l.SAPLocation AS LGORT,mm.CostCenter,mm.MiscOrderNo AS LIFNR,md.Item,CAST(md.Qty*md.UnitQty AS decimal(10,3)) AS Qty,
			md.BaseUom,'' AS MATNR_TH,'' AS UMLGO,mm.MiscOrderNo+'1',@CurrDate,0,@BatchNo, 3
		FROM ORD_MiscOrderMstr mm INNER JOIN ORD_MiscOrderDet md ON mm.MiscOrderNo=md.MiscOrderNo
		INNER JOIN MD_Location l ON l.Code=mm.Location
		WHERE mm.SubType in(0,26) AND mm.Status=2 AND mm.CancelDate>@StartTime AND mm.CancelDate<=@EndTime AND mm.IsCs=0
		--0004--0005
		AND mm.Location not in (Select Code from #NONSAP) AND mm.CreateUser<>3910
		
		--处理计划外出入库同一个批次发生冲销的问题	
		SELECT s1.LIFNR INTO #TEMP2 FROM #Temp1_STMES0001 s1 INNER JOIN #Temp2_STMES0001 s2 ON s1.LIFNR=s2.LIFNR
		WHERE s1.BatchNo=@BatchNo AND s2.BatchNo=@BatchNo
		IF @@ROWCOUNT>0
		BEGIN
			DELETE a FROM #Temp1_STMES0001 a INNER JOIN #TEMP2 b ON a.LIFNR=b.LIFNR
			DELETE a FROM #Temp2_STMES0001 a INNER JOIN #TEMP2 b ON a.LIFNR=b.LIFNR
		END
		
		DROP TABLE #TEMP2*/
		
		
		--插入移库数据	
		INSERT INTO #Temp1_STMES0001(ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, 
			ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, DataType)
		SELECT rm.RecNo,rd.Seq,'311' BWARTWA,@WERKS AS WERKS,
			rm.CreateDate,rm.CreateDate,--0006 rm.EffDate,rm.EffDate change to rm.CreateDate,rm.CreateDate
			--CASE WHEN rm.OrderSubType=0 THEN lf.SAPLocation ELSE lt.SAPLocation END AS LGORT,
			lf.SAPLocation as LGORT,
			'' AS KOSTL,'' AS LIFNR,rd.Item,CAST(rd.RecQty*rd.UnitQty/*--0001*/ AS decimal(10,3)) AS Qty,
			rd.BaseUom,'' AS MATNR_TH,
			--CASE WHEN rm.OrderSubType=0 THEN lt.SAPLocation ELSE lf.SAPLocation END  AS UMLGO,
			 lt.SAPLocation AS UMLGO,
			rm.RecNo+'0',@CurrDate,0,@BatchNo, 2
		FROM ORD_RecMstr_2 rm INNER JOIN ORD_RecDet_2 rd ON rm.RecNo=rd.RecNo
		LEFT JOIN MD_Location lf ON lf.Code=rd.LocFrom
		LEFT JOIN MD_Location lt ON lt.Code=rd.LocTo
		WHERE rm.Type<>1 AND rm.CreateDate>@StartTime AND rm.CreateDate<=@EndTime AND lf.SAPLocation<>lt.SAPLocation AND rm.CreateUser<>3910
			AND Isnull(rm.Flow,'')/*--0002*/ NOT IN(SELECT Flow FROM #TransferFlowEqualWithSaleFlow)
		
		INSERT INTO #Temp2_STMES0001(ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, 
			ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, DataType)
		SELECT rm.RecNo,rd.Seq,'312' BWARTWA,@WERKS AS WERKS,
			rm.LastModifyDate,rm.LastModifyDate,lf.SAPLocation AS LGORT,--0006 rm.EffDate,rm.EffDate change to rm.LastModifyDate,rm.LastModifyDate
			'' AS KOSTL,'' AS LIFNR,rd.Item,CAST(rd.RecQty*rd.UnitQty AS decimal(10,3)) AS Qty,
			rd.BaseUom,'' AS MATNR_TH,lt.SAPLocation AS UMLGO,rm.RecNo+'1',@CurrDate,0,@BatchNo, 2
		FROM ORD_RecMstr_2 rm INNER JOIN ORD_RecDet_2 rd ON rm.RecNo=rd.RecNo
		LEFT JOIN MD_Location lf ON lf.Code=rd.LocFrom
		LEFT JOIN MD_Location lt ON lt.Code=rd.LocTo
		WHERE rm.Type<>1 AND rm.LastModifyDate>@StartTime AND rm.LastModifyDate<=@EndTime AND rm.Status=1 AND rm.LastModifyUser<>3910
			----2014/09/24
			AND lf.SAPLocation<>lt.SAPLocation
			-------
			AND Isnull(rm.Flow,'')/*--0002*/ NOT IN(SELECT Flow FROM #TransferFlowEqualWithSaleFlow)
		
		--插入委外采购发货
		INSERT INTO #Temp1_STMES0001(ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, 
			ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, DataType)
		SELECT rm.RecNo,rd.Seq,CASE WHEN rm.OrderSubType=0 THEN '541' ELSE '542' END AS BWARTWA,@WERKS AS WERKS,
			rm.CreateDate,rm.CreateDate,CASE WHEN rm.OrderSubType=0 THEN lf.SAPLocation ELSE lt.SAPLocation END AS LGORT,--0006 rm.EffDate,rm.EffDate change to rm.CreateDate,rm.CreateDate
			'' AS KOSTL,CASE WHEN rm.OrderSubType=0 THEN rd.LocTo ELSE rd.LocFrom END AS LIFNR,rd.Item,CAST(rd.RecQty*rd.UnitQty AS decimal(10,3)) AS Qty,
			rd.BaseUom,'' AS MATNR_TH,'' AS UMLGO,rm.RecNo+'0',@CurrDate,0,@BatchNo, 1
		FROM ORD_RecMstr_7 rm INNER JOIN ORD_RecDet_7 rd ON rm.RecNo=rd.RecNo
		LEFT JOIN MD_Location lf ON lf.Code=rd.LocFrom
		LEFT JOIN MD_Location lt ON lt.Code=rd.LocTo
		WHERE rm.Type<>1 AND rm.CreateDate>@StartTime AND rm.CreateDate<=@EndTime AND rm.CreateUser<>3910
		
		INSERT INTO #Temp2_STMES0001(ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, 
			ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, DataType)
		SELECT rm.RecNo,rd.Seq,CASE WHEN rm.OrderSubType=0 THEN '542' ELSE '541' END AS BWARTWA,@WERKS AS WERKS,
			rm.LastModifyDate,rm.LastModifyDate,CASE WHEN rm.OrderSubType=0 THEN lf.SAPLocation ELSE lt.SAPLocation END AS LGORT,--0006 rm.EffDate,rm.EffDate change to rm.LastModifyDate,rm.LastModifyDate
			'' AS KOSTL,CASE WHEN rm.OrderSubType=0 THEN  rd.LocTo ELSE rd.LocFrom END AS LIFNR,rd.Item,CAST(rd.RecQty*rd.UnitQty AS decimal(10,3)) AS Qty,
			rd.BaseUom,'' AS MATNR_TH,'' AS UMLGO,rm.RecNo+'1',@CurrDate,0,@BatchNo, 1
		FROM ORD_RecMstr_7 rm INNER JOIN ORD_RecDet_7 rd ON rm.RecNo=rd.RecNo
		LEFT JOIN MD_Supplier sf ON sf.Code=rm.PartyFrom
		LEFT JOIN MD_Supplier st ON st.Code=rm.PartyTo
		LEFT JOIN MD_Location lf ON lf.Code=rd.LocFrom
		LEFT JOIN MD_Location lt ON lt.Code=rd.LocTo
		WHERE rm.Type<>1 AND rm.LastModifyDate>@StartTime AND rm.LastModifyDate<=@EndTime AND rm.Status=1 AND rm.LastModifyUser<>3910

		--插入物料替换数据
		INSERT INTO #Temp1_STMES0001(ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, 
			ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, DataType)
		SELECT ie.Id,1 as 'ZMESKOSEQ','309',@WERKS AS WERKS,ie.EffDate,ie.EffDate,
			l.SAPLocation AS LGORT,'' AS KOSTL,'' AS LIFNR,ie.ItemFrom,Cast(ie.Qty*ie.UnitQty As decimal(13,3)),ie.BaseUom,ie.ItemTo,'' AS UMLGO,
			Cast(ie.Id as varchar)+'1',@CurrDate,0,@BatchNo, 4
		FROM INV_ItemExchange ie INNER JOIN MD_Location l ON ie.LocationFrom=l.Code
		WHERE ie.CreateDate>@StartTime AND ie.CreateDate<=@EndTime and ie.ItemExchangeType=0  AND ie.CreateUser<>3910
		
		INSERT INTO #Temp2_STMES0001(ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, 
			ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, DataType)
		SELECT ie.Id,1 as 'ZMESKOSEQ','309',@WERKS AS WERKS,ie.EffDate,ie.EffDate,
			l.SAPLocation AS LGORT,'' AS KOSTL,'' AS LIFNR,ie.ItemTo,Cast(ie.Qty*ie.UnitQty As Decimal(13,3)),ie.BaseUom,ie.ItemFrom,'' AS UMLGO,
			Cast(ie.Id As varchar)+'1',@CurrDate,0,@BatchNo, 4
		FROM INV_ItemExchange ie INNER JOIN MD_Location l ON ie.LocationFrom=l.Code
		WHERE ie.LastModifyDate>@StartTime AND ie.LastModifyDate<=@EndTime AND IsVoid=1 and ie.ItemFrom<>ie.ItemTo and  ie.ItemExchangeType=0  AND ie.LastModifyUser<>3910
		
		--处理同一个批次发生冲销的问题	
		SELECT s1.ZMESKO INTO #TEMP1 FROM #Temp1_STMES0001 s1 INNER JOIN #Temp2_STMES0001 s2 ON s1.ZMESKO=s2.ZMESKO
		WHERE s1.BatchNo=@BatchNo AND s2.BatchNo=@BatchNo
		IF @@ROWCOUNT>0
		BEGIN
			DELETE a FROM #Temp1_STMES0001 a INNER JOIN #TEMP1 b ON a.ZMESKO=b.ZMESKO
			DELETE a FROM #Temp2_STMES0001 a INNER JOIN #TEMP1 b ON a.ZMESKO=b.ZMESKO
		END
		
		INSERT INTO SAP_Interface_STMES0001(ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, UniqueCode)
		SELECT ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, UniqueCode FROM #Temp1_STMES0001
		UNION ALL
		SELECT ZMESKO, ZMESKOSEQ, BWARTWA, WERKS, BLDAT, BUDAT, LGORT, KOSTL, LIFNR, MATNR1, EPFMG, ERFME, MATNR_TH, UMLGO, ZMESGUID, ZCSRQSJ, Status, BatchNo, UniqueCode FROM #Temp2_STMES0001
		
		
		DECLARE @UniqueCode varchar(50)
		WHILE EXISTS(SELECT * FROM SAP_Interface_STMES0001 WHERE BatchNo=@BatchNo AND UniqueCode IS NULL)
		BEGIN
			DECLARE @LastOrderNo varchar(50)
			SET @UniqueCode=REPLACE(NEWID(),'-','')
			UPDATE TOP(5000) SAP_Interface_STMES0001 SET UniqueCode=@UniqueCode WHERE BatchNo=@BatchNo AND UniqueCode IS NULL
			SELECT TOP 1 @LastOrderNo=ZMESKO FROM SAP_Interface_STMES0001 WHERE BatchNo=@BatchNo AND UniqueCode IS NULL
			PRINT @LastOrderNo
			IF EXISTS(SELECT 1 FROM SAP_Interface_STMES0001 WHERE BatchNo=@BatchNo AND UniqueCode=@UniqueCode AND ZMESKO=@LastOrderNo )
			BEGIN
				UPDATE SAP_Interface_STMES0001 SET UniqueCode=@UniqueCode WHERE BatchNo=@BatchNo AND ZMESKO=@LastOrderNo
			END
		END
		----0003--移库的产品组信息已经不重要，可以为空
		Update a
			Set a.SPART =isnull(b.Division,'') from SAP_Interface_STMES0001 a,MD_Item b 
				Where a.BatchNo=@BatchNo and a.MATNR1 =b.Code
		----0003
		SELECT Status=1,BatchNo=@BatchNo,'生成库存转储接口数据成功。'
END



