using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    public float rotationSpeed;
    public float xSpeed;
    public float ySpeed;
    public float zSpeed;

    // Update is called once per frame
    void Update()
    {
        //RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
        Skybox s = GetComponent<Skybox>();
        s.transform.rotation = Quaternion.Euler(Time.time * rotationSpeed * xSpeed, Time.time * rotationSpeed * ySpeed, Time.time * rotationSpeed * zSpeed);
    }
}
