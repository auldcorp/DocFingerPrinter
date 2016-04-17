using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocFingerPrinterBeta.Models
{
    /// <summary>
    /// image model
    /// </summary>
    public class Image
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string UniqueMark { get; set; }
        public byte[] OriginalImageBinary { get; set; }
        public byte[] MarkedImageBinary { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}