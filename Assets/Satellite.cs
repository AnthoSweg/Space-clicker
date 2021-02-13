using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : MonoBehaviour
{
    public Vector2 speedMinMax;
    public Vector2 dstMinMax;

    private float speed;
    private Transform t;

    void Start()
    {
        t = this.transform;
        t.eulerAngles = RandomRotation();
        speed = Random.Range(speedMinMax.x, speedMinMax.y);
        t.GetChild(0).transform.localPosition = new Vector3(Random.Range(dstMinMax.x, dstMinMax.y), 0, 0);
    }

    void Update()
    {
        //Vector3 newRot = t.eulerAngles;
        //newRot += t.forward.normalized * speed * Time.deltaTime;
        //t.eulerAngles = newRot;

        t.Rotate(new Vector3(0, speed * Time.deltaTime, 0), Space.Self);
    }

    private Vector3 RandomRotation()
    {
        Vector3 newRot = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        return newRot;
    }
}
