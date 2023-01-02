using GlobalAir.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBO.ViewModels
{
    public class FBOResult
    {

        public FBOManagement_GetFBO_Result FBO { get; set; }
        public FBOAverageFuelPrices averageprices { get; set; }
        public FBOManagement_Stats_Result fboStats { get; set; }
        public RatingStats ratingStats { get; set; }
        public string companyName { get; set; }
        public string companyfullAddress { get; set; }
        public string fboIsApproved { get; set; }
        public bool fboIsExpired { get; set; }
        public int newReviews { get; set; }
        public decimal? averageFuelPrice { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public bool isUpgradeEligible { get; set; }
        public int platinium_level_count { get; set; }
    }
   public class FBOManagement_CheckPlatinum_Result
    {
        public int total { get; set; }
    }
}
