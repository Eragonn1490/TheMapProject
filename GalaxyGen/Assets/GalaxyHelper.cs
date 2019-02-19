using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GalaxyHelper : MonoBehaviour
{
    public static GalaxyHelper instance;
    public GameObject playerObject;

    public float systemX;
    public float systemY;
    public int systemID;
    public string systemName;

    bool lastPositionInStarSystem = false;
    public float lastSystemX;
    public float lastSystemY;
    public int lastSystemID;
    public string lastSystemName;

    public GameObject exitHolesPrefabTest;

    bool loadingStarmap;
    bool loadingStarsystem;
    bool wasAtStarmap;
    bool wasAtStarsystem;

    Generation_Solar_System currentSystemGen;

   

    public int[] nearbySystemIDS;
    public Vector2[] nearbySystemPositions;

    public Galaxy gennedGalaxy;
    public bool loadFromSaveInsteadOfGen;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        
    }

    bool genOnNextFrame;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (SceneManager.GetActiveScene().name == "Solar_System")
            {
                LoadMapFromSystem();
            }
        }
        
        if (loadingStarsystem)
        {
            Debug.Log("loading starsystem");
            if (SceneManager.GetActiveScene().name != "Solar_System")
            {
                return;
            }
            else
            {

                FinishLoadingStarSystem();
            }
        }
        if (loadingStarmap)
        {
            Debug.Log("loading starmap");
            if (SceneManager.GetActiveScene().name != "Main")
            {
                return;
            }
            else
            {
                FinishLoadingStarMap();
            }

        }
        //if (genOnNextFrame)
        //{
        //    FinishLoadingStarSystem();
        //    genOnNextFrame = false;

        //}
    }


    public void LoadMapFromSystem()
    {
        if (currentSystemGen != null)
        {
            lastSystemName = currentSystemGen.sysNameGen;
            lastSystemX = currentSystemGen.xCoord;
            lastSystemY = currentSystemGen.yCoord;
            lastSystemID = currentSystemGen.systemID;
        }
        currentSystemGen = null;
        systemName = "";

        

        loadingStarmap = true;
        loadingStarsystem = false;
        wasAtStarsystem = true;
        wasAtStarmap = false;

        if (playerObject)
        {
            playerObject.SetActive(false);
        }


        var genSys = GameObject.FindObjectOfType<Generation_Solar_System>();


        CallLoadStarmap();
        
    }

    public void LoadSystemFromMap(float sysX, float sysY, int sysID, string sysName)
    {
        
        currentSystemGen = null;

        systemX = sysX;
        systemY = sysY;
        systemID = sysID;
        systemName = sysName;
        //nearbySystemIDS = linkedIDs;
        //nearbySystemPositions = linkedPositions;

        loadingStarmap = false;
        loadingStarsystem = true;
        if (playerObject)
        {
            playerObject.SetActive(false);
        }
        
        wasAtStarmap = true;
        wasAtStarsystem = false;

        Debug.Log("system set to " + systemX + " " + systemY);

        CallLoadStarsystem();


    }

    public void LoadSystemLinkFromSystem(float sysX, float sysY, int sysID, string sysName)
    {

        lastSystemX = systemX;
        lastSystemY = systemY;
        lastSystemID = systemID;
        lastSystemName = systemName;

        systemX = sysX;
        systemY = sysY;
        systemID = sysID;
        systemName = sysName;

        loadingStarmap = false;
        loadingStarsystem = true;
        if (playerObject)
        {
            playerObject.SetActive(false);
        }

        wasAtStarmap = false;
        wasAtStarsystem = true;

        CallLoadStarsystem();
    }

    void CallLoadStarmap()
    {
        SceneManager.LoadScene("Main");
    }

    void CallLoadStarsystem()
    {
        SceneManager.LoadScene("Solar_System");
    }

    void FinishLoadingStarSystem()
    {
        if (!genOnNextFrame)
        {
            genOnNextFrame = true;
            return;
        }
        genOnNextFrame = false;
        Debug.Log("finishing system load");
        loadingStarsystem = false;
        
        if (!wasAtStarmap)
        {
            //need to position based on where it just came from
        }
        else
        {
            //just put it somewhere for now, can determine if this is even necessary later.
            playerObject.transform.position = new Vector3(-288, -90, 0);
        }

        

        currentSystemGen = GameObject.FindObjectOfType<Generation_Solar_System>();
        Debug.Log("current system gen is " + currentSystemGen);

        if (wasAtStarsystem)
        {
            currentSystemGen.ClearSystem();
            currentSystemGen.LoadInGenFromSolarSystem(gennedGalaxy.generatedSystems[systemID], gennedGalaxy.solarSystems[systemID], gennedGalaxy);
            float sysSizePlacement = (currentSystemGen.starSystemInfo.AUFarPlanet + .5f) * currentSystemGen.starSystemInfo.AUtoSystemScale * currentSystemGen.starSystemInfo.systemDistanceScaleMod;
            Vector3 normalDiff = new Vector3(lastSystemX - systemX, lastSystemY - systemY).normalized; //this needs work: it actually needs to go thru the exits in the system, find the one it came from, then place it near it based on orientation.
            normalDiff *= sysSizePlacement;
            playerObject.transform.position = normalDiff;
        }
        else
        {
            if (currentSystemGen && !loadFromSaveInsteadOfGen)
            {
                Debug.Log("attempting to generate system raw");
                currentSystemGen.xCoord = systemX;
                currentSystemGen.yCoord = systemY;
                currentSystemGen.systemID = systemID;
                currentSystemGen.sysNameGen = systemName;



                currentSystemGen.ALSOGENOBJECTS = true;
                currentSystemGen.GENSYSTEM = true;
            }
            else if (currentSystemGen)
            {
                Debug.Log("attempting to load from saved data instead of generating raw for " + systemID);
                currentSystemGen.LoadInGenFromSolarSystem(gennedGalaxy.generatedSystems[systemID], gennedGalaxy.solarSystems[systemID], gennedGalaxy);
            }
        }
        

        if (playerObject)
        {
            playerObject.SetActive(true);
        }
          
        if (Camera.main)
        {
            if (Camera.main.GetComponent<CamTrackPlayer>())
            {
                Camera.main.GetComponent<CamTrackPlayer>().trackObject = playerObject;
                Camera.main.GetComponent<CamTrackPlayer>().isTracking = true;
            }

        }

    }

    public void FinishLoadingStarMap()
    {
        loadingStarmap = false;
    }
}
