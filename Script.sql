-- [Grade], [Student], [StudentAddress]
CREATE TABLE [dbo].[Grade](
	[GradeId] [varchar](10) NOT NULL,
	[Name] [varchar](50) NOT NULL
 CONSTRAINT [PK_Grade] PRIMARY KEY NONCLUSTERED 
(
	[GradeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[Student](
	[GradeId] [varchar](10) NOT NULL,
	[StudentId] [varchar](10) NOT NULL,
	[Name] [varchar](50) NOT NULL
 CONSTRAINT [PK_Student] PRIMARY KEY NONCLUSTERED 
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[StudentAddress](
	[StudentId] [varchar](10) NOT NULL,
	[Address] [varchar](50) NOT NULL
 CONSTRAINT [PK_StudentAddress] PRIMARY KEY NONCLUSTERED 
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT INTO [dbo].[Grade]
([GradeId],[Name])
values
('202001','Grade-A'),
('202002','Grade-B')
GO
INSERT INTO [dbo].[Student]
([GradeId],[StudentId],[Name])
values
('202001','2020010001','Student-A'),
('202001','2020010002','Student-B'),
('202002','2020020001','Student-C'),
('202002','2020020002','Student-D')
GO
INSERT INTO [dbo].[StudentAddress]
([StudentId],[Address])
values
('2020010001','Address-A'),
('2020010002','Address-B'),
('2020020001','Address-C'),
('2020020002','Address-D')
GO
-- Test
CREATE TABLE [dbo].[Test](
	[ROW] [int] IDENTITY(1,1) NOT NULL,
	[NAME] [varchar](10) NOT NULL,
    [MAKE_DATE] [datetime] NOT NULL,
    [SALE_AMT] [int] NULL,
    [SALE_DATE] [datetime] NULL,
    [TAX]decimal(10, 6) NULL,
    [REMARK] [nvarchar](10) NOT NULL,
	[UPDATE_USER_ID] [varchar](10) NULL,
	[UPDATE_PROG_CD] [varchar](10) NULL,
	[UPDATE_DATE_TIME] [datetime] NULL
 CONSTRAINT [PK_Test] PRIMARY KEY NONCLUSTERED 
(
	[ROW] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[TestMaster](
	[ROW] [int] IDENTITY(1,1) NOT NULL,
	[ID] [char](10) NOT NULL,
	[NAME] [varchar](10) NOT NULL,
    [BORN_DATE] [datetime] NULL
 CONSTRAINT [PK_TestMaster] PRIMARY KEY NONCLUSTERED 
(
	[ROW] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[TestDetail](
	[ROW] [int] IDENTITY(1,1) NOT NULL,
	[MASTER_ID] [char](10) NOT NULL,
	[ID] [char](10) NOT NULL,
	[NAME] [varchar](10) NOT NULL,
    [BORN_DATE] [datetime] NULL
 CONSTRAINT [PK_TestDetail] PRIMARY KEY NONCLUSTERED 
(
	[ROW] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT INTO [dbo].[TestMaster]
([ID],[NAME],[BORN_DATE])
values
('A123456789','A',getdate()),
('B123456789','B',getdate())
GO
INSERT INTO [dbo].[TestDetail]
([MASTER_ID],[ID],[NAME],[BORN_DATE])
values
('A123456789','C123456789','C',getdate()),
('A123456789','D123456789','D',getdate()),
('B123456789','E123456789','E',getdate()),
('B123456789','F123456789','F',getdate())
GO