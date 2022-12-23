using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBO.ViewModels
{
    public class FBOInfoUpdateModel
    {
        public String companyID { get; set; }
        public bool cc_mastercard { get; set; }
        public bool cc_visa { get; set; }
        public bool cc_discover { get; set; }
        public bool cc_amex { get; set; }
        public String ManagerName { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String City { get; set; }

        public string stateid { get; set; }
        public IFormFile logo { get; set; }
        public string managerpic { get; set; }
        public String Zipcode { get; set; }
        public String Phone { get; set; }
        public String Fax { get; set; }
        public String Arinc { get; set; }
        public String Unicom { get; set; }
        public String FAARepair { get; set; }
        public String Email { get; set; }
        public String URL { get; set; }
        public String OpHours { get; set; }
        public String RampDescription { get; set; }
        public String ComDescription { get; set; }
        public String strRampFee { get; set; }
        public String username { get; set; }


    }
}
