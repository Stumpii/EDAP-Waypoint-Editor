using DR_Tools.Models;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows;

namespace EDAP_Waypoint_Editor
{
    [AddINotifyPropertyChangedInterface]
    internal class ProgramSettings
    {
        #region Properties

        public double MainFormHeight { get; set; } = 450;

        public double MainFormWidth { get; set; } = 550;

        public double MainFormX { get; set; } = 100;

        public double MainFormY { get; set; } = 100;

        public string LastOpenFilepath { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string Note3 { get; set; }
        public string Note4 { get; set; }

        public List<ConstructionSite> ConstructionSites { get; set; } = new List<ConstructionSite>();

        #endregion Properties

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (ConstructionSites is null)
                this.ConstructionSites = new List<ConstructionSite>();
        }
    }
}