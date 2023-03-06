using Eco.Gameplay.Items;
using Eco.Shared.Items;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using Eco.ET.TechTree;
using Eco.Shared.IoC;
using Eco.ET.Utility;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Gameplay.Systems.Tooltip;
using Eco.Gameplay.Systems.NewTooltip.TooltipLibraryFiles;

namespace Eco.ET.ToolTips
{
    [TooltipLibrary] public static class ElectricTooltipLibrary
    {

        public static void Initialize() => BatteryItem.ChargeChanged.Add(MarkTooltipChargeDirty);
        static void MarkTooltipChargeDirty(BatteryItem item) => ServiceHolder<ITooltipSubscriptions>.Obj.MarkTooltipPartDirty(nameof(TooltipCharge), instance: item);

        [NewTooltip(CacheAs.Instance | CacheAs.User, 11, TTCat.Controls, flags: TTFlags.ClearCacheForAllUsers)]
        public static LocString TooltipCharge(this BatteryItem item)
        {
            var s = new LocStringBuilder();
            double mins;
            double hours;
            switch (item.batteryStatus)
            {
                case BatteryStatusTypes.Charged:
                    hours = Math.Floor((item.MaxCapacity - item.currentCharge) / Math.Abs(item.lastChangeWatts));
                    mins = Math.Floor(((item.MaxCapacity - item.currentCharge) / Math.Abs(item.lastChangeWatts) - hours) * 60);
                    s.Append(Localizer.DoStr($"Battery is charging at {Math.Round(item.lastChangeWatts, 2) + "w"} \nfull in {Text.Info(Text.Int(hours))} hours and {Text.Info(Text.Int(mins))} mins\nholding {Text.Info(Math.Round(item.currentCharge, 2) + " of max " + Text.Info(item.MaxCapacity) + " watt hours")}"));
                    break;
                case BatteryStatusTypes.Discharged:
                    hours = Math.Floor((item.MaxCapacity - item.currentCharge) / Math.Abs(item.lastChangeWatts));
                    mins = Math.Floor(((item.MaxCapacity - item.currentCharge) / Math.Abs(item.lastChangeWatts) - hours) * 60);
                    s.Append(Localizer.DoStr($"Battery is discharging at {Math.Round(item.lastChangeWatts, 2) + "w"} \nempty in {Text.Info(Text.Int(hours))} hours and {Text.Info(Text.Int(mins))} mins\nholding {Text.Info(Math.Round(item.currentCharge, 2) + " of max " + Text.Info(item.MaxCapacity) + " watt hours")}"));
                    break;
                case BatteryStatusTypes.LowDischargeRate:
                    s.Append(Localizer.DoStr($"Battery output of {Text.Info(item.MaxDischargeRate)+"w"} cannot power this object\nholding {Text.Info(Math.Round(item.currentCharge, 2) + " of max " + Text.Info(item.MaxCapacity) + " watt hours")}"));
                    break;
                case BatteryStatusTypes.LowChargeRate:
                    s.Append(Localizer.DoStr($"Battery charge rate of {Text.Info(item.MaxChargeRate) + "w"} cannot charge here\nholding {Text.Info(Math.Round(item.currentCharge, 2) + " of max " + Text.Info(item.MaxCapacity) + " watt hours")}"));
                    break;
                case BatteryStatusTypes.Full:
                    s.Append(Localizer.DoStr($"Battery is full \nholding {Text.Info(item.MaxCapacity) + " watt hours"} of energy"));
                    break;
                case BatteryStatusTypes.Empty:
                    s.Append(Localizer.DoStr($"Battery is empty \nholding {Text.Info("0 of max "+Text.Int(item.MaxCapacity) + " watt hours")}"));
                    break;
                case BatteryStatusTypes.Idle:
                    s.Append(Localizer.DoStr($"Battery is idle \nholding {Text.Info(Math.Round(item.currentCharge, 2) + " of max " + Text.Info(item.MaxCapacity) + " watt hours")}"));
                    break;
            }
            return new TooltipSection(Localizer.DoStr("Battery Info"), s.ToLocString());
        }
    }
}
