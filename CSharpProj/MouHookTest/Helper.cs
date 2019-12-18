using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MouHookTest
{
    class Helper
    {
        internal void ProcessHideRun(string name)
        {
            ProcessHideRun(name, "");
        }

        internal void ProcessHideRun(string name, string args)
        {
            var proc = new Process();
            proc.StartInfo.FileName = name;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();
        }

        internal bool IsAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        internal bool ScQueryCheck(string strproc)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "sc";
            p.StartInfo.Arguments = "query " + strproc;
            p.Start();
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            if (output.Contains("RUNNING"))
                return true;
            return false;
        }
    }
}
