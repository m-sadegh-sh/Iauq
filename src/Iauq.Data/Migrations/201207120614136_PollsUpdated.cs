namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PollsUpdated : DbMigration
    {
        public override void Up()
        {
            //AddColumn("Files", "Guid", c => c.Guid(nullable: false));
            AddColumn("Polls", "ShowOnHomePage", c => c.Boolean(nullable: false));
            //AddColumn("Polls", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("Polls", "CreateDateTicks", c => c.Long(nullable: false));
            DropColumn("Polls", "StartActiveTimeTicks");
            DropColumn("Polls", "EndActiveTimeTicks");
        }
        
        public override void Down()
        {
            AddColumn("Polls", "EndActiveTimeTicks", c => c.Long());
            AddColumn("Polls", "StartActiveTimeTicks", c => c.Long());
            DropColumn("Polls", "CreateDateTicks");
            //DropColumn("Polls", "CreateDate");
            DropColumn("Polls", "ShowOnHomePage");
            //DropColumn("Files", "Guid");
        }
    }
}
