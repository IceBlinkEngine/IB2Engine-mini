using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Text;
using System.IO;
//using IceBlink;
using System.ComponentModel;
using Newtonsoft.Json;

namespace IceBlink2mini
{
    public class Area 
    {
        //public string rememberedWeatherName = "";
        //public float rememberedWeatherDuration = 0;
        //public float skyCoverCloudsChance = 0;
        //public float skyCoverSeveriy
        public string Filename = "newArea";
        public int AreaVisibleDistance = 4;
        public bool RestingAllowed = false;
        public int MapSizeX = 16;
        public int MapSizeY = 16;
        public bool UseMiniMapFogOfWar = true;
	    public bool areaDark = false;
	    public bool UseDayNightCycle = false;
        //public bool useMiniProps = false;
        //public bool useSuperTinyProps = false;
	    public int TimePerSquare = 6; //in minutes for now
	    //public string MusicFileName = "forest.mp3";
	    //public string ImageFileName = "none";
        //public int backgroundImageStartLocX = 0;
        //public int backgroundImageStartLocY = 0;
	    
	    //public string AreaMusic = "none";
	    //public int AreaMusicDelay = 0;
	    //public int AreaMusicDelayRandomAdder = 0;
	    //public string AreaSounds = "none";
	    //public int AreaSoundsDelay = 0;
	    //public int AreaSoundsDelayRandomAdder = 0;
	    //public List<Tile> Tiles = new List<Tile>();
        public List<string> Layer1Filename = new List<string>();
        public List<int> Layer1Rotate = new List<int>();
        public List<int> Layer1Mirror = new List<int>();
        public List<string> Layer2Filename = new List<string>();
        public List<int> Layer2Rotate = new List<int>();
        public List<int> Layer2Mirror = new List<int>();
        public List<string> Layer3Filename = new List<string>();
        public List<int> Layer3Rotate = new List<int>();
        public List<int> Layer3Mirror = new List<int>();
        public List<int> Walkable = new List<int>();
        public List<int> LoSBlocked = new List<int>();
        public List<int> Visible = new List<int>();
        public List<Prop> Props = new List<Prop>();
	    public List<string> InitialAreaPropTagsList = new List<string>();
	    public List<Trigger> Triggers = new List<Trigger>();
	    public int NextIdNumber = 100;
        public string OnHeartBeatIBScript = "none";
        public string OnHeartBeatIBScriptParms = "";
	    //public List<LocalInt> AreaLocalInts = new List<LocalInt>();
	    //public List<LocalString> AreaLocalStrings = new List<LocalString>();
        public string inGameAreaName = "newArea";
        //public string areaWeatherScript = "";
        //public string areaWeatherScriptParms = "";
        //public string effectChannelScript1 = "";
        //public string effectChannelScript2 = "";
        //public string effectChannelScript3 = "";
        //public string effectChannelScript4 = "";
        //public string effectChannelScriptParms1 = "";
        //public string effectChannelScriptParms2 = "";
        //public string effectChannelScriptParms3 = "";
        //public string effectChannelScriptParms4 = "";
        //public WeatherEffect areaWeather = new WeatherEffect();
        //public string areaWeatherName = "";
        //public int weatherDurationMultiplierForScale = 1;
        //public string westernNeighbourArea = "";
        //public string easternNeighbourArea = "";
        //public string northernNeighbourArea = "";
        //public string southernNeighbourArea = "";

        //public string sourceBitmapName = "";
                
        public Area()
	    {	
	    }
	
	    public bool GetBlocked(int playerXPosition, int playerYPosition)
        {        
            if (this.Walkable[playerYPosition * this.MapSizeX + playerXPosition] == 0)
            {
                return true;
            }
            foreach (Prop p in this.Props)
            {
                if ((p.LocationX == playerXPosition) && (p.LocationY == playerYPosition))
                {
                    if (p.HasCollision)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
	
	    public Trigger getTriggerByLocation(int x, int y)
        {
            foreach (Trigger t in this.Triggers)
            {
                foreach (Coordinate p in t.TriggerSquaresList)
                {
                    if ((p.X == x) && (p.Y == y))
                    {
                        return t;
                    }
                }
            }
            return null;
        }
	    public Trigger getTriggerByTag(String tag)
        {
            foreach (Trigger t in this.Triggers)
            {
                if (t.TriggerTag.Equals(tag))
                {
            	    return t;
                }
            }
            return null;
        }
	    public Prop getPropByLocation(int x, int y)
        {
            foreach (Prop p in this.Props)
            {
                if ((p.LocationX == x) && (p.LocationY == y))
                {
                    return p;
                }            
            }
            return null;
        }
	    public Prop getPropByTag(String tag)
        {
            foreach (Prop p in this.Props)
            {
                if (p.PropTag.Equals(tag))
                {
            	    return p;
                }
            }
            return null;
        }
    }
}
