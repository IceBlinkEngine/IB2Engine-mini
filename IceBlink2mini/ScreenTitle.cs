using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2mini
{
    public class ScreenTitle 
    {
	    public Module mod;
	    public GameView gv;
	
	    private IbbButton btnNewGame = null;
	    private IbbButton btnLoadSavedGame = null;
	    private IbbButton btnPlayerGuide = null;
	    private IbbButton btnBeginnerGuide = null;
	    private IbbButton btnAbout = null;        

        public ScreenTitle(Module m, GameView g)
	    {
		    mod = m;
		    gv = g;
		    setControlsStart();
	    }
	
	    public void setControlsStart()
	    {
            int pH = (int)((float)gv.screenHeight / 100.0f);

    	    if (btnNewGame == null)
		    {
			    btnNewGame = new IbbButton(gv, 1.0f);	
			    btnNewGame.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnNewGame.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnNewGame.Text = "New Game";
                btnNewGame.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnNewGame.Y = (1 * gv.squareSize) + (2 * pH);
                btnNewGame.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNewGame.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnLoadSavedGame == null)
		    {
			    btnLoadSavedGame = new IbbButton(gv, 1.0f);	
			    btnLoadSavedGame.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnLoadSavedGame.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnLoadSavedGame.Text = "Load Saved Game";
                btnLoadSavedGame.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnLoadSavedGame.Y = (2 * gv.squareSize) + (4 * pH);
                btnLoadSavedGame.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnLoadSavedGame.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnPlayerGuide == null)
		    {
			    btnPlayerGuide = new IbbButton(gv, 1.0f);	
			    btnPlayerGuide.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnPlayerGuide.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnPlayerGuide.Text = "Player's Guide";
                btnPlayerGuide.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnPlayerGuide.Y = (3 * gv.squareSize) + (6 * pH);
                btnPlayerGuide.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPlayerGuide.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }		
		    if (btnBeginnerGuide == null)
		    {
			    btnBeginnerGuide = new IbbButton(gv, 1.0f);	
			    btnBeginnerGuide.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnBeginnerGuide.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnBeginnerGuide.Text = "Beginner's Guide";
                btnBeginnerGuide.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnBeginnerGuide.Y = (4 * gv.squareSize) + (8 * pH);
                btnBeginnerGuide.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnBeginnerGuide.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnAbout == null)
		    {
			    btnAbout = new IbbButton(gv, 1.0f);	
			    btnAbout.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnAbout.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnAbout.Text = "Credits";
                btnAbout.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnAbout.Y = (5 * gv.squareSize) + (10 * pH);
                btnAbout.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAbout.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }		
	    }

	    //TITLE SCREEN  
        public void redrawTitle()
        {                        
    	    //DRAW TITLE SCREEN
            float dstHeight = ((float)gv.screenWidth / (float)gv.cc.title.PixelSize.Width) * (float)gv.cc.title.PixelSize.Height;
            //do narration with image setup    	
            IbRect src = new IbRect(0, 0, gv.cc.title.PixelSize.Width, gv.cc.title.PixelSize.Height);
            IbRect dst = new IbRect(0, 0, gv.screenWidth, (int)dstHeight);
            gv.DrawBitmap(gv.cc.title, src, dst);

            //Draw This Module's Version Number
            int xLoc = (gv.screenWidth / 2) - 4;
            int pH = (int)((float)gv.screenHeight / 100.0f);
            gv.DrawText("v" + mod.moduleVersion, xLoc, (7 * gv.squareSize) + (pH * 4), "wh");
            
            drawTitleControls();
            if (gv.showMessageBox)
            {
                gv.messageBox.onDrawLogBox();
            }
        }
        public void drawTitleControls()
	    {    	
		    btnNewGame.Draw();		
		    btnLoadSavedGame.Draw();		
		    btnPlayerGuide.Draw();
		    btnBeginnerGuide.Draw();           
		    btnAbout.Draw();
	    }
        public void onTouchTitle(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
    	    btnNewGame.glowOn = false;
		    btnLoadSavedGame.glowOn = false;
		    btnPlayerGuide.glowOn = false;
		    btnBeginnerGuide.glowOn = false;				
		    btnAbout.glowOn = false;
            if (gv.showMessageBox)
            {
                gv.messageBox.btnReturn.glowOn = false;
            }

            switch (eventType)
		    {
		    case MouseEventType.EventType.MouseUp:
                int x = (int)e.X;
                int y = (int)e.Y;
				
			    btnNewGame.glowOn = false;
			    btnLoadSavedGame.glowOn = false;
			    btnAbout.glowOn = false;
			    btnPlayerGuide.glowOn = false;
			    btnBeginnerGuide.glowOn = false;
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
                }
                else
                {
                    if (btnNewGame.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        if (gv.mod.mustUsePreMadePC)
                        {
                            //no spell selection offered
                            gv.showMessageBox = true;
                            gv.cc.tutorialMessageMainMap();
                            gv.screenType = "main";
                            gv.cc.doUpdate();
                        }
                        else
                        {
                            gv.screenType = "partyBuild";
                            gv.screenPartyBuild.loadPlayerList();
                        }
                    }
                    else if (btnLoadSavedGame.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        if (gv.cc.slot5.Equals(""))
                        {
                            //Toast.makeText(gv.gameContext, "Still Loading Data... try again in a second", Toast.LENGTH_SHORT).show();
                        }
                        else
                        {
                            gv.cc.doLoadSaveGameSetupDialog();
                        }
                    }
                    else if (btnPlayerGuide.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        gv.showMessageBox = true;
                        gv.cc.tutorialPlayersGuide();
                    }
                    else if (btnBeginnerGuide.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        gv.showMessageBox = true;
                        gv.cc.tutorialBeginnersGuide();
                    }
                    else if (btnAbout.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        gv.showMessageBox = true;
                        gv.cc.doAboutDialog();
                    }
                }					
			    break;

            case MouseEventType.EventType.MouseDown:
            case MouseEventType.EventType.MouseMove:
                x = (int)e.X;
                y = (int)e.Y;

                if (gv.showMessageBox)
                {
                    if (gv.messageBox.btnReturn.getImpact(x, y))
                    {
                        gv.messageBox.btnReturn.glowOn = true;
                    }
                }
                else
                {
                    if (btnNewGame.getImpact(x, y))
                    {
                        btnNewGame.glowOn = true;
                    }
                    else if (btnLoadSavedGame.getImpact(x, y))
                    {
                        btnLoadSavedGame.glowOn = true;
                    }
                    else if (btnAbout.getImpact(x, y))
                    {
                        btnAbout.glowOn = true;
                    }
                    else if (btnPlayerGuide.getImpact(x, y))
                    {
                        btnPlayerGuide.glowOn = true;
                    }
                    else if (btnBeginnerGuide.getImpact(x, y))
                    {
                        btnBeginnerGuide.glowOn = true;
                    }
                }
			    break;		
		    }
	    }
    }
}
