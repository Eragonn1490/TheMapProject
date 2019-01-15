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
    public float diameterBaseOfDefaultMiles = 865000; //base diameter of sun
    //density values if desired

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
    public bool ALSOGENOBJECTS;
    public float systemGenScale = 1.0f; //modification to size on gen for everything.

    public enum STAR_SPRITES { };
    public Sprite[] starSpriteResources;
    public Sprite[] planetSpriteResources;

    public float AUtoSystemScale = 100f; //convert AU to unity transform units. Modify to get proper size.
    public float objectSizeToPlayScale = 1.0f; //
    public float basePlanetScale = .08f; //base scale for regular planets
    public float baseGasGiantScale = .23f; //base scale for gas giants
    public float minimumAUPlanetDistance = 8f; //minimum distance planets need to be apart in distance. 

    public struct STAR_REGION_RANGE
    {
        public float rangeMinAU;
        public float rangeMaxAU;
    };

    

    public GameObject systemContainer; //create and assign creations under this
    StarSystemInfo starSystemInfo;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GENSYSTEM) //ez debug gen
        {
            GenerateSystem(xCoord, yCoord, universeSeed);
            GENSYSTEM = false;
        }
    }


    void GenerateSystem(int xC, int yC, int rS)  
    {
        //depending on necessities: may need to dry-generate system objects thru script only, then pass into system gen later. Example: ability to click on map and run simplified version of this to get 
        //star (color, properties, etc), number of planets and types, etc, without entering a system scene.  this can be achieved thru not generating any gameobjects until a later phase.


        Random.InitState((xC + yC) * rS + ((xC * yC) % rS)); //note: the default random function may not be desirable, replace if necessary.

        SYSTEM_SIZE size = SYSTEM_SIZE.MEDIUM;
        SYSTEM_TYPE type = SYSTEM_TYPE.STAR;
        STARTEMPERATURES_TYPES star = STARTEMPERATURES_TYPES.G_YELLOW_1;
        float starTemperature = starTemps[(int)STARTEMPERATURES_TYPES.G_YELLOW_1];
        float starUnitySize = starSizes[(int)STARTEMPERATURES_TYPES.G_YELLOW_1];
        float starDiameter = starUnitySize * diameterBaseOfDefaultMiles; //based on diameter of Sol
        float starDensity = 1.4f;  //g/cm3   add density values later to be able to more easily determine mass and gravity
                                   //mass can be calculated from diameter and density: calculate volume from diameter -> radius... V = 4/3 * pi * r^3, Mass = density times volume


        //modify star size or temperature here for variance
        //
        //
        //
        float minRangeAU = starUnitySize + starUnitySize / 3f; //one third of star size for corona

        STAR_REGION_RANGE hotRange;
        STAR_REGION_RANGE habRange;
        STAR_REGION_RANGE coldRange;
        STAR_REGION_RANGE asteroidRange;

        hotRange.rangeMinAU = minRangeAU;
        hotRange.rangeMaxAU = minRangeAU + starTemperature / habitationConstant * starUnitySize * habModifierStartAU; //5500f / 5500f * 1.0f * .75f
        habRange.rangeMinAU = hotRange.rangeMaxAU;
        habRange.rangeMaxAU = minRangeAU + starTemperature / habitationConstant * starUnitySize * coldModifierStartAU; //5500f / 5500f * 1.0f * 1.5f;
        coldRange.rangeMinAU = habRange.rangeMaxAU;
        coldRange.rangeMaxAU = minRangeAU + starTemperature / habitationConstant * starUnitySize * coldModifierEndAU; //5500f / 5500f * 1.0f * 50f;
        //asteroid range between hab and cold, so far 1/6 of hab to maybe close 1/12 of cold, remember cold range is huge
        asteroidRange.rangeMinAU = habRange.rangeMaxAU - Random.Range(0, (habRange.rangeMaxAU - habRange.rangeMinAU) / 6f);
        asteroidRange.rangeMaxAU = coldRange.rangeMinAU + Random.Range(0, (coldRange.rangeMaxAU - coldRange.rangeMaxAU) / 12f);
        bool hasAsteroidBelt = true; //set randomly

        //generate star system info for celestial object
        starSystemInfo = new StarSystemInfo();
        starSystemInfo.SetCelestialBodyInfo(star, starUnitySize, starDiameter, starDensity, starTemperature, hasAsteroidBelt);
        starSystemInfo.SetRegionRangeValues(hotRange, habRange, coldRange, asteroidRange);
     



        if (ALSOGENOBJECTS)
        {
            GenerateSystemObjects();
        }

    }

    void GenerateSystemObjects()
    {
        //generate system gameobject, add any system handler scripts to this
        systemContainer = new GameObject();
        systemContainer.transform.parent = this.transform;
        

        //create star/blackole/other central object.

    }

    GameObject GenerateCentralObject(StarSystemInfo systemInfo)
    {
        GameObject newSystemObject = new GameObject();



        return newSystemObject;

    }

    OrbitableInfo GeneratePlanet(int region, List<OrbitableInfo> preGennedInRegion, STAR_REGION_RANGE genRange) //generate planet information: type of planet, size of planet, planet coloration (which sprite to use), gravity, etc. 
    {
        bool canGenerate = false;


        //determine if can place a planet here by checking the gen range and all planets already genned in that region.

        if (!canGenerate) //no space left in that orbital region, by minimumAUDistanceOfplanets there
            return null;

        OrbitableInfo planetInfo = new OrbitableInfo();

        return planetInfo;
    }

    GameObject GeneratePlanetGameobject()
    {

        GameObject newPlanet = new GameObject();
        //process to build a planet:
        //Planet Gameobject named: Planet_Type
        //+capsule collider 2d
        //+PlanetHitDetection.cs
        //+Orbitable.cs
        //+Rotate.cs
        //scaling: .08 for normal planets, .23 for giants, see current scaling factors at beginning
        //->GO:Planet Highlight
        //     +sprite renderer -> link sprite to highlight
        //->GO:SystemInfo
        //     +canvas renderer
        //     +Text
        //     +SystemInfo.cs -> link to Text, get other info
        //->GO:Minimap Icon
        //     +sprite renderer -> link sprite to ?
        //->GO:Planet Darkness
        //     +sprite renderer -> link to darkness. Scale appopriately (*7 for current normal assets?)
        //->GO:Gravity Field
        //     +PlanetGravity.cs -> set values by planet
        //     +circlecollider2d

        return newPlanet;
    }
}
