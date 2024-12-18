using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Packet
{
    public enum PacketCode
    {
        S_EnterSession = 100,
        R_EnterSession = 101,

        S_Move = 3000,
        R_Move = 3001,
        S_MoveDir = 3002,
        R_MoveDir = 3003,
        S_OnHitbox = 3004,
        R_OnDamage = 3005,
        S_SetPosition = 3011,
        R_SetPosition = 3012,

        S_RequestSessionData = 5001,
        R_RequestSessionData = 5002,
        R_BrodcastEnterSessionEntity = 5003,
        R_BrodcastExitSessionEntity = 5004,

        S_JoinParty = 5010,
        R_JoinParty = 5011,
        S_ExitParty = 5012,
        R_ExitParty = 5013,
        S_CreateParty = 5014,
        R_CreateParty = 5015,
    }
    /* ErrorCode의 정보
     * Succes       : 단일 패킷에 대한 송신 완료 코드
     * Error        : 패킷 전송에 대해 오류가 발생, 헤더 패킷의 메시지 참조
     * Completed    : 두 개 이상의 패킷이 전부 전송에 성공하였을 때 보내는 코드
     * Wait         : 두 개 이상의 패킷을 전송하여야 할 때 보내는 코드
     */
    public enum StateCode
    {
        Success, Error, Complete, Wait
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public struct PacketUnitData
    {
        public int uid;
        public ushort utype;
        public int hp; // hp 값을 0으로 넘기면 클라이언트에서 해당 유닛을 없애야함
        public MoveVector moveVector;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class MoveVector
    {

        public double dirX;
        public double dirZ;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public float x;
        public float y;
        public float z;
        public float speed;
        public static float Distance(MoveVector v1, MoveVector v2)
        {
            float num = v1.x - v2.x;
            float num2 = v1.y - v2.y;
            float num3 = v1.z - v2.z;
            return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3);
        }
        public void ResetDir()
        {
            dirX = 0;
            dirZ = 0;
        }
        public static MoveVector operator /(MoveVector _vector, float _value)
        {
            MoveVector moveVector = new MoveVector();
            moveVector.x = _vector.x / _value;
            moveVector.y = _vector.y / _value;
            moveVector.z = _vector.z / _value;
            return moveVector;
        }
        public static MoveVector operator *(MoveVector _vector, float _value)
        {
            MoveVector moveVector = new MoveVector();
            moveVector.x = _vector.x * _value;
            moveVector.y = _vector.y * _value;
            moveVector.z = _vector.z * _value;
            return moveVector;
        }
        public static MoveVector operator -(MoveVector _v1, MoveVector _v2)
        {
            MoveVector moveVector = new MoveVector();
            moveVector.x = _v1.x - _v2.x;
            moveVector.y = _v1.y - _v2.y;
            moveVector.z = _v1.z - _v2.z;
            return moveVector;
        }
        public static MoveVector operator +(MoveVector _v1, MoveVector _v2)
        {
            MoveVector moveVector = new MoveVector();
            moveVector.x = _v1.x + _v2.x;
            moveVector.y = _v1.y + _v2.y;
            moveVector.z = _v1.z + _v2.z;
            return moveVector;
        }
        public float Magnitude()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }
        public void Normalize()
        {
            float num = Magnitude();
            var n = new MoveVector();
            if (num > 1E-05f)
            {
                n = this / num;
                dirX = n.x;
                dirZ = n.z;
            }
        }
        public static MoveVector Lerp(MoveVector a, MoveVector b, float t)
        {
            return a * (1 - t) + b * t;
        }
        public void Dir(MoveVector _target)
        {
            var temp = new MoveVector();
            temp = _target - this;
            temp.Normalize();
            dirX = temp.dirX;
            dirZ = temp.dirZ;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class Header
    {
        public PacketCode code;
        public StateCode stateCode = StateCode.Success;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class S_EnterSession : Header
    {
        public int sessionIDX;
        public S_EnterSession()
        {
            code = PacketCode.S_EnterSession;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class R_EnterSession : Header
    {
        public R_EnterSession()
        {
            code = PacketCode.R_EnterSession;
        }
    }



    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class S_Move : Header
    {
        public MoveVector moveVector;
        public S_Move()
        {
            code = PacketCode.S_Move;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class R_Move : Header
    {
        public int uid;
        public MoveVector moveVector;
        public R_Move()
        {
            code = PacketCode.R_Move;
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class S_MoveDir : Header
    {
        public MoveVector moveVector;
        public S_MoveDir()
        {
            code = PacketCode.S_MoveDir;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class R_MoveDir : Header
    {
        public MoveVector moveVector;
        public R_MoveDir()
        {
            code = PacketCode.R_MoveDir;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class S_OnHitbox : Header
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string attackName;
        public int uidLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public ushort[] uids;
        public S_OnHitbox()
        {
            code = PacketCode.S_OnHitbox;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class R_OnDamage : Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public PacketUnitData[] packetUnitData = new PacketUnitData[50];
        public int unitDataLength;
        public R_OnDamage()
        {
            code = PacketCode.R_OnDamage;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class S_SetPosition : Header
    {
        public MoveVector moveVector;
        public S_SetPosition()
        {
            code = PacketCode.S_SetPosition;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class R_SetPosition : Header
    {
        public MoveVector moveVector;
        public R_SetPosition()
        {
            code = PacketCode.R_SetPosition;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), Serializable]
    public class S_RequestSessionData : Header
    {
        public int sessionCode = 0;
        public S_RequestSessionData()
        {
            code = PacketCode.S_RequestSessionData;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), Serializable]
    public class R_RequestSessionData : Header
    {
        public PacketUnitData[] packetUnitData;
        public R_RequestSessionData()
        {
            code = PacketCode.R_RequestSessionData;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), Serializable]
    public class R_BrodcastEnterSessionEntity : Header
    {
        public PacketUnitData packetUnitData;
        public R_BrodcastEnterSessionEntity()
        {
            code = PacketCode.R_BrodcastEnterSessionEntity;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), Serializable]
    public class R_BrodcastExitSessionEntity : Header
    {
        public int uid;
        public R_BrodcastExitSessionEntity()
        {
            code = PacketCode.R_BrodcastExitSessionEntity;
        }
    }

    // Party
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), Serializable]
    public class S_CreateParty : Header
    {
        public int cid = 0;
        public S_CreateParty()
        {
            code = PacketCode.S_CreateParty;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), Serializable]
    public class R_CreateParty : Header
    {
        public int partyIDX = 0;
        public R_CreateParty()
        {
            code = PacketCode.R_CreateParty;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), Serializable]
    public class S_JoinParty : Header
    {
        public int cid = 0;
        public int partyIDX = 0;
        public S_JoinParty()
        {
            code = PacketCode.S_JoinParty;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), Serializable]
    public class R_JoinParty : Header
    {
        public R_JoinParty()
        {
            code = PacketCode.R_JoinParty;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), Serializable]
    public class S_ExitParty : Header
    {
        public int cid = 0;
        public S_ExitParty()
        {
            code = PacketCode.S_ExitParty;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), Serializable]
    public class R_ExitParty : Header
    {
        public R_ExitParty()
        {
            code = PacketCode.R_ExitParty;
        }
    }


    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    //public class S_EnterWorld : Header
    //{
    //    public int worldCode;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)]
    //    public string entity;
    //    public S_EnterWorld()
    //    {
    //        code = PacketCode.S_EnterWorld;
    //    }
    //}
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    //public class R_EnterWorld : Header
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 900)]
    //    public string entities;
    //    public R_EnterWorld()
    //    {
    //        code = PacketCode.R_EnterWorld;
    //    }
    //}
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    //public class S_Entity : Header
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)]
    //    public string entity;
    //    public S_Entity()
    //    {
    //        code = PacketCode.S_Entity;
    //    }
    //}
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    //public class R_Entity : Header
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 900)]
    //    public string entities;
    //    public R_Entity()
    //    {
    //        code = PacketCode.R_Entity;
    //    }
    //}
}
