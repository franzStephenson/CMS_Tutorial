using CMS_Udemy.Models.Data;
using CMS_Udemy.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS_Udemy.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            //Declare list of CategoryVM
            List<CategoryVM> categoryVMList;

            //Init the list
            using(db DB = new db())
            {
                categoryVMList = DB.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            return PartialView(categoryVMList);
        }

        //GET: /shop/category/name
        public ActionResult Category(string name)
        {
            //Declare a list of productVM
            List<ProductsVM> productVMList;

           using(db DB = new db())
            {
                //Get Category id
                CategoryDTO categoryDTO = DB.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.ID;

                //Init the list
                productVMList = DB.Products.ToArray().Where(x => x.CategoryID == catId).Select(x => new ProductsVM(x)).ToList();

                if (!DB.Products.Any(x => x.CategoryID.Equals(catId)))
                {
                    return RedirectToAction("Index", "Shop");
                }


                //Get category name
                var productCat = DB.Products.Where(x => x.CategoryID == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;
            }

            return View(productVMList);
        }

        //GET: /shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //Declare the VM and DTO
            ProductsVM model;
            ProductsDTO dto;

            //Init product id
            int id = 0;

            using(db DB = new db())
            {
                //Check if product exists
                if(!DB.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                //Inti productDTO
                dto = DB.Products.Where(x => x.Slug == name).FirstOrDefault();

                //Get id
                id = dto.ID;

                //Init model
                model = new ProductsVM(dto);
            }

            //Get gallery images
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                  .Select(fn => Path.GetFileName(fn));


            return View("ProductDetails", model);
        }
    }
}