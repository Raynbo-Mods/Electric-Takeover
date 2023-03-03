namespace Eco.Mods.TechTree
{
    using System;
    using System.Collections.Generic;
    using Eco.Core.Items;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Housing;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Modules;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Property;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.Tooltip;
    using Eco.Shared.Math;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Items;
    using Eco.Gameplay.Housing.PropertyValues;
    using Eco.Gameplay.Systems.NewTooltip;
    using Eco.Core.Controller;

    [Serialized]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(MinimapComponent))]
    [RequireComponent(typeof(LinkComponent))]
    [RequireComponent(typeof(CraftingComponent))]
    [RequireComponent(typeof(PowerConsumptionComponent))]
    [RequireComponent(typeof(PowerGridComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(SolidAttachedSurfaceRequirementComponent))]
    [RequireComponent(typeof(PluginModulesComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [RequireRoomContainment]
    [RequireRoomVolume(25)]
    [RequireRoomMaterialTier(0.8f, typeof(IndustryLavishReqTalent), typeof(IndustryFrugalReqTalent))]
    [Ecopedia("Work Stations", "Craft Tables", subPageName: "WainwrightTable Item")]
    public partial class ElectricUpgradeTableObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(ElectricUpgradeTableItem);
        public override LocString DisplayName => Localizer.DoStr("Electric Upgrade Table");

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Crafting"));
            this.GetComponent<PowerConsumptionComponent>().Initialize(200f);
            this.GetComponent<PowerGridComponent>().Initialize(20f, new ElectricPower());
            this.GetComponent<HousingComponent>().HomeValue = ElectricUpgradeTableItem.homeValue;
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Electric Upgrade Table")]
    [Ecopedia("Work Stations", "Craft Tables", createAsSubPage: true)]
    [AllowPluginModules(Tags = new[] { "ModernUpgrade" }, ItemTypes = new[] { typeof(IndustryUpgradeItem) })] //noloc
    public partial class ElectricUpgradeTableItem : WorldObjectItem<ElectricUpgradeTableObject>, IPersistentData
    {
        public override LocString DisplayDescription => Localizer.DoStr("A workbench for upgrading things to electric");


        public override DirectionAxisFlags RequiresSurfaceOnSides { get; } = 0
                    | DirectionAxisFlags.Down
                ;
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = new HomeFurnishingValue()
        {
            Category = HousingConfig.GetRoomCategory("Industrial"),
            TypeForRoomLimit = Localizer.DoStr(""),
        };

        [Serialized, SyncToView, TooltipChildren, NewTooltipChildren(CacheAs.Instance)] public object? PersistentData { get; set; }
    }

    [RequiresSkill(typeof(ElectronicsSkill), 2)]
    [Ecopedia("Work Stations", "Craft Tables", subPageName: "Electric Upgrade Table")]
    public partial class ElectricUpgradeTableRecipe : RecipeFamily
    {
        public ElectricUpgradeTableRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "Electric Upgrade Table",  //noloc
                displayName: Localizer.DoStr("Electric Upgrade Table"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(SmallBatteryItem), 10),
                    new IngredientElement(typeof(SteelBarItem), 10),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<ElectricUpgradeTableItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 5;

            this.LaborInCalories = CreateLaborInCaloriesValue(180, typeof(ElectronicsSkill));

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(ElectricUpgradeTableRecipe), start: 5, skillType: typeof(ElectronicsSkill), typeof(ElectronicsFocusedSpeedTalent), typeof(ElectronicsParallelSpeedTalent));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Electric Upgrade Table"), recipeType: typeof(ElectricUpgradeTableRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(ElectricMachinistTableObject), recipe: this);
        }

        partial void ModsPreInitialize();

        partial void ModsPostInitialize();
    }
}
