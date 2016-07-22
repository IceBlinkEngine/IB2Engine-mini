﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class ScreenSpellLevelUp 
    {
	    private Module mod;
	    private GameView gv;
	
	    public int castingPlayerIndex = 0;
	    private int spellSlotIndex = 0;
	    private int slotsPerPage = 20;
	    private List<IbbButton> btnSpellSlots = new List<IbbButton>();
	    private IbbButton btnHelp = null;
	    private IbbButton btnSelect = null;
	    private IbbButton btnExit = null;
	    List<string> spellsToLearnTagsList = new List<string>();
	    private Player pc;
        public bool infoOnly = false; //set to true when called for info only
        private string stringMessageSpellLevelUp = "";
        private IbbHtmlTextBox description;
	
	
	    public ScreenSpellLevelUp(Module m, GameView g) 
	    {
		    mod = m;
		    gv = g;
		    setControlsStart();
		    pc = new Player();
		    stringMessageSpellLevelUp = gv.cc.loadTextToString("data/MessageSpellLevelUp.txt");
	    }
	
	    public void resetPC(bool info_only, Player p)
	    {
		    pc = p;
            infoOnly = info_only;
        }
	
	    public void setControlsStart()
	    {			
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;

            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;
		
		    if (btnSelect == null)
		    {
			    btnSelect = new IbbButton(gv, 0.8f);	
			    btnSelect.Text = "LEARN SELECTED SPELL";
			    btnSelect.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnSelect.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnSelect.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
			    btnSelect.Y = 9 * gv.squareSize + pH * 2;
                btnSelect.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSelect.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnHelp == null)
		    {
			    btnHelp = new IbbButton(gv, 0.8f);	
			    btnHelp.Text = "HELP";
			    btnHelp.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnHelp.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnHelp.X = 5 * gv.squareSize + padW * 1 + gv.oXshift;
			    btnHelp.Y = 9 * gv.squareSize + pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    if (btnExit == null)
		    {
			    btnExit = new IbbButton(gv, 0.8f);	
			    btnExit.Text = "EXIT";
			    btnExit.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnExit.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnExit.X = (15 * gv.squareSize) - padW * 1 + gv.oXshift;
			    btnExit.Y = 9 * gv.squareSize + pH * 2;
                btnExit.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnExit.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    for (int y = 0; y < slotsPerPage; y++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    int x = y % 5;
			    int yy = y / 5;
			    btnNew.X = ((x + 4) * gv.squareSize) + (padW * (x+1)) + gv.oXshift;
			    btnNew.Y = (1 + yy) * gv.squareSize + (padW * yy);

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnSpellSlots.Add(btnNew);
		    }			
	    }
	
	    //CAST SELECTOR SCREEN (COMBAT and MAIN)
        public void redrawSpellLevelUp(bool inPcCreation)
        {
    	    spellsToLearnTagsList.Clear();
    	    fillToLearnList();
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
            int textH = (int)gv.drawFontRegHeight;
    	    int spacing = textH;
            int tabX = 5 * gv.squareSize + pW * 3;
            int noticeX = 5 * gv.squareSize + pW * 3;
            int noticeY = pH * 1 + spacing;
    	    int tabStartY = 4 * gv.squareSize + pW * 10;

            if (!infoOnly)
            {
                //DRAW TEXT		
                locY = (gv.squareSize * 0) + (pH * 2);
                gv.DrawText("Select One " + mod.spellLabelSingular + " to Learn", noticeX, pH * 1, 1.0f, Color.Gray);
                gv.DrawText(getCastingPlayer().name + " SP: " + getCastingPlayer().sp + "/" + getCastingPlayer().spMax, pW * 50, pH * 1, 1.0f, Color.Yellow);

                //DRAW NOTIFICATIONS
                if (isSelectedSpellSlotInKnownSpellsRange())
                {
                    Spell sp = GetCurrentlySelectedSpell();
                    Player pc = getCastingPlayer();

                    //check to see if already known
                    if (pc.knownSpellsTags.Contains(sp.tag))
                    {
                        //say that you already know this one
                        gv.DrawText("Already Known", noticeX, noticeY, 1.0f, Color.Yellow);
                    }
                    else //spell not known
                    {
                        //check if available to learn
                        if (isAvailableToLearn(sp.tag))
                        {
                            gv.DrawText("Available to Learn", noticeX, noticeY, 1.0f, Color.Lime);
                        }
                        else //not available yet
                        {
                            gv.DrawText(mod.spellLabelSingular + " Not Available to Learn Yet", noticeX, noticeY, 1.0f, Color.Red);
                        }
                    }
                }
            }	
		
		    //DRAW ALL SPELL SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnSpellSlots)
		    {			
			    Player pc = getCastingPlayer();						
			
			    if (cntSlot == spellSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			
			    //show only spells for the PC class
			    if (cntSlot < pc.playerClass.spellsAllowed.Count)
			    {
				    SpellAllowed sa = pc.playerClass.spellsAllowed[cntSlot];
				    Spell sp = mod.getSpellByTag(sa.tag);

                    if (infoOnly)
                    {
                        if (pc.knownSpellsTags.Contains(sp.tag)) //check to see if already known, if so turn on button
                        {
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small");
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        }
                        else //spell not known yet
                        {
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small_off");
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(sp.spellImage + "_off");                            
                        }
                    }
                    else
                    {
                        if (pc.knownSpellsTags.Contains(sp.tag)) //check to see if already known, if so turn off button
                        {
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        }
                        else //spell not known yet
                        {
                            if (isAvailableToLearn(sp.tag)) //if available to learn, turn on button
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);	
                                gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                            }
                            else //not available to learn, turn off button
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
                                gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = gv.cc.LoadBitmap(sp.spellImage + "_off");
                            }
                        }
                    }				
			    }
			    else //slot is not in spells allowed index range
			    {
                    gv.cc.DisposeOfBitmap(ref btn.Img);
                    btn.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
				    btn.Img2 = null;
			    }			
			    btn.Draw();
			    cntSlot++;
		    }

            //DRAW DESCRIPTION BOX
            locY = tabStartY;
            if (isSelectedSpellSlotInKnownSpellsRange())
            {
                Spell sp = GetCurrentlySelectedSpell();
                
                string textToSpan = "<u>Description</u>" + "<BR>";
                textToSpan += "<b><i><big>" + sp.name + "</big></i></b><BR>";
                textToSpan += "SP Cost: " + sp.costSP + "<BR>";
                textToSpan += "Target Range: " + sp.range + "<BR>";
                textToSpan += "Area of Effect Radius: " + sp.aoeRadius + "<BR>";
                textToSpan += "Available at Level: " + getLevelAvailable(sp.tag) + "<BR>";
                textToSpan += "<BR>";
                textToSpan += sp.description;

                description.tbXloc = 11 * gv.squareSize;
                description.tbYloc = 1 * gv.squareSize;
                description.tbWidth = pW * 40;
                description.tbHeight = pH * 80;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
            }

            if (infoOnly)
            {
                btnSelect.Text = "RETURN";
                btnSelect.Draw();
            }
            else
            {
                btnSelect.Text = "LEARN SELECTED " + mod.spellLabelSingular.ToUpper();
                btnHelp.Draw();
                btnExit.Draw();
                btnSelect.Draw();
            }
        }
        public void onTouchSpellLevelUp(MouseEventArgs e, MouseEventType.EventType eventType, bool inPcCreation)
	    {
		    btnHelp.glowOn = false;
		    btnExit.glowOn = false;
		    btnSelect.glowOn = false;
		
		    switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) e.X;
			    int y = (int) e.Y;
			    if (btnHelp.getImpact(x, y))
			    {
				    btnHelp.glowOn = true;
			    }
			    else if (btnSelect.getImpact(x, y))
			    {
				    btnSelect.glowOn = true;
			    }
			    else if (btnExit.getImpact(x, y))
			    {
				    btnExit.glowOn = true;
			    }
			    break;

            case MouseEventType.EventType.MouseUp:
                x = (int)e.X;
                y = (int)e.Y;
			
			    btnHelp.glowOn = false;
			    btnExit.glowOn = false;
			    btnSelect.glowOn = false;
			
			    for (int j = 0; j < slotsPerPage; j++)
			    {
				    if (btnSpellSlots[j].getImpact(x, y))
				    {
                        gv.PlaySound("btn_click");
					    spellSlotIndex = j;
				    }
			    }
			    if (btnHelp.getImpact(x, y))
			    {
                    if (!infoOnly)
                    {
                        gv.PlaySound("btn_click");
                        tutorialMessageCastingScreen();
                    }
			    }
			    else if (btnSelect.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
                    if (infoOnly)
                    {
                        gv.screenType = "party";
                    }
                    else
                    {
                        doSelectedSpellToLearn(inPcCreation);
                    }
			    }
			    else if (btnExit.getImpact(x, y))
			    {
                    if (!infoOnly)
                    {
                        gv.PlaySound("btn_click");
                        if (inPcCreation)
                        {
                            gv.screenType = "pcCreation";
                        }
                        else
                        {
                            gv.screenType = "party";
                        }
                    }						
			    }
			    break;		
		    }
	    }
    
        public void doSelectedSpellToLearn(bool inPcCreation)
        {
    	    if (isSelectedSpellSlotInKnownSpellsRange())
		    {
			    Spell sp = GetCurrentlySelectedSpell();
			    if (isAvailableToLearn(sp.tag))
			    {
				    Player pc = getCastingPlayer();		
				    pc.knownSpellsTags.Add(sp.tag);
				    if (inPcCreation)
				    {
					    gv.screenPcCreation.SaveCharacter(pc);
			    	    gv.screenPartyBuild.pcList.Add(pc);
			    	    gv.screenType = "partyBuild";
				    }
				    else
				    {
					    gv.screenType = "party";
					    gv.screenParty.spellGained += sp.name + ", ";
					    gv.screenParty.doLevelUpSummary();
				    }
			    }
			    else
			    {
				    gv.sf.MessageBox("Can't learn that spell, try another or exit");
			    }
		    }	
        }
            
        public bool isAvailableToLearn(string spellTag)
        {
    	    if (spellsToLearnTagsList.Contains(spellTag))
    	    {
    		    return true;
    	    }
    	    return false;
        }
    
        public void fillToLearnList()
        {
    	    spellsToLearnTagsList = getCastingPlayer().getSpellsToLearn();	    
        }
    
        public Spell GetCurrentlySelectedSpell()
	    {
    	    SpellAllowed sa = getCastingPlayer().playerClass.spellsAllowed[spellSlotIndex];
		    return mod.getSpellByTag(sa.tag);
	    }
	    public bool isSelectedSpellSlotInKnownSpellsRange()
	    {
		    return spellSlotIndex < getCastingPlayer().playerClass.spellsAllowed.Count;
	    }	
	    public int getLevelAvailable(string tag)
	    {
		    SpellAllowed sa = getCastingPlayer().playerClass.getSpellAllowedByTag(tag);
		    if (sa != null)
		    {
			    return sa.atWhatLevelIsAvailable;
		    }
		    return 0;
	    }
	    public Player getCastingPlayer()
	    {
		    return pc;
	    }
	    public void tutorialMessageCastingScreen()
        {
		    gv.sf.MessageBoxHtml(this.stringMessageSpellLevelUp);	
        }

    }
}