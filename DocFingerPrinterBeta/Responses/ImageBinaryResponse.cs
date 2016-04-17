using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocFingerPrinterBeta.Responses
{
    /// <summary>
    /// imageBinary response for returning binary of image
    /// </summary>
    public class ImageBinaryResponse : BaseResponse
    {
        public byte[] ImageBinary { get; set; }
    }
}
