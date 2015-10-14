using System;

namespace AkkaStats.Persistance.Messages
{
    public class HubMessage
    {
        public DateTime TimeStamp { get; private set; }
        public string Message { get; private set; }

        public HubMessage(DateTime timeStamp, string message)
        {
            TimeStamp = timeStamp;
            Message = message;
        }

        public static HubMessage Create(DateTime timeStamp, string message)
        {
            return new HubMessage(timeStamp, message);
        }

    }
}