namespace Project.Migrations.ShareeArchivesDB
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScriptF : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        BrandId = c.Int(nullable: false, identity: true),
                        BrandName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.BrandId);
            
            CreateTable(
                "dbo.Sharees",
                c => new
                    {
                        ShareeId = c.Int(nullable: false, identity: true),
                        ShareeName = c.String(nullable: false, maxLength: 50),
                        FirstIntroduceOn = c.DateTime(nullable: false, storeType: "date"),
                        OnSale = c.Boolean(nullable: false),
                        Picture = c.String(),
                        ModelId = c.Int(nullable: false),
                        BrandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ShareeId)
                .ForeignKey("dbo.Brands", t => t.BrandId, cascadeDelete: true)
                .ForeignKey("dbo.Models", t => t.ModelId, cascadeDelete: true)
                .Index(t => t.ModelId)
                .Index(t => t.BrandId);
            
            CreateTable(
                "dbo.Models",
                c => new
                    {
                        ModelId = c.Int(nullable: false, identity: true),
                        ModelName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ModelId);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        StockId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        ShareeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StockId)
                .ForeignKey("dbo.Sharees", t => t.ShareeId, cascadeDelete: true)
                .Index(t => t.ShareeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stocks", "ShareeId", "dbo.Sharees");
            DropForeignKey("dbo.Sharees", "ModelId", "dbo.Models");
            DropForeignKey("dbo.Sharees", "BrandId", "dbo.Brands");
            DropIndex("dbo.Stocks", new[] { "ShareeId" });
            DropIndex("dbo.Sharees", new[] { "BrandId" });
            DropIndex("dbo.Sharees", new[] { "ModelId" });
            DropTable("dbo.Stocks");
            DropTable("dbo.Models");
            DropTable("dbo.Sharees");
            DropTable("dbo.Brands");
        }
    }
}
