/*
Griffin Murphy and Aasim Mukadama
Final Project - Culinary Chaos
PROG2370 - Prof. Rajkumar
December 08, 2023
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CulinaryChaos
{
    public class IngredientManager
    {
        //List of ingredients for this class
        private List<Ingredient> privIngredients;

        //Public list of ingredients
        //Allows for access to privIngredients without editing
        public List<Ingredient> ingredients
        {
            get { return privIngredients; }
        }

        //Ingredient class is texture and rectangle
        public class Ingredient
        {
            public Texture2D Texture { get; set; }
            public Rectangle Rectangle { get; set; }
       
        }

        //Manager  gets all ingredients
        public IngredientManager(Texture2D burgerTopTexture, Texture2D burgerBottomTexture, Texture2D lettuceTexture, Texture2D tomatoTexture, Texture2D pattyTexture)
        {
            //Private list is a list of all ingredients
            privIngredients = new List<Ingredient>();

            //Initialize ingredients 
            InitializeIngredients(burgerTopTexture, burgerBottomTexture, lettuceTexture, tomatoTexture, pattyTexture);
        }

        //Create the ingredients
        private void InitializeIngredients(Texture2D burgerTopTexture, Texture2D burgerBottomTexture, Texture2D lettuceTexture, Texture2D tomatoTexture, Texture2D pattyTexture)
        {
            // Define rectangles for each ingredient and associate them with their textures
            privIngredients.Add(new Ingredient
            {
                Texture = burgerTopTexture,
                Rectangle = new Rectangle(0, 5, 200, 200)
            });

            privIngredients.Add(new Ingredient
            {
                Texture = burgerBottomTexture,
                Rectangle = new Rectangle(-20, 75, 200, 200)
            });

            privIngredients.Add(new Ingredient
            {
                Texture = lettuceTexture,
                Rectangle = new Rectangle(-20, 145, 200, 200)
            });

            privIngredients.Add(new Ingredient
            {
                Texture = tomatoTexture,
                Rectangle = new Rectangle(-10, 215, 200, 200)
            });

            privIngredients.Add(new Ingredient
            {
                Texture = pattyTexture,
                Rectangle = new Rectangle(-15, 285, 200, 200)
            });

        }

        public void DrawIngredients(SpriteBatch spriteBatch)
        {
            //Draw ingredients based on their positions and textures
            foreach (Ingredient ingredient in ingredients)
            {
                spriteBatch.Draw(ingredient.Texture, ingredient.Rectangle, Microsoft.Xna.Framework.Color.White);
            }
        }
    }

}
