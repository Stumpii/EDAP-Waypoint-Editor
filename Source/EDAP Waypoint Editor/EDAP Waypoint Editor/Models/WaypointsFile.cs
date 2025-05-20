using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDAP_Waypoint_Editor.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Globalshoppinglist
    {
        public Dictionary<string, int> BuyCommodities { get; set; }
        public bool UpdateCommodityCount { get; set; }
        public bool Skip { get; set; }
        public bool Completed { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class Waypoint
    {
        public string SystemName { get; set; } = "";
        public string StationName { get; set; } = "";
        public string GalaxyBookmarkType { get; set; } = "";
        public int GalaxyBookmarkNumber { get; set; }
        public string SystemBookmarkType { get; set; } = "";
        public int SystemBookmarkNumber { get; set; }
        public Dictionary<string, int> SellCommodities { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> BuyCommodities { get; set; } = new Dictionary<string, int>();

        public string Comment { get; set; }
        public bool UpdateCommodityCount { get; set; }
        public bool FleetCarrierTransfer { get; set; }
        public bool Skip { get; set; }
        public bool Completed { get; set; }
    }
}