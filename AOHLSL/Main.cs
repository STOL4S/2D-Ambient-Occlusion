using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AOHLSL
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Sprite Cube1;
        private Sprite Cube2;
        private Sprite Cube3;

        private List<Sprite> SpriteRegister;

        private Texture2D GroundTexture;

        private RenderTarget2D DiffuseTarget;
        private RenderTarget2D DepthTarget;
        private RenderTarget2D AmbientTarget;

        private Effect SSAO_Shader;

        private int DISPLAY_MODE = 0;
        bool NextPress = true;
        bool PrevPress = false;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            DepthTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            DiffuseTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            AmbientTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SSAO_Shader = Content.Load<Effect>("Shaders/SSAO");

            GroundTexture = Content.Load<Texture2D>("Sprites/0");

            Cube1 = new Sprite(Content, "Sprites/obj", new Vector2(162, 128));
            Cube2 = new Sprite(Content, "Sprites/obj", new Vector2(130, 144));
            Cube3 = new Sprite(Content, "Sprites/obj", new Vector2(94, 128));

            Cube1.RGBVal = 0.1f;
            Cube2.RGBVal = 0.2f;
            Cube3.RGBVal = 0.01f;

            SpriteRegister = new List<Sprite>();
            SpriteRegister.AddRange(new Sprite[] { Cube3, Cube1, Cube2 });

            float RVal = 0.01f;

            for (int i = 0; i < SpriteRegister.Count; i++)
            {
                SpriteRegister[i].RGBVal = RVal;
                RVal += 0.005f;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Cube2.Position.X -= 2f;
                Cube2.Position.Y -= 1;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Cube2.Position.X += 2f;
                Cube2.Position.Y += 1;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
            {
                NextPress = true;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.NumPad6) && NextPress == true)
            {
                DISPLAY_MODE++;
                if (DISPLAY_MODE > 3)
                    DISPLAY_MODE = 0;

                NextPress = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
            {
                PrevPress = true;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.NumPad4) && PrevPress == true)
            {
                DISPLAY_MODE--;
                if (DISPLAY_MODE < 0)
                    DISPLAY_MODE = 3;

                PrevPress = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(DiffuseTarget);
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, null);

            DrawGround(_spriteBatch);
            DrawSpriteRegister(_spriteBatch, SSAO_Shader);

            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.SetRenderTarget(DepthTarget);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, SSAO_Shader, null);

            SSAO_Shader.CurrentTechnique = SSAO_Shader.Techniques["DepthTechnique"];
            SSAO_Shader.CurrentTechnique.Passes[0].Apply();

            DrawSpriteRegister(_spriteBatch, SSAO_Shader);

            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.SetRenderTarget(AmbientTarget);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, SSAO_Shader, null);

            SSAO_Shader.CurrentTechnique = SSAO_Shader.Techniques["SSAOTechnique"];
            SSAO_Shader.CurrentTechnique.Passes[0].Apply();

            _spriteBatch.Draw(DepthTarget, new Vector2(0, 0), Color.White);

            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.White);

            if (DISPLAY_MODE < 2)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, SSAO_Shader, null);

                SSAO_Shader.CurrentTechnique = SSAO_Shader.Techniques["CompositeTechnique"];
                SSAO_Shader.CurrentTechnique.Passes[0].Apply();
                SSAO_Shader.Parameters["SSAO_Texture"].SetValue(AmbientTarget);
                SSAO_Shader.Parameters["DISPLAY_MODE"].SetValue(DISPLAY_MODE);

                _spriteBatch.Draw(DiffuseTarget, new Vector2(0, 0), Color.White);

                _spriteBatch.End();
            }
            else if (DISPLAY_MODE == 2)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, null);

                _spriteBatch.Draw(DiffuseTarget, new Vector2(0, 0), Color.White);

                _spriteBatch.End();
            }
            else if (DISPLAY_MODE == 3)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, null);

                _spriteBatch.Draw(DepthTarget, new Vector2(0, 0), Color.White);

                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void DrawSpriteRegister(SpriteBatch _spriteBatch, Effect Shader)
        {
            float R_OFFSET = 0.01f;

            for (int i = 0; i < SpriteRegister.Count; i++)
            {
                if (Shader.CurrentTechnique.Name == "DepthTechnique")
                {
                    Shader.Parameters["R_OFFSET"].SetValue(SpriteRegister[i].RGBVal);
                    R_OFFSET += 0.25f;
                }
                SpriteRegister[i].Draw(_spriteBatch);
            }
        }

        private void DrawGround(SpriteBatch _spriteBatch)
        {
            for (int x = 0; x < 48; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    _spriteBatch.Draw(GroundTexture, new Vector2(x * 32, y * 32), Color.White);
                }
            }
        }
    }
}