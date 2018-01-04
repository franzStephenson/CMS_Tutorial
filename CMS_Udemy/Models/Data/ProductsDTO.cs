using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMS_Udemy.Models.Data
{
    [Table("tblProducts")]
    public class ProductsDTO
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public string ImageName { get; set; }

        [ForeignKey("CategoryID")]
        public virtual CategoryDTO Category { get; set; }
    }
}