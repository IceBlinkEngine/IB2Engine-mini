﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class IB2Button
    {
        [JsonIgnore]
        public GameView gv;
        public string tag = "";
        public string ImgFilename = "";    //this is the normal button and color intensity
        public string ImgOffFilename = ""; //this is usually a grayed out button
        public string ImgOnFilename = "";  //useful for buttons that are toggled on like "Move"
        public string Img2Filename = "";   //usually used for an image on top of default button like arrows or inventory backpack image
        public string Img2OffFilename = "";   //usually used for turned off image on top of default button like spell not available
        public string Img3Filename = "";   //typically used for convo plus notification icon
        public string GlowFilename = "";   //typically the green border highlight when hoover over or press button
        [JsonConverter(typeof(StringEnumConverter))]
        public buttonState btnState = buttonState.Normal;
        public bool btnNotificationOn = true; //used to determine whether Img3 is shown or not
        public bool glowOn = false;
        public string Text = "";
        public string Quantity = "";
        public string HotKey = "";
        public int X = 0;
        public int Y = 0;
        public string IBScript = "none";
        public int Width = 0;
        public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;
        public bool show = true;

        public IB2Button()
        {
            
        }

        public IB2Button(GameView g)
        {
            gv = g;
        }

        public void setupIB2Button(GameView g)
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
                
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Height);
                IbRect dst = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity), (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity), (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));

                IbRect srcGlow = new IbRect(0, 0, gv.cc.GetFromBitmapList(GlowFilename).PixelSize.Width, gv.cc.GetFromBitmapList(GlowFilename).PixelSize.Height);
                IbRect dstGlow = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity) - (int)(4 * gv.screenDensity),
                                            (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity) - (int)(4 * gv.screenDensity),
                                            (int)((float)Width * gv.screenDensity) + (int)(8 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(8 * gv.screenDensity));

                //draw glow first if on
                if (glowOn)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(GlowFilename), srcGlow, dstGlow);
                }

                //draw the proper button State
                if (btnState == buttonState.On)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOnFilename), src, dst);
                }
                else if (btnState == buttonState.Off)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOffFilename), src, dst);
                }
                else
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgFilename), src, dst);
                }
                //draw the standard overlay image if has one
                if ((btnState == buttonState.Off) && (!Img2OffFilename.Equals("")))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(Img2OffFilename), src, dst);
                }
                else if (!Img2Filename.Equals(""))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(Img2Filename), src, dst);
                }
                //draw the notification image if turned on (like a level up or additional convo nodes image)
                if ((btnNotificationOn) && (!Img3Filename.Equals("")))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(Img3Filename), src, dst);
                }

                // DRAW TEXT                
                int stringSize = Text.Length * (gv.fontWidth + gv.fontCharSpacing);

                //place in the center
                int ulX = ((int)(this.Width * gv.screenDensity) - stringSize) / 2;
                int ulY = ((int)(this.Height * gv.screenDensity) - gv.fontHeight) / 2;

                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 2; y++)
                    {
                        int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                        int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY + y);
                        gv.DrawText(Text, xLoc, yLoc, "bk");
                    }
                }
                int xLoc1 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX);
                int yLoc1 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY);
                gv.DrawText(Text, xLoc1, yLoc1, "wh");


                // DRAW QUANTITY
                stringSize = Quantity.Length * (gv.fontWidth + gv.fontCharSpacing);

                //place in the bottom right
                ulX = (((int)(this.Width * gv.screenDensity) - stringSize) / 8) * 7;
                ulY = (((int)(this.Height * gv.screenDensity) - gv.fontHeight) / 8) * 7;

                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 2; y++)
                    {
                        int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                        int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY + y);
                        gv.DrawText(Quantity, xLoc, yLoc, "bk");
                    }
                }
                int xLoc2 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX);
                int yLoc2 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY);
                gv.DrawText(Quantity, xLoc2, yLoc2, "wh");



                // DRAW HOTKEY
                if (gv.showHotKeys)
                {
                    stringSize = HotKey.Length * (gv.fontWidth + gv.fontCharSpacing);

                    //place in the bottom center
                    ulX = ((int)(this.Width * gv.screenDensity) - stringSize) / 2;
                    ulY = (((int)(this.Height * gv.screenDensity) - gv.fontHeight) / 4) * 3;

                    for (int x = 0; x <= 2; x++)
                    {
                        for (int y = 0; y <= 2; y++)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                            int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY + y);
                            gv.DrawText(HotKey, xLoc, yLoc, "bk");
                        }
                    }
                    int xLoc3 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX);
                    int yLoc3 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY);
                    gv.DrawText(HotKey, xLoc3, yLoc3, "rd");
                }
            }
        }

        public void Update(int elapsed, IB2Panel parentPanel)
        {
            //animate button?
        }
    }
}
