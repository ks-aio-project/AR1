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
                        Debug.Log($"KKS QR 텍스트 : {result.Text}");
                        lastQRCodeContent = result.Text;

                        GlobalVariable.Instance.last_qrcode = lastQRCodeContent;

                        //// QR 코드의 3개의 코너 좌표 가져오기
                        //var points = result.ResultPoints;
                        //float x = points[0].X;
                        //float y = points[0].Y;

                        //float screenX = x * (Screen.width / (float)image.width);
                        //float screenY = y * (Screen.height / (float)image.height);

                        //// 스크린 포인트 생성
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

                        //            // A에서 B(TrackedImage)로 향하는 벡터 계산
                        //            if (GetComponent<TrackedImageInfomation1>().currentForwardObject == null)
                        //            {
                        //                return;
                        //            }

                        //            Vector3 directionToB = qrCodePosition - GetComponent<TrackedImageInfomation1>().currentForwardObject.transform.position;
                        //            directionToB.Normalize();  // 방향 벡터 정규화

                        //            // A의 forward 벡터
                        //            Vector3 forwardA = GetComponent<TrackedImageInfomation1>().currentForwardObject.transform.forward;

                        //            // 삼각함수를 사용한 각도 계산 (Cosine 법칙)
                        //            float dotProduct = Vector3.Dot(forwardA, directionToB);  // 내적 계산
                        //            float magnitudeA = forwardA.magnitude;
                        //            float magnitudeB = directionToB.magnitude;
                        //            float cosTheta = dotProduct / (magnitudeA * magnitudeB);  // 각도 구하기 위한 Cosine
                        //            float angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;  // 라디안에서 각도로 변환

                        //            // 부호를 결정하기 위한 외적 (Cross Product)
                        //            Vector3 cross = Vector3.Cross(forwardA, directionToB);
                        //            if (cross.y < 0)
                        //            {
                        //                angle = -angle;  // 음의 값이면 시계 방향으로 회전한 각도
                        //            }

                        //            //// 360도 범위로 보정
                        //            //if (angle < 0)
                        //            //{
                        //            //    angle += 360;
                        //            //}

                        //            // 90도로 스냅 처리
                        //            //float snappedAngle = Mathf.Round(angle / 90) * 90;

                        //            // 현재 카메라 Y축 회전 각도를 삼각함수를 통해 계산
                        //            float cameraYRotation = Camera.main.transform.eulerAngles.y;

                        //            // 1. objs[0]에서 TrackedImage로 향하는 벡터
                        //            Vector3 directionFromObjToTracked = qrCodePosition - GetComponent<TrackedImageInfomation1>().currentForwardObject.transform.position;
                        //            directionFromObjToTracked.Normalize();

                        //            // 2. 카메라에서 TrackedImage로 향하는 벡터
                        //            Vector3 directionFromCameraToTracked = qrCodePosition - Camera.main.transform.position;
                        //            directionFromCameraToTracked.Normalize();

                        //            // 3. 두 벡터 사이의 각도 계산 (내적 사용)
                        //            dotProduct = Vector3.Dot(directionFromObjToTracked, directionFromCameraToTracked);
                        //            float angleBetweenObjAndCamera = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;  // 라디안 -> 각도 변환

                        //            Debug.Log($"kks angleBetweenObjAndCamera : {angleBetweenObjAndCamera}");

                        //            // 외적을 사용해 부호 결정 (시계/반시계)
                        //            Vector3 crossProduct = Vector3.Cross(directionFromObjToTracked, directionFromCameraToTracked);
                        //            if (crossProduct.y < 0)
                        //            {
                        //                angleBetweenObjAndCamera = -angleBetweenObjAndCamera;  // 시계 방향일 때 음수로 만듦
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

                        //            // 오브젝트 위치 조정 (카메라 앞쪽으로 2m)
                        //            obj.transform.position = qrCodePosition + new Vector3(0, 0, 2f);
                        //            anchor.transform.position = obj.transform.position;
                        //            obj.transform.parent = anchor.transform;

                        //            // 오브젝트의 로테이션 설정
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
                        //            obj.AddComponent<UpdatePosition>(); // 디버깅용 스크립트
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

                        //            provisioningCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "제품 코드 : C-A-0012\n" +
                        //                "전력 사용량 : 1000w\n" +
                        //                "총 사용량 : 3500kWh\n" +
                        //                $"포지션 : {qrCodePosition}";

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

    // Toggle 값이 변경될 때 실행되는 함수
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

        // ARPlaneManager가 관리하는 모든 평면들에 접근
        foreach (var plane in arPlaneManager.trackables)
        {
            var visualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            if (visualizer != null)
            {
                visualizer.enabled = false; // 시각화 비활성화
            }

            var meshRenderer = plane.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false; // MeshRenderer도 끄기
            }
        }
    }

    void ShowPlaneVisualizers()
    {
        arPlaneManager.planePrefab.GetComponent<ARPlaneMeshVisualizer>().enabled = true;
        arPlaneManager.planePrefab.GetComponent<MeshRenderer>().enabled = true;

        // ARPlaneManager가 관리하는 모든 평면들에 접근
        foreach (var plane in arPlaneManager.trackables)
        {
            var visualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            if (visualizer != null)
            {
                visualizer.enabled = true; // 시각화 비활성화
            }

            var meshRenderer = plane.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true; // MeshRenderer도 끄기
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