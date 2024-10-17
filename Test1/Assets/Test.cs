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
    public LocationManager locationManager;
    public GOMap goMap;
    public Material testLineMaterial;
    public GOUVMappingStyle testStyle;

    public GameObject prefab;
    // Start is called before the first frame update
    void OnInteraction()
    {

        //dropTestLine();
        string start = "35.121027,129.103394";
        string goal = "35.120075225830078,129.10319519042969";
        //StartCoroutine(FetchDirections(start, goal));
        GeneratePoi();
    }

    void GeneratePoi()
    {
        var dir = new Coordinates(35.120075225830078, 129.10319519042969).convertCoordinateToVector();
        dir.y = 30;
        Vector3 rotate = new Vector3(90, 0, 0);
        GameObject obj = Instantiate(prefab, dir, Quaternion.Euler(rotate), null);
        obj.GetComponent<TextMeshPro>().text = "제 1정보통신관";
    }
    void dropTestLine()
    {
        Debug.Log("a");
        //1) Create a list of coordinates that will represent the polyline
        List<Coordinates> polyline = new List<Coordinates>();
        var location = locationManager.currentLocation;
        location.altitude = 0;
        polyline.Add(location);
        polyline.Add(new Coordinates(35.121027, 129.103394));


        //2) Set line width
        float width = 3;

        //3) Set the line height
        float height = 2;

        //4) Choose a material for the line (this time we link the material from the inspector)
        Material material = testLineMaterial;

        //5) call drop line
        goMap.dropLine(polyline, width, height, material, testStyle );

    }

    //private IEnumerator FetchDirections(string start, string goal)
    //{

    //    string api = "https://naveropenapi.apigw.ntruss.com/map-direction/v1/driving";
    //    string url = $"{api}?start={start}&goal={goal}";
    //    Debug.Log("Naver Dircetions 5 API : " + url);
    //    using (UnityWebRequest request = UnityWebRequest.Get(url))
    //    {
    //        request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", "lzop1y2nh7");
    //        request.SetRequestHeader("X-NCP-APIGW-API-KEY", "tuLo6FDIgZGbRYo7K1FQkRPpYLWwCH6OAjvVKRY0");

    //        yield return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.Success)
    //        {
    //            string jsonResult = request.downloadHandler.text;
    //            Debug.Log(jsonResult);
    //            // JSON 파싱 및 경로 처리
    //        }
    //        else
    //        {
    //            Debug.LogError("Error: " + request.error);
    //        }
    //    }
    //}
}
