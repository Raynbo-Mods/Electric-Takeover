using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System.ComponentModel;

namespace Eco.Mods.TechTree
{

    [Serialized]
    [LocDisplayName("Small Battery")]
    [Weight(500)]
    [Tag("Batteries")]
    [Category("Power")]
    [Ecopedia("Items", "Tools", createAsSubPage: true)]
    public partial class SmallBatteryItem : BatteryItem
    {
        public override int MaxChargeRate => 30;
        public override int MaxDischargeRate => 15;
        public override float IdleDischargeRate => 0.1f;
        public override int MaxCapacity => 45;
    }
    [RequiresSkill(typeof(ElectronicsSkill), 1)]
    public partial class SmallBatteryRecipe : RecipeFamily
    {
        public SmallBatteryRecipe()
        {
            Recipe recipe_1 = new Recipe();
            recipe_1.Init(
                "Small Battery",
                Localizer.DoStr("Small Battery"),
                new List<IngredientElement> { new IngredientElement(typeof(CopperPlateItem), 5, staticIngredient: true), new IngredientElement(typeof(IronPlateItem), 5, staticIngredient: true), new IngredientElement(typeof(SubstrateItem), 2), new IngredientElement(typeof(SyntheticRubberItem), 5) },
                new List<CraftingElement> { new CraftingElement<SmallBatteryItem>(1) }
            );
            this.Recipes = new List<Recipe> { recipe_1 };
            this.ExperienceOnCraft = 1f;
            this.LaborInCalories = CreateLaborInCaloriesValue(100f, typeof(ElectronicsSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(SmallBatteryRecipe), 4f, typeof(ElectronicsSkill));
            this.Initialize(Localizer.DoStr("Small Battery"), typeof(SmallBatteryRecipe));
            CraftingComponent.AddRecipe(typeof(ElectronicsAssemblyObject), this);
        }
    }
}