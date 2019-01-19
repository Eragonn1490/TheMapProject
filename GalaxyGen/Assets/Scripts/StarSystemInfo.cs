﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystemInfo
{
    public string systemName;
    public int generatedSystemSeed;
    public float systemDistanceScaleMod;
    public Generation_Solar_System.STARTEMPERATURES_TYPES mainBodyType;
    public Generation_Solar_System.SYSTEM_SIZE systemSize;
    public Generation_Solar_System.SYSTEM_TYPE systemType;
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
    public List<GameObject> orbitalGameobjects = new List<GameObject>(); 

    public void SetCelestialBodyInfo(Generation_Solar_System.STARTEMPERATURES_TYPES inBodyType, Generation_Solar_System.SYSTEM_SIZE inSize, Generation_Solar_System.SYSTEM_TYPE inSystemType, float inSizeScale, float inPhysicalSize, float inBodyDensity, float inTempmeratureKelvin, bool asteroidBelt, float inBaseAngle)
    {
        mainBodyType = inBodyType; //type mostly will help point to the sprite asset used
        systemSize = inSize;
        systemType = inSystemType;
        mainBodyUnitySizeScale = inSizeScale;
        mainBodyPhysicalSize = inPhysicalSize;
        mainBodyDensity = inBodyDensity;
        mainBodyTemperatureKelvin = inTempmeratureKelvin;
        baseSystemAngle = inBaseAngle;
    }

    public void SetRegionRangeValues(Generation_Solar_System.Star_Region_Range inHot, Generation_Solar_System.Star_Region_Range inHab, Generation_Solar_System.Star_Region_Range inCold, Generation_Solar_System.Star_Region_Range inAsteroid)
    {
        hotRange = inHot;
        habRange = inHab;
        coldRange = inCold;
        asteroidBeltRange = inAsteroid;
    }

    public void SetDistanceScale(float inDistanceScale)
    {
        systemDistanceScaleMod = inDistanceScale;
    }

    public void AddOrbitalObject(OrbitableInfo inObject)
    {
        //Debug.Log("testing orbital object " + inObject.orbitalName + " " + inObject.GetInstanceID());
        //if (!orbitals.Contains(inObject))
        {
            Debug.Log("adding new orbital object " + inObject.orbitalName);
            orbitals.Add(inObject);
        }
    }
    
}
