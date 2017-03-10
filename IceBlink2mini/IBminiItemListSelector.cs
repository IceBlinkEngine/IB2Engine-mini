using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IceBlink2mini
{
    public class IBminiItemListSelector
    {
        public GameView gv;
        public string currentSender = "";
        public string HeaderText = "";
        public List<string> itemList = new List<string>();
        public int selectedIndex = -1;
        public int currentLocX = 0;
        public int currentLocY = 0;
        public int Width = 0;
        public int Height = 0;
        public List<IbbButton> btnSelections = new List<IbbButton>();
        public bool showIBminiItemListSelector = false;

        public IBminiItemListSelector()
        {

        }
        public void setupIBminiItemListSelector(GameView g, List<string> itList, string headertxt, string senderScreen)
        {
            gv = g;
            currentSender = senderScreen;
            HeaderText = headertxt;
            itemList = itList;
            setControlsStart();
        }
        public void setControlsStart()
        {
            btnSelections.Clear();

            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padW = gv.squareSize / 6;
            
            for (int y = 0; y < itemList.Count; y++)
            {
                IbbButton btnNew = new IbbButton(gv, 1.0f);
                btnNew.Img = "btn_large";
                btnNew.Glow = "btn_large_glow";
                btnNew.X = (int)(currentLocX * gv.screenDensity) + padW;
                btnNew.Y = (int)(currentLocY * gv.screenDensity) + ((y + 1) * (int)(gv.squareSize * 0.65f)) + (y * padW);
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity * 0.75f);
                btnNew.Width = (int)(gv.ibbwidthL * gv.screenDensity * 2.88f);
                btnNew.Text = itemList[y];
                btnSelections.Add(btnNew);
            }
        }                
        public void drawItemListSelection()
        {
            //IF CONTROLS ARE NULL, CREATE THEM
            if (btnSelections.Count < 1)
            {
                setControlsStart();
            }

            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);

            int locX = (int)(currentLocX * gv.screenDensity) + (pW * 4);
            int locY = (int)(currentLocY * gv.screenDensity) + (pH * 4);
                        
            //DRAW PANEL BACKGROUND
            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList("ui_bg_log").PixelSize.Width, gv.cc.GetFromBitmapList("ui_bg_log").PixelSize.Height);
            IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
            gv.DrawBitmap(gv.cc.GetFromBitmapList("ui_bg_log"), src, dst);

            //DRAW TEXT		
            int textWidth = HeaderText.Length * (gv.fontWidth + gv.fontCharSpacing);
            locX = (int)(currentLocX * gv.screenDensity) + (((int)(Width * gv.screenDensity) - textWidth) / 2);
            gv.DrawText(HeaderText, locX, locY, "wh");
            
            //DRAW ALL SELECTION BUTTONS		
            foreach (IbbButton btn in btnSelections)
            {
                btn.Draw();
            }
        }        
        public void onTouchItemListSelection(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType)
        {
            //btnReturn.glowOn = false;
            foreach (IbbButton btn in btnSelections)
            {
                btn.glowOn = false;
            }

            switch (eventType)
            {
                case MouseEventType.EventType.MouseDown:
                case MouseEventType.EventType.MouseMove:
                    int x = (int)eX;
                    int y = (int)eY;

                    foreach (IbbButton btn in btnSelections)
                    {
                        if (btn.getImpact(x, y))
                        {
                            btn.glowOn = true;
                        }
                    }
                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)eX;
                    y = (int)eY;

                    int index = 0;
                    foreach (IbbButton btn in btnSelections)
                    {
                        if (btn.getImpact(x, y))
                        {
                            selectedIndex = index;
                            showIBminiItemListSelector = false;
                            if (currentSender.Equals("savegame"))
                            {
                                gv.cc.doSavesDialog(selectedIndex);
                            }
                            else if (currentSender.Equals("loadsavegame"))
                            {
                                gv.cc.doLoadSaveGameDialog(selectedIndex);
                            }
                            else if (currentSender.Equals("castselectorspelltarget"))
                            {
                                gv.screenCastSelector.doSpellTarget(selectedIndex);
                            }
                            else if (currentSender.Equals("traituseselectortraittarget"))
                            {
                                gv.screenTraitUseSelector.doTraitTarget(selectedIndex);
                            }
                            else if (currentSender.Equals("inventoryitemaction"))
                            {
                                gv.screenInventory.doItemAction(selectedIndex);
                            }
                            else if (currentSender.Equals("inventoryselectpcuseitem"))
                            {
                                gv.screenInventory.doSelectPcUseItem(selectedIndex);
                            }
                            else if (currentSender.Equals("mainmapselectcaster"))
                            {
                                gv.screenMainMap.doCastSelector(selectedIndex);
                            }
                            else if (currentSender.Equals("mainmapselecttraituser"))
                            {
                                gv.screenMainMap.doTraitUserSelector(selectedIndex);
                            }
                            else if (currentSender.Equals("partyscreenlevelup"))
                            {
                                gv.screenParty.doLevelUp(selectedIndex);
                            }
                            else if (currentSender.Equals("inventorydropforever"))
                            {
                                gv.screenInventory.doDropForever(selectedIndex);
                            }
                            else if (currentSender.Equals("shopinventoryaction"))
                            {
                                gv.screenShop.doShopInventoryActions(selectedIndex);
                            }
                            else if (currentSender.Equals("shopshopaction"))
                            {
                                gv.screenShop.doShopShopActions(selectedIndex);
                            }
                            else if (currentSender.Equals("verifyclosing"))
                            {
                                gv.doVerifyClosing(selectedIndex);
                            }
                            else if (currentSender.Equals("verifyreturntomain"))
                            {
                                gv.cc.doVerifyReturnToMain(selectedIndex);
                            }
                            return;
                        }
                        index++;
                    }
                    break;
            }
        }
    }
}
