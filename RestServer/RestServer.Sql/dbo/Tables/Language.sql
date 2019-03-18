CREATE TABLE [dbo].[Language]
(
	[LanguageId]					INT NOT NULL, 
    [LanguageName]					VARCHAR(50) NOT NULL, 
	[CreatedBy]						VARCHAR(50) NOT NULL, 
    [CreationDateTime]				DATETIME2 NOT NULL, 
    [LastModifiedBy]				VARCHAR(50) NULL, 
    [LastModificationDateTime]		DATETIME2 NULL,
	CONSTRAINT [PK_dbo.Language]	PRIMARY KEY CLUSTERED ([LanguageId]),
	CONSTRAINT [UK_Language]		UNIQUE ([LanguageName])
)
