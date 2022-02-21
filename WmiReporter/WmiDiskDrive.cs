using System.Collections.Generic;
using System.Management;
using System.Runtime.Serialization;

namespace WmiReporter
{
    [DataContract]
    internal class WmiDiskDrive
    {
        [DataMember] public string Model { get; private set; }
        [DataMember] public uint? Partitions { get; private set; }
        [DataMember] public string SerialNumber { get; private set; }
        [DataMember] public ulong? Size { get; private set; }

        public string Manufacturer { get; }

        public string JsonKey => Model ?? "" + Manufacturer ?? "";

        private static string queryFields = "Model, Partitions, Size, Manufacturer";
        public static IEnumerable<WmiDiskDrive> Query ()
        {
            try {
                return Query( "SELECT " + queryFields + ", SerialNumber FROM Win32_DiskDrive" );
            }
            catch (ManagementException) {
                return Query( "SELECT " + queryFields + " FROM Win32_DiskDrive" );
            }
        }
        private static IEnumerable<WmiDiskDrive> Query (string query)
        {
            ManagementObjectCollection mosQueryCollection = new ManagementObjectSearcher( query ).Get();
            List<WmiDiskDrive> result = new List<WmiDiskDrive>();
            foreach (ManagementBaseObject memory in mosQueryCollection)
                result.Add( new WmiDiskDrive( new WmiObject( memory ) ) );
            return result;
        }

        private WmiDiskDrive (WmiObject wmiObject)
        {
            Model = wmiObject.GetValue( "Model" );
            Partitions = wmiObject.GetValue<uint?>( "Partitions" );
            SerialNumber = wmiObject.GetValue( "SerialNumber" );
            Size = wmiObject.GetValue<ulong?>( "Size" );
            Manufacturer = wmiObject.GetValue( "Manufacturer" );
        }
    }
}
