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

    private NodeHandle nh;
    private List<Vector2> positions;
    private List<GameObject> uiPoints = new List<GameObject>();
    private bool listPopulated = false,firstCB = false;
    private int numberAngles;
    void Start()
    {
        nh = rosmaster.getNodeHandle();
        nh.subscribe<sm.LaserScan>(topic,10,LaserCB);
    }
    
    void LaserCB(sm.LaserScan msg)
    {
        firstCB = true;
        print(msg.ranges.Length);
        positions = PopulatePointList(msg.ranges, msg.angle_min, msg.angle_max,msg.angle_increment);
        numberAngles = msg.ranges.Length;
        
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
            if (points[i].x != Mathf.Infinity && points[i].y != Mathf.Infinity)
            {
                GameObject p = objects[i];
                p.transform.position = new Vector3(points[i].x, points[i].y);
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
        point.x = distance * Mathf.Cos(angle);
        point.y = distance * Mathf.Sin(angle);

        return point;
    }

    List<Vector2> PopulatePointList(float[] distances,float minAngle, float maxAngle, float increment)
    {
        int numAngles = distances.Length;
        List<Vector2> points = new List<Vector2>();
        for (int x = 0; x < numAngles; x++)
        {
            points.Add(CalculatePoint(minAngle + (increment * x), distances[x]));
        }
        return points;
    }
    // Update is called once per frame
    void Update()
    {
        if(firstCB == true && !listPopulated)
        {
            listPopulated = true;
            PopulateGameObjectList(uiPoints, numberAngles);
        }
        if (listPopulated)
        {
            Debug.Log(uiPoints.Count);
            DisplayPoints(positions, uiPoints);
        }
        
    }
}
