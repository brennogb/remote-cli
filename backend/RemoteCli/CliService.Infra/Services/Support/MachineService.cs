using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CliService.Domain.Models;
using Microsoft.Win32;

namespace CliService.Infra.Services.Support
{
    public class MachineService : IMachineService
    {
        public Machine GetMachineInfo()
        {
            return new Machine
            {
                Id = GetMachineId(),
                Name = GetMachineName(),
                WindowsVersion = GetWindowsVersion(),
                NetFrameworkVersion = GetFrameworkVersion().ToString(3),
                Antivirus = GetAntivirusInfo(),
                Firewalls = GetFirewallInfo(),
                Networks = GetNetworks(),
                StorageDevices = GetStorageDeviceInfo()
            };
        }
        
        private Version GetFrameworkVersion()
        {
            using var ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full");
            if (ndpKey == null)
                throw new NotSupportedException(
                    @"No registry key found under 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full' to determine running framework version");
            var value = (int) (ndpKey.GetValue("Release") ?? 0);
            if (value >= 461808)
                return new Version(4, 7, 2);
            if (value >= 461308)
                return new Version(4, 7, 1);
            if (value >= 460798)
                return new Version(4, 7, 0);
            if (value >= 394802)
                return new Version(4, 6, 2);
            if (value >= 394254)
                return new Version(4, 6, 1);
            if (value >= 393295)
                return new Version(4, 6, 0);
            if (value >= 379893)
                return new Version(4, 5, 2);
            if (value >= 378675)
                return new Version(4, 5, 1);
            if (value >= 378389)
                return new Version(4, 5, 0);

            throw new NotSupportedException($"No 4.5 or later framework version detected, framework key value: {value}");
        }

        private string GetMachineId()
        {
            return QueryWMI("Win32_ComputerSystemProduct", properties: "UUID").First()["UUID"];
        }

        private string GetMachineName()
        {
            return Environment.MachineName;
        }

        private IEnumerable<Network> GetNetworks()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.OperationalStatus == OperationalStatus.Up)
                .Select(x => new Network
                {
                    AdapterName = x.Name,
                    IPv4 = x.GetIPProperties().UnicastAddresses
                        .FirstOrDefault(y => y.Address.AddressFamily == AddressFamily.InterNetwork)?.Address.ToString()
                });
        }
        
        private IEnumerable<StorageDevice> GetStorageDeviceInfo()
                    => DriveInfo.GetDrives().ToList()
            .Where(x => x.IsReady)
            .Select(x => new StorageDevice
            {
                Name = x.Name,
                AvailableSize = x.AvailableFreeSpace,
                TotalSize = x.TotalSize,
            });

        private IEnumerable<Dictionary<string, string>> QueryWMI(string className, string nameSpace = "root\\cimv2", params string[] properties)
        {
            var query = $"SELECT {string.Join(',', properties)} FROM {className}";
            var results = new ManagementObjectSearcher(nameSpace, query).Get();

            var result = new List<Dictionary<string, string>>();

            foreach (var item in results)
            {
                var itemProperties = properties.Select(x => new KeyValuePair<string, string>(x, item.Properties[x].Value?.ToString()));
                result.Add(new Dictionary<string, string>(itemProperties));
            }
            
            return result;
        }

        private IEnumerable<Antivirus> GetAntivirusInfo()
        {
            return Enumerable.Empty<Antivirus>();
        }

        private IEnumerable<Firewall> GetFirewallInfo()
        {
            return Enumerable.Empty<Firewall>();
        }

        private string GetWindowsVersion()
                    => QueryWMI("Win32_OperatingSystem", properties: "Caption").First()["Caption"];
    }
}