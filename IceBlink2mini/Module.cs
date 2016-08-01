﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using Newtonsoft.Json;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2mini
{
    public class Module
    {
        public string moduleName = "none";
        public string moduleLabelName = "none";
        public int moduleVersion = 1;
        public string saveName = "empty";
        public string defaultPlayerFilename = "drin.json";
        public bool mustUsePreMadePC = false;
        public int numberOfPlayerMadePcsAllowed = 1;
        public int MaxPartySize = 6;
        public string moduleDescription = "";
        public string moduleCredits = "";
        public int nextIdNumber = 100;
        public int WorldTime = 0;
        public int TimePerRound = 6;
        public bool debugMode = false;
        public float diagonalMoveCost = 1.5f;
        public bool allowSave = true;
        public bool useLuck = false;
        public bool hideRoster = false;
        public bool use3d6 = false;
        public bool useUIBackground = true;
        public string fontName = "Metamorphous";
        public string fontFilename = "Metamorphous-Regular.ttf";
        public float fontD2DScaleMultiplier = 1.0f;
        public int logNumberOfLines = 20;
        public string spellLabelSingular = "Spell";
        public string spellLabelPlural = "Spells";
        public string goldLabelSingular = "Gold";
        public string goldLabelPlural = "Gold";
        public bool ArmorClassAscending = true;
        public List<Container> moduleContainersList = new List<Container>();
        public List<Shop> moduleShopsList = new List<Shop>();

        //[JsonIgnore]
        public List<Item> moduleItemsList = new List<Item>();
        //[JsonIgnore]
        public List<Encounter> moduleEncountersList = new List<Encounter>();        
        //[JsonIgnore]
        public List<Creature> moduleCreaturesList = new List<Creature>();
        //[JsonIgnore]
        public List<JournalQuest> moduleJournal = new List<JournalQuest>();
        //[JsonIgnore]
        public List<PlayerClass> modulePlayerClassList = new List<PlayerClass>();
        //[JsonIgnore]
        public List<Race> moduleRacesList = new List<Race>();
        //[JsonIgnore]
        public List<Spell> moduleSpellsList = new List<Spell>();
        //[JsonIgnore]
        public List<Trait> moduleTraitsList = new List<Trait>();
        //[JsonIgnore]
        public List<Effect> moduleEffectsList = new List<Effect>();
        //[JsonIgnore]
        public List<Convo> moduleConvoList = new List<Convo>();
        //[JsonIgnore]
        public List<IBScript> moduleIBScriptList = new List<IBScript>();

        public List<Area> moduleAreasObjects = new List<Area>();
        public List<GlobalInt> moduleGlobalInts = new List<GlobalInt>();
        public List<GlobalString> moduleGlobalStrings = new List<GlobalString>();
        public List<ConvoSavedValues> moduleConvoSavedValuesList = new List<ConvoSavedValues>();
        public string startingArea = "";
        public int startingPlayerPositionX = 0;
        public int startingPlayerPositionY = 0;
        public int PlayerLocationX = 4;
        public int PlayerLocationY = 1;
        public int PlayerLastLocationX = 4;
        public int PlayerLastLocationY = 1;
        [JsonIgnore]
        public bool PlayerFacingLeft = true;
        public Area currentArea = new Area();
        [JsonIgnore]
        public Encounter currentEncounter = new Encounter();
        public int partyGold = 0;
        public bool showPartyToken = false;
        public string partyTokenFilename = "prp_party";
        [JsonIgnore]
        public Bitmap partyTokenBitmap;
        public List<Player> playerList = new List<Player>();
        public List<Player> partyRosterList = new List<Player>();
        public List<Player> companionPlayerList = new List<Player>();
        public List<ItemRefs> partyInventoryRefsList = new List<ItemRefs>();
        public List<JournalQuest> partyJournalQuests = new List<JournalQuest>();
        public List<JournalQuest> partyJournalCompleted = new List<JournalQuest>();
        public string partyJournalNotes = "";
        public int selectedPartyLeader = 0;
        [JsonIgnore]
        public bool returnCheck = false;
        [JsonIgnore]
        public bool addPCScriptFired = false;
        [JsonIgnore]
        public bool uncheckConvo = false;
        [JsonIgnore]
        public bool removeCreature = false;
        [JsonIgnore]
        public bool deleteItemUsedScript = false;
        [JsonIgnore]
        public int indexOfPCtoLastUseItem = 0;
        public bool com_showGrid = false;
        public bool map_showGrid = false;
        public bool playMusic = false;
        public bool playSoundFx = false;
        public bool playButtonSounds = false;
        public bool playButtonHaptic = false;
        public bool showTutorialParty = true;
        public bool showTutorialInventory = true;
        public bool showTutorialCombat = true;
        public bool showAutosaveMessage = true;
        public bool allowAutosave = true;
        public int combatAnimationSpeed = 100;
        public bool fastMode = false;
        public int attackAnimationSpeed = 100;
        public float soundVolume = 1.0f;
        public string OnHeartBeatIBScript = "none";
        public string OnHeartBeatIBScriptParms = "";
        public bool showInteractionState = false;
        public bool avoidInteraction = false;
        public bool useRealTimeTimer = false;
        public int realTimeTimerLengthInMilliSeconds = 1500;
        public int attackFromBehindToHitModifier = 2;
        public int attackFromBehindDamageModifier = 0;        
        [JsonIgnore]
        public List<Bitmap> loadedTileBitmaps = new List<Bitmap>();
        public List<string> loadedTileBitmapsNames = new List<string>();
        [JsonIgnore]
        public List<System.Drawing.Bitmap> loadedMinimapTileBitmaps = new List<System.Drawing.Bitmap>();
        public List<string> loadedMinimapTileBitmapsNames = new List<string>();
        public bool doConvo = true;
        public int noTriggerLocX = -1;
        public int noTriggerLocY = -1;
        public bool firstTriggerCall = true;
        public bool isRecursiveCall = false;

        public Module()
        {

        }
        public void loadAreas(GameView gv)
        {
            /*foreach (string areaName in this.moduleAreasList)
            {
                try
                {
                    using (StreamReader file = File.OpenText(gv.mainDirectory + "\\modules\\" + this.moduleName + "\\areas\\" + areaName + ".lvl"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Area newArea = (Area)serializer.Deserialize(file, typeof(Area));
                        foreach (Prop p in newArea.Props)
                        {
                            p.initializeProp();
                        }
                        moduleAreasObjects.Add(newArea);
                    }
                }
                catch (Exception ex)
                {
                    gv.errorLog(ex.ToString());
                }
            }*/
        }
        public void setCurrentArea(string filename, GameView gv)
        {
            try
            {
                foreach (Area area in this.moduleAreasObjects)
                {
                    if (area.Filename.Equals(filename))
                    {
                        this.currentArea = area;
                        //gv.cc.DisposeOfBitmap(ref gv.cc.bmpMap);
                        //gv.cc.bmpMap = gv.cc.LoadBitmap(this.currentArea.ImageFileName);
                        //TODO gv.cc.LoadTileBitmapList();
                        foreach (Prop p in this.currentArea.Props)
                        {
                            gv.cc.DisposeOfBitmap(ref p.token);
                            p.token = gv.cc.LoadBitmap(p.ImageFileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }
        }

        public int getNextIdNumber()
        {
            this.nextIdNumber++;
            return this.nextIdNumber;
        }
        public Player getPlayerByName(string tag)
        {
            foreach (Player pc in this.playerList)
            {
                if (pc.name.Equals(tag))
                {
                    return pc;
                }
            }
            return null;
        }
        public Item getItemByTag(string tag)
        {
            foreach (Item it in this.moduleItemsList)
            {
                if (it.tag.Equals(tag)) return it;
            }
            return null;
        }
        public Item getItemByResRef(string resref)
        {
            foreach (Item it in this.moduleItemsList)
            {
                if (it.resref.Equals(resref)) return it;
            }
            return null;
        }
        public ItemRefs getItemRefsInInventoryByResRef(string resref)
        {
            foreach (ItemRefs itr in this.partyInventoryRefsList)
            {
                if (itr.resref.Equals(resref)) return itr;
            }
            return null;
        }
        public Item getItemByResRefForInfo(string resref)
        {
            foreach (Item it in this.moduleItemsList)
            {
                if (it.resref.Equals(resref)) return it;
            }
            return new Item();
        }
        public ItemRefs createItemRefsFromItem(Item it)
        {
            ItemRefs newIR = new ItemRefs();
            newIR.tag = it.tag + "_" + this.getNextIdNumber();
            newIR.name = it.name;
            newIR.resref = it.resref;
            newIR.canNotBeUnequipped = it.canNotBeUnequipped;
            newIR.quantity = it.quantity;
            return newIR;
        }
        public Container getContainerByTag(string tag)
        {
            foreach (Container it in this.moduleContainersList)
            {
                if (it.containerTag.Equals(tag)) return it;
            }
            return null;
        }
        public Shop getShopByTag(string tag)
        {
            foreach (Shop s in this.moduleShopsList)
            {
                if (s.shopTag.Equals(tag)) return s;
            }
            return null;
        }
        public Encounter getEncounter(string name)
        {
            foreach (Encounter e in this.moduleEncountersList)
            {
                if (e.encounterName.Equals(name)) return e;
            }
            return null;
        }
        public Convo getConvoByName(string name)
        {
            foreach (Convo e in this.moduleConvoList)
            {
                if (e.ConvoFileName.Equals(name)) return e;
            }
            return null;
        }
        public Creature getCreatureInCurrentEncounterByTag(string tag)
        {
            foreach (Creature crt in this.currentEncounter.encounterCreatureList)
            {
                if (crt.tag.Equals(tag)) return crt;
            }
            return null;
        }
        public Spell getSpellByTag(string tag)
        {
            foreach (Spell s in this.moduleSpellsList)
            {
                if (s.tag.Equals(tag)) return s;
            }
            return null;
        }
        public Trait getTraitByTag(string tag)
        {
            foreach (Trait t in this.moduleTraitsList)
            {
                if (t.tag.Equals(tag)) return t;
            }
            return null;
        }        
        public Effect getEffectByTag(string tag)
        {
            foreach (Effect ef in this.moduleEffectsList)
            {
                if (ef.tag.Equals(tag)) return ef;
            }
            return null;
        }
        public PlayerClass getPlayerClass(string tag)
        {
            foreach (PlayerClass p in this.modulePlayerClassList)
            {
                if (p.tag.Equals(tag)) return p;
            }
            return null;
        }
        public Race getRace(string tag)
        {
            foreach (Race r in this.moduleRacesList)
            {
                if (r.tag.Equals(tag)) return r;
            }
            return null;
        }
        public IBScript getIBScriptByName(string name)
        {
            foreach (IBScript t in this.moduleIBScriptList)
            {
                if (t.scriptName.Equals(name)) return t;
            }
            return null;
        }
        public JournalQuest getJournalCategoryByTag(string tag)
        {
            foreach (JournalQuest it in this.moduleJournal)
            {
                if (it.Tag.Equals(tag)) return it;
            }
            return null;
        }
        public JournalQuest getPartyJournalActiveCategoryByTag(string tag)
        {
            foreach (JournalQuest it in this.partyJournalQuests)
            {
                if (it.Tag.Equals(tag)) return it;
            }
            return null;
        }
        public JournalQuest getPartyJournalCompletedCategoryByTag(string tag)
        {
            foreach (JournalQuest it in this.partyJournalCompleted)
            {
                if (it.Tag.Equals(tag)) return it;
            }
            return null;
        }
    }
}
