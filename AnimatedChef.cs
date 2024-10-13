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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace CulinaryChaos
{

    public class AnimatedChef
    {
        //Constructors
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int currentFrame;
        private int totalFrames;
        private int slowDown;

        public AnimatedChef(Texture2D texture, int rows, int columns)
        {
            //Get when created
            Texture = texture;
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
        }

        public void Update()
        {
            //Add to slow down timer
            slowDown++;

            //When slowdown gets to 20
            if(slowDown == 20)
            {
                //Reset
                slowDown = 0;
                
                //Next frame
                currentFrame++;

                //If current frame gets to last frame, reset
                if (currentFrame == totalFrames)
                    currentFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            //Draw animation
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = currentFrame / Columns;
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);


            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);


        }
    }
    
}
