using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string filename { get; set; }
        public byte[] imageBinary { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}