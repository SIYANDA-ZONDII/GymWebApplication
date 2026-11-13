/* Run this in the GroupWst23 database if PasswordReset does not already exist. */
IF OBJECT_ID(N'dbo.PasswordReset', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PasswordReset
    (
        Reset_ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PasswordReset PRIMARY KEY,
        Member_ID VARCHAR(50) NOT NULL,
        Reset_Code VARCHAR(64) NOT NULL,
        Expiry_Date DATETIME NOT NULL,
        Is_Used BIT NOT NULL CONSTRAINT DF_PasswordReset_Is_Used DEFAULT (0),
        Created_Date DATETIME NOT NULL CONSTRAINT DF_PasswordReset_Created_Date DEFAULT (GETDATE())
    );

    CREATE INDEX IX_PasswordReset_Member_Used_Expiry
        ON dbo.PasswordReset (Member_ID, Is_Used, Expiry_Date);
END;
GO
