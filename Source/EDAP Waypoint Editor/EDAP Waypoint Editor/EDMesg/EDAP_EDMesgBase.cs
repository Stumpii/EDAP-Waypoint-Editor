using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDAP_Waypoint_Editor.Models
{
    public class EDMesgAction
    {
    }

    public class EDMesgEvent
    {
    }

    public class EDMesgEnvelope
    {
        public string type { get; set; }
        public IDictionary<string, object> data { get; set; }

        public EDMesgEnvelope(string type, IDictionary<string, object> data)
        {
            this.type = type;
            this.data = data;
        }

        public string GetJSon()

        {
            return JsonConvert.SerializeObject(this);
        }
    }
}