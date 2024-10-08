using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using ZXing.Common;
using Toggle = UnityEngine.UI.Toggle;

public class QRCodeScan : MonoBehaviour
{
    public XROrigin xrOrigin;
    public ARCameraManager cameraManager;
    public ARRaycastManager arRaycastManager;
    public ARPlaneManager arPlaneManager;
    public ARAnchorManager arAnchorManager;
    public GameObject provisioningCanvas;

    string lastQRCodeContent;

    public GameObject rectPrefab;
    public GameObject GPM_webView;
    GameObject createPrefab;

    public Toggle PlaneShowToggle;
    private void Start()
    {
        PlaneShowToggle.onValueChanged.AddListener(OnPlaneToggleValueChanged);
    }

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
                        Debug.Log($"KKS QR �ؽ�Ʈ : {result.Text}");
                        lastQRCodeContent = result.Text;

                        GlobalVariable.Instance.last_qrcode = lastQRCodeContent;

                        //// QR �ڵ��� 3���� �ڳ� ��ǥ ��������
                        //var points = result.ResultPoints;
                        //float x = points[0].X;
                        //float y = points[0].Y;

                        //float screenX = x * (Screen.width / (float)image.width);
                        //float screenY = y * (Screen.height / (float)image.height);

                        //// ��ũ�� ����Ʈ ����
                        //Vector2 screenPoint = new Vector2(screenX, screenY);

                        //List<ARRaycastHit> hits = new List<ARRaycastHit>();

                        //if (arRaycastManager.Raycast(screenPoint, hits, TrackableType.Planes))
                        //{
                        //    if (createPrefab != null)
                        //    {
                        //        Destroy(createPrefab);
                        //    }
                        //    GameObject obj;
                        //    ARAnchor anchor;
                        //    Pose hitPose = hits[0].pose;

                        //    Vector3 qrCodePosition = hitPose.position;
                        //    Quaternion qrCodeRoation = hitPose.rotation;
                        //    switch (result.Text)
                        //    {
                        //        case "box":
                        //            obj = Instantiate(GetComponent<TrackedImageInfomation1>().arObjectPrefab[2]);
                        //            anchor = obj.AddComponent<ARAnchor>();
                        //            createPrefab = obj;

                        //            // A���� B(TrackedImage)�� ���ϴ� ���� ���
                        //            if (GetComponent<TrackedImageInfomation1>().currentForwardObject == null)
                        //            {
                        //                return;
                        //            }

                        //            Vector3 directionToB = qrCodePosition - GetComponent<TrackedImageInfomation1>().currentForwardObject.transform.position;
                        //            directionToB.Normalize();  // ���� ���� ����ȭ

                        //            // A�� forward ����
                        //            Vector3 forwardA = GetComponent<TrackedImageInfomation1>().currentForwardObject.transform.forward;

                        //            // �ﰢ�Լ��� ����� ���� ��� (Cosine ��Ģ)
                        //            float dotProduct = Vector3.Dot(forwardA, directionToB);  // ���� ���
                        //            float magnitudeA = forwardA.magnitude;
                        //            float magnitudeB = directionToB.magnitude;
                        //            float cosTheta = dotProduct / (magnitudeA * magnitudeB);  // ���� ���ϱ� ���� Cosine
                        //            float angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;  // ���ȿ��� ������ ��ȯ

                        //            // ��ȣ�� �����ϱ� ���� ���� (Cross Product)
                        //            Vector3 cross = Vector3.Cross(forwardA, directionToB);
                        //            if (cross.y < 0)
                        //            {
                        //                angle = -angle;  // ���� ���̸� �ð� �������� ȸ���� ����
                        //            }

                        //            //// 360�� ������ ����
                        //            //if (angle < 0)
                        //            //{
                        //            //    angle += 360;
                        //            //}

                        //            // 90���� ���� ó��
                        //            //float snappedAngle = Mathf.Round(angle / 90) * 90;

                        //            // ���� ī�޶� Y�� ȸ�� ������ �ﰢ�Լ��� ���� ���
                        //            float cameraYRotation = Camera.main.transform.eulerAngles.y;

                        //            // 1. objs[0]���� TrackedImage�� ���ϴ� ����
                        //            Vector3 directionFromObjToTracked = qrCodePosition - GetComponent<TrackedImageInfomation1>().currentForwardObject.transform.position;
                        //            directionFromObjToTracked.Normalize();

                        //            // 2. ī�޶󿡼� TrackedImage�� ���ϴ� ����
                        //            Vector3 directionFromCameraToTracked = qrCodePosition - Camera.main.transform.position;
                        //            directionFromCameraToTracked.Normalize();

                        //            // 3. �� ���� ������ ���� ��� (���� ���)
                        //            dotProduct = Vector3.Dot(directionFromObjToTracked, directionFromCameraToTracked);
                        //            float angleBetweenObjAndCamera = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;  // ���� -> ���� ��ȯ

                        //            Debug.Log($"kks angleBetweenObjAndCamera : {angleBetweenObjAndCamera}");

