using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject renders;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            renders.SetActive(!renders.activeSelf);
        }
    }
}