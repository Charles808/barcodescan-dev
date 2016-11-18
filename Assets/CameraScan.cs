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
public class CameraScan : MonoBehaviour
{    
	public Text debugText;

	private bool cameraInitialized;
	private BarcodeReader barCodeReader;

	void Start()
	{        
		barCodeReader = new BarcodeReader();
		StartCoroutine(InitializeCamera());
		DebugText("Program Started !");
	}

	private IEnumerator InitializeCamera()
	//private void InitializeCamera()
	{
		// Waiting a little seem to avoid the Vuforia's crashes.
		yield return new WaitForSeconds(1.25f);

		var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(Vuforia.Image.PIXEL_FORMAT.RGB888, true);
		//var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(Vuforia.Image.PIXEL_FORMAT.GRAYSCALE, true);
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

	void DebugText (string msg) {
		debugText.text = msg;
	}

    public void DecodeScan()
    {
        if (cameraInitialized)
        {
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
            try
            {
                var cameraFeed = CameraDevice.Instance.GetCameraImage(Vuforia.Image.PIXEL_FORMAT.RGB888);
                //var cameraFeed = CameraDevice.Instance.GetCameraImage(Vuforia.Image.PIXEL_FORMAT.GRAYSCALE);
                if (cameraFeed == null)
                {
                    return;
                }
                var data = barCodeReader.Decode(cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight, RGBLuminanceSource.BitmapFormat.RGB24);
                if (data != null)
                {
                    // QRCode detected.
                    Debug.Log(data.Text);
                    DebugText(data.Text);
                }
                else
                {
                    Debug.Log("No QR code detected !");
                    DebugText("No QR Code Detected !");
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

    private void Update()
    {
        if (Input.GetKey("escape"))
            Application.Quit();
    }

}