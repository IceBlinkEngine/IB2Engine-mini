﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2mini
{
    public class ScreenTokenSelector
    {
        //public Module gv.mod;
	    public GameView gv;
        public Player pc;
	    private int tknPageIndex = 0;
	    private int tknSlotIndex = 0;
	    private int slotsPerPage = 15;
        private int maxPages = 40;
	    private List<IbbButton> btnTokenSlot = new List<IbbButton>();
	    private IbbButton btnTokensLeft = null;
	    private IbbButton btnTokensRight = null;
	    private IbbButton btnPageIndex = null;
	    private IbbButton btnAction = null;
        private IbbButton btnExit = null;
        public string callingScreen = "pcCreation"; //party, pcCreation
        public List<string> playerTokenList = new List<string>();

        public ScreenTokenSelector(Module m, GameView g)
	    {
		    //gv.mod = m;
		    gv = g;
	    }

        public void resetTokenSelector(string callingScreenToReturnTo, Player p)
        {
            pc = p;
            callingScreen = callingScreenToReturnTo;
            LoadPlayerTokenList();
        }

        public void LoadPlayerTokenList()
        {
            playerTokenList.Clear();
            try
            {
                //Load from module folder first
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\override"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\override", "*.png");
                    //directory.mkdirs(); 
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if (filename.StartsWith("pc_"))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                playerTokenList.Add(fileNameWithOutExt);
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
                    if (entry.Key.StartsWith("pc_"))
                    {
                        if (!playerTokenList.Contains(entry.Key))
                        {
                            playerTokenList.Add(entry.Key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                gv.errorLog(ex.ToString());
            }
            try
            {
                //Load from PlayerTokens folder last
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\\\default\\NewModule\\graphics", "*.png");
                    //directory.mkdirs(); 
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if (filename.StartsWith("pc_"))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                if (!playerTokenList.Contains(fileNameWithOutExt))
                                {
                                    playerTokenList.Add(fileNameWithOutExt);
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

            if (btnTokensLeft == null)
            {
                btnTokensLeft = new IbbButton(gv, 1.0f);
            }
			    btnTokensLeft.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnTokensLeft.Img2 = "ctrl_left_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    btnTokensLeft.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnTokensLeft.X = 4 * gv.squareSize;
			    btnTokensLeft.Y = (1 * gv.squareSize / 2);
                btnTokensLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTokensLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);

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

            if (btnTokensRight == null)
            {
                btnTokensRight = new IbbButton(gv, 1.0f);
            }
			    btnTokensRight.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnTokensRight.Img2 = "ctrl_right_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnTokensRight.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnTokensRight.X = 6 * gv.squareSize;
			    btnTokensRight.Y = (1 * gv.squareSize / 2);
                btnTokensRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTokensRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);


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
			    IbbButton btnNew = new IbbButton(gv, 1.0f);
                //gv.cc.DisposeOfBitmap(ref btnNew.Img);
                btnNew.Img = "item_slot"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
                //gv.cc.DisposeOfBitmap(ref btnNew.Glow);
                btnNew.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    if (y < 5)
			    {
				    btnNew.X = ((y + 2) * gv.squareSize + gv.squareSize / 2) + (padW * (y + 1));
				    btnNew.Y = 2 * gv.squareSize;
			    }
			    else if ((y >=5 ) && (y < 10))
			    {
				    btnNew.X = ((y - 5 + 2) * gv.squareSize + gv.squareSize / 2) + (padW * ((y - 5) + 1));
				    btnNew.Y = 3 * gv.squareSize + padW;
			    }
                else
                {
                    btnNew.X = ((y - 10 + 2) * gv.squareSize + gv.squareSize / 2) + (padW * ((y - 10) + 1));
                    btnNew.Y = 4 * gv.squareSize + (padW * 2);
                }

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);
			
			    btnTokenSlot.Add(btnNew);
		    }			
	    }
	
	    //INVENTORY SCREEN (COMBAT and MAIN)
        public void redrawTokenSelector()
        {
            //IF CONTROLS ARE NULL, CREATE THEM
    	    if (btnAction == null)
    	    {
    		    setControlsStart();
    	    }
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
            int locX = gv.squareSize * 4 + pW * 2;

            int textH = (int)gv.fontHeight;
            int spacing = textH;
            int tabX = pW * 4;
    	    int tabX2 = 5 * gv.squareSize + pW * 2;
    	    int leftStartY = pH * 4;
    	    int tabStartY = 5 * gv.squareSize + pW * 10;
    	
            //DRAW TEXT		
		    locY = (pH * 2);
		    gv.DrawText("Token Selection", locX, locY, "wh");
		    
		    //DRAW LEFT/RIGHT ARROWS and PAGE INDEX
		    btnPageIndex.Draw();
		    btnTokensLeft.Draw();
		    btnTokensRight.Draw();		
		
		    //DRAW ALL INVENTORY SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnTokenSlot)
		    {
			    if (cntSlot == tknSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    if ((cntSlot + (tknPageIndex * slotsPerPage)) < playerTokenList.Count)
			    {
                    //gv.cc.DisposeOfBitmap(ref btn.Img2);
                    btn.Img2 = playerTokenList[cntSlot + (tknPageIndex * slotsPerPage)];
			    }
			    else
			    {
				    btn.Img2 = null;
			    }
			    btn.Draw();
			    cntSlot++;
		    }		
		    
		    btnAction.Draw();
            btnExit.Draw();
        }
        public void onTouchTokenSelector(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType)
	    {
		    btnTokensLeft.glowOn = false;
		    btnTokensRight.glowOn = false;
		    btnAction.glowOn = false;
            btnExit.glowOn = false;
            
            switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) eX;
			    int y = (int) eY;
			    if (btnTokensLeft.getImpact(x, y))
			    {
				    btnTokensLeft.glowOn = true;
			    }
			    else if (btnTokensRight.getImpact(x, y))
			    {
				    btnTokensRight.glowOn = true;
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
			
			    btnTokensLeft.glowOn = false;
			    btnTokensRight.glowOn = false;
			    btnAction.glowOn = false;
                btnExit.glowOn = false;
                
                for (int j = 0; j < slotsPerPage; j++)
			    {
				    if (btnTokenSlot[j].getImpact(x, y))
				    {
					    if (tknSlotIndex == j)
                        {                            
                            //return to calling screen
                            if (callingScreen.Equals("party"))
                            {
                                gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex].tokenFilename = playerTokenList[GetIndex()];
                                gv.screenType = "party";
                                gv.screenParty.tokenLoad(gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex]);
                            }
                            else if (callingScreen.Equals("pcCreation"))
                            {
                                //set PC token filename to the currently selected image
                                gv.screenPcCreation.pc.tokenFilename = playerTokenList[GetIndex()];
                                gv.screenType = "pcCreation";
                                gv.screenPcCreation.tokenLoad(gv.screenPcCreation.pc);
                            }
                            doCleanUp();
                        }
					    tknSlotIndex = j;
				    }
			    }
			    if (btnTokensLeft.getImpact(x, y))
			    {
				    if (tknPageIndex > 0)
				    {
					    tknPageIndex--;
					    btnPageIndex.Text = (tknPageIndex + 1) + "";
				    }
			    }
			    else if (btnTokensRight.getImpact(x, y))
			    {
				    if (tknPageIndex < maxPages)
				    {
					    tknPageIndex++;
					    btnPageIndex.Text = (tknPageIndex + 1) + "";
				    }
			    }
			    else if (btnAction.getImpact(x, y))
			    {
				    //return to calling screen
                    if (callingScreen.Equals("party"))
                    {
                        gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex].tokenFilename = playerTokenList[GetIndex()];
                        gv.screenType = "party";
                        gv.screenParty.tokenLoad(gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex]);
                    }
                    else if (callingScreen.Equals("pcCreation"))
                    {
                        //set PC portrait filename to the currently selected image
                        gv.screenPcCreation.pc.tokenFilename = playerTokenList[GetIndex()];
                        gv.screenType = "pcCreation";
                        gv.screenPcCreation.tokenLoad(gv.screenPcCreation.pc);
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
		    btnTokenSlot.Clear();
		    btnTokensLeft = null;
		    btnTokensRight = null;
		    btnPageIndex = null;
		    btnAction = null;
            btnExit = null;
	    }
	
	    public int GetIndex()
	    {
		    return tknSlotIndex + (tknPageIndex * slotsPerPage);
	    }	
	    public bool isSelectedPtrSlotInPortraitListRange()
	    {
            return GetIndex() < playerTokenList.Count;
	    }
    }
}
