using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class ScreenPcCreation
    {
        //public Module gv.mod;
        public GameView gv;

        private IbbButton btnName = null;
        private IbbButton btnRace = null;
        private IbbButton btnClass = null;
        private IbbButton btnGender = null;
        private IbbPortrait btnPortrait = null;
        private IbbButton btnToken = null;

        private IbbButton btnRollStats = null;
        private IbbButton btnFinished = null;
        private IbbButton btnAbort = null;
        private IbbButton btnHelp = null;

        private IBminiTextBox description;

        private string blankItemSlot;
        private int pcCreationIndex = 0;
        //private int pcTokenSelectionIndex = 0;
        //private int pcPortraitSelectionIndex = 0;
        private int pcRaceSelectionIndex = 0;
        private int pcClassSelectionIndex = 0;
        public List<string> playerTokenList = new List<string>();
        public List<string> playerPortraitList = new List<string>();
        public List<Race> playerRaces = new List<Race>();
        public Player pc;

        public ScreenPcCreation(Module m, GameView g)
        {
            //gv.mod = m;
            gv = g;
            blankItemSlot = "item_slot";
            LoadPlayerBitmapList();
            LoadPlayerPortraitList();
            CreateRaceList();
            resetPC();
            setControlsStart();
            description = new IBminiTextBox(gv);
            description.tbXloc = 0 * gv.squareSize;
            description.tbYloc = 3 * gv.squareSize;
            description.tbWidth = 6 * gv.squareSize;
            description.tbHeight = 6 * gv.squareSize;
            description.showBoxBorder = false;
        }

        public void resetPC()
        {
            if ((gv.mod.defaultPlayerFilename.Equals("")) || (gv.mod.defaultPlayerFilename.Equals("none")))
            {
                if (gv.mod.defaultPlayerFilenameList.Count > 0)
                {
                    pc = gv.cc.LoadPlayer(gv.mod.defaultPlayerFilenameList[0].stringValue);
                }                
            }
            else
            {
                pc = gv.cc.LoadPlayer(gv.mod.defaultPlayerFilename);
            }
            //pc = gv.cc.LoadPlayer(gv.mod.defaultPlayerFilename);
            //pc.token = gv.cc.LoadBitmap(pc.tokenFilename);
            //pc.portrait = gv.cc.LoadBitmap(pc.portraitFilename);
            pc.playerClass = gv.cc.getPlayerClass(pc.classTag);
            pc.race = this.getAllowedRace(pc.raceTag);
            //pc.name = "CharacterName";
            //pc.tag = "characterName";
            pcCreationIndex = 0;
            reRollStats(pc);
        }
        public void CreateRaceList()
        {
            //Create Race List
            playerRaces.Clear();
            foreach (Race rc in gv.cc.datafile.dataRacesList)
            {
                if (rc.UsableByPlayer)
                {
                    Race newRace = rc.DeepCopy();
                    newRace.classesAllowed.Clear();
                    foreach (string s in rc.classesAllowed)
                    {
                        PlayerClass plc = gv.cc.getPlayerClass(s);
                        if ((plc != null) && (plc.UsableByPlayer))
                        {
                            newRace.classesAllowed.Add(s);
                        }
                    }
                    playerRaces.Add(newRace);
                }
            }
        }
        public Race getAllowedRace(string tag)
        {
            foreach (Race r in this.playerRaces)
            {
                if (r.tag.Equals(tag)) return r;
            }
            return null;
        }
        public void LoadPlayerBitmapList()
        {
            playerTokenList.Clear();
            try
            {
                //Load from module folder first
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\pctokens"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\pctokens", "*.png");
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if (filename.EndsWith("_pc.png"))
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
            try
            {
                //Load from PlayerTokens folder last
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\PlayerTokens"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\PlayerTokens", "*.png");
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if (filename.EndsWith("_pc.png"))
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
        public void LoadPlayerPortraitList()
        {
            playerPortraitList.Clear();
            try
            {
                //Load from module folder first
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\portraits"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\portraits", "*.png");
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if ((filename.EndsWith(".png")) || (filename.EndsWith(".PNG")))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                playerPortraitList.Add(fileNameWithOutExt);
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
            try
            {
                //Load from PlayerTokens folder last
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\PlayerPortraits"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\PlayerPortraits", "*.png");
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if ((filename.EndsWith(".png")) || (filename.EndsWith(".PNG")))
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
            int padW = gv.squareSize / 6;
            int center = (gv.squareSize * gv.squaresInWidth / 2);

            if (btnPortrait == null)
            {
                btnPortrait = new IbbPortrait(gv, 1.0f);
            }
                btnPortrait.ImgBG = "item_slot";
                btnPortrait.Glow = "btn_small_glow";
                btnPortrait.X = 5 * gv.squareSize - gv.squareSize / 2;
                btnPortrait.Y = 0 * gv.squareSize + gv.squareSize / 2;
                btnPortrait.Height = (int)(gv.ibpheight * gv.screenDensity);
                btnPortrait.Width = (int)(gv.ibpwidth * gv.screenDensity);

            if (btnToken == null)
            {
                btnToken = new IbbButton(gv, 1.0f);
            }
                btnToken.Img = "item_slot";
                btnToken.Img2 = pc.tokenFilename;
                btnToken.Glow = "btn_small_glow";
                btnToken.X = 5 * gv.squareSize - gv.squareSize / 2;
                btnToken.Y = 2 * gv.squareSize + gv.squareSize / 2;
                btnToken.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnToken.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            
            if (btnName == null)
            {
                btnName = new IbbButton(gv, 1.0f);
            }
                btnName.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnName.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnName.X = 1 * gv.squareSize;
                btnName.Y = (int)(0.5 * gv.squareSize);
                btnName.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnName.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            if (btnRace == null)
            {
                btnRace = new IbbButton(gv, 1.0f);
            }
                btnRace.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnRace.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnRace.X = 1 * gv.squareSize;
                btnRace.Y = (int)(1.5 * gv.squareSize) + gv.squareSize / 8;
                btnRace.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRace.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            if (btnGender == null)
            {
                btnGender = new IbbButton(gv, 1.0f);
            }
                btnGender.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnGender.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnGender.X = 1 * gv.squareSize;
                btnGender.Y = (int)(2.5 * gv.squareSize) + gv.squareSize / 4;
                btnGender.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnGender.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            if (btnClass == null)
            {
                btnClass = new IbbButton(gv, 1.0f);
            }
                btnClass.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnClass.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnClass.X = 1 * gv.squareSize;
                btnClass.Y = (int)(3.5 * gv.squareSize) + 3 * gv.squareSize / 8;
                btnClass.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnClass.Width = (int)(gv.ibbwidthL * gv.screenDensity);
                        
            if (btnRollStats == null)
            {
                btnRollStats = new IbbButton(gv, 1.0f);
            }
                btnRollStats.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnRollStats.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnRollStats.Text = "Roll Stats";
                btnRollStats.X = 1 * gv.squareSize;
                btnRollStats.Y = 6 * gv.squareSize - gv.squareSize / 4;
                btnRollStats.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRollStats.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            if (btnFinished == null)
            {
                btnFinished = new IbbButton(gv, 1.0f);
            }
                btnFinished.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnFinished.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnFinished.Text = "Finished";
                btnFinished.X = 7 * gv.squareSize;
                btnFinished.Y = 6 * gv.squareSize - gv.squareSize / 4;
                btnFinished.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnFinished.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            if (btnAbort == null)
            {
                btnAbort = new IbbButton(gv, 0.8f);
            }
                btnAbort.Text = "Abort";
                btnAbort.Img = "btn_small"; // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnAbort.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnAbort.X = 4 * gv.squareSize + gv.squareSize / 4;
                btnAbort.Y = 6 * gv.squareSize - gv.squareSize / 4;
                btnAbort.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAbort.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnHelp == null)
            {
                btnHelp = new IbbButton(gv, 0.8f);
            }
                btnHelp.Text = "HELP";
                btnHelp.Img = "btn_small"; // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnHelp.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnHelp.X = 5 * gv.squareSize + gv.squareSize / 2;
                btnHelp.Y = 6 * gv.squareSize - gv.squareSize / 4;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            
        }

        public void redrawPcCreation()
        {
            setControlsStart();
            //Player pc = gv.mod.playerList.get(0);
            gv.sf.UpdateStats(pc);

            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);

            int locX = 4 * gv.squareSize;
            //int textH = (int)gv.drawFontRegHeight;
            int spacing = gv.fontHeight + gv.fontLineSpacing;
            int locY = 3 * gv.squareSize + gv.squareSize / 2;
            int locY2 = 3 * gv.squareSize + gv.squareSize / 2;
            int tabX = 5 * gv.squareSize;
            int tabX2 = 8 * gv.squareSize;
            int leftStartY = pH * 20;
            int tokenStartX = locX + (gv.fontHeight * 5);
            int tokenStartY = pH * 5 + (spacing / 2);
            int portraitStartX = 12 * gv.squareSize + (gv.fontHeight * 5);
            int portraitStartY = pH * 5 + (spacing / 2);
            int tokenRectPad = pW * 1;

            //Page Title
            gv.DrawText("CREATE CHARACTER", pW * 40 + gv.squareSize, pH * 1, "yl");

            Color color = Color.White;

            int actext = 0;
            if (gv.mod.ArmorClassAscending) { actext = pc.AC; }
            else { actext = 20 - pc.AC; }
            
            if (!gv.mod.use3d6)
            {
                gv.DrawText("Rolling: 2d6 + 6", locX + pW, locY += (spacing), "gy");
            }
            else if (gv.mod.use3d6)
            {
                gv.DrawText("Rolling: 3d6", locX + pW, locY += (spacing), "gy");
            }
            gv.DrawText("STR: " + pc.baseStr + " + " + (pc.strength - pc.baseStr) + " = " + pc.strength + " (" + ((pc.strength - 10) / 2) + ")", locX + pW, locY += (spacing), "wh");
            gv.DrawText("AC: " + actext + ", BAB: " + pc.baseAttBonus, tabX2, locY2 += (spacing * 2), "wh");
            gv.DrawText("DEX: " + pc.baseDex + " + " + (pc.dexterity - pc.baseDex) + " = " + pc.dexterity + " (" + ((pc.dexterity - 10) / 2) + ")", locX + pW, locY += spacing, "wh");
            gv.DrawText("HP: " + pc.hp + "/" + pc.hpMax, tabX2, locY2 += spacing, "wh");
            gv.DrawText("CON: " + pc.baseCon + " + " + (pc.constitution - pc.baseCon) + " = " + pc.constitution + " (" + ((pc.constitution - 10) / 2) + ")", locX + pW, locY += spacing, "wh");
            gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, tabX2, locY2 += spacing, "wh");
            gv.DrawText("INT: " + pc.baseInt + " + " + (pc.intelligence - pc.baseInt) + " = " + pc.intelligence + " (" + ((pc.intelligence - 10) / 2) + ")", locX + pW, locY += spacing, "wh");
            gv.DrawText("FORT: " + pc.fortitude, tabX2, locY2 += spacing, "wh");
            gv.DrawText("REF:  " + pc.reflex, tabX2, locY2 += spacing, "wh");
            gv.DrawText("WILL: " + pc.will, tabX2, locY2 += spacing, "wh");
            gv.DrawText("WIS: " + pc.baseWis + " + " + (pc.wisdom - pc.baseWis) + " = " + pc.wisdom + " (" + ((pc.wisdom - 10) / 2) + ")", locX + pW, locY += spacing, "wh");
            gv.DrawText("CHA: " + pc.baseCha + " + " + (pc.charisma - pc.baseCha) + " = " + pc.charisma + " (" + ((pc.charisma - 10) / 2) + ")", locX + pW, locY += spacing, "wh");

            //Description
            int yLoc = 0 * gv.squareSize + gv.squareSize / 2;
            description.tbXloc = tabX + gv.squareSize - pW;
            description.tbYloc = yLoc;
            description.tbWidth = 4 * gv.squareSize;
            description.tbHeight = 6 * gv.squareSize;

            string textToSpan = "";
            if (pcCreationIndex == 2)
            {
                textToSpan = "<gn>Description:</gn>" + Environment.NewLine;
                textToSpan += pc.race.description;
            }
            else if (pcCreationIndex == 4)
            {
                textToSpan = "<gn>Description:</gn>" + Environment.NewLine;
                textToSpan += pc.playerClass.description;
            }            
            //IbRect rect = new IbRect(tabX + gv.squareSize - pW, yLoc, pW * 35, pH * 50);            
            description.linesList.Clear();
            description.AddFormattedTextToTextBox(textToSpan);
            description.onDrawTextBox();
            //gv.DrawText(textToSpan, rect, 1.0f, Color.White);

            btnPortrait.Img = pc.portraitFilename;
            btnPortrait.Draw();
            btnToken.Draw();
            btnName.Text = pc.name;
            btnName.Draw();
            btnRace.Text = pc.race.name;
            btnRace.Draw();
            if (pc.isMale)
            {
                btnGender.Text = "Male";
            }
            else
            {
                btnGender.Text = "Female";
            }
            btnGender.Draw();
            btnClass.Text = pc.playerClass.name;
            btnClass.Draw();

            btnRollStats.Draw();
            btnFinished.Draw();
            btnHelp.Draw();
            btnAbort.Draw();
            if (gv.showMessageBox)
            {
                gv.messageBox.onDrawLogBox();
            }
        }
        public void onTouchPcCreation(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType)
        {
            btnRollStats.glowOn = false;
            btnFinished.glowOn = false;
            btnAbort.glowOn = false;
            btnHelp.glowOn = false;
            btnName.glowOn = false;
            btnRace.glowOn = false;
            btnClass.glowOn = false;
            btnGender.glowOn = false;
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

                    if (btnRollStats.getImpact(x, y))
                    {
                        btnRollStats.glowOn = true;
                    }
                    else if (btnFinished.getImpact(x, y))
                    {
                        btnFinished.glowOn = true;
                    }
                    else if (btnAbort.getImpact(x, y))
                    {
                        btnAbort.glowOn = true;
                    }
                    else if (btnHelp.getImpact(x, y))
                    {
                        btnHelp.glowOn = true;
                    }
                    else if (btnName.getImpact(x, y))
                    {
                        btnName.glowOn = true;
                    }
                    else if (btnRace.getImpact(x, y))
                    {
                        btnRace.glowOn = true;
                    }
                    else if (btnClass.getImpact(x, y))
                    {
                        btnClass.glowOn = true;
                    }
                    else if (btnGender.getImpact(x, y))
                    {
                        btnGender.glowOn = true;
                    }                    
                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)eX;
                    y = (int)eY;

                    btnName.glowOn = false;
                    btnRace.glowOn = false;
                    btnClass.glowOn = false;
                    btnGender.glowOn = false;
                    btnRollStats.glowOn = false;
                    btnFinished.glowOn = false;
                    btnAbort.glowOn = false;
                    btnHelp.glowOn = false;
                    
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

                    if (btnName.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        pcCreationIndex = 1;
                        changePcName();
                    }
                    else if (btnRace.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        pcCreationIndex = 2;
                        pcRaceSelectionIndex++;
                        if (pcRaceSelectionIndex >= this.playerRaces.Count)
                        {
                            pcRaceSelectionIndex = 0;
                        }
                        pc.race = playerRaces[pcRaceSelectionIndex];
                        pc.raceTag = pc.race.tag;
                        resetClassSelection(pc);
                        gv.sf.UpdateStats(pc);
                        pc.hp = pc.hpMax;
                        pc.sp = pc.spMax;
                    }
                    else if (btnGender.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        pcCreationIndex = 3;
                        if (pc.isMale)
                        {
                            pc.isMale = false;
                        }
                        else
                        {
                            pc.isMale = true;
                        }
                    }
                    else if (btnClass.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        pcCreationIndex = 4;
                        pcClassSelectionIndex++;
                        if (pcClassSelectionIndex >= pc.race.classesAllowed.Count)
                        {
                            pcClassSelectionIndex = 0;
                        }
                        pc.playerClass = gv.cc.getPlayerClass(pc.race.classesAllowed[pcClassSelectionIndex]);
                        pc.classTag = pc.playerClass.tag;
                        gv.sf.UpdateStats(pc);
                        pc.hp = pc.hpMax;
                        pc.sp = pc.spMax;
                    }
                    else if (btnPortrait.getImpact(x, y))
                    {
                        //pass items to selector
                        gv.screenType = "portraitSelector";
                        gv.screenPortraitSelector.resetPortraitSelector("pcCreation", pc);
                    }
                    else if (btnToken.getImpact(x, y))
                    {
                        gv.screenType = "tokenSelector";
                        gv.screenTokenSelector.resetTokenSelector("pcCreation", pc);
                    }

                    else if (btnRollStats.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        reRollStats(pc);
                    }
                    else if (btnFinished.getImpact(x, y))
                    {
                        //hurghxxx
                        if ((pc.name != "CharacterName") && (pc.name != ""))
                        {
                            gv.PlaySound("btn_click");
                            
                            //if automatically learned traits or spells add them
                            foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                            {
                                if ((ta.automaticallyLearned) && (ta.atWhatLevelIsAvailable == pc.classLevel))
                                {
                                    pc.knownTraitsTags.Add(ta.tag);
                                }
                            }
                            foreach (SpellAllowed sa in pc.playerClass.spellsAllowed)
                            {
                                if ((sa.automaticallyLearned) && (sa.atWhatLevelIsAvailable == pc.classLevel))
                                {
                                    pc.knownSpellsTags.Add(sa.tag);
                                }
                            }

                            //check to see if have any traits to learn
                            List<string> traitTagsList = new List<string>();
                            traitTagsList = pc.getTraitsToLearn(gv);

                            //check to see if have any spells to learn
                            List<string> spellTagsList = new List<string>();
                            spellTagsList = pc.getSpellsToLearn();

                            if (traitTagsList.Count > 0)
                            {
                                gv.screenTraitLevelUp.resetPC(false, false, pc);
                                gv.screenType = "learnTraitCreation";
                            }

                            else if (spellTagsList.Count > 0)
                            {
                                gv.screenSpellLevelUp.resetPC(false, false, pc);
                                gv.screenType = "learnSpellCreation";
                            }
                            else
                            {
                                //no spells or traits to learn
                                //save character, add them to the pcList of screenPartyBuild, and go back to build screen
                                this.SaveCharacter(pc);
                                gv.screenPartyBuild.pcList.Add(pc);
                                gv.screenType = "partyBuild";
                            }
                        }
                        else
                        {
                            gv.sf.MessageBoxHtml("Name cannot be CharacterName or blank, please choose a different one.");
                        }
                    }
                    else if (btnAbort.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        gv.screenType = "partyBuild";
                    }
                    else if (btnHelp.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        gv.showMessageBox = true;
                        gv.cc.tutorialPcCreation();
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
        public void changePcName()
        {
            using (TextInputDialog itSel = new TextInputDialog(gv, "Choose a unique Name for this PC."))
            {
                itSel.textInput = "Type unique Name Here";

                var ret = itSel.ShowDialog();

                if (ret == DialogResult.OK)
                {
                    if (itSel.textInput.Length > 0)
                    {
                        pc.name = itSel.textInput;
                        pc.tag = itSel.textInput.ToLower();
                        bool foundNameConflict = false;
                        foreach (Player p in gv.mod.playerList)
                        {
                            if ((p.name == pc.name) || (p.tag == pc.tag))
                            {
                                gv.sf.MessageBoxHtml("This name already exists, please choose a different one.");
                                pc.name = "";
                                pc.tag = "";
                                itSel.textInput = "Type unique Name Here";
                                foundNameConflict = true;
                                break;
                            }
                        }
                        if (foundNameConflict == false)
                        {
                            foreach (Player p in gv.screenPartyBuild.pcList)
                            {
                                if ((p.name == pc.name) || (p.tag == pc.tag))
                                {
                                    gv.sf.MessageBoxHtml("This name already exists, please choose a different one.");
                                    pc.name = "";
                                    pc.tag = "";
                                    itSel.textInput = "Type unique Name Here";
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Entering a blank name is not allowed", Toast.LENGTH_SHORT).show();
                    }
                }
            }
        }
        public void reRollStats(Player p)
        {
            if (gv.mod.use3d6 == true)
            {
                p.baseStr = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseDex = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseInt = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseCha = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseCon = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseWis = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                int sumOfAttributeBoni = ((p.baseStr - 10) / 2) + ((p.baseDex - 10) / 2) + ((p.baseCon - 10) / 2) + ((p.baseInt - 10) / 2) + ((p.baseWis - 10) / 2) + ((p.baseCha - 10) / 2);
                if (sumOfAttributeBoni > 6)
                {
                    p.baseLuck = 10 - (sumOfAttributeBoni - 6);
                }
                else
                {
                    p.baseLuck = 10 + (6 - sumOfAttributeBoni);
                }
                //p.baseLuck = (int)(14 - ((p.baseStr - 10) / 2) - ((p.baseDex - 10) / 2) - ((p.baseCon - 10) / 2) - ((p.baseInt - 10) / 2) - ((p.baseWis - 10) / 2) - ((p.baseCha - 10) / 2));
                if (p.baseLuck < 3)
                {
                    p.baseLuck = 3;
                }
            }
            else
            {
                p.baseStr = 6 + gv.sf.RandInt(12);
                p.baseDex = 6 + gv.sf.RandInt(12);
                p.baseInt = 6 + gv.sf.RandInt(12);
                p.baseCha = 6 + gv.sf.RandInt(12);
                p.baseCon = 6 + gv.sf.RandInt(12);
                p.baseWis = 6 + gv.sf.RandInt(12);
                int sumOfAttributeBoni = ((p.baseStr - 10) / 2) + ((p.baseDex - 10) / 2) + ((p.baseCon - 10) / 2) + ((p.baseInt - 10) / 2) + ((p.baseWis - 10) / 2) + ((p.baseCha - 10) / 2);
                if (sumOfAttributeBoni > 6)
                {
                    p.baseLuck = 10 - (sumOfAttributeBoni - 6);
                }
                else
                {
                    p.baseLuck = 10 + (6 - sumOfAttributeBoni);
                }
                //p.baseLuck = (int)(14 - ((p.baseStr - 10) / 2) - ((p.baseDex - 10) / 2) - ((p.baseCon - 10) / 2) - ((p.baseInt - 10) / 2) - ((p.baseWis - 10) / 2) - ((p.baseCha - 10) / 2));
                if (p.baseLuck < 3)
                {
                    p.baseLuck = 3;
                }
            }

            gv.sf.UpdateStats(p);
            p.hp = p.hpMax;
            p.sp = p.spMax;
        }
        public void resetClassSelection(Player p)
        {
            pcClassSelectionIndex = 0;
            p.playerClass = gv.cc.getPlayerClass(p.race.classesAllowed[pcClassSelectionIndex]);
            p.classTag = p.playerClass.tag;
            gv.sf.UpdateStats(p);
            p.hp = p.hpMax;
            p.sp = p.spMax;
        }
        public void SaveCharacter(Player p)
        {
            string filename = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\characters\\" + pc.tag + ".json";
            gv.cc.MakeDirectoryIfDoesntExist(filename);
            string json = JsonConvert.SerializeObject(pc, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(json.ToString());
            }
        }
    }
}
