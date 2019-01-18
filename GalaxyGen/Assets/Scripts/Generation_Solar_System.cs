using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generation_Solar_System : MonoBehaviour
{
    public bool FORCESYSTEMTYPE;
    public STARTEMPERATURES_TYPES FORCETYPE;

    public enum SYSTEM_SIZE {SMALL, MEDIUM, LARGE, XLARGE };
    public enum SYSTEM_TYPE {STAR, BLACKHOLE, EMPTY, ASTEROID};
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
    private int coldRangeMoonMax = 12;
    private float coldRangeMoonIsAnotherPlanetChance = .2f; //less chance than hot but much more potential chances
    private float gasChanceMoonModifier = 3.0f;
    private int gasChanceMoonMaxModifier = 4;
    

    //some reduction in values for smaller system on number of planets due to scale, can be modified or removed.
    //reduced size and temperatures of some things to clamp size within safe float distances
    public float diameterBaseOfDefaultMiles = 865000; //base diameter of sun
    public float baseSpeedDefaultEarthMPH = 66615; //base speed at 1 AU, cheat value for determinism of speed.
    public float orbitalVelocityModifier = .0002f; //66615 * .0002 = ~20f, etc.  Modify mph to get acceptable speed.
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
    float habModifierStartAU = .75f;
    float coldModifierStartAU = 2.0f;
    float coldModifierEndAU = 15f; //shrunk a bit to squish stuff together, original 50

    public float blackHoleChance;
    public float emptyChance;
    public float asteroidChance;
    //default: star chance

    public int xCoord;
    public int yCoord;
    public int universeSeed;
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

    public float AUtoSystemScale = 25f; //convert AU to unity transform units. Modify to get proper size. (original 100 was way too big)
    public float objectSizeToPlayScale = 1.0f; //
    public float basePlanetScale = .08f; //base scale for regular planets
    public float baseGasGiantScale = .23f; //base scale for gas giants
    public float minimumAUPlanetDistance = 8f; //minimum distance planets need to be apart in distance. 

    public struct Star_Region_Range
    {
        public float rangeMinAU;
        public float rangeMaxAU;
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
        
        starSystemInfo = new StarSystemInfo();
        starSystemInfo.generatedSystemSeed = (xC + yC) * rS + ((xC * yC) % rS);
        starSystemInfo.systemName = "Rigel"; //test name

        int randSize = Random.Range(0, 4);
        int randStarType = Random.Range(0, 15);
        SYSTEM_SIZE size = (SYSTEM_SIZE)randSize;
        
        SYSTEM_TYPE type = SYSTEM_TYPE.STAR;
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
        starUnitySize += (Random.Range(-starUnitySize / 10f, starUnitySize / 10f));
        starTemperature += (Random.Range(-starTemperature / 10f, starTemperature / 10f));
        float starDiameter = starUnitySize * diameterBaseOfDefaultMiles; //based on diameter of Sol
        float mandatoryMinAU = .15f; //brown dwarf sanity
        float minRangeAU = starUnitySize + starUnitySize / 3f; //* AUtoSystemScale + starUnitySize / 10f; //one third of star size for corona
        if (minRangeAU < mandatoryMinAU)
        {
            minRangeAU = mandatoryMinAU;
        }

        Star_Region_Range hotRange;
        Star_Region_Range habRange;
        Star_Region_Range coldRange;
        Star_Region_Range asteroidRange;

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
        //asteroid range between hab and cold, so far 1/6 of hab to maybe close 1/12 of cold, remember cold range is huge
        asteroidRange.rangeMinAU = habRange.rangeMaxAU - Random.Range(0, (habRange.rangeMaxAU - habRange.rangeMinAU) / 6f);
        asteroidRange.rangeMaxAU = coldRange.rangeMinAU + Random.Range(0, (coldRange.rangeMaxAU - coldRange.rangeMaxAU) / 12f);
        bool hasAsteroidBelt = true; //set randomly

        //Debug.Log("hot range is " + hotRange.rangeMinAU + " to " + hotRange.rangeMaxAU);
        //Debug.Log("hab range is " + habRange.rangeMinAU + " to " + habRange.rangeMaxAU);
        //Debug.Log("cold range is " + coldRange.rangeMinAU + " to " + coldRange.rangeMaxAU);


        //generate star system info for celestial object
        starSystemInfo.SetCelestialBodyInfo(star, size, type,  starUnitySize, starDiameter, starDensity, starTemperature, hasAsteroidBelt);
        starSystemInfo.SetRegionRangeValues(hotRange, habRange, coldRange, asteroidRange);
        starSystemInfo.SetDistanceScale(starDistanceMod); //huge stars need stuff closer while tiny stars need to push stuff further out

        //roll planets count depending on system size.
        int numPlanets = 0;
        int maxValPlanets = maxPlanetsSystem[(int)star];
        int maxSetPlanets = 0;
        int minSetPlanets = 0;
        switch (size) //clamping some values for certain system star sizes
        {
            case SYSTEM_SIZE.SMALL:
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
            case SYSTEM_SIZE.MEDIUM:
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
            case SYSTEM_SIZE.LARGE:
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
            case SYSTEM_SIZE.XLARGE:
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
        Debug.Log("number of planets is " + numPlanets);
        List<OrbitableInfo> orbitalsGenerated = new List<OrbitableInfo>();
        for (int i = 0; i < numPlanets; i++)
        {
            Debug.Log("generating planet " + i);
            OrbitableInfo genPlanet = GeneratePlanet(ref orbitalsGenerated, starSystemInfo.systemDistanceScaleMod, hotRange, habRange, coldRange, asteroidRange, true, starSystemInfo.generatedSystemSeed, i);
            genPlanet.orbitalName = genPlanet.orbitalType.ToString() + " " + i + " " + genPlanet.orbitalRange.ToString() + " "; //name later based on distances from central object and star system name
            starSystemInfo.AddOrbitalObject(genPlanet);
            if ( i > 75)
            {
                break;
            }
        }

        if (ALSOGENOBJECTS)
        {
            GenerateSystemObjects(starSystemInfo);
        }

    }

    

    OrbitableInfo GeneratePlanet(ref List<OrbitableInfo> preGenned, float systemScale, Star_Region_Range hotRange, Star_Region_Range habRange, Star_Region_Range coldRange, Star_Region_Range asteroidRange, bool hasAsteroidBelt, int baseSeed, int planetNum) //generate planet information: type of planet, size of planet, planet coloration (which sprite to use), gravity, etc. 
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


        List<float> competingRanges = new List<float>(); //list of already existing planet ranges for comparison. idea is to make a set of ranges for any spawnable location, check if it can spawn there at all, move to the next, etc, and cancel out if everything is filled.
        for (int i = 0; i < preGenned.Count; i++)
        {
            competingRanges.Add(preGenned[i].distanceFromCenterAU);
        }
        //determine actual distance from center object.
        float AUDistance = Random.Range(targetRange.rangeMinAU, targetRange.rangeMaxAU);
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

        planetInfo.orbitSize = new Vector2(planetInfo.distanceFromCenterUnityScale, planetInfo.distanceFromCenterUnityScale * Random.Range(.85f, 1.20f));
        planetInfo.orbitOffset = new Vector2(Random.Range(AUtoSystemScale / 80f * -1f, AUtoSystemScale / 80f), Random.Range((AUtoSystemScale / 80f) * -1f, AUtoSystemScale / 80f));
        planetInfo.orbitAngle = Random.Range(0.0f, 360f); //should this be weighted to a set value? see solar system model. maybe just need to randomly have a weird one. possibly same with orbit size.


        //public float orbitalVelocityModifier = .0002f; //66615 * .0002 = ~20f, etc.  Modify mph to get acceptable speed.
        //public float diameterBaseOfDefaultMiles = 865000; //base diameter of sun
        //public float baseSpeedDefaultEarthMPH = 66615; //base speed at 1 AU, cheat value for determinism of speed.
        //public float orbitalVelocityModifier = .0002f; //66615 * .0002 = ~20f, etc.  Modify mph to get acceptable speed.
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
        float orbitPeriod = 1.0f / Mathf.Sqrt(planetInfo.distanceFromCenterAU) * modSpeed;
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
        Debug.Log("planet has " + planetInfo.numMoons);
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
        if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_HOT)
        {
            moonChance = habRangeMoonChance;
            chanceOfAdditionalMoons = habChanceOfAdditionalMoons;
            chanceMoonIsAPlanet = habRangeMoonIsAnotherPlanetChance;
            maxMoons = habRangeMoonMax;
        }
        if (planetInfo.orbitalRange == SYSTEM_REGION_RANGES.REGION_HOT)
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
            OrbitableInfo newMoon = new OrbitableInfo();

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
            float moonMinRangeAU = .0115f;
            float moonMaxRangeAU = .05f;
            if (planetInfo.orbitalType == ORBITAL_TYPES.ORBITAL_GAS_GIANT)
            {
                moonMinRangeAU = .0925f; //need to offset because gas giants are bigger
                moonMaxRangeAU = .155f;
            }
            if (newMoon.orbitalType == ORBITAL_TYPES.ORBITAL_PLANET)
            {
                moonMinRangeAU *= 1.25f;
                moonMaxRangeAU *= 1.25f;
            }
            float moonDistRangeAU = Random.Range(moonMinRangeAU, moonMaxRangeAU);
            float moonDistUnityScale = moonDistRangeAU * AUtoSystemScale;

            newMoon.distanceFromCenterAU = moonDistRangeAU;
            newMoon.distanceFromCenterUnityScale = moonDistUnityScale;
            float moonUnityScale = .05f;
            moonUnityScale += Random.Range(-.01f, .02f);
            newMoon.planetUnityScale = moonUnityScale;
            newMoon.orbitSize = new Vector2(newMoon.distanceFromCenterUnityScale, newMoon.distanceFromCenterUnityScale + Random.Range(-newMoon.distanceFromCenterUnityScale / 10f, newMoon.distanceFromCenterUnityScale / 10f));
            newMoon.orbitOffset = new Vector2(Random.Range((AUtoSystemScale / 200f) * -1f, AUtoSystemScale / 200f), Random.Range((AUtoSystemScale / 200f) * -1f, AUtoSystemScale / 200f)); //may need tweaking for proper offset ranges.

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
            newMoon.orbitStart = Random.Range(0, 360f);
            newMoon.rotatesClockwise = Random.Range(0, 1.0f) < .5f ? true : false;
            newMoon.numMoons = 0;
            newMoon.moons = null;

            planetInfo.moons.Add(newMoon);
        }


    }

    void GenerateSystemObjects(StarSystemInfo infoBase)
    {
        Debug.Log("Generating System Objects");
        //generate system gameobject, add any system handler scripts to this
        if (systemContainer)
        {
            systemContainer.SetActive(false); //NOTE: for debug gen only
        }
        systemContainer = new GameObject();
        systemContainer.transform.parent = this.transform;
        systemContainer.name = "System Container " + xCoord + " : " + yCoord + " : " + infoBase.systemName;

        GameObject centralObject = GenerateCentralObject(infoBase);
        centralObject.transform.parent = systemContainer.transform;
        for (int i = 0; i < infoBase.orbitals.Count; i++)
        {
            Debug.Log("generating orbitals count for " + i + " of " + infoBase.orbitals.Count);
            GameObject newPlanet = GeneratePlanetGameobject(infoBase.orbitals[i], infoBase, systemContainer.gameObject);
            for (int m = 0; m < infoBase.orbitals[i].numMoons; m++)
            {
                Debug.Log("Genning moon " + m);
                GameObject newMoon = GenerateMoonGameobject(infoBase.orbitals[i].moons[m], newPlanet, infoBase);
            }
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

    GameObject GenerateCentralObject(StarSystemInfo infoBase)
    {
        //create star/blackole/other central object.
        GameObject centralObject = new GameObject(infoBase.systemName, typeof(CapsuleCollider2D));
        
        GameObject spriteObject = new GameObject("Sprite", typeof(SpriteRenderer));
        spriteObject.transform.parent = centralObject.transform;
        GameObject systemInfo = new GameObject("System Info", typeof(CanvasRenderer), typeof(Text), typeof(SystemInfo));
        systemInfo.transform.parent = centralObject.transform;
        GameObject minimapIcon = new GameObject("Minimap Icon", typeof(SpriteRenderer));
        minimapIcon.transform.parent = centralObject.transform;
        GameObject gravityField = new GameObject("Gravity Field", typeof(CircleCollider2D), typeof(PlanetGravity));
        gravityField.transform.parent = centralObject.transform;

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
        Debug.Log("generating system planet object");
        GameObject newPlanet = new GameObject(planetInfo.orbitalName, typeof(CapsuleCollider2D), typeof(PlanetHitDetection), typeof(Orbitable), typeof(Rotate));
        newPlanet.transform.parent = centerObject.transform;
        var orbitCopy = newPlanet.AddComponent<OrbitableInfo>();
        orbitCopy.SetCopyFrom(planetInfo);
        GameObject spriteObject = new GameObject("Sprite", typeof(SpriteRenderer));
        spriteObject.transform.parent = newPlanet.transform;
        GameObject planetHighlight = new GameObject("Planet Highlight", typeof(SpriteRenderer));
        planetHighlight.transform.parent = newPlanet.transform; //TODO set up base sprite for highlighter
        GameObject systemInfo = new GameObject("System Info", typeof(CanvasRenderer), typeof(Text), typeof(SystemInfo));
        systemInfo.transform.parent = newPlanet.transform;
        GameObject minimapIcon = new GameObject("Minimap Icon", typeof(SpriteRenderer));
        minimapIcon.transform.parent = newPlanet.transform;
        GameObject planetDarkness = new GameObject("Planet Darkness", typeof(SpriteRenderer), typeof(LookAt));
        planetDarkness.transform.parent = newPlanet.transform;
        GameObject gravityField = new GameObject("Gravity Field", typeof(CircleCollider2D), typeof(PlanetGravity));
        gravityField.transform.parent = newPlanet.transform;

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
        orbitSet.size = planetInfo.orbitSize;
        orbitSet.offset = planetInfo.orbitOffset;
        orbitSet.angle = planetInfo.orbitAngle;
        orbitSet.clockWise = planetInfo.rotatesClockwise;
        orbitSet.drawGizmo = true;
        Debug.Log("done generating planet");
        return newPlanet;
    }

    GameObject GenerateMoonGameobject(OrbitableInfo moonInfo, GameObject parentObject, StarSystemInfo infoBase)
    {
        Debug.Log("<b><color=red>MAKIN DA MOON</color></b>"); 
        GameObject newMoon = new GameObject(moonInfo.orbitalName, typeof(CapsuleCollider2D), typeof(PlanetHitDetection), typeof(Orbitable), typeof(Rotate));
        newMoon.transform.parent = parentObject.transform;
        var orbitCopy = newMoon.AddComponent<OrbitableInfo>();
        orbitCopy.SetCopyFrom(moonInfo);
        GameObject spriteObject = new GameObject("Sprite", typeof(SpriteRenderer));
        spriteObject.transform.parent = newMoon.transform;
        GameObject planetHighlight = new GameObject("Planet Highlight", typeof(SpriteRenderer));
        planetHighlight.transform.parent = newMoon.transform; //TODO set up base sprite for highlighter
        GameObject systemInfo = new GameObject("System Info", typeof(CanvasRenderer), typeof(Text), typeof(SystemInfo));
        systemInfo.transform.parent = newMoon.transform;
        GameObject minimapIcon = new GameObject("Minimap Icon", typeof(SpriteRenderer));
        minimapIcon.transform.parent = newMoon.transform;
        GameObject planetDarkness = new GameObject("Planet Darkness", typeof(SpriteRenderer), typeof(LookAt));
        planetDarkness.transform.parent = newMoon.transform;
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
        orbitSet.size = moonInfo.orbitSize;
        orbitSet.offset = moonInfo.orbitOffset;
        orbitSet.angle = moonInfo.orbitAngle;
        orbitSet.clockWise = moonInfo.rotatesClockwise;
        orbitSet.drawGizmo = true;


        Debug.Log("done makin da moon");

        return newMoon;
    }
}
