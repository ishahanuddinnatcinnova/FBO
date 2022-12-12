using GlobalAir.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBO.ViewModels
{
    public class FboResultMainModel
    {
        public FBOManagement_GetFBO_Result FBO { get; set; }
      
        public List<FBOManagement_GetFBOs_Result> FBOs = new List<FBOManagement_GetFBOs_Result>();
        public string companyName { get; set; }
        public string companyfullAddress { get; set; }
        public string fboIsApproved { get; set; }   
        public bool fboIsExpired { get; set; }  
        public FBOManagement_GetRegionAverages_Result averageprices { get; set; }
        public string companyLogo;
        public int reviews_of_ratings { get; set; }
        public int fbo_count { get; set; }
        public decimal? averageFuelPrice { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public FBOManagement_Stats_Result fboStats { get; set; }
    }
}
