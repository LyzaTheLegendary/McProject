using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    public enum PacketId
    {
        STATUS = 0,
        PINGRESPONSE = 1,
        ENCRYPTIONREQUEST = 1,
        KICK = 0,
        COMPRESSIONREQUEST = 3,
        LOGINSUCCESS = 2,
        DOTICK = 0,
        JOIN_INFO = 40,
    }
}
