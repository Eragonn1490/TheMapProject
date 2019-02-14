using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbitable : MonoBehaviour
{
    //Public
    public GameObject star;
    public LineRenderer lr;
    public bool drawGizmo;
    [Header("Orbit Parameters")]
    public Vector2 size = new Vector2(2, 1);
    public Vector2 offset = new Vector2(0, 0);
    public float angle = 0;
    public int res = 200;
    public bool clockWise = false;
    [Header("Planet Parameters")]
    public int startPoint = 0; //may also cause weirdness with gen, because you need to know acceptable start ranges when you gen.
    public float speed = 1;

    [HideInInspector]
    public Vector2[] trajectory;

    //Private
    private int currentPoint;
    private Vector2 preOffsetPos;

    private void Start()
    {
        if (star == null)
            return;

        if (lr == null)
            lr = GetComponent<LineRenderer>();

        UpdateTrajectory();

        currentPoint = startPoint;
        preOffsetPos = trajectory[startPoint];
        transform.position = (Vector3)preOffsetPos + star.transform.position;
    }

    private void Update()
    {

        if (star == null)
            return;

        //Find the point closest to desired distance
        float dist = 0;

        for (; ; currentPoint++) //note: infinite loop here for certain speeds possible, like high values
        {
            
            dist = Vector2.Distance(trajectory[currentPoint], preOffsetPos);
            if (currentPoint == res) currentPoint = 0;
            if (dist >= speed * Time.deltaTime) break;
        }

        //Cut mag of direction to a bit shorter as needed
        Vector2 dir = trajectory[currentPoint] - preOffsetPos;
        dir = dir.normalized * speed * Time.deltaTime;
        //Debug.DrawLine(transform.localPosition, transform.position + (Vector3)dir, Color.red);

        //Update pre offset position
        preOffsetPos += dir;

        //Update position with offset
        transform.position = star.transform.position + (Vector3)preOffsetPos;

    }

    public void UpdateTrajectory()
    {
        //Create trajectory array if necessary
        if (trajectory == null || res != trajectory.Length + 1)
            trajectory = new Vector2[res + 1];

        //Calculate trajectory
        for (int i = 0; i < res; i++)
        {
            float index = i;
            if (clockWise) index = res - i;
            float theta = index / res * 2 * Mathf.PI;
            trajectory[i] = GetPoint(theta);
        }
        trajectory[trajectory.Length - 1] = trajectory[0];

        //Update line renderer too
        if (lr != null)
        {
            lr.positionCount = res + 1;
            for (int i = 0; i < res; i++)
                lr.SetPosition(i, trajectory[i]);
            lr.SetPosition(res, lr.GetPosition(0));
        }
    }

    public void MoveToStart()
    {
        currentPoint = startPoint;
        preOffsetPos = trajectory[startPoint];
        transform.position = (Vector3)preOffsetPos + star.transform.position;
    }

    private Vector2 GetPoint(float theta)
    {
        Quaternion quat = Quaternion.AngleAxis(-angle, Vector3.forward);
        Vector2 tmp = new Vector2(size.x * Mathf.Cos(theta), size.y * Mathf.Sin(theta));
        tmp = quat * tmp;
        tmp += offset;
        return tmp;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmo && trajectory != null)
        {
            for (int i = 1; i < trajectory.Length; i++)
            {
                Gizmos.DrawLine(
                    star.transform.position + (Vector3)trajectory[i - 1],
                    star.transform.position + (Vector3)trajectory[i]);
            }
        }
    }
}