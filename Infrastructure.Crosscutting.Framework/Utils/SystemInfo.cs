using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public class SystemInfo
    {
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
        public string MotherboardSerialNumber { get; set; }
        public string ProcessorId { get; set; }
        public string UserName { get; set; }
        public string MachineName { get; set; }
        public string DomainName { get; set; }

        public SystemInfo()
        {
#if SILVERLIGHT

            if (System.Windows.Application.Current.HasElevatedPermissions && System.Runtime.InteropServices.Automation.AutomationFactory.IsAvailable)
            {
                using (dynamic WshNetwork = System.Runtime.InteropServices.Automation.AutomationFactory.CreateObject("WScript.Network"))
                {
                    UserName = WshNetwork.UserName;
                    MachineName = WshNetwork.ComputerName;
                    DomainName = WshNetwork.UserDomain;
                }

                using (dynamic wmiLocator = System.Runtime.InteropServices.Automation.AutomationFactory.CreateObject("WbemScripting.SWbemLocator"))
                {
                    wmiLocator.Security_.ImpersonationLevel = 3;
                    wmiLocator.Security_.AuthenticationLevel = 4;
                    var wmiService = wmiLocator.ConnectServer(".", "root\\CIMV2");

                    var wmiQuery = "SELECT MACAddress FROM Win32_NetworkAdapterConfiguration where IPEnabled=true";
                    var queryResults = wmiService.ExecQuery(wmiQuery);
                    foreach (dynamic o in queryResults)
                    {
                        MACAddress = o.MACAddress;
                        break;
                    }

                    wmiQuery = "SELECT SerialNumber FROM Win32_BaseBoard";
                    queryResults = wmiService.ExecQuery(wmiQuery);
                    foreach (dynamic o in queryResults)
                    {
                        MotherboardSerialNumber = o.SerialNumber;
                        break;
                    }

                    wmiQuery = "SELECT ProcessorID FROM Win32_Processor";
                    queryResults = wmiService.ExecQuery(wmiQuery);
                    foreach (dynamic o in queryResults)
                    {
                        ProcessorId = o.ProcessorID;
                        break;
                    }

                    wmiQuery = "SELECT IPAddress FROM Win32_NetworkAdapterConfiguration where IPEnabled=true";
                    queryResults = wmiService.ExecQuery(wmiQuery);
                    foreach (dynamic o in queryResults)
                    {
                        if (o.IPAddress is Array)
                        {
                            IPAddress = string.Join(",", o.IPAddress);
                        }
                        else IPAddress = o.IPAddress;
                        break;
                    }
                }
            }
#else
            UserName = Environment.UserName;
            MachineName = Environment.MachineName;
            DomainName = Environment.UserDomainName;
            ProcessorId = GetProcessorID();
            MotherboardSerialNumber = GetSerialNumber();
            MACAddress = GetMacAddress();
            IPAddress = GetIpAddress();
#endif
        }

        static string GetProcessorID()
        {
            string sProcessorID = "";

#if !SILVERLIGHT
            string sQuery = "SELECT ProcessorId FROM Win32_Processor";

            using (ManagementObjectSearcher oManagementObjectSearcher = new System.Management.ManagementObjectSearcher("root\\CIMV2", sQuery))
            {
                using (System.Management.ManagementObjectCollection oCollection = oManagementObjectSearcher.Get())
                {
                    foreach (System.Management.ManagementObject oManagementObject in oCollection)
                    {
                        sProcessorID = (string)oManagementObject["ProcessorId"];
                    }
                }
            }
#endif

            return (sProcessorID);
        }

        static string GetSerialNumber()
        {
            string sSerialNumber = "";

#if !SILVERLIGHT
            string sQuery = "SELECT SerialNumber FROM Win32_BaseBoard";

            using (System.Management.ManagementObjectSearcher oManagementObjectSearcher = new System.Management.ManagementObjectSearcher("root\\CIMV2", sQuery))
            {
                using (System.Management.ManagementObjectCollection oCollection = oManagementObjectSearcher.Get())
                {
                    foreach (System.Management.ManagementObject oManagementObject in oCollection)
                    {
                        sSerialNumber = (string)oManagementObject["SerialNumber"];
                    }
                }
            }
#endif

            return (sSerialNumber);
        }

        static string GetMacAddress()
        {
            string sMACAddress = "";

#if !SILVERLIGHT
            string sQuery = "SELECT MACAddress FROM Win32_NetworkAdapterConfiguration where IPEnabled=true";

            using (System.Management.ManagementObjectSearcher oManagementObjectSearcher = new System.Management.ManagementObjectSearcher("root\\CIMV2", sQuery))
            {
                using (System.Management.ManagementObjectCollection oCollection = oManagementObjectSearcher.Get())
                {
                    foreach (System.Management.ManagementObject oManagementObject in oCollection)
                    {
                        sMACAddress = (string)oManagementObject["MACAddress"];
                    }
                }
            }
#endif

            return (sMACAddress);
        }

        static string GetIpAddress()
        {
            string sIPAddress = null;

#if !SILVERLIGHT
            string sQuery = "SELECT IPAddress FROM Win32_NetworkAdapterConfiguration where IPEnabled=true";

            using (System.Management.ManagementObjectSearcher oManagementObjectSearcher = new System.Management.ManagementObjectSearcher("root\\CIMV2", sQuery))
            {
                using (System.Management.ManagementObjectCollection oCollection = oManagementObjectSearcher.Get())
                {
                    foreach (System.Management.ManagementObject oManagementObject in oCollection)
                    {
                        if (oManagementObject["IPAddress"] is Array)
                        {
                            sIPAddress = string.Join(",", (string[])oManagementObject["IPAddress"]);
                        }
                        else sIPAddress = (string)oManagementObject["IPAddress"];
                    }
                }
            }
#endif

            return (sIPAddress);
        }
    }
}
