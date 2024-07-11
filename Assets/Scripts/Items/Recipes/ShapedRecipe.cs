using System.Linq;

using Newtonsoft.Json.Linq;

namespace EscapeGuan.Items.Recipes
{
    public class ShapedRecipe : Recipe
    {
        public int Width, Height;

        public override bool Match(CraftingInput input)
        {
            if (input.Items.Count != Ingredients.Count)
                return false;
            if (input.Width != Width || input.Height != Height)
                return false;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Item i = Ingredients[x + y * Width];
                    if (i == null)
                    {
                        if (input.Items[x + y * Width] != null)
                            return false;
                    }
                    else if (input.Items[x + y * input.Width].Base != i)
                        return false;
                }
            }
            return true;
        }

        public static Recipe Load(JObject obj)
        {
            ShapedRecipe res = new();
            string[] ingredients = obj["ingredients"].Values<string>().ToArray();
            string[] shapes = obj["shape"].Values<string>().ToArray();
            foreach (string s in shapes)
            {
                string[] splited = s.Split(';');
                foreach (string x in splited)
                {
                    int v = int.Parse(x);
                    if (v < 0)
                        res.Ingredients.Add(null);
                    else
                        res.Ingredients.Add(ItemRegistry.Main.GetObject(ingredients[v]));
                }
                res.Width = splited.Length;
            }
            res.Height = shapes.Length;
            res.Result = ItemRegistry.Main.GetObject(obj["result"].Value<string>());
            return res;
        }

        protected ShapedRecipe() : base() { }
    }
}
