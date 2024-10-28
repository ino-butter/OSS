using Cinemachine;
using GoMap;
using GoShared;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class Test : MonoBehaviour
{
    public TextAsset pointTextAsset;
    public Dictionary<string, Dictionary<string, object>> point_table;

    public List<GameObject> pois = new List<GameObject>();

    public LocationManager locationManager;
    public GOMap goMap;
    public Material testLineMaterial;
    public GOUVMappingStyle testStyle;

    public GameObject prefab;
    public Transform worldCanvas;
    private NavMeshAgent agent;

    // 위치 탭
    public RectTransform poiTab;
    public TextMeshProUGUI poiTextField;
    public TextMeshProUGUI poiSupportTextField;
    public Image poiImage;
    public Sprite[] poiSprite;

    public Image gpsButton;
    public ScrollViewTap scrollViewTap;
    public CinemachineFreeLook freeLookCharacter;
    public CinemachineFreeLook freeLookLookAt;
    public Transform character;
    public Transform lookAt;
    public bool isFreeLockCam;
    public string lookAtTarget;
    public string LookATarget
    {
        set
        {
            lookAtTarget = value;
            lookAt.transform.position = new Coordinates(double.Parse(point_table[lookAtTarget]["latitude"].ToString()), 
                double.Parse(point_table[lookAtTarget]["longitude"].ToString())).convertCoordinateToVector();
            IsFreeLockCam = true;
            poiTextField.text = lookAtTarget;
            poiSupportTextField.text = point_table[lookAtTarget]["support"].ToString();
            if (value.CompareTo("대학본관") == 0)
                poiImage.sprite = poiSprite[0];
            else
                poiImage.sprite = poiSprite[1];
            poiTab.DOAnchorPosY(0, 0.1f);
        }
    }
    public bool IsFreeLockCam
    {
        set 
        { 
            isFreeLockCam = value;
            if (isFreeLockCam)
            {
                gpsButton.color = UnityEngine.Color.blue;
                freeLookCharacter.gameObject.SetActive(false);
                freeLookLookAt.gameObject.SetActive(true);
                scrollViewTap.DragDown();
            }
            else
            {
                gpsButton.color = UnityEngine.Color.white;
                freeLookCharacter.gameObject.SetActive(true);
                freeLookLookAt.gameObject.SetActive(false);
                poiTab.DOAnchorPosY(300, 0.1f);
            }
        }
    }


    private void Awake()
    {
        point_table = CSVReader.ReadStringIndex(pointTextAsset);
    }
    //void PathFind(string _start, string _goal)
    //{
    //    string next = path_table[_start][_goal].ToString();
    //    Debug.Log(next);
    //    if (next.CompareTo("x") == 0)
    //    {
    //        Debug.Log(string.Format("find : {0} goal : {1}", next, _goal));
    //    }
    //    else if (next.CompareTo("e") == 0)
    //        return;
    //    else
    //    {
    //        double[] param = new double[4];
    //        param[0] = double.Parse(distance_table[_start]["latitude"].ToString());
    //        param[1] = double.Parse(distance_table[_start]["longitude"].ToString());
    //        param[2] = double.Parse(distance_table[next]["latitude"].ToString());
    //        param[3] = double.Parse(distance_table[next]["longitude"].ToString());
    //        dropTestLine(param);
    //        Debug.Log(string.Format("contiue start : {0} goal : {1}", next, _goal));
    //        PathFind(next, _goal);
    //    }
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            TestNav();
        if (Input.GetKeyDown(KeyCode.F3))
            GeneratePoi();
        MovePoint();
        Orbit();
    }

    void Orbit()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            freeLookCharacter.m_YAxis.m_MaxSpeed = 4;
            freeLookCharacter.m_XAxis.m_MaxSpeed = 500;

            freeLookLookAt.m_YAxis.m_MaxSpeed = 4;
            freeLookLookAt.m_XAxis.m_MaxSpeed = 500;
        }

        if (Input.GetMouseButtonUp(1))
        {
            freeLookCharacter.m_XAxis.m_MaxSpeed = 0;
            freeLookCharacter.m_YAxis.m_MaxSpeed = 0;
            freeLookLookAt.m_XAxis.m_MaxSpeed = 0;
            freeLookLookAt.m_YAxis.m_MaxSpeed = 0;
        }

        //if (Input.touchCount > 0)
        //{
        //    // 첫 번째 터치 가져오기
        //    Touch touch = Input.GetTouch(0);

        //    switch (touch.phase)
        //    {
        //        case TouchPhase.Began:
        //            freeLookCharacter.m_XAxis.m_MaxSpeed = 2;
        //            freeLookCharacter.m_XAxis.m_MaxSpeed = 300;

        //            freeLookLookAt.m_XAxis.m_MaxSpeed = 2;
        //            freeLookLookAt.m_XAxis.m_MaxSpeed = 300;
        //            break;
        //        case TouchPhase.Moved:
        //            break;
        //        case TouchPhase.Ended:
        //            freeLookCharacter.m_XAxis.m_MaxSpeed = 0;

        //            freeLookLookAt.m_XAxis.m_MaxSpeed = 0;
        //            break;
        //    }
        //}
    }

    void TestNav()
    {
        //NavMeshLink navMeshLink = goMap.transform.GetChild(0).GetComponent<NavMeshLink>();
        //navMeshLink.startPoint = new Coordinates(35.120183, 129.103121).convertCoordinateToVector();
        //navMeshLink.endPoint = new Coordinates(35.122774, 129.102742).convertCoordinateToVector();
        agent = prefab.GetComponent<NavMeshAgent>();
        agent.transform.position = new Coordinates(35.120075225830078, 129.10319519042969).convertCoordinateToVector();
        agent.enabled = true;
    }
    void MovePoint()
    {
        if (agent == null)
            return;
        agent.SetDestination(new Coordinates(35.122774, 129.102742).convertCoordinateToVector());
    }
    void GeneratePoi()
    {
        foreach(var point in point_table)
        {
            double latitude = double.Parse(point.Value["latitude"].ToString());
            double longitude = double.Parse(point.Value["longitude"].ToString());
            var dir = new Coordinates(latitude, longitude).convertCoordinateToVector();
            dir.y = 5;
            Vector3 rotate = new Vector3(90, 0, 0);
            GameObject obj = Instantiate(prefab, dir, Quaternion.Euler(rotate), null);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = point.Key;
            obj.transform.SetParent(worldCanvas.transform);
            pois.Add(obj);
        }
    }
    public void DropTestLine(params double[] _value)
    {
        //1) Create a list of coordinates that will represent the polyline
        List<Coordinates> polyline = new List<Coordinates>();
        var location = locationManager.currentLocation;
        location.altitude = 0;
        polyline.Add(new Coordinates(_value[0], _value[1]));
        polyline.Add(new Coordinates(_value[2], _value[3]));


        //2) Set line width
        float width = 3;

        //3) Set the line height
        float height = 3;

        //4) Choose a material for the line (this time we link the material from the inspector)
        Material material = testLineMaterial;

        //5) call drop line
        goMap.dropLine(polyline, width, height, material, testStyle );

    }
}
