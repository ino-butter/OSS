using GoShared;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ServerTest : MonoBehaviour
{

    bool isInSession = false;
    readonly float syncPositionTime = 0.2f;
    float syncPositionTimer = 0;

    public GameObject prefabCharacter;
    public LocationManager locationManager;

    Dictionary<int, GameObject> units = new Dictionary<int, GameObject>();
    void Start()
    {
        GameManager.instance.GetManager<ServerManager>().TryConnectServer();
    }

    private void OnEnable()
    {
        GameManager.instance.GetManager<ServerManager>().ServerEventer.AddPacketEvent(PacketCode.R_EnterSession, R_EnterSession);
        GameManager.instance.GetManager<ServerManager>().ServerEventer.AddPacketEvent(PacketCode.R_BrodcastEnterSessionEntity, R_BrodcastEnterSessionEntity);
        GameManager.instance.GetManager<ServerManager>().ServerEventer.AddPacketEvent(PacketCode.R_BrodcastExitSessionEntity, R_BrodcastExitSessionEntity);
        GameManager.instance.GetManager<ServerManager>().ServerEventer.AddPacketEvent(PacketCode.R_RequestSessionData, R_RequestSessionData);
        GameManager.instance.GetManager<ServerManager>().ServerEventer.AddPacketEvent(PacketCode.R_Move, R_Move);
    }



    private void OnDisable()
    {
        GameManager.instance.GetManager<ServerManager>().ServerEventer.DeletePacketEvent(PacketCode.R_EnterSession, R_EnterSession);
        GameManager.instance.GetManager<ServerManager>().ServerEventer.DeletePacketEvent(PacketCode.R_BrodcastEnterSessionEntity, R_BrodcastEnterSessionEntity);
        GameManager.instance.GetManager<ServerManager>().ServerEventer.DeletePacketEvent(PacketCode.R_BrodcastExitSessionEntity, R_BrodcastExitSessionEntity);
        GameManager.instance.GetManager<ServerManager>().ServerEventer.DeletePacketEvent(PacketCode.R_RequestSessionData, R_RequestSessionData);
        GameManager.instance.GetManager<ServerManager>().ServerEventer.DeletePacketEvent(PacketCode.R_Move, R_Move);
    }


    private void S_EnterSession()
    {
        var s_es = new S_EnterSession();
        s_es.sessionIDX = 0;
        GameManager.instance.GetManager<ServerManager>().Send(s_es);
        Debug.Log("세션 연결 요청 보냄");
    }
    private void R_EnterSession(PacketCode packetCode, byte[] buffer)
    {
        var r_es = new R_EnterSession();
        isInSession = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            S_EnterSession();
        if (Input.GetKeyDown(KeyCode.F12))
            isInSession = true;
        S_Move();
    }
    private void S_Move()
    {
        if (isInSession == false)
            return;
        syncPositionTimer += Time.deltaTime;
        if(syncPositionTimer >= syncPositionTime)
        {
            syncPositionTimer = 0;
            var s_m = new S_Move();
            s_m.moveVector = new MoveVector();
            s_m.moveVector.dirX = locationManager.currentLocation.latitude;
            s_m.moveVector.dirZ = locationManager.currentLocation.longitude;
            GameManager.instance.GetManager<ServerManager>().Send(s_m);
        }
    }
    private void R_Move(PacketCode packetCode, byte[] buffer)
    {
        var r_m = ServerManager.ByteArrayToStruct<R_Move>(buffer);
        var character = units[r_m.uid];
        character.transform.position = new Coordinates(r_m.moveVector.dirX, r_m.moveVector.dirZ, 0).convertCoordinateToVector();
        Debug.Log("이동 데이터 받음");
    }
    private void R_BrodcastEnterSessionEntity(PacketCode packetCode, byte[] buffer)
    {
        var r_bese = ServerManager.ByteArrayToStruct<R_BrodcastEnterSessionEntity>(buffer);
        var character = Instantiate(prefabCharacter, Vector3.zero, Quaternion.identity, null); 
        units.Add(r_bese.packetUnitData.uid, character);
        Debug.Log("유저가 접속함");
    }
    private void R_BrodcastExitSessionEntity(PacketCode packetCode, byte[] buffer)
    {
        var r_bese = ServerManager.ByteArrayToStruct<R_BrodcastExitSessionEntity>(buffer);
        Destroy(units[r_bese.uid]);
        units.Remove(r_bese.uid);
        Debug.Log("유저가 나감");
    }
    private void R_RequestSessionData(PacketCode packetCode, byte[] buffer)
    {
        var r_rsd = ServerManager.ByteArrayToStruct<R_RequestSessionData>(buffer);
        foreach(var unit in r_rsd.packetUnitData)
        {
            var character = Instantiate(prefabCharacter, Vector3.zero, Quaternion.identity, null);
            units.Add(unit.uid, character);
        }
        Debug.Log("세션 접속 데이터를 받음");
    }
}
