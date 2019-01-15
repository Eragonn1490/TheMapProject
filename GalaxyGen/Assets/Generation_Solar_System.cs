using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation_Solar_System : MonoBehaviour
{
    public enum SYSTEM_SIZE {SMALL, MEDIUM, LARGE, XLARGE };
    public enum SYSTEM_TYPE {STAR, BLACKHOLE, EMPTY, ASTEROID}
    public enum STARTEMPERATURES_TYPES          {O_BLUE_BIG_15, B_BLUE_MEDIUM_7, A_BLUE_SMALL_2p5, F_BLUE_1p3, F_WHITE_1p2, G_WHITE_1p1, G_YELLOW_1, K_ORANGE_p9, K_RED_p7, M_RED_p4, GIANT_RED_500, GIANT_BLUE_75, DWARF_RED_p45, DWARF_WHITE_p01, DWARF_BROWN_p_008};
    public float[] starSizes = new float[]      {15           , 7              , 2.5f            , 1.3f      , 1.2f       , 1.1f       , 1f        , .9f        , .7f     , .4f     , 500f         , 75f          , .45f         , .01f           , .008f            };
    public float[] starChanceMods = new float[] {.1f          , .12f           , .25f            , .5f       , .8f        , .9f        , 1f        , 1.4f       , 1.8f    , 2.5f    , .05f         , .02f         , 4.5f         , .05f           , .25f             };
    public float[] starTemps = new float[]      {35000f       , 17000f         , 9000f           , 7000f     , 6500f      , 6000f      , 5500f     , 4500f      , 4000f   , 3000f   , 2500f        , 45000f       , 3000f        , 100000f        , 1000f            };
    //avg temperatures in KELVIN: O+ blue > 25k, B blue 11k to 25k, A 7.5k to 11k, F 6 to 7.5k, G 5k to 6k, K 3.5k to 5k, M <3.5k, change to ranges later?
    //habitation default at 1 AU (earth) for 1.0 size star with 1.0 radius at 5500K
    //assume habitation range: .75 AU to 2.0 AU for default star
    //Venus is .72 AU away, Mars is 1.524 AU away
    //Mercury is .39 AU away in hot zone, assume hot zone start at least .2 AU away and hot zone goes to .75 AU
    //max cold distance at 50 AU, pluto is 40 au away
    float habitationConstant = 5500f; //temperature of earth sun in Kelvin
    float habModifierStartAU = .75f;
    float coldModifierStartAU = 2.0f;
    float coldModifierEndAU = 50f; 

    public float blackHoleChance;
    public float emptyChance;
    public float asteroidChance;
    //default: star chance

    public int xCoord;
    public int yCoord;
    public int universeSeed;
    public bool GENSYSTEM;
    public float systemGenScale = 1.0f; //modification to size on gen for everything.


    public GameObject[] starPrefabs;
    public GameObject[] planetPrefabs;

    public struct STAR_REGION_RANGE
    {
        public float rangeMinAU;
        public float rangeMaxAU;
    };

    STAR_REGION_RANGE hotRange;
    STAR_REGION_RANGE habRange;
    STAR_REGION_RANGE coldRange;
    STAR_REGION_RANGE asteroidRange;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GENSYSTEM)
        {
            GenerateSystem(xCoord, yCoord, universeSeed);
        }
    }


    void GenerateSystem(int xC, int yC, int rS)
    {

        Random.InitState((xC + yC) * rS + ((xC * yC) % rS));

        SYSTEM_SIZE size = SYSTEM_SIZE.MEDIUM;
        SYSTEM_TYPE type = SYSTEM_TYPE.STAR;
        STARTEMPERATURES_TYPES star = STARTEMPERATURES_TYPES.G_YELLOW_1;
        float starTemperature = starTemps[(int)STARTEMPERATURES_TYPES.G_YELLOW_1];
        float starSize = starSizes[(int)STARTEMPERATURES_TYPES.G_YELLOW_1];
        float minRangeAU = 15.0f + 15.0f / 3f; //one third of star size for corona
        hotRange.rangeMinAU = minRangeAU;
        hotRange.rangeMaxAU = minRangeAU + starTemperature / habitationConstant * starSize * habModifierStartAU; //5500f / 5500f * 1.0f * .75f
        habRange.rangeMinAU = hotRange.rangeMaxAU;
        habRange.rangeMaxAU = minRangeAU + starTemperature / habitationConstant * starSize * coldModifierStartAU; //5500f / 5500f * 1.0f * 1.5f;
        coldRange.rangeMinAU = habRange.rangeMaxAU;
        coldRange.rangeMaxAU = minRangeAU + starTemperature / habitationConstant * starSize * coldModifierEndAU; //5500f / 5500f * 1.0f * 50f;







    }
}
