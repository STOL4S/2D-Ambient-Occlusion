namespace AmbientOcclusion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap Input = (Bitmap)Image.FromFile("obj.png");
            Bitmap Background = (Bitmap)Image.FromFile("0.png");

            //BACK CUBE
            Sprite Cube1 = new Sprite(Input, new Point(24, 12));

            //FRONT CUBE
            Sprite Cube2 = new Sprite(Input, new Point(8, 20));

            Bitmap BackBuffer = new Bitmap(64, 64);

            using (Graphics G = Graphics.FromImage(BackBuffer))
            {
                G.Clear(Color.White);

                G.DrawImage(Background, new Point(0, 0));
                G.DrawImage(Background, new Point(32, 0));
                G.DrawImage(Background, new Point(0, 32));
                G.DrawImage(Background, new Point(32, 32));

                Cube1.Draw(G);
                Cube2.Draw(G);
            }

            BackBuffer.Save("Buffer.png");

            SSAO.GenerateComposite(BackBuffer, new Sprite[] { Cube1, Cube2 });
        }
    }
}