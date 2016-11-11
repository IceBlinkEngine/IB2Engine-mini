using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Media;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX;
using SharpDX.Windows;
using Message = System.Windows.Forms.Message;
using System.Diagnostics;

namespace IceBlink2mini
{
    public partial class GameView : RenderForm
    {
        //this class is handled differently than Android version
        public float screenDensity;
        public int screenWidth;
        public int screenHeight;
        public int squareSizeInPixels = 24;
        public int tileSizeInPixels = 48;
        public int standardTokenSize = 24;
        public int squareSize; //in dp (squareSizeInPixels * screenDensity)
        public int pS; // = squareSize / 10 ... used for small UI and text location adjustments based on squaresize
        public int squaresInWidth = 19;
        public int squaresInHeight = 11;
        public int ibbwidthL = 120;
        public int ibbwidthR = 25;
        public int ibbheight = 25;
        public int ibpwidth = 27;
        public int ibpheight = 42;
        public int playerOffset = 5;
        public int playerOffsetX = 5;
        public int playerOffsetY = 5;
        public int oXshift = 0;
        public int oYshift = 0;
        public string mainDirectory;
        public bool showHotKeys = false;
        public int fontHeight = 8;
        public int fontWidth = 8;
        public int fontCharSpacing = 1;
        public int fontLineSpacing = 2;

        //DIRECT2D STUFF
        public SharpDX.Direct3D11.Device _device;
        public SwapChain _swapChain;
        public Texture2D _backBuffer;
        public RenderTargetView _backBufferView;
        public SharpDX.Direct2D1.Factory factory2D;
        public SharpDX.DirectWrite.Factory factoryDWrite;
        public RenderTarget renderTarget2D;
        public SolidColorBrush sceneColorBrush;

        public string versionNum = "v1.00";
        public string fixedModule = "";
        public Dictionary<char, SharpDX.RectangleF> charList = new Dictionary<char, SharpDX.RectangleF>();
        public string screenType = "splash"; //launcher, title, main, party, inventory, combatInventory, shop, journal, combat, combatCast, convo
        public AnimationState animationState = AnimationState.None;
        public int triggerIndex = 0;
        public int triggerPropIndex = 0;
        public BitmapStringConversion bsc;

        public IB2HtmlLogBox log;
        public IBminiMessageBox messageBox;
        public bool showMessageBox = false;
        public IBminiItemListSelector itemListSelector;
        public CommonCode cc;
        public Module mod;
        public ScriptFunctions sf;
        public PathFinderAreas pfa;
        public ScreenParty screenParty;
        public ScreenInventory screenInventory;
        public ScreenItemSelector screenItemSelector;
        public ScreenPortraitSelector screenPortraitSelector;
        public ScreenTokenSelector screenTokenSelector;
        public ScreenPcSelector screenPcSelector;
        public ScreenJournal screenJournal;
        public ScreenShop screenShop;
        public ScreenCastSelector screenCastSelector;
        public ScreenConvo screenConvo;
        public ScreenTitle screenTitle;
        public ScreenPcCreation screenPcCreation;
        public ScreenSpellLevelUp screenSpellLevelUp;
        public ScreenTraitLevelUp screenTraitLevelUp;
        public ScreenLauncher screenLauncher;
        public ScreenCombat screenCombat;
        public ScreenMainMap screenMainMap;
        public ScreenPartyBuild screenPartyBuild;
        public ScreenPartyRoster screenPartyRoster;
        public bool touchEnabled = true;
        
        public SoundPlayer soundPlayer = new SoundPlayer();
        public Dictionary<string, Stream> oSoundStreams = new Dictionary<string, Stream>();
        public System.Media.SoundPlayer playerButtonEnter = new System.Media.SoundPlayer();
        public System.Media.SoundPlayer playerButtonClick = new System.Media.SoundPlayer();
       
        public Timer gameTimer = new Timer();
        public Stopwatch gameTimerStopwatch = new Stopwatch();
        public long previousTime = 0;
        public bool stillProcessingGameLoop = false;
        public float fps = 0;
        public int reportFPScount = 0;
        public Timer animationTimer = new Timer();

        public GameView()
        {
            InitializeComponent();

            cc = new CommonCode(this);
            mod = new Module();
            bsc = new BitmapStringConversion();
            
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GameView_MouseWheel);
            mainDirectory = Directory.GetCurrentDirectory();

            try
            {
                playerButtonClick.SoundLocation = mainDirectory + "\\default\\NewModule\\sounds\\btn_click.wav";
                playerButtonClick.Load();
            }
            catch (Exception ex) { errorLog(ex.ToString()); } 
            try
            {
                playerButtonEnter.SoundLocation = mainDirectory + "\\default\\NewModule\\sounds\\btn_hover.wav";
                playerButtonEnter.Load();
            }
            catch (Exception ex) { errorLog(ex.ToString()); }

            this.MinimumSize = new Size(100, 100);
            //this is the standard way, comment out the next 3 lines if manually forcing a screen resolution for testing UI layouts
            //this.WindowState = FormWindowState.Maximized;
            //this.Width = Screen.PrimaryScreen.Bounds.Width;
            //this.Height = Screen.PrimaryScreen.Bounds.Height;            
            //for testing other screen sizes, manually enter a resolution here
            //typical resolutions: 1366x768, 1920x1080, 1280x1024, 1280x800, 1024x768, 800x600, 1440x900, 1280x720, 640x360, 427x240, 1368x792, 912x528, 456x264, 960x540,
            this.Width = 1366;
            this.Height = 768;

            screenWidth = this.Width;
            screenHeight = this.Height;
            float sqrW = (float)screenWidth / (float)(squaresInWidth);
            float sqrH = (float)screenHeight / (float)(squaresInHeight);
            if (sqrW > sqrH)
            {
                squareSize = (int)(sqrH);
            }
            else
            {
                squareSize = (int)(sqrW);
            }
            if ((squareSize >= 96) && (squareSize < 102))
            {
                squareSize = 96;
            }
            else if ((squareSize >= 68) && (squareSize < 76))
            {
                squareSize = 72;
            }
            else if ((squareSize >= 48) && (squareSize < 52))
            {
                squareSize = 48;
            }
            screenDensity = (float)squareSize / (float)squareSizeInPixels;
            oXshift = (screenWidth - (squareSize * squaresInWidth)) / 2;
            oYshift = (screenHeight - (squareSize * squaresInHeight)) / 2;

