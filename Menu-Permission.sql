
CREATE TABLE [dbo].[Menu](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Name] [varchar](250) NULL,
	[Controller] [varchar](100) NOT NULL,
	[Action] [varchar](100) NOT NULL,	
	[IsMain] [bit] NOT NULL,
	[ParentId] [int] NULL,
	[Order] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL	
 )

 

 
 CREATE TABLE [dbo].[RoleMenuPermission](
	[Id] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[RoleId] INT NULL,
	[MenuId] INT NULL,
	[IsView] BIT NOT NULL,
    [IsList] BIT NOT NULL,
    [IsAdd] BIT NOT NULL,
    [IsDelete] BIT NOT NULL,
    [IsEdit] BIT NOT NULL,
	CreatedBy INT NOT NULL
 )

 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Settings','','',1,1,1,NULL,GETDATE(),GETDATE()) 
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Users','User','Index',0,1,1,1,GETDATE(),GETDATE())
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Game','Game','Index',0,1,2,1,GETDATE(),GETDATE())
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Team','Team','Index',0,1,3,1,GETDATE(),GETDATE())
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Look','Look','Index',0,1,4,1,GETDATE(),GETDATE())
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Session','session','Index',0,1,5,1,GETDATE(),GETDATE())
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Filter','Filter','Index',0,1,6,1,GETDATE(),GETDATE())
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Status','Status','Index',0,1,7,1,GETDATE(),GETDATE())
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Report','Report','Index',0,1,8,1,GETDATE(),GETDATE())
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Action Play','Play','Index',0,1,9,1,GETDATE(),GETDATE())
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Session Calendar','SessionSchedule','Index',0,1,10,1,GETDATE(),GETDATE())
 INSERT [dbo].[Menu] (Name, Controller,Action,IsMain,IsActive,[Order],ParentId,CreatedDate,UpdatedDate) VALUES ('Poll','Poll','Index',0,1,11,1,GETDATE(),GETDATE())

 