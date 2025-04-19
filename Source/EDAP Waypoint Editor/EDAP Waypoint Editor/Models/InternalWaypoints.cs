using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDAP_Waypoint_Editor.Models
{
    [AddINotifyPropertyChangedInterface]
    public class InternalWaypoints
    {
        public List<InternalWaypoint> Waypoints { get; set; } = new List<InternalWaypoint>();
        public InternaGlobalshoppinglist globalshoppinglist { get; set; } = new InternaGlobalshoppinglist();
    }

    [AddINotifyPropertyChangedInterface]
    public class InternaGlobalshoppinglist
    {
        public string Name { get; set; }
        public List<ShoppingItem> BuyCommodities { get; set; } = new List<ShoppingItem>();
        public bool UpdateCommodityCount { get; set; }
        public bool Skip { get; set; }
        public bool Completed { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class InternalWaypoint
    {
        public InternalWaypoint()
        {
        }

        public InternalWaypoint(string SystemName, String StationName)
        {
            this.SystemName = SystemName;
            this.StationName = StationName;
        }

        public string Name { get; set; } = "";
        public string SystemName { get; set; } = "";
        public string StationName { get; set; } = "";
        public string GalaxyBookmarkType { get; set; } = "";
        public int GalaxyBookmarkNumber { get; set; }
        public string SystemBookmarkType { get; set; } = "";
        public int SystemBookmarkNumber { get; set; }
        public List<ShoppingItem> SellCommodities { get; set; } = new List<ShoppingItem>();
        public List<ShoppingItem> BuyCommodities { get; set; } = new List<ShoppingItem>();

        public bool UpdateCommodityCount { get; set; }
        public bool FleetCarrierTransfer { get; set; }
        public bool Skip { get; set; }
        public bool Completed { get; set; }

        public bool StationDefined
        { get { return StationName != ""; } }
    }

    public class ShoppingItem
    {
        public ShoppingItem()
        {
        }

        public ShoppingItem(string Name)
        {
            this.Name = Name;
        }

        public ShoppingItem(string Name, int Quantity)
        {
            this.Name = Name;
            this.Quantity = Quantity;
        }

        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}