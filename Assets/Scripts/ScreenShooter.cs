using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class ScreenShooter : MonoBehaviour
    {
        [SerializeField] private String saveDir;
        private int frameCounter = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("Screenshot");
                ScreenCapture.CaptureScreenshot(saveDir + "/" + frameCounter + ".png");
                frameCounter++;
            }
        }
    }
}