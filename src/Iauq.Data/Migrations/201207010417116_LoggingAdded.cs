using System.Data.Entity.Migrations;

namespace Iauq.Data.Migrations
{
    public partial class LoggingAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Logs",
                c => new
                         {
                             Id = c.Int(nullable: false, identity: true),
                             Message = c.String(unicode: true, isMaxLength: true),
                             Stack = c.String(isMaxLength: true),
                             ExceptionType = c.String(maxLength: 1024)
                         })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("Logs");
        }
    }
}