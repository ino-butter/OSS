using GoMap;
using GoShared;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Test : MonoBehaviour
{
    public TextAsset textAsset;
    public Dictionary<string, Dictionary<string, object>> path_table;

    public TextAsset localTextAsset;
    public Dictionary<string, Dictionary<string, object>> local;
    public LocationManager locationManager;
    public GOMap goMap;
    public Material testLineMaterial;
    public GOUVMappingStyle testStyle;

    public GameObject prefab;
    private NavMeshAgent agent;

    private void Awake()
    {
        path_table = CSVReader.ReadStringIndex(textAsset);
        local = CSVReader.ReadStringIndex(localTextAsset);
    }
    void PathFind(string _start, string _goal)
    {
        string next = path_table[_start][_goal].ToString();
        Debug.Log(next);
        if (next.CompareTo("x") == 0)
        {
            Debug.Log(string.Format("find : {0} goal : {1}", next, _goal));
        }
        else if (next.CompareTo("e") == 0)
            return;
        else
        {
            double[] param = new double[4];
            param[0] = double.Parse(local[_start]["latitude"].ToString());
            param[1] = double.Parse(local[_start]["longitude"].ToString());
            param[2] = double.Parse(local[next]["latitude"].ToString());
            param[3] = double.Parse(local[next]["longitude"].ToString());
            dropTestLine(param);
            Debug.Log(string.Format("contiue start : {0} goal : {1}", next, _goal));
            PathFind(next, _goal);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            TestNav();
        if (Input.GetKeyDown(KeyCode.F2))
            PathFind("1", "7");
        MovePoint();
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
    void GeneratePoi(double _latitude, double _longitude)
    {
        var dir = new Coordinates(_latitude, _longitude).convertCoordinateToVector();
        dir.y = 30;
        Vector3 rotate = new Vector3(90, 0, 0);
        GameObject obj = Instantiate(prefab, dir, Quaternion.Euler(rotate), null);
        obj.GetComponent<TextMeshPro>().text = "제 1정보통신관";
    }
    void dropTestLine(params double[] _value)
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
        float height = 5;

        //4) Choose a material for the line (this time we link the material from the inspector)
        Material material = testLineMaterial;

        //5) call drop line
        goMap.dropLine(polyline, width, height, material, testStyle );

    }
}
