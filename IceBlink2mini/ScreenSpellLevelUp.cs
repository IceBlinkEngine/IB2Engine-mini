using System;
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
	    //private Module gv.mod;
	    private GameView gv;
	
	    public int castingPlayerIndex = 0;
        public int spellToLearnIndex = 1;
	    private int spellSlotIndex = 0;
	    private int slotsPerPage = 24;
	    private List<IbbButton> btnSpellSlots = new List<IbbButton>();
	    private IbbButton btnHelp = null;
	    private IbbButton btnSelect = null;
	    private IbbButton btnExit = null;
	    List<string> spellsToLearnTagsList = new List<string>();
	    private Player pc;
        public bool infoOnly = false; //set to true when called for info only
        public bool inCombat = false;
        private string stringMessageSpellLevelUp = "";
        private IbbHtmlTextBox description;
	
	
	    public ScreenSpellLevelUp(Module m, GameView g) 
	    {
		    //gv.mod = m;
		    gv = g;
		    setControlsStart();
		    pc = new Player();
		    stringMessageSpellLevelUp = gv.cc.loadTextToString("MessageSpellLevelUp.txt");
	    }
	
	    public void resetPC(bool info_only, bool in_Combat, Player p)
	    {
		    pc = p;
            infoOnly = info_only;
            inCombat = in_Combat;
            spellToLearnIndex = 1;
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
            }
			    btnSelect.Text = "LEARN SELECTED SPELL";
			    btnSelect.Img ="btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
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
			    btnExit.Img ="btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnExit.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnExit.X = (5 * gv.squareSize) + padW * 5;
                btnExit.Y = 6 * gv.squareSize - pH * 2;
                btnExit.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnExit.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            for (int y = 0; y < slotsPerPage; y++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnNew.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    int x = y % 6;
			    int yy = y / 6;
			    btnNew.X = ((x) * gv.squareSize) + (padW * (x+1));
			    btnNew.Y = (1 + yy) * gv.squareSize + (padW * yy);

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnSpellSlots.Add(btnNew);
		    }			
	    }
	
	    //CAST SELECTOR SCREEN (COMBAT and MAIN)
        public void redrawSpellLevelUp(bool inPcCreation)
        {
            Player pc = getCastingPlayer();

            btnSelect.Text = "LEARN SELECTED " + gv.mod.getPlayerClass(getCastingPlayer().classTag).spellLabelPlural;

            spellsToLearnTagsList.Clear();
    	    fillToLearnList();
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
            int textH = (int)gv.fontHeight;
    	    int spacing = textH;
            int tabX = pW * 4;
            int noticeX = pW * 5;
            int noticeY = pH * 3 + spacing;
    	    int tabStartY = 4 * gv.squareSize + pW * 10;

            if (!infoOnly)
            {
                //DRAW TEXT		
                locY = (gv.squareSize * 0) + (pH * 2);
                gv.DrawText("Select " + spellToLearnIndex + " of " + gv.mod.getPlayerClass(pc.classTag).spellsToLearnAtLevelTable[getCastingPlayer().classLevel] + " " + gv.mod.getPlayerClass(pc.classTag).spellLabelPlural + " to Learn", noticeX, pH * 1, "gy");
                gv.DrawText(getCastingPlayer().name + " SP: " + getCastingPlayer().sp + "/" + getCastingPlayer().spMax, pW * 50, pH * 1, "yl");

                //DRAW NOTIFICATIONS
                if (isSelectedSpellSlotInKnownSpellsRange())
                {
                    Spell sp = GetCurrentlySelectedSpell();                    

                    //check to see if already known
                    if ((pc.knownSpellsTags.Contains(sp.tag)) || (pc.learningSpellsTags.Contains(sp.tag)))
                    {
                        //say that you already know this one
                        gv.DrawText("Already Known", noticeX, noticeY, "yl");
                    }
                    else //spell not known
                    {
                        //check if available to learn
                        if (isAvailableToLearn(sp.tag))
                        {
                            gv.DrawText("Available to Learn", noticeX, noticeY, "gn");
                        }
                        else //not available yet
                        {
                            gv.DrawText(gv.mod.getPlayerClass(pc.classTag).spellLabelSingular + " Not Available to Learn Yet", noticeX, noticeY, "rd");
                        }
                    }
                }
            }
            else
            {
                gv.DrawText(gv.mod.getPlayerClass(pc.classTag).spellLabelPlural + " Known or Available for this Class", noticeX, pH * 1, "gy");
            }

            //DRAW ALL SPELL SLOTS		
            int cntSlot = 0;
		    foreach (IbbButton btn in btnSpellSlots)
		    {	
			    if (cntSlot == spellSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			
			    //show only spells for the PC class
			    if (cntSlot < pc.playerClass.spellsAllowed.Count)
			    {
				    SpellAllowed sa = pc.playerClass.spellsAllowed[cntSlot];
				    Spell sp = gv.mod.getSpellByTag(sa.tag);

                    if (infoOnly)
                    {
                        if (pc.knownSpellsTags.Contains(sp.tag)) //check to see if already known, if so turn on button
                        {
                            //gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = "btn_small";
                            //gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = sp.spellImage;
                        }
                        else //spell not known yet
                        {
                            //gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = "btn_small_off";
                            //gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = sp.spellImage + "_off";                            
                        }
                    }
                    else
                    {
                        if (pc.knownSpellsTags.Contains(sp.tag)) //check to see if already known, if so turn off button
                        {
                            //gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = "btn_small_off";
                            //gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = sp.spellImage + "_off";
                        }
                        else //spell not known yet
                        {
                            if (isAvailableToLearn(sp.tag)) //if available to learn, turn on button
                            {
                                //gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = "btn_small"; 
                                //gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = sp.spellImage;
                            }
                            else //not available to learn, turn off button
                            {
                                //gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = "btn_small_off"; 
                                //gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = sp.spellImage + "_off";
                            }
                        }
                    }				
			    }
			    else //slot is not in spells allowed index range
			    {
                    //gv.cc.DisposeOfBitmap(ref btn.Img);
                    btn.Img = "btn_small_off"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
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
                
                string textToSpan = "<gy>Description</gy>" + "<BR>";
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

            if (infoOnly)
            {
                btnSelect.Text = "RETURN";
                btnSelect.Draw();
            }
            else
            {
                btnSelect.Text = "LEARN SELECTED " + gv.mod.getPlayerClass(pc.classTag).spellLabelSingular.ToUpper();
                btnHelp.Draw();
                btnExit.Draw();
                btnSelect.Draw();
            }
            if (gv.showMessageBox)
            {
                gv.messageBox.onDrawLogBox();
            }
        }
        public void onTouchSpellLevelUp(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType, bool inPcCreation)
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
                        doSelectedSpellToLearn(inPcCreation);
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
                        spellToLearnIndex = 1;

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
				    //pc.knownSpellsTags.Add(sp.tag);
                    pc.learningSpellsTags.Add(sp.tag);
                    gv.screenParty.spellGained += sp.name + ", ";
                    //check to see if there are more spells to learn at this level
                    spellToLearnIndex++;
                    if (spellToLearnIndex <= gv.mod.getPlayerClass(pc.classTag).spellsToLearnAtLevelTable[getCastingPlayer().classLevel])
                    {
                        //more to learn, keep going
                    }
                    else //finished learning all spells available for this level
                    {
                        if (inPcCreation)
                        {
                            foreach (string s in pc.learningTraitsTags)
                            {
                                pc.knownTraitsTags.Add(s);
                            }
                            pc.learningTraitsTags.Clear();

                            foreach (string s in pc.learningSpellsTags)
                            {
                                pc.knownSpellsTags.Add(s);
                            }
                            pc.learningTraitsTags.Clear();
                            pc.learningSpellsTags.Clear();

                            gv.screenPcCreation.SaveCharacter(pc);
                            gv.screenPartyBuild.pcList.Add(pc);
                            gv.screenType = "partyBuild";
                        }
                        else
                        {
                            foreach (string s in pc.learningTraitsTags)
                            {
                                pc.knownTraitsTags.Add(s);
                            }
                            pc.learningTraitsTags.Clear();

                            foreach (string s in pc.learningSpellsTags)
                            {
                                pc.knownSpellsTags.Add(s);
                            }
                            pc.learningTraitsTags.Clear();
                            pc.learningSpellsTags.Clear();

                            gv.screenType = "party";
                            //gv.screenParty.spellGained += sp.name + ", ";
                            gv.screenParty.doLevelUpSummary();
                        }
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
		    return gv.mod.getSpellByTag(sa.tag);
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
