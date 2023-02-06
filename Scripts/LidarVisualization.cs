using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ros_CSharp;
using sm = Messages.sensor_msgs;


public class LidarVisualization : MonoBehaviour
{
    [Header("Ros")]
    public ROSCore rosmaster;
    public string topic;
    [Header("Objects")]
    public GameObject pointRender;
    [Header("Settings")]
    public Color circleColor;
    public Color lineColorStart,lineColorEnd;
    public Vector2 startPosition;
    public bool showLines, showCircles;
    public bool flipX, flipY;
    public float spread;
    public float rotation;
    [Header("Debug")]
    public int pointCount;
    //TODO NEXT: EXTRA SETTINGS, LINES, CHANGE SHAPE
    private NodeHandle nh;
    private LineRenderer lr;
    private List<Vector2> positions;
    private List<GameObject> uiPoints = new List<GameObject>();
    private bool listPopulated = false,firstCB = false;
    private int numberAngles;
    void Start()
    {
        nh = rosmaster.getNodeHandle();
        nh.subscribe<sm.LaserScan>(topic,10,LaserCB);
        lr = this.GetComponent<LineRenderer>();
    }
    
    void LaserCB(sm.LaserScan msg)
    {
        firstCB = true;
        //print(msg.ranges.Length);
        positions = PopulatePointList(msg.ranges, msg.angle_min, msg.angle_max,msg.angle_increment);
        numberAngles = msg.ranges.Length;
        lr.positionCount = positions.Count;

    }
    void PopulateGameObjectList(List<GameObject> objects,int length)
    {
        for(int x = 0; x < length; x++)
        {
            GameObject p = Instantiate(pointRender, this.transform);
            p.transform.position = new Vector3(0,0,0);
            objects.Add(p);
        }
    }
    void DisplayPoints(List<Vector2> points,List<GameObject> objects)
    {  
        for(int i = 0; i < numberAngles; i++)
        {
            GameObject p = objects[i];
            if (points[i].x != Mathf.Infinity && points[i].y != Mathf.Infinity && !double.IsNaN(points[i].x) && !double.IsNaN(points[i].y))
            {
         
                p.transform.position = new Vector3(points[i].x, points[i].y);
            }
            else
            {
                p.transform.position = startPosition;
            }
        }
    }

    void DrawLines(List<Vector2> points)
    {
        lr.positionCount = points.Count;
        lr.startColor = lineColorStart;
        lr.endColor = lineColorEnd;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].x != Mathf.Infinity && points[i].y != Mathf.Infinity)
            {
                Vector3 p = new Vector3((points[i].x + startPosition.x) * spread, (points[i].y + startPosition.y) * spread, -1);
                lr.SetPosition(i, points[i]);
            }
            else
            {
                Vector3 p = new Vector3(0, 0, 229);
                lr.SetPosition(i, p);
            }
            if(i > points.Count - pointCount)
            {
                Vector3 p = new Vector3(0, 0, 229);
                lr.SetPosition(i, p);
            }
        }
    }
    void FlushPointObjects()
    {
        GameObject g = this.gameObject;
        for (var i = g.transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(g.transform.GetChild(i).gameObject);
        }
    }
    Vector2 CalculatePoint(float angle, float distance)
    {
        Vector2 point = new Vector2();
        point.x = distance * Mathf.Cos(angle) * spread + startPosition.x;
        point.y = distance * Mathf.Sin(angle) * spread + startPosition.y;

        var v = point - startPosition;
        Quaternion rot = Quaternion.Euler(0, 0, rotation);

        v = rot * v;
        v = startPosition + v;
        point = v;

        if (flipX)
        {
            point.x *= -1;
        }
        if (flipY)
        {
            point.y *= -1;
        }
        return point;
    }



    List<Vector2> PopulatePointList(float[] distances,float minAngle, float maxAngle, float increment)
    {
        int numAngles = distances.Length;
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < numAngles; i++)
        {
            Vector2 point = CalculatePoint(minAngle + (increment * i), distances[i]);
            points.Add(point);
        }
        return points;
    }

    void ClearLines(List<Vector2> points)
    {
        lr.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = new Vector3(0, 0, 229);
            lr.SetPosition(i, p);
        }
    }

    void ClearCirclees()
    {
        Color clear = new Color(0, 0, 0, 0);
        foreach (SpriteRenderer point in this.GetComponentsInChildren<SpriteRenderer>())
        {
            point.color = clear; ;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(SpriteRenderer point in this.GetComponentsInChildren<SpriteRenderer>())
        {
            point.color = circleColor; 
        }
        if(firstCB == true && !listPopulated)
        {
            listPopulated = true;
            PopulateGameObjectList(uiPoints, numberAngles);
        }
        if (listPopulated)
        {
            if (showCircles)
            {
                DisplayPoints(positions, uiPoints);
            }
            else
            {
                ClearCirclees();
            }
            if (showLines)
            {
                DrawLines(positions);
            }
            else
            {
                ClearLines(positions);
            }
        }
        
    }
}
