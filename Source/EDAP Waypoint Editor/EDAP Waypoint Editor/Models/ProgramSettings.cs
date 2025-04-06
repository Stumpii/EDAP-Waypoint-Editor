using DR_Tools.Models;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #endregion Properties
    }
}