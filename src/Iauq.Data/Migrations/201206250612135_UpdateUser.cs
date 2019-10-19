namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Users", "SecurityToken", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            AlterColumn("Users", "SecurityToken", c => c.String(nullable: false, maxLength: 32, fixedLength: true));
        }
    }
}
