using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AO.Database.Entities
{
    using System.Data.Linq;

    public class DBInstancedItem
    {
        public int containertype { get; set; }
        public int containerinstance { get; set; }
        public int containerplacement { get; set; }
        public int itemtype { get; set; }
        public int iteminstance { get; set; }
        public int lowid { get; set; }
        public int highid { get; set; }
        public int quality { get; set; }
        public int multiplecount { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float headingx { get; set; }
        public float headingy { get; set; }
        public float headingz { get; set; }
        public float headingw { get; set; }
        public Binary stats { get; set; }
        
    }
}
