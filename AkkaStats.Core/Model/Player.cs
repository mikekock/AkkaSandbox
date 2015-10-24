using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaStats.Core
{
    public class Player : IEntity<Guid>
    {
        public Player(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        
        public Guid Id { get; set; }
        public string Name { get; private set; }
        public string Url { get; private set; }

        public void MoveWebsite(string url)
        {
            Url = url;
        }
    }

    public class Hitter : Player
    {
        public Hitter(Guid id, string name) : base(id, name)
        {
           
        }

        public int HomeRuns { get; private set; }

        public void HitHomeRun()
        {
            ++HomeRuns;
        }
    }

    public class Pitcher : Player
    {
        public Pitcher(Guid id, string name) : base(id, name)
        {

        }

        public int Wins { get; private set; }

        public void WonGame()
        {
            ++Wins;
        }
    }
}
