DECLARE @CurrentMigration [nvarchar](max)

IF object_id('[dbo].[__MigrationHistory]') IS NOT NULL
    SELECT @CurrentMigration =
        (SELECT TOP (1) 
        [Project1].[MigrationId] AS [MigrationId]
        FROM ( SELECT 
        [Extent1].[MigrationId] AS [MigrationId]
        FROM [dbo].[__MigrationHistory] AS [Extent1]
        WHERE [Extent1].[ContextKey] = N'Ecat.Data.Contexts.EcatContext+EcatCtxConfig'
        )  AS [Project1]
        ORDER BY [Project1].[MigrationId] DESC)

IF @CurrentMigration IS NULL
    SET @CurrentMigration = '0'

IF @CurrentMigration < '201708311912215_ECAT2_Init'
BEGIN
    CREATE TABLE [dbo].[CogEcmspeResult] (
        [PersonId] [int] NOT NULL,
        [InstrumentId] [int] NOT NULL,
        [Attempt] [int] NOT NULL,
        [Accommodate] [float] NOT NULL,
        [Avoid] [float] NOT NULL,
        [Collaborate] [float] NOT NULL,
        [Compete] [float] NOT NULL,
        [Compromise] [float] NOT NULL,
        CONSTRAINT [PK_dbo.CogEcmspeResult] PRIMARY KEY ([PersonId], [InstrumentId], [Attempt])
    )
    CREATE INDEX [IX_PersonId] ON [dbo].[CogEcmspeResult]([PersonId])
    CREATE INDEX [IX_InstrumentId] ON [dbo].[CogEcmspeResult]([InstrumentId])
    CREATE TABLE [dbo].[CogInstrument] (
        [Id] [int] NOT NULL IDENTITY,
        [Version] [nvarchar](50),
        [IsActive] [bit] NOT NULL,
        [CogInstructions] [nvarchar](max),
        [CogInstrumentType] [nvarchar](50),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.CogInstrument] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[CogInventory] (
        [Id] [int] NOT NULL IDENTITY,
        [InstrumentId] [int] NOT NULL,
        [DisplayOrder] [int] NOT NULL,
        [IsScored] [bit] NOT NULL,
        [IsDisplayed] [bit] NOT NULL,
        [AdaptiveDescription] [nvarchar](max),
        [InnovativeDescription] [nvarchar](max),
        [ItemType] [nvarchar](50),
        [ItemDescription] [nvarchar](max),
        [IsReversed] [bit],
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.CogInventory] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_InstrumentId] ON [dbo].[CogInventory]([InstrumentId])
    CREATE TABLE [dbo].[Person] (
        [PersonId] [int] NOT NULL IDENTITY,
        [IsActive] [bit] NOT NULL,
        [BbUserId] [nvarchar](50),
        [BbUserName] [nvarchar](50),
        [LastName] [nvarchar](50) NOT NULL,
        [FirstName] [nvarchar](50) NOT NULL,
        [AvatarLocation] [nvarchar](50),
        [GoByName] [nvarchar](50),
        [Gender] [nvarchar](50) NOT NULL,
        [Affiliation] [nvarchar](50) NOT NULL,
        [Paygrade] [nvarchar](50) NOT NULL,
        [Component] [nvarchar](50) NOT NULL,
        [Email] [nvarchar](80) NOT NULL,
        [RegistrationComplete] [bit] NOT NULL,
        [InstituteRole] [nvarchar](50) NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.Person] PRIMARY KEY ([PersonId])
    )
    CREATE UNIQUE INDEX [IX_UniqueEmailAddress] ON [dbo].[Person]([Email])
    CREATE TABLE [dbo].[ProfileFaculty] (
        [PersonId] [int] NOT NULL,
        [Bio] [nvarchar](max),
        [HomeStation] [nvarchar](50),
        [IsCourseAdmin] [bit] NOT NULL,
        [IsReportViewer] [bit] NOT NULL,
        [AcademyId] [nvarchar](50),
        CONSTRAINT [PK_dbo.ProfileFaculty] PRIMARY KEY ([PersonId])
    )
    CREATE INDEX [IX_PersonId] ON [dbo].[ProfileFaculty]([PersonId])
    CREATE TABLE [dbo].[FacultyInCourse] (
        [FacultyPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [BbCourseMemId] [nvarchar](50),
        [IsDeleted] [bit] NOT NULL,
        [DeletedById] [int],
        [DeletedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.FacultyInCourse] PRIMARY KEY ([FacultyPersonId], [CourseId])
    )
    CREATE INDEX [IX_FacultyPersonId] ON [dbo].[FacultyInCourse]([FacultyPersonId])
    CREATE INDEX [IX_CourseId] ON [dbo].[FacultyInCourse]([CourseId])
    CREATE TABLE [dbo].[Course] (
        [Id] [int] NOT NULL IDENTITY,
        [AcademyId] [nvarchar](50),
        [BbCourseId] [nvarchar](60),
        [Name] [nvarchar](50),
        [ClassNumber] [nvarchar](50),
        [Term] [nvarchar](50),
        [GradReportPublished] [bit] NOT NULL,
        [StartDate] [datetime2](7) NOT NULL,
        [GradDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.Course] PRIMARY KEY ([Id])
    )
    CREATE UNIQUE INDEX [IX_UniqueBbCourseId] ON [dbo].[Course]([BbCourseId])
    CREATE TABLE [dbo].[SpResponse] (
        [AssessorPersonId] [int] NOT NULL,
        [AssesseePersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [InventoryItemId] [int] NOT NULL,
        [ItemResponse] [nvarchar](50),
        [ItemModelScore] [int] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.SpResponse] PRIMARY KEY ([AssessorPersonId], [AssesseePersonId], [CourseId], [WorkGroupId], [InventoryItemId])
    )
    CREATE INDEX [IX_AssessorPersonId_CourseId_WorkGroupId] ON [dbo].[SpResponse]([AssessorPersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[SpResponse]([AssesseePersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_InventoryItemId] ON [dbo].[SpResponse]([InventoryItemId])
    CREATE TABLE [dbo].[CrseStudentInGroup] (
        [StudentId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [HasAcknowledged] [bit] NOT NULL,
        [BbCrseStudGroupId] [nvarchar](50),
        [IsDeleted] [bit] NOT NULL,
        [DeletedById] [int],
        [DeletedDate] [datetime2](7),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.CrseStudentInGroup] PRIMARY KEY ([StudentId], [CourseId], [WorkGroupId])
    )
    CREATE INDEX [IX_StudentId_CourseId] ON [dbo].[CrseStudentInGroup]([StudentId], [CourseId])
    CREATE INDEX [IX_CourseId] ON [dbo].[CrseStudentInGroup]([CourseId])
    CREATE INDEX [IX_WorkGroupId] ON [dbo].[CrseStudentInGroup]([WorkGroupId])
    CREATE TABLE [dbo].[StratResponse] (
        [AssessorPersonId] [int] NOT NULL,
        [AssesseePersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [StratPosition] [int] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.StratResponse] PRIMARY KEY ([AssessorPersonId], [AssesseePersonId], [CourseId], [WorkGroupId])
    )
    CREATE INDEX [IX_AssessorPersonId_CourseId_WorkGroupId] ON [dbo].[StratResponse]([AssessorPersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[StratResponse]([AssesseePersonId], [CourseId], [WorkGroupId])
    CREATE TABLE [dbo].[StudSpComment] (
        [AuthorPersonId] [int] NOT NULL,
        [RecipientPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [FacultyPersonId] [int],
        [RequestAnonymity] [bit] NOT NULL,
        [CommentText] [nvarchar](max),
        [CreatedDate] [datetime2](7) NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.StudSpComment] PRIMARY KEY ([AuthorPersonId], [RecipientPersonId], [CourseId], [WorkGroupId])
    )
    CREATE INDEX [IX_AuthorPersonId_CourseId_WorkGroupId] ON [dbo].[StudSpComment]([AuthorPersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[StudSpComment]([RecipientPersonId], [CourseId], [WorkGroupId])
    CREATE TABLE [dbo].[StudSpCommentFlag] (
        [AuthorPersonId] [int] NOT NULL,
        [RecipientPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [Author] [nvarchar](50),
        [Recipient] [nvarchar](50),
        [Faculty] [nvarchar](50),
        [FlaggedByFacultyId] [int],
        CONSTRAINT [PK_dbo.StudSpCommentFlag] PRIMARY KEY ([AuthorPersonId], [RecipientPersonId], [CourseId], [WorkGroupId])
    )
    CREATE INDEX [IX_AuthorPersonId_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[StudSpCommentFlag]([AuthorPersonId], [RecipientPersonId], [CourseId], [WorkGroupId])
    CREATE TABLE [dbo].[WorkGroup] (
        [WorkGroupId] [int] NOT NULL IDENTITY,
        [CourseId] [int] NOT NULL,
        [WgModelId] [int] NOT NULL,
        [Category] [nvarchar](4),
        [GroupNumber] [nvarchar](6),
        [AssignedSpInstrId] [int],
        [AssignedKcInstrId] [int],
        [CustomName] [nvarchar](50),
        [BbGroupId] [nvarchar](50),
        [DefaultName] [nvarchar](50),
        [SpStatus] [nvarchar](50),
        [IsPrimary] [bit] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.WorkGroup] PRIMARY KEY ([WorkGroupId])
    )
    CREATE UNIQUE INDEX [IX_UniqueCourseGroup] ON [dbo].[WorkGroup]([CourseId], [GroupNumber], [Category])
    CREATE INDEX [IX_WgModelId] ON [dbo].[WorkGroup]([WgModelId])
    CREATE INDEX [IX_AssignedSpInstrId] ON [dbo].[WorkGroup]([AssignedSpInstrId])
    CREATE TABLE [dbo].[SpInstrument] (
        [Id] [int] NOT NULL IDENTITY,
        [Name] [nvarchar](50),
        [IsActive] [bit] NOT NULL,
        [Version] [nvarchar](50),
        [StudentInstructions] [nvarchar](max),
        [FacultyInstructions] [nvarchar](max),
        [ModifiedDate] [datetime2](7),
        [ModifiedById] [int],
        CONSTRAINT [PK_dbo.SpInstrument] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[SpInventory] (
        [Id] [int] NOT NULL IDENTITY,
        [InstrumentId] [int] NOT NULL,
        [DisplayOrder] [int] NOT NULL,
        [IsScored] [bit] NOT NULL,
        [IsDisplayed] [bit] NOT NULL,
        [Behavior] [nvarchar](max),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.SpInventory] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_InstrumentId] ON [dbo].[SpInventory]([InstrumentId])
    CREATE TABLE [dbo].[FacSpComment] (
        [RecipientPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [FacultyPersonId] [int] NOT NULL,
        [CreatedDate] [datetime2](7) NOT NULL,
        [CommentText] [nvarchar](max),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.FacSpComment] PRIMARY KEY ([RecipientPersonId], [CourseId], [WorkGroupId])
    )
    CREATE INDEX [IX_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[FacSpComment]([RecipientPersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_CourseId] ON [dbo].[FacSpComment]([CourseId])
    CREATE INDEX [IX_FacultyPersonId_CourseId] ON [dbo].[FacSpComment]([FacultyPersonId], [CourseId])
    CREATE TABLE [dbo].[FacSpCommentFlag] (
        [RecipientPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [FacultyId] [int] NOT NULL,
        [Author] [nvarchar](50),
        [Recipient] [nvarchar](50),
        CONSTRAINT [PK_dbo.FacSpCommentFlag] PRIMARY KEY ([RecipientPersonId], [CourseId], [WorkGroupId])
    )
    CREATE INDEX [IX_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[FacSpCommentFlag]([RecipientPersonId], [CourseId], [WorkGroupId])
    CREATE TABLE [dbo].[FacSpResponse] (
        [AssesseePersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [InventoryItemId] [int] NOT NULL,
        [FacultyPersonId] [int] NOT NULL,
        [ItemResponse] [nvarchar](50),
        [ItemModelScore] [real] NOT NULL,
        [IsDeleted] [bit] NOT NULL,
        [DeletedById] [int],
        [DeletedDate] [datetime2](7),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.FacSpResponse] PRIMARY KEY ([AssesseePersonId], [CourseId], [WorkGroupId], [InventoryItemId])
    )
    CREATE INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[FacSpResponse]([AssesseePersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_FacultyPersonId_CourseId] ON [dbo].[FacSpResponse]([FacultyPersonId], [CourseId])
    CREATE INDEX [IX_InventoryItemId] ON [dbo].[FacSpResponse]([InventoryItemId])
    CREATE TABLE [dbo].[FacStratResponse] (
        [AssesseePersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [StratPosition] [int] NOT NULL,
        [StratResultId] [int],
        [FacultyPersonId] [int] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.FacStratResponse] PRIMARY KEY ([AssesseePersonId], [CourseId], [WorkGroupId])
    )
    CREATE INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[FacStratResponse]([AssesseePersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_FacultyPersonId_CourseId] ON [dbo].[FacStratResponse]([FacultyPersonId], [CourseId])
    CREATE INDEX [IX_WorkGroupId] ON [dbo].[FacStratResponse]([WorkGroupId])
    CREATE TABLE [dbo].[SpResult] (
        [StudentId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [AssignedInstrumentId] [int] NOT NULL,
        [AssessResult] [nvarchar](50),
        [CompositeScore] [int] NOT NULL,
        [BreakOut_IneffA] [int] NOT NULL,
        [BreakOut_IneffU] [int] NOT NULL,
        [BreakOut_EffA] [int] NOT NULL,
        [BreakOut_EffU] [int] NOT NULL,
        [BreakOut_HighEffU] [int] NOT NULL,
        [BreakOut_HighEffA] [int] NOT NULL,
        [BreakOut_NotDisplay] [int] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.SpResult] PRIMARY KEY ([StudentId], [CourseId], [WorkGroupId])
    )
    CREATE INDEX [IX_StudentId_CourseId_WorkGroupId] ON [dbo].[SpResult]([StudentId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_CourseId] ON [dbo].[SpResult]([CourseId])
    CREATE INDEX [IX_WorkGroupId] ON [dbo].[SpResult]([WorkGroupId])
    CREATE INDEX [IX_AssignedInstrumentId] ON [dbo].[SpResult]([AssignedInstrumentId])
    CREATE TABLE [dbo].[StratResult] (
        [StudentId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [OriginalStratPosition] [int] NOT NULL,
        [FinalStratPosition] [int] NOT NULL,
        [StratCummScore] [decimal](18, 2) NOT NULL,
        [StudStratAwardedScore] [decimal](18, 3) NOT NULL,
        [FacStratAwardedScore] [decimal](18, 3) NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.StratResult] PRIMARY KEY ([StudentId], [CourseId], [WorkGroupId])
    )
    CREATE INDEX [IX_StudentId_CourseId_WorkGroupId] ON [dbo].[StratResult]([StudentId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_CourseId] ON [dbo].[StratResult]([CourseId])
    CREATE INDEX [IX_WorkGroupId] ON [dbo].[StratResult]([WorkGroupId])
    CREATE TABLE [dbo].[WorkGroupModel] (
        [Id] [int] NOT NULL IDENTITY,
        [Name] [nvarchar](50),
        [AssignedSpInstrId] [int],
        [EdLevel] [nvarchar](50),
        [WgCategory] [nvarchar](50),
        [MaxStratStudent] [decimal](18, 2) NOT NULL,
        [MaxStratFaculty] [decimal](18, 2) NOT NULL,
        [IsActive] [bit] NOT NULL,
        [StratDivisor] [int] NOT NULL,
        [StudStratCol] [nvarchar](50),
        [FacStratCol] [nvarchar](50),
        CONSTRAINT [PK_dbo.WorkGroupModel] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_AssignedSpInstrId] ON [dbo].[WorkGroupModel]([AssignedSpInstrId])
    CREATE TABLE [dbo].[StudentInCourse] (
        [StudentPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [BbCourseMemId] [nvarchar](50),
        [IsDeleted] [bit] NOT NULL,
        [DeletedById] [int],
        [DeletedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.StudentInCourse] PRIMARY KEY ([StudentPersonId], [CourseId])
    )
    CREATE INDEX [IX_StudentPersonId] ON [dbo].[StudentInCourse]([StudentPersonId])
    CREATE INDEX [IX_CourseId] ON [dbo].[StudentInCourse]([CourseId])
    CREATE TABLE [dbo].[ProfileStudent] (
        [PersonId] [int] NOT NULL,
        [Bio] [nvarchar](max),
        [HomeStation] [nvarchar](50),
        [ContactNumber] [nvarchar](50),
        [Commander] [nvarchar](50),
        [Shirt] [nvarchar](50),
        [CommanderEmail] [nvarchar](50),
        [ShirtEmail] [nvarchar](50),
        CONSTRAINT [PK_dbo.ProfileStudent] PRIMARY KEY ([PersonId])
    )
    CREATE INDEX [IX_PersonId] ON [dbo].[ProfileStudent]([PersonId])
    CREATE TABLE [dbo].[RoadRunner] (
        [Id] [int] NOT NULL IDENTITY,
        [Location] [nvarchar](200),
        [PhoneNumber] [nvarchar](50),
        [LeaveDate] [datetime2](7) NOT NULL,
        [ReturnDate] [datetime2](7) NOT NULL,
        [SignOut] [bit] NOT NULL,
        [PrevSignOut] [bit] NOT NULL,
        [PersonId] [int] NOT NULL,
        CONSTRAINT [PK_dbo.RoadRunner] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_PersonId] ON [dbo].[RoadRunner]([PersonId])
    CREATE TABLE [dbo].[Security] (
        [PersonId] [int] NOT NULL,
        [BadPasswordCount] [int] NOT NULL,
        [PasswordHash] [nvarchar](400),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.Security] PRIMARY KEY ([PersonId])
    )
    CREATE INDEX [IX_PersonId] ON [dbo].[Security]([PersonId])
    CREATE TABLE [dbo].[CogEcpeResult] (
        [PersonId] [int] NOT NULL,
        [InstrumentId] [int] NOT NULL,
        [Attempt] [int] NOT NULL,
        [Outcome] [int] NOT NULL,
        CONSTRAINT [PK_dbo.CogEcpeResult] PRIMARY KEY ([PersonId], [InstrumentId], [Attempt])
    )
    CREATE INDEX [IX_PersonId] ON [dbo].[CogEcpeResult]([PersonId])
    CREATE INDEX [IX_InstrumentId] ON [dbo].[CogEcpeResult]([InstrumentId])
    CREATE TABLE [dbo].[CogEsalbResult] (
        [PersonId] [int] NOT NULL,
        [InstrumentId] [int] NOT NULL,
        [Attempt] [int] NOT NULL,
        [LaissezFaire] [float] NOT NULL,
        [Contingent] [float] NOT NULL,
        [Management] [float] NOT NULL,
        [Idealized] [float] NOT NULL,
        [Individual] [float] NOT NULL,
        [Inspirational] [float] NOT NULL,
        [IntellectualStim] [float] NOT NULL,
        CONSTRAINT [PK_dbo.CogEsalbResult] PRIMARY KEY ([PersonId], [InstrumentId], [Attempt])
    )
    CREATE INDEX [IX_PersonId] ON [dbo].[CogEsalbResult]([PersonId])
    CREATE INDEX [IX_InstrumentId] ON [dbo].[CogEsalbResult]([InstrumentId])
    CREATE TABLE [dbo].[CogEtmpreResult] (
        [PersonId] [int] NOT NULL,
        [InstrumentId] [int] NOT NULL,
        [Attempt] [int] NOT NULL,
        [Creator] [int] NOT NULL,
        [Advancer] [int] NOT NULL,
        [Refiner] [int] NOT NULL,
        [Executor] [int] NOT NULL,
        CONSTRAINT [PK_dbo.CogEtmpreResult] PRIMARY KEY ([PersonId], [InstrumentId], [Attempt])
    )
    CREATE INDEX [IX_PersonId] ON [dbo].[CogEtmpreResult]([PersonId])
    CREATE INDEX [IX_InstrumentId] ON [dbo].[CogEtmpreResult]([InstrumentId])
    CREATE TABLE [dbo].[CogResponse] (
        [CogInventoryId] [int] NOT NULL,
        [PersonId] [int] NOT NULL,
        [Attempt] [int] NOT NULL,
        [ItemScore] [float] NOT NULL,
        CONSTRAINT [PK_dbo.CogResponse] PRIMARY KEY ([CogInventoryId], [PersonId], [Attempt])
    )
    CREATE INDEX [IX_CogInventoryId] ON [dbo].[CogResponse]([CogInventoryId])
    CREATE INDEX [IX_PersonId] ON [dbo].[CogResponse]([PersonId])
    ALTER TABLE [dbo].[CogEcmspeResult] ADD CONSTRAINT [FK_dbo.CogEcmspeResult_dbo.CogInstrument_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [dbo].[CogInstrument] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CogEcmspeResult] ADD CONSTRAINT [FK_dbo.CogEcmspeResult_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CogInventory] ADD CONSTRAINT [FK_dbo.CogInventory_dbo.CogInstrument_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [dbo].[CogInstrument] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[ProfileFaculty] ADD CONSTRAINT [FK_dbo.ProfileFaculty_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[FacultyInCourse] ADD CONSTRAINT [FK_dbo.FacultyInCourse_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[FacultyInCourse] ADD CONSTRAINT [FK_dbo.FacultyInCourse_dbo.ProfileFaculty_FacultyPersonId] FOREIGN KEY ([FacultyPersonId]) REFERENCES [dbo].[ProfileFaculty] ([PersonId])
    ALTER TABLE [dbo].[SpResponse] ADD CONSTRAINT [FK_dbo.SpResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssesseePersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[SpResponse] ADD CONSTRAINT [FK_dbo.SpResponse_dbo.CrseStudentInGroup_AssessorPersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssessorPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[SpResponse] ADD CONSTRAINT [FK_dbo.SpResponse_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[SpResponse] ADD CONSTRAINT [FK_dbo.SpResponse_dbo.SpInventory_InventoryItemId] FOREIGN KEY ([InventoryItemId]) REFERENCES [dbo].[SpInventory] ([Id])
    ALTER TABLE [dbo].[SpResponse] ADD CONSTRAINT [FK_dbo.SpResponse_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[CrseStudentInGroup] ADD CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[CrseStudentInGroup] ADD CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.StudentInCourse_StudentId_CourseId] FOREIGN KEY ([StudentId], [CourseId]) REFERENCES [dbo].[StudentInCourse] ([StudentPersonId], [CourseId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CrseStudentInGroup] ADD CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.ProfileStudent_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[ProfileStudent] ([PersonId])
    ALTER TABLE [dbo].[CrseStudentInGroup] ADD CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[StratResponse] ADD CONSTRAINT [FK_dbo.StratResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssesseePersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[StratResponse] ADD CONSTRAINT [FK_dbo.StratResponse_dbo.CrseStudentInGroup_AssessorPersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssessorPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[StratResponse] ADD CONSTRAINT [FK_dbo.StratResponse_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[StudSpComment] ADD CONSTRAINT [FK_dbo.StudSpComment_dbo.CrseStudentInGroup_AuthorPersonId_CourseId_WorkGroupId] FOREIGN KEY ([AuthorPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[StudSpComment] ADD CONSTRAINT [FK_dbo.StudSpComment_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[StudSpComment] ADD CONSTRAINT [FK_dbo.StudSpComment_dbo.CrseStudentInGroup_RecipientPersonId_CourseId_WorkGroupId] FOREIGN KEY ([RecipientPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[StudSpComment] ADD CONSTRAINT [FK_dbo.StudSpComment_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[StudSpCommentFlag] ADD CONSTRAINT [FK_dbo.StudSpCommentFlag_dbo.StudSpComment_AuthorPersonId_RecipientPersonId_CourseId_WorkGroupId] FOREIGN KEY ([AuthorPersonId], [RecipientPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[StudSpComment] ([AuthorPersonId], [RecipientPersonId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[WorkGroup] ADD CONSTRAINT [FK_dbo.WorkGroup_dbo.SpInstrument_AssignedSpInstrId] FOREIGN KEY ([AssignedSpInstrId]) REFERENCES [dbo].[SpInstrument] ([Id])
    ALTER TABLE [dbo].[WorkGroup] ADD CONSTRAINT [FK_dbo.WorkGroup_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[WorkGroup] ADD CONSTRAINT [FK_dbo.WorkGroup_dbo.WorkGroupModel_WgModelId] FOREIGN KEY ([WgModelId]) REFERENCES [dbo].[WorkGroupModel] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[SpInventory] ADD CONSTRAINT [FK_dbo.SpInventory_dbo.SpInstrument_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [dbo].[SpInstrument] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[FacSpComment] ADD CONSTRAINT [FK_dbo.FacSpComment_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[FacSpComment] ADD CONSTRAINT [FK_dbo.FacSpComment_dbo.FacultyInCourse_FacultyPersonId_CourseId] FOREIGN KEY ([FacultyPersonId], [CourseId]) REFERENCES [dbo].[FacultyInCourse] ([FacultyPersonId], [CourseId])
    ALTER TABLE [dbo].[FacSpComment] ADD CONSTRAINT [FK_dbo.FacSpComment_dbo.CrseStudentInGroup_RecipientPersonId_CourseId_WorkGroupId] FOREIGN KEY ([RecipientPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[FacSpComment] ADD CONSTRAINT [FK_dbo.FacSpComment_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[FacSpCommentFlag] ADD CONSTRAINT [FK_dbo.FacSpCommentFlag_dbo.FacSpComment_RecipientPersonId_CourseId_WorkGroupId] FOREIGN KEY ([RecipientPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[FacSpComment] ([RecipientPersonId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[FacSpResponse] ADD CONSTRAINT [FK_dbo.FacSpResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssesseePersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[FacSpResponse] ADD CONSTRAINT [FK_dbo.FacSpResponse_dbo.FacultyInCourse_FacultyPersonId_CourseId] FOREIGN KEY ([FacultyPersonId], [CourseId]) REFERENCES [dbo].[FacultyInCourse] ([FacultyPersonId], [CourseId])
    ALTER TABLE [dbo].[FacSpResponse] ADD CONSTRAINT [FK_dbo.FacSpResponse_dbo.SpInventory_InventoryItemId] FOREIGN KEY ([InventoryItemId]) REFERENCES [dbo].[SpInventory] ([Id])
    ALTER TABLE [dbo].[FacSpResponse] ADD CONSTRAINT [FK_dbo.FacSpResponse_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[FacStratResponse] ADD CONSTRAINT [FK_dbo.FacStratResponse_dbo.FacultyInCourse_FacultyPersonId_CourseId] FOREIGN KEY ([FacultyPersonId], [CourseId]) REFERENCES [dbo].[FacultyInCourse] ([FacultyPersonId], [CourseId])
    ALTER TABLE [dbo].[FacStratResponse] ADD CONSTRAINT [FK_dbo.FacStratResponse_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[FacStratResponse] ADD CONSTRAINT [FK_dbo.FacStratResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssesseePersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[SpResult] ADD CONSTRAINT [FK_dbo.SpResult_dbo.SpInstrument_AssignedInstrumentId] FOREIGN KEY ([AssignedInstrumentId]) REFERENCES [dbo].[SpInstrument] ([Id])
    ALTER TABLE [dbo].[SpResult] ADD CONSTRAINT [FK_dbo.SpResult_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[SpResult] ADD CONSTRAINT [FK_dbo.SpResult_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[SpResult] ADD CONSTRAINT [FK_dbo.SpResult_dbo.CrseStudentInGroup_StudentId_CourseId_WorkGroupId] FOREIGN KEY ([StudentId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[StratResult] ADD CONSTRAINT [FK_dbo.StratResult_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[StratResult] ADD CONSTRAINT [FK_dbo.StratResult_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[StratResult] ADD CONSTRAINT [FK_dbo.StratResult_dbo.CrseStudentInGroup_StudentId_CourseId_WorkGroupId] FOREIGN KEY ([StudentId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[WorkGroupModel] ADD CONSTRAINT [FK_dbo.WorkGroupModel_dbo.SpInstrument_AssignedSpInstrId] FOREIGN KEY ([AssignedSpInstrId]) REFERENCES [dbo].[SpInstrument] ([Id])
    ALTER TABLE [dbo].[StudentInCourse] ADD CONSTRAINT [FK_dbo.StudentInCourse_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[StudentInCourse] ADD CONSTRAINT [FK_dbo.StudentInCourse_dbo.ProfileStudent_StudentPersonId] FOREIGN KEY ([StudentPersonId]) REFERENCES [dbo].[ProfileStudent] ([PersonId])
    ALTER TABLE [dbo].[ProfileStudent] ADD CONSTRAINT [FK_dbo.ProfileStudent_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[RoadRunner] ADD CONSTRAINT [FK_dbo.RoadRunner_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[Security] ADD CONSTRAINT [FK_dbo.Security_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId])
    ALTER TABLE [dbo].[CogEcpeResult] ADD CONSTRAINT [FK_dbo.CogEcpeResult_dbo.CogInstrument_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [dbo].[CogInstrument] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CogEcpeResult] ADD CONSTRAINT [FK_dbo.CogEcpeResult_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CogEsalbResult] ADD CONSTRAINT [FK_dbo.CogEsalbResult_dbo.CogInstrument_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [dbo].[CogInstrument] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CogEsalbResult] ADD CONSTRAINT [FK_dbo.CogEsalbResult_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CogEtmpreResult] ADD CONSTRAINT [FK_dbo.CogEtmpreResult_dbo.CogInstrument_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [dbo].[CogInstrument] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CogEtmpreResult] ADD CONSTRAINT [FK_dbo.CogEtmpreResult_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CogResponse] ADD CONSTRAINT [FK_dbo.CogResponse_dbo.CogInventory_CogInventoryId] FOREIGN KEY ([CogInventoryId]) REFERENCES [dbo].[CogInventory] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CogResponse] ADD CONSTRAINT [FK_dbo.CogResponse_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    CREATE TABLE [dbo].[__MigrationHistory] (
        [MigrationId] [nvarchar](150) NOT NULL,
        [ContextKey] [nvarchar](300) NOT NULL,
        [Model] [varbinary](max) NOT NULL,
        [ProductVersion] [nvarchar](32) NOT NULL,
        CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
    )
    INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
    VALUES (N'201708311912215_ECAT2_Init', N'Ecat.Data.Contexts.EcatContext+EcatCtxConfig',  0x1F8B0800000000000400ED7DDB72DCB892E0FB46EC3F28F4B43BD163D9EE333D3D0E7926644B6E3B8ED55648ED73F6CD41554125465791754896DAEE89F9B27DD84F9A5F5890C50B2E99B81104C972C589E86315C04422919948241299FFFD7FFFDFF97F7CDDAC4F9E4896C769F2FAF4C5B3E7A7272459A4CB3859BD3EDD150FFFFCF3E97FFCFBFFFC1FE757CBCDD793BF35FD7E2CFBD12F93FCF5E963516C5F9D9DE58B47B289F2679B7891A579FA503C5BA49BB368999EBD7CFEFCDFCE5EBC382314C429857572727EBB4B8A7843AA3FE89F6FD36441B6C52E5A5FA74BB2CEEBDF69CB5D05F5E4D76843F26DB420AF4FAF1651F1EC322AA267F4AB827C2DF2D3938B751C514CEEC8FAE1F4244A92B4880A8AE7ABCF39B92BB23459DD6DE90FD1FAB76F5B42FB3D44EB9CD4F8BFEABA9B4EE5F9CB722A67DD870DA8C52E2FD28D25C0173FD6B439133F77A2F0694BBB8AAE9BED9A7C2DA75D919092687B4BF2DDBA789391E8F74FBBE2F4441CF5D5DB75567EC0527ABF2ACFEE8ADD9224C53311C80F276DD71F5A1EA1AC54FEEF8793B7B4E72E23AF13B22BB268FDC3C9CDEE7E1D2FFE4ABEFD96FE4E92D7C96EBD66B1A678DF64E99664C5B71AE90F097978B8383DD9A3F521297E7C797AF22BFD2CBA5F937639CFF4303EF78371D51B8BABDE38BC8F578FDEA0F49CCDAF697119E7DB75F4CD06CEF919C396ECEF57542714DF18667D9BAEAE169B7C4BF6EC66C5ABF4DB242EE227F24C80E29D59691BF70343A75BF2504FE5860E94261F962249C1CE1F92BCC8761B2A6A861F5C1405D96C0B69BDCE44DCC415ECD0EA275B2CBE7D20B513E9056441F5E2265D460569005DA67419893DA4A7345EF684F1362DBBA7597F6C4AB1215EA064E926CEAD01FD1A3DC5AB4AEED0F53F3DB925EBAA4BFE186FF73BB228805FD8EEEF282AB7E95A1676A6D797BB74972D4A74536DD7DFA26C4564414051DF0B8011DA4D571CE57D0F2DBA753708D5F3B34E09EA54234B460BC57849F2789590EC1907621CAD28AB37BDCAD2AB18860C77459A915F089D2E15BFE54D44D54B9694304845509DACD4846806A4B624358B4F4FAEA3AF1F49B22A1E5F9FFE0BB583DFC55FC9B2F9A1C6E1731253239A7E43E9ABD79FF9C5A2DCAA9A71DEA4946FA2C441B4EB155D54ACAC409BFED303DED75B8E87CAD186261565E2F82126CB37DF243E30FBF092D5C3F4DFBFD163888DA67BA2134DB36FA556278BBD950EEB8EB62BAEEFA02EAD5A60B407D8AFD132EEEAA306DA437BD4108ECA031CCB9B55541BD99FB225C97A5A6AF9DD824E6AD957D37CC86B9CFA83BA5846DB52F951CE5A64F1B650EB5B3F8AEB035DE4A728F8A8D4B60DA123CB7182CE2BBF25A50709E085892B739DD9EAAAC3010B50A9EB9D7478638F5A68EFCF39D5DCFBEFA6750CF67752F5A8BF3D59656FEE4BB277980F25F6FB71CA7F0F3DD2C7282FFC8C6349CA777136D2C81774BB88B28FE922D2A8542F04FE257DF32DC4425E6FA97830464530725E6F2F1E1EE275EC899AD6A3DF44DF5659B40CCF47E59169B34D936A23093CF6D5268AD78A517F1E64D45BB28AE98657ADF4DEF1DBEDD9AE2AF57A5B6EA271B12B48B9C1865FC5710D9777D182EECFDF40ABA5762EB55D3A63856F916C14A1D9D691769B46CBDB5D4237DB8BE53223794E7210BFAE1FE050931AA5D3B0DC033A07AB10BD238B5D1623D46B1A01DC84260933B1DD1AAFFDE59A6A51DB2ED2A2D62DD8A236CDFDECCD2CA52A9BB49C656D7772DF1FAAFDA9B1D1E254A1AC7E7AFEDCC7AEFE3EDD90BB2288A1F2217F4B392E2717CB4D9CF4D5EAE539729B66C5DF62F247679338FB1516747FDF7C1BC0EE4645784F0B58EFD56CFF21D9776AF46C2D14AC48AB7B4A7A47D3DD560D292E448CB716114564E771D242C274EDE217168F9499C4A518471335CB64732FBCC7D7457B49A3F551621D1ABD54E1FD1ECE35D90C7F36FE905F92D2F0ECEDADACC1581B7EF577FDECBE86E50DD44BD315572BF5FF891603D2CDD61CA460EECAA34EE9E6421562DBA3510D20D6482F4811625D6D956005EA96E4F4A0A6D0E75D9766B88BD2ECCDD34C9A01D6119E04DADB651EE5D1AB0188CE84EDA49B8CAA2F381FE5070E53E2F6CD21B75A9D6C203BB3E38D9CEB7636E62E36F53BB8E18C406437538EF4938F9142F805DFAEA33CFF75B7B9F7E11A540FF51BC93683BB53B368B93F5A546C9F3FF6B703E8F92A2BE0DDDC1252899C1B2027FF90B3A1A03B77386EB5BA7D96D90D6554A546D92B23F5704290CE54815E19D48520D735C1A831EDF60EA3FDBE8AE3D67580D0935A650CE52E8E5E2DCA25F4EB5FB274B785712D9BDBBE553F0065B4931C3483F6749C0046E166769830C13D004A83DD6C91FD7B9AFDAE2072DB0C202AB649284A1D7A1DE33BB9747F81507D3E8EDDD398AF56C7F7FD47847839F3839DDB35320E34AF8312CA9011175B4EA643AFC06D8940E3BB293892F60B921768DD07D8F5B684D289D0F00145950056C1633D311FF7EAAC61319DADD1F503AD8DA6593A22427D6C7D27DDC1DB0447FE800E34EB7164CEED1E5C5196169B023B47DF1327693A1485CE20A65C1F15C27C475BBC5B55A3C399E908E2DB6DD50A5CBB4EFDFC1692A5E5E4C390A08CB3AF373884DD9BF5FB2C83D7016D89EFA3FC62F17B92FEB126CB55FF33F99BFB868D04EC8EB70A93DD872D8EFFA65BB2C205206DDBD6E86ABDE9BCB71BC419EC811EBEBD609E66D684D6DB155A423BB8F75B749D082DE00CF63022B40BE6BBE231CD3E3D282FBD4AE5D45D52ED3F11DD05623BE82C903AD9A2AB30E07AFA62A4E077ADD7C6D2AF59CF5C7FAB784B16F1361622A6E01EEADBC4AE9BE37595DD6D22A435E01E9AFB4367ADD1A05D8A86298FF0DF283985ED6AC22F5C7FEBC8C466F1AC6413E41DA48B4642DDB9A771119B2E41D75F49FED6F36C40FAB6AF2DD919E7B131FAEC27EA19309E69934930DDEDE7C1B966CDE7227CA6990FEFFF359A13FF89E3BC5417E9F8A8C05DBAB6B3C5A45437EAEE2768603CF024ADEA6732094F276BC10472F192B3108E8E725F87F1A3D3DBE29A9C72E04D9AC76C30F2013A8E5D0F7D927F4E7D36F4E341763D3899208BB8922D941E6339392A3D06C2484AAF3A925969AFD6469CAACE13A6D4479281B91E90CA53875AEB28F38F1DC98B8B244DBE6DAAF0B6BE09612A29F88D7C55BD36F49305E06D4622D41739335D5FFB657C397064C5A9F0F2F8B881E3E0C3412962BB0649578FCD3A5AE9512C7B7D69753E8227DB497DE0E67ADA1EB99973BF5F1F819ABEB22FC1D3AD21370A7C710877D120ECED90232C5CFF3DBF8472DCF78FFB7EE07DFF7ADBEC0643E76C60D4CED043B591C4C30E548AECAADCEE9BA062CDAEAFD814F1AB819E5B8E521D829B9393426454B47D1845FBF138FAAF97A6B110447FEF433CE99055B50CFD35C85B3A895595920F95B7BF78790F41C9AC7DDEF19387912EF22A5D2015942AEB95A535DF7CFDD785CBD76F2BF109F15CE6CD7DA080934BF21051D90D31A7EBEDDDB64C22B053A511F514457393C59B28EB7DDA1DFBD4C8F33A1265C1647F6B3E6822F6D9680BB41B107581F7F5181460F57640DC2BD1C705360F592D9E1383E71CB887FAE25F3E0D59616C73E98FE3AC08D984AFFDFB61CD7A7571C439DF2F8A3BDC4BFF4AD87D06D557D7A4DCDB8C5F1AD9DF021ABC37729F8286D37B1CE9D5710A7D30360E2E338F5D568497F5C454FF9A0FC7926BC5DFF4F5C1D0400219F05277482F8BBD141A5AEA6A1D58D00240E90C8DB6EFAC46BEEC63847AD5D1DA93B637E3D518575DBAE58551163BE18FE9A49E3D5FD5F52D0AC04238A61480C60A617CFB4A3D1BA87E41BBF305AC3DD03E3F0F59EFC0E69CE07C38D11E30142F7C3D9C2FE42743FAB388F5CB2C4DF984724C75E66DB8077838F25D3B8181E9AE638F951354631D2B2718BD77228F54D094370E7EF5DE34F3F43B2B0B48D3F94ED2CF7A13ACB445BDBF717E8D71B4C5D4AF280FFB5AD16FE6468F313A21638B465640EABC8BAA481BA019CA2AD73FCE86CF9F3844BA4525DA7066C6DE5142EC18D88D2DD647E9D81D3044C8F9059A92C043C50739BBCD95D87A0A0E1257ACF7063A5E68D071139DC0267A0CF1710C9DE9A587559AC25FDC0C7703E6AE28A6F05AEA0033841DE8F326AF39BDBC5AFA5348107647C77228507DCCAA32CC2335E71C03A002EFFF484D4A333E5C9675F50CB01CE6D65E6D34DF183F1E9A724CD14D338521138FF508535123EDF194E0FC549ADDFCA7F25A7AE20F9F0F742FF7F854998942B0DC06FCDA0093A852A7D5EFDE6A4F40DAC6A858857164CBFE9E5BB9CB7A4C8C6310640626D2F1A7F67B05F96997C3D713B236E98D6B2269FAF131DD2414D071581ABE095EF077BD7CBDDDEB828601873D65559563E9FE443CA4617E9391E8F74FBB0E6736FCB16B9CD78E232FB03ACA13EA0F847BCADDE0BCBA485FDFA995CDAA4DA0183ADED1ECBF7E87ECE31E729119EC7652DE323F3B9D75D42F4A5BCF999F9C37B5EEFBE3BE76F8FBDAA72C5EC549B4F6788279E7195E05EAED6EB3E1B6AE4BB28837D1FAF4E426A3FFAA58F3F4C5CFA727778BA884EC32CC6E590D75F147942DC9D266B41F5D1CB6E1069B6E08847599212C8F54D0DDC929D5A4C91E0564A674DA04F827026E91A63C8C63B02934568878FE7ECFA0AFB75774F4A7920986BE77FEFBCAE009BA9FB1A2AF95A8B435ED07DA129A71844C16DEC7F1F564A342F6327E8AF32ECEC075CFAD37C3B7E9E09CD36C85430CD5EBC1B5F0D04AFA047DBC25F4C45F9DC1DD9DAF60340FE6DC9E9F6990071EAA399E5B849CC7F6194C0410A31E5FBCDCC2181F4AA674A972ACCDEED74076AC1209A51C02BA395E6318E1DAF6C591ADFFD662DBF473568D574996AED7F8C378EF59D88DCC7D3871BB93FEBCD9A73D6F696EA13E3FE7D4DCE7BF1F4777624A53AF07FD28C03771AA50583F3D7FEE4365BD4F37E46EBF26C33BFD93225A14618A7197919851B21C7EA0BBC7380B715FB29FCDD5268A073781AB290D339266D361539E506D68AB1C6F5C6A3998ABC61BA1FA83DDDCCCEA2E5BEC52BACACBE26E668AEE5E7B81D8EE9B2024F9160937A1B9D7DE729B46CBDB5D9294B26DBBAF74DF1E5D48D0581FD3856E2F78E965DFB9794C1312662BF848A227E2E5A1E02DA14C9178017547CFD8CCA5B5EB41E126234FBE40B9192D2E6AA493C22F4DAF4E93488D92392EF7E877D6278B5D56C986AD3669BEFC4EEDD388EA973CFF23CD9674C7E91C9F6ED01A50EFA3FC51A111FEE245FB8C7C05A5108D86A500C1109AE453AAD0DEAF1872BABA5A6C89C3D53DFD32894BFFF1330EC6B46404DE9CB9682A830F2EE8EEBAD9CA0E804002E82FFAAB9D48AFCBFB5DB148BB1BA8BEFB8726F489632E245B06D6472E7D8675B4F5F028249B1F43166FA85D83A82F41CFA3F57D6F49EF801C45FDD045FD6314E739F9F35D14337122295D477B43B8F40AD14D9EBD387503741D25D18A6CFA03A267A5681DFFD979F15DE124CBF8295EEEA2756F40F936CE2A69F400AB2055C2B25D1915156F6CC1F551D79D8650E86BB013A807E19E9E35363308ACB2A50E3A5C7D29ED62B3CDFADB670C94A3DA3E74B55D254DEA1B8C71B17C8A9245DF24747489E2A42F90ABAFF4C8633FA13E3A8C91178512837B819A01E9EA598DB1A3C07A4CEEA145D79326737AEDCA69B1719FBA5204BA47CB666AC94AE939EB3011B15EEE193F15827D68B1F261381FA1DCDF76D13D7B67980C7FF48E768224C9D38377B5DCB78380322FB42AB17490F58B3C4F17718515C090ACF6E4E77A952C4F5A04D4E92EBB075F6CD7D3936B2AEAF1960A3745EBF5E93F49E4D40FD15E600943741A9F1FE3C5A928C89F927D34CEC945951D980E15E58B6829B32525DF92FF85CA3EC9CA0B9B684D8F4774C8284E0A5951C4C922DE466BB399089F1BDE4E95E8B503892D97644B92F262C96CB98C30501B7867ED90020975143B3F63B851CBA4578B4D0EB996144C847C82B02ADBDB925BB18166C9B09AC984E159CDD2CD846DF907E9EDC37E8C93E0EE10BB0A1949CC99151902625429E442E656711C279653A364B2D28AF780887188BD0804BBAB939A99B3B57A794D30D127700930DF91E4869E286D989A4DD83190DC30293EA622371D4A072E37DDF29ACB8DAA76F2A1C80D503E1E6769552D795666987E7632A3A8433F8EC4E0081D94BCE00B6B242D9A3AE387282B7500BF192B8BEF27BCCB8AF840803D3EEC1F1A4CEDDCA0C23FC08941B53EFDB8373C2F7219908DF805CE878C70E53ED339CF3FCF9F3D9355ACC96000872A456008450E2134989A3348B43ED9AD006293032754F07DA4CBAC6EA6E981FA0CDE7713B9B8C3F8C69784D3C1DA5FD20A1FA587951E45454194BB4DCA0BB269E77087AB6EE3332994DE8DC424F8D2C9AA9B08E9271EC2D6D213C5F04C2E262D198D032DAEBF74C5DE78BE73B8FCD2959E3463EC099C0894F308C4A7B3BFF8EA52FE694EA8624788232DF49302F08C4EA518EE01B80F5B8F199C46A1A2731877282BD0750CC2D7A934673E55F1BA19F09F02FD002CA8589BB971215F51D0885B90F2829E7912AE4CC88FB22F248EF1A817CB508F93C98A4BE5017A9E395C79155CBA71261098D18D7C7FFA526830933B78FEF4D52FCDE46900367775FB4DE368ED2418AE0EBC694C39F49EA177C5E92AA59AF1B685084DC415A746E9803C71EA053E8A0E263ADD41D188AB81D4F59ECD2C39F3BDC9B17608719130198D9F25AACF88D1CC632275450305467389EDD2141C1C4D437F0F3191EAE535F4237F173191EA5A9566BC8D9634F32E45582DB4714EC63AB4E67038D62DE2619F8F91875E66CC88BCFBF2CEF4F0AB31B32BA141D81D442894E7514D7EB3FB176575F051F8CFD03456152CF4CE77E35BC7382AE3191333B68F95354B553C6156C0946740D72711C6F54F47D9F34D309BC1B66FB2A007BDF32335628D9952AB7FBDB1FFE85A588D4D6845AC5E8579E86255294F8C2F8CEA7AB226A1FD8B639372A0C671429E22D3B41805303B0D486FE851000AF98ECB80BAF7295879564F8C661F7AE197ADC23F2881093A834D132837AB5D5EE526D98B7146DD14712C426E863895E7B10932686FB95D1D8FD0C63FB10A4BF41AC065841DF89EC9CA40EC19A788D12B24C3EAE9330FC66D7037D93F1565646556B055867801DA601B298A4288BD1425EE0CB6534818E844EC745FF9C154355F859B42EF1931BB37ADC7526A6C9DC752661E1A4F538E54CB169ADAA400E3D6D5A31D9E13A80B9BF679BFD58F19957805D096668B61738E1DFB85155E5DD69049A052B3A6BC38B426454BDC067C17A8255778B6950962A44157D5D71379CB6D708086FB0FF8967BDCF3B41A95A0876A35E5E7B15D03550D35E714F40B30A7A5365A4D99D6121D2AF4E1458B4900F5A625FC0C8E32C01CEA6BC1CAE2B5613AF63B37D65385CCEB06441E9FA81C35FE0C45436A98F0C34C62340D97C3D024FD2E8235A17AB58D1BDD82EB5BD77108116B0703CF60B0276050B112677FD82225927FFAB31D498E18BF940D77334EC820D2C48C67E75D1B56A664324C9FD17A8995BC0ED39F70A0A32E610A931B24C094BB63075DA6A7ED51171824FC65890A8D2017262A62CFE0A48154BD37E62EB124FD30ECD58C02F017A5E843BC6E7449203E13F0315967DC8EEEC16E02F52DD4A50A9DB1AC059E5DAC7670812C013C2CE298487E5585008C9FF4C67C5A5E38CB5AFFF5B326409E086B518C2C4BB56E7410A5FACB9092D40C39968E37C62C98B6375E9E9E4C3D1697EA6F6D541F05E1CD512F714CF019F9CC36D3EB1C9BB40686390DDC5E074E2D9BC1F791CAE098C7C04D4A54256AE4BE034AC9D8D569707C0E594A8E959C345262F2F487ED3988848CF90008C22084170E23EC0C4E6FD609336CB265F466A609E4C99840928CC3C99061951EC33437466F2E1BFFE9D9045E621F443E0C366584414272A03BF2F85F958B42F3F61F1A24F4EEA84623800A53137B06FBA43881264D86C6C5A9FE6C60661306C33D9B75C740CC07A315CCAD69B62426E86853A504E1CCFDF0CD34504EE4BB419CB7EF11E85984808E356FFA8B7D8109138C1D6142F41D3E00DFDDA6D1F276972424FB52730EB6D6524F88FBBA4E362A4F060D71D2F08CEDC47928F2C1980F5D9919F0DF1D59EC32BA923AEE13FA812788BA8B6DD13701B205EBF9393AC0E307E31E84B233E09D06614DB411DF6D2A7BA6733492F73DD321FEC7E79EE9390C2AC42D74BABA5A6CF22DA9E34C35AA0BEE0EDE3CF33DADAE9DE14166B48FAA67108C2BD5AB3517EE6CD137C860877D8072A8337F4283804E94D5942B5DEAA612C011A35B32335FF2E8E9EEF86998A8515325DA9F4567AA4027A13EE7AD3CF3687D6FA73DC12F30DEEC3ADB32273CCC3C15A8722E8134A872D966A442997918E850A9F7A08C3A4F358A4E20A81E45576A268AB4D86C334B3B14FE04E350A6B72D8B2203CD5399AA2713489BAA976E46EA949D88813E95BB0FCCAEF354A9F80C82EA547CB5E6A1542DE3BCD02F101E7589C1C1C7C0B4291AEB3501461D3D4E4CBB642638B0649E04BBEA15A9D07520069DA5F244B00FA93891D5999CD2BCA2DF94A5E493827E41B21A85AB455494BF91AF74CD17BBBC48375192A445F5F9ABCF3979BBCE4A56C85F9F520B440E042A61DE9102F6FF9F9EEC3BA81CF9126F4A103BE3278701B246A809B8BDF4C70405D76A610DB41B926ED70480D2488CE6FB36F6430620048798016A627EA0894971575A4A61900C01740197101036A85507887B73026204BC4CD162C727748750E4D3A81960D9E66184C171792F6DC095E5EE7510F77D3450998CB332382612584B3CB54CF2898A0D80A924920B81D7CB81720DF812DC1A600F2C3064095880462BC055E6435134E63A31DB1F06D18E95B74D6A77446E4D14B790241E172F1360753A5C25D7D6498F4D9509AEDFA4BC126620215062BC82065217AE74B15C66E5234808281B49A6436D1FC18208561B15A4DF34D97B23784FB7D9D139072A088EED60008FF31F8000B91E7A882A79E26C6D0114637B8146C737D6DD77C2F406CC0FD433289AF0D017ED21A19D9C6CFC4846A71E6A733A10A132BB8168EBF3243123171767A0A518D65B393DE423846E2A9BD5147208DA718ABE7B7C0FD00DE989CF0CFE00A2976C5B29088680058825D98183502BCD4CA9253ED7D64E8B79B3ED935ACC6BED81A9C5661ABFD8158F18AD807EAA29C9DD413A0906B6924A00C81168D418103A1A412FB91413125E72F9A091F06E8B5351B59DE49736A5B1FCA5B5C43504E23A1B4E89FD464B2AC87437048DB015BE0ABDB9EA962CE26D6C4237A6AB2923B45F7865AF0E6A182964CE9F6DDD98063A40334577C50CF1AF6035AFD8E98D0103E463CFF45E286766A9223DD5D332B353058F8086546656AA92FC0E84EADE10E34A5EEA834F44EC0A91055F6835A841953AEB00519002EA864F01E80D1144F0F6286802010C4796DAF56A481DBEB7E19CB88F3CD28A870B904CF63C7BA59DCE4C40FB9ACD4F6724002E4043CAE96C04E57AF46538A58580F4346408A57DE0C86481AD036E6CC6E9AD211592D0039F969CD4A33FA9E4541E4359029C1F5AE949407A6A6664E44910BDE53A328DE447E0C7AE55A2CA9DA0FEC07492C2775E0928C20EA8FA91401E2D1915713F8A89C2B13F3E480847FC98DAB9BDA9A7D76CBA5C458AB9E9759B0BC5C26A37CE956628B4CA6FD4D3547D8A51D1DC31A81D219C00736868B910E96D31532D2FF6A062388E6CAB1E35277EDDF11BEFAE3A2FA35FC107F1F6FA567908C781063889B7C32BBCAD421783B9287CAC764409E2596D06534A1BD0CB007FA57459D2229C344155CC1BF957396AA4DE267E16F1232FCE1B0928284A6ADDD6E34A4C2350722FFD9D954EACF87811834BB050E2052DCA1E493356AAFADAAD79F9896736AA402A9908227C1F728175DB954483BF3098A7B2443C47C03692C8847C30D890EE65A952BC9E7C585979D51481C2F23D890614821FCC742AE062E5DAEB31A38D10FEC2EBF558B87D112FB40D85D6A09D15F12FD83718C1149E1843C083EA7E5DA96833B2E1D5A5757304EB4BFB2321584DDAE6F8E487A45D54A91139E10AC2BAB94A3584FD9151AA186C62237B221D1B466B463DAC6EAC769640E5588F3404EAC40E69A620753F914D03E8A9D6EEF2074A9A211ACB04ECE0413BEC986D28B39E4C60F639E584C44474BE0825669493E10E269C4228B9A1802A4A35EAC5082ED6E85350E1D28C16ABE595BA4D8E6A2BE28289AD0D672E66B7F64E5A31A375307E55DACEE625E674D3D45AD13D4818D8636B10686D75376A78316A7ABF32DA952854DCCA8438EA506145152C1FC40916252CD5345293C6CC6D0D963EEA479660CE6BC30B5F87DB5E9BAB5E070A05BFE435BDE1B5BCDE35BDDB752051D05B5DF642531D8F08F554DE0C021F20378ECA5B5513B043C72672630AA53E0CA8A52A0EA29C1E521EC417F590822032780F5414CA5200545315AEE0A68194AE60D06E120D286881149C1864EE72690460FA9AFA091CF2780505067FF075AC822278ED042D655DF4B290AE1F52C9AA8CFEBCF64472FAB38A9879DAABD2C3480EFF214820649DC74542EF194032D33B8944A0D33F9CFADCE0C5AA961B3469D2B94DC2FDA56A000641137063547278D56BF3A6D79C4E63BDE705F3406BA965C651A6FCE440A540BC04E72346C86390BC589A913A7DB140283C678121DC500C2567C5D593CC88A5F004BA5E481588AB90E4AC088D4C52B94A93D2247315A8A5C85D610A39146B0119420DE866C45C8A6CA29EE81586BFCCBD1D86492DC53999FB3BF844276A4A997B3CD469467A124DC954622FB3092999C99E3E1EB9A84C94587EDD26ED6BDBCECFEE168F6413D53F9C9FD12E0BB22D76D17A1FAAD3345C47DB6D9CACF2EECBFA9793BB6DB42851FFE7BBD393AF9B7592BF3E7D2C8AEDABB3B3BC029D3FDBC48B2CCDD387E2D922DD9C45CBF4ECE5F3E7FF76F6E2C5D9660FE36CC185189D0BD8B623514E885644682DA9B624EFE22C2F2EA322BA8F4A4FCDDBE546EAC6A628E429D7D2B61909CE4228AF5B93D1A7F9AEFC77970FF15989CFB37AC85C4A592880EB48FA8ECEB254A6D58489C22C97205018778B681D654D8E482941E4DB74BDDB24BAB49138243E83340B4D9D5B1A87785150F9DF163CB0F6470B380BCA5D9B7419154480C53658C07B4A63618AF54FE630E8C7EBE89E72AD8813D760036FB32532ACFA473B3859BA89730054F3BB0CEDFC4C60505120CE24891034942868A662C81A18FDA51037AFCC6450F53D2A37A2B458F1D1DFA8A4C6E576C582687FB490DEBC4CA9FB24AC7AF7AB0D07D564A852F4E6221B098DE670AFB71C81CBA5446077CD16D0D365FC1093E59B6FE27AF02DF6102F2511E75BA6254D9D55E5439A9AAC9A8EC2847E3E8C2CF9DFC12EE37CBB8EBE7DCA96652A451622DF6223A7770B4A2911BFF6571B48350E3230A6C162575C46DB52575C927C91C5DB42D24A60079BD549D2A7483902D2C5620C6A5BC88AA5FBD50E128EA7D868B36AB7E489AA7779D1BADF8F5ACF42EBD547A69EDA0E3E0E1AE839ECC300F6BAB71DFFCDFDE79C64224EDDAFB690CA7F43B0F6BF9B43FB18E5850CABFBD51C5275869441313FDB9C1E28E7641FD34504A848A1CD1CEA2FE99B6F3282DDAF3676D62F65590061BF6A7EB38173F1F010AF63689A6C830DC49BE8DB2AABEA3D70DCDFFE6A674D6EB66952F95DC5834EFDB339B4AB4D14AF7938F54FE6306EC92A2E8B2D94542991584BC739B887CD9C4B3B262E7605299D4AB289C3341DF7108B3D040FC0B0DB44F8920C0E9B8906C0F09BCA9B38153477F983F9F7EFD30DB92B0095C135D86C72FBC09D8BE526168D30BEC9CE04DBA659F1B798FC21AA49B1CDC6494535D8469221E6E7A9B17B1741D593EFC5C221F68CAF85806EEC753C17280052A38D13A4C44304D8FD6A6312EDBFBA261BD9C2E29AAC8E7E551925F9E0D7FC6C71C4DD7F226B7FAEC11A9EACFBB986C9C882271170E67C5B86EFE7217151523AC6C6B8DA0E9A6CFBDADABD6FD7519EFFBADBDC8B3A9D6B3087F71BC9363CA0FD2F16363D356AF7DBC9CDEE7E1DE78FA2C0821DCCE1D3FD342B6441637EB6C35506D5FD3A198165C3CB7B0A2D5345C75E70551FA3E257BF2181B72BB9D542B0EBA73B2AC86CEB183B611BC82F02E31A6C1C8A4C9881ECF1151AAD8E59F49B767525CF5FD762E74EACAEDD2B3FAF0C936D3B1EDD6CD481F0F4AAEF3E2ED78973D8D30D80E03A7DFFDD52D4E9EDCF8720BAEFA3FC62F17B92FEB126CB95B82B4A8D5636494D7B105BA0F9FB36BA8F8A45616768F3BCD9191B5C813D077B43FDFDD1E408A1B7AA45B849F3587670094D4759B3DCC499B458BD258D2DA3EA2269CAEF5179A88A4821D220B4D9DC2AD499EF61C040F321C8D9506EBD5BF28F1DC98B8B244DBE6DE2F2A5234F4CB1D52AEEAE0A5BAAAA640BF7515D8305BC8C44E08ECE351CB58CAB96A94A75F8543410405B6503C3382A9CE115CEF5B6A9562853D10E0E535905A49B1DB4F64536A000AD142AE5AC5529E3CD4D8BA85381F6C908AE325DA995C4A2792E0C2455F16D282EF5283FFB7CAD125EDDCF56A11854D1AFAAEAEB1C5EEDAF366E684A14C889CF35581D52D8C4BCC029456CB687FDD7851236D36CB1D2BBBC4837F2A508FBBB8D5B04718738F0E0257988A8869051E31A6CB8A73C6947C54E8834EF7EB571D1DC64F126121991F9F9683059B9403CBECE6081395DB7847F9B21F3B82D73FB8BD1F4F54EA4F54B632F3CC00ED627371C3ED8218410F917F411C5D2DB330FA63EAAA3541E1F79CCE891C71BF2183DC5E239A7FBF5B83FDAC5D1F9F35AB2C0DC22E81C7C96DFE7617FB0A041CF3E3BDF3EC5A3C802E57A3D8AADA30B500FE228BE9089090BEE9CFC7E63EE5BDE2EB63968AEECEF7AB1FD7D5D3F0F15F136D486389F48BA6370CF776506C8458B3C28C19E013E7A104755386C240E533E488EBFE49A26A05A8F42DC95CDEAED0373CEDB857FAA73C01E7680EF8554E819BB1E73F5DA51CBBD526C4D164759E561F91D55F4DC6C4BBD4100DB426C03A1EE5F757F159D6EF4F8FFFBA71D45C5880DDBEEC008B865481E1E2E04AF5E0DE74BD308A1AC04F85905F0B31DC02B1CBF2B6BECAE70DCAEAC317B1FAF1E1500BB6607A0D894BB661BA0BFA645EDF545C0B21D40063D1338F4B8DFB844847BD974F01A8B26FB8EEAEBEF7BEBF994C5AB3889D60A0B11E96261D5690680DA2DEDD0B7BBCD06D886C436BB9BDFEAEB8B3FA26C49962070B08B95B5AB1902EE71B47B6DC2E19A5ADB3D35105F8EBB4F441C0260AA51194306855D6FAF961FC91311B309353FDA40FAFB0A0EAC637FB780177DAD24AF2DBEC2B1BAD8680F170C55951AC7889DA930B88C9FE25C74F4F32D0EBA9402435468D562AF3925805CC364749054BEB5B735A42C686B6411692068AC22D84323358E61211D53C89839BCC79585DE3270B3CFFE8594BE3210011D008CDE30EBBBF0FCD4D28755350216059815866FB28BCA88E4048BCCCF16FBC8639C093B71FD93033E401243B1CD12330022FBFB642410AAFBD753183B900E82A8FA7818A3184E47EA9288F4E6314D0824305C830566247A22B22A677EB6799B56ECB24406C6FE6EC1E2D4CAAFBCB21C7F373F5A502C234F202CAEC1029EB53A1E6BE3EB0A4CF635FFEA1A942E761FFA6980ED2E5ADE4479FE479A2DA919269EA9E4560B1EA83F7C1FE58F029E5CCBD1716295EB8FAD55D8936595C00CF856F3FDF0CC3BDD7A3F545F2E52D1C9D3FE38297662CA147AE0271C9A2143A9007CCF1CF5318AA951F8E7BB2816FDD17C8BDDC982AE86E448637FB7F1A125D18A6C00B75CF7BBC53A2C49B48EFF94CEF6DDCF366BBA8C9FE2E52E5A8B2BDAFD6EC521DB789F005D06C835D9C02CC87A4D166565BBBB22DE8860C5D649E90FB610A50705A20067A8419410BE671552BDE110FDC7ED8F16F82C9FA264211EAEBA5F6D4E430F65F943F12854FF680EE7EA2B35A1A5A975BF4E4960BC056C32B0DC24C53E4C932D8925BB85F9B6214F8B434B4A19858D0467A3D7BD837294542455ECD28E5EFFD2FEDD1649AD0B94729553ABF9977550AB79E775B154B162E9BECBE9092512DD2FCB6AA577DF724A8A3D3FDEFD63FD76BD7F1ED274A07B7EFC40F2E2B7F47792BC3E7DF9FCC5CBD3938B751CE5FB32B6752DD6578B2A15449424695117B93528CEFAE2C7B2382B596ECEC4CFED4BBC9650F27CC95DFC3262C81EBAB0CAA6E77F256208D279C34D5493492C2E320ED059BD4F001F201C7E7E26E2768E4A5E39DDD7A771B98A22595F5143897C7D7DFA9FD547AF4E3EFC9F2FCD773F9C540F8B5F9D3C3FF9AFD3935F7765DDD0B254F043B4969F448883F3D3B44380FDB617122DE998F16D41B0255CF7601ED669E400685FC1B50F88B76CE9D67E80EABAAD7D8134155BCDE1B08A52279758460D33A994C54B2F3202AF56DBEA2F846AE4F279EC4D44D9294BAA834A85AD35C9DA7C18FB3192A7285B3C46D484B98EBE7E24C9AA787C7DFA2FCF59B074FE7A516B430EF660EF6397C514EAA6F218FEAF4DF4F57FDBE205D44BF53A6FDE2F088AB80D984B46AC4A71A78731F2B2DCF6C822CEABFDEB5FD5A0ED581B4C4B315BCE9E84BAE7D361B8EBFC2E1186BB4071F92FDCC180354D3DC82652C9D407E4B698A95F25279634F5812A53D3145CA283D642374065D07EF6AEA349EA5D1779D80FBBE2A25EB998AD33EA15705774D418AC111D980AA47E018B9548BD52A32B4BEA176C5DA3D43325D862A57E4177854BFDC2656A98FA055C07EE88400D8C88CF49FC8F1DA9BEAF636C7E38F990EF7F7D75F21B5DDDD2A060B0FBD91E3BB83E6A0F1B812F89EA9794F3DD9714454647D99F42B94CAA58440F660D178BE8F9A4C99534ED631DF3654C7B18C85D69C01E5335664E65215033EED43EAA07BEC1A2B2F51C2D8D66C7D8C2E7BDF8BB9B841D0ECD77FD848B0F44F72C166D50BA3B2373B1E8CEEA9A8B400FE3CB709583097A31FC28139CF920C0C6B64D074463D9FC648DDF00B63257D4D42BE47D9553BF863D54E7D45D949902A7C6226804B82B77DA07AEB16C6369CFCCE45B5FBB0BFD48950CC86277043B2B5F6D83F775CAB4657A9D25D3C16EFB13BF6FF7C32FCC54FA5D93494477C1B0FBDE3F86AEA6831D662F4EFECB99EE2F1C66C5B1E290137BD963622F9D2E7F0599B1BD10E03EEF770FCDA5F1F3EE9266B3F9B95F38CCF6ACAEAB246BB65328127C0CA8EDF59A9BC1CB8E81DB0FFD9C5D7C1E9CCCB00CACCD7C6D135265DE3E17005229DEE399F1BBB91B5324BA3C54E3F768C81E0DD9A3212B59005C8AACEFD0C253141B36D4849A4A9FC027067501C65584C2942C399EFBDABF8A01A86787A00460426AD08276BC12B49C54682D6831AF977DE6E5A20455574876EA4B2E60DD276295A931E3235A95AD81E3D76D7B18CA5F2EFD72DC007C6C0001D46DEF2DA11FCA2ECAD4D30E11403DFA53FB96C8FEE87242AA2B0779F565303584BCC26D9332FA850A941237D4C9C68A13A9C56DA6307BA9268C1B7D5F95BBCAE7FEDE7AFF758567A725A06B6D5B59ECEA835B7A209B0FFB99B56DDA53E75B7E88323F6A2EFCFF627FDFCD142BF78AEA4B5D6C822DAA40B25B6B07030F005B611B6C989AE9CEE61C5B2ADD73B4C9302E6BAE80BA57C85D2D75CF3EF6B6AEBAFB3963BEC63B5A917CB6F15903B09E8FC722C33CEB040B9D7B38E88205CE3DC0F5C6E19E64D04A508EAF31BBE08BE36B4C0C4C5787DCA3BCCC6E63C14B799B09CCD4BD3B87EC40D7848408AE5E303064364EF5915DE3F6AF2B3C450B0DE7C8F6ED733F0815E8EA173FAAC1F1D4E05171697C9F737726DB4972FF48AF037CB1700CA37A616D1B4D3A726ACE61FC5334B4823F2DA086DDDAE5E87B0C349EB421E927D678E261C387B89B589E96AD379F093CC108BADF788BB665AA0FF7D20753DC7566ABEA9AF2F1C71762E0DBABD1B599F56BB179A82D43BC5DF455130DD0E7BA0582E1E10D4C2D6A9E733A9469BFA87626BD1FBCBEA1A6ECEF9F76053D239087870B7FA03E7B0175E50BA72B5F18BD8F578FBE81F999E2AF6951DFAD7DA7EF635A53E3B8B11D37B6C3D8D83E65F12A4ECA82449E0CF2775EA1EDEB8DEF361B6E1F5A52D9DE940E1246CA5FFC7C7A52D600A1CD2F1D86A94BA55FFC11515A2E6D46FBD1C5C3156EB0D9AADB96C32BEFD88184EB94FFF56CAA4D2B6CF66AF9913C1120B16BAF39FE7D85C75BF72B2C107DAD44B129543E948269C611DE39781FC7478C6385E865FC14E769AF68AE56A9BE4D7DF343A341FB42B67A1E574568BA2741AC417871DD1A5B75AEFE2CE1F3C91873F687976332D0215337377AD3411E304138A66EEEEDBB498A68510C9291B30C848A2C53E31B45C03FC6D9003EAC3DAE48AAF9FE08F7076C2C6FB769B4BCDD25653DCDC3B0848DEA41BC7C6EBD30378F694206E1FD8F247A2243C45ADE926297254340BEA386FDA75DD1677FA2E33FF900134E679B1B7564B1CBE2EFABF040444537CFFF48B325B58BBA038F8B7DDF007A1FE58F4A51FB8BBD14CFD66B51D5183D5618F5CBB59378E4E4A1C228D5A18BB47300E94158B15D1EADEF8F7C77E43BC07289E23C277FBE8BE2CED1EC5A0996725EB2625C656E70AEA3245A914D6F38D4FC8CD6F19FDD11DC114CB28C9FE2E52E5AF785936FE37DD5A8FEA00AB25E9345B12BAF53E28D0D342BB5516CB6D971BF3AEA0D0044F5BEAC9F13F862F914258B7ECF82293BC4493F10575FA9AD6F35151B19EA13D2CB560836140A2B9173962011315BE72DFB752F461E55943D485119E1CFDDF1F651E117799E2EF625248175FA82E601B94A962765ED41B1DA7A8DD21D593F3C135AAEE98E106FD7F182A2F1FAF4C5A9C86B9F92BDD7F9E4A24A2D51E644CA1755094A91D5E8E04A6C9A940C12326D038FCB3F4943507E2759E9898AD6D444292B37D2959285234E16F1365A8394107A1BFAD1CAE9B570C5964BB22DEB8826053459A3F1D4FBE5593B8040701D39CECF183ED2B2D7D562933747DA5972183B01111FBEED60F88C9BD64C588D7B1ED33E42C0D94CAE48C2AD2DD02C329CB8BC1D70FEA90E0B576819866334C556903554841822B6071664087657BF4E35674FC533286464FDBBA700F31B490252F1C6E72801F8121EB804A8922B1F8A043079A8EBACB963F13F5B0E81E77FAEE5C8FFCD2A5BF03F5A6A021959975AFC10B91F8C35630DDBAA99B767F73F0531AC4716108038C8BA066756C5E38DE05C54E62CFA02A770EBBD94C6EAB34A9C8482DDB7F2A09F3F7B264377629440BAC620C7D36435B09CD7EAF028135C7B77D98D8EE6CB776FBE7C3F32C0A4A26E5F965428E5F8EEB345DC857C83C106D1C1ECCA32B000995F07617B3C1137B2603D1911293E810C06BC141A8D472C2E2C9CD863289B770BDF5670BF1F046BA1A9B2B1E1C6F71FB7D230F593D338CA29DC89C94E2D8D7C5A6213BC4E9D71B87CDC2C167CC3CCD907CF3A3E030EAA5F786A18A9ADCC007094D4666EF48FC91F22DA260B2765DDEA69140FC964FE710DCC9A469E20270E326549C90D24370EE505B25EEE699CD89CD8D9D89D338D3986D6D0233B669C24CC9F201CB66766A6621E5A04B0A295CE4794A91A25762791D1B87022EC3191D82B3E51BFC431C7C893BEAC798CBDD24A406DED6B43B0863CC88D2602733CCA59F1F47CCF722D8372251794BE734B7FF574D9D2DA251D9AAD34E55846E193F0A6DE68FC31A2B167C526D3D8EBB850E3096C7768B8B1DC78DCF474E54482E13B06B306576963B3E6788ACD9AC3C6D76D4DF10920DFBEEF100345F4555301830736E833B3F0110216CFCBE0020AE33288BFCBDE693142C0D0681B16183B2ABA59F6A0DBC718EB3FCE7661C50BE36F135DB0D0DD96DBE6F038456BB610E7E5E9D9C598EFF246E2AD39DA215DB990303B0D539E04E28A83D86FB0122CC870236F39908EA1A8CF45C384E7A471B5CB0C37AFAA368658C1619C40FB7D990E906FEBA68338F5A8CA9220234E25EE5EE09AF64F037D64BBB8071538EDB0E44103A8FFBEAA109BC8BBB7B0C7AB515FB98DB55DD9BE3E1B7FC392832A82D8C4BDC3546667213B46AF8C6C2803EC515F5554B6D9786148CE9EFF63C0E680970CDF4B481220164869ED508FCA6DBC99471118C0713AEE9C46E279B4F06EB05C0AD60E9123F30FE37B3978FE172A0D06721DF3D50DC57314D73673035959C9D19A89466109B8E85EB79C377C6D3E7635C5A639F1C88DA2E420B270B8813830C7E88B7D8EB697AA0A997A59EEA1F2384FE42C3F083B58AB9DA11D02FEF6D99139BDD61A6195E55478752C95E99FE9C6E2A2A0DEECA9B0CD387E6D47A69980C93E958793C8DB8BE393C9FEFE99A37352CBF5A325EA3E72FD54B8FE3BC8D0DD717DB0F0F131783B6C08B9399F8DED8A19F901EE58FC10F8E9ED2CDFDD8EF5E8762C9E18F199C19CE2C08577A541B68D1ECF7467B781B8BCDB1DFF452BC712CDB35C335755DD1B7255B54D73621501F7604EAA411E7C07E19FFDF05F408AB1FC5275E3F9A4FE2988C7DE995FFD5D9ED7F30DC6539E7939002FDDA6D1F276972424FB02116B42ECD461CAE1C0FE3C8C7E0ACC43CC8466C03F7764B1CB28B93D728FC2D4AD07E30DDDF6C743D122ED8C66B0FEF55EA48D4998C65E647FCD375F2EF27CF93742ADE9896F48E357960ECD522E85A5A7C05333AF5E8E73D841562E9F5BDD729EC5E6A0B44663A85114D6DCD4551EADEFE7ACAF3AFC25FE629B0E476331B39A91CA62D86C063A6B4CA61A436B59F2D404D456B1D966B3B6B39809483CC6B51D8EE662A73523D5C5B2DA0C74D7B88C3586F6B2E5ABF1D597655C4725AD506007DF108AC5C00800EEF7E17456C0B810764626C3B1F84D82B9A6AFAC46E2A4119494152705555057F49BB2846452D02F48D6F2D192BC8BB3BCB88C8AE83E026248CAAFEE48017B534F4FF61D544ECABBC523D944AF4F97F7295DFFE81EF4724AB733D2B0ACF5060DCAB62343B25D4C066C352E3C5EDB8C0ED7F6D08CD6C8A3344ED3008DD0B4E9600BB7E2F2184207702CA18F664C29C6431A54EA018D2A75D22E1A329A6A1043D86CE49B049F6D84C660DB757300C2D6E5F9009DC0B901FD74F3E413D0C853E5DBC1D9F25DB4037239B58001B9767840AE8BCD80FB02A1EA41F77DB403EFBB6906678231A54199366830A659CBAB4A65C937C3FC6AA12AB9A8637030A5A2E43AE8F58A8A55F86644A398338A5C48563922CE26722F9391154228B4A3631A0BA19C790A1C532FFB722F13BD0A5B125D13AA534D6C072EA10AAACFF071D80EA6D25DE7AFC445BCEEA094F3BA8F813A23ECC3795099713D3055C67532332BDA0007CCAC683B28CC8AB68F664C362A4B1A8F6D84C662DB75246D837F645AB64D2011DB56BD79C95E8BC146B481096D6340732E6D7044B6033624DBC7604CCE13050ECAF5C046E53AE98755A831AE15190E555ECCD10A3E2AB03EEA13A637706C40DDD92A87763B57CD7946E1561241607BEF193F5933427091365A5A60BD07270774846DA028CFA5F644E176C3EE29374010A4A76222E851613F17ADF1CF4103F7FF0A907A43F7439234332589F822F84048C2663BBED8158F1841807EE1C8019CD46A72A80E607DC9D158243A72C04F2E8067509CC043D6CE14A65D1E14BEB4B03573E73A7B9F0A0E833D16C970C0B34E5F6EB8258B781B9B1085E97A8022C21CD1DB5A0D75E67D88308AEE8AE9008E84FD6C140E020E82E4EFA83EC75D196E8430B3BE909E434E1FF08DB400FC195E5D49185C554A7D7CABC9E1579A75A328660A75F33D59C811557DAB7430F59C72ED0E379C39DF5B3915C857DFCC46E5801F9D24BA3D12EDEB791A2804697FD4BA027BF2887277447A86DA1C47111A4601690882A61E705772E34DDFE8C489F40CCA0FE0F14AEDBCEE4B12B12EB99632CA42E6C329D31148C3C7296909A3086BEA6B064D821C7AE5A1CBCEE2577B042301E777319417E537614406F5D768AFA53C9049CB2C26F5E6BDF24B4872A82AA4836731BCFBB00732C0EB8B5E25F62083C269A52A15EEC35F156A8A4A86D755C6F6C6E8434F5751D35979CC565780F637FDE06E7B0D776B2A147B71C84A17FADC84875BF75D5967D770D577524D5EEF6B1E8800703D5815190C2AC80EE567E4222F789AC2F114BD09C3943CD5D304AB8FEA6732A15DAF488D4EAD637EF03D21981B1EAF41095D79A39D7DABC8DE07753FA4E0EA2D9A11042FD138B02722A4B1ACAAC1674626ACBEDB70375B039B599A126D8654C10B7F0D1E2430EC668C54F042342DD073885B71C2C60B0A4A96286209FB4FBF8D18D4CF1F4955A448E3C4CC441DA03822410CAA30998A8CB2F2909F094E716F124AF958510B4CAA3A004F4D84544ACBCEBCBE8D37F36E0CB2185EA98C799F82F887BD3A87A1021D26640819A616960C1A471F5ED6C197AB2FDC54B5B743235C0D052681DEE719C6BD1F62DA489279F87603EAE99BDD7B5C157998BE9050DD800CAA14EC90B520BCD165AD05E4E5ED88E411F28503E4506514E7A7CF3D7FDE4F1B7CDCEC87680E9395135A03F3D564BDF63065F90154F5A5E2699383B213722F437A4E959ED9C334C5F7577BD586BDAD72665DC521529580D81FEBDA9F0E3CBCCAC197D52453AE87C9877D7F83A66AC50830D283249C1C831143C30941F820D8B4E1FC97C8DC0D92650EC205F20BCD961E8A7797BD09A266044D4A474F9C1072EA48524164FE26290807E106E0ED6C4B13D583D8FE445133842E519E278E083A7DF333AE615A3799236C5FE08A10C0039FEAB5723F42287940EC35C0FA7B9C6E999EAC04D126BF6ADBCECFF64FC0EB1FE89F7425A215A96ED7F3EAD7F3336A5A17F186ECFFBA2455504303E29CC24C48951DAD03DAF4F9903CA44DD62F01A3A64BD35C2FC03529A26554441759113F448BD2D1BC20791E27ABD393BF45EB1DED72B5B92FA3E53EED8AEDAE289D6B9BFB35C73B65EE30D5F8E76712CEE79FB6E55FB98F295034633A05F22979B38BD7CB16EF77D15A5C340C449994EC17427FDFAF6579914656DF5A48BFA68921A09A7C6D2EB5DFC866BBA6C0F24FC95DF4445C70FB9C938F64152DCA53FD53BC2C8F5C1810FD42F0643FBF8CA355166DF21A46F73DFD93F2F072F3F5DFFF3F29D55C41498F0300 , N'6.1.3-40302')
END

