using System;
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
    public class ScreenMainMap
    {
        //public Module gv.mod;
        public GameView gv;

        public IB2UILayout mainUiLayout = null;
        public bool showMiniMap = false;
        public bool showClock = false;
        public bool showFullParty = false;
        public bool showArrows = true;
        public bool showTogglePanel = true;
        public bool hideClock = false;
        public List<FloatyText> floatyTextPool = new List<FloatyText>();
        public List<FloatyTextByPixel> floatyTextByPixelPool = new List<FloatyTextByPixel>();
        public IBminiTextBox floatyTextBox;
        public int mapStartLocXinPixels;
        public int movementDelayInMiliseconds = 100;
        private long timeStamp = 0;
        private bool finishedMove = true;
        public Bitmap minimap = null;
        public List<Sprite> spriteList = new List<Sprite>();
        public float sqrScale = 1.0f; //1.0 or 0.6364
        public bool use11x11 = false;

        public ScreenMainMap(Module m, GameView g)
        {
            //gv.mod = m;
            gv = g;
            mapStartLocXinPixels = 1 * gv.squareSize;
            loadMainUILayout();
            floatyTextBox = new IBminiTextBox(gv);
            floatyTextBox.showShadow = true;
        }
        public void loadMainUILayout()
        {
            try
            {
                mainUiLayout = new IB2UILayout(gv);
                createLogPanel();
                createButtonsPanel();
                createTogglesPanel();
                createPortraitsPanel();
                createArrowsPanel();
                mainUiLayout.setupIB2UILayout(gv);

                IB2ToggleButton tgl = mainUiLayout.GetToggleByTag("tglMiniMap");
                if (tgl != null)
                {
                    showMiniMap = tgl.toggleOn;
                }
                IB2ToggleButton tgl2 = mainUiLayout.GetToggleByTag("tglClock");
                if (tgl2 != null)
                {
                    showClock = tgl2.toggleOn;
                }
                IB2ToggleButton tgl3 = mainUiLayout.GetToggleByTag("tglFullParty");
                if (tgl3 != null)
                {
                    showFullParty = tgl3.toggleOn;
                }
                float sqrW = (float)gv.screenWidth / (gv.squaresInWidth);
                float sqrH = (float)gv.screenHeight / (gv.squaresInHeight);
                gv.cc.addLogText("red", "screenDensity: " + gv.screenDensity);
                gv.cc.addLogText("fuchsia", "screenWidth: " + gv.screenWidth);
                gv.cc.addLogText("lime", "screenHeight: " + gv.screenHeight);
                gv.cc.addLogText("yellow", "squareSize: " + gv.squareSize);
                gv.cc.addLogText("yellow", "sqrW: " + sqrW);
                gv.cc.addLogText("yellow", "sqrH: " + sqrH);
                gv.cc.addLogText("yellow", "fontWidth: " + gv.fontWidth);
                gv.cc.addLogText("yellow", "");
                gv.cc.addLogText("red", "Welcome to " + gv.mod.moduleLabelName);
                gv.cc.addLogText("fuchsia", "Swipe up/down to scroll this message log box");
                
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error Loading MainUILayout.json: " + ex.ToString());
                //gv.errorLog(ex.ToString());
            }
        }
        public void createLogPanel()
        {
            //create log panel
            IB2Panel newPanel = new IB2Panel(gv);
            newPanel.tag = "logPanel";
            newPanel.backgroundImageFilename = "ui_bg_log";
            newPanel.shownLocX = 48;
            newPanel.shownLocY = 0;
            newPanel.hiddenLocX = -288;
            newPanel.hiddenLocY = 0;
            newPanel.hidingXIncrement = 3;
            newPanel.hidingYIncrement = 0;
            newPanel.Width = 192;
            newPanel.Height = 336;
            if (gv.toggleSettings.showLogPanel)
            {
                newPanel.showing = true;
            }
            else
            {
                newPanel.hiding = true;
            }

            IB2HtmlLogBox newLog = gv.log;
            newLog.tbXloc = 10;
            newLog.tbYloc = 10;
            newLog.tbWidth = 186;
            newLog.tbHeight = 330;
            newLog.numberOfLinesToShow = 28;
            newPanel.logList.Add(newLog);
            mainUiLayout.panelList.Add(newPanel);
        }
        public void createButtonsPanel()
        {
            //create buttons panel
            IB2Panel newPanel = new IB2Panel(gv);
            newPanel.tag = "BottomPanel";
            newPanel.backgroundImageFilename = "none";
            newPanel.shownLocX = 0;
            newPanel.shownLocY = 0;
            newPanel.Width = 48;
            newPanel.Height = 336;

            //toggle
            IB2ToggleButton newToggle = new IB2ToggleButton(gv);            
            newToggle.tag = "tglLog";
            newToggle.ImgOnFilename = "tgl_log_on";
            newToggle.ImgOffFilename = "tgl_log_off";
            newToggle.toggleOn = gv.toggleSettings.showLogPanel;            
            newToggle.X = 0;
            newToggle.Y = 0;
            newToggle.Width = 48;
            newToggle.Height = 48;
            newToggle.show = true;
            newPanel.toggleList.Add(newToggle);

            //toggle
            newToggle = new IB2ToggleButton(gv);
            newToggle.tag = "tglMiniMap";
            newToggle.ImgOnFilename = "tgl_minimap_on";
            newToggle.ImgOffFilename = "tgl_minimap_off";
            newToggle.toggleOn = gv.toggleSettings.showMiniMap;
            showMiniMap = gv.toggleSettings.showMiniMap;
            newToggle.X = 0;
            newToggle.Y = 48;
            newToggle.Width = 48;
            newToggle.Height = 48;
            newToggle.show = true;
            newPanel.toggleList.Add(newToggle);

            //button
            IB2Button newButton = new IB2Button(gv);
            newButton.tag = "btnTraitUseOnMainMap";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "btntrait";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "";
            newButton.X = 0;
            newButton.Y = 96;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 1.0f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            //button
            newButton = new IB2Button(gv);
            newButton.tag = "btnCastOnMainMap";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "btnspell";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "C";
            newButton.X = 0;
            newButton.Y = 144;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 1.0f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            //button
            newButton = new IB2Button(gv);
            newButton.tag = "btnSave";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "btndisk";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "";
            newButton.X = 0;
            newButton.Y = 192;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 1.0f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            //toggle   
            newToggle = new IB2ToggleButton(gv);
            newToggle.tag = "tglFullParty";
            newToggle.ImgOnFilename = "tgl_fullparty_on";
            newToggle.ImgOffFilename = "tgl_fullparty_off";
            newToggle.toggleOn = gv.toggleSettings.showFullParty;
            showFullParty = gv.toggleSettings.showFullParty;
            newToggle.X = 0;
            newToggle.Y = 240;
            newToggle.Width = 48;
            newToggle.Height = 48;
            newToggle.show = true;
            newPanel.toggleList.Add(newToggle);

            //button
            newButton = new IB2Button(gv);
            newButton.tag = "btnSettings";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "btnsettings";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "";
            newButton.X = 0;
            newButton.Y = 288;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 1.0f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            mainUiLayout.panelList.Add(newPanel);
        }
        public void createTogglesPanel()
        {
            //create buttons panel
            IB2Panel newPanel = new IB2Panel(gv);
            newPanel.tag = "TogglePanel";
            newPanel.backgroundImageFilename = "none";
            newPanel.shownLocX = 48;
            newPanel.shownLocY = 288;
            newPanel.hiddenLocX = 48;
            newPanel.hiddenLocY = 384;
            newPanel.hidingXIncrement = 0;
            newPanel.hidingYIncrement = 3;
            newPanel.Width = 336;
            newPanel.Height = 48;
            showTogglePanel = gv.toggleSettings.showTogglePanel;
            if (gv.toggleSettings.showTogglePanel)
            {
                newPanel.currentLocX = 48;
                newPanel.currentLocY = 288;
                newPanel.showing = true;
            }
            else
            {
                newPanel.currentLocX = 48;
                newPanel.currentLocY = 384;
                newPanel.hiding = true;
            }
            

            //toggle
            IB2ToggleButton newToggle = new IB2ToggleButton(gv);
            newToggle.tag = "tglGrid";
            newToggle.ImgOnFilename = "tgl_grid_on";
            newToggle.ImgOffFilename = "tgl_grid_off";
            newToggle.toggleOn = gv.toggleSettings.map_showGrid;
            gv.mod.map_showGrid = gv.toggleSettings.map_showGrid;
            newToggle.X = 0;
            newToggle.Y = 0;
            newToggle.Width = 48;
            newToggle.Height = 48;
            newToggle.show = true;
            newPanel.toggleList.Add(newToggle);

            //toggle
            newToggle = new IB2ToggleButton(gv);
            newToggle.tag = "tglClock";
            newToggle.ImgOnFilename = "tgl_clock_on";
            newToggle.ImgOffFilename = "tgl_clock_off";
            newToggle.toggleOn = gv.toggleSettings.showClock;
            showClock = gv.toggleSettings.showClock;
            newToggle.X = 48;
            newToggle.Y = 0;
            newToggle.Width = 48;
            newToggle.Height = 48;
            newToggle.show = true;
            newPanel.toggleList.Add(newToggle);

            //toggle
            newToggle = new IB2ToggleButton(gv);
            newToggle.tag = "tglSound";
            newToggle.ImgOnFilename = "tgl_sound_on";
            newToggle.ImgOffFilename = "tgl_sound_off";
            newToggle.toggleOn = gv.toggleSettings.playSoundFx;
            gv.mod.playSoundFx = gv.toggleSettings.playSoundFx;
            newToggle.X = 96;
            newToggle.Y = 0;
            newToggle.Width = 48;
            newToggle.Height = 48;
            newToggle.show = true;
            newPanel.toggleList.Add(newToggle);

            //toggle
            newToggle = new IB2ToggleButton(gv);
            newToggle.tag = "tglZoom";
            newToggle.ImgOnFilename = "tgl_zoom_on";
            newToggle.ImgOffFilename = "tgl_zoom_off";
            newToggle.toggleOn = !gv.toggleSettings.map_use11x11;
            use11x11 = gv.toggleSettings.map_use11x11;
            if (use11x11)
            {
                sqrScale = 0.6364f;
            }
            else
            {
                sqrScale = 1.0f;
            }
            newToggle.X = 144;
            newToggle.Y = 0;
            newToggle.Width = 48;
            newToggle.Height = 48;
            newToggle.show = true;
            newPanel.toggleList.Add(newToggle);

            //toggle
            newToggle = new IB2ToggleButton(gv);
            newToggle.tag = "tglDebugMode";
            newToggle.ImgOnFilename = "tgl_debugmode_on";
            newToggle.ImgOffFilename = "tgl_debugmode_off";
            newToggle.toggleOn = gv.toggleSettings.debugMode;
            gv.mod.debugMode = gv.toggleSettings.debugMode;
            newToggle.X = 192;
            newToggle.Y = 0;
            newToggle.Width = 48;
            newToggle.Height = 48;
            newToggle.show = true;
            newPanel.toggleList.Add(newToggle);

            mainUiLayout.panelList.Add(newPanel);
        }
        public void createPortraitsPanel()
        {
            //create buttons panel
            IB2Panel newPanel = new IB2Panel(gv);
            newPanel.tag = "portraitPanel";
            newPanel.backgroundImageFilename = "ui_bg_log";
            newPanel.shownLocX = 384;
            newPanel.shownLocY = 0;
            newPanel.Width = 144;
            newPanel.Height = 196;

            //portrait
            IB2Portrait newPort = new IB2Portrait(gv);
            newPort.tag = "port0";
            newPort.ImgBGFilename = "item_slot";
            newPort.ImgFilename = "ptr_adela";
            newPort.ImgLUFilename = "btnLevelUpPlus";
            newPort.GlowFilename = "btn_ptr_glow";
            newPort.X = 3;
            newPort.Y = 3;
            newPort.Width = 42;
            newPort.Height = 65;
            newPort.scaler = 0.8f;
            newPanel.portraitList.Add(newPort);

            //portrait
            newPort = new IB2Portrait(gv);
            newPort.tag = "port1";
            newPort.ImgBGFilename = "item_slot";
            newPort.ImgFilename = "ptr_adela";
            newPort.ImgLUFilename = "btnLevelUpPlus";
            newPort.GlowFilename = "btn_ptr_glow";
            newPort.X = 51;
            newPort.Y = 3;
            newPort.Width = 42;
            newPort.Height = 65;
            newPort.scaler = 0.8f;
            newPanel.portraitList.Add(newPort);

            //portrait
            newPort = new IB2Portrait(gv);
            newPort.tag = "port2";
            newPort.ImgBGFilename = "item_slot";
            newPort.ImgFilename = "ptr_adela";
            newPort.ImgLUFilename = "btnLevelUpPlus";
            newPort.GlowFilename = "btn_ptr_glow";
            newPort.X = 99;
            newPort.Y = 3;
            newPort.Width = 42;
            newPort.Height = 65;
            newPort.scaler = 0.8f;
            newPanel.portraitList.Add(newPort);

            //portrait
            newPort = new IB2Portrait(gv);
            newPort.tag = "port3";
            newPort.ImgBGFilename = "item_slot";
            newPort.ImgFilename = "ptr_adela";
            newPort.ImgLUFilename = "btnLevelUpPlus";
            newPort.GlowFilename = "btn_ptr_glow";
            newPort.X = 3;
            newPort.Y = 74;
            newPort.Width = 42;
            newPort.Height = 65;
            newPort.scaler = 0.8f;
            newPanel.portraitList.Add(newPort);

            //portrait
            newPort = new IB2Portrait(gv);
            newPort.tag = "port4";
            newPort.ImgBGFilename = "item_slot";
            newPort.ImgFilename = "ptr_adela";
            newPort.ImgLUFilename = "btnLevelUpPlus";
            newPort.GlowFilename = "btn_ptr_glow";
            newPort.X = 51;
            newPort.Y = 74;
            newPort.Width = 42;
            newPort.Height = 65;
            newPort.scaler = 0.8f;
            newPanel.portraitList.Add(newPort);

            //portrait
            newPort = new IB2Portrait(gv);
            newPort.tag = "port5";
            newPort.ImgBGFilename = "item_slot";
            newPort.ImgFilename = "ptr_adela";
            newPort.ImgLUFilename = "btnLevelUpPlus";
            newPort.GlowFilename = "btn_ptr_glow";
            newPort.X = 99;
            newPort.Y = 74;
            newPort.Width = 42;
            newPort.Height = 65;
            newPort.scaler = 0.8f;
            newPanel.portraitList.Add(newPort);

            //button
            IB2Button newButton = new IB2Button(gv);
            newButton.tag = "btnParty";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "btnparty";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "P";
            newButton.X = 0;
            newButton.Y = 144;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 1.0f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            //button
            newButton = new IB2Button(gv);
            newButton.tag = "btnInventory";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "btninventory";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "I";
            newButton.X = 48;
            newButton.Y = 144;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 1.0f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            //button
            newButton = new IB2Button(gv);
            newButton.tag = "btnJournal";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "btnjournal";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "J";
            newButton.X = 96;
            newButton.Y = 144;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 1.0f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            mainUiLayout.panelList.Add(newPanel);
        }
        public void createArrowsPanel()
        {
            //create buttons panel
            IB2Panel newPanel = new IB2Panel(gv);
            newPanel.tag = "arrowPanel";
            newPanel.backgroundImageFilename = "ui_bg_arrows";
            newPanel.shownLocX = 384;
            newPanel.shownLocY = 192;
            newPanel.Width = 144;
            newPanel.Height = 144;

            //button
            IB2Button newButton = new IB2Button(gv);
            newButton.tag = "ctrlUpArrow";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "ctrl_up_arrow";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "";
            newButton.X = 48;
            newButton.Y = 0;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 0.8f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            //button
            newButton = new IB2Button(gv);
            newButton.tag = "ctrlLeftArrow";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "ctrl_left_arrow";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "";
            newButton.X = 0;
            newButton.Y = 48;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 0.8f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            //button
            newButton = new IB2Button(gv);
            newButton.tag = "ctrlRightArrow";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "ctrl_right_arrow";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "";
            newButton.X = 96;
            newButton.Y = 48;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 0.8f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            //button
            newButton = new IB2Button(gv);
            newButton.tag = "ctrlDownArrow";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "ctrl_down_arrow";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "";
            newButton.X = 48;
            newButton.Y = 96;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 0.8f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            //button
            newButton = new IB2Button(gv);
            newButton.tag = "btnWait";
            newButton.ImgFilename = "btn_small";
            newButton.ImgOffFilename = "btn_small_off";
            newButton.ImgOnFilename = "btn_small_on";
            newButton.Img2Filename = "btnwait";
            newButton.Img2OffFilename = "";
            newButton.Img3Filename = "";
            newButton.GlowFilename = "btn_small_glow";
            newButton.btnState = buttonState.Normal;
            newButton.btnNotificationOn = false;
            newButton.glowOn = false;
            newButton.Text = "";
            newButton.Quantity = "";
            newButton.HotKey = "";
            newButton.X = 48;
            newButton.Y = 48;
            newButton.IBScript = "none";
            newButton.Width = 48;
            newButton.Height = 48;
            newButton.scaler = 0.8f;
            newButton.show = true;
            newPanel.buttonList.Add(newButton);

            mainUiLayout.panelList.Add(newPanel);
        }
        
        //MAIN SCREEN UPDATE
        public void Update(int elapsed)
        {
            mainUiLayout.Update(elapsed);

            #region PROP AMBIENT SPRITES
            foreach (Sprite spr in spriteList)
            {
                spr.Update(elapsed, gv);
            }
            #endregion

            #region FLOATY TEXT            
            if (floatyTextPool.Count > 0)
            {
                int shiftUp = (int)(0.05f * elapsed);
                foreach (FloatyText ft in floatyTextPool)
                {
                    ft.z += shiftUp;
                    ft.timeToLive -= (int)(elapsed);
                }

                //remove expired floaty text
                for (int i = floatyTextPool.Count - 1; i >= 0; i--)
                {
                    if (floatyTextPool[i].timeToLive <= 0)
                    {
                        floatyTextPool.RemoveAt(i);
                    }
                }

                //remove if too many floats are in pool
                for (int i = floatyTextPool.Count - 1; i >= 0; i--)
                {
                    if (((floatyTextPool.Count - 1 - i) > 15))
                    {
                        floatyTextPool.RemoveAt(i);
                    }
                }
            }            
            #endregion
        }
        
        //MAIN SCREEN DRAW
        public void resetMiniMapBitmap()
        {
            int minimapSquareSizeInPixels = 4 * gv.squareSize / gv.mod.currentArea.MapSizeX;
            int drawW = minimapSquareSizeInPixels * gv.mod.currentArea.MapSizeX;
            int drawH = minimapSquareSizeInPixels * gv.mod.currentArea.MapSizeY;
            using (System.Drawing.Bitmap surface = new System.Drawing.Bitmap(drawW, drawH))
            {
                using (Graphics device = Graphics.FromImage(surface))
                {
                    //draw background image first
                    /*if ((!gv.mod.currentArea.ImageFileName.Equals("none")) && (gv.cc.bmpMap != null))
                    {
                        System.Drawing.Bitmap bg = gv.cc.LoadBitmapGDI(gv.mod.currentArea.ImageFileName);
                        Rectangle srcBG = new Rectangle(0, 0, bg.Width, bg.Height);
                        Rectangle dstBG = new Rectangle(gv.mod.currentArea.backgroundImageStartLocX * minimapSquareSizeInPixels,
                                                        gv.mod.currentArea.backgroundImageStartLocY * minimapSquareSizeInPixels,
                                                        minimapSquareSizeInPixels * (bg.Width / 50),
                                                        minimapSquareSizeInPixels * (bg.Height / 50));
                        device.DrawImage(bg, dstBG, srcBG, GraphicsUnit.Pixel);
                        bg.Dispose();
                        bg = null;
                    }*/
                    #region Draw Layer 1
                    for (int x = 0; x < gv.mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < gv.mod.currentArea.MapSizeY; y++)
                        {
                            string tile = gv.mod.currentArea.Layer1Filename[y * gv.mod.currentArea.MapSizeX + x];
                            Rectangle src = new Rectangle(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);
                            float scalerX = gv.cc.GetFromTileBitmapList(tile).PixelSize.Width / gv.tileSizeInPixels;
                            float scalerY = gv.cc.GetFromTileBitmapList(tile).PixelSize.Height / gv.tileSizeInPixels;
                            int brX = (int)(minimapSquareSizeInPixels * scalerX);
                            int brY = (int)(minimapSquareSizeInPixels * scalerY);
                            Rectangle dst = new Rectangle(x * minimapSquareSizeInPixels, y * minimapSquareSizeInPixels, brX, brY);
                            device.DrawImage(gv.cc.GetFromTileGDIBitmapList(tile), dst, src, GraphicsUnit.Pixel);
                        }
                    }
                    #endregion
                    #region Draw Layer 2
                    for (int x = 0; x < gv.mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < gv.mod.currentArea.MapSizeY; y++)
                        {
                            string tile = gv.mod.currentArea.Layer2Filename[y * gv.mod.currentArea.MapSizeX + x];
                            Rectangle src = new Rectangle(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);
                            float scalerX = gv.cc.GetFromTileBitmapList(tile).PixelSize.Width / gv.tileSizeInPixels;
                            float scalerY = gv.cc.GetFromTileBitmapList(tile).PixelSize.Height / gv.tileSizeInPixels;
                            int brX = (int)(minimapSquareSizeInPixels * scalerX);
                            int brY = (int)(minimapSquareSizeInPixels * scalerY);
                            Rectangle dst = new Rectangle(x * minimapSquareSizeInPixels, y * minimapSquareSizeInPixels, brX, brY);
                            device.DrawImage(gv.cc.GetFromTileGDIBitmapList(tile), dst, src, GraphicsUnit.Pixel);
                        }
                    }
                    #endregion
                    #region Draw Layer 3
                    for (int x = 0; x < gv.mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < gv.mod.currentArea.MapSizeY; y++)
                        {
                            string tile = gv.mod.currentArea.Layer3Filename[y * gv.mod.currentArea.MapSizeX + x];
                            Rectangle src = new Rectangle(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);
                            float scalerX = gv.cc.GetFromTileBitmapList(tile).PixelSize.Width / gv.tileSizeInPixels;
                            float scalerY = gv.cc.GetFromTileBitmapList(tile).PixelSize.Height / gv.tileSizeInPixels;
                            int brX = (int)(minimapSquareSizeInPixels * scalerX);
                            int brY = (int)(minimapSquareSizeInPixels * scalerY);
                            Rectangle dst = new Rectangle(x * minimapSquareSizeInPixels, y * minimapSquareSizeInPixels, brX, brY);
                            device.DrawImage(gv.cc.GetFromTileGDIBitmapList(tile), dst, src, GraphicsUnit.Pixel);
                        }
                    }
                    #endregion
                    minimap = gv.cc.ConvertGDIBitmapToD2D((System.Drawing.Bitmap)surface.Clone());
                }
            }
        }        
        public void redrawMain()
        {
            setExplored();
            if (!gv.mod.currentArea.areaDark)
            {
                drawWorldMap();               
                drawProps();
                if (gv.mod.map_showGrid)
                {
                    drawGrid();
                }
            }
            drawPlayer();
            if (!gv.mod.currentArea.areaDark)
            {
                drawMovingProps();
            }
                        
            if (!gv.mod.currentArea.areaDark)
            {
                if (gv.mod.currentArea.UseDayNightCycle)
                {
                    drawOverlayTints();
                }
                drawFogOfWar();
            }

            drawFloatyTextPool();
            drawMainMapFloatyText();

            if ((showClock) && (!hideClock))
            {
                drawMainMapClockText();
            }
            drawUiLayout();
            drawMiniMap();
            if (gv.showMessageBox)
            {
                gv.messageBox.onDrawLogBox();
            }
        }        
        public void drawWorldMap()
        {
            /*
            int minX = gv.mod.PlayerLocationX - gv.playerOffset - 2; //using -2 in case a large tile (3x3) needs to start off the visible map space to be seen
            if (minX < 0) { minX = 0; }
            int minY = gv.mod.PlayerLocationY - gv.playerOffset - 2; //using -2 in case a large tile (3x3) needs to start off the visible map space to be seen
            if (minY < 0) { minY = 0; }

            int maxX = gv.mod.PlayerLocationX + gv.playerOffset + 1;
            if (maxX > this.gv.mod.currentArea.MapSizeX) { maxX = this.gv.mod.currentArea.MapSizeX; }
            int maxY = gv.mod.PlayerLocationY + gv.playerOffset + 1; // use 2 so that extends down to bottom of screen
            if (maxY > this.gv.mod.currentArea.MapSizeY) { maxY = this.gv.mod.currentArea.MapSizeY; }
            */
            int offset = gv.playerOffset;
            if (use11x11)
            {
                offset = gv.playerOffsetZoom;
            }
            #region Draw Layer 1
            for (int x = gv.mod.PlayerLocationX - offset; x <= gv.mod.PlayerLocationX + offset; x++)
            {
                for (int y = gv.mod.PlayerLocationY - offset; y <= gv.mod.PlayerLocationY + offset; y++)
                {
                    //check if valid map location
                    if (x < 0) { continue; }
                    if (y < 0) { continue; }
                    if (x > this.gv.mod.currentArea.MapSizeX - 1) { continue; }
                    if (y > this.gv.mod.currentArea.MapSizeY - 1) { continue; }

                    string tile = gv.mod.currentArea.Layer1Filename[y * gv.mod.currentArea.MapSizeX + x];
                    int tlX = (x - gv.mod.PlayerLocationX + offset) * (int)(gv.squareSize * sqrScale);
                    int tlY = (y - gv.mod.PlayerLocationY + offset) * (int)(gv.squareSize * sqrScale);
                    float scalerX = gv.cc.GetFromTileBitmapList(tile).PixelSize.Width / gv.tileSizeInPixels;
                    if (scalerX == 0) { scalerX = 1.0f; }
                    float scalerY = gv.cc.GetFromTileBitmapList(tile).PixelSize.Height / gv.tileSizeInPixels;
                    if (scalerY == 0) { scalerY = 1.0f; }
                    int brX = (int)((int)(gv.squareSize * sqrScale) * scalerX);
                    int brY = (int)((int)(gv.squareSize * sqrScale) * scalerY);

                    try
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);
                        IbRect dst = new IbRect(tlX + mapStartLocXinPixels, tlY, brX, brY);
                        bool mirror = false;
                        if (gv.mod.currentArea.Layer1Mirror[y * gv.mod.currentArea.MapSizeX + x] == 1) { mirror = true; }
                        gv.DrawBitmap(gv.cc.GetFromTileBitmapList(tile), src, dst, gv.mod.currentArea.Layer1Rotate[y * gv.mod.currentArea.MapSizeX + x], mirror);
                    }
                    catch { }
                }
            }
            #endregion
            #region Draw Layer 2
            for (int x = gv.mod.PlayerLocationX - offset; x <= gv.mod.PlayerLocationX + offset; x++)
            {
                for (int y = gv.mod.PlayerLocationY - offset; y <= gv.mod.PlayerLocationY + offset; y++)
                {
                    //check if valid map location
                    if (x < 0) { continue; }
                    if (y < 0) { continue; }
                    if (x > this.gv.mod.currentArea.MapSizeX - 1) { continue; }
                    if (y > this.gv.mod.currentArea.MapSizeY - 1) { continue; }

                    string tile = gv.mod.currentArea.Layer2Filename[y * gv.mod.currentArea.MapSizeX + x];
                    int tlX = (x - gv.mod.PlayerLocationX + offset) * (int)(gv.squareSize * sqrScale);
                    int tlY = (y - gv.mod.PlayerLocationY + offset) * (int)(gv.squareSize * sqrScale);
                    float scalerX = gv.cc.GetFromTileBitmapList(tile).PixelSize.Width / gv.tileSizeInPixels;
                    if (scalerX == 0) { scalerX = 1.0f; }
                    float scalerY = gv.cc.GetFromTileBitmapList(tile).PixelSize.Height / gv.tileSizeInPixels;
                    if (scalerY == 0) { scalerY = 1.0f; }
                    int brX = (int)((int)(gv.squareSize * sqrScale) * scalerX);
                    int brY = (int)((int)(gv.squareSize * sqrScale) * scalerY);

                    try
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);
                        IbRect dst = new IbRect(tlX + mapStartLocXinPixels, tlY, brX, brY);
                        bool mirror = false;
                        if (gv.mod.currentArea.Layer2Mirror[y * gv.mod.currentArea.MapSizeX + x] == 1) { mirror = true; }
                        gv.DrawBitmap(gv.cc.GetFromTileBitmapList(tile), src, dst, gv.mod.currentArea.Layer2Rotate[y * gv.mod.currentArea.MapSizeX + x], mirror);
                    }
                    catch { }
                }
            }
            #endregion
            #region Draw Layer 3
            if (gv.mod.currentArea.Layer3Filename.Count > 0)
            {
                for (int x = gv.mod.PlayerLocationX - offset; x <= gv.mod.PlayerLocationX + offset; x++)
                {
                    for (int y = gv.mod.PlayerLocationY - offset; y <= gv.mod.PlayerLocationY + offset; y++)
                    {
                        //check if valid map location
                        if (x < 0) { continue; }
                        if (y < 0) { continue; }
                        if (x > this.gv.mod.currentArea.MapSizeX - 1) { continue; }
                        if (y > this.gv.mod.currentArea.MapSizeY - 1) { continue; }

                        string tile = gv.mod.currentArea.Layer3Filename[y * gv.mod.currentArea.MapSizeX + x];
                        int tlX = (x - gv.mod.PlayerLocationX + offset) * (int)(gv.squareSize * sqrScale);
                        int tlY = (y - gv.mod.PlayerLocationY + offset) * (int)(gv.squareSize * sqrScale);
                        float scalerX = gv.cc.GetFromTileBitmapList(tile).PixelSize.Width / gv.tileSizeInPixels;
                        if (scalerX == 0) { scalerX = 1.0f; }
                        float scalerY = gv.cc.GetFromTileBitmapList(tile).PixelSize.Height / gv.tileSizeInPixels;
                        if (scalerY == 0) { scalerY = 1.0f; }
                        int brX = (int)((int)(gv.squareSize * sqrScale) * scalerX);
                        int brY = (int)((int)(gv.squareSize * sqrScale) * scalerY);

                        try
                        {
                            IbRect src = new IbRect(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);
                            IbRect dst = new IbRect(tlX + mapStartLocXinPixels, tlY, brX, brY);
                            bool mirror = false;
                            if (gv.mod.currentArea.Layer3Mirror[y * gv.mod.currentArea.MapSizeX + x] == 1) { mirror = true; }
                            gv.DrawBitmap(gv.cc.GetFromTileBitmapList(tile), src, dst, gv.mod.currentArea.Layer3Rotate[y * gv.mod.currentArea.MapSizeX + x], mirror);
                        }
                        catch { }
                    }
                }
            }
            #endregion
        }
        public void drawProps()
        {
            int offset = gv.playerOffset;
            if (use11x11)
            {
                offset = gv.playerOffsetZoom;
            }
            foreach (Prop p in gv.mod.currentArea.Props)
            {
                if ((p.isShown) && (!p.isMover))
                {
                    if ((p.LocationX >= gv.mod.PlayerLocationX - offset) && (p.LocationX <= gv.mod.PlayerLocationX + offset)
                        && (p.LocationY >= gv.mod.PlayerLocationY - offset) && (p.LocationY <= gv.mod.PlayerLocationY + offset))
                    {
                        //prop X - playerX
                        int x = ((p.LocationX - gv.mod.PlayerLocationX) * (int)(gv.squareSize * sqrScale)) + (offset * (int)(gv.squareSize * sqrScale));
                        int y = ((p.LocationY - gv.mod.PlayerLocationY) * (int)(gv.squareSize * sqrScale)) + (offset * (int)(gv.squareSize * sqrScale));
                        int dstW = (int)(((float)gv.cc.GetFromBitmapList(p.ImageFileName).PixelSize.Width / (float)(gv.squareSizeInPixels) * (float)(gv.squareSize * sqrScale)));
                        int dstH = (int)(((float)gv.cc.GetFromBitmapList(p.ImageFileName).PixelSize.Height / (float)(gv.squareSizeInPixels) * (float)(gv.squareSize * sqrScale)));
                        if (p.ImageFileName.StartsWith("tkn_"))
                        {
                            dstH = (int)(((float)(gv.cc.GetFromBitmapList(p.ImageFileName).PixelSize.Height / 2) / (float)(gv.squareSizeInPixels) * (float)(gv.squareSize * sqrScale)));
                        }
                        int dstXshift = (dstW - (int)(gv.squareSize * sqrScale)) / 2;
                        int dstYshift = (dstH - (int)(gv.squareSize * sqrScale)) / 2;
                        IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(p.ImageFileName).PixelSize.Width, gv.cc.GetFromBitmapList(p.ImageFileName).PixelSize.Width);
                        IbRect dst = new IbRect(x + mapStartLocXinPixels - dstXshift, y - dstYshift, dstW, dstH);
                                                
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(p.ImageFileName), src, dst, !p.PropFacingLeft);

                        if (gv.mod.showInteractionState == true)
                        {
                            if (!p.EncounterWhenOnPartySquare.Equals("none"))
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("encounter_indicator");
                                src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                continue;
                            }

                            if (p.unavoidableConversation)
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                                src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                continue;
                            }

                            if (!p.ConversationWhenOnPartySquare.Equals("none"))
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("optional_conversation_indicator");
                                src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                continue;
                            }
                        }
                    }
                }
            }
        }
        public void drawMovingProps()
        {
            int offset = gv.playerOffset;
            if (use11x11)
            {
                offset = gv.playerOffsetZoom;
            }
            foreach (Prop p in gv.mod.currentArea.Props)
            {
                if ((p.isShown) && (p.isMover))
                {
                    if ((p.LocationX >= gv.mod.PlayerLocationX - offset) && (p.LocationX <= gv.mod.PlayerLocationX + offset)
                        && (p.LocationY >= gv.mod.PlayerLocationY - offset) && (p.LocationY <= gv.mod.PlayerLocationY + offset))
                    {
                        //prop X - playerX
                        int x = ((p.LocationX - gv.mod.PlayerLocationX) * (int)(gv.squareSize * sqrScale)) + (offset * (int)(gv.squareSize * sqrScale));
                        int y = ((p.LocationY - gv.mod.PlayerLocationY) * (int)(gv.squareSize * sqrScale)) + (offset * (int)(gv.squareSize * sqrScale));
                        int dstW = (int)(((float)gv.cc.GetFromBitmapList(p.ImageFileName).PixelSize.Width / (float)(gv.squareSizeInPixels) * (float)(gv.squareSize * sqrScale)));
                        int dstH = (int)(((float)gv.cc.GetFromBitmapList(p.ImageFileName).PixelSize.Height / (float)(gv.squareSizeInPixels) * (float)(gv.squareSize * sqrScale)));
                        if (p.ImageFileName.StartsWith("tkn_"))
                        {
                            dstH = (int)(((float)(gv.cc.GetFromBitmapList(p.ImageFileName).PixelSize.Height / 2) / (float)(gv.squareSizeInPixels) * (float)(gv.squareSize * sqrScale)));
                        }
                        int dstXshift = (dstW - (int)(gv.squareSize * sqrScale)) / 2;
                        int dstYshift = (dstH - (int)(gv.squareSize * sqrScale)) / 2;
                        IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(p.ImageFileName).PixelSize.Width, gv.cc.GetFromBitmapList(p.ImageFileName).PixelSize.Width);
                        IbRect dst = new IbRect(x + mapStartLocXinPixels - dstXshift, y - dstYshift, dstW, dstH);
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(p.ImageFileName), src, dst);

                        if (gv.mod.showInteractionState)
                        {
                            if (!p.EncounterWhenOnPartySquare.Equals("none"))
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("encounter_indicator");
                                src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                continue;
                            }

                            if (p.unavoidableConversation)
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                                src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                continue;
                            }

                            if (!p.ConversationWhenOnPartySquare.Equals("none"))
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("optional_conversation_indicator");
                                src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                continue;
                            }
                        }
                    }
                }
            }            
        }
        public void drawMiniMap()
        {
            if (showMiniMap)
            {
                int pW = (int)((float)gv.screenWidth / 100.0f);
                int pH = (int)((float)gv.screenHeight / 100.0f);
                int shift = pW;
                
                //minimap should be 4 squares wide
                int minimapSquareSizeInPixels = 4 * gv.squareSize / gv.mod.currentArea.MapSizeX;
                int drawW = minimapSquareSizeInPixels * gv.mod.currentArea.MapSizeX;
                int drawH = minimapSquareSizeInPixels * gv.mod.currentArea.MapSizeY;

                /*TODO
                    //draw a dark border
                    Paint pnt = new Paint();
                    pnt.setColor(Color.DKGRAY);
                    pnt.setStrokeWidth(pW * 2);
                    pnt.setStyle(Paint.Style.STROKE);	
                    canvas.drawRect(new Rect(gv.oXshift, pH, gv.oXshift + drawW + pW, pH + drawH + pW), pnt);
                */
                //draw minimap
                if (minimap == null) { resetMiniMapBitmap(); }
                IbRect src = new IbRect(0, 0, minimap.PixelSize.Width, minimap.PixelSize.Height);
                IbRect dst = new IbRect(gv.squareSize, pH, drawW, drawH);
                gv.DrawBitmap(minimap, src, dst);

                //draw Fog of War
                if (gv.mod.currentArea.UseMiniMapFogOfWar)
                {
                    for (int x = 0; x < this.gv.mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < this.gv.mod.currentArea.MapSizeY; y++)
                        {
                            int xx = x * minimapSquareSizeInPixels;
                            int yy = y * minimapSquareSizeInPixels;
                            src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                            dst = new IbRect(gv.squareSize + xx, pH + yy, minimapSquareSizeInPixels, minimapSquareSizeInPixels);
                            if (gv.mod.currentArea.Visible[y * gv.mod.currentArea.MapSizeX + x] == 0)
                            {
                                gv.DrawBitmap(gv.cc.black_tile, src, dst);
                            }
                        }
                    }
                }
                                
	            //draw a location marker square RED
                int x2 = gv.mod.PlayerLocationX * minimapSquareSizeInPixels + gv.squareSize;
                int y2 = gv.mod.PlayerLocationY * minimapSquareSizeInPixels;
                src = new IbRect(0, 0, gv.cc.map_marker.PixelSize.Width, gv.cc.map_marker.PixelSize.Height);
                dst = new IbRect(x2, y2 + pH, minimapSquareSizeInPixels, minimapSquareSizeInPixels);
                gv.DrawBitmap(gv.cc.map_marker, src, dst);	            
            }
        }
        public void drawPlayer()
        {
            int offset = gv.playerOffset;
            if (use11x11)
            {
                offset = gv.playerOffsetZoom;
            }
            if (gv.mod.selectedPartyLeader >= gv.mod.playerList.Count)
            {
                gv.mod.selectedPartyLeader = 0;
            }
            int x = offset * (int)(gv.squareSize * sqrScale);
            int y = offset * (int)(gv.squareSize * sqrScale);
            int shift = (int)(gv.squareSize * sqrScale) / 3;
            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(gv.mod.playerList[gv.mod.selectedPartyLeader].tokenFilename).PixelSize.Width, gv.cc.GetFromBitmapList(gv.mod.playerList[gv.mod.selectedPartyLeader].tokenFilename).PixelSize.Width);
            IbRect dst = new IbRect(x + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
            if (gv.mod.showPartyToken)
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(gv.mod.partyTokenFilename), src, dst, !gv.mod.playerList[0].combatFacingLeft);
            }
            else
            {
                if ((showFullParty) && (gv.mod.playerList.Count > 1))
                {
                    if (gv.mod.playerList[0].combatFacingLeft == true)
                    {
                        //gv.oXshift = gv.oXshift + shift / 2;
                    }
                    else
                    {
                        //gv.oXshift = gv.oXshift - shift / 2;
                    }
                    int reducedSquareSize = (int)(gv.squareSize * sqrScale) * 2 / 3;
                    for (int i = gv.mod.playerList.Count - 1; i >= 0; i--)
                    {
                        if ((i == 0) && (i != gv.mod.selectedPartyLeader))
                        {
                            dst = new IbRect(x + shift + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            gv.DrawBitmap(gv.cc.GetFromBitmapList(gv.mod.playerList[i].tokenFilename), src, dst, !gv.mod.playerList[i].combatFacingLeft);
                        }
                        if ((i == 1) && (i != gv.mod.selectedPartyLeader))
                        {
                            dst = new IbRect(x - shift + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            gv.DrawBitmap(gv.cc.GetFromBitmapList(gv.mod.playerList[i].tokenFilename), src, dst, !gv.mod.playerList[i].combatFacingLeft);
                        }
                        if ((i == 2) && (i != gv.mod.selectedPartyLeader))
                        {
                            if (gv.mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + (shift) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (gv.mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x - (shift) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            gv.DrawBitmap(gv.cc.GetFromBitmapList(gv.mod.playerList[i].tokenFilename), src, dst, !gv.mod.playerList[i].combatFacingLeft);
                        }
                        if ((i == 3) && (i != gv.mod.selectedPartyLeader))
                        {
                            if (gv.mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (gv.mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (gv.mod.selectedPartyLeader == 2)
                            {
                                dst = new IbRect(x + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            gv.DrawBitmap(gv.cc.GetFromBitmapList(gv.mod.playerList[i].tokenFilename), src, dst, !gv.mod.playerList[i].combatFacingLeft);
                        }
                        if ((i == 4) && (i != gv.mod.selectedPartyLeader))
                        {
                            if (gv.mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (gv.mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (gv.mod.selectedPartyLeader == 2)
                            {
                                dst = new IbRect(x - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (gv.mod.selectedPartyLeader == 3)
                            {
                                dst = new IbRect(x - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            gv.DrawBitmap(gv.cc.GetFromBitmapList(gv.mod.playerList[i].tokenFilename), src, dst, !gv.mod.playerList[i].combatFacingLeft);
                        }

                        if ((i == 5) && (i != gv.mod.selectedPartyLeader))
                        {
                            if (gv.mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (gv.mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (gv.mod.selectedPartyLeader == 2)
                            {
                                dst = new IbRect(x + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (gv.mod.selectedPartyLeader == 3)
                            {
                                dst = new IbRect(x + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (gv.mod.selectedPartyLeader == 4)
                            {
                                dst = new IbRect(x + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x - (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            gv.DrawBitmap(gv.cc.GetFromBitmapList(gv.mod.playerList[i].tokenFilename), src, dst, !gv.mod.playerList[i].combatFacingLeft);
                        }
                    }
                    
                    if (gv.mod.playerList[0].combatFacingLeft == true)
                    {
                        //gv.oXshift = gv.oXshift - shift / 2;
                    }
                    else
                    {
                        //gv.oXshift = gv.oXshift + shift / 2;
                    }
                }
                //always draw party leader on top
                int storeShift = shift;
                shift = 0;
                if (gv.mod.selectedPartyLeader == 0)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                    else
                    {
                        dst = new IbRect(x + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                }
                else if (gv.mod.selectedPartyLeader == 1)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x + shift + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                    else
                    {
                        dst = new IbRect(x + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                }
                else if (gv.mod.selectedPartyLeader == 2)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x - shift + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                    else
                    {
                        dst = new IbRect(x + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                }
                else if (gv.mod.selectedPartyLeader == 3)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x + (shift * 2) + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                    else
                    {
                        dst = new IbRect(x + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                }
                else if (gv.mod.selectedPartyLeader == 4)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x - (shift * 2) + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                    else
                    {
                        dst = new IbRect(x + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                }
                else if (gv.mod.selectedPartyLeader == 5)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x - (shift * 3) + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                    else
                    {
                        dst = new IbRect(x + mapStartLocXinPixels, y, (int)(gv.squareSize * sqrScale), (int)(gv.squareSize * sqrScale));
                    }
                }                
                gv.DrawBitmap(gv.cc.GetFromBitmapList(gv.mod.playerList[gv.mod.selectedPartyLeader].tokenFilename), src, dst, !gv.mod.playerList[gv.mod.selectedPartyLeader].combatFacingLeft);
                shift = storeShift;
            }
        }
        public void drawGrid()
        {
            int offset = gv.playerOffset;
            if (use11x11)
            {
                offset = gv.playerOffsetZoom;
            }

            int minX = gv.mod.PlayerLocationX - offset;
            if (minX < 0) { minX = 0; }
            int minY = gv.mod.PlayerLocationY - offset;
            if (minY < 0) { minY = 0; }

            int maxX = gv.mod.PlayerLocationX + offset + 1;
            if (maxX > this.gv.mod.currentArea.MapSizeX) { maxX = this.gv.mod.currentArea.MapSizeX; }
            int maxY = gv.mod.PlayerLocationY + offset + 1;
            if (maxY > this.gv.mod.currentArea.MapSizeY) { maxY = this.gv.mod.currentArea.MapSizeY; }

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    int tlX = (x - gv.mod.PlayerLocationX + offset) * (int)(gv.squareSize * sqrScale);
                    int tlY = (y - gv.mod.PlayerLocationY + offset) * (int)(gv.squareSize * sqrScale);
                    int brX = (int)(gv.squareSize * sqrScale);
                    int brY = (int)(gv.squareSize * sqrScale);
                    IbRect src = new IbRect(0, 0, gv.cc.walkBlocked.PixelSize.Width, gv.cc.walkBlocked.PixelSize.Height);
                    IbRect dst = new IbRect(tlX + mapStartLocXinPixels, tlY, brX, brY);
                    if (gv.mod.currentArea.LoSBlocked[y * gv.mod.currentArea.MapSizeX + x] == 1)
                    {
                        gv.DrawBitmap(gv.cc.losBlocked, src, dst);
                    }
                    if (gv.mod.currentArea.Walkable[y * gv.mod.currentArea.MapSizeX + x] == 0)
                    {
                        gv.DrawBitmap(gv.cc.walkBlocked, src, dst);
                    }
                    else
                    {
                        gv.DrawBitmap(gv.cc.walkPass, src, dst);
                    }
                }
            }
        }
        public void drawMainMapFloatyText()
        {
            floatyTextBox.onDrawTextBox();
            /*
            int txtH = (int)gv.fontHeight;

            for (int x = 0; x <= 2; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    gv.DrawText(gv.cc.floatyText, gv.cc.floatyTextLoc.X + x + mapStartLocXinPixels, gv.cc.floatyTextLoc.Y + y + txtH, "bk");
                }
            }
            
            gv.DrawText(gv.cc.floatyText, gv.cc.floatyTextLoc.X + mapStartLocXinPixels, gv.cc.floatyTextLoc.Y + txtH, "wh");
            */
        }
        public void drawOverlayTints()
        {
            int offset = gv.playerOffset;
            if (use11x11)
            {
                offset = gv.playerOffsetZoom;
            }

            IbRect src = new IbRect(0, 0, gv.cc.tint_sunset.PixelSize.Width, gv.cc.tint_sunset.PixelSize.Height);
            //IbRect dst = new IbRect(gv.oXshift + mapStartLocXinPixels, 0, ((int)(gv.squareSize * sqrScale) * (gv.playerOffsetX * 2 + 1)), ((int)(gv.squareSize * sqrScale) * (gv.playerOffsetY * 2 + 2)));
            IbRect dst = new IbRect(mapStartLocXinPixels, 0, ((int)(gv.squareSize * sqrScale) * (offset * 2 + 1)) + gv.pS, ((int)(gv.squareSize * sqrScale) * (offset * 2 + 2)) + gv.pS);

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
        public void drawMainMapClockText()
        {
            int timeofday = gv.mod.WorldTime % (24 * 60);
            int hour = timeofday / 60;
            int minute = timeofday % 60;
            string sMinute = minute + "";
            if (minute < 10)
            {
                sMinute = "0" + minute;
            }

            int txtH = (int)gv.fontHeight;
            int xLoc = 1 * gv.squareSize;
            int yLoc = (gv.squaresInHeight - 1) * gv.squareSize + gv.squareSize - gv.fontHeight - gv.fontHeight;
            if (showTogglePanel)
            {
                yLoc = (gv.squaresInHeight - 1) * gv.squareSize - gv.fontHeight - gv.fontHeight;
            }
                      
            for (int x = 0; x <= 2; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    if (gv.mod.useRationSystem)
                    {
                        gv.DrawText(hour + ":" + sMinute + " Rations(" + gv.mod.numberOfRationsRemaining.ToString() + ")", x + xLoc, y + yLoc, "bk");
                    }
                    else
                    {
                        gv.DrawText(hour + ":" + sMinute, x + xLoc, y + yLoc, "bk");
                    }
                }
            }
            if (gv.mod.useRationSystem)
            {
                gv.DrawText(hour + ":" + sMinute + " Rations(" + gv.mod.numberOfRationsRemaining.ToString() + ")", xLoc, yLoc, "wh");
            }
            else
            {
                gv.DrawText(hour + ":" + sMinute, xLoc, yLoc, "wh");
            }
        }
        public void drawFogOfWar()
        {
            /*int minX = gv.mod.PlayerLocationX - gv.playerOffsetX-1;
            if (minX < 0) { minX = 0; }
            int minY = gv.mod.PlayerLocationY - gv.playerOffsetY-1;
            if (minY < 0) { minY = 0; }

            int maxX = gv.mod.PlayerLocationX + gv.playerOffsetX + 2;
            if (maxX > this.gv.mod.currentArea.MapSizeX) { maxX = this.gv.mod.currentArea.MapSizeX; }
            int maxY = gv.mod.PlayerLocationY + gv.playerOffsetY + 3;
            if (maxY > this.gv.mod.currentArea.MapSizeY) { maxY = this.gv.mod.currentArea.MapSizeY; }

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    int tlX = (x - gv.mod.PlayerLocationX + gv.playerOffsetX) * gv.squareSize;
                    int tlY = (y - gv.mod.PlayerLocationY + gv.playerOffsetY) * gv.squareSize;
                    int brX = gv.squareSize;
                    int brY = gv.squareSize;
                    IbRect src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                    IbRect dst = new IbRect(tlX + mapStartLocXinPixels, tlY, brX, brY);
                    if (gv.mod.currentArea.Visible[y * gv.mod.currentArea.MapSizeX + x] == 0)
                    {
                        gv.DrawBitmap(gv.cc.black_tile, src, dst);
                    }
                }
            }*/
            int offset = gv.playerOffset;
            if (use11x11)
            {
                offset = gv.playerOffsetZoom;
            }
            for (int x = gv.mod.PlayerLocationX - offset; x <= gv.mod.PlayerLocationX + offset; x++)
            {
                for (int y = gv.mod.PlayerLocationY - offset; y <= gv.mod.PlayerLocationY + offset; y++)
                {
                    //check if valid map location
                    if (x < 0) { continue; }
                    if (y < 0) { continue; }
                    if (x > this.gv.mod.currentArea.MapSizeX - 1) { continue; }
                    if (y > this.gv.mod.currentArea.MapSizeY - 1) { continue; }

                    int tlX = (x - gv.mod.PlayerLocationX + offset) * (int)(gv.squareSize * sqrScale);
                    int tlY = (y - gv.mod.PlayerLocationY + offset) * (int)(gv.squareSize * sqrScale);
                    int brX = (int)(gv.squareSize * sqrScale);
                    int brY = (int)(gv.squareSize * sqrScale);

                    try
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                        IbRect dst = new IbRect(tlX + mapStartLocXinPixels, tlY, brX, brY);
                        if (gv.mod.currentArea.Visible[y * gv.mod.currentArea.MapSizeX + x] == 0)
                        {
                            gv.DrawBitmap(gv.cc.black_tile, src, dst);
                        }                        
                    }
                    catch { }
                }
            }
        }
        public void drawFloatyTextPool()
        {
            if (floatyTextPool.Count > 0)
            {
                int txtH = (int)gv.fontHeight;
                //int pH = (int)((float)gv.screenHeight / 200.0f);

                foreach (FloatyText ft in floatyTextPool)
                {
                    if (gv.cc.getDistance(ft.location, new Coordinate(gv.mod.PlayerLastLocationX, gv.mod.PlayerLocationY)) > 3)
                    {
                        continue; //out of range from view so skip drawing floaty message
                    }

                    ft.onDrawTextBox();

                    /*
                    //location.X should be the the props actual map location in squares (not screen location)
                    int xLoc = (ft.location.X + gv.playerOffsetX - gv.mod.PlayerLocationX) * (int)(gv.squareSize * sqrScale);
                    int yLoc = ((ft.location.Y + gv.playerOffsetY - gv.mod.PlayerLocationY) * (int)(gv.squareSize * sqrScale)) - (ft.z);

                    for (int x = 0; x <= 2; x++)
                    {
                        for (int y = 0; y <= 2; y++)
                        {
                            gv.DrawText(ft.value, xLoc + x + mapStartLocXinPixels, yLoc + y + txtH, "bk");
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
                    else if (ft.color.Equals("red"))
                    {
                        colr = "rd";
                    }
                    else
                    {
                        colr = "wh";
                    }
                    gv.DrawText(ft.value, xLoc + mapStartLocXinPixels, yLoc + txtH, colr);
                    */
                }
            }
        }
        public void drawUiLayout()
        {
            //SET PORTRAITS
            foreach (IB2Panel pnl in mainUiLayout.panelList)
            {
                if (pnl.tag.Equals("portraitPanel"))
                {
                    foreach (IB2Portrait ptr in pnl.portraitList)
                    {
                        ptr.show = false;
                    }
                    int index = 0;
                    foreach (Player pc in gv.mod.playerList)
                    {
                        if (pc.IsReadyToAdvanceLevel()) { pnl.portraitList[index].levelUpOn = true; }
                        else { pnl.portraitList[index].levelUpOn = false; }
                        pnl.portraitList[index].show = true;
                        pnl.portraitList[index].ImgFilename = pc.portraitFilename;
                        pnl.portraitList[index].TextHP = pc.hp + "/" + pc.hpMax;
                        pnl.portraitList[index].TextSP = pc.sp + "/" + pc.spMax;
                        if (gv.mod.selectedPartyLeader == index)
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

            mainUiLayout.Draw();
        }

        public void addFloatyText(int sqrX, int sqrY, string value, string color, int length)
        {
            FloatyText floatyBox = new FloatyText(sqrX, sqrY, value, color, length);
            floatyBox.showShadow = true;
            floatyBox.gv = gv;
            floatyBox.linesList.Clear();
            floatyBox.tbWidth = 5 * gv.squareSize;
            floatyBox.AddFormattedTextToTextBox(value);
            //based on number of lines, pick YLoc
            //floatyBox.location.Y = (gridy * (int)(gv.squareSize * sqrScale)) - ((floatyTextBox.linesList.Count / 2) * (gv.fontHeight + gv.fontLineSpacing)) + ((int)(gv.squareSize * sqrScale) / 2);
            floatyBox.tbHeight = (floatyBox.linesList.Count + 1) * (gv.fontHeight + gv.fontLineSpacing);

            floatyTextPool.Add(floatyBox);
        }
        public void addFloatyText(Prop floatyCarrier, string value, string color, int length)
        {
            floatyTextByPixelPool.Add(new FloatyTextByPixel (floatyCarrier, value, color, length));
        }
        
        public void onTouchMain(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType)
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
                        return;
                    }

                    //NEW SYSTEM
                    mainUiLayout.setHover(x, y);

                    //Draw Floaty Text On Mouse Over Prop
                    int offset = gv.playerOffset;
                    if (use11x11)
                    {
                        offset = gv.playerOffsetZoom;
                    }
                    int gridx = ((eX - gv.squareSize) / (int)(gv.squareSize * sqrScale)) + 1;
                    int gridy = (eY) / (int)(gv.squareSize * sqrScale);
                    int actualX = gv.mod.PlayerLocationX + (gridx - offset) - (mapStartLocXinPixels / (int)(gv.squareSize));
                    int actualY = gv.mod.PlayerLocationY + (gridy - offset);
                    //gv.cc.floatyText = "";
                    floatyTextBox.linesList.Clear();
                    if (IsTouchInMapWindow(gridx, gridy))
                    {
                        foreach (Prop p in gv.mod.currentArea.Props)
                        {
                            if ((p.LocationX == actualX) && (p.LocationY == actualY))
                            {
                                if ((!p.MouseOverText.Equals("none")) && ((gv.mod.currentArea.Visible[actualY * gv.mod.currentArea.MapSizeX + actualX] == 1)))
                                {
                                    string text = p.MouseOverText;
                                    floatyTextBox.tbWidth = 5 * gv.squareSize;
                                    floatyTextBox.tbXloc = ((gridx) * (int)(gv.squareSize * sqrScale));
                                    floatyTextBox.AddFormattedTextToTextBox(text);
                                    //based on number of lines, pick YLoc
                                    floatyTextBox.tbYloc = (gridy * (int)(gv.squareSize * sqrScale)) - ((floatyTextBox.linesList.Count / 2) * (gv.fontHeight + gv.fontLineSpacing)) + (gv.squareSize / 2);                                    
                                    floatyTextBox.tbHeight = (floatyTextBox.linesList.Count + 1) * (gv.fontHeight + gv.fontLineSpacing);
                                    //floatyTextBox.linesList.Clear();
                                    

                                    //gv.cc.floatyText = p.MouseOverText;
                                    //int halfWidth = (p.MouseOverText.Length * (gv.fontWidth + gv.fontCharSpacing)) / 2;
                                    //gv.cc.floatyTextLoc = new Coordinate((gridx * gv.squareSize) - mapStartLocXinPixels - halfWidth, gridy * gv.squareSize);
                                }
                            }
                        }
                    }
                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)eX;
                    y = (int)eY;
                    offset = gv.playerOffset;
                    if (use11x11)
                    {
                        offset = gv.playerOffsetZoom;
                    }
                    int gridX = ((eX - gv.squareSize) / (int)(gv.squareSize * sqrScale)) + 1;
                    int gridY = (int)eY / (int)(gv.squareSize * sqrScale);
                    int actualx = gv.mod.PlayerLocationX + (gridX - offset - (mapStartLocXinPixels / (int)(gv.squareSize)));
                    int actualy = gv.mod.PlayerLocationY + (gridY - offset);

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

                    //NEW SYSTEM FOR GLOW
                    //mainUiLayout.setHover(-1, -1);

                    //NEW SYSTEM
                    string rtn = mainUiLayout.getImpact(x, y);

                    //check to see if toggle or button is using IBScript and do script
                    IB2Button btnScript = mainUiLayout.GetButtonByTag(rtn);
                    if (btnScript != null)
                    {
                        if ((btnScript.IBScript.Equals("none")) || (btnScript.IBScript.Equals("")))
                        {
                            //no IBScript so move on
                        }
                        else
                        {
                            gv.cc.doIBScriptBasedOnFilename(btnScript.IBScript, "");
                        }
                    }

                    if (rtn.Equals("tglGrid"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            gv.mod.map_showGrid = false;
                            gv.toggleSettings.map_showGrid = gv.mod.map_showGrid;
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            gv.mod.map_showGrid = true;
                            gv.toggleSettings.map_showGrid = gv.mod.map_showGrid;
                        }
                    }
                    if (rtn.Equals("tglZoom"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            use11x11 = true;
                            gv.toggleSettings.map_use11x11 = use11x11;
                            sqrScale = 0.6364f;
                            gv.cc.addLogText("lime", "zoom out to 11x11 map");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            use11x11 = false;
                            gv.toggleSettings.map_use11x11 = use11x11;
                            sqrScale = 1.0f;
                            gv.cc.addLogText("lime", "zoom in to 7x7 map");
                        }
                        return;
                    }
                    if (rtn.Equals("tglClock"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        tgl.toggleOn = !tgl.toggleOn;
                        showClock = !showClock;
                        gv.toggleSettings.showClock = showClock;
                    }
                    if (rtn.Equals("tglSound"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            gv.mod.playMusic = false;
                            gv.mod.playSoundFx = false;
                            gv.toggleSettings.playSoundFx = gv.mod.playSoundFx;
                            gv.cc.addLogText("lime", "SoundFX Off");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            gv.mod.playMusic = true;
                            gv.mod.playSoundFx = true;
                            gv.toggleSettings.playSoundFx = gv.mod.playSoundFx;
                            gv.cc.addLogText("lime", "SoundFX On");
                        }
                    }
                    if (rtn.Equals("tglDebugMode"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            gv.mod.debugMode = false;
                            gv.toggleSettings.debugMode = gv.mod.debugMode;
                            gv.cc.addLogText("lime", "DebugMode Off");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            gv.mod.debugMode = true;
                            gv.toggleSettings.debugMode = gv.mod.debugMode;
                            gv.cc.addLogText("lime", "DebugMode On");
                        }
                    }
                    if (rtn.Equals("tglFullParty"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            showFullParty = false;
                            gv.toggleSettings.showFullParty = showFullParty;
                            gv.cc.addLogText("lime", "Show Party Leader");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            showFullParty = true;
                            gv.toggleSettings.showFullParty = showFullParty;
                            gv.cc.addLogText("lime", "Show Full Party");
                        }
                    }
                    if (rtn.Equals("tglMiniMap"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            showMiniMap = false;
                            gv.toggleSettings.showMiniMap = showMiniMap;
                            gv.cc.addLogText("lime", "Hide Mini Map");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            showMiniMap = true;
                            gv.toggleSettings.showMiniMap = showMiniMap;
                            gv.cc.addLogText("lime", "Show Mini Map");
                        }
                    }
                    if ((rtn.Equals("ctrlUpArrow")) || ((gv.mod.PlayerLocationX == actualx) && ((gv.mod.PlayerLocationY - 1) == actualy)))
                    {
                        
                            if (gv.mod.PlayerLocationY > 0)
                            {
                                if (gv.mod.currentArea.GetBlocked(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY - 1) == false)
                                {
                                    gv.mod.PlayerLastLocationX = gv.mod.PlayerLocationX;
                                    gv.mod.PlayerLastLocationY = gv.mod.PlayerLocationY;
                                    gv.mod.PlayerLocationY--;
                                    gv.cc.doUpdate();
                                }
                            }
                        
                    }
                    else if ((rtn.Equals("ctrlDownArrow")) || ((gv.mod.PlayerLocationX == actualx) && ((gv.mod.PlayerLocationY + 1) == actualy)))
                    {

                        
                            int mapheight = gv.mod.currentArea.MapSizeY;
                            if (gv.mod.PlayerLocationY < (mapheight - 1))
                            {
                                if (gv.mod.currentArea.GetBlocked(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY + 1) == false)
                                {
                                    gv.mod.PlayerLastLocationX = gv.mod.PlayerLocationX;
                                    gv.mod.PlayerLastLocationY = gv.mod.PlayerLocationY;
                                    gv.mod.PlayerLocationY++;
                                    gv.cc.doUpdate();
                                }
                            }
                        
                    }
                    else if ((rtn.Equals("ctrlLeftArrow")) || (((gv.mod.PlayerLocationX - 1) == actualx) && (gv.mod.PlayerLocationY == actualy)))
                    {
                        
                            if (gv.mod.PlayerLocationX > 0)
                            {
                                if (gv.mod.currentArea.GetBlocked(gv.mod.PlayerLocationX - 1, gv.mod.PlayerLocationY) == false)
                                {
                                    gv.mod.PlayerLastLocationX = gv.mod.PlayerLocationX;
                                    gv.mod.PlayerLastLocationY = gv.mod.PlayerLocationY;
                                    gv.mod.PlayerLocationX--;
                                    foreach (Player pc in gv.mod.playerList)
                                    {
                                        if (!pc.combatFacingLeft)
                                        {
                                            pc.combatFacingLeft = true;
                                        }
                                    }
                                    gv.cc.doUpdate();
                                }
                            }
                        
                    }
                    else if ((rtn.Equals("ctrlRightArrow")) || (((gv.mod.PlayerLocationX + 1) == actualx) && (gv.mod.PlayerLocationY == actualy)))
                    {
                        
                            int mapwidth = gv.mod.currentArea.MapSizeX;
                            if (gv.mod.PlayerLocationX < (mapwidth - 1))
                            {
                                if (gv.mod.currentArea.GetBlocked(gv.mod.PlayerLocationX + 1, gv.mod.PlayerLocationY) == false)
                                {
                                    gv.mod.PlayerLastLocationX = gv.mod.PlayerLocationX;
                                    gv.mod.PlayerLastLocationY = gv.mod.PlayerLocationY;
                                    gv.mod.PlayerLocationX++;
                                    foreach (Player pc in gv.mod.playerList)
                                    {
                                        if (pc.combatFacingLeft)
                                        {
                                            pc.combatFacingLeft = false;
                                        }
                                    }
                                    gv.cc.doUpdate();
                                }
                            }
                        
                    }
                    else if (rtn.Equals("btnParty"))
                    {
                        gv.screenParty.resetPartyScreen();
                        gv.screenType = "party";
                        gv.cc.tutorialMessageParty(false);
                    }
                    else if ((rtn.Equals("port0")) && (gv.mod.playerList.Count > 0))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            gv.mod.selectedPartyLeader = 0;
                            gv.cc.partyScreenPcIndex = 0;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            gv.mod.selectedPartyLeader = 0;
                            gv.cc.partyScreenPcIndex = 0;
                        }
                    }
                    else if ((rtn.Equals("port1")) && (gv.mod.playerList.Count > 1))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            gv.mod.selectedPartyLeader = 1;
                            gv.cc.partyScreenPcIndex = 1;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            gv.mod.selectedPartyLeader = 1;
                            gv.cc.partyScreenPcIndex = 1;
                        }
                    }
                    else if ((rtn.Equals("port2")) && (gv.mod.playerList.Count > 2))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            gv.mod.selectedPartyLeader = 2;
                            gv.cc.partyScreenPcIndex = 2;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            gv.mod.selectedPartyLeader = 2;
                            gv.cc.partyScreenPcIndex = 2;
                        }
                    }
                    else if ((rtn.Equals("port3")) && (gv.mod.playerList.Count > 3))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            gv.mod.selectedPartyLeader = 3;
                            gv.cc.partyScreenPcIndex = 3;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            gv.mod.selectedPartyLeader = 3;
                            gv.cc.partyScreenPcIndex = 3;
                        }
                    }
                    else if ((rtn.Equals("port4")) && (gv.mod.playerList.Count > 4))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            gv.mod.selectedPartyLeader = 4;
                            gv.cc.partyScreenPcIndex = 4;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            gv.mod.selectedPartyLeader = 4;
                            gv.cc.partyScreenPcIndex = 4;
                        }
                    }
                    else if ((rtn.Equals("port5")) && (gv.mod.playerList.Count > 5))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            gv.mod.selectedPartyLeader = 5;
                            gv.cc.partyScreenPcIndex = 5;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            gv.mod.selectedPartyLeader = 5;
                            gv.cc.partyScreenPcIndex = 5;
                        }
                    }
                    else if (rtn.Equals("btnInventory"))
                    {
                        gv.screenType = "inventory";
                        gv.screenInventory.resetInventory();
                        gv.cc.tutorialMessageInventory(false);
                    }
                    else if (rtn.Equals("btnJournal"))
                    {
                        gv.screenType = "journal";
                    }
                    else if (rtn.Equals("btnSettings"))
                    {
                        //gv.cc.doSettingsDialogs();
                        foreach (IB2Panel pnl in mainUiLayout.panelList)
                        {
                            if (pnl.tag.Equals("TogglePanel"))
                            {
                                showTogglePanel = !showTogglePanel;
                                gv.toggleSettings.showTogglePanel = showTogglePanel;
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
                    else if (rtn.Equals("tglLog"))
                    {
                        //gv.cc.doSettingsDialogs();
                        foreach (IB2Panel pnl in mainUiLayout.panelList)
                        {
                            if (pnl.tag.Equals("logPanel"))
                            {
                                //hides left
                                if (pnl.currentLocX < pnl.shownLocX)
                                {
                                    pnl.showing = true;                                    
                                }
                                else
                                {
                                    pnl.hiding = true;                                    
                                }
                            }
                        }
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            gv.toggleSettings.showLogPanel = false;
                            gv.cc.addLogText("lime", "Hide Log");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            gv.toggleSettings.showLogPanel = true;
                            gv.cc.addLogText("lime", "Show Log");
                        }
                    }
                    else if (rtn.Equals("btnSave"))
                    {
                        if (gv.mod.allowSave)
                        {
                            //gv.cc.doSavesDialog();
                            gv.cc.doSavesSetupDialog();
                        }
                    }
                    else if (rtn.Equals("btnWait"))
                    {
                        gv.cc.doUpdate();
                    }
                    else if (rtn.Equals("btnCastOnMainMap"))
                    {
                        doCastSelectorSetup();                        
                    }
                    else if (rtn.Equals("btnTraitUseOnMainMap"))
                    {
                        doTraitUserSelectorSetup();
                    }
                    else if (rtn.Equals("btnToggleArrows"))
                    {
                        foreach (IB2Panel pnl in mainUiLayout.panelList)
                        {
                            if (pnl.tag.Equals("arrowPanel"))
                            {
                                showArrows = !showArrows;
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
            }
        }
        public void doCastSelectorSetup()
        {
            List<int> pcIndex = new List<int>();
            //If only one PC, do not show select PC dialog...just go to cast selector
            if (pcIndex.Count == 1)
            {
                try
                {
                    gv.screenCastSelector.castingPlayerIndex = pcIndex[0];
                    gv.screenCombat.spellSelectorIndex = 0;
                    gv.screenType = "mainMapCast";
                    return;
                }
                catch (Exception ex)
                {
                    //print error
                    gv.sf.MessageBoxHtml("error with Pc Selector screen: " + ex.ToString());
                    gv.errorLog(ex.ToString());
                    return;
                }
            }

            List<string> pcNames = new List<string>();            
            pcNames.Add("cancel");

            int cnt = 0;
            foreach (Player p in gv.mod.playerList)
            {
                if (p.isAlive())
                {
                    if (hasMainMapTypeSpell(p))
                    {
                        pcNames.Add(p.name);
                        pcIndex.Add(cnt);
                    }
                }
                cnt++;
            }
            
            gv.itemListSelector.setupIBminiItemListSelector(gv, pcNames, "Select Caster", "mainmapselectcaster");
            gv.itemListSelector.showIBminiItemListSelector = true;
        }
        public void doCastSelector(int selectedIndex)
        {
            if (selectedIndex > 0)
            {
                List<int> pcIndex = new List<int>();
                int cnt = 0;
                foreach (Player p in gv.mod.playerList)
                {
                    if (hasMainMapTypeSpell(p))
                    {
                        pcIndex.Add(cnt);
                    }
                    cnt++;
                }
                try
                {
                    gv.screenCastSelector.castingPlayerIndex = pcIndex[selectedIndex - 1]; // pcIndex.get(item - 1);
                    gv.screenCombat.spellSelectorIndex = 0;
                    gv.screenType = "mainMapCast";
                }
                catch (Exception ex)
                {
                    gv.sf.MessageBoxHtml("error with Pc Selector screen: " + ex.ToString());
                    gv.errorLog(ex.ToString());
                    //print error
                }
            }
            else if (selectedIndex == 0) // selected "cancel"
            {
                //do nothing
            }
        }
        public void doTraitUserSelectorSetup()
        {
            List<int> pcIndex = new List<int>();
            //If only one PC, do not show select PC dialog...just go to cast selector
            if (pcIndex.Count == 1)
            {
                try
                {
                    gv.screenTraitUseSelector.traitUsingPlayerIndex = pcIndex[0];
                    gv.screenCombat.traitUseSelectorIndex = 0;
                    gv.screenType = "mainMapTraitUse";
                    return;
                }
                catch (Exception ex)
                {
                    //print error
                    gv.sf.MessageBoxHtml("error with Pc Selector screen: " + ex.ToString());
                    gv.errorLog(ex.ToString());
                    return;
                }
            }

            List<string> pcNames = new List<string>();
            pcNames.Add("cancel");

            int cnt = 0;
            foreach (Player p in gv.mod.playerList)
            {
                if (p.isAlive())
                {
                    if (hasMainMapTypeTrait(p))
                    {
                        pcNames.Add(p.name);
                        pcIndex.Add(cnt);
                    }
                }
                cnt++;
            }

            gv.itemListSelector.setupIBminiItemListSelector(gv, pcNames, "Select Trait User", "mainmapselecttraituser");
            gv.itemListSelector.showIBminiItemListSelector = true;
        }
        public void doTraitUserSelector(int selectedIndex)
        {
            if (selectedIndex > 0)
            {
                List<int> pcIndex = new List<int>();
                int cnt = 0;
                foreach (Player p in gv.mod.playerList)
                {
                    if (hasMainMapTypeTrait(p))
                    {
                        pcIndex.Add(cnt);
                    }
                    cnt++;
                }
                try
                {
                    gv.screenTraitUseSelector.traitUsingPlayerIndex = pcIndex[selectedIndex - 1]; // pcIndex.get(item - 1);
                    gv.screenCombat.traitUseSelectorIndex = 0;
                    gv.screenType = "mainMapTraitUse";
                }
                catch (Exception ex)
                {
                    gv.sf.MessageBoxHtml("error with Pc Selector screen: " + ex.ToString());
                    gv.errorLog(ex.ToString());
                    //print error
                }
            }
            else if (selectedIndex == 0) // selected "cancel"
            {
                //do nothing
            }
        }

        public void onKeyUp(Keys keyData)
        {
            if ((moveDelay()) && (finishedMove))
            {
                if (keyData == Keys.Left | keyData == Keys.D4 | keyData == Keys.NumPad4)
                {
                    
                        moveLeft();
                    
                }
                else if (keyData == Keys.Right | keyData == Keys.D6 | keyData == Keys.NumPad6)
                {
                    
                        moveRight();
                    
                }
                else if (keyData == Keys.Up | keyData == Keys.D8 | keyData == Keys.NumPad8)
                {
                    
                        moveUp();
                    
                }
                else if (keyData == Keys.Down | keyData == Keys.D2 | keyData == Keys.NumPad2)
                {
                    
                        moveDown();
                    
                }
                else { }
            }
            if (keyData == Keys.Q)
            {
                if (gv.mod.allowSave)
                {
                    gv.cc.QuickSave();
                    gv.cc.addLogText("lime", "Quicksave Completed");
                }
                else
                {
                    gv.cc.addLogText("red", "No save allowed at this time.");
                }
            }
            else if (keyData == Keys.D)
            {
                IB2ToggleButton tgl = mainUiLayout.GetToggleByTag("tglDebugMode");
                if (tgl == null) { return; }
                if (gv.mod.debugMode)
                {
                    tgl.toggleOn = false;
                    gv.mod.debugMode = false;
                    gv.cc.addLogText("lime", "DebugMode Turned Off");
                }
                else
                {
                    tgl.toggleOn = true;
                    gv.mod.debugMode = true;
                    gv.cc.addLogText("lime", "DebugMode Turned On");
                }
            }
            else if (keyData == Keys.I)
            {
                gv.screenType = "inventory";
                gv.screenInventory.resetInventory();
                gv.cc.tutorialMessageInventory(false);
            }
            else if (keyData == Keys.J)
            {
                gv.screenType = "journal";
            }
            else if (keyData == Keys.P)
            {
                gv.screenParty.resetPartyScreen();
                gv.screenType = "party";
                gv.cc.tutorialMessageParty(false);
            }
            else if (keyData == Keys.C)
            {
                doCastSelectorSetup();
                /*List<string> pcNames = new List<string>();
                List<int> pcIndex = new List<int>();
                pcNames.Add("cancel");

                int cnt = 0;
                foreach (Player p in gv.mod.playerList)
                {
                    if (hasMainMapTypeSpell(p))
                    {
                        pcNames.Add(p.name);
                        pcIndex.Add(cnt);
                    }
                    cnt++;
                }

                //If only one PC, do not show select PC dialog...just go to cast selector
                if (pcIndex.Count == 1)
                {
                    try
                    {
                        gv.screenCastSelector.castingPlayerIndex = pcIndex[0];
                        gv.screenCombat.spellSelectorIndex = 0;
                        gv.screenType = "mainMapCast";
                        return;
                    }
                    catch (Exception ex)
                    {
                        //print error
                        IBMessageBox.Show(gv, "error with Pc Selector screen: " + ex.ToString());
                        gv.errorLog(ex.ToString());
                        return;
                    }
                }

                using (ItemListSelector pcSel = new ItemListSelector(gv, pcNames, "Select Caster"))
                {
                    pcSel.ShowDialog();

                    if (pcSel.selectedIndex > 0)
                    {
                        try
                        {
                            gv.screenCastSelector.castingPlayerIndex = pcIndex[pcSel.selectedIndex - 1]; // pcIndex.get(item - 1);
                            gv.screenCombat.spellSelectorIndex = 0;
                            gv.screenType = "mainMapCast";
                        }
                        catch (Exception ex)
                        {
                            IBMessageBox.Show(gv, "error with Pc Selector screen: " + ex.ToString());
                            gv.errorLog(ex.ToString());
                            //print error
                        }
                    }
                    else if (pcSel.selectedIndex == 0) // selected "cancel"
                    {
                        //do nothing
                    }
                }*/
            }
            /*else if (keyData == Keys.X)
            {
                if (!hideClock)
                {
                    hideClock = true;
                }
                else
                {
                    hideClock = false;
                }
                foreach (IB2Panel pnl in mainUiLayout.panelList)
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
        }
        private bool moveDelay()
        {
            long elapsed = DateTime.Now.Ticks - timeStamp;
            if (elapsed > 10000 * movementDelayInMiliseconds) //10,000 ticks in 1 ms
            {
                timeStamp = DateTime.Now.Ticks;
                return true;
            }
            return false;
        }
        private void moveLeft()
        {
            if (gv.mod.PlayerLocationX > 0)
            {
                if (gv.mod.currentArea.GetBlocked(gv.mod.PlayerLocationX - 1, gv.mod.PlayerLocationY) == false)
                {
                   
                    //gv.mod.blockTrigger = false;
                    //gv.mod.blockTriggerMovingProp = false;

                    gv.mod.PlayerLastLocationX = gv.mod.PlayerLocationX;
                    gv.mod.PlayerLastLocationY = gv.mod.PlayerLocationY;
                    gv.mod.PlayerLocationX--;
                    foreach (Player pc in gv.mod.playerList)
                    {
                        if (!pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = true;
                        }
                    }
                    gv.cc.doUpdate();
                }
            }
        }
        private void moveRight()
        {
            int mapwidth = gv.mod.currentArea.MapSizeX;
            if (gv.mod.PlayerLocationX < (mapwidth - 1))
            {
                if (gv.mod.currentArea.GetBlocked(gv.mod.PlayerLocationX + 1, gv.mod.PlayerLocationY) == false)
                {
                    
                    //gv.mod.blockTrigger = false;
                    //gv.mod.blockTriggerMovingProp = false;
                   
                    gv.mod.PlayerLastLocationX = gv.mod.PlayerLocationX;
                    gv.mod.PlayerLastLocationY = gv.mod.PlayerLocationY;
                    gv.mod.PlayerLocationX++;
                    foreach (Player pc in gv.mod.playerList)
                    {
                        if (pc.combatFacingLeft)
                        {
                            pc.combatFacingLeft = false;
                        }
                    }
                    gv.cc.doUpdate();
                }
            }
        }
        private void moveUp()
        {
            if (gv.mod.PlayerLocationY > 0)
            {
                if (gv.mod.currentArea.GetBlocked(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY - 1) == false)
                {

                    //gv.mod.blockTrigger = false;
                    //gv.mod.blockTriggerMovingProp = false;

                    gv.mod.PlayerLastLocationX = gv.mod.PlayerLocationX;
                    gv.mod.PlayerLastLocationY = gv.mod.PlayerLocationY;
                    gv.mod.PlayerLocationY--;
                    gv.cc.doUpdate();
                }
            }
        }
        private void moveDown()
        {
            int mapheight = gv.mod.currentArea.MapSizeY;
            if (gv.mod.PlayerLocationY < (mapheight - 1))
            {
                if (gv.mod.currentArea.GetBlocked(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY + 1) == false)
                {
                   
                    //gv.mod.blockTrigger = false;
                    //gv.mod.blockTriggerMovingProp = false;

                    gv.mod.PlayerLastLocationX = gv.mod.PlayerLocationX;
                    gv.mod.PlayerLastLocationY = gv.mod.PlayerLocationY;
                    gv.mod.PlayerLocationY++;
                    gv.cc.doUpdate();
                }
            }
        }

        public List<string> wrapList(string str, int wrapLength)
        {
            if (str == null)
            {
                return null;
            }
            if (wrapLength < 1)
            {
                wrapLength = 1;
            }
            int inputLineLength = str.Length;
            int offset = 0;
            List<string> returnList = new List<string>();

            while ((inputLineLength - offset) > wrapLength)
            {
                if (str.ElementAt(offset) == ' ')
                {
                    offset++;
                    continue;
                }

                int spaceToWrapAt = str.LastIndexOf(' ', wrapLength + offset);

                if (spaceToWrapAt >= offset)
                {
                    // normal case
                    returnList.Add(str.Substring(offset, spaceToWrapAt));
                    offset = spaceToWrapAt + 1;
                }
                else
                {
                    // do not wrap really long word, just extend beyond limit
                    spaceToWrapAt = str.IndexOf(' ', wrapLength + offset);
                    if (spaceToWrapAt >= 0)
                    {
                        returnList.Add(str.Substring(offset, spaceToWrapAt));
                        offset = spaceToWrapAt + 1;
                    }
                    else
                    {
                        returnList.Add(str.Substring(offset));
                        offset = inputLineLength;
                    }
                }
            }

            // Whatever is left in line is short enough to just pass through
            returnList.Add(str.Substring(offset));
            return returnList;
        }
        private void setExplored()
        {
            // set current position to visible
            if (gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX < gv.mod.currentArea.Visible.Count)
            {
                gv.mod.currentArea.Visible[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX] = 1;
            }
            // set tiles to visible around the PC
            for (int x = gv.mod.PlayerLocationX - gv.mod.currentArea.AreaVisibleDistance; x <= gv.mod.PlayerLocationX + gv.mod.currentArea.AreaVisibleDistance; x++)
            {
                for (int y = gv.mod.PlayerLocationY - gv.mod.currentArea.AreaVisibleDistance; y <= gv.mod.PlayerLocationY + gv.mod.currentArea.AreaVisibleDistance; y++)
                {
                    int xx = x;
                    int yy = y;
                    if (xx < 1) { xx = 0; }
                    if (xx > (gv.mod.currentArea.MapSizeX - 1)) { xx = (gv.mod.currentArea.MapSizeX - 1); }
                    if (yy < 1) { yy = 0; }
                    if (yy > (gv.mod.currentArea.MapSizeY - 1)) { yy = (gv.mod.currentArea.MapSizeY - 1); }
                    if (IsLineOfSightForEachCorner(new Coordinate(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY), new Coordinate(xx, yy)))
                    {
                        gv.mod.currentArea.Visible[yy * gv.mod.currentArea.MapSizeX + xx] = 1;
                    }
                }
            }
            //make all adjacent squares visible
            int minX = gv.mod.PlayerLocationX - 1;
            if (minX < 0) { minX = 0; }
            int minY = gv.mod.PlayerLocationY - 1;
            if (minY < 0) { minY = 0; }

            int maxX = gv.mod.PlayerLocationX + 1;
            if (maxX > this.gv.mod.currentArea.MapSizeX - 1) { maxX = this.gv.mod.currentArea.MapSizeX - 1; }
            int maxY = gv.mod.PlayerLocationY + 1;
            if (maxY > this.gv.mod.currentArea.MapSizeY - 1) { maxY = this.gv.mod.currentArea.MapSizeY - 1; }

            for (int xx = minX; xx <= maxX; xx++)
            {
                for (int yy = minY; yy <= maxY; yy++)
                {
                    gv.mod.currentArea.Visible[yy * gv.mod.currentArea.MapSizeX + xx] = 1;
                }
            }
        }
        public bool IsTouchInMapWindow(int sqrX, int sqrY)
        {
            //all input coordinates are in Screen Location, not Map Location
            if ((sqrX < 0) || (sqrY < 0))
            {
                return false;
            }
            if ((sqrX > 19) || (sqrY > 10))
            {
                return false;
            }
            return true;
        }
        public bool IsLineOfSightForEachCorner(Coordinate s, Coordinate e)
        {
            //start is at the center of party location square
            Coordinate start = new Coordinate((s.X * (int)(gv.squareSize * sqrScale)) + ((int)(gv.squareSize * sqrScale) / 2), (s.Y * (int)(gv.squareSize * sqrScale)) + ((int)(gv.squareSize * sqrScale) / 2));
            //check center of all four sides of the end square
            int halfSquare = ((int)(gv.squareSize * sqrScale) / 2);
            //left side center
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * (int)(gv.squareSize * sqrScale), e.Y * (int)(gv.squareSize * sqrScale) + halfSquare), e)) { return true; }
            //right side center
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * (int)(gv.squareSize * sqrScale) + (int)(gv.squareSize * sqrScale), e.Y * (int)(gv.squareSize * sqrScale) + halfSquare), e)) { return true; }
            //top side center
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * (int)(gv.squareSize * sqrScale) + halfSquare, e.Y * (int)(gv.squareSize * sqrScale)), e)) { return true; }
            //bottom side center
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * (int)(gv.squareSize * sqrScale) + halfSquare, e.Y * (int)(gv.squareSize * sqrScale) + (int)(gv.squareSize * sqrScale)), e)) { return true; }

            return false;
        }
        public bool IsVisibleLineOfSight(Coordinate s, Coordinate e, Coordinate endSquare)
        {
            // Bresenham Line algorithm
            Coordinate start = s;
            Coordinate end = e;
            int deltax = Math.Abs(end.X - start.X);
            int deltay = Math.Abs(end.Y - start.Y);
            int ystep = 10;
            int xstep = 10;
            int gridx = 0;
            int gridy = 0;
            int gridXdelayed = s.X;
            int gridYdelayed = s.Y;

            //gv.DrawLine(end.X + gv.oXshift, end.Y + gv.oYshift, start.X + gv.oXshift, start.Y + gv.oYshift, Color.Lime, 1);
            
            #region low angle version
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
                        gridx = nextPoint.X / (int)(gv.squareSize * sqrScale);
                        gridy = nextPoint.Y / (int)(gv.squareSize * sqrScale);
                        if (gridx < 1) { gridx = 0; }
                        if (gridx > (gv.mod.currentArea.MapSizeX - 1)) { gridx = (gv.mod.currentArea.MapSizeX - 1); }
                        if (gridy < 1) { gridy = 0; }
                        if (gridy > (gv.mod.currentArea.MapSizeY - 1)) { gridy = (gv.mod.currentArea.MapSizeY - 1); }
                        if (gv.mod.currentArea.LoSBlocked[gridy * gv.mod.currentArea.MapSizeX + gridx] == 1)
                        {
                            if ((gridx == endSquare.X) && (gridy == endSquare.Y))
                            {
                                //you are on the end square so return true
                                return true;
                            }
                            else
                            {
                                return false;
                            }
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
                        gridx = nextPoint.X / (int)(gv.squareSize * sqrScale);
                        gridy = nextPoint.Y / (int)(gv.squareSize * sqrScale);
                        if (gridx < 1) { gridx = 0; }
                        if (gridx > (gv.mod.currentArea.MapSizeX - 1)) { gridx = (gv.mod.currentArea.MapSizeX - 1); }
                        if (gridy < 1) { gridy = 0; }
                        if (gridy > (gv.mod.currentArea.MapSizeY - 1)) { gridy = (gv.mod.currentArea.MapSizeY - 1); }
                        if (gv.mod.currentArea.LoSBlocked[gridy * gv.mod.currentArea.MapSizeX + gridx] == 1)
                        {
                            if ((gridx == endSquare.X) && (gridy == endSquare.Y))
                            {
                                //you are on the end square so return true
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            #endregion

            #region steep version
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
                        gridx = nextPoint.X / (int)(gv.squareSize * sqrScale);
                        gridy = nextPoint.Y / (int)(gv.squareSize * sqrScale);
                        if (gridx < 1) { gridx = 0; }
                        if (gridx > (gv.mod.currentArea.MapSizeX - 1)) { gridx = (gv.mod.currentArea.MapSizeX - 1); }
                        if (gridy < 1) { gridy = 0; }
                        if (gridy > (gv.mod.currentArea.MapSizeY - 1)) { gridy = (gv.mod.currentArea.MapSizeY - 1); }
                        if (gv.mod.currentArea.LoSBlocked[gridy * gv.mod.currentArea.MapSizeX + gridx] == 1)
                        {
                            if ((gridx == endSquare.X) && (gridy == endSquare.Y))
                            {
                                //you are on the end square so return true
                                return true;
                            }
                            else
                            {
                                return false;
                            }
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
                        gridx = nextPoint.X / (int)(gv.squareSize * sqrScale);
                        gridy = nextPoint.Y / (int)(gv.squareSize * sqrScale);
                        if (gridx < 1) { gridx = 0; }
                        if (gridx > (gv.mod.currentArea.MapSizeX - 1)) { gridx = (gv.mod.currentArea.MapSizeX - 1); }
                        if (gridy < 1) { gridy = 0; }
                        if (gridy > (gv.mod.currentArea.MapSizeY - 1)) { gridy = (gv.mod.currentArea.MapSizeY - 1); }
                        if (gv.mod.currentArea.LoSBlocked[gridy * gv.mod.currentArea.MapSizeX + gridx] == 1)
                        {
                            if ((gridx == endSquare.X) && (gridy == endSquare.Y))
                            {
                                //you are on the end square so return true
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            #endregion

            return true;
        }
        public bool hasMainMapTypeSpell(Player pc)
        {
            foreach (string s in pc.knownSpellsTags)
            {
                Spell sp = gv.mod.getSpellByTag(s);
                if (sp == null) { continue; }
                if ((sp.useableInSituation.Equals("Always")) || (sp.useableInSituation.Equals("OutOfCombat")))
                {
                    return true;
                }
            }
            return false;
        }
        public bool hasMainMapTypeTrait(Player pc)
        {
            foreach (string s in pc.knownTraitsTags)
            {
                Trait tr = gv.mod.getTraitByTag(s);
                if (tr == null) { continue; }
                if ((tr.useableInSituation.Equals("Always")) || (tr.useableInSituation.Equals("OutOfCombat")))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
