using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBO.ViewModels
{
    public class FBOAverageFuelPrices
    {
        public int? CompanyID { get; set; }
        public decimal? Average_JETA { get; set; }
        public decimal? Average_100LL { get; set; }
        public decimal? Average_SAF { get; set; }
        public string FAARegionCode { get; set; }
        public string RegionName { get; set; }
    }
}
