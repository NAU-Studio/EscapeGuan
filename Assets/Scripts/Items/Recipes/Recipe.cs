using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace EscapeGuan.Items.Recipes
{
    public abstract class Recipe
    {
        public List<Item> Ingredients = new();
        public Item Result;

        public abstract bool Match(CraftingInput input);

        public static Recipe FromFile(string text)
        {
            JObject j = JObject.Parse(text);
            if (j["shaped"].Value<bool>())
                return ShapedRecipe.Load(j);
            else
                return ShapelessRecipe.Load(j);
        }

        protected Recipe() { }
    }
}
