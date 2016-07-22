﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class IbbButton
    {
        //this class is handled differently than Android version
        public Bitmap Img = null;    //this is the normal button and color intensity
        public Bitmap ImgOff = null; //this is usually a grayed out button
        public Bitmap ImgOn = null;  //useful for buttons that are toggled on like "Move"
        public Bitmap Img2 = null;   //usually used for an image on top of default button like arrows or inventory backpack image
        public Bitmap Img2Off = null;   //usually used for turned off image on top of default button like spell not available
        public Bitmap Img3 = null;   //typically used for convo plus notification icon
        public Bitmap Glow = null;   //typically the green border highlight when hoover over or press button
        public buttonState btnState = buttonState.Normal;
        public bool btnNotificationOn = true; //used to determine whether Img3 is shown or not
        public bool glowOn = false;
        public string Text = "";
        public string Quantity = "";
        public string HotKey = "";
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;
        public GameView gv;

        public IbbButton(GameView g, float sc)
        {
            gv = g;
            scaler = sc;
        }

        public bool getImpact(int x, int y)
        {
            if ((x >= X) && (x <= (X + this.Width)))
            {
                if ((y >= Y + gv.oYshift) && (y <= (Y + gv.oYshift + this.Height)))
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
            return false;
        }

        public void Draw()
        {
            int pH = (int)((float)gv.screenHeight / 200.0f);
            int pW = (int)((float)gv.screenHeight / 200.0f);
            float fSize = (float)(gv.squareSize / 4) * scaler;

            IbRect src = new IbRect(0, 0, this.Img.PixelSize.Width, this.Img.PixelSize.Height);
            IbRect src2 = new IbRect(0, 0, this.Img.PixelSize.Width, this.Img.PixelSize.Height);
            IbRect src3 = new IbRect(0, 0, this.Img.PixelSize.Width, this.Img.PixelSize.Height);

            if (this.Img2 != null)
            {
                src2 = new IbRect(0, 0, this.Img2.PixelSize.Width, this.Img2.PixelSize.Width);
            }
            if (this.Img3 != null)
            {
                src3 = new IbRect(0, 0, this.Img3.PixelSize.Width, this.Img3.PixelSize.Width);
            }
            IbRect dst = new IbRect(this.X, this.Y, (int)((float)this.Img.PixelSize.Width * gv.screenDensity), (int)((float)this.Img.PixelSize.Height * gv.screenDensity));

            IbRect srcGlow = new IbRect(0, 0, this.Glow.PixelSize.Width, this.Glow.PixelSize.Height);
            IbRect dstGlow = new IbRect(this.X - (int)(7 * gv.screenDensity), 
                                        this.Y - (int)(7 * gv.screenDensity), 
                                        (int)((float)this.Width) + (int)(15 * gv.screenDensity), 
                                        (int)((float)this.Height) + (int)(15 * gv.screenDensity));

            //draw glow first if on
            if ((this.glowOn) && (this.Glow != null))
            {
                gv.DrawBitmap(this.Glow, srcGlow, dstGlow);
            }
            //draw the proper button State
            if ((this.btnState == buttonState.On) && (this.ImgOn != null))
            {
                gv.DrawBitmap(this.ImgOn, src, dst);
            }
            else if ((this.btnState == buttonState.Off) && (this.ImgOff != null))
            {
                gv.DrawBitmap(this.ImgOff, src, dst);
            }
            else
            {
                gv.DrawBitmap(this.Img, src, dst);
            }
            //draw the standard overlay image if has one
            if ((this.btnState == buttonState.Off) && (this.Img2Off != null))
            {
                gv.DrawBitmap(this.Img2Off, src2, dst);
            }
            else if (this.Img2 != null)
            {
                gv.DrawBitmap(this.Img2, src2, dst);
            }
            //draw the notification image if turned on (like a level up or additional convo nodes image)
            if ((this.btnNotificationOn) && (this.Img3 != null))
            {
                gv.DrawBitmap(this.Img3, src3, dst);
            }

            float thisFontHeight = gv.drawFontRegHeight;
            if (scaler > 1.05f)
            {
                thisFontHeight = gv.drawFontLargeHeight;
            }
            else if (scaler < 0.95f)
            {
                thisFontHeight = gv.drawFontSmallHeight;
            }
            
            // DRAW TEXT
            float stringSize = gv.cc.MeasureString(Text, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

            //place in the center
            float ulX = ((this.Width) - stringSize) / 2;
            float ulY = ((this.Height) - thisFontHeight) / 2;

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(Text, this.X + ulX + x, this.Y + ulY + y , scaler, Color.Black);
                }
            }
            gv.DrawText(Text, this.X + ulX, this.Y + ulY, scaler, Color.White);
            
            // DRAW QUANTITY
            stringSize = gv.cc.MeasureString(Quantity, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);
            
            //place in the bottom right quadrant
            ulX = (((this.Width) - stringSize) / 8) * 7;
            ulY = (((this.Height) - thisFontHeight) / 8) * 7;

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(Quantity, this.X + ulX + x, this.Y + ulY + y, scaler, Color.Black);
                }
            }
            gv.DrawText(Quantity, this.X + ulX, this.Y + ulY, scaler, Color.White);

            // DRAW HOTKEY
            if (gv.showHotKeys)
            {
                stringSize = gv.cc.MeasureString(HotKey, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

                //place in the bottom center
                ulX = ((this.Width) - stringSize) / 2;
                ulY = (((this.Height) - thisFontHeight) / 4) * 3;

                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        gv.DrawText(HotKey, this.X + ulX + x, this.Y + ulY + y, scaler, Color.Black);
                    }
                }
                gv.DrawText(HotKey, this.X + ulX, this.Y + ulY, scaler, Color.Red);
            }
        }
    }
}
