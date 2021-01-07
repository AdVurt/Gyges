using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObject : MonoBehaviour
{

    public Transform objectToRotateAround;
    public float rotateX = 0f;
    public float rotateY = 0f;
    public float rotateZ = 0f;

    private Transform _transform;

    void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        _transform.RotateAround(objectToRotateAround.position, Vector3.right, rotateX * Time.deltaTime);
        _transform.RotateAround(objectToRotateAround.position, Vector3.up, rotateY * Time.deltaTime);
        _transform.RotateAround(objectToRotateAround.position, Vector3.forward, rotateZ * Time.deltaTime);
    }
}
