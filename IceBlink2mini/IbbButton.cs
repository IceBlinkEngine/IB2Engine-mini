using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using Bitmap = SharpDX.Direct2D1.Bitmap;
//using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class IbbButton
    {
        //this class is handled differently than Android version
        public string Img = null;    //this is the normal button and color intensity
        public string ImgOff = null; //this is usually a grayed out button
        public string ImgOn = null;  //useful for buttons that are toggled on like "Move"
        public string Img2 = null;   //usually used for an image on top of default button like arrows or inventory backpack image
        public string Img2Off = null;   //usually used for turned off image on top of default button like spell not available
        public string Img3 = null;   //typically used for convo plus notification icon
        public string Glow = null;   //typically the green border highlight when hoover over or press button
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
                if ((y >= Y) && (y <= (Y + this.Height)))
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

            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img).PixelSize.Width, gv.cc.GetFromBitmapList(Img).PixelSize.Height);
            IbRect src2 = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img).PixelSize.Width, gv.cc.GetFromBitmapList(Img).PixelSize.Height);
            IbRect src3 = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img).PixelSize.Width, gv.cc.GetFromBitmapList(Img).PixelSize.Height);

            if (this.Img2 != null)
            {
                src2 = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img2).PixelSize.Width, gv.cc.GetFromBitmapList(Img2).PixelSize.Width);
            }
            if (this.Img3 != null)
            {
                src3 = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img3).PixelSize.Width, gv.cc.GetFromBitmapList(Img3).PixelSize.Width);
            }
            IbRect dst = new IbRect(this.X, this.Y, (int)((float)this.Width), (int)((float)this.Height));

            IbRect srcGlow = new IbRect(0, 0, gv.cc.GetFromBitmapList(Glow).PixelSize.Width, gv.cc.GetFromBitmapList(Glow).PixelSize.Height);
            IbRect dstGlow = new IbRect(this.X - (int)(2 * gv.screenDensity), 
                                        this.Y - (int)(2 * gv.screenDensity), 
                                        (int)((float)this.Width) + (int)(4 * gv.screenDensity), 
                                        (int)((float)this.Height) + (int)(4 * gv.screenDensity));

            //draw glow first if on
            if ((this.glowOn) && (this.Glow != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Glow), srcGlow, dstGlow);
            }
            //draw the proper button State
            if ((this.btnState == buttonState.On) && (this.ImgOn != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOn), src, dst);
            }
            else if ((this.btnState == buttonState.Off) && (this.ImgOff != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOff), src, dst);
            }
            else
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img), src, dst);
            }
            //draw the standard overlay image if has one
            if ((this.btnState == buttonState.Off) && (this.Img2Off != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img2Off), src2, dst);
            }
            else if (this.Img2 != null)
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img2), src2, dst);
            }
            //draw the notification image if turned on (like a level up or additional convo nodes image)
            if ((this.btnNotificationOn) && (this.Img3 != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img3), src3, dst);
            }

            // DRAW TEXT
            int stringSize = Text.Length * (gv.fontWidth + gv.fontCharSpacing);

            //place in the center
            int ulX = ((this.Width) - stringSize) / 2;
            int ulY = ((this.Height) - gv.fontHeight) / 2;

            for (int x = 0; x <= 2; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    gv.DrawText(Text, this.X + ulX + x, this.Y + ulY + y, "bk");
                }
            }
            gv.DrawText(Text, this.X + ulX, this.Y + ulY, "wh");



            // DRAW QUANTITY
            stringSize = Quantity.Length * (gv.fontWidth + gv.fontCharSpacing);

            //place in the bottom right
            ulX = (((this.Width) - stringSize) / 8) * 7;
            ulY = (((this.Height) - gv.fontHeight) / 8) * 7;

            for (int x = 0; x <= 2; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    gv.DrawText(Quantity, this.X + ulX + x, this.Y + ulY + y, "bk");
                }
            }
            gv.DrawText(Quantity, this.X + ulX, this.Y + ulY, "wh");



            // DRAW HOTKEY
            if (gv.showHotKeys)
            {
                stringSize = HotKey.Length * (gv.fontWidth + gv.fontCharSpacing);

                //place in the bottom center
                ulX = ((this.Width) - stringSize) / 2;
                ulY = (((this.Height) - gv.fontHeight) / 4) * 3;

                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 2; y++)
                    {
                        gv.DrawText(HotKey, this.X + ulX + x, this.Y + ulY + y, "bk");
                    }
                }
                gv.DrawText(HotKey, this.X + ulX, this.Y + ulY, "rd");
            }
        }
    }
}
