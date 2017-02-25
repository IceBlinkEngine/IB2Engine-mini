using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2mini
{
    public class ScreenLauncher 
    {
	    private Module mod;
	    private GameView gv;
	
	    private IbbButton btnLeft = null;
	    private IbbButton btnRight = null;
	    private IbbButton btnModuleName = null;
        private IbbButton btnGetUpdates = null;
        private IBminiTextBox description;
	    //private List<Module> moduleList = new List<Module>();
        private List<ModuleInfo> moduleInfoList = new List<ModuleInfo>();
        public List<ModuleInfo> modsAvailableList = new List<ModuleInfo>();
        private List<Bitmap> titleList = new List<Bitmap>();
	    private int moduleIndex = 0;
	
	
	    public ScreenLauncher(Module m, GameView g) 
	    {
		    mod = m;
		    gv = g;
		    setControlsStart();
            int pH = (int)((float)gv.screenHeight / 100.0f);
            description = new IBminiTextBox(gv);
            description.tbXloc = 0 * gv.squareSize;
            description.tbYloc = 6 * gv.squareSize;
            description.tbWidth = 16 * gv.squareSize;
            description.tbHeight = 6 * gv.squareSize;
            description.showBoxBorder = false;
	    }
	
        //when click on "Get Updates"
        //download mod_available.json from server
        public void downloadFile(string filename, string outFolder)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFile("http://www.iceblinkengine.com/ibmini_modules/" + filename, outFolder + "\\" + filename);
                    MessageBox.Show("Completed Downloading: " + filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Downloading: " + ex.ToString());
                }
            }
        }
        //convert to object
        public void loadModsAvailableList()
        {
            modsAvailableList.Clear();
            try
            {
                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(gv.mainDirectory + "\\modules\\mods_available.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    modsAvailableList = (List<ModuleInfo>)serializer.Deserialize(file, typeof(List<ModuleInfo>));
                }
            }
            catch { }
        }
        //compare to moduleInfoList and add any that are not there and assign button name
        public void setupModuleInfoListAndButtonText()
        {
            //go through all moduleInfoList items and set buttonText to PLAY
            foreach (ModuleInfo modInfo in moduleInfoList)
            {
                modInfo.buttonText = "PLAY";
            }
            //go through each item in modsAvailableList and see if is in moduleInfoList
            foreach (ModuleInfo modAvail in modsAvailableList)
            {
                bool foundOne = false;
                foreach (ModuleInfo modInfo in moduleInfoList)
                {
                    if (modAvail.moduleName.Equals(modInfo.moduleName))
                    {
                        foundOne = true;
                        //if is there check versions and set to UPDATE or PLAY
                        if (modAvail.moduleVersion > modInfo.moduleVersion)
                        {
                            modInfo.buttonText = "UPDATE";
                        }
                    }
                }
                if (!foundOne)
                {
                    //if not there, add to list and set to DOWNLOAD
                    modAvail.buttonText = "DOWNLOAD";
                    moduleInfoList.Add(modAvail);
                }
            }            
        }
        
        /*public void loadModuleFiles()
        {
            string[] files;

            files = Directory.GetFiles(gv.mainDirectory + "\\modules", "*.mod", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileName(file) != "NewModule.mod")
                {
                    // Process each file
                    Module mod = gv.cc.LoadModule(file, true);
                    if (mod == null)
                    {
                        gv.sf.MessageBox("returned a null module");
                    }
                    moduleList.Add(mod);
                    //titleList.Add(gv.cc.LoadBitmap("title", mod));
                    titleList.Add(gv.cc.GetFromBitmapList(mod.titleImageName));
                }
            }
        }*/
        public void loadModuleInfoFiles()
        {
            moduleInfoList.Clear();
            titleList.Clear();
            string[] files;

            files = Directory.GetFiles(gv.mainDirectory + "\\modules", "*.mod", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileName(file) != "NewModule.mod")
                {
                    // Process each file
                    ModuleInfo modinfo = gv.cc.LoadModuleFileInfo(file);
                    if (modinfo == null)
                    {
                        gv.sf.MessageBox("returned a null module");
                    }
                    moduleInfoList.Add(modinfo);
                    titleList.Add(gv.cc.GetFromBitmapList(modinfo.titleImageName));
                }
            }            
        }

        public void setControlsStart()
        {
            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int wideX = (gv.squaresInWidth * gv.squareSize / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2);
            int smallLeftX = wideX - (int)(gv.ibbwidthR * gv.screenDensity);
            int smallRightX = wideX + (int)(gv.ibbwidthL * gv.screenDensity);
            int largeRightX = wideX + (int)(gv.ibbwidthL * gv.screenDensity) + (int)(gv.ibbwidthR * gv.screenDensity) + (int)(gv.ibbwidthR * gv.screenDensity / 2);
            int padW = gv.squareSize / 6;

            if (btnLeft == null)
            {
                btnLeft = new IbbButton(gv, 1.0f);
            }
                btnLeft.Img = "btn_small";
                btnLeft.Img2 = "ctrl_left_arrow";
                btnLeft.Glow = "btn_small_glow";
                btnLeft.X = smallLeftX;
                btnLeft.Y = (2 * gv.squareSize) + (gv.squareSize / 6);                
                btnLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnModuleName == null)
            {
                btnModuleName = new IbbButton(gv, 1.0f);
            }
                btnModuleName.Img = "btn_large";
                btnModuleName.Glow = "btn_large_glow";
                btnModuleName.Text = "";
                btnModuleName.X = wideX;
                btnModuleName.Y = (2 * gv.squareSize) + (gv.squareSize / 6);                
                btnModuleName.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnModuleName.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            if (btnRight == null)
            {
                btnRight = new IbbButton(gv, 1.0f);
            }
                btnRight.Img = "btn_small";
                btnRight.Img2 = "ctrl_right_arrow";
                btnRight.Glow = "btn_small_glow";
                btnRight.X = smallRightX;
                btnRight.Y = (2 * gv.squareSize) + (gv.squareSize / 6);
                btnRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnGetUpdates == null)
            {
                btnGetUpdates = new IbbButton(gv, 1.0f);
            }
                btnGetUpdates.Img = "btn_large";
                btnGetUpdates.Glow = "btn_large_glow";
                btnGetUpdates.Text = "GET UPDATES";
                btnGetUpdates.X = wideX;
                btnGetUpdates.Y = (6 * gv.squareSize) - (pH * 2);
                btnGetUpdates.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnGetUpdates.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            
        }
        //TITLE SCREEN  
        public void redrawLauncher()
        {
            int titleW = gv.squareSize * 4;
            int titleH = gv.squareSize * 2;
            int titleX = (gv.squaresInWidth * gv.squareSize / 2) - (gv.squareSize * 2);
            //DRAW TITLE SCREEN
    	    if ((titleList.Count > 0) && (moduleIndex < titleList.Count))
		    {
                IbRect src = new IbRect(0, 0, titleList[moduleIndex].PixelSize.Width, titleList[moduleIndex].PixelSize.Height);
                IbRect dst = new IbRect(titleX, 0, titleW, titleH);
                gv.DrawBitmap(titleList[moduleIndex], src, dst);
		    }

            

            //DRAW DESCRIPTION BOX
            if ((moduleInfoList.Count > 0) && (moduleIndex < moduleInfoList.Count))
		    {
                btnModuleName.Text = moduleInfoList[moduleIndex].buttonText + " MODULE";
                drawLauncherControls();

                string textToSpan = "<gn>" + moduleInfoList[moduleIndex].moduleLabelName + "</gn><br>";
                description.tbXloc = 1 * gv.squareSize / 2;
                description.tbYloc = 3 * gv.squareSize + (gv.squareSize / 4);
                description.tbWidth = 10 * gv.squareSize + gv.squareSize / 2;
                description.tbHeight = 6 * gv.squareSize;
                textToSpan += moduleInfoList[moduleIndex].moduleDescription;
                description.linesList.Clear();
                description.AddFormattedTextToTextBox(textToSpan);
                description.onDrawTextBox();                	    	    
		    }
        }
        public void drawLauncherControls()
	    {    	
		    btnLeft.Draw();		
		    btnRight.Draw();
		    btnModuleName.Draw();
            btnGetUpdates.Draw();
	    }
        public void onTouchLauncher(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType)
	    {
    	    btnLeft.glowOn = false;
    	    btnRight.glowOn = false;	
    	    btnModuleName.glowOn = false;
            btnGetUpdates.glowOn = false;
		
		    switch (eventType)
		    {
		        case MouseEventType.EventType.MouseUp:
			        int x = (int) eX;
			        int y = (int) eY;
				
			        btnLeft.glowOn = false;
	    	        btnRight.glowOn = false;	
	    	        btnModuleName.glowOn = false;
                    btnGetUpdates.glowOn = false;

                    if (btnLeft.getImpact(x, y))
			        {
                        if (moduleIndex > 0)
				        {
					        moduleIndex--;
					        btnModuleName.Text = moduleInfoList[moduleIndex].moduleName;
				        }
			        }
			        else if (btnRight.getImpact(x, y))
			        {
                        if (moduleIndex < moduleInfoList.Count-1)
				        {
					        moduleIndex++;
					        btnModuleName.Text = moduleInfoList[moduleIndex].moduleName;
				        }
			        }	    	
			        else if (btnModuleName.getImpact(x, y))
			        {
                        if (moduleInfoList[moduleIndex].buttonText.Equals("PLAY"))
                        {
                            //load the mod since we only have the ModuleInfo                            
                            gv.mod = gv.cc.LoadModule(moduleInfoList[moduleIndex].moduleName + ".mod");
                            gv.resetGame();
                            gv.cc.LoadSaveListItems();
                            gv.screenType = "title";
                        }
                        else if (moduleInfoList[moduleIndex].buttonText.Equals("UPDATE"))
                        {
                            //download and replace existing file
                            downloadFile(moduleInfoList[moduleIndex].moduleName + ".mod", gv.mainDirectory + "\\modules");
                            //once download is complete, do the "Get Updates" button stuff
                            loadModuleInfoFiles();
                            loadModsAvailableList();
                            setupModuleInfoListAndButtonText();
                        }
                        else if (moduleInfoList[moduleIndex].buttonText.Equals("DOWNLOAD"))
                        {
                            //download file
                            downloadFile(moduleInfoList[moduleIndex].moduleName + ".mod", gv.mainDirectory + "\\modules");
                            //once download is complete, do the "Get Updates" button stuff
                            loadModuleInfoFiles();
                            loadModsAvailableList();
                            setupModuleInfoListAndButtonText();
                        }
                    }
                    else if (btnGetUpdates.getImpact(x, y))
                    {
                        downloadFile("mods_available.json", gv.mainDirectory + "\\modules");
                        loadModsAvailableList();
                        setupModuleInfoListAndButtonText();
                    }
                    break;
		
		        case MouseEventType.EventType.MouseMove:
		        case MouseEventType.EventType.MouseDown:
			        x = (int) eX;
			        y = (int) eY;
				
			        if (btnLeft.getImpact(x, y))
			        {
                        btnLeft.glowOn = true;
			        }
			        else if (btnRight.getImpact(x, y))
			        {
				        btnRight.glowOn = true;
			        }
			        else if (btnModuleName.getImpact(x, y))
			        {
                        btnModuleName.glowOn = true;
			        }
                    else if (btnGetUpdates.getImpact(x, y))
                    {
                        btnGetUpdates.glowOn = true;
                    }
                    break;		
		    }
	    }
    }
}
