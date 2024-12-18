using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public enum ServerState
{
    Debug, Run
}
public class ServerManager : BaseManager
{
    public ServerState serverState = ServerState.Debug;
    public bool isConnected = false;

    public string id = "test_id_001";
    public string ip = "127.0.0.1";
    public int port = 50001;

    private IPEndPoint remoteAddr;
    private EndPoint endPoint;
    public Socket socket;

    private SocketAsyncEventArgs socketEvent;
    [SerializeField]
    private ServerEvent serverEventer;
    public ServerEvent ServerEventer { get { return serverEventer; } }

    private int bufferSize = 65535;
    void OnApplicationQuit()
    {
        CloseClient();
    }
    private void Awake()
    {
        serverEventer = GetComponent<ServerEvent>();
    }
    public void TryConnectServer()
    {
        if(serverState == ServerState.Debug)
        {
            isConnected = true;
            return;
        }
        endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(endPoint);
            isConnected = true;
            Debug.Log("Connecte Success");

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            //StartCoroutine(ServerReconnect());
        }

        // 메모리 버퍼를 초기화 한다. 크기는 1024이다
        socketEvent = new SocketAsyncEventArgs();
        socketEvent.SetBuffer(new byte[bufferSize], 0, bufferSize);
        socketEvent.UserToken = socket;
        // 메시지가 오면 이벤트를 발생시킨다. (IOCP로 꺼내는 것)
        socketEvent.Completed += Client_Completed;
        // 메시지가 오면 이벤트를 발생시킨다. (IOCP로 넣는 것)
        socket.ReceiveAsync(socketEvent);
        // 접속 환영 메시지
        remoteAddr = (IPEndPoint)socket.RemoteEndPoint;
    }
    private void Client_Completed(object sender, SocketAsyncEventArgs e)
    {
        if (socket.Connected && socketEvent.BytesTransferred > 0)
        {
            // 수신 데이터는 e.Buffer에 있다.
            byte[] data = e.Buffer;

            // 데이터를 string으로 변환한다.
            string msg = Encoding.ASCII.GetString(data);
            // 메모리 버퍼를 초기화 한다. 크기는 1024이다
            socketEvent.SetBuffer(new byte[bufferSize], 0, bufferSize);
            Packet.Header header = ByteArrayToStruct<Packet.Header>(data);
            serverEventer.ProcessEvent(header, data);
            // 메시지의 끝이 이스케이프 \r\n의 형태이면 서버에 표시한다.
            ////if (sb.Length >= 2 && sb[sb.Length - 2] == CR && sb[sb.Length - 1] == LF)
            ////{
            ////    // 개행은 없애고..
            ////    sb.Length = sb.Length - 2;
            ////    // string으로 변환한다.
            ////    msg = sb.ToString();
            ////    // 콘솔에 출력한다.
            ////    // Client로 Echo를 보낸다.
            ////    // 만약 메시지가 exit이면 접속을 끊는다.
            ////    if ("exit".Equals(msg, StringComparison.OrdinalIgnoreCase))
            ////    {
            ////        // 접속 종료 메시지
            ////        Console.WriteLine($"Disconnected : (From: {remoteAddr.Address.ToString()}:{remoteAddr.Port}, Connection time: {DateTime.Now})");
            ////        // 접속을 중단한다.
            ////        socket.DisconnectAsync(socketEvent);
            ////        return;
            ////    }
            ////    // 버퍼를 비운다.
            ////    sb.Clear();
            ////}
            // 메시지가 오면 이벤트를 발생시킨다. (IOCP로 넣는 것)
            this.socket.ReceiveAsync(socketEvent);
        }
        else
        {
            // 접속이 끊겼다..
            Console.WriteLine($"Disconnected :  (From: {remoteAddr.Address.ToString()}:{remoteAddr.Port}, Connection time: {DateTime.Now})");
        }
    }
    public void Send<T>(T data)
    {
        try
        {
            byte[] sendPacket = StructToByteArray(data);
            socket.Send(sendPacket, 0, sendPacket.Length, SocketFlags.None);
        }

        catch (Exception ex)
        {
            if (serverState == ServerState.Debug)
                Debug.Log("서버 디버깅 모드 패킷 버림");
            else
                Debug.Log(ex.ToString());
            return;
        }
    }

    void CloseClient()
    {
        if (socket != null)
        {
            socket.Close();
            socket = null;
        }
    }

    public byte[] StructToByteArray(object obj)
    {
        int size = Marshal.SizeOf(obj);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(obj, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    public static T ByteArrayToStruct<T>(byte[] buffer)
    {
        int size = Marshal.SizeOf(typeof(T));
        if (size > buffer.Length)
        {
            throw new Exception();
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 0, ptr, size);
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }
}
