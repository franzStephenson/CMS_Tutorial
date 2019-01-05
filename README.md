# CMS_Tutorial
my cms tutorial course
Admin user admin password1
#Tables

CREATE TABLE [dbo].[tblCategories] (
    [Id]      INT          IDENTITY (1, 1) NOT NULL,
    [Name]    VARCHAR (50) NULL,
    [Sorting] INT          NULL,
    [Slug]    VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[tblOrderDetails] (
    [Id]        INT IDENTITY (1, 1) NOT NULL,
    [OrderId]   INT NULL,
    [UserId]    INT NULL,
    [ProductId] INT NULL,
    [Quantity]  INT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[tblOrders] (
    [OrderId]   INT           IDENTITY (1, 1) NOT NULL,
    [UserId]    INT           NULL,
    [CreatedAt] DATETIME2 (7) NULL,
    PRIMARY KEY CLUSTERED ([OrderId] ASC)
);
CREATE TABLE [dbo].[tblPages] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [Title]      VARCHAR (50)  NULL,
    [Slug]       VARCHAR (50)  NULL,
    [Body]       VARCHAR (MAX) NULL,
    [Sorting]    INT           NULL,
    [HasSideBar] BIT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[tblProducts] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)    NULL,
    [Slug]         VARCHAR (50)    NULL,
    [Description]  VARCHAR (MAX)   NULL,
    [Price]        NUMERIC (18, 2) NULL,
    [CategoryName] VARCHAR (50)    NULL,
    [CategoryID]   INT             NULL,
    [ImageName]    VARCHAR (100)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[tblRoles] (
    [Id]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[tblSideBar] (
    [Id]   INT           NOT NULL,
    [Body] VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[tblUserRoles] (
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([RoleId] ASC, [UserId] ASC)
);
CREATE TABLE [dbo].[tblUsers] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [FirstName]    VARCHAR (50) NULL,
    [LastName]     VARCHAR (50) NULL,
    [EmailAddress] VARCHAR (50) NULL,
    [Username]     VARCHAR (50) NULL,
    [Password]     VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
