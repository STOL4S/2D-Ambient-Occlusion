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
                G.Clear(Color.FromArgb(255, 0, 0, 0));

                long RGB = 0xFF040000;
                Color CC = Color.FromArgb((int)RGB);

                for (int i = 0; i < SpriteArray.Length; i++) 
                {
                    RGB += 0x00080000;
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
                        int Delta = (int)(32 / SpriteArray[i].Texture.Height);

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
                            }
                            //PIXEL IS PART OF AN OBJECT
                            //SCAN AROUND THIS PIXEL
                            else
                            {
                                //ASSUME YOU ARE A DARKER PIXEL ON THE POSITION BUFFER
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
                                O = 255 - (int)(Occlusion * STRENGTH);
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

                //LAST, SCAN ENTIRE POSITION MAP FOR NON-OCCUPIED PIXELS
                //CHECK TO SEE IF THEY REQUIRE SHADING
                for (int y = 1; y < Pos.Height - 1; y++)
                {
                    for (int x = 1; x < Pos.Width - 1; x++) 
                    {
                        //IF YOU ARE AN ALPHA PIXEL (BACKGROUND)
                        //CHECK SURROUNDING PIXELS IN THE FOLLOWING PATTERN:
                        //*** SCAN THIS ROW
                        //*&* SCAN THE LEFT AND RIGHT PIXEL HERE
                        //XXX DO NOT SCAN THIS ROW!
                        //ONLY CHECK NEXT TO THE PIXEL AND ABOVE IT TO PREVENT
                        //AMBIENT OCCLUSION FROM BEING GENERATED ON TOP OF SURFACES

                        float Occlusion = 1.0f;
                        if (Pos.GetPixel(x, y).R == 0)
                        {
                            for (int j = -1; j <= 0; j++)
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    //GET OBJECT POSITION RELATIVE TO GROUND
                                    //IF THE PIXEL IS OCCUPIED
                                    if (Pos.GetPixel(x, y + j).R > 0)
                                    {
                                        Occlusion -= (1.0f / 9.0f);
                                    }
                                }
                            }

                            //CHECK 2 PIXELS ABOVE TARGET PIXEL
                            //ONLY GIVE HALF OCCLUSION FOR THIS
                            for (int j = -1; j <= 1; j++)
                            {
                                if (y >= 2)
                                {
                                    if (Pos.GetPixel(x + j, y - 2).R > 0)
                                    {
                                        Occlusion -= (1.0f / 14.0f);
                                    }
                                }
                            }

                            //CHECK 3 PIXELS ABOVE TARGET PIXEL
                            //ONLY GIVE HALF OCCLUSION FOR THIS
                            for (int j = -1; j <= 1; j++)
                            {
                                if (y >= 3)
                                {
                                    if (Pos.GetPixel(x + j, y - 3).R > 0)
                                    {
                                        Occlusion -= (1.0f / 18.0f);
                                    }
                                }
                            }
                        }


                        //YOU ARE ANY PIXEL IN THE POSITION BUFFER THAT IS OCCUPIED
                        //CHECK DIRECTLY BELOW YOURSELF FOR OCCLUSION
                        //if (Pos.GetPixel(x, y).R > 0)
                        //{
                        //    for (int j = 1; j < 3; j++)
                        //    {
                        //        Color PPBuffer = Pos.GetPixel(x, y + j);
                        //        if (PPBuffer == Color.FromArgb(255, 0, 0, 0))
                        //        {
                        //            Occlusion -= (1.0f / 9.0f) * (1);
                        //        }
                        //    }
                        //}

                        int O = 255;
                        if (Occlusion != 1)
                        {
                            O = (int)(Occlusion * 255);

                            if (O > 255)
                                O = 255;
                            if (O < 0)
                                O = 0;

                            if (AO.GetPixel(x, y).R < O)
                            {
                                O = O - (int)(AO.GetPixel(x, y).R / 1.5f);
                            }

                            AO.SetPixel(x, y, Color.FromArgb(O, O, O));
                        }
                    }
                }
            }

            AO.Save("AO.png");
            Pos.Save("Pos.png");

            return AO;
        }

        public static Bitmap GenerateComposite(Bitmap BackBuffer, Sprite[] SpriteArray)
        {
            Bitmap AOBuffer = Generate(BackBuffer, SpriteArray);
            Bitmap Output = new Bitmap(BackBuffer.Width, BackBuffer.Height);

            for (int y = 0; y < Output.Height; y++)
            {
                for (int x = 0; x < Output.Width; x++)
                {
                    Color BP = BackBuffer.GetPixel(x, y);
                    float AO = AOBuffer.GetPixel(x, y).R / 255.0f;

                    int FinalR = (int)(BP.R * AO);
                    int FinalG = (int)(BP.G * AO);
                    int FinalB = (int)(BP.B * AO);

                    FinalR = CorrectRGBValue(FinalR);
                    FinalG = CorrectRGBValue(FinalG);
                    FinalB = CorrectRGBValue(FinalB);

                    Color NC = Color.FromArgb(FinalR, FinalG, FinalB);

                    Output.SetPixel(x, y, NC);
                }
            }

#if DEBUG
            Output.Save("GeneratedComposite.png");
#endif

            return Output;
        }

        private static int CorrectRGBValue(int _Input)
        {
            if (_Input < 0)
            {
                return 0;
            }
            else if (_Input > 255)
            {
                return 255;
            }
            else
            {
                return _Input;
            }
        }
    }
}
