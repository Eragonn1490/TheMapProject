using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Generation_Solar_System : MonoBehaviour
{
    //public static Generation_Solar_System instance;

    public string sysNameGen;
    public bool FORCESYSTEMTYPE;
    public STARTEMPERATURES_TYPES FORCETYPE;

    
    //public enum SYSTEM_SIZE {SMALL, MEDIUM, LARGE, XLARGE };
    //public enum SYSTEM_TYPE {STAR, BLACKHOLE, EMPTY, ASTEROID};
    public enum STARTEMPERATURES_TYPES             {O_BLUE_BIG_10, B_BLUE_MEDIUM_7, A_BLUE_SMALL_2p5, F_BLUE_1p3, F_WHITE_1p2, G_WHITE_1p1, G_YELLOW_1, K_ORANGE_p9, K_RED_p7, M_RED_p4, GIANT_RED_50, GIANT_BLUE_25, DWARF_RED_p2, DWARF_WHITE_p05, DWARF_BROWN_p_02};
    private float[] starSizes = new float[]        {10           , 7              , 2.5f            , 1.3f      , 1.2f       , 1.1f       , 1f        , .9f        , .7f     , .4f     , 50f          , 25f          , .2f         , .05f           , .02f            };
    private float[] starChanceMods = new float[]   {.1f          , .12f           , .25f            , .5f       , .8f        , .9f        , 1f        , 1.4f       , 1.8f    , 2.5f    , .05f         , .02f         , 4.5f         , .05f           , .25f             };
    private float[] starTemps = new float[]        {20000f       , 17000f         , 9000f           , 7000f     , 6500f      , 6000f      , 5500f     , 4500f      , 4000f   , 3000f   , 2500f        , 30000f       , 3000f        , 100000f        , 1000f            };
    private float[] starDistanceMods = new float[] {.3f          , .4f            , .7f             , .8f       , .9f        , 1.0f       , 1.0f      , 1.2f       , 1.4f    , 1.8f    , .8f         , .15f         , 2.5f         , 2.0f           , 3.0f             };
    private int  [] maxPlanetsSystem = new int[]   {100          , 100            , 80              , 50        , 48         , 46         , 42        , 36         , 24      , 20      , 20           , 100          , 12           , 10             , 8                };
    private float minOrbitalSpeed = .4f;
    private float maxOrbitalSpeed = 45f;
    //moon stuff, dump into better place later
    private float hotRangeMoonChance = .15f;
    private float hotRangeChanceOfAnotherMoon = .25f;
    private int hotRangeMoonMax = 2;
    private float hotRangeMoonIsAnotherPlanetChance = .25f; //more likely in hot range as a moon itself probably would just get removed due to distance from sun
    private float habRangeMoonChance = .5f;
    private float habChanceOfAdditionalMoons = .5f;
    private int habRangeMoonMax = 5;
    private float habRangeMoonIsAnotherPlanetChance = .05f;
    private float coldRangeMoonChance = .75f;
    private float coldChanceOfAdditionalMoons = .75f;
    private int coldRangeMoonMax = 6;
    private float coldRangeMoonIsAnotherPlanetChance = .2f; //less chance than hot but much more potential chances
    private float gasChanceMoonModifier = 3.0f;
    private int gasChanceMoonMaxModifier = 2;
    

    //some reduction in values for smaller system on number of planets due to scale, can be modified or removed.
    //reduced size and temperatures of some things to clamp size within safe float distances
    public float diameterBaseOfDefaultMiles = 865000; //base diameter of sun
    public float baseSpeedDefaultEarthMPH = 66615; //base speed at 1 AU, cheat value for determinism of speed.
    public float orbitalVelocityModifier = .00001f; //66615 * .0002 = ~20f, etc.  Modify mph to get acceptable speed.
                                                   //relative velocities to earth
                                                   //Velocity = 1/R^(1/2) (1 divided by sqrt of AU)
                                                   //earth at 92.96 million miles, 1AU
                                                   //1 / 1  = 1
                                                   //conversion to mph = * baseSpeedDefaultEarthMPH


    //density values if desired

    //avg temperatures in KELVIN: O+ blue > 25k, B blue 11k to 25k, A 7.5k to 11k, F 6 to 7.5k, G 5k to 6k, K 3.5k to 5k, M <3.5k, change to ranges later?
    //habitation default at 1 AU (earth) for 1.0 size star with 1.0 radius at 5500K
    //assume habitation range: .75 AU to 2.0 AU for default star
    //Venus is .72 AU away, Mars is 1.524 AU away
    //Mercury is .39 AU away in hot zone, assume hot zone start at least .2 AU away and hot zone goes to .75 AU
    //max cold distance at 50 AU, pluto is 40 au away
    float habitationConstant = 5500f; //temperature of earth sun in Kelvin
    float habModifierStartAU = .85f; //increased slightly, original 75
    float coldModifierStartAU = 2.5f; //also increased slightly, original 2.0
    float coldModifierEndAU = 15f; //shrunk a bit to squish stuff together, original 50

    public float blackHoleChance;
    public float emptyChance;
    public float asteroidChance;
    //default: star chance

    public float xCoord;
    public float yCoord;
    public int universeSeed;
    public int systemID;
    public bool GENSYSTEM;
    public bool ALSOGENOBJECTS;
    public float systemGenScale = 1.0f; //modification to size on gen for everything, not currently used


    public enum SYSTEM_REGION_RANGES { REGION_HOT, REGION_HAB, REGION_COLD, REGION_ASTEROID };


    //these will need some work: adding in various types of each for variety in each category, etc. As is: defining some region ranges for sprites manually.
    //may want to split out any special planet types as well. can also individually define chances of each planet sprite appearing, or, if you want more complexity, define some sort of planet formation algo that then determines result based on planet composition.
    public enum STAR_SPRITES { };
    public Sprite[] starSpriteResources;
    //public enum PLANETTYPESNORMAL { PLANET0, PLANET1, PLANET2, PLANET3 }; //fill out for defined types if desired
    //public enum MOONTYPESNORMAL { MOON0, MOON1, MOON2, MOON3, MOON4, MOON5};
    public enum ORBITAL_TYPES {ORBITAL_SUN, ORBITAL_BLACK_HOLE, ORBITAL_PLANET, ORBITAL_GAS_GIANT, ORBITAL_ASTEROID, ORTIBAL_MOON }; //how it looks
    public int spriteResourceHotEnd;
    public int spriteResourceHabEnd;
    //cold end is length
    public Sprite[] planetSpriteResources;
    public Sprite[] moonSpriteResources;
    public Sprite[] planetGasGiantsResources;
    public Sprite[] asteroidResources;

    public GameObject systemLinkObjectPrefab; //whatever warp thing to exit the system there is.

    public float AUtoSystemScale = 25f; //convert AU to unity transform units. Modify to get proper size. (original 100 was way too big)
    public float objectSizeToPlayScale = 1.0f; //
    public float basePlanetScale = .08f; //base scale for regular planets
    public float baseGasGiantScale = .23f; //base scale for gas giants
    public float minimumAUPlanetDistance = .42f; //minimum distance planets need to be apart in AU distance. reminder: take into account system scaling.

    [System.Serializable]
    public class Star_Region_Range
    {
        public float rangeMinAU;
        public float rangeMaxAU;

        public Star_Region_Range(float minRangeSet, float maxRangeSet)
        {
            rangeMinAU = minRangeSet;
            rangeMaxAU = maxRangeSet;
        }

    }

    [System.Serializable]
    public class Spawn_Counts_Planets
    {
        [SerializeField]
        public int planetsMin = 0;
        [SerializeField]
        public int planetsMax = 1;

        public Spawn_Counts_Planets(int min, int max)
        {
            planetsMin = min;
            planetsMax = max;
        }
    }

    public Spawn_Counts_Planets smallSystem = new Spawn_Counts_Planets(1,4);
    public Spawn_Counts_Planets mediumSystem = new Spawn_Counts_Planets(5, 15);
    public Spawn_Counts_Planets largeSystem = new Spawn_Counts_Planets(15, 28);
    public Spawn_Counts_Planets extraLargeSystem = new Spawn_Counts_Planets(28, 40); //reduced from 70+


    public float baseGasGiantChance = 20f;
    public float baseAsteroidChance = 25f;

    public GameObject systemContainer; //create and assign creations under this
    public StarSystemInfo starSystemInfo;

    private void Awake()
    {

    }

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


    public void ClearSystem()
    {
        //if (systemContainer != null)
        //{
        //    DestroyImmediate(systemContainer);
        //}
    }

    public void LoadInGenFromSolarSystem(SolarSystem pregenned, SolarSystemData pregennedData, Galaxy refGalaxy)
    {
        Debug.Log("attepting load in system");


        starSystemInfo = new StarSystemInfo();
        starSystemInfo.systemDistanceScaleMod = pregenned.systemDistanceScaleMod;
        starSystemInfo.mainBodyType = pregenned.mainBodyType;
        starSystemInfo.systemSize = pregenned.systemSize;
        starSystemInfo.systemType = pregenned.systemType;
        starSystemInfo.mainBodyUnitySizeScale = pregenned.mainBodyUnitySizeScale;
        starSystemInfo.mainBodyPhysicalSize = pregenned.mainBodyPhysicalSize;
        starSystemInfo.mainBodyDensity = pregenned.mainBodyDensity;
        starSystemInfo.mainBodyTemperatureKelvin = pregenned.mainBodyTemperatureKelvin;
        starSystemInfo.hasAsteroidBelt = pregenned.hasAsteroidBelt;
        starSystemInfo.baseSystemAngle = pregenned.baseSystemAngle;

        starSystemInfo.hotRange = pregenned.hotRange;
        starSystemInfo.coldRange = pregenned.coldRange;
        starSystemInfo.habRange = pregenned.habRange;
        starSystemInfo.asteroidBeltRange = pregenned.asteroidBeltRange;

        starSystemInfo.orbitals = pregenned.orbitals;
        starSystemInfo.AUFarPlanet = pregenned.AUFarPlanet;
        starSystemInfo.AUtoSystemScale = pregenned.AUtoSystemScale;

        starSystemInfo.systemName = pregennedData.name;
        starSystemInfo.generatedSystemSeed = pregennedData.generationSeed;
        starSystemInfo.systemID = pregennedData.id;
        starSystemInfo.posX = pregennedData.posX;
        starSystemInfo.posY = pregennedData.posY;
        Debug.Log("Position of system is " + starSystemInfo.posX + " " + starSystemInfo.posY);
        starSystemInfo.systemSize = pregennedData.size;
        starSystemInfo.systemType = pregennedData.type;
        
        Debug.Log("linked id gen is " + starSystemInfo.systemID);
        


        starSystemInfo.linkedSystemIDs = pregennedData.linkedIDs;
        starSystemInfo.linkedSystemNames = new string[starSystemInfo.linkedSystemIDs.Length];
        starSystemInfo.linkedSystemPositionsX = new float[starSystemInfo.linkedSystemIDs.Length];
        starSystemInfo.linkedSystemPositionsY = new float[starSystemInfo.linkedSystemIDs.Length];
        Debug.Log("and has " + starSystemInfo.linkedSystemIDs.Length);
        for (int i = 0; i < starSystemInfo.linkedSystemIDs.Length; i++)
        {
            starSystemInfo.linkedSystemNames[i] = refGalaxy.solarSystems[starSystemInfo.linkedSystemIDs[i]].name;
            starSystemInfo.linkedSystemPositionsX[i] = refGalaxy.solarSystems[starSystemInfo.linkedSystemIDs[i]].posX;
            starSystemInfo.linkedSystemPositionsY[i] = refGalaxy.solarSystems[starSystemInfo.linkedSystemIDs[i]].posY;
        }
        starSystemInfo.numberActualLinks = pregennedData.linked;


        GenerateSystemObjects(starSystemInfo);

            /*
             * //Public
	public int id;
    public int generationSeed;
	public float posX;
	public float posY;
	public string name;
	public SolarSystemSize size;
	public SolarSystemType type;
	public bool generated = false;
	public int[] linkedIDs;
	public int linked;
             * */
    }

    public SolarSystem GetSystemFromGen(int xC, int yC, int rS) //TODO: if types are generated inside gal gen, then those will need to get passed in here and set as types for generation, as otherwise this class generates everything from scratch.
    {
        
        GenerateSystem(xC, yC, rS);

        var solarReturn = new SolarSystem();
        solarReturn.systemDistanceScaleMod = starSystemInfo.systemDistanceScaleMod;
        solarReturn.mainBodyType = starSystemInfo.mainBodyType;
        solarReturn.systemSize = starSystemInfo.systemSize;
        solarReturn.systemType = starSystemInfo.systemType;
        solarReturn.mainBodyUnitySizeScale = starSystemInfo.mainBodyUnitySizeScale;
        solarReturn.mainBodyPhysicalSize = starSystemInfo.mainBodyPhysicalSize;
        solarReturn.mainBodyDensity = starSystemInfo.mainBodyDensity;
        solarReturn.mainBodyTemperatureKelvin = starSystemInfo.mainBodyTemperatureKelvin;
        solarReturn.hasAsteroidBelt = starSystemInfo.hasAsteroidBelt;
        solarReturn.baseSystemAngle = starSystemInfo.baseSystemAngle;

        solarReturn.hotRange = starSystemInfo.hotRange;
        solarReturn.coldRange = starSystemInfo.coldRange;
        solarReturn.habRange = starSystemInfo.habRange;
        solarReturn.asteroidBeltRange = starSystemInfo.asteroidBeltRange;

        solarReturn.orbitals = starSystemInfo.orbitals;
        solarReturn.AUFarPlanet = starSystemInfo.AUFarPlanet;
        solarReturn.AUtoSystemScale = starSystemInfo.AUtoSystemScale;

        /*
         * 
         * 
            public int id;
            public int generationSeed;
            public float posX;
            public float posY;
            public string name;
            public SolarSystemSize size;
            public SolarSystemType type;
            public bool generated = true;
            public int[] linkedIDs;
            public int linked;
            //from Generation_Solar_System
            public float systemDistanceScaleMod;
            public Generation_Solar_System.STARTEMPERATURES_TYPES mainBodyType;
            public SolarSystemSize systemSize;
            public SolarSystemType systemType;
            public float mainBodyUnitySizeScale;
            public float mainBodyPhysicalSize;
            public float mainBodyDensity;
            public float mainBodyTemperatureKelvin;
            public bool hasAsteroidBelt;
            public float baseSystemAngle; //for elliptical orbits along same line-ish

            public Generation_Solar_System.Star_Region_Range hotRange;
            public Generation_Solar_System.Star_Region_Range coldRange;
            public Generation_Solar_System.Star_Region_Range habRange;
            public Generation_Solar_System.Star_Region_Range asteroidBeltRange;

            public List<OrbitableInfo> orbitals = new List<OrbitableInfo>();
            //public List<GameObject> orbitalGameobjects = new List<GameObject>();
         * */



        return solarReturn;
    }

    public void GenerateSystem(float xC, float yC, int rS)  
    {
        rS += 11521; //avoid 0
        //depending on necessities: may need to dry-generate system objects thru script only, then pass into system gen later. Example: ability to click on map and run simplified version of this to get 
        //star (color, properties, etc), number of planets and types, etc, without entering a system scene.  this can be achieved thru not generating any gameobjects until a later phase.

        //Debug.Log("doing generate of " + xC + " " + yC + " " + rS);
        Random.InitState((int)((xC + yC) * 10000f) * rS + ((int)((xC * yC) * 10000f) % rS)); //note: the default random function may not be desirable, replace if necessary.
        
        starSystemInfo = new StarSystemInfo();
        starSystemInfo.generatedSystemSeed = (int)((xC + yC) * 10000f) * rS + ((int)((xC * yC) * 10000f) % rS);
        starSystemInfo.systemName = sysNameGen;
        



        int randSize = Random.Range(0, 4);
        int randStarType = Random.Range(0, 15);
        SolarSystemSize size = (SolarSystemSize)randSize;
        
        SolarSystemType type = SolarSystemType.Star;
        STARTEMPERATURES_TYPES star = (STARTEMPERATURES_TYPES)randStarType;
        if (FORCESYSTEMTYPE)
        {
            star = FORCETYPE;
        }
        float starTemperature = starTemps[(int)star];
        float starUnitySize = starSizes[(int)star];
        float starDistanceMod = starDistanceMods[(int)star];
        float starDensity = 1.4f;  //g/cm3   add density values later to be able to more easily determine mass and gravity
                                   //mass can be calculated from diameter and density: calculate volume from diameter -> radius... V = 4/3 * pi * r^3, Mass = density times volume
                     


        //modify star size or temperature here for variance
        starUnitySize += (Random.Range(-starUnitySize / 10f, starUnitySize / 10f)); //current assets sprite size = you need to be about 25 dist away at 1.0 scale
        //Debug.Log("star unity size is " + starUnitySize);
        float starMinDistCalc = (starUnitySize + starUnitySize/2f )* 25f; //*  //one half of star size for corona
        //account for distance scale of system
        starMinDistCalc /= starDistanceMod; //for .2, dist will be say, 50 / .2 = 250
        starTemperature += (Random.Range(-starTemperature / 10f, starTemperature / 10f));
        float starDiameter = starUnitySize * diameterBaseOfDefaultMiles; //based on diameter of Sol
        float mandatoryMinAU = .25f; //brown dwarf sanity
        float minRangeAU = starMinDistCalc;
        //Debug.Log("min range unity size is " + minRangeAU);
        minRangeAU /= AUtoSystemScale;
        //Debug.Log("min range AU scale is " + minRangeAU);
        if (minRangeAU < mandatoryMinAU)
        {
            minRangeAU = mandatoryMinAU;
        }

        Star_Region_Range hotRange = new Star_Region_Range(0,0);
        Star_Region_Range habRange = new Star_Region_Range(0, 0); ;
        Star_Region_Range coldRange = new Star_Region_Range(0, 0); ;
        Star_Region_Range asteroidRange = new Star_Region_Range(0, 0); ;

        float minDistHot = .5f; //brown dwarf sanity
        float minDistHab = .65f;
        float minDistCold = 2.0f;
        hotRange.rangeMinAU = minRangeAU;
        hotRange.rangeMaxAU = minRangeAU + starTemperature / habitationConstant * starUnitySize * habModifierStartAU; //5500f / 5500f * 1.0f * .75f
        if (hotRange.rangeMaxAU - hotRange.rangeMinAU < minDistHot)
        {
            hotRange.rangeMaxAU = hotRange.rangeMinAU + minDistHot;
        }
        habRange.rangeMinAU = hotRange.rangeMaxAU;
        habRange.rangeMaxAU = minRangeAU + starTemperature / habitationConstant * starUnitySize * coldModifierStartAU; //5500f / 5500f * 1.0f * 1.5f;
        if (habRange.rangeMaxAU - habRange.rangeMinAU < minDistHab)
        {
            habRange.rangeMaxAU = habRange.rangeMinAU + minDistHab;
        }
        coldRange.rangeMinAU = habRange.rangeMaxAU;
        coldRange.rangeMaxAU = minRangeAU + starTemperature / habitationConstant * starUnitySize * coldModifierEndAU; //5500f / 5500f * 1.0f * 50f;
        if (coldRange.rangeMaxAU - coldRange.rangeMinAU < minDistCold)
        {
            coldRange.rangeMaxAU = coldRange.rangeMinAU + minDistCold;
        }
        //asteroid range between hab and cold, so far 1/8 of hab to maybe close 1/12 of cold, remember cold range is huge
        asteroidRange.rangeMinAU = habRange.rangeMaxAU - Random.Range(0, (habRange.rangeMaxAU - habRange.rangeMinAU) / 8f);
        asteroidRange.rangeMaxAU = coldRange.rangeMinAU + Random.Range(0, (coldRange.rangeMaxAU - coldRange.rangeMaxAU) / 12f);
        bool hasAsteroidBelt = true; //set randomly

        //Debug.Log("hot range is " + hotRange.rangeMinAU + " to " + hotRange.rangeMaxAU);
        //Debug.Log("hab range is " + habRange.rangeMinAU + " to " + habRange.rangeMaxAU);
        //Debug.Log("cold range is " + coldRange.rangeMinAU + " to " + coldRange.rangeMaxAU);


        //generate star system info for celestial object
        float systemBaseAngle = Random.Range(0, 360f);
        starSystemInfo.SetCelestialBodyInfo(star, size, type,  starUnitySize, starDiameter, starDensity, starTemperature, hasAsteroidBelt, systemBaseAngle);
        starSystemInfo.SetRegionRangeValues(hotRange, habRange, coldRange, asteroidRange);
        starSystemInfo.SetDistanceScale(starDistanceMod, AUtoSystemScale); //huge stars need stuff closer while tiny stars need to push stuff further out
        starSystemInfo.posX = xC;
        starSystemInfo.posY = yC;

        //roll planets count depending on system size.
        int numPlanets = 0;
        int maxValPlanets = maxPlanetsSystem[(int)star];
        int maxSetPlanets = 0;
        int minSetPlanets = 0;
        switch (size) //clamping some values for certain system star sizes
        {
            case SolarSystemSize.Small:
                if (smallSystem.planetsMax <= maxValPlanets)
                {
                    minSetPlanets = 1;
                    maxSetPlanets = maxValPlanets;
                }
                else
                {
                    minSetPlanets = smallSystem.planetsMin;
                    maxSetPlanets = smallSystem.planetsMax;
                }
                
                break;
            case SolarSystemSize.Medium:
                if (mediumSystem.planetsMax <= maxValPlanets)
                {
                    minSetPlanets = maxValPlanets / 2;
                    maxSetPlanets = maxValPlanets;
                }
                else
                {
                    minSetPlanets = mediumSystem.planetsMin;
                    maxSetPlanets = mediumSystem.planetsMax;
                }
                break;
            case SolarSystemSize.Large:
                if (smallSystem.planetsMax <= maxValPlanets)
                {
                    minSetPlanets = maxValPlanets / 2;
                    maxSetPlanets = maxValPlanets;
                }
                else
                {
                    minSetPlanets = largeSystem.planetsMin;
                    maxSetPlanets = largeSystem.planetsMax;
                }
                break;
            case SolarSystemSize.XLarge:
                if (smallSystem.planetsMax <= maxValPlanets)
                {
                    minSetPlanets = maxValPlanets / 2;
                    maxSetPlanets = maxValPlanets;
                }
                else
                {
                    minSetPlanets = extraLargeSystem.planetsMin;
                    maxSetPlanets = extraLargeSystem.planetsMax;
                }
                break;
        }
        numPlanets = Random.Range(minSetPlanets, maxSetPlanets);
        starSystemInfo.AUtoSystemScale = AUtoSystemScale;
        //Debug.Log("number of planets is " + numPlanets);
        List<OrbitableInfo> orbitalsGenerated = new List<OrbitableInfo>();
        for (int i = 0; i < numPlanets; i++)
        {
            //Debug.Log("generating planet " + i);
            OrbitableInfo genPlanet = GeneratePlanet(ref orbitalsGenerated, starSystemInfo.systemDistanceScaleMod, hotRange, habRange, coldRange, asteroidRange, true, starSystemInfo.generatedSystemSeed, i, starSystemInfo.baseSystemAngle);
            if (genPlanet != null)
            {
                genPlanet.orbitalName = genPlanet.orbitalType.ToString() + " " + i + " " + genPlanet.orbitalRange.ToString() + " "; //name later based on distances from central object and star system name
                starSystemInfo.AddOrbitalObject(genPlanet);
            }
            else
            {
                //Debug.Log("<b><color=red>NO PLANET GENNED</color></b>");
            }
            
            if ( i > 75) //hard limit for now
            {
                break;
            }
        }

        if (ALSOGENOBJECTS)
        {
            GenerateSystemObjects(starSystemInfo);
        }

    }

    bool CheckPointWithinRegion(float AUpoint, Star_Region_Range testRegion)
    {
        if (AUpoint >= testRegion.rangeMinAU && AUpoint <= testRegion.rangeMaxAU)
        {
            return true;
        }
        return false;
    }

    float GetExclusionSpawnAU(ref List<OrbitableInfo> preGenned, float systemScale, Star_Region_Range hotRange, Star_Region_Range habRange, Star_Region_Range coldRange, Star_Region_Range asteroidRange, bool hasAsteroidBelt, int baseSeed, int planetNum)
    {
        float AUSpawn = 0.0f;

        return AUSpawn;
    }


    OrbitableInfo GeneratePlanet(ref List<OrbitableInfo> preGenned, float systemScale, Star_Region_Range hotRange, Star_Region_Range habRange, Star_Region_Range coldRange, Star_Region_Range asteroidRange, bool hasAsteroidBelt, int baseSeed, int planetNum, float baseAngle) //generate planet information: type of planet, size of planet, planet coloration (which sprite to use), gravity, etc. 
    {
        //determine orbital object type
        float modifiedAsteroidChance = baseAsteroidChance;
        if (!hasAsteroidBelt)
        {
            modifiedAsteroidChance = 0.0f; //no asteroid belt so any asteroids are simply rogue objects
        }
        float modifiedGasGiantChance = baseGasGiantChance;

        float planetChance = 100f - modifiedAsteroidChance - modifiedGasGiantChance;
        if (planetChance <= 0.0f)
        {
            planetChance = 0.0f;
        }
        //seed from base seed and planet number
        Random.InitState(baseSeed + 37 * planetNum + 100003);

        float typeRoll = Random.Range(0.0f, planetChance + modifiedGasGiantChance + modifiedAsteroidChance);
        ORBITAL_TYPES orbitalType = ORBITAL_TYPES.ORBITAL_PLANET;
        if (typeRoll >planetChance && typeRoll <= (planetChance + modifiedGasGiantChance))
        {
            orbitalType = ORBITAL_TYPES.ORBITAL_GAS_GIANT;
        }
        if (typeRoll > (planetChance + modifiedGasGiantChance))
        {
            orbitalType = ORBITAL_TYPES.ORBITAL_ASTEROID;
        }
        
        //place chances depending on type. TODO: editable baselines.
        float chanceHot = 15f;
        float chanceHab = 30f;
        float chanceCold = 55f;
        float chanceAsteroid = 0.0f;

        if (orbitalType == ORBITAL_TYPES.ORBITAL_GAS_GIANT)
        {
            chanceHot *= .8f;
            chanceHab *= 1.1f;
            chanceCold *= 1.1f;
        }
        if (orbitalType == ORBITAL_TYPES.ORBITAL_ASTEROID)
        {
            chanceHot = .2f;
            chanceHab = .2f;
            chanceCold = .2f;
            chanceAsteroid = 99.4f;
        }

        float genRange = Random.Range(0.0f, chanceHot + chanceHab + chanceCold + chanceAsteroid);
        SYSTEM_REGION_RANGES rangePlacement = SYSTEM_REGION_RANGES.REGION_HOT;
        if (genRange > chanceHot && genRange <= (chanceHot + chanceHab))
        {
            rangePlacement = SYSTEM_REGION_RANGES.REGION_HAB;
        }
        if (genRange > (chanceHot + chanceHab) && genRange <= (chanceHot+chanceHab+chanceCold) )
        {
            rangePlacement = SYSTEM_REGION_RANGES.REGION_COLD;
        }
        if (genRange > (chanceHot + chanceHab + chanceCold) && genRange <= (chanceHot + chanceHab + chanceCold + chanceAsteroid))
        {
            rangePlacement = SYSTEM_REGION_RANGES.REGION_ASTEROID;
        }

        if (hasAsteroidBelt)
        {  //dont spawn these inside the asteroid belt
            //Debug.Log("hab starts at " + habRange.rangeMinAU + " to " + habRange.rangeMaxAU);
            //Debug.Log("cold range starts at " + coldRange.rangeMinAU + " to " + coldRange.rangeMaxAU);
            habRange.rangeMaxAU = asteroidRange.rangeMinAU;
            coldRange.rangeMinAU = asteroidRange.rangeMaxAU;
            //Debug.Log("hab range is now " + habRange.rangeMinAU + " to " + habRange.rangeMaxAU);
            //Debug.Log("cold range is now " + coldRange.rangeMinAU + " to " + coldRange.rangeMaxAU);
        }
        float AUDistance = 0.0f;
        //set the target range
        Star_Region_Range targetRange = hotRange;
        if (rangePlacement == SYSTEM_REGION_RANGES.REGION_HAB)
        {
            targetRange = habRange;
            //Debug.Log("target range is in hab range");
        }
        else if (rangePlacement == SYSTEM_REGION_RANGES.REGION_COLD)
        {
            targetRange = coldRange;
            //Debug.Log("target range is in cold range");
        }
        else if (rangePlacement == SYSTEM_REGION_RANGES.REGION_ASTEROID)
        {
            targetRange = asteroidRange;
            //Debug.Log("target range is in asteroid range");
        }
        else
        {
            //Debug.Log("target range is in hot range");
        }
        if (orbitalType != ORBITAL_TYPES.ORBITAL_ASTEROID)
        {
            //create exclusion zones from previously generated planets from list
            //order list first
            OrbitableInfo[] orderedList = preGenned.OrderBy(x => x.distanceFromCenterAU).ToArray(); //since we dont know the distance order at any given time we need to get it every time


            List<Star_Region_Range> exclusionRanges = new List<Star_Region_Range>();
            for (int i = 0; i < orderedList.Length; i++)
            {
                Star_Region_Range excludedRange = new Star_Region_Range(0, 0);
                //exclude range at planet AU size. add more if it has moons and/or is a gas giant.
                float excludeAUposition = orderedList[i].distanceFromCenterAU;
                float addMod = 0.0f;
                if (orderedList[i].orbitalType == ORBITAL_TYPES.ORBITAL_GAS_GIANT)
                {
                    addMod += minimumAUPlanetDistance / .2f;
                }
                if (preGenned[i].numMoons > 0)
                {
                    addMod += minimumAUPlanetDistance / .1f;
                }

                excludedRange.rangeMinAU = excludeAUposition - ((addMod + minimumAUPlanetDistance) / systemScale);
                excludedRange.rangeMaxAU = excludeAUposition + ((addMod + minimumAUPlanetDistance) / systemScale);

                exclusionRanges.Add(excludedRange);
            }
            
            //trim out exclusion ranges that dont matter
            List<Star_Region_Range> removalExcess = new List<Star_Region_Range>();
            for (int i = 0; i < exclusionRanges.Count; i++)
            {
                if ((targetRange.rangeMinAU > exclusionRanges[i].rangeMaxAU) ||
                    (targetRange.rangeMaxAU < exclusionRanges[i].rangeMinAU))
                {
                    removalExcess.Add(exclusionRanges[i]);
                }
            }
            for (int i = 0; i < removalExcess.Count; i++)
            {
                exclusionRanges.Remove(removalExcess[i]);
            }
            removalExcess.Clear();


            //exclusions ranges are in order, now to create trim values.
            List<Star_Region_Range> fullRanges = new List<Star_Region_Range>();
            List<Star_Region_Range> removalRanges = new List<Star_Region_Range>();
            List<Star_Region_Range> addRanges = new List<Star_Region_Range>();
            Star_Region_Range fullRangeBase = new Star_Region_Range(hotRange.rangeMinAU, coldRange.rangeMaxAU);
            fullRanges.Add(fullRangeBase);
            //Debug.Log("base range is " + fullRangeBase.rangeMinAU + " to " + fullRangeBase.rangeMaxAU);
            //Debug.Log("target range is " + targetRange.rangeMinAU + " to " + targetRange.rangeMaxAU);

            //exclusion 5 cases:
            //not near, ignore
            //left side in, trim
            //right side in, trim
            //fully within exclusion zone: remove this range
            //enveloping exclusion zone completely: split chunk

            for (int i = 0; i < exclusionRanges.Count; i++) //for each exclusion range we need to carve out a chunk of the acceptable spawning range
            {
                //Debug.Log("doing exclusion range " + i + " which is excluding " + exclusionRanges[i].rangeMinAU + " to " + exclusionRanges[i].rangeMaxAU);
                for (int j = 0; j < fullRanges.Count; j++)
                {
                    //Debug.Log("checking range " + j + " which has " + fullRanges[j].rangeMinAU + " to " + fullRanges[j].rangeMaxAU);
                    //first, the no-ops
                    if ((exclusionRanges[i].rangeMaxAU < fullRanges[j].rangeMinAU) ||
                        (exclusionRanges[i].rangeMinAU > fullRanges[j].rangeMaxAU))
                    {
                        //Debug.Log("not within range at all");
                        continue;
                    }
                    else if (exclusionRanges[i].rangeMinAU < fullRanges[j].rangeMinAU && exclusionRanges[i].rangeMaxAU > fullRanges[j].rangeMaxAU) //the rare case: completely envelops, A < C && B > D
                    {
                        //Debug.Log("rare case");
                        //remove this segment from further consideration. cant actually remove it inside the op though
                        fullRanges[j].rangeMinAU = 0.0f; fullRanges[j].rangeMaxAU = 0.0f;
                        removalRanges.Add(fullRanges[j]);

                    }
                    else if (exclusionRanges[i].rangeMinAU > fullRanges[j].rangeMinAU && exclusionRanges[i].rangeMaxAU > fullRanges[j].rangeMaxAU) //carve out a chunk instead of splitting: A > C && B > D
                    {
                        float minRange = fullRanges[j].rangeMinAU;
                        float maxRange = exclusionRanges[i].rangeMinAU;
                        fullRanges[j].rangeMinAU = minRange; fullRanges[j].rangeMaxAU = maxRange;
                    }
                    else if (exclusionRanges[i].rangeMinAU < fullRanges[j].rangeMinAU && exclusionRanges[i].rangeMaxAU < fullRanges[j].rangeMaxAU) //carve out a chunk instead of splitting: A < C && B < D
                    {
                        float minRange = exclusionRanges[i].rangeMaxAU;
                        float maxRange = fullRanges[j].rangeMaxAU;
                        fullRanges[j].rangeMinAU = minRange; fullRanges[j].rangeMaxAU = maxRange;
                    }
                    else //need to split a new range out between these values. this should only ever be fully inside a single range, A > C && B < D
                    {

                        float splitMinRange = exclusionRanges[i].rangeMaxAU;
                        float splitMaxRange = fullRanges[j].rangeMaxAU;
                        float newMin = fullRanges[j].rangeMinAU;
                        float newMax = exclusionRanges[i].rangeMinAU;
                        //Debug.Log("split new chunk, " + splitMinRange + " to " + splitMaxRange + " and " + newMin + " to " + newMax);
                        fullRanges[j].rangeMinAU = newMin; fullRanges[j].rangeMaxAU = newMax;
                        addRanges.Add(new Star_Region_Range(splitMinRange, splitMaxRange));

                    }
                }
                //housekeeping
                if (addRanges.Count > 0)
                {
                    for (int ar = 0; ar < addRanges.Count; ar++)
                    {
                        fullRanges.Add(addRanges[ar]);
                    }
                }
                if (removalRanges.Count > 0)
                {
                    for (int r = 0; r < removalRanges.Count; r++)
                    {
                        fullRanges.Remove(removalRanges[r]);
                    }
                }
                addRanges.Clear();
                removalRanges.Clear();
            }
            //then, we need to squish for our random stuff. remember, they are in order. we need to determine max and min within (and if its even possible to gen in that space), then squish values for one continous random, then balloon back out to proper range.
            float minSpawnable = targetRange.rangeMinAU;
            float maxSpawnable = targetRange.rangeMaxAU;
            bool foundMin = false;
            int minIndex = 0;
            bool foundMax = false;
            //Debug.Log("original spawn range is " + targetRange.rangeMinAU + " to " + targetRange.rangeMaxAU);
            //for (int i = 0; i < fullRanges.Count; i++)
            //{
            //    Debug.Log("range possible at " + i + " is " + fullRanges[i].rangeMinAU + " to " + fullRanges[i].rangeMaxAU);
            //}
            for (int i = 0; i < fullRanges.Count; i++)
            {
                if (!foundMin)
                {
                    //Debug.Log("testing min" + minSpawnable  + " within " + fullRanges[i].rangeMinAU + " to " + fullRanges[i].rangeMaxAU);
                    if (CheckPointWithinRegion(minSpawnable, fullRanges[i]))
                    {
                        //Debug.Log("within min at " + i + " which has min of " + fullRanges[i].rangeMinAU);
                        foundMin = true; //acceptable range, dont need to modify
                        minIndex = i;
                    }
                    else
                    {
                        //Debug.Log("not found!");
                        if (minSpawnable < fullRanges[i].rangeMinAU && fullRanges[i].rangeMinAU < targetRange.rangeMaxAU)
                        {
                            
                            foundMin = true;
                            minSpawnable = fullRanges[i].rangeMinAU;
                            //Debug.Log("setting min here! min is now " + minSpawnable);
                            minIndex = i;
                        }
                    }
                }
                if (!foundMax)
                {
                    if (CheckPointWithinRegion(targetRange.rangeMaxAU, fullRanges[i]))
                    {
                        //Debug.Log("max trigger 1");
                        foundMax = true; //also in acceptable range
                        maxSpawnable = targetRange.rangeMaxAU; // set it back to normal
                        fullRanges[i].rangeMaxAU = maxSpawnable;
                    }
                    else //move up till we cant anymore, if we never set it back then by default we are done.
                    {
                        //Debug.Log("max trigger 2: point of " + targetRange.rangeMaxAU + " not within " + fullRanges[i].rangeMinAU + " to " + fullRanges[i].rangeMaxAU);
                        if (foundMin)
                        {
                            if (targetRange.rangeMaxAU > fullRanges[i].rangeMinAU)
                            {
                                maxSpawnable = fullRanges[i].rangeMaxAU;
                                //Debug.Log("max trigger 2.5: target set to " + fullRanges[i].rangeMaxAU);
                                foundMax = true;
                            }

                        }
                    }
                }
            }
            //Debug.Log("now over here");
            if (!foundMin) //no acceptable spawn location in this range!
            {
                //Debug.Log("no min found!");
                return null; //for now, generate no planet. could potentially generate it in another zone instead.
            }
            if (!foundMax)
            {
                //Debug.Log("no max found!");
                return null;
            }
            //Debug.Log("max is set to " + maxSpawnable);
            //pack together potential spawning regions
            List<Star_Region_Range> spawnableRanges = new List<Star_Region_Range>();
            spawnableRanges.Add(new Star_Region_Range(minSpawnable, fullRanges[minIndex].rangeMaxAU));
            for (int i = minIndex + 1; i < fullRanges.Count; i++)
            {
                if (CheckPointWithinRegion(maxSpawnable, fullRanges[i])) //done adding ranges, we found the end point
                {
                    spawnableRanges.Add(new Star_Region_Range(fullRanges[i].rangeMinAU, maxSpawnable));
                    break;
                }

            }

            if (spawnableRanges.Count < 0)
            {
                //Debug.Log("somehow, no spawnable ranges found!"); //just in case
                return null;
            }
            float baseRange = maxSpawnable - minSpawnable;
            //Debug.Log("base range is " + baseRange);
            float packedValue = 0.0f;
            for (int i = 0; i < spawnableRanges.Count; i++)
            {
                //Debug.Log("packed value from " + packedValue + " and the range is at " + spawnableRanges[i].rangeMinAU + " to " + spawnableRanges[i].rangeMaxAU);
                packedValue += (spawnableRanges[i].rangeMaxAU - spawnableRanges[i].rangeMinAU);
            }

            //Debug.Log("packed value is " + packedValue);
            //determine actual rolled distance within packed value
            AUDistance = Random.Range(0.0f, packedValue);
            //blow back up result
            for (int i = 0; i < spawnableRanges.Count; i++)
            {
                if (AUDistance < spawnableRanges[i].rangeMaxAU - spawnableRanges[i].rangeMinAU) //within this region
                {
                    AUDistance = spawnableRanges[i].rangeMinAU + AUDistance;
                    break;
                }
                else //subtract portion and continue
                {
                    AUDistance -= (spawnableRanges[i].rangeMaxAU - spawnableRanges[i].rangeMinAU);
                }
            }
        }
        else //place asteroid without caring about exclusion ranges
        {
            AUDistance = Random.Range(targetRange.rangeMinAU, targetRange.rangeMaxAU);
        }
        

        

        





        //TODO: determine if inside anothers orbit. Alternatively: make blocking ranges and piecemeal them together before generating range.
        //Debug.Log("set AU distance to " + AUDistance);

        //Debug.Log("distance scale mod is " + systemScale);
        float unityDistance = AUDistance * AUtoSystemScale * systemScale;
        //Debug.Log("unity distance is " + unityDistance);
        float modifiedSizeScale = basePlanetScale + Random.Range((basePlanetScale / 8f) * -1f, (basePlanetScale / 8f)); //TODO: determine acceptable size mods in editable format
        if (orbitalType == ORBITAL_TYPES.ORBITAL_GAS_GIANT)
        {
            modifiedSizeScale = baseGasGiantScale + Random.Range((baseGasGiantScale / 4f) * -1f, (baseGasGiantScale / 3f));
        }



        //bool canGenerate = false;
        //determine if can place a planet here by checking the gen range and all planets already genned in that region.
        //if (!canGenerate) //no space left in that orbital region, by minimumAUDistanceOfplanets there
        //    return null;
        OrbitableInfo planetInfo = new OrbitableInfo();
        planetInfo.orbitalID = planetNum;
        planetInfo.orbitTarget = -1; //central object
        planetInfo.orbitalType = orbitalType;
        planetInfo.orbitalRange = rangePlacement;
        planetInfo.distanceFromCenterAU = AUDistance;
        planetInfo.distanceFromCenterUnityScale = unityDistance;
        planetInfo.planetUnityScale = modifiedSizeScale;
        //Debug.Log("final AU distance is " + AUDistance);
        //Debug.Log("final unity distance is " + planetInfo.distanceFromCenterUnityScale);
        float baseSizeMod = .04f;
        float baseAngleMod = 1.0f;
        if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_HAB)
        {
            baseAngleMod = 1.5f;
            baseSizeMod = .05f;
        }
        if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_ASTEROID)
        {
            baseAngleMod = .05f;
            baseSizeMod = .02f;
        }
        if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_COLD)
        {
            baseAngleMod = 3.0f;
            baseSizeMod = .07f;
        }
        //Debug.Log("base size mod is " + baseSizeMod);
        //Debug.Log("sizescale is " + planetInfo.distanceFromCenterUnityScale);
        float sizeX = planetInfo.distanceFromCenterUnityScale;
        float randMod = Random.Range(.85f, .85f + baseSizeMod);
        //Debug.Log("rand mod is " + randMod);
        float sizeY = randMod * planetInfo.distanceFromCenterUnityScale;
        planetInfo.orbitSizeX = sizeX;
        planetInfo.orbitSizeY = sizeY;
        //Debug.Log("actual orbit size is " + planetInfo.orbitSize);
        var v2Offset = new Vector2(Random.Range(AUtoSystemScale / 80f * -1f, AUtoSystemScale / 80f), Random.Range((AUtoSystemScale / 80f) * -1f, AUtoSystemScale / 80f));
        planetInfo.orbitOffsetX = v2Offset.x;
        planetInfo.orbitOffsetY = v2Offset.y;
        
        
        planetInfo.orbitAngle = baseAngle + Random.Range(-baseAngleMod, baseAngleMod); //along system angle for less collisions on elliptical orbits
        planetInfo.orbitStart = Random.Range(0, 50);
        if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_COLD)
        {
            if (Random.Range(0, 1.0f) > .92f)
            {
                planetInfo.orbitAngle += Random.Range(10, 15f); //random awkward orbit possible in cold regions
            }
        }


        //public float orbitalVelocityModifier = .0002f; //66615 * .0002 = ~20f, etc.  Modify mph to get acceptable speed.
        //public float diameterBaseOfDefaultMiles = 865000; //base diameter of sun
        //public float baseSpeedDefaultEarthMPH = 66615; //base speed at 1 AU, cheat value for determinism of speed.
        //relative velocities to earth
        //Velocity = 1/R^(1/2) (1 divided by sqrt of AU)
        //earth at 92.96 million miles, 1AU
        //1 / 1  = 1
        //conversion to mph = * baseSpeedDefaultEarthMPH

        //Debug.Log("distance from center in AU is " + planetInfo.distanceFromCenterAU);
        //Debug.Log("base default earth speed is " + baseSpeedDefaultEarthMPH);
        //Debug.Log("orbital velocity modifier is " + orbitalVelocityModifier);
        float modSpeed = baseSpeedDefaultEarthMPH * orbitalVelocityModifier;
        //Debug.Log("modspeed is " + modSpeed);
        //Debug.Log("modspeed is " + modSpeed);
        float orbitPeriod = 1.0f / Mathf.Sqrt(planetInfo.distanceFromCenterAU) * modSpeed;
        //Debug.Log("orbitperiod is " + orbitPeriod);
        //Debug.Log("orbit period is " + orbitPeriod);
        planetInfo.orbitSpeed = orbitPeriod; //note: this is actually deterministic depending on star / central object mass/gravity and distance away, calculations above
        planetInfo.rotatesClockwise = (Random.Range(0.0f, 1.0f) < .5f? true: false);

        

        

        //TODO: more here for choosing planet types
        //determine look based on random selection of sprites. effectively choosing planet type: may need a lot more work here.
        planetInfo.spritePlanetResourceID = 0;
        if (planetInfo.orbitalType == ORBITAL_TYPES.ORBITAL_PLANET)
        {
            if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_HOT)
            {
                planetInfo.spritePlanetResourceID = Random.Range(0, spriteResourceHotEnd);
            }
            else if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_HAB)
            {
                planetInfo.spritePlanetResourceID = Random.Range(spriteResourceHotEnd, spriteResourceHabEnd);
            }
            else
            {
                planetInfo.spritePlanetResourceID = Random.Range(spriteResourceHabEnd, planetSpriteResources.Length);
            }
        }

        GeneratePlanetsMoons(ref planetInfo, baseSeed);

        preGenned.Add(planetInfo);
        return planetInfo;
    }

    void GeneratePlanetsMoons(ref OrbitableInfo planetInfo, int baseSeed)
    {
        
        if (planetInfo.orbitalType == ORBITAL_TYPES.ORBITAL_ASTEROID || planetInfo.orbitalType == ORBITAL_TYPES.ORTIBAL_MOON)
        {
            return; //no moons here?
        }
        Random.InitState((baseSeed + planetInfo.orbitalID) * 87 + 3721);
        float moonChance = 0.0f;
        float chanceOfAdditionalMoons = 0.0f;
        float chanceMoonIsAPlanet = 0.0f;
        int maxMoons = 0;
        if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_HOT)
        {
            moonChance = hotRangeMoonChance;
            chanceOfAdditionalMoons = hotRangeChanceOfAnotherMoon;
            chanceMoonIsAPlanet = hotRangeMoonIsAnotherPlanetChance;
            maxMoons = hotRangeMoonMax;
        }
        if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_HAB)
        {
            moonChance = habRangeMoonChance;
            chanceOfAdditionalMoons = habChanceOfAdditionalMoons;
            chanceMoonIsAPlanet = habRangeMoonIsAnotherPlanetChance;
            maxMoons = habRangeMoonMax;
        }
        if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_COLD)
        {
            moonChance = coldRangeMoonChance;
            chanceOfAdditionalMoons = coldChanceOfAdditionalMoons;
            chanceMoonIsAPlanet = coldRangeMoonIsAnotherPlanetChance;
            maxMoons = coldRangeMoonMax;
        }
        if (planetInfo.orbitalType == ORBITAL_TYPES.ORBITAL_GAS_GIANT)
        {
            moonChance *= gasChanceMoonModifier;
            maxMoons *= gasChanceMoonMaxModifier;
        }

        int numMoonsToGen = 0;
        float hasMoon = Random.Range(0.0f, 1.0f);
        if (hasMoon < moonChance)
        {
            numMoonsToGen = 1;
            float hasAdditionalMoons = Random.Range(0.0f, chanceOfAdditionalMoons);
            if (hasAdditionalMoons < chanceOfAdditionalMoons)
            {
                numMoonsToGen += Random.Range(1, maxMoons);
            }
        }
        
        planetInfo.numMoons = numMoonsToGen;
        for (int i = 0; i < numMoonsToGen; i++)
        {
            //gen moon into orbital
            //each moon is similar to a planet but it orbits the planet instead, so all references to its orbitable target is the planet proper.
            OrbitableInfoMoon newMoon = new OrbitableInfoMoon();

            newMoon.orbitalID = i;
            newMoon.orbitalName = "Moon " + i;
            newMoon.orbitalOrder = i; //should set this by distance as well
            newMoon.orbitTarget = planetInfo.orbitalID;
            newMoon.orbitalType = ORBITAL_TYPES.ORTIBAL_MOON;
            newMoon.orbitalRange = planetInfo.orbitalRange;

            //determine if its a planet instead of a moon
            float moonTypeRoll = Random.Range(0.0f, 1.0f);
            if (moonTypeRoll < chanceMoonIsAPlanet)
            {
                newMoon.orbitalType = ORBITAL_TYPES.ORBITAL_PLANET;
            }
            //sprite id by type
            if (newMoon.orbitalType == ORBITAL_TYPES.ORTIBAL_MOON)
            {
                newMoon.spritePlanetResourceID = Random.Range(0, moonSpriteResources.Length);
            }
            else if (newMoon.orbitalType == ORBITAL_TYPES.ORBITAL_PLANET)
            {
                newMoon.spritePlanetResourceID = Random.Range(0, planetSpriteResources.Length);
            }

            //moon distance gen: moon from earth is about 0.00257 AU, at 238,856 miles.  Callisto from jupiter is 1,170,042 miles at ~.0126 au, Pasiphae is 14,602,223 at .157 AU. lets set range from .00225 AU to .175 AU
            //will probably need to set some factors, like if its around a gas giant, or if its another planet, etc.
            //need to change actual values due to unity scaling issues
            float distMinUnityScaleMoon = 4f; // at least 4 unity units away from a normal planet
            //float distMinUnityScaleGG = 7f; //at least 7 unity units away from a gas giant

            float moonMinRangeAU = distMinUnityScaleMoon / AUtoSystemScale;
            float moonMaxRangeAU = moonMinRangeAU * 2.0f;
            if (planetInfo.orbitalType == ORBITAL_TYPES.ORBITAL_GAS_GIANT)
            {
                moonMinRangeAU = moonMinRangeAU * 1.45f; //need to offset because gas giants are bigger
                moonMaxRangeAU = moonMaxRangeAU * 1.15f;
            }
            if (newMoon.orbitalType == ORBITAL_TYPES.ORBITAL_PLANET)
            {
                moonMinRangeAU *= 1.35f;
                moonMaxRangeAU *= 1.15f;
            }
            float moonDistRangeAU = Random.Range(moonMinRangeAU, moonMaxRangeAU);
            float moonDistUnityScale = moonDistRangeAU * AUtoSystemScale;

            newMoon.distanceFromCenterAU = moonDistRangeAU;
            newMoon.distanceFromCenterUnityScale = moonDistUnityScale;
            float moonUnityScale = .05f;
            moonUnityScale += Random.Range(-.01f, .02f);
            newMoon.planetUnityScale = moonUnityScale;
            var v2Orbsize = new Vector2(newMoon.distanceFromCenterUnityScale, newMoon.distanceFromCenterUnityScale + Random.Range(-newMoon.distanceFromCenterUnityScale / 10f, newMoon.distanceFromCenterUnityScale / 10f));
            newMoon.orbitSizeX = v2Orbsize.x;
            newMoon.orbitSizeY = v2Orbsize.y;
            var v2OrbOffset = new Vector2(Random.Range((AUtoSystemScale / 200f) * -1f, AUtoSystemScale / 200f), Random.Range((AUtoSystemScale / 200f) * -1f, AUtoSystemScale / 200f)); //may need tweaking for proper offset ranges.
            newMoon.orbitOffsetX = v2OrbOffset.x;
            newMoon.orbitOffsetY = v2OrbOffset.y;

            newMoon.orbitAngle = Random.Range(0, 360f);
            //velocity of Satellite motion: equal to SQRT of (G * Mcentral)/R^2   where G = 6.673 * 10^-11 N * m^2/kg^2
            //therefore:  determine G * Mcentral aka determine value by size of planet, or calculate planet mass from a density calculation, then divide by R at distanceAU, then take the square root of that    
            float orbitCalcValMod = .15f;
            float orbitMassCalc = planetInfo.planetUnityScale * orbitCalcValMod;
            orbitMassCalc /= newMoon.distanceFromCenterAU;
            float orbitPeriod = Mathf.Sqrt(orbitMassCalc);
            
            if (orbitPeriod < minOrbitalSpeed)
            {
                orbitPeriod = minOrbitalSpeed; 
            }
            if (orbitPeriod > maxOrbitalSpeed)
            {
                orbitPeriod = maxOrbitalSpeed;
            }
            newMoon.orbitSpeed = orbitPeriod; //may need to tweak moon speed differently
            newMoon.orbitStart = Random.Range(0, 50);
            newMoon.rotatesClockwise = Random.Range(0, 1.0f) < .5f ? true : false;
            //newMoon.numMoons = 0;
            //newMoon.moons = null;

            planetInfo.moons.Add(newMoon);
        }


    }

    void GenerateSystemObjects(StarSystemInfo infoBase)
    {
        Debug.Log("Generating System Objects");
        //generate system gameobject, add any system handler scripts to this
        if (!systemContainer)
        {
            Debug.Log("making new container");
            systemContainer = new GameObject();
            //    DestroyImmediate(systemContainer);
            //    systemContainer = null;
        }
        //systemContainer = new GameObject();
        systemContainer.transform.parent = this.transform;
        systemContainer.name = "System Container " + infoBase.posX + " : " + infoBase.posY + " : " + infoBase.systemName;

        GameObject centralObject = GenerateCentralObject(infoBase);
        centralObject.transform.parent = systemContainer.transform;
        for (int i = 0; i < infoBase.orbitals.Count; i++)
        {
            //Debug.Log("generating orbitals count for " + (i+1) + " of " + infoBase.orbitals.Count );
            GameObject newPlanet = GeneratePlanetGameobject(infoBase.orbitals[i], infoBase, systemContainer.gameObject);
            for (int m = 0; m < infoBase.orbitals[i].numMoons; m++)
            {
                //Debug.Log("Genning moon " + (m+1));
                GameObject newMoon = GenerateMoonGameobject(infoBase.orbitals[i].moons[m], newPlanet, infoBase);
            }
        }

        if (infoBase.linkedSystemIDs != null)
        {
            int[] linkIDs = new int[infoBase.linkedSystemIDs.Length];
            Vector2[] linkPositions = new Vector2[infoBase.linkedSystemPositionsX.Length];
            for (int i = 0; i < infoBase.linkedSystemIDs.Length; i++)
            {
                linkIDs[i] = infoBase.linkedSystemIDs[i];
                linkPositions[i] = new Vector2(infoBase.linkedSystemPositionsX[i], infoBase.linkedSystemPositionsY[i]);
            }
            GenerateExitObjects(infoBase, new Vector2(infoBase.posX, infoBase.posY), linkPositions, infoBase.systemID, linkIDs, infoBase.systemName, infoBase.linkedSystemNames);
        }
        

    }

    

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
    void GenerateExitObjects(StarSystemInfo infoBase, Vector2 thisStarPosition, Vector2[] linkedStarPositions, int thisStarID, int[] linkedStarIDs, string thisStarName, string[] linkedStarNames)
    {
        Debug.Log("Size scale is " + infoBase.AUtoSystemScale + " and " + infoBase.systemDistanceScaleMod);
        //Debug.Log("and far planet is " + infoBase.AUFarPlanet);
        float sysSizePlacement = (infoBase.AUFarPlanet + .25f) * infoBase.AUtoSystemScale * infoBase.systemDistanceScaleMod;
        for (int i = 0; i < linkedStarPositions.Length && i < infoBase.numberActualLinks; i++)
        {
            GameObject newExit = Instantiate(systemLinkObjectPrefab);
            newExit.transform.parent = this.transform;
            var sExit = newExit.GetComponent<SystemExit>();


            sExit.GenExit(linkedStarPositions[i].x, linkedStarPositions[i].y, infoBase.posX, infoBase.posY, linkedStarNames[i], sysSizePlacement);
            sExit.linkID = linkedStarIDs[i];
        }
    }


    GameObject GenerateCentralObject(StarSystemInfo infoBase)
    {
        //create star/blackole/other central object.
        GameObject centralObject = new GameObject(infoBase.systemName, typeof(CapsuleCollider2D));
        
        GameObject spriteObject = new GameObject("Sprite", typeof(SpriteRenderer));
        spriteObject.transform.SetParent(centralObject.transform, false);
        GameObject systemInfo = new GameObject("System Info", typeof(CanvasRenderer), typeof(Text), typeof(SystemInfo));
        systemInfo.transform.SetParent(centralObject.transform, false);
        GameObject minimapIcon = new GameObject("Minimap Icon", typeof(SpriteRenderer));
        minimapIcon.transform.SetParent(centralObject.transform, false);
        GameObject gravityField = new GameObject("Gravity Field", typeof(CircleCollider2D), typeof(PlanetGravity));
        gravityField.transform.SetParent(centralObject.transform, false);

        //set values
        var sprite = spriteObject.GetComponent<SpriteRenderer>();
        sprite.sprite = starSpriteResources[(int)infoBase.mainBodyType];
        var gravityFieldScript = gravityField.GetComponent<PlanetGravity>();
        gravityFieldScript.pullForce = 1; //TODO determine some values based on mass and size usable for everything
        gravityFieldScript.pullRadius = 1;
        var systemInfoScript = systemInfo.GetComponent<SystemInfo>();
        systemInfoScript.starName = systemInfo.GetComponent<Text>(); //TODO determine other hookups here, or for later.
        systemInfoScript.starName.text = infoBase.systemName;

        centralObject.transform.localScale = new Vector3(infoBase.mainBodyUnitySizeScale, infoBase.mainBodyUnitySizeScale, infoBase.mainBodyUnitySizeScale);

        return centralObject;

    }

    GameObject GeneratePlanetGameobject(OrbitableInfo planetInfo, StarSystemInfo infoBase, GameObject centerObject)
    {
        //Debug.Log("generating system planet object");
        GameObject newPlanet = new GameObject(planetInfo.orbitalName, typeof(CapsuleCollider2D), typeof(PlanetHitDetection), typeof(Orbitable), typeof(Rotate));
        newPlanet.transform.parent = centerObject.transform;
        //var orbitCopy = newPlanet.AddComponent<OrbitableInfo>();
        //orbitCopy.SetCopyFrom(planetInfo);
        GameObject spriteObject = new GameObject("Sprite", typeof(SpriteRenderer));
        spriteObject.transform.SetParent(newPlanet.transform, false);
        GameObject planetHighlight = new GameObject("Planet Highlight", typeof(SpriteRenderer));
        planetHighlight.transform.parent = newPlanet.transform; //TODO set up base sprite for highlighter
        GameObject systemInfo = new GameObject("System Info", typeof(CanvasRenderer), typeof(Text), typeof(SystemInfo));
        systemInfo.transform.SetParent(newPlanet.transform, false);
        GameObject minimapIcon = new GameObject("Minimap Icon", typeof(SpriteRenderer));
        minimapIcon.transform.SetParent(newPlanet.transform, false);
        GameObject planetDarkness = new GameObject("Planet Darkness", typeof(SpriteRenderer), typeof(LookAt));
        planetDarkness.transform.SetParent(newPlanet.transform, false);
        GameObject gravityField = new GameObject("Gravity Field", typeof(CircleCollider2D), typeof(PlanetGravity));
        gravityField.transform.SetParent(newPlanet.transform, false);

        //set values
        var sprite = spriteObject.GetComponent<SpriteRenderer>();
        if (planetInfo.orbitalType == ORBITAL_TYPES.ORBITAL_PLANET)
        {
            sprite.sprite = planetSpriteResources[(int)planetInfo.spritePlanetResourceID];
        }
        if (planetInfo.orbitalType == ORBITAL_TYPES.ORBITAL_GAS_GIANT)
        {
            sprite.sprite = planetGasGiantsResources[(int)planetInfo.spritePlanetResourceID];
        }
        if (planetInfo.orbitalType == ORBITAL_TYPES.ORBITAL_ASTEROID)
        {
            sprite.sprite = asteroidResources[(int)planetInfo.spritePlanetResourceID];
        }
        if (planetInfo.orbitalType == ORBITAL_TYPES.ORTIBAL_MOON)
        {
            sprite.sprite = moonSpriteResources[(int)planetInfo.spritePlanetResourceID];
        }
        var gravityFieldScript = gravityField.GetComponent<PlanetGravity>();
        gravityFieldScript.pullForce = 1; //TODO determine some values based on mass and size usable for everything
        gravityFieldScript.pullRadius = 1;
        var systemInfoScript = systemInfo.GetComponent<SystemInfo>();
        systemInfoScript.starName = systemInfo.GetComponent<Text>(); //TODO determine other hookups here, or for later.
        systemInfoScript.starName.text = infoBase.systemName;

        //set value for highlight and planet darkness sprites here

        var lookatScript = planetDarkness.GetComponent<LookAt>();
        lookatScript.target = centerObject.transform;

        newPlanet.transform.localScale = new Vector3(planetInfo.planetUnityScale, planetInfo.planetUnityScale, planetInfo.planetUnityScale);
        //set distance base values and orbitable script
        var orbitSet = newPlanet.GetComponent<Orbitable>();
        orbitSet.speed = planetInfo.orbitSpeed;
        orbitSet.star = centerObject;
        orbitSet.size = new Vector2(planetInfo.orbitSizeX, planetInfo.orbitSizeY);
        orbitSet.offset = new Vector2(planetInfo.orbitOffsetX, planetInfo.orbitOffsetY);
        orbitSet.angle = planetInfo.orbitAngle;
        orbitSet.clockWise = planetInfo.rotatesClockwise;
        orbitSet.drawGizmo = true;
        orbitSet.startPoint = planetInfo.orbitStart;
        return newPlanet;
    }

    GameObject GenerateMoonGameobject(OrbitableInfoMoon moonInfo, GameObject parentObject, StarSystemInfo infoBase)
    {
        GameObject newMoon = new GameObject(moonInfo.orbitalName, typeof(CapsuleCollider2D), typeof(PlanetHitDetection), typeof(Orbitable), typeof(Rotate));
        newMoon.transform.parent = parentObject.transform;
        //var orbitCopy = newMoon.AddComponent<OrbitableInfo>();
        //orbitCopy.SetCopyFrom(moonInfo);
        GameObject spriteObject = new GameObject("Sprite", typeof(SpriteRenderer));
        spriteObject.transform.SetParent(newMoon.transform, false);
        GameObject planetHighlight = new GameObject("Planet Highlight", typeof(SpriteRenderer));
        planetHighlight.transform.SetParent(newMoon.transform, false); //TODO set up base sprite for highlighter
        GameObject systemInfo = new GameObject("System Info", typeof(CanvasRenderer), typeof(Text), typeof(SystemInfo));
        systemInfo.transform.SetParent(newMoon.transform, false);
        GameObject minimapIcon = new GameObject("Minimap Icon", typeof(SpriteRenderer));
        minimapIcon.transform.SetParent(newMoon.transform, false);
        GameObject planetDarkness = new GameObject("Planet Darkness", typeof(SpriteRenderer), typeof(LookAt));
        planetDarkness.transform.SetParent(newMoon.transform, false);
        GameObject gravityField = new GameObject("Gravity Field", typeof(CircleCollider2D), typeof(PlanetGravity));
        gravityField.transform.parent = newMoon.transform;

        //set values
        var sprite = spriteObject.GetComponent<SpriteRenderer>();
        if (moonInfo.orbitalType == ORBITAL_TYPES.ORBITAL_PLANET)
        {
            sprite.sprite = planetSpriteResources[(int)moonInfo.spritePlanetResourceID];
        }
        if (moonInfo.orbitalType == ORBITAL_TYPES.ORBITAL_GAS_GIANT)
        {
            sprite.sprite = planetGasGiantsResources[(int)moonInfo.spritePlanetResourceID];
        }
        if (moonInfo.orbitalType == ORBITAL_TYPES.ORBITAL_ASTEROID)
        {
            sprite.sprite = asteroidResources[(int)moonInfo.spritePlanetResourceID];
        }
        if (moonInfo.orbitalType == ORBITAL_TYPES.ORTIBAL_MOON)
        {
            sprite.sprite = moonSpriteResources[(int)moonInfo.spritePlanetResourceID];
        }
        var gravityFieldScript = gravityField.GetComponent<PlanetGravity>();
        gravityFieldScript.pullForce = 1; //TODO determine some values based on mass and size usable for everything
        gravityFieldScript.pullRadius = 1;
        var systemInfoScript = systemInfo.GetComponent<SystemInfo>();
        systemInfoScript.starName = systemInfo.GetComponent<Text>(); //TODO determine other hookups here, or for later.
        systemInfoScript.starName.text = infoBase.systemName;

        //set value for highlight and planet darkness sprites here

        var lookatScript = planetDarkness.GetComponent<LookAt>();
        lookatScript.target = parentObject.transform;

        newMoon.transform.localScale = new Vector3(moonInfo.planetUnityScale, moonInfo.planetUnityScale, moonInfo.planetUnityScale);
        //set distance base values and orbitable script
        var orbitSet = newMoon.GetComponent<Orbitable>();
        orbitSet.speed = moonInfo.orbitSpeed;
        orbitSet.star = parentObject;
        orbitSet.size = new Vector2(moonInfo.orbitSizeX, moonInfo.orbitSizeY);
        orbitSet.offset = new Vector2(moonInfo.orbitOffsetX, moonInfo.orbitOffsetY);
        orbitSet.angle = moonInfo.orbitAngle;
        orbitSet.clockWise = moonInfo.rotatesClockwise;
        orbitSet.drawGizmo = true;



        return newMoon;
    }
}
