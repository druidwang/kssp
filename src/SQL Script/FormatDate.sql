
/****** Object:  UserDefinedFunction [dbo].[FormatDate]    Script Date: 07/05/2012 14:55:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.Objects WHERE Type='fn' AND name='FormatDate')
	DROP FUNCTION FormatDate
CREATE FUNCTION [dbo].[FormatDate]
(@date as datetime,
@formatstring as varchar(100)
)
RETURNS varchar(100) AS 
BEGIN
    declare @datestring as varchar(100)
    set @datestring=@formatstring
    --year
    set @datestring=replace(@datestring, 'yyyy', cast(year(@date) as char(4)))
    set @datestring=replace(@datestring, 'yy', right(cast(year(@date) as char(4)),2))
    --millisecond
    set @datestring=replace(@datestring, 'ms', replicate('0',3-len(cast(datepart(ms,@date) as varchar(3)))) + cast(datepart(ms, @date) as varchar(3)))
    --month
    set @datestring=replace(@datestring, 'mm', replicate('0',2-len(cast(month(@date) as varchar(2)))) + cast(month(@date) as varchar(2)))
    set @datestring=replace(@datestring, 'm', cast(month(@date) as varchar(2)))
    --day
    set @datestring=replace(@datestring, 'dd', replicate('0',2-len(cast(day(@date) as varchar(2)))) + cast(day(@date) as varchar(2)))
    set @datestring=replace(@datestring, 'd',  cast(day(@date) as varchar(2)))
    --hour
    set @datestring=replace(@datestring, 'hh', replicate('0',2-len(cast(datepart(hh,@date) as varchar(2)))) + cast(datepart(hh, @date) as varchar(2)))
    set @datestring=replace(@datestring, 'h',  cast(datepart(hh, @date) as varchar(2)))
   
    --minute
    set @datestring=replace(@datestring, 'nn', replicate('0',2-len(cast(datepart(n,@date) as varchar(2)))) + cast(datepart(n, @date) as varchar(2)))
    set @datestring=replace(@datestring, 'n', cast(datepart(n, @date) as varchar(2)))
    --second
    set @datestring=replace(@datestring, 'ss', replicate('0',2-len(cast(datepart(ss,@date) as varchar(2)))) + cast(datepart(ss, @date) as varchar(2)))
    set @datestring=replace(@datestring, 's', cast(datepart(ss, @date) as varchar(2)))
   
    --week
    set @datestring=replace(@datestring, 'ww', replicate('0',2-len(cast(datepart(ww,@date) as varchar(2)))) + cast(datepart(ww, @date) as varchar(2)))
    set @datestring=replace(@datestring, 'wk', replicate('0',2-len(cast(datepart(wk,@date) as varchar(2)))) + cast(datepart(wk, @date) as varchar(2)))
 
    return @datestring
END
GO

