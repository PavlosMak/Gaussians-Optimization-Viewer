using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using UnityEngine.UIElements;
using TMPro;
using Unity.VisualScripting;

public class GaussianRenderer : MonoBehaviour
{
    private GaussianAnimation _animation;
    private int frame = 0;

    private MaterialPropertyBlock block;

    //TODO: Change this path to point to where you downloaded the frames.
    private string basePath = "path/to/optimization_frames/"; //don't forget the `/` in the end


    [SerializeField] private Mesh ellipsoidMesh; // Reference to the ellipsoid mesh
    [SerializeField] private Material ellipsoidMaterial; // Reference to the material to be used for the ellipsoids

    [SerializeField] private TMP_Text _frameCountText;
    [SerializeField] private TMP_Text _gaussiansCountText;

    [SerializeField] private Vector3 gaussianPosition = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private float gaussianScale = 1.0f;

    [SerializeField] private bool hideSky = false;

    [SerializeField] private Collider _collider;

    private bool showKernels = false;
    private bool animationPlaying = false;

    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject mainTrain;
    [SerializeField] private GameObject transparentTrain;


    private List<Gaussian> ReadFrame(string filePath, bool featuresAreRgb = false)
    {
        List<Gaussian> gaussians = new List<Gaussian>();
        if (!string.IsNullOrEmpty(filePath))
        {
            // Check if the file exists at the specified path
            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    sr.ReadLine(); // Skip first row since it's just the column names
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine(); // Read a line from the file
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] fields = line.Split(','); // Split the line into fields using comma as delimiter
                            Vector3 position = new Vector3(
                                float.Parse(fields[2]), -float.Parse(fields[3]), float.Parse(fields[4])
                            );
                            Vector3 scale = new Vector3(
                                float.Parse(fields[5]), float.Parse(fields[6]), float.Parse(fields[7]));

                            Quaternion rotation = new Quaternion(
                                float.Parse(fields[8]), float.Parse(fields[9]), float.Parse(fields[10]),
                                float.Parse(fields[11])
                            );
                            float opacity = float.Parse(fields[12]);

                            Vector3 colorFeatures = new Vector3(float.Parse(fields[13]), float.Parse(fields[14]),
                                float.Parse(fields[15]));

                            Gaussian gaussian = new Gaussian(position, scale, rotation, opacity, colorFeatures,
                                featuresAreRgb);
                            gaussians.Add(gaussian);
                        }
                    }
                }
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
        _animation = new GaussianAnimation();
        _animation.AddFrame(ReadFrame(basePath + 0 + ".csv", true));
        for (int i = 10; i <= 1000; i += 10)
        {
            string path = basePath + i + ".csv";
            _animation.AddFrame(ReadFrame(path));
        }

        block = new MaterialPropertyBlock();
    }

    void Update()
    {
        //first update the UI
        _frameCountText.text = "Frame: " + frame + "/100";
        _gaussiansCountText.text = "Gaussians: " + _animation.Frames[frame].Count;
        //control logic
        if (Input.GetKeyDown(KeyCode.T))
        {
            MakeTrainTransparent();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            showKernels = !showKernels;
        }

        if (!showKernels)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animationPlaying = !animationPlaying;
        }

        if (Input.GetKeyDown(KeyCode.N) || animationPlaying)
        {
            frame = (frame + 1) % _animation.Frames.Count;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            hideSky = !hideSky;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            frame = frame - 1;
            if (frame < 0)
            {
                frame = _animation.Frames.Count - 1;
            }
            // frame = Math.Max(0, (frame - 1)) % _animation.Frames.Count;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            floor.SetActive(!floor.activeSelf);
        }

        //rendering
        RenderParams rp = new RenderParams(ellipsoidMaterial) { matProps = block };

        foreach (var gaussian in _animation.Frames[frame])
        {
            //render
            Vector3 position = gaussian.Position + gaussianPosition;
            if (hideSky && IsFloater(gaussian, position))
            {
                continue;
            }

            Matrix4x4 matrix = Matrix4x4.TRS(position, gaussian.Rotation,
                gaussianScale * gaussian.Scale);
            Color color = gaussian.SH2RGB();
            block.SetColor("_Color", color);
            Graphics.RenderMesh(rp, ellipsoidMesh, 0, matrix);
        }
    }

    bool IsFloater(Gaussian gaussian, Vector3 position)
    {
        Color color = gaussian.SH2RGB();
        return (position.y > 0.1 && color.maxColorComponent == color.b && !_collider.bounds.Contains(position)) ||
               position.y > 2.9;
    }

    void MakeTrainTransparent()
    {
        mainTrain.SetActive(!mainTrain.activeSelf);
        transparentTrain.SetActive(!transparentTrain.activeSelf);
    }
}