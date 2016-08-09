using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IceBlink2mini
{
    public class IBminiMessageBox
    {
        public GameView gv;
        public string tag = "";
        public List<string> tagStack = new List<string>();
        public List<IBminiFormattedLine> logLinesList = new List<IBminiFormattedLine>();
        public int currentTopLineIndex = 0;
        public int numberOfLinesToShow = 17;
        public int currentLocX = 0;
        public int currentLocY = 0;        
        public int Width = 0;
        public int Height = 0;
        public float xLoc = 0;
        public int startY = 0;
        public int moveDeltaY = 0;
        public int tbHeight = 200;
        public int tbWidth = 300;
        public int tbXloc = 10;
        public int tbYloc = 10;
        public float fontHeightToWidthRatio = 1.0f;
        public IbbButton btnReturn = null;        

        public IBminiMessageBox()
        {

        }

        public IBminiMessageBox(GameView g)
        {
            gv = g;            
        }

        public void setupIBminiMessageBox()
        {
            setControlsStart();
        }

        public void setControlsStart()
        {
            int pH = (int)((float)gv.screenHeight / 100.0f);

            if (btnReturn == null)
            {
                btnReturn = new IbbButton(gv, 1.0f);
                btnReturn.Img = gv.cc.LoadBitmap("btn_large");
                btnReturn.Glow = gv.cc.LoadBitmap("btn_large_glow");
                btnReturn.Text = "Return";
                btnReturn.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturn.Width = (int)(gv.ibbwidthL * gv.screenDensity);
                btnReturn.X = (int)(currentLocX * gv.screenDensity) + (int)((Width * gv.screenDensity) / 2) - (int)((gv.ibbwidthL * gv.screenDensity) / 2);
                btnReturn.Y = (int)(currentLocY * gv.screenDensity) + (int)(Height * gv.screenDensity) - (int)(gv.ibbheight * gv.screenDensity);                
            }            
        }

        public void DrawString(string text, float x, float y, string fontColor)
        {
            if ((y > -2) && (y <= (tbHeight * gv.screenDensity) - gv.fontHeight))
            {
                gv.DrawText(text, x + tbXloc + gv.pS, y, fontColor);
            }
        }

        public void AddHtmlTextToLog(string htmlText)
        {
            //Remove any '\r\n' hard returns from message
            htmlText = htmlText.Replace("\r\n", "<br>");
            htmlText = htmlText.Replace("\n\n", "<br>");
            htmlText = htmlText.Replace("\"", "'");

            if ((htmlText.EndsWith("<br>")) || (htmlText.EndsWith("<BR>")))
            {
                List<IBminiFormattedLine> lnList = gv.cc.ProcessHtmlString(htmlText, (int)(tbWidth * gv.screenDensity), tagStack, true);
                foreach (IBminiFormattedLine fl in lnList)
                {
                    logLinesList.Add(fl);
                }
            }
            else
            {
                List<IBminiFormattedLine> lnList = gv.cc.ProcessHtmlString(htmlText + "<br>", (int)(tbWidth * gv.screenDensity), tagStack, true);
                foreach (IBminiFormattedLine fl in lnList)
                {
                    logLinesList.Add(fl);
                }
            }
            scrollToEnd();
        }
        public void onDrawLogBox()
        {
            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList("ui_bg_log").PixelSize.Width, gv.cc.GetFromBitmapList("ui_bg_log").PixelSize.Height);
            IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
            gv.DrawBitmap(gv.cc.GetFromBitmapList("ui_bg_log"), src, dst);

            //ratio of #lines to #pixels
            float ratio = (float)(logLinesList.Count) / (float)(tbHeight * gv.screenDensity);
            if (ratio < 1.0f) { ratio = 1.0f; }
            if (moveDeltaY != 0)
            {
                int lineMove = (startY + moveDeltaY) * (int)ratio;
                SetCurrentTopLineAbsoluteIndex(lineMove);
            }
            //only draw lines needed to fill textbox
            float xLoc = 0.0f;
            float yLoc = 15.0f;
            int maxLines = currentTopLineIndex + numberOfLinesToShow;

            if (maxLines > logLinesList.Count) { maxLines = logLinesList.Count; }
            for (int i = currentTopLineIndex; i < maxLines; i++)
            {
                //loop through each line and print each word
                foreach (IBminiFormattedWord word in logLinesList[i].wordsList)
                {
                    int xLoc2 = (int)((currentLocX * gv.screenDensity + xLoc));
                    int yLoc2 = (int)((currentLocY * gv.screenDensity + yLoc));
                    DrawString(word.text + " ", xLoc2, yLoc2, word.color);
                    xLoc += (word.text.Length + 1) * (gv.fontWidth + gv.fontCharSpacing);
                }
                xLoc = 0;
                yLoc += gv.fontHeight + gv.fontLineSpacing;
            }
            btnReturn.Draw();
        }

        public void scrollToEnd()
        {
            SetCurrentTopLineIndex(logLinesList.Count);
        }
        public void SetCurrentTopLineIndex(int changeValue)
        {
            currentTopLineIndex += changeValue;
            if (currentTopLineIndex > logLinesList.Count - numberOfLinesToShow)
            {
                currentTopLineIndex = logLinesList.Count - numberOfLinesToShow;
            }
            if (currentTopLineIndex < 0)
            {
                currentTopLineIndex = 0;
            }
        }
        public void SetCurrentTopLineAbsoluteIndex(int absoluteValue)
        {
            currentTopLineIndex = absoluteValue;
            if (currentTopLineIndex > logLinesList.Count - numberOfLinesToShow)
            {
                currentTopLineIndex = logLinesList.Count - numberOfLinesToShow;
            }
            if (currentTopLineIndex < 0)
            {
                currentTopLineIndex = 0;
            }
        }
        private bool isMouseWithinTextBox(MouseEventArgs e)
        {
            if ((e.X > (int)(currentLocX * gv.screenDensity)) && (e.X < (int)(tbWidth * gv.screenDensity) + (int)(currentLocX * gv.screenDensity)) && (e.Y > (int)(currentLocY * gv.screenDensity)) && (e.Y < (int)(tbHeight * gv.screenDensity) + (int)(currentLocY * gv.screenDensity)))
            {
                return true;
            }
            return false;
        }
        public void onMouseWheel(object sender, MouseEventArgs e)
        {
            if (isMouseWithinTextBox(e))
            {
                // Update the drawing based upon the mouse wheel scrolling. 
                int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

                if (numberOfTextLinesToMove != 0)
                {
                    SetCurrentTopLineIndex(-numberOfTextLinesToMove);
                }
            }
        }
    }
}
