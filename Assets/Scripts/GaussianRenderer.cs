using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;

public class GaussianRenderer : MonoBehaviour
{
    public string
        filePath =
            "/home/pavlos/Desktop/stuff/Uni-Masters/Q5/GraphicsSeminar/train_smaller_frames/1000.csv"; // Set the file path in the Inspector or provide it programmatically

    private GaussianAnimation _animation;
    private int frame = 0;

    private MaterialPropertyBlock block;

    [SerializeField] private Mesh ellipsoidMesh; // Reference to the ellipsoid mesh
    [SerializeField] private Material ellipsoidMaterial; // Reference to the material to be used for the ellipsoids


    private List<Gaussian> ReadFrame(string filePath)
    {
        List<Gaussian> gaussians = new List<Gaussian>();
        if (!string.IsNullOrEmpty(filePath))
        {
            // Check if the file exists at the specified path
            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    sr.ReadLine(); // Skip row names
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine(); // Read a line from the file

                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] fields = line.Split(','); // Split the line into fields using comma as delimiter
                            Vector3 position = new Vector3(
                                float.Parse(fields[2]), float.Parse(fields[3]), float.Parse(fields[4])
                            );
                            Vector3 scale = new Vector3(
                                float.Parse(fields[5]), float.Parse(fields[6]), float.Parse(fields[7]));
                            // if (scale.x >= 10 || scale.y >= 10 || scale.z >= 10)
                            // {
                            //     continue; //Filter big gaussians
                            // }

                            Quaternion rotation = new Quaternion(
                                float.Parse(fields[8]), float.Parse(fields[9]), float.Parse(fields[10]),
                                float.Parse(fields[11])
                            );
                            float opacity = float.Parse(fields[12]);
                            Gaussian gaussian = new Gaussian(position, scale, rotation, opacity);
                            gaussians.Add(gaussian);
                        }
                    }
                }

                Debug.Log("Gaussians read!");
                Debug.Log(gaussians.Count);
            }
            else
            {
                Debug.LogError("File does not exist at the specified path: " + filePath);
            }
        }
        else
        {
            Debug.LogError("File path is not specified!");
        }

        return gaussians;
    }

    void Start()
    {
        Debug.Log("Loading Animation...");
        string basePath = "/home/pavlos/Desktop/stuff/Uni-Masters/Q5/GraphicsSeminar/train_smaller_frames/";
        _animation = new GaussianAnimation();
        for (int i = 10; i <= 1000; i += 10)
        {
            string path = basePath + i + ".csv";
            _animation.AddFrame(ReadFrame(path));
        }

        block = new MaterialPropertyBlock();
        Debug.Log("Loaded Animation - " + _animation.Frames.Count + " frames!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            frame = (frame + 1) % _animation.Frames.Count;
            Debug.Log(frame);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            frame = Math.Max(0, (frame - 1)) % _animation.Frames.Count;
            Debug.Log(frame);
        }

        RenderParams rp = new RenderParams(ellipsoidMaterial) { matProps = block };

        foreach (var gaussian in _animation.Frames[frame])
        {
            Matrix4x4 matrix = Matrix4x4.TRS(gaussian.Position, gaussian.Rotation, gaussian.Scale);
            //TODO CONT FROM: We need the actual colors
            block.SetColor("_Color", new Color(gaussian.Opacity, 0, 0, gaussian.Opacity));
            Graphics.RenderMesh(rp, ellipsoidMesh, 0, matrix);
        }
    }
}