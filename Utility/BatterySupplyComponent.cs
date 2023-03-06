namespace Eco.ET.Components
{
    using System.ComponentModel;
    using System.Linq;
    using Eco.Gameplay.Components;
    using Eco.ET.TechTree;
    using Eco.Core.Controller;
    using Eco.Core.Utils;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Players;
    using Eco.Shared.Localization;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Shared.Serialization;
    using Eco.Shared.IoC;
    using static Eco.Gameplay.Items.AuthorizationInventory;
    using Eco.Mods.TechTree;
    using Eco.ET.Utility;
    using Eco.Shared.Utils;
    using Eco.Gameplay.Economy;
    using Eco.Shared.View;
    using Eco.Shared.Networking;
    using static Eco.Gameplay.Disasters.DisasterPlugin;
    using Eco.Core.Systems;

    [Serialized]
    [RequireComponent(typeof(StatusComponent))]
    [RequireComponent(typeof(StorageComponent))]
    [CreateComponentTab("Battery"), LocDescription("Manage battery supply"), Priority(-100)]
    public class BatterySupplyComponent : StorageComponent, INotifyPropertyChanged, IController, IViewController
    {
        readonly object BatteryLock = new();

        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.UI;

        [Serialized, SyncToView(propertyName: "Energy", Flags=SyncFlags.MustRequest)] public float Energy { get; private set; }
        [Serialized, SyncToView(propertyName: "ConsumptionRate", Flags=SyncFlags.MustRequest)] public float ConsumptionRate { get; private set; }
        [Serialized] private Inventory BatterySupply { get; set; }
        [Serialized] public BatteryItem? CurrentBattery { get; private set; }
        [Serialized] public float TotalWattHours { get; private set; }
        [Serialized] public BatteryStatusTypes BatteryStatus { get; private set; }
        [SyncToView] public bool ForceActiveTab => true;
        public override Inventory Inventory => BatterySupply;
        public bool Discharging = false;
        public bool Charging = false;

        public override bool Enabled => this.CurrentBattery != null && this.BatteryStatus != BatteryStatusTypes.LowDischargeRate && (( this.BatteryStatus != BatteryStatusTypes.Empty && this.Discharging) || (this.BatteryStatus != BatteryStatusTypes.Full && this.Charging) || (this.BatteryStatus != BatteryStatusTypes.Full && this.BatteryStatus != BatteryStatusTypes.Empty));

        private float energyUsedLastTick;

        private StatusElement status;

        public BatterySupplyComponent() { }

        public void Initialize(int slots)
        {
            if (this.BatterySupply == null)
                this.BatterySupply = new AuthorizationInventory(slots, AuthorizationFlags.AuthedMayAdd | AuthorizationFlags.AuthedMayRemove);

            this.BatterySupply.SetOwner(this.Parent);

            this.BatterySupply.AddInvRestriction(new TagRestriction(new string[] { "Batteries" }));

            this.TotalWattHours = this.BatterySupply.NonEmptyStacks.Sum(stack =>
            {
                return ((BatteryItem)stack.Item).currentCharge;
            });
            this.status = this.Parent.GetComponent<StatusComponent>().CreateStatusElement();
            this.BatterySupply.OnChanged.Add(this.OnBatteryAdded);
            this.OnBatteryAdded(null);
            this.UpdateStatus();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Destroy()
        {
            base.Destroy();

            this.BatterySupply.OnChanged.Remove(this.OnBatteryAdded);
        }

        void OnBatteryAdded(User? user)
        {
            bool taken = true;
            this.TotalWattHours = this.BatterySupply.NonEmptyStacks.Sum(stack => {
                BatteryItem battery = (BatteryItem)stack.Item;
                if (battery == this.CurrentBattery)
                {
                    taken = false;
                }
                return battery.currentCharge;
            });
            if (taken)
            {
                this.CurrentBattery = null;
            }

            lock (this.BatteryLock)
            {
                this.LoadBattery();
                this.UpdateStatus();
            }
        }


        void LoadBattery()
        {
            this.BatterySupply.Modify(changeSet =>
            {
                this.BatterySupply.NonEmptyStacks.ForEach<ItemStack>((itemStack) => {
                    if (CurrentBattery == null)
                    {
                        if (itemStack.Item != null)
                        {
                            this.CurrentBattery = (BatteryItem)itemStack.Item;
                            this.Energy = this.CurrentBattery.currentCharge;
                        }
                    }
                    else
                    {
                        (this.BatteryStatus, this.Energy) = this.CurrentBattery.Update();
                    }
                });
            });
        }

        void UpdateStatus()
        {
            LocString operational = Localizer.DoStr("Operational");
            LocString error = Localizer.DoStr("Error");
            double mins;
            double hours;
            if (this.CurrentBattery == null)
            {
                operational = Localizer.DoStr("wait what?? how is this working without a battery??");
                error = Localizer.DoStr("missing battery");
            } 
            else
            {
                switch (this.BatteryStatus)
                {
                    case BatteryStatusTypes.Charged:
                        hours = Math.Floor((this.CurrentBattery.MaxCapacity - this.CurrentBattery.currentCharge) / Math.Abs(this.CurrentBattery.lastChangeWatts));
                        mins = Math.Floor(((this.CurrentBattery.MaxCapacity - this.CurrentBattery.currentCharge) / Math.Abs(this.CurrentBattery.lastChangeWatts) - hours) * 60);
                        operational = Localizer.DoStr($"Battery is charging at {Math.Round(this.CurrentBattery.lastChangeWatts, 2) + "w"} \nfull in {Text.Info(Text.Int(hours))} hours and {Text.Info(Text.Int(mins))} mins\nholding {Text.Info(Math.Round(this.CurrentBattery.currentCharge, 2) + " of max " + Text.Info(this.CurrentBattery.MaxCapacity) + " watt hours")}");
                        break;
                    case BatteryStatusTypes.Discharged:
                        hours = Math.Floor((this.CurrentBattery.currentCharge) / Math.Abs(this.CurrentBattery.lastChangeWatts));
                        mins = Math.Floor(((this.CurrentBattery.currentCharge) / Math.Abs(this.CurrentBattery.lastChangeWatts) - hours) * 60);
                        operational = Localizer.DoStr($"Battery is discharging at {Math.Round(this.CurrentBattery.lastChangeWatts, 2) + "w"} \nempty in {Text.Info(Text.Int(hours))} hours and {Text.Info(Text.Int(mins))} mins\nholding {Text.Info(Math.Round(this.CurrentBattery.currentCharge, 2) + " of max " + Text.Info(this.CurrentBattery.MaxCapacity) + " watt hours")}");
                        break;
                    case BatteryStatusTypes.LowDischargeRate:
                        operational = Localizer.DoStr($"Battery output of {Text.Info(this.CurrentBattery.MaxDischargeRate) + "w"} cannot power this object\nholding {Text.Info(Math.Round(this.CurrentBattery.currentCharge, 2) + " of max " + Text.Info(this.CurrentBattery.MaxCapacity) + " watt hours")}");
                        break;
                    case BatteryStatusTypes.LowChargeRate:
                        operational = Localizer.DoStr($"Battery charge rate of {Text.Info(this.CurrentBattery.MaxChargeRate) + "w"} cannot charge here\nholding {Text.Info(Math.Round(this.CurrentBattery.currentCharge, 2) + " of max " + Text.Info(this.CurrentBattery.MaxCapacity) + " watt hours")}");
                        break;
                    case BatteryStatusTypes.Full:
                        operational = Localizer.DoStr($"Battery is full \nholding {Text.Info(this.CurrentBattery.MaxCapacity) + " watt hours"} of energy");
                        break;
                    case BatteryStatusTypes.Empty:
                        operational = Localizer.DoStr($"Battery is empty \nholding {Text.Info("0 of max " + Text.Int(this.CurrentBattery.MaxCapacity) + " watt hours")}");
                        break;
                    case BatteryStatusTypes.Idle:
                        operational = Localizer.DoStr($"Battery is idle \nholding {Text.Info(Math.Round(this.CurrentBattery.currentCharge, 2) + " of max " + Text.Info(this.CurrentBattery.MaxCapacity) + " watt hours")}");
                        break;
                }
                error = operational;
            }
            this.status.SetStatusMessage(this.Enabled, operational, error);
        }
        public override void Tick()
        {
            this.UpdateStatus();
            base.Tick();
        }
        public override void LateTick()
        {
            this.ConsumptionRate = this.energyUsedLastTick / ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime;
            this.energyUsedLastTick = 0;
        }
        public void Charge(float time, float watts)
        {
            this.Charging = true;
            this.Discharging = false;
            lock (this.BatteryLock)
            {
                if (this.CurrentBattery != null && this.CurrentBattery.currentCharge < this.CurrentBattery.MaxCapacity && this.Enabled && this.Parent.GetOrCreateComponent<PowerConsumptionComponent>().Enabled && this.Parent.GetOrCreateComponent<OnOffComponent>().On)
                {
                    this.BatteryStatus = this.CurrentBattery.Charge((time * watts), time);
                    this.Energy = CurrentBattery.currentCharge;
                } 
                else if (this.CurrentBattery != null)
                {
                    (this.BatteryStatus, this.Energy) = this.CurrentBattery.Update();
                }
            }
        }
        
        public void Discharge(float time, float watts)
        {
            this.Discharging = true;
            this.Charging = false;
            lock (this.BatteryLock)
            {
                if (this.CurrentBattery != null && this.CurrentBattery.currentCharge > 0 && this.Enabled && this.Parent.Operating)
                {
                    this.BatteryStatus = this.CurrentBattery.Discharge((time * watts), time);
                    this.Energy = CurrentBattery.currentCharge;
                    this.energyUsedLastTick = time * watts;
                }
                else
                {
                    if (this.CurrentBattery != null)
                    {
                        (this.BatteryStatus, this.Energy) = this.CurrentBattery.Update();
                    } else
                    {
                    }
                    this.Energy = 0;
                    this.energyUsedLastTick = 0;
                }
            }
        }
    }
}
