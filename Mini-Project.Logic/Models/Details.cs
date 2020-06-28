using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_Project.Logic.Models
{
    public class Detail
    {
        public string UserName { get; set; }
        public string Rating { get; set; }
        public string Datetime { get; set; }
        public string Link { get; set; }

        public string Comment { get; set; }
    }
    public class YotubeDetails
    {
        public List<Detail> Details { get; set; }
    }

    public class AmazonPageDetails
    {
        public string UserNames { get; set; }
        public string Reviews { get; set; }
        public string ReviewDates { get; set; }
        public string Ratings { get; set; }
    }

    public class PageDetails
    {
        public List<AmazonPageDetails> pageDetails { get; set; }

    }
}
