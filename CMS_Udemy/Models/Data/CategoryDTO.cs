using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMS_Udemy.Models.Data
{
    [Table("tblCategories")]
    public class CategoryDTO
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int Sorting { get; set; }
        public string Slug { get; set; }
    }
}