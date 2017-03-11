using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class ScreenCastSelector 
    {
	    //private Module gv.mod;
	    private GameView gv;
	
	    public int castingPlayerIndex = 0;
	    private int spellSlotIndex = 0;
	    private int slotsPerPage = 24;
	    private List<IbbButton> btnSpellSlots = new List<IbbButton>();
	    private IbbButton btnHelp = null;
	    private IbbButton btnSelect = null;
	    private IbbButton btnExit = null;
	    private string stringMessageCastSelector = "";
        private IbbHtmlTextBox description;
        public bool isInCombat = false;
	
	    public ScreenCastSelector(Module m, GameView g) 
	    {
		    //gv.mod = m;
		    gv = g;
		    stringMessageCastSelector = gv.cc.loadTextToString("MessageCastSelector.txt");
	    }
	
	    public void setControlsStart()
	    {			
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;

            if (description == null)
            {
                description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            }
            description.tbXloc = 320;
            description.tbYloc = 100;
            description.tbWidth = 300;
            description.tbHeight = 500;
            description.showBoxBorder = false;

            if (btnSelect == null)
            {
                btnSelect = new IbbButton(gv, 0.8f);
            }
			    btnSelect.Text = "CAST SELECTED SPELL";
			    btnSelect.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnSelect.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnSelect.X = 2 * gv.squareSize + padW * 2;
                btnSelect.Y = 6 * gv.squareSize - pH * 2;
                btnSelect.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSelect.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            if (btnHelp == null)
            {
                btnHelp = new IbbButton(gv, 0.8f);
            }
			    btnHelp.Text = "HELP";
			    btnHelp.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnHelp.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnHelp.X = 1 * gv.squareSize + padW * 1;
			    btnHelp.Y = 6 * gv.squareSize - pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnExit == null)
            {
                btnExit = new IbbButton(gv, 0.8f);
            }
			    btnExit.Text = "EXIT";
			    btnExit.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnExit.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnExit.X = (5 * gv.squareSize) + padW * 5;
			    btnExit.Y = 6 * gv.squareSize - pH * 2;
                btnExit.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnExit.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            for (int y = 0; y < slotsPerPage; y++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnNew.ImgOff = "btn_small_off";
                btnNew.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    int x = y % 6;
			    int yy = y / 6;
			    btnNew.X = ((x) * gv.squareSize) + (padW * (x+1));
			    btnNew.Y = (1 + yy) * gv.squareSize + (padW * yy);

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnSpellSlots.Add(btnNew);
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
                    Spell sp = gv.mod.getSpellByTag(sa.tag);

                    btn.Img2 = sp.spellImage;
                    btn.Img2Off = sp.spellImage + "_off";

                    if (pc.knownSpellsTags.Contains(sp.tag))
                    {
                        if (isInCombat) //all spells can be used in combat
                        {
                            btn.btnState = buttonState.Normal;
                        }
                        //not in combat so check if spell can be used on adventure maps
                        else if ((sp.useableInSituation.Equals("Always")) || (sp.useableInSituation.Equals("OutOfCombat")))
                        {
                            btn.btnState = buttonState.Normal;
                        }
                        else //can't be used on adventure map
                        {
                            btn.btnState = buttonState.Off;
                        }
                    }
                    else //spell not known
                    {
                        btn.btnState = buttonState.Off;
                    }
                }
                else //slot is not in spells allowed index range
                {
                    btn.btnState = buttonState.Off;
                    btn.Img2 = null;
                    btn.Img2Off = null;
                }
			    cntSlot++;
		    }
	    }
	
	    //CAST SELECTOR SCREEN (COMBAT and MAIN)
        public void redrawCastSelector(bool inCombat)
        {
            Player pc = getCastingPlayer();

            isInCombat = inCombat;
    	    //IF CONTROLS ARE NULL, CREATE THEM
    	    if (btnSelect == null)
    	    {
    		    setControlsStart();
    	    }
            
            btnSelect.Text = "CAST SELECTED " + gv.mod.getPlayerClass(getCastingPlayer().classTag).spellLabelSingular.ToUpper();

            int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
            //int textH = (int)gv.cc.MeasureString("GetHeight", gv.drawFontReg, gv.Width).Height;
            int textH = (int)gv.fontHeight;
            int spacing = textH; 
            //int spacing = (int)gv.mSheetTextPaint.getTextSize() + pH;
    	    int tabX = pW * 4;
    	    int noticeX = pW * 5;
    	    int noticeY = pH * 3 + spacing;
    	    int leftStartY = pH * 3;
    	    int tabStartY = 4 * gv.squareSize + pW * 10;

            //DRAW TEXT		
		    locY = (gv.squareSize * 0) + (pH * 2);
		    //gv.mSheetTextPaint.setColor(Color.LTGRAY);
		    gv.DrawText("Select a " + gv.mod.getPlayerClass(pc.classTag).spellLabelSingular + " to Cast", noticeX, pH * 3, "wh");
		    //gv.mSheetTextPaint.setColor(Color.YELLOW);
		    gv.DrawText(getCastingPlayer().name + " SP: " + getCastingPlayer().sp + "/" + getCastingPlayer().spMax, pW * 55, leftStartY, "yl");
		
		    //DRAW NOTIFICATIONS
		    if (isSelectedSpellSlotInKnownSpellsRange())
		    {
			    Spell sp = GetCurrentlySelectedSpell();			    	
			
			    if (pc.knownSpellsTags.Contains(sp.tag))
			    {
				    if (inCombat) //all spells can be used in combat
				    {
					    //if currently selected is usable say "Available to Cast" in lime
					    if (pc.sp >= GetCurrentlySelectedSpell().costSP)
					    {
						    //gv.mSheetTextPaint.setColor(Color.GREEN);
                            gv.DrawText("Available to Cast", noticeX, noticeY, "gn");
					    }
					    else //if known but not enough spell points, "Insufficient SP to Cast" in yellow
					    {
						    //gv.mSheetTextPaint.setColor(Color.YELLOW);
                            gv.DrawText("Insufficient SP", noticeX, noticeY, "yl");
					    }					
				    }
				    //not in combat so check if spell can be used on adventure maps
				    else if ((sp.useableInSituation.Equals("Always")) || (sp.useableInSituation.Equals("OutOfCombat")))
				    {
					    //if currently selected is usable say "Available to Cast" in lime
					    if (pc.sp >= GetCurrentlySelectedSpell().costSP)
					    {
						    //gv.mSheetTextPaint.setColor(Color.GREEN);
                            gv.DrawText("Available to Cast", noticeX, noticeY, "gn");
					    }
					    else //if known but not enough spell points, "Insufficient SP to Cast" in yellow
					    {
						    //gv.mSheetTextPaint.setColor(Color.YELLOW);
                            gv.DrawText("Insufficient SP", noticeX, noticeY, "yl");
					    }					
				    }
				    else //can't be used on adventure map
				    {
					    //gv.mSheetTextPaint.setColor(Color.YELLOW);
                        gv.DrawText("Not Available Here", noticeX, noticeY, "yl");
				    }	
			    }
			    else //spell not known
			    {
				    //if unknown spell, "Spell Not Known Yet" in red
				    //gv.mSheetTextPaint.setColor(Color.RED);
                    gv.DrawText(gv.mod.getPlayerClass(pc.classTag).spellLabelSingular + " Not Known Yet", noticeX, noticeY, "rd");
			    }
		    }		
		
		    //DRAW ALL SPELL SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnSpellSlots)
		    {			
			    //Player pc = getCastingPlayer();						
			
			    if (cntSlot == spellSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			
			    		
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;		
		    if (isSelectedSpellSlotInKnownSpellsRange())
		    {
			    Spell sp = GetCurrentlySelectedSpell();
			    string textToSpan = "<gy>Description</gy><BR>";
	            textToSpan += "<gn>" + sp.name + "</gn><BR>";
	            textToSpan += "<yl>SP Cost: " + sp.costSP + "</yl><BR>";
	            textToSpan += "Target Range: " + sp.range + "<BR>";
	            textToSpan += "Area of Effect Radius: " + sp.aoeRadius + "<BR>";
	            textToSpan += "Available at Level: " + getLevelAvailable(sp.tag) + "<BR>";
	            textToSpan += "<BR>";
	            textToSpan += sp.description;

                description.tbXloc = 7 * gv.squareSize + pW;
                description.tbYloc = 1 * gv.squareSize;
                description.tbWidth = 4 * gv.squareSize;
                description.tbHeight = pH * 80;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
		    }
		
		    btnHelp.Draw();	
		    btnExit.Draw();	
		    btnSelect.Draw();
            if (gv.showMessageBox)
            {
                gv.messageBox.onDrawLogBox();
            }
        }
        public void onTouchCastSelector(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType, bool inCombat)
	    {
		    btnHelp.glowOn = false;
		    btnExit.glowOn = false;
		    btnSelect.glowOn = false;
            if (gv.showMessageBox)
            {
                gv.messageBox.btnReturn.glowOn = false;
            }

            switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) eX;
			    int y = (int) eY;

                if (gv.showMessageBox)
                {
                    if (gv.messageBox.btnReturn.getImpact(x, y))
                    {
                        gv.messageBox.btnReturn.glowOn = true;
                    }
                    return;
                }

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
			    x = (int) eX;
			    y = (int) eY;
			
			    btnHelp.glowOn = false;
			    //btnInfo.glowOn = false;
			    btnExit.glowOn = false;
			    btnSelect.glowOn = false;

                if (gv.showMessageBox)
                {
                    gv.messageBox.btnReturn.glowOn = false;
                }
                if (gv.showMessageBox)
                {
                    if (gv.messageBox.btnReturn.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        gv.showMessageBox = false;
                    }
                    return;
                }

                for (int j = 0; j < slotsPerPage; j++)
			    {
				    if (btnSpellSlots[j].getImpact(x, y))
				    {
					    spellSlotIndex = j;
				    }
			    }
			    if (btnHelp.getImpact(x, y))
			    {
                    gv.showMessageBox = true;
                    tutorialMessageCastingScreen();
			    }
			    else if (btnSelect.getImpact(x, y))
			    {
				    doSelectedSpell(inCombat);
			    }
			    else if (btnExit.getImpact(x, y))
			    {
				    if (inCombat)
				    {
					    if (gv.screenCombat.canMove)
					    {
						    gv.screenCombat.currentCombatMode = "move";
					    }
					    else
					    {
						    gv.screenCombat.currentCombatMode = "attack";
					    }
					    gv.screenType = "combat";
					    doCleanUp();
				    }
				    else
				    {
					    gv.screenType = "main";	
					    doCleanUp();
				    }							
			    }
			    break;		
		    }
	    }
    
        public void doCleanUp()
	    {
    	    btnSpellSlots.Clear();
    	    btnHelp = null;
    	    btnSelect = null;
    	    btnExit = null;
	    }
    
        public void doSelectedSpell(bool inCombat)
	    {            
		    if (isSelectedSpellSlotInKnownSpellsRange())
		    {
			    //only allow to cast spells that you know and are usable on this map
			    if (getCastingPlayer().knownSpellsTags.Contains(GetCurrentlySelectedSpell().tag))
			    {
				    if (inCombat) //Combat Map
				    {
					
					    if (getCastingPlayer().sp >= GetCurrentlySelectedSpell().costSP)
					    {
						    gv.cc.currentSelectedSpell = GetCurrentlySelectedSpell();
						    gv.screenType = "combat";
						    gv.screenCombat.currentCombatMode = "cast";
						    doCleanUp();
					    }
					    else
					    {
						    //Toast.makeText(gv.gameContext, "Not Enough SP for that spell", Toast.LENGTH_SHORT).show();
					    }
				    }
				    else //Adventure Map
				    {
					    //only cast if useable on adventure maps
					    if ((GetCurrentlySelectedSpell().useableInSituation.Equals("Always")) || (GetCurrentlySelectedSpell().useableInSituation.Equals("OutOfCombat")))
					    {						
						    if (getCastingPlayer().sp >= GetCurrentlySelectedSpell().costSP)
						    {
							    gv.cc.currentSelectedSpell = GetCurrentlySelectedSpell();
                                //if target is SELF then just do doTraitTarget(self)
                                if (gv.cc.currentSelectedSpell.spellTargetType.Equals("Self"))
                                {
                                    doSpellTarget(getCastingPlayer(), getCastingPlayer());
                                }
                                else
                                {
                                    //ask for target
                                    // selected to USE ITEM

                                    List<string> pcNames = new List<string>();
                                    pcNames.Add("cancel");
                                    foreach (Player p in gv.mod.playerList)
                                    {
                                        pcNames.Add(p.name);
                                    }

                                    //If only one PC, do not show select PC dialog...just go to cast selector
                                    if (gv.mod.playerList.Count == 1)
                                    {
                                        try
                                        {
                                            Player target = gv.mod.playerList[0];
                                            gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, target, target, true);
                                            gv.screenType = "main";
                                            doCleanUp();
                                            return;
                                        }
                                        catch (Exception ex)
                                        {
                                            gv.errorLog(ex.ToString());
                                        }
                                    }

                                    gv.itemListSelector.setupIBminiItemListSelector(gv, pcNames, gv.mod.getPlayerClass(getCastingPlayer().classTag).spellLabelSingular + " Target", "castselectorspelltarget");
                                    gv.itemListSelector.showIBminiItemListSelector = true;
                                }
						    }
						    else
						    {
							    //Toast.makeText(gv.gameContext, "Not Enough SP for that spell", Toast.LENGTH_SHORT).show();
						    }
					    }
				    }
			    }
		    }            
	    }
        public void doSpellTarget(int selectedIndex)
        {
            Player pc = getCastingPlayer();
            if (selectedIndex > 0)
            {
                Player target = gv.mod.playerList[selectedIndex - 1];
                doSpellTarget(pc, target);
            }
            else if (selectedIndex == 0) // selected "cancel"
            {
                //do nothing
            }
        }
        public void doSpellTarget(Player pc, Player target)
        {
            try
            {
                gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, pc, target, !isInCombat);
                gv.screenType = "main";
                doCleanUp();
            }
            catch (Exception ex)
            {
                gv.sf.MessageBoxHtml("error with Pc Selector screen: " + ex.ToString());
                gv.errorLog(ex.ToString());
            }
        }

        public Spell GetCurrentlySelectedSpell()
	    {
    	    SpellAllowed sa = getCastingPlayer().playerClass.spellsAllowed[spellSlotIndex];
		    return gv.mod.getSpellByTag(sa.tag);
	    }
	    public bool isSelectedSpellSlotInKnownSpellsRange()
	    {
		    return spellSlotIndex < getCastingPlayer().playerClass.spellsAllowed.Count;
	    }	
	    public int getLevelAvailable(String tag)
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
		    return gv.mod.playerList[castingPlayerIndex];
	    }
	    public void tutorialMessageCastingScreen()
        {
		    //gv.sf.MessageBoxHtml(this.stringMessageCastSelector);
            gv.messageBox.logLinesList.Clear();
            gv.messageBox.AddHtmlTextToLog(this.stringMessageCastSelector);
            gv.messageBox.currentTopLineIndex = 0;
        }
    }
}
