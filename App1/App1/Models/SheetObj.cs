using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Models
{
    public class SheetObj
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public int LatestRow { get; set; }
        public string UpdateRange { get; set; }
    }
}
