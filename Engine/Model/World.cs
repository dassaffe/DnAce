using System.Collections.Generic;

namespace Engine.Model
{
    public class World
    {
        public List<Area> Areas { get; private set; }
        public List<Quest> Quests { get; private set; }
        public World()
        {
            Areas = new List<Area>();
            Quests = new List<Quest>();
        }
    }
}
