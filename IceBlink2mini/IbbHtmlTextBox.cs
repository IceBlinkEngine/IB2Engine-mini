using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class IbbHtmlTextBox
    {
        public GameView gv;
        public List<string> tagStack = new List<string>();
        public List<IBminiFormattedLine> logLinesList = new List<IBminiFormattedLine>();
        public int tbHeight = 200;
        public int tbWidth = 300;
        public int tbXloc = 10;
        public int tbYloc = 10;
        public float fontHeightToWidthRatio = 1.0f;
        public bool showBoxBorder = false;

        public IbbHtmlTextBox(GameView g, int locX, int locY, int width, int height)
        {
            gv = g;
            tbXloc = locX;
            tbYloc = locY;
            tbWidth = width;
            tbHeight = height;
        }
        public IbbHtmlTextBox(GameView g)
        {
            gv = g;
        }

        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bmp, int x, int y)
        {
            IbRect src = new IbRect(0, 0, bmp.PixelSize.Width, bmp.PixelSize.Height);
            IbRect dst = new IbRect(x + tbXloc, y + tbYloc, bmp.PixelSize.Width, bmp.PixelSize.Height);
            gv.DrawBitmap(bmp, src, dst);
        }
        public void DrawString(string text, float x, float y, string fontColor)
        {
            if ((y > -2) && (y <= tbHeight - gv.fontHeight))
            {
                gv.DrawText(text, x + tbXloc, y + tbYloc, fontColor);
            }
        }

        public void AddHtmlTextToLog(string htmlText)
        {            
            htmlText = htmlText.Replace("\r\n", "<br>");
            htmlText = htmlText.Replace("\n\n", "<br>");
            htmlText = htmlText.Replace("\"", "'");

            if ((htmlText.EndsWith("<br>")) || (htmlText.EndsWith("<BR>")))
            {
                List<IBminiFormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText, tbWidth, tagStack, true);
                foreach (IBminiFormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
            else
            {
                List<IBminiFormattedLine> linesList = gv.cc.ProcessHtmlString(htmlText + "<br>", tbWidth, tagStack, true);
                foreach (IBminiFormattedLine fl in linesList)
                {
                    logLinesList.Add(fl);
                }
            }
        }
        
        public void onDrawLogBox()
        {
            //only draw lines needed to fill textbox
            float xLoc = 0;
            float yLoc = 0;
            //loop through 5 lines from current index point
            for (int i = 0; i < logLinesList.Count; i++)
            {
                //loop through each line and print each word
                foreach (IBminiFormattedWord word in logLinesList[i].wordsList)
                {
                    DrawString(word.text + " ", xLoc, yLoc, word.color);
                    xLoc += (word.text.Length + 1) * (gv.fontWidth + gv.fontCharSpacing);
                }
                xLoc = 0;
                yLoc += gv.fontHeight + gv.fontLineSpacing;
            }

            //draw border for debug info
            if (showBoxBorder)
            {
                gv.DrawRectangle(new IbRect(tbXloc, tbYloc, tbWidth, tbHeight), Color.DimGray, 1);
            }
        }
    }
}
