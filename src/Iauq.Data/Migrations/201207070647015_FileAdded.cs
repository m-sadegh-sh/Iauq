namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FileAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Files",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsPublished = c.Boolean(nullable: false),
                        AccessModeShort = c.Short(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        CreateDateTicks = c.Long(nullable: false),
                        AccessCount = c.Long(nullable: false),
                        Name = c.String(),
                        ContentType = c.String(),
                        Size = c.Long(nullable: false),
                        UploaderId = c.Int(nullable: false),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Files", t => t.ParentId)
                .ForeignKey("Users", t => t.UploaderId)
                .Index(t => t.ParentId)
                .Index(t => t.UploaderId);
            
        }
        
        public override void Down()
        {
            DropIndex("Files", new[] { "UploaderId" });
            DropIndex("Files", new[] { "ParentId" });
            DropForeignKey("Files", "UploaderId", "Users");
            DropForeignKey("Files", "ParentId", "Files");
            DropTable("Files");
        }
    }
}