            pS = squareSize / 10; //used for small UI and text location adjustments based on squaresize for consistent look on all devices/screen resolutions

            InitializeRenderer(); //uncomment this for DIRECT2D ADDITIONS

            //CREATES A FONTFAMILY
            fillCharList();

            fontWidth = (int)(6 * screenDensity); //4
            fontHeight = (int)(6 * screenDensity); //4
            fontCharSpacing = fontWidth / 8; //8
            fontLineSpacing = fontHeight / 2; //2

            //force font to best size if squareSize is near to multiple of 48px
            if ((squareSize >= 40) && (squareSize < 56)) //48x48
            {
                fontWidth = 12; //8
                fontHeight = 12; //8
                fontCharSpacing = 1; //1
                fontLineSpacing = 6; //4
            }
            else if ((squareSize >= 64) && (squareSize < 80)) //72x72
            {
                fontWidth = 16;
                fontHeight = 16;
                fontCharSpacing = 1;
                fontLineSpacing = 8;
            }
            else if ((squareSize >= 84) && (squareSize < 108)) //96x96
            {
                fontWidth = 24;
                fontHeight = 24;
                fontCharSpacing = 2;
                fontLineSpacing = 12;
            }
            else if ((squareSize >= 128) && (squareSize < 160)) //144x144
            {
                fontWidth = 32;
                fontHeight = 32;
                fontCharSpacing = 2;
                fontLineSpacing = 16;
            }

            animationTimer.Tick += new System.EventHandler(this.AnimationTimer_Tick);

            log = new IB2HtmlLogBox(this);
            log.numberOfLinesToShow = 40;

            //setup messageBox defaults
            messageBox = new IBminiMessageBox(this);
            messageBox.currentLocX = 100;
            messageBox.currentLocY = 25;
            messageBox.numberOfLinesToShow = 28;
            messageBox.tbWidth = 250;
            messageBox.Width = 250;
            messageBox.Height = 195;
            messageBox.tbHeight = 187;
            messageBox.setupIBminiMessageBox();
            
            //setup itemListSelector defaults
            itemListSelector = new IBminiItemListSelector();
            itemListSelector.currentLocX = 73;
            itemListSelector.currentLocY = 25;
            itemListSelector.Width = 310;
            itemListSelector.Height = 220;
            
