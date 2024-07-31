using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace EscapeGuan.Items.Recipes
{
    public class ShapelessRecipe : Recipe
    {
        public override bool Match(CraftingInput input)
        {
            List<Item> il = input.ShapelessList;
            if (Ingredients.Count != il.Count)
                return false;
            bool m = false;
            foreach (Item i in Ingredients)
                m = il.Contains(i);
            return m;
        }

        public static Recipe Load(JObject obj)
        {
            ShapelessRecipe res = new();
            foreach (string s in obj["ingredients"].Values<string>())
                res.Ingredients.Add(ItemRegistry.Main.GetObject(s));
            res.Result = ItemRegistry.Main.GetObject(obj["result"].Value<string>());
            return res;
        }

        protected ShapelessRecipe() : base() { }
    }
}
