using System.Runtime.InteropServices;

namespace Monitoring.Infrastructure.RomEditor.Helpers
{
    public static class ByteHelper
    {
        public static byte[] GetBytes(this object obj)
        {
            var size = Marshal.SizeOf(obj);
            var arr = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public static T FromBytes<T>(this byte[] arr)
        {
            var obj = default(T);
            var size = Marshal.SizeOf(obj);
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);
            if (obj != null)
            {
                obj = (T)Marshal.PtrToStructure(ptr, obj.GetType());
                Marshal.FreeHGlobal(ptr);

                return obj;
            }
            return default(T);
        }

        public static void SetBytesAtPosition(this byte[] dest, int ptr, byte[] src)
        {
            for (var i = 0; i < src.Length; i++)
            {
                dest[ptr + i] = src[i];
            }
        }
    }
}