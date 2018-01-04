using CMS_Udemy.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS_Udemy.Models.ViewModels.Shop
{
   
    public class CategoryVM
    {
        public CategoryVM()
        {

        }
        public CategoryVM(CategoryDTO row)
        {
            ID = row.ID;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;

        }
        public int ID { get; set; }
        public string Name { get; set; }
        public int Sorting { get; set; }
        public string Slug { get; set; }
        
    }
}