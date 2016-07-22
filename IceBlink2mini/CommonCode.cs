﻿using Newtonsoft.Json;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using System;
//using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
//using System.Threading;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2mini
{
    public class CommonCode
    {
        //this class is handled differently than Android version
        public GameView gv;

        public float weatherSoundMultiplier = 2.7f;
        public bool blockSecondPropTriggersCall = false;
        public List<FloatyText> floatyTextList = new List<FloatyText>();
        public int floatyTextCounter = 0;
        public bool floatyTextOn = false;

        public IbbPanel pnlLog = null;
        public IbbPanel pnlToggles = null;
        public IbbPanel pnlPortraits = null;
        public IbbPanel pnlHotkeys = null;
        public IbbPanel pnlArrows = null;
        public IbbButton btnReturn = null;
        public IbbButton ctrlUpArrow = null;
        public IbbButton ctrlDownArrow = null;
        public IbbButton ctrlLeftArrow = null;
        public IbbButton ctrlRightArrow = null;
        public IbbButton ctrlUpRightArrow = null;
        public IbbButton ctrlDownLeftArrow = null;
        public IbbButton ctrlUpLeftArrow = null;
        public IbbButton ctrlDownRightArrow = null;
        public IbbButton btnInventory = null;
        public IbbButton btnHelp = null;
        public IbbToggleButton tglSound = null;
        public IbbPortrait ptrPc0 = null;
        public IbbPortrait ptrPc1 = null;
        public IbbPortrait ptrPc2 = null;
        public IbbPortrait ptrPc3 = null;
        public IbbPortrait ptrPc4 = null;
        public IbbPortrait ptrPc5 = null;

        public int partyScreenPcIndex = 0;
        public int partyItemSlotIndex = 0;
        public string currentMusic = "";
        public string slotA = "Autosave";
        public string slot0 = "Quicksave";
        public string slot1 = "";
        public string slot2 = "";
        public string slot3 = "";
        public string slot4 = "";
        public string slot5 = "";

        public Bitmap title;
        //public Bitmap bmpMap;
        public Bitmap walkPass;
        public Bitmap btnIni;
        public Bitmap btnIniGlow;
        public Bitmap walkBlocked;
        public Bitmap losBlocked;
        public Bitmap hitSymbol;
        public Bitmap missSymbol;
        public Bitmap highlight_green;
        public Bitmap highlight_red;
        public Bitmap black_tile;
        public Bitmap black_tile2;
        public Bitmap turn_marker;
        public Bitmap pc_dead;
        public Bitmap pc_stealth;
        public Bitmap offScreen;
        public Bitmap offScreen5;
        public Bitmap offScreen6;
        public Bitmap offScreen7;
        public Bitmap offScreenTrans;
        public Bitmap death_fx;
        public Bitmap tint_dawn;
        public Bitmap tint_sunrise;
        public Bitmap tint_sunset;
        public Bitmap tint_dusk;
        public Bitmap tint_night;
        public Bitmap ui_bg_fullscreen;
        public Bitmap ui_portrait_frame;
        public Bitmap facing1;
        public Bitmap facing2;
        public Bitmap facing3;
        public Bitmap facing4;
        public Bitmap facing6;
        public Bitmap facing7;
        public Bitmap facing8;
        public Bitmap facing9;
        //off for now
        //public Bitmap tint_rain;

        public Dictionary<string, Bitmap> tileBitmapList = new Dictionary<string, Bitmap>();
        public Dictionary<string, Bitmap> commonBitmapList = new Dictionary<string, Bitmap>();
        public Dictionary<string, System.Drawing.Bitmap> tileGDIBitmapList = new Dictionary<string, System.Drawing.Bitmap>();

        public Spell currentSelectedSpell = new Spell();
        public string floatyText = "";
        public string floatyText2 = "";
        public string floatyText3 = "";
        public Coordinate floatyTextLoc = new Coordinate();
        public int creatureIndex = 0;
        public bool calledConvoFromProp = false;
        public bool calledEncounterFromProp = false;
        public int currentPlayerIndexUsingItem = 0;

        public string stringBeginnersGuide = "";
        public string stringPlayersGuide = "";
        public string stringPcCreation = "";
        public string stringMessageCombat = "";
        public string stringMessageInventory = "";
        public string stringMessageParty = "";
        public string stringMessageMainMap = "";

        public bool doOnEnterAreaUpdate = false;
        public bool recursiveCall = false;
        int fallBackSquareX = 0;
        int fallBackSquareY = 0;

        public CommonCode(GameView g)
        {
            gv = g;
        }

        //LOAD FILES
        public void LoadTestParty()
        {
            gv.sf.AddCharacterToParty(gv.mod.defaultPlayerFilename); //drin.json is default
            gv.mod.partyTokenFilename = "prp_party";
            gv.mod.partyTokenBitmap = this.LoadBitmap(gv.mod.partyTokenFilename);
        }
        public Player LoadPlayer(string filename)
        {
            Player toReturn = null;

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\" + filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                toReturn = (Player)serializer.Deserialize(file, typeof(Player));
            }
            return toReturn;
        }
        public void LoadCurrentConvo(string filename)
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\dialog\\" + filename + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.screenConvo.currentConvo = (Convo)serializer.Deserialize(file, typeof(Convo));
            }
        }
        public void AutoSave()
        {
            string filename = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\autosave.json";
            MakeDirectoryIfDoesntExist(filename);
            string json = JsonConvert.SerializeObject(gv.mod, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(json.ToString());
            }
        }
        public void QuickSave()
        {
            string filename = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\quicksave.json";
            MakeDirectoryIfDoesntExist(filename);
            string json = JsonConvert.SerializeObject(gv.mod, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(json.ToString());
            }
        }
        public void SaveGame(string filename)
        {
            string filepath = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename;
            MakeDirectoryIfDoesntExist(filepath);
            string json = JsonConvert.SerializeObject(gv.mod, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.Write(json.ToString());
            }
        }
        public void SaveGameInfo(string filename)
        {
            ModuleInfo newModInfo = new ModuleInfo();
            newModInfo.saveName = gv.mod.saveName;

            string filepath = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename;
            MakeDirectoryIfDoesntExist(filepath);
            string json = JsonConvert.SerializeObject(newModInfo, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.Write(json.ToString());
            }
        }
        public void doSavesDialog()
        {
            List<string> saveList = new List<string> { slot0, slot1, slot2, slot3, slot4, slot5 };

            using (ItemListSelector itSel = new ItemListSelector(gv, saveList, "Choose a slot to save game."))
            {
                itSel.IceBlinkButtonClose.Enabled = true;
                itSel.IceBlinkButtonClose.Visible = true;
                itSel.setupAll(gv);
                var ret = itSel.ShowDialog();

                if (itSel.selectedIndex == 0)
                {
                    try
                    {
                        QuickSave();
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
                else if (itSel.selectedIndex == 1)
                {
                    Player pc = gv.mod.playerList[0];
                    gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                    slot1 = gv.mod.saveName;
                    try
                    {
                        SaveGame("slot1.json");
                        SaveGameInfo("slot1info.json");
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
                else if (itSel.selectedIndex == 2)
                {
                    Player pc = gv.mod.playerList[0];
                    gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                    slot2 = gv.mod.saveName;
                    try
                    {
                        SaveGame("slot2.json");
                        SaveGameInfo("slot2info.json");
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
                else if (itSel.selectedIndex == 3)
                {
                    Player pc = gv.mod.playerList[0];
                    gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                    slot3 = gv.mod.saveName;
                    try
                    {
                        SaveGame("slot3.json");
                        SaveGameInfo("slot3info.json");
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
                else if (itSel.selectedIndex == 4)
                {
                    Player pc = gv.mod.playerList[0];
                    gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                    slot4 = gv.mod.saveName;
                    try
                    {
                        SaveGame("slot4.json");
                        SaveGameInfo("slot4info.json");
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
                else if (itSel.selectedIndex == 5)
                {
                    Player pc = gv.mod.playerList[0];
                    gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                    slot5 = gv.mod.saveName;
                    try
                    {
                        SaveGame("slot5.json");
                        SaveGameInfo("slot5info.json");
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
            }
        }
        public void doLoadSaveGameDialog()
        {
            List<string> saveList = new List<string> { slotA, slot0, slot1, slot2, slot3, slot4, slot5 };

            using (ItemListSelector itSel = new ItemListSelector(gv, saveList, "Choose a Saved Game to Load."))
            {
                itSel.IceBlinkButtonClose.Visible = true;
                itSel.IceBlinkButtonClose.Enabled = true;
                itSel.ShowDialog();

                if (itSel.selectedIndex == 0)
                {
                    bool result = LoadSave("autosave.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 1)
                {
                    bool result = LoadSave("quicksave.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 2)
                {
                    bool result = LoadSave("slot1.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 3)
                {
                    bool result = LoadSave("slot2.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 4)
                {
                    bool result = LoadSave("slot3.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 5)
                {
                    bool result = LoadSave("slot4.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 6)
                {
                    bool result = LoadSave("slot5.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
            }
        }
        public ModuleInfo LoadModuleInfo(string filename)
        {
            ModuleInfo m = new ModuleInfo();
            try
            {
                using (StreamReader file = File.OpenText(gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    m = (ModuleInfo)serializer.Deserialize(file, typeof(ModuleInfo));
                }
            }
            catch { }
            return m;
        }
        public void LoadSaveListItems()
        {
            slot1 = LoadModuleInfo("slot1info.json").saveName;
            slot2 = LoadModuleInfo("slot2info.json").saveName;
            slot3 = LoadModuleInfo("slot3info.json").saveName;
            slot4 = LoadModuleInfo("slot4info.json").saveName;
            slot5 = LoadModuleInfo("slot5info.json").saveName;
        }

        public bool LoadSave(string filename)
        {
            //  load a new module (actually already have a new module at this point from launch screen		
            //  load the saved game module
            Module saveMod = LoadSaveGameModule(filename);
            if (saveMod == null) { return false; }
            //  replace parts of new module with parts of saved game module
            //
            // U = update from save file	 
            //
            //module file
            //{
            //U  "saveName": "Drin, Level:1, XP:150, WorldTime:24", (use all save)
            gv.mod.saveName = saveMod.saveName;
            //U  "playerList": [], (use all save)  Update PCs later further down
            gv.mod.playerList = new List<Player>();
            foreach (Player pc in saveMod.playerList)
            {
                gv.mod.playerList.Add(pc.DeepCopy());
            }
            setMainPc();
            //U  "partyRosterList": [], (use all save)  Update PCs later further down
            gv.mod.partyRosterList = new List<Player>();
            foreach (Player pc in saveMod.partyRosterList)
            {
                gv.mod.partyRosterList.Add(pc.DeepCopy());
            }
            //U  "partyJournalQuests": [], (use tags from save to get all from new)
            gv.mod.partyJournalQuests.Clear();
            foreach (JournalQuest jq in saveMod.partyJournalQuests)
            {
                foreach (JournalEntry je in jq.Entries)
                {
                    gv.sf.AddJournalEntryNoMessages(jq.Tag, je.Tag);
                }
            }
            //U  "partyJournalNotes": "", (use all save)
            gv.mod.partyJournalNotes = saveMod.partyJournalNotes;
            //U  "partyJournalCompleted": [], (use tags from save to get all from new)
            // NOT CURRENTLY USED
            //U  "partyInventoryTagList": [], (use all save) update Items later on down
            gv.mod.partyInventoryRefsList.Clear();
            foreach (ItemRefs s in saveMod.partyInventoryRefsList)
            {
                gv.mod.partyInventoryRefsList.Add(s.DeepCopy());
            }
            //U  "moduleShopsList": [], (have an original shop items tags list and the current tags list to see what to add or delete from the save tags list)
            this.updateShops(saveMod);
            //  "moduleName": "Lanterna2", Don't need to update
            //  "moduleAreasList": [], Don't need to update
            //U  "moduleAreasObjects": [],
            //                (triggers: use save trigger "enabled" value to update new)
            //                (tiles: use save "visible" to update new)
            //                (props: have an original props tags list and the current tags list to see what to add or delete from the save tags list)		               
            this.updateAreas(saveMod);
            //
            //U  "currentArea": {},
            gv.mod.setCurrentArea(saveMod.currentArea.Filename, gv);
            //U  "moduleContainersList": [], (have an original containers items tags list and the current tags list to see what to add or delete from the save tags list)
            this.updateContainers(saveMod);
            //U  "moduleConvoSavedValuesList": [], (use all save)
            gv.mod.moduleConvoSavedValuesList.Clear();
            foreach (ConvoSavedValues csv in saveMod.moduleConvoSavedValuesList)
            {
                gv.mod.moduleConvoSavedValuesList.Add(csv.DeepCopy());
            }
            //  "moduleConvosList": [], Don't need to update
            //U  "moduleEncountersList": [], (use new except delete those completed already in save)
            foreach (Encounter enc in saveMod.moduleEncountersList)
            {
                if (enc.encounterCreatureRefsList.Count <= 0)
                {
                    //if the encounter was completed in the saveMod then clear all creatures in the newMod
                    Encounter e = gv.mod.getEncounter(enc.encounterName);
                    e.encounterCreatureList.Clear();
                    e.encounterCreatureRefsList.Clear();
                }
            }
            //U  "moduleGlobalInts": [], (use all save)
            gv.mod.moduleGlobalInts.Clear();
            foreach (GlobalInt g in saveMod.moduleGlobalInts)
            {
                gv.mod.moduleGlobalInts.Add(g.DeepCopy());
            }
            //U  "moduleGlobalStrings": [], (use all save)
            gv.mod.moduleGlobalStrings.Clear();
            foreach (GlobalString g in saveMod.moduleGlobalStrings)
            {
                gv.mod.moduleGlobalStrings.Add(g.DeepCopy());
            }
            //  "moduleLabelName": "Lanterna 2 (demo)", don't need to update
            //U  "combatAnimationSpeed": 100, (use all save)
            gv.mod.combatAnimationSpeed = saveMod.combatAnimationSpeed;
            //gv.mod.combatAnimationSpeed = 100;

            //U  "partyGold": 70, (use all save)
            gv.mod.partyGold = saveMod.partyGold;
            //U  "com_showGrid": false, (use all save)
            gv.mod.com_showGrid = saveMod.com_showGrid;
            gv.mod.map_showGrid = saveMod.map_showGrid;
            /*gv.mod.sendProgressReport = saveMod.sendProgressReport;
            if (saveMod.uniqueSessionIdNumberTag.Equals(""))
            {
                gv.mod.uniqueSessionIdNumberTag = gv.sf.RandInt(1000000) + "";
            }
            else
            {
                gv.mod.uniqueSessionIdNumberTag = saveMod.uniqueSessionIdNumberTag;
            }*/
            //U  "allowAutosave": true, (use all save)
            gv.mod.allowAutosave = saveMod.allowAutosave;
            //U  "WorldTime": 24, (use all save)
            gv.mod.WorldTime = saveMod.WorldTime;
            gv.mod.nextIdNumber = saveMod.nextIdNumber;
            //U  "PlayerLocationY": 2, (use all save)
            gv.mod.PlayerLocationY = saveMod.PlayerLocationY;
            //U  "PlayerLocationX": 1, (use all save)
            gv.mod.PlayerLocationX = saveMod.PlayerLocationX;
            //U  "playButtonHaptic": false, (use all save)
            gv.mod.playButtonHaptic = saveMod.playButtonHaptic;
            //U  "playButtonSounds": false, (use all save)
            gv.mod.playButtonSounds = saveMod.playButtonSounds;
            //U  "playMusic": false, (use all save)
            gv.mod.playMusic = saveMod.playMusic;
            //U  "playSoundFx": false, (use all save)
            gv.mod.playSoundFx = saveMod.playSoundFx;
            //U  "PlayerLastLocationY": 1, (use all save)
            gv.mod.PlayerLastLocationY = saveMod.PlayerLastLocationY;
            //U  "PlayerLastLocationX": 2, (use all save)
            gv.mod.PlayerLastLocationX = saveMod.PlayerLastLocationX;
            //U  "selectedPartyLeader": 0, (use all save)
            gv.mod.selectedPartyLeader = saveMod.selectedPartyLeader;
            //U  "showAutosaveMessage": true, (use all save)
            gv.mod.showAutosaveMessage = saveMod.showAutosaveMessage;
            //U  "showPartyToken": false, (use all save)
            gv.mod.showPartyToken = saveMod.showPartyToken;
            //U  "showTutorialCombat": true, (use all save)
            gv.mod.showTutorialCombat = saveMod.showTutorialCombat;
            //U  "showTutorialInventory": true, (use all save)
            gv.mod.showTutorialInventory = saveMod.showTutorialInventory;
            //U  "showTutorialParty": true, (use all save)
            gv.mod.showTutorialParty = saveMod.showTutorialParty;
            //U  "soundVolume": 1.0, (use all save)
            gv.mod.soundVolume = saveMod.soundVolume;
            //U  "startingPlayerPositionX": 4, (use all save)
            gv.mod.startingPlayerPositionX = saveMod.startingPlayerPositionX;
            //U  "startingPlayerPositionY": 1 (use all save)
            gv.mod.startingPlayerPositionY = saveMod.startingPlayerPositionY;
            //gv.mod.OnHeartBeatLogicTree = saveMod.OnHeartBeatLogicTree;
            //gv.mod.OnHeartBeatParms = saveMod.OnHeartBeatParms;
            gv.mod.OnHeartBeatIBScript = saveMod.OnHeartBeatIBScript;
            gv.mod.OnHeartBeatIBScriptParms = saveMod.OnHeartBeatIBScriptParms;
            //}

            LoadRaces();
            LoadPlayerClasses();
            LoadItems();
            //no load of containers
            //no load of shops
            LoadEffects();
            LoadSpells();
            LoadTraits();
            LoadCreatures();
            //no load of encounters
            LoadJournal();
            //LoadTileBitmapList();
            gv.initializeSounds();

            gv.mod.partyTokenFilename = "prp_party";
            gv.mod.partyTokenBitmap = this.LoadBitmap(gv.mod.partyTokenFilename);

            this.updatePlayers();
            this.updatePartyRosterPlayers();

            gv.createScreens();
            gv.screenMainMap.resetMiniMapBitmap();
            return true;
        }
        public Module LoadSaveGameModule(string filename)
        {
            Module toReturn = null;

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                toReturn = (Module)serializer.Deserialize(file, typeof(Module));
            }
            return toReturn;
        }
        public void updateContainers(Module saveMod)
        {
            foreach (Container saveCnt in saveMod.moduleContainersList)
            {
                //fill container with items that are still in the saved game 
                Container updatedCont = gv.mod.getContainerByTag(saveCnt.containerTag);
                if (updatedCont != null)
                {
                    //this container in the save also exists in the newMod so clear it out and add everything in the save
                    updatedCont.containerItemRefs.Clear();
                    foreach (ItemRefs it in saveCnt.containerItemRefs)
                    {
                        //check to see if item resref in save game container still exists in toolset
                        Item newItem = gv.mod.getItemByResRef(it.resref);
                        if (newItem != null)
                        {
                            updatedCont.containerItemRefs.Add(it.DeepCopy());
                        }
                    }
                    //compare lists and add items that are new
                    foreach (ItemRefs itemRef in updatedCont.initialContainerItemRefs)
                    {
                        //check to see if item in toolset does not exist in save initial list so it is new and add it
                        if (!saveCnt.containsInitialItemWithResRef(itemRef.resref))
                        {
                            //item is not in the saved game initial container item list so add it to the container
                            //check to see if item resref in save game container still exists in toolset
                            Item newItem = gv.mod.getItemByResRef(itemRef.resref);
                            if (newItem != null)
                            {
                                updatedCont.containerItemRefs.Add(itemRef.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
        public void updateShops(Module saveMod)
        {
            foreach (Shop saveShp in saveMod.moduleShopsList)
            {
                Shop updatedShop = gv.mod.getShopByTag(saveShp.shopTag);
                if (updatedShop != null)
                {
                    //this shop in the save also exists in the newMod so clear it out and add everything in the save
                    updatedShop.shopItemRefs.Clear();
                    foreach (ItemRefs it in saveShp.shopItemRefs)
                    {
                        Item newItem = gv.mod.getItemByResRef(it.resref);
                        if (newItem != null)
                        {
                            updatedShop.shopItemRefs.Add(it.DeepCopy());
                        }
                    }
                    //compare lists and add items that are new
                    foreach (ItemRefs itemRef in updatedShop.initialShopItemRefs)
                    {
                        if (!saveShp.containsInitialItemWithResRef(itemRef.resref))
                        {
                            //item is not in the saved game initial container item list so add it to the container
                            Item newItem = gv.mod.getItemByResRef(itemRef.resref);
                            if (newItem != null)
                            {
                                updatedShop.shopItemRefs.Add(itemRef.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
        public void updateAreas(Module saveMod)
        {
            foreach (Area ar in gv.mod.moduleAreasObjects)
            {
                foreach (Area sar in saveMod.moduleAreasObjects)
                {
                    if (sar.Filename.Equals(ar.Filename)) //sar is saved game, ar is new game from toolset version
                    {
                        //tiles
                        for (int index = 0; index < ar.Layer1Filename.Count; index++)
                        {
                            ar.Visible[index] = sar.Visible[index];
                        }

                        //props
                        //start at the end of the newMod prop list and work up
                        //if the prop tag is found in the save game, update it
                        //else if not found in saved game, but exists in the 
                        //saved game initial list (the toolset version of the prop list), remove prop
                        //else leave it alone
                        for (int index = ar.Props.Count - 1; index >= 0; index--)
                        {
                            Prop prp = ar.Props[index];
                            bool foundOne = false;
                            foreach (Prop sprp in sar.Props) //sprp is the saved game prop
                            {
                                if (prp.PropTag.Equals(sprp.PropTag))
                                {
                                    foundOne = true; //the prop tag exists in the saved game
                                    //replace the one in the toolset with the one from the saved game
                                    ar.Props.RemoveAt(index);
                                    ar.Props.Add(sprp.DeepCopy());
                                    //prp = sprp.DeepCopy();
                                    break;
                                }
                            }
                            if (!foundOne) //didn't find the prop tag in the saved game
                            {
                                if (sar.InitialAreaPropTagsList.Contains(prp.PropTag))
                                {
                                    //was once on the map, but was deleted so remove from the newMod prop list
                                    ar.Props.RemoveAt(index);
                                }
                                else
                                {
                                    //is new to the mod so leave it alone, don't remove from the prop list
                                }
                            }
                        }
                        //triggers
                        foreach (Trigger tr in ar.Triggers)
                        {
                            foreach (Trigger str in sar.Triggers)
                            {
                                if (tr.TriggerTag.Equals(str.TriggerTag))
                                {
                                    tr.Enabled = str.Enabled;
                                    tr.EnabledEvent1 = str.EnabledEvent1;
                                    tr.EnabledEvent2 = str.EnabledEvent2;
                                    tr.EnabledEvent3 = str.EnabledEvent3;
                                    //may want to copy the trigger's squares list from the save game if builders can modify the list with scripts
                                }
                            }
                        }
                    }
                }
            }
        }
        public void updatePlayers()
        {
            //load player Bitmap, race, class, known spells, equipped items
            foreach (Player pc in gv.mod.playerList)
            {
                try { pc.token = LoadBitmap(pc.tokenFilename); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.portrait = LoadBitmap(pc.portraitFilename); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.race = gv.mod.getRace(pc.raceTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.playerClass = gv.mod.getPlayerClass(pc.classTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                //may not need this(deleted already) as it is not used anywhere, only knownspellstags is used
            }
        }
        public void updatePartyRosterPlayers()
        {
            //load player Bitmap, race, class, known spells, equipped items
            foreach (Player pc in gv.mod.partyRosterList)
            {
                try { pc.token = LoadBitmap(pc.tokenFilename); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.portrait = LoadBitmap(pc.portraitFilename); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.race = gv.mod.getRace(pc.raceTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.playerClass = gv.mod.getPlayerClass(pc.classTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                //may not need this as it is not used anywhere, only knownspellstags is used
            }
        }
        public void setMainPc()
        {
            foreach (Player pc in gv.mod.playerList)
            {
                if (pc.mainPc)
                {
                    return;
                }
            }
            if (gv.mod.playerList.Count > 0)
            {
                gv.mod.playerList[0].mainPc = true;
            }
        }
        public Module LoadModule(string folderAndFilename, bool fullPath)
        {
            Module toReturn = null;
            if (fullPath)
            {
                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(folderAndFilename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    toReturn = (Module)serializer.Deserialize(file, typeof(Module));
                }
            }
            else
            {
                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(GetModulePath() + "\\" + folderAndFilename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    toReturn = (Module)serializer.Deserialize(file, typeof(Module));
                }
            }
            return toReturn;
        }
        public void LoadRaces()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\races.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleRacesList = (List<Race>)serializer.Deserialize(file, typeof(List<Race>));
            }
        }
        public void LoadPlayerClasses()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\playerClasses.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.modulePlayerClassList = (List<PlayerClass>)serializer.Deserialize(file, typeof(List<PlayerClass>));
            }
        }
        public void LoadItems()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\items.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleItemsList = (List<Item>)serializer.Deserialize(file, typeof(List<Item>));
            }
        }
        public void LoadContainers()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\containers.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleContainersList = (List<Container>)serializer.Deserialize(file, typeof(List<Container>));
            }
        }
        public void LoadShops()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\shops.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleShopsList = (List<Shop>)serializer.Deserialize(file, typeof(List<Shop>));
            }
        }
        public void LoadJournal()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\journal.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleJournal = (List<JournalQuest>)serializer.Deserialize(file, typeof(List<JournalQuest>));
            }
        }
        public void LoadEffects()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\effects.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleEffectsList = (List<Effect>)serializer.Deserialize(file, typeof(List<Effect>));
            }
        }
        public void LoadSpells()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\spells.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleSpellsList = (List<Spell>)serializer.Deserialize(file, typeof(List<Spell>));
            }
        }
        public void LoadTraits()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\traits.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleTraitsList = (List<Trait>)serializer.Deserialize(file, typeof(List<Trait>));
            }
        }
                
        public void LoadCreatures()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\creatures.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleCreaturesList = (List<Creature>)serializer.Deserialize(file, typeof(List<Creature>));
            }
        }
        public void LoadEncounters()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\encounters.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleEncountersList = (List<Encounter>)serializer.Deserialize(file, typeof(List<Encounter>));
            }
        }

        public string GetModulePath()
        {
            return gv.mainDirectory + "\\modules\\" + gv.mod.moduleName;
        }

        //GENERAL
        public void nullOutControls()
        {
            btnReturn = null;
            ctrlUpArrow = null;
            ctrlDownArrow = null;
            ctrlLeftArrow = null;
            ctrlRightArrow = null;
            ctrlUpRightArrow = null;
            ctrlDownLeftArrow = null;
            ctrlUpLeftArrow = null;
            ctrlDownRightArrow = null;
            btnInventory = null;
            btnHelp = null;
            tglSound = null;
        }        
        public void setPanelsStart()
        {
            int mapSize = gv.playerOffsetY + gv.playerOffsetY + 1;
            int topPanelHeight = mapSize - 2;
            int leftPanelWidth = gv.squaresInWidth - mapSize - 4;

            if (pnlLog == null)
            {
                pnlLog = new IbbPanel(gv);
                pnlLog.ImgBG = this.LoadBitmap("ui_bg_log");
                pnlLog.LocX = gv.oXshift - gv.pS;
                pnlLog.LocY = 0;
                pnlLog.Height = topPanelHeight * gv.squareSize;
                pnlLog.Width = leftPanelWidth * gv.squareSize;
                gv.log.tbWidth = leftPanelWidth * gv.squareSize - gv.pS - gv.pS;
                gv.log.tbHeight = topPanelHeight * gv.squareSize - gv.pS - gv.pS;
            }
            if (pnlToggles == null)
            {
                pnlToggles = new IbbPanel(gv);
                pnlToggles.ImgBG = this.LoadBitmap("ui_bg_toggles");
                pnlToggles.LocX = gv.oXshift - gv.pS;
                pnlToggles.LocY = topPanelHeight * gv.squareSize + gv.pS;
                pnlToggles.Height = 3 * gv.squareSize + gv.pS + gv.pS;
                pnlToggles.Width = leftPanelWidth * gv.squareSize;
            }
            if (pnlPortraits == null)
            {
                pnlPortraits = new IbbPanel(gv);
                pnlPortraits.ImgBG = this.LoadBitmap("ui_bg_portraits");
                pnlPortraits.LocX = (leftPanelWidth + mapSize) * gv.squareSize + gv.oXshift + gv.pS;
                pnlPortraits.LocY = 0 * gv.squareSize;
                pnlPortraits.Height = topPanelHeight * gv.squareSize;
                pnlPortraits.Width = 4 * gv.squareSize;
            }
            if (pnlArrows == null)
            {
                pnlArrows = new IbbPanel(gv);
                pnlArrows.ImgBG = this.LoadBitmap("ui_bg_arrows");
                pnlArrows.LocX = (leftPanelWidth + mapSize) * gv.squareSize + gv.oXshift + gv.pS;
                pnlArrows.LocY = topPanelHeight * gv.squareSize + gv.pS;
                pnlArrows.Height = 3 * gv.squareSize + gv.pS + gv.pS;
                pnlArrows.Width = 4 * gv.squareSize;
            }
            if (pnlHotkeys == null)
            {
                pnlHotkeys = new IbbPanel(gv);
                pnlHotkeys.ImgBG = this.LoadBitmap("ui_bg_hotkeys");
                pnlHotkeys.LocX = leftPanelWidth * gv.squareSize + gv.oXshift;
                pnlHotkeys.LocY = mapSize * gv.squareSize + gv.pS;
                pnlHotkeys.Height = 1 * gv.squareSize + gv.pS + gv.pS;
                pnlHotkeys.Width = mapSize * gv.squareSize;
            }
        }
        public void setControlsStart()
        {
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padW = gv.squareSize / 6;
            int hotkeyShift = 0;
            if (gv.useLargeLayout)
            {
                hotkeyShift = 1;
            }

            if (ctrlUpArrow == null)
            {
                ctrlUpArrow = new IbbButton(gv, 1.0f);
                ctrlUpArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlUpArrow.Img2 = this.LoadBitmap("ctrl_up_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_up_arrow);
                ctrlUpArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlUpArrow.X = gv.cc.pnlArrows.LocX + 1 * gv.squareSize + gv.squareSize / 2;
                ctrlUpArrow.Y = gv.cc.pnlArrows.LocY + 0 * gv.squareSize + gv.pS;
                ctrlUpArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlUpArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlLeftArrow == null)
            {
                ctrlLeftArrow = new IbbButton(gv, 1.0f);
                ctrlLeftArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlLeftArrow.Img2 = this.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_left_arrow);
                ctrlLeftArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlLeftArrow.X = gv.cc.pnlArrows.LocX + 0 * gv.squareSize + gv.squareSize / 2;
                ctrlLeftArrow.Y = gv.cc.pnlArrows.LocY + 1 * gv.squareSize + gv.pS;
                ctrlLeftArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlLeftArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlRightArrow == null)
            {
                ctrlRightArrow = new IbbButton(gv, 1.0f);
                ctrlRightArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlRightArrow.Img2 = this.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_right_arrow);
                ctrlRightArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlRightArrow.X = gv.cc.pnlArrows.LocX + 2 * gv.squareSize + gv.squareSize / 2;
                ctrlRightArrow.Y = gv.cc.pnlArrows.LocY + 1 * gv.squareSize + gv.pS;
                ctrlRightArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlRightArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlDownArrow == null)
            {
                ctrlDownArrow = new IbbButton(gv, 1.0f);
                ctrlDownArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlDownArrow.Img2 = this.LoadBitmap("ctrl_down_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_down_arrow);
                ctrlDownArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlDownArrow.X = gv.cc.pnlArrows.LocX + 1 * gv.squareSize + gv.squareSize / 2;
                ctrlDownArrow.Y = gv.cc.pnlArrows.LocY + 2 * gv.squareSize + gv.pS;
                ctrlDownArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlDownArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlUpRightArrow == null)
            {
                ctrlUpRightArrow = new IbbButton(gv, 1.0f);
                ctrlUpRightArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlUpRightArrow.Img2 = this.LoadBitmap("ctrl_up_right_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_up_right_arrow);
                ctrlUpRightArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlUpRightArrow.X = gv.cc.pnlArrows.LocX + 2 * gv.squareSize + gv.squareSize / 2;
                ctrlUpRightArrow.Y = gv.cc.pnlArrows.LocY + 0 * gv.squareSize + gv.pS;
                ctrlUpRightArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlUpRightArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlUpLeftArrow == null)
            {
                ctrlUpLeftArrow = new IbbButton(gv, 1.0f);
                ctrlUpLeftArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlUpLeftArrow.Img2 = this.LoadBitmap("ctrl_up_left_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_up_left_arrow);
                ctrlUpLeftArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlUpLeftArrow.X = gv.cc.pnlArrows.LocX + 0 * gv.squareSize + gv.squareSize / 2;
                ctrlUpLeftArrow.Y = gv.cc.pnlArrows.LocY + 0 * gv.squareSize + gv.pS;
                ctrlUpLeftArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlUpLeftArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlDownRightArrow == null)
            {
                ctrlDownRightArrow = new IbbButton(gv, 1.0f);
                ctrlDownRightArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlDownRightArrow.Img2 = this.LoadBitmap("ctrl_down_right_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_down_right_arrow);
                ctrlDownRightArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlDownRightArrow.X = gv.cc.pnlArrows.LocX + 2 * gv.squareSize + gv.squareSize / 2;
                ctrlDownRightArrow.Y = gv.cc.pnlArrows.LocY + 2 * gv.squareSize + gv.pS;
                ctrlDownRightArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlDownRightArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlDownLeftArrow == null)
            {
                ctrlDownLeftArrow = new IbbButton(gv, 1.0f);
                ctrlDownLeftArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlDownLeftArrow.Img2 = this.LoadBitmap("ctrl_down_left_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_down_left_arrow);
                ctrlDownLeftArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlDownLeftArrow.X = gv.cc.pnlArrows.LocX + 0 * gv.squareSize + gv.squareSize / 2;
                ctrlDownLeftArrow.Y = gv.cc.pnlArrows.LocY + 2 * gv.squareSize + gv.pS;
                ctrlDownLeftArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlDownLeftArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnInventory == null)
            {
                btnInventory = new IbbButton(gv, 0.8f);
                btnInventory.HotKey = "I";
                btnInventory.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnInventory.Img2 = this.LoadBitmap("btninventory"); // BitmapFactory.decodeResource(getResources(), R.drawable.btninventory);
                btnInventory.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnInventory.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 1) * gv.squareSize;
                btnInventory.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
                btnInventory.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventory.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnHelp == null)
            {
                btnHelp = new IbbButton(gv, 0.8f);
                btnHelp.Text = "HELP";
                btnHelp.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnHelp.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnHelp.X = 6 * gv.squareSize + padW * 1 + gv.oXshift;
                btnHelp.Y = 9 * gv.squareSize + pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnReturn == null)
            {
                btnReturn = new IbbButton(gv, 1.0f);
                btnReturn.Text = "RETURN";
                btnReturn.Img = this.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_large);
                btnReturn.Glow = this.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_large_glow);
                btnReturn.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnReturn.Y = 10 * gv.squareSize + pH * 2;
                btnReturn.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturn.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }

        }
        public void setPortraitsStart()
        {
            float scale = 1.0f;
            int tabX1 = gv.cc.pnlPortraits.LocX + 0 * gv.squareSize + gv.squareSize / 2 + gv.pS;
            int tabX2 = gv.cc.pnlPortraits.LocX + 2 * gv.squareSize + gv.pS;
            int tabY1 = gv.cc.pnlPortraits.LocY + 0 * gv.squareSize + gv.squareSize / 2;
            int tabY2 = gv.cc.pnlPortraits.LocY + 2 * gv.squareSize + gv.squareSize / 2;
            int tabY3 = gv.cc.pnlPortraits.LocY + 4 * gv.squareSize + gv.squareSize / 2;
                        
            if (!gv.useLargeLayout)
            {
                scale = 0.75f;
                tabY1 = gv.cc.pnlPortraits.LocY + 0 * gv.squareSize + gv.squareSize / 2;
                tabY2 = gv.cc.pnlPortraits.LocY + 2 * gv.squareSize;
                tabY3 = gv.cc.pnlPortraits.LocY + 3 * gv.squareSize + gv.squareSize / 2;
            }
            int ptrHeight = (int)(gv.ibpheight * gv.screenDensity * scale);
            int ptrWidth = (int)(gv.ibpwidth * gv.screenDensity * scale);

            if (ptrPc0 == null)
            {
                ptrPc0 = new IbbPortrait(gv, 0.8f);
                ptrPc0.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc0.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc0.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc0.X = tabX1;
                ptrPc0.Y = tabY1;
                ptrPc0.Height = ptrHeight;
                ptrPc0.Width = ptrWidth;
            }
            if (ptrPc1 == null)
            {
                ptrPc1 = new IbbPortrait(gv, 0.8f);
                ptrPc1.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc1.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc1.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc1.X = tabX2;
                ptrPc1.Y = tabY1;
                ptrPc1.Height = ptrHeight;
                ptrPc1.Width = ptrWidth;
            }
            if (ptrPc2 == null)
            {
                ptrPc2 = new IbbPortrait(gv, 0.8f);
                ptrPc2.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc2.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc2.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc2.X = tabX1;
                ptrPc2.Y = tabY2;
                ptrPc2.Height = ptrHeight;
                ptrPc2.Width = ptrWidth;
            }
            if (ptrPc3 == null)
            {
                ptrPc3 = new IbbPortrait(gv, 0.8f);
                ptrPc3.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc3.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc3.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc3.X = tabX2;
                ptrPc3.Y = tabY2;
                ptrPc3.Height = ptrHeight;
                ptrPc3.Width = ptrWidth;
            }
            if (ptrPc4 == null)
            {
                ptrPc4 = new IbbPortrait(gv, 0.8f);
                ptrPc4.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc4.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc4.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc4.X = tabX1;
                ptrPc4.Y = tabY3;
                ptrPc4.Height = ptrHeight;
                ptrPc4.Width = ptrWidth;
            }
            if (ptrPc5 == null)
            {
                ptrPc5 = new IbbPortrait(gv, 0.8f);
                ptrPc5.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc5.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc5.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc5.X = tabX2;
                ptrPc5.Y = tabY3;
                ptrPc5.Height = ptrHeight;
                ptrPc5.Width = ptrWidth;
            }
        }
        public void setToggleButtonsStart()
        {
            if (tglSound == null)
            {
                tglSound = new IbbToggleButton(gv);
                tglSound.ImgOn = this.LoadBitmap("tgl_music_on"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sound_on);
                tglSound.ImgOff = this.LoadBitmap("tgl_music_off"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sound_off);
                tglSound.X = gv.cc.pnlToggles.LocX + 2 * gv.squareSize + gv.squareSize / 4;
                tglSound.Y = gv.cc.pnlToggles.LocY + 1 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglSound.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglSound.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
            }
        }

        //TUTORIAL MESSAGES
        public void tutorialMessageMainMap()
        {
            gv.sf.MessageBoxHtml(this.stringMessageMainMap);
        }
        public void tutorialMessageParty(bool helpCall)
        {
            if ((gv.mod.showTutorialParty) || (helpCall))
            {
                gv.sf.MessageBoxHtml(this.stringMessageParty);
                gv.mod.showTutorialParty = false;
            }
        }
        public void tutorialMessageInventory(bool helpCall)
        {
            if ((gv.mod.showTutorialInventory) || (helpCall))
            {
                gv.sf.MessageBoxHtml(this.stringMessageInventory);
                gv.mod.showTutorialInventory = false;
            }
        }
        public void tutorialMessageCombat(bool helpCall)
        {
            if ((gv.mod.showTutorialCombat) || (helpCall))
            {
                gv.sf.MessageBoxHtml(this.stringMessageCombat);
                gv.mod.showTutorialCombat = false;
            }
        }
        public void tutorialPcCreation()
        {
            gv.sf.MessageBoxHtml(this.stringPcCreation);
        }
        public void tutorialPlayersGuide()
        {
            gv.sf.MessageBoxHtml(this.stringPlayersGuide);
        }
        public void tutorialBeginnersGuide()
        {
            gv.sf.MessageBoxHtml(this.stringBeginnersGuide);
        }

        public void addLogText(string color, string text)
        {
            if (color.Equals("red"))
            {
                gv.log.AddHtmlTextToLog("<font color='red'>" + text + "</font>");
            }
            else if (color.Equals("lime"))
            {
                gv.log.AddHtmlTextToLog("<font color='lime'>" + text + "</font>");
            }
            else if (color.Equals("yellow"))
            {
                gv.log.AddHtmlTextToLog("<font color='yellow'>" + text + "</font>");
            }
            else if (color.Equals("teal"))
            {
                gv.log.AddHtmlTextToLog("<font color='teal'>" + text + "</font>");
            }
            else if (color.Equals("blue"))
            {
                gv.log.AddHtmlTextToLog("<font color='blue'>" + text + "</font>");
            }
            else if (color.Equals("fuchsia"))
            {
                gv.log.AddHtmlTextToLog("<font color='fuchsia'>" + text + "</font>");
            }
            else
            {
                gv.log.AddHtmlTextToLog("<font color='white'>" + text + "</font>");
            }
            /*
            <?xml version="1.0" encoding="utf-8"?>
            <resources>
             <color name="white">#FFFFFF</color>
             <color name="yellow">#FFFF00</color>
             <color name="fuchsia">#FF00FF</color>
             <color name="red">#FF0000</color>
             <color name="silver">#C0C0C0</color>
             <color name="gray">#808080</color>
             <color name="olive">#808000</color>
             <color name="purple">#800080</color>
             <color name="maroon">#800000</color>
             <color name="aqua">#00FFFF</color>
             <color name="lime">#00FF00</color>
             <color name="teal">#008080</color>
             <color name="green">#008000</color>
             <color name="blue">#0000FF</color>
             <color name="navy">#000080</color>
             <color name="black">#000000</color>
            </resources>
            */
        }
        public void addLogText(string text)
        {
            gv.log.AddHtmlTextToLog(text);		
        }
        public void addFloatyText(Coordinate coorInSquares, string value)
        {
            int txtH = (int)gv.drawFontRegHeight;
            int x = ((coorInSquares.X * gv.squareSize) + (gv.squareSize / 2) + gv.oXshift) - (txtH / 2);
            int y = ((coorInSquares.Y * gv.squareSize) + (gv.squareSize / 2) + txtH) - (txtH / 2);
            Coordinate coor = new Coordinate(x, y);
            floatyTextList.Add(new FloatyText(coor, value));
        }
        public void addFloatyText(Coordinate coorInSquares, string value, string color)
        {
            int txtH = (int)gv.drawFontRegHeight;
            int x = ((coorInSquares.X * gv.squareSize) + (gv.squareSize / 2) + gv.oXshift) - (txtH / 2);
            int y = ((coorInSquares.Y * gv.squareSize) + (gv.squareSize / 2) + txtH) - (txtH / 2);
            Coordinate coor = new Coordinate(x, y);
            floatyTextList.Add(new FloatyText(coor, value, color));
        }
        public void addFloatyText(Coordinate coorInSquares, string value, int shiftUp)
        {
            int txtH = (int)gv.drawFontRegHeight;
            int x = ((coorInSquares.X * gv.squareSize) + (gv.squareSize / 2) + gv.oXshift) - (txtH / 2);
            int y = ((coorInSquares.Y * gv.squareSize) + (gv.squareSize / 2) + txtH) - (txtH / 2) - shiftUp;
            Coordinate coor = new Coordinate(x, y);
            floatyTextList.Add(new FloatyText(coor, value));
        }

        public void doSettingsDialogs()
        {
            /*TODO
		    try
            {
			    final CharSequence[] items = {"Play Music","Button Sounds","Button Vibrate","DebugMode","Autosave Upon Exit","Show Message When Autosaving"};
	        
        	    //final Container container = mod.getContainerByTag(tag);
        	    final boolean[] checkedSettings = new boolean[6]; 
        	    if (gv.mod.playMusic)
                {
            	    checkedSettings[0] = true;
                }
                if (gv.mod.playButtonSounds)
                {
            	    checkedSettings[1] = true;
                }
                if (gv.mod.playButtonHaptic)
                {
            	    checkedSettings[2] = true;
                }
                if (gv.mod.debugMode)
                {
            	    checkedSettings[3] = true;
                }     
                if (gv.mod.allowAutosave)
                {
            	    checkedSettings[4] = true;
                }
                if (gv.mod.showAutosaveMessage)
                {
            	    checkedSettings[5] = true;
                }
        	    //set initial states for checkedSettings for loop
        	    //final List<String> containerItems = new ArrayList<String>();
                      
                //final CharSequence[] items = containerItems.toArray(new CharSequence[containerItems.size()]);
                // Creating and Building the Dialog 
                AlertDialog.Builder builder = new AlertDialog.Builder(gv.gameContext);
                builder.setTitle("Settings and Guides");
                builder.setMultiChoiceItems(items, checkedSettings, new DialogInterface.OnMultiChoiceClickListener() {
				
				    @Override
				    public void onClick(DialogInterface dialog, int which, boolean isChecked) {
					    switch (which)
					    {
					    case 0:
						    if (gv.mod.playMusic)
	                        {
							    gv.stopMusic();
							    gv.stopAmbient();
							    gv.mod.playMusic = false;
	    					    addLogText("lime","Music Off");
	                        }
	                        else
	                        {
	                    	    gv.startMusic();
	                    	    gv.startAmbient();
	                    	    gv.mod.playMusic = true;
	                    	    addLogText("lime","Music On");
	                        }					
						    break;
						
					    case 1:
						    if (gv.mod.playButtonSounds)
	                        {
							    gv.mod.playButtonSounds = false;
	    					    addLogText("lime","Button Sounds Off");
	                        }
	                        else
	                        {
	                    	    gv.mod.playButtonSounds = true;
	                    	    addLogText("lime","Button Sounds On");
	                        }					
						    break;	
						
					    case 2:
						    if (gv.mod.playButtonHaptic)
	                        {
							    gv.mod.playButtonHaptic = false;
	    					    addLogText("lime","Button Vibrate Off");
	                        }
	                        else
	                        {
	                    	    gv.mod.playButtonHaptic = true;
	                    	    addLogText("lime","Button Vibrate On");
	                        }
						    break;
						
					    case 3:
						    if (gv.mod.debugMode)
	                        {
							    gv.mod.debugMode = false;
	    					    addLogText("lime","DebugMode Off");
	                        }
	                        else
	                        {
	                    	    gv.mod.debugMode = true;
	                    	    addLogText("lime","DebugMode On");
	                        }
						    break;
					
					    case 4:
						    if (gv.mod.allowAutosave)
	                        {
							    gv.mod.allowAutosave = false;
	    					    addLogText("lime","Autosave Off");
	                        }
	                        else
	                        {
	                    	    gv.mod.allowAutosave = true;
	                    	    addLogText("lime","Autosave On");
	                        }
						    break;
						
					    case 5:
						    if (gv.mod.showAutosaveMessage)
	                        {
							    gv.mod.showAutosaveMessage = false;
	    					    addLogText("lime","Autosave Message Off");
	                        }
	                        else
	                        {
	                    	    gv.mod.showAutosaveMessage = true;
	                    	    addLogText("lime","Autosave Message On");
	                        }
						    break;
					    }
				    }
			    });
            
                // Set the action buttons
                builder.setPositiveButton("Player's Guide", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int id) 
                    {           
                	    tutorialPlayersGuide();
                        // user clicked OK, so save the mSelectedItems results somewhere
                        // here we are trying to retrieve the selected items indices
                        //ActionDialog.dismiss();
                        //invalidate();
                    }
                });
             
                builder.setNeutralButton("OK", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int id) 
                    {                     
                        // user clicked OK, so save the mSelectedItems results somewhere
                        // here we are trying to retrieve the selected items indices
                	    gv.ActionDialog.dismiss();
                	    gv.invalidate();
                    }
                });
            
                builder.setNegativeButton("Beginner's Guide", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int id) 
                    {
                	    tutorialBeginnersGuide();
                    }
                });
            
                gv.ActionDialog = builder.create();
                gv.ActionDialog.show();
            }
            catch (Exception ex)
            {
                //IBMessageBox.Show(game, "failed to open conversation with tag: " + tag);
            }
            */
        }

        public void doAboutDialog()
        {
            gv.sf.MessageBoxHtml(gv.mod.moduleCredits);
        }

        public void SwitchToNextAvailablePartyLeader()
        {
            int idx = 0;
            foreach (Player pc in gv.mod.playerList)
            {
                if (!pc.isDead())
                {
                    gv.mod.selectedPartyLeader = idx;
                    return;
                }
                idx++;
            }
        }

        public void doUpdate()
        {
            //reset the timer interval, important for synching with party move
            if (gv.mod.useRealTimeTimer == true)
            {
                //gv.realTimeTimer.Stop();
                //gv.realTimeTimer.Start();
            }

            //in case whole party is unconscious and bleeding, end the game (outside combat here)
            bool endGame = true;
            foreach (Player pc in gv.mod.playerList)
            {
                if (pc.hp >= 0)
                {
                    endGame = false;
                    break;
                }
            }

            if (endGame == true)
            {
                gv.resetGame();
                gv.screenType = "title";
                IBMessageBox.Show(gv, "Everybody is unconscious and bleeding - your party has been defeated!");
                return;
            }
                        
            //CLEAN UP START SCREENS IF DONE WITH THEM
            if (gv.screenLauncher != null)
            {
                gv.screenLauncher = null;
                gv.screenPartyBuild = null;
                gv.screenPcCreation = null;
                gv.screenTitle = null;
            }

            gv.sf.dsWorldTime();
            //IBScript Module heartbeat
            gv.cc.doIBScriptBasedOnFilename(gv.mod.OnHeartBeatIBScript, gv.mod.OnHeartBeatIBScriptParms);
            //IBScript Area heartbeat
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentArea.OnHeartBeatIBScript, gv.mod.currentArea.OnHeartBeatIBScriptParms);
            //apply effects
            applyEffects();
            //do Prop heartbeat
            doPropHeartBeat();
            
            //script hook for full screen effects on channels 1 to 4 (also called in doTransitionBasedOnAreaLocation)
            //doChannelScripts();
            
            blockSecondPropTriggersCall = false;
            //do Conversation and/or Encounter if on Prop (check before props move)
            gv.triggerPropIndex = 0;
            gv.triggerIndex = 0;
            doPropTriggers();

            //move any props that are active and only if they are not on the party location
            doPropMoves();

            //do Conversation and/or Encounter if on Prop (check after props move)
            if (!blockSecondPropTriggersCall)
            {
                gv.triggerPropIndex = 0;
                gv.triggerIndex = 0;
                doPropTriggers();
            }

            //move any props that are active and only if they are not on the party location
            //doPropMoves();

            //check for levelup available and switch button image
            checkLevelUpAvailable(); //move this to on update and use a plus overlay in top left            
        }
                
        public void checkLevelUpAvailable()
        {            
            if (gv.mod.playerList.Count > 0)
            {
                if (gv.mod.playerList[0].IsReadyToAdvanceLevel()) { gv.cc.ptrPc0.levelUpOn = true; }
                else { gv.cc.ptrPc0.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 1)
            {
                if (gv.mod.playerList[1].IsReadyToAdvanceLevel()) { gv.cc.ptrPc1.levelUpOn = true; }
                else { gv.cc.ptrPc1.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 2)
            {
                if (gv.mod.playerList[2].IsReadyToAdvanceLevel()) { gv.cc.ptrPc2.levelUpOn = true; }
                else { gv.cc.ptrPc2.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 3)
            {
                if (gv.mod.playerList[3].IsReadyToAdvanceLevel()) { gv.cc.ptrPc3.levelUpOn = true; }
                else { gv.cc.ptrPc3.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 4)
            {
                if (gv.mod.playerList[4].IsReadyToAdvanceLevel()) { gv.cc.ptrPc4.levelUpOn = true; }
                else { gv.cc.ptrPc4.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 5)
            {
                if (gv.mod.playerList[5].IsReadyToAdvanceLevel()) { gv.cc.ptrPc5.levelUpOn = true; }
                else { gv.cc.ptrPc5.levelUpOn = false; }
            }
        }
                
        public void doPropHeartBeat()
        {
            foreach (Prop prp in gv.mod.currentArea.Props)
            { 
                gv.sf.ThisProp = prp;
                //IBScript Prop heartbeat
                gv.cc.doIBScriptBasedOnFilename(prp.OnHeartBeatIBScript, prp.OnHeartBeatIBScriptParms);
                gv.sf.ThisProp = null;
            }
        }
        
        public void doPropMoves()
        {
            //foreach (Prop propObject in gv.mod.currentArea.Props)
            //{
                //propObject.lastLocationX = propObject.LocationX;
                //propObject.lastLocationY = propObject.LocationY;
            //}
            
            #region Synchronization: update the position of time driven movers (either when the party switches area or when a time driven mover enters the current area)

            //Synchronization: check for all time driven movers either 1. found when entering an area (three variants: move into current area, move on current area, move out of current area) or 2. coming in from outside while party is already on current area
            //three nested loops running through area/prop/waypoint
            for (int i = gv.mod.moduleAreasObjects.Count - 1; i >= 0; i--)
            {
                //the check for the two conditions itself; donOnEnterAreaUpdate is set in the region above 
                if ((gv.mod.moduleAreasObjects[i].Filename != gv.mod.currentArea.Filename) || (doOnEnterAreaUpdate))
                {
                    for (int j = gv.mod.moduleAreasObjects[i].Props.Count - 1; j >= 0; j--)
                    {
                        int relevantAreaIndex = 0;
                        int relevantPropIndex = 0;
                        int relevantWaypointIndex = 0;
                        bool foundProp = false;
                        int nearestPointInTime = 0;
                        string relevantPropTag = "";

                        if ((gv.mod.moduleAreasObjects[i].Props[j].MoverType == "daily") || (gv.mod.moduleAreasObjects[i].Props[j].MoverType == "weekly") || (gv.mod.moduleAreasObjects[i].Props[j].MoverType == "monthly") || (gv.mod.moduleAreasObjects[i].Props[j].MoverType == "yearly"))
                        {

                            int listEndCheckedIndexOfNextWaypoint = 0;
                            for (int k = gv.mod.moduleAreasObjects[i].Props[j].WayPointList.Count - 1; k >= 0; k--)
                            {
                                List<string> timeUnitsList = new List<string>();
                                int currentTimeInInterval = 0;

                                //convert the string from the toolset for departure time into separate ints, filtering out ":" and blanks
                                //format in toolset is number:number:number
                                //with these ranges [0 to 336]:[0 to 23]:[0 to 59]
                                //actually it's 1 to 336 for for intuitive feeling, but below code treats zero and 1 the same way
                                //think: 1 equals monday, 2 equals tuesday and so forth
                                timeUnitsList = gv.mod.moduleAreasObjects[i].Props[j].WayPointList[k].departureTime.Split(':').Select(x => x.Trim()).ToList();

                                int dayCounter = Convert.ToInt32(timeUnitsList[0]);
                                int hourCounter = Convert.ToInt32(timeUnitsList[1]);
                                int minuteCounter = Convert.ToInt32(timeUnitsList[2]);

                                //catch entries of zero
                                //counter is reduced by one to make below calculation work the same for day/minutes/hours
                                if ((dayCounter == 0) || (dayCounter == 1))
                                {
                                    dayCounter = 0;
                                }
                                else
                                {
                                    dayCounter = (dayCounter - 1);
                                }

                                //turn the the three counters into one number for departure time (in seconds)
                                int convertedDepartureTime = dayCounter * 86400 + hourCounter * 3600 + minuteCounter * 60;

                                //automatically overwritwe departure time for last in line waypoint to be at the end of the respective time interval 
                                //and factor in the duration of one step 
                                //this makes sure that within each time cycle every waypoint is only used once

                                if (k == gv.mod.moduleAreasObjects[i].Props[j].WayPointList.Count - 1)
                                {
                                    if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("daily"))
                                    {
                                        convertedDepartureTime = 86400 - (gv.mod.currentArea.TimePerSquare * 60 + 1);//times 60 is necceessary as world time and time per square are measured in minutes  
                                    }
                                    if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("weekly"))
                                    {
                                        convertedDepartureTime = 604800 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                    }
                                    if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("monthly"))
                                    {
                                        convertedDepartureTime = 2419200 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                    }
                                    if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("yearly"))
                                    {
                                        convertedDepartureTime = 29030400 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                    }
                                }

                                if (k == 0)
                                {
                                    if (convertedDepartureTime < (gv.mod.currentArea.TimePerSquare * 60 + 1))
                                    {
                                        convertedDepartureTime = gv.mod.currentArea.TimePerSquare * 60 + 1;
                                    }
                                }

                                //use modulo operation to get the current time (in seconds) in each of the intervals
                                //the intervalls endlessly run from zero to maximum length to zero to maximum length and so forth
                                if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("daily"))
                                {
                                    currentTimeInInterval = (gv.mod.WorldTime * 60) % 86400;
                                }
                                if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("weekly"))
                                {
                                    currentTimeInInterval = (gv.mod.WorldTime * 60) % 604800;
                                }
                                if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("monthly"))
                                {
                                    currentTimeInInterval = (gv.mod.WorldTime * 60) % 2419200;
                                }
                                if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("yearly"))
                                {
                                    currentTimeInInterval = (gv.mod.WorldTime * 60) % 29030400;
                                }

                                //we look for waypoints whose time has already been reached in this step
                                if (currentTimeInInterval >= convertedDepartureTime)
                                {
                                    //we filter those waypoints out who are older than other waypoitns whose time has already been reached
                                    if (convertedDepartureTime > nearestPointInTime)
                                    {
                                        //we store and overwrite the waypoints whose time has been reached, overwriting the older ones so long until only the youngest waypoint whose time has already been reached remains
                                        nearestPointInTime = convertedDepartureTime;
                                        relevantAreaIndex = i;
                                        relevantPropIndex = j;
                                        relevantWaypointIndex = k;
                                        relevantPropTag = gv.mod.moduleAreasObjects[i].Props[j].PropTag;
                                        foundProp = true;
                                    }
                                }
                            }

                            //a waypint whose time has been reached has been found in above step, it's the youngest of these
                            if (foundProp)
                            {
                                //activate the filter again for the next props in the loop
                                foundProp = false;

                                //check whether the waypoint found was a last in line wayoint
                                //this is important as we will look whether the waypoint after the found one has a different area name
                                //note: if areea name of next waypoint is different than current one, the prop is transitioned to the other area (going in or out)
                                if (relevantWaypointIndex >= gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList.Count - 1)
                                {
                                    listEndCheckedIndexOfNextWaypoint = 0;
                                }
                                else
                                {
                                    listEndCheckedIndexOfNextWaypoint = relevantWaypointIndex + 1;
                                }

                                //we check the situation that the party enters a fresh area
                                //there are three situations to handle:
                                //1. the current waypoint is on different map, but the next waypoint is on current map: move prop to next waypoint (move into area)
                                //2. the current waypoint and the next are on current map: move prop to current waypoint (move on current area)
                                //3. the current waypoint is on this map, but the next waypoint is on different map: move prop to next waypoint (move out of current area)

                                if (doOnEnterAreaUpdate == true)
                                {
                                    //1. the current waypoint is on different map, but the next waypoint is on current map: move prop to next waypoint (move into area)
                                    if ((gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName == gv.mod.currentArea.Filename) && (gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName != gv.mod.currentArea.Filename))
                                    {
                                        //apply only for props that are not already in current area
                                        bool isOnCurrentAreaAlready = false;
                                        foreach (Prop p in gv.mod.currentArea.Props)
                                        {
                                            if (p.PropTag == relevantPropTag)
                                            {
                                                isOnCurrentAreaAlready = true;
                                            }
                                        }

                                        if (isOnCurrentAreaAlready == false)
                                        {
                                            //we assign the index of next in line waypoint
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = listEndCheckedIndexOfNextWaypoint;
                                            //set move to target coordinates
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y.ToString());
                                        }
                                        //prop already exists on current area, so we only relocate it, but no transfer
                                        else
                                        {
                                            //we assign the index of next in line waypoint
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = listEndCheckedIndexOfNextWaypoint;
                                            //set move to target coordinates
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                            gv.sf.osController("osSetPropLocation.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y.ToString());
                                        }
                                    }

                                    //2. the current waypoint and the next are on current map: move prop to current waypoint (move on current area, venetually transfer in from other area)
                                    else if ((gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName == gv.mod.currentArea.Filename) && (gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName == gv.mod.currentArea.Filename))
                                    {
                                        //apply only for props that are not already in current area
                                        bool isOnCurrentAreaAlready = false;
                                        foreach (Prop p in gv.mod.currentArea.Props)
                                        {
                                            if (p.PropTag == relevantPropTag)
                                            {
                                                isOnCurrentAreaAlready = true;
                                            }
                                        }
                                        //prop is not on current area, so transfer and tehn relocate it
                                        if (isOnCurrentAreaAlready == false)
                                        {
                                            //note: the index will be updated a few lines down in the normal move section to the correct target
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = relevantWaypointIndex;
                                            //note: the move to target coordinates will be updated a few lines down in the normal move section
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].Y;
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].Y.ToString());
                                        }
                                        //the prop is already on the current area, so just relocate it on area
                                        else
                                        {
                                            //note: the index will be updated a few lines down in the normal move section to the correct target
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = relevantWaypointIndex;
                                            //note: the move to target coordinates will be updated a few lines down in the normal move section
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].Y;
                                            gv.sf.osController("osSetPropLocation.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].Y.ToString());
                                        }
                                    }
                                    //3. remove from current area (move out of current area)
                                    else if ((gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName != gv.mod.currentArea.Filename) && (gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName == gv.mod.currentArea.Filename))
                                    {
                                        //apply only for props that ARE already in current area
                                        bool isOnCurrentAreaAlready = false;
                                        foreach (Prop p in gv.mod.currentArea.Props)
                                        {
                                            if (p.PropTag == relevantPropTag)
                                            {
                                                isOnCurrentAreaAlready = true;
                                            }
                                        }

                                        if (isOnCurrentAreaAlready)
                                        {
                                            //we assign the index of next in line waypoint
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = listEndCheckedIndexOfNextWaypoint;
                                            //set move to target coordinates
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y.ToString());
                                        }

                                    }
                                    //4. remove from echo prop (and transfer to fitting area)
                                    else if ((gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName != gv.mod.currentArea.Filename) && (gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName != gv.mod.currentArea.Filename))
                                    {
                                        //apply only for props that ARE already in current area
                                        bool isOnCurrentAreaAlready = false;
                                        foreach (Prop p in gv.mod.currentArea.Props)
                                        {
                                            if (p.PropTag == relevantPropTag)
                                            {
                                                isOnCurrentAreaAlready = true;
                                            }
                                        }

                                        if (isOnCurrentAreaAlready)
                                        {
                                            //we assign the index of next in line waypoint
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = listEndCheckedIndexOfNextWaypoint;
                                            //set move to target coordinates
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].passOneMove = true;
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y.ToString());
                                        }
                                    }
                                }
                                //we handle props entering the current area while the party is in it
                                //we will look for props whose next in line waypoint is on current map: we then move the prop to next in line waypoint
                                //note: this will allow props entering the current map even if the departure time of first waypoint on current map is not reached yet
                                //note: this is not run for props already on the current map (see condition at the very start that exempts current area from the loops), so no worries about affecting those already existing props
                                else
                                {
                                    if (gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName == gv.mod.currentArea.Filename)
                                    {
                                        gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = listEndCheckedIndexOfNextWaypoint;
                                        //set move to target coordinates
                                        gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                        gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                        gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].passOneMove = true;
                                        int xLocForFloaty = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                        int yLocForFloaty = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                        
                                        gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y.ToString());

                                        //gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X
                                        //xxxxx
                                        //added floaty text that announces the area transfer
                                        //string shownAreaName = "";
                                        //for (int a = gv.mod.moduleAreasObjects.Count - 1; a >= 0; a--)
                                        //{
                                        //if (gv.mod.moduleAreasObjects[a].Filename == gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName)
                                        //{
                                        //shownAreaName = gv.mod.moduleAreasObjects[a].inGameAreaName;
                                        //}
                                        //}
                                        //IBMessageBox.Show(gv, "Prop just appeared");
                                        gv.screenMainMap.addFloatyText(xLocForFloaty, yLocForFloaty, "Just arrived here", "white", 4000);

                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region move ALL movers on current map (post, random, patrol, daily, weekly, monthly, yearly; also handle chasing)
            //begin moving the existing props in this map

            for (int i = gv.mod.currentArea.Props.Count - 1; i >= 0; i--)
            {
                //clear the lists with pixel destination coordinates of props
                gv.mod.currentArea.Props[i].destinationPixelPositionXList.Clear();
                gv.mod.currentArea.Props[i].destinationPixelPositionXList = new List<int>();
                gv.mod.currentArea.Props[i].destinationPixelPositionYList.Clear();
                gv.mod.currentArea.Props[i].destinationPixelPositionYList = new List<int>();
                gv.mod.currentArea.Props[i].pixelMoveSpeed = 1;

                //set the currentPixel position of the props
                int xOffSetInSquares = gv.mod.currentArea.Props[i].LocationX - gv.mod.PlayerLocationX;
                int yOffSetInSquares = gv.mod.currentArea.Props[i].LocationY - gv.mod.PlayerLocationY;
                int playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffsetX * gv.squareSize);
                int playerPositionYInPix = gv.playerOffsetY * gv.squareSize;

                gv.mod.currentArea.Props[i].currentPixelPositionX = playerPositionXInPix + (xOffSetInSquares * gv.squareSize);
                gv.mod.currentArea.Props[i].currentPixelPositionY = playerPositionYInPix + (yOffSetInSquares * gv.squareSize);

                //if (gv.mod.currentArea.Props[i].passOneMove == true)
                //{
                    //gv.mod.currentArea.Props[i].passOneMove = false;
                    //continue;
                //}
                //else
                if (1 ==1)
                {
                    /*
                    #region delay a mover for one turn on same square as party
                    //I suggest to modify this, so the prop will only wait for one turn and then move on, regardless of shared location with player
                    //otherwise the player can pin down a mover forever which feels weird imho
                    if ((gv.mod.currentArea.Props[i].LocationX == gv.mod.PlayerLocationX) && (gv.mod.currentArea.Props[i].LocationY == gv.mod.PlayerLocationY))
                    {
                        if (gv.sf.GetLocalInt(gv.mod.currentArea.Props[i].PropTag, "hasAlreadyWaited") == -1)
                        {
                            gv.sf.SetLocalInt(gv.mod.currentArea.Props[i].PropTag, "hasAlreadyWaited", "1");
                            //do nothing since prop and player are on the same square
                            continue;
                        }
                    }
                    else
                    {
                        gv.sf.SetLocalInt(gv.mod.currentArea.Props[i].PropTag, "hasAlreadyWaited", "-1");
                    }
                    #endregion
                    */

                    #region DISABLED: dont move props further away than ten squares
                    //Here I would suggest a full disable - the illsuion of a living wold would not work with a time freeze bubble outside 10 square radius
                    //if (getDistance(new Coordinate(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY), new Coordinate(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY)) > 10)
                    //{
                    //do nothing since prop and player are far away from each other
                    //continue;			
                    //}
                    #endregion

                    if ((gv.mod.currentArea.Props[i].isMover) && (gv.mod.currentArea.Props[i].isActive))
                    {
                        //determine move distance first
                        int moveDist = this.getMoveDistance(gv.mod.currentArea.Props[i]);
                        //gv.mod.currentArea.Props[i].pixelMoveSpeed = moveDist;


                        #region Chaser code
                        if ((gv.mod.currentArea.Props[i].isChaser) && (!gv.mod.currentArea.Props[i].ReturningToPost))
                        {
                            //determine if start chasing or stop chasing (set isCurrentlyChasing to true or false)
                            if (!gv.mod.currentArea.Props[i].isCurrentlyChasing)
                            {
                                //not chasing so see if in detect distance and set to true
                                if (getDistance(new Coordinate(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY), new Coordinate(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY)) <= gv.mod.currentArea.Props[i].ChaserDetectRangeRadius)
                                {
                                    gv.mod.currentArea.Props[i].isCurrentlyChasing = true;
                                    gv.mod.currentArea.Props[i].ChaserStartChasingTime = gv.mod.WorldTime;
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Chasing...", "red", 1500);
                                    if (gv.mod.debugMode)
                                    {
                                        //gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "following you", "red", 4000);
                                        gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " start chasing " + gv.mod.currentArea.Props[i].ChaserChaseDuration + " seconds</font><BR>");
                                    }
                                }
                            }
                            else //is chasing so see if out of follow range and set to false
                            {
                                if (getDistance(new Coordinate(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY), new Coordinate(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY)) >= gv.mod.currentArea.Props[i].ChaserGiveUpChasingRangeRadius)
                                {
                                    gv.mod.currentArea.Props[i].isCurrentlyChasing = false;
                                    gv.mod.currentArea.Props[i].ReturningToPost = true;
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Lost interest...", "green", 1500);
                                    if (gv.mod.debugMode)
                                    {
                                        //gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Nevermind...", "green", 1000);
                                        //gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "nevermind", "green", 4000);
                                        gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " stop chasing on range</font><BR>");
                                    }
                                }
                                else if (gv.mod.WorldTime - gv.mod.currentArea.Props[i].ChaserStartChasingTime >= gv.mod.currentArea.Props[i].ChaserChaseDuration)
                                {
                                    gv.mod.currentArea.Props[i].isCurrentlyChasing = false;
                                    gv.mod.currentArea.Props[i].ReturningToPost = true;
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Lost interest...", "green", 1500);
                                    if (gv.mod.debugMode)
                                    {
                                        //gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "nevermind", "green", 4000);
                                        gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " stop chasing on duration</font><BR>");
                                    }
                                }
                                else
                                {
                                    if (gv.mod.debugMode)
                                    {
                                        int timeRemain = gv.mod.currentArea.Props[i].ChaserChaseDuration - (gv.mod.WorldTime - gv.mod.currentArea.Props[i].ChaserStartChasingTime);
                                        gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " chasing " + timeRemain + " seconds left</font><BR>");
                                    }
                                }
                            }
                        }
                        //check to see if currently chasing, if so, chase
                        if ((gv.mod.currentArea.Props[i].isChaser) && (gv.mod.currentArea.Props[i].isCurrentlyChasing))
                        {
                            //move the distance
                            this.moveToTarget(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY, gv.mod.currentArea.Props[i], moveDist);
                            if (moveDist > 1)
                            {
                                gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Double move", "yellow", 1500);
                            }
                            doPropBarkString(gv.mod.currentArea.Props[i]);
                            if (gv.mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</font><BR>");
                                gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "(" + gv.mod.currentArea.Props[i].LocationX + "," + gv.mod.currentArea.Props[i].LocationY + ")", "yellow", 4000);
                            }
                        }
                        #endregion

                        #region Mover type: post
                        //not chasing so do mover type
                        else if (gv.mod.currentArea.Props[i].MoverType.Equals("post"))
                        {
                            //move towards post location if not already there
                            if ((gv.mod.currentArea.Props[i].LocationX == gv.mod.currentArea.Props[i].PostLocationX) && (gv.mod.currentArea.Props[i].LocationY == gv.mod.currentArea.Props[i].PostLocationY))
                            {
                                //already there so do not move
                                gv.mod.currentArea.Props[i].ReturningToPost = false;
                            }
                            else
                            {
                                this.moveToTarget(gv.mod.currentArea.Props[i].PostLocationX, gv.mod.currentArea.Props[i].PostLocationY, gv.mod.currentArea.Props[i], moveDist);
                                if (moveDist > 1)
                                {
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Double move", "yellow", 1500);
                                }
                                if (gv.mod.debugMode)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</font><BR>");
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "(" + gv.mod.currentArea.Props[i].LocationX + "," + gv.mod.currentArea.Props[i].LocationY + ")", "yellow", 4000);
                                }
                            }
                            doPropBarkString(gv.mod.currentArea.Props[i]);
                        }
                        #endregion

                        #region Mover type: random
                        else if (gv.mod.currentArea.Props[i].MoverType.Equals("random"))
                        {
                            gv.mod.currentArea.Props[i].randomMoverTimerForNextTarget += 1;
                            //check to see if at target square already and if so, change target
                            if (((gv.mod.currentArea.Props[i].LocationX == gv.mod.currentArea.Props[i].CurrentMoveToTarget.X) && (gv.mod.currentArea.Props[i].LocationY == gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y)) || (gv.mod.currentArea.Props[i].randomMoverTimerForNextTarget >= 20))
                            {
                                gv.mod.currentArea.Props[i].randomMoverTimerForNextTarget = 0;
                                Coordinate newCoor = this.getNewRandomTarget(gv.mod.currentArea.Props[i]);
                                //gv.screenMainMap.addFloatyText(prp.LocationX, prp.LocationY, "(" + newCoor.X + "," + newCoor.Y + ")", "blue", 4000);
                                gv.mod.currentArea.Props[i].CurrentMoveToTarget = new Coordinate(newCoor.X, newCoor.Y);
                                gv.mod.currentArea.Props[i].ReturningToPost = false;
                            }
                            //move to target
                            this.moveToTarget(gv.mod.currentArea.Props[i].CurrentMoveToTarget.X, gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y, gv.mod.currentArea.Props[i], moveDist);
                            if (moveDist > 1)
                            {
                                gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Double move", "yellow", 1500);
                            }
                            //gv.screenMainMap.addFloatyText(prp.LocationX, prp.LocationY, "(" + prp.CurrentMoveToTarget.X + "," + prp.CurrentMoveToTarget.Y + ")", "red", 4000);
                            if (gv.mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</font><BR>");
                                gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "(" + gv.mod.currentArea.Props[i].LocationX + "," + gv.mod.currentArea.Props[i].LocationY + ")", "yellow", 4000);
                            }
                            doPropBarkString(gv.mod.currentArea.Props[i]);
                        }
                        #endregion

                        #region Mover type: patrol
                        else if (gv.mod.currentArea.Props[i].MoverType.Equals("patrol"))
                        {
                            bool mustWait = false;
                            if (gv.mod.currentArea.Props[i].WayPointList.Count > 0)
                            {
                                //move towards next waypoint location if not already there
                                if ((gv.mod.currentArea.Props[i].LocationX == gv.mod.currentArea.Props[i].CurrentMoveToTarget.X) && (gv.mod.currentArea.Props[i].LocationY == gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y))
                                {
                                    if ((gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].WaitDuration > 0) && (gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].turnsAlreadyWaited <= gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].WaitDuration))
                                    {
                                        gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].turnsAlreadyWaited += 1;
                                        mustWait = true;
                                    }
                                    else
                                    {
                                        gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].turnsAlreadyWaited = 0;
                                        //already there so set next way point location (revert to index 0 if at last way point)
                                        if (gv.mod.currentArea.Props[i].WayPointListCurrentIndex >= gv.mod.currentArea.Props[i].WayPointList.Count - 1)
                                        {
                                            gv.mod.currentArea.Props[i].WayPointListCurrentIndex = 0;
                                        }
                                        else
                                        {
                                            gv.mod.currentArea.Props[i].WayPointListCurrentIndex++;
                                        }
                                        gv.mod.currentArea.Props[i].CurrentMoveToTarget.X = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X;
                                        gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y;
                                        gv.mod.currentArea.Props[i].ReturningToPost = false;
                                    }

                                }
                                //move to next target
                                if (!mustWait)
                                {
                                    this.moveToTarget(gv.mod.currentArea.Props[i].CurrentMoveToTarget.X, gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y, gv.mod.currentArea.Props[i], moveDist);
                                    if (moveDist > 1)
                                    {
                                        gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Double move", "yellow", 1500);
                                    }
                                }
                                if (gv.mod.debugMode)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</font><BR>");
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "(" + gv.mod.currentArea.Props[i].LocationX + "," + gv.mod.currentArea.Props[i].LocationY + ")", "yellow", 4000);
                                }
                            }
                            doPropBarkString(gv.mod.currentArea.Props[i]);
                        }
                        #endregion

                        #region time driven movers (daily, weekly, monthly, yearly)
                        else if (gv.mod.currentArea.Props[i].MoverType.Equals("daily") || gv.mod.currentArea.Props[i].MoverType.Equals("weekly") || gv.mod.currentArea.Props[i].MoverType.Equals("monthly") || gv.mod.currentArea.Props[i].MoverType.Equals("yearly"))
                        {
                            bool departureTimeReached = false;
                            List<string> timeUnitsList = new List<string>();
                            int currentTimeInInterval = 0;
                            bool registerRemoval = false;

                            timeUnitsList = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].departureTime.Split(':').Select(x => x.Trim()).ToList();

                            int dayCounter = Convert.ToInt32(timeUnitsList[0]);
                            int hourCounter = Convert.ToInt32(timeUnitsList[1]);
                            int minuteCounter = Convert.ToInt32(timeUnitsList[2]);

                            if ((dayCounter == 0) || (dayCounter == 1))
                            {
                                dayCounter = 0;
                            }
                            else
                            {
                                dayCounter = (dayCounter - 1);
                            }

                            int convertedDepartureTime = dayCounter * 86400 + hourCounter * 3600 + minuteCounter * 60;

                            if (gv.mod.currentArea.Props[i].WayPointListCurrentIndex == gv.mod.currentArea.Props[i].WayPointList.Count - 1)
                            {
                                if (gv.mod.currentArea.Props[i].MoverType.Equals("daily"))
                                {
                                    convertedDepartureTime = 86400 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                }
                                if (gv.mod.currentArea.Props[i].MoverType.Equals("weekly"))
                                {
                                    convertedDepartureTime = 604800 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                }
                                if (gv.mod.currentArea.Props[i].MoverType.Equals("monthly"))
                                {
                                    convertedDepartureTime = 2419200 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                }
                                if (gv.mod.currentArea.Props[i].MoverType.Equals("yearly"))
                                {
                                    convertedDepartureTime = 29030400 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                }
                            }

                            if (gv.mod.currentArea.Props[i].WayPointListCurrentIndex == 0)
                            {
                                if (convertedDepartureTime < (gv.mod.currentArea.TimePerSquare * 60 + 1))
                                {
                                    convertedDepartureTime = gv.mod.currentArea.TimePerSquare * 60 + 1;
                                }
                            }

                            if (gv.mod.currentArea.Props[i].MoverType.Equals("daily"))
                            {
                                currentTimeInInterval = (gv.mod.WorldTime * 60) % 86400;
                            }
                            if (gv.mod.currentArea.Props[i].MoverType.Equals("weekly"))
                            {
                                currentTimeInInterval = (gv.mod.WorldTime * 60) % 604800;
                            }
                            if (gv.mod.currentArea.Props[i].MoverType.Equals("monthly"))
                            {
                                currentTimeInInterval = (gv.mod.WorldTime * 60) % 2419200;
                            }
                            if (gv.mod.currentArea.Props[i].MoverType.Equals("yearly"))
                            {
                                currentTimeInInterval = (gv.mod.WorldTime * 60) % 29030400;
                            }

                            if (currentTimeInInterval >= convertedDepartureTime)
                            {
                                departureTimeReached = true;
                            }

                            if (gv.mod.currentArea.Props[i].WayPointList.Count > 0)
                            {

                                if (departureTimeReached)
                                {
                                    //already there so set next way point location (revert to index 0 if at last way point)
                                    if (gv.mod.currentArea.Props[i].WayPointListCurrentIndex >= gv.mod.currentArea.Props[i].WayPointList.Count - 1)
                                    {
                                        gv.mod.currentArea.Props[i].WayPointListCurrentIndex = 0;

                                        if (gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName != gv.mod.currentArea.Filename)
                                        {
                                            gv.mod.currentArea.Props[i].CurrentMoveToTarget.X = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X;
                                            gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y;
                                            gv.mod.currentArea.Props[i].ReturningToPost = false;
                                            //added floaty text that announces the area transfer
                                            string shownAreaName = "";
                                            for (int a = gv.mod.moduleAreasObjects.Count - 1; a >= 0; a--)
                                            {
                                                if (gv.mod.moduleAreasObjects[a].Filename == gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName)
                                                {
                                                    shownAreaName = gv.mod.moduleAreasObjects[a].inGameAreaName;
                                                }
                                            }

                                            gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "Heading off towards " + shownAreaName, "white", 4000);
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.currentArea.Props[i].PropTag, gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName, gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X.ToString(), gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y.ToString());
                                            registerRemoval = true;
                                        }
                                    }
                                    else
                                    {
                                        gv.mod.currentArea.Props[i].WayPointListCurrentIndex++;
                                        if (gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName != gv.mod.currentArea.Filename)
                                        {
                                            gv.mod.currentArea.Props[i].CurrentMoveToTarget.X = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X;
                                            gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y;
                                            gv.mod.currentArea.Props[i].ReturningToPost = false;
                                            //added floaty text that announces the area transfer
                                            string shownAreaName = "";
                                            for (int a = gv.mod.moduleAreasObjects.Count - 1; a >= 0; a--)
                                            {
                                                if (gv.mod.moduleAreasObjects[a].Filename == gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName)
                                                {
                                                    shownAreaName = gv.mod.moduleAreasObjects[a].inGameAreaName;
                                                }
                                            }

                                            gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "Heading off towards " + shownAreaName, "white", 4000);
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.currentArea.Props[i].PropTag, gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName, gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X.ToString(), gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y.ToString());
                                            registerRemoval = true;
                                        }
                                    }
                                    if (!registerRemoval)
                                    {
                                        gv.mod.currentArea.Props[i].CurrentMoveToTarget.X = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X;
                                        gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y;
                                        gv.mod.currentArea.Props[i].ReturningToPost = false;
                                    }

                                }

                                //move to next target
                                if (!registerRemoval)
                                {
                                    if ((gv.mod.currentArea.Props[i].LocationX == gv.mod.currentArea.Props[i].CurrentMoveToTarget.X) && (gv.mod.currentArea.Props[i].LocationY == gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y))
                                    {

                                    }
                                    else
                                    {
                                        this.moveToTarget(gv.mod.currentArea.Props[i].CurrentMoveToTarget.X, gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y, gv.mod.currentArea.Props[i], moveDist);
                                        if (moveDist > 1)
                                        {
                                            gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Double move", "yellow", 1500);
                                        }
                                    }
                                }

                                if ((gv.mod.debugMode) && (!registerRemoval))
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</font><BR>");
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "(" + gv.mod.currentArea.Props[i].LocationX + "," + gv.mod.currentArea.Props[i].LocationY + ")", "yellow", 4000);
                                }
                            }
                            if (!registerRemoval)
                            {
                                doPropBarkString(gv.mod.currentArea.Props[i]);
                            }
                        }
                        #endregion
                    }
                }
            }

            #endregion

        }
        public void doPropBarkString(Prop prp)
        {
            List<BarkString> chosenBarks = new List<BarkString>();
            Random rnd3 = new Random();
            int decider = 0;

            if (prp.WayPointList.Count > 0)
            {
                if ((prp.LocationX == prp.CurrentMoveToTarget.X) && (prp.LocationY == prp.CurrentMoveToTarget.Y))
                {
                    foreach (BarkString b in prp.WayPointList[prp.WayPointListCurrentIndex].BarkStringsAtWayPoint)
                    {
                        if (gv.sf.RandInt(100) < b.ChanceToShow)
                        {
                            chosenBarks.Add(b);
                        }
                    }
                    if (chosenBarks.Count > 0)
                    {
                        decider = rnd3.Next(0, chosenBarks.Count);
                        gv.screenMainMap.addFloatyText(prp.LocationY, prp.LocationY, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);
                    }
                }
                else
                {
                    //do barks for patrol, random, chasing or time driven
                    if (prp.WayPointListCurrentIndex == 0)
                    {
                        foreach (BarkString b in prp.WayPointList[prp.WayPointList.Count - 1].BarkStringsOnTheWayToNextWayPoint)
                        {
                            if (gv.sf.RandInt(100) < b.ChanceToShow)
                            {
                                chosenBarks.Add(b);
                            }
                        }
                        if (chosenBarks.Count > 0)
                        {
                            decider = rnd3.Next(0, chosenBarks.Count);
                            gv.screenMainMap.addFloatyText(prp.LocationY, prp.LocationY, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);
                        }                        
                    }
                    else
                    {
                        foreach (BarkString b in prp.WayPointList[prp.WayPointListCurrentIndex - 1].BarkStringsOnTheWayToNextWayPoint)
                        {
                            if (gv.sf.RandInt(100) < b.ChanceToShow)
                            {
                                chosenBarks.Add(b);
                                //gv.screenMainMap.addFloatyText(prp.LocationX, prp.LocationY, b.FloatyTextOneLiner, b.Color, b.LengthOfTimeToShowInMilliSeconds);
                                //break;
                            }
                        }
                        if (chosenBarks.Count > 0)
                        {
                            decider = rnd3.Next(0, chosenBarks.Count);
                            gv.screenMainMap.addFloatyText(prp.LocationY, prp.LocationY, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);
                        }
                    }
                }
            }
        }
        public int getMoveDistance(Prop prp)
        {
            if (gv.sf.RandInt(100) <= prp.ChanceToMove2Squares)
            {
                return 2;
            }
            else if (gv.sf.RandInt(100) <= prp.ChanceToMove0Squares)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public Coordinate getNewRandomTarget(Prop prp)
        {
            Coordinate newCoor = new Coordinate();

            //X range
            int minX = prp.PostLocationX - prp.RandomMoverRadius;
            if (minX < 0) { minX = 0; }
            int maxX = prp.PostLocationX + prp.RandomMoverRadius;
            if (maxX > gv.mod.currentArea.MapSizeX - 1) { maxX = gv.mod.currentArea.MapSizeX - 1; }

            //Y range
            int minY = prp.PostLocationY - prp.RandomMoverRadius;
            if (minY < 0) { minY = 0; }
            int maxY = prp.PostLocationY + prp.RandomMoverRadius;
            if (maxY > gv.mod.currentArea.MapSizeY - 1) { maxY = gv.mod.currentArea.MapSizeY - 1; }

            //get random location...check if location is valid first...do for loop and exit when found one, try 10 times
            for (int i = 0; i < 10; i++)
            {
                int x = gv.sf.RandInt(minX, maxX);
                int y = gv.sf.RandInt(minY, maxY);
                //if (gv.mod.currentArea.Tiles.get(y * gv.mod.currentArea.MapSizeX + x).Walkable)
                if (!gv.mod.currentArea.GetBlocked(x, y))
                {
                    newCoor.X = x;
                    newCoor.Y = y;
                    return newCoor;
                }
            }
            return new Coordinate(prp.LocationX, prp.LocationY);
        }
        public void moveToTarget(int targetX, int targetY, Prop prp, int moveDistance)
        {
            //store last location
            //prp.lastLocationX = prp.LocationX;
            //prp.lastLocationX = prp.LocationX;
                        
            Random rnd2 = new Random();
            for (int i = 0; i < moveDistance; i++)
            {
                gv.pfa.resetGrid();
                Coordinate newCoor = gv.pfa.findNewPoint(new Coordinate(prp.LocationX, prp.LocationY), new Coordinate(targetX, targetY), prp);
                if ((newCoor.X == -1) && (newCoor.Y == -1))
                {
                    //didn't find a path, don't move
                    //why do we eliminate the moveto target here? Was fitting for a static world (that would never open a new path anyway). In a dynamic world I think we better keep the moveto information?
                    //as props can move through each other now (or that's the plan) it might still work with theese lines
                    //then again, mayhaps dynamic collision controlled waits are still useful, like e.g. blocked city gates (collission static props) would be nice to queue NPCs in the morning
                    //prp.CurrentMoveToTarget.X = prp.LocationX;
                    //prp.CurrentMoveToTarget.Y = prp.LocationY;
                    return;
                }

                //new code for preventing movers from ending on the same square
                bool nextStepSquareIsOccupied = false;
                foreach (Prop otherProp in gv.mod.currentArea.Props)
                {
                    //check whether an active mover prop is on the field found as next step on the path
                    if ((otherProp.LocationX == newCoor.X) && (otherProp.LocationY == newCoor.Y) && (otherProp.isMover == true) && (otherProp.isActive == true))
                    {
                        nextStepSquareIsOccupied = true;
                        break;
                    }
                }

                if (nextStepSquareIsOccupied == true)
                {
                    bool originSquareOccupied = false;
                    //check wheter the next field found on path is the destination square, i.e. the waypoint our prop is headed to                                  
                    if ((newCoor.X == prp.CurrentMoveToTarget.X) && (newCoor.Y == prp.CurrentMoveToTarget.Y))
                    {
                        //let's find out whether our prop can stay on its origin square, i.e. skip move, or whether it already comes from an occupied square and has to "sidestep"
                        //Note: moving along path, double move, wont work when the target square is the destination square, i.e. the end of the path
                        foreach (Prop otherProp2 in gv.mod.currentArea.Props)
                        {
                            if ((otherProp2.LocationX == prp.LocationX) && (otherProp2.LocationY == prp.LocationY) && (otherProp2.isMover == true) && (otherProp2.isActive == true))
                            {
                                if (otherProp2.PropTag != prp.PropTag)
                                {
                                    originSquareOccupied = true;
                                    break;
                                }
                            }
                        }

                        int decider2 = rnd2.Next(0, 2);
                        //another step forward, ie (at least) 2 steps on path
                        //check whether to stay on origin square ("step back")
                        if ((originSquareOccupied == false) && (decider2 == 0))
                        {//4
                            return;
                        }//4

                        //sidestep to nearby free square because destination square and origin square are both occupied
                        else
                        {//4
                            //find alternative target spot, as near to (occupied) destination square as possible
                            int targetTile = newCoor.Y * gv.mod.currentArea.MapSizeX + newCoor.X;//the index of the original target spot in the current area's tiles list
                            List<int> freeTilesByIndex = new List<int>();// a new list used to store the indices of all free tiles on current area
                            int tileLocX = 0;//just temporary storage in for locations of tiles
                            int tileLocY = 0;//just temporary storage in for locations of tiles
                            double floatTileLocY = 0;//was uncertain about rounding and conversion details, therefore need this one (see below)
                            bool tileIsFree = true;//identify a tile suited as new alternative target loaction
                            int nearestTileByIndex = -1;//store the nearest tile by index; as the relevant loop runs this will be replaced several times likely with ever nearer tiles
                            int dist = 0;//distance between the orignally intended target location (i.e. destination squre) and a free tile
                            int deltaX = 0;//temporary value used for distance calculation 
                            int deltaY = 0;//temporary value used for distance calculation 

                            //FIRST PART: get all FREE tiles on the current area
                            for (int j = 0; j < gv.mod.currentArea.Layer1Filename.Count; j++)
                            {
                                //get the x and y location of current tile by calculation derived from index number, assuming that counting starts at top left corner of a map (0x, 0y)
                                //and that each horizintal x-line is counted first, then counting next horizonal x-line starting from the left again
                                tileIsFree = true;
                                //Note: When e.g. MapsizeY is 7, the y values range from 0 to 6
                                tileLocX = j % gv.mod.currentArea.MapSizeY;
                                //Note: ensure rounding down here 
                                floatTileLocY = j / gv.mod.currentArea.MapSizeX;
                                tileLocY = (int)Math.Floor(floatTileLocY);

                                //look at content of currently checked tile, with three checks for walkable, occupied by creature, occupied by pc
                                //walkbale check
                                if (gv.mod.currentArea.Walkable[j] == 0)
                                {
                                    tileIsFree = false;
                                }

                                //party occupied check
                                if (tileIsFree == true)
                                {
                                    if ((gv.mod.PlayerLocationX == tileLocX) && (gv.mod.PlayerLocationY == tileLocY))
                                    {
                                        tileIsFree = false;
                                    }
                                }

                                //creature occupied check
                                if (tileIsFree == true)
                                {
                                    foreach (Prop occupyingProp in gv.mod.currentArea.Props)
                                    {
                                        if ((occupyingProp.LocationX == tileLocX) && (occupyingProp.LocationY == tileLocY))
                                        {
                                            tileIsFree = false;
                                            break;
                                        }
                                    }
                                }

                                //this writes all free tiles into a fresh list; please note that the values of the elements of this new list are our relevant index values
                                //therefore it's not the index (which doesnt correalte to locations) in this list that's relevant, but the value of the element at that index
                                if (tileIsFree == true)
                                {
                                    freeTilesByIndex.Add(j);
                                }
                            }

                            //SECOND PART: find the free tile NEAREST to originally intended summon location
                            for (int k = 0; k < freeTilesByIndex.Count; k++)
                            {//5
                                dist = 0;

                                //get location x and y of the tile stored at the index number j, i.e. get the value of elment indexed with i and transform to x and y location
                                tileLocX = freeTilesByIndex[k] % gv.mod.currentArea.MapSizeY;
                                floatTileLocY = freeTilesByIndex[k] / gv.mod.currentArea.MapSizeX;
                                tileLocY = (int)Math.Floor(floatTileLocY);

                                //get distance between the current free tile and the originally intended target location (i.e. teh deistination square in this case, aka path end)
                                deltaX = (int)Math.Abs((tileLocX - prp.LocationX));
                                deltaY = (int)Math.Abs((tileLocY - prp.LocationY));
                                if (deltaX > deltaY)
                                {
                                    dist = deltaX;
                                }
                                else
                                {
                                    dist = deltaY;
                                }


                                //only very close to target tiles 
                                if (dist < 3)
                                {//6
                                    gv.pfa.resetGrid();
                                    Coordinate newCoor2 = gv.pfa.findNewPoint(new Coordinate(tileLocX, tileLocY), new Coordinate(targetX, targetY), prp, tileLocX, tileLocY, 6);
                                    if ((newCoor2.X != -1) && (newCoor2.Y != -1) && (prp.lengthOfLastPath < 4))
                                    {
                                        nearestTileByIndex = freeTilesByIndex[k];
                                        break;
                                    }
                                }
                            }

                            if (nearestTileByIndex != -1)
                            {
                                //get the nearest tile's x and y location and use it as target square coordinates
                                tileLocX = nearestTileByIndex % gv.mod.currentArea.MapSizeY;
                                floatTileLocY = nearestTileByIndex / gv.mod.currentArea.MapSizeX;
                                tileLocY = (int)Math.Floor(floatTileLocY);

                                prp.LocationX = tileLocX;
                                prp.LocationY = tileLocY;
                            }
                        }
                    }

                    //the (occupied) target square is not the destination, i.e. path end, square
                    else
                    {
                        originSquareOccupied = false;
                        //check whether origin square is occupied, too 
                        foreach (Prop otherProp2 in gv.mod.currentArea.Props)
                        {
                            if ((otherProp2.LocationX == prp.LocationX) && (otherProp2.LocationY == prp.LocationY) && (otherProp2.isMover == true) && (otherProp2.isActive == true))
                            {
                                if (otherProp2.PropTag != prp.PropTag)
                                {
                                    originSquareOccupied = true;
                                    break;
                                }
                            }
                        }
                        //origin square is occupied, waiting is no option therefore, so we must do (at least) double move forward (target square is occupied, too)
                        if (originSquareOccupied == true)
                        {
                            //careful, watch for infinite loop, recursive calling here
                            prp.LocationX = newCoor.X;
                            prp.LocationY = newCoor.Y;
                            moveToTarget(targetX, targetY, prp, 1);
                            return;
                        }
                        //origin square not occupied, so waiting would be an alternative to double move: 50/50 situation
                        else
                        {
                            //make random roll to randomly choose between the two next alternatives
                            int decider = rnd2.Next(0, 2);
                            //another step forward, ie (at least) 2 steps on path
                            if (decider == 0)
                            {
                                prp.LocationX = newCoor.X;
                                prp.LocationY = newCoor.Y;
                                //recursive call, careful
                                moveToTarget(targetX, targetY, prp, 1);
                                return;
                            }
                            //Skip whole move, ie 0 steps on path (rolled a 1 as random roll)
                            else
                            {
                                return;
                            }
                        }
                    }
                }
                //target square is (finally) not occupied
                else
                {
                    //WIP
                    if ((newCoor.X < prp.LocationX) && (!prp.PropFacingLeft)) //move left
                    {
                        //TODO                        prp.token = gv.cc.flip(prp.token);
                        prp.PropFacingLeft = true;
                    }
                    else if ((newCoor.X > prp.LocationX) && (prp.PropFacingLeft)) //move right
                    {
                        //TODO                        prp.token = gv.cc.flip(prp.token);
                        prp.PropFacingLeft = false;
                    }
                    prp.LocationX = newCoor.X;
                    prp.LocationY = newCoor.Y;

                }
            }            
        }

        public void applyEffects()
        {
            try
            {
                foreach (Player pc in gv.mod.playerList)
                {
                    foreach (Effect ef in pc.effectsList)
                    {
                        //decrement duration of all
                        ef.durationInUnits -= gv.mod.currentArea.TimePerSquare;
                        if (!ef.usedForUpdateStats) //not used for stat updates
                        {
                            doEffectScript(pc, ef);
                        }
                    }
                }
                //if remaining duration <= 0, remove from list
                foreach (Player pc in gv.mod.playerList)
                {
                    for (int i = pc.effectsList.Count; i > 0; i--)
                    {
                        if (pc.effectsList[i - 1].durationInUnits <= 0)
                        {
                            pc.effectsList.RemoveAt(i - 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }
        }
        public void doEffectScript(object src, Effect ef)
        {
            if (ef.effectScript.Equals("efGeneric"))
            {
                gv.sf.efGeneric(src, ef);
            }

            else if (ef.effectScript.Equals("efHeld"))
            {
                gv.sf.efHeld(src, ef);
            }
            else if (ef.effectScript.Equals("efSleep"))
            {
                gv.sf.efSleep(src, ef);
            }
            else if (ef.effectScript.Equals("efRegenMinor"))
            {
                gv.sf.efRegenMinor(src, ef);
            }
            else if (ef.effectScript.Equals("efPoisonedLight"))
            {
                gv.sf.efPoisoned(src, ef, 2);
            }
            else if (ef.effectScript.Equals("efPoisonedMedium"))
            {
                gv.sf.efPoisoned(src, ef, 4);
            }
        }
        public void doPropTriggers()
        {
            try
            {
                //search area for any props that share the party location
                bool foundOne = false;
                foreach (Prop prp in gv.mod.currentArea.Props)
                {
                    bool doNotTriggerProp = false;
                    if ((prp.isMover == false) || ((prp.MoverType == "Post") && (prp.isChaser == false)))
                    {
                        if (gv.realTimeTimerMilliSecondsEllapsed >= gv.mod.realTimeTimerLengthInMilliSeconds)
                        {
                            doNotTriggerProp = true;
                        }
                    }
                                        
                    if ((prp.LocationX == gv.mod.PlayerLocationX) && (prp.LocationY == gv.mod.PlayerLocationY) && (prp.isActive) && (doNotTriggerProp == false))
                    {
                        //prp.wasTriggeredLastUpdate = true;
                        foundOne = true;
                        blockSecondPropTriggersCall = true;
                        gv.triggerPropIndex++;
                        if ((gv.triggerPropIndex == 1) && (!prp.ConversationWhenOnPartySquare.Equals("none")))
                        {

                            if (prp.unavoidableConversation == true)
                            {
                                calledConvoFromProp = true;
                                gv.sf.ThisProp = prp;
                                doConversationBasedOnTag(prp.ConversationWhenOnPartySquare);
                                break;
                            }
                            else if (gv.mod.avoidInteraction == false)
                            {
                                calledConvoFromProp = true;
                                gv.sf.ThisProp = prp;                                
                                doConversationBasedOnTag(prp.ConversationWhenOnPartySquare);
                                break;
                            }
                            else
                            {
                                foundOne = false;
                                break;
                            }
                        }
                        else if ((gv.triggerPropIndex == 2) && (!prp.EncounterWhenOnPartySquare.Equals("none")))
                        {
                            calledEncounterFromProp = true;
                            gv.sf.ThisProp = prp;                            
                            doEncounterBasedOnTag(prp.EncounterWhenOnPartySquare);
                            break;
                        }
                        else if (gv.triggerPropIndex < 3)
                        {
                            gv.mod.isRecursiveCall = true;
                            doPropTriggers();
                            gv.mod.isRecursiveCall = false;
                            break;
                        }
                        if (gv.triggerPropIndex > 2)
                        {
                            gv.triggerPropIndex = 0;
                            //set flags back to false
                            calledConvoFromProp = false;
                            calledEncounterFromProp = false;
                            foundOne = false;
                            //delete prop if flag is set to do so and break foreach loop
                            if (prp.DeletePropWhenThisEncounterIsWon)
                            {
                                gv.mod.currentArea.Props.Remove(prp);
                            }
                            break;
                        }
                    }
                }
                if (!foundOne)
                {
                    doTrigger();
                }
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
        public void doTrigger()
        {
            if (gv.realTimeTimerMilliSecondsEllapsed < gv.mod.realTimeTimerLengthInMilliSeconds)
            {
                try
                {
                    Trigger trig = gv.mod.currentArea.getTriggerByLocation(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY);
                    if ((trig != null) && (trig.Enabled))
                    {
                        blockSecondPropTriggersCall = true;
                        //iterate through each event                  
                        //#region Event1 stuff
                        //check to see if enabled and parm not "none"
                        /*for (int i = 0; i < 15; i++)
                        {
                            gv.Render();
                        }*/
                        gv.triggerIndex++;

                        if ((gv.triggerIndex == 1) && (trig.EnabledEvent1) && (!trig.Event1FilenameOrTag.Equals("none")))
                        {
                            //check to see what type of event
                            if (trig.Event1Type.Equals("container"))
                            {
                                doContainerBasedOnTag(trig.Event1FilenameOrTag);
                                doTrigger();
                            }
                            else if (trig.Event1Type.Equals("transition"))
                            {
                                doTransitionBasedOnAreaLocation(trig.Event1FilenameOrTag, trig.Event1TransPointX, trig.Event1TransPointY);
                            }
                            else if (trig.Event1Type.Equals("conversation"))
                            {
                                if (trig.conversationCannotBeAvoided == true)
                                {
                                    doConversationBasedOnTag(trig.Event1FilenameOrTag);
                                }
                                else if (gv.mod.avoidInteraction == false)
                                {
                                    doConversationBasedOnTag(trig.Event1FilenameOrTag);
                                }
                            }
                            else if (trig.Event1Type.Equals("encounter"))
                            {
                                doEncounterBasedOnTag(trig.Event1FilenameOrTag);
                            }
                            else if (trig.Event1Type.Equals("script"))
                            {
                                doScriptBasedOnFilename(trig.Event1FilenameOrTag, trig.Event1Parm1, trig.Event1Parm2, trig.Event1Parm3, trig.Event1Parm4);
                                doTrigger();
                            }
                            else if (trig.Event1Type.Equals("ibscript"))
                            {
                                doIBScriptBasedOnFilename(trig.Event1FilenameOrTag, trig.Event1Parm1);
                                doTrigger();
                            }
                            //do that event
                            if (trig.DoOnceOnlyEvent1)
                            {
                                trig.EnabledEvent1 = false;
                            }
                        }
                        //#endregion
                        //#region Event2 stuff
                        //check to see if enabled and parm not "none"
                        else if ((gv.triggerIndex == 2) && (trig.EnabledEvent2) && (!trig.Event2FilenameOrTag.Equals("none")))
                        {
                            //check to see what type of event
                            if (trig.Event2Type.Equals("container"))
                            {
                                doContainerBasedOnTag(trig.Event2FilenameOrTag);
                                doTrigger();
                            }
                            else if (trig.Event2Type.Equals("transition"))
                            {
                                doTransitionBasedOnAreaLocation(trig.Event2FilenameOrTag, trig.Event2TransPointX, trig.Event2TransPointY);
                            }
                            else if (trig.Event2Type.Equals("conversation"))
                            {
                                if (trig.conversationCannotBeAvoided == true)
                                {
                                    doConversationBasedOnTag(trig.Event2FilenameOrTag);
                                }
                                else if (gv.mod.avoidInteraction == false)
                                {
                                    doConversationBasedOnTag(trig.Event2FilenameOrTag);
                                }
                            }
                            else if (trig.Event2Type.Equals("encounter"))
                            {
                                doEncounterBasedOnTag(trig.Event2FilenameOrTag);
                            }
                            else if (trig.Event2Type.Equals("script"))
                            {
                                doScriptBasedOnFilename(trig.Event2FilenameOrTag, trig.Event2Parm1, trig.Event2Parm2, trig.Event2Parm3, trig.Event2Parm4);
                                doTrigger();
                            }
                            else if (trig.Event1Type.Equals("ibscript"))
                            {
                                doIBScriptBasedOnFilename(trig.Event2FilenameOrTag, trig.Event2Parm1);
                                doTrigger();
                            }
                            //do that event
                            if (trig.DoOnceOnlyEvent2)
                            {
                                trig.EnabledEvent2 = false;
                            }
                        }
                        //#endregion
                        //#region Event3 stuff
                        //check to see if enabled and parm not "none"
                        else if ((gv.triggerIndex == 3) && (trig.EnabledEvent3) && (!trig.Event3FilenameOrTag.Equals("none")))
                        {
                            //check to see what type of event
                            if (trig.Event3Type.Equals("container"))
                            {
                                doContainerBasedOnTag(trig.Event3FilenameOrTag);
                                doTrigger();
                            }
                            else if (trig.Event3Type.Equals("transition"))
                            {
                                doTransitionBasedOnAreaLocation(trig.Event3FilenameOrTag, trig.Event3TransPointX, trig.Event3TransPointY);
                            }
                            else if (trig.Event3Type.Equals("conversation"))
                            {
                                if (trig.conversationCannotBeAvoided == true)
                                {
                                    doConversationBasedOnTag(trig.Event3FilenameOrTag);
                                }
                                else if (gv.mod.avoidInteraction == false)
                                {
                                    doConversationBasedOnTag(trig.Event3FilenameOrTag);
                                }
                            }
                            else if (trig.Event3Type.Equals("encounter"))
                            {
                                doEncounterBasedOnTag(trig.Event3FilenameOrTag);
                            }
                            else if (trig.Event3Type.Equals("script"))
                            {
                                doScriptBasedOnFilename(trig.Event3FilenameOrTag, trig.Event3Parm1, trig.Event3Parm2, trig.Event3Parm3, trig.Event3Parm4);
                                doTrigger();
                            }
                            else if (trig.Event1Type.Equals("ibscript"))
                            {
                                doIBScriptBasedOnFilename(trig.Event3FilenameOrTag, trig.Event3Parm1);
                                doTrigger();
                            }
                            //do that event
                            if (trig.DoOnceOnlyEvent3)
                            {
                                trig.EnabledEvent3 = false;
                            }
                        }
                        else if (gv.triggerIndex < 4)
                        {
                            doTrigger();
                        }
                        //#endregion
                        if (gv.triggerIndex > 3)
                        {
                            gv.triggerIndex = 0;
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
        }

        public void doContainerBasedOnTag(string tag)
        {

            try
            {
                Container container = gv.mod.getContainerByTag(tag);
                gv.screenType = "itemSelector";
                gv.screenItemSelector.resetItemSelector(container.containerItemRefs, "container", "main");
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }

        }
        public void doConversationBasedOnTag(string tag)
        {
           
            //if (gv.mod.doConvo)
            //{
                try
                {
                    LoadCurrentConvo(tag);
                    gv.screenType = "convo";
                    gv.screenConvo.startConvo();
                }
                catch (Exception ex)
                {
                    gv.sf.MessageBox("failed to open conversation with tag: " + tag);
                }
            //}
            //else
            //{
                //gv.mod.doConvo = true;
            //}
        }
        public void doSpellBasedOnScriptOrEffectTag(Spell spell, object source, object target)
        {
            gv.sf.AoeTargetsList.Clear();

            if (!spell.spellEffectTag.Equals("none"))
            {
                gv.sf.spGeneric(spell, source, target);
            }

            //WIZARD SPELLS
            else if (spell.spellScript.Equals("spFlameFingers"))
            {
                gv.sf.spFlameFingers(source, target, spell);
            }
            else if (spell.spellScript.Equals("spMageBolt"))
            {
                gv.sf.spMageBolt(source, target);
            }
            else if (spell.spellScript.Equals("spSleep"))
            {
                gv.sf.spSleep(source, target, spell);
            }
            else if (spell.spellScript.Equals("spMageArmor"))
            {
                gv.sf.spMageArmor(source, target);
            }
            else if (spell.spellScript.Equals("spMinorRegen"))
            {
                gv.sf.spMinorRegen(source, target);
            }
            else if (spell.spellScript.Equals("spWeb"))
            {
                gv.sf.spWeb(source, target, spell);
            }
            else if (spell.spellScript.Equals("spIceStorm"))
            {
                gv.sf.spIceStorm(source, target, spell);
            }
            else if (spell.spellScript.Equals("spFireball"))
            {
                gv.sf.spFireball(source, target, spell);
            }
            else if (spell.spellScript.Equals("spLightning"))
            {
                gv.sf.spLightning(source, target, spell);
            }
            
            //CLERIC SPELLS
            else if (spell.tag.Equals("minorHealing"))
            {
                gv.sf.spHeal(source, target, 8);
            }
            else if (spell.tag.Equals("moderateHealing"))
            {
                gv.sf.spHeal(source, target, 16);
            }
            else if (spell.tag.Equals("massMinorHealing"))
            {
                gv.sf.spMassHeal(source, target, 8);
            }
            else if (spell.spellScript.Equals("spBless"))
            {
                gv.sf.spBless(source, target);
            }
            else if (spell.spellScript.Equals("spMagicStone"))
            {
                gv.sf.spMagicStone(source, target);
            }
            else if (spell.spellScript.Equals("spBlastOfLight"))
            {
                gv.sf.spBlastOfLight(source, target, spell);
            }
            else if (spell.spellScript.Equals("spHold"))
            {
                gv.sf.spHold(source, target);
            }
        }
        public void doScriptBasedOnFilename(string filename, string prm1, string prm2, string prm3, string prm4)
        {
            if (!filename.Equals("none"))
            {
                //send to ga, gc, og, or os controllers
                if (filename.StartsWith("gc"))
                {
                    gv.sf.gcController(filename, prm1, prm2, prm3, prm4);
                }
                else if (filename.StartsWith("ga"))
                {
                    gv.sf.gaController(filename, prm1, prm2, prm3, prm4);
                }
                else if (filename.StartsWith("og"))
                {
                    gv.sf.ogController(filename, prm1, prm2, prm3, prm4);
                }
                else if (filename.StartsWith("os"))
                {
                    gv.sf.osController(filename, prm1, prm2, prm3, prm4);
                }
            }
        }
        public void doIBScriptBasedOnFilename(string filename, string parms)
        {
            try
            {
                if (!filename.Equals("none"))
                {
                    IBScriptEngine e = new IBScriptEngine(gv, filename, parms);
                    e.RunScript();
                }
            }
            catch (Exception ex)
            {
                gv.sf.MessageBox("failed to run IBScript: " + filename);
            }
        }
        public void doEncounterBasedOnTag(string name)
        {
            try
            {
                gv.mod.currentEncounter = gv.mod.getEncounter(name);
                if (gv.mod.currentEncounter.encounterCreatureRefsList.Count > 0)
                {
                    gv.screenCombat.doCombatSetup();
                    int foundOnePc = 0;
                    foreach (Player chr in gv.mod.playerList)
                    {
                        if (chr.hp > 0)
                        {
                            foundOnePc = 1;
                        }
                    }
                    if (foundOnePc == 0)
                    {
                        //IBMessageBox.Show(game, "Party is wiped out...game over");
                    }
                }
                else
                {
                    //IBMessageBox.Show(game, "no creatures left here"); 
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }
        }
                
        public void doTransitionBasedOnAreaLocation(string areaFilename, int x, int y)
        {
            try
            {
                gv.mod.PlayerLocationX = x;
                gv.mod.PlayerLocationY = y;                    
                gv.mod.setCurrentArea(areaFilename, gv);                    
                gv.screenMainMap.resetMiniMapBitmap();
                doOnEnterAreaUpdate = true;
                doPropMoves();
                doOnEnterAreaUpdate = false;
                gv.triggerIndex = 0;
                doTrigger();                
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }            
        }
                
        public void doItemScriptBasedOnUseItem(Player pc, ItemRefs itRef, bool destroyItemAfterUse)
        {
            Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
            bool foundScript = false;
            if (it.onUseItem.Equals("itHealLight.cs"))
            {
                gv.sf.itHeal(pc, it, 8);
                foundScript = true;
            }
            else if (it.onUseItem.Equals("itHealMedium.cs"))
            {
                gv.sf.itHeal(pc, it, 16);
                foundScript = true;
            }
            else if (it.onUseItem.Equals("itRegenSPLight.cs"))
            {
                gv.sf.itSpHeal(pc, it, 20);
                foundScript = true;
            }
            else if (it.onUseItem.Equals("itForceRest.cs"))
            {
                gv.sf.itForceRest();
                foundScript = true;
            }
            if ((foundScript) && (destroyItemAfterUse))
            {
                gv.sf.RemoveItemFromInventory(itRef, 1);
            }
        }

        //TESTING STUFF
        public void createSpritesForTesting()
        {
            for (int i = 0; i < 100; i++)
            {
                Sprite spr = new Sprite(gv, "hit_symbol", gv.sf.RandInt(1000), gv.sf.RandInt(1000), (float)(gv.sf.RandInt(100) + 1) / 1000f, (float)(gv.sf.RandInt(100) + 1) / 1000f, 0, (float)(gv.sf.RandInt(100) + 1) / 10000f, 1.0f, gv.sf.RandInt(10000) + 3000, false, 100);
                gv.screenMainMap.spriteList.Add(spr);
            }            
        }
        
        //MISC FUNCTIONS
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
        public System.Drawing.Bitmap LoadBitmapGDI(string filename) //change this to LoadBitmapGDI
        {
            System.Drawing.Bitmap bm = null;
            bm = LoadBitmapGDI(filename, gv.mod); //change this to LoadBitmapGDI
            return bm;
        }
        public System.Drawing.Bitmap LoadBitmapGDI(string filename, Module mdl) //change this to LoadBitmapGDI
        {
            System.Drawing.Bitmap bm = null;

            try
            {
                if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".jpg"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".jpg");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename);
                }                
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename);
                }
                //NewModule folders
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".jpg"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".jpg");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\pctokens\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\pctokens\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\pctokens\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\pctokens\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename);
                }

                else
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\missingtexture.png");
                }		
            }
            catch (Exception ex)
            {
                if (bm == null)
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\missingtexture.png");
                    return bm;
                }
                gv.errorLog(ex.ToString());
            }

            return bm;
        }

        public string loadTextToString(string filename)
        {
            string txt = null;
            try
            {
                if (File.Exists(GetModulePath() + "\\data\\" + filename))
                {
                    txt = File.ReadAllText(GetModulePath() + "\\data\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\data\\" + filename))
                {
                    txt = File.ReadAllText(gv.mainDirectory + "\\default\\NewModule\\data\\" + filename);
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
                return null;
            }
            return txt;
        }
        public float MeasureString(string text, SharpDX.DirectWrite.FontWeight fw, SharpDX.DirectWrite.FontStyle fs, float fontHeight)
        {
            // Measure string width.
            SharpDX.DirectWrite.TextFormat tf = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, fw, fs, SharpDX.DirectWrite.FontStretch.Normal, fontHeight) { TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Near };
            SharpDX.DirectWrite.TextLayout tl = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, text, tf, gv.Width, gv.Height);
            float returnWidth = tl.Metrics.Width;
            if (tf != null)
            {
                tf.Dispose();
                tf = null;
            }
            if (tl != null)
            {
                tl.Dispose();
                tl = null;
            }
            return returnWidth;
        }
        public CoordinateF MeasureStringSize(string text, SharpDX.DirectWrite.FontWeight fw, SharpDX.DirectWrite.FontStyle fs, float fontHeight)
        {
            // Measure string width.
            SharpDX.DirectWrite.TextFormat tf = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, fw, fs, SharpDX.DirectWrite.FontStretch.Normal, fontHeight) { TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Near };
            SharpDX.DirectWrite.TextLayout tl = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, text, tf, gv.Width, gv.Height);
            CoordinateF returnSize = new CoordinateF(tl.Metrics.Width, tl.Metrics.Height);
            if (tf != null)
            {
                tf.Dispose();
                tf = null;
            }
            if (tl != null)
            {
                tl.Dispose();
                tl = null;
            }
            return returnSize;
        }
        public void MakeDirectoryIfDoesntExist(string filenameAndFullPath)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filenameAndFullPath);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
        }
        /*public System.Drawing.Bitmap flip(System.Drawing.Bitmap src)
        {
            src.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return src;
        }*/
        /*public System.Drawing.Bitmap FlipHorz(System.Drawing.Bitmap src)
        {
            src.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return src;
        }*/

        //DIRECT2D STUFF
        public SharpDX.Direct2D1.Bitmap GetFromBitmapList(string fileNameWithOutExt)
        {
            //check to see if in list already and return bitmap it if found
            if (commonBitmapList.ContainsKey(fileNameWithOutExt))
            {
                return commonBitmapList[fileNameWithOutExt];
            }
            //try loading and adding to list and return bitmap
            else
            {
                commonBitmapList.Add(fileNameWithOutExt, LoadBitmap(fileNameWithOutExt));
                return commonBitmapList[fileNameWithOutExt];
            }
        }
        public SharpDX.Direct2D1.Bitmap GetFromTileBitmapList(string fileNameWithOutExt)
        {
            //check to see if in list already and return bitmap it if found
            if (tileBitmapList.ContainsKey(fileNameWithOutExt))
            {
                return tileBitmapList[fileNameWithOutExt];
            }
            //try loading and adding to list and return bitmap
            else
            {
                tileBitmapList.Add(fileNameWithOutExt, LoadBitmap(fileNameWithOutExt));
                return tileBitmapList[fileNameWithOutExt];
            }
        }
        public System.Drawing.Bitmap GetFromTileGDIBitmapList(string fileNameWithOutExt)
        {
            //check to see if in list already and return bitmap it if found
            if (tileGDIBitmapList.ContainsKey(fileNameWithOutExt))
            {
                return tileGDIBitmapList[fileNameWithOutExt];
            }
            //try loading and adding to list and return bitmap
            else
            {
                tileGDIBitmapList.Add(fileNameWithOutExt, LoadBitmapGDI(fileNameWithOutExt));
                return tileGDIBitmapList[fileNameWithOutExt];
            }
        }
        public void DisposeOfBitmap(ref SharpDX.Direct2D1.Bitmap bmp)
        {
            if (bmp != null)
            {
                bmp.Dispose();
                bmp = null;
            }
        }
        public SharpDX.Direct2D1.Bitmap LoadBitmap(string file, Module mdl) //change this to LoadBitmap
        {
            // Loads from file using System.Drawing.Image
            using (var bitmap = LoadBitmapGDI(file, mdl)) //change this to LoadBitmapGDI
            {
                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                var size = new Size2(bitmap.Width, bitmap.Height);

                // Transform pixels from BGRA to RGBA
                int stride = bitmap.Width * sizeof(int);
                using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // Not optimized 
                            byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }
                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;
                    return new SharpDX.Direct2D1.Bitmap(gv.renderTarget2D, size, tempStream, stride, bitmapProperties);
                }
            }
        }
        public SharpDX.Direct2D1.Bitmap LoadBitmap(string file) //change this to LoadBitmap
        {
            // Loads from file using System.Drawing.Image
            using (var bitmap = LoadBitmapGDI(file)) //change this to LoadBitmapGDI
            {
                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                var size = new Size2(bitmap.Width, bitmap.Height);

                // Transform pixels from BGRA to RGBA
                int stride = bitmap.Width * sizeof(int);
                using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // Not optimized 
                            byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }
                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;
                    return new SharpDX.Direct2D1.Bitmap(gv.renderTarget2D, size, tempStream, stride, bitmapProperties);
                }
            }
        }
        public SharpDX.Direct2D1.Bitmap ConvertGDIBitmapToD2D(System.Drawing.Bitmap gdibitmap)
        {
            var sourceArea = new System.Drawing.Rectangle(0, 0, gdibitmap.Width, gdibitmap.Height);
            var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
            var size = new Size2(gdibitmap.Width, gdibitmap.Height);

            // Transform pixels from BGRA to RGBA
            int stride = gdibitmap.Width * sizeof(int);
            using (var tempStream = new DataStream(gdibitmap.Height * stride, true, true))
            {
                // Lock System.Drawing.Bitmap
                var bitmapData = gdibitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                // Convert all pixels 
                for (int y = 0; y < gdibitmap.Height; y++)
                {
                    int offset = bitmapData.Stride * y;
                    for (int x = 0; x < gdibitmap.Width; x++)
                    {
                        // Not optimized 
                        byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        int rgba = R | (G << 8) | (B << 16) | (A << 24);
                        tempStream.Write(rgba);
                    }
                }
                gdibitmap.UnlockBits(bitmapData);
                tempStream.Position = 0;
                return new SharpDX.Direct2D1.Bitmap(gv.renderTarget2D, size, tempStream, stride, bitmapProperties);
            }
        }
        public List<FormattedLine> ProcessHtmlString(string text, int width, List<string> tagStack)
        {
            bool tagMode = false;
            string tag = "";
            FormattedWord newWord = new FormattedWord();
            FormattedLine newLine = new FormattedLine();
            List<FormattedLine> logLinesList = new List<FormattedLine>();
            int lineHeight = 0;
            float xLoc = 0;

            char previousChar = ' ';
            char nextChar = ' ';
            int charIndex = -1;
            foreach (char c in text)
            {
                charIndex++;

                //get the previous char and the next char, used to get ' < ' and ' >= '
                if (charIndex > 0)
                {
                    previousChar = text[charIndex - 1];
                }
                if (charIndex < text.Length - 1)
                {
                    nextChar = text[charIndex + 1];
                }
                string combinedChars = previousChar.ToString() + c.ToString() + nextChar.ToString();

                #region Start/Stop Tags
                //start a tag and check for end of word
                if ((c == '<') && (!combinedChars.Contains("<=")) && (!combinedChars.Equals(" < ")))
                {
                    tagMode = true;

                    if (newWord.text != "")
                    {
                        newWord.fontStyle = GetFontStyle(tagStack);
                        newWord.fontWeight = GetFontWeight(tagStack);
                        newWord.underlined = GetIsUnderlined(tagStack);
                        newWord.fontSize = GetFontSizeInPixels(tagStack);
                        newWord.color = GetColor(tagStack);
                        if (gv.textFormat != null)
                        {
                            gv.textFormat.Dispose();
                            gv.textFormat = null;
                        }

                        if (gv.textLayout != null)
                        {
                            gv.textLayout.Dispose();
                            gv.textLayout = null;
                        }
                        gv.textFormat = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, newWord.fontWeight, newWord.fontStyle, SharpDX.DirectWrite.FontStretch.Normal, newWord.fontSize) { TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Near };
                        gv.textLayout = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, newWord.text + " ", gv.textFormat, gv.Width, gv.Height);
                        //font = new Font(gv.family, newWord.fontSize, newWord.fontStyle);
                        float height = gv.textLayout.Metrics.Height;
                        float wordWidth = gv.textLayout.Metrics.WidthIncludingTrailingWhitespace * 1.2f;
                        if (height > lineHeight) { lineHeight = (int)height; }
                        //int wordWidth = (int)(frm.gCanvas.MeasureString(newWord.word, font)).Width;
                        float modifiedWidth = 0;
                        modifiedWidth = width;                        
                        if (xLoc + wordWidth > modifiedWidth) //word wrap
                        {
                            //end last line and add it to the log
                            newLine.lineHeight = lineHeight;
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new FormattedLine();
                            newLine.wordsList.Add(newWord);
                            xLoc = 0;
                        }
                        else //no word wrap, just add word
                        {
                            newLine.wordsList.Add(newWord);
                        }
                        //instead of drawing, just add to line list 
                        //DrawString(g, word, font, brush, xLoc, yLoc);
                        xLoc += wordWidth;
                        newWord = new FormattedWord();
                    }
                    continue;
                }
                //end a tag
                else if ((c == '>') && (!combinedChars.Equals(" > ")) && (!combinedChars.Contains(">=")))
                {
                    //check for ending type tag
                    if (tag.StartsWith("/"))
                    {
                        //if </>, remove corresponding tag from stack
                        string tagMinusSlash = tag.Substring(1);
                        if (tag.StartsWith("/font"))
                        {
                            for (int i = tagStack.Count - 1; i > 0; i--)
                            {
                                if (tagStack[i].StartsWith("font"))
                                {
                                    tagStack.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            tagStack.Remove(tagMinusSlash);
                        }
                    }
                    else
                    {
                        //check for line break
                        if ((tag.ToLower() == "br") || (tag == "BR"))
                        {
                            newWord.fontStyle = GetFontStyle(tagStack);
                            newWord.fontWeight = GetFontWeight(tagStack);
                            newWord.underlined = GetIsUnderlined(tagStack);
                            newWord.fontSize = GetFontSizeInPixels(tagStack);
                            newWord.color = GetColor(tagStack);
                            //end last line and add it to the log
                            newLine.lineHeight = lineHeight;
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new FormattedLine();
                            //newLine.wordsList.Add(newWord);
                            xLoc = 0;
                        }
                        //else if <>, add this tag to the stack
                        tagStack.Add(tag);
                    }
                    tagMode = false;
                    tag = "";
                    continue;
                }
                #endregion

                #region Words
                if (!tagMode)
                {
                    if (c != ' ') //keep adding to word until hit a space
                    {
                        newWord.text += c;
                    }
                    else //hit a space so end word
                    {
                        newWord.fontStyle = GetFontStyle(tagStack);
                        newWord.fontWeight = GetFontWeight(tagStack);
                        newWord.underlined = GetIsUnderlined(tagStack);
                        newWord.fontSize = GetFontSizeInPixels(tagStack);
                        newWord.color = GetColor(tagStack);
                        if (gv.textFormat != null)
                        {
                            gv.textFormat.Dispose();
                            gv.textFormat = null;
                        }

                        if (gv.textLayout != null)
                        {
                            gv.textLayout.Dispose();
                            gv.textLayout = null;
                        }
                        gv.textFormat = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, newWord.fontWeight, newWord.fontStyle, SharpDX.DirectWrite.FontStretch.Normal, newWord.fontSize) { TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Near };
                        gv.textLayout = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, newWord.text + " ", gv.textFormat, gv.Width, gv.Height);
                        //font = new Font(gv.family, newWord.fontSize, newWord.fontStyle);
                        //float adjustedWordDimensionsForScreenDensity = (100f / gv.squareSize);
                        float adjustedWordWidthForScreenDensity = (1920f / gv.Width);
                        float adjustedWordHeightForScreenDensity = (1080f / gv.Height);

                        float wordWidth = (gv.textLayout.Metrics.WidthIncludingTrailingWhitespace) * adjustedWordWidthForScreenDensity;
                        float height = gv.textLayout.Metrics.Height * adjustedWordHeightForScreenDensity;
                        if (height > lineHeight) { lineHeight = (int)height; }

                        if (xLoc + wordWidth > width) //word wrap
                        {
                            //end last line and add it to the log
                            newLine.lineHeight = lineHeight;
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new FormattedLine();
                            newLine.wordsList.Add(newWord);
                            xLoc = 0;
                        }
                        else //no word wrap, just add word
                        {
                            newLine.wordsList.Add(newWord);
                        }
                        //instead of drawing, just add to line list 
                        //DrawString(g, word, font, brush, xLoc, yLoc);
                        xLoc += wordWidth;
                        newWord = new FormattedWord();
                    }
                }
                else if (tagMode)
                {
                    tag += c;
                }
                #endregion
            }
            tagStack.Clear();
            return logLinesList;
        }
        private SharpDX.Color GetColor(List<string> tagStack)
        {
            //will end up using the last color on the stack
            SharpDX.Color clr = SharpDX.Color.White;
            foreach (string s in tagStack)
            {
                if ((s == "font color='red'") || (s == "font color = 'red'"))
                {
                    clr = SharpDX.Color.Red;
                }
                else if ((s == "font color='lime'") || (s == "font color = 'lime'"))
                {
                    clr = SharpDX.Color.Lime;
                }
                else if ((s == "font color='black'") || (s == "font color = 'black'"))
                {
                    clr = SharpDX.Color.Black;
                }
                else if ((s == "font color='white'") || (s == "font color = 'white'"))
                {
                    clr = SharpDX.Color.White;
                }
                else if ((s == "font color='silver'") || (s == "font color = 'silver'"))
                {
                    clr = SharpDX.Color.Gray;
                }
                else if ((s == "font color='grey'") || (s == "font color = 'grey'"))
                {
                    clr = SharpDX.Color.DimGray;
                }
                else if ((s == "font color='aqua'") || (s == "font color = 'aqua'"))
                {
                    clr = SharpDX.Color.Aqua;
                }
                else if ((s == "font color='fuchsia'") || (s == "font color = 'fuchsia'"))
                {
                    clr = SharpDX.Color.Fuchsia;
                }
                else if ((s == "font color='yellow'") || (s == "font color = 'yellow'"))
                {
                    clr = SharpDX.Color.Yellow;
                }
            }
            return clr;
        }
        private SharpDX.DirectWrite.FontStyle GetFontStyle(List<string> tagStack)
        {
            SharpDX.DirectWrite.FontStyle style = SharpDX.DirectWrite.FontStyle.Normal;
            foreach (string s in tagStack)
            {
                if (s == "i")
                {
                    style = style | SharpDX.DirectWrite.FontStyle.Italic;
                }
            }
            return style;
        }
        private SharpDX.DirectWrite.FontWeight GetFontWeight(List<string> tagStack)
        {
            SharpDX.DirectWrite.FontWeight style = SharpDX.DirectWrite.FontWeight.Normal;
            foreach (string s in tagStack)
            {
                if (s == "b")
                {
                    style = style | SharpDX.DirectWrite.FontWeight.Bold;
                }
            }
            return style;
        }
        private bool GetIsUnderlined(List<string> tagStack)
        {
            bool isUnderlined = false;
            foreach (string s in tagStack)
            {
                if (s == "u")
                {
                    isUnderlined = true; ;
                }
            }
            return isUnderlined;
        }
        private float GetFontSizeInPixels(List<string> tagStack)
        {
            float fSize = gv.drawFontRegHeight * (float)gv.squareSize / 100.0f;
            foreach (string s in tagStack)
            {
                if (s == "big")
                {
                    fSize = gv.drawFontLargeHeight * (float)gv.squareSize / 100.0f;
                }
                else if (s == "small")
                {
                    fSize = gv.drawFontSmallHeight * (float)gv.squareSize / 100.0f;
                }
            }
            return fSize;
        }
    }
}