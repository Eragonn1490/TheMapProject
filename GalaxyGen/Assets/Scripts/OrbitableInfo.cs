using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitableInfo
{
    public string orbitalName;
    public int orbitalID = -1; //number from gen, sequential
    public int orbitalOrder = -1; //set after generation by distance
    public int orbitTarget = -1; //-1  = the central object, 0+ orbiting a planet: number from gen.
    public GameObject orbitTargetObject; //only set during object gen phase.
    //type of planet, size of planet, planet coloration(which sprite to use), gravity, etc.
    public Generation_Solar_System.ORBITAL_TYPES orbitalType;
    public Generation_Solar_System.SYSTEM_REGION_RANGES orbitalRange;

    public int spritePlanetResourceID;
    public float distanceFromCenterAU;
    public float distanceFromCenterUnityScale;
    public float planetUnityScale; //default is .08 for normal planets, .23 for gas giants, .05f for moons

    public Vector2 orbitOffset;
    public Vector2 orbitSize;
    public float orbitAngle;
    public float orbitSpeed;
    public int orbitStart;
    public bool rotatesClockwise;

    public int numMoons;
    public List<OrbitableInfo> moons = new List<OrbitableInfo>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCopyFrom(OrbitableInfo otherInfo)
    {
        orbitalName = otherInfo.orbitalName;
        orbitalID = otherInfo.orbitalID;
        orbitalOrder = otherInfo.orbitalOrder;
        orbitTarget = otherInfo.orbitTarget;
        orbitTargetObject = otherInfo.orbitTargetObject;
        orbitalType = otherInfo.orbitalType;
        orbitalRange = otherInfo.orbitalRange;
        spritePlanetResourceID = otherInfo.spritePlanetResourceID;
        distanceFromCenterAU = otherInfo.distanceFromCenterAU;
        distanceFromCenterUnityScale = otherInfo.distanceFromCenterUnityScale;
        planetUnityScale = otherInfo.planetUnityScale;
        orbitOffset = otherInfo.orbitOffset;
        orbitSize = otherInfo.orbitSize;
        orbitAngle = otherInfo.orbitAngle;
        orbitSpeed = otherInfo.orbitSpeed;
        orbitStart = otherInfo.orbitStart;
        rotatesClockwise = otherInfo.rotatesClockwise;
    }
}
