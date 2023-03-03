using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
namespace Eco.Mods.TechTree
{

    [Serialized]
    [LocDisplayName("Nuclear Battery")]
    [Weight(2000)]
    [Tag("Batteries")]
    [Ecopedia("Items", "Tools", createAsSubPage: true)]
    public partial class NuclearBatteryItem : BatteryItem
    {
        public override int MaxChargeRate => 160;
        public override int MaxDischargeRate => 80;
        public override float IdleDischargeRate => 0.02f;
        public override int MaxCapacity => 240;
    }
    [RequiresSkill(typeof(ElectronicsSkill), 7)]
    public partial class NuclearBatteryRecipe : RecipeFamily
    {
        public NuclearBatteryRecipe()
        {
            Recipe recipe_1 = new Recipe();
            recipe_1.Init(
                "Nuclear Battery",
                Localizer.DoStr("Nuclear Battery"),
                new List<IngredientElement> { new IngredientElement(typeof(CopperPlateItem), 30, staticIngredient: true), new IngredientElement(typeof(IronPlateItem), 30, staticIngredient: true), new IngredientElement(typeof(SubstrateItem), 10), new IngredientElement(typeof(SyntheticRubberItem), 20) },
                new List<CraftingElement> { new CraftingElement<NuclearBatteryItem>(1) }
            );
            this.Recipes = new List<Recipe> { recipe_1 };
            this.ExperienceOnCraft = 3f;
            this.LaborInCalories = CreateLaborInCaloriesValue(300f, typeof(ElectronicsSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(NuclearBatteryRecipe), 4f, typeof(ElectronicsSkill));
            this.Initialize(Localizer.DoStr("Nuclear Battery"), typeof(NuclearBatteryRecipe));
            CraftingComponent.AddRecipe(typeof(ElectronicsAssemblyObject), this);
        }
    }
}