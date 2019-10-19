namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class TemplateAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Templates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Body = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Templates");
        }
    }
}
