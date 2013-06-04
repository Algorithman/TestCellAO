using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AO.Database.Entities
{
    public class DBItem
    {
        public int containertype { get; set; }
        public int containerinstance { get; set; }
        public int containerplacement { get; set; }
        public int lowid { get; set; }
        public int highid { get; set; }
        public int quality { get; set; }
        public int multiplecount { get; set; }
    }
}
