using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTrackPlayer : MonoBehaviour
{
    public GameObject trackObject;

    public bool isTracking; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTracking && trackObject)
        {
            this.transform.position = new Vector3(trackObject.transform.position.x, trackObject.transform.position.y, -10f);
        }
        
    }
}
