using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private IBminiTextBox description;
	    //private List<Module> moduleList = new List<Module>();
        private List<ModuleInfo> moduleInfoList = new List<ModuleInfo>();
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
            int wideX = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2);
            int smallLeftX = wideX - (int)(gv.ibbwidthR * gv.screenDensity);
            int smallRightX = wideX + (int)(gv.ibbwidthL * gv.screenDensity);
		    int padW = gv.squareSize/6;
		
		    if (btnLeft == null)
		    {
			    btnLeft = new IbbButton(gv, 1.0f);
			    btnLeft.Img = "btn_small";
			    btnLeft.Img2 = "ctrl_left_arrow";
			    btnLeft.Glow = "btn_small_glow";
			    btnLeft.X = smallLeftX;
                btnLeft.Y = (5 * gv.squareSize) - (pH * 2);
                btnLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnModuleName == null)
		    {
			    btnModuleName = new IbbButton(gv, 1.0f);
			    btnModuleName.Img = "btn_large";
			    btnModuleName.Glow = "btn_large_glow";
			    btnModuleName.Text = "";
                btnModuleName.X = wideX;
			    btnModuleName.Y = (5 * gv.squareSize) - (pH * 2);
                btnModuleName.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnModuleName.Width = (int)(gv.ibbwidthL * gv.screenDensity);
		    }
		    if (btnRight == null)
		    {
			    btnRight = new IbbButton(gv, 1.0f);
			    btnRight.Img = "btn_small";
			    btnRight.Img2 = "ctrl_right_arrow";
			    btnRight.Glow = "btn_small_glow";
			    btnRight.X = smallRightX;
			    btnRight.Y = (5 * gv.squareSize) - (pH * 2);
                btnRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }	
	    }

	    //TITLE SCREEN  
        public void redrawLauncher()
        {
            //DRAW TITLE SCREEN
    	    if ((titleList.Count > 0) && (moduleIndex < titleList.Count))
		    {
                IbRect src = new IbRect(0, 0, titleList[moduleIndex].PixelSize.Width, titleList[moduleIndex].PixelSize.Height);
                IbRect dst = new IbRect((gv.screenWidth / 2) - (gv.squareSize * 4), 0, gv.squareSize * 8, gv.squareSize * 4);
                gv.DrawBitmap(titleList[moduleIndex], src, dst);
		    }
            	
    	    //DRAW DESCRIPTION BOX
            if ((moduleInfoList.Count > 0) && (moduleIndex < moduleInfoList.Count))
		    {
                
                string textToSpan = "<gn>Module Description</gn>" + "<br>";
                description.tbXloc = 1 * gv.squareSize;
                description.tbYloc = 6 * gv.squareSize;
                description.tbWidth = 18 * gv.squareSize;
                description.tbHeight = 6 * gv.squareSize;
                textToSpan += moduleInfoList[moduleIndex].moduleDescription;
                description.linesList.Clear();
                description.AddFormattedTextToTextBox(textToSpan);
                description.onDrawTextBox();
                
                btnModuleName.Text = moduleInfoList[moduleIndex].moduleLabelName;
	    	    drawLauncherControls();
		    }
        }
        public void drawLauncherControls()
	    {    	
		    btnLeft.Draw();		
		    btnRight.Draw();
		    btnModuleName.Draw();
	    }
        public void onTouchLauncher(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType)
	    {
    	    btnLeft.glowOn = false;
    	    btnRight.glowOn = false;	
    	    btnModuleName.glowOn = false;
		
		    switch (eventType)
		    {
		        case MouseEventType.EventType.MouseUp:
			        int x = (int) eX;
			        int y = (int) eY;
				
			        btnLeft.glowOn = false;
	    	        btnRight.glowOn = false;	
	    	        btnModuleName.glowOn = false;
			
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
                        //TODO load the mod since we only have the ModuleInfo
				        //gv.mod = moduleInfoList[moduleIndex];
                        gv.mod = gv.cc.LoadModule(moduleInfoList[moduleIndex].moduleName + ".mod");
                        gv.resetGame();
				        gv.cc.LoadSaveListItems();
				        gv.screenType = "title";
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
			        break;		
		    }
	    }
    }
}
