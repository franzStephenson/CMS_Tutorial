using CMS_Udemy.Models.Data;
using CMS_Udemy.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS_Udemy.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page="")
        {
            //Get/Set page slug
            if (page == "")
                page = "home";

            //Declare model and DTO
            PageVm model;
            DTO dto;

            //check if page exists

            using(db DB = new db())
            {
                if (!DB.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            //Get page DTO
            using(db DB = new db())
            {
                dto = DB.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            //Set page title
            ViewBag.PageTitle = dto.Title;

            //check for sidebar
            if(dto.HasSideBar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            //Init model
            model = new PageVm(dto);

            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            //Declare a list of PageVM
            List<PageVm> pageVMList;

            //Get all pages except home
            using(db DB = new db())
            {
                pageVMList = DB.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVm(x)).ToList();
            }


            return PartialView(pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            //Declare model
            SideBarVM model;

            //Init model
            using(db DB = new db())
            {
                SideBarDTO dto = DB.SideBar.Find(1);
                model = new SideBarVM(dto);
            }

            return PartialView(model);
        }
    }
}