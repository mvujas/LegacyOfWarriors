using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Net
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
