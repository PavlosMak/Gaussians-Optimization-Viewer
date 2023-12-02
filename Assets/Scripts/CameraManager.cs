using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private string PathToCameras =
        "/home/pavlos/Desktop/stuff/Uni-Masters/Q5/GraphicsSeminar/train_smaller_frames/cameras.csv";

    [SerializeField] private GameObject cubePrefab;
    private List<Camera> _cameras;

    private List<Camera> ReadCameras(string filePath)
    {
        List<Camera> cameras = new List<Camera>();
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
                            Debug.Log("Camera Line: ");
                            int id = int.Parse(fields[1]);
                            float fx = float.Parse(fields[2]);
                            float fy = float.Parse(fields[3]);
                            int height = int.Parse(fields[4]);
                            int width = int.Parse(fields[5]);
                            string img_name = fields[6];
                            Vector3 position = new Vector3(
                                float.Parse(fields[7]), float.Parse(fields[8]), float.Parse(fields[9])
                            );
                            Quaternion rotation = new Quaternion();
                            Camera camera = new Camera(id, img_name, width, height, position, rotation, fy, fx);
                            cameras.Add(camera);
                        }
                    }
                }

                Debug.Log("Cameras read!");
                Debug.Log(cameras.Count);
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

        return cameras;
    }


    // Start is called before the first frame update
    void Start()
    {
        _cameras = ReadCameras(PathToCameras);
        // string jsonString = File.ReadAllText(PathToCameras);
        // _cameras = JsonUtility.FromJson<CameraList>(jsonString).cameraList;
        // Debug.Log(_cameras.Count);
        foreach (var camera in _cameras)
        {
            Debug.Log("Making Camera " + camera.id);
            Debug.Log("Camera height" + camera.height);
            Debug.Log("Camera width" + camera.width);
            Debug.Log("Camera img name" + camera.img_name);
            GameObject cube = (GameObject)Instantiate(cubePrefab,
                camera.position, camera.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}