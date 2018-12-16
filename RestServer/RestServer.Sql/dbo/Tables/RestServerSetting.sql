CREATE TABLE [dbo].[RestServerSetting]
(
	[Key]										VARCHAR (50) NOT NULL,
	[Value]										VARCHAR(MAX) NOT NULL,
	[CreatedBy]									VARCHAR(50) NOT NULL, 
    [CreationDateTime]							DATETIME2 NOT NULL, 
    [LastModifiedBy]							VARCHAR(50) NULL, 
    [LastModificationDateTime]					DATETIME2 NULL,
	CONSTRAINT [PK_dbo.RestServerSetting]		PRIMARY KEY CLUSTERED ([Key])
)
