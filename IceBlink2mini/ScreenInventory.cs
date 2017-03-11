using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2mini
{
    public class ScreenInventory 
    {
	    //public Module gv.mod;
	    public GameView gv;
	    private int inventoryPageIndex = 0;
	    private int inventorySlotIndex = 0;
	    private int slotsPerPage = 20;
	    private List<IbbButton> btnInventorySlot = new List<IbbButton>();
	    private IbbButton btnInventoryLeft = null;
	    private IbbButton btnInventoryRight = null;
	    private IbbButton btnPageIndex = null;
	    private IbbButton btnHelp = null;
	    private IbbButton btnInfo = null;
	    private IbbButton btnReturn = null;
	    private IbbHtmlTextBox description;
        public bool inCombat = false;
	
	    public ScreenInventory(Module m, GameView g)
	    {
		    //gv.mod = m;
		    gv = g;
		    //setControlsStart();
	    }
	
	    public void setControlsStart()
	    {			
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;

            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;

            if (btnInventoryLeft == null)
            {
                btnInventoryLeft = new IbbButton(gv, 1.0f);
            }
			    btnInventoryLeft.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnInventoryLeft.Img2 = "ctrl_left_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    btnInventoryLeft.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnInventoryLeft.X = 3 * gv.squareSize + (pW * 3);
			    btnInventoryLeft.Y = (0 * gv.squareSize) + (pH * 2);
                btnInventoryLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventoryLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnPageIndex == null)
            {
                btnPageIndex = new IbbButton(gv, 1.0f);
            }
			    btnPageIndex.Img = "btn_small_off"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
			    btnPageIndex.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnPageIndex.Text = "1";
			    btnPageIndex.X = 4 * gv.squareSize + (pW * 4);
                btnPageIndex.Y = (0 * gv.squareSize) + (pH * 2);
                btnPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnInventoryRight == null)
            {
                btnInventoryRight = new IbbButton(gv, 1.0f);
            }
			    btnInventoryRight.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnInventoryRight.Img2 = "ctrl_right_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnInventoryRight.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnInventoryRight.X = 5 * gv.squareSize + (pW * 5);
                btnInventoryRight.Y = (0 * gv.squareSize) + (pH * 2);
                btnInventoryRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventoryRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);


            if (btnReturn == null)
            {
                btnReturn = new IbbButton(gv, 1.2f);
            }
			    btnReturn.Text = "RETURN";
			    btnReturn.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnReturn.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnReturn.X = (gv.squaresInWidth * gv.squareSize / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
			    btnReturn.Y = 6 * gv.squareSize - pH * 2;
                btnReturn.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturn.Width = (int)(gv.ibbwidthL * gv.screenDensity);

            if (btnHelp == null)
            {
                btnHelp = new IbbButton(gv, 0.8f);
            }
			    btnHelp.Text = "HELP";
			    btnHelp.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnHelp.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnHelp.X = (gv.squaresInWidth * gv.squareSize / 2) - (int)((gv.ibbwidthL / 2) * gv.screenDensity) - (int)(gv.squareSize * 1.5);
                btnHelp.Y = 6 * gv.squareSize - pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            if (btnInfo == null)
            {
                btnInfo = new IbbButton(gv, 0.8f);
            }
			    btnInfo.Text = "INFO";
			    btnInfo.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnInfo.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnInfo.X = (gv.squaresInWidth * gv.squareSize / 2) + (int)((gv.ibbwidthL / 2) * gv.screenDensity) + (int)(gv.squareSize * 0.5);
                btnInfo.Y = 6 * gv.squareSize - pH * 2;
                btnInfo.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInfo.Width = (int)(gv.ibbwidthR * gv.screenDensity);

            for (int y = 0; y < slotsPerPage; y++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = "item_slot"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
			    btnNew.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    if (y < 5)
			    {
				    btnNew.X = ((y+2) * gv.squareSize) + (padW * (y+1));
				    btnNew.Y = 1 * gv.squareSize + padW;
			    }
			    else if ((y >=5 ) && (y < 10))
			    {
				    btnNew.X = ((y-5+2) * gv.squareSize) + (padW * ((y-5)+1));
				    btnNew.Y = 2 * gv.squareSize + padW * 2;
			    }
			    else if ((y >=10 ) && (y < 15))
			    {
				    btnNew.X = ((y-10+2) * gv.squareSize) + (padW * ((y-10)+1));
				    btnNew.Y = 3 * gv.squareSize + (padW * 3);
			    }
			    else
			    {
				    btnNew.X = ((y-15+2) * gv.squareSize) + (padW * ((y-15)+1));
				    btnNew.Y = 4 * gv.squareSize + (padW * 4);
			    }

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnInventorySlot.Add(btnNew);
		    }			
	    }
	
        public void resetInventory()
        {
            if (btnReturn == null)
            {
                setControlsStart();
            }
            doItemStacking();
            int cntSlot = 0;
            foreach (IbbButton btn in btnInventorySlot)
            {
                if ((cntSlot + (inventoryPageIndex * slotsPerPage)) < gv.mod.partyInventoryRefsList.Count)
                {
                    Item it = gv.mod.getItemByResRefForInfo(gv.mod.partyInventoryRefsList[cntSlot + (inventoryPageIndex * slotsPerPage)].resref);
                    //gv.cc.DisposeOfBitmap(ref btn.Img2);
                    btn.Img2 = it.itemImage;
                    ItemRefs itr = gv.mod.partyInventoryRefsList[cntSlot + (inventoryPageIndex * slotsPerPage)];
                    if (itr.quantity > 1)
                    {
                        btn.Quantity = itr.quantity + "";
                    }
                    else
                    {
                        btn.Quantity = "";
                    }
                }
                else
                {
                    btn.Img2 = null;
                    btn.Quantity = "";
                }
                cntSlot++;
            }
        }
	    //INVENTORY SCREEN (COMBAT and MAIN)
        public void redrawInventory()
        {
            //IF CONTROLS ARE NULL, CREATE THEM
    	    if (btnReturn == null)
    	    {
    		    setControlsStart();
    	    }
    	
    	    doItemStacking();
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
    	    int textH = (int)gv.fontHeight;
            int spacing = textH;
            int tabX = pW * 4;
    	    int tabX2 = 5 * gv.squareSize + pW * 2;
    	    int leftStartY = pH * 4;
    	    int tabStartY = 5 * gv.squareSize + pW * 10;
    	
            //DRAW TEXT		
		    locY = gv.squareSize / 2;
		    gv.DrawText("Party", locX + (gv.squareSize * 1) + pW * 2, locY, "wh");
            gv.DrawText("Inventory", locX + (gv.squareSize * 1) + pW * 2, locY += spacing, "wh");
		    locY = gv.squareSize / 2;
            gv.DrawText("Party", tabX2 + (gv.squareSize * 2), locY, "yl");
            gv.DrawText(gv.mod.goldLabelPlural + ": " + gv.mod.partyGold, tabX2 + (gv.squareSize * 2), locY += spacing, "yl");

		    //DRAW LEFT/RIGHT ARROWS and PAGE INDEX
		    btnPageIndex.Draw();
		    btnInventoryLeft.Draw();
		    btnInventoryRight.Draw();		
		
		    //DRAW ALL INVENTORY SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnInventorySlot)
		    {
			    if (cntSlot == inventorySlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;		
		    if (isSelectedItemSlotInPartyInventoryRange())
		    {
			    ItemRefs itRef = GetCurrentlySelectedItemRefs();
        	    Item it = gv.mod.getItemByResRefForInfo(itRef.resref);

                //Description
		        string textToSpan = "";
                //textToSpan = "Description:" + Environment.NewLine;
        	    //textToSpan += it.name + Environment.NewLine;
                textToSpan = "<gy>Description</gy>" + "<BR>";
	            textToSpan += "<gn>" + it.name + "</gn><BR>";
	            if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
	            {
	        	    textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<BR>";
	                textToSpan += "Attack Bonus: " + it.attackBonus + "<BR>";
	                textToSpan += "Attack Range: " + it.attackRange + "<BR>";
	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	                textToSpan += "Tap 'INFO' for Full Description<BR>";
	            }    
	            else if (!it.category.Equals("General"))
	            {
	        	    textToSpan += "AC Bonus: " + it.armorBonus + "<BR>";
	                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";
	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	                textToSpan += "Tap 'INFO' for Full Description<BR>";
	            }
	            else if (it.category.Equals("General"))
	            {
	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	        	    textToSpan += "Tap 'INFO' for Full Description<BR>";
	            }
                
                description.tbXloc = (8 * gv.squareSize);
                description.tbYloc = 1 * gv.squareSize + gv.squareSize / 6;
                description.tbWidth = 3 * gv.squareSize;
                description.tbHeight = 8 * gv.squareSize;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
		    }
		    btnHelp.Draw();	
		    btnInfo.Draw();	
		    btnReturn.Draw();
            if (gv.showMessageBox)
            {
                gv.messageBox.onDrawLogBox();
            }
        }
        public string isUseableBy(Item it)
        {
    	    string strg = "";
    	    foreach (PlayerClass cls in gv.mod.modulePlayerClassList)
    	    {
                string firstLetter = cls.name.Substring(0,1);
    		    foreach (ItemRefs stg in cls.itemsAllowed)
    		    {
    			    if (stg.resref.Equals(it.resref))
    			    {
    				    strg += firstLetter + ", ";
    			    }
    		    }
    	    }
    	    return strg;
        }
	    public void onTouchInventory(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType, bool inCombat)
	    {
		    btnInventoryLeft.glowOn = false;
		    btnInventoryRight.glowOn = false;
		    btnHelp.glowOn = false;
		    btnInfo.glowOn = false;
		    btnReturn.glowOn = false;
            if (gv.showMessageBox)
            {
                gv.messageBox.btnReturn.glowOn = false;
            }

            //int eventAction = event.getAction();
            switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) eX;
			    int y = (int) eY;

                if (gv.showMessageBox)
                {
                    if (gv.messageBox.btnReturn.getImpact(x, y))
                    {
                        gv.messageBox.btnReturn.glowOn = true;
                    }
                    return;
                }

                if (btnInventoryLeft.getImpact(x, y))
			    {
				    btnInventoryLeft.glowOn = true;
			    }
			    else if (btnInventoryRight.getImpact(x, y))
			    {
				    btnInventoryRight.glowOn = true;
			    }
			    else if (btnHelp.getImpact(x, y))
			    {
				    btnHelp.glowOn = true;
			    }
			    else if (btnInfo.getImpact(x, y))
			    {
				    btnInfo.glowOn = true;
			    }
			    else if (btnReturn.getImpact(x, y))
			    {
				    btnReturn.glowOn = true;
			    }
			    break;
			
		    case MouseEventType.EventType.MouseUp:
			    x = (int) eX;
			    y = (int) eY;
			
			    btnInventoryLeft.glowOn = false;
			    btnInventoryRight.glowOn = false;
			    btnHelp.glowOn = false;
			    btnInfo.glowOn = false;
			    btnReturn.glowOn = false;

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

                for (int j = 0; j < slotsPerPage; j++)
			    {
				    if (btnInventorySlot[j].getImpact(x, y))
				    {
					    if (inventorySlotIndex == j)
					    {
						    if (inCombat)
						    {
                                if (isSelectedItemSlotInPartyInventoryRange())
                                {
                                    doItemActionSetup(true);
                                }
						    }
						    else
						    {
                                if (isSelectedItemSlotInPartyInventoryRange())
                                {
                                    doItemActionSetup(false);
                                }
						    }
					    }
					    inventorySlotIndex = j;
				    }
			    }
			    if (btnInventoryLeft.getImpact(x, y))
			    {
				    if (inventoryPageIndex > 0)
				    {
					    inventoryPageIndex--;
					    btnPageIndex.Text = (inventoryPageIndex + 1) + "";
                        resetInventory();
				    }
			    }
			    else if (btnInventoryRight.getImpact(x, y))
			    {
				    if (inventoryPageIndex < 9)
				    {
					    inventoryPageIndex++;
					    btnPageIndex.Text = (inventoryPageIndex + 1) + "";
                        resetInventory();
				    }
			    }
			    else if (btnHelp.getImpact(x, y))
			    {
				    tutorialMessageInventory(true);
			    }
			    else if (btnInfo.getImpact(x, y))
			    {
				    if (isSelectedItemSlotInPartyInventoryRange())
				    {				
					    ItemRefs itRef = GetCurrentlySelectedItemRefs();
					    if (itRef == null) { return;}
	            	    Item it = gv.mod.getItemByResRef(itRef.resref);
	            	    if (it == null) {return;}
					    gv.sf.ShowFullDescription(it);
				    }				
			    }
			    else if (btnReturn.getImpact(x, y))
			    {
				    if (inCombat)
				    {
					    if (gv.screenCombat.canMove)
					    {
						    gv.screenCombat.currentCombatMode = "move";
					    }
					    else
					    {
						    gv.screenCombat.currentCombatMode = "attack";
					    }
					    gv.screenType = "combat";
					    doCleanUp();
				    }
				    else
				    {
					    gv.screenType = "main";	
					    doCleanUp();
				    }							
			    }
			    break;		
		    }
	    }
	
	    public void doCleanUp()
	    {
		    btnInventorySlot.Clear();
		    btnInventoryLeft = null;
		    btnInventoryRight = null;
		    btnPageIndex = null;
		    btnHelp = null;
		    btnInfo = null;
		    btnReturn = null;
	    }
	
        public void doItemActionSetup(bool inCombat)
        {
            this.inCombat = inCombat;
            List<string> actionList = new List<string> { "Use Item", "Drop Item", "View Item Description" };
            gv.itemListSelector.setupIBminiItemListSelector(gv, actionList, "Item Action", "inventoryitemaction");
            gv.itemListSelector.showIBminiItemListSelector = true;

        }
        public void doItemAction(int selectedIndex)
	    {            
            ItemRefs itRef = GetCurrentlySelectedItemRefs();
	        Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
            if ((selectedIndex == 0) && ( (!it.onUseItemIBScript.Equals("none")) || (!it.onUseItemCastSpellTag.Equals("none")) ) )
            {
                doSelectPcUseItemSetup();
	            	
            }	                
	        else if (selectedIndex == 1) // selected to DROP ITEM
	        {
                doDropForeverSetup();           		
	            	
	        }
	        else if (selectedIndex == 2) // selected to VIEW ITEM
	        {
	            gv.sf.ShowFullDescription(it);
	        }                                

            resetInventory();
	    }
        public void doDropForeverSetup()
        {
            List<string> actionList = new List<string> { "Drop Forever", "Keep Item" };
            gv.itemListSelector.setupIBminiItemListSelector(gv, actionList, "Do you wish to drop this item forever?", "inventorydropforever");
            gv.itemListSelector.showIBminiItemListSelector = true;
        }
        public void doDropForever(int selectedIndex)
        {
            if (selectedIndex == 0)
            {
                //drop item
                ItemRefs itRef = GetCurrentlySelectedItemRefs();
                Item it = gv.mod.getItemByResRef(itRef.resref);
                if (!it.plotItem)
                {
                    gv.sf.RemoveItemFromInventory(itRef, 1);
                }
                else
                {
                    gv.sf.MessageBoxHtml("You can't drop this item.");
                }
                resetInventory();
            }
        }
        public void doSelectPcUseItemSetup()
        {
            // selected to USE ITEM
            List<string> pcNames = new List<string>();
            pcNames.Add("cancel");
            if (inCombat)
            {
                Player pc = gv.mod.playerList[gv.screenCombat.currentPlayerIndex];
                pcNames.Add(pc.name);
            }
            else
            {
                foreach (Player pc in gv.mod.playerList)
                {
                    pcNames.Add(pc.name);
                }
            }
            gv.itemListSelector.setupIBminiItemListSelector(gv, pcNames, "Selected PC to Use Item", "inventoryselectpcuseitem");
            gv.itemListSelector.showIBminiItemListSelector = true;
        }
        public void doSelectPcUseItem(int selectedIndex)
        {
            if (selectedIndex > 0)
            {
                try
                {
                    ItemRefs itRef = GetCurrentlySelectedItemRefs();
                    Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
                    if (inCombat)
                    {
                        //check to see if use IBScript first
                        /*if (!it.onUseItem.Equals("none"))
                        {
                            Player pc = gv.mod.playerList[gv.screenCombat.currentPlayerIndex];
                            doItemInventoryScriptBasedOnFilename(pc);
                            gv.screenCombat.currentCombatMode = "move";
                            gv.screenType = "combat";
                            gv.screenCombat.endPcTurn(false);
                        }*/
                        if (!it.onUseItemIBScript.Equals("none"))
                        {
                            doItemInventoryIBScript(gv.screenCombat.currentPlayerIndex);
                            gv.screenCombat.currentCombatMode = "move";
                            gv.screenType = "combat";
                            gv.screenCombat.endPcTurn(false);
                        }
                        else if (!it.onUseItemCastSpellTag.Equals("none"))
                        {
                            doItemInventoryCastSpellCombat(gv.screenCombat.currentPlayerIndex);
                            gv.screenCombat.currentCombatMode = "cast";
                            gv.screenType = "combat";
                        }
                    }
                    else
                    {
                        //check to see if use IBScript first
                        /*if (!it.onUseItem.Equals("none"))
                        {
                            Player pc = gv.mod.playerList[selectedIndex - 1];
                            doItemInventoryScriptBasedOnFilename(pc);
                        }*/
                        if (!it.onUseItemIBScript.Equals("none"))
                        {
                            doItemInventoryIBScript(selectedIndex - 1);
                        }
                        else if (!it.onUseItemCastSpellTag.Equals("none"))
                        {
                            doItemInventoryCastSpell(selectedIndex - 1);
                        }
                    }
                    resetInventory();
                }
                catch (Exception ex)
                {
                    gv.errorLog(ex.ToString());
                }
            }
        }

	    /*public void doItemInventoryScriptBasedOnFilename(Player pc)
        {
    	    if (isSelectedItemSlotInPartyInventoryRange())
		    {
    		    ItemRefs itRef = GetCurrentlySelectedItemRefs();
        	    gv.cc.doItemScriptBasedOnUseItem(pc, itRef, true);	    	
		    }
            resetInventory();
        }*/
	    public void doItemInventoryIBScript(int pcIndex)
        {
    	    if (isSelectedItemSlotInPartyInventoryRange())
		    {
    		    ItemRefs itRef = GetCurrentlySelectedItemRefs();
    		    Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
    		    gv.mod.indexOfPCtoLastUseItem = pcIndex;
                //do IBScript
                gv.cc.doIBScriptBasedOnFilename(it.onUseItemIBScript, it.onUseItemIBScriptParms);
                if (it.destroyItemAfterOnUseItemIBScript)
                {
                    gv.sf.RemoveItemFromInventory(itRef, 1);
                }	    	
		    }
            resetInventory();
        }
        public void doItemInventoryCastSpellCombat(int pcIndex)
        {
            if (isSelectedItemSlotInPartyInventoryRange())
            {
                ItemRefs itRef = GetCurrentlySelectedItemRefs();
                Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
                gv.mod.indexOfPCtoLastUseItem = pcIndex;
                gv.cc.currentSelectedSpell = gv.mod.getSpellByTag(it.onUseItemCastSpellTag);
                if (it.destroyItemAfterOnUseItemCastSpell)
                {
                    gv.sf.RemoveItemFromInventory(itRef, 1);
                }
            }
            resetInventory();
        }
        public void doItemInventoryCastSpell(int pcIndex)
        {
            if (isSelectedItemSlotInPartyInventoryRange())
            {
                ItemRefs itRef = GetCurrentlySelectedItemRefs();
                Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
                Spell sp = gv.mod.getSpellByTag(it.onUseItemCastSpellTag);
                Player pc = gv.mod.playerList[pcIndex];
                gv.mod.indexOfPCtoLastUseItem = pcIndex;
                gv.cc.doSpellBasedOnScriptOrEffectTag(sp, it, pc, true);
                if (it.destroyItemAfterOnUseItemCastSpell)
                {
                    gv.sf.RemoveItemFromInventory(itRef, 1);
                }
            }
            resetInventory();
        }
        public int GetIndex()
	    {
		    return inventorySlotIndex + (inventoryPageIndex * slotsPerPage);
	    }
	
	    public void doItemStacking()
	    {
		    for (int i = 0; i < gv.mod.partyInventoryRefsList.Count; i++)
		    {
			    ItemRefs itr = gv.mod.partyInventoryRefsList[i];
			    Item itm = gv.mod.getItemByResRefForInfo(itr.resref);
			    if (itm.isStackable)
			    {
				    for (int j = gv.mod.partyInventoryRefsList.Count - 1; j >= 0; j--)
				    {
					    ItemRefs it = gv.mod.partyInventoryRefsList[j];
					    //do check to see if same resref and then stack and delete
					    if ((it.resref.Equals(itr.resref)) && (i != j))
					    {
						    itr.quantity += it.quantity;
						    gv.mod.partyInventoryRefsList.RemoveAt(j);
					    }
				    }
			    }
		    }
	    }
	    public ItemRefs GetCurrentlySelectedItemRefs()
	    {
		    return gv.mod.partyInventoryRefsList[GetIndex()];
	    }
	    public bool isSelectedItemSlotInPartyInventoryRange()
	    {
		    return GetIndex() < gv.mod.partyInventoryRefsList.Count;
	    }
	    public void tutorialMessageInventory(bool helpCall)
        {
    	    if ((gv.mod.showTutorialInventory) || (helpCall))
		    {
    		    gv.sf.MessageBoxHtml(gv.cc.stringMessageInventory);    		
			    gv.mod.showTutorialInventory = false;
		    }
        }
    }
}
