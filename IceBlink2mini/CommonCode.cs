using Newtonsoft.Json;
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

        public bool blockSecondPropTriggersCall = false;
        public List<FloatyText> floatyTextList = new List<FloatyText>();
        public int floatyTextCounter = 0;
        public bool floatyTextOn = false;
        public IbbButton btnHelp = null;
        public int partyScreenPcIndex = 0;
        public int partyItemSlotIndex = 0;
        public string slotA = "Autosave";
        public string slot0 = "Quicksave";
        public string slot1 = "";
        public string slot2 = "";
        public string slot3 = "";
        public string slot4 = "";
        public string slot5 = "";

        public Bitmap walkPass;
        public Bitmap walkBlocked;
        public Bitmap losBlocked;
        public Bitmap hitSymbol;
        public Bitmap missSymbol;
        public Bitmap highlight_green;
        public Bitmap highlight_red;
        public Bitmap black_tile;
        public Bitmap turn_marker;
        public Bitmap pc_dead;
        public Bitmap pc_stealth;
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
        
        public CommonCode(GameView g)
        {
            gv = g;
        }

        //LOAD FILES
        public void LoadTestParty()
        {
            gv.sf.AddCharacterToParty(gv.mod.defaultPlayerFilename); //drin.json is default
            gv.mod.partyTokenFilename = "prp_party";
        }
        public Player LoadPlayer(string filename)
        {
            Player toReturn = null;

            //try finding player in module companionPlayerList first
            string nameMinusJson = filename.Replace(".json", "");
            foreach (Player p in gv.mod.companionPlayerList)
            {
                if (p.name.Equals(nameMinusJson))
                {
                    return p;
                }
            }
            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(gv.mainDirectory + "\\default\\NewModule\\data\\" + filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                toReturn = (Player)serializer.Deserialize(file, typeof(Player));
            }
            return toReturn;
        }
        public void QuickSave()
        {
            try
            {
                //QuickSave();
                SaveSaveGame("quicksave.json");
            }
            catch (Exception ex)
            {
                gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                gv.errorLog(ex.ToString());
            }
        }
        public void doSavesDialog(int selectedIndex)
        {
            if (selectedIndex == 0)
            {
                try
                {
                    SaveSaveGame("quicksave.json");
                }
                catch (Exception ex)
                {
                    gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                    gv.errorLog(ex.ToString());
                }
            }
            else if (selectedIndex == 1)
            {
                Player pc = gv.mod.playerList[0];
                gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                slot1 = gv.mod.saveName;
                try
                {
                    SaveSaveGame("slot1.json");
                }
                catch (Exception ex)
                {
                    gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                    gv.errorLog(ex.ToString());
                }
            }
            else if (selectedIndex == 2)
            {
                Player pc = gv.mod.playerList[0];
                gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                slot2 = gv.mod.saveName;
                try
                {
                    SaveSaveGame("slot2.json");
                }
                catch (Exception ex)
                {
                    gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                    gv.errorLog(ex.ToString());
                }
            }
            else if (selectedIndex == 3)
            {
                Player pc = gv.mod.playerList[0];
                gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                slot3 = gv.mod.saveName;
                try
                {
                    SaveSaveGame("slot3.json");
                }
                catch (Exception ex)
                {
                    gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                    gv.errorLog(ex.ToString());
                }
            }
            else if (selectedIndex == 4)
            {
                Player pc = gv.mod.playerList[0];
                gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                slot4 = gv.mod.saveName;
                try
                {
                    SaveSaveGame("slot4.json");
                }
                catch (Exception ex)
                {
                    gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                    gv.errorLog(ex.ToString());
                }
            }
            else if (selectedIndex == 5)
            {
                Player pc = gv.mod.playerList[0];
                gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                slot5 = gv.mod.saveName;
                try
                {
                    SaveSaveGame("slot5.json");
                }
                catch (Exception ex)
                {
                    gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                    gv.errorLog(ex.ToString());
                }
            }
            
        }
        public void doSavesSetupDialog()
        {
            List<string> saveList = new List<string> { slot0, slot1, slot2, slot3, slot4, slot5 };
            gv.itemListSelector.setupIBminiItemListSelector(gv, saveList, "Choose a slot to save game.", "savegame");
            gv.itemListSelector.showIBminiItemListSelector = true;
        }
        public void doLoadSaveGameDialog(int selectedIndex)
        {
            if (selectedIndex == 0)
            {
                bool result = LoadSaveGame("autosave.json");
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
            else if (selectedIndex == 1)
            {
                bool result = LoadSaveGame("quicksave.json");
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
            else if (selectedIndex == 2)
            {
                bool result = LoadSaveGame("slot1.json");
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
            else if (selectedIndex == 3)
            {
                bool result = LoadSaveGame("slot2.json");
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
            else if (selectedIndex == 4)
            {
                bool result = LoadSaveGame("slot3.json");
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
            else if (selectedIndex == 5)
            {
                bool result = LoadSaveGame("slot4.json");
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
            else if (selectedIndex == 6)
            {
                bool result = LoadSaveGame("slot5.json");
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
        public void doLoadSaveGameSetupDialog()
        {
            List<string> saveList = new List<string> { slotA, slot0, slot1, slot2, slot3, slot4, slot5 };
            gv.itemListSelector.setupIBminiItemListSelector(gv, saveList, "Choose a Saved Game to Load.", "loadsavegame");
            gv.itemListSelector.showIBminiItemListSelector = true;
        }
        public SaveGame LoadModuleInfo(string filename)
        {
            SaveGame m = new SaveGame();
            try
            {
                using (StreamReader file = File.OpenText(gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    m = (SaveGame)serializer.Deserialize(file, typeof(SaveGame));
                }
            }
            catch { }
            return m;
        }
        public void LoadSaveListItems()
        {
            slot1 = LoadModuleInfo("slot1.json").saveName;
            slot2 = LoadModuleInfo("slot2.json").saveName;
            slot3 = LoadModuleInfo("slot3.json").saveName;
            slot4 = LoadModuleInfo("slot4.json").saveName;
            slot5 = LoadModuleInfo("slot5.json").saveName;
        }

        //SAVE SAVEGAME
        public void SaveSaveGame(string filename)
        {
            SaveGame saveMod = new SaveGame();

            saveMod.saveName = gv.mod.saveName;
            saveMod.playerList = new List<Player>();
            foreach (Player pc in gv.mod.playerList)
            {
                saveMod.playerList.Add(pc.DeepCopy());
            }
            saveMod.partyRosterList = new List<Player>();
            foreach (Player pc in gv.mod.partyRosterList)
            {
                saveMod.partyRosterList.Add(pc.DeepCopy());
            }
            saveMod.partyJournalQuests.Clear();
            foreach (JournalQuest jq in gv.mod.partyJournalQuests)
            {
                JournalQuest savJQ = jq.DeepCopy();
                saveMod.partyJournalQuests.Add(savJQ);                
            }
            saveMod.partyInventoryRefsList.Clear();
            foreach (ItemRefs s in gv.mod.partyInventoryRefsList)
            {
                saveMod.partyInventoryRefsList.Add(s.DeepCopy());
            }
            saveMod.moduleShopsList.Clear();
            foreach (Shop shp in gv.mod.moduleShopsList)
            {
                saveMod.moduleShopsList.Add(shp.DeepCopy());
            }
            saveMod.moduleAreasObjects.Clear();
            foreach (Area ar in gv.mod.moduleAreasObjects)
            {
                AreaSave sar = new AreaSave();
                sar.Filename = ar.Filename;
                sar.Visible.Clear();
                foreach (int vis in ar.Visible)
                {
                    sar.Visible.Add(vis);
                }
                sar.Props.Clear();
                foreach (Prop prp in ar.Props)
                {
                    PropSave sprp = new PropSave();
                    sprp.PropTag = prp.PropTag;
                    sprp.LocationX = prp.LocationX;
                    sprp.LocationY = prp.LocationY;
                    sprp.lastLocationX = prp.lastLocationX;
                    sprp.lastLocationY = prp.lastLocationY;
                    sprp.PropFacingLeft = prp.PropFacingLeft;
                    sprp.isShown = prp.isShown;
                    sprp.isActive = prp.isActive;
                    sprp.isMover = prp.isMover;
                    sprp.isChaser = prp.isChaser;
                    sar.Props.Add(sprp);
                }
                sar.InitialAreaPropTagsList.Clear();
                foreach (string prp in ar.InitialAreaPropTagsList)
                {                    
                    sar.InitialAreaPropTagsList.Add(prp);
                }
                sar.Triggers.Clear();
                foreach (Trigger tr in ar.Triggers)
                {
                    TriggerSave str = new TriggerSave();
                    str.TriggerTag = tr.TriggerTag;
                    str.Enabled = tr.Enabled;
                    str.EnabledEvent1 = tr.EnabledEvent1;
                    str.EnabledEvent2 = tr.EnabledEvent2;
                    str.EnabledEvent3 = tr.EnabledEvent3;
                    sar.Triggers.Add(str);
                }
                saveMod.moduleAreasObjects.Add(sar);
            }
            saveMod.currentAreaFilename = gv.mod.currentArea.Filename;
            saveMod.moduleContainersList.Clear();
            foreach (Container cnt in gv.mod.moduleContainersList)
            {
                saveMod.moduleContainersList.Add(cnt.DeepCopy());
            }
            saveMod.moduleConvoSavedValuesList.Clear();
            foreach (ConvoSavedValues csv in gv.mod.moduleConvoSavedValuesList)
            {
                saveMod.moduleConvoSavedValuesList.Add(csv.DeepCopy());
            }
            saveMod.moduleEncountersCompletedList.Clear();
            foreach (Encounter enc in gv.mod.moduleEncountersList)
            {
                EncounterSave senc = new EncounterSave();
                senc.encounterName = enc.encounterName;
                if (enc.encounterCreatureRefsList.Count <= 0)
                {
                    senc.completed = true;
                }
                else
                {
                    senc.completed = false;
                }
                saveMod.moduleEncountersCompletedList.Add(senc);
            }
            saveMod.moduleGlobalInts.Clear();
            foreach (GlobalInt g in gv.mod.moduleGlobalInts)
            {
                saveMod.moduleGlobalInts.Add(g.DeepCopy());
            }
            saveMod.moduleGlobalStrings.Clear();
            foreach (GlobalString g in gv.mod.moduleGlobalStrings)
            {
                saveMod.moduleGlobalStrings.Add(g.DeepCopy());
            }
            saveMod.partyGold = gv.mod.partyGold;
            saveMod.WorldTime = gv.mod.WorldTime;
            saveMod.PlayerLocationY = gv.mod.PlayerLocationY;
            saveMod.PlayerLocationX = gv.mod.PlayerLocationX;
            saveMod.PlayerLastLocationY = gv.mod.PlayerLastLocationY;
            saveMod.PlayerLastLocationX = gv.mod.PlayerLastLocationX;
            saveMod.selectedPartyLeader = gv.mod.selectedPartyLeader;
            saveMod.showTutorialCombat = gv.mod.showTutorialCombat;
            saveMod.showTutorialInventory = gv.mod.showTutorialInventory;
            saveMod.showTutorialParty = gv.mod.showTutorialParty;
            saveMod.showTutorialCombat = gv.mod.showTutorialCombat;
            saveMod.showTutorialInventory = gv.mod.showTutorialInventory;
            saveMod.showTutorialParty = gv.mod.showTutorialParty;

            //SAVE THE FILE
            string filepath = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename;
            MakeDirectoryIfDoesntExist(filepath);
            string json = JsonConvert.SerializeObject(saveMod, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.Write(json.ToString());
            }

            //SAVE THE FILE
            //filepath = "C:\\Users\\Slowdive\\Dropbox\\IceBlink2mini\\saves\\" + gv.mod.moduleName + "\\" + filename;
            //MakeDirectoryIfDoesntExist(filepath);
            //json = JsonConvert.SerializeObject(saveMod, Newtonsoft.Json.Formatting.Indented);
            //using (StreamWriter sw = new StreamWriter(filepath))
            //{
            //    sw.Write(json.ToString());
            //}
        }
        //LOAD SAVEGAME
        public bool LoadSaveGame(string filename)
        {
            //  load a new module (actually already have a new module at this point from launch screen		
            //  load the saved game module
            SaveGame saveMod = null;
            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                saveMod = (SaveGame)serializer.Deserialize(file, typeof(SaveGame));
            }
            if (saveMod == null) { return false; }
            //  replace parts of new module with parts of saved game module
            //
            // U = update from save file	 
            //
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
            //U  "partyInventoryTagList": [], (use all save) update Items later on down
            gv.mod.partyInventoryRefsList.Clear();
            foreach (ItemRefs s in saveMod.partyInventoryRefsList)
            {
                gv.mod.partyInventoryRefsList.Add(s.DeepCopy());
            }
            //U  "moduleShopsList": [], (have an original shop items tags list and the current tags list to see what to add or delete from the save tags list)
            this.updateShops(saveMod);
            //U  "moduleAreasObjects": [],
            //                (triggers: use save trigger "enabled" value to update new)
            //                (tiles: use save "visible" to update new)
            //                (props: have an original props tags list and the current tags list to see what to add or delete from the save tags list)		               
            this.updateAreas(saveMod);
            //
            //U  "currentArea": {},
            gv.mod.setCurrentArea(saveMod.currentAreaFilename, gv);
            //U  "moduleContainersList": [], (have an original containers items tags list and the current tags list to see what to add or delete from the save tags list)
            this.updateContainers(saveMod);
            //U  "moduleConvoSavedValuesList": [], (use all save)
            gv.mod.moduleConvoSavedValuesList.Clear();
            foreach (ConvoSavedValues csv in saveMod.moduleConvoSavedValuesList)
            {
                gv.mod.moduleConvoSavedValuesList.Add(csv.DeepCopy());
            }
            //U  "moduleEncountersList": [], (use new except delete those completed already in save)
            foreach (EncounterSave enc in saveMod.moduleEncountersCompletedList)
            {
                if (enc.completed)
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
            //U  "partyGold": 70, (use all save)
            gv.mod.partyGold = saveMod.partyGold;
            //U  "WorldTime": 24, (use all save)
            gv.mod.WorldTime = saveMod.WorldTime;
            //U  "PlayerLocationY": 2, (use all save)
            gv.mod.PlayerLocationY = saveMod.PlayerLocationY;
            //U  "PlayerLocationX": 1, (use all save)
            gv.mod.PlayerLocationX = saveMod.PlayerLocationX;
            //U  "PlayerLastLocationY": 1, (use all save)
            gv.mod.PlayerLastLocationY = saveMod.PlayerLastLocationY;
            //U  "PlayerLastLocationX": 2, (use all save)
            gv.mod.PlayerLastLocationX = saveMod.PlayerLastLocationX;
            //U  "selectedPartyLeader": 0, (use all save)
            gv.mod.selectedPartyLeader = saveMod.selectedPartyLeader;
            //U  "showTutorialCombat": true, (use all save)
            gv.mod.showTutorialCombat = saveMod.showTutorialCombat;
            //U  "showTutorialInventory": true, (use all save)
            gv.mod.showTutorialInventory = saveMod.showTutorialInventory;
            //U  "showTutorialParty": true, (use all save)
            gv.mod.showTutorialParty = saveMod.showTutorialParty;
            
            gv.initializeSounds();

            gv.mod.partyTokenFilename = "prp_party";
            //gv.mod.partyTokenBitmap = this.LoadBitmap(gv.mod.partyTokenFilename);

            this.updatePlayers();
            this.updatePartyRosterPlayers();

            gv.createScreens();
            gv.screenMainMap.resetMiniMapBitmap();
            return true;
        }
        public void updateShops(SaveGame saveMod)
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
                                //make sure to add to initial list so it doesn't keep getting duplicated with every load save
                                updatedShop.initialShopItemRefs.Add(itemRef.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
        public void updateAreas(SaveGame saveMod)
        {
            foreach (Area ar in gv.mod.moduleAreasObjects)
            {
                foreach (AreaSave sar in saveMod.moduleAreasObjects)
                {
                    if (sar.Filename.Equals(ar.Filename)) //sar is saved game, ar is new game from toolset version
                    {
                        //tiles
                        for (int index = 0; index < ar.Visible.Count; index++)
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
                            foreach (PropSave sprp in sar.Props) //sprp is the saved game prop
                            {
                                if (prp.PropTag.Equals(sprp.PropTag))
                                {
                                    foundOne = true; //the prop tag exists in the saved game
                                    //replace the one in the toolset with the one from the saved game
                                    prp.LocationX = sprp.LocationX;
                                    prp.LocationY = sprp.LocationY;
                                    prp.lastLocationX = sprp.lastLocationX;
                                    prp.lastLocationY = sprp.lastLocationY;
                                    prp.PropFacingLeft = sprp.PropFacingLeft;
                                    prp.isShown = sprp.isShown;
                                    prp.isActive = sprp.isActive;
                                    prp.isMover = sprp.isMover;
                                    prp.isChaser = sprp.isChaser;
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
                            foreach (TriggerSave str in sar.Triggers)
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
        public void updateContainers(SaveGame saveMod)
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
                                //make sure to add to initial list so it doesn't keep getting duplicated with every load save
                                updatedCont.initialContainerItemRefs.Add(itemRef.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
        
        public void updatePlayers()
        {
            foreach (Player pc in gv.mod.playerList)
            {
                try { pc.race = gv.mod.getRace(pc.raceTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.playerClass = gv.mod.getPlayerClass(pc.classTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
            }
        }
        public void updatePartyRosterPlayers()
        {
            foreach (Player pc in gv.mod.partyRosterList)
            {
                try { pc.race = gv.mod.getRace(pc.raceTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.playerClass = gv.mod.getPlayerClass(pc.classTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
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
                //used for loading up the launcher screen only
                using (StreamReader sr = File.OpenText(folderAndFilename))
                {
                    string s = "";
                    s = sr.ReadLine();
                    if (!s.Equals("MODULE"))
                    {
                        MessageBox.Show("module file did not have 'MODULE' on first line, aborting...");
                        return null;
                    }
                    //Read in the module file line
                    for (int i = 0; i < 99; i++)
                    {
                        s = sr.ReadLine();
                        if ((s == null) || (s.Equals("AREAS")))
                        {
                            break;
                        }
                        toReturn = (Module)JsonConvert.DeserializeObject(s, typeof(Module));
                    }
                    for (int i = 0; i < 9999; i++)
                    {
                        s = sr.ReadLine();
                        if ((s == null) || (s.Equals("IMAGES")) || (s.Equals("END")))
                        {
                            break;
                        }
                    }
                    //Read in the images
                    commonBitmapList.Clear();
                    ImageData imd;
                    for (int i = 0; i < 9999; i++)
                    {
                        s = sr.ReadLine();
                        if ((s == null) || (s.Equals("END")))
                        {
                            break;
                        }
                        imd = (ImageData)JsonConvert.DeserializeObject(s, typeof(ImageData));                        
                        commonBitmapList.Add(imd.name, ConvertGDIBitmapToD2D(gv.bsc.ConvertImageDataToBitmap(imd)));
                    }
                }
            }
            else
            {
                //used for opening the entire module files
                using (StreamReader sr = File.OpenText(GetModulePath() + "\\" + folderAndFilename))
                {
                    string s = "";
                    s = sr.ReadLine();
                    if (!s.Equals("MODULE"))
                    {
                        MessageBox.Show("module file did not have 'MODULE' on first line, aborting...");
                        return null;
                    }
                    //Read in the module file line
                    for (int i = 0; i < 99; i++)
                    {
                        s = sr.ReadLine();
                        if ((s == null) || (s.Equals("AREAS")))
                        {
                            break;
                        }
                        toReturn = (Module)JsonConvert.DeserializeObject(s, typeof(Module));
                    }
                    //Read in the areas
                    toReturn.moduleAreasObjects.Clear();
                    Area ar;
                    for (int i = 0; i < 9999; i++)
                    {
                        s = sr.ReadLine();
                        if ((s == null) || (s.Equals("ENCOUNTERS")))
                        {
                            break;
                        }
                        ar = (Area)JsonConvert.DeserializeObject(s, typeof(Area));
                        toReturn.moduleAreasObjects.Add(ar);
                    }
                    //Read in the encounters
                    toReturn.moduleEncountersList.Clear();
                    Encounter enc;
                    for (int i = 0; i < 9999; i++)
                    {
                        s = sr.ReadLine();
                        if ((s == null) || (s.Equals("CONVOS")))
                        {
                            break;
                        }
                        enc = (Encounter)JsonConvert.DeserializeObject(s, typeof(Encounter));
                        toReturn.moduleEncountersList.Add(enc);
                    }
                    //Read in the areas
                    toReturn.moduleConvoList.Clear();
                    Convo cnv;
                    for (int i = 0; i < 9999; i++)
                    {
                        s = sr.ReadLine();
                        if ((s == null) || (s.Equals("IMAGES")) || (s.Equals("END")))
                        {
                            break;
                        }
                        cnv = (Convo)JsonConvert.DeserializeObject(s, typeof(Convo));
                        toReturn.moduleConvoList.Add(cnv);
                    }
                    //Read in the images
                    commonBitmapList.Clear();
                    ImageData imd;
                    for (int i = 0; i < 9999; i++)
                    {
                        s = sr.ReadLine();
                        if ((s == null) || (s.Equals("END")))
                        {
                            break;
                        }
                        imd = (ImageData)JsonConvert.DeserializeObject(s, typeof(ImageData));                        
                        commonBitmapList.Add(imd.name, ConvertGDIBitmapToD2D(gv.bsc.ConvertImageDataToBitmap(imd)));
                    }
                }
            }
            return toReturn;
        }
        
        public string GetModulePath()
        {
            return gv.mainDirectory + "\\modules";
        }

        //GENERAL
        public void nullOutControls()
        {
            btnHelp = null;
        }        
        public void setControlsStart()
        {
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padW = gv.squareSize / 6;
            
            if (btnHelp == null)
            {
                btnHelp = new IbbButton(gv, 0.8f);
                btnHelp.Text = "HELP";
                btnHelp.Img = "btn_small"; // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnHelp.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnHelp.X = 6 * gv.squareSize + padW * 1;
                btnHelp.Y = 9 * gv.squareSize + pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);
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
        public void doAboutDialog()
        {
            gv.sf.MessageBoxHtml(gv.mod.moduleCredits);
        }
        
        public void addLogText(string color, string text)
        {
            if (color.Equals("red"))
            {
                gv.log.AddHtmlTextToLog("<rd>" + text + "</rd>");
            }
            else if (color.Equals("lime"))
            {
                gv.log.AddHtmlTextToLog("<gn>" + text + "</gn>");
            }
            else if (color.Equals("yellow"))
            {
                gv.log.AddHtmlTextToLog("<yl>" + text + "</yl>");
            }
            else if (color.Equals("teal"))
            {
                gv.log.AddHtmlTextToLog("<bu>" + text + "</bu>");
            }
            else if (color.Equals("blue"))
            {
                gv.log.AddHtmlTextToLog("<bu>" + text + "</bu>");
            }
            else if (color.Equals("fuchsia"))
            {
                gv.log.AddHtmlTextToLog("<ma>" + text + "</ma>");
            }
            else
            {
                gv.log.AddHtmlTextToLog("<wh>" + text + "</wh>");
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
            int txtH = (int)gv.fontHeight;
            int x = ((coorInSquares.X * gv.squareSize) + (gv.squareSize / 2)) - (txtH / 2);
            int y = ((coorInSquares.Y * gv.squareSize) + (gv.squareSize / 2) + txtH) - (txtH / 2);
            Coordinate coor = new Coordinate(x, y);
            floatyTextList.Add(new FloatyText(coor, value));
        }
        public void addFloatyText(Coordinate coorInSquares, string value, string color)
        {
            int txtH = (int)gv.fontHeight;
            int x = ((coorInSquares.X * gv.squareSize) + (gv.squareSize / 2)) - (txtH / 2);
            int y = ((coorInSquares.Y * gv.squareSize) + (gv.squareSize / 2) + txtH) - (txtH / 2);
            Coordinate coor = new Coordinate(x, y);
            floatyTextList.Add(new FloatyText(coor, value, color));
        }
        public void addFloatyText(Coordinate coorInSquares, string value, int shiftUp)
        {
            int txtH = (int)gv.fontHeight;
            int x = ((coorInSquares.X * gv.squareSize) + (gv.squareSize / 2)) - (txtH / 2);
            int y = ((coorInSquares.Y * gv.squareSize) + (gv.squareSize / 2) + txtH) - (txtH / 2) - shiftUp;
            Coordinate coor = new Coordinate(x, y);
            floatyTextList.Add(new FloatyText(coor, value));
        }

        public void doUpdate()
        {
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
                gv.sf.MessageBoxHtml("Everybody is unconscious and bleeding - your party has been defeated!");
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

            //check for levelup available and switch button image
            checkLevelUpAvailable(); //move this to on update and use a plus overlay in top left            
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
        public void checkLevelUpAvailable()
        {            
            if (gv.mod.playerList.Count > 0)
            {
                //if (gv.mod.playerList[0].IsReadyToAdvanceLevel()) { gv.cc.ptrPc0.levelUpOn = true; }
                //else { gv.cc.ptrPc0.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 1)
            {
                //if (gv.mod.playerList[1].IsReadyToAdvanceLevel()) { gv.cc.ptrPc1.levelUpOn = true; }
                //else { gv.cc.ptrPc1.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 2)
            {
                //if (gv.mod.playerList[2].IsReadyToAdvanceLevel()) { gv.cc.ptrPc2.levelUpOn = true; }
                //else { gv.cc.ptrPc2.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 3)
            {
                //if (gv.mod.playerList[3].IsReadyToAdvanceLevel()) { gv.cc.ptrPc3.levelUpOn = true; }
                //else { gv.cc.ptrPc3.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 4)
            {
                //if (gv.mod.playerList[4].IsReadyToAdvanceLevel()) { gv.cc.ptrPc4.levelUpOn = true; }
                //else { gv.cc.ptrPc4.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 5)
            {
                //if (gv.mod.playerList[5].IsReadyToAdvanceLevel()) { gv.cc.ptrPc5.levelUpOn = true; }
                //else { gv.cc.ptrPc5.levelUpOn = false; }
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
                int playerPositionXInPix = gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffsetX * gv.squareSize);
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
                                        gv.cc.addLogText("<yl>" + gv.mod.currentArea.Props[i].PropTag + " start chasing " + gv.mod.currentArea.Props[i].ChaserChaseDuration + " seconds</yl><BR>");
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
                                        gv.cc.addLogText("<yl>" + gv.mod.currentArea.Props[i].PropTag + " stop chasing on range</yl><BR>");
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
                                        gv.cc.addLogText("<yl>" + gv.mod.currentArea.Props[i].PropTag + " stop chasing on duration</yl><BR>");
                                    }
                                }
                                else
                                {
                                    if (gv.mod.debugMode)
                                    {
                                        int timeRemain = gv.mod.currentArea.Props[i].ChaserChaseDuration - (gv.mod.WorldTime - gv.mod.currentArea.Props[i].ChaserStartChasingTime);
                                        gv.cc.addLogText("<yl>" + gv.mod.currentArea.Props[i].PropTag + " chasing " + timeRemain + " seconds left</yl><BR>");
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
                                gv.cc.addLogText("<yl>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</yl><BR>");
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
                                    gv.cc.addLogText("<yl>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</yl><BR>");
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
                                gv.cc.addLogText("<yl>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</yl><BR>");
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
                                    gv.cc.addLogText("<yl>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</yl><BR>");
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
                                    gv.cc.addLogText("<yl>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</yl><BR>");
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
                        /*if (gv.realTimeTimerMilliSecondsEllapsed >= gv.mod.realTimeTimerLengthInMilliSeconds)
                        {
                            doNotTriggerProp = true;
                        }*/
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
            try
            {
                Trigger trig = gv.mod.currentArea.getTriggerByLocation(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY);
                if ((trig != null) && (trig.Enabled))
                {
                    blockSecondPropTriggersCall = true;
                    //iterate through each event                  
                    #region Event1 stuff
                    //check to see if enabled and parm not "none"                    
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
                    #endregion
                    #region Event2 stuff
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
                    #endregion
                    #region Event3 stuff
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
                    #endregion
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
            try
            {
                gv.screenConvo.currentConvo = gv.mod.getConvoByName(tag);
                if (gv.screenConvo.currentConvo != null)
                {
                    gv.screenType = "convo";
                    gv.screenConvo.startConvo();
                }
                else
                {
                    gv.sf.MessageBox("failed to find conversation in list with tag: " + tag);
                }
            }
            catch (Exception ex)
            {
                gv.sf.MessageBox("failed to open conversation with tag: " + tag);
                gv.errorLog(ex.ToString());
            }
        }
        public void doSpellBasedOnScriptOrEffectTag(Spell spell, object source, object target, bool outsideCombat)
        {
            gv.sf.AoeTargetsList.Clear();

            if (!spell.spellEffectTag.Equals("none"))
            {
                gv.sf.spGeneric(spell, source, target, outsideCombat);
            }

            //WIZARD SPELLS
            else if (spell.spellScript.Equals("spDimensionDoor"))
            {
                gv.sf.spDimensionDoor(source, target);
            }
                        
            //CLERIC SPELLS
            else if (spell.tag.Equals("minorHealing"))
            {
                //gv.sf.spHeal(source, target, 8);
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
        /*public void doItemScriptBasedOnUseItem(Player pc, ItemRefs itRef, bool destroyItemAfterUse)
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
        }*/
                        
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
        public int getCreatureSize(string tokenfilename)
        {
            //1=normal, 2=wide, 3=tall, 4=large
            int width = gv.cc.GetFromBitmapList(tokenfilename).PixelSize.Width;
            int height = gv.cc.GetFromBitmapList(tokenfilename).PixelSize.Height;
            //normal
            if ((width == gv.standardTokenSize) && (height == gv.standardTokenSize * 2))
            {
                return 1;
            }
            //wide
            else if ((width == gv.standardTokenSize * 2) && (height == gv.standardTokenSize * 2))
            {
                return 2;
            }
            //tall
            else if ((width == gv.standardTokenSize) && (height == gv.standardTokenSize * 4))
            {
                return 3;
            }
            //large
            else if ((width == gv.standardTokenSize * 2) && (height == gv.standardTokenSize * 4))
            {
                return 4;
            }
            return 1;
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
                //default graphics locations
                if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".jpg"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".jpg");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename + ".jpg"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename + ".jpg");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename + ".jpg"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename + ".jpg");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename);
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
        public void MakeDirectoryIfDoesntExist(string filenameAndFullPath)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filenameAndFullPath);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
        }
        
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
        public List<IBminiFormattedLine> ProcessHtmlString(string text, int width, List<string> tagStack, bool IBmini)
        {
            bool tagMode = false;
            string tag = "";
            IBminiFormattedWord newWord = new IBminiFormattedWord();
            IBminiFormattedLine newLine = new IBminiFormattedLine();
            List<IBminiFormattedLine> logLinesList = new List<IBminiFormattedLine>();
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
                        newWord.color = GetColor(tagStack);
                        int wordWidth = (newWord.text.Length + 1) * (gv.fontWidth + gv.fontCharSpacing);
                        if (xLoc + wordWidth > (width) - (gv.fontWidth * 2)) //word wrap
                        {
                            //end last line and add it to the log
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new IBminiFormattedLine();
                            newLine.wordsList.Add(newWord);
                            xLoc = 0;
                        }
                        else //no word wrap, just add word
                        {
                            newLine.wordsList.Add(newWord);
                        }
                        //instead of drawing, just add to line list 
                        xLoc += wordWidth;
                        newWord = new IBminiFormattedWord();
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
                            newWord.color = GetColor(tagStack);
                            //end last line and add it to the log
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new IBminiFormattedLine();
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
                        newWord.color = GetColor(tagStack);
                        int wordWidth = (newWord.text.Length + 1) * (gv.fontWidth + gv.fontCharSpacing);
                        if (xLoc + wordWidth > (width) - (gv.fontWidth * 2)) //word wrap
                        {
                            //end last line and add it to the log
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new IBminiFormattedLine();
                            newLine.wordsList.Add(newWord);
                            xLoc = 0;
                        }
                        else //no word wrap, just add word
                        {
                            newLine.wordsList.Add(newWord);
                        }
                        //instead of drawing, just add to line list 
                        xLoc += wordWidth;
                        newWord = new IBminiFormattedWord();
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
        private string GetColor(List<string> tagStack)
        {
            //will end up using the last color on the stack
            string clr = "wh";
            foreach (string s in tagStack)
            {
                if ((s.Equals("Bk")) || (s.Equals("bk")))
                {
                    clr = "bk";
                }
                else if ((s.Equals("Bu")) || (s.Equals("bu")))
                {
                    clr = "bu";
                }
                else if ((s.Equals("Gn")) || (s.Equals("gn")))
                {
                    clr = "gn";
                }
                else if ((s.Equals("Gy")) || (s.Equals("gy")))
                {
                    clr = "gy";
                }
                else if ((s.Equals("Ma")) || (s.Equals("ma")))
                {
                    clr = "ma";
                }
                else if ((s.Equals("Rd")) || (s.Equals("rd")))
                {
                    clr = "rd";
                }
                else if ((s.Equals("Yl")) || (s.Equals("yl")))
                {
                    clr = "yl";
                }
            }
            return clr;
        }
    }
}
