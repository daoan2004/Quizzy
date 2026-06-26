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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240524205121_InitialCreate'
)
BEGIN
    CREATE TABLE [Blogs] (
        [ID] bigint NOT NULL IDENTITY,
        [userID] bigint NOT NULL,
        [title] nvarchar(max) NOT NULL,
        [body] nvarchar(max) NOT NULL,
        [description] nvarchar(max) NOT NULL,
        [status] bit NOT NULL,
        [publishAt] datetime2 NOT NULL,
        [updatedAt] datetime2 NOT NULL,
        [blog_picture] nvarchar(max) NOT NULL,
        [link_media] nvarchar(max) NOT NULL,
        [url] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Blogs] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240524205121_InitialCreate'
)
BEGIN
    CREATE TABLE [Slider] (
        [ID] bigint NOT NULL IDENTITY,
        [Title] nvarchar(max) NOT NULL,
        [image] nvarchar(max) NOT NULL,
        [backlink] nvarchar(max) NOT NULL,
        [description] nvarchar(max) NOT NULL,
        [status] bit NOT NULL,
        CONSTRAINT [PK_Slider] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240524205121_InitialCreate'
)
BEGIN
    CREATE TABLE [Subjects] (
        [ID] bigint NOT NULL IDENTITY,
        [UserID] bigint NOT NULL,
        [title] nvarchar(max) NOT NULL,
        [brief_info] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [rate] int NOT NULL,
        [contacts_links] nvarchar(max) NOT NULL,
        [isHot] bit NOT NULL,
        CONSTRAINT [PK_Subjects] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240524205121_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240524205121_InitialCreate', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240525204206_AddedPricePackageModel'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240525204206_AddedPricePackageModel', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240525204729_newPricePackage'
)
BEGIN
    CREATE TABLE [PricePackages] (
        [ID] bigint NOT NULL IDENTITY,
        [SubjectID] bigint NOT NULL,
        [PackageType] int NOT NULL,
        [ListPrice] bigint NOT NULL,
        [SalePrice] bigint NOT NULL,
        CONSTRAINT [PK_PricePackages] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240525204729_newPricePackage'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240525204729_newPricePackage', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240525205142_mediaurlsubjects'
)
BEGIN
    ALTER TABLE [Subjects] ADD [media_url] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240525205142_mediaurlsubjects'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240525205142_mediaurlsubjects', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240525205752_hasnull'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'media_url');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Subjects] ALTER COLUMN [media_url] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240525205752_hasnull'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'isHot');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Subjects] ALTER COLUMN [isHot] bit NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240525205752_hasnull'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'contacts_links');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Subjects] ALTER COLUMN [contacts_links] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240525205752_hasnull'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'UserID');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Subjects] ALTER COLUMN [UserID] bigint NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240525205752_hasnull'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240525205752_hasnull', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240529205025_add-more'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'contacts_links');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Subjects] DROP COLUMN [contacts_links];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240529205025_add-more'
)
BEGIN
    EXEC sp_rename N'[Subjects].[media_url]', N'thumbnail_color', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240529205025_add-more'
)
BEGIN
    CREATE TABLE [Blogs_Category] (
        [BlogID] bigint NOT NULL,
        [CategoryID] bigint NOT NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240529205025_add-more'
)
BEGIN
    CREATE TABLE [Category] (
        [ID] bigint NOT NULL IDENTITY,
        [title] nvarchar(max) NOT NULL,
        [description] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Category] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240529205025_add-more'
)
BEGIN
    CREATE TABLE [Users] (
        [ID] bigint NOT NULL IDENTITY,
        [email] nvarchar(max) NOT NULL,
        [fullname] nvarchar(max) NOT NULL,
        [password] nvarchar(max) NOT NULL,
        [address] nvarchar(max) NULL,
        [Phone] nvarchar(max) NOT NULL,
        [gender] bit NOT NULL,
        [Dob] datetime2 NULL,
        [RoleID] bigint NULL,
        [profile_picture] nvarchar(max) NULL,
        [register_date] datetime2 NULL,
        [description] nvarchar(max) NULL,
        [status] int NULL,
        [verificationToken] nvarchar(max) NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240529205025_add-more'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240529205025_add-more', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [PricePackages] DROP CONSTRAINT [PK_PricePackages];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    EXEC sp_rename N'[PricePackages]', N'Price_package';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Dob');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Users] ALTER COLUMN [Dob] date NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [Users] ADD [PasswordResetToken] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [Users] ADD [PasswordResetTokenExpires] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [Subjects] ADD [registerDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [Slider] ADD [publishAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [Slider] ADD [updatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [Slider] ADD [userID] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Price_package]') AND [c].[name] = N'PackageType');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Price_package] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [Price_package] ALTER COLUMN [PackageType] bigint NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [Price_package] ADD CONSTRAINT [PK_Price_package] PRIMARY KEY ([ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE TABLE [PracticeLevel] (
        [ID] int NOT NULL IDENTITY,
        [title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_PracticeLevel] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE TABLE [QuizBank] (
        [ID] bigint NOT NULL IDENTITY,
        [SubjectID] bigint NOT NULL,
        [TopicID] int NOT NULL,
        [LevelID] int NOT NULL,
        [Status] bit NOT NULL,
        [GroupID] nvarchar(max) NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [QA] nvarchar(max) NOT NULL,
        [QB] nvarchar(max) NOT NULL,
        [QC] nvarchar(max) NOT NULL,
        [QD] nvarchar(max) NOT NULL,
        [QE] nvarchar(max) NOT NULL,
        [QF] nvarchar(max) NOT NULL,
        [Qcorrect] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_QuizBank] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE TABLE [Recipe] (
        [ID] bigint NOT NULL IDENTITY,
        [PricePackage_ID] bigint NOT NULL,
        [UserID] bigint NOT NULL,
        [SubjectID] bigint NOT NULL,
        [PricePackage_Type] bigint NOT NULL,
        [BuyAt] datetime2 NOT NULL,
        [EndAt] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Recipe] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Recipe_Price_package_PricePackage_ID] FOREIGN KEY ([PricePackage_ID]) REFERENCES [Price_package] ([ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Recipe_Subjects_SubjectID] FOREIGN KEY ([SubjectID]) REFERENCES [Subjects] ([ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Recipe_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE TABLE [Role] (
        [RoleID] bigint NOT NULL IDENTITY,
        [RoleName] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Role] PRIMARY KEY ([RoleID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE TABLE [Subject_Category] (
        [SubjectID] bigint NOT NULL,
        [CategoryID] bigint NOT NULL,
        CONSTRAINT [PK_Subject_Category] PRIMARY KEY ([SubjectID], [CategoryID]),
        CONSTRAINT [FK_Subject_Category_Category_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [Category] ([ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Subject_Category_Subjects_SubjectID] FOREIGN KEY ([SubjectID]) REFERENCES [Subjects] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE TABLE [SubjectTopic] (
        [id] int NOT NULL IDENTITY,
        [subjectId] bigint NOT NULL,
        [title] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_SubjectTopic] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE TABLE [SimulationExam] (
        [ID] bigint NOT NULL IDENTITY,
        [SubjectID] bigint NOT NULL,
        [LevelID] int NOT NULL,
        [ExamName] nvarchar(max) NOT NULL,
        [Number_Question] int NOT NULL,
        [Duration] int NOT NULL,
        [Passrate] decimal(5,2) NOT NULL,
        CONSTRAINT [PK_SimulationExam] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_SimulationExam_PracticeLevel_LevelID] FOREIGN KEY ([LevelID]) REFERENCES [PracticeLevel] ([ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_SimulationExam_Subjects_SubjectID] FOREIGN KEY ([SubjectID]) REFERENCES [Subjects] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE TABLE [QuizHandle] (
        [ID] bigint NOT NULL IDENTITY,
        [UserID] bigint NOT NULL,
        [PracticeID] bigint NOT NULL,
        [QuizID] bigint NOT NULL,
        [QAnswer] nvarchar(max) NOT NULL,
        [isMark] bit NOT NULL,
        [status] bit NOT NULL,
        [isCorrect] bit NOT NULL,
        CONSTRAINT [PK_QuizHandle] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_QuizHandle_QuizBank_QuizID] FOREIGN KEY ([QuizID]) REFERENCES [QuizBank] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE TABLE [Practice] (
        [ID] bigint NOT NULL IDENTITY,
        [UserID] bigint NOT NULL,
        [SubjectID] bigint NOT NULL,
        [title] nvarchar(max) NOT NULL,
        [taken_date] datetime2 NOT NULL,
        [duration] time NOT NULL,
        [number_quest] int NOT NULL,
        [number_correct] int NOT NULL,
        [levelID] int NOT NULL,
        [topicID] int NOT NULL,
        [time_taken] time NOT NULL,
        [Quest_group] nvarchar(max) NOT NULL,
        [Status] bit NOT NULL,
        CONSTRAINT [PK_Practice] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Practice_PracticeLevel_levelID] FOREIGN KEY ([levelID]) REFERENCES [PracticeLevel] ([ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Practice_SubjectTopic_topicID] FOREIGN KEY ([topicID]) REFERENCES [SubjectTopic] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Practice_Subjects_SubjectID] FOREIGN KEY ([SubjectID]) REFERENCES [Subjects] ([ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Practice_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Users_RoleID] ON [Users] ([RoleID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Slider_userID] ON [Slider] ([userID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Price_package_SubjectID] ON [Price_package] ([SubjectID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Practice_levelID] ON [Practice] ([levelID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Practice_SubjectID] ON [Practice] ([SubjectID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Practice_topicID] ON [Practice] ([topicID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Practice_UserID] ON [Practice] ([UserID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_QuizHandle_QuizID] ON [QuizHandle] ([QuizID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Recipe_PricePackage_ID] ON [Recipe] ([PricePackage_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Recipe_SubjectID] ON [Recipe] ([SubjectID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Recipe_UserID] ON [Recipe] ([UserID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_SimulationExam_LevelID] ON [SimulationExam] ([LevelID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_SimulationExam_SubjectID] ON [SimulationExam] ([SubjectID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    CREATE INDEX [IX_Subject_Category_CategoryID] ON [Subject_Category] ([CategoryID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [Price_package] ADD CONSTRAINT [FK_Price_package_Subjects_SubjectID] FOREIGN KEY ([SubjectID]) REFERENCES [Subjects] ([ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [Slider] ADD CONSTRAINT [FK_Slider_Users_userID] FOREIGN KEY ([userID]) REFERENCES [Users] ([ID]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Role_RoleID] FOREIGN KEY ([RoleID]) REFERENCES [Role] ([RoleID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055407_RebuildCurrentSchemaFromModels'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260626055407_RebuildCurrentSchemaFromModels', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055632_SeedLookupData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Description', N'title') AND [object_id] = OBJECT_ID(N'[PracticeLevel]'))
        SET IDENTITY_INSERT [PracticeLevel] ON;
    EXEC(N'INSERT INTO [PracticeLevel] ([ID], [Description], [title])
    VALUES (1, N''Easy'', N''Easy''),
    (2, N''Medium'', N''Medium''),
    (3, N''Hard'', N''Hard'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Description', N'title') AND [object_id] = OBJECT_ID(N'[PracticeLevel]'))
        SET IDENTITY_INSERT [PracticeLevel] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055632_SeedLookupData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleID', N'RoleName') AND [object_id] = OBJECT_ID(N'[Role]'))
        SET IDENTITY_INSERT [Role] ON;
    EXEC(N'INSERT INTO [Role] ([RoleID], [RoleName])
    VALUES (CAST(1 AS bigint), N''Admin''),
    (CAST(2 AS bigint), N''Customer''),
    (CAST(3 AS bigint), N''Marketing''),
    (CAST(4 AS bigint), N''Sale''),
    (CAST(5 AS bigint), N''Expert''),
    (CAST(6 AS bigint), N''Guest'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleID', N'RoleName') AND [object_id] = OBJECT_ID(N'[Role]'))
        SET IDENTITY_INSERT [Role] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055632_SeedLookupData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'subjectId', N'title') AND [object_id] = OBJECT_ID(N'[SubjectTopic]'))
        SET IDENTITY_INSERT [SubjectTopic] ON;
    EXEC(N'INSERT INTO [SubjectTopic] ([id], [subjectId], [title])
    VALUES (1, CAST(0 AS bigint), N''General'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'subjectId', N'title') AND [object_id] = OBJECT_ID(N'[SubjectTopic]'))
        SET IDENTITY_INSERT [SubjectTopic] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626055632_SeedLookupData'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260626055632_SeedLookupData', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626060029_SeedSampleData'
)
BEGIN
    SET IDENTITY_INSERT [Users] ON;
    INSERT INTO [Users] ([ID], [email], [fullname], [password], [address], [Phone], [gender], [Dob], [RoleID], [profile_picture], [register_date], [description], [status], [verificationToken], [PasswordResetToken], [PasswordResetTokenExpires]) VALUES
    (10001, N'admin@quizzy.local', N'Admin Quizzy', N'0e7517141fb53f21ee439b355b5a1d0a', N'Ho Chi Minh City', N'0900000001', 1, '1995-01-01', 1, NULL, '2026-06-01T08:00:00', N'Sample admin account', 1, NULL, NULL, NULL),
    (10002, N'marketing@quizzy.local', N'Marketing Quizzy', N'fec598679576abc64f49c35b72a0368f', N'Ho Chi Minh City', N'0900000002', 0, '1996-02-02', 3, NULL, '2026-06-02T08:00:00', N'Sample marketing account', 1, NULL, NULL, NULL),
    (10003, N'customer@quizzy.local', N'Customer Quizzy', N'681ae46305e29b966801a96331ae607d', N'Ha Noi', N'0900000003', 1, '2000-03-03', 2, NULL, '2026-06-03T08:00:00', N'Sample customer account', 1, NULL, NULL, NULL);
    SET IDENTITY_INSERT [Users] OFF;

    SET IDENTITY_INSERT [Category] ON;
    INSERT INTO [Category] ([ID], [title], [description]) VALUES
    (101, N'Programming', N'Programming and software engineering'),
    (102, N'Database', N'Database design and SQL practice'),
    (103, N'Web Development', N'Frontend and backend web development'),
    (104, N'Computer Science', N'Computer science foundations');
    SET IDENTITY_INSERT [Category] OFF;

    SET IDENTITY_INSERT [Subjects] ON;
    INSERT INTO [Subjects] ([ID], [UserID], [title], [brief_info], [Description], [rate], [isHot], [thumbnail_color], [registerDate]) VALUES
    (201, 10002, N'C# Fundamentals', N'Learn C# syntax, OOP, LINQ, and practical problem solving.', N'A beginner friendly C# course with quizzes for syntax, classes, collections, LINQ, and exception handling.', 5, 1, N'subject.jpg', '2026-06-10T09:00:00'),
    (202, 10002, N'ASP.NET Core MVC', N'Build web apps with controllers, views, routing, and EF Core.', N'Hands-on ASP.NET Core MVC subject covering routing, Razor views, model binding, validation, authentication basics, and EF Core.', 5, 1, N'subject.jpg', '2026-06-11T09:00:00'),
    (203, 10002, N'SQL Server Essentials', N'Practice relational design, joins, indexes, and transactions.', N'SQL Server training for database fundamentals, querying, normalization, joins, stored procedures, and query performance.', 4, 1, N'subject.jpg', '2026-06-12T09:00:00');
    SET IDENTITY_INSERT [Subjects] OFF;

    SET IDENTITY_INSERT [Price_package] ON;
    INSERT INTO [Price_package] ([ID], [SubjectID], [PackageType], [ListPrice], [SalePrice]) VALUES
    (301, 201, 1, 59, 39), (302, 201, 2, 99, 69), (303, 201, 3, 159, 109),
    (304, 202, 1, 69, 49), (305, 202, 2, 119, 79), (306, 202, 3, 189, 129),
    (307, 203, 1, 49, 29), (308, 203, 2, 89, 59), (309, 203, 3, 139, 99);
    SET IDENTITY_INSERT [Price_package] OFF;

    INSERT INTO [Subject_Category] ([SubjectID], [CategoryID]) VALUES
    (201, 101), (201, 104), (202, 101), (202, 103), (203, 102), (203, 104);

    SET IDENTITY_INSERT [Slider] ON;
    INSERT INTO [Slider] ([ID], [userID], [Title], [image], [backlink], [description], [status], [publishAt], [updatedAt]) VALUES
    (401, 10002, N'C# Fundamentals', N'image1.jpg', N'/Subjects/Details/201', N'Start learning C# with guided quizzes.', 1, '2026-06-13T09:00:00', '2026-06-13T09:00:00'),
    (402, 10002, N'ASP.NET Core MVC', N'image2.jpg', N'/Subjects/Details/202', N'Practice MVC and EF Core through real questions.', 1, '2026-06-14T09:00:00', '2026-06-14T09:00:00'),
    (403, 10002, N'SQL Server Essentials', N'image3.jpg', N'/Subjects/Details/203', N'Build confidence in SQL queries and schema design.', 1, '2026-06-15T09:00:00', '2026-06-15T09:00:00');
    SET IDENTITY_INSERT [Slider] OFF;

    SET IDENTITY_INSERT [Blogs] ON;
    INSERT INTO [Blogs] ([ID], [userID], [title], [body], [description], [status], [publishAt], [updatedAt], [blog_picture], [link_media], [url]) VALUES
    (501, 10002, N'How to Prepare for Technical Quizzes', N'Plan your study time, review fundamentals, and practice small sets of questions every day.', N'A practical guide for quiz preparation.', 1, '2026-06-16T09:00:00', '2026-06-16T09:00:00', N'blog-web.jpg', N'', N'/Blogs/BlogsDetail?blogid=501&userid=10002'),
    (502, 10002, N'Why Practice History Matters', N'Looking back at previous attempts helps you find weak topics and improve faster.', N'Use practice history to learn smarter.', 1, '2026-06-17T09:00:00', '2026-06-17T09:00:00', N'OpenSlide.jpg', N'', N'/Blogs/BlogsDetail?blogid=502&userid=10002'),
    (503, 10002, N'Choosing the Right Subject Package', N'Short packages are good for refreshers; longer packages fit deep learning plans.', N'Compare packages before registering.', 1, '2026-06-18T09:00:00', '2026-06-18T09:00:00', N'FSlide.png', N'', N'/Blogs/BlogsDetail?blogid=503&userid=10002');
    SET IDENTITY_INSERT [Blogs] OFF;

    INSERT INTO [Blogs_Category] ([BlogID], [CategoryID]) VALUES
    (501, 101), (501, 104), (502, 104), (503, 103);

    SET IDENTITY_INSERT [SimulationExam] ON;
    INSERT INTO [SimulationExam] ([ID], [SubjectID], [LevelID], [ExamName], [Number_Question], [Duration], [Passrate]) VALUES
    (601, 201, 1, N'C# Entry Simulation Exam', 10, 20, 60.00),
    (602, 201, 2, N'C# Intermediate Simulation Exam', 15, 30, 70.00),
    (603, 202, 1, N'MVC Entry Simulation Exam', 10, 20, 60.00),
    (604, 202, 2, N'MVC Intermediate Simulation Exam', 15, 30, 70.00),
    (605, 203, 1, N'SQL Entry Simulation Exam', 10, 20, 60.00),
    (606, 203, 2, N'SQL Intermediate Simulation Exam', 15, 30, 70.00);
    SET IDENTITY_INSERT [SimulationExam] OFF;

    SET IDENTITY_INSERT [QuizBank] ON;
    INSERT INTO [QuizBank] ([ID], [SubjectID], [TopicID], [LevelID], [Status], [GroupID], [Title], [QA], [QB], [QC], [QD], [QE], [QF], [Qcorrect]) VALUES
    (1001, 201, 1, 1, 1, N'Choose one', N'Which keyword declares a variable whose type is inferred?', N'var', N'let', N'dim', N'auto', N'', N'', N'A'),
    (1002, 201, 1, 1, 1, N'Choose one', N'Which type stores true or false values in C#?', N'int', N'bool', N'string', N'decimal', N'', N'', N'B'),
    (1003, 201, 1, 1, 1, N'Choose one', N'Which symbol starts a single-line comment in C#?', N'#', N'--', N'//', N'/*', N'', N'', N'C'),
    (1004, 201, 1, 2, 1, N'Choose one', N'Which feature lets a class provide multiple methods with the same name but different parameters?', N'Inheritance', N'Encapsulation', N'Overloading', N'Boxing', N'', N'', N'C'),
    (1005, 201, 1, 2, 1, N'Choose one', N'Which LINQ method filters a sequence?', N'Select', N'Where', N'OrderBy', N'GroupBy', N'', N'', N'B'),
    (1006, 201, 1, 2, 1, N'Choose one', N'Which block always runs after try/catch if present?', N'using', N'final', N'finally', N'lock', N'', N'', N'C'),
    (1007, 201, 1, 3, 1, N'Choose one', N'Which construct releases IDisposable resources automatically?', N'using', N'await', N'yield', N'params', N'', N'', N'A'),
    (1008, 201, 1, 3, 1, N'Choose one', N'Which modifier prevents a class from being inherited?', N'private', N'sealed', N'static', N'abstract', N'', N'', N'B'),
    (1009, 202, 1, 1, 1, N'Choose one', N'In MVC, which component receives HTTP requests first?', N'Model', N'View', N'Controller', N'Migration', N'', N'', N'C'),
    (1010, 202, 1, 1, 1, N'Choose one', N'Razor view files usually use which extension?', N'.html', N'.cshtml', N'.razorclass', N'.mvc', N'', N'', N'B'),
    (1011, 202, 1, 1, 1, N'Choose one', N'Which method registers MVC controllers and views?', N'AddControllersWithViews', N'UseStaticFiles', N'AddDbContext', N'UseRouting', N'', N'', N'A'),
    (1012, 202, 1, 2, 1, N'Choose one', N'Which EF Core method loads related data eagerly?', N'Include', N'Attach', N'SaveChanges', N'Migrate', N'', N'', N'A'),
    (1013, 202, 1, 2, 1, N'Choose one', N'Which file commonly stores connection strings in ASP.NET Core?', N'launch.json', N'appsettings.json', N'package.json', N'web.config only', N'', N'', N'B'),
    (1014, 202, 1, 2, 1, N'Choose one', N'Which middleware enables authentication?', N'UseAuthentication', N'UseAuthorizationOnly', N'UseEndpoints', N'UseMvcData', N'', N'', N'A'),
    (1015, 202, 1, 3, 1, N'Choose one', N'Which attribute restricts access to authenticated users?', N'ValidateAntiForgeryToken', N'Authorize', N'Bind', N'RouteOnly', N'', N'', N'B'),
    (1016, 202, 1, 3, 1, N'Choose one', N'Which service configures EF Core SQL Server?', N'UseSqlServer', N'UseKestrelOnly', N'AddRazorRuntime', N'UseSqlClientView', N'', N'', N'A'),
    (1017, 203, 1, 1, 1, N'Choose one', N'Which SQL clause filters rows?', N'ORDER BY', N'WHERE', N'GROUP BY', N'JOIN', N'', N'', N'B'),
    (1018, 203, 1, 1, 1, N'Choose one', N'Which key uniquely identifies a row?', N'Foreign key', N'Primary key', N'Index hint', N'Check key', N'', N'', N'B'),
    (1019, 203, 1, 1, 1, N'Choose one', N'Which command reads data from a table?', N'SELECT', N'UPDATE', N'DELETE', N'ALTER', N'', N'', N'A'),
    (1020, 203, 1, 2, 1, N'Choose one', N'Which join returns matching rows from both tables?', N'LEFT JOIN', N'RIGHT JOIN', N'INNER JOIN', N'CROSS JOIN', N'', N'', N'C'),
    (1021, 203, 1, 2, 1, N'Choose one', N'Which aggregate counts rows?', N'SUM', N'AVG', N'COUNT', N'MIN', N'', N'', N'C'),
    (1022, 203, 1, 2, 1, N'Choose one', N'Which normal form reduces repeating groups?', N'1NF', N'2NF', N'3NF', N'BCNF only', N'', N'', N'A'),
    (1023, 203, 1, 3, 1, N'Choose one', N'Which object can improve read performance but may slow writes?', N'Index', N'View only', N'Cursor', N'Trigger only', N'', N'', N'A'),
    (1024, 203, 1, 3, 1, N'Choose one', N'Which isolation issue allows reading uncommitted data?', N'Dirty read', N'Deadlock', N'Rollback', N'Checkpoint', N'', N'', N'A');
    SET IDENTITY_INSERT [QuizBank] OFF;

    SET IDENTITY_INSERT [Recipe] ON;
    INSERT INTO [Recipe] ([ID], [PricePackage_ID], [UserID], [SubjectID], [PricePackage_Type], [BuyAt], [EndAt], [Status]) VALUES
    (701, 301, 10003, 201, 1, '2026-06-20T09:00:00', '2026-09-20T09:00:00', N'Registrated'),
    (702, 304, 10003, 202, 1, '2026-06-21T09:00:00', '2026-09-21T09:00:00', N'Registrated'),
    (703, 307, 10003, 203, 1, '2026-06-22T09:00:00', '2026-09-22T09:00:00', N'Submitted');
    SET IDENTITY_INSERT [Recipe] OFF;

    SET IDENTITY_INSERT [Practice] ON;
    INSERT INTO [Practice] ([ID], [UserID], [SubjectID], [title], [taken_date], [duration], [number_quest], [number_correct], [levelID], [topicID], [time_taken], [Quest_group], [Status]) VALUES
    (801, 10003, 201, N'C# Warm-up Practice', '2026-06-23T10:00:00', '00:20:00', 5, 3, 1, 1, '00:12:30', N'Choose one', 1),
    (802, 10003, 202, N'MVC Routing Practice', '2026-06-24T10:00:00', '00:25:00', 5, 4, 2, 1, '00:15:10', N'Choose one', 1);
    SET IDENTITY_INSERT [Practice] OFF;

    SET IDENTITY_INSERT [QuizHandle] ON;
    INSERT INTO [QuizHandle] ([ID], [UserID], [PracticeID], [QuizID], [QAnswer], [isMark], [status], [isCorrect]) VALUES
    (901, 10003, 801, 1001, N'A', 0, 1, 1), (902, 10003, 801, 1002, N'B', 0, 1, 1),
    (903, 10003, 801, 1003, N'A', 0, 1, 0), (904, 10003, 801, 1004, N'C', 0, 1, 1),
    (905, 10003, 801, 1005, N'A', 0, 1, 0), (906, 10003, 802, 1009, N'C', 0, 1, 1),
    (907, 10003, 802, 1010, N'B', 0, 1, 1), (908, 10003, 802, 1011, N'A', 0, 1, 1),
    (909, 10003, 802, 1012, N'A', 0, 1, 1), (910, 10003, 802, 1013, N'C', 0, 1, 0);
    SET IDENTITY_INSERT [QuizHandle] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626060029_SeedSampleData'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260626060029_SeedSampleData', N'8.0.5');
END;
GO

COMMIT;
GO

