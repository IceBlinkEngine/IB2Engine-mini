using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using Newtonsoft.Json;

namespace IceBlink2mini
{
    public class Trait 
    {
	    public string name = "newTrait";
	    public string tag = "newTraitTag";
	    public string traitImage = "sp_magebolt";
	    public string description = "";
	    public string prerequisiteTrait = "none";
	    public int skillModifier = 0;
	    public string skillModifierAttribute = "str";
	    public string useableInSituation = "Always"; //InCombat, OutOfCombat, Always, Passive
	    public string spriteFilename = "none";
	    public string spriteEndingFilename = "none";
        public string traitStartSound = "none";
        public string traitEndSound = "none";
        public int costSP = 10;	
	    public string traitTargetType = "Enemy"; //Self, Enemy, Friend, PointLocation
	    public string traitEffectType = "Damage"; //Damage, Heal, Buff, Debuff
        public bool isUsedForCombatSquareEffect = false;
        public AreaOfEffectShape aoeShape = AreaOfEffectShape.Circle;
        public int aoeRadius = 1;
	    public int range = 2;	
	    public string traitScript = "none";
        public bool isPassive = true; //passive traits are on all the time like two attack, cleave, evasion, etc. non-passive (or active) traits are used like spells (think power attack, remove trap, etc.)
        public bool usesTurnToActivate = true; //some traits are meant to be used in the same turn such as Power Attack and Set Trap
        public List<EffectTagForDropDownList> traitEffectTagList = new List<EffectTagForDropDownList>();
        public List<EffectTagForDropDownList> removeEffectTagList = new List<EffectTagForDropDownList>();
        
        public Trait()
	    {
		
	    }
	
	    public Trait DeepCopy()
	    {
		    Trait copy = new Trait();
		    copy.name = this.name;
		    copy.tag = this.tag;
		    copy.traitImage = this.traitImage;
		    copy.description = this.description;
		    copy.prerequisiteTrait = this.prerequisiteTrait;
		    copy.skillModifier = this.skillModifier;
		    copy.skillModifierAttribute = this.skillModifierAttribute;
		    copy.useableInSituation = this.useableInSituation;
		    copy.spriteFilename = this.spriteFilename;	
		    copy.spriteEndingFilename = this.spriteEndingFilename;
            copy.traitStartSound = this.traitStartSound;
            copy.traitEndSound = this.traitEndSound;
		    copy.costSP = this.costSP;
		    copy.traitTargetType = this.traitTargetType;
		    copy.traitEffectType = this.traitEffectType;
            copy.isUsedForCombatSquareEffect = this.isUsedForCombatSquareEffect;
            copy.aoeShape = this.aoeShape;
            copy.aoeRadius = this.aoeRadius;
		    copy.range = this.range;
		    copy.traitScript = this.traitScript;
            copy.isPassive = this.isPassive;

            copy.traitEffectTagList = new List<EffectTagForDropDownList>();
            foreach (EffectTagForDropDownList s in this.traitEffectTagList)
            {
                copy.traitEffectTagList.Add(s);
            }

            copy.removeEffectTagList = new List<EffectTagForDropDownList>();
            foreach (EffectTagForDropDownList s in this.removeEffectTagList)
            {
                copy.removeEffectTagList.Add(s);
            }
            return copy;
	    }
    }
}
