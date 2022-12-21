using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBO.ViewModels
{
    public class RatingStats
    {
        public double averageRating
        {
            get;
            set;
        }

        public double avgRating1
        {
            get;
            set;
        }
        public double avgRating2
        {
            get;
            set;
        }
        public double avgRating3
        {
            get;
            set;
        }
        public double avgRating4
        {
            get;
            set;
        }
        public double avgRating5
        {
            get;
            set;
        }

        public string RatedBy
        {
            get;
            set;
        }
        public int TotalReviews
        {
            get;
            set;
        }
    }

}
