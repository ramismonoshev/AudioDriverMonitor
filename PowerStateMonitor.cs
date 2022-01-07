using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;

namespace MAudioDriverMonitor
{
    class PowerStateMonitor
    {
        private static PowerStateMonitor instance = null;
        internal static PowerStateMonitor Instance()
        {
            if (PowerStateMonitor.instance == null)
            {
                PowerStateMonitor.instance = new PowerStateMonitor();
            }
            return PowerStateMonitor.instance;
        }
        internal Dictionary<String, ServiceWrapper> SuspendEventRegisteredServices = new Dictionary<String, ServiceWrapper>();
        internal Dictionary<String, ServiceWrapper> ResumeEventRegisteredServices = new Dictionary<String, ServiceWrapper>();
        internal bool MonitoringOn;
        internal void registerSuspendEvent(String serviceName)
        {
            ServiceWrapper serviceInformation = new ServiceWrapper(serviceName);
            SuspendEventRegisteredServices.Add(serviceName, serviceInformation);
            SimpleLogger.Instance().WriteLine("New service registered for Suspend events: " + serviceInformation.ToString());
        }
        internal void registerResumeEvent(String serviceName)
        {
            ServiceWrapper serviceInformation = new ServiceWrapper(serviceName);
            ResumeEventRegisteredServices.Add(serviceName, serviceInformation);
            SimpleLogger.Instance().WriteLine("New service registered for Resume events: " + serviceInformation.ToString());
        }

        internal void unregisterSuspendEvent(String serviceName)
        {
            SuspendEventRegisteredServices.Remove(serviceName);
        }

        internal void unregisterResumeEvent(String serviceName)
        {
            ResumeEventRegisteredServices.Remove(serviceName);
        }
        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs powerModeChangedEvent)
        {
            switch (powerModeChangedEvent.Mode)
            {
                case PowerModes.Suspend:
                    onSuspend();
                    break;
                case PowerModes.Resume:
                    onResume();
                    break;
                case PowerModes.StatusChange:
                    onStatusChanged();
                    break;
                default:
                    break;
            }
        }
        private void onSuspend()
        {
            SimpleLogger.Instance().WriteLine("PowerModes.Suspend event triggered !");
            if (MonitoringOn)
            {
                try
                {
                    stopServices();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Unable to stop services:\n" + ex.Message + "\n" + ex.InnerException.Message + "");
                }
            }
        }
        private void onResume()
        {
            SimpleLogger.Instance().WriteLine("PowerModes.Resume event triggered !");
            if (MonitoringOn)
            {
                try
                {
                    startServices();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Unable to start services:\n" + ex.Message + "\n" + ex.InnerException.Message + "");
                }
            }
        }

        private void onStatusChanged()
        {
            SimpleLogger.Instance().WriteLine("PowerModes.StatusChanged event triggered !");
        }

        internal void registerHandler()
        {
            SimpleLogger.Instance().WriteLine("Registering PowerModeChanged event handler");
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
        }

        internal PowerStateMonitor()
        {
            SimpleLogger.Instance().WriteLine("Starting PowerStateMonitor");
            registerHandler();
        }

        ~PowerStateMonitor()
        {
            SimpleLogger.Instance().WriteLine("Stopping PowerStateMonitor");
        }


        internal void startServicesMonitoring()
        {
            MonitoringOn = true;
        }
        internal void stopServicesMonitoring()
        {
            MonitoringOn = false;
        }

        internal void startServices()
        {
            SimpleLogger.Instance().WriteLine("Starting " + ResumeEventRegisteredServices.Count + " services");
            foreach (ServiceWrapper serviceInformation in ResumeEventRegisteredServices.Values)
            {
                serviceInformation.start();
            }
        }

        internal void stopServices()
        {
            SimpleLogger.Instance().WriteLine("Stopping " + ResumeEventRegisteredServices.Count + " services");
            foreach (ServiceWrapper serviceInformation in SuspendEventRegisteredServices.Values)
            {
                serviceInformation.stop();
            }
        }
    }
}
