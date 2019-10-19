namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class GuidAddedToFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("Files", "Guid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Files", "Guid");
        }
    }
}
