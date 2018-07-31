using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingLogbook.Tracker
{
    public class FishingLog
    {
        public FishingLog()
        {
            Logbook = new LinkedList<CaughtFish>();
            Conditions = new HashSet<AggregateCatchConditions>();
        }
        public HashSet<AggregateCatchConditions> Conditions
        {
            get;
            private set;
        }
        public LinkedList<CaughtFish> Logbook
        {
            get;
            private set;
        }
        public void RecordCatch(int fishID, int fishSize, int fishQuality)
        {
            Logbook.AddLast(new CaughtFish(fishID,fishQuality,Game1.timeOfDay,fishSize, new SDate(Game1.dayOfMonth, Game1.currentSeason, Game1.year),Game1.player.currentLocation.Name, Game1.isRaining));
            AggregateCatchConditions aggregate = Conditions.FirstOrDefault(c => c.ObjectID == fishID);
            if (aggregate != null)
                aggregate.Add(Logbook.Last().Conditions);
            else
                Conditions.Add(new AggregateCatchConditions(fishID, Logbook.Last().Conditions));
        }
    }
}
