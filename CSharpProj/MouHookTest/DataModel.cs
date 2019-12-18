using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MouHookTest
{
    class DataModel
    {
        const ulong FILE_DEVICE_MOUCLASS_INPUT_INJECTION = 48781;

        private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return (((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method));
        }

        #region "constants"
        public const uint GENERIC_READ = unchecked(0x80000000);
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint OPEN_EXISTING = 3;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        public const int INVALID_HANDLE_VALUE = -1;
        #endregion

        #region "CTL_CODE"
        [Flags]
        internal enum EIOControlCode : uint
        {
            IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT = (uint)(FILE_DEVICE_MOUCLASS_INPUT_INJECTION << 16) | (0 << 14) | (2600 << 2) | 0,
            IOCTL_INJECT_MOUSE_BUTTON_INPUT = (uint)(FILE_DEVICE_MOUCLASS_INPUT_INJECTION << 16) | (0 << 14) | (2850 << 2) | 0,
            IOCTL_INJECT_MOUSE_MOVEMENT_INPUT = (uint)(FILE_DEVICE_MOUCLASS_INPUT_INJECTION << 16) | (0 << 14) | (2851 << 2) | 0,
            IOCTL_INJECT_MOUSE_INPUT_PACKET = (uint)(FILE_DEVICE_MOUCLASS_INPUT_INJECTION << 16) | (0 << 14) | (2870 << 2) | 0,
        }
        #endregion

        [Flags]
        internal enum ButtonCode : ushort
        {
            MOUSE_LEFT_BUTTON_DOWN = 0x0001,  // Left Button changed to down.
            MOUSE_LEFT_BUTTON_UP = 0x0002,  // Left Button changed to up.
            MOUSE_RIGHT_BUTTON_DOWN = 0x0004,  // Right Button changed to down.
            MOUSE_RIGHT_BUTTON_UP = 0x0008,  // Right Button changed to up.
            MOUSE_MIDDLE_BUTTON_DOWN = 0x0010,  // Middle Button changed to down.
            MOUSE_MIDDLE_BUTTON_UP = 0x0020,  // Middle Button changed to up.

            MOUSE_BUTTON_4_DOWN = 0x0040,
            MOUSE_BUTTON_4_UP = 0x0080,
            MOUSE_BUTTON_5_DOWN = 0x0100,
            MOUSE_BUTTON_5_UP = 0x0200,

            MOUSE_WHEEL = 0x0400,
            MOUSE_HWHEEL = 0x0800
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct MOUSE_CLASS_BUTTON_DEVICE_INFORMATION
        {
            public ushort UnitId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct MOUSE_CLASS_MOVEMENT_DEVICE_INFORMATION
        {
            public ushort UnitId;
            public byte AbsoluteMovement;
            public byte VirtualDesktop;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct MOUSE_DEVICE_STACK_INFORMATION
        {
            public MOUSE_CLASS_BUTTON_DEVICE_INFORMATION ButtonDevice;
            public MOUSE_CLASS_MOVEMENT_DEVICE_INFORMATION MovementDevice;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct INJECT_MOUSE_BUTTON_INPUT_REQUEST
        {
            public IntPtr ProcessId;
            public ushort ButtonFlags;
            public ushort ButtonData;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct INJECT_MOUSE_MOVEMENT_INPUT_REQUEST
        {
            public IntPtr ProcessId;
            public ushort IndicatorFlags;
            public int MovementX;
            public int MovementY;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct INJECT_MOUSE_INPUT_PACKET_REQUEST
        {
            public IntPtr ProcessId;
            public byte UseButtonDevice;
            public long MovementX;
            public long MovementY;
        }
    }
}
