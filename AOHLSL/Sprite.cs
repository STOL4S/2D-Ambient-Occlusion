using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AOHLSL
{
    public class Sprite
    {
        private Texture2D Tex;

        public Vector2 Position;

        public Rectangle CollisionBox;

        public float Rotation;

        public float RGBVal;

        public Sprite()
        {
            this.Tex = null;
            this.Position = Vector2.Zero;
            this.Rotation = 0.0f;

            this.CollisionBox = new Rectangle();
        }

        public Sprite(ContentManager Content, string FileName, Vector2 Position)
        {
            this.Tex = Content.Load<Texture2D>(FileName);

            this.Position = Position;
            this.Rotation = 0.0f;

            this.CollisionBox = new Rectangle((int)Position.X, (int)Position.Y,
                Tex.Width, Tex.Height);
        }

        public void Update(GameTime Time)
        {
        }

        public void Draw(SpriteBatch Batch)
        {
            Batch.Draw(Tex, Position, null, Color.White, Rotation, 
                new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
        }
    }
}
