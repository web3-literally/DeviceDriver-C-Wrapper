using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MouHookTest.DataModel;

namespace MouHookTest
{
    internal class Connector
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            EIOControlCode IoControlCode,
            [MarshalAs(UnmanagedType.AsAny)]
            [In] object InBuffer,
            uint nInBufferSize,
            [MarshalAs(UnmanagedType.AsAny)]
            [Out] object OutBuffer,
            uint nOutBufferSize,
            ref uint pBytesReturned,
            [In] ref NativeOverlapped Overlapped
        );
        
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFileW(string lpFileName, uint dwDesiredAccess,
            uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CloseHandle(string lpFileName, uint dwDesiredAccess,
            uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        //public static extern int CreateFile(string lpFileName, int dwDesiredAccess,
        //    int dwShareMode, int lpSecurityAttributes, int dwCreationDisposition,
        //    int dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
    }

    internal class InitStackIo
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            EIOControlCode IoControlCode,
            ref MOUSE_DEVICE_STACK_INFORMATION InBuffer,
            int nInBufferSize,
            ref MOUSE_DEVICE_STACK_INFORMATION OutBuffer,
            int nOutBufferSize,
            out uint pBytesReturned,
            IntPtr Overlapped
        );
    }

    internal class MouInputIo
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            EIOControlCode IoControlCode,
            ref INJECT_MOUSE_BUTTON_INPUT_REQUEST InBuffer,
            int nInBufferSize,
            ref INJECT_MOUSE_BUTTON_INPUT_REQUEST OutBuffer,
            int nOutBufferSize,
            out uint pBytesReturned,
            IntPtr Overlapped
        );
    }

    internal class MouMoveIo
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            EIOControlCode IoControlCode,
            ref INJECT_MOUSE_MOVEMENT_INPUT_REQUEST InBuffer,
            int nInBufferSize,
            ref INJECT_MOUSE_MOVEMENT_INPUT_REQUEST OutBuffer,
            int nOutBufferSize,
            out uint pBytesReturned,
            IntPtr Overlapped
        );
    }
}
