using FishingLogbook.Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingLogbook.UI
{
    public class TooltipPatch
    {
        public static void OnTooltipDisplay(Bookcase.Events.ItemTooltipEvent e, FishingLog fishingLog)
        {
            if (e.Item.Category == StardewValley.Object.FishCategory)
            {
                AggregateCatchConditions conditions = fishingLog.Conditions.FirstOrDefault(c => c.ObjectID == e.Item.ParentSheetIndex);
                if (conditions != null)
                {
                    e.AddLine("Conditions: ");
                    if (conditions.Rain && !conditions.NoRain)
                        e.AddLine("-Rain");
                    else if (conditions.Rain && conditions.NoRain)
                        e.AddLine("-Any Weather");
                    else if (!conditions.Rain && conditions.NoRain)
                        e.AddLine("-Sunshine");
                    if (conditions.Day && conditions.Night)
                        e.AddLine("-Any Time");
                    else if (conditions.Day && !conditions.Night)
                        e.AddLine("-Daytime");
                    else
                        e.AddLine("-Nighttime");
                    e.AddLine("-" + conditions.Seasons.Select(c => c.Substring(0, 1).ToUpper() + c.Substring(1)).Aggregate((c, x) => c + "\n-" + x));
                    e.AddLine("Found at: ");
                    e.AddLine("-" + conditions.Locations.Aggregate((c, x) => c + "\n-" + x));
                }
                else
                    e.AddLine("Nothing in the fishing logbook.");
            }
        }
    }
}
