using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Security.Principal;

namespace MAudioDriverMonitor
{
    static class Program
    {
        static private bool isAdministrator()
        {
            SimpleLogger.Instance().WriteLine("Checking privileges...");
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SimpleLogger.Instance();
            SimpleLogger.Instance().WriteLine("Starting " + Application.ProductName);
            if (!isAdministrator())
            {
                SimpleLogger.Instance().WriteLine("No administrator rights, exiting...");
                MessageBox.Show("This application must be started with Administrator privileges !", "Access Denied !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                PowerStateMonitor.Instance();
                PowerStateMonitor.Instance().registerSuspendEvent("AudioEndpointBuilder");
                PowerStateMonitor.Instance().registerResumeEvent("AudioSrv");
                MonitorForm monitorForm = new MonitorForm();
                Application.Run(monitorForm);
            }

            SimpleLogger.Instance().WriteLine("Stopping " + Application.ProductName);
        }
    }
}
