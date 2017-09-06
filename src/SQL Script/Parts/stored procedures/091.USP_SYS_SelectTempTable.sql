SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_SYS_SelectTempTable')
BEGIN
	DROP PROCEDURE USP_SYS_SelectTempTable
END
GO

CREATE PROCEDURE dbo.USP_SYS_SelectTempTable
	@TempTable varchar(50)
AS 
BEGIN
	declare @firstPage char(12)
	declare @pageStartHX char(6)
	declare @pageEndHX char(10)
	declare @pageStart int
	declare @pageEnd int

	SELECT  @firstPage = CONVERT(char(12), AU.first_page, 2)
		FROM tempdb.sys.tables T
		JOIN tempdb.sys.partitions P
				ON  P.[object_id] = T.[object_id]
		JOIN tempdb.sys.system_internals_allocation_units AU
				ON  (AU.type_desc = N'IN_ROW_DATA' AND AU.container_id = P.partition_id)
				OR  (AU.type_desc = N'ROW_OVERFLOW_DATA' AND AU.container_id = P.partition_id)
				OR  (AU.type_desc = N'LOB_DATA' AND AU.container_id = P.hobt_id)
		WHERE T.name LIKE @TempTable + '%'

	if (@firstPage is not null)
	begin
		set @pageStartHX = '0x' + SUBSTRING(@firstPage, 11, 2) + SUBSTRING(@firstPage, 9, 2)
		set @pageEndHX = '0x' + SUBSTRING(@firstPage, 7, 2) + SUBSTRING(@firstPage, 5, 2) + SUBSTRING(@firstPage, 3, 2) + SUBSTRING(@firstPage, 1, 2)
		set @pageStart = CONVERT(int, CONVERT(varbinary,@pageStartHX, 1))
		set @pageEnd = CONVERT(int, CONVERT(varbinary, @pageEndHX, 1))

		DBCC TRACEON (3604);
		DBCC PAGE (tempdb, @pageStart, @pageEnd, 3) WITH TABLERESULTS;
	end
	else
	begin
		print N'临时表' + @TempTable + N'不存在'
	end
END
GO