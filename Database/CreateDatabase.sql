USE master;
GO

IF DB_ID(N'NasaApodGalleryDb') IS NULL
BEGIN
    CREATE DATABASE NasaApodGalleryDb;
END;
GO

USE NasaApodGalleryDb;
GO

IF OBJECT_ID(N'dbo.ApodEntries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApodEntries
    (
        Id INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_ApodEntries PRIMARY KEY,
        [Date] DATE NOT NULL,
        Title NVARCHAR(500) NOT NULL,
        Explanation NVARCHAR(MAX) NOT NULL,
        Url NVARCHAR(2048) NOT NULL,
        MediaType NVARCHAR(50) NOT NULL,
        ServiceVersion NVARCHAR(50) NULL,
        SavedAt DATETIME2 NOT NULL
            CONSTRAINT DF_ApodEntries_SavedAt DEFAULT SYSUTCDATETIME(),
        PostId INT NULL,
        Permalink NVARCHAR(2048) NULL,
        HdUrl NVARCHAR(2048) NULL,
        Credit NVARCHAR(MAX) NULL,
        Copyright NVARCHAR(MAX) NULL,
        Alt NVARCHAR(MAX) NULL,
        CONSTRAINT UQ_ApodEntries_Date UNIQUE ([Date])
    );
END;
GO
