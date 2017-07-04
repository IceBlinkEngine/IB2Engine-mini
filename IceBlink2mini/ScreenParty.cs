using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class ScreenParty
    {
        //public Module gv.mod;
        public GameView gv;
        private IbbHtmlTextBox description;
        private IbbHtmlTextBox attackAndDamageInfo;

        public List<IbbButton> btnPartyIndex = new List<IbbButton>();
        private IbbPortrait btnPortrait = null;
        private IbbButton btnToken = null;
        private IbbButton btnHead = null;
        private IbbButton btnNeck = null;
        private IbbButton btnBody = null;
        private IbbButton btnMainHand = null;
        private IbbButton btnOffHand = null;
        private IbbButton btnRing = null;
        private IbbButton btnRing2 = null;
        private IbbButton btnFeet = null;
        private IbbButton btnAmmo = null;
        private IbbButton btnHelp = null;
        private IbbButton btnInfo = null;
        private IbbButton btnReturn = null;
        private IbbButton btnLevelUp = null;
        private IbbButton btnPartyRoster = null;
        private IbbButton btnSpells = null;
        private IbbButton btnTraits = null;
        private IbbButton btnEffects = null;
        //private IbbButton btnOthers = null;
        //private bool dialogOpen = false;
        public string traitGained = "";
        public string spellGained = "";


        public ScreenParty(Module m, GameView g)
        {
            //gv.mod = m;
            gv = g;
            setControlsStart();
            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;
            attackAndDamageInfo = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            attackAndDamageInfo.showBoxBorder = false;
        }

        public void setControlsStart()
        {
            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padW = gv.squareSize / 6;

            if (btnPortrait == null)
            {
                btnPortrait = new IbbPortrait(gv, 1.0f);
            }
                btnPortrait.ImgBG = "item_slot";
                btnPortrait.Glow = "btn_small_glow";
                btnPortrait.X = 0 * gv.squareSize - (pW * 0);
                btnPortrait.Y = 1 * gv.squareSize + pH * 2;
                btnPortrait.Height = (int)(gv.ibpheight * gv.screenDensity);
                btnPortrait.Width = (int)(gv.ibpwidth * gv.screenDensity);

            if (btnToken == null)
            {
                btnToken = new IbbButton(gv, 1.0f);
            }
                btnToken.Img = "item_slot";
                //btnToken.Img2 = gv.cc.LoadBitmap(pc.tokenFilename);
                btnToken.Glow = "btn_small_glow";
                btnToken.X = 0 * gv.squareSize - (pW * 0);
                btnToken.Y = 3 * gv.squareSize + pH * 2;
                btnToken.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnToken.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnSpells == null)
            {
                btnSpells = new IbbButton(gv, 0.6f);
            }
                //btnSpells.Text = gv.mod.spellLabelPlural.ToUpper();
                btnSpells.Text = "";
                btnSpells.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnSpells.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnSpells.Img2 = "btnspell";
                btnSpells.X = 6 * gv.squareSize + padW * 2 + (int)(gv.squareSize * 0.75f);
                //btnSpells.Y = 10 * gv.squareSize + pH * 2; See OnDraw for Y
                btnSpells.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSpells.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnTraits == null)
            {
                btnTraits = new IbbButton(gv, 0.6f);
            }
                btnTraits.Text = "";
                btnTraits.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnTraits.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnTraits.Img2 = "btntrait";
                btnTraits.X = 7 * gv.squareSize + padW * 3 + (int)(gv.squareSize * 0.75f);
                //btnSpells.Y = 10 * gv.squareSize + pH * 2; See OnDraw for Y
                btnTraits.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTraits.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnEffects == null)
            {
                btnEffects = new IbbButton(gv, 0.6f);
            }
                btnEffects.Text = "";
                btnEffects.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnEffects.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnEffects.Img2 = "btneffect";
                btnEffects.X = 8 * gv.squareSize + padW * 4 + (int)(gv.squareSize * 0.75f);
                //btnSpells.Y = 10 * gv.squareSize + pH * 2; See OnDraw for Y
                btnEffects.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnEffects.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnPartyRoster == null)
            {
                btnPartyRoster = new IbbButton(gv, 0.6f);
            }
                btnPartyRoster.Text = "ROSTER";
                btnPartyRoster.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnPartyRoster.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnPartyRoster.X = (gv.squaresInWidth * gv.squareSize / 2) + (int)((gv.ibbwidthL / 2) * gv.screenDensity) + (int)(gv.squareSize * 0.5);
                btnPartyRoster.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPartyRoster.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            
            if (btnHelp == null)
            {
                btnHelp = new IbbButton(gv, 0.8f);
            }
                btnHelp.Text = "HELP";
                btnHelp.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnHelp.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnHelp.X = (gv.squaresInWidth * gv.squareSize / 2) - (int)((gv.ibbwidthL / 2) * gv.screenDensity) - (int)(gv.squareSize * 1.5);
                btnHelp.Y = 6 * gv.squareSize - pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnInfo == null)
            {
                btnInfo = new IbbButton(gv, 0.8f);
            }
                btnInfo.Text = "INFO";
                btnInfo.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnInfo.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnInfo.X = 5 * gv.squareSize + (padW * 6);
                //btnInfo.Y = 9 * gv.squareSize + pH * 2;
                btnInfo.Y = 6 * gv.squareSize - (pH * 2);
                btnInfo.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInfo.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnReturn == null)
            {
                btnReturn = new IbbButton(gv, 1.2f);
            }
                btnReturn.Text = "RETURN";
                btnReturn.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnReturn.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnReturn.X = (gv.squaresInWidth * gv.squareSize / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnReturn.Y = 6 * gv.squareSize - pH * 2;
                btnReturn.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturn.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            if (btnLevelUp == null)
            {
                btnLevelUp = new IbbButton(gv, 1.2f);
            }
                btnLevelUp.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnLevelUp.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnLevelUp.Text = "Level Up";
                //btnLevelUp.X = 5 * gv.squareSize + padW * 1 + gv.oXshift;
                //btnLevelUp.Y = 8 * gv.squareSize - pH * 2;
                btnLevelUp.X = 6 * gv.squareSize + (padW * (7));
                btnLevelUp.Y = pH * 2; ;
                btnLevelUp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnLevelUp.Width = (int)(gv.ibbwidthL * gv.screenDensity);


            if (btnMainHand == null)
            {
                btnMainHand = new IbbButton(gv, 1.0f);
            }
                btnMainHand.Img = "item_slot_mainhand"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_mainhand);
                btnMainHand.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnMainHand.X = 1 * gv.squareSize + (padW * 2);
                btnMainHand.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnMainHand.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnMainHand.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnHead == null)
            {
                btnHead = new IbbButton(gv, 1.0f);
            }
                btnHead.Img = "item_slot_head"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_head);
                btnHead.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnHead.X = 2 * gv.squareSize + (padW * 3);
                btnHead.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnHead.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHead.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnNeck == null)
            {
                btnNeck = new IbbButton(gv, 1.0f);
            }
                btnNeck.Img = "item_slot_neck"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_neck);
                btnNeck.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnNeck.X = 3 * gv.squareSize + (padW * 4);
                btnNeck.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnNeck.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNeck.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnOffHand == null)
            {
                btnOffHand = new IbbButton(gv, 1.0f);
            }
                btnOffHand.Img = "item_slot_offhand"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_offhand);
                btnOffHand.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnOffHand.X = 4 * gv.squareSize + (padW * 5);
                btnOffHand.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnOffHand.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnOffHand.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnRing == null)
            {
                btnRing = new IbbButton(gv, 1.0f);
            }
                btnRing.Img = "item_slot_ring"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_ring);
                btnRing.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnRing.X = 1 * gv.squareSize + (padW * 2);
                btnRing.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnRing.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRing.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnBody == null)
            {
                btnBody = new IbbButton(gv, 1.0f);
            }
                btnBody.Img = "item_slot_body"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_body);
                btnBody.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnBody.X = 2 * gv.squareSize + (padW * 3);
                btnBody.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnBody.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnBody.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnFeet == null)
            {
                btnFeet = new IbbButton(gv, 1.0f);
            }
                btnFeet.Img = "item_slot_feet"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_feet);
                btnFeet.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnFeet.X = 3 * gv.squareSize + (padW * 4);
                btnFeet.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnFeet.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnFeet.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnRing2 == null)
            {
                btnRing2 = new IbbButton(gv, 1.0f);
            }
                btnRing2.Img = "item_slot_ring"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_ring);
                btnRing2.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnRing2.X = 4 * gv.squareSize + (padW * 5);
                btnRing2.Y = 2 * gv.squareSize + padW; //not used, see onDraw function
                btnRing2.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRing2.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnAmmo == null)
            {
                btnAmmo = new IbbButton(gv, 1.0f);
            }
                btnAmmo.Img = "item_slot_ammo"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_ring);
                btnAmmo.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnAmmo.X = 5 * gv.squareSize + (padW * 6);
                btnAmmo.Y = 2 * gv.squareSize + padW; //not used, see onDraw function
                btnAmmo.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAmmo.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            
            for (int x = 0; x < 6; x++)
            {
                IbbButton btnNew = new IbbButton(gv, 1.0f);
                btnNew.Img = "item_slot"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
                btnNew.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnNew.X = ((x) * gv.squareSize) + (padW * (x + 1));
                btnNew.Y = pH * 2;
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);

                btnPartyIndex.Add(btnNew);
            }
        }

        public void resetPartyScreen()
        {
            int cntPCs = 0;
            foreach (IbbButton btn in btnPartyIndex)
            {
                if (cntPCs < gv.mod.playerList.Count)
                {
                    //gv.cc.DisposeOfBitmap(ref btn.Img2);
                    btn.Img2 = gv.mod.playerList[cntPCs].tokenFilename;
                }
                cntPCs++;
            }
            resetTokenAndPortrait();
        }
        public void resetTokenAndPortrait()
        {
            btnToken.Img2 = gv.mod.playerList[gv.cc.partyScreenPcIndex].tokenFilename;
            btnPortrait.Img = gv.mod.playerList[gv.cc.partyScreenPcIndex].portraitFilename;
        }

        public void redrawParty()
        {
            if (gv.cc.partyScreenPcIndex >= gv.mod.playerList.Count)
            {
                gv.cc.partyScreenPcIndex = 0;
            }
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
            gv.sf.UpdateStats(pc);
            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padH = gv.squareSize / 6;
            int locY = 0;
            int locX = 1 * gv.squareSize + (padH * 3);
            int tabX0 = 2 * gv.squareSize + (padH * 3);
            int tabX = 4 * gv.squareSize;
            int tabX2 = 8 * gv.squareSize + (padH * 5);
            //int textH = (int)gv.cc.MeasureString("GetHeight", gv.drawFontReg, gv.Width).Height;
            //int textH = gv.drawFontRegHeight;
            int spacing = gv.fontHeight + gv.fontLineSpacing;
            int leftStartY = btnPartyIndex[0].Y + btnPartyIndex[0].Height + (pH * 2);

            //DRAW EACH PC BUTTON
            int cntPCs = 0;
            foreach (IbbButton btn in btnPartyIndex)
            {
                if (cntPCs < gv.mod.playerList.Count)
                {
                    if (cntPCs == gv.cc.partyScreenPcIndex) { btn.glowOn = true; }
                    else { btn.glowOn = false; }
                    btn.Draw();
                }
                cntPCs++;
            }
            //DRAW TOKEN AND PORTRAIT
            btnPortrait.Draw();
            btnToken.Draw();

            //DRAW LEFT STATS
            //name            
            gv.DrawText(pc.name, locX, locY += leftStartY, "wh");

            //race
            gv.DrawText("Race: " + gv.cc.getRace(pc.raceTag).name, locX, locY += spacing, "wh");

            //gender
            if (pc.isMale)
            {
                gv.DrawText("Gender: Male", locX, locY += spacing, "wh");
            }
            else
            {
                gv.DrawText("Gender: Female", locX, locY += spacing, "wh");
            }

            //class
            gv.DrawText("Class: " + gv.cc.getPlayerClass(pc.classTag).name, locX, locY += spacing, "wh");
            gv.DrawText("Level: " + pc.classLevel, locX, locY += spacing, "wh");
            gv.DrawText("XP: " + pc.XP + "/" + pc.XPNeeded, locX, locY += spacing, "wh");
            int actext = 0;
            //float locY2 = 4 * gv.squareSize + gv.squareSize / 4;
            if (gv.mod.ArmorClassAscending) { actext = pc.AC; }
            else { actext = 20 - pc.AC; }
            gv.DrawText("AC: " + actext + " BAB: " + pc.baseAttBonus, locX, locY += spacing, "wh");
            gv.DrawText("HP: " + pc.hp + "/" + pc.hpMax, locX, locY += spacing, "wh");
            gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, locX, locY += spacing, "wh");            
            //gv.DrawText("---------------", locX, locY += spacing, 1.0f, Color.White);

            //LOCATE STATS INFO BUTTONS
            //locY += spacing;
                        
            int bottomLocY = 2 * gv.squareSize + gv.squareSize / 2;
            btnSpells.Y = bottomLocY;
            btnTraits.Y = bottomLocY;
            btnEffects.Y = bottomLocY;
            //btnOthers.Y = bottomLocY;
            btnPartyRoster.Y = 6 * gv.squareSize - pH * 2;

            //LOCATE EQUIPMENT SLOTS
            int startSlotsY = (int)locY + (gv.squareSize / 4) + padH;
            btnHead.Y = startSlotsY;
            btnNeck.Y = startSlotsY;
            btnMainHand.Y = startSlotsY;
            btnOffHand.Y = startSlotsY;
            btnAmmo.Y = startSlotsY;
            int startSlotsY2 = startSlotsY + gv.squareSize + padH;
            btnRing.Y = startSlotsY2;
            btnRing2.Y = startSlotsY2;
            btnBody.Y = startSlotsY2;
            btnFeet.Y = startSlotsY2;
            btnInfo.Y = startSlotsY2;

            //DRAW RIGHT STATS
            
            locY = 0;
            gv.DrawText("STR: " + pc.baseStr + " + " + (pc.strength - pc.baseStr) + " = " + pc.strength + " (" + ((pc.strength - 10) / 2) + ")", tabX, locY += leftStartY, "wh");
            gv.DrawText("DEX: " + pc.baseDex + " + " + (pc.dexterity - pc.baseDex) + " = " + pc.dexterity + " (" + ((pc.dexterity - 10) / 2) + ")", tabX, locY += spacing, "wh");
            gv.DrawText("CON: " + pc.baseCon + " + " + (pc.constitution - pc.baseCon) + " = " + pc.constitution + " (" + ((pc.constitution - 10) / 2) + ")", tabX, locY += spacing, "wh");
            gv.DrawText("INT: " + pc.baseInt + " + " + (pc.intelligence - pc.baseInt) + " = " + pc.intelligence + " (" + ((pc.intelligence - 10) / 2) + ")", tabX, locY += spacing, "wh");
            gv.DrawText("WIS: " + pc.baseWis + " + " + (pc.wisdom - pc.baseWis) + " = " + pc.wisdom + " (" + ((pc.wisdom - 10) / 2) + ")", tabX, locY += spacing, "wh");
            gv.DrawText("CHA: " + pc.baseCha + " + " + (pc.charisma - pc.baseCha) + " = " + pc.charisma + " (" + ((pc.charisma - 10) / 2) + ")", tabX, locY += spacing, "wh");
            gv.DrawText("FORT: " + pc.fortitude, tabX, locY += spacing, "wh");
            gv.DrawText("REF:  " + pc.reflex, tabX, locY += spacing, "wh");
            gv.DrawText("WILL: " + pc.will, tabX, locY += spacing, "wh");
            
            //DRAW LEVEL UP BUTTON
            if (gv.mod.playerList[gv.cc.partyScreenPcIndex].IsReadyToAdvanceLevel())
            {
                btnLevelUp.Draw();
            }
            
            if (gv.cc.partyItemSlotIndex == 0) { btnMainHand.glowOn = true; }
            else { btnMainHand.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 1) { btnHead.glowOn = true; }
            else { btnHead.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 2) { btnNeck.glowOn = true; }
            else { btnNeck.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 3) { btnOffHand.glowOn = true; }
            else { btnOffHand.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 4) { btnRing.glowOn = true; }
            else { btnRing.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 5) { btnBody.glowOn = true; }
            else { btnBody.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 6) { btnFeet.glowOn = true; }
            else { btnFeet.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 7) { btnRing2.glowOn = true; }
            else { btnRing2.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 8) { btnAmmo.glowOn = true; }
            else { btnAmmo.glowOn = false; }

            //gv.cc.DisposeOfBitmap(ref btnMainHand.Img2);
            btnMainHand.Img2 = gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref).itemImage;
            //gv.cc.DisposeOfBitmap(ref btnHead.Img2);
            btnHead.Img2 = gv.cc.getItemByResRefForInfo(pc.HeadRefs.resref).itemImage;
            //gv.cc.DisposeOfBitmap(ref btnNeck.Img2);
            btnNeck.Img2 = gv.cc.getItemByResRefForInfo(pc.NeckRefs.resref).itemImage;
            //gv.cc.DisposeOfBitmap(ref btnOffHand.Img2);
            btnOffHand.Img2 = gv.cc.getItemByResRefForInfo(pc.OffHandRefs.resref).itemImage;
            //gv.cc.DisposeOfBitmap(ref btnRing.Img2);
            btnRing.Img2 = gv.cc.getItemByResRefForInfo(pc.RingRefs.resref).itemImage;
            //gv.cc.DisposeOfBitmap(ref btnBody.Img2);
            btnBody.Img2 = gv.cc.getItemByResRefForInfo(pc.BodyRefs.resref).itemImage;
            //gv.cc.DisposeOfBitmap(ref btnFeet.Img2);
            btnFeet.Img2 = gv.cc.getItemByResRefForInfo(pc.FeetRefs.resref).itemImage;
            //gv.cc.DisposeOfBitmap(ref btnRing2.Img2);
            btnRing2.Img2 = gv.cc.getItemByResRefForInfo(pc.Ring2Refs.resref).itemImage;
            //gv.cc.DisposeOfBitmap(ref btnAmmo.Img2);
            btnAmmo.Img2 = gv.cc.getItemByResRefForInfo(pc.AmmoRefs.resref).itemImage;
            
            ItemRefs itr = gv.mod.getItemRefsInInventoryByResRef(pc.AmmoRefs.resref);
            if (itr != null)
            {
                btnAmmo.Quantity = itr.quantity + "";
            }
            else
            {
                btnAmmo.Quantity = "";
            }
            
            btnMainHand.Draw();
            btnHead.Draw();
            btnNeck.Draw();
            btnOffHand.Draw();
            btnRing.Draw();
            btnBody.Draw();
            btnFeet.Draw();
            btnRing2.Draw();
            btnAmmo.Draw();
            btnSpells.Draw();
            btnTraits.Draw();
            btnEffects.Draw();
            //btnOthers.Draw();
            if (gv.mod.hideRoster == false)
            {
                btnPartyRoster.Draw();
            }
            
            //DRAW DESCRIPTION BOX
            Item it = new Item();
            if (gv.cc.partyItemSlotIndex == 0) { it = gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 1) { it = gv.cc.getItemByResRefForInfo(pc.HeadRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 2) { it = gv.cc.getItemByResRefForInfo(pc.NeckRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 3) { it = gv.cc.getItemByResRefForInfo(pc.OffHandRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 4) { it = gv.cc.getItemByResRefForInfo(pc.RingRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 5) { it = gv.cc.getItemByResRefForInfo(pc.BodyRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 6) { it = gv.cc.getItemByResRefForInfo(pc.FeetRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 7) { it = gv.cc.getItemByResRefForInfo(pc.Ring2Refs.resref); }
            else if (gv.cc.partyItemSlotIndex == 8) { it = gv.cc.getItemByResRefForInfo(pc.AmmoRefs.resref); }

            //Description
            string textToSpan = "";
            textToSpan = "<gy>Description</gy>" + "<BR>";
            textToSpan += "<gn>" + it.name + "</gn><BR>";
            if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
            {
                textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<BR>";
                textToSpan += "Attack Bonus: " + it.attackBonus + "<BR>";
                textToSpan += "Attack Range: " + it.attackRange + "<BR>";
                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
                textToSpan += "Tap 'INFO' for Full Description<BR>";
            }
            else if (!it.category.Equals("General"))
            {
                textToSpan += "AC Bonus: " + it.armorBonus + "<BR>";
                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";
                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
                textToSpan += "Tap 'INFO' for Full Description<BR>";
            }
            locY = btnBody.Y + btnBody.Height + (pH * 2);

            int xLoc = (6 * gv.squareSize) + (pW * 5) + (int)(gv.squareSize * 0.75f);
            int yLoc = startSlotsY + (gv.squareSize / 4);
            int width = 4 * gv.squareSize;
            int height = 8 * gv.squareSize;
            DrawTextLayout(description, textToSpan, xLoc, yLoc, width, height);

            btnHelp.Draw();
            btnInfo.Draw();
            btnReturn.Draw();

            //Current attack and damage box

            //1. get number of attacks with melee or ranged (numAtt)
            int numAtt = 1;
            numAtt = gv.sf.CalcNumberOfAttacks(pc);
            /*if ((gv.sf.hasTrait(pc, "twoAttack")) && (gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee")))
            {
                numAtt = 2;
            }
            if ((gv.sf.hasTrait(pc, "rapidshot")) && (gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged")))
            {
                numAtt = 2;
            }
            if ((gv.sf.hasTrait(pc, "rapidshot2")) && (gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged")))
            {
                numAtt = 3;
            }*/

            //2. calculate attack modifier with current weapon (attackMod)
            int attackMod = 0;
            int modifier = 0;
            if (gv.sf.isMeleeAttack(pc))
            {
                modifier = gv.sf.CalcPcMeleeAttackAttributeModifier(pc);                
            }
            else //ranged weapon used
            {
                modifier = (pc.dexterity - 10) / 2;
                int preciseShotAdder = 0;
                preciseShotAdder = gv.sf.CalcPcRangedAttackModifier(pc);
                if (preciseShotAdder > 0)
                {
                    modifier += preciseShotAdder;
                }
                else
                {
                    if (gv.sf.hasTrait(pc, "preciseshot2"))
                    {
                        modifier += 2;
                    }
                    else if (gv.sf.hasTrait(pc, "preciseshot"))
                    {
                        modifier++;
                    }
                }
                Item it2 = gv.cc.getItemByResRefForInfo(pc.AmmoRefs.resref);
                if (it2 != null)
                {
                    modifier += gv.cc.getItemByResRefForInfo(pc.AmmoRefs.resref).attackBonus;
                }
            }

            attackMod = modifier + pc.baseAttBonus + gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref).attackBonus;

            //3. Calculate damage with current weapon (numberOfDiceRolled, typeOfDieRolled, damModifier)  
            int numberOfDiceRolled = 0;
            int typeOfDieRolled = 0;
            int damModifier = 0;
            string damageType = gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage;

            if (gv.sf.isMeleeAttack(pc))
            {
                damModifier = gv.sf.CalcPcMeleeDamageAttributeModifier(pc);                
            }
            else //ranged weapon used
            {
                damModifier = 0;
                int preciseShotAdder = 0;
                preciseShotAdder = gv.sf.CalcPcRangedDamageModifier(pc);
                if (preciseShotAdder > 0)
                {
                    damModifier += preciseShotAdder;
                }
                else
                {
                    if (gv.sf.hasTrait(pc, "preciseshot2"))
                    {
                        damModifier += 2;
                    }
                    else if (gv.sf.hasTrait(pc, "preciseshot"))
                    {
                        damModifier++;
                    }
                }                
                Item it3 = gv.cc.getItemByResRefForInfo(pc.AmmoRefs.resref);
                if (it3 != null)
                {
                    damModifier += gv.cc.getItemByResRefForInfo(pc.AmmoRefs.resref).damageAdder;
                    damageType = gv.cc.getItemByResRefForInfo(pc.AmmoRefs.resref).typeOfDamage;
                }
            }

            damModifier += gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref).damageAdder;
            numberOfDiceRolled = gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref).damageNumDice;
            typeOfDieRolled = gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref).damageDie;

            //4. Draw TextBox with info from abvoe about attack and damage
            //Description
            string textToSpan2 = "";
            textToSpan2 = "<u>Current Att & Damg</u>" + "<BR>";
            textToSpan2 += "Num of attacks: " + numAtt + "<BR>";
            textToSpan2 += "Att bonus: " + attackMod + "<BR>";
            textToSpan2 += "Damg: " + numberOfDiceRolled + "d" + typeOfDieRolled + "+" + damModifier + "<BR>";
            textToSpan2 += "Damg type: " + damageType + "<BR>";

            //locY = leftStartY;

            xLoc = (7 * gv.squareSize) + gv.squareSize / 2;
            yLoc = leftStartY;
            width = 7 * gv.squareSize;
            height = 6 * gv.squareSize;
            DrawTextLayout(attackAndDamageInfo, textToSpan2, xLoc, yLoc, width, height);
            if (gv.showMessageBox)
            {
                gv.messageBox.onDrawLogBox();
            }
        }
        public void DrawTextLayout(IbbHtmlTextBox tb, string text, int xLoc, int yLoc, int width, int height)
        {
            tb.tbXloc = xLoc;
            tb.tbYloc = yLoc;
            tb.tbWidth = width;
            tb.tbHeight = height;
            tb.logLinesList.Clear();
            tb.AddHtmlTextToLog(text);
            tb.onDrawLogBox();                       
        }
        public void onTouchParty(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType, bool inCombat)
        {
            btnLevelUp.glowOn = false;
            btnPartyRoster.glowOn = false;
            btnHelp.glowOn = false;
            btnInfo.glowOn = false;
            btnReturn.glowOn = false;
            btnSpells.glowOn = false;
            btnTraits.glowOn = false;
            btnEffects.glowOn = false;
            //btnOthers.glowOn = false;
            if (gv.showMessageBox)
            {
                gv.messageBox.btnReturn.glowOn = false;
            }

            //int eventAction = event.getAction();
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

                    if (btnLevelUp.getImpact(x, y))
                    {
                        btnLevelUp.glowOn = true;
                    }
                    else if (btnPartyRoster.getImpact(x, y))
                    {
                        btnPartyRoster.glowOn = true;
                    }
                    else if (btnHelp.getImpact(x, y))
                    {
                        btnHelp.glowOn = true;
                    }
                    else if (btnInfo.getImpact(x, y))
                    {
                        btnInfo.glowOn = true;
                    }
                    else if (btnReturn.getImpact(x, y))
                    {
                        btnReturn.glowOn = true;
                    }
                    else if (btnSpells.getImpact(x, y))
                    {
                        btnSpells.glowOn = true;
                    }
                    else if (btnTraits.getImpact(x, y))
                    {
                        btnTraits.glowOn = true;
                    }
                    else if (btnEffects.getImpact(x, y))
                    {
                        btnEffects.glowOn = true;
                    }
                    /*else if (btnOthers.getImpact(x, y))
                    {
                        btnOthers.glowOn = true;
                    }*/
                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)eX;
                    y = (int)eY;

                    btnLevelUp.glowOn = false;
                    btnPartyRoster.glowOn = false;
                    btnHelp.glowOn = false;
                    btnInfo.glowOn = false;
                    btnReturn.glowOn = false;
                    btnSpells.glowOn = false;
                    btnTraits.glowOn = false;
                    btnEffects.glowOn = false;
                    //btnOthers.glowOn = false;

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

                    Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];

                    if (btnPortrait.getImpact(x, y))
                    {
                        if (!inCombat)
                        {
                            //pass items to selector
                            gv.screenType = "portraitSelector";
                            gv.screenPortraitSelector.resetPortraitSelector("party", pc);
                        }
                    }
                    else if (btnToken.getImpact(x, y))
                    {
                        if (!inCombat)
                        {
                            gv.screenType = "tokenSelector";
                            gv.screenTokenSelector.resetTokenSelector("party", pc);
                        }
                    }
                    else if (btnSpells.getImpact(x, y))
                    {
                        gv.screenSpellLevelUp.resetPC(true, inCombat, pc);
                        gv.screenType = "learnSpellLevelUp";
                    }
                    else if (btnTraits.getImpact(x, y))
                    {
                        gv.screenTraitLevelUp.resetPC(true, inCombat, pc);
                        gv.screenType = "learnTraitLevelUp";
                    }
                    else if (btnEffects.getImpact(x, y))
                    {
                        string allEffects = "";
                        foreach (Effect ef in pc.effectsList)
                        {
                            int left = ef.durationInUnits;
                            allEffects += ef.name + " (" + left + ")" + "<br>";
                        }
                        gv.sf.MessageBoxHtml("<big><b>CURRENT EFFECTS</b></big><br><b><small>(#) denotes effect time left</small></b><br><br>" + allEffects);
                    }
                    /*else if (btnOthers.getImpact(x, y))
                    {
                        gv.sf.MessageBoxHtml("<big><b><u>SAVING THROW MODIFIERS</u></b></big><br>" +
                                "Fortitude: " + pc.fortitude + "<br>" +
                                "Will: " + pc.will + "<br>" +
                                "Reflex: " + pc.reflex + "<br><br>" +
                                "<big><b><u>RESISTANCES (%)</u></b></big><br>" +
                                "Acid: " + pc.damageTypeResistanceTotalAcid + "<br>" +
                                "Cold: " + pc.damageTypeResistanceTotalCold + "<br>" +
                                "Normal: " + pc.damageTypeResistanceTotalNormal + "<br>" +
                                "Electricity: " + pc.damageTypeResistanceTotalElectricity + "<br>" +
                                "Fire: " + pc.damageTypeResistanceTotalFire + "<br>" +
                                "Magic: " + pc.damageTypeResistanceTotalMagic + "<br>" +
                                "Poison: " + pc.damageTypeResistanceTotalPoison + "<br>"
                                );
                    }*/
                    else if (btnMainHand.getImpact(x, y))
                    {
                        if (gv.cc.partyItemSlotIndex == 0)
                        {
                            switchEquipment(inCombat);
                        }
                        gv.cc.partyItemSlotIndex = 0;
                    }
                    else if (btnHead.getImpact(x, y))
                    {
                        if (inCombat)
                        {
                            gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                            return;
                        }
                        if (gv.cc.partyItemSlotIndex == 1)
                        {
                            switchEquipment(inCombat);
                        }
                        gv.cc.partyItemSlotIndex = 1;
                    }
                    else if (btnNeck.getImpact(x, y))
                    {
                        if (inCombat)
                        {
                            gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                            return;
                        }
                        if (gv.cc.partyItemSlotIndex == 2)
                        {
                            switchEquipment(inCombat);
                        }
                        gv.cc.partyItemSlotIndex = 2;
                    }
                    else if (btnOffHand.getImpact(x, y))
                    {
                        if (gv.cc.partyItemSlotIndex == 3)
                        {
                            switchEquipment(inCombat);
                        }
                        gv.cc.partyItemSlotIndex = 3;
                    }
                    else if (btnRing.getImpact(x, y))
                    {
                        if (inCombat)
                        {
                            gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                            return;
                        }
                        if (gv.cc.partyItemSlotIndex == 4)
                        {
                            switchEquipment(inCombat);
                        }
                        gv.cc.partyItemSlotIndex = 4;
                    }
                    else if (btnBody.getImpact(x, y))
                    {
                        if (inCombat)
                        {
                            gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                            return;
                        }
                        if (gv.cc.partyItemSlotIndex == 5)
                        {
                            switchEquipment(inCombat);
                        }
                        gv.cc.partyItemSlotIndex = 5;
                    }
                    else if (btnFeet.getImpact(x, y))
                    {
                        if (inCombat)
                        {
                            gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                            return;
                        }
                        if (gv.cc.partyItemSlotIndex == 6)
                        {
                            switchEquipment(inCombat);
                        }
                        gv.cc.partyItemSlotIndex = 6;
                    }
                    else if (btnRing2.getImpact(x, y))
                    {
                        if (inCombat)
                        {
                            gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                            return;
                        }
                        if (gv.cc.partyItemSlotIndex == 7)
                        {
                            switchEquipment(inCombat);
                        }
                        gv.cc.partyItemSlotIndex = 7;
                    }
                    else if (btnAmmo.getImpact(x, y))
                    {
                        if (gv.cc.partyItemSlotIndex == 8)
                        {
                            switchEquipment(inCombat);
                        }
                        gv.cc.partyItemSlotIndex = 8;
                    }

                    else if (btnLevelUp.getImpact(x, y))
                    {
                        if (inCombat)
                        {
                            gv.sf.MessageBoxHtml("Can't Level up during combat.");
                            return;
                        }
                        if (gv.mod.playerList[gv.cc.partyScreenPcIndex].IsReadyToAdvanceLevel())
                        {
                            if (gv.mod.playerList[gv.cc.partyScreenPcIndex].isDead())
                            {
                                //Toast.makeText(gv.gameContext, "Can't Level Up a Dead Character", Toast.LENGTH_SHORT).show();
                            }
                            else
                            {
                                doLevelUpSetup();
                            }
                        }
                    }
                    else if (btnHelp.getImpact(x, y))
                    {
                        tutorialMessageParty(true);
                    }
                    else if (btnInfo.getImpact(x, y))
                    {
                        Item it = new Item();
                        if (gv.cc.partyItemSlotIndex == 0) { it = gv.cc.getItemByResRef(pc.MainHandRefs.resref); }
                        else if (gv.cc.partyItemSlotIndex == 1) { it = gv.cc.getItemByResRef(pc.HeadRefs.resref); }
                        else if (gv.cc.partyItemSlotIndex == 2) { it = gv.cc.getItemByResRef(pc.NeckRefs.resref); }
                        else if (gv.cc.partyItemSlotIndex == 3) { it = gv.cc.getItemByResRef(pc.OffHandRefs.resref); }
                        else if (gv.cc.partyItemSlotIndex == 4) { it = gv.cc.getItemByResRef(pc.RingRefs.resref); }
                        else if (gv.cc.partyItemSlotIndex == 5) { it = gv.cc.getItemByResRef(pc.BodyRefs.resref); }
                        else if (gv.cc.partyItemSlotIndex == 6) { it = gv.cc.getItemByResRef(pc.FeetRefs.resref); }
                        else if (gv.cc.partyItemSlotIndex == 7) { it = gv.cc.getItemByResRef(pc.Ring2Refs.resref); }
                        else if (gv.cc.partyItemSlotIndex == 8) { it = gv.cc.getItemByResRef(pc.AmmoRefs.resref); }
                        if (it != null)
                        {
                            gv.sf.ShowFullDescription(it);
                        }
                    }
                    else if (btnReturn.getImpact(x, y))
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
                        }
                        else
                        {
                            gv.screenType = "main";
                        }
                    }
                    else if (btnPartyRoster.getImpact(x, y))
                    {
                        if (!inCombat)
                        {
                            gv.screenType = "partyRoster";
                        }
                    }
                    if (!inCombat)
                    {
                        for (int j = 0; j < gv.mod.playerList.Count; j++)
                        {
                            if (btnPartyIndex[j].getImpact(x, y))
                            {
                                gv.mod.selectedPartyLeader = j;
                                gv.cc.addLogText("lime", gv.mod.playerList[j].name + " is Party Leader");
                                if (gv.cc.partyScreenPcIndex == j)
                                {
                                    doInterPartyConvo(); //not used in The Raventhal
                                }
                                gv.cc.partyScreenPcIndex = j;
                                resetTokenAndPortrait();
                            }
                        }
                    }
                    break;
            }
        }
        public void tokenLoad(Player p)
        {
            //p.token = gv.cc.LoadBitmap(p.tokenFilename);
            btnToken.Img2 = p.tokenFilename;
        }
        public void portraitLoad(Player p)
        {
            //p.portrait = gv.cc.LoadBitmap(p.portraitFilename);
            btnPortrait.Img = p.portraitFilename;
        }
        public String isUseableBy(Item it)
        {
            string strg = "";
            foreach (string s in it.classesAllowed)
            {
                strg += s.Substring(0, 1) + ", ";
            }
            /*foreach (PlayerClass cls in gv.cc.datafile.dataPlayerClassList)
            {
                string firstLetter = cls.name.Substring(0, 1);
                foreach (ItemRefs stg in cls.itemsAllowed)
                {
                    if (stg.resref.Equals(it.resref))
                    {
                        strg += firstLetter + ", ";
                    }
                }
            }*/
            return strg;
        }
        public void doInterPartyConvo()
        {
            if (gv.cc.partyScreenPcIndex == 0)
            {
                return;
            }
            if (gv.cc.partyScreenPcIndex >= gv.mod.playerList.Count)
            {
                return;
            }
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
            gv.cc.doConversationBasedOnTag(pc.name);
        }
        public bool canNotBeUnequipped()
        {
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
            if (gv.cc.partyItemSlotIndex == 0) { return pc.MainHandRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 1) { return pc.HeadRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 2) { return pc.NeckRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 3) { return pc.OffHandRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 4) { return pc.RingRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 5) { return pc.BodyRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 6) { return pc.FeetRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 7) { return pc.Ring2Refs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 8) { return pc.AmmoRefs.canNotBeUnequipped; }
            return false;
        }
        public void switchEquipment(bool inCombat)
        {
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
            if (gv.cc.partyItemSlotIndex == 3)
            {
                if (gv.cc.getItemByResRefForInfo(pc.MainHandRefs.resref).twoHanded)
                {
                    gv.sf.MessageBoxHtml("Can't equip an item in off-hand while using a two-handed weapon. Unequip the two-handed weapon from the main-hand first.");
                    return;
                }
            }

            //check to see if ammo can be used by MainHand weapon
            if (gv.cc.partyItemSlotIndex == 8)
            {
                Item itMH = gv.cc.getItemByResRef(pc.MainHandRefs.resref);
                if ((!itMH.category.Equals("Ranged")) || (itMH.ammoType.Equals("none")))
                {
                    gv.sf.MessageBoxHtml("Can't use ammo with the weapon currently equipped in your main-hand.");
                    return;
                }
            }

            //check to see if item can not be unequipped
            if (canNotBeUnequipped())
            {
                gv.sf.MessageBoxHtml("Can't unequip this item...PC specific item or a cursed item.");
                return;
            }

            List<ItemRefs> allowedItems = new List<ItemRefs>();

            //add any other allowed items to the allowed list
            foreach (ItemRefs itRef in gv.mod.partyInventoryRefsList)
            {
                Item it = gv.cc.getItemByResRef(itRef.resref);
                if (gv.cc.partyItemSlotIndex == 0)
                {
                    if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
                    {
                        if (it.containsAllowedClassByTag(pc.playerClass.tag))
                        //if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                        {
                            allowedItems.Add(itRef);
                        }
                    }
                }
                else if ((it.category.Equals("Head")) && (gv.cc.partyItemSlotIndex == 1))
                {
                    if (it.containsAllowedClassByTag(pc.playerClass.tag))
                    {
                        allowedItems.Add(itRef);
                    }
                }
                else if ((it.category.Equals("Neck")) && (gv.cc.partyItemSlotIndex == 2))
                {
                    if (it.containsAllowedClassByTag(pc.playerClass.tag))
                    {
                        allowedItems.Add(itRef);
                    }
                }
                else if ((it.category.Equals("Shield")) && (gv.cc.partyItemSlotIndex == 3))
                {
                    if (it.containsAllowedClassByTag(pc.playerClass.tag))
                    {
                        allowedItems.Add(itRef);
                    }
                }
                else if ((it.category.Equals("Ring")) && (gv.cc.partyItemSlotIndex == 4))
                {
                    if (it.containsAllowedClassByTag(pc.playerClass.tag))
                    {
                        allowedItems.Add(itRef);
                    }
                }
                else if ((it.category.Equals("Armor")) && (gv.cc.partyItemSlotIndex == 5))
                {
                    if (it.containsAllowedClassByTag(pc.playerClass.tag))
                    {
                        allowedItems.Add(itRef);
                    }
                }
                else if ((it.category.Equals("Feet")) && (gv.cc.partyItemSlotIndex == 6))
                {
                    if (it.containsAllowedClassByTag(pc.playerClass.tag))
                    {
                        allowedItems.Add(itRef);
                    }
                }
                else if ((it.category.Equals("Ring")) && (gv.cc.partyItemSlotIndex == 7))
                {
                    if (it.containsAllowedClassByTag(pc.playerClass.tag))
                    {
                        allowedItems.Add(itRef);
                    }
                }
                else if ((it.category.Equals("Ammo")) && (gv.cc.partyItemSlotIndex == 8))
                {
                    if (it.containsAllowedClassByTag(pc.playerClass.tag))
                    {
                        Item itMH = gv.cc.getItemByResRef(pc.MainHandRefs.resref);
                        if ((itMH.category.Equals("Ranged")) && (!itMH.ammoType.Equals("none")) && (itMH.ammoType.Equals(it.ammoType)))
                        {
                            allowedItems.Add(itRef);
                        }
                    }
                }
            }

            //pass items to selector
            gv.screenType = "itemSelector";
            if (inCombat)
            {
                gv.screenItemSelector.resetItemSelector(allowedItems, "equip", "combatParty");
            }
            else
            {
                gv.screenItemSelector.resetItemSelector(allowedItems, "equip", "party");
            }
        }
        public void doLevelUpSetup()
        {
            List<string> actionList = new List<string> { "Cancel", "LEVEL UP" };
            gv.itemListSelector.setupIBminiItemListSelector(gv, actionList, "Level Up Action", "partyscreenlevelup");
            gv.itemListSelector.showIBminiItemListSelector = true;
        }
        public void doLevelUp(int selectedIndex)
        {
            if (selectedIndex == 0) // selected to Cancel
            {
                //do nothing
            }
            else if (selectedIndex == 1) // selected to LEVEL UP
            {
                Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
                //LEVEL UP ALL STATS AND UPDATE STATS
                pc.LevelUp();
                gv.sf.UpdateStats(pc);
                traitGained = "Trait Gained: ";
                spellGained = "Spell Gained: ";

                //if automatically learned traits or spells add them
                foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                {
                    if ((ta.automaticallyLearned) && (ta.atWhatLevelIsAvailable == pc.classLevel))
                    {
                        traitGained += ta.name + ", ";
                        pc.knownTraitsTags.Add(ta.tag);
                    }
                }
                foreach (SpellAllowed sa in pc.playerClass.spellsAllowed)
                {
                    if ((sa.automaticallyLearned) && (sa.atWhatLevelIsAvailable == pc.classLevel))
                    {
                        spellGained += sa.name + ", ";
                        pc.knownSpellsTags.Add(sa.tag);
                    }
                }

                //check to see if have any traits to learn
                List<string> traitTagsList = new List<string>();
                traitTagsList = pc.getTraitsToLearn(gv);

                //check to see if have any spells to learn
                List<string> spellTagsList = new List<string>();
                spellTagsList = pc.getSpellsToLearn();

                //if so then ask which one
                if (traitTagsList.Count > 0)
                {
                    gv.screenTraitLevelUp.resetPC(false, false, pc);
                    gv.screenType = "learnTraitLevelUp";
                }
                else if (spellTagsList.Count > 0)
                {
                    gv.screenSpellLevelUp.resetPC(false, false, pc);
                    gv.screenType = "learnSpellLevelUp";
                }
                else //no spells or traits to learn
                {
                    doLevelUpSummary();
                }
            }
        }
        public void doLevelUpSummary()
        {
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
            int babGained = pc.playerClass.babTable[pc.classLevel] - pc.playerClass.babTable[pc.classLevel - 1];

            string text = pc.name + " has gained:<br>"
                   + "HP: +" + pc.playerClass.hpPerLevelUp + "<br>"
                   + "SP: +" + pc.playerClass.spPerLevelUp + "<br>"
                   + "BAB: +" + babGained + "<br>"
                   + traitGained + "<br>"
                   + spellGained;
            gv.sf.MessageBoxHtml(text);
        }
        public void tutorialMessageParty(bool helpCall)
        {
            if ((gv.mod.showTutorialParty) || (helpCall))
            {
                gv.sf.MessageBoxHtml(gv.cc.stringMessageParty);
                gv.mod.showTutorialParty = false;
            }
        }
    }
}
