USE [MyNorthwind]
GO


CREATE TABLE [TestGuid](
    [RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[IntValue] [int] NOT NULL,
	[RowGuid] UniqueIdentifier NOT NULL,     
    CONSTRAINT [PK_TestGuid] PRIMARY KEY CLUSTERED 
    (
        [RowIndex] ASC
    )
) 
GO

/****** Object:  Table [dbo].[Categories]    Script Date: 2019/7/19 20:43:29 ******/
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)
)
GO



/****** Object:  Table [dbo].[Customers]    Script Date: 2019/7/19 20:43:29 ******/
CREATE TABLE [dbo].[Customers](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerName] [nvarchar](50) NOT NULL,
	[ContactName] [nvarchar](50) NULL,
	[Address] [nvarchar](50) NULL,
	[PostalCode] [nvarchar](10) NULL,
	[Tel] [nvarchar](50) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)
)
GO



/****** Object:  Table [dbo].[OrderDetails]    Script Date: 2019/7/19 20:43:29 ******/
CREATE TABLE [dbo].[OrderDetails](
	[OrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[ProductID] ASC
)
)
GO


/****** Object:  Table [dbo].[Orders]    Script Date: 2019/7/19 20:43:29 ******/
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[OrderDate] [datetime] NOT NULL,
	[SumMoney] [money] NOT NULL,
	[Comment] [nvarchar](300) NULL,
	[Finished] [bit] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)
)
GO


/****** Object:  Table [dbo].[Products]    Script Date: 2019/7/19 20:43:29 ******/
CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](50) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[Unit] [nvarchar](10) NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[Remark] [nvarchar](max) NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)
)
GO


/****** Object:  Table [dbo].[TestTable1]    Script Date: 2019/7/19 20:43:29 ******/
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
)
)
GO


/****** Object:  Table [dbo].[TestType]    Script Date: 2019/7/19 20:43:29 ******/
CREATE TABLE [dbo].[TestType](
	[Rid] [int] IDENTITY(1,1) NOT NULL,
	[StrValue] [nvarchar](255) NOT NULL,
	[Str2Value] [nvarchar](255) NULL,
	[TextValue] [nvarchar](max) NOT NULL,
	[Text2Value] [nvarchar](max) NULL,
	[IntValue] [int] NOT NULL,
	[Int2Value] [int] NULL,
	[ShortValue] [smallint] NOT NULL,
	[Short2Value] [smallint] NULL,
	[LongValue] [bigint] NOT NULL,
	[Long2Value] [bigint] NULL,
	[CharValue] [char](1) NOT NULL,
	[Char2Value] [char](1) NULL,
	[BoolValue] [bit] NOT NULL,
	[Bool2Value] [bit] NULL,
	[TimeValue] [datetime] NOT NULL,
	[Time2Value] [datetime] NULL,
	[DecimalValue] [money] NOT NULL,
	[Decimal2Value] [money] NULL,
	[FloatValue] [real] NOT NULL,
	[Float2Value] [real] NULL,
	[DoubleValue] [float] NOT NULL,
	[Double2Value] [float] NULL,
	[GuidValue] [uniqueidentifier] NOT NULL,
	[Guid2Value] [uniqueidentifier] NULL,
	[ByteValue] [tinyint] NOT NULL,
	[Byte2Value] [tinyint] NULL,
--	[SByteValue] [tinyint] NOT NULL,
--	[SByte2Value] [tinyint] NULL,
	[TimeSpanValue] [time] NOT NULL,
	[TimeSpan2Value] [time] NULL,
	[WeekValue] [int] NOT NULL,
	[Week2Value] [int] NULL,
	[BinValue] [varbinary](max) NOT NULL,
	[Bin2Value] [varbinary](max) NULL,
--	[UShortValue] [smallint] NOT NULL,
--	[UShort2Value] [smallint] NULL,
--	[UIntValue] [int] NOT NULL,
--	[UInt2Value] [int] NULL,
--	[ULongValue] [bigint] NOT NULL,
--	[ULong2Value] [bigint] NULL
)
GO



SET IDENTITY_INSERT Categories ON 
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (1, N'手机')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (2, N'MP3/4')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (3, N'U盘')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (4, N'存储卡')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (5, N'项链')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (6, N'手链')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (7, N'耳环')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (8, N'发饰')
SET IDENTITY_INSERT Categories OFF
GO


SET IDENTITY_INSERT Customers ON 
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (1, N'上海天民信息技术有限公司', N'吴伟谊', N'桂平路471号6号楼5楼', N'430000', N'54500599')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (2, N'上海黎敏交通设施器材有限公司', N'倪广德', N'上海市青浦区华新镇纪鹤公路3369号', N'430000', N'021-39790193')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (3, N'上海天骥（佳速空运）货运代理有限公司', N'吴娟', N'吴中路2768弄5号仓库', N'430000', N'021－64598989')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (4, N'深圳市容电电子有限公司', N'王盛', N'深圳市宝安43区A栋', N'430000', N'0755-7863400-232')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (5, N'易达兴业电子有限公司', N'王建荣', N'广州市天河南二路32号1408', N'430000', N'87564169')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (6, N'武进市华源照明器材有限公司', N'王春良', N'江苏省武进市牛塘镇', N'430000', N'86-519-6392299')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (7, N'上海东佳铸锻厂', N'罗先生', N'上海浦东大道2311号', N'430000', N'38820914')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (8, N'上海讯拓通讯设备有限公司', N'颜先生', N'上海市大连路1546号A4F座', N'430000', N'65796045')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (9, N'深圳市梅兰达实业有限公司', N'张清龙', N'深圳市南山区怡海广场东座2405室', N'430000', N'6401777/13823215275')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (10, N'美浓工贸有限公司', N'宋勇', N'长寿路1118号26_C座', N'430000', N'62110205')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (11, N'美博达科技有限公司', N'tonywu', N'深圳市南山区科技园汇景豪苑海悦阁22A', N'430000', N'0755-6966310')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (12, N'灵通网络信息技术有限公司', N'joy.chen', N'江阴市文化西路111号', N'430000', N'6800206')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (13, N'浪潮通软', N'孟令广', N'淮海中路755号新华联15楼E座', N'430000', N'64723259')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (14, N'老衲集团', N'舒服生', N'广东省深圳市福田区', N'430000', N'073532260014')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (15, N'名麟电脑元件厂', N'黎贤', N'广东省中山市坦洲镇联一工业区', N'430000', N'0760-6212910')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (16, N'雷盾电气(深圳)有限公司', N'张先生', N'深圳市红荔路莲花二村20栋503#', N'430000', N'86-755-3341040-13652')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (17, N'明明软件', N'郑小东', N'广西南宁市星湖路32号', N'430000', N'07715315327')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (18, N'欣宇科技公司', N'李先生', N'上海市胶州路397号', N'430000', N'62581722')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (19, N'星际（杭州）网络技术有限公司上海办事处', N'余先生', N'上海市浦东新区商城路660号乐凯大厦1507室', N'430000', N'021－68877785')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (20, N'徐州市万信通讯器材有限公司', N'张楠', N'徐州市淮海东路125号', N'430000', N'0516-3706987')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (21, N'信恒科技', N'王伟', N'虹桥路2323号3楼', N'430000', N'32070331')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (22, N'世码上海电子有限公司', N'stearly', N'上海市虹口区', N'430000', N'13817598281')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (23, N'世码电子有限公司', N'stearly', N'广州市洪德路2-6号新世纪商贸中心', N'430000', N'020-84328727')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (24, N'世华国际金融信息有限公司', N'吴先生', N'上海市中山西路1279弄8号12楼', N'430000', N'021-52574595*228')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (25, N'松达软件', N'杨生', N'深圳市', N'430000', N'0755-3170818')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (26, N'上外网', N'高开心', N'上海市大连西路550号', N'430000', N'55389088')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (27, N'深圳市明珠电子有限公司', N'莫生', N'南山区南新路大新大厦6楼', N'430000', N'86-755-6079487')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (28, N'深圳市闪电数码科技有限公司', N'李建军', N'深圳市桃源村13栋7楼', N'430000', N'07556789809')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (29, N'深圳市斯盛科技开发有限公司', N'陈静', N'深圳市南山区常兴路国兴大厦七、八楼', N'430000', N'07556530145')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (30, N'深圳市天灿网络开发有限公司', N'蒋先生', N'深圳华强北路彩电大厦', N'430000', N'0755-3239397')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (31, N'深圳市泰达鑫通信技术有限公司', N'毛生', N'深圳市车公庙泰然九路1号西京大厦六楼', N'430000', N'86-755-3584924')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (32, N'深圳市太之阳网络技术有限公司', N'lanny la', N'广东省深圳市福田区商报路天健公寓室', N'430000', N'0755-3937372')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (33, N'深圳市网科软件开发有限公司', N'冯先生', N'广东深圳市人民北路物资大厦507', N'430000', N'0755-2198221')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (34, N'深圳市普飞特多媒体技术开发有限公司', N'潘云', N'深圳市福田区北环大道7003号中审大厦1101', N'430000', N'07553940051')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (35, N'深圳市创远实业有限公司', N'miss zha', N'深圳市深南中路中航苑航都大厦13楼', N'430000', N'0755-3790030')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (36, N'深圳瑞集电子有限公司', N'徐志传', N'福田区燕南路404栋', N'430000', N'0755-6159877')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (37, N'深圳市捷通网络通讯服务有限公司', N'蒋桦', N'深圳市华发北路25栋416室628', N'430000', N'0755-3218178')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (38, N'深圳深信服科技有限公司', N'熊先生', N'深圳科技园科苑路24栋', N'430000', N'0755-6719445')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (39, N'深圳生溢快捷电路有限公司上海办事处', N'曹勇', N'上海市浦东南路南码头路44号303室', N'430000', N'58811501')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (40, N'深圳希望之光计算机有限公司', N'夏义威', N'深圳市华强北赛格宝华大厦612', N'430000', N'0755-3779850')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (41, N'深圳博通达电子科技发展有限公司', N'聂小姐', N'华强北群星广场B座3001室', N'430000', N'0755-3748848')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (42, N'深圳讳铨', N'宋小姐', N'深圳市赛格广场1C027', N'430000', N'0755-3797733')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (43, N'上海美承电子商务公司', N'吕先生', N'上海虎丘路50号文汇大厦18楼', N'430000', N'021-63295827')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (44, N'上海络天信息技术有限公司', N'张晓文', N'上海天山西路385弄12号', N'430000', N'52190178')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (45, N'上海满舟信息技术有限公司', N'唐文俊', N'上海市沪青平公路101弄4号楼702室', N'430000', N'021－64204200')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (46, N'上海昆成科技有限公司', N'张亮', N'天山路789号天山商厦西楼1603室', N'430000', N'13817694606')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (47, N'上海力通科技发展有限公司', N'宋小姐', N'昭化路508号307A室', N'430000', N'62408830')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (48, N'上海联杰通信设备有限公司', N'张晓平', N'上海市淮海西路442弄85号浦江大厦2202室', N'430000', N'021-52581068')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (49, N'上海录信信息科技有限公司', N'孙隽', N'上海*浦东*篮村路471弄6号307室', N'430000', N'58897265')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (50, N'上海明星光电缆有限公司', N'张金龙', N'上海奉贤星火农场', N'430000', N'57503335  1390170855')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (51, N'上海明沪科技发展有限公司', N'陈保和', N'中山北一路1250#沪办大厦2#楼1309', N'430000', N'021-55389516')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (52, N'上海明亚计算机科技有限公司', N'陶文斌', N'上海市天目西路547号恒基不夜城B座逸昇阁70', N'430000', N'33030860、1300219782')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (53, N'上海青鸟信息技术(咨询)有限公司', N'georgexu', N'上海市普陀区石泉路450弄12号', N'430000', N'021-52914266')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (54, N'上海清华科睿实业有限公司', N'王以捷', N'肇嘉浜路825号宏业商务中心1号楼5-EF座', N'430000', N'021-64863118')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (55, N'上海千晟电子有限公司', N'张晓松', N'上海市四川北路2115号221室', N'430000', N'021-36080289')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (56, N'上海新动信息技术有限公司', N'姜伟莉', N'上海江宁路420号15E', N'430000', N'62728722')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (57, N'上海新华电话交换机厂', N'蒋向群', N'上海市宁夏路白玉新村129号', N'430000', N'62601416')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (58, N'上海轩东新技术开发有限公司', N'潘鹏', N'南丹路189弄2号1802', N'430000', N'64383279')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (59, N'上海信思网络通信有限公司', N'盛玉春', N'博山东路41号8座504室', N'430000', N'02158856185')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (60, N'上海协航通信技术有限公司', N'王先生', N'武宁路423号', N'430000', N'021-62547450')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (61, N'上海闪讯计算机信息技术有限公司', N'邱珏敏', N'上海市马当路525号', N'430000', N'021-63842686')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (62, N'上海市艺捷电脑科技有限公司', N'乐艺，张', N'上海市黄兴路1616号百安居数码广场221室', N'430000', N'55093617')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (63, N'上海市南洋高科技有限公司', N'吴红泉', N'上海市奉贤区南桥路266号', N'430000', N'37100373')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (64, N'上海热线应用数据中心', N'魏小姐', N'浦东松林路357号通贸大酒店26楼', N'430000', N'58351556，58355414，')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (65, N'上海仕达软件有限公司', N'苏列章', N'上海市康定路1201弄2号1504室', N'430000', N'021-62301411')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (66, N'_88水晶珠宝有限公司', N'何正兵', N'中国江苏连云港东海县徐海路新新巷37-18号', N'430000', N'7288890')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (67, N'联邦师教文具用品工场', N'丁峰', N'中国上海浦东新区金桥路244-2号', N'430000', N'021-58509084')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (68, N'青岛泽瑞木制工艺礼品有限公司', N'丁斌', N'青岛胶州市李哥庄镇前辛疃工业园', N'430000', N'0532-8296456')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (69, N'青岛泽瑞木制工艺品有限公司', N'丁斌', N'青岛胶州李哥庄镇前辛疃工业园　', N'430000', N'0532-8296456')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (70, N'青岛高艺彩绘转印有限公司', N'崔先生', N'青岛市山东路52号华嘉大厦805室', N'430000', N'+86-532-5012030')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (71, N'新时代家庭用品有限公司', N'赵先生', N'扬州市文昌中路133号', N'430000', N'514-7339941')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (72, N'兴化八达现代办公设备公司', N'朱跃璜', N'江苏兴化英武路十二号楼', N'430000', N'0523-3233425')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (73, N'鹭厅玩具/礼品', N'吴先生', N'温州苍南金龙大道[第一工业区]', N'430000', N'0577-82871108')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (74, N'鹭厅印务', N'WSFSAF', N'浙江省温州苍南', N'430000', N'057764578042')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (75, N'如皋市陆姚软木制品厂', N'张斌', N'江苏如皋市何庄', N'430000', N'0513-7381013')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (76, N'深圳市东科办公用品有限公司', N'宋先生', N'深圳市罗湖区田贝四路水田一街十号二楼', N'430000', N'07555531772')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (77, N'深圳市精良实业有限公司', N'沈先生', N'梅林新阁小区新阁大厦16层1619室', N'430000', N'0755-3311709')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (78, N'深圳雅格威皮具有限公司', N'钟卫平', N'深圳市福田区新闻路景苑大厦B座1001', N'430000', N'013823138365')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (79, N'上海良会贸易有限公司', N'庄雯雯', N'晋元路310-1-1205', N'430000', N'63174491')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (80, N'上海前伟园艺有限公司', N'高先生', N'上海嘉定马陆镇包桥村', N'430000', N'021-39150375')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (81, N'上海星汉工艺品有限公司', N'郭岗', N'上海市虹桥路1017号3号楼1104室', N'430000', N'（86-21）62194002')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (82, N'上海雄方科技有限公司', N'曾爱国', N'四川北路1885号', N'430000', N'021-56667802')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (83, N'上海心石珠宝有限公司', N'陈继平', N'上海市局门路434弄10号304室', N'430000', N'021-63018728')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (84, N'上海市现代娱乐股份有限公司', N'季冬冬', N'上海市虹口区大炮路565号', N'430000', N'021－4823742')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (85, N'上海瑞讯工贸有限公司', N'陈海清', N'上海闵行区剑川路1208号', N'430000', N'54713127')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (86, N'上海市普陀区攀高工艺品厂', N'张守东', N'上海市静安区安远路839号三楼', N'430000', N'021-62318210')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (87, N'上海仁久工贸有限公司', N'陈理腾', N'浙江北路368号', N'430000', N'63937430')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (88, N'上海森普经贸实业有限公司', N'周先生', N'上海市静安区安远路501弄5号21楼D座', N'430000', N'32271170')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (89, N'上海森大商务有限公司', N'熊耕发', N'上海市棕榈路451弄39号101室', N'430000', N'52983245')
INSERT [dbo].[Customers] ([CustomerID], [CustomerName], [ContactName], [Address], [PostalCode], [Tel]) VALUES (90, N'上海顺誉贸易有限公司', N'王先生', N'上海市海宁路165号', N'430000', N'63070903')
SET IDENTITY_INSERT Customers OFF
GO


INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (1, 2, 4400.0000, 5)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (1, 3, 4550.0000, 4)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (1, 157, 51.5000, 3)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (1, 158, 102.8000, 6)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (2, 160, 50.0000, 2)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (2, 161, 35.0000, 4)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (2, 163, 33.0000, 5)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (3, 469, 84.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (3, 470, 141.0000, 4)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (3, 473, 72.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (3, 475, 40.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (3, 476, 58.0000, 7)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (4, 159, 59.0000, 3)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (4, 160, 50.0000, 6)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (4, 161, 35.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (4, 162, 164.0000, 9)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (5, 79, 1600.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (5, 80, 1080.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (5, 82, 1560.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (5, 83, 1085.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (5, 471, 120.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (5, 472, 103.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (5, 474, 118.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (5, 475, 40.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (6, 12, 350.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (6, 13, 554.5000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (6, 15, 1340.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (6, 82, 1560.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (6, 83, 1085.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (6, 473, 72.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (6, 474, 118.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (6, 475, 40.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (7, 479, 68.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (7, 480, 85.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (7, 481, 65.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (7, 482, 48.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (7, 486, 48.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (7, 549, 37.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (7, 550, 115.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (8, 391, 185.0000, 7)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (8, 392, 98.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (8, 393, 141.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (8, 394, 108.0000, 9)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (8, 395, 36.6000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (8, 396, 36.6000, 5)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (9, 236, 50.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (9, 237, 26.8000, 5)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (9, 238, 22.8000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (9, 549, 37.0000, 6)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (9, 550, 115.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (9, 551, 27.0000, 8)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (9, 552, 33.0000, 9)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (10, 391, 185.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (10, 392, 98.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (10, 393, 141.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (10, 394, 108.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (10, 396, 36.6000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (10, 549, 37.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (10, 550, 115.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (11, 313, 36.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (11, 315, 280.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (11, 469, 84.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (11, 470, 141.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (11, 474, 118.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (11, 475, 40.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (12, 550, 115.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (12, 554, 38.1800, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (12, 555, 56.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (12, 560, 58.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (12, 561, 29.8000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (13, 9, 1160.0000, 3)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (13, 157, 51.5000, 5)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (13, 158, 102.8000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (14, 162, 164.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (14, 163, 33.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (14, 165, 29.9000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (14, 315, 280.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (14, 320, 130.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (14, 321, 899.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (14, 322, 35.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (15, 13, 554.5000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (15, 15, 1340.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (15, 18, 888.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (15, 313, 36.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (15, 314, 53.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (15, 315, 280.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (16, 168, 55.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (16, 169, 55.0000, 5)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (16, 170, 40.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (16, 171, 41.0000, 4)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (16, 172, 55.0000, 6)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (17, 313, 36.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (17, 315, 280.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (17, 316, 79.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (17, 319, 38.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (17, 320, 130.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (18, 472, 103.0000, 6)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (19, 550, 115.0000, 5)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (19, 555, 56.0000, 6)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (19, 556, 43.8000, 7)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (20, 470, 141.0000, 11)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (20, 471, 120.0000, 12)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (20, 472, 103.0000, 12)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (20, 474, 118.0000, 14)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (20, 476, 58.0000, 15)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (21, 82, 1560.0000, 7)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (21, 83, 1085.0000, 8)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (21, 86, 1150.0000, 9)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (21, 88, 1596.0000, 10)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (22, 170, 40.0000, 10)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (22, 171, 41.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (22, 176, 102.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (22, 564, 180.0000, 20)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (22, 565, 36.0000, 30)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (23, 470, 141.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (24, 158, 102.8000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (25, 391, 185.0000, 20)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (25, 393, 141.0000, 30)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (25, 394, 108.0000, 40)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (26, 469, 84.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (27, 392, 98.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (27, 393, 141.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (28, 393, 141.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (28, 394, 108.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (28, 397, 30.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (28, 398, 318.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (29, 481, 65.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (29, 482, 48.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (29, 484, 29.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (29, 486, 48.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (29, 487, 29.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (29, 488, 72.5000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (30, 315, 280.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (30, 316, 79.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (31, 316, 79.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (31, 320, 130.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (31, 321, 899.0000, 1)
INSERT [dbo].[OrderDetails] ([OrderID], [ProductID], [UnitPrice], [Quantity]) VALUES (31, 322, 35.0000, 1)
GO


SET IDENTITY_INSERT Orders ON 
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (1, 1, CAST(N'2010-12-30T10:16:26.580' AS DateTime), 40971.3000, N'已对 C# 编译器进行了多处改进，以便消除不符合语言规范的地方。其中一些改进是重大更改，而其他改进则只是软件更新或增强。有关重大更改的更多信息，请参见 Visual C# 2008 重大更改。有关在 Service Pack 1 中修复的其他 Bug 的更多信息，请参见 Visual Studio 2008 Service Pack 1 Beta for Visual C# 更改及已修复问题列表。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (2, 3, CAST(N'2010-12-30T10:16:51.373' AS DateTime), 405.0000, N'Visual C# 2008 Service Pack 1 引入了一个新功能 - 实时语义错误，该功能可提供更为完整的一组关于代码的错误信息。此功能检查以前仅在生成后报告的表达式级错误。在您编写代码时，该功能用红色波浪下划线突出显示错误。有关波浪下划线的更多信息，请参见编辑代码 (Visual C#)。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (3, 5, CAST(N'2010-12-30T10:17:20.560' AS DateTime), 1166.0000, N'在 Visual Studio 的早期版本中，仅用打开文件内的任务注释填充任务列表。在 Visual Studio 2008 Service Pack 1 中，C# 集成开发环境 (IDE) 显示解决方案中所有打开的及关闭的文件中的任务注释。有关更多信息，请参见如何：创建任务列表注释。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (4, 10, CAST(N'2010-12-30T10:17:45.907' AS DateTime), 1988.0000, N'Visual Studio 2008 Service Pack 1 允许您使用重命名重构功能对 XAML 中定义的引用进行重命名。有关重命名重构的更多信息，请参见重命名。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (5, 5, CAST(N'2010-12-30T10:18:20.903' AS DateTime), 5706.0000, N'Visual C# 2008 Service Pack 1 通过禁用基元值转换和显式定义的用户转换来提高 Enumerable.Cast<T> 方法的性能。从 int 数据类型到 long 数据类型的转换便是一个基元值转换的示例。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (6, 6, CAST(N'2010-12-30T10:18:54.280' AS DateTime), 5119.5000, N'C# 3.0 语言和编译器引入了多种新的语言功能。这些新的语言构造可以分别用在各种上下文中，并且可以共同完成语言集成查询 (LINQ)。有关 LINQ 的更多信息，请参见 The LINQ Project（LINQ 项目）。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (7, 12, CAST(N'2010-12-30T10:19:24.700' AS DateTime), 466.0000, N'Visual Studio 2008 允许您为项目指定 .NET Framework 的版本（即 .NET Framework 2.0、3.0 或 3.5）。应用程序的 .NET Framework 目标是在计算机上运行此应用程序时所需的 .NET Framework 的版本。有关更多信息，请参见以特定的 .NET Framework 为目标。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (8, 4, CAST(N'2010-12-30T10:19:53.280' AS DateTime), 2725.6000, N'为 Windows Presentation Foundation、Windows Communication Foundation 和 Web 项目提供了多个新的项目模板。有关更多信息，请参见 Visual C# 版本中的项目模板和 Visual Studio 中的默认项目模板。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (9, 6, CAST(N'2010-12-30T10:20:28.767' AS DateTime), 1056.8000, N'在 Visual C# 2008 及更低版本中，类型推理会导致从方法重载决策过程中排除指针类型的数组。在下面的代码中，Visual C# 2005 编译器会选择 Test 的非泛型版本，原因是 Test 的泛型版本由于具有类型参数 int*[] 而不予考虑。而在 Visual C# 2008 中会选择 Test 的泛型版本。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (10, 16, CAST(N'2010-12-30T10:21:01.263' AS DateTime), 720.6000, N'Visual C# 2008 文档包含特定于 C# 语言的信息，例如关键字、编译器选项、错误消息和编程概念。此文档还向您提供了有关如何使用集成开发环境 (IDE) 的概述。此外，还有许多链接指向有关以下内容的更加详细的帮助：.NET Framework 类、ASP.NET Web 开发、调试、SQL 数据库编程以及更多。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (11, 10, CAST(N'2010-12-30T10:21:31.357' AS DateTime), 699.0000, N'C# 是一种简洁、类型安全的面向对象的语言，开发人员可以使用它来构建在 .NET Framework 上运行的各种安全、可靠的应用程序。使用 C#，您可以创建传统的 Windows 客户端应用程序、XML Web services、分布式组件、客户端/服务器应用程序、数据库应用程序等等。Visual C# 2008 提供了高级代码编辑器、方便的用户界面设计器、集成调试器和许多其他工具，使您可以更容易在 C# 语言 3.0 版和 .NET Framework 3.5 版的基础上开发应用程序。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (12, 7, CAST(N'2010-12-30T10:22:01.500' AS DateTime), 296.9800, N'C# 语法表现力强，而且简单易学。C# 的大括号语法使任何熟悉 C、C++ 或 Java 的人都可以立即上手。了解上述任何一种语言的开发人员通常在很短的时间内就可以开始使用 C# 高效地进行工作。C# 语法简化了 C++ 的诸多复杂性，并提供了很多强大的功能，例如可为 null 的值类型、枚举、委托、lambda 表达式和直接内存访问，这些都是 Java 所不具备的。C# 支持泛型方法和类型，从而提供了更出色的类型安全和性能。C# 还提供了迭代器，允许集合类的实施者定义自定义的迭代行为，以便容易被客户端代码使用。在 C# 3.0 中，语言集成查询 (LINQ) 表达式使强类型查询成为了一流的语', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (13, 18, CAST(N'2010-12-30T10:22:31.217' AS DateTime), 3840.3000, N'作为一种面向对象的语言，C# 支持封装、继承和多态性的概念。所有的变量和方法，包括 Main 方法（应用程序的入口点），都封装在类定义中。类可能直接从一个父类继承，但它可以实现任意数量的接口。重写父类中的虚方法的各种方法要求 override 关键字作为一种避免意外重定义的方式。在 C# 中，结构类似于一个轻量类；它是一种堆栈分配的类型，可以实现接口，但不支持继承。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (14, 5, CAST(N'2010-12-30T10:23:13.827' AS DateTime), 1570.9000, N'在 C# 中，如果必须与其他 Windows 软件（如 COM 对象或本机 Win32 DLL）交互，则可以通过一个称为“互操作”的过程来实现。互操作使 C# 程序能够完成本机 C++ 应用程序可以完成的几乎任何任务。在直接内存访问必不可少的情况下，C# 甚至支持指针和“不安全”代码的概念。

C# 的生成过程比 C 和 C++ 简单，比 Java 更为灵活。没有单独的头文件，也不要求按照特定顺序声明方法和类型。C# 源文件可以定义任意数量的类、结构、接口和事件。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (15, 20, CAST(N'2010-12-30T10:23:41.437' AS DateTime), 3151.5000, N'C# 程序在 .NET Framework 上运行，它是 Windows 的一个不可或缺的组件，包括一个称为公共语言运行库 (CLR) 的虚拟执行系统和一组统一的类库。CLR 是 Microsoft 的公共语言基础结构 (CLI) 的商业实现。CLI 是一种国际标准，是用于创建语言和库在其中无缝协同工作的执行和开发环境的基础。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (16, 19, CAST(N'2010-12-30T10:24:16.903' AS DateTime), 864.0000, N'用 C# 编写的源代码被编译为一种符合 CLI 规范的中间语言 (IL)。IL 代码与资源（例如位图和字符串）一起作为一种称为程序集的可执行文件存储在磁盘上，通常具有的扩展名为 .exe 或 .dll。程序集包含清单，它提供有关程序集的类型、版本、区域性和安全要求等信息。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (17, 15, CAST(N'2010-12-30T10:24:51.013' AS DateTime), 563.0000, N'执行 C# 程序时，程序集将加载到 CLR 中，这可能会根据清单中的信息执行不同的操作。然后，如果符合安全要求，CLR 就会执行实时 (JIT) 编译以将 IL 代码转换为本机机器指令。CLR 还提供与自动垃圾回收、异常处理和资源管理有关的其他服务。由 CLR 执行的代码有时称为“托管代码”，它与编译为面向特定系统的本机机器语言的“非托管代码”相对应。下图阐释了 C# 源代码文件、.NET Framework 类库、程序集和 CLR 的编译时与运行时的关系。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (18, 6, CAST(N'2010-12-30T10:25:15.907' AS DateTime), 618.0000, N'语言互操作性是 .NET Framework 的一项主要功能。因为由 C# 编译器生成的 IL 代码符合公共类型规范 (CTS)，因此从 C# 生成的 IL 代码可以与从 Visual Basic、Visual C++、Visual J# 的 .NET 版本或者其他 20 多种符合 CTS 的语言中的任何一种生成的代码进行交互。单一程序集可能包含用不同 .NET 语言编写的多个模块，并且类型可以相互引用，就像它们是用同一种语言编写的。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (19, 2, CAST(N'2010-12-30T10:25:42.607' AS DateTime), 1217.6000, N'除了运行时服务之外，.NET Framework 还包含一个由 4000 多个类组成的内容详尽的库，这些类被组织为命名空间，为从文件输入和输出、字符串操作、XML 分析到 Windows 窗体控件的所有内容提供了各种有用的功能。典型的 C# 应用程序使用 .NET Framework 类库广泛地处理常见的“日常”任务。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (20, 5, CAST(N'2010-12-30T10:26:35.420' AS DateTime), 6749.0000, N'Visual C# 集成开发环境 (IDE) 是一种通过常用用户界面公开的开发工具的集合。有些工具是与其他 Visual Studio 语言共享的，还有一些工具（如 C# 编译器）是 Visual C# 特有的。本节中的文档提供如何在使用 IDE 时针对开发过程的各个阶段使用最重要的 Visual C# 工具的概述。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (21, 14, CAST(N'2010-12-30T10:27:09.513' AS DateTime), 45910.0000, N'如果您正在开发 ASP.NET 2.0 Web 应用程序，将会使用 Visual Web Developer IDE，它是 Visual Studio 的一个完全集成部分。但是，如果您的代码隐藏页是用 Visual C# 编写的，则会使用 Visual Web Developer 中的 Visual C# 代码编辑器。因此，本节中的某些主题（如设计用户界面 (Visual C#)）可能不适用于 Web 应用程序。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (22, 7, CAST(N'2010-12-30T10:27:39.170' AS DateTime), 5223.0000, N'可以使用 ASP.NET 网页作为 Web 应用程序的可编程用户接口。ASP.NET 网页在任何浏览器或客户端设备中向用户提供信息，并使用服务器端代码来实现应用程序逻辑。ASP.NET 网页有下列特点：', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (23, 1, CAST(N'2010-12-30T10:27:50.390' AS DateTime), 141.0000, N'基于 Microsoft ASP.NET 技术。在该技术中，在服务器上运行的代码动态地生成到浏览器或客户端设备的网页输出。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (24, 2, CAST(N'2010-12-30T10:28:00.217' AS DateTime), 102.8000, N'兼容所有浏览器或移动设备。ASP.NET 网页自动为样式、布局等功能呈现正确的、符合浏览器的 HTML。此外，您还可以将 ASP.NET 网页设计为在特定浏览器（如 Microsoft Internet Explorer 6）上运行并利用浏览器特定的功能。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (25, 6, CAST(N'2010-12-30T10:28:31.060' AS DateTime), 12250.0000, N'兼容 .NET 公共语言运行时所支持的任何语言，其中包括 Microsoft Visual Basic、Microsoft Visual C#、Microsoft J# 和 Microsoft JScript .NET。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (26, 10, CAST(N'2010-12-30T10:28:43.937' AS DateTime), 84.0000, N'基于 Microsoft .NET Framework 生成。它提供了 Framework 的所有优点，包括托管环境、类型安全性和继承。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (27, 6, CAST(N'2010-12-30T10:28:55.030' AS DateTime), 239.0000, N'在 ASP.NET 网页中，用户界面编程分为两个部分：可视组件和逻辑。如果您以前使用过类似于 Visual Basic 和 Visual C++ 的工具，您将认同在页的可视部分和页后与之交互的代码之间存在这样一种划分。', 1)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (28, NULL, CAST(N'2010-12-30T10:29:03.317' AS DateTime), 597.0000, N'可视元素由一个包含静态标记（例如 HTML 或 ASP.NET 服务器控件或两者）的文件组成。ASP.NET 网页用作要显示的静态文本和控件的容器。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (29, NULL, CAST(N'2010-12-30T10:29:20.450' AS DateTime), 291.5000, N'ASP.NET 网页的逻辑由代码组成，这些代码由您创建以与页进行交互。代码可以驻留在页的 script 块中或者单独的类中。如果代码在单独的类文件中，则该文件称为“代码隐藏”文件。代码隐藏文件中的代码可以使用 Visual Basic、 Visual C#、Visual J# 或 JScript .NET 编写。有关如何构建 ASP.NET 网页的更多信息，请参见 ASP.NET 网页代码模型。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (30, NULL, CAST(N'2010-12-30T10:29:29.180' AS DateTime), 359.0000, N'ASP.NET 网页编译为动态链接库 (.dll) 文件。用户第一次浏览到 .aspx 页时，ASP.NET 自动生成表示该页的 .NET 类文件，然后编译此文件。.dll 文件在服务器上运行，并动态生成页的 HTML 输出。有关如何编译 ASP.NET 应用程序的更多信息，请参见 ASP.NET 编译概述。', 0)
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [SumMoney], [Comment], [Finished]) VALUES (31, 17, CAST(N'2010-12-30T10:29:45.657' AS DateTime), 1143.0000, N'Web 应用程序编程带来了一些特殊的难题，在对传统的基于客户端的应用程序进行编程时，通常不会遇到这些难题。这些难题包括：
实现多样式的 Web 用户界面   使用基本的 HTML 功能来设计和实现用户接口既困难又费事，特别是在页具有复杂布局且包含大量动态内容和功能齐全的用户交互对象时。
客户端与服务器的分离   在 Web 应用程序中，客户端（浏览器）和服务器是不同的程序，它们通常在不同的计算机（甚至不同的操作系统）上运行。因此，共同组成应用程序的这两个部分仅共享很少的信息；它们可以进行通信，但通常只交换很小块的简单信息。
', 0)
SET IDENTITY_INSERT Orders OFF
GO



SET IDENTITY_INSERT Products ON 
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (1, N'四钻 iphone 8GB 3G美版，送卡贴！', 1, N'个', 4450.0000, N'', 78)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (2, N'iPhone 苹果 3G 8G版另有16G 只售全新机电话13810670132', 1, N'个', 4400.0000, N'', 35)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (3, N'【双钻信誉】完美原装iPhone 3G 8g 送卡帖【超值热卖】好漂亮哦', 1, N'个', 4550.0000, N'', 75)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (4, N'皇冠 APPLE iphone 8G 免卡贴 原包装 三码合一 苹果手机 AP08', 1, N'个', 5200.0000, N'', 44)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (5, N'【无忧通讯】苹果 iPhone 3G 二代 8G/16G 黑色/白色', 1, N'个', 4250.0000, N'', 101)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (6, N'【东方电子】 纽曼数码RV30录音笔（2G）㊣全新正品　五钻信誉', 1, N'个', 365.0000, N'', 69)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (7, N'[五钻]全新摩托罗拉 CDMA moto K2 全新正品！特价480', 1, N'个', 600.0000, N'', 56)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (8, N'大陆行货4钻信誉7天包退换◤ 摩托罗拉 K1★2电2充耳机1G卡带发票', 1, N'个', 580.0000, N'', 46)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (9, N'全新摩托罗拉V9手机 红黑蓝有港行【科希手机网】冲三皇冠实体店', 1, N'个', 1160.0000, N'', 35)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (10, N'港行摩托罗拉A1600 4G卡1860元◥◣深圳通讯◢◤独家免费安装地图', 1, N'个', 1860.0000, N'', 45)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (11, N'MOTO K1 摩托罗拉 双电双充送2G卡 包邮499元', 1, N'个', 520.0000, N'', 75)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (12, N'己成交几百台皆~好评 可上网验证 港版V3ie 另购1G卡豪华配置', 1, N'个', 350.0000, N'', 62)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (13, N'【冲钻】摩托罗拉 V3ie 大陆行货 全国联保 带发票 七天包退换', 1, N'个', 554.5000, N'', 79)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (14, N'摩托罗拉A1200/A1200R双电双充送2G卡蓝牙耳机读卡器三层保护膜', 1, N'个', 1130.0000, N'', 73)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (15, N'【翔宇数码】摩托罗拉 Z10[迎新年送厚礼]砖石信誉+100好评', 1, N'个', 1340.0000, N'', 22)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (16, N'镜面鹅卵石 全新摩托罗拉U9手机 黑红紫港行热卖 ◆冲三皇冠实体', 1, N'个', 755.0000, N'', 102)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (17, N'《实体店销售》100%原装 原厂特惠摩托罗拉V8（2G）版送三件套', 1, N'个', 1100.0000, N'', 55)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (18, N'摩托罗拉 E6全国联保100%好评', 1, N'个', 888.0000, N'', 21)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (19, N'全新原装★摩托罗拉 L72★全配 有小礼物', 1, N'个', 546.0000, N'', 93)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (20, N'三钻100+%消保 摩托罗拉V3ie 可选\两电两冲耳机数据线1G卡', 1, N'个', 378.0000, N'', 63)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (21, N'摩托罗拉 A1200双电双充 送数据线 线耳机 蓝牙耳机送2G卡', 1, N'个', 1180.0000, N'', 59)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (22, N'摩托罗拉V8(512)(2G)/ 进来看下/否则你会后悔/全套卖990元！！', 1, N'个', 1050.0000, N'', 35)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (23, N'【实体店】MOTO Z6 2原电2充2048M内存 全国可货到付款', 1, N'个', 650.0000, N'', 104)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (24, N'K2紫色到货◥◣冲皇冠 100%好评◢◤K2只卖【500元【500元。', 1, N'个', 600.0000, N'', 45)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (25, N'☆冲皇冠☆ 深圳实体CDMA 摩托罗拉K2 送2G卡包邮 金色到货', 1, N'个', 600.0000, N'', 71)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (26, N'摩托罗拉 Z6 两原电 两充 耳机 数据线 2G卡 658元包邮', 1, N'个', 602.0000, N'', 115)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (27, N'★摩托罗拉 K2★ 幻镜魅力两电两充耳机送2G~冲皇冠特价包邮', 1, N'个', 600.0000, N'', 47)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (28, N'[三仁电子皇冠热推]全新摩托罗拉 K1 原电原充颜色齐全', 1, N'个', 530.0000, N'', 100)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (29, N'◥◣原装港版V 3ie 选配双电双充1GB卡◢◤消保成交已近600台', 1, N'个', 350.0000, N'', 45)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (30, N'全新MOTO E6超值特卖冲四皇冠★网逗手机支付宝第一旗舰店', 1, N'个', 1250.0000, N'', 66)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (31, N'摩托罗拉 V8【套餐选择包您满意】████████████', 1, N'个', 1300.0000, N'', 30)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (32, N'冲5钻 全新 摩托罗拉 Z3 宝蓝魅力 绝美滑盖 2电2冲1G卡 消保商家', 1, N'个', 470.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (33, N'【四钻信誉100%好评】 摩托罗拉 ROKR U9 配置多选！金色到货', 1, N'个', 684.0000, N'', 117)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (34, N'【促销：两原装电池送1G卡】MOTO-A1200/A1200E 全国联保★带发票', 1, N'个', 923.0000, N'', 108)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (35, N'摩托罗拉 黄金版V8（2G）送原装耳机 三件套', 1, N'个', 1260.0000, N'', 23)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (36, N'摩托罗拉 ROKR E8魔幻界面内有港行发票【魔遇数码坊】钻石', 1, N'个', 1185.0000, N'', 49)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (37, N'摩托罗拉v3ie两电两充+耳机送1G卡 读卡器/！火爆成交', 1, N'个', 360.0000, N'', 75)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (38, N'原装MOTO【赠1G卡+读卡器+包邮费】 蓝牙耳机 送手机套', 1, N'个', 488.0000, N'', 36)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (39, N'摩托罗拉 ROKR E2 另有 两电两充耳机数据线送1G卡 520元送读卡器', 1, N'个', 407.0000, N'', 30)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (40, N'鹅卵石Linux音乐手机 摩托罗拉 U9 两电两充 送金士顿1G卡!!', 1, N'个', 718.0000, N'', 99)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (41, N'摩托罗拉 E6', 1, N'个', 3000.0000, N'', 104)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (42, N'全新摩托罗拉V3ie 可上网验证 二原电二充耳机另有送1G卡', 1, N'个', 355.0000, N'', 77)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (43, N'摩托罗拉 V8（2G）全新行货，全套配置，包开发票', 1, N'个', 1570.0000, N'', 75)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (44, N'冲皇冠 特价 摩托罗拉 W270 正行货 全国联保 新款', 1, N'个', 374.0000, N'', 60)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (45, N'全新摩托罗拉 Z6(新开业特价销售)', 1, N'个', 595.0000, N'', 37)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (46, N'【100%商家认证+消保卖家】全新摩托罗拉 ROKR U9双电双充1G卡', 1, N'个', 730.0000, N'', 97)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (47, N'MOTO K1 摩托罗拉K1 双电双充2G卡 免邮费送礼品', 1, N'个', 500.0000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (48, N'★◥◣冲皇冠◢◤★中国电信CDMA摩托罗拉 V3c超值特价', 1, N'个', 290.0000, N'', 61)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (49, N'冲三钻 摩托罗拉 ROKR U9 消保卖家+100%好评', 1, N'个', 595.0000, N'', 103)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (50, N'★冲钻★ CDMA 摩托罗拉K2 送【2G卡+读卡器】 全套配置=510元', 1, N'个', 600.0000, N'', 22)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (51, N'【冲皇冠100%信誉】摩托罗拉 K2 只卖【500元【500元 配置如下', 1, N'个', 600.0000, N'', 14)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (52, N'双皇冠 摩托罗拉A1600 双电 带发票全国联保 现货', 1, N'个', 2300.0000, N'', 106)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (53, N'*中国电信年末促销支持189号*全新摩托罗拉 MOTO V3c批发', 1, N'个', 299.0000, N'', 16)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (54, N'◣金鸿◥全新原装摩托罗拉1200◆2电2充◆2G卡 蓝牙耳机读卡器', 1, N'个', 960.0000, N'', 16)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (55, N'摩托罗拉 RAZR2 V9皇冠100%好评', 1, N'个', 1350.0000, N'', 31)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (56, N'摩托罗拉 K1两电两充数据线送耳机只卖420送', 1, N'个', 450.0000, N'', 42)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (57, N'全新摩托罗拉 A1200/A1200e 大陆行货★全国联保★带发票', 1, N'个', 1020.0000, N'', 14)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (58, N'皇冠信誉 全新原装全套摩托罗拉 V3ie 两电一充 包邮费!!!', 1, N'个', 550.0000, N'', 28)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (59, N'4钻原装摩托罗拉A1200A1200R两原电2充蓝牙2G卡售999元不满意退货', 1, N'个', 1130.0000, N'', 42)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (60, N'特惠【中国电信 CDMA】全新MOTO k 2两原电俩充全套 送1G卡', 1, N'个', 448.0000, N'', 116)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (61, N'全新【摩托罗拉 Z3】两电 两充 耳机 数据线', 1, N'个', 414.0000, N'', 105)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (62, N'【皇冠信誉】摩托罗拉 黄金版 RAZR2 V8 手机 送大礼包', 1, N'个', 1288.0000, N'', 43)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (63, N'全新100%原装☆摩托罗拉E2【2原电1充送耳机1G卡】★热销中▲', 1, N'个', 478.0000, N'', 99)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (64, N'让女人尖叫的手机 摩托罗拉 U9 超低价 港版 送耳机 特价680元', 1, N'个', 740.0000, N'', 96)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (65, N'冲三钻 摩托罗拉 V3ie 大陆行货 全国联保 带发票', 1, N'个', 557.0000, N'', 5)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (66, N'5钻~!全新大陆行货摩托罗拉A1800 CDMA/GSM双模带发票全国联保', 1, N'个', 3320.0000, N'', 82)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (67, N'摩托罗拉 E6', 1, N'个', 1500.0000, N'', 22)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (68, N'摩托罗拉V8 全套+蓝牙耳机 原装三件套 玫瑰金全新原装到货', 1, N'个', 1460.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (69, N'摩托罗拉 Z8 原电 原充 送卡包邮', 1, N'个', 966.0000, N'', 55)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (70, N'摩托罗拉V70两电一充耳机', 1, N'个', 260.0000, N'', 78)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (71, N'100%全新原装◆摩托罗拉V8（512M）无条件包退换（支持货到付款）', 1, N'个', 1350.0000, N'', 31)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (72, N'全新原装摩托罗拉K1红 蓝520淘宝最低（送1G卡） 金色限量有货', 1, N'个', 520.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (73, N'香港天域冲皇冠●正规港行 摩托罗拉ZN5 500像素 全国联保', 1, N'个', 2180.0000, N'', 100)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (74, N'摩托罗拉 V8颜色全双原电原充送蓝牙耳机3件套包邮费', 1, N'个', 1260.0000, N'', 8)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (75, N'RAZR2代 热卖全新摩托罗拉V8 2G 手机 四色有港行◆冲三皇冠实体', 1, N'个', 1265.0000, N'', 78)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (76, N'摩托罗拉 V3ie另送1G卡（黑色，银灰色，红色）', 1, N'个', 495.0000, N'', 101)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (77, N'◥◣三钻/100%/消保/◢◤特价摩托罗拉Z6两电两冲送2G卡', 1, N'个', 540.0000, N'', 82)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (78, N'摩托罗拉 E6 大陆行货 全国联保 带正规发票 送礼品', 1, N'个', 1100.0000, N'', 47)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (79, N'苹果 I POD TOUCH 8GB', 2, N'个', 1600.0000, N'', 55)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (80, N'徐家汇实体店* ipod nano 4代 8G 大红色 全新带港票联保', 2, N'个', 1080.0000, N'', 82)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (81, N'徐家汇实体店*大陆行货 苹果iPod shuffle4代 2G全国联保', 2, N'个', 485.0000, N'', 50)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (82, N'*实体店*苹果 MP4 touch二代8G 全新未拆 带港票 全国联保', 2, N'个', 1560.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (83, N'iPod nano 4代 8G 大陆行货CH/A、全国联保！', 2, N'个', 1085.0000, N'', 50)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (84, N'iPod Shuffle 3代 1G银 大陆行货CH/A全国联保！', 2, N'个', 345.0000, N'', 118)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (85, N'iPod shuffle 4代 1G 4色 大陆行货CH/A、全国联保 承接采购', 2, N'个', 345.0000, N'', 56)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (86, N'苹果 iPod Nano第4代 8G 红色', 2, N'个', 1150.0000, N'', 72)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (87, N'ipod shuffle 1G 4代 1GB 大陆行货', 2, N'个', 350.0000, N'', 45)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (88, N'100％好评 正品行货＋发票 苹果 iPod nano 第四代 16GB', 2, N'个', 1596.0000, N'', 20)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (89, N'100％好评 正品行货＋发票 苹果 iPod SHUFFLE 2G 第二代', 2, N'个', 506.0000, N'', 86)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (90, N'[香港数码代购有小票]apple/苹果itouch 2代 8G（免费刻字）', 2, N'个', 1750.0000, N'', 99)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (91, N'苹果最新iPod touch 2代全新 16G 港行 圣诞特惠价', 2, N'个', 2250.0000, N'', 98)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (92, N'苹果 iPod shuffle 1GB', 2, N'个', 380.0000, N'', 98)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (93, N'苹果 iPod shuffle 2GB', 2, N'个', 560.0000, N'', 32)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (94, N'大陆行货 苹果最新第四代iPod Nano 4代 8G 特价主机', 2, N'个', 1250.0000, N'', 11)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (95, N'全新上市ipod nano4代8G红色限量版', 2, N'个', 1208.0000, N'', 93)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (96, N'肆钻 ipod touch2代8G 联保现货 原装正品', 2, N'个', 1710.0000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (97, N'◆四钻代理商◆IPOD NANO四代(16G)大陆行货含发票全球联保', 2, N'个', 1550.0000, N'', 119)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (98, N'小巧玲珑,可随身携带的shuffle,给你意想不到的惊喜!', 2, N'个', 350.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (99, N'全新APPLE苹果iPod MB528CH/A TOUCH 2(8G)', 2, N'个', 1978.0000, N'', 109)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (100, N'全新APPLE苹果iPod IPOD NONO (4TH GEN) 8GB MB745CH/A', 2, N'个', 1168.0000, N'', 12)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (101, N'移动公司礼品（未拆封）全新 iPod video 30G 全国联保，假一赔十', 2, N'个', 1300.0000, N'', 82)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (102, N'全新APPLE苹果iPod MB681CH/A IPOD SHUFFLE 2GB PINK', 2, N'个', 510.0000, N'', 31)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (103, N'〓金南京〓iPod touch 2代(16G)正品 联保现货 新年礼包 销量过千', 2, N'个', 2050.0000, N'', 63)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (104, N'★★苹果驿站★★ppleipod全新行货nano 4 8G 彩虹版 礼包', 2, N'个', 935.0000, N'', 76)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (105, N'苹果新款 ipod nano 4代 4G香港行货 全国联保 ★★实体店★★', 2, N'个', 899.0000, N'', 119)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (106, N'08年新款 ipod touch 2代 8G 香港行货 全国联保 ★★实体店★★', 2, N'个', 1750.0000, N'', 80)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (107, N'苹果新款ipod nano 4代 16G 港货全国联保 ★★实体店★★', 2, N'个', 1420.0000, N'', 28)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (108, N'08年新款 ipod touch 2代 32G 香港行货 全国联保★★实体店★★', 2, N'个', 2900.0000, N'', 20)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (109, N'震撼价格___全新欧洲版', 2, N'个', 2500.0000, N'', 63)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (110, N'[苹果官方免费机上雕刻] ipod nano 苹果4代 16G 颜色齐全', 2, N'个', 1579.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (111, N'限量特价！iPod nano8G★大陆行货全国联保！送7件豪华大礼包邮', 2, N'个', 1298.0000, N'', 40)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (112, N'新品4代！iPod shuffle 2G 大陆行货★送6件大礼带发票、包邮', 2, N'个', 535.0000, N'', 66)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (113, N'｜皇冠｜苹果 ipod touch 2代 16G 行货联保 限量抢购1周 送红包', 2, N'个', 2199.0000, N'', 106)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (114, N'【ipod正品】新款iPod touch 2代 8G正品现货 全球联保', 2, N'个', 1599.0000, N'', 4)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (115, N'［苹果商店直送 免费机上雕刻］ipod touch2代 32G 苹果 黑色', 2, N'个', 3199.0000, N'', 22)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (116, N'IPOD nano 三代(4G银色)', 2, N'个', 650.0000, N'', 112)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (117, N'苹果iPod nano 4G', 2, N'个', 960.0000, N'', 15)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (118, N'苹果iPod touch (16GB)', 2, N'个', 2180.0000, N'', 26)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (119, N'★上海浦东ipod专卖★ipod nano4代 8G 限量版红色 全国联保', 2, N'个', 1090.0000, N'', 35)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (120, N'★上海浦东ipod专卖★ipod classic 120G 全新行货 全国联保', 2, N'个', 1600.0000, N'', 6)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (121, N'★上海浦东ipod专卖★iPod touch 2代 8G 全新行货 全国联保', 2, N'个', 1650.0000, N'', 114)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (122, N'原装 ★【ipod nano4代8G 霓虹版 】★', 2, N'个', 1050.0000, N'', 9)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (123, N'apple ★【ipod nano4代8G 】★ 红色限量版 现货', 2, N'个', 1280.0000, N'', 99)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (124, N'【钻石精品】08新款港行ipod shuffle4代 1G 大红色限量版', 2, N'个', 428.0000, N'', 4)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (125, N'火红靓丽的I Pod Nano你是不是也想得到它？不要犹豫了，买下它吧', 2, N'个', 1580.0000, N'', 7)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (126, N'你是时尚一族吗？那还等什么，I Pod Nano你时尚的代名词', 2, N'个', 1580.0000, N'', 86)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (127, N'全新大陆行货I Pod Nano新款上市,快来抢购啊!', 2, N'个', 1180.0000, N'', 52)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (128, N'I Pod nano 四代8G;16G火爆上市 正品行货', 2, N'个', 1154.0000, N'', 46)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (129, N'〓金南京〓iPod nano 4代(8G)正品 联保现货 新年礼包 销量过千', 2, N'个', 970.0000, N'', 37)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (130, N'皇冠实体/苹果apple ipod touch 2代 8GB 正品香港行货', 2, N'个', 1738.0000, N'', 76)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (131, N'原装大陆行货 ipod shuffle 4代1G 彩虹版降!降!机会难得抓紧时间', 2, N'个', 380.0000, N'', 36)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (132, N'沈阳苹果专卖,touch 1代 16GB 全新原封 完美破解', 2, N'个', 2150.0000, N'', 61)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (133, N'★上海百脑汇店★ipod nano 4代 8G 正规大陆行货 全国联保', 2, N'个', 1020.0000, N'', 22)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (134, N'★上海百脑汇店★新品ipod nano 4代 8G 全新行货 全国联保', 2, N'个', 950.0000, N'', 42)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (135, N'★上海百脑汇店★ipod shuffle 4代 1G 大红色限量版 全国联保', 2, N'个', 395.0000, N'', 119)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (136, N'ipod classic 80G 魅力无限', 2, N'个', 1400.0000, N'', 18)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (137, N'Ipod shuffle 1G', 2, N'个', 485.0000, N'', 104)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (138, N'★上海浦东ipod专卖★ipod touch 2代 16G 全新行货 全国联保', 2, N'个', 2060.0000, N'', 94)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (139, N'◆四钻信誉◆IPOD Shuffle(2G)大陆行货含发票全球联保', 2, N'个', 530.0000, N'', 118)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (140, N'皇冠实体/苹果apple ipod touch 2代 16GB 正品香港行货', 2, N'个', 2199.0000, N'', 96)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (141, N'紅色特別版香港行貨iPod nano 4代16G', 2, N'个', 1500.0000, N'', 43)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (142, N'伍钻【上海IPOD】原装正品 ipod nano4代8G 9色版 销售过百件', 2, N'个', 958.0000, N'', 24)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (143, N'苹果树 实体苹果专卖 iPod CLASSIC 120G (黑色，银色)新行货', 2, N'个', 1580.0000, N'', 78)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (144, N'皇冠店【袁绍】08新款 iPod shuffle 4代 1G 香港行货联保带票', 2, N'个', 339.0000, N'', 67)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (145, N'皇冠店【袁绍】Apple iPod classic 2代 120G 正品行货 黑银 现货', 2, N'个', 1599.0000, N'', 59)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (146, N'皇冠店【袁绍】08新款 Apple iPod NANO 4代 16G 正品行货联保', 2, N'个', 1319.0000, N'', 32)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (147, N'★四钻实体店★迎圣诞 Touch2 8GB 原装行货 现货热销中', 2, N'个', 1575.0000, N'', 112)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (148, N'苹果树 实体苹果专卖 NANO 4代 8G 全新行货全国联保', 2, N'个', 998.0000, N'', 30)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (149, N'实体苹果专卖店 ipod nano 4代 16G NANO 16G全新行货，全国联保', 2, N'个', 1398.0000, N'', 109)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (150, N'实体苹果专卖店 ipod touch 2代 32G 全新行货全国联保', 2, N'个', 2668.0000, N'', 100)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (151, N'实体苹果专卖 苹果树 nano 4代 4G 全新行货全球联保', 2, N'个', 888.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (152, N'苹果树 实体苹果专卖店 iPod touch 2代 16G 行货全新全球联保', 2, N'个', 2060.0000, N'', 92)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (153, N'苹果树 iPod TOUCH 16G 全新没有拆封行货 全球联保', 2, N'个', 1798.0000, N'', 12)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (154, N'苹果树 iPod touch 32G 行货 全新没有拆封 全球联保', 2, N'个', 2358.0000, N'', 102)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (155, N'实体苹果专卖 ipod touch 2代 8G 全新行货全国联保', 2, N'个', 1578.0000, N'', 18)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (156, N'冲皇冠【蟲蟲居】大陆行货 原装正品 ipod nano4代8G彩虹版 现货', 2, N'个', 1110.0000, N'', 72)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (157, N'★胜创Kingmax 超棒 4G U盘 牛年限量版★送硅胶套 3皇冠实体店', 3, N'个', 51.5000, N'', 80)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (158, N'五皇冠 原装行货 Kingmax 胜创 09新超棒 8G 8GB U盘 优盘 C049', 3, N'个', 102.8000, N'', 60)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (159, N'淘宝年终大促销-80万大奖等你拿 金士顿DT1 4GB U盘/优盘', 3, N'个', 59.0000, N'', 100)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (160, N'五皇冠 原装行货 Kingmax胜创 4G U-Drive UD U盘 高速优盘 C075', 3, N'个', 50.0000, N'', 14)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (161, N'*四钻信誉* 可做挂饰 迷你可爱SONY小精灵 2G U盘', 3, N'个', 35.0000, N'', 83)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (162, N'台电U盘 16G 台电 16GB U盘 晶灵Ⅲ代 全铝镁合金外壳 杀毒+加密', 3, N'个', 164.0000, N'', 50)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (163, N'低价冲四钻 金士顿2GB U盘绝对足量800查询 一年保修', 3, N'个', 33.0000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (164, N'全新SONY 4GU盘 索尼4G 全钢品质 U盘 4GB', 3, N'个', 37.0000, N'', 93)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (165, N'【两件包快递】2G金士顿 U盘 逸盘 官网验证防伪 5年保 附测试图', 3, N'个', 29.9000, N'', 107)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (166, N'出厂价 足量版SONY旋转式全钢U盘 4GB 索尼全钢优盘', 3, N'个', 42.0000, N'', 47)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (167, N'新品SONY 4GU盘 金属质感 超溥防水 5年包换', 3, N'个', 68.0000, N'', 110)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (168, N'金士顿KingSton行货正品 全国联保 4G U盘/优盘 五年保换', 3, N'个', 55.0000, N'', 73)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (169, N'双皇冠 大陆行货 金士顿/kingston DT1 优盘4G U盘 逸盘 800防伪', 3, N'个', 55.0000, N'', 39)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (170, N'金士顿4GU盘优盘金士顿4G足量特价 45元江浙沪带U盘检测软件', 3, N'个', 40.0000, N'', 112)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (171, N'四钻 全新金士顿KINGSTON 旋转DT101 4G U盘 岁末限时促销500件', 3, N'个', 41.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (172, N'桃色浪漫纽约风情 PNY 双子盘 4G U盘 金属抗摔防水 360度旋转', 3, N'个', 55.0000, N'', 45)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (173, N'[正品行货]金士顿 DTMS 迷你/Mini优盘/U盘 4G 4GB 800防伪', 3, N'个', 58.0000, N'', 76)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (174, N'■行货 胜创 kingmax 白色 圣诞纪念版 4G U盘 超棒★四皇冠', 3, N'个', 54.5000, N'', 99)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (175, N'■蓝色 台电行货 晶彩 16G U盘 加密大师 杀毒 U盘■四皇冠', 3, N'个', 172.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (176, N'白金数码 KINGMAX 8G U盘 超棒 优盘 红 牛年版 终保 皇冠实体店', 3, N'个', 102.0000, N'', 114)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (177, N'全新行货 Codisk COU-F808 睿薄名片式 8G U盘 SSD酷盘★四皇冠', 3, N'个', 120.0000, N'', 83)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (178, N'SONY4GU盘足兆金属质感超溥防水5年包换特价48小时', 3, N'个', 59.0000, N'', 43)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (179, N'全新行货 800防伪 正品 金士顿 DTI 4G U盘/逸盘 五钻信誉', 3, N'个', 53.0000, N'', 30)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (180, N'KINGMAX 4G U盘 超棒 优盘 红色 牛年 纪念版 终保 皇冠实体店', 3, N'个', 50.0000, N'', 65)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (181, N'皇冠 金士顿 4G 4GB DataTraveler逸盘 优盘 U盘 CU28', 3, N'个', 38.0000, N'', 88)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (182, N'皇冠信誉 金士顿8G U盘 8GB优盘 逸盘 行货防伪', 3, N'个', 102.0000, N'', 42)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (183, N'皇冠信誉 Sandisk 行货专卖店　 8G U3 盘 实物照', 3, N'个', 108.0000, N'', 106)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (184, N'【消保+实体+3钻】索尼U盘 旋转型 4G U盘 足量版52元', 3, N'个', 52.0000, N'', 28)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (185, N'■防伪行货 联想T180 奥运纪念版 高速 黑色 8G U盘■四皇冠', 3, N'个', 135.0000, N'', 101)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (186, N'金士顿DT400 U盘 16G 原装行货 正规发票 全国联保', 3, N'个', 189.0000, N'', 102)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (187, N'TF/MICRO 2G 只随手机发', 3, N'个', 50.0000, N'', 12)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (188, N'冲五钻，SONY最新款翻盖U盘，硬币大小，五年包换！4G仅售58元！', 3, N'个', 58.0000, N'', 64)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (189, N'三皇冠 kingston 金士顿 逸盘 优盘 U盘 8G 正品行货 5年包换', 3, N'个', 108.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (190, N'钻石信誉全百好评 威刚-开拓者 正品行货 4GB 优盘/U盘 年底促销!', 3, N'个', 40.0000, N'', 102)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (191, N'新款 800正品★金士顿Kingston DT101 4G U盘★多彩旋转3皇冠实体', 3, N'个', 55.5000, N'', 49)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (192, N'年底抢购价 800防伪 韩国现代 8G U盘 高速优盘 金属防水国际大奖', 3, N'个', 82.9900, N'', 58)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (193, N'【实体店批发】SONY不锈钢旋转 U盘优盘 4GB 足量专卖 进口芯片', 3, N'个', 39.9000, N'', 110)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (194, N'支持20抵价券★三钻信誉★五年保全新索尼旋转金属U盘足量4GB', 3, N'个', 76.0000, N'', 38)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (195, N'◆皇冠◆经典款 4G U盘 韩国现代U盘4G 天梭 正品行货', 3, N'个', 40.0000, N'', 65)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (196, N'冲钻金士顿 4G U盘 限量版 江浙沪包邮只要45元', 3, N'个', 40.0000, N'', 81)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (197, N'■台电行货 晶灵III 8G U盘 加密大师 金属外壳■四皇冠', 3, N'个', 81.8000, N'', 59)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (198, N'800防伪 Kingmax胜创 U-Drive 4G 尊爵黑色 高速优盘/U盘★双皇冠', 3, N'个', 48.5000, N'', 77)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (199, N'日本Hello kittyU盘(4G)迎圣诞日本原包进口!MM最爱,2件起包邮', 3, N'个', 58.0000, N'', 31)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (200, N'[全国联保]正品行货 金士顿 DT101 II 旋转加密优盘 U盘 8G 8GB', 3, N'个', 102.0000, N'', 28)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (201, N'行货800防伪 金士顿 kingston DTI 4G U盘/逸盘/优盘★双皇冠', 3, N'个', 53.0000, N'', 80)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (202, N'#100%好评,冲4钻#索尼迷你可爱SONY小精灵U盘2GB 女生钟爱', 3, N'个', 36.0000, N'', 108)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (203, N'【人气NO1】美国 PNY 4G 熊猫烤漆精装版 防水防尘U盘(带套)(黑)', 3, N'个', 53.0000, N'', 10)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (204, N'钻石信誉SONY（索尼）2GBU盘优盘 SONYU盘 精美木盒包装', 3, N'个', 33.0000, N'', 103)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (205, N'■行货 胜创 kingmax 牛年纪念版 2G U盘 超棒 送保护套★四皇冠', 3, N'个', 36.8000, N'', 91)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (206, N'金士顿 U盘 8G U盘 回馈淘友 圣诞狂卖 疯狂促销 买1送1 抢了哦', 3, N'个', 53.0000, N'', 54)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (207, N'3钻特惠淘友!保证正品足量 清华紫光优盘4G 紫光4G优盘', 3, N'个', 36.0000, N'', 24)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (208, N'金属旋转SONY U盘 4G', 3, N'个', 55.0000, N'', 79)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (209, N'【4·3堂】折扣店-正品行货金士顿2G优盘/U盘★800防伪 全国联保', 3, N'个', 38.0000, N'', 20)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (210, N'09最新款 索尼高速防水 4GB/4G U盘 正宗足量', 3, N'个', 62.0000, N'', 25)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (211, N'三皇冠 行货800防伪 金士顿DT1 2G U盘 2GB优盘 逸盘 S007', 3, N'个', 41.5000, N'', 113)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (212, N'冲皇冠 宇瞻 正品 微笑碟 AH320 8G U盘 铁盒密封三星双芯片', 3, N'个', 105.0000, N'', 31)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (213, N'〖皇冠信誉〗足量.金士顿限量版U盘4G 金士顿限量版4GU盘', 3, N'个', 38.0000, N'', 105)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (214, N'bolo珍藏版E8牛气冲天 U盘8GB 8G优盘 旋转设计全钢机身 礼盒装', 3, N'个', 92.0000, N'', 39)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (215, N'TCL水晶U盘——奢侈科技最佳礼品 情侣u盘 u盘个性 移动硬盘', 3, N'个', 999.0000, N'', 42)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (216, N'全新外贸 Hello kitty 迷你小巧 U盘 4G 足量版 非升级 假一赔十', 3, N'个', 45.0000, N'', 47)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (217, N'皇冠 宇瞻 AH321 钢铁侠 行货 U盘 4G 优盘 行货 联保5年', 3, N'个', 47.5000, N'', 29)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (218, N'*官方网站验证*正品金士顿 4GB U盘/800防伪 五年保固', 3, N'个', 49.9000, N'', 6)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (219, N'推缩型 索尼★SONY 小精灵 8GB/8G U盘 粉色', 3, N'个', 58.0000, N'', 108)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (220, N'三皇冠 宇瞻Apacer AH321钢铁侠16G 16GB U盘 优盘 五年保行货', 3, N'个', 162.0000, N'', 10)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (221, N'金士顿U盘8G DTI一年包换新100%足量 北京中关村实体店保障', 3, N'个', 75.0000, N'', 2)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (222, N'热卖 kingston金士顿U盘 8G 8GB优盘 原装800防伪正品行货 逸盘', 3, N'个', 78.0000, N'', 50)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (223, N'双皇冠 800防伪 现代芯锐4G 高速优盘/U盘 金属防水国际设计大奖', 3, N'个', 55.0000, N'', 25)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (224, N'冲2钻-全新索尼U盘第二代全钢2GU盘-足量足兆-5年保修不锈钢', 3, N'个', 30.0000, N'', 57)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (225, N'金士顿kingston 4G U盘/优盘 DT101 旋转加密蓝色★双皇冠', 3, N'个', 55.0000, N'', 66)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (226, N'冲击你的心理底线！省代直销，忆捷F1 U盘 8G 特价69.9元！', 3, N'个', 69.9000, N'', 74)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (227, N'全新行货 Codisk COU-F808 睿薄名片式 16G U盘 SSD酷盘★四皇冠', 3, N'个', 235.0000, N'', 20)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (228, N'〖皇冠信誉〗 金士顿 行货KINGSTON DTI 4G U盘 假一罚十', 3, N'个', 55.0000, N'', 40)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (229, N'双皇冠 正品金士顿/Kingston DT101 2G U盘/优盘 新款旋转加密', 3, N'个', 40.0000, N'', 37)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (230, N'■行货 胜创 kingmax U-Drive 高速 4G U盘★四皇冠', 3, N'个', 48.8000, N'', 52)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (231, N'双冠 奥运熊猫精装版 PNY 必恩威 U盘 4G 800防伪 Y014', 3, N'个', 49.4000, N'', 54)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (232, N'SanDisk U盘 4GB 原装行货 全国联保 实体经营', 3, N'个', 60.0000, N'', 6)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (233, N'热卖-金士顿 Kingston U盘 优盘 4G/4GB 逸盘 足量版 原装芯片', 3, N'个', 55.0000, N'', 12)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (234, N'★SSK飚王 诱惑 4G U盘 SFD042 超薄/360度旋转/送挂绳★三皇冠', 3, N'个', 59.5000, N'', 3)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (235, N'正品 特价SD卡 2GB五年全国联保SD/2GB 2GB* Secure Digital Card', 4, N'个', 50.0000, N'', 69)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (236, N'怡怡数码 行货800防伪 金士顿 高速 SDHC 4G CLASS4', 4, N'个', 50.0000, N'', 30)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (237, N'全国联保 创见Transcend SD 2G 零利润,只赚信用', 4, N'个', 26.8000, N'', 100)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (238, N'全国联保 金士顿Kingston SD 1G 低价促销 假一赔十', 4, N'个', 22.8000, N'', 89)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (239, N'冲皇冠 100%全新行货 金士顿 1G SD卡 全国联保 终身保固', 4, N'个', 23.0000, N'', 71)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (240, N'高速存储卡 Kingston 正品 SD卡 4GB 联保五年 SDHC标准 4G W101', 4, N'个', 70.0000, N'', 26)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (241, N'冲钻特价SD卡 2GB sd 金士顿 手机内存卡 相机内存卡', 4, N'个', 50.0000, N'', 90)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (242, N'联想 SD卡 1GB 奥运纪念版 正品行货', 4, N'个', 30.0000, N'', 99)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (243, N'绝对正品行货 假一罚百 kingston 金士顿2G SD卡 2GB SD 终身质保', 4, N'个', 35.0000, N'', 37)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (244, N'四钻联保金士顿行货SD2G 假一罚十 EMS10元起', 4, N'个', 24.8000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (245, N'全新正品行货 金士顿 Kingston SD 8G SDHC Class4', 4, N'个', 105.0000, N'', 47)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (246, N'冲4钻 大量批发金士顿SD1G 16元', 4, N'个', 16.0000, N'', 31)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (247, N'全新行货 创见 SD 4G 闪存卡 Transcend SD卡 4GB 全国联保', 4, N'个', 55.0000, N'', 44)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (248, N'正品行货sandisk 2G SD卡 800防伪 五年质保', 4, N'个', 28.0000, N'', 72)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (249, N'消保.全新5年保 金士顿SD卡 KingStong SD卡 1G', 4, N'个', 20.0000, N'', 68)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (250, N'皇冠信誉 金士顿原装行货SD卡 2GB 全国联保', 4, N'个', 27.0000, N'', 41)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (251, N'冲皇冠 原装 美国必恩威 PNY SD2G 内存卡 2GSD', 4, N'个', 28.0000, N'', 67)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (252, N'成交近百■全新金士顿港货1G SD卡■江浙沪快递６元', 4, N'个', 15.0000, N'', 70)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (253, N'皇冠信誉 SanDisk SD 4G SDHC 800防伪全新正品行货 5年质保', 4, N'个', 44.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (254, N'正品创见SD卡 4GB', 4, N'个', 55.0000, N'', 29)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (255, N'全国联保★正品防伪★金士顿 SD卡★ 2G', 4, N'个', 29.0000, N'', 55)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (256, N'金士顿 1GB SD储存卡 &lt;全新行货,实体店铺&gt;', 4, N'个', 48.0000, N'', 112)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (257, N'Transcend(创见)SD卡4GB原装行货 相机专用', 4, N'个', 60.0000, N'', 9)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (258, N'正品行货 金士顿SD卡 8GB 金士顿SDHC SD卡 118包申通快递', 4, N'个', 103.0000, N'', 90)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (259, N'【小熊特价】★胜创KINGMAX SD卡 8GB 高速CLASS6', 4, N'个', 102.0000, N'', 18)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (260, N'Apacer SD卡高速卡2GB宇瞻SD 100X(2GB)闪存卡28元送读卡器一个', 4, N'个', 28.0000, N'', 10)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (261, N'◣假一赔十◢1G手机内存卡只需25元冲钻价', 4, N'个', 35.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (262, N'[实店*保证正品]金士顿 Kingston 800防伪 SD卡 2GB', 4, N'个', 65.0000, N'', 26)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (263, N'【浙江E购网】金士顿 SD卡 高速 8GB只要188元！正品行货', 4, N'个', 99.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (264, N'【浙江E购网】SANDISK SD(如意卡) 2GB SD卡 全国联保 5年保修', 4, N'个', 35.0000, N'', 53)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (265, N'【浙江E购网】金士顿 SD卡 高速 1GB只要29元！正品行货', 4, N'个', 29.0000, N'', 97)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (266, N'行货800 金士顿2G 2GB SD存储卡', 4, N'个', 59.0000, N'', 98)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (267, N'圣业物资 Transcend 创见 SD 4G 4GB 有防伪 终身保 正宗行货', 4, N'个', 51.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (268, N'四钻授权正品 创见 SD 4G 4GB 行货 全国联保 终身保修', 4, N'个', 48.5000, N'', 49)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (269, N'皇冠信誉 Sandisk 行货专卖店 SD 2G', 4, N'个', 30.0000, N'', 35)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (270, N'SD卡 2GB 创见 SD 2G存储卡（终身质保）30元', 4, N'个', 30.0000, N'', 100)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (271, N'金士顿2g sd卡/金士顿sd卡2gb/金士顿2g sd卡/sd卡金士顿行货', 4, N'个', 39.0000, N'', 107)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (272, N'金士顿SD卡 2G SD卡 正品行货 38元送SD读卡器一个', 4, N'个', 38.0000, N'', 83)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (273, N'【皇冠100%信誉店】松下SD2G卡/SD2GB 高速卡 日本原装卡 正品', 4, N'个', 35.0000, N'', 45)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (274, N'『皇冠信誉』三星原装SD2G卡 兼容性高 三星原厂简包装', 4, N'个', 46.0000, N'', 89)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (275, N'金士顿SD卡4G Class6 SDHC高速卡 正品行货 全国终身联保', 4, N'个', 49.0000, N'', 84)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (276, N'皇冠 全国联保行货 Kingston 金士顿 SD卡 4G CLASS6极速', 4, N'个', 49.9000, N'', 33)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (277, N'原装进口松下SD卡 2GB', 4, N'个', 35.0000, N'', 10)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (278, N'冲皇冠 Transcend 创见 SD 4G SD卡 4GB(非SDHC)官网验证', 4, N'个', 47.0000, N'', 93)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (279, N'【三钻+实体】Kingston 金士顿 2G SD卡『正品行货 全国联保』', 4, N'个', 39.0000, N'', 93)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (280, N'金士顿SDHC SD卡4G 4GB闪存卡存储卡正品行货KS4 5年联保终身保固', 4, N'个', 57.0000, N'', 67)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (281, N'冲四钻-正品行货800防伪kingston 金士顿 SD卡 2G 5年包换', 4, N'个', 38.0000, N'', 99)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (282, N'皇冠信誉 全国联保!金士顿 kingston 2G SD卡 正品行货假一赔十', 4, N'个', 29.9000, N'', 11)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (283, N'3钻+消保 行货 kingmax 胜创 SD 2G SD卡 2GB 扩展卡 800防伪', 4, N'个', 26.0000, N'', 78)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (284, N'★3钻卖家★(原装行货) 创见SD卡4GB / 承诺五年包换', 4, N'个', 50.0000, N'', 95)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (285, N'★3钻卖家★ 金士顿 SD卡 4GB (原装行货) / 承诺五年包换', 4, N'个', 48.0000, N'', 13)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (286, N'创见 SD卡 4GB (正品行货) ------ 3钻信誉 100%好评卖家', 4, N'个', 50.0000, N'', 42)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (287, N'金士顿 SD卡 4GB (正品行货) ------ 3钻信誉 100%好评卖家', 4, N'个', 48.0000, N'', 58)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (288, N'SanDisk SD卡 1G 正品行货 假一罚十', 4, N'个', 45.0000, N'', 105)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (289, N'SD卡 1GB全新盒装 kingston【金士顿1G SD卡赛格电子包邮冲钻', 4, N'个', 88.0000, N'', 28)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (290, N'★老鼠街★ 800行货 KINGMAX SDHC SD 4G 4GB存储卡', 4, N'个', 45.0000, N'', 65)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (291, N'国内组装 SD存储卡 2GB', 4, N'个', 27.0000, N'', 11)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (292, N'包邮！SD卡 2GB 适用于各类数码产品', 4, N'个', 70.0000, N'', 114)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (293, N'金士顿 kingston 2G SD卡五年联保800行货', 4, N'个', 35.0000, N'', 68)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (294, N'◇双钻信誉◇东芝SD卡4G 超高速型 CLASS 6 白色高速卡', 4, N'个', 145.0000, N'', 60)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (295, N'◇双钻信誉◇金士顿SD/Kingston SD卡 2G 原装行货 全国联保', 4, N'个', 52.0000, N'', 5)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (296, N'●雁儿的小店●SD闪存卡 TwinMOS SD卡128M 热卖100件火爆销售中', 4, N'个', 20.0000, N'', 96)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (297, N'二皇冠 行货 金士顿高速 SDHC 4G Class 6 Kingston 4G SD卡 4GB', 4, N'个', 48.0000, N'', 50)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (298, N'100%好评,金士顿16G/SD卡,正品行货,800防伪,全国联保,终身质保', 4, N'个', 415.0000, N'', 106)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (299, N'100%好评,金士顿8G/SD卡,正品行货,800防伪,全国联保,终身质保', 4, N'个', 170.0000, N'', 55)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (300, N'100%好评,金士顿4G/SD卡,正品行货,800防伪,全国联保,终身质保', 4, N'个', 80.0000, N'', 72)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (301, N'100%好评,金士顿4G/SD卡,正品行货,800防伪,全国联保,终身质保', 4, N'个', 85.0000, N'', 9)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (302, N'100%好评,金士顿2G/SD卡,正品行货,800防伪,全国联保,终身质保', 4, N'个', 35.0000, N'', 113)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (303, N'最新版kingston 金士顿 SD卡 1GB 五年保换 全国联保', 4, N'个', 33.0000, N'', 39)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (304, N'632 kingston 金士顿 SD 2G 终身质保800防伪 正品行货', 4, N'个', 45.0000, N'', 83)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (305, N'〓华星数码店〓特价 金士顿SD 1G内存卡 +5元送读卡器', 4, N'个', 15.0000, N'', 58)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (306, N'〓华星数码店〓特价 KingSton SD 1G内存卡+5元送读卡器', 4, N'个', 15.0000, N'', 102)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (307, N'■原装Sandisk 2GB SD卡■800防伪5年全国联保', 4, N'个', 110.0000, N'', 22)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (308, N'★宇瞻 4GB SD卡 4G★【防伪行货5年联保】极速SD卡', 4, N'个', 136.0000, N'', 78)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (309, N'◣原装金士顿 SD卡 2GB◢全国联保金士顿官方网查', 4, N'个', 110.0000, N'', 63)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (310, N'正品行货 800防伪 创见 Transcend SD 4G 五钻信誉', 4, N'个', 53.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (311, N'金士顿 KingSton SD卡 4G 全国联保', 4, N'个', 51.0000, N'', 101)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (312, N'金士顿 SD卡 2GB KingSton SD 2G 五年质保 50X 相机内存卡', 4, N'个', 22.9000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (313, N'925纯银饰品★tiffany项链 大海星项链★圣诞礼物', 5, N'个', 36.0000, N'', 65)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (314, N'特价┆韩国进口┆时尚个性随性设计长款项链N526', 5, N'个', 53.0000, N'', 101)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (315, N'T213z蒂比欧二代X50倍运动保健项圈 送90元纯钛吊坠', 5, N'件', 280.0000, N'', 101)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (316, N'TIFFANY925纯银 全水滴项链(送包装 即买即送)', 5, N'件', 79.0000, N'', 86)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (317, N'四钻★特价施华洛世奇水晶套装首饰-暖 日韩国进口饰品,生日礼物', 5, N'件', 228.0000, N'', 80)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (318, N'【韩流饰品店】十字镶嵌吊坠 纯银饰品 流行饰品', 5, N'个', 58.0000, N'', 100)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (319, N'开运吉祥物将星鼠红玉髓-属龙者09年聚财旺事业', 5, N'个', 38.0000, N'', 92)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (320, N'『东海水晶岛』★巴西黄水晶笑面佛2#', 5, N'个', 130.0000, N'', 40)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (321, N'◥专柜真品◥假货勊星◥附专柜发票心连心TIFFANY项链', 5, N'个', 899.0000, N'', 98)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (322, N'*歌特*LOLITA*DIY蕾丝颈饰（无花款）', 5, N'个', 35.0000, N'', 116)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (323, N'施华洛世奇水晶6202(539)28MM　丽致街水晶饰界冲钻特价', 5, N'个', 30.0000, N'', 77)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (324, N'*歌特&amp;amp;LOLITA*DIY蕾丝颈链', 5, N'个', 25.0000, N'', 82)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (325, N'S105 正品盛邦负离子保健运动能量X30红色钛项圈', 5, N'个', 90.0000, N'', 64)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (326, N'东晶 天然黄水晶随形坠40mm GG12107', 5, N'个', 36.0000, N'', 50)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (327, N'钻石信誉~韩国进口-可爱美女图像项链x1168', 5, N'个', 68.0000, N'', 83)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (328, N'无香银坊 百搭蛇骨软项圈回馈价', 5, N'个', 30.0000, N'', 92)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (329, N'镂空星月项链★GUCCI925纯银项链★韩国进口★钻石信誉品质保证', 5, N'个', 62.0000, N'', 96)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (330, N'附防伪法国Julie项链正品饰品∥萨尔茨堡∥秋冬促销圣诞生日礼物', 5, N'个', 56.0000, N'', 73)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (331, N'经典时尚/蒂梵尼tiffany项链房子钥匙吊坠细链925银饰品', 5, N'个', 29.0000, N'', 66)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (332, N'精品大特价！韩国新款银心圆坠项链', 5, N'个', 36.0000, N'', 113)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (333, N'＜钻石信誉＞S925纯银项链/项坠/女士吊坠套链*圆围海豚钻*', 5, N'个', 39.9000, N'', 67)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (334, N'天然水晶-情侣饰品-天然黑玛瑙日月同辉情侣挂件/吊坠/项链', 5, N'个', 39.0000, N'', 68)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (335, N'特 Vivienne Westwood经典土星满钻项链', 5, N'个', 59.0000, N'', 85)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (336, N'韩国进口 字母满钻项链', 5, N'个', 48.0000, N'', 27)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (337, N'极品天然绿色彩虹眼黑曜石葫芦貔貅吊坠保平安**辟邪转运', 5, N'个', 98.0000, N'', 38)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (338, N'925纯银饰品 GUCCI古奇经典大8字橄圆牌项链', 5, N'个', 49.0000, N'', 28)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (339, N'【甜心宠尚】三叶草镶钻镀白金珍珠项链 购正品满百可1元换购', 5, N'个', 29.0000, N'', 119)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (340, N'【银阁】正品disney项链 米奇项链MX016--925银(原厂包装)', 5, N'个', 78.0000, N'', 78)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (341, N'【银阁】正品disney项链 米奇项链MX015--925银(原厂包装)', 5, N'个', 78.0000, N'', 105)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (342, N'金色双层链 松石珍珠多坠饰 小Y型项链 ‖0435', 5, N'个', 40.0000, N'', 46)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (343, N'钻石信誉*镀白金18寸蛋形竹节 925纯银项链*亏本冲双钻', 5, N'个', 31.0000, N'', 24)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (344, N'新款包邮◆tiffany项链 圆环皇冠项链◆925纯银饰品', 5, N'个', 45.0000, N'', 35)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (345, N'Tiffany/蒂凡尼925纯银五水滴项链*三件包邮', 5, N'个', 39.0000, N'', 62)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (346, N'钻石信誉*镀白金16寸蛋形竹节 925纯银项链*亏本冲双钻', 5, N'个', 29.0000, N'', 61)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (347, N'天然水晶天然白水晶6MM佛链精品特价促销', 5, N'个', 188.0000, N'', 66)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (348, N'★钻石信誉|925纯银★Tiffany开口桃心项链', 5, N'个', 40.0000, N'', 50)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (349, N'流行饰界〖新品推广〗【飞标风盘情侣项链(黑色)】316L精钢', 5, N'个', 85.0000, N'', 74)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (350, N'★三钻名店★swarovski梯子系列 蝴蝶钻 项链 1044', 5, N'个', 40.0000, N'', 27)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (351, N'现货*韩国进口*925纯银珍珠母贝粉色花朵项链', 5, N'个', 96.0000, N'', 40)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (352, N'时尚族人の非主流 贵族气息 姜东元戴的酷刀项链', 5, N'个', 48.0000, N'', 15)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (353, N'925纯银----TiffanyP062镶钻罗马直牌项链（米乐宝）', 5, N'个', 59.0000, N'', 36)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (354, N'送高档包装新款925纯银施华洛世奇水晶项链转运珠礼品生日礼物', 5, N'个', 178.0000, N'', 90)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (355, N'新款钻石信誉奥地利施华洛世奇水晶项链 南极冰块系列蓝 多种颜色', 5, N'个', 48.0000, N'', 26)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (356, N'新款 钻石信誉 奥地利施华洛世奇水晶吊坠 不规则茶24mm', 5, N'个', 48.0000, N'', 75)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (357, N'F37【龙威】5钻 925纯银八心八箭50分心火项链 包快递终身保修', 5, N'个', 136.0000, N'', 141)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (358, N'100%好评+消保★12381韩国进口◆四方四正红色水晶银色项链', 5, N'个', 90.0000, N'', 71)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (359, N'极美彩虹黑耀石挂件* 开光*风水转运轮 避小人-属鸡者09年开运物', 5, N'个', 168.0000, N'', 63)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (360, N'无香银坊 925纯银红黑玛瑙长项链（77cm）', 5, N'个', 255.0000, N'', 20)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (361, N'无香银坊 925纯银配天然黄水晶长项链（57cm）', 5, N'个', 425.0000, N'', 71)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (362, N'支持VIP 哈喜欢★韩国进口粉可爱镶钻猫头鹰项链(两色)', 5, N'个', 83.9900, N'', 18)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (363, N'【紫艺佳人饰】红珊瑚十字架吊坠 高层 欧美日韩时尚', 5, N'个', 65.0000, N'', 101)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (364, N'无香银坊 925纯银莹石紫水晶项链（42cm）', 5, N'个', 122.0000, N'', 54)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (365, N'蒂梵尼字母锁头项链', 5, N'个', 48.0000, N'', 79)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (366, N'16552151◆俏皮海豚_蓝水晶项链#2特价减15', 5, N'个', 32.0000, N'', 46)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (367, N'§韩国原装进口饰品§经典项链店主推荐哦~', 5, N'个', 118.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (368, N'无香银坊 925纯银琥珀紫水晶项链（43cm）', 5, N'个', 130.0000, N'', 111)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (369, N'毛衣链/项链 正品施华洛世奇水晶 （含真牛皮带银扣皮绳）', 5, N'个', 138.0000, N'', 17)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (370, N'韩国进口饰品-芭蕾舞girl珍珠项链|okba品牌', 5, N'个', 72.0000, N'', 75)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (371, N'无香银坊 925纯银黄水晶项链（42cm）', 5, N'个', 95.0000, N'', 46)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (372, N'◣韩国饰品代购◥靓丽华美炫亮水晶花朵长坠皓石项链', 5, N'个', 188.0000, N'', 58)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (373, N'★四钻信誉！进口韩国饰品闪亮铁塔项链(特2430)', 5, N'个', 39.0000, N'', 118)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (374, N'正品蒂凡尼银饰 Tiffany蒂梵尼项链(925)-大开口心套链(TL097)', 5, N'个', 79.0000, N'', 20)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (375, N'清新波西米亞 绿叶碎花小棉球原色小木珠长款木质毛衣项链 浅棕色', 5, N'个', 29.0000, N'', 35)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (376, N'〓正品TIFFANY/蒂凡尼 圆形锁扣项链 1年质保 热卖中〓', 5, N'个', 147.0000, N'', 62)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (377, N'精美的木饰品耳坠,项链套装3,精品礼品,送礼佳品,时尚饰品配件', 5, N'个', 40.0000, N'', 91)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (378, N'925银饰品 字母项链/名字项链定做 最特色的个性生日礼物PN0164', 5, N'个', 105.0000, N'', 65)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (379, N'量', 5, N'个', 45.0000, N'', 22)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (380, N'【韩国首饰预订】代购my girl李多海 粉、蓝、白银色水晶梅花项链', 5, N'个', 49.0000, N'', 116)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (381, N'彩虹罗曼史08新款韩国进口142厘米仿珍珠项链/毛衣链', 5, N'个', 39.0000, N'', 108)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (382, N'景泰蓝项链,个人收藏品', 5, N'个', 30.0000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (383, N'RYの欧美日韩风 水晶蝴蝶 黑色珠链项链', 5, N'个', 76.0000, N'', 13)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (384, N'◥◣冲钻期间，批价销售◢◤三行字中心牌OT扣项链 TL0031A', 5, N'个', 80.0000, N'', 74)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (385, N'925纯银tiffany蒂凡尼银饰--天国阶梯情侣项链', 5, N'个', 69.0000, N'', 37)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (386, N'圣诞促销大礼★送包装★TIFFANY项链 一行字鸡心水泡项链', 5, N'个', 58.0000, N'', 60)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (387, N'V1205疯狂促销/订做时尚个性-名字项链*字母项链/韩国个性定做', 5, N'个', 93.0000, N'', 103)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (388, N'4钻信誉【恋链不舍】㊣Swarovski项链 黑色不规则水晶2011', 5, N'个', 36.0000, N'', 100)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (389, N'迪士尼新品◆米奇个性系列P0045P圆牌项链☆米奇专柜正品包邮', 5, N'个', 270.0000, N'', 44)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (390, N'迪士尼新品◆维尼披风衣服DC-WB0-P0042PM项链☆米奇专柜正品包邮', 5, N'个', 215.0000, N'', 10)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (391, N'天然水晶天然虎晶石14MM手链精品特价促销', 6, N'个', 185.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (392, N'韩版专业定制纯银名字手链，字母手链，生日礼物', 6, N'个', 98.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (393, N'泰瑞宝：招财转运的小叶紫檀黄水晶金发晶风水手链', 6, N'个', 141.0000, N'', 87)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (394, N'天然水晶虎晶石手链10mm(虎晶石圆珠手链)送礼礼品礼物', 6, N'个', 108.0000, N'', 85)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (395, N'水晶物语*时光痕迹-天然水晶虎眼（虎睛）石手链', 6, N'个', 36.6000, N'', 87)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (396, N'水晶物语*时光痕迹-天然水晶绿色彩发晶手链', 6, N'个', 36.6000, N'', 103)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (397, N'水晶物语*时光痕迹-天然水晶白菜（发财）手链', 6, N'个', 30.0000, N'', 63)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (398, N'★炫酷银吧★海盗船正品☆925纯银镶☆天然琥珀石☆手链', 6, N'个', 318.0000, N'', 36)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (399, N'★炫酷银吧★海盗船正品☆925纯银镶☆天然绿松石☆手链', 6, N'个', 535.0000, N'', 94)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (400, N'纯天然水晶石碧玺７Ａ手链附权威证书送精美礼袋', 6, N'个', 648.7000, N'', 104)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (401, N'925纯银手工镶嵌橄榄石，珍珠精致手镯*乐淘堡印度尼泊尔专卖*', 6, N'个', 395.0000, N'', 67)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (402, N'双皇冠信誉ES0051 Ecats蓝色魅惑施华洛世奇水晶手链', 6, N'个', 285.0000, N'', 20)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (403, N'TIFFANY手链 ◆全豆 手链925纯银※包邮促销+圣诞送礼', 6, N'个', 60.0000, N'', 100)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (404, N'【超低价促销】绽梅手链-章子怡07绽放系列(影视剧)', 6, N'个', 62.0000, N'', 100)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (405, N'【超低价促销】绽梅手链-章子怡07绽放系列(影视剧)', 6, N'个', 40.0000, N'', 116)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (406, N'【冲钻特价】天然茶水晶加白水晶刻面手链8MM', 6, N'个', 28.0000, N'', 45)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (407, N'【恋恋水晶】AAA极品天然琥珀手链 刻面珠子8mm', 6, N'个', 550.0000, N'', 92)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (408, N'精品巴西进口紫水晶情侣手链', 6, N'个', 168.0000, N'', 65)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (409, N'特价 天然橄榄石手链《岁岁平安》', 6, N'个', 49.0000, N'', 63)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (410, N'【冲钻特价】天然极品粉水晶个性手链 珠子8MM', 6, N'个', 28.0000, N'', 55)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (411, N'100%好评★925银饰★手工编制★时尚红绳配925银幸运雕花坠手链★', 6, N'个', 29.0000, N'', 63)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (412, N'【魅缘购物】TIFFANY爱神之箭手链', 6, N'个', 50.0000, N'', 22)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (413, N'墨西哥极品紫彩虹眼辟邪旺财转运黑曜石貔貅', 6, N'个', 110.0000, N'', 105)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (414, N'实物拍摄 墨西哥天然黑曜石双彩眼手链/黑耀石太极转运鼠手链', 6, N'个', 165.0000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (415, N'梦想水晶*AAA级12MM天然茶晶（烟晶）极品圆珠光面手链', 6, N'个', 98.0000, N'', 59)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (416, N'圣诞精品特价促销包邮 天然水晶手链天然橄榄石手链戒面花', 6, N'个', 266.0000, N'', 31)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (417, N'神秘俏佳人*新款上市●☆天然茶晶手链（小）', 6, N'个', 66.0000, N'', 60)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (418, N'全场包邮★tiffany手链 五小O手链★925纯银饰品', 6, N'个', 45.0000, N'', 73)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (419, N'纯银饰品 玉石手链 时尚饰品 生日礼物 kd06', 6, N'个', 68.0000, N'', 79)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (420, N'施华洛世奇水晶手链（香槟色）', 6, N'个', 48.0000, N'', 33)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (421, N'【恋水晶】天然铜发晶手链 马达加斯加魔鬼发手链', 6, N'个', 160.0000, N'', 114)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (422, N'【恋水晶】AAA天然巴西黄水晶碎石手链 天然巴西黄水晶手链', 6, N'个', 95.0000, N'', 61)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (423, N'气质男款 镀金 镀铂金男士手链免运费包快递 男友的礼物 送赠品', 6, N'个', 38.8000, N'', 64)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (424, N'【冲钻特价】天然极品粉水晶个性手链', 6, N'个', 28.0000, N'', 110)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (425, N'神秘俏佳人*新款上市●☆天然黄玉小佛珠手链（小）', 6, N'个', 55.0000, N'', 85)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (426, N'神秘俏佳人*新款上市●☆天然黄玉佛珠手链（大）', 6, N'个', 70.0000, N'', 11)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (427, N'天然白水晶手链', 6, N'个', 88.0000, N'', 76)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (428, N'Tiffany925银正品 网状心型手链【女友生日绝美礼物】', 6, N'个', 140.0000, N'', 14)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (429, N'★爱琅阁★精致925纯银施华洛世奇水晶时尚手链全场包邮 圣诞礼物', 6, N'个', 98.0000, N'', 75)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (430, N'TIFFANY手链批发▲包邮+包装▲水泡狗牌手链925纯银手链情侣手链', 6, N'个', 53.0000, N'', 111)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (431, N'【银饰佳人】镶石满天星手镯 纯银饰品 高层韩国饰品29', 6, N'个', 111.0000, N'', 117)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (432, N'★银来福★925纯银 车花身童镯-单只 c4209', 6, N'个', 79.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (433, N'冰种红纹石12mm', 6, N'个', 380.0000, N'', 12)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (434, N'天然红虎石圆珠手链12MM', 6, N'个', 35.0000, N'', 72)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (435, N'天然水晶戒面花手链 天然石榴石紫水晶黄水晶橄榄石手链', 6, N'个', 380.0000, N'', 109)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (436, N'冲钻特价 天然黄水晶10MM', 6, N'个', 28.0000, N'', 43)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (437, N'B5★周大福款★皇冠70分锆钻手链★【晶心坊】包邮', 6, N'个', 58.0000, N'', 56)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (438, N'AS爱纱手链--高贵十字架腕带', 6, N'个', 89.0000, N'', 18)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (439, N'时尚流行生日礼物 925纯银镀白梅花镶施华洛水晶手链饰品', 6, N'个', 56.8000, N'', 110)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (440, N'热缩片示范应用--个性饰品 吊坠 耳坠 手链', 6, N'个', 38.0000, N'', 77)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (441, N'【Swarovski施华洛世奇】绿色风水晶手链饰品●专柜包装盒', 6, N'个', 98.0000, N'', 95)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (442, N'绿幽灵手链[打折]', 6, N'个', 88.0000, N'', 114)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (443, N'【秀饰良品100%好评双钻店】纯银正品Tiffany威尼斯方盒子手链', 6, N'个', 220.0000, N'', 76)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (444, N'【秀饰良品双钻店】Tiffany蜜桃别针手链 纯银正品100%好评', 6, N'个', 180.0000, N'', 115)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (445, N'【秀饰良品100%好评双钻店】纯银正品Tiffany闭口网镯', 6, N'个', 380.0000, N'', 107)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (446, N'◥◣鳅鳅的最爱◢◤韩国明星镀金皮手链', 6, N'个', 68.0000, N'', 104)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (447, N'银至尊Tiffany蒂凡尼925纯银首饰手链 TFBL101 精美圈圈 流行饰品', 6, N'个', 98.0000, N'', 64)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (448, N'银至尊Tiffany蒂凡尼925纯银首饰手链 TFBL131 串联珠珠 流行饰品', 6, N'个', 108.0000, N'', 57)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (449, N'五钻店【贵州印象】99纯银手工●手镯篇●订做●镶丝园镯', 6, N'个', 210.0000, N'', 30)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (450, N'银至尊Tiffany蒂凡尼925纯银首饰手链 TFBL134 字母锁链W流行饰品', 6, N'个', 128.0000, N'', 60)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (451, N'纯天然紫水晶手镯8mm', 6, N'个', 25.0000, N'', 81)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (452, N'外贸手链', 6, N'个', 30.0000, N'', 112)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (453, N'东方神起允浩在中戴的同款掉五1837手链', 6, N'个', 38.0000, N'', 37)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (454, N'特价包快递★5.5mm平安纹手链20.5cm 复古泰银龙头手链 海盗船', 6, N'个', 195.0000, N'', 70)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (455, N'天使之泪 珍珠首饰 流行饰品 手链手镯 珠宝首饰 6521吊牌价￥317', 6, N'个', 126.8000, N'', 111)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (456, N'TIFFANY 手链 五小心 手链 925纯银◆圣诞节佳礼+包装', 6, N'个', 49.0000, N'', 11)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (457, N'925银*时尚饰品*LINKS百圈子吊红心手链', 6, N'个', 60.0000, N'', 58)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (458, N'925银*时尚饰品*Links*粉玉色石头手链', 6, N'个', 38.0000, N'', 110)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (459, N'新店不为赢利只为信誉 Tiffany分色简约手镯', 6, N'个', 26.0000, N'', 18)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (460, N'TV购物热销/金镇抗疲劳手链/钛锗男女款手链', 6, N'个', 99.0000, N'', 104)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (461, N'圣诞精品特价促销包邮 天然水晶手链天然萤石手链', 6, N'个', 196.0000, N'', 90)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (462, N'925纯银饰品★tiffany手链 新盒子手链★新款上市', 6, N'个', 36.0000, N'', 105)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (463, N'天然水晶天然蓝月光石9MM手链精品特价促销', 6, N'个', 398.0000, N'', 109)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (464, N'★Echo★艾可儿华丽瑞丽VIVI盒子手链－70310005', 6, N'个', 39.0000, N'', 68)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (465, N'两钻信誉 镀白周大福骄人系列钻饰心愿手链*', 6, N'个', 37.9500, N'', 55)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (466, N'时尚饰品 镶石开口手镯 多层爱心女友礼物', 6, N'个', 117.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (467, N'冲三钻 纯银正品【Gucci 长牌圆珠手链】名牌珠宝专卖', 6, N'个', 135.0000, N'', 10)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (468, N'天然水晶*手链/手镯**精品橄榄绿戒面花手链**流行饰品', 6, N'个', 98.0000, N'', 33)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (469, N'822163瑞士钻石八心八箭纯银镀白金璀璨奢华物语耳钻', 7, N'个', 84.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (470, N'822164瑞士钻石八心八箭纯银镀白金璀璨奢华物语耳钻', 7, N'个', 141.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (471, N'100%好评★专柜正品㊣★E609银之堡◆925纯银圆角方形水晶耳环', 7, N'个', 120.0000, N'', 48)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (472, N'韩国进口耳环 韩网直购预定4天 19245璀璨水滴恋', 7, N'个', 103.0000, N'', 59)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (473, N'抵价券两件包邮韩国进口㊣皓石镶钻耳环E02624', 7, N'个', 72.0000, N'', 105)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (474, N'全美晶体 天然极品海蓝宝花生耳饰 自然清新 最正的颜色118.0', 7, N'个', 118.0000, N'', 45)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (475, N'【媄麗偂沿-冲冠特价】韩国进口-明星耳环*E01455', 7, N'个', 40.0000, N'', 7)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (476, N'【媄麗偂沿-冲冠特价】韩国进口-明星耳环*E01524', 7, N'个', 58.0000, N'', 25)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (477, N'【媄麗偂沿-冲冠特价】韩国进口-明星耳环*E01526', 7, N'个', 58.0000, N'', 10)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (478, N'【媄麗偂沿-冲冠特价】韩国进口-明星耳环*E01450', 7, N'个', 60.0000, N'', 82)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (479, N'【媄麗偂沿-冲冠特价】韩国进口-925银耳环*ES09056', 7, N'个', 68.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (480, N'【媄麗偂沿-冲冠特价】韩国进口-明星耳环*E01453', 7, N'个', 85.0000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (481, N'*sweetday*意大利摩登饰品 金色富贵新古典母贝宝瓶耳坠', 7, N'个', 65.0000, N'', 74)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (482, N'韩国进口饰品*可爱耳环/耳钩/韩国进口耳饰EL2746', 7, N'个', 48.0000, N'', 21)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (483, N'镀白周大福南瓜皮靴耳坠', 7, N'个', 34.5000, N'', 58)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (484, N'清仓特价！Moon Cat耳饰-－马毛斑点小鹿', 7, N'个', 29.0000, N'', 5)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (485, N'时尚链舞品牌饰品Tiffany(蒂梵尼)1837大耳环', 7, N'个', 47.0000, N'', 104)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (486, N'㊣925银饰品新款耳环', 7, N'个', 48.0000, N'', 69)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (487, N'上海女孩*四钻信用*韩国进口 甜美彩色珠珠耳环', 7, N'个', 29.0000, N'', 51)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (488, N'925银镀白金,黑玛瑙菱形耳环', 7, N'个', 72.5000, N'', 30)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (489, N'925银镀白金,全钻大钥匙耳环', 7, N'个', 89.0000, N'', 82)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (490, N'【韩国首饰预订】代购母贝晶莹别致花朵水晶流苏耳圈', 7, N'个', 78.0000, N'', 95)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (491, N'冲皇冠 威妮华新款 K金蓝色彩釉珍珠水晶耳环/耳夹/耳钉/礼物', 7, N'个', 59.0000, N'', 82)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (492, N'百分百925纯银首饰★酷酷方耳圈耳环', 7, N'个', 88.0000, N'', 31)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (493, N'【三钻买38元包邮】 珍珠 耳饰耳钉耳坠耳环 9733吊牌价100元', 7, N'个', 40.0000, N'', 109)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (494, N'【淑馨苑】★圆光面珠韩版耳饰★925银★长宽16*10MM.重3.4克.', 7, N'个', 28.9000, N'', 78)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (495, N'〓韩国直运〓~韩国耳环~韩星佩带的华丽耳环2179', 7, N'个', 56.0000, N'', 57)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (496, N'〓韩国直运〓~韩国耳环~华丽的满钻雪花耳坠2172', 7, N'个', 55.0000, N'', 35)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (497, N'〓韩国直运〓~韩国耳环~气质款叶型镶钻耳环2191', 7, N'个', 65.0000, N'', 9)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (498, N'〓韩国直运〓~韩国耳环~张真英佩带的多切面水晶耳环2157', 7, N'个', 86.0000, N'', 69)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (499, N'〓韩国直运〓~韩国耳环~多切面高贵蓝水晶纯银耳环1281 预订款', 7, N'个', 80.0000, N'', 94)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (500, N'〓韩国直运〓~韩国耳环~宋惠乔佩带的椭圆流苏耳环1645', 7, N'个', 39.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (501, N'皇冠店新品韩国直送施华洛世奇OL水晶爱人心 925纯银耳环-见详情', 7, N'个', 146.0000, N'', 52)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (502, N'Magic Box 韩网直供 圣诞专题 明星款欢声笑语可爱小鹿 耳饰', 7, N'个', 49.9900, N'', 99)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (503, N'〓韩国直运〓~韩国耳环~朴善英在韩剧中佩带的蝴蝶结水钻耳环1027', 7, N'个', 38.0000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (504, N'〓韩国直运〓~韩国耳环~方型水晶耳丁2148', 7, N'个', 38.0000, N'', 64)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (505, N'〓韩国直运〓~韩国耳环~简洁时尚的十字架耳环2145', 7, N'个', 39.5000, N'', 20)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (506, N'〓韩国直运〓~韩国耳环~李孝利佩带的镂空十字架耳环1146 预定款', 7, N'个', 65.0000, N'', 31)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (507, N'︶ㄣ梦幽阁~精品银饰/绞花精品耳坠 e12051/电镀白金 防氧化处理', 7, N'个', 32.0000, N'', 64)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (508, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01676', 7, N'个', 36.9000, N'', 9)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (509, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01673', 7, N'个', 33.3000, N'', 21)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (510, N'★时尚流行.复古风情★纯银 泰银花环耳坠109050', 7, N'个', 38.0000, N'', 99)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (511, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01665', 7, N'个', 33.3000, N'', 93)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (512, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01672', 7, N'个', 63.9000, N'', 113)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (513, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01664', 7, N'个', 47.7000, N'', 43)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (514, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01666', 7, N'个', 63.9000, N'', 73)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (515, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01667', 7, N'个', 33.3000, N'', 10)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (516, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01655', 7, N'个', 45.0000, N'', 59)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (517, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01663', 7, N'个', 59.4000, N'', 84)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (518, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01651', 7, N'个', 90.9000, N'', 91)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (519, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01650', 7, N'个', 36.9000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (520, N'●．妞妞┇佈丁．●韩国同步流行时尚耳饰EL01656', 7, N'个', 36.9000, N'', 94)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (521, N'925银饰品 tiffany银饰系列 X交叉耳坠', 7, N'个', 32.0000, N'', 50)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (522, N'925银饰品 tiffany银饰系列 正心长链耳坠', 7, N'个', 32.0000, N'', 66)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (523, N'外贸出口尾单★新款立体坠长钩金色耳饰', 7, N'个', 30.0000, N'', 89)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (524, N'外贸出口尾单★新款金色滴釉蝴蝶可爱耳饰环', 7, N'个', 30.0000, N'', 71)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (525, N'外贸出口尾单★新款银兰滴釉菱形流苏耳饰', 7, N'个', 32.0000, N'', 55)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (526, N'外贸出口尾单★新款银色滴釉流苏梅花耳饰', 7, N'个', 30.0000, N'', 17)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (527, N'外贸出口尾单★新款三色滴釉流苏心状耳饰', 7, N'个', 30.0000, N'', 67)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (528, N'◣钻石信誉◥100%信誉--韩国进口耳环---珍珠贝壳圈', 7, N'个', 55.0000, N'', 31)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (529, N'◣钻石信誉◥非代购【韩国进口】耳环--明星款-紫钻双十字架双花', 7, N'个', 90.0000, N'', 108)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (530, N'◣钻石信誉◥韩国进口耳环--明星款-时尚魅力双C流苏', 7, N'个', 39.0000, N'', 96)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (531, N'【韩国进口*可爱的小女孩耳环】★木木宝贝韩饰★', 7, N'个', 35.0000, N'', 25)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (532, N'【韩国进口*超赞淑女款水滴花朵耳环】★木木宝贝韩饰★', 7, N'个', 96.0000, N'', 53)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (533, N'【韩国直送*水晶奢华款花朵闪耀大耳圈】★木木宝贝韩饰★', 7, N'个', 102.0000, N'', 112)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (534, N'【新光】花语 水晶耳环', 7, N'个', 30.5500, N'', 5)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (535, N'韩国进口㊣恋期90天金荷娜款水钻水滴型耳饰', 7, N'个', 70.0000, N'', 103)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (536, N'韩国进口㊣韩剧明星空镜子金色镶钻耳饰', 7, N'个', 139.0000, N'', 101)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (537, N'〓韩国直运〓~韩国耳环~气质型镐玛瑙水钻耳环1303', 7, N'个', 83.0000, N'', 108)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (538, N'〓韩国直运〓~韩国耳环~对十字水钻耳环2130', 7, N'个', 58.5000, N'', 114)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (539, N'外贸出口尾单★新款多色桃心时尚耳饰', 7, N'个', 32.0000, N'', 108)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (540, N'〓韩国直运〓~韩国耳环~高贵气质的水钻流苏耳环2136', 7, N'个', 69.0000, N'', 77)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (541, N'外贸出口尾单★新款金银环新颖耳饰', 7, N'个', 30.0000, N'', 7)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (542, N'只赚5元·钻石信誉韩国进口饰品满钻。爱心耳环EW1043-1两色', 7, N'个', 72.0000, N'', 47)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (543, N'钻石信誉~韩国进口-92.5纯银璀璨锆石圆球耳环e0965', 7, N'个', 112.0000, N'', 8)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (544, N'欧美名牌ARTINI''S JEWELRY雅天妮思品牌贝壳天使翅膀心耳饰', 7, N'个', 45.0000, N'', 104)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (545, N'外贸出口尾单★新款双色梅花镂空镶钻流苏耳饰', 7, N'个', 30.0000, N'', 5)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (546, N'『冲皇冠特惠』☆韩国进口☆时尚圆牌大圈耳环 特价', 7, N'个', 30.0000, N'', 91)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (547, N'韩国Cutieangel原装正品边夹 NO.HO1390', 8, N'个', 48.0000, N'', 70)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (548, N'◢☆心晴冲5钻★北京◣韩国进口彩色毛球蝴蝶结小猴子发绳~4色', 8, N'个', 45.0000, N'', 103)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (549, N'【蓝鸟BLUE BIRD】●千千雅阁●镶钻四瓣花皮筋/粉色', 8, N'个', 37.0000, N'', 57)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (550, N'新光饰品 新光发饰 新光发夹 施华洛世奇水钻横夹 送女友生日礼物', 8, N'个', 115.0000, N'', 60)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (551, N'特价 新光饰品 水钻发饰 水钻发夹 水钻边夹 水钻发卡 水钻单夹', 8, N'个', 27.0000, N'', 11)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (552, N'新光饰品 新光发饰 新光发夹 水钻发夹 水钻对夹 送女友生日礼物', 8, N'个', 33.0000, N'', 78)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (553, N'两钻信誉FB12*满钻竖卡竖夹发夹*新光水晶68折特卖', 8, N'个', 63.8000, N'', 38)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (554, N'fairybox 韩国进口 韩国发饰 韩国直送 雅致花朵发圈', 8, N'个', 38.1800, N'', 97)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (555, N'包邮,满99元9折包邮,满199元8折包邮,满百包邮,新光,红色蝴蝶发绳', 8, N'个', 56.0000, N'', 24)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (556, N'甜甜新光饰品※最新款银色满钻海星竖夹7244※施华洛世奇水晶', 8, N'个', 43.8000, N'', 57)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (557, N'特价/FQ303★韩国发饰★BOBO韩国进口女孩头圈(发圈)', 8, N'个', 28.0000, N'', 70)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (558, N'特价/J331韩国进口发饰◆韩国的粉色长颈鹿香蕉夹', 8, N'个', 28.0000, N'', 29)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (559, N'用支付宝享88折【满钻红粉佳人】成对发夹C6019紫', 8, N'个', 25.0000, N'', 84)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (560, N'★新光饰品★【钻石信誉】银底满钻海琴湾新光发簪', 8, N'个', 58.0000, N'', 64)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (561, N'★新光饰品★【钻石信誉】银底黑锆幸运四叶花新光发簪', 8, N'个', 29.8000, N'', 62)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (562, N'★新光饰品★【钻石信誉】银版彩钻菱形星光璀璨新光发箍', 8, N'个', 39.9000, N'', 74)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (563, N'★新光饰品★【钻石信誉】银底紫钻双双XX新光发箍', 8, N'个', 36.5000, N'', 57)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (564, N'法国正品*Aznavour*奇妙图案发夹FA1178双色', 8, N'个', 180.0000, N'', 59)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (565, N'『流行美饰』优雅孔雀 一字夹 （红、黄、蓝三色选择）', 8, N'个', 36.0000, N'', 37)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (566, N'★新光饰品★【钻石信誉】银底紫锆一生钟情新光发箍', 8, N'个', 49.9000, N'', 59)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (567, N'★新光饰品★【钻石信誉】银底粉钻心中的王者新光发箍', 8, N'个', 49.9000, N'', 112)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (568, N'★新光饰品★【钻石信誉】银底蓝锆一生钟情新光发箍', 8, N'个', 49.9000, N'', 14)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (569, N'【现货】华丽黑白双色宝石蝴蝶抓夹#703812', 8, N'个', 38.0000, N'', 72)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (570, N'【现货】气质款蝴蝶珍珠流苏香蕉夹#703823', 8, N'个', 40.0000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (571, N'◆五钻信誉◆Q2636Herlenar海伦尔头饰发饰 缨子花大发圈 黑色', 8, N'个', 32.0000, N'', 69)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (572, N'*双钻信誉*檀香木发簪*雕刻发簪*工艺发簪*民族工艺*柔美发簪', 8, N'个', 39.0000, N'', 45)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (573, N'甜甜新光饰品※最新款紫色满钻竖夹7267※施华洛世奇水晶', 8, N'个', 137.5000, N'', 52)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (574, N'*双钻信誉*檀香木发簪*雕刻发簪*工艺发簪*生肖簪', 8, N'个', 40.0000, N'', 23)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (575, N'*双钻信誉*檀香木发簪*雕刻发簪*工艺发簪*旋律簪', 8, N'个', 27.0000, N'', 15)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (576, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*芳华簪', 8, N'个', 39.0000, N'', 17)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (577, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*发簪', 8, N'个', 35.0000, N'', 102)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (578, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*发簪', 8, N'个', 39.5000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (579, N'*双钻信誉*檀香木发簪*雕刻发簪*工艺发簪*民族工艺*幸福簪', 8, N'个', 66.0000, N'', 105)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (580, N'*双钻信誉*檀香木发簪*雕刻发簪*工艺发簪*优美簪', 8, N'个', 59.0000, N'', 64)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (581, N'*双钻信誉*檀香木发簪*雕刻发簪*工艺发簪*民族工艺*发簪', 8, N'个', 34.5000, N'', 39)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (582, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*民族工艺*发簪', 8, N'个', 29.9000, N'', 72)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (583, N'*双钻信誉*绿檀木发簪*雕刻发簪*发簪', 8, N'个', 39.5000, N'', 97)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (584, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*暗香簪*发簪', 8, N'个', 29.9000, N'', 38)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (585, N'*双钻信誉*檀香木发簪*雕刻发簪*工艺发簪*百和簪', 8, N'个', 39.5000, N'', 89)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (586, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*希望簪*发簪', 8, N'个', 29.9000, N'', 74)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (587, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*发簪', 8, N'个', 39.9900, N'', 88)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (588, N'*双钻信誉*檀香木发簪*雕刻发簪*工艺发簪*民族工艺*柔美发簪', 8, N'个', 29.9900, N'', 57)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (589, N'*双钻信誉*桃木发簪*雕刻发簪*工艺发簪*发簪', 8, N'个', 49.9900, N'', 9)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (590, N'*双钻信誉*檀香木发簪*雕刻发簪*工艺发簪*民族工艺*发簪', 8, N'个', 29.9900, N'', 9)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (591, N'韩国进口㊣selene畅销甜美果冻版小花中号发抓', 8, N'个', 69.0000, N'', 15)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (592, N'*双钻信誉*桃木发簪*雕刻发簪*工艺发簪*发簪', 8, N'个', 25.0000, N'', 85)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (593, N'*双钻信誉*桃木发簪*雕刻发簪*工艺发簪*发簪*梅花簪', 8, N'个', 54.9900, N'', 83)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (594, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*纯爱簪*发簪', 8, N'个', 49.5000, N'', 97)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (595, N'*双钻信誉*桃木发簪*雕刻发簪*工艺发簪*发簪*虚心簪', 8, N'个', 34.9000, N'', 55)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (596, N'*双钻信誉*檀香木发簪*雕刻发簪*工艺发簪*魅力簪', 8, N'个', 68.9000, N'', 107)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (597, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*发簪', 8, N'个', 42.5000, N'', 96)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (598, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*温柔兰花簪', 8, N'个', 32.5000, N'', 67)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (599, N'*双钻信誉*黄杨木发簪*雕刻发簪*', 8, N'个', 35.9000, N'', 72)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (600, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*兰花簪*发簪', 8, N'个', 30.0000, N'', 52)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (601, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*发簪', 8, N'个', 29.0000, N'', 96)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (602, N'韩国正宗KKNEKKI发绳 双花戏珠', 8, N'个', 29.0000, N'', 110)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (603, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*民族工艺*发簪', 8, N'个', 59.5000, N'', 98)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (604, N'*双钻信誉*黄杨木发簪*雕刻发簪*工艺发簪*梅花簪*发簪', 8, N'个', 38.0000, N'', 36)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (605, N'YDXZH01186韩国头饰-&gt;发夹', 8, N'个', 27.0000, N'', 110)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (606, N'☆50元2件包邮★发饰达人韩版彩色水晶韩国狗雪纳瑞一字夹边夹', 8, N'个', 28.0000, N'', 85)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (607, N'美人钗头饰双钻信誉◆Kimling发饰◆M154黑色百变发梳', 8, N'个', 25.0000, N'', 24)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (608, N'韩国饰品施华洛世奇水晶SWAROVSKI竖夹香蕉夹SS02 白咖蓝粉黑8色', 8, N'个', 150.0000, N'', 14)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (609, N'【Blue Bird头饰发饰韩国饰品】大号苹果发圈LQ06(特价)红橘绿3色', 8, N'个', 38.0000, N'', 82)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (610, N'韩国发饰/头饰 施华洛世奇水晶SWAROVSKI水滴水晶边夹SJ29 2色', 8, N'个', 118.0000, N'', 5)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (611, N'韩国饰品代购直送进口发饰品花发圈发绳植物-F03072时尚礼物', 8, N'个', 48.0000, N'', 16)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (612, N'3钻正品★韩国进口☆法国Aznavour Paris双色珍珠花朵发夹 单只', 8, N'个', 32.0000, N'', 36)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (613, N'3钻★韩国进口☆法国Aznavour Paris闪亮水晶钮扣发夹发饰 单只', 8, N'个', 29.0000, N'', 42)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (614, N'3钻★韩国进口☆法国Aznavour Paris单边双层丝带发夹发饰 3色', 8, N'个', 82.0000, N'', 20)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (615, N'3钻正品★韩国进口☆法国Aznavour Paris糖果圆片发夹发饰 单只', 8, N'个', 29.0000, N'', 26)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (616, N'3钻正品★韩国进口☆法国Aznavour Paris糖果蝴蝶结发夹发饰 单只', 8, N'个', 28.0000, N'', 61)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (617, N'美丽五叶花-卓雅妮专柜精品', 8, N'个', 29.0000, N'', 98)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (618, N'新光饰品☆施华洛世奇☆锆石镶钻心绳圈', 8, N'个', 72.1000, N'', 19)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (619, N'红袖添香，做个古典美人－天然淡水珍珠发簪', 8, N'个', 30.0000, N'', 34)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (620, N'fairybox 韩国进口 韩国首饰 韩国直送 绸带蝴蝶发夹', 8, N'个', 40.0000, N'', 81)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (621, N'甜甜新光饰品※最新款透明锆石满钻蝴蝶竖夹7245※施华洛世奇水晶', 8, N'个', 106.2000, N'', 89)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (622, N'★100%韩国直送★*发饰*木质翘尾蝴蝶结猫猫发圈 2色特价', 8, N'个', 28.0000, N'', 55)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (623, N'【蓝鸟BLUE BIRD】●千千雅阁●满钻蝴蝶结发圈/粉色', 8, N'个', 116.0000, N'', 5)
INSERT [dbo].[Products] ([ProductID], [ProductName], [CategoryID], [Unit], [UnitPrice], [Remark], [Quantity]) VALUES (624, N'【韩国进口 发饰/头饰】七彩金沙琉璃波浪细发箍发卡KG13（特价）', 8, N'个', 25.0000, N'', 116)
SET IDENTITY_INSERT Products OFF
GO
GO



/****** Object:  Index [IX_CategoryID]    Script Date: 2019/7/19 20:43:29 ******/
CREATE NONCLUSTERED INDEX [IX_CategoryID] ON [dbo].[Products]
(
	[CategoryID] ASC
)
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_Finished]  DEFAULT ((0)) FOR [Finished]
GO
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_Unit]  DEFAULT ('个') FOR [Unit]
GO
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_UnitPrice]  DEFAULT ((0)) FOR [UnitPrice]
GO
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_Amount]  DEFAULT ((0)) FOR [Quantity]
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

ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Orders]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Products]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Customers]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categories] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([CategoryID])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categories]
GO


/****** Object:  View [dbo].[OrderDetailView]    Script Date: 2019/7/19 20:43:29 ******/
CREATE VIEW [dbo].[OrderDetailView]
AS
SELECT     dbo.[OrderDetails].*, dbo.Products.ProductName, dbo.Products.Unit
FROM         dbo.[OrderDetails] INNER JOIN
                      dbo.Products ON dbo.[OrderDetails].ProductID = dbo.Products.ProductID
GO


/****** Object:  View [dbo].[OrdersView]    Script Date: 2019/7/19 20:43:29 ******/
CREATE VIEW [dbo].[OrdersView]
AS
SELECT     dbo.Orders.OrderID, dbo.Orders.CustomerID, dbo.Orders.OrderDate, dbo.Orders.SumMoney, dbo.Orders.Finished, 
                      ISNULL(dbo.Customers.CustomerName, N'') AS CustomerName
FROM         dbo.Orders LEFT OUTER JOIN
                      dbo.Customers ON dbo.Orders.CustomerID = dbo.Customers.CustomerID
GO







/****** Object:  StoredProcedure [dbo].[InsertCustomer]    Script Date: 2019/7/19 20:43:29 ******/
create procedure [dbo].[InsertCustomer]( 
    @CustomerName nvarchar(50), 
    @ContactName nvarchar(50), 
    @Address nvarchar(50), 
    @PostalCode nvarchar(10), 
    @Tel nvarchar(50) ,
	@CustomerID int output
) 
as
begin
insert into Customers (CustomerName, ContactName, [Address], PostalCode, Tel) 
values( @CustomerName, @ContactName, @Address, @PostalCode, @Tel);

set @CustomerID = scope_identity();

end
GO


/****** Object:  StoredProcedure [dbo].[GetCustomerById]    Script Date: 2019/7/19 20:43:29 ******/
create procedure [dbo].[GetCustomerById](
	@CustomerID int
)
as
select * from Customers where CustomerID = @CustomerID;
GO



/****** Object:  StoredProcedure [dbo].[UpdateCustomer]    Script Date: 2019/7/19 20:43:29 ******/
create procedure [dbo].[UpdateCustomer](  
    @CustomerName nvarchar(50),  
    @ContactName nvarchar(50),  
    @Address nvarchar(50),  
    @PostalCode nvarchar(10),  
    @Tel nvarchar(50),  
    @CustomerID int
)  
as
update Customers  
set CustomerName = @CustomerName,  
    ContactName = @ContactName,  
    [Address] = @Address,  
    PostalCode = @PostalCode,  
    Tel = @Tel  
where CustomerID = @CustomerID;
GO



/****** Object:  StoredProcedure [dbo].[DeleteCustomer]    Script Date: 2019/7/19 20:43:29 ******/
create procedure [dbo].[DeleteCustomer](
	@CustomerID int
)
as
delete from Customers
where CustomerID = @CustomerID;
GO




/****** Object:  StoredProcedure [dbo].[GetCustomerList]    Script Date: 2019/7/19 20:43:29 ******/
create procedure [dbo].[GetCustomerList](
    @SearchWord nvarchar(50),
    @PageIndex int = 0,
    @PageSize int = 20,
    @TotalRecords int output
)
as
begin
 
set @SearchWord = N'%' + @SearchWord + N'%';

      
select  @TotalRecords = count(*) 
from  Customers as c
where c.CustomerName like @SearchWord;


select * from(
	select row_number() over (order by c.CustomerID asc) as RowIndex, c.*
	from   Customers as c
) as t
where  t.RowIndex > (@PageSize * @PageIndex) and t.RowIndex <= (@PageSize * (@PageIndex+1));
    
end;
GO



