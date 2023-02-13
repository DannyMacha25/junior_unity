using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ros_CSharp;
using sm = Messages.sensor_msgs;

/* NOTES:
 * 1. I should probably make the color of the center dot on the circle visualization
 *  a different color.
 * 
 * 
 * 
 */
public class LidarVisualization : MonoBehaviour
{
    [Header("Ros")]
    public ROSCore rosmaster;
    public string topic;
    [Header("Objects")]
    public GameObject pointRender;
    [Header("Settings")]
    public Color circleColor;
    public Color lineColor;
    public Vector2 startPosition;
    public bool showLines, showCircles;
    public bool flipX, flipY;
    public float lineWidth = 0.2f;
    public float spread;
    public float rotation;
    public int lineSmoothing = 1;
    [Header("Debug - Don't worry abt this :3")]
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
    /*
     * Populate game oobject list with sprite objects
     * 
     */
    void PopulateGameObjectList(List<GameObject> objects,int length)
    {
        for(int x = 0; x < length; x++)
        {
            GameObject p = Instantiate(pointRender, this.transform);
            p.transform.position = new Vector3(0,0,0);
            objects.Add(p);
        }
    }
    /*
     * Makes the circle sprites visible and updates their positions
     * 
     */
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
                if (flipX)
                {
                    p.transform.position = new Vector3(p.transform.position.x * -1, p.transform.position.y, p.transform.position.z);
                }
                if (flipY)
                {
                    p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y * -1, p.transform.position.z);
                }
            }
        }
    }

    /*
     * Displays the line renderer and updates all of the points
     * 
     */
    void DrawLines(List<Vector2> points)
    {
        lr.startWidth = lineWidth;
        //lr.endWidth = lineWidth;
        lr.positionCount = points.Count;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        Vector3 prevValidPoint = new Vector3(0, 0, -2.7f);
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].x != Mathf.Infinity && points[i].y != Mathf.Infinity && !double.IsNaN(points[i].x) && !double.IsNaN(points[i].y))
            {
                if (lineSmoothing > 0 && i % lineSmoothing == 0)
                {
                    Vector3 p = new Vector3(points[i].x, points[i].y, -2.7f);
                    lr.SetPosition(i, p);
                    prevValidPoint = p;
                }
                else
                {
                    lr.SetPosition(i, prevValidPoint);
                }
            }
            else
            {
                if (points[i + 1].x != Mathf.Infinity && points[i + 1].y != Mathf.Infinity && !double.IsNaN(points[i + 1].x) && !double.IsNaN(points[i + 1].y))
                {
                    Vector3 p = new Vector3(points[i + 1].x, points[i + 1].y, 229);
                    lr.SetPosition(i, p);
                }
                else 
                {
                    Vector3 p = new Vector3(prevValidPoint.x, prevValidPoint.y, 229);
                    lr.SetPosition(i, p);
                }
            }
            if(i > points.Count - pointCount)
            {
                Vector3 p = new Vector3(0, 0, 229);
                lr.SetPosition(i, p);
            }
        }
    }
    /*
     * Unecessary :3
     * 
     */
    void FlushPointObjects()
    {
        GameObject g = this.gameObject;
        for (var i = g.transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(g.transform.GetChild(i).gameObject);
        }
    }
    /*
     * Calculate 2D point given an angle and distance (based of startPosition)
     * 
     */
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

    /*
     * Populates the point list with calculated points
     * 
     */
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

    /*
     * Just sets all the points on the linerenderer to a non viewablle spot
     * 
     */
    void ClearLines(List<Vector2> points)
    {
        lr.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = new Vector3(0, 0, 229);
            lr.SetPosition(i, p);
        }
    }

    /*
     * Gives all the circles an alpha of 0
     * 
     */
    void ClearCirclees()
    {
        Color clear = new Color(0, 0, 0, 0);
        foreach (SpriteRenderer point in this.GetComponentsInChildren<SpriteRenderer>())
        {
            point.color = clear;
        }
    }

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
