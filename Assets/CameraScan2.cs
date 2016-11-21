using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

using Vuforia;
using ZXing;

//using System.Threading;
//using ZXing.QrCode;
//using ZXing.Common;


//[AddComponentMenu("System/VuforiaScanner")]
public class CameraScan2 : MonoBehaviour
{
    public Text debugText1, debugText2, debugText3;

    private bool cameraInitialized;
    private BarcodeReader barCodeReader;

    void Start()
    {
        barCodeReader = new BarcodeReader();
        barCodeReader.AutoRotate = true;
        barCodeReader.TryHarder = false;

        StartCoroutine(InitializeCamera());
        DebugText("Program Started !", 1);

        InvokeRepeating("FocusTrigger", 2f, 2f);
    }

    private IEnumerator InitializeCamera()
    //private void InitializeCamera()
    {
        // Waiting a little seem to avoid the Vuforia's crashes.
        yield return new WaitForSeconds(1.25f);

        //var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(Vuforia.Image.PIXEL_FORMAT.RGB888, true);
        var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(Vuforia.Image.PIXEL_FORMAT.GRAYSCALE, true);
        Debug.Log(String.Format("FormatSet : {0}", isFrameFormatSet));

        // Force autofocus.
        var isAutoFocus = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        if (!isAutoFocus)
        {
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
        }
        Debug.Log(String.Format("AutoFocus : {0}", isAutoFocus));
        cameraInitialized = true;
    }

    void DebugText(string msg, int i)
    {
        switch(i)
        {
            case 1:
                debugText1.text = msg;
                break;
            case 2:
                debugText2.text = "Height : " + msg;
                break;
            case 3:
                debugText3.text = "Width : " + msg;
                break;
        }
    }

    void FocusTrigger()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
    }

    public void DecodeScan()
    {
        if (cameraInitialized)
        {
            try
            {
                //var cameraFeed = CameraDevice.Instance.GetCameraImage(Vuforia.Image.PIXEL_FORMAT.RGB888);
                var cameraFeed = CameraDevice.Instance.GetCameraImage(Vuforia.Image.PIXEL_FORMAT.GRAYSCALE);
                if (cameraFeed == null)
                {
                    return;
                }

                //var data = barCodeReader.Decode(cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight, RGBLuminanceSource.BitmapFormat.RGB24);
                var data = barCodeReader.Decode(cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight, RGBLuminanceSource.BitmapFormat.Gray8);
                //var data = barCodeReader.Decode(cameraFeed.Pixels, 1, 1, RGBLuminanceSource.BitmapFormat.Gray8);
                DebugText(cameraFeed.BufferHeight.ToString(), 2);
                DebugText(cameraFeed.BufferWidth.ToString(), 3);

                if (data != null)
                {
                    // QRCode detected.
                    Debug.Log(data.Text);
                    DebugText(data.Text, 1);
                }
                else
                {
                    Debug.Log("No result : Try again");
                    DebugText("No Result : Try again", 1);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    public void HardMode(bool flag)
    {
        if(flag)
        {
            barCodeReader.TryHarder = true;
            DebugText("On", 2);
        }
            
        else
        {
            barCodeReader.TryHarder = false;
            DebugText("Off", 2);
        }
           
    }

    private void Update()
    {
        if (Input.GetKey("escape"))
            Application.Quit();


        //if (Input.touchCount != 0)
        //CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
    }

}