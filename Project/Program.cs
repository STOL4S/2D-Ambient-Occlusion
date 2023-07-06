namespace AmbientOcclusion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap Input = (Bitmap)Image.FromFile("tile2.png");
            Bitmap Input2 = (Bitmap)Image.FromFile("tile3.png");

            Bitmap Background = (Bitmap)Image.FromFile("tile1.png");

            int OFF = 32;
            //BACK CUBE
            Sprite Cube1 = new Sprite(Input, new Point(64 + OFF, 12 + OFF));

            //FRONT CUBE
            Sprite Cube2 = new Sprite(Input, new Point(48 + OFF, 20 + OFF));

            //FRONT CUBE
            Sprite Cube3 = new Sprite(Input, new Point(72 + OFF, 28 + OFF));

            //FRONT MOST CUBE
            Sprite Cube4 = new Sprite(Input2, new Point(128 + OFF, 72 + OFF));

            Bitmap BackBuffer = new Bitmap(384, 384);

            using (Graphics G = Graphics.FromImage(BackBuffer))
            {
                G.Clear(Color.White);

                for (int x = 0; x < 20; x++)
                {
                    for (int y = 0; y < 20; y++)
                    {
                        G.DrawImage(Background, new Point(16 * x, 16 * y));
                    }
                }

                Cube1.Draw(G);
                Cube2.Draw(G);
                Cube3.Draw(G);
                //Cube4.Draw(G);
            }

            BackBuffer.Save("Buffer.png");

            SSAO.GenerateComposite(BackBuffer, new Sprite[] { Cube1, Cube2, Cube3});
        }
    }
}