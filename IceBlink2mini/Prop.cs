﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using Newtonsoft.Json;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2mini
{
    public class Prop 
    {
	    public int LocationX = 0;
	    public int LocationY = 0;
        public int lastLocationX = 0;
        public int lastLocationY = 0;
	    public string ImageFileName = "blank";
        public bool PropFacingLeft = true;
	    public string MouseOverText = "none";
	    public bool HasCollision = false;
	    public bool isShown = true;
	    public bool isActive = true;
	    public string PropTag = "newProp";
	    public string PropCategoryName = "newCategory";
	    public string ConversationWhenOnPartySquare = "none";
	    public string EncounterWhenOnPartySquare = "none";
	    public bool DeletePropWhenThisEncounterIsWon = false;
        public string OnEnterSquareIBScript = "none";
        public string OnEnterSquareIBScriptParms = "";
        public string OnEnterSquareScript = "none";
        public string OnEnterSquareScriptParm1 = "none";
        public string OnEnterSquareScriptParm2 = "none";
        public string OnEnterSquareScriptParm3 = "none";
        public string OnEnterSquareScriptParm4 = "none";
        public bool canBeTriggeredByPc = true;
        public bool canBeTriggeredByCreature = true;
        public int numberOfScriptCallsRemaining = 999;
        public bool isTrap = false;
        public int trapDCforDisableCheck = 10;
        public List<LocalInt> PropLocalInts = new List<LocalInt>();
	    public List<LocalString> PropLocalStrings = new List<LocalString>();
	    //All THE PROJECT LIVING WORLD STUFF
	    public int PostLocationX = 0;
	    public int PostLocationY = 0;
	    public List<WayPoint> WayPointList = new List<WayPoint>();
	    public int WayPointListCurrentIndex = 0;
	    public bool isMover = false;
	    public int ChanceToMove2Squares = 0;
	    public int ChanceToMove0Squares = 0;
	    public string MoverType = "post"; //post, random, patrol, daily, weekly, monthly, yearly
        //Removed save ignore for currentMoveToTarget,, othrisw all patroling props head towards 0,0 after loading a save
	    public Coordinate CurrentMoveToTarget = new Coordinate(0,0);
	    public bool isChaser = false;
        [JsonIgnore]
	    public bool isCurrentlyChasing = false;
	    public int ChaserDetectRangeRadius = 2;
	    public int ChaserGiveUpChasingRangeRadius = 3;
	    public int ChaserChaseDuration = 24;
        [JsonIgnore]
	    public int ChaserStartChasingTime = 0;
	    public int RandomMoverRadius = 5;
	    public bool ReturningToPost = false;
        public string OnHeartBeatIBScript = "none";
        public string OnHeartBeatIBScriptParms = "";
        public bool passOneMove = false;
        public int randomMoverTimerForNextTarget = 0;
        public int lengthOfLastPath = 0;
        public bool unavoidableConversation = false;
        public List<int> destinationPixelPositionXList = new List<int>();
        public List<int> destinationPixelPositionYList = new List<int>();
        //Turned the 2 ints below to floats for storing pix coordinates with higher precision
        public float currentPixelPositionX = 0;
        public float currentPixelPositionY = 0;
        public int pixelMoveSpeed = 1;
        public bool wasTriggeredLastUpdate = false;
        public bool blockTrigger = false;
    
        public Prop()
        {
    	
        }
    
        public void initializeProp()
        {
    	    CurrentMoveToTarget = new Coordinate(this.LocationX, this.LocationY);
        }
    
        public Prop DeepCopy()
        {
    	    Prop copy = new Prop();
		    copy.LocationX = this.LocationX;
		    copy.LocationY = this.LocationY;
            copy.blockTrigger = this.blockTrigger;
            copy.wasTriggeredLastUpdate = this.wasTriggeredLastUpdate;
            copy.lastLocationX = this.lastLocationX;
            copy.lastLocationY = this.lastLocationY;
            copy.ImageFileName = this.ImageFileName;
		    copy.PropFacingLeft = this.PropFacingLeft;
		    copy.MouseOverText = this.MouseOverText;
		    copy.HasCollision = this.HasCollision;
		    copy.isShown = this.isShown;
		    copy.isActive = this.isActive;
		    copy.PropTag = this.PropTag;
		    copy.PropCategoryName = this.PropCategoryName;
		    copy.ConversationWhenOnPartySquare = this.ConversationWhenOnPartySquare;
		    copy.EncounterWhenOnPartySquare = this.EncounterWhenOnPartySquare;
		    copy.DeletePropWhenThisEncounterIsWon = this.DeletePropWhenThisEncounterIsWon;
            copy.OnEnterSquareIBScript = this.OnEnterSquareIBScript;
            copy.OnEnterSquareIBScriptParms = this.OnEnterSquareIBScriptParms;
            copy.OnEnterSquareScript = this.OnEnterSquareScript;
            copy.OnEnterSquareScriptParm1 = this.OnEnterSquareScriptParm1;
            copy.OnEnterSquareScriptParm2 = this.OnEnterSquareScriptParm1;
            copy.OnEnterSquareScriptParm3 = this.OnEnterSquareScriptParm1;
            copy.OnEnterSquareScriptParm4 = this.OnEnterSquareScriptParm1;
            copy.canBeTriggeredByPc = this.canBeTriggeredByPc;
            copy.canBeTriggeredByCreature = this.canBeTriggeredByCreature;
            copy.numberOfScriptCallsRemaining = this.numberOfScriptCallsRemaining;
            copy.isTrap = this.isTrap;
            copy.trapDCforDisableCheck = this.trapDCforDisableCheck;
            copy.PropLocalInts = new List<LocalInt>();
            foreach (LocalInt l in this.PropLocalInts)
            {
                LocalInt Lint = new LocalInt();
                Lint.Key = l.Key;
                Lint.Value = l.Value;
                copy.PropLocalInts.Add(Lint);
            }        
            copy.PropLocalStrings = new List<LocalString>();
            foreach (LocalString l in this.PropLocalStrings)
            {
                LocalString Lstr = new LocalString();
                Lstr.Key = l.Key;
                Lstr.Value = l.Value;
                copy.PropLocalStrings.Add(Lstr);
            }
            //PROJECT LIVING WORLD STUFF
            copy.PostLocationX = this.PostLocationX;
            copy.PostLocationY = this.PostLocationY;
            copy.WayPointList = new List<WayPoint>();
            foreach (WayPoint coor in this.WayPointList)
            {
        	    WayPoint c = new WayPoint();
                c.X = coor.X;
                c.Y = coor.Y;
                c.areaName = coor.areaName;
                c.departureTime = coor.departureTime;
                copy.WayPointList.Add(c);
            }
            copy.WayPointListCurrentIndex = this.WayPointListCurrentIndex;
            copy.isMover = this.isMover;
            copy.ChanceToMove2Squares = this.ChanceToMove2Squares;
		    copy.ChanceToMove0Squares = this.ChanceToMove0Squares;
		    copy.MoverType = this.MoverType;
		    copy.CurrentMoveToTarget = new Coordinate(this.CurrentMoveToTarget.X, this.CurrentMoveToTarget.Y);
		    copy.isChaser = this.isChaser;
		    copy.isCurrentlyChasing = this.isCurrentlyChasing;
		    copy.ChaserDetectRangeRadius = this.ChaserDetectRangeRadius;
		    copy.ChaserGiveUpChasingRangeRadius = this.ChaserGiveUpChasingRangeRadius;
		    copy.ChaserChaseDuration = this.ChaserChaseDuration;
		    copy.ChaserStartChasingTime = this.ChaserStartChasingTime;
		    copy.RandomMoverRadius = this.RandomMoverRadius;
            copy.OnHeartBeatIBScript = this.OnHeartBeatIBScript;
            copy.OnHeartBeatIBScriptParms = this.OnHeartBeatIBScriptParms;
            copy.passOneMove = this.passOneMove;
            copy.randomMoverTimerForNextTarget = this.randomMoverTimerForNextTarget;
            copy.lengthOfLastPath = this.lengthOfLastPath;
            copy.unavoidableConversation = this.unavoidableConversation;
            copy.destinationPixelPositionXList = this.destinationPixelPositionXList;
            copy.destinationPixelPositionYList = this.destinationPixelPositionYList;
            copy.currentPixelPositionX = this.currentPixelPositionX;
            copy.currentPixelPositionY = this.currentPixelPositionY;
            copy.pixelMoveSpeed = this.pixelMoveSpeed;
		    return copy;
        }
    }
}
