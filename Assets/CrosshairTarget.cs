using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
    [SerializeField]Camera mainCamera;
    Ray ray;
    RaycastHit hitInfo;

    void Update()
    {
        GetAimMiddle();
    }
    public void GetAimMiddle()
    {
        ray.origin = mainCamera.transform.position;
        ray.direction = mainCamera.transform.forward;
        Physics.Raycast(ray, out hitInfo);
        transform.position = hitInfo.point;
    }
}
