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

        // �޸� ���۸� �ʱ�ȭ �Ѵ�. ũ��� 1024�̴�
        socketEvent = new SocketAsyncEventArgs();
        socketEvent.SetBuffer(new byte[bufferSize], 0, bufferSize);
        socketEvent.UserToken = socket;
        // �޽����� ���� �̺�Ʈ�� �߻���Ų��. (IOCP�� ������ ��)
        socketEvent.Completed += Client_Completed;
        // �޽����� ���� �̺�Ʈ�� �߻���Ų��. (IOCP�� �ִ� ��)
        socket.ReceiveAsync(socketEvent);
        // ���� ȯ�� �޽���
        remoteAddr = (IPEndPoint)socket.RemoteEndPoint;
    }
    private void Client_Completed(object sender, SocketAsyncEventArgs e)
    {
        if (socket.Connected && socketEvent.BytesTransferred > 0)
        {
            // ���� �����ʹ� e.Buffer�� �ִ�.
            byte[] data = e.Buffer;

            // �����͸� string���� ��ȯ�Ѵ�.
            string msg = Encoding.ASCII.GetString(data);
            // �޸� ���۸� �ʱ�ȭ �Ѵ�. ũ��� 1024�̴�
            socketEvent.SetBuffer(new byte[bufferSize], 0, bufferSize);
            Packet.Header header = ByteArrayToStruct<Packet.Header>(data);
            serverEventer.ProcessEvent(header, data);
            // �޽����� ���� �̽������� \r\n�� �����̸� ������ ǥ���Ѵ�.
            ////if (sb.Length >= 2 && sb[sb.Length - 2] == CR && sb[sb.Length - 1] == LF)
            ////{
            ////    // ������ ���ְ�..
            ////    sb.Length = sb.Length - 2;
            ////    // string���� ��ȯ�Ѵ�.
            ////    msg = sb.ToString();
            ////    // �ֿܼ� ����Ѵ�.
            ////    // Client�� Echo�� ������.
            ////    // ���� �޽����� exit�̸� ������ ���´�.
            ////    if ("exit".Equals(msg, StringComparison.OrdinalIgnoreCase))
            ////    {
            ////        // ���� ���� �޽���
            ////        Console.WriteLine($"Disconnected : (From: {remoteAddr.Address.ToString()}:{remoteAddr.Port}, Connection time: {DateTime.Now})");
            ////        // ������ �ߴ��Ѵ�.
            ////        socket.DisconnectAsync(socketEvent);
            ////        return;
            ////    }
            ////    // ���۸� ����.
            ////    sb.Clear();
            ////}
            // �޽����� ���� �̺�Ʈ�� �߻���Ų��. (IOCP�� �ִ� ��)
            this.socket.ReceiveAsync(socketEvent);
        }
        else
        {
            // ������ �����..
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
                Debug.Log("���� ����� ��� ��Ŷ ����");
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
