namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PollBodyRenamedToDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("Polls", "Description", c => c.String());
            DropColumn("Polls", "Body");
        }
        
        public override void Down()
        {
            AddColumn("Polls", "Body", c => c.String());
            DropColumn("Polls", "Description");
        }
    }
}
