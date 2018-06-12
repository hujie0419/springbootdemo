USE Gungnir;
GO	

IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   id = OBJECT_ID(N'[dbo].[ConvertToShort62]')
                    AND OBJECTPROPERTY(id, N'IsScalarFunction') = 1 )
    DROP FUNCTION [dbo].[ConvertToShort62];
GO

CREATE FUNCTION [dbo].[ConvertToShort62] ( @Key BIGINT )
RETURNS NVARCHAR(100)
AS
    BEGIN
        DECLARE @SourceCharSet NVARCHAR(62)= N'0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
        DECLARE @rest BIGINT= @Key;
        DECLARE @result NVARCHAR(100)= N'';
        DECLARE @restIndex BIGINT;
        IF ( @rest = 0 )
            RETURN N'0';
        WHILE ( @rest != 0 )
            BEGIN
                SET @restIndex = ( @rest - ( @rest / 62 ) * 62 );
                SET @result = @result + SUBSTRING(@SourceCharSet,
                                                  @restIndex + 1, 1);
                SET @rest = @rest / 62;
            END;
        SET @result = REVERSE(@result);
        RETURN @result;
    END;