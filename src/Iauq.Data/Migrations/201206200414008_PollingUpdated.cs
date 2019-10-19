namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PollingUpdated : DbMigration
    {
        public override void Up()
        {
            AlterColumn("ChoiceItems", "Text", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("Choices", "Title", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("Polls", "Title", c => c.String(nullable: false, maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("Polls", "Title", c => c.String());
            AlterColumn("Choices", "Title", c => c.String());
            AlterColumn("ChoiceItems", "Text", c => c.String());
        }
    }
}
