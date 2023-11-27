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

    [SerializeField] private Vector3 gaussianPosition = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private Vector3 gaussianRotation = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private float gaussianScale = 1.0f;

    [SerializeField] private bool hideSky = false;

    [SerializeField] private GameObject train;
    [SerializeField] private Material trainMaterial;
    [SerializeField] private List<Material> _materials;

    [SerializeField] private Collider _collider;
    private bool isTransparent = false;

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

                            Gaussian gaussian = new Gaussian(position, scale, rotation, opacity, colorFeatures);
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            MakeTrainTransparent();
        }

        RenderParams rp = new RenderParams(ellipsoidMaterial) { matProps = block };

        Quaternion rotation = Quaternion.Euler(gaussianRotation);

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
            block.SetColor("_Color", gaussian.SH2RGB());
            Graphics.RenderMesh(rp, ellipsoidMesh, 0, matrix);
        }
    }

    bool IsFloater(Gaussian gaussian, Vector3 position)
    {
        Color color = gaussian.SH2RGB();
        return (position.y > 0.1 && color.maxColorComponent == color.b && !_collider.bounds.Contains(position)) || position.y > 2.9;
    }

    void MakeTrainTransparent()
    {
        //CONT From this doesn't work
        isTransparent = !isTransparent;
        int mode = isTransparent ? 3 : 0;

        Debug.Log("Making Transparent");
        // MeshRenderer renderer = train.GetComponent<MeshRenderer>();
        // for (int i = 0; i < renderer.materials.Length; i++)
        // {
        //     if (isTransparent)
        //     {
        //         MakeTransparent(renderer.materials[i]);
        //     }
        //     else
        //     {
        //         MakeOpaque(renderer.materials[i]);
        //     }
        // }
        foreach (var material in _materials)
        {
            if (isTransparent)
            {
                MakeTransparent(material);
            }
            else
            {
                MakeOpaque(material);
            }
        }
    }

    void MakeTransparent(Material material)
    {
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }

    void MakeOpaque(Material material)
    {
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
        material.SetFloat("_Mode", 0.0f);
    }
}