namespace Eco.Gameplay.Components
{
    using Eco.Core.Controller;
    using Eco.Gameplay.Objects;
    using Eco.Shared.IoC;
    using Eco.Shared.Serialization;
    using Eco.Gameplay.UI;
    using Eco.Shared.Localization;
    using System.Runtime.CompilerServices;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Gameplay.Players;

    [Serialized]
    [LocDisplayName("Battery Discharge")]
    [RequireComponent(typeof(BatterySupplyComponent)), NoIcon]
    public class BatteryConsumptionComponent : WorldObjectComponent
    {
        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.UI;
        [SyncToView] public float WattsPerSecond { get; set; }

        private BatterySupplyComponent fuelSupply;

        public BatteryConsumptionComponent() {  }

        public void Initialize(BatterySupplyComponent supply, float WattsPerHour)
        {
            this.fuelSupply = supply;
            this.WattsPerSecond = WattsPerHour;
            
        }

        public override void Tick()
        {
            this.fuelSupply.Discharge(ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime, this.WattsPerSecond);
        }
    }
}
