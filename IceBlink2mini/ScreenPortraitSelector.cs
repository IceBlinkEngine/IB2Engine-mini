﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2mini
{
    public class ScreenPortraitSelector
    {
        //public Module gv.mod;
	    public GameView gv;
        public Player pc;
	    private int ptrPageIndex = 0;
	    private int ptrSlotIndex = 0;
	    private int slotsPerPage = 10;
        private int maxPages = 40;
	    private List<IbbPortrait> btnPortraitSlot = new List<IbbPortrait>();
	    private IbbButton btnPortraitsLeft = null;
	    private IbbButton btnPortraitsRight = null;
	    private IbbButton btnPageIndex = null;
	    private IbbButton btnAction = null;
        private IbbButton btnExit = null;
        public string callingScreen = "main"; //main, party, inventory
        public List<string> playerPortraitList = new List<string>();

        public ScreenPortraitSelector(Module m, GameView g)
	    {
		    //gv.mod = m;
		    gv = g;
	    }

        public void resetPortraitSelector(string callingScreenToReturnTo, Player p)
        {
            pc = p;
            callingScreen = callingScreenToReturnTo;
            LoadPlayerPortraitList();
        }

        public void LoadPlayerPortraitList()
        {
            playerPortraitList.Clear();
            //OVERRIDE
            try
            {
                //Load from module folder first
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\override"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\override", "*.png");
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if (filename.StartsWith("pptr_"))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                if (!playerPortraitList.Contains(fileNameWithOutExt))
                                {
                                    playerPortraitList.Add(fileNameWithOutExt);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            gv.errorLog(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                gv.errorLog(ex.ToString());
            }
            //MODULE SPECIFIC
            try
            {
                //foreach (Bitmap b in gv.cc.commonBitmapList)
                foreach (KeyValuePair<string, SharpDX.Direct2D1.Bitmap> entry in gv.cc.moduleBitmapList)
                {
                    // do something with entry.Value or entry.Key
                    if (entry.Key.StartsWith("pptr_"))
                    {
                        if (!playerPortraitList.Contains(entry.Key))
                        {
                            playerPortraitList.Add(entry.Key);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                gv.errorLog(ex.ToString());
            }
            //DEFAULTS
            try
            {
                //Load from PlayerTokens folder last
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\default\\NewModule\\graphics", "*.png");
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if (filename.StartsWith("pptr_"))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                if (!playerPortraitList.Contains(fileNameWithOutExt))
                                {
                                    playerPortraitList.Add(fileNameWithOutExt);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            gv.errorLog(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                gv.errorLog(ex.ToString());
            }
        }

	    public void setControlsStart()
	    {			
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;

            if (btnPortraitsLeft == null)
            {
                btnPortraitsLeft = new IbbButton(gv, 1.0f);
            }
			    btnPortraitsLeft.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnPortraitsLeft.Img2 = "ctrl_left_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    btnPortraitsLeft.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnPortraitsLeft.X = 4 * gv.squareSize;
			    btnPortraitsLeft.Y = (1 * gv.squareSize / 2);
                btnPortraitsLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPortraitsLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnPageIndex == null)
            {
                btnPageIndex = new IbbButton(gv, 1.0f);
            }
			    btnPageIndex.Img = "btn_small_off"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
			    btnPageIndex.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnPageIndex.Text = "1";
			    btnPageIndex.X = 5 * gv.squareSize;
			    btnPageIndex.Y = (1 * gv.squareSize / 2);
                btnPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnPortraitsRight == null)
            {
                btnPortraitsRight = new IbbButton(gv, 1.0f);
            }
			    btnPortraitsRight.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnPortraitsRight.Img2 = "ctrl_right_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnPortraitsRight.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnPortraitsRight.X = 6 * gv.squareSize;
			    btnPortraitsRight.Y = (1 * gv.squareSize / 2);
                btnPortraitsRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPortraitsRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);


            if (btnAction == null)
            {
                btnAction = new IbbButton(gv, 1.0f);
            }
                btnAction.Text = "USE SELECTED";
                btnAction.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnAction.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnAction.X = (gv.squareSize * gv.squaresInWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity);
			    btnAction.Y = 6 * gv.squareSize - pH;
                btnAction.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAction.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            if (btnExit == null)
            {
                btnExit = new IbbButton(gv, 1.0f);
            }
                btnExit.Text = "EXIT";
                btnExit.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnExit.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnExit.X = (gv.squareSize * gv.squaresInWidth / 2);
                btnExit.Y = 6 * gv.squareSize - pH;
                btnExit.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnExit.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            for (int y = 0; y < slotsPerPage; y++)
		    {
			    IbbPortrait btnNew = new IbbPortrait(gv, 0.8f);
                btnNew.ImgBG = "item_slot"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
                btnNew.Glow = "btn_ptr_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    if (y < 5)
			    {
				    btnNew.X = ((y + 2) * gv.squareSize) + (padW * (y * 2 + 1));
				    btnNew.Y = 2 * gv.squareSize - gv.squareSize / 4;
			    }
			    else
			    {
				    btnNew.X = ((y - 5 + 2) * gv.squareSize) + (padW * ((y - 5) * 2 + 1));
				    btnNew.Y = 4 * gv.squareSize - gv.squareSize / 4;
			    }

                btnNew.Height = (int)(gv.ibpheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibpwidth * gv.screenDensity);	
			
			    btnPortraitSlot.Add(btnNew);
		    }			
	    }
	
	    //INVENTORY SCREEN (COMBAT and MAIN)
        public void redrawPortraitSelector()
        {
            //IF CONTROLS ARE NULL, CREATE THEM
    	    if (btnAction == null)
    	    {
    		    setControlsStart();
    	    }
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = gv.squareSize * 4;
    	    int textH = (int)gv.fontHeight;
            int spacing = textH;
            int tabX = pW * 4;
    	    int tabX2 = 5 * gv.squareSize + pW * 2;
    	    int leftStartY = pH * 4;
    	    int tabStartY = 5 * gv.squareSize + pW * 10;
    	
            //DRAW TEXT		
		    locY = (pH * 2);
		    gv.DrawText("Portrait Selection", locX, locY, "wh");
		    
		    //DRAW LEFT/RIGHT ARROWS and PAGE INDEX
		    btnPageIndex.Draw();
		    btnPortraitsLeft.Draw();
		    btnPortraitsRight.Draw();		
		
		    //DRAW ALL INVENTORY SLOTS		
		    int cntSlot = 0;
		    foreach (IbbPortrait btn in btnPortraitSlot)
		    {
			    if (cntSlot == ptrSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    if ((cntSlot + (ptrPageIndex * slotsPerPage)) < playerPortraitList.Count)
			    {
                    //gv.cc.DisposeOfBitmap(ref btn.Img);
                    btn.Img = playerPortraitList[cntSlot + (ptrPageIndex * slotsPerPage)];
			    }
			    else
			    {
				    btn.Img = null;
			    }
			    btn.Draw();
			    cntSlot++;
		    }		
		    
		    btnAction.Draw();
            btnExit.Draw();
        }
        public void onTouchPortraitSelector(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType)
	    {
		    btnPortraitsLeft.glowOn = false;
		    btnPortraitsRight.glowOn = false;
		    btnAction.glowOn = false;
            btnExit.glowOn = false;
            
            switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) eX;
			    int y = (int) eY;
			    if (btnPortraitsLeft.getImpact(x, y))
			    {
				    btnPortraitsLeft.glowOn = true;
			    }
			    else if (btnPortraitsRight.getImpact(x, y))
			    {
				    btnPortraitsRight.glowOn = true;
			    }
			    else if (btnAction.getImpact(x, y))
			    {
				    btnAction.glowOn = true;
			    }
                else if (btnExit.getImpact(x, y))
                {
                    btnExit.glowOn = true;
                }
                break;
			
		    case MouseEventType.EventType.MouseUp:
			    x = (int) eX;
			    y = (int) eY;
			
			    btnPortraitsLeft.glowOn = false;
			    btnPortraitsRight.glowOn = false;
			    btnAction.glowOn = false;
                btnExit.glowOn = false;
                
                for (int j = 0; j < slotsPerPage; j++)
			    {
				    if (btnPortraitSlot[j].getImpact(x, y))
				    {
					    if (ptrSlotIndex == j)
                        {
                            //return to calling screen
                            if (callingScreen.Equals("party"))
                            {
                                gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex].portraitFilename = playerPortraitList[GetIndex()];
                                gv.screenType = "party";
                                gv.screenParty.portraitLoad(gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex]);
                            }
                            else if (callingScreen.Equals("pcCreation"))
                            {
                                //set PC portrait filename to the currently selected image
                                gv.screenPcCreation.pc.portraitFilename = playerPortraitList[GetIndex()];                            
                                gv.screenType = "pcCreation";
                                gv.screenPcCreation.portraitLoad(gv.screenPcCreation.pc);
                            }
                            doCleanUp();
                        }
					    ptrSlotIndex = j;
				    }
			    }
			    if (btnPortraitsLeft.getImpact(x, y))
			    {
				    if (ptrPageIndex > 0)
				    {
					    ptrPageIndex--;
					    btnPageIndex.Text = (ptrPageIndex + 1) + "";
				    }
			    }
			    else if (btnPortraitsRight.getImpact(x, y))
			    {
				    if (ptrPageIndex < maxPages)
				    {
					    ptrPageIndex++;
					    btnPageIndex.Text = (ptrPageIndex + 1) + "";
				    }
			    }
			    else if (btnAction.getImpact(x, y))
			    {
				    //return to calling screen
                    if (callingScreen.Equals("party"))
                    {
                        gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex].portraitFilename = playerPortraitList[GetIndex()];
                        gv.screenType = "party";
                        gv.screenParty.portraitLoad(gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex]);
                    }
                    else if (callingScreen.Equals("pcCreation"))
                    {
                        //set PC portrait filename to the currently selected image
                        gv.screenPcCreation.pc.portraitFilename = playerPortraitList[GetIndex()];
                        gv.screenType = "pcCreation";
                        gv.screenPcCreation.portraitLoad(gv.screenPcCreation.pc);
                    }
					doCleanUp();						
			    }
                else if (btnExit.getImpact(x, y))
                {
                    //do nothing, return to calling screen
                    if (callingScreen.Equals("party"))
                    {
                        gv.screenType = "party";
                    }
                    else if (callingScreen.Equals("pcCreation"))
                    {
                        gv.screenType = "pcCreation";
                    }
                    doCleanUp();
                }                
			    break;		
		    }
	    }
        public void doCleanUp()
	    {
		    btnPortraitSlot.Clear();
		    btnPortraitsLeft = null;
		    btnPortraitsRight = null;
		    btnPageIndex = null;
		    btnAction = null;
            btnExit = null;
	    }
	
	    public int GetIndex()
	    {
		    return ptrSlotIndex + (ptrPageIndex * slotsPerPage);
	    }	
	    public bool isSelectedPtrSlotInPortraitListRange()
	    {
            return GetIndex() < playerPortraitList.Count;
	    }
    }
}
