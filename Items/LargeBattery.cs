using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
namespace Eco.Mods.TechTree
{

    [Serialized]
    [LocDisplayName("Large Battery")]
    [Weight(1000)]
    [Tag("Batteries")]
    [Ecopedia("Items", "Tools", createAsSubPage: true)]
    public partial class LargeBatteryItem : BatteryItem
    {
        public override int MaxChargeRate => 100;
        public override int MaxDischargeRate => 50;
        public override float IdleDischargeRate => 0.05f;
        public override int MaxCapacity => 150;
    }
    [RequiresSkill(typeof(ElectronicsSkill), 5)]
    public partial class LargeBatteryRecipe : RecipeFamily
    {
        public LargeBatteryRecipe()
        {
            Recipe recipe_1 = new Recipe();
            recipe_1.Init(
                "Large Battery",
                Localizer.DoStr("Large Battery"),
                new List<IngredientElement> { new IngredientElement(typeof(CopperPlateItem), 15, staticIngredient: true), new IngredientElement(typeof(IronPlateItem), 15, staticIngredient: true), new IngredientElement(typeof(SubstrateItem), 8), new IngredientElement(typeof(SyntheticRubberItem), 15) },
                new List<CraftingElement> { new CraftingElement<LargeBatteryItem>(1) }
            );
            this.Recipes = new List<Recipe> { recipe_1 };
            this.ExperienceOnCraft = 3f;
            this.LaborInCalories = CreateLaborInCaloriesValue(300f, typeof(ElectronicsSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(LargeBatteryRecipe), 4f, typeof(ElectronicsSkill));
            this.Initialize(Localizer.DoStr("Large Battery"), typeof(LargeBatteryRecipe));
            CraftingComponent.AddRecipe(typeof(ElectronicsAssemblyObject), this);
        }
    }
}