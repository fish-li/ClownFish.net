USE [MyNorthwind]
GO

/****** Object:  Table [dbo].[TestTable1]    Script Date: 07/19/2016 19:35:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TestTable1](
	[rid] [int] IDENTITY(1,1) NOT NULL,
	[intA] [int] NOT NULL,
	[timeA] [datetime] NOT NULL,
	[moneyA] [money] NOT NULL,
	[stringA] [nvarchar](50) NOT NULL,
	[boolA] [bit] NOT NULL,
	[guidA] [uniqueidentifier] NOT NULL,
	[intB] [int] NULL,
	[moneyB] [money] NULL,
	[guidB] [uniqueidentifier] NULL,
	[shortB] [smallint] NULL,
	[charA] [nchar](1) NOT NULL,
	[charB] [nchar](1) NULL,
	[img] [varbinary](max) NULL,
	[g2] [uniqueidentifier] NOT NULL,
	[ts] [timestamp] NOT NULL,
 CONSTRAINT [PK_TestTable1] PRIMARY KEY CLUSTERED 
(
	[rid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [DF_TestTable1_intA]  DEFAULT ((0)) FOR [intA]
GO

ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [DF_TestTable1_timeA]  DEFAULT (getdate()) FOR [timeA]
GO

ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [DF_TestTable1_moneyA]  DEFAULT ((0)) FOR [moneyA]
GO

ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [DF_TestTable1_stringA]  DEFAULT ('') FOR [stringA]
GO

ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [DF_TestTable1_boolA]  DEFAULT ((0)) FOR [boolA]
GO

ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [DF_TestTable1_guidA]  DEFAULT (newid()) FOR [guidA]
GO

ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [DF_TestTable1_charA]  DEFAULT ('') FOR [charA]
GO

ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [DF_TestTable1_g2]  DEFAULT (newsequentialid()) FOR [g2]
GO


