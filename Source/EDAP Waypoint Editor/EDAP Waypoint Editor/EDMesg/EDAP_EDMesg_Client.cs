using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EDAP_Waypoint_Editor.Models
{
    public class EDAP_EDMesg_Client
    {
        #region Fields

        private const int RECEIVE_PORT_NO = 15571;
        private const int SEND_PORT_NO = 15570;

        #endregion Fields

        #region Events

        public event EventHandler<EDAPLocationEventArgs> EDAPLocationEvent_Received; // Event declaration

        public event EventHandler LaunchCompleteEvent_Received; // Event declaration

        public event EventHandler<SpeakingPhraseEventArgs> SpeakingPhraseEvent_Received;

        #endregion Events

        // Event declaration

        #region Constructors

        public EDAP_EDMesg_Client()
        {
            // Create a new thread for the subscriber
            Thread subscriberThread = new Thread(SubscriberTask);
            subscriberThread.IsBackground = true;
            subscriberThread.Start();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Send an Action to EDAP
        /// </summary>
        /// <param name="msg">The action message to send.</param>
        internal void SendActionToEDAP(EDMesgAction Action)
        {
            if (Action is null)
                return;

            var envelope = new EDMesgEnvelope(Action.GetType().Name, Action.AsDictionary());

            using (var client1 = new PushSocket($"tcp://localhost:{SEND_PORT_NO}"))
            {
                // Wait a little for the socket
                Thread.Sleep(50);

                string serMsg = envelope.GetJSon();
                Console.WriteLine($"Sending {serMsg}");

                byte[] bytesToSend1 = UTF8Encoding.UTF8.GetBytes(serMsg);
                client1.SendFrame(bytesToSend1);
            }
        }

        /// <summary>
        /// Subscribe to events from EDAP
        /// </summary>
        private void SubscriberTask()
        {
            var topic = "";
            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect($"tcp://localhost:{RECEIVE_PORT_NO}");
                subSocket.Subscribe(topic);
                Console.WriteLine("Subscriber socket connecting...");
                while (true)
                {
                    byte[] workload = subSocket.ReceiveFrameBytes();

                    string messageTopicReceived = UTF8Encoding.UTF8.GetString(workload);
                    EDMesgEnvelope msgRec = JsonConvert.DeserializeObject<EDMesgEnvelope>(messageTopicReceived);
                    Console.WriteLine($"Received {msgRec}");

                    #region Configure events from EDAP here

                    // Determine the type of event
                    if (msgRec.type == typeof(EDAPLocationEvent).Name)
                    {
                        EDAPLocationEvent_Received?.Invoke(this, new EDAPLocationEventArgs()
                        {
                            EventData = msgRec.data.ToObject<EDAPLocationEvent>()
                        });
                    }
                    else if (msgRec.type == typeof(LaunchCompleteEvent).Name)
                        LaunchCompleteEvent_Received?.Invoke(this, new EventArgs());
                    else if (msgRec.type == typeof(SpeakingPhraseEvent).Name)
                        SpeakingPhraseEvent_Received?.Invoke(this, new SpeakingPhraseEventArgs()
                        {
                            EventData = msgRec.data.ToObject<SpeakingPhraseEvent>()
                        });

                    #endregion Configure events from EDAP here
                }
            }
        }

        #endregion Methods

        #region Classes

        public class EDAPLocationEventArgs : EventArgs
        {
            #region Properties

            public EDAPLocationEvent EventData { get; set; }

            #endregion Properties
        }

        public class SpeakingPhraseEventArgs : EventArgs
        {
            #region Properties

            public SpeakingPhraseEvent EventData { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }

    public class EDAPLocationEvent : EDMesgEvent
    {
        #region Properties

        public string path { get; set; }

        #endregion Properties
    }

    public class GetEDAPLocationAction : EDMesgAction
    { }

    public class LaunchAction : EDMesgAction
    { }

    public class GenericAction : EDMesgAction
    {
        #region Properties

        public string name { get; set; }

        #endregion Properties
    }

    public class LaunchCompleteEvent : EDMesgEvent
    { }

    internal class SystemMapTargetStationByBookmarkAction : EDMesgAction
    {
        public string type { get; set; }
        public int number { get; set; }
    }

    internal class GalaxyMapTargetStationByBookmarkAction : EDMesgAction
    {
        public string type { get; set; }
        public int number { get; set; }
    }

    internal class GalaxyMapTargetSystemByNameAction : EDMesgAction
    {
        public string name { get; set; }
    }

    public class LoadWaypointFileAction : EDMesgAction
    {
        #region Properties

        public string filepath { get; set; }

        #endregion Properties
    }

    public class SpeakingPhraseEvent : EDMesgEvent
    {
        #region Fields

        private float Duration;
        private string Reason;
        private string Text;
        private string Timestamp;

        #endregion Fields
    }

    public class StartWaypointAssistAction : EDMesgAction
    { }

    public class StopAllAssistsAction : EDMesgAction
    { }
}