namespace Eco.Mods.TechTree
{
    using System;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Skills;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;

    [Serialized]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(BatterySupplyComponent))]
    [RequireComponent(typeof(BatteryChargingComponent))]
    [RequireComponent(typeof(PowerConsumptionComponent))]
    [RequireComponent(typeof(PowerGridComponent))]
    public partial class BatteryChargerObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(BatteryChargerItem);
        public override LocString DisplayName => Localizer.DoStr("Battery Charger");
        static BatteryChargerObject()
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
            this.GetComponent<BatteryChargingComponent>().Initialize(this.GetComponent<BatterySupplyComponent>(), 20, 1000);
        }
    }

    [Serialized]
    [LocDisplayName("Battery Charger")]
    public partial class BatteryChargerItem : WorldObjectItem<BatteryChargerObject>
    {
        public override LocString DisplayDescription => Localizer.DoStr("A thing for charging your batteries");
    }
    [RequiresSkill(typeof(ElectronicsSkill), 1)]
    public partial class BatteryChargerRecipe : RecipeFamily
    {
        public BatteryChargerRecipe()
        {
            Recipe recipe_1 = new Recipe();
            recipe_1.Init(
                "Battery Charger",
                Localizer.DoStr("Battery Charger"),
                new List<IngredientElement> { new IngredientElement(typeof(SteelGearboxItem), 20, staticIngredient: false), new IngredientElement(typeof(CopperWiringItem), 100, staticIngredient: false), new IngredientElement(typeof(SteelAxleItem), 1, staticIngredient: true), new IngredientElement(typeof(IronWheelItem), 4, staticIngredient: true), new IngredientElement(typeof(LightBulbItem), 1, staticIngredient: true), new IngredientElement(typeof(AdvancedCircuitItem), 5, staticIngredient: true) },
                new List<CraftingElement> { new CraftingElement<BatteryChargerItem>(1) }
            );
            this.Recipes = new List<Recipe> { recipe_1 };
            this.ExperienceOnCraft = 5f;
            this.LaborInCalories = CreateLaborInCaloriesValue(100f, typeof(ElectronicsSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(BatteryChargerRecipe), 20f, typeof(ElectronicsSkill));
            this.Initialize(Localizer.DoStr("Battery Charger"), typeof(SmallBatteryRecipe));
            CraftingComponent.AddRecipe(typeof(RoboticAssemblyLineObject), this);
        }
    }
}