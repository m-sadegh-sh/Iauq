namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AnswerTableIpAddressLengthUpdatedAndSecurityTokenAddedToUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Answers", "IpAddress", c => c.String(maxLength: 32));
            AddColumn("Users", "SecurityToken", c => c.String(nullable: false, maxLength: 32, fixedLength: true));
        }
        
        public override void Down()
        {
            AlterColumn("Answers", "IpAddress", c => c.String());
            DropColumn("Users", "SecurityToken");
        }
    }
}
