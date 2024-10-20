using System.Collections;
using System.Collections.Generic;
using UnityEngine.Android;
using UnityEngine;
using UnityEngine.UI;

public class DeviceCamera : MonoBehaviour
{
    ChatGPT chatGPT;
    WebCamTexture camTexture;

    public RawImage cameraViewImage; //카메라가 보여질 화면
    [Header("Setting")]
    public Vector2 requestedRatio; //설정하고자 하는 카메라 비율
    public int requestedFPS; //설정하고자 하는 프레임
    private void Start()
    {
        chatGPT = GetComponent<ChatGPT>();

    }
    private void Update()
    {
        UpdateWebCamRawImage();
    }
    private void OnEnable()
    {
        CameraOn();
    }
    private void OnDisable()
    {
        CameraOff();
    }

    public void CameraOn() //카메라 켜기
    {
        cameraViewImage.gameObject.SetActive(true);
        //카메라 권한 확인
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        if (WebCamTexture.devices.Length == 0) //카메라가 없다면..
        {
            Debug.Log("no camera!");
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices; //스마트폰의 카메라 정보를 모두 가져옴.
        int selectedCameraIndex = -1;
        //후면 카메라 찾기
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                selectedCameraIndex = i;
                break;
            }
        }
        //카메라 켜기
        if (selectedCameraIndex >= 0)
        {
            //선택된 후면 카메라를 가져옴.
            int requestedWidth = Screen.width; //설정하고자 하는 가로 픽셀 변수 선언 (현재 화면의 가로 픽셀을 기본값으로 지정)
            int requestedHeight = Screen.height; //설정하고자 하는 세로 픽셀 변수 선언 (현재 화면의 세로 픽셀을 기본값으로 지정)
            camTexture = new WebCamTexture(devices[selectedCameraIndex].name, requestedWidth, requestedHeight, requestedFPS); //카메라 이름으로 WebCamTexture 생성

            camTexture.filterMode = FilterMode.Point;
            camTexture.Play(); //카메라 재생
            cameraViewImage.texture = camTexture; //RawImage에 할당하기

            //camTexture = new WebCamTexture(devices[selectedCameraIndex].name);

            //camTexture.requestedFPS = 60; //카메라 프레임설정

            //cameraViewImage.texture = camTexture; //영상을 raw Image에 할당.

            //camTexture.Play(); // 카메라 시작하기
        }

    }
    private void UpdateWebCamRawImage() //RawImage를 관리하는 함수
    {
        if (!camTexture) 
            return; //WebCamTexture가 존재하지 않으면 종료

        int videoRotAngle = camTexture.videoRotationAngle;
        cameraViewImage.transform.localEulerAngles = new Vector3(0, 0, -videoRotAngle); //카메라 회전 각도를 반영

        int width, height;
        if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) //세로 화면이면
        {
            width = Screen.width; //가로를 고정하고
            height = Screen.width * camTexture.width / camTexture.height; //WebCamTexture의 비율에 따라 세로를 조절
        }
        else //가로 화면이면
        {
            height = Screen.height; //세로를 고정하고
            width = Screen.height * camTexture.width / camTexture.height; //WebCamTexture의 비율에 따라 가로를 조절
        }

        if (Mathf.Abs(videoRotAngle) % 180 != 0f) Swap(ref width, ref height); //WebCamTexture 자체가 회전되어있는 경우 가로/세로 값을 교환
        cameraViewImage.rectTransform.sizeDelta = new Vector2(width, height); //RawImage의 size로 지정
        
    }
    private void Swap<T>(ref T a, ref T b) //두 변수값을 교환하는 함수
    {
        T tmp = a;
        a = b;
        b = tmp;
    }
    private string GetAspectRatio(int width, int height, bool allowPortrait = false) //비율을 반환하는 함수
    {
        if (!allowPortrait && width < height) Swap(ref width, ref height); //세로가 허용되지 않는데, (가로 < 세로)이면 변수값 교환
        float r = (float)width / height; //비율 저장
        return r.ToString("F2"); //소수점 둘째까지 잘라서 문자열로 반환
    }
    public void CameraOff() //카메라 끄기
    {
        cameraViewImage.gameObject.SetActive(false);
        if (camTexture != null)
        {
            camTexture.Stop(); //카메라 정지
            WebCamTexture.Destroy(camTexture); //카메라 객체반납
            camTexture = null; //변수 초기화
        }
    }
    public void CaptureToVision()
    {
        Texture2D tex = new Texture2D(camTexture.width, camTexture.height);
        tex.SetPixels(camTexture.GetPixels());
        tex.Apply();
        chatGPT.textureSample = tex;
        chatGPT.Send2DTexture();
    }


    //private void Start()
    //{
    //    SetWebCam();
    //}
    //[Header("Setting")]
    //public Vector2 requestedRatio; //설정하고자 하는 카메라 비율
    //public int requestedFPS; //설정하고자 하는 프레임

    //[Header("Component")]
    //public RawImage webCamRawImage; //Canvas에 올려둔 RawImage

    //[Header("Data")]
    //public WebCamTexture webCamTexture; //Camera 화면을 지닌 Texture
    //private void SetWebCam() //WebCam을 지정하는 함수
    //{
    //    if (Permission.HasUserAuthorizedPermission(Permission.Camera)) //이미 카메라 권한을 획득한 경우에는
    //        CreateWebCamTexture(); //WebCamTexture를 생성하는 함수 바로 실행
    //    else //카메라 권한이 없는 경우에는
    //    {
    //        PermissionCallbacks permissionCallbacks = new();
    //        permissionCallbacks.PermissionGranted += CreateWebCamTexture; //권한 획득 후 실행될 함수로 WebCamTexture를 생성하는 함수 추가
    //        Permission.RequestUserPermission(Permission.Camera, permissionCallbacks); //카메라 권한을 요청
    //    }
    //}
    //private void CreateWebCamTexture(string permissionName = null) //WebCamTexture를 생성하는 함수
    //{
    //    if (webCamTexture) //이미 WebCamTexture가 존재하는 경우에는
    //    {
    //        Destroy(webCamTexture); //삭제한다 (메모리 관리)
    //        webCamTexture = null;
    //    }
    //    WebCamDevice[] webCamDevices = WebCamTexture.devices; //현재 접근할 수 있는 카메라 기기들을 모두 가져온다
    //    Debug.Log(webCamDevices.Length);
    //    if (Application.platform != RuntimePlatform.WindowsEditor)
    //    {
    //        if (webCamDevices.Length == 0)
    //            return; //만약, 카메라가 없다면 종료
    //    }


    //    int backCamIndex = -1; //후방 카메라를 저장하기 위한 변수
    //    for (int i = 0, l = webCamDevices.Length; i < l; ++i) //카메라를 탐색하면서
    //    {
    //        if (!webCamDevices[i].isFrontFacing) //후방 카메라를 발견하면
    //        {
    //            backCamIndex = i; //인덱스 저장
    //            break; //반복문 빠져나오기
    //        }
    //    }
    //    if (backCamIndex != -1) //후방 카메라를 발견했으면
    //    {
    //        int requestedWidth = Screen.width; //설정하고자 하는 가로 픽셀 변수 선언 (현재 화면의 가로 픽셀을 기본값으로 지정)
    //        int requestedHeight = Screen.height; //설정하고자 하는 세로 픽셀 변수 선언 (현재 화면의 세로 픽셀을 기본값으로 지정)
    //        for (int i = 0, l = webCamDevices[backCamIndex].availableResolutions.Length; i < l; ++i) //현재 선택된 후방 카메라가 활용할 수 있는 해상도를 탐색하면서
    //        {
    //            Resolution resolution = webCamDevices[backCamIndex].availableResolutions[i];
    //            if (GetAspectRatio((int)requestedRatio.x, (int)requestedRatio.y).Equals(GetAspectRatio(resolution.width, resolution.height))) //설정하고자 하는 비율과 일치하는 해상도를 발견하면
    //            {
    //                requestedWidth = resolution.width; //설정하고자 하는 가로 픽셀로 지정
    //                requestedHeight = resolution.height; //설정하고자 하는 세로 픽셀로 지정
    //                break; //반복문 빠져나오기
    //            }
    //        }

    //        webCamTexture = new WebCamTexture(webCamDevices[backCamIndex].name, requestedWidth, requestedHeight, requestedFPS); //카메라 이름으로 WebCamTexture 생성
    //        webCamTexture.filterMode = FilterMode.Trilinear;
    //        webCamTexture.Play(); //카메라 재생
    //        webCamRawImage.texture = webCamTexture; //RawImage에 할당하기
    //    }
    //}
    //private string GetAspectRatio(int width, int height, bool allowPortrait = false) //비율을 반환하는 함수
    //{
    //    if (!allowPortrait && width < height) Swap(ref width, ref height); //세로가 허용되지 않는데, (가로 < 세로)이면 변수값 교환
    //    float r = (float)width / height; //비율 저장
    //    return r.ToString("F2"); //소수점 둘째까지 잘라서 문자열로 반환
    //}
    //private void Swap<T>(ref T a, ref T b) //두 변수값을 교환하는 함수
    //{
    //    T tmp = a;
    //    a = b;
    //    b = tmp;
    //}
    //private void Update()
    //{
    //    UpdateWebCamRawImage();
    //}
    //private void UpdateWebCamRawImage() //RawImage를 관리하는 함수
    //{
    //    if (!webCamTexture) return; //WebCamTexture가 존재하지 않으면 종료

    //    int videoRotAngle = webCamTexture.videoRotationAngle;
    //    webCamRawImage.transform.localEulerAngles = new Vector3(0, 0, -videoRotAngle); //카메라 회전 각도를 반영

    //    int width, height;
    //    if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) //세로 화면이면
    //    {
    //        width = Screen.width; //가로를 고정하고
    //        height = Screen.width * webCamTexture.width / webCamTexture.height; //WebCamTexture의 비율에 따라 세로를 조절
    //    }
    //    else //가로 화면이면
    //    {
    //        height = Screen.height; //세로를 고정하고
    //        width = Screen.height * webCamTexture.width / webCamTexture.height; //WebCamTexture의 비율에 따라 가로를 조절
    //    }

    //    if (Mathf.Abs(videoRotAngle) % 180 != 0f) Swap(ref width, ref height); //WebCamTexture 자체가 회전되어있는 경우 가로/세로 값을 교환
    //    webCamRawImage.rectTransform.sizeDelta = new Vector2(width, height); //RawImage의 size로 지정
    //}
}
