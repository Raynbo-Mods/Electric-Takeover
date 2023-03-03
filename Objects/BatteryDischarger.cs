namespace Eco.Mods.TechTree
{
    using System;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;

    [Serialized]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(StorageComponent))]
    [RequireComponent(typeof(BatterySupplyComponent))]
    [RequireComponent(typeof(BatteryConsumptionComponent))]
    public partial class BatteryDischargerObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(BatteryDischargerItem);
        public override LocString DisplayName => Localizer.DoStr("Battery Discharger");
        static BatteryDischargerObject()
        {
            WorldObject.AddOccupancy<BatteryChargerObject>(new List<BlockOccupancy>(){
            new BlockOccupancy(new Vector3i(0, 0, 0)),
            new BlockOccupancy(new Vector3i(0, 1, 0)),
            new BlockOccupancy(new Vector3i(1, 0, 0)),
            new BlockOccupancy(new Vector3i(1, 1, 0)),
            new BlockOccupancy(new Vector3i(0, 0, 1)),
            new BlockOccupancy(new Vector3i(0, 1, 1)),
            new BlockOccupancy(new Vector3i(1, 0, 1)),
            new BlockOccupancy(new Vector3i(1, 1, 1)),
            });
        }
        protected override void Initialize()
        {
            this.GetComponent<BatterySupplyComponent>().Initialize(1);
            this.GetComponent<BatteryConsumptionComponent>().Initialize(this.GetComponent<BatterySupplyComponent>(), 70);
        }
    }

    [Serialized]
    [LocDisplayName("Battery Discharger")]
    public partial class BatteryDischargerItem : WorldObjectItem<BatteryDischargerObject>
    {
        public override LocString DisplayDescription => Localizer.DoStr("A thing for charging your batteries");
    }
}