using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AmbientOcclusion
{
    public static class SSAO
    {
        public static float STRENGTH = 1.0f;

        public static Bitmap Generate(Bitmap BackBuffer, Sprite[] SpriteArray)
        {
            Bitmap AO = new Bitmap(BackBuffer.Width, BackBuffer.Height);
            Bitmap Pos = new Bitmap(BackBuffer.Width, BackBuffer.Height);

            //CALCULATE POSITION BUFFER
            using (Graphics G = Graphics.FromImage(Pos))
            {
                //BLACK WILL ALWAYS BE BACKGROUND
                G.Clear(Color.Black);

                long RGB = 0xFF000000;
                Color CC = Color.FromArgb((int)RGB);

                for (int i = 0; i < SpriteArray.Length; i++) 
                {
                    RGB += 0x00010000;
                    CC = Color.FromArgb((int)RGB);

                    for (int y = 0; y < SpriteArray[i].Texture.Height; y++)
                    {
                        for (int x = 0; x < SpriteArray[i].Texture.Width; x++)
                        {
                            //IF THIS PIXEL HAS COLOR
                            if (SpriteArray[i].Texture.GetPixel(x, y).A != 0)
                            {
                                Pos.SetPixel(SpriteArray[i].Position.X + x,
                                    SpriteArray[i].Position.Y + y, CC);
                            }
                        }
                    }
                }
            }


            using (Graphics G = Graphics.FromImage(AO))
            {
                G.Clear(Color.White);

                //SPRITES ARE ALWAYS IN ORDER, WITH BACKGROUND OBJECTS BEING
                //FIRST IN THE LIST AND CLOSE UP OBJECTS BEING AT THE END OF THE LIST
                for (int i = 0; i < SpriteArray.Length; i++)
                {
                    //GENERATE A LOCAL POSITION BUFFER FOR THIS SPRITE
                    //WORLD POSITION BUFFER IS ALREADY STORED IN POS
                    Bitmap LocalPos = new Bitmap(SpriteArray[i].Texture.Width, SpriteArray[i].Texture.Height);
                    using (Graphics GG = Graphics.FromImage(LocalPos))
                    {
                        G.Clear(Color.White);

                        //128 COLORS OVER THE COURSE OF 32 PIXELS
                        //128 IS HALF BRIGHTNESS.
                        int Delta = (int)(64 / 32);

                        for (int y = 0; y < SpriteArray[i].Texture.Height; y++)
                        {
                            int C = (int)(225 - (Delta * y));
                            if (C < 0)
                                C = 0;
                            GG.DrawLine(new Pen(Color.FromArgb(C, C, C)), 0, y, SpriteArray[i].Texture.Width, y);
                        }
                    }

                    for (int y = 1; y < SpriteArray[i].Texture.Height - 1; y++)
                    {
                        for (int x = 1; x < SpriteArray[i].Texture.Width - 1; x++)
                        {
                            float Occlusion = 1.0f;

                            //GET POSITION IN WORLD SPACE
                            Color PBuffer = Pos.GetPixel(SpriteArray[i].Position.X + x, 
                                SpriteArray[i].Position.Y + y);

                            //BACKGROUND PIXEL
                            if (PBuffer == Color.FromArgb(255, 0, 0, 0))
                            {
                                //for (int j = -1; j <= 0; j++)
                                //{
                                //    for (int k = -1; k <= 1; k++)
                                //    {
                                //        if (Pos.GetPixel(SpriteArray[i].Position.X + x + k,
                                //            SpriteArray[i].Position.Y + y + j).R > 0)
                                //        {
                                //            Occlusion -= (1.0f / 9.0f);
                                //        }
                                //    }
                                //}
                            }
                            //PIXEL IS PART OF AN OBJECT
                            //SCAN AROUND THIS PIXEL
                            else
                            {
                                //ASSUME YOU ARE A DARKER PIXEL
                                //YOU ARE LOOKING FOR LIGHTER PIXELS
                                //TO OCCLUDE YOU
                                for (int j = -2; j <= 2; j++)
                                {
                                    for (int k = -2; k <= 2; k++)
                                    {
                                        //IF OBJECT IS BEHIND NEIGHBOR OBJECT
                                        //AND NOT A BACKGROUND PIXEL
                                        Color PPBuffer = Pos.GetPixel(SpriteArray[i].Position.X + x + j,
                                            SpriteArray[i].Position.Y + y + k);

                                        //GET DISTANCE
                                        float Dist = Math.Abs(j + k);
                                        if (PPBuffer != Color.FromArgb(255, 0, 0, 0))
                                        {
                                            if (PBuffer.R < PPBuffer.R)
                                            {
                                                Occlusion -= (1.0f - 9.0f) * Dist;
                                            }
                                        }
                                    }
                                }
                            }

                            if (Occlusion <= 0.0f)
                                Occlusion = 0.0f;

                            int O = 255;
                            if (Occlusion != 1.0f)
                            {
                                O = 255 - (int)(Occlusion * 1.5f);
                            }

                            if (O > 255)
                                O = 255;
                            if (O < 0)
                                O = 0;

                            AO.SetPixel(SpriteArray[i].Position.X + x,
                                SpriteArray[i].Position.Y + y, Color.FromArgb(O, O, O));
                        }
                    }
                }
            }

            AO.Save("AO.png");
            Pos.Save("Pos.png");
            return Pos;
        }
    }
}
