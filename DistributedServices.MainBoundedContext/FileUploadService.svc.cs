using Application.MainBoundedContext.DTO;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandler()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FileUploadService : IFileUploadService
    {
        private static Dictionary<string, FileStream> _wcfFileStreams = new Dictionary<string, FileStream>();

        private static string _lastUpdatedFilename;

        public string FileUpload(FileData fileData)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            FileStream fileStream;

            string filepath = Path.Combine(serviceBrokerSettingsElement.FileUploadDirectory, fileData.Filename);

            try
            {
                lock (_wcfFileStreams)
                {
                    _wcfFileStreams.TryGetValue(filepath, out fileStream);

                    if (fileStream == null)
                    {
                        fileStream = File.Open(filepath, FileMode.Create, FileAccess.ReadWrite);

                        _wcfFileStreams.Add(filepath, fileStream);
                    }
                }

                fileStream.Write(fileData.Buffer, 0, fileData.Buffer.Length);

                fileStream.Flush();

                return string.Empty;
            }
            catch (Exception ex)
            {
                FileUploadDone(filepath);

                return ex.ToString();
            }
        }

        public bool FileUploadDone(string filename)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            FileStream fileStream;

            string filepath = Path.Combine(serviceBrokerSettingsElement.FileUploadDirectory, filename);

            lock (_wcfFileStreams)
            {
                _wcfFileStreams.TryGetValue(filepath, out fileStream);

                if (fileStream != null)
                {
                    _wcfFileStreams.Remove(filepath);

                    _lastUpdatedFilename = filepath;

                    fileStream.Close();

                    return true;
                }
            }

            return false;
        }

        public bool PingNetwork(string hostNameOrAddress)
        {
            bool pingStatus = false;

            using (Ping ping = new Ping())
            {
                byte[] buffer = Encoding.ASCII.GetBytes("VFIN");

                int timeout = 120;

                try
                {
                    PingReply reply = ping.Send(hostNameOrAddress, timeout, buffer);

                    pingStatus = (reply.Status == IPStatus.Success);
                }
                catch (Exception)
                {
                    pingStatus = false;
                }
            }

            return pingStatus;
        }
    }
}
