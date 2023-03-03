namespace Eco.Gameplay.Components
{
    using Eco.Core.Controller;
    using Eco.Gameplay.Objects;
    using Eco.Shared.IoC;
    using Eco.Shared.Serialization;
    using Eco.Shared.Localization;
    using Eco.RM.Utility;

    [Serialized]
    [LocDisplayName("Battery Charge")]
    [RequireComponent(typeof(BatteryChargingComponent)), NoIcon]
    [RequireComponent(typeof(PowerConsumptionComponent))]
    [RequireComponent(typeof(PowerGridComponent))]
    public class BatteryChargingComponent : WorldObjectComponent
    {
        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.UI;
        private int powerCost = 0;
        private float lastTickPowerCost = 0;
        public float WattsPerSecond = 20;

        private BatterySupplyComponent fuelSupply;

        public BatteryChargingComponent() { }

        public void Initialize(BatterySupplyComponent supply, int radius, float watts)
        {
            this.fuelSupply = supply;
            this.WattsPerSecond = watts;
            this.Parent.GetOrCreateComponent<PowerConsumptionComponent>().Initialize(powerCost);
            this.Parent.GetOrCreateComponent<PowerGridComponent>().Initialize(radius, new ElectricPower());

        }

        public override void Tick()
        {
            lastTickPowerCost = powerCost;
            if (this.fuelSupply.CurrentBattery != null)
            {
                this.fuelSupply.Charge(ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime, this.WattsPerSecond);
                this.powerCost = (int)this.fuelSupply.CurrentBattery.lastChangeWatts;
                if (lastTickPowerCost != powerCost)
                {
                    if (powerCost > 0)
                    {
                        this.Parent.GetOrCreateComponent<PowerConsumptionComponent>().OverridePowerConsumption(powerCost + 50);
                    }
                    else
                    {
                        this.Parent.GetOrCreateComponent<PowerConsumptionComponent>().OverridePowerConsumption(0);
                    }
                }
            }
        }
    }
}
