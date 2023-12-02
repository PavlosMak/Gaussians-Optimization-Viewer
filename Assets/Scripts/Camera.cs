using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;


[System.Serializable]
public class Camera
{
    public int id;
    public string img_name;
    public int width;
    public int height;
    public Vector3 position;
    public Quaternion rotation;
    public double fy;
    public double fx;

    public Camera(int id, string imgName, int width, int height, Vector3 position, Quaternion rotation, double fy,
        double fx)
    {
        this.id = id;
        this.img_name = imgName;
        this.width = width;
        this.height = height;
        this.position = position;
        this.rotation = rotation;
        this.fy = fy;
        this.fx = fx;
    }
}