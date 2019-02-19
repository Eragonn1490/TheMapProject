using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitableInfoMoon
{
    public string orbitalName;
    public int orbitalID = -1; //number from gen, sequential
    public int orbitalOrder = -1; //set after generation by distance
    public int orbitTarget = -1; //-1  = the central object, 0+ orbiting a planet: number from gen.
    //public GameObject orbitTargetObject; //only set during object gen phase.
    //type of planet, size of planet, planet coloration(which sprite to use), gravity, etc.
    public Generation_Solar_System.ORBITAL_TYPES orbitalType;
    public Generation_Solar_System.SYSTEM_REGION_RANGES orbitalRange;

    public int spritePlanetResourceID;
    public float distanceFromCenterAU;
    public float distanceFromCenterUnityScale;
    public float planetUnityScale; //default is .08 for normal planets, .23 for gas giants, .05f for moons


    public float orbitOffsetX;
    public float orbitOffsetY;

    public float orbitSizeX;
    public float orbitSizeY;
    public float orbitAngle;
    public float orbitSpeed;
    public int orbitStart;
    public bool rotatesClockwise;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCopyFrom(OrbitableInfoMoon otherInfoMoon)
    {
        orbitalName = otherInfoMoon.orbitalName;
        orbitalID = otherInfoMoon.orbitalID;
        orbitalOrder = otherInfoMoon.orbitalOrder;
        orbitTarget = otherInfoMoon.orbitTarget;
        //orbitTargetObject = otherInfoMoon.orbitTargetObject;
        orbitalType = otherInfoMoon.orbitalType;
        orbitalRange = otherInfoMoon.orbitalRange;
        spritePlanetResourceID = otherInfoMoon.spritePlanetResourceID;
        distanceFromCenterAU = otherInfoMoon.distanceFromCenterAU;
        distanceFromCenterUnityScale = otherInfoMoon.distanceFromCenterUnityScale;
        planetUnityScale = otherInfoMoon.planetUnityScale;
        orbitOffsetX = otherInfoMoon.orbitOffsetX;
        orbitOffsetY = otherInfoMoon.orbitOffsetY;
        orbitSizeX = otherInfoMoon.orbitSizeX;
        orbitSizeY = otherInfoMoon.orbitSizeY;
        orbitAngle = otherInfoMoon.orbitAngle;
        orbitSpeed = otherInfoMoon.orbitSpeed;
        orbitStart = otherInfoMoon.orbitStart;
        rotatesClockwise = otherInfoMoon.rotatesClockwise;
    }
}
