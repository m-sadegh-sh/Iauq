using System.Data.Entity.Migrations;

namespace Iauq.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<IauqDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(IauqDbContext context)
        {
        }
    }
}