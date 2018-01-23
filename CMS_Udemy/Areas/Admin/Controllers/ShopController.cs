using CMS_Udemy.Areas.Admin.Models.ViewModels.Shop;
using CMS_Udemy.Models.Data;
using CMS_Udemy.Models.ViewModels.Shop;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CMS_Udemy.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            // Declare list of models
            List<CategoryVM> categoryVMList;

            using (db DB = new db())
            {
                //Init list
                categoryVMList = DB.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();

            }


            //return view with list
            return View(categoryVMList);
        }

        // POST: Admin/Shop/Categories
        [HttpPost]
        public string AddNewCateory(string catName)
        {
            //Declare ID
            string id;

            using (db DB = new db())
            {

                //Check that category name is unique
                if (DB.Categories.Any(x => x.Name == catName))
                {
                    return "titletaken";
                }

                //Init DTO
                CategoryDTO dto = new CategoryDTO();

                //Add to DTO
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                //Save DTO
                DB.Categories.Add(dto);
                DB.SaveChanges();

                //Get DTO ID
                id = dto.ID.ToString();

            }

            //Return ID
            return id;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (db DB = new db())
            {

                //set initial count
                int count = 1;

                //Declare DTO
                CategoryDTO dto;

                //Set sorting for each page
                foreach (var catID in id)
                {
                    dto = DB.Categories.Find(catID);
                    dto.Sorting = count;

                    DB.SaveChanges();
                    count++;
                }

            }

        }

        // GET: Admin/Shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (db DB = new db())
            {
                //Get the page
                CategoryDTO dto = DB.Categories.Find(id);

                //Remove the page
                DB.Categories.Remove(dto);

                //Save
                DB.SaveChanges();
            }


            //Redirect
            return RedirectToAction("Categories");
        }

        // POST: Admin/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (db DB = new db())
            {
                //Check category name is unique
                if (DB.Categories.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }

                //Get DTO
                CategoryDTO dto = DB.Categories.Find(id);

                //Edit DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //Save
                DB.SaveChanges();
            }

            //Return
            return "ok";
        }

        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Init model
            ProductsVM model = new ProductsVM();

            //Add select list of categories to model
            using (db DB = new db())
            {
                model.Categories = new SelectList(DB.Categories.ToList(), "ID", "Name");
            }
            //return view with model
            return View(model);
        }

        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductsVM model, HttpPostedFileBase file)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                using (db DB = new db())
                {
                    model.Categories = new SelectList(DB.Categories.ToList(), "ID", "Name");
                    return View(model);
                }
            }

            using (db DB = new db())
            {
                //Confirm product is unique
                if (DB.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(DB.Categories.ToList(), "ID", "Name");
                    ModelState.AddModelError("", "That product already exist!");
                    return View(model);
                }
            }

            //Declare product ID
            int id;

            //Init and save productDTO
            using (db DB = new db())
            {
                ProductsDTO product = new ProductsDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-");
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryID = model.CategoryID;

                CategoryDTO catDTO = DB.Categories.FirstOrDefault(x => x.ID == model.CategoryID);
                product.CategoryName = catDTO.Name;

                DB.Products.Add(product);
                DB.SaveChanges();

                //Get inserted id
                id = product.ID;
            }
            //Set TempData message
            TempData["SM"] = "You have added a product";

            #region Upload Image
            //Create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            //Check if a file was uploaded
            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);
            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);
            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);
            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);
            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            //Check if file was upload
            if (file != null && file.ContentLength > 0)
            {
                //Get file extension
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/png" &&
                    ext != "image/x-png")
                {
                    using (db DB = new db())
                    {

                        model.Categories = new SelectList(DB.Categories.ToList(), "ID", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image format!");
                        return View(model);

                    }
                }

                //Init image name
                string imagename = file.FileName;

                //save image name to DTO
                using (db DB = new db())
                {
                    ProductsDTO dto = DB.Products.Find(id);
                    dto.ImageName = imagename;

                    DB.SaveChanges();
                }

                //Set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imagename);
                var path2 = string.Format("{0}\\{1}", pathString3, imagename);

                //Save original
                file.SaveAs(path);

                //Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            #endregion


            //Redirect
            return RedirectToAction("AddProduct");
        }

        // GET: Admin/Shop/Products
        public ActionResult Products(int? page, int? catID)
        {
            //Declare a list of ProductVM
            List<ProductsVM> listOfProductVM;

            //Set page number
            var pageNumber = page ?? 1;

            using (db DB = new db())
            {
                //Init list
                listOfProductVM = DB.Products.ToArray()
                    .Where(x => catID == null || catID == 0 || x.CategoryID == catID)
                    .Select(x => new ProductsVM(x))
                    .ToList();

                //Populate categories select list
                ViewBag.Categories = new SelectList(DB.Categories.ToList(), "Id", "Name");


                //Set selected category
                ViewBag.SelectedCat = catID.ToString();

            }

            //Set pagination
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.OnePageOfProducts = onePageOfProducts;

            //Return view with list
            return View(listOfProductVM);
        }

        // GET: Admin/Shop/EditProducts/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            //Declare productVm
            ProductsVM model;

            using (db DB = new db())
            {
                //Get the product
                ProductsDTO dto = DB.Products.Find(id);

                //confirm product exists
                if(dto == null)
                {
                    return Content("That product does not exist.");
                }

                //init model
                model = new ProductsVM(dto);

                //make a select list
                model.Categories = new SelectList(DB.Categories.ToList(), "Id", "Name");

                //get gallery images
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));

            }

            //return view with model
            return View(model);
        }

        // POST: Admin/Shop/EditProducts/id
        [HttpPost]
        public ActionResult EditProduct(ProductsVM model, HttpPostedFileBase file)
        {
            //get product id
            int id = model.ID;

           
            using (db DB = new db())
            {
                //populate categories select list and gallery images
                model.Categories = new SelectList(DB.Categories.ToList(), "Id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));

            //Check model state
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //Make sure product name is unique
            using (db DB = new db())
            {
                if(DB.Products.Where(x=>x.ID != id).Any(x=> x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name alreay exist!");
                    return View(model);
                }
            }

            //update product
            using (db DB = new db())
            {
                ProductsDTO dto = DB.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-");
                dto.Price = model.Price;
                dto.ImageName = model.ImageName;
                dto.CategoryID = model.CategoryID;
                dto.Description = model.Description;

                CategoryDTO catDTO = DB.Categories.FirstOrDefault(x => x.ID == model.CategoryID);
                dto.CategoryName = catDTO.Name;

                DB.SaveChanges();
            }

            //Set tempdata message

            TempData["SM"] = "You have updated the product";

            #region Image Upload
            //check for file upload
            if (file != null && file.ContentLength > 0)
            {
                //get extension
                string ext = file.ContentType.ToLower();

                //verify ext is valid
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/png" &&
                    ext != "image/x-png")
                {
                    using (db DB = new db())
                    {

                        ModelState.AddModelError("", "The image was not uploaded - wrong image format!");
                        return View(model);

                    }
                }

                //set upload directory paths
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));


                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");


                //delete files from directory
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (FileInfo file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                //Save image name
                string imageName = file.FileName;

                using (db DB = new db())
                {
                    ProductsDTO dto = DB.Products.Find(id);
                    dto.ImageName = imageName;

                    DB.SaveChanges();
                }

                //save original and thumb images
                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                
                file.SaveAs(path);

                //Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            
            #endregion


            //redirect
            return RedirectToAction("EditProduct");
        }

        // GET: Admin/Shop/DeleteProduct/id
        public ActionResult DeleteProduct(int id)
        {
            //delete product from db
            using(db DB= new db())
            {
                ProductsDTO dto = DB.Products.Find(id);
                DB.Products.Remove(dto);

                DB.SaveChanges();
            }

            //Delete product folder
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            string pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if(Directory.Exists(pathString))
            {
                Directory.Delete(pathString, true);
            }

            //Redirect
            return RedirectToAction("Products");
        }


        // POST: Admin/Shop/SaveGalleryImages
        [HttpPost]
        public void SaveGalleryImages(int id) {
            //loop through files
            foreach(string fileName in Request.Files)
            {
                //Init the file
                HttpPostedFileBase file = Request.Files[fileName];

                //Check it's not null
                if(file != null && file.ContentLength > 0 )
                {
                    //Set directory paths
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    //Set image paths
                    var path1 = string.Format("{0}\\{1}", pathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, file.FileName);

                    //Save original and thumb
                    file.SaveAs(path1);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);
                }

            }

        }

        // POST: Admin/Shop/DeleteImage
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {

            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
        }

        // GET: Admin/Shop/Orders
        public ActionResult Orders()
        {
            //Init list of ordersforadminVM
            List<OrdersForAdminVM> ordersForAdmin = new List<OrdersForAdminVM>();

            using(db DB = new db())
            {
                //Init list of ordervm
                List<OrderVM> orders = DB.Orders.ToArray().Select(x => new OrderVM(x)).ToList();

                //Loop through list of OrderVM
                foreach(var order in orders)
                {
                    //Init product dict
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();

                    //Declare Total
                    decimal total = 0m;

                    //Init list of OrderdetailsDTO
                    List<OrderDetailsDTO> orderdetailsList = DB.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                    //Get username
                    UsersDTO user = DB.Users.Where(x => x.Id == order.UserId).FirstOrDefault();
                    string username = user.UserName;

                    //Loop through list of OrderDetailsList
                    foreach(var orderDetails in orderdetailsList)
                    {
                        //Get product
                        ProductsDTO product = DB.Products.Where(x => x.ID == orderDetails.ProductId).FirstOrDefault();

                        //Get product price
                        decimal price = product.Price;

                        //Get product name
                        string productname = product.Name;

                        //Add to product dict
                        productsAndQty.Add(productname, orderDetails.Quantity);

                        //get Total
                        total += orderDetails.Quantity * price;
                    }

                    //Add to ordersForAdminVM list
                    ordersForAdmin.Add(new OrdersForAdminVM()
                    {
                        OrderNumber = order.OrderId,
                    UserName = username,
                    Total = total,
                    ProductsAndQty = productsAndQty,
                    CreatedAt = order.CreatedAt

                    });
                }
            }
            return View(ordersForAdmin);
        }

    }
}