using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemExit : MonoBehaviour
{
    public float coordXExitSystem;
    public float coordYExitSystem;
    public string exitSystemName;

    public TextMesh nameMesh;

    public int linkID;

    public void GenExit(float exitX, float exitY, float originX, float originY, string exitName, float systemDistanceSet)
    {

        Debug.Log("attempting to generate exit to " + exitX + " " + exitY + " based on " + originX + " " + originY);
        coordXExitSystem = exitX;
        coordYExitSystem = exitY;
        exitSystemName = exitName;

        Vector3 distMod = new Vector3(exitX - originX, exitY - originY, 0);
        nameMesh.text = exitSystemName;
        Vector3 normalized = distMod.normalized;

        Vector3 setPosition = systemDistanceSet * normalized;
        this.transform.localPosition = setPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GalaxyHelper.instance.LoadSystemLinkFromSystem(coordXExitSystem, coordYExitSystem, linkID, exitSystemName);
        }
    }
}
