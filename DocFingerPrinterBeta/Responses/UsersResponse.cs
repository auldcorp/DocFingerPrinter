using DocFingerPrinterBeta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocFingerPrinterBeta.Responses
{
    /// <summary>
    /// users response for returning list of users
    /// </summary>
    public class UsersResponse : BaseResponse
    {
        public List<User> Users { get; set; }
    }
}
