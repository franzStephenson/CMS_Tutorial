using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS_Udemy.Models.Data;
using System.Web.Mvc;

namespace CMS_Udemy.Models.ViewModels.Pages
{
    public class SideBarVM
    {
       
        public SideBarVM()
        {

        }
        public SideBarVM(SideBarDTO row)
        {
            ID = row.ID;
            Body = row.Body;
        }

        public int ID { get; set; }
        [AllowHtml]
        public string Body { get; set; }
    }
}