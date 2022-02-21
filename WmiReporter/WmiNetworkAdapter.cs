using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.Serialization;
using System.Text;

namespace WmiReporter
{
    [DataContract]
    internal class WmiNetworkAdapter
    {
        [DataMember] public string MACAddress { get; private set; }
        [DataMember] public string Manufacturer { get; private set; }
        [DataMember] public string Name { get; private set; }
        [DataMember] public ulong? Speed { get; private set; }
        [DataMember] public string Status { get; private set; }

        public static IEnumerable<WmiNetworkAdapter> Query ()
        {
            ManagementObjectCollection mosQueryCollection 
                = new ManagementObjectSearcher(
                    "SELECT MACAddress, Manufacturer, Name, Speed, Status FROM Win32_NetworkAdapter" ).Get();

            List<WmiNetworkAdapter> result = new List<WmiNetworkAdapter>();
            foreach (ManagementBaseObject memory in mosQueryCollection)
                result.Add( new WmiNetworkAdapter( new WmiObject( memory ) ) );
            return result;
        }

        private WmiNetworkAdapter (WmiObject wmiObject)
        {
            MACAddress = wmiObject.GetValue( "MACAddress" );
            Manufacturer = wmiObject.GetValue( "Manufacturer" );
            Name = wmiObject.GetValue( "Name" );
            Speed = wmiObject.GetValue<ulong?>( "Speed" );
            Status = wmiObject.GetValue( "Status" );
        }
    }
}
