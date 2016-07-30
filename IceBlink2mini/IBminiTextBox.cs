using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class IBminiTextBox
    {
        public GameView gv;
        public List<string> tagStack = new List<string>();
        public List<IBminiFormattedLine> linesList = new List<IBminiFormattedLine>();
        //float xLoc = 0;
        public int tbHeight = 200;
        public int tbWidth = 300;
        public int tbXloc = 10;
        public int tbYloc = 10;
        public bool showBoxBorder = false;

        public IBminiTextBox(GameView g, int locX, int locY, int width, int height)
        {
            gv = g;
            tbXloc = locX;
            tbYloc = locY;
            tbWidth = width;
            tbHeight = height;
        }
        public IBminiTextBox(GameView g)
        {
            gv = g;
        }

        public void DrawString(string text, float x, float y, string fontColor)
        {
            if ((y > -2) && (y <= tbHeight - gv.fontHeight))
            {
                gv.DrawText(text, x + tbXloc, y + tbYloc, fontColor);
            }
        }

        public void AddFormattedTextToTextBox(string formattedText)
        {
            formattedText = formattedText.Replace("\r\n", "<br>");
            formattedText = formattedText.Replace("\n\n", "<br>");
            formattedText = formattedText.Replace("\"", "'");

            if ((formattedText.EndsWith("<br>")) || (formattedText.EndsWith("<BR>")))
            {
                List<IBminiFormattedLine> lnList = gv.cc.ProcessHtmlString(formattedText, tbWidth, tagStack, true);
                foreach (IBminiFormattedLine fl in lnList)
                {
                    linesList.Add(fl);
                }
            }
            else
            {
                List<IBminiFormattedLine> lnList = gv.cc.ProcessHtmlString(formattedText + "<br>", tbWidth, tagStack, true);
                foreach (IBminiFormattedLine fl in lnList)
                {
                    linesList.Add(fl);
                }
            }
        }
        
        public void onDrawLogBox()
        {
            //only draw lines needed to fill textbox
            float xLoc = 0;
            float yLoc = 0;
            //loop through 5 lines from current index point
            for (int i = 0; i < linesList.Count; i++)
            {
                //loop through each line and print each word
                foreach (IBminiFormattedWord word in linesList[i].wordsList)
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
