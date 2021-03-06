﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2mini
{
    public class IB2ToggleButton
    {
        [JsonIgnore]
        public GameView gv;
        public string tag = "";
        public string ImgOnFilename = "";
        public string ImgOffFilename = "";
        public bool toggleOn = false;
        public int X = 0;
        public int Y = 0;        
        public int Width = 0;
        public int Height = 0;
        public bool show = true;

        public IB2ToggleButton()
        {
            
        }

        public IB2ToggleButton(GameView g)
        {
            gv = g;

        }

        public void setupIB2ToggleButton(GameView g)
        {
            gv = g;

        }

        public bool getImpact(IB2Panel parentPanel, int x, int y)
        {
            if (show)
            {
                if ((x >= (int)((parentPanel.currentLocX + X) * gv.screenDensity)) && (x <= (int)((parentPanel.currentLocX + X + Width) * gv.screenDensity)))
                {
                    if ((y >= (int)((parentPanel.currentLocY + Y) * gv.screenDensity)) && (y <= (int)((parentPanel.currentLocY + Y + Height) * gv.screenDensity)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Draw(IB2Panel parentPanel)
        {
            if (show)
            {
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Height);
                IbRect dst = new IbRect(0, 0, 0, 0);
                dst = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity), (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity), (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));
                
                if (toggleOn)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOnFilename), src, dst);
                }
                else
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOffFilename), src, dst);
                }
            }
        }

        public void Update(int elapsed)
        {
            //animate button?
        }
    }
}
