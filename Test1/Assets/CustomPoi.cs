using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPoi : MonoBehaviour
{
    Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
        Debug.Log(mainCamera.name);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCamera.transform);
        var r = transform.rotation;
        r.x = 0;
        r.z = 0;
        transform.rotation = r;
    }
}
