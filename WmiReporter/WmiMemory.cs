using System.Collections.Generic;
using System.Management;
using System.Runtime.Serialization;

namespace WmiReporter
{
    [DataContract]
    internal class WmiMemory
    {
        public readonly static Dictionary<ushort, string> MemoryTypes
            = new Dictionary<ushort, string>() {
            { 8, "SRAM" },
            { 9, "RAM" },
            { 17, "SDRAM" },
            { 20, "DDR" },
            { 21, "DDR2" },
            { 22, "DDR2" },
            { 24, "DDR3" },
            { 26, "DDR4" }
        };

        [DataMember] public ulong? Capacity { get; private set; }
        [DataMember] public string DeviceLocator { get; private set; }
        [DataMember] public string PartNumber { get; private set; }
        [DataMember] public uint? Speed { get; private set; }
        [DataMember] public ushort MemoryType { get; private set; }
        [DataMember] public string MemoryTypeCode { get; private set; }
        [DataMember] public string Manufacturer { get; private set; }
        [DataMember] public string SerialNumber { get; private set; }

        public string JsonKey => PartNumber ?? "" + Manufacturer ?? "" + SerialNumber ?? "";

        public static IEnumerable<WmiMemory> Query ()
        {
            ManagementObjectCollection mosQueryCollection 
                = new ManagementObjectSearcher( 
                    "SELECT Capacity, DeviceLocator, PartNumber, Speed, MemoryType, Manufacturer, SerialNumber FROM Win32_PhysicalMemory" ).Get();

            List<WmiMemory> result = new List<WmiMemory>();
            foreach (ManagementBaseObject memory in mosQueryCollection)
                result.Add( new WmiMemory( new WmiObject( memory ) ) );
            return result;
        }

        private WmiMemory (WmiObject wmiObject)
        {
            Capacity = wmiObject.GetValue <ulong?>( "Capacity" );
            DeviceLocator = wmiObject.GetValue( "DeviceLocator" );
            PartNumber = wmiObject.GetValue( "PartNumber" );
            Speed = wmiObject.GetValue <uint?>( "Speed" );
            MemoryType = wmiObject.GetValue <ushort?>( "MemoryType" ) ?? 0;
            MemoryTypeCode = MemoryTypes.TryGetValue( MemoryType, out string memoryTypeCode ) ? memoryTypeCode : "Unknown";
            Manufacturer = wmiObject.GetValue( "Manufacturer" );
            SerialNumber = wmiObject.GetValue( "SerialNumber" );
        }
    }
}
