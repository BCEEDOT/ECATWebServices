namespace Ecat.Data.Contexts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init_ECAT : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CogEcmspeResult",
                c => new
                    {
                        PersonId = c.Int(nullable: false),
                        InstrumentId = c.Int(nullable: false),
                        Attempt = c.Int(nullable: false),
                        Accommodate = c.Double(nullable: false),
                        Avoid = c.Double(nullable: false),
                        Collaborate = c.Double(nullable: false),
                        Compete = c.Double(nullable: false),
                        Compromise = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.PersonId, t.InstrumentId, t.Attempt })
                .ForeignKey("dbo.CogInstrument", t => t.InstrumentId, cascadeDelete: true)
                .ForeignKey("dbo.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId)
                .Index(t => t.InstrumentId);
            
            CreateTable(
                "dbo.CogInstrument",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Version = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        CogInstructions = c.String(),
                        CogInstrumentType = c.String(maxLength: 50),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CogInventory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InstrumentId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        IsScored = c.Boolean(nullable: false),
                        IsDisplayed = c.Boolean(nullable: false),
                        AdaptiveDescription = c.String(),
                        InnovativeDescription = c.String(),
                        ItemType = c.String(maxLength: 50),
                        ItemDescription = c.String(),
                        IsReversed = c.Boolean(),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CogInstrument", t => t.InstrumentId, cascadeDelete: true)
                .Index(t => t.InstrumentId);
            
            CreateTable(
                "dbo.Person",
                c => new
                    {
                        PersonId = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        BbUserId = c.String(maxLength: 50),
                        BbUserName = c.String(maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        AvatarLocation = c.String(maxLength: 50),
                        GoByName = c.String(maxLength: 50),
                        Gender = c.String(nullable: false, maxLength: 50),
                        Affiliation = c.String(nullable: false, maxLength: 50),
                        Paygrade = c.String(nullable: false, maxLength: 50),
                        Component = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 80),
                        RegistrationComplete = c.Boolean(nullable: false),
                        InstituteRole = c.String(nullable: false, maxLength: 50),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.PersonId)
                .Index(t => t.Email, unique: true, name: "IX_UniqueEmailAddress");
            
            CreateTable(
                "dbo.ProfileFaculty",
                c => new
                    {
                        PersonId = c.Int(nullable: false),
                        Bio = c.String(),
                        HomeStation = c.String(maxLength: 50),
                        IsCourseAdmin = c.Boolean(nullable: false),
                        IsReportViewer = c.Boolean(nullable: false),
                        AcademyId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.PersonId)
                .ForeignKey("dbo.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.FacultyInCourse",
                c => new
                    {
                        FacultyPersonId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        BbCourseMemId = c.String(maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedById = c.Int(),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.FacultyPersonId, t.CourseId })
                .ForeignKey("dbo.Course", t => t.CourseId)
                .ForeignKey("dbo.ProfileFaculty", t => t.FacultyPersonId)
                .Index(t => t.FacultyPersonId)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.Course",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AcademyId = c.String(maxLength: 50),
                        BbCourseId = c.String(maxLength: 60),
                        Name = c.String(maxLength: 50),
                        ClassNumber = c.String(maxLength: 50),
                        Term = c.String(maxLength: 50),
                        GradReportPublished = c.Boolean(nullable: false),
                        StartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        GradDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.BbCourseId, unique: true, name: "IX_UniqueBbCourseId");
            
            CreateTable(
                "dbo.SpResponse",
                c => new
                    {
                        AssessorPersonId = c.Int(nullable: false),
                        AssesseePersonId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        InventoryItemId = c.Int(nullable: false),
                        ItemResponse = c.String(maxLength: 50),
                        ItemModelScore = c.Int(nullable: false),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.AssessorPersonId, t.AssesseePersonId, t.CourseId, t.WorkGroupId, t.InventoryItemId })
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.AssesseePersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.AssessorPersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.Course", t => t.CourseId)
                .ForeignKey("dbo.SpInventory", t => t.InventoryItemId)
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId)
                .Index(t => new { t.AssessorPersonId, t.CourseId, t.WorkGroupId })
                .Index(t => new { t.AssesseePersonId, t.CourseId, t.WorkGroupId })
                .Index(t => t.InventoryItemId);
            
            CreateTable(
                "dbo.CrseStudentInGroup",
                c => new
                    {
                        StudentId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        HasAcknowledged = c.Boolean(nullable: false),
                        BbCrseStudGroupId = c.String(maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedById = c.Int(),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.StudentId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.Course", t => t.CourseId)
                .ForeignKey("dbo.StudentInCourse", t => new { t.StudentId, t.CourseId }, cascadeDelete: true)
                .ForeignKey("dbo.ProfileStudent", t => t.StudentId)
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId)
                .Index(t => new { t.StudentId, t.CourseId })
                .Index(t => t.CourseId)
                .Index(t => t.WorkGroupId);
            
            CreateTable(
                "dbo.StratResponse",
                c => new
                    {
                        AssessorPersonId = c.Int(nullable: false),
                        AssesseePersonId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        StratPosition = c.Int(nullable: false),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.AssessorPersonId, t.AssesseePersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.AssesseePersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.AssessorPersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId, cascadeDelete: true)
                .Index(t => new { t.AssessorPersonId, t.CourseId, t.WorkGroupId })
                .Index(t => new { t.AssesseePersonId, t.CourseId, t.WorkGroupId });
            
            CreateTable(
                "dbo.StudSpComment",
                c => new
                    {
                        AuthorPersonId = c.Int(nullable: false),
                        RecipientPersonId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        FacultyPersonId = c.Int(),
                        RequestAnonymity = c.Boolean(nullable: false),
                        CommentText = c.String(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.AuthorPersonId, t.RecipientPersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.AuthorPersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.Course", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.RecipientPersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId)
                .Index(t => new { t.AuthorPersonId, t.CourseId, t.WorkGroupId })
                .Index(t => new { t.RecipientPersonId, t.CourseId, t.WorkGroupId });
            
            CreateTable(
                "dbo.StudSpCommentFlag",
                c => new
                    {
                        AuthorPersonId = c.Int(nullable: false),
                        RecipientPersonId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        Author = c.String(maxLength: 50),
                        Recipient = c.String(maxLength: 50),
                        Faculty = c.String(maxLength: 50),
                        FlaggedByFacultyId = c.Int(),
                    })
                .PrimaryKey(t => new { t.AuthorPersonId, t.RecipientPersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.StudSpComment", t => new { t.AuthorPersonId, t.RecipientPersonId, t.CourseId, t.WorkGroupId })
                .Index(t => new { t.AuthorPersonId, t.RecipientPersonId, t.CourseId, t.WorkGroupId });
            
            CreateTable(
                "dbo.WorkGroup",
                c => new
                    {
                        WorkGroupId = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        WgModelId = c.Int(nullable: false),
                        Category = c.String(maxLength: 4),
                        GroupNumber = c.String(maxLength: 6),
                        AssignedSpInstrId = c.Int(),
                        AssignedKcInstrId = c.Int(),
                        CustomName = c.String(maxLength: 50),
                        BbGroupId = c.String(maxLength: 50),
                        DefaultName = c.String(maxLength: 50),
                        SpStatus = c.String(maxLength: 50),
                        IsPrimary = c.Boolean(nullable: false),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.WorkGroupId)
                .ForeignKey("dbo.SpInstrument", t => t.AssignedSpInstrId)
                .ForeignKey("dbo.Course", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.WorkGroupModel", t => t.WgModelId, cascadeDelete: true)
                .Index(t => new { t.CourseId, t.GroupNumber, t.Category }, unique: true, name: "IX_UniqueCourseGroup")
                .Index(t => t.WgModelId)
                .Index(t => t.AssignedSpInstrId);
            
            CreateTable(
                "dbo.SpInstrument",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        Version = c.String(maxLength: 50),
                        StudentInstructions = c.String(),
                        FacultyInstructions = c.String(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        ModifiedById = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SpInventory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InstrumentId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        IsScored = c.Boolean(nullable: false),
                        IsDisplayed = c.Boolean(nullable: false),
                        Behavior = c.String(),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SpInstrument", t => t.InstrumentId, cascadeDelete: true)
                .Index(t => t.InstrumentId);
            
            CreateTable(
                "dbo.FacSpComment",
                c => new
                    {
                        RecipientPersonId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        FacultyPersonId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CommentText = c.String(),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.RecipientPersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.Course", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.FacultyInCourse", t => new { t.FacultyPersonId, t.CourseId })
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.RecipientPersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId)
                .Index(t => new { t.RecipientPersonId, t.CourseId, t.WorkGroupId })
                .Index(t => t.CourseId)
                .Index(t => new { t.FacultyPersonId, t.CourseId });
            
            CreateTable(
                "dbo.FacSpCommentFlag",
                c => new
                    {
                        RecipientPersonId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        FacultyId = c.Int(nullable: false),
                        Author = c.String(maxLength: 50),
                        Recipient = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new { t.RecipientPersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.FacSpComment", t => new { t.RecipientPersonId, t.CourseId, t.WorkGroupId })
                .Index(t => new { t.RecipientPersonId, t.CourseId, t.WorkGroupId });
            
            CreateTable(
                "dbo.FacSpResponse",
                c => new
                    {
                        AssesseePersonId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        InventoryItemId = c.Int(nullable: false),
                        FacultyPersonId = c.Int(nullable: false),
                        ItemResponse = c.String(maxLength: 50),
                        ItemModelScore = c.Single(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedById = c.Int(),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        CrseStudentInGroup_StudentId = c.Int(),
                        CrseStudentInGroup_CourseId = c.Int(),
                        CrseStudentInGroup_WorkGroupId = c.Int(),
                    })
                .PrimaryKey(t => new { t.AssesseePersonId, t.CourseId, t.WorkGroupId, t.InventoryItemId })
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.AssesseePersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.FacultyInCourse", t => new { t.FacultyPersonId, t.CourseId })
                .ForeignKey("dbo.SpInventory", t => t.InventoryItemId)
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId)
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.CrseStudentInGroup_StudentId, t.CrseStudentInGroup_CourseId, t.CrseStudentInGroup_WorkGroupId })
                .Index(t => new { t.AssesseePersonId, t.CourseId, t.WorkGroupId })
                .Index(t => new { t.FacultyPersonId, t.CourseId })
                .Index(t => t.InventoryItemId)
                .Index(t => new { t.CrseStudentInGroup_StudentId, t.CrseStudentInGroup_CourseId, t.CrseStudentInGroup_WorkGroupId });
            
            CreateTable(
                "dbo.FacStratResponse",
                c => new
                    {
                        AssesseePersonId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        StratPosition = c.Int(nullable: false),
                        StratResultId = c.Int(),
                        FacultyPersonId = c.Int(nullable: false),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.AssesseePersonId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.FacultyInCourse", t => new { t.FacultyPersonId, t.CourseId })
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId)
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.AssesseePersonId, t.CourseId, t.WorkGroupId })
                .Index(t => new { t.AssesseePersonId, t.CourseId, t.WorkGroupId })
                .Index(t => new { t.FacultyPersonId, t.CourseId })
                .Index(t => t.WorkGroupId);
            
            CreateTable(
                "dbo.SpResult",
                c => new
                    {
                        StudentId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        AssignedInstrumentId = c.Int(nullable: false),
                        AssessResult = c.String(maxLength: 50),
                        CompositeScore = c.Int(nullable: false),
                        BreakOut_IneffA = c.Int(nullable: false),
                        BreakOut_IneffU = c.Int(nullable: false),
                        BreakOut_EffA = c.Int(nullable: false),
                        BreakOut_EffU = c.Int(nullable: false),
                        BreakOut_HighEffU = c.Int(nullable: false),
                        BreakOut_HighEffA = c.Int(nullable: false),
                        BreakOut_NotDisplay = c.Int(nullable: false),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.StudentId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.SpInstrument", t => t.AssignedInstrumentId)
                .ForeignKey("dbo.Course", t => t.CourseId)
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId)
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.StudentId, t.CourseId, t.WorkGroupId })
                .Index(t => new { t.StudentId, t.CourseId, t.WorkGroupId })
                .Index(t => t.CourseId)
                .Index(t => t.WorkGroupId)
                .Index(t => t.AssignedInstrumentId);
            
            CreateTable(
                "dbo.StratResult",
                c => new
                    {
                        StudentId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        WorkGroupId = c.Int(nullable: false),
                        OriginalStratPosition = c.Int(nullable: false),
                        FinalStratPosition = c.Int(nullable: false),
                        StratCummScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StudStratAwardedScore = c.Decimal(nullable: false, precision: 18, scale: 3),
                        FacStratAwardedScore = c.Decimal(nullable: false, precision: 18, scale: 3),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.StudentId, t.CourseId, t.WorkGroupId })
                .ForeignKey("dbo.Course", t => t.CourseId)
                .ForeignKey("dbo.WorkGroup", t => t.WorkGroupId, cascadeDelete: true)
                .ForeignKey("dbo.CrseStudentInGroup", t => new { t.StudentId, t.CourseId, t.WorkGroupId })
                .Index(t => new { t.StudentId, t.CourseId, t.WorkGroupId })
                .Index(t => t.CourseId)
                .Index(t => t.WorkGroupId);
            
            CreateTable(
                "dbo.WorkGroupModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        AssignedSpInstrId = c.Int(),
                        EdLevel = c.String(maxLength: 50),
                        WgCategory = c.String(maxLength: 50),
                        MaxStratStudent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxStratFaculty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        StratDivisor = c.Int(nullable: false),
                        StudStratCol = c.String(maxLength: 50),
                        FacStratCol = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SpInstrument", t => t.AssignedSpInstrId)
                .Index(t => t.AssignedSpInstrId);
            
            CreateTable(
                "dbo.StudentInCourse",
                c => new
                    {
                        StudentPersonId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        BbCourseMemId = c.String(maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedById = c.Int(),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.StudentPersonId, t.CourseId })
                .ForeignKey("dbo.Course", t => t.CourseId)
                .ForeignKey("dbo.ProfileStudent", t => t.StudentPersonId)
                .Index(t => t.StudentPersonId)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.ProfileStudent",
                c => new
                    {
                        PersonId = c.Int(nullable: false),
                        Bio = c.String(),
                        HomeStation = c.String(maxLength: 50),
                        ContactNumber = c.String(maxLength: 50),
                        Commander = c.String(maxLength: 50),
                        Shirt = c.String(maxLength: 50),
                        CommanderEmail = c.String(maxLength: 50),
                        ShirtEmail = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.PersonId)
                .ForeignKey("dbo.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.RoadRunner",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Location = c.String(maxLength: 200),
                        PhoneNumber = c.String(maxLength: 50),
                        LeaveDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ReturnDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        SignOut = c.Boolean(nullable: false),
                        PrevSignOut = c.Boolean(nullable: false),
                        PersonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.Security",
                c => new
                    {
                        PersonId = c.Int(nullable: false),
                        BadPasswordCount = c.Int(nullable: false),
                        PasswordHash = c.String(maxLength: 400),
                        ModifiedById = c.Int(),
                        ModifiedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.PersonId)
                .ForeignKey("dbo.Person", t => t.PersonId)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.CogEcpeResult",
                c => new
                    {
                        PersonId = c.Int(nullable: false),
                        InstrumentId = c.Int(nullable: false),
                        Attempt = c.Int(nullable: false),
                        Outcome = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PersonId, t.InstrumentId, t.Attempt })
                .ForeignKey("dbo.CogInstrument", t => t.InstrumentId, cascadeDelete: true)
                .ForeignKey("dbo.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId)
                .Index(t => t.InstrumentId);
            
            CreateTable(
                "dbo.CogEsalbResult",
                c => new
                    {
                        PersonId = c.Int(nullable: false),
                        InstrumentId = c.Int(nullable: false),
                        Attempt = c.Int(nullable: false),
                        LaissezFaire = c.Double(nullable: false),
                        Contingent = c.Double(nullable: false),
                        Management = c.Double(nullable: false),
                        Idealized = c.Double(nullable: false),
                        Individual = c.Double(nullable: false),
                        Inspirational = c.Double(nullable: false),
                        IntellectualStim = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.PersonId, t.InstrumentId, t.Attempt })
                .ForeignKey("dbo.CogInstrument", t => t.InstrumentId, cascadeDelete: true)
                .ForeignKey("dbo.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId)
                .Index(t => t.InstrumentId);
            
            CreateTable(
                "dbo.CogEtmpreResult",
                c => new
                    {
                        PersonId = c.Int(nullable: false),
                        InstrumentId = c.Int(nullable: false),
                        Attempt = c.Int(nullable: false),
                        Creator = c.Int(nullable: false),
                        Advancer = c.Int(nullable: false),
                        Refiner = c.Int(nullable: false),
                        Executor = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PersonId, t.InstrumentId, t.Attempt })
                .ForeignKey("dbo.CogInstrument", t => t.InstrumentId, cascadeDelete: true)
                .ForeignKey("dbo.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId)
                .Index(t => t.InstrumentId);
            
            CreateTable(
                "dbo.CogResponse",
                c => new
                    {
                        CogInventoryId = c.Int(nullable: false),
                        PersonId = c.Int(nullable: false),
                        Attempt = c.Int(nullable: false),
                        ItemScore = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.CogInventoryId, t.PersonId, t.Attempt })
                .ForeignKey("dbo.CogInventory", t => t.CogInventoryId, cascadeDelete: true)
                .ForeignKey("dbo.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.CogInventoryId)
                .Index(t => t.PersonId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CogResponse", "PersonId", "dbo.Person");
            DropForeignKey("dbo.CogResponse", "CogInventoryId", "dbo.CogInventory");
            DropForeignKey("dbo.CogEtmpreResult", "PersonId", "dbo.Person");
            DropForeignKey("dbo.CogEtmpreResult", "InstrumentId", "dbo.CogInstrument");
            DropForeignKey("dbo.CogEsalbResult", "PersonId", "dbo.Person");
            DropForeignKey("dbo.CogEsalbResult", "InstrumentId", "dbo.CogInstrument");
            DropForeignKey("dbo.CogEcpeResult", "PersonId", "dbo.Person");
            DropForeignKey("dbo.CogEcpeResult", "InstrumentId", "dbo.CogInstrument");
            DropForeignKey("dbo.CogEcmspeResult", "PersonId", "dbo.Person");
            DropForeignKey("dbo.ProfileStudent", "PersonId", "dbo.Person");
            DropForeignKey("dbo.Security", "PersonId", "dbo.Person");
            DropForeignKey("dbo.RoadRunner", "PersonId", "dbo.Person");
            DropForeignKey("dbo.ProfileFaculty", "PersonId", "dbo.Person");
            DropForeignKey("dbo.FacultyInCourse", "FacultyPersonId", "dbo.ProfileFaculty");
            DropForeignKey("dbo.FacultyInCourse", "CourseId", "dbo.Course");
            DropForeignKey("dbo.SpResponse", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.SpResponse", "InventoryItemId", "dbo.SpInventory");
            DropForeignKey("dbo.SpResponse", "CourseId", "dbo.Course");
            DropForeignKey("dbo.SpResponse", new[] { "AssessorPersonId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.SpResponse", new[] { "AssesseePersonId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.CrseStudentInGroup", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.CrseStudentInGroup", "StudentId", "dbo.ProfileStudent");
            DropForeignKey("dbo.CrseStudentInGroup", new[] { "StudentId", "CourseId" }, "dbo.StudentInCourse");
            DropForeignKey("dbo.StudentInCourse", "StudentPersonId", "dbo.ProfileStudent");
            DropForeignKey("dbo.StudentInCourse", "CourseId", "dbo.Course");
            DropForeignKey("dbo.StratResult", new[] { "StudentId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.SpResult", new[] { "StudentId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.FacStratResponse", new[] { "AssesseePersonId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.FacSpResponse", new[] { "CrseStudentInGroup_StudentId", "CrseStudentInGroup_CourseId", "CrseStudentInGroup_WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.CrseStudentInGroup", "CourseId", "dbo.Course");
            DropForeignKey("dbo.StudSpComment", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.WorkGroup", "WgModelId", "dbo.WorkGroupModel");
            DropForeignKey("dbo.WorkGroupModel", "AssignedSpInstrId", "dbo.SpInstrument");
            DropForeignKey("dbo.StratResult", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.StratResult", "CourseId", "dbo.Course");
            DropForeignKey("dbo.StratResponse", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.SpResult", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.SpResult", "CourseId", "dbo.Course");
            DropForeignKey("dbo.SpResult", "AssignedInstrumentId", "dbo.SpInstrument");
            DropForeignKey("dbo.FacStratResponse", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.FacStratResponse", new[] { "FacultyPersonId", "CourseId" }, "dbo.FacultyInCourse");
            DropForeignKey("dbo.FacSpResponse", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.FacSpResponse", "InventoryItemId", "dbo.SpInventory");
            DropForeignKey("dbo.FacSpResponse", new[] { "FacultyPersonId", "CourseId" }, "dbo.FacultyInCourse");
            DropForeignKey("dbo.FacSpResponse", new[] { "AssesseePersonId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.FacSpComment", "WorkGroupId", "dbo.WorkGroup");
            DropForeignKey("dbo.FacSpComment", new[] { "RecipientPersonId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.FacSpCommentFlag", new[] { "RecipientPersonId", "CourseId", "WorkGroupId" }, "dbo.FacSpComment");
            DropForeignKey("dbo.FacSpComment", new[] { "FacultyPersonId", "CourseId" }, "dbo.FacultyInCourse");
            DropForeignKey("dbo.FacSpComment", "CourseId", "dbo.Course");
            DropForeignKey("dbo.WorkGroup", "CourseId", "dbo.Course");
            DropForeignKey("dbo.SpInventory", "InstrumentId", "dbo.SpInstrument");
            DropForeignKey("dbo.WorkGroup", "AssignedSpInstrId", "dbo.SpInstrument");
            DropForeignKey("dbo.StudSpComment", new[] { "RecipientPersonId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.StudSpCommentFlag", new[] { "AuthorPersonId", "RecipientPersonId", "CourseId", "WorkGroupId" }, "dbo.StudSpComment");
            DropForeignKey("dbo.StudSpComment", "CourseId", "dbo.Course");
            DropForeignKey("dbo.StudSpComment", new[] { "AuthorPersonId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.StratResponse", new[] { "AssessorPersonId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.StratResponse", new[] { "AssesseePersonId", "CourseId", "WorkGroupId" }, "dbo.CrseStudentInGroup");
            DropForeignKey("dbo.CogEcmspeResult", "InstrumentId", "dbo.CogInstrument");
            DropForeignKey("dbo.CogInventory", "InstrumentId", "dbo.CogInstrument");
            DropIndex("dbo.CogResponse", new[] { "PersonId" });
            DropIndex("dbo.CogResponse", new[] { "CogInventoryId" });
            DropIndex("dbo.CogEtmpreResult", new[] { "InstrumentId" });
            DropIndex("dbo.CogEtmpreResult", new[] { "PersonId" });
            DropIndex("dbo.CogEsalbResult", new[] { "InstrumentId" });
            DropIndex("dbo.CogEsalbResult", new[] { "PersonId" });
            DropIndex("dbo.CogEcpeResult", new[] { "InstrumentId" });
            DropIndex("dbo.CogEcpeResult", new[] { "PersonId" });
            DropIndex("dbo.Security", new[] { "PersonId" });
            DropIndex("dbo.RoadRunner", new[] { "PersonId" });
            DropIndex("dbo.ProfileStudent", new[] { "PersonId" });
            DropIndex("dbo.StudentInCourse", new[] { "CourseId" });
            DropIndex("dbo.StudentInCourse", new[] { "StudentPersonId" });
            DropIndex("dbo.WorkGroupModel", new[] { "AssignedSpInstrId" });
            DropIndex("dbo.StratResult", new[] { "WorkGroupId" });
            DropIndex("dbo.StratResult", new[] { "CourseId" });
            DropIndex("dbo.StratResult", new[] { "StudentId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.SpResult", new[] { "AssignedInstrumentId" });
            DropIndex("dbo.SpResult", new[] { "WorkGroupId" });
            DropIndex("dbo.SpResult", new[] { "CourseId" });
            DropIndex("dbo.SpResult", new[] { "StudentId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.FacStratResponse", new[] { "WorkGroupId" });
            DropIndex("dbo.FacStratResponse", new[] { "FacultyPersonId", "CourseId" });
            DropIndex("dbo.FacStratResponse", new[] { "AssesseePersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.FacSpResponse", new[] { "CrseStudentInGroup_StudentId", "CrseStudentInGroup_CourseId", "CrseStudentInGroup_WorkGroupId" });
            DropIndex("dbo.FacSpResponse", new[] { "InventoryItemId" });
            DropIndex("dbo.FacSpResponse", new[] { "FacultyPersonId", "CourseId" });
            DropIndex("dbo.FacSpResponse", new[] { "AssesseePersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.FacSpCommentFlag", new[] { "RecipientPersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.FacSpComment", new[] { "FacultyPersonId", "CourseId" });
            DropIndex("dbo.FacSpComment", new[] { "CourseId" });
            DropIndex("dbo.FacSpComment", new[] { "RecipientPersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.SpInventory", new[] { "InstrumentId" });
            DropIndex("dbo.WorkGroup", new[] { "AssignedSpInstrId" });
            DropIndex("dbo.WorkGroup", new[] { "WgModelId" });
            DropIndex("dbo.WorkGroup", "IX_UniqueCourseGroup");
            DropIndex("dbo.StudSpCommentFlag", new[] { "AuthorPersonId", "RecipientPersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.StudSpComment", new[] { "RecipientPersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.StudSpComment", new[] { "AuthorPersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.StratResponse", new[] { "AssesseePersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.StratResponse", new[] { "AssessorPersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.CrseStudentInGroup", new[] { "WorkGroupId" });
            DropIndex("dbo.CrseStudentInGroup", new[] { "CourseId" });
            DropIndex("dbo.CrseStudentInGroup", new[] { "StudentId", "CourseId" });
            DropIndex("dbo.SpResponse", new[] { "InventoryItemId" });
            DropIndex("dbo.SpResponse", new[] { "AssesseePersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.SpResponse", new[] { "AssessorPersonId", "CourseId", "WorkGroupId" });
            DropIndex("dbo.Course", "IX_UniqueBbCourseId");
            DropIndex("dbo.FacultyInCourse", new[] { "CourseId" });
            DropIndex("dbo.FacultyInCourse", new[] { "FacultyPersonId" });
            DropIndex("dbo.ProfileFaculty", new[] { "PersonId" });
            DropIndex("dbo.Person", "IX_UniqueEmailAddress");
            DropIndex("dbo.CogInventory", new[] { "InstrumentId" });
            DropIndex("dbo.CogEcmspeResult", new[] { "InstrumentId" });
            DropIndex("dbo.CogEcmspeResult", new[] { "PersonId" });
            DropTable("dbo.CogResponse");
            DropTable("dbo.CogEtmpreResult");
            DropTable("dbo.CogEsalbResult");
            DropTable("dbo.CogEcpeResult");
            DropTable("dbo.Security");
            DropTable("dbo.RoadRunner");
            DropTable("dbo.ProfileStudent");
            DropTable("dbo.StudentInCourse");
            DropTable("dbo.WorkGroupModel");
            DropTable("dbo.StratResult");
            DropTable("dbo.SpResult");
            DropTable("dbo.FacStratResponse");
            DropTable("dbo.FacSpResponse");
            DropTable("dbo.FacSpCommentFlag");
            DropTable("dbo.FacSpComment");
            DropTable("dbo.SpInventory");
            DropTable("dbo.SpInstrument");
            DropTable("dbo.WorkGroup");
            DropTable("dbo.StudSpCommentFlag");
            DropTable("dbo.StudSpComment");
            DropTable("dbo.StratResponse");
            DropTable("dbo.CrseStudentInGroup");
            DropTable("dbo.SpResponse");
            DropTable("dbo.Course");
            DropTable("dbo.FacultyInCourse");
            DropTable("dbo.ProfileFaculty");
            DropTable("dbo.Person");
            DropTable("dbo.CogInventory");
            DropTable("dbo.CogInstrument");
            DropTable("dbo.CogEcmspeResult");
        }
    }
}
