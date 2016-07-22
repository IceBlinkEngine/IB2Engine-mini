using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2mini
{
    public class TileEnc 
    {
	    public string Layer1Filename = "t_grass";
        public string Layer2Filename = "t_blank";
        public int Layer1Rotate = 0;
        public int Layer2Rotate = 0;
        public bool Layer1Mirror = false;
        public bool Layer2Mirror = false;
        public bool Walkable = true;
	    public bool LoSBlocked = false;
        
        public TileEnc()
	    {
	
	    }
    }
}
