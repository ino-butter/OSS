using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class EventTrigger
{
    public Packet.PacketCode code;
    public Action action;
}
public class ServerEvent : MonoBehaviour
{
    public delegate void RecivePacket(Packet.PacketCode packetCode, byte[] buffer);
    public Dictionary<Packet.PacketCode, byte[]> buffers = new Dictionary<Packet.PacketCode, byte[]>();
    private Dictionary<Packet.PacketCode, RecivePacket> packetEvents = new Dictionary<Packet.PacketCode, RecivePacket>();
    public void AddPacketEvent(Packet.PacketCode packetCode, RecivePacket reciveEvent)
    {
        if (packetEvents.ContainsKey(packetCode) == false)
            packetEvents.Add(packetCode, null);
        Debug.Log("Add Event : " + packetCode);
        packetEvents[packetCode] += reciveEvent;
    }
    public void DeletePacketEvent(Packet.PacketCode packetCode, RecivePacket reciveEvent)
    {
        if (packetEvents.ContainsKey(packetCode) == false)
            return;
        Debug.Log("Delete Event : " + packetCode);
        packetEvents[packetCode] -= reciveEvent;
    }

    public void ProcessEvent(Packet.Header header, byte[] data)
    {
        GameManager.instance.GetManager<JobQueue>().Enqueue(() =>
        {
            PacketProcess(header, data);
        });
        //var task = Task.Run(() => PacketProcess(header, data));
        //await task;
    }

    private void PacketProcess(Packet.Header header, byte[] data)
    {
        if (buffers.ContainsKey(header.code) == false)
            buffers.Add(header.code, null);
        buffers[header.code] = data;
        if (packetEvents.ContainsKey(header.code))
        {
            Debug.Log(string.Format("패킷 수신 : {0}", header.code));
            packetEvents[header.code]?.Invoke(header.code, data);
        }
    }
}