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

    public GameObject rectPrefab;

    GameObject createPrefab;

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
                        // QR �ڵ��� ��ġ ��ǥ ���
                        ResultPoint[] points = result.ResultPoints;
                        
                        lastQRCodeContent = result.Text;
                        provisioningCanvas.SetActive(true);

                        // QR �ڵ��� 4�� �𼭸� ��ǥ�� ����Ͽ� �ð��� ������Ʈ ����
                        ResultPoint fourthPoint = CalculateFourthPoint(points[0], points[1], points[2]);

                        // ����Ʈ �迭�� �� ��° ����Ʈ �߰�
                        ResultPoint[] fourPoints = new ResultPoint[4] { points[0], points[1], points[2], fourthPoint };

                        DrawRectangle(fourPoints);

                        Debug.Log($"KKS QR �ν� : {result.Text}");

                        provisioningCanvas.transform.rotation = Camera.main.transform.rotation;
                        provisioningCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "��ǰ �ڵ� : C-A-0012\n" +
                            "���� ��뷮 : 1000w\n" +
                            "�� ��뷮 : 3500kWh";
                    }
                }
            }
        }
    }
    ResultPoint CalculateFourthPoint(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft)
    {
        // �밢�� ���͸� ����ϰ� �� ��° ���� ����
        float dx1 = topRight.X - topLeft.X;
        float dy1 = topRight.Y - topLeft.Y;
        float dx2 = bottomLeft.X - topLeft.X;
        float dy2 = bottomLeft.Y - topLeft.Y;

        float fourthX = bottomLeft.X + dx1;
        float fourthY = bottomLeft.Y + dy1;

        return new ResultPoint(fourthX, fourthY);
    }

    void DrawRectangle(ResultPoint[] points)
    {
        Camera arCamera = CameraManager.GetComponent<Camera>();

        // Convert the QR code points to world space
        Vector3 bottomLeft = arCamera.ScreenToWorldPoint(new Vector3(points[0].X, points[0].Y, arCamera.nearClipPlane));
        Vector3 topLeft = arCamera.ScreenToWorldPoint(new Vector3(points[1].X, points[1].Y, arCamera.nearClipPlane));
        Vector3 topRight = arCamera.ScreenToWorldPoint(new Vector3(points[2].X, points[2].Y, arCamera.nearClipPlane));
        Vector3 bottomRight = arCamera.ScreenToWorldPoint(new Vector3(points[3].X, points[3].Y, arCamera.nearClipPlane));

        // Calculate the center position and size for the rectangle
        Vector3 centerPosition = (bottomLeft + topRight) / 2;
        Vector3 size = new Vector3(Vector3.Distance(bottomLeft, bottomRight), Vector3.Distance(bottomLeft, topLeft), 0.1f);
        provisioningCanvas.transform.position = new Vector3(centerPosition.x, centerPosition.y, centerPosition.z);

        // Instantiate the rectangle in the AR environment
        createPrefab = Instantiate(rectPrefab, centerPosition, Quaternion.identity);
        createPrefab.transform.position = new Vector3(centerPosition.x, centerPosition.y, Vector3.Distance(Camera.main.transform.position, centerPosition)+0.1f);
        createPrefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.01f);
        createPrefab.transform.rotation = Quaternion.Euler(90, 0, 0);

        GetComponent<CreatePlaceObject>().testText.text = $"centerPosition : {centerPosition}\n" +
            $"createPrefab.position : {createPrefab.transform.position}\n" +
            $"provisioningCanvas.position : {provisioningCanvas.transform.position}";
    }

    public void ProvisioningCanvasSetInVisible()
    {
        provisioningCanvas.SetActive(false);
        lastQRCodeContent = string.Empty;
        rectPrefab.SetActive(false);
        if(createPrefab != null)
        {
            createPrefab.SetActive(false);
        }
    }
}
