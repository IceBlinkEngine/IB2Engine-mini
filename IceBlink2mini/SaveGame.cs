using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceBlink2mini
{
    public class SaveGame
    {
        public string saveName = "empty";
        public List<Player> playerList = new List<Player>();
        public List<Player> partyRosterList = new List<Player>();
        public List<JournalQuest> partyJournalQuests = new List<JournalQuest>();
        public List<ItemRefs> partyInventoryRefsList = new List<ItemRefs>();
        public List<Shop> moduleShopsList = new List<Shop>();
        public List<AreaSave> moduleAreasObjects = new List<AreaSave>(); //visible, props, triggers
        public string currentAreaFilename = ""; //store area name only
        public List<EncounterSave> moduleEncountersCompletedList = new List<EncounterSave>(); //just name and if empty
        public List<Container> moduleContainersList = new List<Container>();
        public List<ConvoSavedValues> moduleConvoSavedValuesList = new List<ConvoSavedValues>();
        public List<GlobalInt> moduleGlobalInts = new List<GlobalInt>();
        public List<GlobalString> moduleGlobalStrings = new List<GlobalString>();
        public int partyGold = 0;
        public int WorldTime = 0;
        public int PlayerLocationX = 4;
        public int PlayerLocationY = 1;
        public int PlayerLastLocationX = 4;
        public int PlayerLastLocationY = 1;
        public int selectedPartyLeader = 0;
        public bool showTutorialParty = true;
        public bool showTutorialInventory = true;
        public bool showTutorialCombat = true;

        public SaveGame()
        {

        }
    }

    public class EncounterSave
    {
        public string encounterName = "";
        public bool completed = false;

        public EncounterSave()
        {

        }
    }

    public class AreaSave
    {
        public string Filename = "";
        public List<int> Visible = new List<int>();
        public List<PropSave> Props = new List<PropSave>();
        public List<string> InitialAreaPropTagsList = new List<string>();
        public List<TriggerSave> Triggers = new List<TriggerSave>();

        public AreaSave()
        {

        }
    }

    public class PropSave
    {
        public string PropTag = "";
        public int LocationX = 0;
        public int LocationY = 0;
        public int lastLocationX = 0;
        public int lastLocationY = 0;
        public bool PropFacingLeft = true;
        public bool isShown = true;
        public bool isActive = true;
        public bool isMover = false;
        public bool isChaser = false;

        public PropSave()
        {

        }
    }

    public class TriggerSave
    {
        public string TriggerTag = "";
        public bool Enabled = true;
        public bool EnabledEvent1 = true;
        public bool EnabledEvent2 = true;
        public bool EnabledEvent3 = true;

        public TriggerSave()
        {

        }
    }
}
