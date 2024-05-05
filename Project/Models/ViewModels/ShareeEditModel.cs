using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models.ViewModels
{
    public class ShareeEditModel
    {
        public int ShareeId { get; set; }
        [Required, StringLength(50)]
        public string ShareeName { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = "First Introduce"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FirstIntroduceOn { get; set; }
        public bool OnSale { get; set; }
        public HttpPostedFileBase Picture { get; set; }
        public int ModelId { get; set; }
        public int BrandId { get; set; }
        public virtual List<Stock> Stocks { get; set; } = new List<Stock>();
    }
}