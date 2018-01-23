using CMS_Udemy.Models.Data;
using CMS_Udemy.Models.ViewModels.Account;
using CMS_Udemy.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CMS_Udemy.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }

        //GET: /account/login
        [HttpGet]
        public ActionResult Login()
        {
            //Confirm user is not logged in
            string username = User.Identity.Name;
            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("user-profile");
            }

            return View();
        }

        //GET: /account/login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Check if the user is valid
            bool isValid = false;
            using(db DB = new db())
            {
                if (DB.Users.Any(x => x.UserName.Equals(model.UserName) && x.Password.Equals(model.PassWord)))
                {
                    isValid = true;
                }
            }
            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.UserName, model.RememberMe));
            }
        }

        // GET: /account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: /account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            //Check if passwords match
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View("CreateAccount", model);
            }

            using(db DB = new db())
            {

                //check if username is unique
                if (DB.Users.Any(x => x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", "Username " + model.UserName + " is already taken.");
                    model.UserName = "";
                    return View("CreateAccount", model);
                }
                //Create userDTO
                UsersDTO userDTO = new UsersDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    UserName = model.UserName,
                    Password=model.Password                    
                };

                //Add the DTO
                DB.Users.Add(userDTO);

                //Save
                DB.SaveChanges();

                //Add to userRolesDTO
                int id = userDTO.Id;

                UserRolesDTO UserRolesDTO = new UserRolesDTO()
                {
                    UserId = id,
                    RoleId = 2
                };
                DB.UserRoles.Add(UserRolesDTO);
                DB.SaveChanges();
            }

            //Create tempdata message
            TempData["SM"] = "Registrated successfully.";

            //Redirect
            return Redirect("~/account/login");

        }

        // GET: /account/logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return Redirect("~/account/login");
        }

        [Authorize]
        public ActionResult UserNavPartial()
        {
            //Get username
            string username = User.Identity.Name;

            //Declare model
            UserNavPartialVM model;

            using(db DB = new db())
            {
                //Get the user
                UsersDTO dto = DB.Users.FirstOrDefault(x => x.UserName == username);

                //Build the model
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }
            //Return partial view with model
            return PartialView(model);
        }

        // GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            //Get usename
            string username = User.Identity.Name;

            //declare model
            UserProfileVM model;

            using(db DB = new db())
            {
                //Get user
                UsersDTO dto = DB.Users.FirstOrDefault(x => x.UserName == username);

                //Build model
                model = new UserProfileVM(dto);
            }
            return View("UserProfile", model);
        }

        // POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            //check if passwords match
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if(!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View("UserProfile", model);
                }
            }

            using(db DB = new db())
            {
                //get username
                string username = User.Identity.Name;

                //make sure username is unique
                if(DB.Users.Where(x=>x.Id != model.Id).Any(x=>x.UserName == username))
                {
                    ModelState.AddModelError("", "Username " + model.UserName + " already exist.");
                    model.UserName = "";
                    return View("UserProfile", model);
                }

                //Edit dto
                UsersDTO dto = DB.Users.Find(model.Id);
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress = model.EmailAddress;
                dto.UserName = model.UserName;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }


                //Save
                DB.SaveChanges();

                //Set Tempdata message
                TempData["SM"] = "Profile successfully updated.";

            }
            return Redirect("~/account/user-profile");
        }

        // GET: /account/orders
        [Authorize(Roles="User")]
        public ActionResult Orders()
        {
            //Init list of OrderForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            using(db DB = new db())
            {
                //Get user id
                UsersDTO user = DB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;

                //Init list of OrderVM
                List<OrderVM> orders = DB.Orders.Where(x => x.UserId == userId).ToArray().Select(x => new OrderVM(x)).ToList();

                //Loop through list of OrderVM
                foreach(var order in orders)
                {
                    //Init product dictionary
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();

                    //Declare total
                    decimal total = 0m;

                    //Init list of OrderDetailsDTO
                    List<OrderDetailsDTO> orderDetailsDTO = DB.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                    //Loop though list of OrderDetailsDTO
                    foreach(var orderDetails in orderDetailsDTO)
                    {
                        //Get product
                        ProductsDTO product = DB.Products.Where(x => x.ID == orderDetails.ProductId).FirstOrDefault();

                        //Get product price
                        decimal price = product.Price;

                        //Get product name
                        string productName = product.Name;

                        //Add to products dictionary
                        productsAndQty.Add(productName, orderDetails.Quantity);

                        //Get total
                        total += orderDetails.Quantity * price;

                    }

                    //Add to ordersForUserVM list
                    ordersForUser.Add(new OrdersForUserVM() {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt
                    });
                }
            }

            //return view with list of OrderForUserVM
            return View(ordersForUser);
        }
    }
}