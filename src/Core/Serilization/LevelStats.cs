using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Virtex.Lib.Vrtc.Core.Settings
{
    public class LevelStats
    {
        public int Int_LevelNumber;
        public string String_Level_Name;
        public double Double_LevelTimeInMilliSeconds;
        public bool Bool_OpenToPlay;

        public LevelStats(string LevelName, int levelNumber,
            float levelTimeInMilliSeconds,
            bool openToPlay)
        {
            String_Level_Name = LevelName;
            Int_LevelNumber = levelNumber;
            Double_LevelTimeInMilliSeconds = levelTimeInMilliSeconds;
            Bool_OpenToPlay = openToPlay;
        }


        //
        //Level Values
        //

        static bool testLevel = false;

        public static LevelStats[] LevelValues =
        {
            new LevelStats("TEST LEVEL",0,10000,true),

            //Chapter 1
            new LevelStats("L. 1 : l*w*h = ...",1,15320,true),
            new LevelStats("L. 2 : don't think, just run...",2,8410,testLevel),
            new LevelStats("L. 3 : timing and anticipation...",3,25630,testLevel),
            new LevelStats("L. 4 : you must be this high to ride...",4,25210,testLevel),
            new LevelStats("L. 5 : jump...jump and aim...",5,11611,testLevel),
            new LevelStats("L. 6 : this must be how frogs feel...",6,44216,testLevel),
            new LevelStats("L. 7 : going up...",7,35613,testLevel) ,
            new LevelStats("L. 8 : this must be how frogs feel...",8,44216,testLevel),
            new LevelStats("L. 9 : going up...",9,35613,testLevel),
            new LevelStats("L. 10 : this must be how frogs feel...",10,44216,testLevel),

            //Chapter 2
            new LevelStats("L. 11 : this must be how frogs feel...",11,44216,testLevel),
            new LevelStats("L. 12 : going up...",12,35613,testLevel) ,
            new LevelStats("L. 13 : this must be how frogs feel...",13,44216,testLevel),
            new LevelStats("L. 14 : going up...",14,35613,testLevel),
            new LevelStats("L. 15 : this must be how frogs feel...",15,44216,testLevel),
            new LevelStats("L. 16 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 17 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 18 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 19 : going up...",19,35613,testLevel),
            new LevelStats("L. 20 : this must be how frogs feel...",20,44216,testLevel),

            //Chapter 3
            new LevelStats("L. 21 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 22 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 23 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 24 : going up...",19,35613,testLevel),
            new LevelStats("L. 25 : this must be how frogs feel...",20,44216,testLevel),
            new LevelStats("L. 26 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 27 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 28 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 29 : going up...",19,35613,testLevel),
            new LevelStats("L. 30 : this must be how frogs feel...",20,44216,testLevel),
            
            //Chapter 4
            new LevelStats("L. 31 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 32 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 33 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 34 : going up...",19,35613,testLevel),
            new LevelStats("L. 35 : this must be how frogs feel...",20,44216,testLevel),
            new LevelStats("L. 36 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 37 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 38 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 39 : going up...",19,35613,testLevel),
            new LevelStats("L. 40 : this must be how frogs feel...",20,44216,testLevel),
                
            //Chapter 5
            new LevelStats("L. 41 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 42 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 43 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 44 : going up...",19,35613,testLevel),
            new LevelStats("L. 45 : this must be how frogs feel...",20,44216,testLevel),
            new LevelStats("L. 46 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 47 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 48 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 49 : going up...",19,35613,testLevel),
            new LevelStats("L. 50 : this must be how frogs feel...",20,44216,testLevel),
                
            //Chapter 6
            new LevelStats("L. 51 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 52 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 53 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 54 : going up...",19,35613,testLevel),
            new LevelStats("L. 55 : this must be how frogs feel...",20,44216,testLevel),
            new LevelStats("L. 56 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 57 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 58 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 59 : going up...",19,35613,testLevel),
            new LevelStats("L. 60 : this must be how frogs feel...",20,44216,testLevel),
                
            //Chapter 7
            new LevelStats("L. 61 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 62 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 63 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 64 : going up...",19,35613,testLevel),
            new LevelStats("L. 65 : this must be how frogs feel...",20,44216,testLevel),
            new LevelStats("L. 66 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 67 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 68 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 69 : going up...",19,35613,testLevel),
            new LevelStats("L. 70 : this must be how frogs feel...",20,44216,testLevel),

            //Chapter 8
            new LevelStats("L. 71 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 72 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 73 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 74 : going up...",19,35613,testLevel),
            new LevelStats("L. 75 : this must be how frogs feel...",20,44216,testLevel),
            new LevelStats("L. 76 : this must be how frogs feel...",16,44216,testLevel),
            new LevelStats("L. 77 : going up...",17,35613,testLevel) ,
            new LevelStats("L. 78 : this must be how frogs feel...",18,44216,testLevel),
            new LevelStats("L. 79 : going up...",19,35613,testLevel),
			new LevelStats("L. 80 : this must be how frogs feel...",20,44216,testLevel),

			//Chapter 9
			new LevelStats("L. 61 : this must be how frogs feel...",16,44216,testLevel),
			new LevelStats("L. 62 : going up...",17,35613,testLevel) ,
			new LevelStats("L. 63 : this must be how frogs feel...",18,44216,testLevel),
			new LevelStats("L. 64 : going up...",19,35613,testLevel),
			new LevelStats("L. 65 : this must be how frogs feel...",20,44216,testLevel),
			new LevelStats("L. 66 : this must be how frogs feel...",16,44216,testLevel),
			new LevelStats("L. 67 : going up...",17,35613,testLevel) ,
			new LevelStats("L. 68 : this must be how frogs feel...",18,44216,testLevel),
			new LevelStats("L. 69 : going up...",19,35613,testLevel),
			new LevelStats("L. 70 : this must be how frogs feel...",20,44216,testLevel),

			//Chapter 10
			new LevelStats("L. 71 : this must be how frogs feel...",16,44216,testLevel),
			new LevelStats("L. 72 : going up...",17,35613,testLevel) ,
			new LevelStats("L. 73 : this must be how frogs feel...",18,44216,testLevel),
			new LevelStats("L. 74 : going up...",19,35613,testLevel),
			new LevelStats("L. 75 : this must be how frogs feel...",20,44216,testLevel),
			new LevelStats("L. 76 : this must be how frogs feel...",16,44216,testLevel),
			new LevelStats("L. 77 : going up...",17,35613,testLevel) ,
			new LevelStats("L. 78 : this must be how frogs feel...",18,44216,testLevel),
			new LevelStats("L. 79 : going up...",19,35613,testLevel),
			new LevelStats("L. 80 : this must be how frogs feel...",20,44216,testLevel)
        };
    }
}
