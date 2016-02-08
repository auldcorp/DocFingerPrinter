using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace DocFingerPrinterBeta.Models
{
    public class User
    {
        public string Username { get; set; }
        public int ImageMark { get; set; }
    }

    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}