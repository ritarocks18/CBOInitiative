﻿CREATE TABLE [dbo].[SyncConfiguration] (
   [Key] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](512) NOT NULL,
	[Description] [nvarchar](512) NULL,CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