                        //            // ������ ����� ��ȣ ���� (�ð�/�ݽð�)
                        //            Vector3 crossProduct = Vector3.Cross(directionFromObjToTracked, directionFromCameraToTracked);
                        //            if (crossProduct.y < 0)
                        //            {
                        //                angleBetweenObjAndCamera = -angleBetweenObjAndCamera;  // �ð� ������ �� ������ ����
                        //            }

                        //            float rotationResult = 0;

                        //            if (cameraYRotation < 90)
                        //            {
                        //                rotationResult = 90f - cameraYRotation;
                        //            }
                        //            else if (cameraYRotation < 180)
                        //            {
                        //                rotationResult = 180f - cameraYRotation;
                        //            }
                        //            else if (cameraYRotation < 270)
                        //            {
                        //                rotationResult = 270f - cameraYRotation;
                        //            }
                        //            else
                        //            {
                        //                rotationResult = 360f - cameraYRotation;
                        //            }

                        //            Debug.Log($"kks result : {result}");

                        //            // ������Ʈ ��ġ ���� (ī�޶� �������� 2m)
                        //            obj.transform.position = qrCodePosition + new Vector3(0, 0, 2f);
                        //            anchor.transform.position = obj.transform.position;
                        //            obj.transform.parent = anchor.transform;

                        //            // ������Ʈ�� �����̼� ����
                        //            if (rotationResult > 50 && angleBetweenObjAndCamera < 50)
                        //            {
                        //                obj.transform.rotation = Quaternion.Euler(0f, angle - rotationResult, 0f);
                        //            }
                        //            else if (rotationResult > 50 && angleBetweenObjAndCamera > 50)
                        //            {
                        //                obj.transform.rotation = Quaternion.Euler(0f, angle + 90 - rotationResult, 0f);
                        //            }
                        //            else
                        //            {
                        //                obj.transform.rotation = Quaternion.Euler(0f, angle + 270 - rotationResult, 0f);
                        //            }
                        //            obj.AddComponent<UpdatePosition>(); // ������ ��ũ��Ʈ
                        //            if (anchor != null)
                        //            {
                        //                //obj.transform.parent = anchor.transform;
                        //                Debug.Log($"KKS Anchor OK : {anchor.transform.position}");
                        //                Debug.Log($"KKS obj position : {obj.transform.position}");
                        //            }
                        //            else
                        //            {
                        //                Debug.Log("KKS Anchor NO");
                        //            }

                        //            break;

                        //        default:
                        //            provisioningCanvas.SetActive(true);
                        //            provisioningCanvas.transform.position = qrCodePosition + new Vector3(-0.01f, 0.01f, 0);
                        //            provisioningCanvas.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

                        //            provisioningCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "��ǰ �ڵ� : C-A-0012\n" +
                        //                "���� ��뷮 : 1000w\n" +
                        //                "�� ��뷮 : 3500kWh\n" +
                        //                $"������ : {qrCodePosition}";

                        //            if (GPM_webView != null)
                        //            {
                        //                //GPM_webView.GetComponent<WebviewConnect>().ShowUrlPopupDefault("https://ks-aio-project.github.io/");
                        //                GPM_webView.GetComponent<WebviewConnect>().ShowUrlPopupPositionSize("https://ks-aio-project.github.io/", 0, 0, 0.5f, 0.5f);
                        //            }
                        //            break;
                        //    }
                        //}
                    }
                }
            }
        }
    }

    // Toggle ���� ����� �� ����Ǵ� �Լ�
    void OnPlaneToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            ShowPlaneVisualizers();
        }
        else
        {
            HidePlaneVisualizers();
        }
    }

    void HidePlaneVisualizers()
    {
        arPlaneManager.planePrefab.GetComponent<ARPlaneMeshVisualizer>().enabled = false;
        arPlaneManager.planePrefab.GetComponent<MeshRenderer>().enabled = false;

        // ARPlaneManager�� �����ϴ� ��� ���鿡 ����
        foreach (var plane in arPlaneManager.trackables)
        {
            var visualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            if (visualizer != null)
            {
                visualizer.enabled = false; // �ð�ȭ ��Ȱ��ȭ
            }

            var meshRenderer = plane.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false; // MeshRenderer�� ����
            }
        }
    }

    void ShowPlaneVisualizers()
    {
        arPlaneManager.planePrefab.GetComponent<ARPlaneMeshVisualizer>().enabled = true;
        arPlaneManager.planePrefab.GetComponent<MeshRenderer>().enabled = true;

        // ARPlaneManager�� �����ϴ� ��� ���鿡 ����
        foreach (var plane in arPlaneManager.trackables)
        {
            var visualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            if (visualizer != null)
            {
                visualizer.enabled = true; // �ð�ȭ ��Ȱ��ȭ
            }

            var meshRenderer = plane.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true; // MeshRenderer�� ����
            }
        }
    }

    public void ProvisioningCanvasSetInVisible()
    {
        provisioningCanvas.SetActive(false);
        lastQRCodeContent = string.Empty;
        createPrefab.SetActive(false);
    }
}