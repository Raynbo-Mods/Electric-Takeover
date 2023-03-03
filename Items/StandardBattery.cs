using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
namespace Eco.Mods.TechTree
{

    [Serialized]
    [LocDisplayName("Standard Battery")]
    [Weight(750)]
    [Tag("Batteries")]
    [Ecopedia("Items", "Tools", createAsSubPage: true)]
    public partial class StandardBatteryItem : BatteryItem
    {
        public override int MaxChargeRate => 45;
        public override int MaxDischargeRate => 30;
        public override float IdleDischargeRate => 0.08f;
        public override int MaxCapacity => 90;
    }
    [RequiresSkill(typeof(ElectronicsSkill), 3)]
    public partial class StandardBatteryRecipe : RecipeFamily
    {
        public StandardBatteryRecipe()
        {
            Recipe recipe_1 = new Recipe();
            recipe_1.Init(
                "Standard Battery",
                Localizer.DoStr("Standard Battery"),
                new List<IngredientElement> { new IngredientElement(typeof(CopperPlateItem), 10, staticIngredient: true), new IngredientElement(typeof(IronPlateItem), 10, staticIngredient: true), new IngredientElement(typeof(SubstrateItem), 5), new IngredientElement(typeof(SyntheticRubberItem), 10) },
                new List<CraftingElement> { new CraftingElement<StandardBatteryItem>(1) }
            );
            this.Recipes = new List<Recipe> { recipe_1 };
            this.ExperienceOnCraft = 1f;
            this.LaborInCalories = CreateLaborInCaloriesValue(100f, typeof(ElectronicsSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(StandardBatteryRecipe), 4f, typeof(ElectronicsSkill));
            this.Initialize(Localizer.DoStr("Standard Battery"), typeof(StandardBatteryRecipe));
            CraftingComponent.AddRecipe(typeof(ElectronicsAssemblyObject), this);
        }
    }
}