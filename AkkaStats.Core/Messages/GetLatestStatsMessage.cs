using System;

namespace AkkaStats.Core.Messages
{

    public class PlayerMessage
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class GetLatestStatsMessage
    {
        public int Id { get; private set; }
        public string Player { get; private set; }
        public int Points { get; private set; }

        public GetLatestStatsMessage(int id, string player, int points)
        {
            Id = id;
            Player = player;
            Points = points;
        }

       
    }
}
