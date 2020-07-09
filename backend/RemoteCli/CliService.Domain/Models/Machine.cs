using System.Collections.Generic;

namespace CliService.Domain.Models
{
    public class Machine
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public string NetFrameworkVersion { get; set; }
        
        public string WindowsVersion { get; set; }
        
        public IEnumerable<Network> Networks { get; set; }
        
        public IEnumerable<StorageDevice> StorageDevices { get; set; }
        
        public IEnumerable<Antivirus> Antivirus { get; set; }
        
        public IEnumerable<Firewall> Firewalls { get; set; }
    }
}