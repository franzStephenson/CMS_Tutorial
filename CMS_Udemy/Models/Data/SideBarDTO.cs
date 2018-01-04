using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMS_Udemy.Models.Data
{
    [Table("tblSidebar")]
    public class SideBarDTO
    {
        [Key]
        public int ID { get; set; }
        public string Body { get; set; }
    }
}