using Microsoft.Win32.SafeHandles;
using Microsoft.Win32;
using MouHookTest.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static MouHookTest.DataModel;

namespace MouHookTest
{
    public class HookCore : IDisposable
    {
        private SafeFileHandle driverHandle;
        private Helper helper;

        public HookCore()
        {
            
            helper = new Helper();
        }

        public void Dispose()
        {
            driverHandle?.Dispose();
        }
        
        public void Init()
        {
            Init("\\\\.\\" + Config.Device_Name);
        }

        /// <summary>
        /// Creates SafeHandle to driver
        /// </summary>
        /// <param name="path"></param>
        public void Init(string path)
        {
            Console.WriteLine(Connector.GetLastError());
            driverHandle = Connector.CreateFileW(path, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
            if (driverHandle.IsInvalid)
            {
                driverHandle.Close();
                driverHandle.Dispose();
                driverHandle = null;
                throw new Exception("Win32 Exception : 0x" + Convert.ToString(Marshal.GetHRForLastWin32Error(), 16).PadLeft(8, '0'));
            }
        }

        public void Close()
        {
            if (driverHandle != null)
            {

            }
        }

        public bool Install()
        {
            if (helper.IsAdministrator())
            {
                if (helper.ScQueryCheck(Config.Device_File_Name) && !Uninstall())
                {
                    // Already installed, so try to uninstall but cause error
                    return false;
                }

                try
                {
                    if (File.Exists(Config.Sys_File_Temp_Path))
                    {
                        File.Delete(Config.Sys_File_Temp_Path);
                    }
                    
                    File.WriteAllBytes(Config.Sys_File_Temp_Path, Resources.libhook);                                                            
                    helper.ProcessHideRun("sc", @"delete " + Config.Device_File_Name);
                    helper.ProcessHideRun("sc", @"create " + Config.Device_File_Name + " binPath=\"" + Config.Sys_File_Temp_Path + "\" type=kernel");
                    helper.ProcessHideRun("sc", @"start " + Config.Device_File_Name);
                    
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public bool Uninstall()
        {
            if (helper.IsAdministrator())
            {
                var p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "sc";
                p.StartInfo.Arguments = "query " + Config.Device_File_Name;
                p.Start();
                var output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                if (output.Contains("does not exist")) return true;

                try
                {
                    helper.ProcessHideRun("sc", @"stop " + Config.Device_File_Name);
                    helper.ProcessHideRun("sc", @"delete " + Config.Device_File_Name);

                    if (File.Exists(Config.Sys_File_Temp_Path)) File.Delete(Config.Sys_File_Temp_Path);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public bool InitializeMouseDeviceStackContext()
        {
            if (driverHandle == null)
                throw new Exception(
                    "Driver handle is null. Is the driver running and have you called Init() function?");
            
            uint dummy = 0;
            var DeviceStackInformation = new MOUSE_DEVICE_STACK_INFORMATION();
            
            bool status = InitStackIo.DeviceIoControl(
                driverHandle,
                EIOControlCode.IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT,
                ref DeviceStackInformation,
                0,
                ref DeviceStackInformation,
                Marshal.SizeOf(DeviceStackInformation),
                out dummy,
                IntPtr.Zero
            );

            if (status)
            {
                Console.WriteLine("SUCCESS");
            } else
            {
                Console.WriteLine("Failed");
                Console.WriteLine(Connector.GetLastError());
            }
            
            return status;
        }

        public bool InjectMouseButtonInput(ulong processId, ushort buttonFlags, ushort buttonData)
        {
            if (driverHandle == null)
                throw new Exception(
                    "Driver handle is null. Is the driver running and have you called Init() function?");

            INJECT_MOUSE_BUTTON_INPUT_REQUEST Request = new INJECT_MOUSE_BUTTON_INPUT_REQUEST();
            Request.ProcessId = (IntPtr) processId;
            Request.ButtonFlags = buttonFlags;
            Request.ButtonData = buttonData;
            
            uint dummy = 0;           

            bool status = MouInputIo.DeviceIoControl(
                driverHandle,
                EIOControlCode.IOCTL_INJECT_MOUSE_BUTTON_INPUT,
                ref Request,
                Marshal.SizeOf(Request),
                ref Request,
                0,
                out dummy,
                IntPtr.Zero
            );

            if (status)
            {
                Console.WriteLine("SUCCESS");
            } else
            {
                Console.WriteLine("Failed");
                Console.WriteLine(Connector.GetLastError());
            }
            return status;
        }

        public bool InjectMouseMovementInput(ulong processId, ushort indicatorFlags, int movementX, int movementY)
        {
            
            if (driverHandle == null)
                throw new Exception(
                    "Driver handle is null. Is the driver running and have you called Init() function?");

            INJECT_MOUSE_MOVEMENT_INPUT_REQUEST Request = new INJECT_MOUSE_MOVEMENT_INPUT_REQUEST();
            Request.ProcessId = (IntPtr) processId;
            Request.IndicatorFlags = indicatorFlags;
            Request.MovementX = movementX;
            Request.MovementY = movementY;
            
            uint dummy = 0;                        

            bool status = MouMoveIo.DeviceIoControl(
                driverHandle,
                EIOControlCode.IOCTL_INJECT_MOUSE_MOVEMENT_INPUT,
                ref Request,
                Marshal.SizeOf(Request),
                ref Request,
                0,
                out dummy,
                IntPtr.Zero
            );

            if (status)
            {
                Console.WriteLine("SUCCESS");
            } else
            {
                Console.WriteLine("Failed");
                Console.WriteLine(Connector.GetLastError());
            }
            return status;
        }

        public bool InjectMouseButtonClick(ulong processId, ushort button, ulong durationInMilliseconds)
        {
            if (driverHandle == null)
                throw new Exception(
                    "Driver handle is null. Is the driver running and have you called Init() function?");

            ushort releaseButton = (ushort)ButtonCode.MOUSE_LEFT_BUTTON_DOWN;
                        
            // Determine the release button flag based on the button.
            switch ((ButtonCode)button)
            {
                case ButtonCode.MOUSE_LEFT_BUTTON_DOWN:
                    releaseButton = (ushort)ButtonCode.MOUSE_LEFT_BUTTON_UP;
                    break;

                case ButtonCode.MOUSE_RIGHT_BUTTON_DOWN:
                    releaseButton = (ushort)ButtonCode.MOUSE_RIGHT_BUTTON_UP;
                    break;

                case ButtonCode.MOUSE_MIDDLE_BUTTON_DOWN:
                    releaseButton = (ushort)ButtonCode.MOUSE_MIDDLE_BUTTON_UP;
                    break;

                case ButtonCode.MOUSE_BUTTON_4_DOWN:
                    releaseButton = (ushort)ButtonCode.MOUSE_BUTTON_4_UP;
                    break;

                case ButtonCode.MOUSE_BUTTON_5_DOWN:
                    releaseButton = (ushort)ButtonCode.MOUSE_BUTTON_5_UP;
                    break;

                default:
                    Console.WriteLine("Invalid button flag ", button);
                    return false;
            }
            
            INJECT_MOUSE_BUTTON_INPUT_REQUEST Request = new INJECT_MOUSE_BUTTON_INPUT_REQUEST();
            Request.ProcessId = (IntPtr) processId;
            Request.ButtonFlags = button;
            Request.ButtonData = 0;
                       
            uint dummy = 0;

            bool status = MouInputIo.DeviceIoControl(
                driverHandle,
                EIOControlCode.IOCTL_INJECT_MOUSE_BUTTON_INPUT,
                ref Request,
                Marshal.SizeOf(Request),
                ref Request,
                0,
                out dummy,
                IntPtr.Zero
            );

            if (!status)
            {
                Console.WriteLine("Failed");
                Console.WriteLine(Connector.GetLastError());
                return false;
            }

            // Make a delay for durationInMilliseconds
            Thread.Sleep((int)durationInMilliseconds);

            Request.ButtonFlags = releaseButton;
            
            status = MouInputIo.DeviceIoControl(
                driverHandle,
                EIOControlCode.IOCTL_INJECT_MOUSE_BUTTON_INPUT,
                ref Request,
                Marshal.SizeOf(Request),
                ref Request,
                0,
                out dummy,
                IntPtr.Zero
            );

            if (!status)
            {
                Console.WriteLine("Failed");
                Console.WriteLine(Connector.GetLastError());
                return false;
            }

            Console.WriteLine("Success");
            return status;
        }
    }
}
