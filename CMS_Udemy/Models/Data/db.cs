using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CMS_Udemy.Models.Data
{
    public class db:DbContext
    {
        public DbSet<DTO> Pages { get; set; }
        public DbSet<SideBarDTO> SideBar { get; set; }
        public DbSet<CategoryDTO> Categories { get; set; }
        public DbSet<ProductsDTO> Products { get; set; }
        public DbSet<UsersDTO> Users { get; set; }
        public DbSet<RolesDTO> Roles { get; set; }
        public DbSet<UserRolesDTO> UserRoles { get; set; }
        public DbSet<OrderDTO> Orders { get; set; }
        public DbSet<OrderDetailsDTO> OrderDetails { get; set; }
    }
}