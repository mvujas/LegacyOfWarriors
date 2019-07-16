using GameServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Net
{
    public static class MessageTransformer
    {
        public static byte[] PrepareMessageForSending(byte[] message)
        {
            byte[] lengthArray = BitConverter.GetBytes(message.Length);
            byte[] result = ArrayUtils.MergeArrays<byte>(lengthArray, message);
            return result;
        }
    }
}
