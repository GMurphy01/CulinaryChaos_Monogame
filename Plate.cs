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
using System.Security.Cryptography.X509Certificates;

namespace CulinaryChaos
{
    public class Plate
    {
        //Plate constructors; texture, position and hit box

        private Texture2D plateTex;
        public Vector2 Position { get; set; }
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, plateTex.Width, plateTex.Height);

        //Make plate object
        public Plate(Texture2D plateTexture, Vector2 position)
        {
            plateTex = plateTexture;
            Position = position;
        }

        //Draw plate
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(plateTex, Position, Color.White);
        }
    }
}
