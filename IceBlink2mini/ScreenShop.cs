﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2mini
{
    public class ScreenShop 
    {
	    public Module mod;
	    public GameView gv;

	    public List<IbbButton> btnInventorySlot = new List<IbbButton>();
	    public IbbButton btnInventoryLeft = null;
	    public IbbButton btnInventoryRight = null;
	    public IbbButton btnPageIndex = null;
	    public IbbButton btnShopLeft = null;
	    public IbbButton btnShopRight = null;
	    public IbbButton btnShopPageIndex = null;
	    public List<IbbButton> btnShopSlot = new List<IbbButton>();
	    private IbbButton btnHelp = null;
	    private IbbButton btnReturn = null;
	    public int inventoryPageIndex = 0;
	    public int inventorySlotIndex = 0;
	    public int shopPageIndex = 0;
	    public int shopSlotIndex = 0;
	    public string currentShopTag = "";
	    public Shop currentShop = new Shop();
	    private string stringMessageShop = "";
        private IbbHtmlTextBox description;
	
        public ScreenShop(Module m, GameView g)
	    {
		    mod = m;
		    gv = g;
		    setControlsStart();
		    stringMessageShop = gv.cc.loadTextToString("MessageShop.txt");
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
			    btnInventoryLeft.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnInventoryLeft.Img2 = "ctrl_left_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    btnInventoryLeft.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnInventoryLeft.X = 7 * gv.squareSize;
			    btnInventoryLeft.Y = (5 * gv.squareSize) - (pH * 2);
                btnInventoryLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventoryLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnPageIndex == null)
		    {
			    btnPageIndex = new IbbButton(gv, 1.0f);
			    btnPageIndex.Img = "btn_small_off"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
			    btnPageIndex.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnPageIndex.Text = "1";
			    btnPageIndex.X = 8 * gv.squareSize;
			    btnPageIndex.Y = (5 * gv.squareSize) - (pH * 2);
                btnPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnInventoryRight == null)
		    {
			    btnInventoryRight = new IbbButton(gv, 1.0f);
			    btnInventoryRight.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnInventoryRight.Img2 = "ctrl_right_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnInventoryRight.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnInventoryRight.X = 9 * gv.squareSize;
			    btnInventoryRight.Y = (5 * gv.squareSize) - (pH * 2);
                btnInventoryRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventoryRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnShopLeft == null)
		    {
			    btnShopLeft = new IbbButton(gv, 1.0f);
			    btnShopLeft.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnShopLeft.Img2 = "ctrl_left_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    btnShopLeft.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnShopLeft.X = 7 * gv.squareSize;
			    btnShopLeft.Y = (1 * gv.squareSize) - (pH * 2);
                btnShopLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnShopLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnShopPageIndex == null)
		    {
			    btnShopPageIndex = new IbbButton(gv, 1.0f);
			    btnShopPageIndex.Img = "btn_small_off"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
			    btnShopPageIndex.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnShopPageIndex.Text = "1";
			    btnShopPageIndex.X = 8 * gv.squareSize;
			    btnShopPageIndex.Y = (1 * gv.squareSize) - (pH * 2);
                btnShopPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnShopPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnShopRight == null)
		    {
			    btnShopRight = new IbbButton(gv, 1.0f);
			    btnShopRight.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnShopRight.Img2 = "ctrl_right_arrow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnShopRight.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnShopRight.X = 9 * gv.squareSize;
			    btnShopRight.Y = (1 * gv.squareSize) - (pH * 2);
                btnShopRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnShopRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnReturn == null)
		    {
			    btnReturn = new IbbButton(gv, 1.2f);	
			    btnReturn.Text = "EXIT SHOP";
			    btnReturn.Img = "btn_large"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnReturn.Glow = "btn_large_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnReturn.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
			    btnReturn.Y = 9 * gv.squareSize + pH * 2;
                btnReturn.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturn.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnHelp == null)
		    {
			    btnHelp = new IbbButton(gv, 0.8f);	
			    btnHelp.Text = "HELP";
			    btnHelp.Img = "btn_small"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnHelp.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnHelp.X = 0 * gv.squareSize + padW * 1;
			    btnHelp.Y = 9 * gv.squareSize + pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    for (int j = 0; j < 10; j++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = "item_slot"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
			    btnNew.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    if (j < 5)
			    {
				    btnNew.X = ((j+2+4) * gv.squareSize) + (padW * (j+1));
				    btnNew.Y = 6 * gv.squareSize;
			    }
			    else
			    {
				    btnNew.X = ((j-5+2+4) * gv.squareSize) + (padW * ((j-5)+1));
				    btnNew.Y = 7 * gv.squareSize + padW;
			    }
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnInventorySlot.Add(btnNew);
		    }
		    for (int j = 0; j < 10; j++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);	
			    btnNew.Img = "item_slot"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
			    btnNew.Glow = "btn_small_glow"; // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    if (j < 5)
			    {
				    btnNew.X = ((j+2+4) * gv.squareSize) + (padW * (j+1));
				    btnNew.Y = 2 * gv.squareSize;
			    }
			    else
			    {
				    btnNew.X = ((j-5+2+4) * gv.squareSize) + (padW * ((j-5)+1));
				    btnNew.Y = 3 * gv.squareSize + padW;
			    }
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnShopSlot.Add(btnNew);
		    }
	    }

        public void redrawShop()
        {
    	    this.doItemStackingParty();
    	    
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
    	    int textH = (int)gv.fontHeight;
            int spacing = textH;
    	    int tabX = pW * 4;
    	    int tabX2 = 5 * gv.squareSize + pW * 2;
    	    int leftStartY = pH * 4;
    	    int tabStartY = 9 * gv.squareSize + pH * 2;
    	    int tabShopStartY = 4 * gv.squareSize + pH * 2;
    	
    	    gv.DrawText(currentShop.shopName, 7 * gv.squareSize, locY, "gy");
		
	
		    //DRAW LEFT/RIGHT ARROWS and PAGE INDEX of SHOP
		    btnShopPageIndex.Draw();
		    btnShopLeft.Draw();
		    btnShopRight.Draw();		
		
		    //DRAW ALL SHOP INVENTORY SLOTS of SHOP		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnShopSlot)
		    {
			    if (cntSlot == shopSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    if ((cntSlot + (shopPageIndex * 10)) < currentShop.shopItemRefs.Count)
			    {
				    ItemRefs itrs = currentShop.shopItemRefs[cntSlot + (shopPageIndex * 10)];
				    Item it = mod.getItemByResRefForInfo(itrs.resref);
                    //gv.cc.DisposeOfBitmap(ref btn.Img2);
                    btn.Img2 = it.itemImage;	
				    if (itrs.quantity < it.groupSizeForSellingStackableItems)
    			    {
    				    //less than the stack size for selling
    				    int cost = (itrs.quantity * storeSellValueForItem(it)) / it.groupSizeForSellingStackableItems;                        
    				    btn.Text = "" + cost;
    			    }
    			    else //have more than the stack size for selling
    			    {
    				    int full = (itrs.quantity / it.groupSizeForSellingStackableItems) * storeSellValueForItem(it);
    				    int part = ((itrs.quantity % it.groupSizeForSellingStackableItems) * storeSellValueForItem(it)) / it.groupSizeForSellingStackableItems;
    				    int total = full + part;
    				    btn.Text = "" + total;
    			    }
				
				    if (itrs.quantity > 1)
				    {
					    btn.Quantity = itrs.quantity + "";
				    }
				    else
				    {
					    btn.Quantity = "";
				    }
			    }
			    else
			    {
				    btn.Img2 = null;
				    btn.Text = "";
				    btn.Quantity = "";
			    }
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX of SHOP
		    locY = tabShopStartY;		
		    if ((shopSlotIndex + (shopPageIndex * 10)) < currentShop.shopItemRefs.Count)
		    {
                //DRAW DESCRIPTION BOX
			    Item it = mod.getItemByResRefForInfo(currentShop.shopItemRefs[shopSlotIndex + (shopPageIndex * 10)].resref);
			    string textToSpan = "<b><i><big>" + it.name + "</big></i></b><BR>";
	            if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
	            {
	        	    textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<br>";
	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	                textToSpan += "Attack Bonus: " + it.attackBonus + "<br>";
	                textToSpan += "Attack Range: " + it.attackRange + "<BR>";	        	
	            }    
	            else if (!it.category.Equals("General"))
	            {
	        	    textToSpan += "AC Bonus: " + it.armorBonus + "<br>";
	                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";	       
	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	            }
	            else if (it.category.Equals("General"))
	            {
	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	            }
                
                description.tbXloc = 13 * gv.squareSize;
                description.tbYloc = 2 * gv.squareSize;
                description.tbWidth = pW * 40;
                description.tbHeight = pH * 50;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
		    }
		
		    //DRAW LEFT/RIGHT ARROWS and PAGE INDEX
		    btnPageIndex.Draw();
		    btnInventoryLeft.Draw();
		    btnInventoryRight.Draw();		
		
		    //DRAW TEXT		
		    locY = (5 * gv.squareSize) + (pH * 2);
		    gv.DrawText("Party", locX + gv.squareSize * 4, locY, "gy");
            gv.DrawText("Inventory", locX + gv.squareSize * 4, locY += spacing, "gy");
		    locY = (5 * gv.squareSize) + (pH * 2);
		    gv.DrawText("Party", tabX2 + gv.squareSize * 5, locY, "yl");
            gv.DrawText(mod.goldLabelPlural + ": " + mod.partyGold, tabX2 + gv.squareSize * 5, locY += spacing, "yl");
		
		    //DRAW ALL PARTY INVENTORY SLOTS		
		    cntSlot = 0;
		    foreach (IbbButton btn in btnInventorySlot)
		    {
			    if (cntSlot == inventorySlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    if ((cntSlot + (inventoryPageIndex * 10)) < mod.partyInventoryRefsList.Count)
			    {
				    ItemRefs itr = mod.partyInventoryRefsList[cntSlot + (inventoryPageIndex * 10)];
				    Item it = mod.getItemByResRefForInfo(itr.resref);
                    //gv.cc.DisposeOfBitmap(ref btn.Img2);
                    btn.Img2 = it.itemImage;	
				    if (itr.quantity < it.groupSizeForSellingStackableItems)
    			    {
    				    //less than the stack size for selling
    				    int cost = (itr.quantity * storeBuyBackValueForItem(it)) / it.groupSizeForSellingStackableItems;
    				    btn.Text = "" + cost;
    			    }
    			    else //have more than the stack size for selling
    			    {
    				    int full = (itr.quantity / it.groupSizeForSellingStackableItems) * storeBuyBackValueForItem(it);
    				    int part = ((itr.quantity % it.groupSizeForSellingStackableItems) * storeBuyBackValueForItem(it)) / it.groupSizeForSellingStackableItems;
    				    int total = full + part;
    				    btn.Text = "" + total;
    			    }				
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
				    btn.Text = "";
				    btn.Quantity = "";
			    }
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;		
		    if ((inventorySlotIndex + (inventoryPageIndex * 10)) < mod.partyInventoryRefsList.Count)
		    {
			    ItemRefs itr = mod.partyInventoryRefsList[inventorySlotIndex + (inventoryPageIndex * 10)];
			    Item it = mod.getItemByResRefForInfo(itr.resref);
			    string textToSpan = "<b><i><big>" + it.name + "</big></i></b><BR>";
	            if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
	            {
	        	    textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<br>";
	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	                textToSpan += "Attack Bonus: " + it.attackBonus + "<br>";
	                textToSpan += "Attack Range: " + it.attackRange + "<BR>";	            
	            }    
	            else if (!it.category.Equals("General"))
	            {
	        	    textToSpan += "AC Bonus: " + it.armorBonus + "<br>";
	                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";	 
	                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	            }
	            else if (it.category.Equals("General"))
	            {
	        	    textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
	            }
                
                description.tbXloc = 13 * gv.squareSize;
                description.tbYloc = 6 * gv.squareSize;
                description.tbWidth = pW * 40;
                description.tbHeight = pH * 50;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
		    }
				
		    btnHelp.Draw();		
		    btnReturn.Draw();
            if (gv.showMessageBox)
            {
                gv.messageBox.onDrawLogBox();
            }
        }
	
        public string isUseableBy(Item it)
        {
    	    string strg = "";
    	    foreach (PlayerClass cls in mod.modulePlayerClassList)
    	    {
    		    string firstLetter = cls.name.Substring(0,1);
    		    foreach (ItemRefs itr in cls.itemsAllowed)
    		    {
    			    if (itr.resref.Equals(it.resref))
    			    {
    				    strg += firstLetter + ", ";
    			    }
    		    }
    	    }
    	    return strg;
        }
        public void doItemStackingParty()
	    {
		    for (int i = 0; i < mod.partyInventoryRefsList.Count; i++)
		    {
			    ItemRefs itr = mod.partyInventoryRefsList[i];
			    Item itm = mod.getItemByResRefForInfo(itr.resref);
			    if (itm.isStackable)
			    {
				    for (int j = mod.partyInventoryRefsList.Count - 1; j >= 0; j--)
				    {
					    ItemRefs it = mod.partyInventoryRefsList[j];
					    //do check to see if same resref and then stack and delete
					    if ((it.resref.Equals(itr.resref)) && (i != j))
					    {
						    itr.quantity += it.quantity;
						    mod.partyInventoryRefsList.RemoveAt(j);
					    }
				    }
			    }
		    }
	    }
        public void doItemStackingShop()
	    {
    	    //Not being used but leaving here just in case for future use
		    for (int i = 0; i < currentShop.shopItemRefs.Count; i++)
		    {
			    ItemRefs itr = currentShop.shopItemRefs[i];
			    for (int j = currentShop.shopItemRefs.Count - 1; j >= 0; j--)
			    {
				    ItemRefs it = currentShop.shopItemRefs[j];
				    //do check to see if same resref and then stack and delete
				    if ((it.resref.Equals(itr.resref)) && (i != j))
				    {
					    itr.quantity += it.quantity;
					    currentShop.shopItemRefs.RemoveAt(j);
				    }
			    }
		    }
	    }
        public int storeSellValueForItem(Item it)
        {
            int sellPrice = (int)(it.value * ((float)currentShop.sellPercent / 100f));
            if (sellPrice < 1) { sellPrice = 1; }
            return sellPrice;
        }
        public int storeBuyBackValueForItem(Item it)
        {
            int buyPrice = (int)(it.value * ((float)currentShop.buybackPercent / 100f));
            if (buyPrice < 1) { buyPrice = 1; }
            return buyPrice;
        }
    
        public void onTouchShop(int eX, int eY, MouseEventArgs e, MouseEventType.EventType eventType)
	    {
		    btnInventoryLeft.glowOn = false;
		    btnInventoryRight.glowOn = false;
		    btnHelp.glowOn = false;
		    btnReturn.glowOn = false;
		    btnShopLeft.glowOn = false;
		    btnShopRight.glowOn = false;
            if (gv.showMessageBox)
            {
                gv.messageBox.btnReturn.glowOn = false;
            }

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
			    else if (btnReturn.getImpact(x, y))
			    {
				    btnReturn.glowOn = true;
			    }
			    else if (btnShopLeft.getImpact(x, y))
			    {
				    btnShopLeft.glowOn = true;
			    }
			    else if (btnShopRight.getImpact(x, y))
			    {
				    btnShopRight.glowOn = true;
			    }
			    break;
			
		    case MouseEventType.EventType.MouseUp:
                x = (int)eX;
                y = (int)eY;
			
			    btnInventoryLeft.glowOn = false;
			    btnInventoryRight.glowOn = false;
			    btnHelp.glowOn = false;
			    btnReturn.glowOn = false;
			    btnShopLeft.glowOn = false;
			    btnShopRight.glowOn = false;

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

                for (int j = 0; j < 10; j++)
			    {
				    if (btnInventorySlot[j].getImpact(x, y))
				    {
					    if (inventorySlotIndex == j)
					    {
						    doShopInventoryActionsSetup();
					    }
					    inventorySlotIndex = j;
				    }
			    }
			    for (int j = 0; j < 10; j++)
			    {
				    if (btnShopSlot[j].getImpact(x, y))
				    {
					    if (shopSlotIndex == j)
					    {
						    doShopShopActionsSetup();
					    }
					    shopSlotIndex = j;
				    }
			    }
			    if (btnInventoryLeft.getImpact(x, y))
			    {
				    if (inventoryPageIndex > 0)
				    {
					    inventoryPageIndex--;
					    btnPageIndex.Text = (inventoryPageIndex + 1) + "";
				    }
			    }
			    else if (btnInventoryRight.getImpact(x, y))
			    {
				    if (inventoryPageIndex < 9)
				    {
					    inventoryPageIndex++;
					    btnPageIndex.Text = (inventoryPageIndex + 1) + "";
				    }
			    }
			    else if (btnShopLeft.getImpact(x, y))
			    {
				    if (shopPageIndex > 0)
				    {
					    shopPageIndex--;
					    btnShopPageIndex.Text = (shopPageIndex + 1) + "";
				    }
			    }
			    else if (btnShopRight.getImpact(x, y))
			    {
				    if (shopPageIndex < 9)
				    {
					    shopPageIndex++;
					    btnShopPageIndex.Text = (shopPageIndex + 1) + "";
				    }
			    }
			    else if (btnHelp.getImpact(x, y))
			    {
				    tutorialMessageShop();
			    }
			    else if (btnReturn.getImpact(x, y))
			    {
				    gv.screenType = "main";	
			    }
			    break;		
		    }
	    }

        public void doShopInventoryActionsSetup()
        {
            List<string> actionList = new List<string> { "Yes, Sell Item", "No, Keep Item" };
            gv.itemListSelector.setupIBminiItemListSelector(gv, actionList, "Do you wish to sell this item?", "shopinventoryaction");
            gv.itemListSelector.showIBminiItemListSelector = true;
        }
        public void doShopInventoryActions(int selectedIndex)
	    {
		    if ((inventorySlotIndex + (inventoryPageIndex * 10)) < mod.partyInventoryRefsList.Count)
		    {
                //DialogResult dlg = IBMessageBox.Show(gv, "Do you wish to sell this item?", enumMessageButton.YesNo);
                if (selectedIndex == 0)
                {
                    //sell item
                    ItemRefs itr = mod.partyInventoryRefsList[inventorySlotIndex + (inventoryPageIndex * 10)];
                    Item it = mod.getItemByResRef(itr.resref);
                    if (it != null)
                    {
                        if (!it.plotItem)
                        {
                            if (itr.quantity < it.groupSizeForSellingStackableItems)
                            {
                                //less than the stack size for selling
                                mod.partyGold += (itr.quantity * storeBuyBackValueForItem(it)) / it.groupSizeForSellingStackableItems;
                                ItemRefs itrCopy = itr.DeepCopy();
                                itrCopy.quantity = itr.quantity;
                                currentShop.shopItemRefs.Add(itrCopy);
                                //remove item and tag from party inventory
                                gv.sf.RemoveItemFromInventory(itr, itr.quantity);
                            }
                            else //have more than the stack size for selling
                            {
                                mod.partyGold += storeBuyBackValueForItem(it);
                                ItemRefs itrCopy = itr.DeepCopy();
                                itrCopy.quantity = it.groupSizeForSellingStackableItems;
                                currentShop.shopItemRefs.Add(itrCopy);
                                //remove item and tag from party inventory
                                gv.sf.RemoveItemFromInventory(itr, it.groupSizeForSellingStackableItems);
                            }
                        }
                        else
                        {
                            gv.sf.MessageBoxHtml("You can't sell this item.");
                        }
                    }
                }
                if (selectedIndex == 1)
                {
                    //do nothing
                }
		    }
	    }

        public void doShopShopActionsSetup()
        {
            List<string> actionList = new List<string> { "Yes, Buy Item", "No, Don't Buy Item" };
            gv.itemListSelector.setupIBminiItemListSelector(gv, actionList, "Do you wish to buy this item?", "shopshopaction");
            gv.itemListSelector.showIBminiItemListSelector = true;
        }
        public void doShopShopActions(int selectedIndex)
	    {
		    if ((shopSlotIndex + (shopPageIndex * 10)) < currentShop.shopItemRefs.Count)
		    {
                //check to see if have enough gold
	            Item it = mod.getItemByResRef(currentShop.shopItemRefs[shopSlotIndex + (shopPageIndex * 10)].resref);
                if (it != null)
                {
                    if (mod.partyGold < storeSellValueForItem(it))
                    {
                        gv.sf.MessageBoxHtml("Your party does not have enough gold to purchase this item.");
                        return;
                    }
                }
                //DialogResult dlg = IBMessageBox.Show(gv, "Do you wish to buy this item?", enumMessageButton.YesNo);
                if (selectedIndex == 0)
                {
                    //buy item
                    ItemRefs itr = currentShop.shopItemRefs[shopSlotIndex + (shopPageIndex * 10)];
                    it = mod.getItemByResRef(itr.resref);
                    if (it != null)
                    {
                        if (itr.quantity < it.groupSizeForSellingStackableItems)
                        {
                            //less than the stack size for selling
                            mod.partyGold -= (itr.quantity * storeSellValueForItem(it)) / it.groupSizeForSellingStackableItems;
                            //add item and tag to party inventory
                            mod.partyInventoryRefsList.Add(itr.DeepCopy());
                            //remove tag from shop list
                            currentShop.shopItemRefs.Remove(itr);
                        }
                        else //have more than the stack size for selling
                        {
                            //subtract gold from party
                            mod.partyGold -= storeSellValueForItem(it);
                            //add item and tag to party inventory
                            mod.partyInventoryRefsList.Add(itr.DeepCopy());
                            //remove tag from shop list
                            currentShop.shopItemRefs.Remove(itr);
                        }
                    }
                }
                if (selectedIndex == 1)
                {
                    //do nothing
                }
		    }
	    }
	
	    public void tutorialMessageShop()
        {
		    gv.sf.MessageBoxHtml(this.stringMessageShop);    	
        }
    }
}
