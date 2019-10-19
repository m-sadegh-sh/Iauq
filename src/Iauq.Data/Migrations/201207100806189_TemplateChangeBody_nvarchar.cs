namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class TemplateChangeBody_nvarchar : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Templates", "Body", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("Templates", "Body", c => c.String(unicode: false, storeType: "text"));
        }
    }
}
