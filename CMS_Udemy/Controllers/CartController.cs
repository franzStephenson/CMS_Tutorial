﻿using CMS_Udemy.Models.Data;
using CMS_Udemy.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS_Udemy.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            //Init cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Check i cart is empty
            if (cart.Count ==0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            //Calculate total and save to ViewBag
            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }
            ViewBag.GrandTotal = total;

            //Return view with model
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            //Init CartVM
            CartVM model = new CartVM();

            //Init quantity
            int qty = 0;

            //Init price
            decimal price = 0m;

            //Check for cart session
            if(Session["cart"] != null)
            {
                //Get total qty and price
                var list = (List<CartVM>)Session["cart"];

                foreach(var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }
                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                //or set qty and price to 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            //Or set
            return PartialView(model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            //Init CartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Init CartVM
            CartVM model = new CartVM();

            using(db DB = new db())
            {
                //Get the product
                ProductsDTO product = DB.Products.Find(id);

                //Check if the product is already in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                //If not , add new
                if(productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.ID,
                        ProdcutName = product.Name, 
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    //If it is, increment
                    productInCart.Quantity++;
                }
            }

            //Get total qty and price and add to model
            int qty = 0;
            decimal price = 0m;

            foreach(var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            //Save cart back to seesion
            Session["cart"] = cart;


            //Return partial view with model
            return PartialView(model);
        }
        //GET: /cart/IncrementProduct
        public JsonResult IncrementProduct(int  productId)
        {
            //Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using(db DB = new db())
            {
                //Get cartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Increment qty
                model.Quantity++;

                //store needed data
                var result = new { qty = model.Quantity, price = model.Price };


                //return json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
           
        }

        //GET: /cart/DecrementProduct
        public JsonResult DecrementProduct(int productId)
        {
            //Init cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using(db DB = new db())
            {
                //Get cartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //decrement qty
                if(model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                //store needed data
                var result = new { qty = model.Quantity, price = model.Price };

                //return json with model
                return Json(result, JsonRequestBehavior.AllowGet);

            }


           
        }
    }
}