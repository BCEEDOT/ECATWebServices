namespace Ecat.Data.Contexts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserModels : DbMigration
    {
        public override void Up()
        {
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
                "dbo.Person",
                c => new
                    {
                        PersonId = c.Int(nullable: false, identity: true),
                        LmsUserId = c.String(maxLength: 50),
                        LmsUserName = c.String(maxLength: 50),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProfileStudent", "PersonId", "dbo.Person");
            DropForeignKey("dbo.Security", "PersonId", "dbo.Person");
            DropForeignKey("dbo.ProfileFaculty", "PersonId", "dbo.Person");
            DropIndex("dbo.ProfileStudent", new[] { "PersonId" });
            DropIndex("dbo.Security", new[] { "PersonId" });
            DropIndex("dbo.Person", "IX_UniqueEmailAddress");
            DropIndex("dbo.ProfileFaculty", new[] { "PersonId" });
            DropTable("dbo.ProfileStudent");
            DropTable("dbo.Security");
            DropTable("dbo.Person");
            DropTable("dbo.ProfileFaculty");
        }
    }
}
