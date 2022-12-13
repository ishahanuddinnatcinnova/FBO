using GlobalAir.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBO.ViewModels
{
    public class UserViewModel
    {
        public string userID { get; set; }
        public string userFirstname { get; set; }
        public int companyID { get; set; }
        public bool isUser { get; set; }

    }
}
