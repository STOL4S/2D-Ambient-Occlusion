namespace AmbientOcclusion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap Input = (Bitmap)Image.FromFile("tile2.png");
            Bitmap Background = (Bitmap)Image.FromFile("tile1.png");

            //BACK CUBE
            Sprite Cube1 = new Sprite(Input, new Point(64, 12));

            //FRONT CUBE
            Sprite Cube2 = new Sprite(Input, new Point(48, 20));

            //FRONT CUBE
            Sprite Cube3 = new Sprite(Input, new Point(72, 28));

            Bitmap BackBuffer = new Bitmap(256, 256);

            using (Graphics G = Graphics.FromImage(BackBuffer))
            {
                G.Clear(Color.White);

                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        G.DrawImage(Background, new Point(16 * x, 16 * y));
                    }
                }

                Cube1.Draw(G);
                Cube2.Draw(G);
                Cube3.Draw(G);
            }

            BackBuffer.Save("Buffer.png");

            SSAO.GenerateComposite(BackBuffer, new Sprite[] { Cube1, Cube2, Cube3 });
        }
    }
}