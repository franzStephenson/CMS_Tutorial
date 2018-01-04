using CMS_Udemy.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS_Udemy.Models.ViewModels.Shop
{
    public class ProductsVM
    {
        public ProductsVM()
        {

        }
        
        public ProductsVM(ProductsDTO row)
        {
            ID = row.ID;
            Name = row.Name;
            Slug = row.Slug;
            Description = row.Description;
            Price = row.Price;
            CategoryName = row.CategoryName;
            CategoryID = row.CategoryID;
            ImageName = row.ImageName;
        }
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        [Required]
        public int CategoryID { get; set; }
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}