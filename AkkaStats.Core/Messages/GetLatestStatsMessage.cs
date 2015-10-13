namespace AkkaStats.Core.Messages
{
    public enum State
    {
        Create = 1,
        Read = 0,
        Update = 2,
        Delete = 3
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
