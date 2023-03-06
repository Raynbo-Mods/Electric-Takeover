namespace Eco.ET.TechTree
{
    using System;
    using System.Collections.Generic;
    using Eco.Core.Items;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Mods.TechTree;
    using Eco.Gameplay.Skills;
    using Eco.Shared.Math;
    using Eco.ET.Components;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Items;
    using Eco.Gameplay.Systems.Tooltip;
    using Eco.Gameplay.Systems.NewTooltip;
    using Eco.Core.Controller;

    [Serialized]
    [LocDisplayName("Electric Truck")]
    [Weight(25000)]
    [Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
    public partial class ElectricTruckItem : WorldObjectItem<ElectricTruckObject>, IPersistentData
    {
        public override LocString DisplayDescription { get { return Localizer.DoStr("Modern truck for hauling sizable loads.\nruns on battery power!"); } }
        [Serialized, SyncToView, TooltipChildren, NewTooltipChildren(CacheAs.Instance)] public object PersistentData { get; set; }
    }
    [RequiresSkill(typeof(IndustrySkill), 3)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Electric Truck Item")]
    public partial class ElectricTruckRecipe : RecipeFamily
    {
        public ElectricTruckRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "Electric Truck",
                displayName: Localizer.DoStr("Electric Truck"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(TruckItem), 1, staticIngredient: true),
                    new IngredientElement(typeof(ElectricUpgradeKitItem), 2, staticIngredient: true),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<ElectricTruckItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 5;

            this.LaborInCalories = CreateLaborInCaloriesValue(1000, typeof(IndustrySkill));

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(ElectricTruckRecipe), start: 20, skillType: typeof(IndustrySkill));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Electric Truck"), recipeType: typeof(ElectricTruckRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(RoboticAssemblyLineObject), recipe: this);
        }

        partial void ModsPreInitialize();

        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(StandaloneAuthComponent))]
    [RequireComponent(typeof(BatterySupplyComponent))]
    [RequireComponent(typeof(BatteryConsumptionComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(TailingsReportComponent))]
    [RequireComponent(typeof(MovableLinkComponent))]
    [RequireComponent(typeof(VehicleComponent))]
    [RequireComponent(typeof(CustomTextComponent))]
    [RequireComponent(typeof(ModularStockpileComponent))]
    [RequireComponent(typeof(MinimapComponent))]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Electric Truck Item")]
    public partial class ElectricTruckObject : PhysicsWorldObject, IRepresentsItem
    {
        static ElectricTruckObject()
        {
            WorldObject.AddOccupancy<ElectricTruckObject>(new List<BlockOccupancy>(0));
        }
        public override bool PlacesBlocks => false;
        public override LocString DisplayName { get { return Localizer.DoStr("Electric Truck"); } }
        public Type RepresentedItemType { get { return typeof(ElectricTruckItem); } }

        private ElectricTruckObject() { }
        protected override void Initialize()
        {
            base.Initialize();
            this.GetComponent<CustomTextComponent>().Initialize(200);
            this.GetComponent<BatterySupplyComponent>().Initialize(1);
            this.GetComponent<BatteryConsumptionComponent>().Initialize(this.GetComponent<BatterySupplyComponent>(), 25);
            this.GetComponent<StockpileComponent>().Initialize(new Vector3i(2, 2, 3));
            this.GetComponent<PublicStorageComponent>().Initialize(36, 8000000);
            this.GetComponent<MinimapComponent>().InitAsMovable();
            this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
            this.GetComponent<VehicleComponent>().Initialize(20, 2, 2);
        }
    }
}
