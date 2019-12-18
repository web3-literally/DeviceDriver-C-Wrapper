using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MouHookTest.DataModel;

namespace MouHookTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing sys wrapper in c#");

            HookCore hc = new HookCore();
            if (!hc.Install())
            {
                Console.WriteLine("Install failed");
            }
            hc.Init();
            hc.InitializeMouseDeviceStackContext();

            hc.InjectMouseMovementInput(18832, 0, 500, 30);
            hc.InjectMouseButtonClick(18832, (ushort) ButtonCode.MOUSE_RIGHT_BUTTON_DOWN, 300);

            hc.Uninstall();
        }
    }
}
