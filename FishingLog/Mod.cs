using Bookcase.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingLog
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += LoadFishingLog;
            SaveEvents.AfterSave += SaveFishingLog;
            BookcaseEvents.AfterFishCaught.Add((e) =>
            {
                if (e.fishSize == -1)
                    return;
                Monitor.Log($"Fish caught {e.fishID} :: {e.fishSize}");
                FishingLog.RecordCatch(e.fishID, e.fishSize, e.fishQuality);
                SaveFishingLog(null, null);
            });
            BookcaseEvents.OnTooltip.Add(OnTooltip, EventBus<TooltipEvent>.Priority.Low);
        }
        private void OnTooltip(TooltipEvent e)
        {
            if(e.Item.Category == StardewValley.Object.FishCategory)
            {
                AggregateCatchConditions conditions = FishingLog.Conditions.FirstOrDefault(c => c.ObjectID == e.Item.ParentSheetIndex);
                if (conditions != null)
                {
                    e.AddLine("Conditions: ");
                    if(conditions.Rain && !conditions.NoRain)
                        e.AddLine("-Rain");
                    else if (conditions.Rain && conditions.NoRain)
                        e.AddLine("-Any Weather");
                    else if(!conditions.Rain && conditions.NoRain)
                        e.AddLine("-Sunshine");
                    if (conditions.Day && conditions.Night)
                        e.AddLine("-Any Time");
                    else if (conditions.Day && !conditions.Night)
                        e.AddLine("-Daytime");
                    else
                        e.AddLine("-Nighttime");
                    e.AddLine("-" + conditions.Seasons.Select(c=>c.Substring(0,1).ToUpper() + c.Substring(1)).Aggregate((c, x) =>  c + "\n-" + x));
                    e.AddLine("Found at: ");
                    e.AddLine("-" + conditions.Locations.Aggregate((c, x) => c + "\n-" + x));
                }
                else
                    e.AddLine("Nothing in the fishing logbook.");
            }
        }
        public FishingLog FishingLog
        {
            get;
            private set;
        }

        private void SaveFishingLog(object sender, EventArgs e)
        {
            Monitor.Log("Saving to " + FishingLogSavePath);
            Helper.WriteJsonFile(FishingLogSavePath, FishingLog);
            Monitor.Log("Complete");
        }

        private void LoadFishingLog(object sender, EventArgs e)
        {
            Monitor.Log("Attempting to load " + FishingLogSavePath);
            FishingLog = Helper.ReadJsonFile<FishingLog>(FishingLogSavePath);
            if (FishingLog == null)
            {
                Monitor.Log("FishingLog not found.");
                FishingLog = new FishingLog();
                return;
            }
            Monitor.Log("Load successful.");
        }

        private string FishingLogSavePath
        {
            get
            {
                return SaveInfoPath("Logbook");
            }
        }
        private string SaveInfoPath(string name)
        {
            return $"{Constants.CurrentSavePath}\\{Constants.SaveFolderName}_{ModManifest.UniqueID}_{name}.json";
        }
    }
}
