using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDAP_Waypoint_Editor.Models
{
    internal class EDMesg
    {
    }

    public class UndockCompleteEvent
    { }

    public class SpeakingPhraseEvent
    {
        private string Text;
        private string Reason;
        private float Duration;
        private string Timestamp;
    }

    public class EDMesgEnvelope
    {
        public string type { get; set; }
        public Dictionary<string, string> data { get; set; }

        public EDMesgEnvelope(string type, Dictionary<string, string> data)
        {
            this.type = type;
            this.data = data;
        }

        public EDMesgEnvelope()
        {
        }

        public string GetJSon()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}