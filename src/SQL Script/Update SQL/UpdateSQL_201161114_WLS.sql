ALTER TABLE dbo.SCM_FlowStrategy ADD SafeTime DECIMAL(18,8) NULL,SeqGroup VARCHAR(50),MrpTotalAdjust DECIMAL(18,8),MrpWeight INT,MrpTotal DECIMAL(18,8)
go
DELETE FROM SYS_CodeDet WHERE Code = 'FlowStrategy'
go
INSERT INTO dbo.SYS_CodeDet 
        ( Code, Value, Desc1, IsDefault, Seq )
VALUES  ( 'FlowStrategy', '1', 'CodeDetail_FlowStrategy_Manual', 1, 0)
		,( 'FlowStrategy', '2', 'CodeDetail_FlowStrategy_KB', 0, 1)
		,( 'FlowStrategy', '3', 'CodeDetail_FlowStrategy_JIT', 0, 2)
		,( 'FlowStrategy', '4', 'CodeDetail_FlowStrategy_SEQ', 0, 3)
		,( 'FlowStrategy', '7', 'CodeDetail_FlowStrategy_ANDON', 0, 4)
go