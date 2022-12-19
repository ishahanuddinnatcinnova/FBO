using GlobalAir.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBO.ViewModels
{
    public class ServiceResponseViewModel
    {
        public bool isRedirect { get; set; }
        public string redirectURL { get; set; }

        public ResponseDataViewModel data = new ResponseDataViewModel();
        


    }

    public class ResponseDataViewModel
    {
        public List<FBOManagement_GetFBOs_Result> FBOs { get; set; }
        public FBOResult FBO { get; set; }
        public FuelCardDiscountsModel fuelcards { get; set; }

        public FBOManagement_UpdateBasicServices_Result basic = new FBOManagement_UpdateBasicServices_Result();
        public services_Accepted_GetCreditCards_Result fboCreditCards;
    }
}
