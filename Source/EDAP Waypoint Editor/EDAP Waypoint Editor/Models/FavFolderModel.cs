using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DR_Tools.Models
{
    [AddINotifyPropertyChangedInterface]
    public class FavFolderModel
    {
        #region Fields

        private string _group="Group Name";
        private string location = "Enter Path/Location";
        private string name = "Enter Name";
        private string note = "";

        #endregion Fields

        #region Properties

        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public string Location { get => location; set => location = value; }
        public string Name { get => name; set => name = value; }
        public string Note { get => note; set => note = value; }

        #endregion Properties
    }
}
