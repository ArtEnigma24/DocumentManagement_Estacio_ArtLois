USE DocumentManagement;
GO

CREATE OR ALTER PROCEDURE dbo.SP_Document
    @Mode             VARCHAR(50),
    @Id               INT           = NULL,
    @OriginalFileName NVARCHAR(255) = NULL,
    @StoredFileName   NVARCHAR(100) = NULL,
    @ContentType      NVARCHAR(100) = NULL,
    @FileSize         BIGINT        = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Mode = 'INSERT_DOCUMENT'
    BEGIN
        INSERT INTO dbo.Documents (OriginalFileName, StoredFileName, ContentType, FileSize)
        VALUES (@OriginalFileName, @StoredFileName, @ContentType, @FileSize);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END

    ELSE IF @Mode = 'GET_BY_ID'
    BEGIN
        SELECT Id, OriginalFileName, StoredFileName, ContentType, FileSize, UploadedAtUtc
        FROM dbo.Documents
        WHERE Id = @Id;
    END

    ELSE IF @Mode = 'GET_ALL'
    BEGIN
        SELECT Id, OriginalFileName, StoredFileName, ContentType, FileSize, UploadedAtUtc
        FROM dbo.Documents
        ORDER BY UploadedAtUtc DESC;
    END

    ELSE IF @Mode = 'DELETE_DOCUMENT'
    BEGIN
        DELETE FROM dbo.Documents
        OUTPUT DELETED.StoredFileName
        WHERE Id = @Id;
    END
END
GO