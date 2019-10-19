namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FilesReUpdated : DbMigration
    {
        public override void Up()
        {
            DropColumn("Files", "CreateDate");
        }
        
        public override void Down()
        {
            AddColumn("Files", "CreateDate", c => c.DateTime(nullable: false));
        }
    }
}
