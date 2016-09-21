﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2mini
{
    public class FloatyText
    {
        public Coordinate location = new Coordinate();
        public string value = "";
        public string color = "red"; //red, yellow, blue, green, white
        public int timer = 0;
        public int timerLength = 4000; //time in ms
        public int timeToLive = 2000;
        public int z = 0; //float height multiplier

        public FloatyText()
        {

        }
        public FloatyText(int X, int Y, string val)
        {
            location = new Coordinate(X, Y);
            value = val;
            color = "red";
        }
        public FloatyText(int X, int Y, string val, string clr, int length)
        {
            location = new Coordinate(X, Y);
            value = val;
            color = clr;
            timerLength = length;
            timeToLive = length;
        }
        public FloatyText(Coordinate coor, string val)
        {
            location = coor;
            value = val;
            color = "red";
        }
        public FloatyText(Coordinate coor, string val, string clr)
        {
            location = coor;
            value = val;
            color = clr;
        }
    }
}
