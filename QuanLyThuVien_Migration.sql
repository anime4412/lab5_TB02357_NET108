IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Image] nvarchar(255) NULL,
    [Active] bit NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeCode] nvarchar(20) NOT NULL,
    [FullName] nvarchar(150) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Password] nvarchar(255) NOT NULL,
    [Phone] nvarchar(15) NULL,
    [Role] nvarchar(50) NOT NULL,
    [Active] bit NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Books] (
    [Id] int NOT NULL IDENTITY,
    [ISBN] nvarchar(20) NOT NULL,
    [Title] nvarchar(250) NOT NULL,
    [CategoryId] int NULL,
    [Author] nvarchar(150) NULL,
    [PublishYear] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    [AvailableQuantity] int NOT NULL,
    [CoverImage] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [Active] bit NOT NULL,
    CONSTRAINT [PK_Books] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Books_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'Description', N'Image', N'Name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([Id], [Active], [Description], [Image], [Name])
VALUES (1, CAST(1 AS bit), N'Sách chuyên ngành Lập trình C#, ASP.NET Core, Cơ sở dữ liệu', NULL, N'Công nghệ thông tin'),
(2, CAST(1 AS bit), N'Sách Quản trị kinh doanh, Tài chính doanh nghiệp', NULL, N'Kinh tế & Khởi nghiệp'),
(3, CAST(1 AS bit), N'Tiểu thuyết, truyện ngắn, tác phẩm kinh điển', NULL, N'Văn học & Nghệ thuật'),
(4, CAST(1 AS bit), N'Phát triển bản thân, tư duy lãnh đạo, giao tiếp', NULL, N'Kỹ năng sống'),
(5, CAST(1 AS bit), N'Toán học, Vật lý, Thiên văn học', NULL, N'Khoa học cơ bản');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'Description', N'Image', N'Name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'Email', N'EmployeeCode', N'FullName', N'Password', N'Phone', N'Role') AND [object_id] = OBJECT_ID(N'[Employees]'))
    SET IDENTITY_INSERT [Employees] ON;
INSERT INTO [Employees] ([Id], [Active], [Email], [EmployeeCode], [FullName], [Password], [Phone], [Role])
VALUES (1, CAST(1 AS bit), N'admin@thuvien.edu.vn', N'NV_ADMIN', N'Nguyễn Quản Trị (Admin)', N'Admin@123', N'0911223344', N'Admin'),
(2, CAST(1 AS bit), N'thuthu@thuvien.edu.vn', N'NV_THUTHU', N'Trần Thị Thủ Thư (Staff)', N'Thuthu@123', N'0988776655', N'Librarian');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'Email', N'EmployeeCode', N'FullName', N'Password', N'Phone', N'Role') AND [object_id] = OBJECT_ID(N'[Employees]'))
    SET IDENTITY_INSERT [Employees] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'Author', N'AvailableQuantity', N'CategoryId', N'CoverImage', N'Description', N'ISBN', N'Price', N'PublishYear', N'Quantity', N'Title') AND [object_id] = OBJECT_ID(N'[Books]'))
    SET IDENTITY_INSERT [Books] ON;
INSERT INTO [Books] ([Id], [Active], [Author], [AvailableQuantity], [CategoryId], [CoverImage], [Description], [ISBN], [Price], [PublishYear], [Quantity], [Title])
VALUES (1, CAST(1 AS bit), N'Robert C. Martin', 8, 1, NULL, N'Sách gối đầu giường của mọi lập trình viên chuyên nghiệp.', N'9780132350884', 250000.0, 2021, 10, N'Clean Code: A Handbook of Agile Software Craftsmanship'),
(2, CAST(1 AS bit), N'Andrew Hunt & David Thomas', 7, 1, NULL, N'Hành trình từ lập trình viên nghiệp dư trở thành bậc thầy phát triển phần mềm.', N'9780201616224', 280000.0, 2022, 8, N'The Pragmatic Programmer'),
(3, CAST(1 AS bit), N'Dale Carnegie', 14, 4, NULL, N'Nghệ thuật thu phục lòng người kinh điển.', N'9786045882885', 95000.0, 2023, 15, N'Đắc Nhân Tâm'),
(4, CAST(1 AS bit), N'Nguyễn Nhật Ánh', 11, 3, NULL, N'Tác phẩm văn học lãng mạn đầy cảm xúc.', N'9786042171120', 110000.0, 2020, 12, N'Mắt Biếc'),
(5, CAST(1 AS bit), N'Yuval Noah Harari', 10, 5, NULL, N'Khám phá toàn cảnh lịch sử tiến hóa và văn minh nhân loại.', N'9780062316097', 185000.0, 2022, 10, N'Sapiens: Lược Sử Loài Người');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'Author', N'AvailableQuantity', N'CategoryId', N'CoverImage', N'Description', N'ISBN', N'Price', N'PublishYear', N'Quantity', N'Title') AND [object_id] = OBJECT_ID(N'[Books]'))
    SET IDENTITY_INSERT [Books] OFF;
GO

CREATE INDEX [IX_Books_CategoryId] ON [Books] ([CategoryId]);
GO

CREATE UNIQUE INDEX [IX_Books_ISBN] ON [Books] ([ISBN]);
GO

CREATE UNIQUE INDEX [IX_Employees_Email] ON [Employees] ([Email]);
GO

CREATE UNIQUE INDEX [IX_Employees_EmployeeCode] ON [Employees] ([EmployeeCode]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261118141947_InitialCreate', N'8.0.11');
GO

COMMIT;
GO

