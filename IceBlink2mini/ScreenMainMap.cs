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
        public Module mod;
        public GameView gv;

        public IB2UILayout mainUiLayout = null;
        public bool showMiniMap = false;
        public bool showClock = false;
        public bool showFullParty = false;
        public bool showArrows = true;
        public bool hideClock = false;
        public List<FloatyText> floatyTextPool = new List<FloatyText>();
        public List<FloatyTextByPixel> floatyTextByPixelPool = new List<FloatyTextByPixel>();
        public int mapStartLocXinPixels;
        public int movementDelayInMiliseconds = 100;
        private long timeStamp = 0;
        private bool finishedMove = true;
        public Bitmap minimap = null;
        public List<Sprite> spriteList = new List<Sprite>();

        public ScreenMainMap(Module m, GameView g)
        {
            mod = m;
            gv = g;
            mapStartLocXinPixels = 4 * gv.squareSize + gv.oXshift;
            loadMainUILayout();          
        }
        public void loadMainUILayout()
        {
            try
            {
                if (File.Exists(gv.mainDirectory + "\\override\\MainUILayout.json"))
                {
                    using (StreamReader file = File.OpenText(gv.mainDirectory + "\\override\\MainUILayout.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        mainUiLayout = (IB2UILayout)serializer.Deserialize(file, typeof(IB2UILayout));
                        mainUiLayout.setupIB2UILayout(gv);
                    }
                }
                else
                {
                    using (StreamReader file = File.OpenText(gv.mainDirectory + "\\default\\NewModule\\data\\MainUILayout.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        mainUiLayout = (IB2UILayout)serializer.Deserialize(file, typeof(IB2UILayout));
                        mainUiLayout.setupIB2UILayout(gv);
                    }
                }
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
                foreach (IB2Panel pnl in mainUiLayout.panelList)
                {
                    if (pnl.tag.Equals("logPanel"))
                    {
                        float sqrW = (float)gv.screenWidth / (gv.squaresInWidth + 2f / 10f);
                        float sqrH = (float)gv.screenHeight / (gv.squaresInHeight + 3f / 10f);
                        gv.log = pnl.logList[0];
                        gv.cc.addLogText("red", "screenDensity: " + gv.screenDensity);
                        gv.cc.addLogText("fuchsia", "screenWidth: " + gv.screenWidth);
                        gv.cc.addLogText("lime", "screenHeight: " + gv.screenHeight);
                        gv.cc.addLogText("yellow", "squareSize: " + gv.squareSize);
                        gv.cc.addLogText("yellow", "sqrW: " + sqrW);
                        gv.cc.addLogText("yellow", "sqrH: " + sqrH);
                        gv.cc.addLogText("yellow", "fontWidth: " + gv.fontWidth);
                        gv.cc.addLogText("yellow", "");
                        gv.cc.addLogText("red", "Welcome to " + mod.moduleLabelName);
                        gv.cc.addLogText("fuchsia", "You can scroll this message log box, use mouse wheel");
                        gv.cc.addLogText("yellow", "'x' will hide/show all UI panels");
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Loading MainUILayout.json: " + ex.ToString());
                gv.errorLog(ex.ToString());
            }
        }
        
        //MAIN SCREEN UPDATE
        public void Update(int elapsed)
        {
            mainUiLayout.Update(elapsed);

            //handle RealTime Timer events if module uses this system
            /*if (mod.useRealTimeTimer)
            {
                gv.realTimeTimerMilliSecondsEllapsed += elapsed;
                if (gv.realTimeTimerMilliSecondsEllapsed >= mod.realTimeTimerLengthInMilliSeconds)
                {
                    gv.cc.doUpdate();
                    gv.realTimeTimerMilliSecondsEllapsed = 0;
                }
            }*/

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
            int minimapSquareSizeInPixels = 4 * gv.squareSize / mod.currentArea.MapSizeX;
            int drawW = minimapSquareSizeInPixels * mod.currentArea.MapSizeX;
            int drawH = minimapSquareSizeInPixels * mod.currentArea.MapSizeY;
            using (System.Drawing.Bitmap surface = new System.Drawing.Bitmap(drawW, drawH))
            {
                using (Graphics device = Graphics.FromImage(surface))
                {
                    //draw background image first
                    /*if ((!mod.currentArea.ImageFileName.Equals("none")) && (gv.cc.bmpMap != null))
                    {
                        System.Drawing.Bitmap bg = gv.cc.LoadBitmapGDI(mod.currentArea.ImageFileName);
                        Rectangle srcBG = new Rectangle(0, 0, bg.Width, bg.Height);
                        Rectangle dstBG = new Rectangle(mod.currentArea.backgroundImageStartLocX * minimapSquareSizeInPixels,
                                                        mod.currentArea.backgroundImageStartLocY * minimapSquareSizeInPixels,
                                                        minimapSquareSizeInPixels * (bg.Width / 50),
                                                        minimapSquareSizeInPixels * (bg.Height / 50));
                        device.DrawImage(bg, dstBG, srcBG, GraphicsUnit.Pixel);
                        bg.Dispose();
                        bg = null;
                    }*/
                    #region Draw Layer 1
                    for (int x = 0; x < mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < mod.currentArea.MapSizeY; y++)
                        {
                            string tile = mod.currentArea.Layer1Filename[y * mod.currentArea.MapSizeX + x];
                            Rectangle src = new Rectangle(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);
                            float scalerX = gv.cc.GetFromTileBitmapList(tile).PixelSize.Width / 100;
                            float scalerY = gv.cc.GetFromTileBitmapList(tile).PixelSize.Height / 100;
                            int brX = (int)(minimapSquareSizeInPixels * scalerX);
                            int brY = (int)(minimapSquareSizeInPixels * scalerY);
                            Rectangle dst = new Rectangle(x * minimapSquareSizeInPixels, y * minimapSquareSizeInPixels, brX, brY);
                            device.DrawImage(gv.cc.GetFromTileGDIBitmapList(tile), dst, src, GraphicsUnit.Pixel);
                        }
                    }
                    #endregion
                    #region Draw Layer 2
                    for (int x = 0; x < mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < mod.currentArea.MapSizeY; y++)
                        {
                            string tile = mod.currentArea.Layer2Filename[y * mod.currentArea.MapSizeX + x];
                            Rectangle src = new Rectangle(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);
                            float scalerX = gv.cc.GetFromTileBitmapList(tile).PixelSize.Width / 100;
                            float scalerY = gv.cc.GetFromTileBitmapList(tile).PixelSize.Height / 100;
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
            if (!mod.currentArea.areaDark)
            {
                drawWorldMap();               
                drawProps();
                if (mod.map_showGrid)
                {
                    drawGrid();
                }
            }
            drawPlayer();
            if (!mod.currentArea.areaDark)
            {
                drawMovingProps();
            }
            drawMainMapFloatyText();
            drawFloatyTextPool();
                        
            if (!mod.currentArea.areaDark)
            {
                drawFogOfWar();
                drawSprites();
                bool hideOverlayNeeded = false;
                if (mod.currentArea.UseDayNightCycle)
                {
                    drawOverlayTints();
                    hideOverlayNeeded = true;
                }

                if (hideOverlayNeeded)
                {
                    drawBlackTilesOverTints();
                    hideOverlayNeeded = false;
                }
            } 
           
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
            int minX = mod.PlayerLocationX - gv.playerOffset - 2; //using -2 in case a large tile (3x3) needs to start off the visible map space to be seen
            if (minX < 0) { minX = 0; }
            int minY = mod.PlayerLocationY - gv.playerOffset - 2; //using -2 in case a large tile (3x3) needs to start off the visible map space to be seen
            if (minY < 0) { minY = 0; }

            int maxX = mod.PlayerLocationX + gv.playerOffset + 1;
            if (maxX > this.mod.currentArea.MapSizeX) { maxX = this.mod.currentArea.MapSizeX; }
            int maxY = mod.PlayerLocationY + gv.playerOffset + 1; // use 2 so that extends down to bottom of screen
            if (maxY > this.mod.currentArea.MapSizeY) { maxY = this.mod.currentArea.MapSizeY; }
            */
            #region Draw Layer 1
            for (int x = mod.PlayerLocationX - gv.playerOffset; x <= mod.PlayerLocationX + gv.playerOffset; x++)
            {
                for (int y = mod.PlayerLocationY - gv.playerOffset; y <= mod.PlayerLocationY + gv.playerOffset; y++)
                {
                    //check if valid map location
                    if (x < 0) { continue; }
                    if (y < 0) { continue; }
                    if (x > this.mod.currentArea.MapSizeX - 1) { continue; }
                    if (y > this.mod.currentArea.MapSizeY - 1) { continue; }

                    string tile = mod.currentArea.Layer1Filename[y * mod.currentArea.MapSizeX + x];
                    int tlX = (x - mod.PlayerLocationX + gv.playerOffsetX) * gv.squareSize;
                    int tlY = (y - mod.PlayerLocationY + gv.playerOffsetY) * gv.squareSize;
                    float scalerX = gv.cc.GetFromTileBitmapList(tile).PixelSize.Width / 100;
                    float scalerY = gv.cc.GetFromTileBitmapList(tile).PixelSize.Height / 100;
                    int brX = (int)(gv.squareSize * scalerX);
                    int brY = (int)(gv.squareSize * scalerY);

                    try
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);
                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                        bool mirror = false;
                        if (mod.currentArea.Layer1Mirror[y * mod.currentArea.MapSizeX + x] == 1) { mirror = true; }
                        gv.DrawBitmap(gv.cc.GetFromTileBitmapList(tile), src, dst, mod.currentArea.Layer1Rotate[y * mod.currentArea.MapSizeX + x], mirror);
                    }
                    catch { }
                }
            }
            #endregion
            #region Draw Layer 2
            for (int x = mod.PlayerLocationX - gv.playerOffset; x <= mod.PlayerLocationX + gv.playerOffset; x++)
            {
                for (int y = mod.PlayerLocationY - gv.playerOffset; y <= mod.PlayerLocationY + gv.playerOffset; y++)
                {
                    //check if valid map location
                    if (x < 0) { continue; }
                    if (y < 0) { continue; }
                    if (x > this.mod.currentArea.MapSizeX - 1) { continue; }
                    if (y > this.mod.currentArea.MapSizeY - 1) { continue; }

                    string tile = mod.currentArea.Layer2Filename[y * mod.currentArea.MapSizeX + x];
                    int tlX = (x - mod.PlayerLocationX + gv.playerOffsetX) * gv.squareSize;
                    int tlY = (y - mod.PlayerLocationY + gv.playerOffsetY) * gv.squareSize;
                    float scalerX = gv.cc.GetFromTileBitmapList(tile).PixelSize.Width / 100;
                    float scalerY = gv.cc.GetFromTileBitmapList(tile).PixelSize.Height / 100;
                    int brX = (int)(gv.squareSize * scalerX);
                    int brY = (int)(gv.squareSize * scalerY);

                    try
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.GetFromTileBitmapList(tile).PixelSize.Width, gv.cc.GetFromTileBitmapList(tile).PixelSize.Height);
                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                        bool mirror = false;
                        if (mod.currentArea.Layer2Mirror[y * mod.currentArea.MapSizeX + x] == 1) { mirror = true; }
                        gv.DrawBitmap(gv.cc.GetFromTileBitmapList(tile), src, dst, mod.currentArea.Layer2Rotate[y * mod.currentArea.MapSizeX + x], mirror);
                    }
                    catch { }
                }
            }
            #endregion
        }
        public void drawProps()
        {
            foreach (Prop p in mod.currentArea.Props)
            {
                if ((p.isShown) && (!p.isMover))
                {
                    if ((p.LocationX >= mod.PlayerLocationX - gv.playerOffsetX) && (p.LocationX <= mod.PlayerLocationX + gv.playerOffsetX)
                        && (p.LocationY >= mod.PlayerLocationY - gv.playerOffsetY) && (p.LocationY <= mod.PlayerLocationY + gv.playerOffsetY))
                    {
                        //prop X - playerX
                        int x = ((p.LocationX - mod.PlayerLocationX) * gv.squareSize) + (gv.playerOffsetX * gv.squareSize);
                        int y = ((p.LocationY - mod.PlayerLocationY) * gv.squareSize) + (gv.playerOffsetY * gv.squareSize);
                        int dstW = (int)(((float)p.token.PixelSize.Width / (float)gv.squareSizeInPixels) * (float)gv.squareSize);
                        int dstH = (int)(((float)p.token.PixelSize.Height / (float)gv.squareSizeInPixels) * (float)gv.squareSize);
                        int dstXshift = (dstW - gv.squareSize) / 2;
                        int dstYshift = (dstH - gv.squareSize) / 2;
                        IbRect src = new IbRect(0, 0, p.token.PixelSize.Width, p.token.PixelSize.Width);
                        IbRect dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels - dstXshift, y - dstYshift, dstW, dstH);
                                                
                        gv.DrawBitmap(p.token, src, dst, !p.PropFacingLeft);

                        if (mod.showInteractionState == true)
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
            foreach (Prop p in mod.currentArea.Props)
            {
                if ((p.isShown) && (p.isMover))
                {
                    if ((p.LocationX >= mod.PlayerLocationX - gv.playerOffsetX) && (p.LocationX <= mod.PlayerLocationX + gv.playerOffsetX)
                        && (p.LocationY >= mod.PlayerLocationY - gv.playerOffsetY) && (p.LocationY <= mod.PlayerLocationY + gv.playerOffsetY))
                    {
                        //prop X - playerX
                        int x = ((p.LocationX - mod.PlayerLocationX) * gv.squareSize) + (gv.playerOffsetX * gv.squareSize);
                        int y = ((p.LocationY - mod.PlayerLocationY) * gv.squareSize) + (gv.playerOffsetY * gv.squareSize);
                        int dstW = (int)(((float)p.token.PixelSize.Width / (float)gv.squareSizeInPixels) * (float)gv.squareSize);
                        int dstH = (int)(((float)p.token.PixelSize.Height / (float)gv.squareSizeInPixels) * (float)gv.squareSize);
                        int dstXshift = (dstW - gv.squareSize) / 2;
                        int dstYshift = (dstH - gv.squareSize) / 2;
                        IbRect src = new IbRect(0, 0, p.token.PixelSize.Width, p.token.PixelSize.Width);
                        IbRect dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels - dstXshift, y - dstYshift, dstW, dstH);
                        gv.DrawBitmap(p.token, src, dst);

                        if (mod.showInteractionState)
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
                int minimapSquareSizeInPixels = 4 * gv.squareSize / mod.currentArea.MapSizeX;
                int drawW = minimapSquareSizeInPixels * mod.currentArea.MapSizeX;
                int drawH = minimapSquareSizeInPixels * mod.currentArea.MapSizeY;

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
                IbRect dst = new IbRect(pW, pH, drawW, drawH);
                gv.DrawBitmap(minimap, src, dst);

                //draw Fog of War
                if (mod.currentArea.UseMiniMapFogOfWar)
                {
                    for (int x = 0; x < this.mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < this.mod.currentArea.MapSizeY; y++)
                        {
                            int xx = x * minimapSquareSizeInPixels;
                            int yy = y * minimapSquareSizeInPixels;
                            src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                            dst = new IbRect(pW + xx, pH + yy, minimapSquareSizeInPixels, minimapSquareSizeInPixels);
                            if (mod.currentArea.Visible[y * mod.currentArea.MapSizeX + x] == 0)
                            {
                                gv.DrawBitmap(gv.cc.black_tile, src, dst);
                            }
                        }
                    }
                }
                                
	            //draw a location marker square RED
                int x2 = mod.PlayerLocationX * minimapSquareSizeInPixels;
                int y2 = mod.PlayerLocationY * minimapSquareSizeInPixels;
                src = new IbRect(0, 0, gv.cc.pc_dead.PixelSize.Width, gv.cc.pc_dead.PixelSize.Height);
                dst = new IbRect(pW + x2, pH + y2, minimapSquareSizeInPixels, minimapSquareSizeInPixels);
                gv.DrawBitmap(gv.cc.pc_dead, src, dst);	            
            }
        }
        public void drawPlayer()
        {
            if (mod.selectedPartyLeader >= mod.playerList.Count)
            {
                mod.selectedPartyLeader = 0;
            }
            int x = gv.playerOffsetX * gv.squareSize;
            int y = gv.playerOffsetY * gv.squareSize;
            int shift = gv.squareSize / 3;
            IbRect src = new IbRect(0, 0, mod.playerList[mod.selectedPartyLeader].token.PixelSize.Width, mod.playerList[mod.selectedPartyLeader].token.PixelSize.Width);
            IbRect dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
            if (mod.showPartyToken)
            {
                gv.DrawBitmap(mod.partyTokenBitmap, src, dst, !mod.playerList[0].combatFacingLeft);
            }
            else
            {
                if ((showFullParty) && (mod.playerList.Count > 1))
                {
                    if (mod.playerList[0].combatFacingLeft == true)
                    {
                        gv.oXshift = gv.oXshift + shift / 2;
                    }
                    else
                    {
                        gv.oXshift = gv.oXshift - shift / 2;
                    }
                    int reducedSquareSize = gv.squareSize * 2 / 3;
                    for (int i = mod.playerList.Count - 1; i >= 0; i--)
                    {
                        if ((i == 0) && (i != mod.selectedPartyLeader))
                        {
                            dst = new IbRect(x + gv.oXshift + shift + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft);
                        }
                        if ((i == 1) && (i != mod.selectedPartyLeader))
                        {
                            dst = new IbRect(x + gv.oXshift - shift + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft);
                        }
                        if ((i == 2) && (i != mod.selectedPartyLeader))
                        {
                            if (mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x + gv.oXshift - (shift) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft);
                        }
                        if ((i == 3) && (i != mod.selectedPartyLeader))
                        {
                            if (mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 2)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft);
                        }
                        if ((i == 4) && (i != mod.selectedPartyLeader))
                        {
                            if (mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 2)
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 3)
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft);
                        }

                        if ((i == 5) && (i != mod.selectedPartyLeader))
                        {
                            if (mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 2)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 3)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 4)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft);
                        }
                    }
                    
                    if (mod.playerList[0].combatFacingLeft == true)
                    {
                        gv.oXshift = gv.oXshift - shift / 2;
                    }
                    else
                    {
                        gv.oXshift = gv.oXshift + shift / 2;
                    }
                }
                //always draw party leader on top
                int storeShift = shift;
                shift = 0;
                if (mod.selectedPartyLeader == 0)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                    else
                    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                }
                else if (mod.selectedPartyLeader == 1)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x + gv.oXshift + shift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                    else
                    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                }
                else if (mod.selectedPartyLeader == 2)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x + gv.oXshift - shift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                    else
                    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                }
                else if (mod.selectedPartyLeader == 3)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x + gv.oXshift + (shift * 2) + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                    else
                    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                }
                else if (mod.selectedPartyLeader == 4)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x + gv.oXshift - (shift * 2) + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                    else
                    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                }
                else if (mod.selectedPartyLeader == 5)
                {
                    if (showFullParty)
                    {
                        dst = new IbRect(x + gv.oXshift - (shift * 3) + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                    else
                    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                    }
                }                
                gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft);
                shift = storeShift;
            }
        }
        public void drawGrid()
        {
            int minX = mod.PlayerLocationX - gv.playerOffsetX;
            if (minX < 0) { minX = 0; }
            int minY = mod.PlayerLocationY - gv.playerOffsetY;
            if (minY < 0) { minY = 0; }

            int maxX = mod.PlayerLocationX + gv.playerOffsetX + 1;
            if (maxX > this.mod.currentArea.MapSizeX) { maxX = this.mod.currentArea.MapSizeX; }
            int maxY = mod.PlayerLocationY + gv.playerOffsetY + 1;
            if (maxY > this.mod.currentArea.MapSizeY) { maxY = this.mod.currentArea.MapSizeY; }

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    int tlX = (x - mod.PlayerLocationX + gv.playerOffsetX) * gv.squareSize;
                    int tlY = (y - mod.PlayerLocationY + gv.playerOffsetY) * gv.squareSize;
                    int brX = gv.squareSize;
                    int brY = gv.squareSize;
                    IbRect src = new IbRect(0, 0, gv.cc.walkBlocked.PixelSize.Width, gv.cc.walkBlocked.PixelSize.Height);
                    IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                    if (mod.currentArea.LoSBlocked[y * mod.currentArea.MapSizeX + x] == 1)
                    {
                        gv.DrawBitmap(gv.cc.losBlocked, src, dst);
                    }
                    if (mod.currentArea.Walkable[y * mod.currentArea.MapSizeX + x] == 0)
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
            int txtH = (int)gv.fontHeight;

            for (int x = 0; x <= 2; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    gv.DrawText(gv.cc.floatyText, gv.cc.floatyTextLoc.X + x + gv.oXshift + mapStartLocXinPixels, gv.cc.floatyTextLoc.Y + y + txtH, "bk");
                }
            }
            
            gv.DrawText(gv.cc.floatyText, gv.cc.floatyTextLoc.X + gv.oXshift + mapStartLocXinPixels, gv.cc.floatyTextLoc.Y + txtH, "wh");
        }
        public void drawOverlayTints()
        {
            IbRect src = new IbRect(0, 0, gv.cc.tint_sunset.PixelSize.Width, gv.cc.tint_sunset.PixelSize.Height);
            //IbRect dst = new IbRect(gv.oXshift + mapStartLocXinPixels, 0, (gv.squareSize * (gv.playerOffsetX * 2 + 1)), (gv.squareSize * (gv.playerOffsetY * 2 + 2)));
            IbRect dst = new IbRect(mapStartLocXinPixels-gv.oXshift, -gv.oYshift, (gv.squareSize * (gv.playerOffsetX * 2 + 1))+ 2*gv.oXshift + gv.pS, (gv.squareSize * (gv.playerOffsetY * 2 + 2)) + gv.pS);

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
                if (spr.movementMethod.Contains("rain") || spr.movementMethod.Contains("snow") || spr.movementMethod.Contains("sandStorm"))
                {
                    spr.Draw(gv);
                }
            }

            foreach (Sprite spr in spriteList)
            {
                if (!spr.movementMethod.Contains("rain") && !spr.movementMethod.Contains("snow") && !spr.movementMethod.Contains("sandStorm"))
                {
                    spr.Draw(gv);
                }
            }

            drawBlackTilesOverTints();
        }
        public void drawMainMapClockText()
        {
            int timeofday = mod.WorldTime % (24 * 60);
            int hour = timeofday / 60;
            int minute = timeofday % 60;
            string sMinute = minute + "";
            if (minute < 10)
            {
                sMinute = "0" + minute;
            }

            int txtH = (int)gv.fontHeight;            
            for (int x = 0; x <= 2; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    gv.DrawText(hour + ":" + sMinute, gv.oXshift + x + (gv.playerOffsetX - 1) * gv.squareSize, (gv.playerOffsetY * 2) * gv.squareSize - (6 * gv.pS), "bk");
                }
            }
            gv.DrawText(hour + ":" + sMinute, gv.oXshift + (gv.playerOffsetX - 1) * gv.squareSize, (gv.playerOffsetY * 2) * gv.squareSize - (6 * gv.pS), "wh");
        }
        public void drawFogOfWar()
        {            
            int minX = mod.PlayerLocationX - gv.playerOffsetX-1;
            if (minX < 0) { minX = 0; }
            int minY = mod.PlayerLocationY - gv.playerOffsetY-1;
            if (minY < 0) { minY = 0; }

            int maxX = mod.PlayerLocationX + gv.playerOffsetX + 2;
            if (maxX > this.mod.currentArea.MapSizeX) { maxX = this.mod.currentArea.MapSizeX; }
            int maxY = mod.PlayerLocationY + gv.playerOffsetY + 3;
            if (maxY > this.mod.currentArea.MapSizeY) { maxY = this.mod.currentArea.MapSizeY; }

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    int tlX = (x - mod.PlayerLocationX + gv.playerOffsetX) * gv.squareSize;
                    int tlY = (y - mod.PlayerLocationY + gv.playerOffsetY) * gv.squareSize;
                    int brX = gv.squareSize;
                    int brY = gv.squareSize;
                    IbRect src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                    IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                    if (mod.currentArea.Visible[y * mod.currentArea.MapSizeX + x] == 0)
                    {
                        gv.DrawBitmap(gv.cc.black_tile, src, dst);
                    }
                }
            }
        }
        public void drawBlackTilesOverTints()
        {
            int width = gv.playerOffsetX * 2 + 1;
            int height = gv.playerOffsetY * 2 + 1;

            //at left edge
            for (int i = -2; i < gv.playerOffsetX - mod.PlayerLocationX; i++)
            {
                drawColumnOfBlack(i);                    
            }

            //at top edge
            for (int i = -2; i < gv.playerOffsetY - mod.PlayerLocationY; i++)
            {
                drawRowOfBlack(i);
            }

            //at right edge
            for (int i = -1; i <= gv.playerOffsetX + mod.PlayerLocationX - mod.currentArea.MapSizeX + 1; i++)
            {
                drawColumnOfBlack(width - i);                    
            }

            //at bottom edge
            for (int i = -1; i <= gv.playerOffsetY + mod.PlayerLocationY - mod.currentArea.MapSizeY + 1; i++)
            {
                drawRowOfBlack(height - i);
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
                    if (gv.cc.getDistance(ft.location, new Coordinate(mod.PlayerLastLocationX, mod.PlayerLocationY)) > 3)
                    {
                        continue; //out of range from view so skip drawing floaty message
                    }

                    //location.X should be the the props actual map location in squares (not screen location)
                    int xLoc = (ft.location.X + gv.playerOffsetX - mod.PlayerLocationX) * gv.squareSize;
                    int yLoc = ((ft.location.Y + gv.playerOffsetY - mod.PlayerLocationY) * gv.squareSize) - (ft.z);

                    for (int x = 0; x <= 2; x++)
                    {
                        for (int y = 0; y <= 2; y++)
                        {
                            gv.DrawText(ft.value, xLoc + x + gv.oXshift + mapStartLocXinPixels, yLoc + y + txtH, "bk");
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
                    gv.DrawText(ft.value, xLoc + gv.oXshift + mapStartLocXinPixels, yLoc + txtH, colr);
                }
            }
        }
        public void drawColumnOfBlack(int col)
        {
            for (int y = -1; y < gv.playerOffsetY * 2 + 1 + 2; y++)
            {
                int tlX = col * gv.squareSize;
                int tlY = y * gv.squareSize;
                int brX = gv.squareSize;
                int brY = gv.squareSize;
                IbRect src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                IbRect dst = new IbRect(tlX + mapStartLocXinPixels + gv.oXshift, tlY, brX, brY);

                gv.DrawBitmap(gv.cc.black_tile, src, dst);
            }
        }
        public void drawRowOfBlack(int row)
        {
            for (int x = -1; x < gv.playerOffsetX * 2 + 1 + 2; x++)
            {
                int tlX = x * gv.squareSize;
                int tlY = row * gv.squareSize;
                int brX = gv.squareSize;
                int brY = gv.squareSize;
                IbRect src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                gv.DrawBitmap(gv.cc.black_tile, src, dst);
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
                    foreach (Player pc in mod.playerList)
                    {
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
            floatyTextPool.Add(new FloatyText(sqrX, sqrY, value, color, length));
        }
        public void addFloatyText(Prop floatyCarrier, string value, string color, int length)
        {
            floatyTextByPixelPool.Add(new FloatyTextByPixel (floatyCarrier, value, color, length));
        }
        
        public void onTouchMain(MouseEventArgs e, MouseEventType.EventType eventType)
        {
            if (gv.showMessageBox)
            {
                gv.messageBox.btnReturn.glowOn = false;
            }
            switch (eventType)
            {
                case MouseEventType.EventType.MouseDown:
                case MouseEventType.EventType.MouseMove:
                    int x = (int)e.X;
                    int y = (int)e.Y;

                    if (gv.showMessageBox)
                    {
                        if (gv.messageBox.btnReturn.getImpact(x, y))
                        {
                            gv.messageBox.btnReturn.glowOn = true;
                        }
                    }

                    //NEW SYSTEM
                    mainUiLayout.setHover(x, y);

                    //Draw Floaty Text On Mouse Over Prop
                    int gridx = (int)e.X / gv.squareSize;
                    int gridy = (int)e.Y / gv.squareSize;
                    int actualX = mod.PlayerLocationX + (gridx - gv.playerOffsetX) - (mapStartLocXinPixels / gv.squareSize);
                    int actualY = mod.PlayerLocationY + (gridy - gv.playerOffsetY);
                    gv.cc.floatyText = "";
                    if (IsTouchInMapWindow(gridx, gridy))
                    {
                        foreach (Prop p in mod.currentArea.Props)
                        {
                            if ((p.LocationX == actualX) && (p.LocationY == actualY))
                            {
                                if (!p.MouseOverText.Equals("none"))
                                {
                                    gv.cc.floatyText = p.MouseOverText;
                                    int halfWidth = (p.MouseOverText.Length * (gv.fontWidth + gv.fontCharSpacing)) / 2;
                                    gv.cc.floatyTextLoc = new Coordinate((gridx * gv.squareSize) - mapStartLocXinPixels - halfWidth, gridy * gv.squareSize);
                                }
                            }
                        }
                    }
                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)e.X;
                    y = (int)e.Y;
                    int gridX = (int)e.X / gv.squareSize;
                    int gridY = (int)e.Y / gv.squareSize;
                    int actualx = mod.PlayerLocationX + (gridX - gv.playerOffsetX);
                    int actualy = mod.PlayerLocationY + (gridY - gv.playerOffsetY);

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
                            mod.map_showGrid = false;
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            mod.map_showGrid = true;
                        }
                    }
                    if (rtn.Equals("tglInteractionState"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            mod.showInteractionState = false;
                            gv.cc.addLogText("yellow", "Hide info about interaction state of NPC and creatures (encounter = red, mandatory conversation = orange and optional conversation = green");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            mod.showInteractionState = true;
                            gv.cc.addLogText("lime", "Show info about interaction state of NPC and creatures (encounter = red, mandatory conversation = orange and optional conversation = green");
                        }
                    }
                    if (rtn.Equals("tglAvoidConversation"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            mod.avoidInteraction = false;
                            gv.cc.addLogText("lime", "Normal move mode: party does all possible conversations");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            mod.avoidInteraction = true;
                            gv.cc.addLogText("yellow", "In a hurry: Party is avoiding all conversations that are not mandatory");
                        }
                    }

                    if (rtn.Equals("tglClock"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        tgl.toggleOn = !tgl.toggleOn;
                        showClock = !showClock;
                    }
                    if (rtn.Equals("tglSound"))
                    {
                        IB2ToggleButton tgl = mainUiLayout.GetToggleByTag(rtn);
                        if (tgl == null) { return; }
                        if (tgl.toggleOn)
                        {
                            tgl.toggleOn = false;
                            mod.playMusic = false;
                            mod.playSoundFx = false;
                            //TODO gv.screenCombat.tglSoundFx.toggleOn = false;
                            //gv.stopMusic();
                            //gv.stopAmbient();
                            gv.cc.addLogText("lime", "Music Off, SoundFX Off");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            mod.playMusic = true;
                            mod.playSoundFx = true;
                            //TODO gv.screenCombat.tglSoundFx.toggleOn = true;
                            //gv.startMusic();
                            //gv.startAmbient();
                            gv.cc.addLogText("lime", "Music On, SoundFX On");
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
                            gv.cc.addLogText("lime", "Show Party Leader");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            showFullParty = true;
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
                            gv.cc.addLogText("lime", "Hide Mini Map");
                        }
                        else
                        {
                            tgl.toggleOn = true;
                            showMiniMap = true;
                            gv.cc.addLogText("lime", "Show Mini Map");
                        }
                    }
                    if ((rtn.Equals("ctrlUpArrow")) || ((mod.PlayerLocationX == actualx) && ((mod.PlayerLocationY - 1) == actualy)))
                    {
                        
                            if (mod.PlayerLocationY > 0)
                            {
                                if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY - 1) == false)
                                {
                                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                                    mod.PlayerLocationY--;
                                    gv.cc.doUpdate();
                                }
                            }
                        
                    }
                    else if ((rtn.Equals("ctrlDownArrow")) || ((mod.PlayerLocationX == actualx) && ((mod.PlayerLocationY + 1) == actualy)))
                    {

                        
                            int mapheight = mod.currentArea.MapSizeY;
                            if (mod.PlayerLocationY < (mapheight - 1))
                            {
                                if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY + 1) == false)
                                {
                                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                                    mod.PlayerLocationY++;
                                    gv.cc.doUpdate();
                                }
                            }
                        
                    }
                    else if ((rtn.Equals("ctrlLeftArrow")) || (((mod.PlayerLocationX - 1) == actualx) && (mod.PlayerLocationY == actualy)))
                    {
                        
                            if (mod.PlayerLocationX > 0)
                            {
                                if (mod.currentArea.GetBlocked(mod.PlayerLocationX - 1, mod.PlayerLocationY) == false)
                                {
                                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                                    mod.PlayerLocationX--;
                                    foreach (Player pc in mod.playerList)
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
                    else if ((rtn.Equals("ctrlRightArrow")) || (((mod.PlayerLocationX + 1) == actualx) && (mod.PlayerLocationY == actualy)))
                    {
                        
                            int mapwidth = mod.currentArea.MapSizeX;
                            if (mod.PlayerLocationX < (mapwidth - 1))
                            {
                                if (mod.currentArea.GetBlocked(mod.PlayerLocationX + 1, mod.PlayerLocationY) == false)
                                {
                                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                                    mod.PlayerLocationX++;
                                    foreach (Player pc in mod.playerList)
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
                    else if ((rtn.Equals("port0")) && (mod.playerList.Count > 0))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            mod.selectedPartyLeader = 0;
                            gv.cc.partyScreenPcIndex = 0;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            mod.selectedPartyLeader = 0;
                            gv.cc.partyScreenPcIndex = 0;
                        }
                    }
                    else if ((rtn.Equals("port1")) && (mod.playerList.Count > 1))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            mod.selectedPartyLeader = 1;
                            gv.cc.partyScreenPcIndex = 1;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            mod.selectedPartyLeader = 1;
                            gv.cc.partyScreenPcIndex = 1;
                        }
                    }
                    else if ((rtn.Equals("port2")) && (mod.playerList.Count > 2))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            mod.selectedPartyLeader = 2;
                            gv.cc.partyScreenPcIndex = 2;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            mod.selectedPartyLeader = 2;
                            gv.cc.partyScreenPcIndex = 2;
                        }
                    }
                    else if ((rtn.Equals("port3")) && (mod.playerList.Count > 3))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            mod.selectedPartyLeader = 3;
                            gv.cc.partyScreenPcIndex = 3;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            mod.selectedPartyLeader = 3;
                            gv.cc.partyScreenPcIndex = 3;
                        }
                    }
                    else if ((rtn.Equals("port4")) && (mod.playerList.Count > 4))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            mod.selectedPartyLeader = 4;
                            gv.cc.partyScreenPcIndex = 4;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            mod.selectedPartyLeader = 4;
                            gv.cc.partyScreenPcIndex = 4;
                        }
                    }
                    else if ((rtn.Equals("port5")) && (mod.playerList.Count > 5))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            mod.selectedPartyLeader = 5;
                            gv.cc.partyScreenPcIndex = 5;
                            gv.screenParty.resetPartyScreen();
                            gv.screenType = "party";
                            gv.cc.tutorialMessageParty(false);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            mod.selectedPartyLeader = 5;
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
                        gv.cc.doSettingsDialogs();
                    }
                    else if (rtn.Equals("btnSave"))
                    {
                        if (mod.allowSave)
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
                        /*List<string> pcNames = new List<string>();
                        List<int> pcIndex = new List<int>();
                        pcNames.Add("cancel");

                        int cnt = 0;
                        foreach (Player p in mod.playerList)
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
            foreach (Player p in mod.playerList)
            {
                if (hasMainMapTypeSpell(p))
                {
                    pcNames.Add(p.name);
                    pcIndex.Add(cnt);
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
                foreach (Player p in mod.playerList)
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
                if (mod.allowSave)
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
                if (gv.mod.debugMode)
                {
                    gv.mod.debugMode = false;
                    gv.cc.addLogText("lime", "DebugMode Turned Off");
                }
                else
                {
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
                foreach (Player p in mod.playerList)
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
            else if (keyData == Keys.X)
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
            }
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
            if (mod.PlayerLocationX > 0)
            {
                if (mod.currentArea.GetBlocked(mod.PlayerLocationX - 1, mod.PlayerLocationY) == false)
                {
                   
                    //gv.mod.blockTrigger = false;
                    //gv.mod.blockTriggerMovingProp = false;

                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                    mod.PlayerLocationX--;
                    foreach (Player pc in mod.playerList)
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
            int mapwidth = mod.currentArea.MapSizeX;
            if (mod.PlayerLocationX < (mapwidth - 1))
            {
                if (mod.currentArea.GetBlocked(mod.PlayerLocationX + 1, mod.PlayerLocationY) == false)
                {
                    
                    //gv.mod.blockTrigger = false;
                    //gv.mod.blockTriggerMovingProp = false;
                   
                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                    mod.PlayerLocationX++;
                    foreach (Player pc in mod.playerList)
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
            if (mod.PlayerLocationY > 0)
            {
                if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY - 1) == false)
                {

                    //gv.mod.blockTrigger = false;
                    //gv.mod.blockTriggerMovingProp = false;

                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                    mod.PlayerLocationY--;
                    gv.cc.doUpdate();
                }
            }
        }
        private void moveDown()
        {
            int mapheight = mod.currentArea.MapSizeY;
            if (mod.PlayerLocationY < (mapheight - 1))
            {
                if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY + 1) == false)
                {
                   
                    //gv.mod.blockTrigger = false;
                    //gv.mod.blockTriggerMovingProp = false;

                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                    mod.PlayerLocationY++;
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
            mod.currentArea.Visible[mod.PlayerLocationY * mod.currentArea.MapSizeX + mod.PlayerLocationX] = 1;
            // set tiles to visible around the PC
            for (int x = mod.PlayerLocationX - mod.currentArea.AreaVisibleDistance; x <= mod.PlayerLocationX + mod.currentArea.AreaVisibleDistance; x++)
            {
                for (int y = mod.PlayerLocationY - mod.currentArea.AreaVisibleDistance; y <= mod.PlayerLocationY + mod.currentArea.AreaVisibleDistance; y++)
                {
                    int xx = x;
                    int yy = y;
                    if (xx < 1) { xx = 0; }
                    if (xx > (mod.currentArea.MapSizeX - 1)) { xx = (mod.currentArea.MapSizeX - 1); }
                    if (yy < 1) { yy = 0; }
                    if (yy > (mod.currentArea.MapSizeY - 1)) { yy = (mod.currentArea.MapSizeY - 1); }
                    if (IsLineOfSightForEachCorner(new Coordinate(mod.PlayerLocationX, mod.PlayerLocationY), new Coordinate(xx, yy)))
                    {
                        mod.currentArea.Visible[yy * mod.currentArea.MapSizeX + xx] = 1;
                    }
                }
            }
            //make all adjacent squares visible
            int minX = mod.PlayerLocationX - 1;
            if (minX < 0) { minX = 0; }
            int minY = mod.PlayerLocationY - 1;
            if (minY < 0) { minY = 0; }

            int maxX = mod.PlayerLocationX + 1;
            if (maxX > this.mod.currentArea.MapSizeX - 1) { maxX = this.mod.currentArea.MapSizeX - 1; }
            int maxY = mod.PlayerLocationY + 1;
            if (maxY > this.mod.currentArea.MapSizeY - 1) { maxY = this.mod.currentArea.MapSizeY - 1; }

            for (int xx = minX; xx <= maxX; xx++)
            {
                for (int yy = minY; yy <= maxY; yy++)
                {
                    mod.currentArea.Visible[yy * mod.currentArea.MapSizeX + xx] = 1;
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
            Coordinate start = new Coordinate((s.X * gv.squareSize) + (gv.squareSize / 2), (s.Y * gv.squareSize) + (gv.squareSize / 2));
            //check center of all four sides of the end square
            int halfSquare = (gv.squareSize / 2);
            //left side center
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize, e.Y * gv.squareSize + halfSquare), e)) { return true; }
            //right side center
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + gv.squareSize, e.Y * gv.squareSize + halfSquare), e)) { return true; }
            //top side center
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + halfSquare, e.Y * gv.squareSize), e)) { return true; }
            //bottom side center
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + halfSquare, e.Y * gv.squareSize + gv.squareSize), e)) { return true; }

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
                        gridx = nextPoint.X / gv.squareSize;
                        gridy = nextPoint.Y / gv.squareSize;
                        if (gridx < 1) { gridx = 0; }
                        if (gridx > (mod.currentArea.MapSizeX - 1)) { gridx = (mod.currentArea.MapSizeX - 1); }
                        if (gridy < 1) { gridy = 0; }
                        if (gridy > (mod.currentArea.MapSizeY - 1)) { gridy = (mod.currentArea.MapSizeY - 1); }
                        if (mod.currentArea.LoSBlocked[gridy * mod.currentArea.MapSizeX + gridx] == 1)
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
                        gridx = nextPoint.X / gv.squareSize;
                        gridy = nextPoint.Y / gv.squareSize;
                        if (gridx < 1) { gridx = 0; }
                        if (gridx > (mod.currentArea.MapSizeX - 1)) { gridx = (mod.currentArea.MapSizeX - 1); }
                        if (gridy < 1) { gridy = 0; }
                        if (gridy > (mod.currentArea.MapSizeY - 1)) { gridy = (mod.currentArea.MapSizeY - 1); }
                        if (mod.currentArea.LoSBlocked[gridy * mod.currentArea.MapSizeX + gridx] == 1)
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
                        gridx = nextPoint.X / gv.squareSize;
                        gridy = nextPoint.Y / gv.squareSize;
                        if (gridx < 1) { gridx = 0; }
                        if (gridx > (mod.currentArea.MapSizeX - 1)) { gridx = (mod.currentArea.MapSizeX - 1); }
                        if (gridy < 1) { gridy = 0; }
                        if (gridy > (mod.currentArea.MapSizeY - 1)) { gridy = (mod.currentArea.MapSizeY - 1); }
                        if (mod.currentArea.LoSBlocked[gridy * mod.currentArea.MapSizeX + gridx] == 1)
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
                        gridx = nextPoint.X / gv.squareSize;
                        gridy = nextPoint.Y / gv.squareSize;
                        if (gridx < 1) { gridx = 0; }
                        if (gridx > (mod.currentArea.MapSizeX - 1)) { gridx = (mod.currentArea.MapSizeX - 1); }
                        if (gridy < 1) { gridy = 0; }
                        if (gridy > (mod.currentArea.MapSizeY - 1)) { gridy = (mod.currentArea.MapSizeY - 1); }
                        if (mod.currentArea.LoSBlocked[gridy * mod.currentArea.MapSizeX + gridx] == 1)
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
                Spell sp = mod.getSpellByTag(s);
                if ((sp.useableInSituation.Equals("Always")) || (sp.useableInSituation.Equals("OutOfCombat")))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
