using System.Data.Entity.Migrations;

namespace Iauq.Data.Migrations
{
    public partial class LoggingUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("Logs", "RequestUrl", c => c.String(isMaxLength: true, unicode: true));
            AddColumn("Logs", "ReferUrl", c => c.String(isMaxLength: true, unicode: true));
            AddColumn("Logs", "IpAddress", c => c.String(maxLength: 32));
            AddColumn("Logs", "LogDate", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
            DropColumn("Logs", "RequestUrl");
            DropColumn("Logs", "ReferUrl");
            DropColumn("Logs", "IpAddress");
            DropColumn("Logs", "LogDate");
        }
    }
}