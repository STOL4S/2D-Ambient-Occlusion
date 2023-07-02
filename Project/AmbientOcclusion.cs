using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AmbientOcclusion
{
    public static class AmbientOcclusion
    {
        public static float STRENGTH = 1.5f;

        public static Bitmap Generate(Bitmap _Object, bool _SelfShadow)
        {
            Bitmap AO = new Bitmap(_Object.Width, _Object.Height);

            Bitmap PositionBuffer = new Bitmap(_Object.Width, _Object.Height);
            using (Graphics G = Graphics.FromImage(PositionBuffer))
            {
                G.Clear(Color.White);

                //128 COLORS OVER THE COURSE OF 32 PIXELS
                //128 IS HALF BRIGHTNESS.
                int Delta = (int)(64 / 32);

                for (int y = 0; y < _Object.Height; y++)
                {
                    int C = (int)(225 - (Delta * y));
                    if (C < 0)
                        C = 0;
                    G.DrawLine(new Pen(Color.FromArgb(C, C, C)), 0, y, _Object.Width, y);
                }
            }

            using (Graphics G = Graphics.FromImage(AO))
            {
                G.Clear(Color.White);

                for (int y = 1; y < _Object.Height - 1; y++)
                {
                    for (int x = 1; x < _Object.Width - 1; x++)
                    {
                        //IF YOU ARE AN ALPHA PIXEL (BACKGROUND)
                        //CHECK SURROUNDING PIXELS IN THE FOLLOWING PATTERN:
                        //*** SCAN THIS ROW
                        //*&* SCAN THE LEFT AND RIGHT PIXEL HERE
                        //XXX DO NOT SCAN THIS ROW!
                        //ONLY CHECK NEXT TO THE PIXEL AND ABOVE IT TO PREVENT
                        //AMBIENT OCCLUSION FROM BEING GENERATED ON TOP OF SURFACES

                        float Occlusion = 1.0f;
                        if (_Object.GetPixel(x, y).A <= 0)
                        {
                            for (int j = -1; j <= 0; j++)
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    if (_Object.GetPixel(x + i, y + j).A > 0)
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
                                    if (_Object.GetPixel(x + j, y - 2).A > 0)
                                    {
                                        Occlusion -= (1.0f / 21.0f);
                                    }
                                }
                            }
                        }

                        if (_SelfShadow)
                        {
                            if (_Object.GetPixel(x, y).A > 0)
                            {
                                Vector3 AvgColor = new Vector3(0, 0, 0);

                                for (int j = -1; j <= 1; j++)
                                {
                                    for (int i = -1; i <= 1; i++)
                                    {
                                        Color C = _Object.GetPixel(x + i, y + j);
                                        AvgColor += new Vector3(C.R, C.G, C.B);
                                    }
                                }

                                //DIVIDE BY 9 SAMPLES TO GET AVG COLOR
                                AvgColor /= 9;

                                Color Current = _Object.GetPixel(x, y);

                                //IF DELTA IS NEGATIVE, CURRENT PIXEL IS DARKER THAN SURROUNDING PIXELS
                                float Delta = (Current.R - AvgColor.X) 
                                    + (Current.G - AvgColor.Y) + (Current.B - AvgColor.Z);

                                if (Delta >= 64)
                                {
                                    Occlusion -= (Delta / 5120.0f);
                                }
                            }
                        }

                        //GET POSITION FROM POSITION MAP
                        int O = 255;
                        if (Occlusion != 1)
                        {
                            O = (int)(Occlusion * 132) + (int)(PositionBuffer.GetPixel(x, y).R / STRENGTH);
                        }

                        if (O > 255)
                            O = 255;
                        G.DrawRectangle(new Pen(Color.FromArgb(O, O, O)), x, y, 1, 1);
                    }
                }

                //CLEAN BORDERS OF THE IMAGE TO PREVENT WEIRD CONNECTION BETWEEN TEXTURES
                G.DrawRectangle(new Pen(Color.White), 0, _Object.Height - 1, _Object.Width, 1);
            }

#if DEBUG
            AO.Save("GeneratedOcclusion.png");
#endif

            return AO;
        }

        public static Bitmap GenerateComposite(Bitmap _Object, bool _SelfShadow)
        {
            Bitmap AOBuffer = Generate(_Object, _SelfShadow);
            Bitmap OBJBuffer = new Bitmap(AOBuffer.Width, AOBuffer.Height);
            Bitmap Output = new Bitmap(AOBuffer.Width, AOBuffer.Height);

            using (Graphics G = Graphics.FromImage(OBJBuffer))
            {
                G.Clear(Color.White);
                G.DrawImage(_Object, new Point(0, 0));
            }

            for (int y = 0; y < Output.Height; y++)
            {
                for (int x = 0; x < Output.Width; x++)
                {
                    Color BP = OBJBuffer.GetPixel(x, y);
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
