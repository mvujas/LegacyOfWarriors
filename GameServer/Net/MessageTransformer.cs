using System;
using Utils;

namespace GameServer.Net
{
    public static class MessageTransformer
    {
        public static byte[] PrepareMessageForSending(byte[] message)
        {
            byte[] lengthArray = BitConverter.GetBytes(message.Length);
            byte[] result = ArrayUtils.MergeArrays(lengthArray, message);
            return result;
        }
    }
}
