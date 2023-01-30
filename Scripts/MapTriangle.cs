using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ros_CSharp;
using Messages.geometry_msgs;
using Messages.nav_msgs;

public class MapTriangle : MonoBehaviour
{
    [Header("Ros")]
    public ROSCore rosmaster;
    NodeHandle nh;

    [Header("Unity")]
    public GameObject triangle;
    public float uiMapSizeX;
    public float uiMapSizeY;
    public bool flipX;
    public bool flipY;

    private UnityEngine.Vector3 currPos;
    private UnityEngine.Quaternion currRot;
    private UnityEngine.Vector2 mapSize = new Vector2(0,0);

    void Start()
    {
        nh = rosmaster.getNodeHandle();
        nh.subscribe<TransformStamped>("/robot_position", 10,posCB);
        nh.subscribe<OccupancyGrid>("/map", 10, mapCB);
    }

    void posCB(TransformStamped msg)
    {
        //Debug.Log((float)msg.transform.translation.x);
        currPos = CalculateTrianglePosition(mapSize, msg);
        currRot = new UnityEngine.Quaternion();
        currRot.x = (float)msg.transform.rotation.x;
        currRot.y = (float)msg.transform.rotation.y;
        currRot.z = (float)msg.transform.rotation.z;
        currRot.w = (float)msg.transform.rotation.w;

    }

    void mapCB(OccupancyGrid msg)
    {
        //mapSize = new Vector2(msg.info.height, msg.info.width);
        mapSize = new Vector2(2.7f,2.7f);
    }

    UnityEngine.Vector3 CalculateTrianglePosition(Vector2 size,TransformStamped msg)
    {

        UnityEngine.Vector3 trianglePos;
        float scaleX = size.x / uiMapSizeX;
        float scaleY = size.y / uiMapSizeY;

        if (flipX)
        {
            scaleX *= -1f;
        }
        if (flipY)
        {
            scaleY *= -1f;
        }

        trianglePos = new UnityEngine.Vector3((float)msg.transform.translation.x/scaleX,0f,(float)msg.transform.translation.y/scaleY);
        return trianglePos;
    }
    // Update is called once per frame
    void Update()
    {
        triangle.GetComponent<UnityEngine.Transform>().localPosition = currPos;
        triangle.GetComponent<UnityEngine.Transform>().rotation = currRot;
        
    }
}
