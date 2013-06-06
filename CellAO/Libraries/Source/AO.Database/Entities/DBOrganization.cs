using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AO.Database.Entities
{
    public class DBOrganization
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int LeaderID { get; set; }
        public int GovernmentForm { get; set; }
        public string Description { get; set; }
        public string Objective { get; set; }
        public string History { get; set; }
        public int Tax { get; set; }
        public UInt64 Bank { get; set; }
        public int Commission { get; set; }
        public int ContractsID { get; set; }
        public int CityID { get; set; }
        public int TowerFieldID { get; set; }
    }
}
