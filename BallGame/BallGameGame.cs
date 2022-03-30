using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace BallGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BallGameGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        
        public BallGameGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 
        float displayHeight;
        float displayWidth;
        int score;
        int lives;
        protected override void Initialize()
        {
             displayHeight = GraphicsDevice.Viewport.Height;
             displayWidth = GraphicsDevice.Viewport.Width;
             
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            LoadBallContent();
            LoadBatContent();
            loadTeargetContent();
            LoadBackgroundContent();
            LoadTitleScreenContent();
            font = Content.Load<SpriteFont>(@"Fonts/font1");
            setupGame();
        }


        #region GameSpriteStruct Structure and the setUpSprite Method

        struct GameSpriteStruct
        {
            public Texture2D SpriteTexture;
            public Rectangle SpriteRectangle;
            public float X;
            public float Y;
            public float XSpeed;
            public float YSpeed;
            public float WidthFactor;
            public float TicksToCrossScreen;
            public bool Visible;
        }
        void setupSprite(
            ref GameSpriteStruct sprite,
            float widthFactor,
            float ticksToCrossScreen,
            float initialX,
            float initialY,
            bool initialVisibility)
        {
            sprite.WidthFactor = widthFactor;
            sprite.TicksToCrossScreen = ticksToCrossScreen;
            sprite.SpriteRectangle.Width = (int)((displayWidth * widthFactor) + 0.5f);
            float aspectRatio =
                (float)sprite.SpriteTexture.Width / sprite.SpriteTexture.Height;
            sprite.SpriteRectangle.Height =
                (int)((sprite.SpriteRectangle.Width / aspectRatio) + 0.5f);
            sprite.X = initialX;
            sprite.Y = initialY;
            sprite.SpriteRectangle.X = (int)initialX;
            sprite.SpriteRectangle.Y = (int)initialY;
            sprite.XSpeed = displayWidth / ticksToCrossScreen;
            sprite.YSpeed = sprite.XSpeed;
            sprite.Visible = initialVisibility;
        }

        #endregion

        #region GameState enum
        enum GameState
        {
            titleScreen,
            playingGsme
        }
        GameState state = GameState.titleScreen;
        #endregion

        #region Target code and data
        Texture2D targetTexture;
        GameSpriteStruct[] target;
        int numberOfTargets = 20;
        int highscore;
        float targetHeight;
        float targetStepFactor = 0.1f;
        float targetHeightLimit;

        private void loadTeargetContent()
        {
            targetTexture = Content.Load<Texture2D>(@"Images/Target");
           // setUpTargets();
        }

        void setUpTargets()
        {
            targetHeight = displayHeight - displayHeight;
            targetHeightLimit = (displayHeight - displayHeight) + ((displayHeight) / 2);
            target = new GameSpriteStruct[numberOfTargets];
            float targetSpacing = displayWidth / numberOfTargets;
            for (int i = 0; i < numberOfTargets; i++)
            {
                target[i].SpriteTexture = targetTexture;
                setupSprite(ref target[i],
                    0.1f,
                    1000,
                    (displayWidth - displayWidth) + (i * targetSpacing),
                    (displayHeight - displayHeight),true);
            }
            
        }

        void startTarget()
        {
            setUpTargets();
        }     

        void   resetTargetDisplay()
        {
            targetHeight = targetHeight + (displayHeight * targetStepFactor);
            if (targetHeight > targetHeightLimit)
            {
                targetHeight = displayHeight - displayHeight;

            }
            for (int i = 0; i < numberOfTargets; i++)
            {
                target[i].Visible = true;
                target[i].Y = targetHeight;
            }
        }

        private void UpdateTarget()
        {
            bool noTarget = true;
            for (int i = 0; i < numberOfTargets; i++)
            {
                if (target[i].Visible)
                {
                    noTarget = false;
                    if (Ball.SpriteRectangle.Intersects(target[i].SpriteRectangle))
                    {
                        Ball.YSpeed = Ball.YSpeed * -1;
                        score = score + 10;                     
                        target[i].Visible = false;
                        break;
                    }
                }
                target[i].SpriteRectangle.X = (int)target[i].X;
                target[i].SpriteRectangle.Y = (int)target[i].Y;
            }
            if (noTarget)
            {
                resetTargetDisplay();
            }
        }

        private void DrawTarget()
        {
            for (int i = 0; i < numberOfTargets; i++)
            {
                if (target[i].Visible)
                {
                    spriteBatch.Draw(target[i].SpriteTexture, target[i].SpriteRectangle, Color.White);
                }
            }
        }

        #endregion

        #region Ball code and data
        GameSpriteStruct Ball;

        private void LoadBallContent()
        {
            Ball.SpriteTexture = Content.Load<Texture2D>(@"Images/Ball");
        }

        private void startBall()
        {
         
        }

        private void UpdateBall()
        {
            Ball.X = Ball.X + Ball.XSpeed;
            Ball.Y = Ball.Y + Ball.YSpeed;
            Ball.SpriteRectangle.X = (int)(Ball.X + 0.5f);
            Ball.SpriteRectangle.Y = (int)(Ball.Y + 0.5f);
            if (Ball.X + Ball.SpriteRectangle.Width >= displayWidth)
            {
                Ball.XSpeed = Ball.XSpeed * -1;
            }
            if (Ball.X <= 0)
            {
                Ball.XSpeed = Ball.XSpeed * -1;
            }
            if (Ball.Y + Ball.SpriteRectangle.Height >= displayHeight)
            {
                Ball.YSpeed = Math.Abs(Ball.YSpeed) * -1;
            }
            if (Ball.Y <= 0)
            {
                Ball.YSpeed = Math.Abs(Ball.YSpeed);
            }
            if (Ball.Y + Ball.SpriteRectangle.Height >= displayHeight)
            {
                Ball.YSpeed = Math.Abs(Ball.YSpeed) * -1;
                if (lives > 0)
                {
                    lives--;
                    if (lives <= 0)
                    {
                        gameOver();
                    }
                }
            }
        }

        private void DrawBall()
        {
            spriteBatch.Draw(Ball.SpriteTexture, Ball.SpriteRectangle, Color.White);
        }

        #endregion 

        #region Bat code and data
        GameSpriteStruct Bat;

        private void LoadBatContent()
        {
            Bat.SpriteTexture = Content.Load<Texture2D>(@"Images/Batt");
        }

        private void UpdateBat()
        {
            KeyboardState keyBoard = Keyboard.GetState();
            if (keyBoard.IsKeyDown(Keys.A) && Bat.SpriteRectangle.X > displayWidth - displayWidth)
            {
                Bat.X = Bat.X - Bat.XSpeed;
                Bat.SpriteRectangle.X = (int)(Bat.X - 0.4f);
            }
            if (keyBoard.IsKeyDown(Keys.D) && Bat.SpriteRectangle.X + Bat.SpriteRectangle.Width < displayWidth)
            {
                Bat.X = Bat.X + Bat.XSpeed;
                Bat.SpriteRectangle.X = (int)(Bat.X + 0.4);
            }
            if (keyBoard.IsKeyDown(Keys.W) && Bat.SpriteRectangle.Y > displayHeight - displayHeight)
            {
                Bat.Y = Bat.Y - Bat.YSpeed;
                Bat.SpriteRectangle.Y = (int)(Bat.Y - 0.4);
            }
            if (keyBoard.IsKeyDown(Keys.S) && Bat.SpriteRectangle.Y + Bat.SpriteRectangle.Height < displayHeight)
            {
                Bat.Y = Bat.Y + Bat.YSpeed;
                Bat.SpriteRectangle.Y = (int)(Bat.Y + 0.4);
            }
            if (Ball.SpriteRectangle.Intersects(Bat.SpriteRectangle))
            {
                Ball.YSpeed = Ball.YSpeed * -1;
            }
            if (Bat.X + Bat.SpriteRectangle.Width >= displayWidth)
            {
                Bat.SpriteRectangle.X = (int)displayWidth - Bat.SpriteRectangle.Width;
            }
            if (Bat.X <= 0)
            {
                Bat.SpriteRectangle.X = (int)displayWidth - (int)displayWidth;
            }
            if (Bat.Y + Bat.SpriteRectangle.Height >= displayHeight)
            {
                Bat.SpriteRectangle.Y = (int)displayHeight - Bat.SpriteRectangle.Height;
            }
            if (Bat.Y < 0)
            {
                Bat.SpriteRectangle.Y = (int)displayHeight - (int)displayHeight;
            }
        }

        private void DrawBat()
        {
            spriteBatch.Draw(Bat.SpriteTexture, Bat.SpriteRectangle, Color.White);
        }

        #endregion

        #region Background code data
        GameSpriteStruct Background;

        private void LoadBackgroundContent()
        {
            Background.SpriteTexture = Content.Load<Texture2D>(@"Images/Background");
            Background.SpriteRectangle = new Rectangle(
                (int)(displayWidth - displayWidth),
                (int)(displayHeight - displayHeight),
                (int)displayWidth,
                (int)displayHeight);
        }
        private void UpdateBackground()
        {
        }
        private void DrawBackground()
        {
            spriteBatch.Draw(Background.SpriteTexture, Background.SpriteRectangle, Color.DarkBlue);
        }

        #endregion

        #region Title Screen code data
        GameSpriteStruct TitleScreen;    

        private void LoadTitleScreenContent()
        {
            TitleScreen.SpriteTexture = Content.Load<Texture2D>(@"Images/TitleScreen");
            TitleScreen.SpriteRectangle = new Rectangle(
                (int)(displayWidth - displayWidth),
                (int)(displayHeight - displayHeight),
                (int)(displayWidth),
                (int)(displayHeight));
        }

        private void UpdateTitleScren()
        {
            KeyboardState Start = Keyboard.GetState();
            if (Start.IsKeyDown(Keys.Enter))
            {
                startGame();
            }
        }

        private void DrawTitleScreen()
        {
            spriteBatch.Draw(TitleScreen.SpriteTexture, TitleScreen.SpriteRectangle, Color.White);
            drawText("High Score : " + highscore.ToString(), Color.White, 40, 545);
        }

        #endregion

        #region Game Stste management

        void startGame()
        {
            score = 0;
            lives = 3;
            setupGame();
            state = GameState.playingGsme;
        }

        void gameOver()
        {
            if (score > highscore)
            {
                highscore = score;
            }
            state = GameState.titleScreen;
        }


        #endregion

        void setupGame()
        {
            score = 0;
            lives = 3;
            setupSprite(ref Ball, 0.05f, 200,200, 200,true);
            setupSprite(ref Bat, 0.1f, 120, displayWidth / 2, displayHeight - 50,true);
            setUpTargets();
         }
        void drawText(string text, Color textColor, float x, float y)
        {
            int layer;
            Vector2 textVector = new Vector2(x, y);

            //Draw the shadow
            Color backColor = new Color(0, 0, 0, 20);
            for (layer = 0; layer < 10; layer++)
            {
                spriteBatch.DrawString(font, text, textVector, backColor);
                textVector.X++;
                textVector.Y++;
            }

            //Draw the solid part of the characters
            backColor = new Color(190, 190, 190);
            for (layer = 0; layer < 5; layer++)
            {
                spriteBatch.DrawString(font, text, textVector, backColor);
                textVector.X++;
                textVector.Y++;
            }
            //Draw the top part of the characters
            spriteBatch.DrawString(font, text, textVector, textColor);
        }
    
       /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (state)
            {
                case GameState.titleScreen:
                    UpdateTitleScren();
                    break;
                case GameState.playingGsme:                   
                        UpdateBall();
                        if (lives <= 0)
                        {
                            return;
                        }
                        UpdateBat();
                        UpdateTarget();
                        break;
                    
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            switch (state)
            {
                case GameState.titleScreen:
                    DrawTitleScreen();
                    break;

                case GameState.playingGsme:
                    DrawBackground();
                    DrawBall();
                    DrawBat();
                    DrawTarget();
                    drawText("Score : " + score.ToString() + " " + "Lives : " + lives.ToString(), Color.White, displayWidth - displayWidth, displayHeight - 50);
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

      

     

    }
}
