using C5;
using Eco.RM.Utility;
using Eco.Core.ElasticSearch;
using Eco.Core.Items;
using Eco.Core.Utils;
using Eco.Gameplay.EcopediaRoot;
using Eco.Gameplay.Items;
using Eco.Gameplay.Systems.NewTooltip.TooltipLibraryFiles;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using Eco.Simulation.Agents;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.IoC;
using Eco.Shared.View;
using Eco.Simulation.Time;
using Eco.Gameplay.Garbage;
using Eco.Gameplay.Interactions;
using Eco.Gameplay.GameActions;
using Eco.Core.Controller;

namespace Eco.Mods.TechTree
{
    [Serialized]
    [LocDisplayName("Battery")]
    [Weight(0)]
    [MaxStackSize(1)]
    [Ecopedia("Items", "Batteries")]
    [Category("Hidden")]
    public class BatteryItem : Item
    {
        public static readonly ThreadSafeAction<BatteryItem> ChargeChanged = new ThreadSafeAction<BatteryItem>();
        public override LocString DisplayDescription { get { return Localizer.DoStr("A item used to store electric energy."); } }
        public virtual int MaxChargeRate => 10;
        public virtual int MaxDischargeRate => 6;
        public virtual float IdleDischargeRate => 0.025f;
        public virtual int MaxCapacity => 1000;
        [Serialized] public float CurrentCharge = 0;
        [Notify] public float currentCharge
        {
            get => this.CurrentCharge;

            private set
            {
                if (value == this.CurrentCharge) return;
                this.CurrentCharge = value;
                ChargeChanged.Invoke(this);
            }
        }
        public float lastChangeWatts = 0;
        public float lastWH = 0;
        [Serialized] public BatteryStatusTypes BatteryStatus = BatteryStatusTypes.Idle;
        [Notify] public BatteryStatusTypes batteryStatus
        {
            get => this.BatteryStatus;
            private set
            {
                if (value == this.batteryStatus) return;
                this.BatteryStatus = value;
                ChargeChanged.Invoke(this);
            }
        }
        public BatteryItem() { }

        public BatteryStatusTypes Charge(float watts, float time)
        {
            float wh = 3600 / time;
            this.lastWH = wh;
            if (watts / wh > this.MaxChargeRate / wh)
            {
                watts = Math.Clamp(watts, 0, this.MaxChargeRate);
            }
            if (this.currentCharge + watts / wh < this.MaxCapacity)
            {
                this.lastChangeWatts = watts;
                this.batteryStatus = BatteryStatusTypes.Charged;
                this.currentCharge += watts / wh;
            }
            else if (this.currentCharge + watts / wh > this.MaxCapacity)
            {
                this.lastChangeWatts = watts;
                this.batteryStatus = BatteryStatusTypes.Full;
                this.currentCharge = this.MaxCapacity;
            }
            else
            {
                this.batteryStatus = BatteryStatusTypes.Full;
            }
            return this.batteryStatus;
        }
        public BatteryStatusTypes Discharge(float watts, float time)
        {
            float wh = 3600 / time;
            this.lastWH = wh;
            if (watts / wh > this.MaxDischargeRate / wh)
            {
                this.batteryStatus = BatteryStatusTypes.LowDischargeRate;
            }
            else if (this.currentCharge - watts / wh >= 0)
            {
                this.batteryStatus = BatteryStatusTypes.Discharged;
                this.lastChangeWatts = -watts;
                this.currentCharge -= watts / wh;
            }
            else if (this.currentCharge - watts / wh < 0)
            {
                this.batteryStatus = BatteryStatusTypes.Empty;
                this.lastChangeWatts = -watts;
                this.currentCharge = 0;
            }
            else
            {
                this.batteryStatus = BatteryStatusTypes.Empty;
            }
            return this.batteryStatus;
        }
        public (BatteryStatusTypes, float) Update()
        {
            if (currentCharge <= 0)
            {
                return (BatteryStatusTypes.Empty, 0);
            }
            else if (currentCharge >= MaxCapacity)
            {
                return (BatteryStatusTypes.Full, MaxCapacity);
            }
            else if (0 < currentCharge && currentCharge < MaxCapacity)
            {
                return (BatteryStatusTypes.Idle, currentCharge);
            }
            return (BatteryStatusTypes.Empty, 0);
        }
    }
}
