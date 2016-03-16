using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Engine.GameElements
{
    public class Event
    {
        [XmlElement("Event")]
        public List<string> EventColl;
        public string EventId;
        [XmlIgnore]
        public List<object> EventList;
        public string EventName;

        public Image EventImage;

        public Event()
        {
            EventId = Guid.NewGuid().ToString();
            EventName = "New Event";
            EventColl = new List<string>();
            EventList = new List<object>();
        }

        public void AnalyzeEvent()
        {
            foreach (string evt in EventColl)
            {
                string[] split = evt.Replace("[", String.Empty).Replace("]", String.Empty).Split(new[] { ':' }, 3);
                if (split.Any())
                {
                    switch (split[0])
                    {
                        case "TeleportEvent":
                            if (split.Length == 3)
                            {
                                float posX =
                                    float.Parse(split[2].Substring((split[2].IndexOf("X:") + 2),
                                        split[2].IndexOf(" Y") - (split[2].IndexOf("X:") + 2)));
                                float posY =
                                    float.Parse(split[2].Substring(split[2].IndexOf("Y:") + 2,
                                        split[2].IndexOf("}") - (split[2].IndexOf("Y:") + 2)));
                                var tp = new TeleportEvent(split[1], new Vector2(posX, posY));
                                EventList.Add(tp);
                            }
                            break;
                    }
                }
            }
        }

        public class TeleportEvent
        {
            public readonly string TargetMapId;
            public Vector2 TargetPosition;

            public TeleportEvent(string targetMapId, Vector2 targetPosition)
            {
                TargetMapId = targetMapId;
                TargetPosition = targetPosition;
            }

            public override string ToString()
            {
                return string.Format("[{0}:{1}:{2}]", GetType().Name, TargetMapId, TargetPosition);
            }
        }

        public class MessageEvent
        {
            public string MessageId;
            public Image FaceImage;
            public string Message;


            public override string ToString()
            {
                return string.Format("[{0}:{1}]", GetType().Name, MessageId);
            }
        }
    }
}