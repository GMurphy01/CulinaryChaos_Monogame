/*
Griffin Murphy and Aasim Mukadama
Final Project - Culinary Chaos
PROG2370 - Prof. Rajkumar
December 08, 2023
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;


namespace CulinaryChaos
{
    public class Game1 : Game
    {
        //All textures
        Texture2D background;
        Texture2D chefTexture;
        Texture2D burgerTopTexture;
        Texture2D burgerBottomTexture;
        Texture2D lettuceTexture;
        Texture2D tomatoTexture;
        Texture2D pattyTexture;
        Texture2D plateTexture;
        Texture2D greenCheckTexture;
        Texture2D mainMenuBackgroundTexture;
        Texture2D helpScreenTexture;
        Texture2D blueMenuBackgroundTexture;
        Texture2D oilSpillTexture;

        //Animation Textures
        Texture2D chefCelebration;
        Texture2D chefSlipping;

        //Audio 
        Song inGameMusic;
        Song menuMusic;
        SoundEffect pickUpEffect;
        SoundEffect putDownEffect;

        //All vectors
        Vector2 chefPosition;
        Vector2 platePosition;

        //All speeds
        float chefSpeed;
        bool isSpeedChanged = false;
        float speedChangeDuration = 5f; 
        float speedChangeTimer = 0f;
        float originalChefSpeed = 600f;

        //Graphics, spritebatch and sprite effects
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteEffects spriteEffect = SpriteEffects.None;

        //Variables
        private float timer = 120f;
        private const float initialTime = 120f;
        private int score = 0;
        private bool completedObj = true;
        private int objCompletion = 0;
        private bool greenCheckPosOne = false;
        private bool greenCheckPosTwo = false;
        private bool greenCheckPosThree = false;
        private string playerName = "";
        private int screen = 0;
        private string highscores;
        private bool getHighscores = true;
        private int audioTrack = 0;
        private int tempAudio = 0;

        //Typing Cooldown timer
        float cooldown = 0.25f;
        float cooldownTimer = 0f;
        bool canAssignKey = true;

        //Carrying ingredient
        bool isCarryingIngredient = false;

        //Objects
        private IngredientManager ingredientManager;
        private AnimatedChef celebratingChef;
        private AnimatedChef fallingChef;
        private SpriteFont font;
        private SpriteFont gameRulesFont;
        private IngredientManager.Ingredient pickedUpIngredient = null;
        private Plate plate;
        private Random randomizer = new Random();
        private List<Texture2D> firstThreeIngredients;
        private List<Texture2D> objIngredients;

        public Game1()
        {
            //Initialize game
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Chef initial position and speed
            chefPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            chefSpeed = 600f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Load sprite batch
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load textures
            chefTexture = Content.Load<Texture2D>("images/spatulaChef");
            background = Content.Load<Texture2D>("images/NewBackground");
            burgerTopTexture = Content.Load<Texture2D>("images/BurgerTopNoBack");
            burgerBottomTexture = Content.Load<Texture2D>("images/BurgerBottom");
            lettuceTexture = Content.Load<Texture2D>("images/Lettuce");
            tomatoTexture = Content.Load<Texture2D>("images/Tomato");
            pattyTexture = Content.Load<Texture2D>("images/Patty");
            plateTexture = Content.Load<Texture2D>("images/Plate3");
            greenCheckTexture = Content.Load<Texture2D>("images/GreenCheck");
            mainMenuBackgroundTexture = Content.Load<Texture2D>("images/MainMenuBackground");
            helpScreenTexture = Content.Load<Texture2D>("images/HelpScreen");
            blueMenuBackgroundTexture = Content.Load<Texture2D>("images/blueMenuBackground");
            oilSpillTexture = Content.Load<Texture2D>("images/OilSpill");

            //Load Animation textures
            chefCelebration = Content.Load<Texture2D>("animations/ChefAnimation");
            chefSlipping = Content.Load<Texture2D>("animations/FallingChef");

            //Load fonts
            font = Content.Load<SpriteFont>("fonts/Timer");
            gameRulesFont = Content.Load<SpriteFont>("fonts/GameRules");

            //Load animations
            celebratingChef = new AnimatedChef(chefCelebration, 2, 2);
            fallingChef = new AnimatedChef(chefSlipping, 2, 2); 

            //Load ingredients
            ingredientManager = new IngredientManager(burgerTopTexture, burgerBottomTexture, lettuceTexture, tomatoTexture, pattyTexture);

            //Load plate
            plate = new Plate(plateTexture, new Vector2(680, 170));

            //Load your audio files
            inGameMusic = Content.Load<Song>("audio/InGame");
            menuMusic = Content.Load<Song>("audio/MainMenu");
            pickUpEffect = Content.Load<SoundEffect>("audio/PickUp");
            putDownEffect = Content.Load<SoundEffect>("audio/PutDown");

            //Play menu audio to start
            MediaPlayer.Play(menuMusic);
            MediaPlayer.Volume = 0.15f;
            audioTrack = 0;
        }

        //Changing song
        private void changeSong()
        {
            //Stop Media player, play new song
            MediaPlayer.Stop();
            if (audioTrack == 0)
            {
                //Play menu audio
                MediaPlayer.Play(menuMusic);
                MediaPlayer.Volume = 0.15f;
            }

            if (audioTrack == 1)
            {
                //Play menu audio
                MediaPlayer.Play(inGameMusic);
                MediaPlayer.Volume = 0.05f;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            //Song controller
            if (tempAudio != audioTrack)
            {
                //If the audio track has been changed, change song
                changeSong();
                tempAudio = audioTrack;
            }
            //Main menu logic
            if (screen == 0)
            {
                audioTrack = 0;

                //Check for keyboard state
                var kstate = Keyboard.GetState();

                //Update the cooldown timer for tpying to decrease by 1 every second
                cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                //If the cooldown timer reaches zero, allow typing
                if (cooldownTimer <= 0)
                {
                    canAssignKey = true;
                    cooldownTimer = 0;
                }

                //Check all keys
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    //if a key is pressed and the cooldown timer is done
                    if (kstate.IsKeyDown(key) && canAssignKey)
                    {
                        //Level One
                        if (key == Keys.D1)
                        {
                            //Change screen
                            screen = 1;
                        }
                        //Level Two
                        if (key == Keys.D2)
                        {
                            screen = 2;
                        }
                        //Help
                        if (key == Keys.D3)
                        {
                            screen = 3;
                        }
                        //About
                        if (key == Keys.D4)
                        {
                            screen = 4;
                        }
                        //Highscores
                        if (key == Keys.D5)
                        {
                            screen = 5;
                            highscores = "";
                            getHighscores = true;

                        }
                        //Game Rules
                        if (key == Keys.D6)
                        {
                            screen = 7;
                        }
                    }
                    
                }
            }
            //Level 1
            if (screen == 1)
            {
                //Change the audio to ingame music
                audioTrack = 1;

                bool endGame = false;

                //Exit app on escpape
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                //Get keyboard state
                var kstate = Keyboard.GetState();

                //If up is pressed
                if (kstate.IsKeyDown(Keys.Up))
                {
                    //Chef moves up by game speed * game time
                    chefPosition.Y -= chefSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //Down is pressed
                if (kstate.IsKeyDown(Keys.Down))
                {
                    //Chef moves down by speed * time
                    chefPosition.Y += chefSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //If left is pressed
                if (kstate.IsKeyDown(Keys.Left))
                {
                    //Chef moves left by speed * time
                    chefPosition.X -= chefSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //Face left
                    spriteEffect = SpriteEffects.FlipHorizontally;
                }

                //If down is pressed
                if (kstate.IsKeyDown(Keys.Right))
                {
                    //Chef moves right by speed * time
                    chefPosition.X += chefSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //Face right
                    spriteEffect = SpriteEffects.None;
                }

                //Border boundries
                /*-------------------------------------------------------------------------------------------*/
                if (chefPosition.X > _graphics.PreferredBackBufferWidth - chefTexture.Width / 8)
                {
                    chefPosition.X = _graphics.PreferredBackBufferWidth - chefTexture.Width / 8;
                }
                else if (chefPosition.X < chefTexture.Width / 8)
                {
                    chefPosition.X = chefTexture.Width / 8;
                }

                if (chefPosition.Y > _graphics.PreferredBackBufferHeight - chefTexture.Height / 3)
                {
                    chefPosition.Y = _graphics.PreferredBackBufferHeight - chefTexture.Height / 3;
                }
                else if (chefPosition.Y < chefTexture.Height / 3)
                {
                    chefPosition.Y = chefTexture.Height / 3;
                }
                /*-------------------------------------------------------------------------------------------*/

                //Ingredient boundries
                if (chefPosition.X < chefTexture.Width)
                {
                    chefPosition.X = chefTexture.Width;
                }

                //Plate boundries
                if (chefPosition.X > _graphics.PreferredBackBufferWidth - 100)
                {
                    chefPosition.X = _graphics.PreferredBackBufferWidth - 100;
                }

                //Create hitboxes
                Rectangle chefRectangle = new Rectangle((int)chefPosition.X, (int)chefPosition.Y + 75, chefTexture.Width, chefTexture.Height);
                Rectangle chefDropOffRectangle = new Rectangle((int)chefPosition.X, (int)chefPosition.Y, chefTexture.Width, chefTexture.Height);
                Rectangle palteRectangle = new Rectangle((int)platePosition.X, (int)platePosition.Y + 75, plateTexture.Width, plateTexture.Height);

                //Picking up & Placing ingredients 
                /*-------------------------------------------------------------------------------------------*/
                //If space is pressed
                if (kstate.IsKeyDown(Keys.Space))
                {
                    //Each ingredient in the ingredient manager class
                    foreach (IngredientManager.Ingredient ingredient in ingredientManager.ingredients)
                    {
                        //If the chef is touching it and is not already carrying an ingredient
                        if (chefRectangle.Intersects(ingredient.Rectangle) && isCarryingIngredient == false)
                        {
                            //Play sound
                            pickUpEffect.Play();

                            //Picked up ingredient true
                            isCarryingIngredient = true;
                            pickedUpIngredient = ingredient;
                            break; // Exit the loop once an ingredient is picked up
                        }
                    }
                }

                //Placing ingredient
                if (isCarryingIngredient)
                {
                    //Space is pressed
                    if (kstate.IsKeyDown(Keys.Space))
                    {
                        //Chef is touching plate hitbox
                        if (chefDropOffRectangle.Intersects(plate.Bounds))
                        {
                            //Sound efffect
                            putDownEffect.Play();

                            // Reset the carried ingredient flag and clear the list
                            CheckIfNeeded(pickedUpIngredient);
                            isCarryingIngredient = false;
                            pickedUpIngredient = null;
                        }
                    }
                }
                /*-------------------------------------------------------------------------------------------*/

                //Check if the timer reaches zero
                if (timer <= 0)
                {
                    //Timer stays at 0
                    timer = 0;
                    endGame = true;
                    
                    //Go to end screen
                    screen = 6;
                }

                //If you get to 10 before the timer is up, you move on to level 2
                if (score == 10)
                {
                    //Go to level 2
                    screen = 2;

                    //reset level one
                    ResetLevelOne();

                    //score is 10
                    score = 10;
                }

                //Timer 
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            //Level 2
            if (screen == 2)
            {
                //Hit Boxes
                Rectangle chefRectangle = new Rectangle((int)chefPosition.X, (int)chefPosition.Y + 75, chefTexture.Width, chefTexture.Height);
                Rectangle chefDropOffRectangle = new Rectangle((int)chefPosition.X, (int)chefPosition.Y, chefTexture.Width, chefTexture.Height);
                Rectangle plateRectangle = new Rectangle((int)platePosition.X, (int)platePosition.Y + 75, plateTexture.Width, plateTexture.Height);
                Rectangle chefFallRectangle = new Rectangle((int)chefPosition.X, (int)chefPosition.Y+20, chefTexture.Width/10, chefTexture.Height-20);
                Rectangle oilRectangle = new Rectangle(180, 205, 70, 0);
                Rectangle oilRectangle2 = new Rectangle(470, 305, 90, 0);

                //Stop and start ingame audio
                audioTrack = 0;
                audioTrack = 1;

                //Let escape quit game
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                //Get keyboard state
                var kstate = Keyboard.GetState();

                //Same controls as level one
                if (kstate.IsKeyDown(Keys.Up))
                {
                    chefPosition.Y -= chefSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (kstate.IsKeyDown(Keys.Down))
                {
                    chefPosition.Y += chefSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (kstate.IsKeyDown(Keys.Left))
                {
                    chefPosition.X -= chefSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //Face left
                    spriteEffect = SpriteEffects.FlipHorizontally;
                }

                if (kstate.IsKeyDown(Keys.Right))
                {
                    chefPosition.X += chefSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //Face right
                    spriteEffect = SpriteEffects.None;
                }

                //Border boundries
                /*-------------------------------------------------------------------------------------------*/
                if (chefPosition.X > _graphics.PreferredBackBufferWidth - chefTexture.Width / 8)
                {
                    chefPosition.X = _graphics.PreferredBackBufferWidth - chefTexture.Width / 8;
                }
                else if (chefPosition.X < chefTexture.Width / 8)
                {
                    chefPosition.X = chefTexture.Width / 8;
                }

                if (chefPosition.Y > _graphics.PreferredBackBufferHeight - chefTexture.Height / 3)
                {
                    chefPosition.Y = _graphics.PreferredBackBufferHeight - chefTexture.Height / 3;
                }
                else if (chefPosition.Y < chefTexture.Height / 3)
                {
                    chefPosition.Y = chefTexture.Height / 3;
                }
                /*-------------------------------------------------------------------------------------------*/

                //Ingredient boundries
                if (chefPosition.X < chefTexture.Width)
                {
                    chefPosition.X = chefTexture.Width;
                }

                //Plate boundries
                if (chefPosition.X > _graphics.PreferredBackBufferWidth - 100)
                {
                    chefPosition.X = _graphics.PreferredBackBufferWidth - 100;
                }

                //Handle if chef steps on oil logic
                /*-------------------------------------------------------------------------------------------*/
                if (chefFallRectangle.Intersects(oilRectangle) || chefFallRectangle.Intersects(oilRectangle2))
                {
                    //Change the chef's speed and start the timer if it's not already changed
                    if (!isSpeedChanged)
                    {
                        //Chef slows down
                        chefSpeed = 200f;

                        //Speed has been changed
                        isSpeedChanged = true;

                        //Start the timer 5 sec
                        speedChangeTimer = speedChangeDuration; 
                    }
                }
                //Update the speed change timer
                if (isSpeedChanged)
                {
                    //Count down
                    speedChangeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    //Check if the timer has elapsed
                    if (speedChangeTimer <= 0f)
                    {
                        //Reset the chef's speed and speed changed false
                        chefSpeed = originalChefSpeed;
                        isSpeedChanged = false;
                    }
                }
                /*-------------------------------------------------------------------------------------------*/

                //Picking up and Placing ingredients
                /*-------------------------------------------------------------------------------------------*/
                if (kstate.IsKeyDown(Keys.Space))
                {
                    //Each ingredient in the ingredient manager class
                    foreach (IngredientManager.Ingredient ingredient in ingredientManager.ingredients)
                    {
                        //If the chef is touching it and is not already carrying an ingredient
                        if (chefRectangle.Intersects(ingredient.Rectangle) && isCarryingIngredient == false)
                        {
                            //Play sound
                            pickUpEffect.Play();

                            //Picked up ingredient true
                            isCarryingIngredient = true;
                            pickedUpIngredient = ingredient;
                            break; 
                        }
                    }
                }

                //Placing ingredient
                if (isCarryingIngredient)
                {
                    if (kstate.IsKeyDown(Keys.Space))
                    {
                        //If chef is touching the plate
                        if (chefDropOffRectangle.Intersects(plate.Bounds))
                        {
                            //Sound efffect
                            putDownEffect.Play();

                            // Reset the carried ingredient flag and clear the list
                            CheckIfNeeded(pickedUpIngredient);
                            isCarryingIngredient = false;
                            pickedUpIngredient = null;
                        }
                    }
                }
                /*-------------------------------------------------------------------------------------------*/

                //Timer 
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                bool endGame = false;

                // Check if the timer reaches zero
                if (timer <= 0)
                {
                    //Endgame screen and endgame
                    timer = 0;
                    endGame = true;
                    screen = 6;
                }
            }
            //Help
            if (screen == 3)
            {
                //Check for keyboard state
                var kstate = Keyboard.GetState();

                //Update the cooldown timer
                cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                //If the cooldown timer reaches zero, allow typingn
                if (cooldownTimer <= 0)
                {
                    canAssignKey = true;
                    cooldownTimer = 0;
                }

                //Check all keys
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if (kstate.IsKeyDown(key) && canAssignKey)
                    {
                        //Main menu on backspace
                        if (key == Keys.Back)
                        {
                            screen = 0;
                        }
                    }

                }
            }
            //About
            if(screen ==4)
            {
                //Check for keyboard state
                var kstate = Keyboard.GetState();

                //Update the cooldown timer
                cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                //If the cooldown timer reaches zero, allow key assignments again
                if (cooldownTimer <= 0)
                {
                    canAssignKey = true;
                    cooldownTimer = 0;
                }

                //Check all keys
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if (kstate.IsKeyDown(key) && canAssignKey)
                    {
                        //Main menu if back
                        if (key == Keys.Back)
                        {
                            screen = 0;
                        }
                    }

                }
            }
            //Highscore
            if (screen == 5)
            {
                //Check for keyboard state
                var kstate = Keyboard.GetState();

                //Update the cooldown timer
                cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                //If the cooldown timer reaches zero, allow key assignments again
                if (cooldownTimer <= 0)
                {
                    canAssignKey = true;
                    cooldownTimer = 0;
                }

                //Check all keys
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if (kstate.IsKeyDown(key) && canAssignKey)
                    {
                        //Main menu if back
                        if (key == Keys.Back)
                        {
                            screen = 0;
                        }
                    }

                }


                if (getHighscores == true)
                {
                    //Read all lines from the file
                    string[] lines = File.ReadAllLines("highscores.txt");

                    //Create a list to hold the player records
                    List<(string name, int score)> playerRecords = new List<(string, int)>();

                    //Parse the lines to extract names and scores
                    foreach (string line in lines)
                    {
                        string[] data = line.Split(',');
                        string playerName = data[0].Split(':')[1].Trim();
                        int score = int.Parse(data[1].Split(':')[1].Trim());
                        playerRecords.Add((playerName, score));
                    }

                    // Sort the player records by score in descending order
                    playerRecords = playerRecords.OrderByDescending(x => x.score).ToList();

                    // Get the top 5 records
                    List<(string name, int score)> top5Records = playerRecords.Take(5).ToList();

                    // Print or use the top 5 records as needed
                    foreach (var record in top5Records)
                    {
                        highscores += ($"Player: {record.name}, Score: {record.score}\n");
                    }
                    getHighscores = false;
                }
            }
            //End screen
            if (screen == 6)
            {
                //Main menu song again
                audioTrack = 1;
                audioTrack = 0;

                //Update aniamtions for chefs
                celebratingChef.Update();
                fallingChef.Update();

                //Check for keyboard state
                var kstate = Keyboard.GetState();

                //Update the cooldown timer
                cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                //If the cooldown timer reaches zero, allow key assignments again
                if (cooldownTimer <= 0)
                {
                    canAssignKey = true;
                    cooldownTimer = 0;
                }

                //Typing name logic
                /*-------------------------------------------------------------------------------------------*/
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if (kstate.IsKeyDown(key) && canAssignKey)
                    {
                        //Delete letter
                        if ((key == Keys.Delete || key == Keys.Back) && playerName.Length > 0)
                        {
                            //Delete the most recent letter if Delete key is pressed and the string is not empty
                            playerName = playerName.Substring(0, playerName.Length - 1);
                        }
                        //Save score
                        else if (key == Keys.Space && playerName.Length > 0)
                        {
                            //Save the highscore
                            saveHighscores();
                            screen = 0;
                        }
                        //Main menu
                        else if (key == Keys.D1)
                        {
                            ResetLevelOne();
                            screen = 0;
                        }
                        //Typing name
                        else if (key != Keys.Delete && key != Keys.Back && key != Keys.Space && key != Keys.D1)
                        {
                            // Assign the pressed key to the string
                            playerName += key.ToString();
                        }

                        // Start the cooldown timer
                        cooldownTimer = cooldown;

                        // Prevent assigning keys until the cooldown ends
                        canAssignKey = false;
                    }
                }
            }
            //Game Rules
            if (screen == 7)
            {
                //Check for keyboard state
                var kstate = Keyboard.GetState();

                //Update the cooldown timer
                cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                //If the cooldown timer reaches zero, allow key assignments again
                if (cooldownTimer <= 0)
                {
                    canAssignKey = true;
                    cooldownTimer = 0;
                }

                //Check all keys
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if (kstate.IsKeyDown(key) && canAssignKey)
                    {
                        //Main menu on back
                        if (key == Keys.Back)
                        {
                            screen = 0;
                        }
                    }

                }
            }
            base.Update(gameTime);
        }

        //Main draw method
        protected override void Draw(GameTime gameTime)
        {
            //Main Menu
            if (screen == 0)
            {
                DrawMainMenu();
            }
            //Level One
            if (screen == 1)
            {
                _spriteBatch.Begin();
                DrawLevelOne();
                _spriteBatch.End();
            }
            //Level 2
            if (screen == 2)
            {
                _spriteBatch.Begin();
                DrawLevelTwo();
                _spriteBatch.End();
            }
            //Help
            if (screen == 3)
            {
                _spriteBatch.Begin();
                DrawHelpScreen();
                _spriteBatch.End();
            }
            //About
            if (screen == 4)
            {
                _spriteBatch.Begin();
                DrawAboutScreen();
                _spriteBatch.End();
            }
            //Highscores
            if (screen == 5)
            {
                _spriteBatch.Begin();
                DrawHighscoreScreen();
                _spriteBatch.End();
            }
            //End Screen
            if (screen == 6)
            {
                _spriteBatch.Begin();
                DrawEndGameScreen();
                _spriteBatch.End();
            }
            //Game Rules screen
            if (screen == 7)
            {
                _spriteBatch.Begin();
                DrawGameRules();
                _spriteBatch.End();
            }
            base.Draw(gameTime);
        }
        //Draw Game rules (Screen 7)
        private void DrawGameRules()
        {
            //Background
            _spriteBatch.Draw(blueMenuBackgroundTexture, new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 480), Microsoft.Xna.Framework.Color.White);

            //Text
            _spriteBatch.DrawString(font, "Press backspace to return to the main menu", new Vector2(100, 30), Microsoft.Xna.Framework.Color.White);
            _spriteBatch.DrawString(gameRulesFont, "Retrieve the ingredients from the table and place them on the plate.\r\n\r\nUtilize the arrow keys to move in any direction. \r\n\r\nUse the spacebar to pick up and drop items. \r\n\r\nIF the wrong item is selected you have to take it to the plate.\r\n\r\nBE CAUTIOUS when you encounter oil spills. \r\n\r\nThe oil spills WILL slow you down for 5 seconds. (Level 2)", new Vector2(50, 150), Microsoft.Xna.Framework.Color.White);
        }

        //Draw Main Menu (Screen 0)
        private void DrawMainMenu()
        {
            _spriteBatch.Begin();

            //Background
            _spriteBatch.Draw(mainMenuBackgroundTexture, new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 480), Microsoft.Xna.Framework.Color.White);

            //Text
            _spriteBatch.DrawString(font, "Main Menu ", new Vector2(320, 30), Microsoft.Xna.Framework.Color.White);
            _spriteBatch.DrawString(font, "- Press [1] for Level 1", new Vector2(225, 100), Microsoft.Xna.Framework.Color.White);
            _spriteBatch.DrawString(font, "- Press [2] for Level 2", new Vector2(225, 150), Microsoft.Xna.Framework.Color.White);
            _spriteBatch.DrawString(font, "- Press [3] for Help", new Vector2(225, 200), Microsoft.Xna.Framework.Color.White);
            _spriteBatch.DrawString(font, "- Press [4] for About", new Vector2(225, 250), Microsoft.Xna.Framework.Color.White);
            _spriteBatch.DrawString(font, "- Press [5] for Highscores", new Vector2(225, 300), Microsoft.Xna.Framework.Color.White);
            _spriteBatch.DrawString(font, "- Press [6] for Game Rules", new Vector2(225, 350), Microsoft.Xna.Framework.Color.White);

            _spriteBatch.End();
        }

        //Draw Main Menu (Screen 6)
        private void DrawEndGameScreen()
        {
            //Draw a rectangle using a 1x1 white pixel texture
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.DarkGray });

            Rectangle endScene = new Rectangle(0, 0, 850, 475);
            _spriteBatch.Draw(pixel, endScene, Color.White);

            //Depending on score, different animation
            if (score>=10)
            {
                //Display happy chef
                celebratingChef.Draw(_spriteBatch, new Vector2(350, 100));
            }
            else
            {
                //Display falling chef
                fallingChef.Draw(_spriteBatch, new Vector2(300, 100));
            }

            //Draw text
            _spriteBatch.DrawString(font, "Score: " + score.ToString(), new Vector2(280, 250), Microsoft.Xna.Framework.Color.Black);
            _spriteBatch.DrawString(font, "Name: " + playerName, new Vector2(280, 330), Microsoft.Xna.Framework.Color.Black);
            _spriteBatch.DrawString(font, "Press space to save game.", new Vector2(225, 390), Microsoft.Xna.Framework.Color.Black);
            _spriteBatch.DrawString(font, "Press [esc] to exit game.", new Vector2(10, 75), Microsoft.Xna.Framework.Color.Black);
            _spriteBatch.DrawString(font, "Press [1] to go to menu", new Vector2(10, 20), Microsoft.Xna.Framework.Color.Black);
        }

        //Draw Level 1 (Screen 1)
        private void DrawLevelOne()
        {
            //Background
            _spriteBatch.Draw(background, new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 480), Microsoft.Xna.Framework.Color.White);

            //Draw the plate
            plate.Draw(_spriteBatch);

            //Draw ingredients
            ingredientManager.DrawIngredients(_spriteBatch);

            //Draw timer and score
            _spriteBatch.DrawString(font, timer.ToString(), new Vector2(320, 420), Microsoft.Xna.Framework.Color.Black);
            _spriteBatch.DrawString(font, score.ToString(), new Vector2(659, 420), Microsoft.Xna.Framework.Color.Black);

            //Draw chef
            _spriteBatch.Draw(
                chefTexture,
                chefPosition,
                null,
                Microsoft.Xna.Framework.Color.White,
                0f,
                new Vector2(chefTexture.Width / 2, chefTexture.Height / 2),
                Vector2.One,
                spriteEffect,
            0f
            );

            //Draw the objScore
            _spriteBatch.DrawString(font, objCompletion.ToString() + "/3", new Vector2(465, 50), Microsoft.Xna.Framework.Color.Black);

            //Objective ingredients
            //List of available ingredients
            List<Texture2D> availableIngredients = new List<Texture2D>
            {
                burgerTopTexture,
                burgerBottomTexture,
                lettuceTexture,
                tomatoTexture,
                pattyTexture
            };

            //Objective is not commpleted
            if (!completedObj)
            {
                //random generated ingredient positions
                Rectangle ingredientPosition1 = new Rectangle(170, -20, 200, 200);
                Rectangle ingredientPosition2 = new Rectangle(250, -20, 200, 200);
                Rectangle ingredientPosition3 = new Rectangle(330, -20, 200, 200);

                //Draw three unique ingredients within the rectangle
                for (int i = 0; i < 3; i++)
                {
                    //Draw each item in the list of first ingredients in the corresponding position
                    if (i == 0)
                    {
                        _spriteBatch.Draw(firstThreeIngredients[i], ingredientPosition1, Color.White);
                    }
                    else if (i == 1)
                    {
                        _spriteBatch.Draw(firstThreeIngredients[i], ingredientPosition2, Color.White);
                    }
                    else if (i == 2)
                    {
                        _spriteBatch.Draw(firstThreeIngredients[i], ingredientPosition3, Color.White);
                    }
                }
            }
            //Once the obj is completed
            else
            {
                //Randomize list with random object
                availableIngredients = availableIngredients.OrderBy(x => randomizer.Next()).ToList();

                //Take the first 3 ingredients
                firstThreeIngredients = availableIngredients.Take(3).ToList();

                //Assign to a secondary list for editing
                objIngredients = availableIngredients.Take(3).ToList();

                //Set completed obj back to false
                completedObj = false;
            }

            //When an ingredient is picked up, draw the ingredient in the chefs hand unless timer 0
            if (pickedUpIngredient == null || timer == 0)
            {
                //Do nothing
            }
            else
            {
                _spriteBatch.Draw(pickedUpIngredient.Texture, new Vector2(chefPosition.X - 30, chefPosition.Y - 40), null, Microsoft.Xna.Framework.Color.White);
            }

            //Drawing completed objectives check marks 
            if (greenCheckPosOne)
            {
                _spriteBatch.Draw(greenCheckTexture, new Rectangle(170, -20, 200, 200), null, Microsoft.Xna.Framework.Color.White);
            }
            if (greenCheckPosTwo)
            {
                _spriteBatch.Draw(greenCheckTexture, new Rectangle(250, -20, 200, 200), null, Microsoft.Xna.Framework.Color.White);
            }
            if (greenCheckPosThree)
            {
                _spriteBatch.Draw(greenCheckTexture, new Rectangle(330, -20, 200, 200), null, Microsoft.Xna.Framework.Color.White);
            }
        }

        //Draw Level 2 (Screen 2)
        private void DrawLevelTwo()
        {
            //Background
            _spriteBatch.Draw(background, new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 480), Microsoft.Xna.Framework.Color.White);

            //Draw the plate
            plate.Draw(_spriteBatch);

            //Draw ingredients
            ingredientManager.DrawIngredients(_spriteBatch);

            //Draw timer and score
            _spriteBatch.DrawString(font, timer.ToString(), new Vector2(320, 420), Microsoft.Xna.Framework.Color.Black);
            _spriteBatch.DrawString(font, score.ToString(), new Vector2(659, 420), Microsoft.Xna.Framework.Color.Black);

            //Draw the objScore
            _spriteBatch.DrawString(font, objCompletion.ToString() + "/3", new Vector2(465, 50), Microsoft.Xna.Framework.Color.Black);

            //Objective ingredients
            //List of available ingredients
            List<Texture2D> availableIngredients = new List<Texture2D>
            {
                burgerTopTexture,
                burgerBottomTexture,
                lettuceTexture,
                tomatoTexture,
                pattyTexture
            };

            //Obj not completed
            if (!completedObj)
            {
                //Ingredients positions
                Rectangle ingredientPosition1 = new Rectangle(170, -20, 200, 200);
                Rectangle ingredientPosition2 = new Rectangle(250, -20, 200, 200);
                Rectangle ingredientPosition3 = new Rectangle(330, -20, 200, 200);

                //Draw three unique ingredients within the rectangle
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                    {
                        _spriteBatch.Draw(firstThreeIngredients[i], ingredientPosition1, Color.White);
                    }
                    else if (i == 1)
                    {
                        _spriteBatch.Draw(firstThreeIngredients[i], ingredientPosition2, Color.White);
                    }
                    else if (i == 2)
                    {
                        _spriteBatch.Draw(firstThreeIngredients[i], ingredientPosition3, Color.White);
                    }
                }
            }
            else
            {
                //Randomize list with random object
                availableIngredients = availableIngredients.OrderBy(x => randomizer.Next()).ToList();

                //Take first three of list
                firstThreeIngredients = availableIngredients.Take(3).ToList();
                
                //Assign to list for editing
                objIngredients = availableIngredients.Take(3).ToList();

                //Reset obj completion tracker
                completedObj = false;
            }

            //When an ingredient is picked up, draw the ingredient in the chefs hand unless timer 0
            if (pickedUpIngredient == null || timer == 0)
            {
                //Do nothing
            }
            else
            {
                _spriteBatch.Draw(pickedUpIngredient.Texture, new Vector2(chefPosition.X - 30, chefPosition.Y - 40), null, Microsoft.Xna.Framework.Color.White);
            }

            //Drawing completed objectives check marks 
            if (greenCheckPosOne)
            {
                _spriteBatch.Draw(greenCheckTexture, new Rectangle(170, -20, 200, 200), null, Microsoft.Xna.Framework.Color.White);
            }
            if (greenCheckPosTwo)
            {
                _spriteBatch.Draw(greenCheckTexture, new Rectangle(250, -20, 200, 200), null, Microsoft.Xna.Framework.Color.White);
            }
            if (greenCheckPosThree)
            {
                _spriteBatch.Draw(greenCheckTexture, new Rectangle(330, -20, 200, 200), null, Microsoft.Xna.Framework.Color.White);
            }

            //Draw Oil Spill
            _spriteBatch.Draw(oilSpillTexture, new Rectangle(170, 125, 100, 100), null, Microsoft.Xna.Framework.Color.White);
            //Draw Oil Spill2
            _spriteBatch.Draw(oilSpillTexture, new Rectangle(470, 225, 100, 100), null, Microsoft.Xna.Framework.Color.White);

            //Draw chef
            _spriteBatch.Draw(
                chefTexture,
                chefPosition,
                null,
                Microsoft.Xna.Framework.Color.White,
                0f,
                new Vector2(chefTexture.Width / 2, chefTexture.Height / 2),
                Vector2.One,
                spriteEffect,
            0f
            );

        }

        //Draw Help (Screen 3)
        private void DrawHelpScreen()
        {
            //Background
            _spriteBatch.Draw(helpScreenTexture, new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 480), Microsoft.Xna.Framework.Color.White);

            //text
            _spriteBatch.DrawString(font, "Press backspace to return to the main menu", new Vector2(100, 30), Microsoft.Xna.Framework.Color.Black);

        }

        //Draw About (Screen 4)
        private void DrawAboutScreen()
        {
            //Background
            _spriteBatch.Draw(blueMenuBackgroundTexture, new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 480), Microsoft.Xna.Framework.Color.White);

            //Text
            _spriteBatch.DrawString(font, "Press backspace to return to the main menu", new Vector2(100, 30), Microsoft.Xna.Framework.Color.White);
            _spriteBatch.DrawString(font, "Griffin Murphy: Programmer / Designer. \r\n Hockey Boy & part time chef. 'I'm Cooking' \r\n\r\nAasim Mukadam: Programmer / Designer. \r\n Soccer Boy & part time hype man. 'Yessir'", new Vector2(100, 200), Microsoft.Xna.Framework.Color.White);
        }

        //Draw Highscore (Screen 5)
        private void DrawHighscoreScreen()
        {
            //Background
            _spriteBatch.Draw(blueMenuBackgroundTexture, new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 480), Microsoft.Xna.Framework.Color.White);

            //Text
            _spriteBatch.DrawString(font, "Highscores", new Vector2(320, 10), Microsoft.Xna.Framework.Color.White);
            _spriteBatch.DrawString(font, "Press backspace to return to the main menu", new Vector2(100, 400), Microsoft.Xna.Framework.Color.White);

            //Highscore variable
            _spriteBatch.DrawString(font, highscores, new Vector2(200, 100), Microsoft.Xna.Framework.Color.White);
        }

        //Check if the deposited ingredient is needed to complete the obj
        private void CheckIfNeeded(IngredientManager.Ingredient ingredient)
        {
            //The texture is in objIngredients list
            if (objIngredients.Contains(ingredient.Texture))
            {
                //1 object completed
                objCompletion++;

                // Get the index of the texture in the list
                int indexOfIngredient = objIngredients.IndexOf(ingredient.Texture);

                //Add one to index for ingredient positioning in the obj box
                indexOfIngredient = indexOfIngredient + 1;

                //put a green check on the ingredient
                putCheckOnObjIngredient(indexOfIngredient);

                //Add a chef as a place holder to keep other ingredients in their same index
                objIngredients.Insert(indexOfIngredient, chefTexture);

                //Remove the ingredient
                objIngredients.Remove(ingredient.Texture);

            }

            //If all objects have been completed, reset the objective and add to score
            if (objCompletion == 3)
            {
                completedObj = true;
                objCompletion = 0;
                greenCheckPosOne = false;
                greenCheckPosTwo = false;
                greenCheckPosThree = false;
                score++;
            }
        }

        //Check off completed ingredient from obj
        private void putCheckOnObjIngredient(int ingredientPosition)
        {
            if (ingredientPosition == 1)
            {
                greenCheckPosOne = true;
            }
            else if (ingredientPosition == 2)
            {
                greenCheckPosTwo = true;
            }
            else if (ingredientPosition == 3)
            {
                greenCheckPosThree = true;
            }
        }

        //Save highscores to text file
        private void saveHighscores()
        {
            //Create file path variable
            string filePath = "highscores.txt";

            //Check if the file doesn't exist. If not, create it
            if (!File.Exists(filePath))
            {
                //Stream writer to create
                using (StreamWriter createFile = File.CreateText(filePath))
                {
                    //Create a new file and leave it empty
                }
            }
            //Append the playerName and score to the file
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine($"Player Name: {playerName}, Score: {score}");
            }

            //Reset level one after saving for next round
            ResetLevelOne();
        }

        //Reset the game when finihsed
        private void ResetLevelOne()
        {
            //reset the Game 1
            greenCheckPosOne = false;
            greenCheckPosTwo = false;
            greenCheckPosThree = false;
            objCompletion = 0;
            score = 0;
            timer = initialTime;
        }
        
    }
}
