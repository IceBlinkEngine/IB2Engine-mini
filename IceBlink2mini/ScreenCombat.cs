﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Color = SharpDX.Color;
using Newtonsoft.Json;

namespace IceBlink2mini
{
    public class ScreenCombat 
    {
        public Module mod;
        public GameView gv;
        public IB2UILayout combatUiLayout = null;
        public bool showHP = false;
        public bool showSP = false;
        public bool showMoveOrder = false;
        public bool showArrows = true;
        public bool showTogglePanel = false;

        //COMBAT STUFF
        //public bool adjustCamToRangedCreature = false;
        private bool isPlayerTurn = true;
        public bool canMove = true;
        public int currentPlayerIndex = 0;
        public int creatureIndex = 0;
        public int currentMoveOrderIndex = 0;
        public List<MoveOrder> moveOrderList = new List<MoveOrder>();
        public int initialMoveOrderListSize = 0;
        public float currentMoves = 0;
        public float creatureMoves = 0;
        //public Coordinate UpperLeftSquare = new Coordinate();
        public string currentCombatMode = "info"; //info, move, cast, attack
        public Coordinate targetHighlightCenterLocation = new Coordinate();
        public Coordinate creatureTargetLocation = new Coordinate();
        public int encounterXP = 0;
        private Creature creatureToAnimate = null;
        private Player playerToAnimate = null;
        private Coordinate hitAnimationLocation = new Coordinate();
        public int spellSelectorIndex = 0;
        public List<string> spellSelectorSpellTagList = new List<string>();
        private Coordinate projectileAnimationLocation = new Coordinate();
        private Coordinate endingAnimationLocation = new Coordinate();
        public bool drawDeathAnimation = true;
        public List<Coordinate> deathAnimationLocations = new List<Coordinate>();
        //private int animationFrameIndex = 0;
        public PathFinderEncounters pf;
        public bool floatyTextOn = false;
        public AnimationState animationState = AnimationState.None;
        //private Bitmap mapBitmap;

        public int mapStartLocXinPixels;
        public float moveCost = 1.0f;
        public List<Sprite> spriteList = new List<Sprite>();
        public List<AnimationSequence> animationSeqStack = new List<AnimationSequence>();
        public bool animationsOn = false;
        public int attackAnimationTimeElapsed = 0;
        public int attackAnimationLengthInMilliseconds = 250;
        public int triggerIndexCombat = 0;
        public bool didTriggerEvent = false;

        public ScreenCombat(Module m, GameView g)
        {
            mod = m;
            gv = g;
            mapStartLocXinPixels = 4 * gv.squareSize;
            loadMainUILayout();
        }

