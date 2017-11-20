namespace Ecat.Data.Contexts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CanvasLogins : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CanvasLogin",
                c => new
                    {
                        PersonId = c.Int(nullable: false),
                        AccessToken = c.String(maxLength: 100),
                        RefreshToken = c.String(maxLength: 100),
                        TokenExpires = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.PersonId)
                .ForeignKey("dbo.ProfileFaculty", t => t.PersonId)
                .Index(t => t.PersonId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CanvasLogin", "PersonId", "dbo.ProfileFaculty");
            DropIndex("dbo.CanvasLogin", new[] { "PersonId" });
            DropTable("dbo.CanvasLogin");
        }
    }
}
