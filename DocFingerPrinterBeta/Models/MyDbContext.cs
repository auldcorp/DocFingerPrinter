using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocFingerPrinterBeta.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base("DocFingerPrinterBeta")
        {

        }

        public DbSet<Image> Images { get; set; }
    }
}
