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
    public float[] position;
    public float[][] rotation;
    public double fy;
    public double fx;

    public Camera(int id, string imgName, int width, int height, float[] position, float[][] rotation, double fy,
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

[System.Serializable]
public class CameraList
{
    public List<Camera> cameraList;
}