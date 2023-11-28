using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private string PathToCameras =
        "/home/pavlos/Desktop/stuff/Uni-Masters/Q5/GraphicsSeminar/train_smaller_frames/cameras.json";

    [SerializeField] private GameObject cubePrefab;
    private List<Camera> _cameras;

    // Start is called before the first frame update
    void Start()
    {
        string jsonString = File.ReadAllText(PathToCameras);
        _cameras = JsonUtility.FromJson<CameraList>(jsonString).cameraList;
        Debug.Log(_cameras.Count);
        foreach (var camera in _cameras)
        {
            Debug.Log("Making Camera...");
            InstantiateCube(camera.rotation, camera.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void InstantiateCube(float[][] orientationMatrix, float[] positionVector)
    {
        if (orientationMatrix.Length != 3 || positionVector.Length != 3)
        {
            Debug.LogError("Invalid matrix or vector size");
            return;
        }

        // Create a rotation matrix from the given 2D array
        Vector3[] columns = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            if (orientationMatrix[i].Length != 3)
            {
                Debug.LogError("Invalid matrix size");
                return;
            }

            columns[i] = new Vector3(orientationMatrix[0][i], orientationMatrix[1][i], orientationMatrix[2][i]);
        }

        Matrix4x4 rotationMatrix = new Matrix4x4(columns[0], columns[1], columns[2], new Vector4(0, 0, 0, 1));

        // Instantiate cube at the given position with the calculated rotation
        GameObject cube = (GameObject)Instantiate(cubePrefab,
            new Vector3(positionVector[0], positionVector[1], positionVector[2]), Quaternion.identity);
        cube.transform.rotation = Quaternion.LookRotation(rotationMatrix.GetColumn(2), rotationMatrix.GetColumn(1));
    }
}