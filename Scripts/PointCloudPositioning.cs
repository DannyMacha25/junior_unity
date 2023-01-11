using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudPositioning : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public GameObject rectangle;
    Transform[] tf;
    void Start()
    {
        tf = this.GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        tf = this.GetComponentsInChildren<Transform>();
        setPositions(position, rotation);
        setScale(scale);
    }

    void setPositions(Vector3 pos, Vector3 rot)
    {
        Debug.Log(tf.Length);
        tf[1].position = rectangle.transform.position;
        tf[1].localRotation = Quaternion.Euler(rot);
    }

    void setScale(Vector3 s)
    {
        tf[1].localScale = s;
    }
}
