using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IceBlink2mini
{
    public class ScreenTraitUseSelector
    {
        //private Module gv.mod;
        private GameView gv;

        public int traitUsingPlayerIndex = 0;
        private int traitSlotIndex = 0;
        private int slotsPerPage = 24;
        private List<IbbButton> btnTraitSlots = new List<IbbButton>();
        private IbbButton btnHelp = null;
        private IbbButton btnSelect = null;
        private IbbButton btnExit = null;
        private string stringMessageTraitUseSelector = "";
        private IbbHtmlTextBox description;
        public bool isInCombat = false;

        public ScreenTraitUseSelector(Module m, GameView g)
        {
            //gv.mod = m;
            gv = g;
            stringMessageTraitUseSelector = gv.cc.loadTextToString("MessageTraitUseSelector.txt");
        }

        public void setControlsStart()
        {
            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padW = gv.squareSize / 6;

            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;

            if (btnSelect == null)
            {
                btnSelect = new IbbButton(gv, 0.8f);
                btnSelect.Text = "USE SELECTED TRAIT";
                btnSelect.Img = "btn_large";
                btnSelect.Glow = "btn_large_glow";
                btnSelect.X = 2 * gv.squareSize + padW * 2;
                btnSelect.Y = 6 * gv.squareSize - pH * 2;
                btnSelect.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSelect.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnHelp == null)
            {
                btnHelp = new IbbButton(gv, 0.8f);
                btnHelp.Text = "HELP";
                btnHelp.Img = "btn_small";
                btnHelp.Glow = "btn_small_glow";
                btnHelp.X = 1 * gv.squareSize + padW * 1;
                btnHelp.Y = 6 * gv.squareSize - pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnExit == null)
            {
                btnExit = new IbbButton(gv, 0.8f);
                btnExit.Text = "EXIT";
                btnExit.Img = "btn_small";
                btnExit.Glow = "btn_small_glow";
                btnExit.X = (5 * gv.squareSize) + padW * 5;
                btnExit.Y = 6 * gv.squareSize - pH * 2;
                btnExit.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnExit.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            for (int y = 0; y < slotsPerPage; y++)
            {
                IbbButton btnNew = new IbbButton(gv, 1.0f);
                btnNew.Img = "btn_small";
                btnNew.ImgOff = "btn_small_off";
                btnNew.Glow = "btn_small_glow";

                int x = y % 6;
                int yy = y / 6;
                btnNew.X = ((x) * gv.squareSize) + (padW * (x + 1));
                btnNew.Y = (1 + yy) * gv.squareSize + (padW * yy);

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);

                btnTraitSlots.Add(btnNew);
            }
            //DRAW ALL TRAIT SLOTS		
            int cntSlot = 0;
            foreach (IbbButton btn in btnTraitSlots)
            {
                Player pc = getTraitUsingPlayer();

                if (cntSlot == traitSlotIndex) { btn.glowOn = true; }
                else { btn.glowOn = false; }

                //show only traits for the PC class
                if (cntSlot < pc.playerClass.traitsAllowed.Count)
                {
                    TraitAllowed ta = pc.playerClass.traitsAllowed[cntSlot];
                    Trait tr = gv.mod.getTraitByTag(ta.tag);

                    btn.Img2 = tr.traitImage;
                    btn.Img2Off = tr.traitImage + "_off";

                    if (pc.knownTraitsTags.Contains(tr.tag))
                    {
                        if (isInCombat) //all spells can be used in combat
                        {
                            if (tr.isPassive)
                            {
                                btn.btnState = buttonState.Off;
                            }
                            else
                            {
                                btn.btnState = buttonState.Normal;
                            }
                        }
                        else if (tr.isPassive)
                        {
                            btn.btnState = buttonState.Off;
                        }
                        //not in combat so check if spell can be used on adventure maps
                        else if ((tr.useableInSituation.Equals("Always")) || (tr.useableInSituation.Equals("OutOfCombat")) || (!tr.isPassive))
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

        //TRAIT USE SELECTOR SCREEN (COMBAT and MAIN)
        public void redrawTraitUseSelector(bool inCombat)
        {
            Player pc = getTraitUsingPlayer();
            
            isInCombat = inCombat;
            //IF CONTROLS ARE NULL, CREATE THEM
            if (btnSelect == null)
            {
                setControlsStart();
            }
            //btnSelect.Text = "USE SELECTED " + gv.mod.getPlayerClass(pc.classTag).spellLabelSingular.ToUpper();

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
            gv.DrawText("Select a Trait to Use", noticeX, pH * 3, "wh");
            //gv.mSheetTextPaint.setColor(Color.YELLOW);
            gv.DrawText(pc.name + " SP: " + pc.sp + "/" + pc.spMax, pW * 55, leftStartY, "yl");

            //DRAW NOTIFICATIONS
            if (isSelectedTraitSlotInKnownTraitsRange())
            {
                Trait tr = GetCurrentlySelectedTrait();

                if (pc.knownTraitsTags.Contains(tr.tag))
                {
                    if (inCombat) //all spells can be used in combat
                    {
                        if (!tr.isPassive)
                        {
                            //if currently selected is usable say "Available to Cast" in lime
                            if (pc.sp >= GetCurrentlySelectedTrait().costSP)
                            {
                                gv.DrawText("Available to Use", noticeX, noticeY, "gn");
                            }
                            else //if known but not enough spell points, "Insufficient SP to Cast" in yellow
                            {
                                gv.DrawText("Insufficient SP", noticeX, noticeY, "yl");
                            }
                        }
                        else //is passive
                        {
                            gv.DrawText("Passive Trait, Always On", noticeX, noticeY, "yl");
                        }
                    }
                    //not in combat so check if spell can be used on adventure maps
                    else if ((tr.useableInSituation.Equals("Always")) || (tr.useableInSituation.Equals("OutOfCombat")))
                    {
                        if (!tr.isPassive)
                        {
                            //if currently selected is usable say "Available to Cast" in lime
                            if (pc.sp >= GetCurrentlySelectedTrait().costSP)
                            {
                                gv.DrawText("Available to Use", noticeX, noticeY, "gn");
                            }
                            else //if known but not enough spell points, "Insufficient SP to Cast" in yellow
                            {
                                gv.DrawText("Insufficient SP", noticeX, noticeY, "yl");
                            }
                        }
                        else //is passive
                        {
                            gv.DrawText("Passive Trait, Always On", noticeX, noticeY, "yl");
                        }
                    }
                    else //can't be used on adventure map
                    {
                        gv.DrawText("Not Available Here", noticeX, noticeY, "yl");
                    }
                }
                else //spell not known
                {
                    gv.DrawText("Trait Not Known Yet", noticeX, noticeY, "rd");
                }
            }

            //DRAW ALL SPELL SLOTS		
            int cntSlot = 0;
            foreach (IbbButton btn in btnTraitSlots)
            {
                //Player pc = getCastingPlayer();						

                if (cntSlot == traitSlotIndex) { btn.glowOn = true; }
                else { btn.glowOn = false; }


                btn.Draw();
                cntSlot++;
            }

            //DRAW DESCRIPTION BOX
            locY = tabStartY;
            if (isSelectedTraitSlotInKnownTraitsRange())
            {
                Trait tr = GetCurrentlySelectedTrait();
                string textToSpan = "<gy>Description</gy><BR>";
                textToSpan += "<gn>" + tr.name + "</gn><BR>";
                textToSpan += "<yl>SP Cost: " + tr.costSP + "</yl><BR>";
                textToSpan += "Target Range: " + tr.range + "<BR>";
                textToSpan += "Area of Effect Radius: " + tr.aoeRadius + "<BR>";
                textToSpan += "Available at Level: " + getLevelAvailable(tr.tag) + "<BR>";
                textToSpan += "<BR>";
                textToSpan += tr.description;

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
        public void onTouchTraitUseSelector(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType, bool inCombat)
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
                    int x = (int)eX;
                    int y = (int)eY;

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
                        if (btnTraitSlots[j].getImpact(x, y))
                        {
                            traitSlotIndex = j;
                        }
                    }
                    if (btnHelp.getImpact(x, y))
                    {
                        gv.showMessageBox = true;
                        tutorialMessageTraitUsingScreen();
                    }
                    else if (btnSelect.getImpact(x, y))
                    {
                        doSelectedTrait(inCombat);
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
            btnTraitSlots.Clear();
            btnHelp = null;
            btnSelect = null;
            btnExit = null;
        }

        public void doSelectedTrait(bool inCombat)
        {
            if (isSelectedTraitSlotInKnownTraitsRange())
            {
                //only allow to cast spells that you know and are usable on this map
                if (getTraitUsingPlayer().knownTraitsTags.Contains(GetCurrentlySelectedTrait().tag))
                {
                    if (inCombat) //Combat Map
                    {
                        if (!GetCurrentlySelectedTrait().isPassive)
                        {
                            if (getTraitUsingPlayer().sp >= GetCurrentlySelectedTrait().costSP)
                            {
                                gv.cc.currentSelectedTrait = GetCurrentlySelectedTrait();
                                //if target is SELF then just use trait on self now
                                if (gv.cc.currentSelectedTrait.traitTargetType.Equals("Self"))
                                {
                                    gv.screenType = "combat";
                                    gv.screenCombat.currentCombatMode = "usetrait";
                                    doCleanUp();
                                    gv.screenCombat.TargetUseTraitPressed(getTraitUsingPlayer());
                                }
                                else
                                {
                                    gv.screenType = "combat";
                                    gv.screenCombat.currentCombatMode = "usetrait";
                                    doCleanUp();
                                }
                            }
                            else
                            {
                                //Toast.makeText(gv.gameContext, "Not Enough SP for that spell", Toast.LENGTH_SHORT).show();
                            }
                        }
                    }
                    else //Adventure Map
                    {
                        if (GetCurrentlySelectedTrait().isPassive)
                        {
                            //do nothing, it is passive
                        }
                        //only cast if useable on adventure maps
                        else if ((GetCurrentlySelectedTrait().useableInSituation.Equals("Always")) || (GetCurrentlySelectedTrait().useableInSituation.Equals("OutOfCombat")))
                        {
                            if (getTraitUsingPlayer().sp >= GetCurrentlySelectedTrait().costSP)
                            {
                                gv.cc.currentSelectedTrait = GetCurrentlySelectedTrait();
                                //if target is SELF then just do doTraitTarget(self)
                                if (gv.cc.currentSelectedTrait.traitTargetType.Equals("Self"))
                                {
                                    doTraitTarget(getTraitUsingPlayer(), getTraitUsingPlayer());
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
                                            gv.cc.doTraitBasedOnScriptOrEffectTag(gv.cc.currentSelectedTrait, target, target, true);
                                            gv.screenType = "main";
                                            doCleanUp();
                                            return;
                                        }
                                        catch (Exception ex)
                                        {
                                            gv.errorLog(ex.ToString());
                                        }
                                    }

                                    gv.itemListSelector.setupIBminiItemListSelector(gv, pcNames, gv.mod.getPlayerClass(getTraitUsingPlayer().classTag).spellLabelSingular + " Target", "traituseselectortraittarget");
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
        public void doTraitTarget(int selectedIndex)
        {
            Player pc = getTraitUsingPlayer();
            if (selectedIndex > 0)
            {
                Player target = gv.mod.playerList[selectedIndex - 1];
                doTraitTarget(pc, target);                
            }
            else if (selectedIndex == 0) // selected "cancel"
            {
                //do nothing
            }
        }
        public void doTraitTarget(Player pc, Player target)
        {
            try
            {
                gv.cc.doTraitBasedOnScriptOrEffectTag(gv.cc.currentSelectedTrait, pc, target, !isInCombat);
                gv.screenType = "main";
                doCleanUp();
            }
            catch (Exception ex)
            {
                gv.sf.MessageBoxHtml("error with Pc Selector screen: " + ex.ToString());
                gv.errorLog(ex.ToString());
            }
        }

        public Trait GetCurrentlySelectedTrait()
        {
            TraitAllowed ta = getTraitUsingPlayer().playerClass.traitsAllowed[traitSlotIndex];
            return gv.mod.getTraitByTag(ta.tag);
        }
        public bool isSelectedTraitSlotInKnownTraitsRange()
        {
            return traitSlotIndex < getTraitUsingPlayer().playerClass.traitsAllowed.Count;
        }
        public int getLevelAvailable(string tag)
        {
            TraitAllowed ta = getTraitUsingPlayer().playerClass.getTraitAllowedByTag(tag);
            if (ta != null)
            {
                return ta.atWhatLevelIsAvailable;
            }
            return 0;
        }
        public Player getTraitUsingPlayer()
        {
            return gv.mod.playerList[traitUsingPlayerIndex];
        }
        public void tutorialMessageTraitUsingScreen()
        {
            //gv.sf.MessageBoxHtml(this.stringMessageCastSelector);
            gv.messageBox.logLinesList.Clear();
            gv.messageBox.AddHtmlTextToLog(this.stringMessageTraitUseSelector);
            gv.messageBox.currentTopLineIndex = 0;
        }
    }
}
