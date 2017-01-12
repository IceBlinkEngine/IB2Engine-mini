using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class IB2Portrait
    {
        [JsonIgnore]
        public GameView gv;
        public string tag = "";
        public string ImgBGFilename = "";
        public string ImgFilename = "";
        public string ImgLUFilename = ""; //used for level up icon
        public string GlowFilename = "";
        public bool glowOn = false;
        public bool levelUpOn = false;
        public string TextHP = "";
        public string TextSP = "";
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;
        public bool show = false;

        public IB2Portrait()
        {
            
        }

        public IB2Portrait(GameView g)
        {
            gv = g;
        }

        public void  setupIB2Portrait(GameView g)
        {
            gv = g;
        }

        public void setHover(IB2Panel parentPanel, int x, int y)
        {
            if (show)
            {
                glowOn = false;

                if ((x >= (int)((parentPanel.currentLocX + X) * gv.screenDensity)) && (x <= (int)((parentPanel.currentLocX + X + Width) * gv.screenDensity)))
                {
                    if ((y >= (int)((parentPanel.currentLocY + Y) * gv.screenDensity)) && (y <= (int)((parentPanel.currentLocY + Y + Height) * gv.screenDensity)))
                    {
                        if (!playedHoverSound)
                        {
                            playedHoverSound = true;
                            gv.playerButtonEnter.Play();
                        }
                        glowOn = true;
                    }
                }
                playedHoverSound = false;
            }
        }

        public bool getImpact(IB2Panel parentPanel, int x, int y)
        {
            if (show)
            {
                if ((x >= (int)((parentPanel.currentLocX + X) * gv.screenDensity)) && (x <= (int)((parentPanel.currentLocX + X + Width) * gv.screenDensity)))
                {
                    if ((y >= (int)((parentPanel.currentLocY + Y) * gv.screenDensity)) && (y <= (int)((parentPanel.currentLocY + Y + Height) * gv.screenDensity)))
                    {
                        if (!playedHoverSound)
                        {
                            playedHoverSound = true;
                            gv.playerButtonEnter.Play();
                        }
                        return true;
                    }
                }
                playedHoverSound = false;                
            }
            return false;
        }

        public void Draw(IB2Panel parentPanel)
        {
            if (show)
            {
                int pH = (int)((float)gv.screenHeight / 200.0f);
                int pW = (int)((float)gv.screenHeight / 200.0f);
                float fSize = (float)(gv.squareSize / 4) * scaler;
                
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height);
                IbRect srcBG = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgBGFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ImgBGFilename).PixelSize.Height);
                IbRect dst = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity), (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity), (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));
                IbRect dstBG = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity) - (int)(2 * gv.screenDensity),
                                            (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity) - (int)(2 * gv.screenDensity),
                                            (int)((float)Width * gv.screenDensity) + (int)(4 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(4 * gv.screenDensity));

                IbRect srcGlow = new IbRect(0, 0, gv.cc.GetFromBitmapList(GlowFilename).PixelSize.Width, gv.cc.GetFromBitmapList(GlowFilename).PixelSize.Height);
                IbRect dstGlow = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity) - (int)(4 * gv.screenDensity),
                                            (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity) - (int)(4 * gv.screenDensity),
                                            (int)((float)Width * gv.screenDensity) + (int)(8 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(8 * gv.screenDensity));

                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgBGFilename), src, dstBG);

                if (glowOn)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(GlowFilename), srcGlow, dstGlow);
                }

                if (!ImgFilename.Equals(""))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgFilename), src, dst);
                }

                if (!ImgLUFilename.Equals(""))
                {
                    if (levelUpOn)
                    {
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgLUFilename), src, dst);
                    }
                }

                if (gv.mod.useUIBackground)
                {
                    IbRect srcFrame = new IbRect(0, 0, gv.cc.ui_portrait_frame.PixelSize.Width, gv.cc.ui_portrait_frame.PixelSize.Height);
                    IbRect dstFrame = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity) - (int)(2 * gv.screenDensity),
                                            (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity) - (int)(2 * gv.screenDensity),
                                            (int)((float)Width * gv.screenDensity) + (int)(4 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(4 * gv.screenDensity));
                    gv.DrawBitmap(gv.cc.ui_portrait_frame, srcFrame, dstFrame);
                }

                //DRAW HP/HPmax
                int ulX = pW * 0;
                int ulY = (int)(Height * gv.screenDensity) - ((int)gv.fontHeight * 2);

                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 2; y++)
                    {
                        int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                        int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH + y);
                        gv.DrawText(TextHP, xLoc, yLoc, "bk");
                    }
                }
                int xLoc1 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX);
                int yLoc1 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH);
                gv.DrawText(TextHP, xLoc1, yLoc1, "gn");


                //DRAW SP/SPmax
                ulX = pW * 0;
                ulY = (int)(Height * gv.screenDensity) - ((int)gv.fontHeight * 1);

                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 2; y++)
                    {
                        int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                        int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH + y);
                        gv.DrawText(TextSP, xLoc, yLoc, "bk");
                    }
                }
                xLoc1 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX);
                yLoc1 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH);
                gv.DrawText(TextSP, xLoc1, yLoc1, "yl");
            }
        }

        public void Update(int elapsed)
        {
            //animate button?
        }
    }
}