        public void loadMainUILayout()
        {
            try
            {
                if (File.Exists(gv.cc.GetModulePath() + "\\data\\CombatUILayout.json"))
                {
                    using (StreamReader file = File.OpenText(gv.cc.GetModulePath() + "\\data\\CombatUILayout.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        combatUiLayout = (IB2UILayout)serializer.Deserialize(file, typeof(IB2UILayout));
                        combatUiLayout.setupIB2UILayout(gv);
                    }
                }
                else
                {
                    using (StreamReader file = File.OpenText(gv.mainDirectory + "\\default\\NewModule\\data\\CombatUILayout.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        combatUiLayout = (IB2UILayout)serializer.Deserialize(file, typeof(IB2UILayout));
                        combatUiLayout.setupIB2UILayout(gv);
                    }
                }

                IB2ToggleButton tgl = combatUiLayout.GetToggleByTag("tglHP");
                if (tgl != null)
                {
                    showHP = tgl.toggleOn;
                }
                IB2ToggleButton tgl2 = combatUiLayout.GetToggleByTag("tglSP");
                if (tgl2 != null)
                {
                    showSP = tgl2.toggleOn;
                }
                foreach (IB2Panel pnlC in combatUiLayout.panelList)
                {
                    if (pnlC.tag.Equals("logPanel"))
                    {
                        foreach (IB2Panel pnlM in gv.screenMainMap.mainUiLayout.panelList)
                        {
                            if (pnlM.tag.Equals("logPanel"))
                            {
                                pnlC.logList[0] = pnlM.logList[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Loading CombatUILayout.json: " + ex.ToString());
                gv.errorLog(ex.ToString());
            }
        }
        public void tutorialMessageCombat(bool helpCall)
        {
            if ((mod.showTutorialCombat) || (helpCall))
            {
                string s =
                        "<big><b>COMBAT</b></big><br><br>" +

                        "<b>1. Player's Turn:</b> Each player takes a turn. The current player will be highlighted with a" +
                        " light blue box. You can Move one square (or stay put) and make one additional action such" +
                        " as ATTACK, CAST, use item, or end turn (SKIP button).<br><br>" +

                        "<b>2. Info Mode:</b> Info mode is the default mode. In this mode you can tap on a token (player or enemy image) to show" +
                        " some of their stats (HP, SP, etc.). If none of the buttons are highlighted, then you are in 'Info' mode. If you are" +
                        " in 'move' mode and want to return to 'info' mode, tap on the move button to unselect it and return to 'info' mode. Same" +
                        " concept works for 'attack' mode back to 'info' mode.<br><br>" +

                        "<b>3. Move:</b> After pressing move, you may move one square and then do one more action or press 'SKIP' to end this Player's" +
                        " turn. You move by pressing one of the arrow direction buttons or tapping on a square adjacent to the PC.<br><br>" +

                        "<b>3. Attack:</b> After pressing attack, move the target selection square by pressing the arrow keys or tapping on any map square." +
                        " Once you have selected a valid target (box will be green), press the 'TARGET' button or tap on the targeted map square (green box)" +
                        " again to complete the action.<br><br>" +

                        "<b>4. Cast:</b> After pressing cast and selecting a spell from the spell selection screen, move the target selection square by" +
                        " pressing the arrow keys or tapping on any map square. Once you have selected a valid target (box will be green), press the" +
                        " 'TARGET' button or tap on the targeted map square (green box) again to complete the action.<br><br>" +

                        "<b>5. Skip:</b> The 'SKIP' button will end the current player's turn.<br><br>" +

                        "<b>6. Use Item:</b> press the inventory button (image of a bag) to show the party inventory screen. Only the current Player" +
                        " may use an item from this screen during combat.<br><br>" +

                        "<small><b>Note:</b> Also, check out the 'Player's Guide' in the settings menu (the rusty gear looking button)</small>"
                        ;
                gv.messageBox.logLinesList.Clear();
                gv.messageBox.AddHtmlTextToLog(s);
                gv.messageBox.currentTopLineIndex = 0;
                mod.showTutorialCombat = false;
                gv.showMessageBox = true;
            }
        }
        public void doAnimationController()
        {
            if (animationState == AnimationState.None)
            {
                return;
            }
            else if (animationState == AnimationState.CreatureThink)
            {
                creatureToAnimate = null;
                playerToAnimate = null;
                doCreatureTurnAfterDelay();
            }
            else if (animationState == AnimationState.CreatureMove)
            {
                creatureToAnimate = null;
                playerToAnimate = null;
                Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
                if (moveCost == mod.diagonalMoveCost)
                {
                    creatureMoves += mod.diagonalMoveCost;
                    moveCost = 1.0f;
                }
                else
                {
                    creatureMoves++;
                }
                //do triggers and anything else needed after each creature or PC move
                afterEachMoveCalls();
                //determine and do the next creature action
                doCreatureNextAction();
            }
        }

        public void doCombatSetup()
        {
            gv.screenType = "combat";
            //Load up all creature stuff
            foreach (CreatureRefs crf in mod.currentEncounter.encounterCreatureRefsList)
            {
                //find this creatureRef in mod creature list
                foreach (Creature c in gv.mod.moduleCreaturesList)
                {
                    if (crf.creatureResRef.Equals(c.cr_resref))
                    {
                        //copy it and add to encounters creature object list
                        try
                        {
                            Creature copy = c.DeepCopy();
                            copy.cr_tag = crf.creatureTag;
                            //gv.cc.DisposeOfBitmap(ref copy.token);
                            //copy.token = gv.cc.LoadBitmap(copy.cr_tokenFilename);
                            copy.combatLocX = crf.creatureStartLocationX;
                            copy.combatLocY = crf.creatureStartLocationY;
                            mod.currentEncounter.encounterCreatureList.Add(copy);
                        }
                        catch (Exception ex)
                        {
                            gv.errorLog(ex.ToString());
                        }
                    }
                }
            }
            //Place all PCs
            for (int index = 0; index < mod.playerList.Count; index++)
            {
                mod.playerList[index].combatLocX = mod.currentEncounter.encounterPcStartLocations[index].X;
                mod.playerList[index].combatLocY = mod.currentEncounter.encounterPcStartLocations[index].Y;
            }
            isPlayerTurn = true;
            currentPlayerIndex = 0;
            creatureIndex = 0;
            currentMoveOrderIndex = 0;
            currentCombatMode = "info";
            drawDeathAnimation = false;
            encounterXP = 0;
            foreach (Creature crtr in mod.currentEncounter.encounterCreatureList)
            {
                encounterXP += crtr.cr_XP;
            }
            pf = new PathFinderEncounters(gv, mod);
            //tutorialMessageCombat(false);            
            gv.cc.tutorialMessageCombat(false);
            //IBScript Setup Combat Hook (run only once)
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnSetupCombatIBScript, gv.mod.currentEncounter.OnSetupCombatIBScriptParms);
            //IBScript Start Combat Round Hook
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnStartCombatRoundIBScript, gv.mod.currentEncounter.OnStartCombatRoundIBScriptParms);
            //determine initiative
            calcualteMoveOrder();
            //do turn controller
            turnController();
        }
        public void calcualteMoveOrder()
        {
            moveOrderList.Clear();
            //go through each PC and creature and make initiative roll
            foreach (Player pc in mod.playerList)
            {
                int roll = gv.sf.RandInt(100) + (((pc.dexterity - 10) / 2) * 5);
                MoveOrder newMO = new MoveOrder();
                newMO.PcOrCreature = pc;
                newMO.rank = roll;
                moveOrderList.Add(newMO);

            }
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                int roll = gv.sf.RandInt(100) + (crt.initiativeBonus * 5);
                MoveOrder newMO = new MoveOrder();
                newMO.PcOrCreature = crt;
                newMO.rank = roll;
                moveOrderList.Add(newMO);
            }
            initialMoveOrderListSize = moveOrderList.Count;
            //sort PCs and creatures based on results
            moveOrderList = moveOrderList.OrderByDescending(x => x.rank).ToList();
            //assign moveOrder to PC and Creature property
            int cnt = 0;
            foreach (MoveOrder m in moveOrderList)
            {
                if (m.PcOrCreature is Player)
                {
                    Player pc = (Player)m.PcOrCreature;
                    pc.moveOrder = cnt;
                }
                else
                {
                    Creature crt = (Creature)m.PcOrCreature;
                    crt.moveOrder = cnt;
                }
                cnt++;
            }
        }
        public void turnController()
        {
            //redraw screen
            //gv.Render();
            if (currentMoveOrderIndex >= initialMoveOrderListSize)
            {
                //hit the end so start the next round
                startNextRoundStuff();
                return;
            }
            //get the next PC or Creature based on currentMoveOrderIndex and moveOrder property
            int idx = 0;
            foreach (Player pc in mod.playerList)
            {
                if (pc.moveOrder == currentMoveOrderIndex)
                {
                    //highlight the portrait of the pc whose current turn it is
                    //gv.cc.ptrPc0.glowOn = false;
                    //gv.cc.ptrPc1.glowOn = false;
                    //gv.cc.ptrPc2.glowOn = false;
                    //gv.cc.ptrPc3.glowOn = false;
                    //gv.cc.ptrPc4.glowOn = false;
                    //gv.cc.ptrPc5.glowOn = false;

                    if (idx == 0)
                    {
                        //gv.cc.ptrPc0.glowOn = true;
                    }
                    if (idx == 1)
                    {
                        //gv.cc.ptrPc1.glowOn = true;
                    }
                    if (idx == 2)
                    {
                        //gv.cc.ptrPc2.glowOn = true;
                    }
                    if (idx == 3)
                    {
                        //gv.cc.ptrPc3.glowOn = true;
                    }
                    if (idx == 4)
                    {
                        //gv.cc.ptrPc4.glowOn = true;
                    }
                    if (idx == 5)
                    {
                        //gv.cc.ptrPc5.glowOn = true;
                    }

                    //write the pc's name to log whsoe turn it is
                    //gv.cc.addLogText("<font color='blue'>It's the turn of " + pc.name + ". </font><BR>");

                    //change creatureIndex or currentPlayerIndex
                    currentPlayerIndex = idx;
                    //set isPlayerTurn 
                    isPlayerTurn = true;

                    currentCombatMode = "info";
                    currentMoveOrderIndex++;
                    gv.Render();
                    //go to start PlayerTurn or start CreatureTurn
                    if ((pc.isHeld()) || (pc.isDead()))
                    {
                        endPcTurn(false);
                    }
                    else
                    {
                        startPcTurn();
                    }
                    return;
                }
                idx++;
            }
            idx = 0;
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if (crt.moveOrder == currentMoveOrderIndex)
                {

                    //gv.cc.addLogText("<font color='blue'>It's the turn of " + crt.cr_name + ". </font><BR>");
                    //change creatureIndex or currentPlayerIndex
                    creatureIndex = idx;
                    //set isPlayerTurn
                    isPlayerTurn = false;
                    gv.touchEnabled = false;                    
                    currentCombatMode = "info";
                    currentMoveOrderIndex++;
                    gv.Render();
                    //go to start PlayerTurn or start CreatureTurn
                    if ((crt.hp > 0) && (!crt.isHeld()))
                    {
                        doCreatureTurn();
                    }
                    else
                    {
                        endCreatureTurn();
                    }
                    return;
                }
                idx++;
            }
            //didn't find one so increment moveOrderIndex and try again
            currentMoveOrderIndex++;
            turnController();
        }
        public void startNextRoundStuff()
        {
            currentMoveOrderIndex = 0;
            gv.sf.dsWorldTime();
            foreach (Player pc in mod.playerList)
            {
                RunAllItemCombatRegenerations(pc);
            }
            applyEffectsCombat();
            //IBScript Start Combat Round Hook
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnStartCombatRoundIBScript, gv.mod.currentEncounter.OnStartCombatRoundIBScriptParms);
            turnController();
        }
        public void RunAllItemCombatRegenerations(Player pc)
        {
            try
            {
                if (mod.getItemByResRefForInfo(pc.BodyRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.BodyRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.BodyRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.BodyRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.MainHandRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.MainHandRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.OffHandRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.OffHandRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.OffHandRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.OffHandRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.RingRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.RingRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.RingRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.RingRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.HeadRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.HeadRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.HeadRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.HeadRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.NeckRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.NeckRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.NeckRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.NeckRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.FeetRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.FeetRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.FeetRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.FeetRefs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.Ring2Refs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.Ring2Refs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.Ring2Refs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.Ring2Refs.resref).hpRegenPerRoundInCombat);
                }

                if (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).spRegenPerRoundInCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.AmmoRefs.resref).spRegenPerRoundInCombat);
                }
                if (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).hpRegenPerRoundInCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.AmmoRefs.resref).hpRegenPerRoundInCombat);
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }
        }
        public void doRegenSp(Player pc, int increment)
        {
            pc.sp += increment;
            if (pc.sp > pc.spMax) { pc.sp = pc.spMax; }
            gv.cc.addLogText("<gn>" + pc.name + " +" + increment + "sp</gn><br>");

        }
        public void doRegenHp(Player pc, int increment)
        {
            pc.hp += increment;
            if (pc.hp > pc.hpMax) { pc.hp = pc.hpMax; }
            gv.cc.addLogText("<gn>" + pc.name + " +" + increment + "hp</gn><br>");
        }
        public void applyEffectsCombat()
        {
            try
            {
                //maybe reorder all based on their order property            
                foreach (Player pc in mod.playerList)
                {
                    foreach (Effect ef in pc.effectsList)
                    {
                        //decrement duration of all
                        ef.durationInUnits -= gv.mod.TimePerRound;
                        if (!ef.usedForUpdateStats) //not used for stat updates
                        {
                            gv.cc.doEffectScript(pc, ef);
                        }
                    }
                }
                foreach (Creature crtr in mod.currentEncounter.encounterCreatureList)
                {
                    foreach (Effect ef in crtr.cr_effectsList)
                    {
                        //increment duration of all
                        ef.durationInUnits -= gv.mod.TimePerRound;
                        if (!ef.usedForUpdateStats) //not used for stat updates
                        {
                            //do script for each effect
                            gv.cc.doEffectScript(crtr, ef);
                        }
                    }
                }
                foreach (Effect ef in mod.currentEncounter.effectsList)
                {
                    //decrement duration of all effects on the encounter map squares
                    ef.durationInUnits -= gv.mod.TimePerRound;

                    foreach (Player pc in mod.playerList)
                    {
                        if ((pc.combatLocX == ef.combatLocX) && (pc.combatLocY == ef.combatLocY))
                        {
                            if (!ef.usedForUpdateStats) //not used for stat updates
                            {
                                gv.cc.doEffectScript(pc, ef);
                            }
                        }
                    }
                    foreach (Creature crtr in mod.currentEncounter.encounterCreatureList)
                    {
                        if ((crtr.combatLocX == ef.combatLocX) && (crtr.combatLocY == ef.combatLocY))
                        {
                            if (!ef.usedForUpdateStats) //not used for stat updates
                            {
                                gv.cc.doEffectScript(crtr, ef);
                            }
                        }
                    }
                }

                //if remaining duration <= 0, remove from list
                foreach (Player pc in mod.playerList)
                {
                    for (int i = pc.effectsList.Count; i > 0; i--)
                    {
                        if (pc.effectsList[i - 1].durationInUnits <= 0)
                        {
                            pc.effectsList.RemoveAt(i - 1);
                        }
                    }
                }
                foreach (Creature crtr in mod.currentEncounter.encounterCreatureList)
                {
                    for (int i = crtr.cr_effectsList.Count; i > 0; i--)
                    {
                        if (crtr.cr_effectsList[i - 1].durationInUnits <= 0)
                        {
                            crtr.cr_effectsList.RemoveAt(i - 1);
                        }
                    }
                }                
                for (int i = mod.currentEncounter.effectsList.Count; i > 0; i--)
                {
                    if (mod.currentEncounter.effectsList[i - 1].durationInUnits <= 0)
                    {
                        mod.currentEncounter.effectsList.RemoveAt(i - 1);
                    }
                }
                
            }
            catch (Exception ex)
            {
                gv.sf.MessageBoxHtml(ex.ToString());
                gv.errorLog(ex.ToString());
            }
            checkEndEncounter();
        }
        public void afterEachMoveCalls()
        {
            triggerIndexCombat = 0;
            doPropTriggers();
        }
        public void doPropTriggers()
        {
            try
            {
                //reset the calling square loaction
                gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX = 0;
                gv.mod.currentEncounter.triggerScriptCalledFromSquareLocY = 0;

                Prop prp = gv.mod.currentEncounter.getPropByLocation(0, 0);
                if (isPlayerTurn)
                {
                    Player pc = mod.playerList[currentPlayerIndex];
                    prp = gv.mod.currentEncounter.getPropByLocation(pc.combatLocX, pc.combatLocY);
                    gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX = pc.combatLocX;
                    gv.mod.currentEncounter.triggerScriptCalledFromSquareLocY = pc.combatLocY;
                }
                else
                {
                    Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
                    prp = gv.mod.currentEncounter.getPropByLocation(crt.combatLocX, crt.combatLocY);
                    gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX = crt.combatLocX;
                    gv.mod.currentEncounter.triggerScriptCalledFromSquareLocY = crt.combatLocY;
                }
                if ((prp != null) && (prp.isActive))
                {
                    //check to see if using an IBScript
                    if (!prp.OnEnterSquareIBScript.Equals("none"))
                    {
                        if ((isPlayerTurn) && (prp.canBeTriggeredByPc)) //only do if PC can trigger
                        {
                            gv.cc.doIBScriptBasedOnFilename(prp.OnEnterSquareIBScript, prp.OnEnterSquareIBScriptParms);
                            decrementAndRemoveProp(prp);
                        }
                        else if ((!isPlayerTurn) && (prp.canBeTriggeredByCreature)) //only do if creature can trigger
                        {
                            gv.cc.doIBScriptBasedOnFilename(prp.OnEnterSquareIBScript, prp.OnEnterSquareIBScriptParms);
                            decrementAndRemoveProp(prp);
                        }
                    }
                    //check to see if using a Script
                    else if (!prp.OnEnterSquareScript.Equals("none"))
                    {
                        if ((isPlayerTurn) && (prp.canBeTriggeredByPc)) //only do if PC can trigger
                        {
                            gv.cc.doScriptBasedOnFilename(prp.OnEnterSquareScript, prp.OnEnterSquareScriptParm1, prp.OnEnterSquareScriptParm2, prp.OnEnterSquareScriptParm3, prp.OnEnterSquareScriptParm4);
                            decrementAndRemoveProp(prp);
                        }
                        else if ((!isPlayerTurn) && (prp.canBeTriggeredByCreature)) //only do if creature can trigger
                        {
                            gv.cc.doScriptBasedOnFilename(prp.OnEnterSquareScript, prp.OnEnterSquareScriptParm1, prp.OnEnterSquareScriptParm2, prp.OnEnterSquareScriptParm3, prp.OnEnterSquareScriptParm4);
                            decrementAndRemoveProp(prp);
                        }
                    }
                }
                doTriggers();
            }
            catch (Exception ex)
            {
                if (gv.mod.debugMode)
                {
                    gv.sf.MessageBox("failed to do prop trigger: " + ex.ToString());
                    gv.errorLog(ex.ToString());
                }
            }
        }
        public void decrementAndRemoveProp(Prop prp)
        {
            prp.numberOfScriptCallsRemaining--;
            if (prp.numberOfScriptCallsRemaining < 1)
            {
                gv.mod.currentEncounter.propsList.Remove(prp);
            }
        }
        public void decrementAndRemoveTrigger(Trigger trg)
        {
            trg.numberOfScriptCallsRemaining--;
            if (trg.numberOfScriptCallsRemaining < 1)
            {
                gv.mod.currentEncounter.Triggers.Remove(trg);
            }
        }
        public void doTriggers()
        {
            try
            {
                //reset the calling square loaction
                gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX = 0;
                gv.mod.currentEncounter.triggerScriptCalledFromSquareLocY = 0;

                Trigger trig = gv.mod.currentEncounter.getTriggerByLocation(0, 0);
                if (isPlayerTurn)
                {
                    Player pc = mod.playerList[currentPlayerIndex];
                    trig = gv.mod.currentEncounter.getTriggerByLocation(pc.combatLocX, pc.combatLocY);
                    gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX = pc.combatLocX;
                    gv.mod.currentEncounter.triggerScriptCalledFromSquareLocY = pc.combatLocY;
                }
                else
                {
                    Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
                    trig = gv.mod.currentEncounter.getTriggerByLocation(crt.combatLocX, crt.combatLocY);
                    gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX = crt.combatLocX;
                    gv.mod.currentEncounter.triggerScriptCalledFromSquareLocY = crt.combatLocY;
                }
                
                if ((trig != null) && (trig.Enabled))
                {
                    //iterate through each event                  
                    #region Event1 stuff
                    //check to see if enabled and parm not "none"                    
                    triggerIndexCombat++;

                    if ((triggerIndexCombat == 1) && (trig.EnabledEvent1) && (!trig.Event1FilenameOrTag.Equals("none")))
                    {
                        //check to see what type of event
                        if (trig.Event1Type.Equals("script"))
                        {
                            if ((isPlayerTurn) && (trig.canBeTriggeredByPc)) //only do if PC can trigger
                            {
                                gv.cc.doScriptBasedOnFilename(trig.Event1FilenameOrTag, trig.Event1Parm1, trig.Event1Parm2, trig.Event1Parm3, trig.Event1Parm4);
                                didTriggerEvent = true;
                            }
                            else if ((!isPlayerTurn) && (trig.canBeTriggeredByCreature)) //only do if creature can trigger
                            {
                                gv.cc.doScriptBasedOnFilename(trig.Event1FilenameOrTag, trig.Event1Parm1, trig.Event1Parm2, trig.Event1Parm3, trig.Event1Parm4);
                                didTriggerEvent = true;
                            }
                            //gv.cc.doScriptBasedOnFilename(trig.Event1FilenameOrTag, trig.Event1Parm1, trig.Event1Parm2, trig.Event1Parm3, trig.Event1Parm4);
                            doTriggers();
                        }
                        else if (trig.Event1Type.Equals("ibscript"))
                        {
                            if ((isPlayerTurn) && (trig.canBeTriggeredByPc)) //only do if PC can trigger
                            {
                                gv.cc.doIBScriptBasedOnFilename(trig.Event1FilenameOrTag, trig.Event1Parm1);
                                didTriggerEvent = true;
                            }
                            else if ((!isPlayerTurn) && (trig.canBeTriggeredByCreature)) //only do if creature can trigger
                            {
                                gv.cc.doIBScriptBasedOnFilename(trig.Event1FilenameOrTag, trig.Event1Parm1);
                                didTriggerEvent = true;
                            }
                            //gv.cc.doIBScriptBasedOnFilename(trig.Event1FilenameOrTag, trig.Event1Parm1);
                            doTriggers();
                        }
                        //do that event
                        if (trig.DoOnceOnlyEvent1)
                        {
                            trig.EnabledEvent1 = false;
                        }
                    }
                    #endregion
                    #region Event2 stuff
                    //check to see if enabled and parm not "none"
                    else if ((triggerIndexCombat == 2) && (trig.EnabledEvent2) && (!trig.Event2FilenameOrTag.Equals("none")))
                    {
                        //check to see what type of event
                        if (trig.Event2Type.Equals("script"))
                        {
                            if ((isPlayerTurn) && (trig.canBeTriggeredByPc)) //only do if PC can trigger
                            {
                                gv.cc.doScriptBasedOnFilename(trig.Event2FilenameOrTag, trig.Event2Parm1, trig.Event2Parm2, trig.Event2Parm3, trig.Event2Parm4);
                                didTriggerEvent = true;
                            }
                            else if ((!isPlayerTurn) && (trig.canBeTriggeredByCreature)) //only do if creature can trigger
                            {
                                gv.cc.doScriptBasedOnFilename(trig.Event2FilenameOrTag, trig.Event2Parm1, trig.Event2Parm2, trig.Event2Parm3, trig.Event2Parm4);
                                didTriggerEvent = true;
                            }
                            doTriggers();
                        }
                        else if (trig.Event2Type.Equals("ibscript"))
                        {
                            if ((isPlayerTurn) && (trig.canBeTriggeredByPc)) //only do if PC can trigger
                            {
                                gv.cc.doIBScriptBasedOnFilename(trig.Event2FilenameOrTag, trig.Event2Parm1);
                                didTriggerEvent = true;
                            }
                            else if ((!isPlayerTurn) && (trig.canBeTriggeredByCreature)) //only do if creature can trigger
                            {
                                gv.cc.doIBScriptBasedOnFilename(trig.Event2FilenameOrTag, trig.Event2Parm1);
                                didTriggerEvent = true;
                            }
                            doTriggers();
                        }
                        //do that event
                        if (trig.DoOnceOnlyEvent2)
                        {
                            trig.EnabledEvent2 = false;
                        }
                    }
                    #endregion
                    #region Event3 stuff
                    //check to see if enabled and parm not "none"
                    else if ((triggerIndexCombat == 3) && (trig.EnabledEvent3) && (!trig.Event3FilenameOrTag.Equals("none")))
                    {
                        //check to see what type of event
                        if (trig.Event3Type.Equals("script"))
                        {
                            if ((isPlayerTurn) && (trig.canBeTriggeredByPc)) //only do if PC can trigger
                            {
                                gv.cc.doScriptBasedOnFilename(trig.Event3FilenameOrTag, trig.Event3Parm1, trig.Event3Parm2, trig.Event3Parm3, trig.Event3Parm4);
                                didTriggerEvent = true;
                            }
                            else if ((!isPlayerTurn) && (trig.canBeTriggeredByCreature)) //only do if creature can trigger
                            {
                                gv.cc.doScriptBasedOnFilename(trig.Event3FilenameOrTag, trig.Event3Parm1, trig.Event3Parm2, trig.Event3Parm3, trig.Event3Parm4);
                                didTriggerEvent = true;
                            }
                            doTriggers();
                        }
                        else if (trig.Event3Type.Equals("ibscript"))
                        {
                            if ((isPlayerTurn) && (trig.canBeTriggeredByPc)) //only do if PC can trigger
                            {
                                gv.cc.doIBScriptBasedOnFilename(trig.Event3FilenameOrTag, trig.Event3Parm1);
                                didTriggerEvent = true;
                            }
                            else if ((!isPlayerTurn) && (trig.canBeTriggeredByCreature)) //only do if creature can trigger
                            {
                                gv.cc.doIBScriptBasedOnFilename(trig.Event3FilenameOrTag, trig.Event3Parm1);
                                didTriggerEvent = true;
                            }
                            doTriggers();
                        }
                        //do that event
                        if (trig.DoOnceOnlyEvent3)
                        {
                            trig.EnabledEvent3 = false;
                        }
                    }
                    else if (triggerIndexCombat < 4)
                    {
                        doTriggers();
                    }
                    #endregion
                    if (triggerIndexCombat > 3)
                    {
                        if (didTriggerEvent)
                        {
                            decrementAndRemoveTrigger(trig);
                        }
                        triggerIndexCombat = 0;
                        didTriggerEvent = false;
                        if (trig.DoOnceOnly)
                        {
                            trig.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (gv.mod.debugMode)
                {
                    gv.sf.MessageBox("failed to do trigger: " + ex.ToString());
                    gv.errorLog(ex.ToString());
                }
            }
        }
        public void doPropOrTriggerCastSpell(string tag)
        {
            Spell sp = gv.mod.getSpellByTag(tag);
            if (sp == null) { return; }
            Coordinate srcCoor = new Coordinate(gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX, gv.mod.currentEncounter.triggerScriptCalledFromSquareLocY);
            //if spell target type is coor, use coor...else use creature or PC on square
            if (sp.spellTargetType.Equals("PointLocation"))
            {
                gv.cc.doSpellBasedOnScriptOrEffectTag(sp, srcCoor, srcCoor, false);
            }
            else
            {
                foreach (Creature crt in gv.mod.currentEncounter.encounterCreatureList)
                {
                    if ((crt.combatLocX == gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX) && (crt.combatLocY == gv.mod.currentEncounter.triggerScriptCalledFromSquareLocY))
                    {
                        gv.cc.doSpellBasedOnScriptOrEffectTag(sp, srcCoor, crt, false);
                    }
                }
                foreach (Player pc in gv.mod.playerList)
                {
                    if ((pc.combatLocX == gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX) && (pc.combatLocY == gv.mod.currentEncounter.triggerScriptCalledFromSquareLocY))
                    {
                        gv.cc.doSpellBasedOnScriptOrEffectTag(sp, srcCoor, pc, false);
                    }
                }
            }
            
            //add ending animation
            string filename = sp.spriteEndingFilename;
            AnimationSequence newSeq = new AnimationSequence();
            animationSeqStack.Add(newSeq);
            AnimationStackGroup newGroup = new AnimationStackGroup();
            animationSeqStack[0].AnimationSeq.Add(newGroup);
            foreach (Coordinate coor in gv.sf.AoeSquaresList)
            {
                addEndingAnimation(newGroup, new Coordinate(getPixelLocX(coor.X), getPixelLocY(coor.Y)), filename);
            }
            //add floaty text
            //add death animations
            newGroup = new AnimationStackGroup();
            animationSeqStack[0].AnimationSeq.Add(newGroup);
            foreach (Coordinate coor in deathAnimationLocations)
            {
                addDeathAnimation(newGroup, new Coordinate(getPixelLocX(coor.X), getPixelLocY(coor.Y)));
            }
            animationsOn = true;
        }
        
        //COMBAT	
        #region PC Combat Stuff
        public void decrementAmmo(Player pc)
        {
            if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged"))
                    && (!mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
            {
                ItemRefs itr = mod.getItemRefsInInventoryByResRef(pc.AmmoRefs.resref);
                if (itr != null)
                {
                    //decrement by one
                    itr.quantity--;
                    if (gv.sf.hasTrait(pc, "rapidshot"))
                    {
                        itr.quantity--;
                    }
                    if (gv.sf.hasTrait(pc, "rapidshot2"))
                    {
                        itr.quantity--;
                    }
                    //if equal to zero, remove from party inventory and from all PCs ammo slot
                    if (itr.quantity < 1)
                    {
                        foreach (Player p in mod.playerList)
                        {
                            if (p.AmmoRefs.resref.Equals(itr.resref))
                            {
                                p.AmmoRefs = new ItemRefs();
                            }
                        }
                        mod.partyInventoryRefsList.Remove(itr);
                    }
                }
            }
        }
        public void startPcTurn()
        {
            //CalculateUpperLeft();
            gv.Render();
            isPlayerTurn = true;
            gv.touchEnabled = true;
            currentCombatMode = "move";
            Player pc = mod.playerList[currentPlayerIndex];
            gv.sf.UpdateStats(pc);
            currentMoves = 0;
            //do onTurn IBScript
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnStartCombatTurnIBScript, gv.mod.currentEncounter.OnStartCombatTurnIBScriptParms);

            if ((pc.isHeld()) || (pc.isDead()) || (pc.isUnconcious()))
            {
                endPcTurn(false);
            }
            if (pc.isImmobile())
            {
                currentMoves = 99;
            }
        }
        public void doCombatAttack(Player pc)
        {
            if (isInRange(pc))
            {

                Item itChk = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                if (itChk != null)
                {
                    if (itChk.automaticallyHitsTarget) //if AoE type attack and automatically hits
                    {
                        //if using ranged and have ammo, use ammo properties
                        if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged"))
                        && (!mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
                        {
                            itChk = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                            if (itChk != null)
                            {
                                //always decrement ammo by one whether a hit or miss
                                this.decrementAmmo(pc);

                                if (!itChk.onScoringHitCastSpellTag.Equals("none"))
                                {
                                    doItemOnHitCastSpell(itChk.onScoringHitCastSpellTag, itChk, targetHighlightCenterLocation);
                                }
                            }
                        }
                        else if (!itChk.onScoringHitCastSpellTag.Equals("none"))
                        {
                            doItemOnHitCastSpell(itChk.onScoringHitCastSpellTag, itChk, targetHighlightCenterLocation);
                        }

                        hitAnimationLocation = new Coordinate(getPixelLocX(targetHighlightCenterLocation.X), getPixelLocY(targetHighlightCenterLocation.Y));

                        //new system
                        AnimationStackGroup newGroup = new AnimationStackGroup();
                        animationSeqStack[0].AnimationSeq.Add(newGroup);
                        addHitAnimation(newGroup);
                        return;
                    }
                }


                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    foreach (Coordinate coor in crt.tokenCoveredSquares)
                    {
                        if ((coor.X == targetHighlightCenterLocation.X) && (coor.Y == targetHighlightCenterLocation.Y))
                        {
                            int attResult = 0; //0=missed, 1=hit, 2=killed
                            int numAtt = 1;
                            int crtLocX = crt.combatLocX;
                            int crtLocY = crt.combatLocY;

                            if ((gv.sf.hasTrait(pc, "twoAttack")) && (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee")))
                            {
                                numAtt = 2;
                            }
                            if ((gv.sf.hasTrait(pc, "rapidshot")) && (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged")))
                            {
                                numAtt = 2;
                            }
                            if ((gv.sf.hasTrait(pc, "rapidshot2")) && (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged")))
                            {
                                numAtt = 3;
                            }
                            for (int i = 0; i < numAtt; i++)
                            {
                                if ((gv.sf.hasTrait(pc, "cleave")) && (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee")))
                                {
                                    attResult = doActualCombatAttack(pc, crt, i);
                                    if (attResult == 2) //2=killed, 1=hit, 0=missed
                                    {
                                        Creature crt2 = GetNextAdjacentCreature(pc);
                                        if (crt2 != null)
                                        {
                                            crtLocX = crt2.combatLocX;
                                            crtLocY = crt2.combatLocY;
                                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), "cleave", "green");
                                            attResult = doActualCombatAttack(pc, crt2, i);
                                        }
                                        break; //do not try and attack same creature that was just killed
                                    }
                                }
                                else
                                {
                                    attResult = doActualCombatAttack(pc, crt, i);
                                    if (attResult == 2) //2=killed, 1=hit, 0=missed
                                    {
                                        break; //do not try and attack same creature that was just killed
                                    }
                                }

                            }
                            if (attResult > 0) //2=killed, 1=hit, 0=missed
                            {
                                hitAnimationLocation = new Coordinate(getPixelLocX(crtLocX), getPixelLocY(crtLocY));
                                //new system
                                AnimationStackGroup newGroup = new AnimationStackGroup();
                                animationSeqStack[0].AnimationSeq.Add(newGroup);
                                addHitAnimation(newGroup);
                            }
                            else
                            {
                                hitAnimationLocation = new Coordinate(getPixelLocX(crtLocX), getPixelLocY(crtLocY));
                                //new system
                                AnimationStackGroup newGroup = new AnimationStackGroup();
                                animationSeqStack[0].AnimationSeq.Add(newGroup);
                                addMissAnimation(newGroup);
                            }
                            return;
                        }
                    }
                }
            }
        }
        public int doActualCombatAttack(Player pc, Creature crt, int attackNumber)
        {
            //always decrement ammo by one whether a hit or miss
            this.decrementAmmo(pc);

            int attackRoll = gv.sf.RandInt(20);
            int attackMod = CalcPcAttackModifier(pc, crt);
            int attack = attackRoll + attackMod;
            int defense = CalcCreatureDefense(pc, crt);
            int damage = CalcPcDamageToCreature(pc, crt);

            bool automaticallyHits = false;
            Item itChk = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
            if (itChk != null)
            {
                automaticallyHits = itChk.automaticallyHitsTarget;
            }
            //natural 20 always hits
            if ((attack >= defense) || (attackRoll == 20) || (automaticallyHits == true)) //HIT
            {
                crt.hp = crt.hp - damage;
                gv.cc.addLogText("<bu>" + pc.name + "</bu><br>");
                gv.cc.addLogText("<wh>attacks </wh><br>");
                gv.cc.addLogText("<gy>" + crt.cr_name + "</gy><br>");
                gv.cc.addLogText("<gn>HITS (-" + damage + "hp)</gn><br>");
                gv.cc.addLogText("<wh>" + attackRoll + "+" + attackMod + ">=" + defense + "</wh><BR>");

                Item it = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                if (it != null)
                {
                    //doOnHitScriptBasedOnFilename(it.onScoringHit, crt, pc);
                    if (!it.onScoringHitCastSpellTag.Equals("none"))
                    {
                        doItemOnHitCastSpell(it.onScoringHitCastSpellTag, it, crt);
                    }
                }

                it = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                if (it != null)
                {
                    //doOnHitScriptBasedOnFilename(it.onScoringHit, crt, pc);
                    if (!it.onScoringHitCastSpellTag.Equals("none"))
                    {
                        doItemOnHitCastSpell(it.onScoringHitCastSpellTag, it, crt);
                    }
                }

                //play attack sound for melee (not ranged)
                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee"))
                {
                    gv.PlaySound(mod.getItemByResRefForInfo(pc.MainHandRefs.resref).itemOnUseSound);
                }

                //Draw floaty text showing damage above Creature
                int txtH = (int)gv.fontHeight;
                int shiftUp = 0 - (attackNumber * txtH);
                gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), damage + "", shiftUp);

                if (crt.hp <= 0)
                {
                    foreach (Coordinate coor in crt.tokenCoveredSquares)
                    {
                        deathAnimationLocations.Add(new Coordinate(coor.X, coor.Y));
                    }
                    gv.cc.addLogText("<gn>You killed the " + crt.cr_name + "</gn><BR>");
                    return 2; //killed
                }
                else
                {
                    return 1; //hit
                }
            }
            else //MISSED
            {
                //play attack sound for melee (not ranged)
                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee"))
                {
                    gv.PlaySound(mod.getItemByResRefForInfo(pc.MainHandRefs.resref).itemOnUseSound);
                }
                gv.cc.addLogText("<bu>" + pc.name + "</bu><br>");
                gv.cc.addLogText("<wh>attacks </wh><br>");
                gv.cc.addLogText("<gy>" + crt.cr_name + "</gy><br>");
                gv.cc.addLogText("<wh>MISSES</wh><br>");
                gv.cc.addLogText("<wh>" + attackRoll + "+" + attackMod + " < " + defense + "</wh><BR>");
                return 0; //missed
            }
        }
        public void doItemOnHitCastSpell(string tag, Item it, object trg)
        {
            Spell sp = gv.mod.getSpellByTag(tag);
            if (sp == null) { return; }
            gv.cc.doSpellBasedOnScriptOrEffectTag(sp, it, trg, false);
        }
        public void endPcTurn(bool endStealthMode)
        {
            //gv.Render();
            //remove stealth if endStealthMode = true		
            Player pc = mod.playerList[currentPlayerIndex];
            if (endStealthMode)
            {
                pc.steathModeOn = false;
            }
            else //else test to see if enter/stay in stealth mode if has trait
            {
                doStealthModeCheck(pc);
            }
            canMove = true;
            turnController();
        }
        public void doStealthModeCheck(Player pc)
        {
            int skillMod = 0;
            if (pc.knownTraitsTags.Contains("stealth4"))
            {
                Trait tr = mod.getTraitByTag("stealth4");
                skillMod = tr.skillModifier;
            }
            else if (pc.knownTraitsTags.Contains("stealth3"))
            {
                Trait tr = mod.getTraitByTag("stealth3");
                skillMod = tr.skillModifier;
            }
            else if (pc.knownTraitsTags.Contains("stealth2"))
            {
                Trait tr = mod.getTraitByTag("stealth2");
                skillMod = tr.skillModifier;
            }
            else if (pc.knownTraitsTags.Contains("stealth"))
            {
                Trait tr = mod.getTraitByTag("stealth");
                skillMod = tr.skillModifier;
            }
            else
            {
                //PC doesn't have stealth trait
                pc.steathModeOn = false;
                return;
            }
            int attMod = (pc.dexterity - 10) / 2;
            int roll = gv.sf.RandInt(20);
            int DC = 18; //eventually change to include area modifiers, proximity to enemies, etc.
            if (roll + attMod + skillMod >= DC)
            {
                pc.steathModeOn = true;
                gv.cc.addLogText("<gn> stealth ON: " + roll + "+" + attMod + "+" + skillMod + ">=" + DC + "</gn><BR>");
            }
            else
            {
                pc.steathModeOn = false;
                gv.cc.addLogText("<gn> stealth OFF: " + roll + "+" + attMod + "+" + skillMod + "<" + DC + "</gn><BR>");
            }
        }
        public void doPlayerCombatFacing(Player pc, int tarX, int tarY)
        {
            if ((tarX == pc.combatLocX) && (tarY > pc.combatLocY)) { pc.combatFacing = 2; }
            if ((tarX > pc.combatLocX) && (tarY > pc.combatLocY)) { pc.combatFacing = 3; }
            if ((tarX < pc.combatLocX) && (tarY > pc.combatLocY)) { pc.combatFacing = 1; }
            if ((tarX == pc.combatLocX) && (tarY < pc.combatLocY)) { pc.combatFacing = 8; }
            if ((tarX > pc.combatLocX) && (tarY < pc.combatLocY)) { pc.combatFacing = 9; }
            if ((tarX < pc.combatLocX) && (tarY < pc.combatLocY)) { pc.combatFacing = 7; }
            if ((tarX > pc.combatLocX) && (tarY == pc.combatLocY)) { pc.combatFacing = 6; }
            if ((tarX < pc.combatLocX) && (tarY == pc.combatLocY)) { pc.combatFacing = 4; }
        }
        #endregion

        #region Creature Combat Stuff
        public void doCreatureTurn()
        {
            canMove = true;
            Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
            //do onStartTurn IBScript
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnStartCombatTurnIBScript, gv.mod.currentEncounter.OnStartCombatTurnIBScriptParms);
            creatureMoves = 0;
            doCreatureNextAction();
        }
        public void doCreatureNextAction()
        {
            Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
            //CalculateUpperLeftCreature();
            if ((crt.hp > 0) && (!crt.isHeld()))
            {
                creatureToAnimate = null;
                playerToAnimate = null;
                //gv.Render();
                animationState = AnimationState.CreatureThink;
                gv.postDelayed("doAnimation", (int)(2.5f * mod.combatAnimationSpeed));
            }
            else
            {
                endCreatureTurn();
            }
        }
        public void doCreatureTurnAfterDelay()
        {
            Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];

            gv.sf.ActionToTake = null;
            gv.sf.SpellToCast = null;

            if (crt.isImmobile())
            {
                creatureMoves = 99;
            }

            //determine the action to take
            doCreatureAI(crt);

            //do the action (melee/ranged, cast spell, use trait, etc.)
            if (gv.sf.ActionToTake == null)
            {
                endCreatureTurn();
            }
            if (gv.sf.ActionToTake.Equals("Attack"))
            {
                Player pc = targetClosestPC(crt);
                gv.sf.CombatTarget = pc;
                CreatureDoesAttack(crt);
            }
            else if (gv.sf.ActionToTake.Equals("Move"))
            {
                if ((creatureMoves + 0.5f) < crt.moveDistance)
                {
                    CreatureMoves();
                }
                else
                {
                    endCreatureTurn();
                }
            }
            else if (gv.sf.ActionToTake.Equals("Cast"))
            {
                if ((gv.sf.SpellToCast != null) && (gv.sf.CombatTarget != null))
                {
                    CreatureCastsSpell(crt);
                }
            }
        }
        public void CreatureMoves()
        {
            Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
            if (creatureMoves + 0.5f < crt.moveDistance)
            {
                Player pc = targetClosestPC(crt);
                //run pathFinder to get new location
                if (pc != null)
                {
                    pf.resetGrid(crt);
                    Coordinate newCoor = pf.findNewPoint(crt, new Coordinate(pc.combatLocX, pc.combatLocY));
                    if ((newCoor.X == -1) && (newCoor.Y == -1))
                    {
                        //didn't find a path, try other PCs
                        bool foundOne = false;
                        //try each PC
                        for (int d = 0; d < gv.mod.playerList.Count; d++)
                        {
                            if ((gv.mod.playerList[d].isAlive()) && (!gv.mod.playerList[d].steathModeOn) && (!gv.mod.playerList[d].isInvisible()))
                            {
                                pf.resetGrid(crt);
                                newCoor = pf.findNewPoint(crt, new Coordinate(gv.mod.playerList[d].combatLocX, gv.mod.playerList[d].combatLocY));
                                if ((newCoor.X == -1) && (newCoor.Y == -1))
                                {
                                    //didn't find a path so keep searching
                                }
                                else
                                {
                                    if (gv.mod.debugMode)
                                    {
                                        gv.cc.addLogText("<yl>player " + d + ":" + newCoor.X + "," + newCoor.Y + "</yl><BR>");
                                    }
                                    //found a path so break
                                    foundOne = true;
                                    break;
                                }
                            }
                        }
                        if (!foundOne)
                        {
                            //try around the nearest PC
                            int closestDist = 999;
                            for (int j = 1; j < 5; j++) //used for radius around PC
                            {
                                for (int x = -j; x <= j; x++)
                                {
                                    for (int y = -j; y <= j; y++)
                                    {
                                        if (isSquareOnCombatMap(pc.combatLocX + x, pc.combatLocY + y))
                                        {
                                            pf.resetGrid(crt);
                                            Coordinate testCoor = pf.findNewPoint(crt, new Coordinate(pc.combatLocX + x, pc.combatLocY + y));
                                            if ((testCoor.X == -1) && (testCoor.Y == -1))
                                            {
                                                //didn't find a path so keep searching
                                            }
                                            else
                                            {
                                                //found a path so check if closer distance
                                                int dist = getDistance(new Coordinate(pc.combatLocX + x, pc.combatLocY + y), new Coordinate(crt.combatLocX, crt.combatLocY));
                                                if (dist < closestDist)
                                                {
                                                    closestDist = dist;
                                                    newCoor.X = testCoor.X;
                                                    newCoor.Y = testCoor.Y;
                                                    foundOne = true;
                                                    if (gv.mod.debugMode)
                                                    {
                                                        gv.cc.addLogText("<yl>dist: " + dist + " coor:" + newCoor.X + "," + newCoor.Y + "</yl><BR>");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!foundOne)
                        {
                            //give up and end
                            gv.Render();
                            endCreatureTurn();
                            return;
                        }                        
                    }
                    if (gv.mod.debugMode)
                    {
                        gv.cc.addLogText("<yl>newCoor:" + newCoor.X + "," + newCoor.Y + "</yl><BR>");
                    }
                    //it's a diagonal move
                    if ((crt.combatLocX != newCoor.X) && (crt.combatLocY != newCoor.Y))
                    {
                        //enough  move points availbale to do the diagonal move
                        if ((crt.moveDistance - creatureMoves) >= mod.diagonalMoveCost)
                        {
                            if ((newCoor.X < crt.combatLocX) && (!crt.combatFacingLeft)) //move left
                            {
                                crt.combatFacingLeft = true;
                            }
                            else if ((newCoor.X > crt.combatLocX) && (crt.combatFacingLeft)) //move right
                            {
                                crt.combatFacingLeft = false;
                            }
                            //CHANGE FACING BASED ON MOVE
                            doCreatureCombatFacing(crt, newCoor.X, newCoor.Y);
                            moveCost = mod.diagonalMoveCost;
                            crt.combatLocX = newCoor.X;
                            crt.combatLocY = newCoor.Y;
                            canMove = false;
                            animationState = AnimationState.CreatureMove;
                            gv.postDelayed("doAnimation", (int)(1f * mod.combatAnimationSpeed));

                        }

                        //try to move horizontally or vertically instead if most points are not enough for diagonal move
                        else if ((crt.moveDistance - creatureMoves) >= 1)
                        {
                            //don't move horizontally/vertically, just give up
                            gv.Render();
                            endCreatureTurn();
                            return;
                            
                            pf.resetGrid(crt);
                            //block the originial diagonal target square and calculate again
                            newCoor = pf.findNewPoint(crt, new Coordinate(pc.combatLocX, pc.combatLocY));
                            if ((newCoor.X == -1) && (newCoor.Y == -1))
                            {
                                //didn't find a path, don't move
                                gv.Render();
                                endCreatureTurn();
                                return;
                            }
                            if ((newCoor.X < crt.combatLocX) && (!crt.combatFacingLeft)) //move left
                            {
                                crt.combatFacingLeft = true;
                            }
                            else if ((newCoor.X > crt.combatLocX) && (crt.combatFacingLeft)) //move right
                            {
                                crt.combatFacingLeft = false;
                            }
                            //CHANGE FACING BASED ON MOVE
                            doCreatureCombatFacing(crt, newCoor.X, newCoor.Y);
                            moveCost = 1;
                            crt.combatLocX = newCoor.X;
                            crt.combatLocY = newCoor.Y;
                            canMove = false;
                            animationState = AnimationState.CreatureMove;
                            gv.postDelayed("doAnimation", (int)(1f * mod.combatAnimationSpeed));
                            
                        }
                        //less than one move point, no move
                        else
                        {
                            canMove = false;
                            animationState = AnimationState.CreatureMove;
                            gv.postDelayed("doAnimation", (int)(1f * mod.combatAnimationSpeed));

                        }
                    }
                    //it's a horizontal or vertical move
                    else
                    {
                        if ((newCoor.X < crt.combatLocX) && (!crt.combatFacingLeft)) //move left
                        {
                            crt.combatFacingLeft = true;
                        }
                        else if ((newCoor.X > crt.combatLocX) && (crt.combatFacingLeft)) //move right
                        {
                            crt.combatFacingLeft = false;
                        }
                        //CHANGE FACING BASED ON MOVE
                        doCreatureCombatFacing(crt, newCoor.X, newCoor.Y);
                        crt.combatLocX = newCoor.X;
                        crt.combatLocY = newCoor.Y;
                        canMove = false;
                        animationState = AnimationState.CreatureMove;
                        gv.postDelayed("doAnimation", (int)(1f * mod.combatAnimationSpeed));

                    }
                }
                else //no target found
                {
                    gv.Render();
                    endCreatureTurn();
                    return;
                }
            }
            //less than a move point left, no move
            else
            {
                gv.Render();
                endCreatureTurn();
                return;
            }
        }
        public void CreatureDoesAttack(Creature crt)
        {
            if (gv.sf.CombatTarget != null)
            {
                Player pc = (Player)gv.sf.CombatTarget;
                //Uses Map Pixel Locations
                int endX = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
                int endY = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);
                int startX = crt.combatLocX * gv.squareSize + (gv.squareSize / 2);
                int startY = crt.combatLocY * gv.squareSize + (gv.squareSize / 2);
                // determine if ranged or melee
                if ((crt.cr_category.Equals("Ranged"))
                        && (CalcDistance(crt, crt.combatLocX, crt.combatLocY, pc.combatLocX, pc.combatLocY) <= crt.cr_attRange)
                        && (isVisibleLineOfSight(new Coordinate(endX, endY), new Coordinate(startX, startY))))
                {
                    //play attack sound for ranged
                    gv.PlaySound(crt.cr_attackSound);
                    if ((pc.combatLocX < crt.combatLocX) && (!crt.combatFacingLeft)) //attack left
                    {
                        crt.combatFacingLeft = true;
                    }
                    else if ((pc.combatLocX > crt.combatLocX) && (crt.combatFacingLeft)) //attack right
                    {
                        crt.combatFacingLeft = false;
                    }
                    //CHANGE FACING BASED ON ATTACK
                    doCreatureCombatFacing(crt, pc.combatLocX, pc.combatLocY);

                    if (crt.hp > 0)
                    {
                        creatureToAnimate = crt;
                        playerToAnimate = null;
                        creatureTargetLocation = new Coordinate(pc.combatLocX, pc.combatLocY);
                        //set attack animation and do a delay
                        attackAnimationTimeElapsed = 0;
                        attackAnimationLengthInMilliseconds = (int)(5f * mod.combatAnimationSpeed);
                        //add projectile animation
                        startX = getPixelLocX(crt.combatLocX);
                        startY = getPixelLocY(crt.combatLocY);
                        endX = getPixelLocX(pc.combatLocX);
                        endY = getPixelLocY(pc.combatLocY);
                        string filename = crt.cr_projSpriteFilename;
                        AnimationSequence newSeq = new AnimationSequence();
                        animationSeqStack.Add(newSeq);
                        AnimationStackGroup newGroup = new AnimationStackGroup();
                        newSeq.AnimationSeq.Add(newGroup);
                        launchProjectile(filename, startX, startY, endX, endY, newGroup);
                        //add ending projectile animation  
                        doStandardCreatureAttack();
                        //add hit or miss animation
                        //add floaty text
                        //add death animations
                        newGroup = new AnimationStackGroup();
                        animationSeqStack[0].AnimationSeq.Add(newGroup);
                        foreach (Coordinate coor in deathAnimationLocations)
                        {
                            addDeathAnimation(newGroup, new Coordinate(getPixelLocX(coor.X), getPixelLocY(coor.Y)));
                        }
                        animationsOn = true;
                    }
                    else
                    {
                        //skip this guys turn
                    }
                }
                else if ((crt.cr_category.Equals("Melee"))
                        && (CalcDistance(crt, crt.combatLocX, crt.combatLocY, pc.combatLocX, pc.combatLocY) <= crt.cr_attRange))
                {
                    if ((pc.combatLocX < crt.combatLocX) && (!crt.combatFacingLeft)) //attack left
                    {
                        crt.combatFacingLeft = true;
                    }
                    else if ((pc.combatLocX > crt.combatLocX) && (crt.combatFacingLeft)) //attack right
                    {
                        crt.combatFacingLeft = false;
                    }
                    //CHANGE FACING BASED ON ATTACK
                    doCreatureCombatFacing(crt, pc.combatLocX, pc.combatLocY);
                    if (crt.hp > 0)
                    {
                        creatureToAnimate = crt;
                        playerToAnimate = null;
                        //do melee attack stuff and animations  
                        AnimationSequence newSeq = new AnimationSequence();
                        animationSeqStack.Add(newSeq);
                        doStandardCreatureAttack();
                        //add hit or miss animation
                        //add floaty text
                        //add death animations
                        AnimationStackGroup newGroup = new AnimationStackGroup();
                        animationSeqStack[0].AnimationSeq.Add(newGroup);
                        foreach (Coordinate coor in deathAnimationLocations)
                        {
                            addDeathAnimation(newGroup, new Coordinate(getPixelLocX(coor.X), getPixelLocY(coor.Y)));
                        }
                        animationsOn = true;
                    }
                    else
                    {
                        //skip this guys turn
                    }
                }
                else //not in range for attack so MOVE
                {
                    CreatureMoves();
                }
            }
            else //no target so move instead
            {
                CreatureMoves();
            }
        }
        public void CreatureCastsSpell(Creature crt)
        {
            Coordinate pnt = new Coordinate();
            if (gv.sf.CombatTarget is Player)
            {
                Player pc = (Player)gv.sf.CombatTarget;
                pnt = new Coordinate(pc.combatLocX, pc.combatLocY);
            }
            else if (gv.sf.CombatTarget is Creature)
            {
                Creature crtTarget = (Creature)gv.sf.CombatTarget;
                pnt = new Coordinate(crtTarget.combatLocX, crtTarget.combatLocY);
            }
            else if (gv.sf.CombatTarget is Coordinate)
            {
                pnt = (Coordinate)gv.sf.CombatTarget;
            }
            else //do not understand, what is the target
            {
                return;
            }
            //Using Map Pixel Locations
            int endX = pnt.X * gv.squareSize + (gv.squareSize / 2);
            int endY = pnt.Y * gv.squareSize + (gv.squareSize / 2);
            int startX = crt.combatLocX * gv.squareSize + (gv.squareSize / 2);
            int startY = crt.combatLocY * gv.squareSize + (gv.squareSize / 2);
            if ((getDistance(pnt, new Coordinate(crt.combatLocX, crt.combatLocY)) <= gv.sf.SpellToCast.range)
                    && (isVisibleLineOfSight(new Coordinate(endX, endY), new Coordinate(startX, startY))))
            {

                if ((pnt.X < crt.combatLocX) && (!crt.combatFacingLeft)) //attack left
                {
                    crt.combatFacingLeft = true;
                }
                else if ((pnt.X > crt.combatLocX) && (crt.combatFacingLeft)) //attack right
                {
                    crt.combatFacingLeft = false;
                }
                //CHANGE FACING BASED ON ATTACK
                doCreatureCombatFacing(crt, pnt.X, pnt.Y);
                creatureTargetLocation = pnt;
                creatureToAnimate = crt;
                playerToAnimate = null;

                //set attack animation and do a delay
                attackAnimationTimeElapsed = 0;
                attackAnimationLengthInMilliseconds = (int)(5f * mod.combatAnimationSpeed);
                AnimationSequence newSeq = new AnimationSequence();
                animationSeqStack.Add(newSeq);
                //add projectile animation
                gv.PlaySound(gv.sf.SpellToCast.spellStartSound);
                startX = getPixelLocX(crt.combatLocX);
                startY = getPixelLocY(crt.combatLocY);
                endX = getPixelLocX(creatureTargetLocation.X);
                endY = getPixelLocY(creatureTargetLocation.Y);
                string filename = gv.sf.SpellToCast.spriteFilename;
                AnimationStackGroup newGroup = new AnimationStackGroup();
                newSeq.AnimationSeq.Add(newGroup);
                launchProjectile(filename, startX, startY, endX, endY, newGroup);
                //gv.PlaySound(gv.sf.SpellToCast.spellEndSound);
                gv.cc.doSpellBasedOnScriptOrEffectTag(gv.sf.SpellToCast, crt, gv.sf.CombatTarget, false);
                //add ending projectile animation
                newGroup = new AnimationStackGroup();
                animationSeqStack[0].AnimationSeq.Add(newGroup);
                filename = gv.sf.SpellToCast.spriteEndingFilename;
                foreach (Coordinate coor in gv.sf.AoeSquaresList)
                {
                    addEndingAnimation(newGroup, new Coordinate(getPixelLocX(coor.X), getPixelLocY(coor.Y)), filename);
                }
                //add floaty text
                //add death animations
                newGroup = new AnimationStackGroup();
                animationSeqStack[0].AnimationSeq.Add(newGroup);
                foreach (Coordinate coor in deathAnimationLocations)
                {
                    addDeathAnimation(newGroup, new Coordinate(getPixelLocX(coor.X), getPixelLocY(coor.Y)));
                }
                animationsOn = true;
            }
            else
            {
                //#region Do a Melee or Ranged Attack
                Player pc = targetClosestPC(crt);
                gv.sf.CombatTarget = pc;
                CreatureDoesAttack(crt);
            }
        }
        public void doCreatureAI(Creature crt)
        {
            //These are the current generic AI types
            //BasicAttacker:          basic attack (ranged or melee)
            //Healer:                 heal Friend(s) until out of SP
            //BattleHealer:           heal Friend(s) and/or attack
            //DamageCaster:           cast damage spells
            //BattleDamageCaster:     cast damage spells and/or attack
            //DebuffCaster:           cast debuff spells
            //BattleDebuffCaster:     cast debuff spells and/or attack
            //GeneralCaster:          cast any of their known spells by random
            //BattleGeneralCaster:    cast any of their known spells by random and/or attack

            if (crt.cr_ai.Equals("BasicAttacker"))
            {
                if (gv.mod.debugMode)
                {
                    //gv.cc.addLogText("<yl>" + crt.cr_name + " is a BasicAttacker</yl><BR>");
                }
                BasicAttacker(crt);
            }
            else if (crt.cr_ai.Equals("GeneralCaster"))
            {
                if (gv.mod.debugMode)
                {
                    //gv.cc.addLogText("<yl>" + crt.cr_name + " is a GeneralCaster</yl><BR>");
                }
                GeneralCaster(crt);
            }
            else
            {
                if (gv.mod.debugMode)
                {
                    //gv.cc.addLogText("<yl>" + crt.cr_name + " is a BasicAttacker</yl><BR>");
                }
                BasicAttacker(crt);
            }
        }
        public void BasicAttacker(Creature crt)
        {
            Player pc = targetClosestPC(crt);
            gv.sf.CombatTarget = pc;
            gv.sf.ActionToTake = "Attack";
        }
        public void GeneralCaster(Creature crt)
        {
            gv.sf.SpellToCast = null;
            //just pick a random spell from KnownSpells
            //try a few times to pick a random spell that has enough SP
            for (int i = 0; i < 10; i++)
            {
                int rnd = gv.sf.RandInt(crt.knownSpellsTags.Count);
                Spell sp = mod.getSpellByTag(crt.knownSpellsTags[rnd - 1]);
                if (sp != null)
                {
                    if (sp.costSP <= crt.sp)
                    {
                        gv.sf.SpellToCast = sp;

                        if (gv.sf.SpellToCast.spellTargetType.Equals("Enemy"))
                        {
                            Player pc = targetClosestPC(crt);
                            gv.sf.CombatTarget = pc;
                            gv.sf.ActionToTake = "Cast";
                            break;
                        }
                        else if (gv.sf.SpellToCast.spellTargetType.Equals("PointLocation"))
                        {
                            Coordinate bestLoc = targetBestPointLocation(crt);
                            if (bestLoc == new Coordinate(-1, -1))
                            {
                                //didn't find a target so use closest PC
                                Player pc = targetClosestPC(crt);
                                gv.sf.CombatTarget = new Coordinate(pc.combatLocX, pc.combatLocY);
                            }
                            else
                            {
                                gv.sf.CombatTarget = targetBestPointLocation(crt);
                            }
                            gv.sf.ActionToTake = "Cast";
                            break;
                        }
                        else if (gv.sf.SpellToCast.spellTargetType.Equals("Friend"))
                        {
                            //target is another creature (currently assumed that spell is a heal spell)
                            Creature targetCrt = GetCreatureWithMostDamaged();
                            if (targetCrt != null)
                            {
                                gv.sf.CombatTarget = targetCrt;
                                gv.sf.ActionToTake = "Cast";
                                break;
                            }
                            else //didn't find a target that needs HP
                            {
                                gv.sf.SpellToCast = null;
                                continue;
                            }
                        }
                        else if (gv.sf.SpellToCast.spellTargetType.Equals("Self"))
                        {
                            //target is self (currently assumed that spell is a heal spell)
                            Creature targetCrt = crt;
                            if (targetCrt != null)
                            {
                                gv.sf.CombatTarget = targetCrt;
                                gv.sf.ActionToTake = "Cast";
                                break;
                            }
                        }
                        else //didn't find a target so set to null so that will use attack instead
                        {
                            gv.sf.SpellToCast = null;
                        }
                    }
                }
            }
            if (gv.sf.SpellToCast == null) //didn't find a spell that matched the criteria so use attack instead
            {
                Player pc = targetClosestPC(crt);
                gv.sf.CombatTarget = pc;
                gv.sf.ActionToTake = "Attack";
            }
        }
        public void endCreatureTurn()
        {
            gv.Render();
            canMove = true;
            gv.sf.ActionToTake = null;
            gv.sf.SpellToCast = null;
            if (checkEndEncounter())
            {
                return;
            }
            turnController();
        }
        public void doStandardCreatureAttack()
        {
            Creature crt = mod.currentEncounter.encounterCreatureList[creatureIndex];
            Player pc = (Player)gv.sf.CombatTarget;

            bool hit = false;
            for (int i = 0; i < crt.cr_numberOfAttacks; i++)
            {
                //this reduces the to hit bonus for each further creature attack by an additional -5
                //creatureMultAttackPenalty = 5 * i;            
                bool hitreturn = doActualCreatureAttack(pc, crt, i);
                if (hitreturn) { hit = true; }
                if (pc.hp <= 0)
                {
                    break; //do not try and attack same PC that was just killed
                }
            }

            //play attack sound for melee
            if (!crt.cr_category.Equals("Ranged"))
            {
                gv.PlaySound(crt.cr_attackSound);
            }

            if (hit)
            {
                hitAnimationLocation = new Coordinate(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY));
                //new system
                AnimationStackGroup newGroup = new AnimationStackGroup();
                animationSeqStack[0].AnimationSeq.Add(newGroup);
                addHitAnimation(newGroup);
            }
            else
            {
                hitAnimationLocation = new Coordinate(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY));
                //new system
                AnimationStackGroup newGroup = new AnimationStackGroup();
                animationSeqStack[0].AnimationSeq.Add(newGroup);
                addMissAnimation(newGroup);
            }
        }
        public bool doActualCreatureAttack(Player pc, Creature crt, int attackNumber)
        {
            int attackRoll = gv.sf.RandInt(20);
            int attackMod = CalcCreatureAttackModifier(crt, pc);
            int defense = CalcPcDefense(pc, crt);
            int damage = CalcCreatureDamageToPc(pc, crt);
            int attack = attackRoll + attackMod;

            if ((attack >= defense) || (attackRoll == 20))
            {
                pc.hp = pc.hp - damage;
                gv.cc.addLogText("<gy>" + crt.cr_name + "</gy><BR>");
                gv.cc.addLogText("<wh>" + "attacks" + "</wh><BR>");
                gv.cc.addLogText("<bu>" + pc.name + "</bu><BR>");
                gv.cc.addLogText("<rd>" + "HITS (-" + damage + "hp)</rd><BR>");
                gv.cc.addLogText("<wh>" + attackRoll + "+" + attackMod + ">=" + defense + "</wh><BR>");

                doOnHitScriptBasedOnFilename(crt.onScoringHit, crt, pc);
                if (!crt.onScoringHitCastSpellTag.Equals("none"))
                {
                    doCreatureOnHitCastSpell(crt, pc);
                }

                //Draw floaty text showing damage above PC
                int txtH = (int)gv.fontHeight;
                int shiftUp = 0 - (attackNumber * txtH);
                gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), damage + "", shiftUp);

                if (pc.hp <= 0)
                {
                    gv.cc.addLogText("<rd>" + pc.name + " drops down unconsciously!" + "</rd><BR>");
                    pc.charStatus = "Dead";
                }
                if (pc.hp <= -20)
                {
                    deathAnimationLocations.Add(new Coordinate(pc.combatLocX, pc.combatLocY));
                }
                return true;
            }
            else
            {
                gv.cc.addLogText("<gy>" + crt.cr_name + "</gy><BR>");
                gv.cc.addLogText("<wh>" + "attacks" + "</wh><BR>");
                gv.cc.addLogText("<bu>" + pc.name + "</bu><BR>");
                gv.cc.addLogText("<wh>" + "MISSES</wh><BR>");
                gv.cc.addLogText("<wh>" + attackRoll + "+" + attackMod + " < " + defense + "</wh><BR>");
                return false;
            }
        }
        public void doCreatureCombatFacing(Creature crt, int tarX, int tarY)
        {
            if ((tarX == crt.combatLocX) && (tarY > crt.combatLocY)) { crt.combatFacing = 2; }
            if ((tarX > crt.combatLocX) && (tarY > crt.combatLocY)) { crt.combatFacing = 3; }
            if ((tarX < crt.combatLocX) && (tarY > crt.combatLocY)) { crt.combatFacing = 1; }
            if ((tarX == crt.combatLocX) && (tarY < crt.combatLocY)) { crt.combatFacing = 8; }
            if ((tarX > crt.combatLocX) && (tarY < crt.combatLocY)) { crt.combatFacing = 9; }
            if ((tarX < crt.combatLocX) && (tarY < crt.combatLocY)) { crt.combatFacing = 7; }
            if ((tarX > crt.combatLocX) && (tarY == crt.combatLocY)) { crt.combatFacing = 6; }
            if ((tarX < crt.combatLocX) && (tarY == crt.combatLocY)) { crt.combatFacing = 4; }
        }
        #endregion

        public void doOnHitScriptBasedOnFilename(string filename, Creature crt, Player pc)
        {
            if (!filename.Equals("none"))
            {
                try
                {
                    if (filename.Equals("onHitBeetleFire.cs"))
                    {
                        float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f));
                        float damage = (1 * gv.sf.RandInt(2)) + 0;
                        int fireDam = (int)(damage * resist);

                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                        + " fireDam = " + fireDam + "</font>" +
                                        "<BR>");
                        }
                        gv.cc.addLogText("<font color='aqua'>" + pc.name + "</font>" +
                                "<font color='white'>" + " is burned for " + "</font>" +
                                "<font color='red'>" + fireDam + "</font>" +
                                "<font color='white'>" + " hit point(s)" + "</font>" +
                                "<BR>");
                        pc.hp -= fireDam;
                    }

                    else if (filename.Equals("onHitMaceOfStunning.cs"))
                    {
                        int tryHold = gv.sf.RandInt(100);
                        if (tryHold > 50)
                        {
                            //attempt to hold PC
                            int saveChkRoll = gv.sf.RandInt(20);
                            int saveChk = saveChkRoll + crt.fortitude;
                            int DC = 15;
                            if (saveChk >= DC) //passed save check
                            {
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " avoids stun (" + saveChkRoll + " + " + crt.fortitude + " >= " + DC + ")</font><BR>");
                            }
                            else
                            {
                                gv.cc.addLogText("<font color='red'>" + crt.cr_name + " is stunned by mace (" + saveChkRoll + " + " + crt.fortitude + " < " + DC + ")</font><BR>");
                                crt.cr_status = "Held";
                                Effect ef = mod.getEffectByTag("hold");
                                crt.AddEffectByObject(ef, 1);
                            }
                        }
                    }
                    else if (filename.Equals("onHitBeetleAcid.cs"))
                    {
                        float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalAcid / 100f));
                        float damage = (1 * gv.sf.RandInt(2)) + 0;
                        int acidDam = (int)(damage * resist);

                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                    + " acidDam = " + acidDam + "</font>" +
                                    "<BR>");
                        }
                        gv.cc.addLogText("<font color='aqua'>" + pc.name + "</font>" +
                                "<font color='white'>" + " is burned for " + "</font>" +
                                "<font color='lime'>" + acidDam + "</font>" +
                                "<font color='white'>" + " hit point(s)" + "</font>" +
                                "<BR>");
                        pc.hp -= acidDam;

                        //attempt to hold PC
                        int saveChkRoll = gv.sf.RandInt(20);
                        int saveChk = saveChkRoll + pc.fortitude;
                        int DC = 10;
                        if (saveChk >= DC) //passed save check
                        {
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " avoids the acid stun (" + saveChkRoll + " + " + pc.fortitude + " >= " + DC + ")</font><BR>");
                        }
                        else
                        {
                            gv.cc.addLogText("<font color='red'>" + pc.name + " is held by an acid stun (" + saveChkRoll + " + " + pc.fortitude + " < " + DC + ")</font><BR>");
                            pc.charStatus = "Held";
                            Effect ef = mod.getEffectByTag("hold");
                            pc.AddEffectByObject(ef, 1);
                        }
                    }
                    else if (filename.Equals("onHitOneFire.cs"))
                    {
                        float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                        float damage = 1.0f;
                        int fireDam = (int)(damage * resist);

                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                        + " fireDam = " + fireDam + "</font>" +
                                        "<BR>");
                        }
                        gv.cc.addLogText("<font color='aqua'>" + crt.cr_name + "</font>" +
                                "<font color='white'>" + " is burned for " + "</font>" +
                                "<font color='red'>" + fireDam + "</font>" +
                                "<font color='white'>" + " hit point(s)" + "</font>" +
                                "<BR>");
                        crt.hp -= fireDam;
                    }
                    else if (filename.Equals("onHitOneTwoFire.cs"))
                    {
                        float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                        float damage = (1 * gv.sf.RandInt(2)) + 0;
                        int fireDam = (int)(damage * resist);

                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                        + " fireDam = " + fireDam + "</font>" +
                                        "<BR>");
                        }
                        gv.cc.addLogText("<font color='aqua'>" + crt.cr_name + "</font>" +
                                "<font color='white'>" + " is burned for " + "</font>" +
                                "<font color='red'>" + fireDam + "</font>" +
                                "<font color='white'>" + " hit point(s)" + "</font>" +
                                "<BR>");
                        crt.hp -= fireDam;
                    }
                    else if (filename.Equals("onHitTwoThreeFire.cs"))
                    {
                        float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                        float damage = (1 * gv.sf.RandInt(2)) + 1;
                        int fireDam = (int)(damage * resist);

                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                        + " fireDam = " + fireDam + "</font>" +
                                        "<BR>");
                        }
                        gv.cc.addLogText("<font color='aqua'>" + crt.cr_name + "</font>" +
                                "<font color='white'>" + " is burned for " + "</font>" +
                                "<font color='red'>" + fireDam + "</font>" +
                                "<font color='white'>" + " hit point(s)" + "</font>" +
                                "<BR>");
                        crt.hp -= fireDam;
                    }
                    else if (filename.Equals("onHitPcPoisonedLight.cs"))
                    {
                        int saveChkRoll = gv.sf.RandInt(20);
                        int saveChk = saveChkRoll + pc.reflex;
                        int DC = 13;
                        if (saveChk >= DC) //passed save check
                        {
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " avoids being poisoned" + "</font>" +
                                    "<BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font>" +
                                            "<BR>");
                            }
                        }
                        else //failed check
                        {
                            gv.cc.addLogText("<font color='red'>" + pc.name + " is poisoned" + "</font>" + "<BR>");
                            Effect ef = mod.getEffectByTag("poisonedLight");
                            pc.AddEffectByObject(ef, 1);
                        }
                    }
                    else if (filename.Equals("onHitPcPoisonedMedium.cs"))
                    {
                        int saveChkRoll = gv.sf.RandInt(20);
                        int saveChk = saveChkRoll + pc.reflex;
                        int DC = 16;
                        if (saveChk >= DC) //passed save check
                        {
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " avoids being poisoned" + "</font>" +
                                    "<BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font>" +
                                            "<BR>");
                            }
                        }
                        else //failed check
                        {
                            gv.cc.addLogText("<font color='red'>" + pc.name + " is poisoned" + "</font>" + "<BR>");
                            Effect ef = mod.getEffectByTag("poisonedMedium");
                            pc.AddEffectByObject(ef, 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    gv.errorLog(ex.ToString());
                }
            }
        }
        public void doCreatureOnHitCastSpell(Creature crt, Player pc)
        {
            Spell sp = gv.mod.getSpellByTag(crt.onScoringHitCastSpellTag);
            if (sp == null) { return; }
            gv.cc.doSpellBasedOnScriptOrEffectTag(sp, crt, pc, false);
        }
        public bool checkEndEncounter()
        {
            int foundOneCrtr = 0;
            foreach (Creature crtr in mod.currentEncounter.encounterCreatureList)
            {
                if (crtr.hp > 0)
                {
                    foundOneCrtr = 1;
                }
            }
            if ((foundOneCrtr == 0) && (gv.screenType.Equals("combat")))
            {
                gv.touchEnabled = true;
                // give gold drop
                if (mod.currentEncounter.goldDrop > 0)
                {
                    gv.cc.addLogText("<yl>The party finds " + mod.currentEncounter.goldDrop + " " + mod.goldLabelPlural + ".<BR></yl>");
                }
                mod.partyGold += mod.currentEncounter.goldDrop;
                // give InventoryList
                if (mod.currentEncounter.encounterInventoryRefsList.Count > 0)
                {

                    string s = "<ma>" + "The party has found:<BR>";
                    foreach (ItemRefs itRef in mod.currentEncounter.encounterInventoryRefsList)
                    {
                        mod.partyInventoryRefsList.Add(itRef.DeepCopy());
                        s += itRef.name + "<BR>";
                        //find this creatureRef in mod creature list

                    }
                    gv.cc.addLogText(s + "</ma>" + "<BR>");
                }

                int giveEachXP = encounterXP / mod.playerList.Count;
                gv.cc.addLogText("fuchsia", "Each receives " + giveEachXP + " XP");
                foreach (Player givePcXp in mod.playerList)
                {
                    givePcXp.XP = givePcXp.XP + giveEachXP;
                }
                //btnSelect.Text = "SELECT";
                gv.screenType = "main";
                //do END ENCOUNTER IBScript
                gv.cc.doIBScriptBasedOnFilename(gv.mod.currentEncounter.OnEndCombatIBScript, gv.mod.currentEncounter.OnEndCombatIBScriptParms);
                if (gv.cc.calledEncounterFromProp)
                {
                    //gv.mod.isRecursiveDoTriggerCallMovingProp = true;
                    //gv.mod.isRecursiveCall = true;
                    gv.cc.doPropTriggers();
                    //gv.mod.isRecursiveCall = false;
                }
                else
                {
                    gv.cc.doTrigger();
                }
                return true;
            }

            int foundOnePc = 0;
            foreach (Player pc in mod.playerList)
            {
                if (pc.hp > 0)
                {
                    foundOnePc = 1;
                }
            }
            if (foundOnePc == 0)
            {
                gv.touchEnabled = true;
                gv.sf.MessageBox("Your party has been defeated!");
                gv.resetGame();
                gv.screenType = "title";
                return true;
            }
            return false;
        }

        //COMBAT SCREEN UPDATE
        public void Update(int elapsed)
        {
            combatUiLayout.Update(elapsed);
            refreshCreatureCoveredSquares();

            #region PROP AMBIENT SPRITES
            foreach (Sprite spr in spriteList)
            {
                spr.Update(elapsed, gv);
            }
            //remove sprite if hit end of life
            for (int x = spriteList.Count - 1; x >= 0; x--)
            {
                if (spriteList[x].timeToLiveInMilliseconds <= 0)
                {
                    try
                    {
                        spriteList.RemoveAt(x);
                    }
                    catch (Exception ex)
                    {
                        gv.errorLog(ex.ToString());
                    }
                }
            }
            #endregion

            #region COMBAT ANIMATION SPRITES
            if (animationsOn)
            {
                attackAnimationTimeElapsed += elapsed;
                if (attackAnimationTimeElapsed >= attackAnimationLengthInMilliseconds)
                {
                    //time is up, reset attack animations to null
                    creatureToAnimate = null;
                    playerToAnimate = null;
                    foreach (AnimationSequence seq in animationSeqStack)
                    {
                        if (seq.AnimationSeq.Count > 0)
                        {
                            if (seq.AnimationSeq[0].turnFloatyTextOn)
                            {
                                floatyTextOn = true; //show any floaty text in the pool
                            }
                            foreach (Sprite spr in seq.AnimationSeq[0].SpriteGroup)
                            {
                                //just update the group at the top of the stack, first in first
                                spr.Update(elapsed, gv);
                            }
                        }
                    }
                    //remove sprites if hit end of life
                    for (int aniseq = animationSeqStack.Count - 1; aniseq >= 0; aniseq--)
                    {
                        for (int stkgrp = animationSeqStack[aniseq].AnimationSeq.Count - 1; stkgrp >= 0; stkgrp--)
                        {
                            for (int sprt = animationSeqStack[aniseq].AnimationSeq[stkgrp].SpriteGroup.Count - 1; sprt >= 0; sprt--)
                            {
                                if (animationSeqStack[aniseq].AnimationSeq[stkgrp].SpriteGroup[sprt].timeToLiveInMilliseconds <= 0)
                                {
                                    try
                                    {
                                        animationSeqStack[aniseq].AnimationSeq[stkgrp].SpriteGroup.RemoveAt(sprt);

                                    }
                                    catch (Exception ex)
                                    {
                                        gv.errorLog(ex.ToString());
                                    }
                                }
                            }
                            if (animationSeqStack[aniseq].AnimationSeq[stkgrp].SpriteGroup.Count == 0)
                            {
                                try
                                {
                                    animationSeqStack[aniseq].AnimationSeq.RemoveAt(stkgrp);
                                }
                                catch (Exception ex)
                                {
                                    gv.errorLog(ex.ToString());
                                }
                            }
                        }
                        if (animationSeqStack[aniseq].AnimationSeq.Count == 0)
                        {
                            try
                            {
                                animationSeqStack.RemoveAt(aniseq);
                            }
                            catch (Exception ex)
                            {
                                gv.errorLog(ex.ToString());
                            }
                        }
                    }
                    //if all animation sequences are done, end this turn
                    if (animationSeqStack.Count == 0)
                    {
                        animationsOn = false;
                        deathAnimationLocations.Clear();

                        //remove any dead creatures                        
                        for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
                        {
                            if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                            {
                                try
                                {
                                    //do OnDeath IBScript
                                    gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                                    mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                                    mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                                }
                                catch (Exception ex)
                                {
                                    gv.errorLog(ex.ToString());
                                }
                            }
                        }
                        if (isPlayerTurn)
                        {
                            checkEndEncounter();
                            gv.touchEnabled = true;
                            animationState = AnimationState.None;
                            endPcTurn(true);
                        }
                        else
                        {
                            animationState = AnimationState.None;
                            endCreatureTurn();
                        }
                    }
                }
            }
            #endregion

            #region FLOATY TEXT
            if (floatyTextOn)
            {
                //move up 50pxl per second (50px/1000ms)*elapsed
                float multiplier = 100.0f / gv.mod.combatAnimationSpeed;
                int shiftUp = (int)(0.05f * elapsed * multiplier);
                foreach (FloatyText ft in gv.cc.floatyTextList)
                {
                    ft.location.Y -= shiftUp;
                    ft.timeToLive -= (int)(elapsed * multiplier);
                }
                //remove sprite if hit end of life
                for (int x = gv.cc.floatyTextList.Count - 1; x >= 0; x--)
                {
                    if (gv.cc.floatyTextList[x].timeToLive <= 0)
                    {
                        try
                        {
                            gv.cc.floatyTextList.RemoveAt(x);
                        }
                        catch (Exception ex)
                        {
                            gv.errorLog(ex.ToString());
                        }
                    }
                }
                if (gv.cc.floatyTextList.Count == 0)
                {
                    floatyTextOn = false;
                }
            }
            #endregion
        }
        public void refreshCreatureCoveredSquares()
        {
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                crt.tokenCoveredSquares.Clear();
                //add normal creature size square location first...add other sizes as needed
                crt.tokenCoveredSquares.Add(new Coordinate(crt.combatLocX, crt.combatLocY));
                
                int width = gv.cc.GetFromBitmapList(crt.cr_tokenFilename).PixelSize.Width;
                int height = gv.cc.GetFromBitmapList(crt.cr_tokenFilename).PixelSize.Height;
                //1=normal, 2=wide, 3=tall, 4=large
                int crtSize = gv.cc.getCreatureSize(crt.cr_tokenFilename);

                //wide
                if (crtSize == 2)
                {
                    crt.tokenCoveredSquares.Add(new Coordinate(crt.combatLocX + 1, crt.combatLocY));
                }
                //tall
                else if (crtSize == 3)
                {
                    crt.tokenCoveredSquares.Add(new Coordinate(crt.combatLocX, crt.combatLocY + 1));
                }
                //large
                else if (crtSize == 4)
                {
                    crt.tokenCoveredSquares.Add(new Coordinate(crt.combatLocX + 1, crt.combatLocY));
                    crt.tokenCoveredSquares.Add(new Coordinate(crt.combatLocX, crt.combatLocY + 1));
                    crt.tokenCoveredSquares.Add(new Coordinate(crt.combatLocX + 1, crt.combatLocY + 1));
                }
            }
        }

        #region Combat Draw
        public void redrawCombat()
        {
            drawCombatMap();
            drawProps();
            drawEffectSquares();
            drawCombatPlayers();
            drawCombatCreatures();            
            drawSprites();
            if (mod.currentEncounter.UseDayNightCycle)
            {
                drawOverlayTints();
            }
            if (!animationsOn)
            {
                drawTargetHighlight();
                drawLosTrail();
            }
            drawFloatyText();
            drawHPText();
            drawSPText();
            drawFloatyTextList();
            drawUiLayout();
            if (gv.showMessageBox)
            {
                gv.messageBox.onDrawLogBox();
            }
        }
        public void drawUiLayout()
        {
            //SET PORTRAITS
            foreach (IB2Panel pnl in combatUiLayout.panelList)
            {
                if (pnl.tag.Equals("portraitPanel"))
                {
                    foreach (IB2Portrait ptr in pnl.portraitList)
                    {
                        ptr.show = false;
                    }
                    int index = 0;
                    foreach (Player pc1 in mod.playerList)
                    {
                        pnl.portraitList[index].show = true;
                        pnl.portraitList[index].ImgFilename = pc1.portraitFilename;
                        pnl.portraitList[index].TextHP = pc1.hp + "/" + pc1.hpMax;
                        pnl.portraitList[index].TextSP = pc1.sp + "/" + pc1.spMax;
                        if (pc1.moveOrder == currentMoveOrderIndex - 1)
                        {
                            pnl.portraitList[index].glowOn = true;
                        }
                        else
                        {
                            pnl.portraitList[index].glowOn = false;
                        }
                        index++;
                    }
                    break;
                }
            }

            //SET MOVES LEFT TEXT
            Player pc = mod.playerList[currentPlayerIndex];
            float movesLeft = pc.moveDistance - currentMoves;
            if (movesLeft < 0) { movesLeft = 0; }
            IB2Button btn = combatUiLayout.GetButtonByTag("btnMoveCounter");
            if (btn != null)
            {
                btn.Text = movesLeft.ToString();
            }

            //SET KILL BUTTON
            if (mod.debugMode)
            {
                IB2ToggleButton tgl = combatUiLayout.GetToggleByTag("tglKill");
                if (tgl != null)
                {
                    tgl.show = true;
                }
            }
            else
            {
                IB2ToggleButton tgl = combatUiLayout.GetToggleByTag("tglKill");
                if (tgl != null)
                {
                    tgl.show = false;
                }
            }

            //SET BUTTON STATES
            //select button
            if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
            {
                btn = combatUiLayout.GetButtonByTag("btnSelect");
                if (btn != null)
                {
                    btn.Img2Filename = "btntarget";
                    //btn.Text = "TARGET";
                }
            }
            else
            {
                btn = combatUiLayout.GetButtonByTag("btnSelect");
                if (btn != null)
                {
                    btn.Img2Filename = "btnselection";
                    //btn.Text = "SELECT";
                }
            }
            //move button
            if (canMove)
            {
                if (currentCombatMode.Equals("move"))
                {
                    btn = combatUiLayout.GetButtonByTag("btnMove");
                    if (btn != null)
                    {
                        btn.btnState = buttonState.On;
                    }
                }
                else
                {
                    btn = combatUiLayout.GetButtonByTag("btnMove");
                    if (btn != null)
                    {
                        btn.btnState = buttonState.Normal;
                    }
                }
            }
            else
            {
                btn = combatUiLayout.GetButtonByTag("btnMove");
                if (btn != null)
                {
                    btn.btnState = buttonState.Off;
                }
            }
            //attack button
            if (currentCombatMode.Equals("attack"))
            {
                btn = combatUiLayout.GetButtonByTag("btnAttack");
                if (btn != null)
                {
                    btn.btnState = buttonState.On;
                }
            }
            else
            {
                btn = combatUiLayout.GetButtonByTag("btnAttack");
                if (btn != null)
                {
                    btn.btnState = buttonState.Normal;
                }
            }
            //cast button
            if (currentCombatMode.Equals("cast"))
            {
                btn = combatUiLayout.GetButtonByTag("btnCast");
                if (btn != null)
                {
                    btn.btnState = buttonState.On;
                }
            }
            else
            {
                btn = combatUiLayout.GetButtonByTag("btnCast");
                if (btn != null)
                {
                    btn.btnState = buttonState.Normal;
                }
            }

            combatUiLayout.Draw();
        }
        public void drawCombatMap()
        {            
            #region Draw Layer1
            for (int x = 0; x < this.mod.currentEncounter.MapSizeX; x++)
            {
                for (int y = 0; y < this.mod.currentEncounter.MapSizeY; y++)
                {
                    string tile = mod.currentEncounter.Layer1Filename[y * mod.currentEncounter.MapSizeX + x];
                    IbRect srcLyr = new IbRect(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);

                    if (srcLyr != null)
                    {
                        int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                        int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                        int tlX = ((x + shiftX) * gv.squareSize) + mapStartLocXinPixels;
                        int tlY = (y + shiftY) * gv.squareSize;
                        float scalerX = srcLyr.Width / gv.tileSizeInPixels;
                        if (scalerX == 0) { scalerX = 1.0f; }
                        float scalerY = srcLyr.Height / gv.tileSizeInPixels;
                        if (scalerY == 0) { scalerY = 1.0f; }
                        int brX = (int)(gv.squareSize * scalerX);
                        int brY = (int)(gv.squareSize * scalerY);
                        IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                        bool mirror = false;
                        if (mod.currentEncounter.Layer1Mirror[y * mod.currentEncounter.MapSizeX + x] == 1) { mirror = true; }
                        gv.DrawBitmap(gv.cc.GetFromTileBitmapList(tile), srcLyr, dstLyr, mod.currentEncounter.Layer1Rotate[y * mod.currentEncounter.MapSizeX + x], mirror);
                    }
                }
            }
            #endregion
            #region Draw Layer2
            for (int x = 0; x < this.mod.currentEncounter.MapSizeX; x++)
            {
                for (int y = 0; y < this.mod.currentEncounter.MapSizeY; y++)
                {
                    string tile = mod.currentEncounter.Layer2Filename[y * mod.currentEncounter.MapSizeX + x];
                    IbRect srcLyr = new IbRect(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);

                    if (srcLyr != null)
                    {
                        int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                        int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                        int tlX = ((x + shiftX) * gv.squareSize) + mapStartLocXinPixels;
                        int tlY = (y + shiftY) * gv.squareSize;
                        float scalerX = srcLyr.Width / gv.tileSizeInPixels;
                        if (scalerX == 0) { scalerX = 1.0f; }
                        float scalerY = srcLyr.Height / gv.tileSizeInPixels;
                        if (scalerY == 0) { scalerY = 1.0f; }
                        int brX = (int)(gv.squareSize * scalerX);
                        int brY = (int)(gv.squareSize * scalerY);
                        IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                        bool mirror = false;
                        if (mod.currentEncounter.Layer2Mirror[y * mod.currentEncounter.MapSizeX + x] == 1) { mirror = true; }
                        gv.DrawBitmap(gv.cc.GetFromTileBitmapList(tile), srcLyr, dstLyr, mod.currentEncounter.Layer2Rotate[y * mod.currentEncounter.MapSizeX + x], mirror);
                    }
                }
            }
            #endregion    
            #region Draw Layer3
            if (mod.currentEncounter.Layer3Filename.Count > 0)
            {
                for (int x = 0; x < this.mod.currentEncounter.MapSizeX; x++)
                {
                    for (int y = 0; y < this.mod.currentEncounter.MapSizeY; y++)
                    {
                        string tile = mod.currentEncounter.Layer3Filename[y * mod.currentEncounter.MapSizeX + x];
                        IbRect srcLyr = new IbRect(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);

                        if (srcLyr != null)
                        {
                            int shiftY = srcLyr.Top / gv.squareSizeInPixels;
                            int shiftX = srcLyr.Left / gv.squareSizeInPixels;
                            int tlX = ((x + shiftX) * gv.squareSize) + mapStartLocXinPixels;
                            int tlY = (y + shiftY) * gv.squareSize;
                            float scalerX = srcLyr.Width / gv.tileSizeInPixels;
                            if (scalerX == 0) { scalerX = 1.0f; }
                            float scalerY = srcLyr.Height / gv.tileSizeInPixels;
                            if (scalerY == 0) { scalerY = 1.0f; }
                            int brX = (int)(gv.squareSize * scalerX);
                            int brY = (int)(gv.squareSize * scalerY);
                            IbRect dstLyr = new IbRect(tlX, tlY, brX, brY);
                            bool mirror = false;
                            if (mod.currentEncounter.Layer3Mirror[y * mod.currentEncounter.MapSizeX + x] == 1) { mirror = true; }
                            gv.DrawBitmap(gv.cc.GetFromTileBitmapList(tile), srcLyr, dstLyr, mod.currentEncounter.Layer3Rotate[y * mod.currentEncounter.MapSizeX + x], mirror);
                        }
                    }
                }
            }
            #endregion        
            #region Draw Grid
            //I brought the pix width and height of source back to normal
            if (mod.com_showGrid)
            {
                for (int x = 0; x < this.mod.currentEncounter.MapSizeX; x++)
                {
                    for (int y = 0; y < this.mod.currentEncounter.MapSizeY; y++)
                    {
                        //TileEnc tile = mod.currentEncounter.encounterTiles[y * mod.currentEncounter.MapSizeX + x];

                        int tlX = (x * gv.squareSize) + mapStartLocXinPixels;
                        int tlY = y * gv.squareSize;
                        int brX = gv.squareSize;
                        int brY = gv.squareSize;

                        IbRect srcGrid = new IbRect(0, 0, gv.squareSizeInPixels, gv.squareSizeInPixels);
                        IbRect dstGrid = new IbRect(tlX, tlY, brX, brY);
                        if (mod.currentEncounter.LoSBlocked[y * mod.currentEncounter.MapSizeX + x] == 1)
                        {
                            gv.DrawBitmap(gv.cc.losBlocked, srcGrid, dstGrid);
                        }
                        if (mod.currentEncounter.Walkable[y * mod.currentEncounter.MapSizeX + x] == 0)
                        {
                            gv.DrawBitmap(gv.cc.walkBlocked, srcGrid, dstGrid);
                        }
                        else
                        {
                            gv.DrawBitmap(gv.cc.walkPass, srcGrid, dstGrid);
                        }
                    }
                }
            }
            #endregion
            #region Draw Pathfinding Numbers
            for (int x = 0; x < this.mod.currentEncounter.MapSizeX; x++)
            {
                for (int y = 0; y < this.mod.currentEncounter.MapSizeY; y++)
                {
                    if ((pf.values != null) && (mod.debugMode))
                    {
                        gv.DrawText(pf.values[x, y].ToString(), x * gv.squareSize + mapStartLocXinPixels, y * gv.squareSize, "wh");
                    }
                }
            }
            #endregion
        }
        public void drawCombatPlayers()
        {
            Player p = mod.playerList[currentPlayerIndex];
            
            IbRect src = new IbRect(0, 0, gv.cc.turn_marker.PixelSize.Width, gv.cc.turn_marker.PixelSize.Width);
            IbRect dst = new IbRect(getPixelLocX(p.combatLocX), getPixelLocY(p.combatLocY), gv.squareSize, gv.squareSize);
            if (isPlayerTurn)
            {
                gv.DrawBitmap(gv.cc.turn_marker, src, dst);
            }
            
            foreach (Player pc in mod.playerList)
            {
                
                src = new IbRect(0, 0, gv.cc.GetFromBitmapList(pc.tokenFilename).PixelSize.Width, gv.cc.GetFromBitmapList(pc.tokenFilename).PixelSize.Width);
                //check if drawing animation of player
                if ((playerToAnimate != null) && (playerToAnimate == pc))
                {
                    src = new IbRect(0, gv.cc.GetFromBitmapList(pc.tokenFilename).PixelSize.Width, gv.cc.GetFromBitmapList(pc.tokenFilename).PixelSize.Width, gv.cc.GetFromBitmapList(pc.tokenFilename).PixelSize.Width);
                }
                dst = new IbRect(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY), gv.squareSize, gv.squareSize);
                if ((pc.isDead()) || (pc.isUnconcious()))
                {
                    src = new IbRect(0, 0, gv.cc.pc_dead.PixelSize.Width, gv.cc.pc_dead.PixelSize.Width);
                    gv.DrawBitmap(gv.cc.pc_dead, src, dst);
                }
                else
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(pc.tokenFilename), src, dst, !pc.combatFacingLeft);
                    //src = new IbRect(0, 0, gv.cc.GetFromBitmapList(pc.tokenFilename).PixelSize.Width, gv.cc.GetFromBitmapList(pc.tokenFilename).PixelSize.Width);
                    foreach (Effect ef in pc.effectsList)
                    {
                        //Bitmap fx = gv.cc.LoadBitmap(ef.spriteFilename);
                        src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ef.spriteFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ef.spriteFilename).PixelSize.Width);
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(ef.spriteFilename), src, dst);
                        //gv.cc.DisposeOfBitmap(ref fx);
                    }
                    if (pc.steathModeOn)
                    {
                        src = new IbRect(0, 0, gv.cc.pc_stealth.PixelSize.Width, gv.cc.pc_stealth.PixelSize.Width);
                        gv.DrawBitmap(gv.cc.pc_stealth, src, dst);
                    }
                    //PLAYER FACING
                    src = new IbRect(0, 0, gv.cc.facing1.PixelSize.Width, gv.cc.facing1.PixelSize.Height);
                    if (pc.combatFacing == 8) { gv.DrawBitmap(gv.cc.facing8, src, dst); }
                    else if (pc.combatFacing == 9) { gv.DrawBitmap(gv.cc.facing9, src, dst); }
                    else if (pc.combatFacing == 6) { gv.DrawBitmap(gv.cc.facing6, src, dst); }
                    else if (pc.combatFacing == 3) { gv.DrawBitmap(gv.cc.facing3, src, dst); }
                    else if (pc.combatFacing == 2) { gv.DrawBitmap(gv.cc.facing2, src, dst); }
                    else if (pc.combatFacing == 1) { gv.DrawBitmap(gv.cc.facing1, src, dst); }
                    else if (pc.combatFacing == 4) { gv.DrawBitmap(gv.cc.facing4, src, dst); }
                    else if (pc.combatFacing == 7) { gv.DrawBitmap(gv.cc.facing7, src, dst); }
                    else { } //didn't find one
                }

                if (showMoveOrder)
                {
                    int mo = pc.moveOrder + 1;
                    drawText(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY) - gv.fontHeight - 2, mo.ToString(), "wh");
                }
                
            }
        }
        public void drawLosTrail()
        {
            Player p = mod.playerList[currentPlayerIndex];
            if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
            {
                //Uses the Screen Pixel Locations
                int endX = getPixelLocX(targetHighlightCenterLocation.X) + (gv.squareSize / 2);
                int endY = getPixelLocY(targetHighlightCenterLocation.Y) + (gv.squareSize / 2);
                int startX = getPixelLocX(p.combatLocX) + (gv.squareSize / 2);
                int startY = getPixelLocY(p.combatLocY) + (gv.squareSize / 2);
                //Uses the Map Pixel Locations
                int endX2 = targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
                int endY2 = targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);
                int startX2 = p.combatLocX * gv.squareSize + (gv.squareSize / 2);
                int startY2 = p.combatLocY * gv.squareSize + (gv.squareSize / 2);

                //check if target is within attack distance, use green if true, red if false
                if (isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX2, startY2)))
                {
                    drawVisibleLineOfSightTrail(new Coordinate(endX, endY), new Coordinate(startX, startY), Color.Lime, 2);
                }
                else
                {
                    drawVisibleLineOfSightTrail(new Coordinate(endX, endY), new Coordinate(startX, startY), Color.Red, 2);
                }
            }
        }
        public void drawCombatCreatures()
        {
            if (mod.currentEncounter.encounterCreatureList.Count > 0)
            {
                if (!isPlayerTurn)
                {
                    if (creatureIndex < mod.currentEncounter.encounterCreatureList.Count)
                    {
                        Creature cr = mod.currentEncounter.encounterCreatureList[creatureIndex];
                        IbRect src = new IbRect(0, 0, gv.cc.turn_marker.PixelSize.Width, gv.cc.turn_marker.PixelSize.Height);
                        foreach (Coordinate coor in cr.tokenCoveredSquares)
                        {
                            IbRect dst = new IbRect(getPixelLocX(coor.X), getPixelLocY(coor.Y), gv.squareSize, gv.squareSize);
                            gv.DrawBitmap(gv.cc.turn_marker, src, dst);
                        }
                    }
                }
            }
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {                
                int width = gv.cc.GetFromBitmapList(crt.cr_tokenFilename).PixelSize.Width;
                int height = gv.cc.GetFromBitmapList(crt.cr_tokenFilename).PixelSize.Height;
                //1=normal, 2=wide, 3=tall, 4=large
                int crtSize = gv.cc.getCreatureSize(crt.cr_tokenFilename);

                IbRect src = new IbRect(0, 0, width, height / 2);
                if ((creatureToAnimate != null) && (creatureToAnimate == crt))
                {
                    src = new IbRect(0, height / 2, width, height / 2);
                }

                //normal
                IbRect dst = new IbRect(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY), gv.squareSize, gv.squareSize);
                //wide
                if (crtSize == 2)
                {
                    dst = new IbRect(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY), gv.squareSize * 2, gv.squareSize);
                }
                //tall
                if (crtSize == 3)
                {
                    dst = new IbRect(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY), gv.squareSize, gv.squareSize * 2);
                }
                //large
                if (crtSize == 4)
                {
                    dst = new IbRect(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY), gv.squareSize * 2, gv.squareSize * 2);
                }

                gv.DrawBitmap(gv.cc.GetFromBitmapList(crt.cr_tokenFilename), src, dst, !crt.combatFacingLeft);
                foreach (Effect ef in crt.cr_effectsList)
                {
                    //Bitmap fx = gv.cc.LoadBitmap(ef.spriteFilename);
                    src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ef.spriteFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ef.spriteFilename).PixelSize.Width);
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ef.spriteFilename), src, dst);
                    //gv.cc.DisposeOfBitmap(ref fx);
                }
                //CREATURE FACING
                src = new IbRect(0, 0, gv.cc.facing1.PixelSize.Width, gv.cc.facing1.PixelSize.Height);
                if (crt.combatFacing == 8) { gv.DrawBitmap(gv.cc.facing8, src, dst); }
                else if (crt.combatFacing == 9) { gv.DrawBitmap(gv.cc.facing9, src, dst); }
                else if (crt.combatFacing == 6) { gv.DrawBitmap(gv.cc.facing6, src, dst); }
                else if (crt.combatFacing == 3) { gv.DrawBitmap(gv.cc.facing3, src, dst); }
                else if (crt.combatFacing == 2) { gv.DrawBitmap(gv.cc.facing2, src, dst); }
                else if (crt.combatFacing == 1) { gv.DrawBitmap(gv.cc.facing1, src, dst); }
                else if (crt.combatFacing == 4) { gv.DrawBitmap(gv.cc.facing4, src, dst); }
                else if (crt.combatFacing == 7) { gv.DrawBitmap(gv.cc.facing7, src, dst); }
                else { } //didn't find one

                if (showMoveOrder)
                {
                    int mo = crt.moveOrder + 1;
                    drawText(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY) - gv.fontHeight - 2, mo.ToString(), "wh");
                }
            }
        }
        public void drawEffectSquares()
        {
            foreach (Effect ef in mod.currentEncounter.effectsList)
            {
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ef.spriteFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ef.spriteFilename).PixelSize.Width);
                IbRect dst = new IbRect(getPixelLocX(ef.combatLocX), getPixelLocY(ef.combatLocY), gv.squareSize, gv.squareSize);
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ef.spriteFilename), src, dst);
            }
        }
        public void drawProps()
        {
            foreach (Prop prp in mod.currentEncounter.propsList)
            {
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(prp.ImageFileName).PixelSize.Width, gv.cc.GetFromBitmapList(prp.ImageFileName).PixelSize.Width);
                IbRect dst = new IbRect(getPixelLocX(prp.LocationX), getPixelLocY(prp.LocationY), gv.squareSize, gv.squareSize);
                gv.DrawBitmap(gv.cc.GetFromBitmapList(prp.ImageFileName), src, dst);
            }
        }
        public void drawTargetHighlight()
        {
            Player pc = mod.playerList[currentPlayerIndex];
            if (currentCombatMode.Equals("attack"))
            {
                Item it = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                //if using ranged and have ammo, use ammo properties
                if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged"))
                        && (!mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
                {
                    //ranged weapon with ammo
                    it = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                }
                if (it == null)
                {
                    it = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                }
                //set squares list
                gv.sf.CreateAoeSquaresList(pc, targetHighlightCenterLocation, it.aoeShape, it.AreaOfEffect);
                foreach (Coordinate coor in gv.sf.AoeSquaresList)
                {
                    bool hl_green = true;
                    int endX2 = coor.X * gv.squareSize + (gv.squareSize / 2);
                    int endY2 = coor.Y * gv.squareSize + (gv.squareSize / 2);
                    int startX2 = targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
                    int startY2 = targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);

                    if (isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX2, startY2)))
                    {
                        hl_green = true;
                    }
                    else
                    {
                        hl_green = false;
                    }
                    if ((coor.X == targetHighlightCenterLocation.X) && (coor.Y == targetHighlightCenterLocation.Y))
                    {
                        int startX3 = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
                        int startY3 = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);
                        if ((isValidAttackTarget(pc)) && (isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX3, startY3))))
                        {
                            hl_green = true;
                        }
                        else
                        {
                            hl_green = false;
                        }
                    }

                    int x = getPixelLocX(coor.X);
                    int y = getPixelLocY(coor.Y);
                    IbRect src = new IbRect(0, 0, gv.cc.highlight_green.PixelSize.Width, gv.cc.highlight_green.PixelSize.Height);
                    IbRect dst = new IbRect(x, y, gv.squareSize, gv.squareSize);
                    if (hl_green)
                    {
                        gv.DrawBitmap(gv.cc.highlight_green, src, dst);
                    }
                    else
                    {
                        gv.DrawBitmap(gv.cc.highlight_red, src, dst);
                    }
                }
            }
            else if (currentCombatMode.Equals("cast"))
            {
                //set squares list
                gv.sf.CreateAoeSquaresList(pc, targetHighlightCenterLocation, gv.cc.currentSelectedSpell.aoeShape, gv.cc.currentSelectedSpell.aoeRadius);
                foreach (Coordinate coor in gv.sf.AoeSquaresList)
                {
                    bool hl_green = true;
                    int endX2 = coor.X * gv.squareSize + (gv.squareSize / 2);
                    int endY2 = coor.Y * gv.squareSize + (gv.squareSize / 2);
                    int startX2 = targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
                    int startY2 = targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);

                    if ((isValidCastTarget(pc)) && (isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX2, startY2))))
                    {
                        hl_green = true;
                    }
                    else
                    {
                        hl_green = false;
                    }
                    if ((coor.X == targetHighlightCenterLocation.X) && (coor.Y == targetHighlightCenterLocation.Y))
                    {
                        int startX3 = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
                        int startY3 = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);
                        if ((isValidCastTarget(pc)) && (isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX3, startY3))))
                        {
                            hl_green = true;
                        }
                        else
                        {
                            hl_green = false;
                        }
                    }

                    int x = getPixelLocX(coor.X);
                    int y = getPixelLocY(coor.Y);
                    IbRect src = new IbRect(0, 0, gv.cc.highlight_green.PixelSize.Width, gv.cc.highlight_green.PixelSize.Height);
                    IbRect dst = new IbRect(x, y, gv.squareSize, gv.squareSize);
                    if (hl_green)
                    {
                        gv.DrawBitmap(gv.cc.highlight_green, src, dst);
                    }
                    else
                    {
                        gv.DrawBitmap(gv.cc.highlight_red, src, dst);
                    }
                }
            }
        }
        public void drawFloatyText()
        {
            int txtH = (int)gv.fontHeight;

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(gv.cc.floatyText, gv.cc.floatyTextLoc.X + x, gv.cc.floatyTextLoc.Y + txtH + y, "bk");
                    gv.DrawText(gv.cc.floatyText2, gv.cc.floatyTextLoc.X + x, gv.cc.floatyTextLoc.Y + (txtH * 2) + y, "bk");
                    gv.DrawText(gv.cc.floatyText3, gv.cc.floatyTextLoc.X + x, gv.cc.floatyTextLoc.Y + (txtH * 3) + y, "bk");
                }
            }
            gv.DrawText(gv.cc.floatyText, gv.cc.floatyTextLoc.X, gv.cc.floatyTextLoc.Y + txtH, "yl");
            gv.DrawText(gv.cc.floatyText2, gv.cc.floatyTextLoc.X, gv.cc.floatyTextLoc.Y + txtH * 2, "yl");
            gv.DrawText(gv.cc.floatyText3, gv.cc.floatyTextLoc.X, gv.cc.floatyTextLoc.Y + txtH * 3, "yl");
        }
        public void drawHPText()
        {
            if ((showHP) && (!animationsOn))
            {
                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    if (crt.hp > (int)(crt.hpMax * 0.66f))
                    {
                        drawText(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY), crt.hp + "", "gn");
                    }
                    else if (crt.hp > (int)(crt.hpMax * 0.33f))
                    {
                        drawText(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY), crt.hp + "", "yl");
                    }
                    else
                    {
                        drawText(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY), crt.hp + "", "rd");
                    }
                }
                foreach (Player pc in mod.playerList)
                {
                    if (pc.hp > (int)(pc.hpMax * 0.66f))
                    {
                        drawText(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY), pc.hp + "", "gn");
                    }
                    else if (pc.hp > (int)(pc.hpMax * 0.33f))
                    {
                        drawText(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY), pc.hp + "", "yl");
                    }
                    else
                    {
                        drawText(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY), pc.hp + "", "rd");
                    }
                }
            }
        }
        public void drawSPText()
        {
            if ((showSP) && (!animationsOn))
            {
                int txtH = gv.fontHeight + 2;
                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    drawText(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY) + txtH, crt.sp + "", "yl");                    
                }
                foreach (Player pc in mod.playerList)
                {
                    drawText(getPixelLocX(pc.combatLocX), getPixelLocY(pc.combatLocY) + txtH, pc.sp + "", "yl");                    
                }
            }
        }
        public void drawText(int xLoc, int yLoc, string text, string colr)
        {
            int txtH = (int)gv.fontHeight;

            for (int x = 0; x <= 2; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    gv.DrawText(text, xLoc + x, yLoc + txtH + y, "bk");
                }
            }
            gv.DrawText(text, xLoc, yLoc + txtH, colr);
        }
        public void drawFloatyTextList()
        {
            if (floatyTextOn)
            {
                int txtH = (int)gv.fontHeight;

                foreach (FloatyText ft in gv.cc.floatyTextList)
                {
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            gv.DrawText(ft.value, ft.location.X + x + mapStartLocXinPixels, ft.location.Y + y, "bk");
                        }
                    }
                    string colr = "yl";
                    if (ft.color.Equals("yellow"))
                    {
                        colr = "yl";
                    }
                    else if (ft.color.Equals("blue"))
                    {
                        colr = "bu";
                    }
                    else if (ft.color.Equals("green"))
                    {
                        colr = "gn";
                    }
                    else
                    {
                        colr = "rd";
                    }
                    gv.DrawText(ft.value, ft.location.X + mapStartLocXinPixels, ft.location.Y, colr);
                }
            }
        }
        public void drawOverlayTints()
        {
            IbRect src = new IbRect(0, 0, gv.cc.tint_sunset.PixelSize.Width, gv.cc.tint_sunset.PixelSize.Height);
            IbRect dst = new IbRect(mapStartLocXinPixels, 0, gv.squareSize * (gv.playerOffsetX + gv.playerOffsetX + 1), gv.squareSize * (gv.playerOffsetY + gv.playerOffsetY + 2));
            int dawn = 5 * 60;
            int sunrise = 6 * 60;
            int day = 7 * 60;
            int sunset = 17 * 60;
            int dusk = 18 * 60;
            int night = 20 * 60;
            int time = gv.mod.WorldTime % 1440;
            if ((time >= dawn) && (time < sunrise))
            {
                gv.DrawBitmap(gv.cc.tint_dawn, src, dst);
            }
            else if ((time >= sunrise) && (time < day))
            {
                gv.DrawBitmap(gv.cc.tint_sunrise, src, dst);
            }
            else if ((time >= day) && (time < sunset))
            {
                //no tint for day
            }
            else if ((time >= sunset) && (time < dusk))
            {
                gv.DrawBitmap(gv.cc.tint_sunset, src, dst);
            }
            else if ((time >= dusk) && (time < night))
            {
                gv.DrawBitmap(gv.cc.tint_dusk, src, dst);
            }
            else if ((time >= night) || (time < dawn))
            {
                gv.DrawBitmap(gv.cc.tint_night, src, dst);
            }

        }
        public void drawSprites()
        {
            foreach (Sprite spr in spriteList)
            {
                spr.Draw(gv);
            }
            if (animationsOn)
            {
                if (attackAnimationTimeElapsed >= attackAnimationLengthInMilliseconds)
                {
                    foreach (AnimationSequence seq in animationSeqStack)
                    {
                        if (seq.AnimationSeq.Count > 0)
                        {
                            foreach (Sprite spr in seq.AnimationSeq[0].SpriteGroup)
                            {
                                //just draw the group at the top of the stack, first in first
                                spr.Draw(gv);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Keyboard Input
        public void onKeyUp(Keys keyData)
        {
            if (keyData == Keys.M)
            {
                if (canMove)
                {
                    currentCombatMode = "move";
                    gv.screenType = "combat";
                }
            }
            else if (keyData == Keys.A)
            {
                Player pc = mod.playerList[currentPlayerIndex];
                currentCombatMode = "attack";
                gv.screenType = "combat";
                setTargetHighlightStartLocation(pc);
            }
            else if (keyData == Keys.P)
            {
                if (currentPlayerIndex > mod.playerList.Count - 1)
                {
                    return;
                }
                gv.cc.partyScreenPcIndex = currentPlayerIndex;
                gv.screenParty.resetPartyScreen();
                gv.screenType = "combatParty";
            }
            else if (keyData == Keys.I)
            {
                gv.screenType = "combatInventory";
                gv.screenInventory.resetInventory();
            }
            else if (keyData == Keys.S)
            {
                gv.screenType = "combat";
                endPcTurn(false);
            }
            else if (keyData == Keys.C)
            {
                Player pc = mod.playerList[currentPlayerIndex];
                if (pc.knownSpellsTags.Count > 0)
                {
                    currentCombatMode = "castSelector";
                    gv.screenType = "combatCast";
                    gv.screenCastSelector.castingPlayerIndex = currentPlayerIndex;
                    spellSelectorIndex = 0;
                    setTargetHighlightStartLocation(pc);
                }
                else
                {
                    //TODO Toast.makeText(gv.gameContext, "PC has no Spells", Toast.LENGTH_SHORT).show();
                }
            }
            /*else if (keyData == Keys.X)
            {
                foreach (IB2Panel pnl in combatUiLayout.panelList)
                {
                    //hides left
                    if (pnl.hidingXIncrement < 0)
                    {
                        if (pnl.currentLocX < pnl.shownLocX)
                        {
                            pnl.showing = true;
                        }
                        else
                        {
                            pnl.hiding = true;
                        }
                    }
                    //hides right
                    else if (pnl.hidingXIncrement > 0)
                    {
                        if (pnl.currentLocX > pnl.shownLocX)
                        {
                            pnl.showing = true;
                        }
                        else
                        {
                            pnl.hiding = true;
                        }
                    }
                    //hides down
                    else if (pnl.hidingYIncrement > 0)
                    {
                        if (pnl.currentLocY > pnl.shownLocY)
                        {
                            if ((pnl.tag.Equals("arrowPanel")) && (!showArrows)) //don't show arrows
                            {
                                continue;
                            }
                            if ((pnl.tag.Equals("TogglePanel")) && (!showTogglePanel)) //don't show toggles
                            {
                                continue;
                            }
                            pnl.showing = true;
                        }
                        else
                        {
                            pnl.hiding = true;
                        }
                    }
                    //hides up
                    else if (pnl.hidingYIncrement < 0)
                    {
                        if (pnl.currentLocY < pnl.shownLocY)
                        {
                            pnl.showing = true;
                        }
                        else
                        {
                            pnl.hiding = true;
                        }
                    }
                }
            }*/

            #region Move PC mode
            if (currentCombatMode.Equals("move"))
            {
                Player pc = mod.playerList[currentPlayerIndex];
                if (keyData == Keys.NumPad7)
                {
                    MoveUpLeft(pc);
                }
                else if (keyData == Keys.NumPad8)
                {
                    MoveUp(pc);
                }
                else if (keyData == Keys.NumPad9)
                {
                    MoveUpRight(pc);
                }
                else if (keyData == Keys.NumPad4)
                {
                    MoveLeft(pc);
                }
                else if (keyData == Keys.NumPad5)
                {
                    //CenterScreenOnPC();
                }
                else if (keyData == Keys.NumPad6)
                {
                    MoveRight(pc);
                }
                else if (keyData == Keys.NumPad1)
                {
                    MoveDownLeft(pc);
                }
                else if (keyData == Keys.NumPad2)
                {
                    MoveDown(pc);
                }
                else if (keyData == Keys.NumPad3)
                {
                    MoveDownRight(pc);
                }
                return;
            }
            #endregion
            #region Move Targeting Mode
            if (currentCombatMode.Equals("attack"))
            {
                Player pc = mod.playerList[currentPlayerIndex];
                if (keyData == Keys.NumPad5)
                {
                    TargetAttackPressed(pc);
                    return;
                }
            }
            if (currentCombatMode.Equals("cast"))
            {
                Player pc = mod.playerList[currentPlayerIndex];
                if (keyData == Keys.NumPad5)
                {
                    TargetCastPressed(pc);
                    return;
                }
            }
            if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
            {
                if (keyData == Keys.NumPad7)
                {
                    MoveTargetHighlight(7);
                }
                else if (keyData == Keys.NumPad8)
                {
                    MoveTargetHighlight(8);
                }
                else if (keyData == Keys.NumPad9)
                {
                    MoveTargetHighlight(9);
                }
                else if (keyData == Keys.NumPad4)
                {
                    MoveTargetHighlight(4);
                }
                else if (keyData == Keys.NumPad6)
                {
                    MoveTargetHighlight(6);
                }
                else if (keyData == Keys.NumPad1)
                {
                    MoveTargetHighlight(1);
                }
                else if (keyData == Keys.NumPad2)
                {
                    MoveTargetHighlight(2);
                }
                else if (keyData == Keys.NumPad3)
                {
                    MoveTargetHighlight(3);
                }
                return;
            }
            #endregion
        }
        #endregion

        #region Mouse Input
        public void onTouchCombat(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType)
        {
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
                    }

                    //NEW SYSTEM
                    combatUiLayout.setHover(x, y);

                    int gridx = (int)(eX - mapStartLocXinPixels) / gv.squareSize;
                    int gridy = (int)(eY) / gv.squareSize;

                    #region FloatyText
                    if (IsInCombatWindow(eX, eY))
                    {
                        gv.cc.floatyText = "";
                        gv.cc.floatyText2 = "";
                        gv.cc.floatyText3 = "";
                        foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                        {
                            //1=normal, 2=wide, 3=tall, 4=large
                            int crtSize = gv.cc.getCreatureSize(crt.cr_tokenFilename);
                            //normal
                            if (crtSize == 1)
                            {
                                if ((crt.combatLocX == gridx) && (crt.combatLocY == gridy))
                                {
                                    gv.cc.floatyText = crt.cr_name;
                                    gv.cc.floatyText2 = "HP:" + crt.hp + " SP:" + crt.sp;
                                    gv.cc.floatyText3 = "AC:" + crt.AC + " " + crt.cr_status;
                                    gv.cc.floatyTextLoc = new Coordinate(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY));
                                }
                            }
                            //wide
                            else if (crtSize == 2)
                            {
                                if (((crt.combatLocX == gridx) && (crt.combatLocY == gridy)) ||
                                    ((crt.combatLocX + 1 == gridx) && (crt.combatLocY == gridy)))
                                {
                                    gv.cc.floatyText = crt.cr_name;
                                    gv.cc.floatyText2 = "HP:" + crt.hp + " SP:" + crt.sp;
                                    gv.cc.floatyText3 = "AC:" + crt.AC + " " + crt.cr_status;
                                    gv.cc.floatyTextLoc = new Coordinate(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY));
                                }
                            }
                            //tall
                            else if (crtSize == 3)
                            {
                                if (((crt.combatLocX == gridx) && (crt.combatLocY == gridy)) ||
                                    ((crt.combatLocX == gridx) && (crt.combatLocY + 1 == gridy)))
                                {
                                    gv.cc.floatyText = crt.cr_name;
                                    gv.cc.floatyText2 = "HP:" + crt.hp + " SP:" + crt.sp;
                                    gv.cc.floatyText3 = "AC:" + crt.AC + " " + crt.cr_status;
                                    gv.cc.floatyTextLoc = new Coordinate(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY));
                                }
                            }
                            //large
                            else if (crtSize == 4)
                            {
                                if (((crt.combatLocX == gridx) && (crt.combatLocY == gridy)) ||
                                    ((crt.combatLocX + 1 == gridx) && (crt.combatLocY == gridy)) ||
                                    ((crt.combatLocX == gridx) && (crt.combatLocY + 1 == gridy)) ||
                                    ((crt.combatLocX + 1 == gridx) && (crt.combatLocY + 1 == gridy)))
                                {
                                    gv.cc.floatyText = crt.cr_name;
                                    gv.cc.floatyText2 = "HP:" + crt.hp + " SP:" + crt.sp;
                                    gv.cc.floatyText3 = "AC:" + crt.AC + " " + crt.cr_status;
                                    gv.cc.floatyTextLoc = new Coordinate(getPixelLocX(crt.combatLocX), getPixelLocY(crt.combatLocY));
                                }
                            }
                        }
                        foreach (Player pc1 in mod.playerList)
                        {
                            if ((pc1.combatLocX == gridx) && (pc1.combatLocY == gridy))
                            {
                                string am = "";
                                ItemRefs itr = mod.getItemRefsInInventoryByResRef(pc1.AmmoRefs.resref);
                                if (itr != null)
                                {
                                    am = itr.quantity + "";
                                }
                                else
                                {
                                    am = "";
                                }

                                gv.cc.floatyText = pc1.name;
                                int actext = 0;
                                if (mod.ArmorClassAscending) { actext = pc1.AC; }
                                else { actext = 20 - pc1.AC; }
                                gv.cc.floatyText2 = "AC:" + actext + " " + pc1.charStatus;
                                gv.cc.floatyText3 = "Ammo: " + am;
                                gv.cc.floatyTextLoc = new Coordinate(getPixelLocX(pc1.combatLocX), getPixelLocY(pc1.combatLocY));
                            }
                        }
                    }
                    #endregion

                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)eX;
                    y = (int)eY;

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
                            return;
                        }
                    }

                    //NEW SYSTEM FOR GLOW
                    //combatUiLayout.setHover(-1, -1);

                    Player pc = mod.playerList[currentPlayerIndex];

                    //NEW SYSTEM
                    string rtn = combatUiLayout.getImpact(x, y);
                    //gv.cc.addLogText("lime", "mouse down: " + rtn);

                    #region Toggles
                    if (rtn.Equals("tglHP"))
                    {
                        IB2ToggleButton tgl = combatUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        tgl.toggleOn = !tgl.toggleOn;
                        showHP = !showHP;
                    }
                    if (rtn.Equals("tglSP"))
                    {
                        IB2ToggleButton tgl = combatUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        tgl.toggleOn = !tgl.toggleOn;
                        showSP = !showSP;
                    }
                    if (rtn.Equals("tglMoveOrder"))
                    {
                        IB2ToggleButton tgl = combatUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        tgl.toggleOn = !tgl.toggleOn;
                        showMoveOrder = !showMoveOrder;
                    }
                    if (rtn.Equals("tglSpeed"))
                    {
                        IB2ToggleButton tgl = combatUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }

                        if (mod.combatAnimationSpeed == 100)
                        {
                            mod.combatAnimationSpeed = 50;
                            tgl.ImgOffFilename = "tgl_speed_2";
                            gv.cc.addLogText("lime", "combat speed: 2x");
                        }
                        else if (mod.combatAnimationSpeed == 50)
                        {
                            mod.combatAnimationSpeed = 25;
                            tgl.ImgOffFilename = "tgl_speed_4";
                            gv.cc.addLogText("lime", "combat speed: 4x");
                        }
                        else if (mod.combatAnimationSpeed == 25)
                        {
                            mod.combatAnimationSpeed = 10;
                            tgl.ImgOffFilename = "tgl_speed_10";
                            gv.cc.addLogText("lime", "combat speed: 10x");
                        }
                        else if (mod.combatAnimationSpeed == 10)
                        {
                            mod.combatAnimationSpeed = 100;
                            tgl.ImgOffFilename = "tgl_speed_1";
                            gv.cc.addLogText("lime", "combat speed: 1x");
                        }
                    }
                    if (rtn.Equals("tglSoundFx"))
                    {
                        IB2ToggleButton tgl = combatUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            mod.playSoundFx = false;
                            gv.cc.addLogText("lime", "SoundFX Off");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            mod.playSoundFx = true;
                            gv.cc.addLogText("lime", "SoundFX On");
                        }
                    }
                    if (rtn.Equals("tglGrid"))
                    {
                        IB2ToggleButton tgl = combatUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            mod.com_showGrid = false;
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            mod.com_showGrid = true;
                        }
                    }
                    if (rtn.Equals("tglHelp"))
                    {                        
                        //tutorialMessageCombat(true);
                        gv.cc.tutorialMessageCombat(true);
                    }
                    if ((rtn.Equals("tglKill")) && (mod.debugMode))
                    {
                        mod.currentEncounter.encounterCreatureList.Clear();
                        mod.currentEncounter.encounterCreatureRefsList.Clear();
                        checkEndEncounter();
                    }
                    #endregion

                    #region TOUCH ON MAP AREA
                    gridx = ((int)(eX - mapStartLocXinPixels) / gv.squareSize);
                    gridy = ((int)(eY) / gv.squareSize);

                    gv.cc.floatyText = "";
                    gv.cc.floatyText2 = "";
                    gv.cc.floatyText3 = "";
                    if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
                    {
                        if (IsInCombatWindow(eX, eY))
                        {
                            //Check for second tap so TARGET
                            if ((gridx == targetHighlightCenterLocation.X) && (gridy == targetHighlightCenterLocation.Y))
                            {
                                if (currentCombatMode.Equals("attack"))
                                {
                                    TargetAttackPressed(pc);
                                }
                                else if (currentCombatMode.Equals("cast"))
                                {
                                    TargetCastPressed(pc);
                                }
                            }
                            targetHighlightCenterLocation.Y = gridy;
                            targetHighlightCenterLocation.X = gridx;
                            return;
                        }
                    }
                    
                    #endregion

                    #region BUTTONS
                    if ((rtn.Equals("ctrlUpArrow")) || ((gridx == pc.combatLocX) && (gridy == pc.combatLocY - 1) && (IsInCombatWindow(eX,eY))))
                    {
                        if (currentCombatMode.Equals("move"))
                        {
                            MoveUp(pc);
                        }
                        else if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
                        {
                            MoveTargetHighlight(8);
                        }
                    }
                    else if ((rtn.Equals("ctrlDownArrow")) || ((gridx == pc.combatLocX) && (gridy == pc.combatLocY + 1) && (IsInCombatWindow(eX, eY))))
                    {
                        if (currentCombatMode.Equals("move"))
                        {
                            MoveDown(pc);
                        }
                        else if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
                        {
                            MoveTargetHighlight(2);
                        }
                    }
                    else if ((rtn.Equals("ctrlLeftArrow")) || ((gridx == pc.combatLocX - 1) && (gridy == pc.combatLocY) && (IsInCombatWindow(eX, eY))))
                    {
                        if (currentCombatMode.Equals("move"))
                        {
                            MoveLeft(pc);
                        }
                        else if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
                        {
                            MoveTargetHighlight(4);
                        }
                    }
                    else if ((rtn.Equals("ctrlRightArrow")) || ((gridx == pc.combatLocX + 1) && (gridy == pc.combatLocY) && (IsInCombatWindow(eX, eY))))
                    {
                        if (currentCombatMode.Equals("move"))
                        {
                            MoveRight(pc);
                        }
                        else if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
                        {
                            MoveTargetHighlight(6);
                        }
                    }
                    else if ((rtn.Equals("ctrlUpRightArrow")) || ((gridx == pc.combatLocX + 1) && (gridy == pc.combatLocY - 1) && (IsInCombatWindow(eX, eY))))
                    {
                        if (currentCombatMode.Equals("move"))
                        {
                            MoveUpRight(pc);
                        }
                        else if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
                        {
                            MoveTargetHighlight(9);
                        }
                    }
                    else if ((rtn.Equals("ctrlDownRightArrow")) || ((gridx == pc.combatLocX + 1) && (gridy == pc.combatLocY + 1) && (IsInCombatWindow(eX, eY))))
                    {
                        if (currentCombatMode.Equals("move"))
                        {
                            MoveDownRight(pc);
                        }
                        else if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
                        {
                            MoveTargetHighlight(3);
                        }
                    }
                    else if ((rtn.Equals("ctrlUpLeftArrow")) || ((gridx == pc.combatLocX - 1) && (gridy == pc.combatLocY - 1) && (IsInCombatWindow(eX, eY))))
                    {
                        if (currentCombatMode.Equals("move"))
                        {
                            MoveUpLeft(pc);
                        }
                        else if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
                        {
                            MoveTargetHighlight(7);
                        }
                    }
                    else if ((rtn.Equals("ctrlDownLeftArrow")) || ((gridx == pc.combatLocX - 1) && (gridy == pc.combatLocY + 1) && (IsInCombatWindow(eX, eY))))
                    {
                        if (currentCombatMode.Equals("move"))
                        {
                            MoveDownLeft(pc);
                        }
                        else if ((currentCombatMode.Equals("attack")) || (currentCombatMode.Equals("cast")))
                        {
                            MoveTargetHighlight(1);
                        }
                    }
                    else if (rtn.Equals("btnSwitchWeapon"))
                    {
                        if (currentPlayerIndex > mod.playerList.Count - 1)
                        {
                            return;
                        }
                        gv.cc.partyScreenPcIndex = currentPlayerIndex;
                        gv.screenParty.resetPartyScreen();
                        gv.screenType = "combatParty";
                    }
                    else if (rtn.Equals("btnMove"))
                    {
                        if (canMove)
                        {
                            if (currentCombatMode.Equals("move"))
                            {
                                currentCombatMode = "info";
                            }
                            else
                            {
                                currentCombatMode = "move";
                            }
                            gv.screenType = "combat";
                        }
                    }
                    else if (rtn.Equals("btnInventory"))
                    {
                        gv.screenType = "combatInventory";
                        gv.screenInventory.resetInventory();
                    }
                    else if (rtn.Equals("btnAttack"))
                    {
                        if (currentCombatMode.Equals("attack"))
                        {
                            currentCombatMode = "info";
                        }
                        else
                        {
                            currentCombatMode = "attack";
                        }
                        gv.screenType = "combat";
                        setTargetHighlightStartLocation(pc);
                    }
                    else if (rtn.Equals("btnCast"))
                    {
                        if (pc.knownSpellsTags.Count > 0)
                        {
                            currentCombatMode = "castSelector";
                            gv.screenType = "combatCast";
                            gv.screenCastSelector.castingPlayerIndex = currentPlayerIndex;
                            spellSelectorIndex = 0;
                            setTargetHighlightStartLocation(pc);
                        }
                        else
                        {
                            //TODO Toast.makeText(gv.gameContext, "PC has no Spells", Toast.LENGTH_SHORT).show();
                        }
                    }
                    else if (rtn.Equals("btnSkipTurn"))
                    {
                        gv.screenType = "combat";
                        endPcTurn(false);
                    }
                    else if (rtn.Equals("btnSelect"))
                    {
                        if (currentCombatMode.Equals("attack"))
                        {
                            TargetAttackPressed(pc);
                        }
                        else if (currentCombatMode.Equals("cast"))
                        {
                            TargetCastPressed(pc);
                        }
                    }
                    else if (rtn.Equals("btnToggleArrows"))
                    {
                        foreach (IB2Panel pnl in combatUiLayout.panelList)
                        {
                            if (pnl.tag.Equals("arrowPanel"))
                            {
                                //hides down
                                showArrows = !showArrows;
                                if (pnl.currentLocY > pnl.shownLocY)
                                {
                                    pnl.showing = true;
                                }
                                else
                                {
                                    pnl.hiding = true;
                                }
                            }
                        }
                    }
                    else if (rtn.Equals("btnSettings"))
                    {
                        //gv.cc.doSettingsDialogs();
                        foreach (IB2Panel pnl in combatUiLayout.panelList)
                        {
                            if (pnl.tag.Equals("TogglePanel"))
                            {
                                showTogglePanel = !showTogglePanel;
                                //hides down
                                if (pnl.currentLocY > pnl.shownLocY)
                                {
                                    pnl.showing = true;
                                }
                                else
                                {
                                    pnl.hiding = true;
                                }
                            }
                        }
                    }
                    break;
                    #endregion
            }
        }

        #endregion
        public bool IsInCombatWindow(int mouseX, int mouseY)
        {
            //all coordinates in screen location pixels
            int top = 0;
            int bottom = (gv.squareSize * 11);
            int left = mapStartLocXinPixels;
            int right = mapStartLocXinPixels + (gv.squareSize * 11);
            if ((mouseX >= left) && (mouseX <= right) && (mouseY >= top) && (mouseY <= bottom))
            {
                return true;
            }
            return false;
        }
        public void doUpdate(Player pc)
        {
            //CalculateUpperLeft();            
            if (moveCost == mod.diagonalMoveCost)
            {
                currentMoves += mod.diagonalMoveCost;
                moveCost = 1.0f;
            }
            else
            {
                currentMoves++;
            }
            float moveleft = pc.moveDistance - currentMoves;
            if (moveleft < 1) { moveleft = 0; }
            //do triggers and anything else needed after each creature or PC move
            afterEachMoveCalls();
        }
        public void MoveTargetHighlight(int numPadDirection)
        {
            switch (numPadDirection)
            {
                case 8: //up
                    if (targetHighlightCenterLocation.Y > 0)
                    {
                        targetHighlightCenterLocation.Y--;                        
                    }
                    break;
                case 2: //down
                    if (targetHighlightCenterLocation.Y < mod.currentEncounter.MapSizeY - 1)
                    {
                        targetHighlightCenterLocation.Y++;                        
                    }
                    break;
                case 4: //left
                    if (targetHighlightCenterLocation.X > 0)
                    {
                        targetHighlightCenterLocation.X--;                        
                    }
                    break;
                case 6: //right
                    if (targetHighlightCenterLocation.X < mod.currentEncounter.MapSizeX - 1)
                    {
                        targetHighlightCenterLocation.X++;                        
                    }
                    break;
                case 9: //upright
                    if ((targetHighlightCenterLocation.X < mod.currentEncounter.MapSizeX - 1) && (targetHighlightCenterLocation.Y > 0))
                    {
                        targetHighlightCenterLocation.X++;
                        targetHighlightCenterLocation.Y--;                        
                    }
                    break;
                case 3: //downright
                    if ((targetHighlightCenterLocation.X < mod.currentEncounter.MapSizeX - 1) && (targetHighlightCenterLocation.Y < mod.currentEncounter.MapSizeY - 1))
                    {
                        targetHighlightCenterLocation.X++;
                        targetHighlightCenterLocation.Y++;                        
                    }
                    break;
                case 7: //upleft
                    if ((targetHighlightCenterLocation.X > 0) && (targetHighlightCenterLocation.Y > 0))
                    {
                        targetHighlightCenterLocation.X--;
                        targetHighlightCenterLocation.Y--;                        
                    }
                    break;
                case 1: //downleft
                    if ((targetHighlightCenterLocation.X > 0) && (targetHighlightCenterLocation.Y < mod.currentEncounter.MapSizeY - 1))
                    {
                        targetHighlightCenterLocation.X--;
                        targetHighlightCenterLocation.Y++;                        
                    }
                    break;
            }

        }
        public void MoveUp(Player pc)
        {
            if (pc.combatLocY > 0)
            {
                //check is walkable (blocked square or PC)
                if (isWalkable(pc.combatLocX, pc.combatLocY - 1))
                {
                    //check if creature -> do attack
                    Creature c = isBumpIntoCreature(pc.combatLocX, pc.combatLocY - 1);
                    if (c != null)
                    {
                        //attack creature
                        targetHighlightCenterLocation.X = pc.combatLocX;
                        targetHighlightCenterLocation.Y = pc.combatLocY - 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= 1.0f)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX, pc.combatLocY - 1);
                        doPlayerCombatFacing(pc, pc.combatLocX, pc.combatLocY - 1);
                        pc.combatLocY--;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveUpRight(Player pc)
        {
            if ((pc.combatLocX < mod.currentEncounter.MapSizeX - 1) && (pc.combatLocY > 0))
            {
                if (isWalkable(pc.combatLocX + 1, pc.combatLocY - 1))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX + 1, pc.combatLocY - 1);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX + 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY - 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= mod.diagonalMoveCost)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX + 1, pc.combatLocY - 1);
                        doPlayerCombatFacing(pc, pc.combatLocX + 1, pc.combatLocY - 1);
                        pc.combatLocX++;
                        pc.combatLocY--;
                        if (pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = false;
                        }
                        moveCost = mod.diagonalMoveCost;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveUpLeft(Player pc)
        {
            if ((pc.combatLocX > 0) && (pc.combatLocY > 0))
            {
                if (isWalkable(pc.combatLocX - 1, pc.combatLocY - 1))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX - 1, pc.combatLocY - 1);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX - 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY - 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= mod.diagonalMoveCost)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX - 1, pc.combatLocY - 1);
                        doPlayerCombatFacing(pc, pc.combatLocX - 1, pc.combatLocY - 1);
                        pc.combatLocX--;
                        pc.combatLocY--;
                        if (!pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = true;
                        }
                        moveCost = mod.diagonalMoveCost;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveDown(Player pc)
        {
            if (pc.combatLocY < mod.currentEncounter.MapSizeY - 1)
            {
                if (isWalkable(pc.combatLocX, pc.combatLocY + 1))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX, pc.combatLocY + 1);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX;
                        targetHighlightCenterLocation.Y = pc.combatLocY + 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= 1.0f)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX, pc.combatLocY + 1);
                        doPlayerCombatFacing(pc, pc.combatLocX, pc.combatLocY + 1);
                        pc.combatLocY++;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveDownRight(Player pc)
        {
            if ((pc.combatLocX < mod.currentEncounter.MapSizeX - 1) && (pc.combatLocY < mod.currentEncounter.MapSizeY - 1))
            {
                if (isWalkable(pc.combatLocX + 1, pc.combatLocY + 1))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX + 1, pc.combatLocY + 1);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX + 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY + 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= mod.diagonalMoveCost)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX + 1, pc.combatLocY + 1);
                        doPlayerCombatFacing(pc, pc.combatLocX + 1, pc.combatLocY + 1);
                        pc.combatLocX++;
                        pc.combatLocY++;
                        if (pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = false;
                        }
                        moveCost = mod.diagonalMoveCost;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveDownLeft(Player pc)
        {
            if ((pc.combatLocX > 0) && (pc.combatLocY < mod.currentEncounter.MapSizeY - 1))
            {
                if (isWalkable(pc.combatLocX - 1, pc.combatLocY + 1))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX - 1, pc.combatLocY + 1);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX - 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY + 1;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= mod.diagonalMoveCost)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX - 1, pc.combatLocY + 1);
                        doPlayerCombatFacing(pc, pc.combatLocX - 1, pc.combatLocY + 1);
                        pc.combatLocX--;
                        pc.combatLocY++;
                        if (!pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = true;
                        }
                        moveCost = mod.diagonalMoveCost;
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveRight(Player pc)
        {
            if (pc.combatLocX < mod.currentEncounter.MapSizeX - 1)
            {
                if (isWalkable(pc.combatLocX + 1, pc.combatLocY))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX + 1, pc.combatLocY);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX + 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= 1.0f)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX + 1, pc.combatLocY);
                        doPlayerCombatFacing(pc, pc.combatLocX + 1, pc.combatLocY);
                        pc.combatLocX++;
                        if (pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = false;
                        }
                        doUpdate(pc);
                    }
                }
            }
        }
        public void MoveLeft(Player pc)
        {
            if (pc.combatLocX > 0)
            {
                if (isWalkable(pc.combatLocX - 1, pc.combatLocY))
                {
                    Creature c = isBumpIntoCreature(pc.combatLocX - 1, pc.combatLocY);
                    if (c != null)
                    {

                        targetHighlightCenterLocation.X = pc.combatLocX - 1;
                        targetHighlightCenterLocation.Y = pc.combatLocY;
                        currentCombatMode = "attack";
                        TargetAttackPressed(pc);
                    }
                    else if ((pc.moveDistance - currentMoves) >= 1.0f)
                    {
                        LeaveThreatenedCheck(pc, pc.combatLocX, pc.combatLocY);
                        doPlayerCombatFacing(pc, pc.combatLocX - 1, pc.combatLocY);
                        pc.combatLocX--;
                        if (!pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = true;
                        }
                        doUpdate(pc);
                    }
                }
            }
        }
        public void TargetAttackPressed(Player pc)
        {
            if (isValidAttackTarget(pc))
            {
                if ((targetHighlightCenterLocation.X < pc.combatLocX) && (!pc.combatFacingLeft)) //attack left
                {
                    pc.combatFacingLeft = true;
                }
                else if ((targetHighlightCenterLocation.X > pc.combatLocX) && (pc.combatFacingLeft)) //attack right
                {
                    pc.combatFacingLeft = false;
                }
                doPlayerCombatFacing(pc, targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y);
                gv.touchEnabled = false;
                creatureToAnimate = null;
                playerToAnimate = pc;
                //set attack animation and do a delay
                attackAnimationTimeElapsed = 0;
                attackAnimationLengthInMilliseconds = (int)(5f * mod.combatAnimationSpeed);
                if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee"))
                        || (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).name.Equals("none"))
                        || (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
                {
                    //do melee attack stuff and animations  
                    AnimationSequence newSeq = new AnimationSequence();
                    animationSeqStack.Add(newSeq);
                    doCombatAttack(pc);
                    //add hit or miss animation
                    //add floaty text
                    //add death animations
                    AnimationStackGroup newGroup = new AnimationStackGroup();
                    animationSeqStack[0].AnimationSeq.Add(newGroup);
                    foreach (Coordinate coor in deathAnimationLocations)
                    {
                        addDeathAnimation(newGroup, new Coordinate(getPixelLocX(coor.X), getPixelLocY(coor.Y)));
                    }
                    animationsOn = true;
                }
                else //Ranged Attack
                {
                    //play attack sound for ranged
                    gv.PlaySound(mod.getItemByResRefForInfo(pc.MainHandRefs.resref).itemOnUseSound);
                    //do ranged attack stuff and animations
                    //add projectile animation
                    int startX = getPixelLocX(pc.combatLocX);
                    int startY = getPixelLocY(pc.combatLocY);
                    int endX = getPixelLocX(targetHighlightCenterLocation.X);
                    int endY = getPixelLocY(targetHighlightCenterLocation.Y);
                    string filename = mod.getItemByResRefForInfo(pc.AmmoRefs.resref).projectileSpriteFilename;
                    AnimationSequence newSeq = new AnimationSequence();
                    animationSeqStack.Add(newSeq);
                    AnimationStackGroup newGroup = new AnimationStackGroup();
                    newSeq.AnimationSeq.Add(newGroup);
                    launchProjectile(filename, startX, startY, endX, endY, newGroup);
                    //add ending projectile animation  
                    doCombatAttack(pc);
                    //add hit or miss animation
                    //add floaty text
                    //add death animations
                    newGroup = new AnimationStackGroup();
                    animationSeqStack[0].AnimationSeq.Add(newGroup);
                    foreach (Coordinate coor in deathAnimationLocations)
                    {
                        addDeathAnimation(newGroup, new Coordinate(getPixelLocX(coor.X), getPixelLocY(coor.Y)));
                    }
                    animationsOn = true;
                }
            }
        }
        public void TargetCastPressed(Player pc)
        {
            //Uses Map Pixel Locations
            int endX = targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
            int endY = targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);
            int startX = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
            int startY = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);

            if ((isValidCastTarget(pc)) && (isVisibleLineOfSight(new Coordinate(endX, endY), new Coordinate(startX, startY))))
            {
                if ((targetHighlightCenterLocation.X < pc.combatLocX) && (!pc.combatFacingLeft)) //attack left
                {
                    pc.combatFacingLeft = true;
                }
                else if ((targetHighlightCenterLocation.X > pc.combatLocX) && (pc.combatFacingLeft)) //attack right
                {
                    pc.combatFacingLeft = false;
                }
                doPlayerCombatFacing(pc, targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y);
                gv.touchEnabled = false;
                creatureToAnimate = null;
                playerToAnimate = pc;
                //set attack animation and do a delay
                attackAnimationTimeElapsed = 0;
                attackAnimationLengthInMilliseconds = (int)(5f * mod.combatAnimationSpeed);
                AnimationSequence newSeq = new AnimationSequence();
                animationSeqStack.Add(newSeq);
                //add projectile animation
                gv.PlaySound(gv.cc.currentSelectedSpell.spellStartSound);
                startX = getPixelLocX(pc.combatLocX);
                startY = getPixelLocY(pc.combatLocY);
                endX = getPixelLocX(targetHighlightCenterLocation.X);
                endY = getPixelLocY(targetHighlightCenterLocation.Y);
                string filename = gv.cc.currentSelectedSpell.spriteFilename;
                AnimationStackGroup newGroup = new AnimationStackGroup();
                newSeq.AnimationSeq.Add(newGroup);
                launchProjectile(filename, startX, startY, endX, endY, newGroup);
                //gv.PlaySound(gv.cc.currentSelectedSpell.spellEndSound);
                object target = getCastTarget(pc);
                gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, pc, target, false);
                //add ending projectile animation
                newGroup = new AnimationStackGroup();
                animationSeqStack[0].AnimationSeq.Add(newGroup);
                filename = gv.cc.currentSelectedSpell.spriteEndingFilename;
                foreach (Coordinate coor in gv.sf.AoeSquaresList)
                {
                    addEndingAnimation(newGroup, new Coordinate(getPixelLocX(coor.X), getPixelLocY(coor.Y)), filename);
                }
                //add floaty text
                //add death animations
                newGroup = new AnimationStackGroup();
                animationSeqStack[0].AnimationSeq.Add(newGroup);
                foreach (Coordinate coor in deathAnimationLocations)
                {
                    addDeathAnimation(newGroup, new Coordinate(getPixelLocX(coor.X), getPixelLocY(coor.Y)));
                }
                animationsOn = true;
            }
        }
        public void launchProjectile(string filename, int startX, int startY, int endX, int endY, AnimationStackGroup group)
        {
            //calculate angle from start to end point
            float angle = AngleRad(new Point(startX, startY), new Point(endX, endY));
            float dX = (endX - startX);
            float dY = (endY - startY);
            //calculate needed TimeToLive based on a constant speed for projectiles
            int ttl = 1000;
            float speed = 2f; //small number is faster travel speed
            if (Math.Abs(dX) > Math.Abs(dY))
            {
                ttl = (int)(Math.Abs(dX) * speed);
            }
            else
            {
                ttl = (int)(Math.Abs(dY) * speed);
            }
            SharpDX.Vector2 vel = SharpDX.Vector2.Normalize(new SharpDX.Vector2(dX, dY));
            Sprite spr = new Sprite(gv, filename, startX, startY, vel.X / speed, vel.Y / speed, angle, 0, 1.0f, ttl, false, 100);
            group.SpriteGroup.Add(spr);
        }
        public void addHitAnimation(AnimationStackGroup group)
        {
            int ttl = 8 * mod.combatAnimationSpeed;
            Sprite spr = new Sprite(gv, "hit_symbol", hitAnimationLocation.X, hitAnimationLocation.Y, 0, 0, 0, 0, 1.0f, ttl, false, ttl / 4);
            group.turnFloatyTextOn = true;
            group.SpriteGroup.Add(spr);
        }
        public void addMissAnimation(AnimationStackGroup group)
        {
            int ttl = 8 * mod.combatAnimationSpeed;
            Sprite spr = new Sprite(gv, "miss_symbol", hitAnimationLocation.X, hitAnimationLocation.Y, 0, 0, 0, 0, 1.0f, ttl, false, ttl / 4);
            group.SpriteGroup.Add(spr);
        }
        public void addDeathAnimation(AnimationStackGroup group, Coordinate Loc)
        {
            int ttl = 16 * mod.combatAnimationSpeed;
            Sprite spr = new Sprite(gv, "death_fx", Loc.X, Loc.Y, 0, 0, 0, 0, 1.0f, ttl, false, ttl / 4);
            group.SpriteGroup.Add(spr);
        }
        public void addEndingAnimation(AnimationStackGroup group, Coordinate Loc, string filename)
        {            
            int ttl = 16 * mod.combatAnimationSpeed;            
            Sprite spr = new Sprite(gv, filename, Loc.X, Loc.Y, 0, 0, 0, 0, 1.0f, ttl, false, ttl / 4);
            group.turnFloatyTextOn = true;
            group.SpriteGroup.Add(spr);
        }
        public float AngleRad(Point start, Point end)
        {
            return (float)(-1 * ((Math.Atan2(start.Y - end.Y, end.X - start.X)) - (Math.PI) / 2));
        }

        //Helper Methods
        public bool isSquareOnCombatMap(int x, int y)
        {
            if (x >= gv.mod.currentEncounter.MapSizeX)
            {
                return false;
            }
            if (x < 0)
            {
                return false;
            }
            if (y >= gv.mod.currentEncounter.MapSizeY)
            {
                return false;
            }
            if (y < 0)
            {
                return false;
            }
            return true;
        }
        public int getPixelLocX(int sqrX)
        {
            return ((sqrX) * gv.squareSize) + mapStartLocXinPixels;
            //return ((sqrX - UpperLeftSquare.X) * gv.squareSize) + gv.oXshift + mapStartLocXinPixels;
        }
        public int getPixelLocY(int sqrY)
        {
            return (sqrY) * gv.squareSize;
            //return (sqrY - UpperLeftSquare.Y) * gv.squareSize;
        }
        public void setTargetHighlightStartLocation(Player pc)
        {
            targetHighlightCenterLocation.X = pc.combatLocX;
            targetHighlightCenterLocation.Y = pc.combatLocY;
        }
        public bool isValidAttackTarget(Player pc)
        {
            if (isInRange(pc))
            {
                Item it = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                //if using ranged and have ammo, use ammo properties
                if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged"))
                        && (!mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
                {
                    //ranged weapon with ammo
                    it = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                }
                if (it == null)
                {
                    it = mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                }
                //check to see if is AoE or Point Target else needs a target PC or Creature
                if (it.AreaOfEffect > 0)
                {
                    return true;
                }

                //Uses the Map Pixel Locations
                int endX2 = targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
                int endY2 = targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);
                int startX2 = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
                int startY2 = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);

                if ((isVisibleLineOfSight(new Coordinate(endX2, endY2), new Coordinate(startX2, startY2)))
                    || (getDistance(new Coordinate(pc.combatLocX, pc.combatLocY), targetHighlightCenterLocation) == 1))
                {
                    foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                    {
                        foreach (Coordinate coor in crt.tokenCoveredSquares)
                        {
                            if ((coor.X == targetHighlightCenterLocation.X) && (coor.Y == targetHighlightCenterLocation.Y))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool isValidCastTarget(Player pc)
        {
            if (isInRange(pc))
            {
                //check to see if is AoE or Point Target else needs a target PC or Creature
                if ((gv.cc.currentSelectedSpell.aoeRadius > 0) || (gv.cc.currentSelectedSpell.spellTargetType.Equals("PointLocation")))
                {
                    return true;
                }
                //is not an AoE ranged attack, is a PC or Creature
                else
                {
                    //check to see if target is a friend or self
                    if ((gv.cc.currentSelectedSpell.spellTargetType.Equals("Friend")) || (gv.cc.currentSelectedSpell.spellTargetType.Equals("Self")))
                    {
                        foreach (Player p in mod.playerList)
                        {
                            if ((p.combatLocX == targetHighlightCenterLocation.X) && (p.combatLocY == targetHighlightCenterLocation.Y))
                            {
                                return true;
                            }
                        }
                    }
                    else //target is a creature
                    {
                        foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                        {
                            foreach (Coordinate coor in crt.tokenCoveredSquares)
                            {
                                if ((coor.X == targetHighlightCenterLocation.X) && (coor.Y == targetHighlightCenterLocation.Y))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public object getCastTarget(Player pc)
        {
            if (isInRange(pc))
            {
                //check to see if is AoE or Point Target else needs a target PC or Creature
                if ((gv.cc.currentSelectedSpell.aoeRadius > 0) || (gv.cc.currentSelectedSpell.spellTargetType.Equals("PointLocation")))
                {
                    return new Coordinate(targetHighlightCenterLocation.X, targetHighlightCenterLocation.Y);
                }
                //is not an AoE ranged attack, is a PC or Creature
                else
                {
                    //check to see if target is a friend or self
                    if ((gv.cc.currentSelectedSpell.spellTargetType.Equals("Friend")) || (gv.cc.currentSelectedSpell.spellTargetType.Equals("Self")))
                    {
                        foreach (Player p in mod.playerList)
                        {
                            if ((p.combatLocX == targetHighlightCenterLocation.X) && (p.combatLocY == targetHighlightCenterLocation.Y))
                            {
                                return p;
                            }
                        }
                    }
                    else //target is a creature
                    {
                        foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                        {
                            foreach (Coordinate coor in crt.tokenCoveredSquares)
                            {
                                if ((coor.X == targetHighlightCenterLocation.X) && (coor.Y == targetHighlightCenterLocation.Y))
                                {
                                    return crt;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        public bool isInRange(Player pc)
        {
            if (currentCombatMode.Equals("attack"))
            {
                int range = 1;
                if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged"))
                        && (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
                {
                    //ranged weapon with no ammo
                    range = 1;
                }
                else
                {
                    range = mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attackRange;
                }

                if (getDistance(new Coordinate(pc.combatLocX, pc.combatLocY), targetHighlightCenterLocation) <= range)
                {
                    return true;
                }
            }
            else if (currentCombatMode.Equals("cast"))
            {
                if (getDistance(new Coordinate(pc.combatLocX, pc.combatLocY), targetHighlightCenterLocation) <= gv.cc.currentSelectedSpell.range)
                {
                    return true;
                }
            }
            return false;
        }
        public bool isAdjacentEnemy(Player pc)
        {
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                foreach (Coordinate coor in crt.tokenCoveredSquares)
                {
                    if (getDistance(new Coordinate(pc.combatLocX, pc.combatLocY), new Coordinate(coor.X, coor.Y)) == 1)
                    {
                        if (!crt.isHeld())
                        {
                            if (crt.hp > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool isAdjacentPc(Creature crt)
        {
            foreach (Player pc in mod.playerList)
            {
                if (getDistance(new Coordinate(pc.combatLocX, pc.combatLocY), new Coordinate(crt.combatLocX, crt.combatLocY)) == 1)
                {
                    if ((pc.hp > 0) && (!pc.isHeld()))
                    { 
                       return true;
                    }
                }
            }
            return false;
        }
        public int getGridX(Coordinate nextPoint)
        {
            //int gridx = ((nextPoint.X - mapStartLocXinPixels - gv.oXshift) / gv.squareSize) + UpperLeftSquare.X;
            int gridx = ((nextPoint.X - mapStartLocXinPixels) / gv.squareSize);
            if (gridx > mod.currentEncounter.MapSizeX - 1) { gridx = mod.currentEncounter.MapSizeX - 1; }
            if (gridx < 0) { gridx = 0; }
            return gridx;
        }
        public int getGridY(Coordinate nextPoint)
        {
            //int gridy = ((nextPoint.Y - gv.oYshift) / gv.squareSize) + UpperLeftSquare.Y;
            int gridy = ((nextPoint.Y) / gv.squareSize);
            if (gridy > mod.currentEncounter.MapSizeY - 1) { gridy = mod.currentEncounter.MapSizeY - 1; }
            if (gridy < 0) { gridy = 0; }
            return gridy;
        }
        public int getMapSquareX(Coordinate nextPoint)
        {
            int gridx = (nextPoint.X / gv.squareSize);
            if (gridx > mod.currentEncounter.MapSizeX - 1) { gridx = mod.currentEncounter.MapSizeX - 1; }
            if (gridx < 0) { gridx = 0; }
            return gridx;
        }
        public int getMapSquareY(Coordinate nextPoint)
        {
            int gridy = (nextPoint.Y / gv.squareSize);
            if (gridy > mod.currentEncounter.MapSizeY - 1) { gridy = mod.currentEncounter.MapSizeY - 1; }
            if (gridy < 0) { gridy = 0; }
            return gridy;
        }
        public bool isVisibleLineOfSight(Coordinate end, Coordinate start)
        {
            //This Method Uses Map Pixel Locations Only

            int deltax = Math.Abs(end.X - start.X);
            int deltay = Math.Abs(end.Y - start.Y);
            int ystep = gv.squareSize / 50;
            int xstep = gv.squareSize / 50;
            if (ystep < 1) { ystep = 1; }
            if (xstep < 1) { xstep = 1; }

            if (deltax > deltay) //Low Angle line
            {
                Coordinate nextPoint = start;
                int error = deltax / 2;

                if (end.Y < start.Y) { ystep = -1 * ystep; } //down and right or left

                if (end.X > start.X) //down and right
                {
                    for (int x = start.X; x <= end.X; x += xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getMapSquareX(nextPoint);
                        int gridy = getMapSquareY(nextPoint);
                        if (mod.currentEncounter.LoSBlocked[gridy * mod.currentEncounter.MapSizeX + gridx] == 1)
                        {
                            return false;
                        }
                    }
                }
                else //down and left
                {
                    for (int x = start.X; x >= end.X; x -= xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getMapSquareX(nextPoint);
                        int gridy = getMapSquareY(nextPoint);
                        if (mod.currentEncounter.LoSBlocked[gridy * mod.currentEncounter.MapSizeX + gridx] == 1)
                        {
                            return false;
                        }
                    }
                }
            }

            else //Low Angle line
            {
                Coordinate nextPoint = start;
                int error = deltay / 2;

                if (end.X < start.X) { xstep = -1 * xstep; } //up and right or left

                if (end.Y > start.Y) //up and right
                {
                    for (int y = start.Y; y <= end.Y; y += ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getMapSquareX(nextPoint);
                        int gridy = getMapSquareY(nextPoint);
                        if (mod.currentEncounter.LoSBlocked[gridy * mod.currentEncounter.MapSizeX + gridx] == 1)
                        {
                            return false;
                        }
                    }
                }
                else //up and right
                {
                    for (int y = start.Y; y >= end.Y; y -= ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getMapSquareX(nextPoint);
                        int gridy = getMapSquareY(nextPoint);
                        if (mod.currentEncounter.LoSBlocked[gridy * mod.currentEncounter.MapSizeX + gridx] == 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        public bool drawVisibleLineOfSightTrail(Coordinate end, Coordinate start, Color penColor, int penWidth)
        {
            // Bresenham Line algorithm
            // Creates a line from Begin to End starting at (x0,y0) and ending at (x1,y1)
            // where x0 less than x1 and y0 less than y1
            // AND line is less steep than it is wide (dx less than dy)    

            int deltax = Math.Abs(end.X - start.X);
            int deltay = Math.Abs(end.Y - start.Y);
            int ystep = gv.squareSize / 50;
            int xstep = gv.squareSize / 50;
            if (ystep < 1) { ystep = 1; }
            if (xstep < 1) { xstep = 1; }

            if (deltax > deltay) //Low Angle line
            {
                Coordinate nextPoint = start;
                int error = deltax / 2;

                if (end.Y < start.Y) { ystep = -1 * ystep; } //down and right or left

                if (end.X > start.X) //down and right
                {
                    int lastX = start.X;
                    int lastY = start.Y;
                    for (int x = start.X; x <= end.X; x += xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getGridX(nextPoint);
                        int gridy = getGridY(nextPoint);
                        gv.DrawLine(lastX, lastY, nextPoint.X, nextPoint.Y, penColor, penWidth);
                        if (mod.currentEncounter.LoSBlocked[gridy * mod.currentEncounter.MapSizeX + gridx] == 1)
                        {
                            return false;
                        }
                        lastX = nextPoint.X;
                        lastY = nextPoint.Y;
                    }
                }
                else //down and left
                {
                    int lastX = start.X;
                    int lastY = start.Y;
                    for (int x = start.X; x >= end.X; x -= xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getGridX(nextPoint);
                        int gridy = getGridY(nextPoint);
                        gv.DrawLine(lastX, lastY, nextPoint.X, nextPoint.Y, penColor, penWidth);
                        if (mod.currentEncounter.LoSBlocked[gridy * mod.currentEncounter.MapSizeX + gridx] == 1)
                        {
                            return false;
                        }
                        lastX = nextPoint.X;
                        lastY = nextPoint.Y;
                    }
                }
            }

            else //Low Angle line
            {
                Coordinate nextPoint = start;
                int error = deltay / 2;

                if (end.X < start.X) { xstep = -1 * xstep; } //up and right or left

                if (end.Y > start.Y) //up and right
                {
                    int lastX = start.X;
                    int lastY = start.Y;
                    for (int y = start.Y; y <= end.Y; y += ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getGridX(nextPoint);
                        int gridy = getGridY(nextPoint);
                        gv.DrawLine(lastX, lastY, nextPoint.X, nextPoint.Y, penColor, penWidth);
                        if (mod.currentEncounter.LoSBlocked[gridy * mod.currentEncounter.MapSizeX + gridx] == 1)
                        {
                            return false;
                        }
                        lastX = nextPoint.X;
                        lastY = nextPoint.Y;
                    }
                }
                else //up and right
                {
                    int lastX = start.X;
                    int lastY = start.Y;
                    for (int y = start.Y; y >= end.Y; y -= ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        int gridx = getGridX(nextPoint);
                        int gridy = getGridY(nextPoint);
                        gv.DrawLine(lastX, lastY, nextPoint.X, nextPoint.Y, penColor, penWidth);
                        if (mod.currentEncounter.LoSBlocked[gridy * mod.currentEncounter.MapSizeX + gridx] == 1)
                        {
                            return false;
                        }
                        lastX = nextPoint.X;
                        lastY = nextPoint.Y;
                    }
                }
            }

            return true;
        }
        public bool IsAttackFromBehind(Player pc, Creature crt)
        {
            if ((pc.combatLocX > crt.combatLocX) && (pc.combatLocY > crt.combatLocY) && (crt.combatFacing == 7)) { return true; }
            if ((pc.combatLocX == crt.combatLocX) && (pc.combatLocY > crt.combatLocY) && (crt.combatFacing == 8)) { return true; }
            if ((pc.combatLocX < crt.combatLocX) && (pc.combatLocY > crt.combatLocY) && (crt.combatFacing == 9)) { return true; }
            if ((pc.combatLocX > crt.combatLocX) && (pc.combatLocY == crt.combatLocY) && (crt.combatFacing == 4)) { return true; }
            if ((pc.combatLocX < crt.combatLocX) && (pc.combatLocY == crt.combatLocY) && (crt.combatFacing == 6)) { return true; }
            if ((pc.combatLocX > crt.combatLocX) && (pc.combatLocY < crt.combatLocY) && (crt.combatFacing == 1)) { return true; }
            if ((pc.combatLocX == crt.combatLocX) && (pc.combatLocY < crt.combatLocY) && (crt.combatFacing == 2)) { return true; }
            if ((pc.combatLocX < crt.combatLocX) && (pc.combatLocY < crt.combatLocY) && (crt.combatFacing == 3)) { return true; }
            return false;
        }
        public bool IsCreatureAttackFromBehind(Player pc, Creature crt)
        {
            if ((crt.combatLocX > pc.combatLocX) && (crt.combatLocY > pc.combatLocY) && (pc.combatFacing == 7)) { return true; }
            if ((crt.combatLocX == pc.combatLocX) && (crt.combatLocY > pc.combatLocY) && (pc.combatFacing == 8)) { return true; }
            if ((crt.combatLocX < pc.combatLocX) && (crt.combatLocY > pc.combatLocY) && (pc.combatFacing == 9)) { return true; }
            if ((crt.combatLocX > pc.combatLocX) && (crt.combatLocY == pc.combatLocY) && (pc.combatFacing == 4)) { return true; }
            if ((crt.combatLocX < pc.combatLocX) && (crt.combatLocY == pc.combatLocY) && (pc.combatFacing == 6)) { return true; }
            if ((crt.combatLocX > pc.combatLocX) && (crt.combatLocY < pc.combatLocY) && (pc.combatFacing == 1)) { return true; }
            if ((crt.combatLocX == pc.combatLocX) && (crt.combatLocY < pc.combatLocY) && (pc.combatFacing == 2)) { return true; }
            if ((crt.combatLocX < pc.combatLocX) && (crt.combatLocY < pc.combatLocY) && (pc.combatFacing == 3)) { return true; }
            return false;
        }
        public int getDistance(Coordinate start, Coordinate end)
        {
            int dist = 0;
            int deltaX = (int)Math.Abs((start.X - end.X));
            int deltaY = (int)Math.Abs((start.Y - end.Y));
            if (deltaX > deltaY)
                dist = deltaX;
            else
                dist = deltaY;
            return dist;
        }
        public bool isWalkable(int x, int y)
        {
            if (mod.currentEncounter.Walkable[y * mod.currentEncounter.MapSizeX + x] == 0)
            {
                return false;
            }
            foreach (Player p in mod.playerList)
            {
                if ((p.combatLocX == x) && (p.combatLocY == y))
                {
                    if ((!p.isDead()) && (p.hp > 0))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public Creature isBumpIntoCreature(int x, int y)
        {
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {                
                foreach (Coordinate coor in crt.tokenCoveredSquares)
                {
                    if ((coor.X == x) && (coor.Y == y))
                    {
                        return crt;
                    }
                }
            }
            return null;
        }
        public void LeaveThreatenedCheck(Player pc, int futurePlayerLocationX, int futurePlayerLocationY)
        {

            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if ((crt.hp > 0) && (!crt.isHeld()))
                {
                    //if started in distance = 1 and now distance = 2 then do attackOfOpportunity
                    //also do attackOfOpportunity if moving within controlled area around a creature, i.e. when distance to cerature after move is still one square
                    //the later makes it harder to circle around a cretaure or break through lines, fighters get more area control this way, allwoing them to protect other charcters with more ease
                    if (((CalcDistance(crt, crt.combatLocX, crt.combatLocY, pc.combatLocX, pc.combatLocY) == 1)
                        && (CalcDistance(crt, crt.combatLocX, crt.combatLocY, futurePlayerLocationX, futurePlayerLocationY) == 2))
                        || ((currentMoves > 0) && (CalcDistance(crt, crt.combatLocX, crt.combatLocY, pc.combatLocX, pc.combatLocY) == 1)
                        && (CalcDistance(crt, crt.combatLocX, crt.combatLocY, futurePlayerLocationX, futurePlayerLocationY) == 1)))
                    {
                        if (pc.steathModeOn)
                        {
                            gv.cc.addLogText("<gn>Avoids Attack of Opportunity due to Stealth</gn><BR>");
                        }
                        else
                        {
                            gv.cc.addLogText("<bu>Attack of Opportunity by: " + crt.cr_name + "</bu><BR>");
                            doActualCreatureAttack(pc, crt, 1);
                            if (pc.hp <= 0)
                            {
                                currentMoves = 99;
                            }
                        }
                    }
                }
            }
        }

        public int CalcPcAttackModifier(Player pc, Creature crt)
        {
            int modifier = 0;
            if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee"))
                    || (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).name.Equals("none"))
                    || (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
            {
                modifier = (pc.strength - 10) / 2;
                //if has critical strike trait use dexterity for attack modifier in melee if greater than strength modifier
                if (pc.knownTraitsTags.Contains("criticalstrike"))
                {
                    int modifierDex = (pc.dexterity - 10) / 2;
                    if (modifierDex > modifier)
                    {
                        modifier = (pc.dexterity - 10) / 2;
                    }
                }
                //if doing sneak attack, bonus to hit roll
                if (pc.steathModeOn)
                {
                    if (pc.knownTraitsTags.Contains("sneakattack"))
                    {
                        //+1 for every 2 levels after level 1
                        int adding = ((pc.classLevel - 1) / 2) + 1;
                        modifier += adding;
                        gv.cc.addLogText("<gn> sneak attack: +" + adding + " to hit</gn><BR>");
                    }
                }
                //all attacks of the PC from behind get a +2 bonus to hit            
                if (IsAttackFromBehind(pc, crt))
                {
                    modifier += mod.attackFromBehindToHitModifier;
                    if (mod.attackFromBehindToHitModifier > 0)
                    {
                        gv.cc.addLogText("<gn> Attack from behind: +" + mod.attackFromBehindToHitModifier.ToString() + " to hit." + "</gn><BR>");
                    }
                }
            }
            else //ranged weapon used
            {
                modifier = (pc.dexterity - 10) / 2;
                //factor in penalty for adjacent enemies when using ranged weapon
                if (isAdjacentEnemy(pc))
                {
                    if (gv.sf.hasTrait(pc, "pointblankshot"))
                    {
                        //has point blank shot trait, no penalty
                    }
                    else
                    {
                        modifier -= 4;
                        gv.cc.addLogText("<yl>" + "-4 ranged attack penalty" + "</yl><BR>");
                        gv.cc.addLogText("<yl>" + "with enemies in melee range" + "</yl><BR>");
                        gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), "-4 att", "yellow");
                    }
                }
                if (gv.sf.hasTrait(pc, "preciseshot2"))
                {
                    modifier += 2;
                    gv.cc.addLogText("<gn> PreciseShotL2: +2 to hit</gn><BR>");
                }
                else if (gv.sf.hasTrait(pc, "preciseshot"))
                {
                    modifier++;
                    gv.cc.addLogText("<gn> PreciseShotL1: +1 to hit</gn><BR>");
                }
            }
            if (gv.sf.hasTrait(pc, "hardtokill"))
            {
                modifier -= 2;
                gv.cc.addLogText("<yl>" + "blinded by rage" + "</yl><BR>");
                gv.cc.addLogText("<yl>" + "-2 attack penalty" + "</yl><BR>");
            }
            int attackMod = modifier + pc.baseAttBonus + mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attackBonus;
            Item it = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
            if (it != null)
            {
                attackMod += mod.getItemByResRefForInfo(pc.AmmoRefs.resref).attackBonus;
            }
            return attackMod;
        }
        public int CalcCreatureDefense(Player pc, Creature crt)
        {
            int defense = crt.AC;
            if (crt.isHeld())
            {
                defense -= 4;
                gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), "+4 att", "green");
            }
            return defense;
        }
        public int CalcPcDamageToCreature(Player pc, Creature crt)
        {
            int damModifier = 0;
            int adder = 0;
            bool melee = false;
            if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee"))
                    || (pc.MainHandRefs.name.Equals("none"))
                    || (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
            {
                melee = true;
                damModifier = (pc.strength - 10) / 2;
                //if has critical strike trait use dexterity for damage modifier in melee if greater than strength modifier
                if (gv.sf.hasTrait(pc, "criticalstrike"))
                {
                    int damModifierDex = (pc.dexterity - 10) / 4;
                    if (damModifierDex > damModifier)
                    {
                        damModifier = (pc.dexterity - 10) / 2;
                    }
                }

                if (IsAttackFromBehind(pc, crt))
                {
                    damModifier += mod.attackFromBehindDamageModifier;
                    if (mod.attackFromBehindDamageModifier > 0)
                    {
                        gv.cc.addLogText("<gn> Attack from behind: +" + mod.attackFromBehindDamageModifier.ToString() + " damage." + "</gn><BR>");
                    }
                }


            }
            else //ranged weapon used
            {
                damModifier = 0;
                if (gv.sf.hasTrait(pc, "preciseshot2"))
                {
                    damModifier += 2;
                    gv.cc.addLogText("<gn> PreciseShotL2: +2 damage</gn><BR>");
                }
                else if (gv.sf.hasTrait(pc, "preciseshot"))
                {
                    damModifier++;
                    gv.cc.addLogText("<gn> PreciseShotL1: +1 damage</gn><BR>");
                }

            }

            int dDam = mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageDie;
            float damage = (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageNumDice * gv.sf.RandInt(dDam)) + damModifier + adder + mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageAdder;
            if (damage < 0)
            {
                damage = 0;
            }
            Item it = mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
            if (it != null)
            {
                damage += mod.getItemByResRefForInfo(pc.AmmoRefs.resref).damageAdder;
            }

            float resist = 0;

            if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Acid"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueAcid / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Normal"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueNormal / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Cold"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueCold / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Electricity"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueElectricity / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Fire"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Magic"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValueMagic / 100f));
            }
            else if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage.Equals("Poison"))
            {
                resist = (float)(1f - ((float)crt.damageTypeResistanceValuePoison / 100f));
            }

            int totalDam = (int)(damage * resist);
            if (totalDam < 0)
            {
                totalDam = 0;
            }
            //if doing sneak attack, does extra damage
            if ((pc.steathModeOn) && (melee) && (IsAttackFromBehind(pc, crt)))
            {
                if (pc.knownTraitsTags.Contains("sneakattack"))
                {
                    //+1d6 for every 2 levels after level 1
                    int multiplier = ((pc.classLevel - 1) / 2) + 1;
                    int adding = 0;
                    for (int i = 0; i < multiplier; i++)
                    {
                        adding += gv.sf.RandInt(6);
                    }
                    totalDam += adding;
                    gv.cc.addLogText("<gn> sneak attack: +" + adding + " damage</gn><BR>");
                }
            }
            return totalDam;
        }
        public int CalcCreatureAttackModifier(Creature crt, Player pc)
        {
            if ((crt.cr_category.Equals("Ranged")) && (isAdjacentPc(crt)))
            {
                gv.cc.addLogText("<yl>" + "-4 ranged attack penalty" + "</yl>" +
                        "<BR>");
                gv.cc.addLogText("<yl>" + "with enemies in melee range" + "</yl>" +
                        "<BR>");
                gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), "-4 att", "yellow");
                return crt.cr_att - 4;
            }
            else //melee weapon used
            {
                int modifier = 0;
                //all attacks of the Creature from behind get a +2 bonus to hit            
                if (IsCreatureAttackFromBehind(pc, crt))
                {
                    modifier += 2;
                    gv.cc.addLogText("<yl>" + crt.cr_name + " attacks from behind: +2 att</yl><BR>");
                }
                return crt.cr_att + modifier;
            }
        }
        public int CalcPcDefense(Player pc, Creature crt)
        {
            int defense = pc.AC;
            if (pc.isHeld())
            {
                defense -= 4;
                gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), "+4 att", "yellow");
            }
            return defense;
        }
        public int CalcCreatureDamageToPc(Player pc, Creature crt)
        {
            int dDam = crt.cr_damageDie;
            float damage = (crt.cr_damageNumDice * gv.sf.RandInt(dDam)) + crt.cr_damageAdder;
            if (damage < 0)
            {
                damage = 0;
            }

            float resist = 0;

            if (crt.cr_typeOfDamage.Equals("Acid"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalAcid / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Normal"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalNormal / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Cold"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalCold / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Electricity"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalElectricity / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Fire"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Magic"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalMagic / 100f));
            }
            else if (crt.cr_typeOfDamage.Equals("Poison"))
            {
                resist = (float)(1f - ((float)pc.damageTypeResistanceTotalPoison / 100f));
            }

            int totalDam = (int)(damage * resist);
            if (totalDam < 0)
            {
                totalDam = 0;
            }

            return totalDam;
        }

        public Player targetClosestPC(Creature crt)
        {
            Player pc = null;
            int farDist = 99;
            foreach (Player p in mod.playerList)
            {
                if ((!p.isDead()) && (p.hp >= 0) && (!p.steathModeOn))
                {
                    int dist = CalcDistance(crt, crt.combatLocX, crt.combatLocY, p.combatLocX, p.combatLocY);
                    if (dist == farDist)
                    {
                        //since at same distance, do a random check to see if switch or stay with current PC target
                        if (gv.sf.RandInt(20) > 10)
                        {
                            //switch target
                            pc = p;
                            if (gv.mod.debugMode)
                            {
                                //gv.cc.addLogText("<yl>target:" + pc.name + "</yl><BR>");
                            }
                        }
                    }
                    else if (dist < farDist)
                    {
                        farDist = dist;
                        pc = p;
                        if (gv.mod.debugMode)
                        {
                            //gv.cc.addLogText("<yl>target:" + pc.name + "</yl><BR>");
                        }
                    }
                }
            }
            return pc;
        }
        public Coordinate targetBestPointLocation(Creature crt)
        {
            Coordinate targetLoc = new Coordinate(-1, -1);
            //JamesManhattan Utility maximization function for the VERY INTELLIGENT CREATURE CASTER
            int utility = 0; //utility
            int optimalUtil = 0; //optimal utility, a storage of the highest achieved
            Coordinate selectedPoint = new Coordinate(crt.combatLocX, crt.combatLocY); //Initial Select Point is Creature itself, then loop through all squares within range!
            for (int y = gv.sf.SpellToCast.range; y > -gv.sf.SpellToCast.range; y--)  //start at far range and work backwards does a pretty good job of avoiding hitting allies.
            {
                for (int x = gv.sf.SpellToCast.range; x > -gv.sf.SpellToCast.range; x--)
                {
                    utility = 0; //reset utility for each point tested
                    selectedPoint = new Coordinate(crt.combatLocX + x, crt.combatLocY + y);

                    //check if selected point is a valid location on combat map
                    if ((selectedPoint.X < 0) || (selectedPoint.X > mod.currentEncounter.MapSizeX - 1) || (selectedPoint.Y < 0) || (selectedPoint.Y > mod.currentEncounter.MapSizeY - 1))
                    {
                        continue;
                    }

                    //check if selected point is in LoS, if not skip this point
                    int endX = selectedPoint.X * gv.squareSize + (gv.squareSize / 2);
                    int endY = selectedPoint.Y * gv.squareSize + (gv.squareSize / 2);
                    int startX = crt.combatLocX * gv.squareSize + (gv.squareSize / 2);
                    int startY = crt.combatLocY * gv.squareSize + (gv.squareSize / 2);
                    if (!isVisibleLineOfSight(new Coordinate(endX, endY), new Coordinate(startX, startY)))
                    {
                        continue;
                    }

                    if (selectedPoint == new Coordinate(crt.combatLocX, crt.combatLocY))
                    {
                        utility -= 4; //the creature at least attempts to avoid hurting itself, but if surrounded might fireball itself!
                        if (crt.hp <= crt.hpMax / 4) //caster is wounded, definately avoids itself.
                        {
                            utility -= 4;
                        }
                    }
                    foreach (Creature crtr in mod.currentEncounter.encounterCreatureList) //if its allies are in the burst subtract a point, or half depending on how evil it is.
                    {
                        if (this.CalcDistance(crtr, crtr.combatLocX, crtr.combatLocY, selectedPoint.X, selectedPoint.Y) <= gv.sf.SpellToCast.aoeRadius) //if friendly creatures are in the AOE burst, count how many, subtract 0.5 for each, evil is evil
                        {
                            utility -= 1;
                        }
                    }
                    foreach (Player tgt_pc in mod.playerList)
                    {
                        if ((this.CalcDistance(null, tgt_pc.combatLocX, tgt_pc.combatLocY, selectedPoint.X, selectedPoint.Y) <= gv.sf.SpellToCast.aoeRadius) && (tgt_pc.hp > 0)) //if players are in the AOE burst, count how many, total count is utility  //&& sf.GetLocalInt(tgt_pc.Tag, "StealthModeOn") != 1  <-throws an annoying message if not found!!
                        {
                            utility += 2;
                            if (utility > optimalUtil)
                            {
                                //optimal found, choose this point
                                optimalUtil = utility;
                                targetLoc = selectedPoint;
                            }
                        }
                    }
                    if (gv.mod.debugMode)
                    {
                        //gv.cc.addLogText("<yl>(" + selectedPoint.X + "," + selectedPoint.Y + "):" + utility + "</yl><BR>");
                    }
                }
            }

            return targetLoc;
        }
        public int CalcDistance(Creature crt, int locCrX, int locCrY, int locPcX, int locPcY)
        {
            int dist = 999;
            if (crt == null)
            {
                int deltaX = (int)Math.Abs((locCrX - locPcX));
                int deltaY = (int)Math.Abs((locCrY - locPcY));
                if (deltaX > deltaY)
                    return deltaX;
                else
                    return deltaY;
            }
            //go through all squares of creature and return the lowest distance
            int crtSize = gv.cc.getCreatureSize(crt.cr_tokenFilename); //1=normal, 2=wide, 3=tall, 4=large
            //crt normal
            if (crtSize == 1)
            {
                int deltaX = (int)Math.Abs((locCrX - locPcX));
                int deltaY = (int)Math.Abs((locCrY - locPcY));
                if (deltaX > deltaY)
                    dist = deltaX;
                else
                    dist = deltaY;
            }
            //crt wide
            else if (crtSize == 2)
            {
                int dist1 = 999;
                int dist2 = 999;
                //main square
                int deltaX = (int)Math.Abs((locCrX - locPcX));
                int deltaY = (int)Math.Abs((locCrY - locPcY));
                if (deltaX > deltaY) { dist1 = deltaX; }
                else { dist1 = deltaY; }
                //right square
                deltaX = (int)Math.Abs((locCrX + 1 - locPcX));
                deltaY = (int)Math.Abs((locCrY - locPcY));
                if (deltaX > deltaY) { dist2 = deltaX; }
                else { dist2 = deltaY; }
                //see which is lower
                if (dist1 > dist2) { dist = dist2; }
                else { dist = dist1; }
            }
            //crt tall
            else if (crtSize == 3)
            {
                int dist1 = 999;
                int dist2 = 999;
                //main square
                int deltaX = (int)Math.Abs((locCrX - locPcX));
                int deltaY = (int)Math.Abs((locCrY - locPcY));
                if (deltaX > deltaY) { dist1 = deltaX; }
                else { dist1 = deltaY; }
                //lower square
                deltaX = (int)Math.Abs((locCrX - locPcX));
                deltaY = (int)Math.Abs((locCrY + 1 - locPcY));
                if (deltaX > deltaY) { dist2 = deltaX; }
                else { dist2 = deltaY; }
                //see which is lower
                if (dist1 > dist2) { dist = dist2; }
                else { dist = dist1; }
            }
            //crt large
            else if (crtSize == 4)
            {
                int dist1 = 999;
                int dist2 = 999;
                int dist3 = 999;
                int dist4 = 999;
                //main square
                int deltaX = (int)Math.Abs((locCrX - locPcX));
                int deltaY = (int)Math.Abs((locCrY - locPcY));
                if (deltaX > deltaY) { dist1 = deltaX; }
                else { dist1 = deltaY; }
                //right square
                deltaX = (int)Math.Abs((locCrX + 1 - locPcX));
                deltaY = (int)Math.Abs((locCrY - locPcY));
                if (deltaX > deltaY) { dist2 = deltaX; }
                else { dist2 = deltaY; }
                //lower square
                deltaX = (int)Math.Abs((locCrX - locPcX));
                deltaY = (int)Math.Abs((locCrY + 1 - locPcY));
                if (deltaX > deltaY) { dist3 = deltaX; }
                else { dist3 = deltaY; }
                //lower right square
                deltaX = (int)Math.Abs((locCrX + 1 - locPcX));
                deltaY = (int)Math.Abs((locCrY + 1 - locPcY));
                if (deltaX > deltaY) { dist4 = deltaX; }
                else { dist4 = deltaY; }
                //see which is lower
                if (dist1 < dist) { dist = dist1; }
                if (dist2 < dist) { dist = dist2; }
                if (dist3 < dist) { dist = dist3; }
                if (dist4 < dist) { dist = dist4; }
            }
            
            return dist;
        }
        public Creature GetCreatureWithLowestHP()
        {
            int lowHP = 999;
            Creature returnCrt = null;
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if (crt.hp > 0)
                {
                    if (crt.hp < lowHP)
                    {
                        lowHP = crt.hp;
                        returnCrt = crt;
                    }
                }
            }
            return returnCrt;
        }
        public Creature GetCreatureWithMostDamaged()
        {
            int damaged = 0;
            Creature returnCrt = null;
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if (crt.hp > 0)
                {
                    int dam = crt.hpMax - crt.hp;
                    if (dam > damaged)
                    {
                        damaged = dam;
                        returnCrt = crt;
                    }
                }
            }
            return returnCrt;
        }
        public Creature GetNextAdjacentCreature(Player pc)
        {
            foreach (Creature nextCrt in mod.currentEncounter.encounterCreatureList)
            {
                if ((CalcDistance(nextCrt, nextCrt.combatLocX, nextCrt.combatLocY, pc.combatLocX, pc.combatLocY) < 2) && (nextCrt.hp > 0))
                {
                    return nextCrt;
                }
            }
            return null;
        }
        public Creature GetCreatureByTag(String tag)
        {
            foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
            {
                if (crt.cr_tag.Equals(tag))
                {
                    return crt;
                }
            }
            return null;
        }
    }
}
