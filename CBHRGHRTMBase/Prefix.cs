using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDevkit;

namespace CBHThmRGHJTAGRTERTMBase
{
    static class Prefix
    {

        private static uint outPOO = 0;
        public static byte[] ReadBytes(this IXboxConsole XBOX, uint offset, uint size)
        {
            byte[] temp = new byte[size];
            XBOX.DebugTarget.GetMemory(offset, size, temp, out outPOO);
            XBOX.DebugTarget.InvalidateMemoryCache(true, offset, size);
            return temp;
        }

        public static void WriteBytes(this IXboxConsole XBOX, uint offset, byte[] memory)
        {
            XBOX.DebugTarget.SetMemory(offset, (uint)memory.Length, memory, out outPOO);
        }
    }
}