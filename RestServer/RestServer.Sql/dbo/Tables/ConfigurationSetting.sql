CREATE TABLE [dbo].[ConfigurationSetting]
(
	[ConfigurationSettingId]					INT NOT NULL,
	[ConfigurationSection]						VARCHAR (50) NOT NULL,
	[ConfigurationValue]						VARCHAR(MAX) NOT NULL,
	[CreatedBy]									VARCHAR(50) NOT NULL, 
    [CreationDateTime]							DATETIME2 NOT NULL, 
    [LastModifiedBy]							VARCHAR(50) NULL, 
    [LastModificationDateTime]					DATETIME2 NULL,
	CONSTRAINT [PK_dbo.ConfigurationSetting]	PRIMARY KEY CLUSTERED ([ConfigurationSettingId]),
	CONSTRAINT [UK_dbo.ConfigurationSection]	UNIQUE([ConfigurationSection])
)
