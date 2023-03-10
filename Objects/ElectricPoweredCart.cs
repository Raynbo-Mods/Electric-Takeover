// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.
// <auto-generated from VehicleTemplate.tt />

namespace Eco.ET.TechTree
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Eco.Core.Items;
    using Eco.ET.Components;
    using Eco.Mods.TechTree;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Components.VehicleModules;
    using Eco.Gameplay.GameActions;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Shared.Math;
    using Eco.Shared.Networking;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Shared.Items;
    using Eco.Gameplay.Systems.Tooltip;
    using Eco.Gameplay.Systems.NewTooltip;
    using Eco.Core.Controller;

    [Serialized]
    [LocDisplayName("Electric Powered Cart")]
    [Weight(15000)]
    [Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
    public partial class ElectricPoweredCartItem : WorldObjectItem<ElectricPoweredCartObject>, IPersistentData
    {
        public override LocString DisplayDescription { get { return Localizer.DoStr("Large cart for hauling sizable loads.\nRuns on electric power!"); } }
        [Serialized, SyncToView, TooltipChildren, NewTooltipChildren(CacheAs.Instance)] public object PersistentData { get; set; }
    }

    /// <summary>
    /// <para>Server side recipe definition for "PoweredCart".</para>
    /// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization. 
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [RequiresSkill(typeof(BasicEngineeringSkill), 6)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "ElectricPoweredCart Item")]
    public partial class ElectricPoweredCartRecipe : RecipeFamily
    {
        public ElectricPoweredCartRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "ElectricPoweredCart",  //noloc
                displayName: Localizer.DoStr("Electric Powered Cart"),

                // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
                // type of the item, the amount of the item, the skill required, and the talent used.
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(ElectricUpgradeKitItem), 1, true),
                    new IngredientElement(typeof(PoweredCartItem), 1, true),
                },

                // Define our recipe output items.
                // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
                // to create.
                items: new List<CraftingElement>
                {
                    new CraftingElement<ElectricPoweredCartItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 4; // Defines how much experience is gained when crafted.

            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(400, typeof(BasicEngineeringSkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(ElectricPoweredCartRecipe), start: 15, skillType: typeof(BasicEngineeringSkill));

            // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "Powered Cart"
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Electric Powered Cart"), recipeType: typeof(ElectricPoweredCartRecipe));
            this.ModsPostInitialize();

            // Register our RecipeFamily instance with the crafting system so it can be crafted.
            CraftingComponent.AddRecipe(tableType: typeof(WainwrightTableObject), recipe: this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();

        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
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
    [RequireComponent(typeof(MinimapComponent))]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "ElectricPoweredCart Item")]
    public partial class ElectricPoweredCartObject : PhysicsWorldObject, IRepresentsItem
    {
        static ElectricPoweredCartObject()
        {
            WorldObject.AddOccupancy<ElectricPoweredCartObject>(new List<BlockOccupancy>(0));
        }
        public override TableTextureMode TableTexture => TableTextureMode.Metal;
        public override bool PlacesBlocks => false;
        public override LocString DisplayName { get { return Localizer.DoStr("Electric Powered Cart"); } }
        public Type RepresentedItemType { get { return typeof(ElectricPoweredCartItem); } }

        private ElectricPoweredCartObject() { }
        protected override void Initialize()
        {
            base.Initialize();
            this.GetComponent<CustomTextComponent>().Initialize(200);
            this.GetComponent<BatterySupplyComponent>().Initialize(1);
            this.GetComponent<BatteryConsumptionComponent>().Initialize(this.GetComponent<BatterySupplyComponent>(), 25);
            this.GetComponent<PublicStorageComponent>().Initialize(18, 3500000);
            this.GetComponent<MinimapComponent>().InitAsMovable();
            this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
            this.GetComponent<VehicleComponent>().Initialize(12, 1.5f, 1);
        }
    }
}
