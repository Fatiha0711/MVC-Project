namespace Project.Migrations.ShareeArchivesDB
{
    using Project.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Project.Models.ShareeArchivesDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\ShareeArchivesDB";
        }

        protected override void Seed(Project.Models.ShareeArchivesDbContext db)
        {
            db.Brands.AddRange(new Brand[]
            {
                new Brand{BrandName="Arong"},
                new Brand{BrandName="DorjiBari"},
                new Brand{BrandName="Style Echo"}
            });
            db.Models.AddRange(new Model[]
            {
                new Model{ModelName="Benarashi"},
                new Model{ModelName="Katan"},
                new Model{ModelName="Jamdani"}
            });
            db.SaveChanges();
            Sharee s = new Sharee
            {
                ShareeName = "Dhakai Jamdani",
                ModelId = 1,
                BrandId = 1,
                FirstIntroduceOn = new DateTime(2024, 1, 1),
                OnSale = true,
                Picture = "1.jpg"
            };
            s.Stocks.Add(new Stock { Category = Category.Occasional, Quantity = 10, Price = 120 });
            s.Stocks.Add(new Stock { Category = Category.Bridal, Quantity = 15, Price = 220 });
            s.Stocks.Add(new Stock { Category = Category.Party, Quantity = 20, Price = 180 });
            db.Sharees.Add(s);
            db.SaveChanges();
        }
    }
}
