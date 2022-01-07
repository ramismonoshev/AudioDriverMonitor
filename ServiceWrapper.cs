using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Windows.Forms;

namespace MAudioDriverMonitor
{
    class ServiceWrapper
    {
        internal String ServiceName;
        internal String DisplayName
        {
            get { return new ServiceController(ServiceName).DisplayName; }
        }
        internal System.ServiceProcess.ServiceControllerStatus Status
        {
            get { return new ServiceController(ServiceName).Status; }
        }
        internal List<ServiceWrapper> DependedOnServices = new List<ServiceWrapper>();
        internal List<ServiceWrapper> DependentServices = new List<ServiceWrapper>();
        internal ServiceWrapper(String serviceName)
        {
            ServiceName = serviceName;
        }
        internal void findRelatedServices()
        {
            ServiceController theService = new ServiceController(ServiceName);
            foreach (System.ServiceProcess.ServiceController service in theService.ServicesDependedOn)
            {
                ServiceWrapper serviceInformation = new ServiceWrapper(service.ServiceName);
                DependedOnServices.Add(serviceInformation);
            }
            foreach (System.ServiceProcess.ServiceController service in theService.DependentServices)
            {
                ServiceWrapper serviceInformation = new ServiceWrapper(service.ServiceName);
                serviceInformation.findRelatedServices();
                DependentServices.Add(serviceInformation);
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            __display(this, sb, 0);
            return sb.ToString();
        }
        private static void __display(ServiceWrapper theService, StringBuilder sb, int depth = 0)
        {
            sb.Append('\t', depth);
            sb.AppendLine("Service: " + theService.ServiceName + " (" + theService.DisplayName + ")");
            depth++;

            sb.Append('\t', depth);
            sb.AppendLine("DependedOnServices: ");
            if (theService.DependedOnServices.Count > 0)
            {
                foreach (ServiceWrapper service in theService.DependedOnServices)
                {
                    __display(service, sb, depth + 1);
                }
            }

            sb.Append('\t', depth);
            sb.AppendLine("DependentServices: ");
            if (theService.DependentServices.Count > 0)
            {
                foreach (ServiceWrapper service in theService.DependentServices)
                {
                    __display(service, sb, depth + 1);
                }
            }
        }
        internal void start()
        {
            ServiceController theService = new ServiceController(ServiceName);
            SimpleLogger.Instance().WriteLine("Starting service: " + theService.ServiceName + " (" + theService.DisplayName + ")");
            theService.Start();
            theService.WaitForStatus(ServiceControllerStatus.Running, System.TimeSpan.FromSeconds(5));
        }
        internal void stop()
        {
            ServiceController theService = new ServiceController(ServiceName);
            SimpleLogger.Instance().WriteLine("Stopping service: " + theService.ServiceName + " (" + theService.DisplayName + ")");
            theService.Stop();
            theService.WaitForStatus(ServiceControllerStatus.Stopped, System.TimeSpan.FromSeconds(5));
        }
    }
}
