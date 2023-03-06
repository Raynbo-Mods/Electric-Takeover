using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System.ComponentModel;
using Eco.Mods.TechTree;

namespace Eco.ET.TechTree
{
    [Serialized]
    [MaxStackSize(1)]
    [Weight(1000)]
    [Category("Power")]
    [LocDisplayName("Electric Upgrade Kit")]
    [Tag("Utility")]
    public partial class ElectricUpgradeKitItem : Item
    {
        public override LocString DisplayDescription => Localizer.DoStr("A kit used to upgrade fuel consuming items to clean electric energy!");
    }
    [RequiresSkill(typeof(ElectronicsSkill), 2)]
    public partial class ElectricUpgradeKitRecipe : RecipeFamily
    {
        public ElectricUpgradeKitRecipe()
        {
            Recipe recipe_1 = new Recipe();
            recipe_1.Init(
                "Electric Upgrade Kit",
                Localizer.DoStr("Electric Upgrade Kit"),
                new List<IngredientElement> { new IngredientElement(typeof(CopperWiringItem), 40), new IngredientElement(typeof(GoldFlakesItem), 20), new IngredientElement(typeof(SmallBatteryItem)), new IngredientElement(typeof(SyntheticRubberItem), 20), new IngredientElement(typeof(CopperPlateItem), 10) },
                new List<CraftingElement> { new CraftingElement<ElectricUpgradeKitItem>(1) }
            );
            this.Recipes = new List<Recipe> { recipe_1 };
            this.ExperienceOnCraft = 4f;
            this.LaborInCalories = CreateLaborInCaloriesValue(500f, typeof(ElectronicsSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(ElectronicsUpgradeRecipe), 15f, typeof(MechanicsSkill));
            this.Initialize(Localizer.DoStr("Electronic Upgrade Kit"), typeof(ElectronicsUpgradeRecipe));
            CraftingComponent.AddRecipe(typeof(ElectronicsAssemblyObject), this);
        }
    }
}