﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocFingerPrinterBeta.Responses
{
    public class ImagesResponse : BaseResponse
    {
        public List<byte[]> Images { get; set; }
    }
}