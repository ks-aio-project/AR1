using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using ZXing.Common;

public class QRCodeScan : MonoBehaviour
{
    public XROrigin xrOrigin;
    public ARCameraManager cameraManager;
    public ARRaycastManager arRaycastManager;
    public GameObject provisioningCanvas;

    string lastQRCodeContent;

    public GameObject rectPrefab;
    public GameObject GPM_webView;
    GameObject createPrefab;

    void Update()
    {
        if(!provisioningCanvas.activeSelf)
        {
            if (cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
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
                        Debug.Log("QR Code detected: " + result.Text);
                        lastQRCodeContent = result.Text;

                        // QR 코드의 3개의 코너 좌표 가져오기
                        var points = result.ResultPoints;
                        if (points.Length == 3)
                        {
                            Vector2[] imagePoints = new Vector2[3];
                            for (int i = 0; i < points.Length; i++)
                            {
                                // ZXing의 좌표를 Unity에서 사용할 수 있는 Vector2로 변환
                                imagePoints[i] = new Vector2(points[i].X, points[i].Y);
                            }

                            Vector3 qrWorldPosition = CalculateQrCodeWorldPosition(result.ResultPoints);
                            MoveCameraToWorldLocation(qrWorldPosition);

                            // 각 점을 3D 공간으로 매핑
                            Vector3[] worldPoints = new Vector3[3];
                            for (int i = 0; i < imagePoints.Length; i++)
                            {
                                // 화면 공간 좌표를 3D 월드 좌표로 변환
                                var screenPoint = new Vector3(imagePoints[i].x, imagePoints[i].y, cameraManager.GetComponent<Camera>().nearClipPlane);
                                worldPoints[i] = cameraManager.GetComponent<Camera>().ScreenToWorldPoint(screenPoint);
                            }

                            // 3D 공간에 QR 코드의 중심에 오브젝트를 배치
                            Vector3 qrCenter = (worldPoints[0] + worldPoints[1] + worldPoints[2]) / 3;
                            //if (createPrefab == null)
                            //{
                            //    createPrefab = Instantiate(rectPrefab, qrCenter + new Vector3(0.05f, -0.05f, 0), Quaternion.identity);
                            //    createPrefab.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                            //}
                            //else
                            //{
                            //    createPrefab.SetActive(true);
                            //    createPrefab.transform.position = qrCenter + new Vector3(0.05f, -0.05f, 0);
                            //}

                            provisioningCanvas.SetActive(true);

                            Debug.Log($"KKS QR 인식 : {result.Text}");

                            provisioningCanvas.transform.position = qrCenter + new Vector3(0.15f, -0.15f, 0);
                            provisioningCanvas.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

                            provisioningCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "제품 코드 : C-A-0012\n" +
                                "전력 사용량 : 1000w\n" +
                                "총 사용량 : 3500kWh";

                            if(GPM_webView != null)
                            {
                                //GPM_webView.GetComponent<WebviewConnect>().ShowUrlPopupDefault("https://ks-aio-project.github.io/");
                                GPM_webView.GetComponent<WebviewConnect>().ShowUrlPopupPositionSize("https://ks-aio-project.github.io/", 0, 0, 0.5f, 0.5f);
                            }
                        }

                        //Debug.Log("kks QR코드 인식");

                        //provisioningCanvas.SetActive(true);

                        //Debug.Log($"KKS QR 인식 : {result.Text}");

                        //provisioningCanvas.transform.rotation = Quaternion.Euler(90, 0, 0);
                        //Debug.Log($"kks rotation");
                        //provisioningCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "제품 코드 : C-A-0012\n" +
                        //    "전력 사용량 : 1000w\n" +
                        //    "총 사용량 : 3500kWh";
                        //Debug.Log($"kks set text");
                    }
                }
            }
        }
    }

    Vector3 CalculateQrCodeWorldPosition(ResultPoint[] points)
    {
        Vector3 screenCenter = new Vector3(points[0].X, points[0].Y, cameraManager.GetComponent<Camera>().nearClipPlane);
        return cameraManager.GetComponent<Camera>().ScreenToWorldPoint(screenCenter);
    }

    void MoveCameraToWorldLocation(Vector3 desiredWorldLocation)
    {
        // 이 함수는 카메라 위치를 이동시키기 위해 XR Origin을 조정합니다.
        if (xrOrigin != null)
        {
            Vector3 cameraOffset = xrOrigin.Camera.transform.position - xrOrigin.transform.position;
            xrOrigin.transform.position = desiredWorldLocation - cameraOffset;
        }
    }

    public void ProvisioningCanvasSetInVisible()
    {
        provisioningCanvas.SetActive(false);
        lastQRCodeContent = string.Empty;
        createPrefab.SetActive(false);
    }
}
