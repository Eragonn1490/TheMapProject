using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProps : MonoBehaviour
{
    public static GameObject PLAYER_OBJECT;

    /// <summary>
    /// These 4 static variables (Down below) save data, so room can be created/joined after the scene change
    /// </summary> 

    public static GameObject m_LastHitPlanet;
    public static bool m_IsPlanetChosen = false;
    public static string m_PlanetName = "";
    public static int m_SolarID = 1;
    //public static NetPlayerController m_NetPC;
    public static PlayerController m_PC;
}