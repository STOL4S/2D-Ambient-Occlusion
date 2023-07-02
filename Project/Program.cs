namespace AmbientOcclusion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap Input = (Bitmap)Image.FromFile("obj.png");
            Bitmap B = AmbientOcclusion.GenerateComposite(Input, true);
        }
    }
}