using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class ScreenTraitLevelUp 
    {

	    private Module mod;
	    private GameView gv;
	
	    private int traitSlotIndex = 0;
        private int traitToLearnIndex = 1;
        private int slotsPerPage = 48;
	    private List<IbbButton> btnTraitSlots = new List<IbbButton>();
	    private IbbButton btnHelp = null;
	    private IbbButton btnSelect = null;
	    private IbbButton btnExit = null;
	    List<string> traitsToLearnTagsList = new List<string>();
	    private Player pc;
        public bool infoOnly = false; //set to true when called for info only
        public bool inCombat = false;
        private string stringMessageTraitLevelUp = "";
        private IbbHtmlTextBox description;
	
	
	    public ScreenTraitLevelUp(Module m, GameView g) 
	    {
		    mod = m;
		    gv = g;
		    setControlsStart();
		    pc = new Player();
		    stringMessageTraitLevelUp = gv.cc.loadTextToString("MessageTraitLevelUp.txt");
	    }
	
	    public void resetPC(bool info_only, bool in_Combat, Player p)
	    {
		    pc = p;
            infoOnly = info_only;
            inCombat = in_Combat;
            traitToLearnIndex = 1;
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
			    btnSelect.Text = "LEARN SELECTED TRAIT";
			    btnSelect.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnSelect.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnSelect.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
			    btnSelect.Y = 9 * gv.squareSize + pH * 2;
                btnSelect.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSelect.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnHelp == null)
		    {
			    btnHelp = new IbbButton(gv, 0.8f);	
			    btnHelp.Text = "HELP";
			    btnHelp.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnHelp.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnHelp.X = 5 * gv.squareSize + padW * 1;
			    btnHelp.Y = 9 * gv.squareSize + pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    if (btnExit == null)
		    {
			    btnExit = new IbbButton(gv, 0.8f);	
			    btnExit.Text = "EXIT";
			    btnExit.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnExit.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnExit.X = (15 * gv.squareSize) - padW * 1;
			    btnExit.Y = 9 * gv.squareSize + pH * 2;
                btnExit.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnExit.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    for (int y = 0; y < slotsPerPage; y++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);
                btnNew.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnNew.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    int x = y % 8;
			    int yy = y / 8;
			    btnNew.X = ((x + 1) * gv.squareSize) + (padW * (x + 1));
			    btnNew.Y = (1 + yy) * gv.squareSize + (padW * yy);

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnTraitSlots.Add(btnNew);
		    }			
	    }
	
	    //CAST SELECTOR SCREEN (COMBAT and MAIN)
        public void redrawTraitLevelUp(bool inPcCreation)
        {
    	    traitsToLearnTagsList.Clear();
    	    fillToLearnList();
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
            int textH = (int)gv.fontHeight;
    	    int spacing = textH;
            int tabX = 5 * gv.squareSize + pW * 3;
            int noticeX = 5 * gv.squareSize + pW * 3;
    	    int noticeY = pH * 1 + spacing;
    	    int tabStartY = 4 * gv.squareSize + pW * 10;

            if (!infoOnly)
            {
                //DRAW TEXT		
                locY = (gv.squareSize * 0) + (pH * 2);
                gv.DrawText("Select " + traitToLearnIndex + " of " + mod.getPlayerClass(pc.classTag).traitsToLearnAtLevelTable[pc.classLevel] + " Traits to Learn", noticeX, pH * 1, "gy");
                
                //DRAW NOTIFICATIONS
                if (isSelectedTraitSlotInKnownTraitsRange())
                {
                    Trait tr = GetCurrentlySelectedTrait();
                    
                    //check to see if already known
                    if ((pc.knownTraitsTags.Contains(tr.tag)) || (pc.learningTraitsTags.Contains(tr.tag)))
                    {
                        //say that you already know this one
                        gv.DrawText("Already Known", noticeX, noticeY, "yl");
                    }
                    else //trait not known
                    {
                        //check if available to learn
                        if (isAvailableToLearn(tr.tag))
                        {
                            gv.DrawText("Available to Learn", noticeX, noticeY, "gn");
                        }
                        else //not available yet
                        {
                            gv.DrawText("Trait Not Available to Learn Yet", noticeX, noticeY, "rd");
                        }
                    }
                }
            }
            else
            {
                gv.DrawText("Traits Known or Available for this Class", noticeX, pH * 1, "gy");
            }
		
		    //DRAW ALL TRAIT SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnTraitSlots)
		    {			
			    if (cntSlot == traitSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			
			    //show only traits for the PC class
			    if (cntSlot < pc.playerClass.traitsAllowed.Count)
			    {
				    TraitAllowed ta = pc.playerClass.traitsAllowed[cntSlot];
				    Trait tr = mod.getTraitByTag(ta.tag);

                    if (infoOnly)
                    {
                        if (pc.knownTraitsTags.Contains(tr.tag)) //check to see if already known, if so turn on button
                        {
                            btn.Img = "btn_small";
                            btn.Img2 = tr.traitImage;
                        }
                        else //trait not known yet
                        {
                            btn.Img = "btn_small_off";
                            btn.Img2 = tr.traitImage + "_off";
                        }
                    }
                    else
                    {
                        if (pc.knownTraitsTags.Contains(tr.tag)) //check to see if already known, if so turn off button
                        {
                            btn.Img = "btn_small_off";
                            btn.Img2 = tr.traitImage + "_off";
                        }
                        else //trait not known yet
                        {
                            if (isAvailableToLearn(tr.tag)) //if available to learn, turn on button
                            {
                                btn.Img = "btn_small";
                                btn.Img2 = tr.traitImage;
                            }
                            else //not available to learn, turn off button
                            {
                                btn.Img = "btn_small_off";
                                btn.Img2 = tr.traitImage + "_off";
                            }
                        }
                    }				
			    }
			    else //slot is not in traits allowed index range
			    {
                    btn.Img = "btn_small_off"; 
				    btn.Img2 = null;
			    }			
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;		
		    if (isSelectedTraitSlotInKnownTraitsRange())
		    {
                Trait tr = GetCurrentlySelectedTrait();
                
                string textToSpan = "<gy>Description</gy>" + "<BR>";
                textToSpan += "<gn>" + tr.name + "</gn><BR>";
                textToSpan += "Available at Level: " + getLevelAvailable(tr.tag) + "<BR>";
                textToSpan += "<BR>";
                textToSpan += tr.description;

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
                btnSelect.Text = "LEARN SELECTED TRAIT";
                btnHelp.Draw();
                btnExit.Draw();
                btnSelect.Draw();
            }
            if (gv.showMessageBox)
            {
                gv.messageBox.onDrawLogBox();
            }
        }
        public void onTouchTraitLevelUp(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType, bool inPcCreation)
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
                x = (int)eX;
                y = (int)eY;
			
			    btnHelp.glowOn = false;
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
				    if (btnTraitSlots[j].getImpact(x, y))
				    {
                        gv.PlaySound("btn_click");
					    traitSlotIndex = j;
				    }
			    }
			    if (btnHelp.getImpact(x, y))
			    {
                    if (!infoOnly)
                    {
                        gv.PlaySound("btn_click");
                        tutorialMessageTraitScreen();
                    }
			    }
			    else if (btnSelect.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
                    if (infoOnly)
                    {
                        if (inCombat)
                        {
                            gv.screenType = "combatParty";
                        }
                        else
                        {
                            gv.screenType = "party";
                        }
                    }
                    else
                    {
                        doSelectedTraitToLearn(inPcCreation);
                    }
			    }
			    else if (btnExit.getImpact(x, y))
			    {
                    if (!infoOnly)
                    {
                        gv.screenParty.traitGained = "";
                        gv.screenParty.spellGained = "";
                        pc.learningTraitsTags.Clear();
                        pc.learningSpellsTags.Clear();
                        traitToLearnIndex = 1;

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
    
        public void doSelectedTraitToLearn(bool inPcCreation)
        {
    	    if (isSelectedTraitSlotInKnownTraitsRange())
		    {
			    Trait tr = GetCurrentlySelectedTrait();
			    if (isAvailableToLearn(tr.tag))
			    {
                    //add trait
                    //pc.knownTraitsTags.Add(tr.tag);
                    pc.learningTraitsTags.Add(tr.tag);
                    gv.screenParty.traitGained += tr.name + ", ";
                    //check to see if there are more traits to learn at this level
                    traitToLearnIndex++;
                    if (traitToLearnIndex <= mod.getPlayerClass(pc.classTag).traitsToLearnAtLevelTable[pc.classLevel])
                    {
                        //more to learn, keep going
                    }
                    else //finished learning all traits available for this level
                    {
                        //else if in creation go back to partybuild				
                        if (inPcCreation)
                        {
                            //if there are spells to learn go to spell screen next
                            List<string> spellTagsList = new List<string>();
                            spellTagsList = pc.getSpellsToLearn();
                            if (spellTagsList.Count > 0)
                            {
                                gv.screenSpellLevelUp.resetPC(false, false, pc);
                                gv.screenType = "learnSpellCreation";
                            }
                            else //no spells to learn
                            {
                                //save character, add them to the pcList of screenPartyBuild, and go back to build screen
                                foreach (string s in pc.learningTraitsTags)
                                {
                                    pc.knownTraitsTags.Add(s);
                                }
                                pc.learningTraitsTags.Clear();
                                gv.screenPcCreation.SaveCharacter(pc);
                                gv.screenPartyBuild.pcList.Add(pc);
                                gv.screenType = "partyBuild";
                            }
                        }
                        else
                        {
                            //if there are spells to learn go to spell screen next
                            List<string> spellTagsList = new List<string>();
                            spellTagsList = pc.getSpellsToLearn();
                            if (spellTagsList.Count > 0)
                            {
                                gv.screenSpellLevelUp.resetPC(false, false, pc);
                                gv.screenType = "learnSpellLevelUp";
                            }
                            else //no spells or traits to learn
                            {
                                foreach (string s in pc.learningTraitsTags)
                                {
                                    pc.knownTraitsTags.Add(s);
                                }
                                pc.learningTraitsTags.Clear();
                                gv.screenType = "party";
                                //gv.screenParty.traitGained += tr.name + ", ";
                                gv.screenParty.doLevelUpSummary();
                            }
                        }
                    }
			    }
			    else
			    {
				    gv.sf.MessageBox("Can't learn that trait, try another or exit");
			    }
		    }	
        }
            
        public bool isAvailableToLearn(string spellTag)
        {
    	    if (traitsToLearnTagsList.Contains(spellTag))
    	    {
    		    return true;
    	    }
    	    return false;
        }    
        public void fillToLearnList()
        {
    	    traitsToLearnTagsList = pc.getTraitsToLearn(gv.mod);	    
        }    
        public Trait GetCurrentlySelectedTrait()
	    {
    	    TraitAllowed ta = pc.playerClass.traitsAllowed[traitSlotIndex];
		    return mod.getTraitByTag(ta.tag);
	    }
	    public bool isSelectedTraitSlotInKnownTraitsRange()
	    {
		    return traitSlotIndex < pc.playerClass.traitsAllowed.Count;
	    }	
	    public int getLevelAvailable(string tag)
	    {
		    TraitAllowed ta = pc.playerClass.getTraitAllowedByTag(tag);
		    if (ta != null)
		    {
			    return ta.atWhatLevelIsAvailable;
		    }
		    return 0;
	    }
	    public void tutorialMessageTraitScreen()
        {
		    gv.sf.MessageBoxHtml(this.stringMessageTraitLevelUp);	
        }
    }
}
