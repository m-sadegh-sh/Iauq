using System.Data.Entity.Migrations;

namespace Iauq.Data.Migrations
{
    public partial class LogLevelAddedToLogs : DbMigration
    {
        public override void Up()
        {
            AddColumn("Logs", "LevelShort", c => c.Short(nullable: false));
        }

        public override void Down()
        {
            DropColumn("Logs", "LevelShort");
        }
    }
}