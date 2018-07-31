using Bookcase.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishingLogbook.Tracker;
using FishingLogbook.UI;

namespace FishingLogbook
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += LoadFishingLog;
            SaveEvents.AfterSave += SaveFishingLog;
            BookcaseEvents.OnTooltip.Add((e) => TooltipPatch.OnTooltipDisplay(e, FishingLog), EventBus<TooltipEvent>.Priority.Low);
            BookcaseEvents.AfterFishCaught.Add((e) =>
            {
                if (e.fishSize == -1)
                    return;
                Monitor.Log($"Fish caught {e.fishID} :: {e.fishSize}");
                FishingLog.RecordCatch(e.fishID, e.fishSize, e.fishQuality);
                SaveFishingLog(null, null);
            });
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
