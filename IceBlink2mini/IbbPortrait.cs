using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class IbbPortrait
    {
        //this class is handled differently than Android version
        public string ImgBG = null;
        public string Img = null;
        public string ImgLU = null; //used for level up icon
        public string Glow = null;
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
        public GameView gv;

        public IbbPortrait(GameView g, float sc)
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

            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgBG).PixelSize.Width, gv.cc.GetFromBitmapList(ImgBG).PixelSize.Height);
            IbRect src2 = new IbRect(0, 0, 0, 0);
            IbRect src3 = new IbRect(0, 0, 0, 0);
            IbRect dstLU = new IbRect(0, 0, 0, 0);

            if (this.Img != null)
            {
                src2 = new IbRect(0, 0, gv.cc.GetFromBitmapList(Img).PixelSize.Width, gv.cc.GetFromBitmapList(Img).PixelSize.Height);
            }
            if (this.ImgLU != null)
            {
                src3 = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgLU).PixelSize.Width, gv.cc.GetFromBitmapList(ImgLU).PixelSize.Height);
            }
            IbRect dstBG = new IbRect(this.X - (int)(1 * gv.screenDensity),
                                        this.Y - (int)(1 * gv.screenDensity),
                                        (int)((float)this.Width) + (int)(2 * gv.screenDensity),
                                        (int)((float)this.Height) + (int)(2 * gv.screenDensity));
            IbRect dst = new IbRect(this.X, this.Y, (int)((float)this.Width), (int)((float)this.Height));
            if (this.ImgLU != null)
            {
                dstLU = new IbRect(this.X, this.Y, gv.cc.GetFromBitmapList(ImgLU).PixelSize.Width, gv.cc.GetFromBitmapList(ImgLU).PixelSize.Height);
            }
            IbRect srcGlow = new IbRect(0, 0, gv.cc.GetFromBitmapList(Glow).PixelSize.Width, gv.cc.GetFromBitmapList(Glow).PixelSize.Height);
            IbRect dstGlow = new IbRect(this.X - (int)(2 * gv.screenDensity), 
                                        this.Y - (int)(2 * gv.screenDensity), 
                                        (int)((float)this.Width) + (int)(4 * gv.screenDensity), 
                                        (int)((float)this.Height) + (int)(4 * gv.screenDensity));

            gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgBG), src, dstBG);

            if ((this.glowOn) && (this.Glow != null))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Glow), srcGlow, dstGlow);
            }
            
            if (this.Img != null)
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img), src2, dst);
            }            
            
            if (this.ImgLU != null)
            {
                if (levelUpOn)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgLU), src3, dstLU);
                }                
            }

            if (gv.mod.useUIBackground)
            {
                IbRect srcFrame = new IbRect(0, 0, gv.cc.ui_portrait_frame.PixelSize.Width, gv.cc.ui_portrait_frame.PixelSize.Height);
                IbRect dstFrame = new IbRect(this.X - (int)(1 * gv.screenDensity),
                                        this.Y - (int)(1 * gv.screenDensity),
                                        (int)((float)this.Width) + (int)(2 * gv.screenDensity),
                                        (int)((float)this.Height) + (int)(2 * gv.screenDensity));
                gv.DrawBitmap(gv.cc.ui_portrait_frame, srcFrame, dstFrame);
            }
                       
            //DRAW HP/HPmax
            int ulX = pW * 0;
            int ulY = this.Height - (gv.fontHeight * 2);

            for (int x = 0; x <= 2; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    gv.DrawText(TextHP, this.X + ulX + x, this.Y + ulY - pH + y, "bk");
                }
            }
            gv.DrawText(TextHP, this.X + ulX, this.Y + ulY - pH, "gn");


            //DRAW SP/SPmax
            ulX = pW * 1;
            ulY = this.Height - (gv.fontHeight * 1);

            for (int x = 0; x <= 2; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    gv.DrawText(TextSP, this.X + ulX - pW + x, this.Y + ulY - pH + y, "bk");
                }
            }
            gv.DrawText(TextSP, this.X + ulX - pW, this.Y + ulY - pH, "yl");
        }
    }
}
