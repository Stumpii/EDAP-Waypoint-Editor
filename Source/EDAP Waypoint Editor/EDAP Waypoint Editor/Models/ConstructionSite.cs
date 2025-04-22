namespace EDAP_Waypoint_Editor
{
    public class ConstructionSite
    {
        public string Name { get; set; }
        public string System { get; set; }
        public string Path { get; set; }

        public ConstructionSite(string Name, string System, string Path)
        {
            this.Name = Name;
            this.System = System;
            this.Path = Path;
        }

        public ConstructionSite()
        {
        }
    }

    public class ConstructionCommodity
    {
        public string Resource { get; set; }
        public double Progress { get; set; }
        public int Required { get; set; }
        public int Delivered { get; set; }
        public int Remaining { get; set; }
        public int Value { get; set; }
    }
}