/****** Object:  StoredProcedure [dbo].[GetPagePermission]    Script Date: 4/27/2021 4:38:16 PM ******/
DROP PROCEDURE IF EXISTS [dbo].[GetPagePermission]
GO
/****** Object:  StoredProcedure [dbo].[GetMenuWithPermission]    Script Date: 4/27/2021 4:38:16 PM ******/
DROP PROCEDURE IF EXISTS [dbo].[GetMenuWithPermission]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserMenuPermission]') AND type in (N'U'))
ALTER TABLE [dbo].[UserMenuPermission] DROP CONSTRAINT IF EXISTS [FK_UserMenuPermission_User]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserMenuPermission]') AND type in (N'U'))
ALTER TABLE [dbo].[UserMenuPermission] DROP CONSTRAINT IF EXISTS [FK_UserMenuPermission_Menu]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleMenuPermission]') AND type in (N'U'))
ALTER TABLE [dbo].[RoleMenuPermission] DROP CONSTRAINT IF EXISTS [FK_RoleMenuPermission_Role]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleMenuPermission]') AND type in (N'U'))
ALTER TABLE [dbo].[RoleMenuPermission] DROP CONSTRAINT IF EXISTS [FK_RoleMenuPermission_Menu]
GO
/****** Object:  Table [dbo].[UserMenuPermission]    Script Date: 4/27/2021 4:38:16 PM ******/
DROP TABLE IF EXISTS [dbo].[UserMenuPermission]
GO
/****** Object:  Table [dbo].[RoleMenuPermission]    Script Date: 4/27/2021 4:38:16 PM ******/
DROP TABLE IF EXISTS [dbo].[RoleMenuPermission]
GO
/****** Object:  Table [dbo].[RoleMenuPermission]    Script Date: 4/27/2021 4:38:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleMenuPermission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NULL,
	[MenuId] [int] NULL,
	[CanbeAdd] [bit] NULL,
	[CanbeEdit] [bit] NULL,
	[CanbeDelete] [bit] NULL,
	[CanbeView] [bit] NULL,
	[CanbeGive] [bit] NULL,
 CONSTRAINT [PK_RoleMenuPermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserMenuPermission]    Script Date: 4/27/2021 4:38:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMenuPermission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[MenuId] [int] NULL,
	[CanbeAdd] [bit] NULL,
	[CanbeEdit] [bit] NULL,
	[CanbeDelete] [bit] NULL,
	[CanbeView] [bit] NULL,
	[CanbeGive] [bit] NULL,
 CONSTRAINT [PK_UserMenuPermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[RoleMenuPermission] ON 
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (1, 1, 2, 1, 1, 1, 1, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (2, 2, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (3, 3, 2, 1, 0, 0, 1, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (4, 4, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (5, 5, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (6, 6, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (7, 7, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (8, 8, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (9, 23, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (10, 25, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (11, 26, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (12, 27, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (13, 28, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (14, 1, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (15, 2, 1, 1, 1, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (16, 3, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (17, 4, 1, 1, 1, 0, 1, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (18, 5, 1, 0, 1, 1, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (19, 6, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (20, 7, 1, 0, 0, 0, 1, 1)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (21, 8, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (22, 23, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (23, 25, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (24, 26, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (25, 27, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[RoleMenuPermission] ([Id], [RoleId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (26, 28, 1, 0, 0, 0, 0, 0)
GO
SET IDENTITY_INSERT [dbo].[RoleMenuPermission] OFF
GO
SET IDENTITY_INSERT [dbo].[UserMenuPermission] ON 
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (1, 112, 1, 1, 1, 0, 1, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (2, 112, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (3, 125, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (4, 125, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (5, 113, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (6, 113, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (7, 6, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (8, 6, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (9, 18, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (10, 18, 2, 1, 1, 1, 1, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (11, 148, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (12, 148, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (13, 42, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (14, 42, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (15, 15, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (16, 15, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (17, 14, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (18, 14, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (19, 79, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (20, 79, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (21, 52, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (22, 52, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (23, 19, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (24, 19, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (25, 80, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (26, 80, 2, 1, 0, 0, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (27, 126, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (28, 126, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (29, 149, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (30, 149, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (31, 62, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (32, 62, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (33, 34, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (34, 34, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (35, 36, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (36, 36, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (37, 41, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (38, 41, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (39, 90, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (40, 90, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (41, 104, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (42, 104, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (43, 2, 1, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (44, 2, 2, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (45, 97, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (46, 97, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (47, 82, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (48, 82, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (49, 93, 1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (50, 93, 2, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (51, 112, 1002, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (52, 112, 1003, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (53, 112, 1004, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (54, 112, 1005, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (55, 112, 1006, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (56, 112, 1007, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (57, 112, 1008, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (58, 112, 1009, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (59, 112, 1010, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (60, 112, 1011, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (61, 112, 1012, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (62, 112, 1013, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (63, 112, 1014, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (64, 112, 1015, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (65, 112, 1016, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (66, 112, 1017, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (67, 112, 1018, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (68, 90, 1002, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (69, 90, 1003, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (70, 90, 1004, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (71, 90, 1005, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (72, 90, 1006, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (73, 90, 1007, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (74, 90, 1008, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (75, 90, 1009, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (76, 90, 1010, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (77, 90, 1011, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (78, 90, 1012, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (79, 90, 1013, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (80, 90, 1014, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (81, 90, 1015, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (82, 90, 1016, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (83, 90, 1017, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (84, 90, 1018, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (85, 104, 1002, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (86, 104, 1003, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (87, 104, 1004, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (88, 104, 1005, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (89, 104, 1006, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (90, 104, 1007, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (91, 104, 1008, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (92, 104, 1009, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (93, 104, 1010, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (94, 104, 1011, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (95, 104, 1012, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (96, 104, 1013, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (97, 104, 1014, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (98, 104, 1015, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (99, 104, 1016, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (100, 104, 1017, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (101, 104, 1018, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (102, 125, 1002, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (103, 125, 1003, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (104, 125, 1004, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (105, 125, 1005, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (106, 125, 1006, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (107, 125, 1007, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (108, 125, 1008, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (109, 125, 1009, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (110, 125, 1010, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (111, 125, 1011, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (112, 125, 1012, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (113, 125, 1013, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (114, 125, 1014, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (115, 125, 1015, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (116, 125, 1016, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (117, 125, 1017, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (118, 125, 1018, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (119, 2, 1002, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (120, 2, 1003, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (121, 2, 1004, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (122, 2, 1005, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (123, 2, 1006, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (124, 2, 1007, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (125, 2, 1008, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (126, 2, 1009, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (127, 2, 1010, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (128, 2, 1011, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (129, 2, 1012, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (130, 2, 1013, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (131, 2, 1014, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (132, 2, 1015, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (133, 2, 1016, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (134, 2, 1017, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (135, 2, 1018, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (136, 97, 1002, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (137, 97, 1003, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (138, 97, 1004, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (139, 97, 1005, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (140, 97, 1006, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (141, 97, 1007, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (142, 97, 1008, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (143, 97, 1009, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (144, 97, 1010, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (145, 97, 1011, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (146, 97, 1012, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (147, 97, 1013, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (148, 97, 1014, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (149, 97, 1015, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (150, 97, 1016, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (151, 97, 1017, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (152, 97, 1018, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (153, 113, 1002, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (154, 113, 1003, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (155, 113, 1004, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (156, 113, 1005, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (157, 113, 1006, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (158, 113, 1007, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (159, 113, 1008, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (160, 113, 1009, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (161, 113, 1010, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (162, 113, 1011, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (163, 113, 1012, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (164, 113, 1013, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (165, 113, 1014, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (166, 113, 1015, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (167, 113, 1016, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (168, 113, 1017, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (169, 113, 1018, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (170, 82, 1002, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (171, 82, 1003, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (172, 82, 1004, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (173, 82, 1005, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (174, 82, 1006, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (175, 82, 1007, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (176, 82, 1008, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (177, 82, 1009, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (178, 82, 1010, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (179, 82, 1011, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (180, 82, 1012, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (181, 82, 1013, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (182, 82, 1014, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (183, 82, 1015, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (184, 82, 1016, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (185, 82, 1017, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (186, 82, 1018, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (187, 93, 1002, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (188, 93, 1003, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (189, 93, 1004, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (190, 93, 1005, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (191, 93, 1006, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (192, 93, 1007, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (193, 93, 1008, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (194, 93, 1009, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (195, 93, 1010, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (196, 93, 1011, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (197, 93, 1012, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (198, 93, 1013, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (199, 93, 1014, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (200, 93, 1015, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (201, 93, 1016, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (202, 93, 1017, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (203, 93, 1018, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (204, 6, 1002, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (205, 6, 1003, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (206, 6, 1004, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (207, 6, 1005, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (208, 6, 1006, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (209, 6, 1007, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (210, 6, 1008, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (211, 6, 1009, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (212, 6, 1010, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (213, 6, 1011, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (214, 6, 1012, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (215, 6, 1013, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (216, 6, 1014, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (217, 6, 1015, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (218, 6, 1016, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (219, 6, 1017, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (220, 6, 1018, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (221, 112, 1019, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (222, 112, 1020, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (223, 90, 1019, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (224, 90, 1020, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (225, 104, 1019, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (226, 104, 1020, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (227, 125, 1019, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (228, 125, 1020, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (229, 2, 1019, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (230, 2, 1020, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (231, 97, 1019, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (232, 97, 1020, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (233, 113, 1019, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (234, 113, 1020, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (235, 82, 1019, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (236, 82, 1020, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (237, 93, 1019, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (238, 93, 1020, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (239, 6, 1019, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[UserMenuPermission] ([Id], [UserId], [MenuId], [CanbeAdd], [CanbeEdit], [CanbeDelete], [CanbeView], [CanbeGive]) VALUES (240, 6, 1020, 0, 0, 0, 0, 0)
GO
SET IDENTITY_INSERT [dbo].[UserMenuPermission] OFF
GO
ALTER TABLE [dbo].[RoleMenuPermission]  WITH CHECK ADD  CONSTRAINT [FK_RoleMenuPermission_Menu] FOREIGN KEY([MenuId])
REFERENCES [dbo].[Menu] ([Id])
GO
ALTER TABLE [dbo].[RoleMenuPermission] CHECK CONSTRAINT [FK_RoleMenuPermission_Menu]
GO
ALTER TABLE [dbo].[RoleMenuPermission]  WITH CHECK ADD  CONSTRAINT [FK_RoleMenuPermission_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[RoleMenuPermission] CHECK CONSTRAINT [FK_RoleMenuPermission_Role]
GO
ALTER TABLE [dbo].[UserMenuPermission]  WITH CHECK ADD  CONSTRAINT [FK_UserMenuPermission_Menu] FOREIGN KEY([MenuId])
REFERENCES [dbo].[Menu] ([Id])
GO
ALTER TABLE [dbo].[UserMenuPermission] CHECK CONSTRAINT [FK_UserMenuPermission_Menu]
GO
ALTER TABLE [dbo].[UserMenuPermission]  WITH CHECK ADD  CONSTRAINT [FK_UserMenuPermission_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserLogin] ([Id])
GO
ALTER TABLE [dbo].[UserMenuPermission] CHECK CONSTRAINT [FK_UserMenuPermission_User]
GO
/****** Object:  StoredProcedure [dbo].[GetMenuWithPermission]    Script Date: 4/27/2021 4:38:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetMenuWithPermission]
	-- Add the parameters for the stored procedure here
	@userId Int,
	@gameId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select * from (Select 
			mm.Id,
			mm.MenuName,
			mm.MenuUrl,
			mm.ParentID,
			mm.IsActive,
			mm.Controller,
			mm.Action,
			Case When 
			(Select ump.CanbeEdit from Menu m
			left join UserMenupermission ump on m.Id = ump.MenuId
			left join UserLogin ul on ump.userId= ul.Id
			where ul.Id = @userId and m.Id = mm.Id) =1 Then (Select ump.CanbeEdit from Menu m
			left join UserMenupermission ump on m.Id = ump.MenuId
			left join UserLogin ul on ump.userId= ul.Id
			where ul.Id = @userId and m.Id = mm.Id)
			Else 
			(Select rmp.CanbeEdit
			from Menu m
			left join RoleMenuPermission rmp on m.Id = rmp.MenuId
			left join Role r on r.Id = rmp.RoleId
			left join GamePlayer gp on gp.RoleId = r.Id
			left join UserLogin ul on ul.Id= gp.UserId 
			and ul.Id = gp.UserId
			where ul.Id = @userId and gp.GameId =@gameId and m.Id = mm.Id)
			End as CanBeEdit,


			Case When 
			(Select ump.CanbeView from Menu m
			left join UserMenupermission ump on m.Id = ump.MenuId
			left join UserLogin ul on ump.userId= ul.Id
			where ul.Id = @userId and m.Id = mm.Id) =1 Then (Select ump.CanbeView from Menu m
			left join UserMenupermission ump on m.Id = ump.MenuId
			left join UserLogin ul on ump.userId= ul.Id
			where ul.Id = @userId and m.Id = mm.Id)
			else 
			(Select rmp.CanbeView
			from Menu m
			left join RoleMenuPermission rmp on m.Id = rmp.MenuId
			left join Role r on r.Id = rmp.RoleId
			left join GamePlayer gp on gp.RoleId = r.Id
			left join UserLogin ul on ul.Id= gp.UserId 
			and ul.Id = gp.UserId
			where ul.Id = @userId and gp.GameId =@gameId and m.Id = mm.Id)
			End as CanbeView,

			Case When 
			(Select ump.CanbeAdd from Menu m
			left join UserMenupermission ump on m.Id = ump.MenuId
			left join UserLogin ul on ump.userId= ul.Id
			where ul.Id = @userId and m.Id = mm.Id) =1 Then (Select ump.CanbeAdd from Menu m
			left join UserMenupermission ump on m.Id = ump.MenuId
			left join UserLogin ul on ump.userId= ul.Id
			where ul.Id = @userId and m.Id = mm.Id)
			else 
			(Select rmp.CanbeAdd
			from Menu m
			left join RoleMenuPermission rmp on m.Id = rmp.MenuId
			left join Role r on r.Id = rmp.RoleId
			left join GamePlayer gp on gp.RoleId = r.Id
			left join UserLogin ul on ul.Id= gp.UserId 
			and ul.Id = gp.UserId
			where ul.Id = @userId and gp.GameId =@gameId and m.Id = mm.Id)
			End as CanbeAdd,

			Case When 
			(Select ump.CanbeGive from Menu m
			left join UserMenupermission ump on m.Id = ump.MenuId
			left join UserLogin ul on ump.userId= ul.Id
			where ul.Id = @userId and m.Id = mm.Id) =1 Then (Select ump.CanbeGive from Menu m
			left join UserMenupermission ump on m.Id = ump.MenuId
			left join UserLogin ul on ump.userId= ul.Id
			where ul.Id = @userId and m.Id = mm.Id)
			else 
			(Select rmp.CanbeGive
			from Menu m
			left join RoleMenuPermission rmp on m.Id = rmp.MenuId
			left join Role r on r.Id = rmp.RoleId
			left join GamePlayer gp on gp.RoleId = r.Id
			left join UserLogin ul on ul.Id= gp.UserId 
			and ul.Id = gp.UserId
			where ul.Id = @userId and gp.GameId =@gameId and m.Id = mm.Id) 
			End as CanbeGive,

			Case When 
			(Select ump.CanbeDelete from Menu m
			left join UserMenupermission ump on m.Id = ump.MenuId
			left join UserLogin ul on ump.userId= ul.Id
			where ul.Id = @userId and m.Id = mm.Id) =1 Then (Select ump.CanbeDelete from Menu m
			left join UserMenupermission ump on m.Id = ump.MenuId
			left join UserLogin ul on ump.userId= ul.Id
			where ul.Id = @userId and m.Id = mm.Id)
			else 
			(Select rmp.CanbeDelete
			from Menu m
			left join RoleMenuPermission rmp on m.Id = rmp.MenuId
			left join Role r on r.Id = rmp.RoleId
			left join GamePlayer gp on gp.RoleId = r.Id
			left join UserLogin ul on ul.Id= gp.UserId 
			and ul.Id = gp.UserId
			where ul.Id = @userId and gp.GameId =@gameId and m.Id = mm.Id)
			End as CanbeDelete
			from  Menu mm ) m
			Where IsActive = 1 and CanbeView = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetPagePermission]    Script Date: 4/27/2021 4:38:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetPagePermission]
	-- Add the parameters for the stored procedure here
	@userId int,
	@roleId int,
	@controllerName varchar(250),
	@actionName varchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @menuId int
    -- Insert statements for procedure here
	Select @menuId = Id from Menu where Controller = @controllerName and Action = @actionName

	Select * from ( Select
	@menuId as Id,
	Case 
	When (Select CanbeAdd from UserMenuPermission where MenuId = m.Id and UserId = @userId) = 1 Then CAST(1 as bit) 
	When (Select CanbeAdd from RoleMenuPermission where MenuId = m.Id and RoleId = @roleId)  = 1 Then CAST(1 as bit) 
	else CAST(0 as bit) 
	End as CanbeAdd,
	Case 
	When (Select CanbeEdit from UserMenuPermission where MenuId = m.Id and UserId = @userId) = 1 Then CAST(1 as bit) 
	When (Select CanbeEdit from RoleMenuPermission where MenuId = m.Id and RoleId = @roleId)   = 1 Then CAST(1 as bit) 
	else CAST(0 as bit) 
	End as CanBeEdit,
	Case 
	When (Select CanbeDelete from UserMenuPermission where MenuId = m.Id and UserId = @userId) = 1 Then CAST(1 as bit) 
	When (Select CanbeDelete from RoleMenuPermission where MenuId = m.Id and RoleId = @roleId)  = 1 Then CAST(1 as bit)  
	else CAST(0 as bit) 
	End as CanbeDelete,
	Case 
	When (Select CanbeView from UserMenuPermission where MenuId = m.Id and UserId = @userId) = 1 Then CAST(1 as bit) 
	When (Select CanbeView from RoleMenuPermission where MenuId = m.Id and RoleId = @roleId)   = 1 Then CAST(1 as bit) 
	else CAST(0 as bit) 
	End as CanbeView,
	Case 
	When (Select CanbeGive from UserMenuPermission where MenuId = m.Id and UserId = @userId) = 1 Then CAST(1 as bit) 
	When (Select CanbeGive from RoleMenuPermission where MenuId = m.Id and RoleId = @roleId)   = 1 Then CAST(1 as bit) 
	else CAST(0 as bit) 
	End as CanbeGive
	From Menu m Where Id = @menuId
	 ) menu
END
GO
