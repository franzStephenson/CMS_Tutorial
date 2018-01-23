using CMS_Udemy.Models.Data;
using CMS_Udemy.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS_Udemy.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //declare list of pagevm
            List<PageVm> pageslist;

           
            using (db DB = new db())
            {
                //Init the list
                pageslist = DB.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVm(x)).ToList();
            }

            //return view with list
            return View(pageslist);
        }

        // GET: Admin/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVm model)
        {
            //Check model state
            if(! ModelState.IsValid)
            {
                return View(model);
            }
            using (db DB = new db())
            {

                //Declare slug
                string slug;

                //Init DTO
                DTO dto = new DTO();

                //DTO Title
                dto.Title = model.Title;

                //Check for and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Make sure title and slug is unique
                if(DB.Pages.Any(x => x.Title == model.Title)||DB.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That Title or slug already exists.");
                    return View(model);
                }

                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 100;


                //Save DTO
                DB.Pages.Add(dto);
                DB.SaveChanges();
            }

            //Set TempData message
            TempData["SM"] = "You have added a new page!";

                //Redirect
                return RedirectToAction("AddPage");
        }

        // GET: Admin/EditPage
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Declare pagevm
            PageVm model;

            using (db DB = new db())
            {

                //Get the page
                DTO dto = DB.Pages.Find(id);

                //confirm page exists
                if(dto == null)
                {
                    return Content("The page does not exist.");
                }

                //Init pagevm
                model = new PageVm(dto);

            }

            //return view with model
            return View(model);

        }

        // POST: Admin/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVm model)
        {
            //check model state
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            using (db DB = new db())
            {
                //Get page ID
                int id = model.ID;

                //Declare slug
                string slug ="home";

                //Get the page
                DTO dto = DB.Pages.Find(id);

                //DTO the title
                dto.Title = model.Title;

                //Check for slug and set it if need be
                if(model.Slug != "home")
                {
                    if(string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                //Check if title and slug are unique
                if(DB.Pages.Where(x => x.ID != id).Any(x => x.Title == model.Title)||
                    DB.Pages.Where(x=> x.ID != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exist.");
                    return View(model);
                }

                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;

                //Save the DTO
                DB.SaveChanges();
            }

            //Set TempData message
            TempData["SM"] = "You have edited the page!";

            //Redirect
            return RedirectToAction("EditPage");
        }

        // GET: Admin/PageDetails
        public ActionResult PageDetails(int id)
        {
            //Declare PageVm
            PageVm model;

            using(db DB = new db())
            {
                //Get the page
                DTO dto = DB.Pages.Find(id);

                //Confirm page exist
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                //Init PageVm
                model = new PageVm(dto);
            }

            return View(model);
        }

        // GET: Admin/DeletePage
        public ActionResult DeletePage(int id)
        {
            using (db DB = new db())
            {
                //Get the page
                DTO dto = DB.Pages.Find(id);

                //Remove the page
                DB.Pages.Remove(dto);

                //Save
                DB.SaveChanges();
            }


            //Redirect
            return RedirectToAction("Index");
        }

        // POST: Admin/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (db DB = new db())
            {

                //set initial count
                int count = 1;

                //Declare DTO
                DTO dto;

                //Set sorting for each page
                foreach(var pageID in id)
                {
                    dto = DB.Pages.Find(pageID);
                    dto.Sorting = count;

                    DB.SaveChanges();
                    count++;
                }

            }

        }


        // GET: Admin/EditSideBar
        [HttpGet]
        public ActionResult EditSideBar()
        {
            //Declare model
            SideBarVM model;

            using (db DB = new db())
            {
                //Get DTO
                SideBarDTO dto = DB.SideBar.Find(1);

                //Init model
                model = new SideBarVM(dto);

            }

            //return view with model
            return View(model);
        }

        // POST: Admin/EditSideBar
        [HttpPost]
        public ActionResult EditSideBar(SideBarVM model)
        {
            using (db DB = new db())
            {
                //Get DTO
                SideBarDTO dto = DB.SideBar.Find(1);

                //DTO body
                dto.Body = model.Body;

                //Save
                DB.SaveChanges();
            }
            //Set TempData message
            TempData["SM"] = "Sidebar Edited";

            return RedirectToAction("EditSideBar");
        }
    }
}