﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocFingerPrinterBeta.Responses
{
    public class ImageBinaryResponse : BaseResponse
    {
        public byte[] ImageBinary { get; set; }
    }
}