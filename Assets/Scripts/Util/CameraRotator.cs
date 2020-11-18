using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRotator : MonoBehaviour
{
    public float rotationSpeed;
    public float xSpeed;
    public float ySpeed;
    public float zSpeed;

    Camera c;

    void Start()
    {
        c = GetComponent<Camera>();
    }

    void Update()
    {
        c.transform.rotation = Quaternion.Euler(Time.time * rotationSpeed * xSpeed, Time.time * rotationSpeed * ySpeed, Time.time * rotationSpeed * zSpeed);
    }
}
