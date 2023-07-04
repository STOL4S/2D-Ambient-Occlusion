using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientOcclusion
{
    public class Sprite
    {
        public Bitmap Texture;

        public Point Position;

        public Sprite()
        {
            this.Texture = new Bitmap(1, 1);
            this.Position = new Point();
        }

        public Sprite(Bitmap _Texture, Point _Position)
        {
            this.Texture= _Texture;
            this.Position = _Position;
        }

        public Sprite(string Path, Point _Position)
        {
            this.Texture = (Bitmap)Image.FromFile(Path);
            this.Position = _Position;
        }

        public void Draw(Graphics G)
        {
            G.DrawImage(this.Texture, this.Position);
        }
    }
}
