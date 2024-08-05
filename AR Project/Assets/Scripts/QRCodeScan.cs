using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using ZXing.Common;

public class QRCodeScan : MonoBehaviour
{
    public ARCameraManager CameraManager;
    public GameObject provisioningCanvas;

    string lastQRCodeContent;

    // Update is called once per frame
    void Update()
    {
        if(!provisioningCanvas.activeSelf)
        {
            if (CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            {
                using (image)
                {
                    var conversionParams = new XRCpuImage.ConversionParams(image, TextureFormat.R8, XRCpuImage.Transformation.MirrorY);
                    var dataSize = image.GetConvertedDataSize(conversionParams);
                    var grayscalePixels = new byte[dataSize];

                    unsafe
                    {
                        fixed (void* ptr = grayscalePixels)
                        {
                            image.Convert(conversionParams, new System.IntPtr(ptr), dataSize);
                        }
                    }

                    IBarcodeReader barcodeReader = new BarcodeReader();
                    var result = barcodeReader.Decode(grayscalePixels, image.width, image.height, RGBLuminanceSource.BitmapFormat.Gray8);

                    if (result != null && lastQRCodeContent != result.Text)
                    {
                        lastQRCodeContent = result.Text;
                        provisioningCanvas.SetActive(true);
                        Debug.Log($"KKS QR 인식 : {result.Text}");

                        provisioningCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "제품 종류 : 에어컨\n" +
                            "제품 코드 : C-A-0012\n" +
                            "전력 사용량 : 1000w\n" +
                            "총 사용량 : 3500kWh";
                    }
                }
            }
        }
    }

    public void ProvisioningCanvasSetInVisible()
    {
        provisioningCanvas.SetActive(false);
        lastQRCodeContent = string.Empty;
    }
}
