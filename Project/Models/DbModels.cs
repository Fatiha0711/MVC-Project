using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public enum Category { Bridal=1, Party, Occasional, RegularWear, Seasonal}
    public class Brand
    {
        public int BrandId { get; set; }
        [Required, StringLength(50), Display(Name = "Brand Name")]
        public string BrandName { get; set; }
        public virtual ICollection<Sharee> Sharees { get; set; } = new List<Sharee>();
    }
    public class Model
    {
        public int ModelId { get; set; }
        [Required, StringLength(50), Display(Name = "Model Name")]
        public string ModelName { get; set; }
        public virtual ICollection<Sharee> Sharees { get; set; } = new List<Sharee>();

    }
    public class Sharee
    {
        public int ShareeId { get; set; }
        [Required, StringLength(50)]
        public string ShareeName { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = "First Introduce"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FirstIntroduceOn { get; set; }
        public bool OnSale { get; set; }
        public string Picture { get; set; }
        //fk
        [ForeignKey("Model")]
        public int ModelId { get; set; }
        [ForeignKey("Brand")]
        public int BrandId { get; set; }
        //nev
        public virtual Model Model { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    }
    public class Stock
    {
        public int StockId { get; set; }
        public Category Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [Required, ForeignKey("Sharee")]
        public int ShareeId { get; set; }
        //
        public virtual Sharee Sharee { get; set; }
    }
    public class ShareeArchivesDbContext : DbContext
    {

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Sharee> Sharees { get; set; }
        public DbSet<Stock> Stocks { get; set; }
    }
}