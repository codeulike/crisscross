-- if you want crisscross to log report runs:
-- 1. create a database called CrissCrossHistory
-- 2. set appSetting "crisscross.StoreCrissCrossHistory" to true in web config
-- 3. set connectionString "CrissCrossHistoryDb" to point to CrissCrossHistory (with a valid login)
-- 4. run this script to create the required table


USE [CrissCrossHistory]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CrissCrossExecutionLog](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[ExecutionId] [nvarchar](128) NULL,
	[CrissCrossInstance] [nvarchar](250) NOT NULL,
	[ReportPath] [nvarchar](850) NOT NULL,
	[UserName] [nvarchar](520) NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[ParametersForUser] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CrissCrossExecutionLog] ADD  DEFAULT (getdate()) FOR [TimeStamp]
GO