            if (fixedModule.Equals("")) //this is the IceBlink Engine app
            {
                screenLauncher = new ScreenLauncher(mod, this);
                screenLauncher.loadModuleFiles();
                screenType = "launcher";
            }
            else //this is a fixed module
            {
                mod = cc.LoadModule(fixedModule + "/" + fixedModule + ".mod", false);
                resetGame();
                cc.LoadSaveListItems();
                screenType = "title";
            }
            gameTimer.Interval = 16; //~60 fps
            gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick);
            gameTimerStopwatch.Start();
            previousTime = gameTimerStopwatch.ElapsedMilliseconds;
            gameTimer.Start();
        }

        public void createScreens()
	    {
		    sf = new ScriptFunctions(mod, this);
		    pfa = new PathFinderAreas(mod);
		    screenParty = new ScreenParty(mod, this);
		    screenInventory = new ScreenInventory(mod, this);
            screenItemSelector = new ScreenItemSelector(mod, this);
            screenPortraitSelector = new ScreenPortraitSelector(mod, this);
            screenTokenSelector = new ScreenTokenSelector(mod, this);
            screenPcSelector = new ScreenPcSelector(mod, this);
		    screenJournal = new ScreenJournal(mod, this);	
		    screenShop = new ScreenShop(mod, this);
		    screenCastSelector = new ScreenCastSelector(mod, this);
		    screenConvo = new ScreenConvo(mod, this);		    
		    screenMainMap = new ScreenMainMap(mod, this);
            screenCombat = new ScreenCombat(mod, this);
            screenTitle = new ScreenTitle(mod, this);
		    screenPcCreation = new ScreenPcCreation(mod, this);
		    screenSpellLevelUp = new ScreenSpellLevelUp(mod, this);
		    screenTraitLevelUp = new ScreenTraitLevelUp(mod, this);		
		    screenLauncher = new ScreenLauncher(mod, this);
		    screenPartyBuild = new ScreenPartyBuild(mod, this);
            screenPartyRoster = new ScreenPartyRoster(mod,this);
	    }
        public void LoadStandardImages()
        {
            //cc.btnIni = cc.LoadBitmap("btn_ini");
            //cc.btnIniGlow = cc.LoadBitmap("btn_ini_glow");
            cc.walkPass = cc.LoadBitmap("walk_pass");
            cc.walkBlocked = cc.LoadBitmap("walk_block");
            cc.losBlocked = cc.LoadBitmap("los_block");
            cc.black_tile = cc.LoadBitmap("black_tile");
            //cc.black_tile2 = cc.LoadBitmap("black_tile2");
            cc.turn_marker = cc.LoadBitmap("turn_marker");
            cc.pc_dead = cc.LoadBitmap("pc_dead");
            cc.pc_stealth = cc.LoadBitmap("pc_stealth");
            //cc.offScreen = cc.LoadBitmap("offScreen");
            //cc.offScreen5 = cc.LoadBitmap("offScreen5");
            //cc.offScreen6 = cc.LoadBitmap("offScreen6");
            //cc.offScreen7 = cc.LoadBitmap("offScreen7");
            //cc.offScreenTrans = cc.LoadBitmap("offScreenTrans");
            //cc.death_fx = cc.LoadBitmap("death_fx");
            cc.hitSymbol = cc.LoadBitmap("hit_symbol");
            cc.missSymbol = cc.LoadBitmap("miss_symbol");
            cc.highlight_green = cc.LoadBitmap("highlight_green");
            cc.highlight_red = cc.LoadBitmap("highlight_red");
            cc.tint_dawn = cc.LoadBitmap("tint_dawn");
            cc.tint_sunrise = cc.LoadBitmap("tint_sunrise");
            cc.tint_sunset = cc.LoadBitmap("tint_sunset");
            cc.tint_dusk = cc.LoadBitmap("tint_dusk");
            cc.tint_night = cc.LoadBitmap("tint_night");
            //off for now
            //cc.tint_rain = cc.LoadBitmap("tint_rain");
            cc.ui_portrait_frame = cc.LoadBitmap("ui_portrait_frame");
            cc.ui_bg_fullscreen = cc.LoadBitmap("ui_bg_fullscreen");
            cc.facing1 = cc.LoadBitmap("facing1");
            cc.facing2 = cc.LoadBitmap("facing2");
            cc.facing3 = cc.LoadBitmap("facing3");
            cc.facing4 = cc.LoadBitmap("facing4");
            cc.facing6 = cc.LoadBitmap("facing6");
            cc.facing7 = cc.LoadBitmap("facing7");
            cc.facing8 = cc.LoadBitmap("facing8");
            cc.facing9 = cc.LoadBitmap("facing9");
        }	
	    public void resetGame()
	    {
		    mod = cc.LoadModule(mod.moduleName + ".mod", false);
            //reset log number of lines based on the value from the Module's mod file
            log.numberOfLinesToShow = mod.logNumberOfLines;            
                        
		    mod.debugMode = false;
		    mod.setCurrentArea(mod.startingArea, this);
		    mod.PlayerLocationX = mod.startingPlayerPositionX;
		    mod.PlayerLocationY = mod.startingPlayerPositionY;
		    LoadStandardImages();
		    	
		    foreach (Container c in mod.moduleContainersList)
            {
                c.initialContainerItemRefs.Clear();
                foreach (ItemRefs i in c.containerItemRefs)
                {
                    c.initialContainerItemRefs.Add(i.DeepCopy());
                }
            }
            foreach (Shop s in mod.moduleShopsList)
            {
                s.initialShopItemRefs.Clear();
                foreach (ItemRefs i in s.shopItemRefs)
                {
                    s.initialShopItemRefs.Add(i.DeepCopy());
                }
            }
            foreach (Area a in mod.moduleAreasObjects)
            {
                a.InitialAreaPropTagsList.Clear();
                foreach (Prop p in a.Props)
                {
                    a.InitialAreaPropTagsList.Add(p.PropTag);
                }            
            }
        
		    cc.nullOutControls();
            cc.setControlsStart();
            
		    createScreens();
		    initializeSounds();
		
		    cc.LoadTestParty();
		
		    //load all the message box helps/tutorials
		    cc.stringBeginnersGuide = cc.loadTextToString("MessageBeginnersGuide.txt");
		    cc.stringPlayersGuide = cc.loadTextToString("MessagePlayersGuide.txt");
		    cc.stringPcCreation = cc.loadTextToString("MessagePcCreation.txt");
		    cc.stringMessageCombat = cc.loadTextToString("MessageCombat.txt");
		    cc.stringMessageInventory = cc.loadTextToString("MessageInventory.txt");
		    cc.stringMessageParty = cc.loadTextToString("MessageParty.txt");
		    cc.stringMessageMainMap = cc.loadTextToString("MessageMainMap.txt");
	    }

        private void fillCharList()
        {
            charList.Add('A', new SharpDX.RectangleF(fontWidth * 0, fontHeight * 0, fontWidth, fontHeight));
            charList.Add('B', new SharpDX.RectangleF(fontWidth * 1, fontHeight * 0, fontWidth, fontHeight));
            charList.Add('C', new SharpDX.RectangleF(fontWidth * 2, fontHeight * 0, fontWidth, fontHeight));
            charList.Add('D', new SharpDX.RectangleF(fontWidth * 3, fontHeight * 0, fontWidth, fontHeight));
            charList.Add('E', new SharpDX.RectangleF(fontWidth * 4, fontHeight * 0, fontWidth, fontHeight));
            charList.Add('F', new SharpDX.RectangleF(fontWidth * 5, fontHeight * 0, fontWidth, fontHeight));
            charList.Add('G', new SharpDX.RectangleF(fontWidth * 6, fontHeight * 0, fontWidth, fontHeight));
            charList.Add('H', new SharpDX.RectangleF(fontWidth * 7, fontHeight * 0, fontWidth, fontHeight));
            charList.Add('I', new SharpDX.RectangleF(fontWidth * 8, fontHeight * 0, fontWidth, fontHeight));
            charList.Add('J', new SharpDX.RectangleF(fontWidth * 9, fontHeight * 0, fontWidth, fontHeight));

            charList.Add('K', new SharpDX.RectangleF(fontWidth * 0, fontHeight * 1, fontWidth, fontHeight));
            charList.Add('L', new SharpDX.RectangleF(fontWidth * 1, fontHeight * 1, fontWidth, fontHeight));
            charList.Add('M', new SharpDX.RectangleF(fontWidth * 2, fontHeight * 1, fontWidth, fontHeight));
            charList.Add('N', new SharpDX.RectangleF(fontWidth * 3, fontHeight * 1, fontWidth, fontHeight));
            charList.Add('O', new SharpDX.RectangleF(fontWidth * 4, fontHeight * 1, fontWidth, fontHeight));
            charList.Add('P', new SharpDX.RectangleF(fontWidth * 5, fontHeight * 1, fontWidth, fontHeight));
            charList.Add('Q', new SharpDX.RectangleF(fontWidth * 6, fontHeight * 1, fontWidth, fontHeight));
            charList.Add('R', new SharpDX.RectangleF(fontWidth * 7, fontHeight * 1, fontWidth, fontHeight));
            charList.Add('S', new SharpDX.RectangleF(fontWidth * 8, fontHeight * 1, fontWidth, fontHeight));
            charList.Add('T', new SharpDX.RectangleF(fontWidth * 9, fontHeight * 1, fontWidth, fontHeight));

            charList.Add('U', new SharpDX.RectangleF(fontWidth * 0, fontHeight * 2, fontWidth, fontHeight));
            charList.Add('V', new SharpDX.RectangleF(fontWidth * 1, fontHeight * 2, fontWidth, fontHeight));
            charList.Add('W', new SharpDX.RectangleF(fontWidth * 2, fontHeight * 2, fontWidth, fontHeight));
            charList.Add('X', new SharpDX.RectangleF(fontWidth * 3, fontHeight * 2, fontWidth, fontHeight));
            charList.Add('Y', new SharpDX.RectangleF(fontWidth * 4, fontHeight * 2, fontWidth, fontHeight));
            charList.Add('Z', new SharpDX.RectangleF(fontWidth * 5, fontHeight * 2, fontWidth, fontHeight));
            charList.Add('a', new SharpDX.RectangleF(fontWidth * 6, fontHeight * 2, fontWidth, fontHeight));
            charList.Add('b', new SharpDX.RectangleF(fontWidth * 7, fontHeight * 2, fontWidth, fontHeight));
            charList.Add('c', new SharpDX.RectangleF(fontWidth * 8, fontHeight * 2, fontWidth, fontHeight));
            charList.Add('d', new SharpDX.RectangleF(fontWidth * 9, fontHeight * 2, fontWidth, fontHeight));

            charList.Add('e', new SharpDX.RectangleF(fontWidth * 0, fontHeight * 3, fontWidth, fontHeight));
            charList.Add('f', new SharpDX.RectangleF(fontWidth * 1, fontHeight * 3, fontWidth, fontHeight));
            charList.Add('g', new SharpDX.RectangleF(fontWidth * 2, fontHeight * 3, fontWidth, fontHeight));
            charList.Add('h', new SharpDX.RectangleF(fontWidth * 3, fontHeight * 3, fontWidth, fontHeight));
            charList.Add('i', new SharpDX.RectangleF(fontWidth * 4, fontHeight * 3, fontWidth, fontHeight));
            charList.Add('j', new SharpDX.RectangleF(fontWidth * 5, fontHeight * 3, fontWidth, fontHeight));
            charList.Add('k', new SharpDX.RectangleF(fontWidth * 6, fontHeight * 3, fontWidth, fontHeight));
            charList.Add('l', new SharpDX.RectangleF(fontWidth * 7, fontHeight * 3, fontWidth, fontHeight));
            charList.Add('m', new SharpDX.RectangleF(fontWidth * 8, fontHeight * 3, fontWidth, fontHeight));
            charList.Add('n', new SharpDX.RectangleF(fontWidth * 9, fontHeight * 3, fontWidth, fontHeight));

            charList.Add('o', new SharpDX.RectangleF(fontWidth * 0, fontHeight * 4, fontWidth, fontHeight));
            charList.Add('p', new SharpDX.RectangleF(fontWidth * 1, fontHeight * 4, fontWidth, fontHeight));
            charList.Add('q', new SharpDX.RectangleF(fontWidth * 2, fontHeight * 4, fontWidth, fontHeight));
            charList.Add('r', new SharpDX.RectangleF(fontWidth * 3, fontHeight * 4, fontWidth, fontHeight));
            charList.Add('s', new SharpDX.RectangleF(fontWidth * 4, fontHeight * 4, fontWidth, fontHeight));
            charList.Add('t', new SharpDX.RectangleF(fontWidth * 5, fontHeight * 4, fontWidth, fontHeight));
            charList.Add('u', new SharpDX.RectangleF(fontWidth * 6, fontHeight * 4, fontWidth, fontHeight));
            charList.Add('v', new SharpDX.RectangleF(fontWidth * 7, fontHeight * 4, fontWidth, fontHeight));
            charList.Add('w', new SharpDX.RectangleF(fontWidth * 8, fontHeight * 4, fontWidth, fontHeight));
            charList.Add('x', new SharpDX.RectangleF(fontWidth * 9, fontHeight * 4, fontWidth, fontHeight));

            charList.Add('y', new SharpDX.RectangleF(fontWidth * 0, fontHeight * 5, fontWidth, fontHeight));
            charList.Add('z', new SharpDX.RectangleF(fontWidth * 1, fontHeight * 5, fontWidth, fontHeight));
            charList.Add('0', new SharpDX.RectangleF(fontWidth * 2, fontHeight * 5, fontWidth, fontHeight));
            charList.Add('1', new SharpDX.RectangleF(fontWidth * 3, fontHeight * 5, fontWidth, fontHeight));
            charList.Add('2', new SharpDX.RectangleF(fontWidth * 4, fontHeight * 5, fontWidth, fontHeight));
            charList.Add('3', new SharpDX.RectangleF(fontWidth * 5, fontHeight * 5, fontWidth, fontHeight));
            charList.Add('4', new SharpDX.RectangleF(fontWidth * 6, fontHeight * 5, fontWidth, fontHeight));
            charList.Add('5', new SharpDX.RectangleF(fontWidth * 7, fontHeight * 5, fontWidth, fontHeight));
            charList.Add('6', new SharpDX.RectangleF(fontWidth * 8, fontHeight * 5, fontWidth, fontHeight));
            charList.Add('7', new SharpDX.RectangleF(fontWidth * 9, fontHeight * 5, fontWidth, fontHeight));

            charList.Add('8', new SharpDX.RectangleF(fontWidth * 0, fontHeight * 6, fontWidth, fontHeight));
            charList.Add('9', new SharpDX.RectangleF(fontWidth * 1, fontHeight * 6, fontWidth, fontHeight));
            charList.Add('.', new SharpDX.RectangleF(fontWidth * 2, fontHeight * 6, fontWidth, fontHeight));
            charList.Add(',', new SharpDX.RectangleF(fontWidth * 3, fontHeight * 6, fontWidth, fontHeight));
            charList.Add('"', new SharpDX.RectangleF(fontWidth * 4, fontHeight * 6, fontWidth, fontHeight));
            charList.Add('\'', new SharpDX.RectangleF(fontWidth * 5, fontHeight * 6, fontWidth, fontHeight));
            charList.Add('?', new SharpDX.RectangleF(fontWidth * 6, fontHeight * 6, fontWidth, fontHeight));
            charList.Add('!', new SharpDX.RectangleF(fontWidth * 7, fontHeight * 6, fontWidth, fontHeight));
            charList.Add('~', new SharpDX.RectangleF(fontWidth * 8, fontHeight * 6, fontWidth, fontHeight));
            charList.Add('#', new SharpDX.RectangleF(fontWidth * 9, fontHeight * 6, fontWidth, fontHeight));

            charList.Add('$', new SharpDX.RectangleF(fontWidth * 0, fontHeight * 7, fontWidth, fontHeight));
            charList.Add('%', new SharpDX.RectangleF(fontWidth * 1, fontHeight * 7, fontWidth, fontHeight));
            charList.Add('^', new SharpDX.RectangleF(fontWidth * 2, fontHeight * 7, fontWidth, fontHeight));
            charList.Add('&', new SharpDX.RectangleF(fontWidth * 3, fontHeight * 7, fontWidth, fontHeight));
            charList.Add('*', new SharpDX.RectangleF(fontWidth * 4, fontHeight * 7, fontWidth, fontHeight));
            charList.Add('(', new SharpDX.RectangleF(fontWidth * 5, fontHeight * 7, fontWidth, fontHeight));
            charList.Add(')', new SharpDX.RectangleF(fontWidth * 6, fontHeight * 7, fontWidth, fontHeight));
            charList.Add('-', new SharpDX.RectangleF(fontWidth * 7, fontHeight * 7, fontWidth, fontHeight));
            charList.Add('_', new SharpDX.RectangleF(fontWidth * 8, fontHeight * 7, fontWidth, fontHeight));
            charList.Add('+', new SharpDX.RectangleF(fontWidth * 9, fontHeight * 7, fontWidth, fontHeight));

            charList.Add('=', new SharpDX.RectangleF(fontWidth * 0, fontHeight * 8, fontWidth, fontHeight));
            charList.Add('[', new SharpDX.RectangleF(fontWidth * 1, fontHeight * 8, fontWidth, fontHeight));
            charList.Add(']', new SharpDX.RectangleF(fontWidth * 2, fontHeight * 8, fontWidth, fontHeight));
            charList.Add('/', new SharpDX.RectangleF(fontWidth * 3, fontHeight * 8, fontWidth, fontHeight));
            charList.Add(':', new SharpDX.RectangleF(fontWidth * 4, fontHeight * 8, fontWidth, fontHeight));
            charList.Add('|', new SharpDX.RectangleF(fontWidth * 4, fontHeight * 8, fontWidth, fontHeight));
            charList.Add(';', new SharpDX.RectangleF(fontWidth * 5, fontHeight * 8, fontWidth, fontHeight));
            charList.Add('<', new SharpDX.RectangleF(fontWidth * 6, fontHeight * 8, fontWidth, fontHeight));
            charList.Add('>', new SharpDX.RectangleF(fontWidth * 7, fontHeight * 8, fontWidth, fontHeight));
            //charList.Add('/', new SharpDX.RectangleF(64, 64, 8, 12));
            charList.Add(' ', new SharpDX.RectangleF(fontWidth * 9, fontHeight * 8, fontWidth, fontHeight));
        }
                
        public void initializeSounds()
	    {
            oSoundStreams.Clear();
            string jobDir = "";
            jobDir = this.mainDirectory + "\\default\\NewModule\\sounds";
            foreach (string f in Directory.GetFiles(jobDir, "*.*", SearchOption.AllDirectories))
            {
                oSoundStreams.Add(Path.GetFileNameWithoutExtension(f), File.OpenRead(Path.GetFullPath(f)));
            }
	    }
	    public void PlaySound(string filenameNoExtension)
	    {            
            if ((filenameNoExtension.Equals("none")) || (filenameNoExtension.Equals("")) || (!mod.playSoundFx))
            {
                //play nothing
                return;
            }
            else
            {
                try
                {
                    soundPlayer.Stream = oSoundStreams[filenameNoExtension];
                    soundPlayer.Play();
                }
                catch (Exception ex)
                {
                    errorLog(ex.ToString());
                    if (mod.debugMode) //SD_20131102
                    {
                        cc.addLogText("<yl>failed to play sound" + filenameNoExtension + "</yl><BR>");
                    }
                    initializeSounds();
                }
            }            
	    }

        //Animation Timer Stuff
        public void postDelayed(string type, int delay)
        {
            if (type.Equals("doAnimation"))
            {
                animationTimer.Enabled = true;
                if (delay < 1)
                {
                    delay = 1;
                }
                animationTimer.Interval = delay;
                animationTimer.Start();
            }
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationTimer.Enabled = false;
            animationTimer.Stop();
            screenCombat.doAnimationController();            
        }
        
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (!stillProcessingGameLoop)
            {
                stillProcessingGameLoop = true; //starting the game loop so do not allow another tick call to run until finished with this tick call.
                long current = gameTimerStopwatch.ElapsedMilliseconds; //get the current total amount of ms since the game launched
                int elapsed = (int)(current - previousTime); //calculate the total ms elapsed since the last time through the game loop
                Update(elapsed); //runs AI and physics
                Render(); //draw the screen frame
                if (reportFPScount >= 10)
                {
                    reportFPScount = 0;
                    fps = 1000 / (current - previousTime);
                }
                reportFPScount++;
                previousTime = current; //remember the current time at the beginning of this tick call for the next time through the game loop to calculate elapsed time
                stillProcessingGameLoop = false; //finished game loop so okay to let the next tick call enter the game loop      
            }  
        }
        private void Update(int elapsed)
        {
            //iterate through spriteList and handle any sprite location and animation frame calculations
            if (screenType.Equals("main"))
            {
                screenMainMap.Update(elapsed);
            }
            else if (screenType.Equals("combat"))
            {
                screenCombat.Update(elapsed);
            }
        }

        //DRAW ROUTINES
        public void DrawText(string text, float xLoc, float yLoc, string color)
        {
            //default is WHITE
            SharpDX.Direct2D1.Bitmap bm = cc.GetFromBitmapList("fontWh.png");
            if (color.Equals("bk"))
            {
                bm = cc.GetFromBitmapList("fontBk.png");
            }
            else if (color.Equals("bu"))
            {
                bm = cc.GetFromBitmapList("fontBu.png");
            }
            else if (color.Equals("gn"))
            {
                bm = cc.GetFromBitmapList("fontGn.png");
            }
            else if (color.Equals("gy"))
            {
                bm = cc.GetFromBitmapList("fontGy.png");
            }
            else if (color.Equals("ma"))
            {
                bm = cc.GetFromBitmapList("fontMa.png");
            }
            else if (color.Equals("rd"))
            {
                bm = cc.GetFromBitmapList("fontRd.png");
            }
            else if (color.Equals("yl"))
            {
                bm = cc.GetFromBitmapList("fontYl.png");
            }

            float x = 0;
            foreach (char c in text)
            {
                if (c == '\r') { continue; }
                if (c == '\n') { continue; }
                char c1 = '0';
                if (!charList.ContainsKey(c)) { c1 = '#'; }
                else c1 = c;
                DrawD2DBitmap(bm, charList[c1], new SharpDX.RectangleF(xLoc + x, yLoc, fontWidth, fontHeight), 0.0f, false, 1.0f, 0, 0, 0, 0, true);
                x += fontWidth + fontCharSpacing;
            }
        }
        public void DrawRectangle(IbRect rect, SharpDX.Color penColor, int penWidth)
        {
            SharpDX.RectangleF r = new SharpDX.RectangleF(rect.Left + oXshift, rect.Top + oYshift, rect.Width, rect.Height);
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, penColor))
            {
                renderTarget2D.DrawRectangle(r, scb, penWidth);
            }
        }
        public void DrawLine(int lastX, int lastY, int nextX, int nextY, SharpDX.Color penColor, int penWidth)
        {
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, penColor))
            {
                renderTarget2D.DrawLine(new Vector2(lastX + oXshift, lastY + oYshift), new Vector2(nextX + oXshift, nextY + oYshift), scb, penWidth);
            }
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, bool mirror)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, mirror);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, int angleInDegrees, bool mirror)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInDegrees, mirror);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInRadians, mirror, 1.0f, 0, 0, 0, 0, false);
        }

        //DIRECT2D STUFF
        public void InitializeRenderer()
        {
            string state = "";
            try
            {                
                // SwapChain description
                state += "Creating Swap Chain:";
                var desc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription =
                        new ModeDescription(this.Width, this.Height,
                                            new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    OutputHandle = this.Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SwapEffect.Discard,
                    Usage = Usage.RenderTargetOutput
                };

                // Create Device and SwapChain                
                state += "Get Highest Feature Level:";
                var featureLvl = SharpDX.Direct3D11.Device.GetSupportedFeatureLevel();
                state += " Highest Feature Level is: " + featureLvl.ToString() + " :Create Device:";
                try
                {
                    SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, new[] { featureLvl }, desc, out _device, out _swapChain);
                }
                catch (Exception ex)
                {
                    this.errorLog(state + "<--->" + ex.ToString());
                    MessageBox.Show("Failed on Create Device using a feature level of " + featureLvl.ToString() + ". Will try using feature level 'Level_9_1' and DriverType.Software instead of DriverType.Hardware");
                    SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Software, DeviceCreationFlags.BgraSupport, new[] { SharpDX.Direct3D.FeatureLevel.Level_9_1 }, desc, out _device, out _swapChain);                    
                }

                if (_device == null)
                {
                    MessageBox.Show("Failed to create a device, closing IceBlink 2. Please send us your 'IB2ErrorLog.txt' file for more debugging help.");
                    Application.Exit();
                }

                // Ignore all windows events
                state += "Create Factory:";
                SharpDX.DXGI.Factory factory = _swapChain.GetParent<SharpDX.DXGI.Factory>();
                factory.MakeWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAll);

                // New RenderTargetView from the backbuffer
                state += "Creating Back Buffer:";
                _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
                
                state += "Create RenderTargetView:";
                _backBufferView = new RenderTargetView(_device, _backBuffer);
                
                factory2D = new SharpDX.Direct2D1.Factory();
                using (var surface = _backBuffer.QueryInterface<Surface>())
                {
                    renderTarget2D = new RenderTarget(factory2D, surface, new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
                }
                renderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;

                //TEXT STUFF
                state += "Creating Text Factory:";
                factoryDWrite = new SharpDX.DirectWrite.Factory();
                sceneColorBrush = new SolidColorBrush(renderTarget2D, SharpDX.Color.Blue);
                renderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;
            }
            catch (SharpDXException ex)
            {
                MessageBox.Show("SharpDX error message appended to IB2ErrorLog.txt");
                this.errorLog(state + "<--->" + ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("SharpDX error message appended to IB2ErrorLog.txt");
                this.errorLog(state + "<--->" + ex.ToString());
            }
        }
        public void BeginDraw()
        {
            _device.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, this.Width, this.Height));
            _device.ImmediateContext.OutputMerger.SetTargets(_backBufferView);
            renderTarget2D.BeginDraw();
        }        
        public void EndDraw()
        {
            renderTarget2D.EndDraw();
            _swapChain.Present(1, PresentFlags.None);
        }
        public void Render()
        {
            BeginDraw(); //uncomment this for DIRECT2D ADDITIONS  
          
            renderTarget2D.Clear(Color4.Black); //uncomment this for DIRECT2D ADDITIONS

            if ((mod.useUIBackground) && (!screenType.Equals("main")) && (!screenType.Equals("combat")) && (!screenType.Equals("launcher")) && (!screenType.Equals("title")))
            {
                drawUIBackground();
            }            
            if (screenType.Equals("title"))
            {
                screenTitle.redrawTitle();
            }
            else if (screenType.Equals("launcher"))
            {
                screenLauncher.redrawLauncher();
            }
            else if (screenType.Equals("pcCreation"))
            {
                screenPcCreation.redrawPcCreation();
            }
            else if (screenType.Equals("learnSpellCreation"))
            {
                screenSpellLevelUp.redrawSpellLevelUp(true);
            }
            else if (screenType.Equals("learnSpellLevelUp"))
            {
                screenSpellLevelUp.redrawSpellLevelUp(false);
            }
            else if (screenType.Equals("learnTraitCreation"))
            {
                screenTraitLevelUp.redrawTraitLevelUp(true);
            }
            else if (screenType.Equals("learnTraitLevelUp"))
            {
                screenTraitLevelUp.redrawTraitLevelUp(false);
            }
            else if (screenType.Equals("main"))
            {
                screenMainMap.redrawMain();
            }
            else if (screenType.Equals("party"))
            {
                screenParty.redrawParty();
            }
            else if (screenType.Equals("combatParty"))
            {
                screenParty.redrawParty();
            }
            else if (screenType.Equals("inventory"))
            {
                screenInventory.redrawInventory();
            }
            else if (screenType.Equals("itemSelector"))
            {
                screenItemSelector.redrawItemSelector();
            }
            else if (screenType.Equals("portraitSelector"))
            {
                screenPortraitSelector.redrawPortraitSelector();
            }
            else if (screenType.Equals("tokenSelector"))
            {
                screenTokenSelector.redrawTokenSelector();
            }
            else if (screenType.Equals("pcSelector"))
            {
                screenPcSelector.redrawPcSelector();
            }
            else if (screenType.Equals("combatInventory"))
            {
                screenInventory.redrawInventory();
            }
            else if (screenType.Equals("journal"))
            {
                screenJournal.redrawJournal();
            }
            else if (screenType.Equals("shop"))
            {
                screenShop.redrawShop();
            }
            else if (screenType.Equals("combat"))
            {
                screenCombat.redrawCombat();
            }
            else if (screenType.Equals("combatCast"))
            {
                screenCastSelector.redrawCastSelector(true);
            }
            else if (screenType.Equals("mainMapCast"))
            {
                screenCastSelector.redrawCastSelector(false);
            }
            else if (screenType.Equals("convo"))
            {
                screenConvo.redrawConvo();
            }
            else if (screenType.Equals("partyBuild"))
            {
                screenPartyBuild.redrawPartyBuild();
            }
            else if (screenType.Equals("partyRoster"))
            {
                screenPartyRoster.redrawPartyRoster();
            }
            if (mod.debugMode)
            {
                int txtH = (int)fontHeight;
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 2; y++)
                    {
                        DrawText("FPS:" + fps.ToString(), x + 5, screenHeight - txtH - 5 + y, "bk");
                    }
                }
                DrawText("FPS:" + fps.ToString(), 5, screenHeight - txtH - 5, "wh");
            }
            if (itemListSelector.showIBminiItemListSelector)
            {
                itemListSelector.drawItemListSelection();
            }
            EndDraw(); //uncomment this for DIRECT2D ADDITIONS
        }
        public void drawUIBackground()
        {
            try
            {
                IbRect src = new IbRect(0, 0, cc.ui_bg_fullscreen.PixelSize.Width, cc.ui_bg_fullscreen.PixelSize.Height);
                IbRect dst = new IbRect(0 - oXshift, 0 - oYshift, screenWidth, screenHeight);
                DrawBitmap(cc.ui_bg_fullscreen, src, dst);
            }
            catch
            { }
        }

        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target)
        {
            DrawD2DBitmap(bitmap, source, target, false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, bool mirror)
        {
            DrawD2DBitmap(bitmap, source, target, 0.0f, mirror, 1.0f , 0, 0, 0, 0, false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, int angleInDegrees, bool mirror)
        {
            //convert degrees to radians
            float angleInRadians = (float)(Math.PI * 2 * (float)angleInDegrees / (float)360);
            DrawD2DBitmap(bitmap, source, target, angleInRadians, mirror, 1.0f, 0, 0, 0, 0, false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, float angleInRadians, bool mirror, float opac, int Xshift, int Yshift, int Xscale, int Yscale, bool NearestNeighbourInterpolation)
        {
            int mir = 1;
            if (mirror) { mir = -1; }
            float xshf = (float)Xshift * 2 * screenDensity;
            float yshf = (float)Yshift * 2 * screenDensity;
            float xscl = 1f + (((float)Xscale * 2 * screenDensity) / squareSize);
            float yscl = 1f + (((float)Yscale * 2 * screenDensity) / squareSize);

            Vector2 center = new Vector2((target.Left + oXshift) + (target.Width / 2), (target.Top + oYshift) + (target.Height / 2));
            renderTarget2D.Transform = SharpDX.Matrix.Transformation2D(center, 0, new Vector2(mir * xscl, yscl), center, angleInRadians, new Vector2(xshf, yshf));
            SharpDX.RectangleF trg = new SharpDX.RectangleF(target.Left + oXshift, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);

            if (NearestNeighbourInterpolation)
            {
                renderTarget2D.DrawBitmap(bitmap, trg, opac, BitmapInterpolationMode.NearestNeighbor, src);
            }
            else
            {
                renderTarget2D.DrawBitmap(bitmap, trg, opac, BitmapInterpolationMode.NearestNeighbor, src);
            }            
            renderTarget2D.Transform = Matrix3x2.Identity;
        }

        //INPUT STUFF
        public bool formMoveable = false;
        public System.Drawing.Point currentPosition;
        public System.Drawing.Point startPosition;

        private void GameView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (showMessageBox)
            {
                messageBox.onMouseWheel(sender, e);
            }
            else if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                log.onMouseWheel(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseWheel);
        }
        private void GameView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Y < 15)
            {
                Cursor.Current = Cursors.NoMove2D;
                formMoveable = true;
                startPosition.X = e.X;
                startPosition.Y = e.Y;
                return;
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseDown);
        }
        private void GameView_MouseUp(object sender, MouseEventArgs e)
        {
            formMoveable = false;
            Cursor.Current = Cursors.Default;
            onMouseEvent(sender, e, MouseEventType.EventType.MouseUp);
        }
        private void GameView_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Y < 15) || (formMoveable))
            {
                Cursor.Current = Cursors.NoMove2D;
            }
            if (formMoveable)
            {
                System.Drawing.Point newPosition = this.Location;
                currentPosition.X = e.X;
                currentPosition.Y = e.Y;
                newPosition.X = newPosition.X - (startPosition.X - currentPosition.X); // .Offset(mouseOffset.X, mouseOffset.Y);                
                newPosition.Y = newPosition.Y - (startPosition.Y - currentPosition.Y);
                this.Location = newPosition;
                return;
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseMove);
        }
        private void GameView_MouseClick(object sender, MouseEventArgs e)
        {
            onMouseEvent(sender, e, MouseEventType.EventType.MouseClick);
        }
        public void onMouseEvent(object sender, MouseEventArgs e, MouseEventType.EventType eventType)
        {
            try 
            {
                int eX = e.X - oXshift;
                int eY = e.Y - oYshift;
                //do only itemListSelector if visible
                if (itemListSelector.showIBminiItemListSelector)
                {
                    itemListSelector.onTouchItemListSelection(eX, eY, e, eventType);
                    return;
                }
                if (touchEnabled)
                {
                    if (screenType.Equals("main"))
                    {
                        screenMainMap.onTouchMain(eX, eY, e, eventType);	
                    }
                    else if (screenType.Equals("launcher"))
                    {
                        screenLauncher.onTouchLauncher(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("pcCreation"))
                    {
                        screenPcCreation.onTouchPcCreation(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("learnSpellCreation"))
                    {
                        screenSpellLevelUp.onTouchSpellLevelUp(eX, eY, e, eventType, true);   	
                    }
                    else if (screenType.Equals("learnSpellLevelUp"))
                    {
                        screenSpellLevelUp.onTouchSpellLevelUp(eX, eY, e, eventType, false);     	
                    }
                    else if (screenType.Equals("learnTraitCreation"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(eX, eY, e, eventType, true);   	
                    }
                    else if (screenType.Equals("learnTraitLevelUp"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(eX, eY, e, eventType, false);     	
                    }
                    else if (screenType.Equals("title"))
                    {
                        screenTitle.onTouchTitle(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("party"))
                    {
                        screenParty.onTouchParty(eX, eY, e, eventType, false);
                    }
                    else if (screenType.Equals("combatParty"))
                    {
                        screenParty.onTouchParty(eX, eY, e, eventType, true);
                    }
                    else if (screenType.Equals("inventory"))
                    {
                        screenInventory.onTouchInventory(eX, eY, e, eventType, false);
                    }
                    else if (screenType.Equals("combatInventory"))
                    {
                        screenInventory.onTouchInventory(eX, eY, e, eventType, true);
                    }
                    else if (screenType.Equals("itemSelector"))
                    {
                        screenItemSelector.onTouchItemSelector(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("portraitSelector"))
                    {
                        screenPortraitSelector.onTouchPortraitSelector(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("tokenSelector"))
                    {
                        screenTokenSelector.onTouchTokenSelector(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("pcSelector"))
                    {
                        screenPcSelector.onTouchPcSelector(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("journal"))
                    {
                        screenJournal.onTouchJournal(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("shop"))
                    {
                        screenShop.onTouchShop(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("combat"))
                    {
                        screenCombat.onTouchCombat(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("combatCast"))
                    {
                        screenCastSelector.onTouchCastSelector(eX, eY, e, eventType, true);
                    }
                    else if (screenType.Equals("mainMapCast"))
                    {
                        screenCastSelector.onTouchCastSelector(eX, eY, e, eventType, false);
                    }
                    else if (screenType.Equals("convo"))
                    {
                        screenConvo.onTouchConvo(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("partyBuild"))
                    {
                        screenPartyBuild.onTouchPartyBuild(eX, eY, e, eventType);
                    }
                    else if (screenType.Equals("partyRoster"))
                    {
                        screenPartyRoster.onTouchPartyRoster(eX, eY, e, eventType);
                    }
                }
            }
            catch (Exception ex) 
            {
                errorLog(ex.ToString());   		
            }		
        }

        public void onKeyboardEvent(Keys keyData)
        {
            try
            {
                if (keyData == Keys.Escape)
                {
                    if (showMessageBox)
                    {
                        showMessageBox = false;
                        return;
                    }
                    if (itemListSelector.showIBminiItemListSelector)
                    {
                        itemListSelector.showIBminiItemListSelector = false;
                        return;
                    }
                    doVerifyClosingSetup();                    
                }
                if (touchEnabled)
                {
                    if (keyData == Keys.H)
                    {
                        if (showHotKeys) { showHotKeys = false; }
                        else { showHotKeys = true; }
                    }
                    if (screenType.Equals("main"))
                    {
                        screenMainMap.onKeyUp(keyData);
                    }
                    else if (screenType.Equals("combat"))
                    {
                        screenCombat.onKeyUp(keyData);
                    }
                    else if (screenType.Equals("convo"))
                    {
                        screenConvo.onKeyUp(keyData);
                    }
                }
            }
            catch (Exception ex)
            {
                errorLog(ex.ToString());
            }
        }        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            onKeyboardEvent(keyData);
                
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //ON FORM CLOSING
        public void doVerifyClosingSetup()
        {
            List<string> actionList = new List<string> { "Yes, Exit", "No, Keep Playing" };
            itemListSelector.setupIBminiItemListSelector(this, actionList, "Are you sure you wish to exit?", "verifyclosing");
            itemListSelector.showIBminiItemListSelector = true;
        }
        public void doVerifyClosing(int selectedIndex)
        {
            if (selectedIndex == 0)
            {
                this.Close();
            }
            if (selectedIndex == 1)
            {
                //keep playing
            }
        }
        private void GameView_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        public void errorLog(string text)
        {
            if (mainDirectory == null) 
            { 
                mainDirectory = Directory.GetCurrentDirectory(); 
            }
            using (StreamWriter writer = new StreamWriter(mainDirectory + "//IBminiErrorLog.txt", true))
            {
                writer.Write(DateTime.Now + ": ");
                writer.WriteLine(text);
            }
        }
    }
}