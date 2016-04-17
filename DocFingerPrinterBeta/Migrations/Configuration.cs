namespace DocFingerPrinterBeta.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    /// <summary>
    /// class that handles database migrations/seeding
    /// </summary>
    internal sealed class Configuration : DbMigrationsConfiguration<DocFingerPrinterBeta.DAL.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "DocFingerPrinterBeta.DAL.ApplicationDbContext";
        }

        protected override void Seed(DocFingerPrinterBeta.DAL.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Roles.AddOrUpdate(
                r => r.Id,
                new Role { Name = "Admin", Id = 1},
                new Role { Name = "User", Id = 2 }
            );
        }
    }
}
