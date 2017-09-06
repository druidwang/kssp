
/****** Object:  StoredProcedure [dbo].[USP_Busi_PauseVanOrder]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_PauseVanOrder')
	DROP PROCEDURE USP_Busi_PauseVanOrder
GO

CREATE PROCEDURE [dbo].[USP_Busi_PauseVanOrder]
AS
BEGIN
	--��ͣ�ƻ���ͣ�Ĺ�������ǰ��λ���ڼƻ���ͣ��λ
	update ORD_OrderMstr_4 set IsPause = 1, IsPlanPause = 0	where CurtOp = PauseSeq and IsPlanPause = 1
END
GO
