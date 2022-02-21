using System.Collections.Generic;
using System.Management;
using System.Runtime.Serialization;

namespace WmiReporter
{
    [DataContract]
    internal class WmiLogicalDisc
    {
        public readonly static Dictionary<uint, string> DiscTypes
            = new Dictionary<uint, string>() {
            { 1, "No Root Directory" },
            { 2, "Removable Disk" },
            { 3, "Local Disk" },
            { 4, "Network Drive" },
            { 5, "Compact Disc" },
            { 6, "RAM Disk" }
        };

        [DataMember] public string Caption { get; private set; }
        [DataMember] public string Name { get; private set; }
        [DataMember] public string DeviceID { get; private set; }
        [DataMember] public string Description { get; private set; }
        [DataMember] public string DriveType { get; private set; }
        [DataMember] public string FileSystem { get; private set; }
        [DataMember] public ulong? Size { get; private set; }
        [DataMember] public ulong? FreeSpace { get; private set; }
        [DataMember] public string ProviderName { get; private set; }
        [DataMember] public string VolumeName { get; private set; }
        [DataMember] public string VolumeSerialNumber { get; private set; }

        public static IEnumerable<WmiLogicalDisc> Query ()
        {
            ManagementObjectCollection mosQueryCollection 
                = new ManagementObjectSearcher(
                    "SELECT Caption, Name, DeviceID, Description, DriveType, FileSystem, Size, FreeSpace, ProviderName, VolumeName, VolumeSerialNumber FROM Win32_LogicalDisk" ).Get();

            List<WmiLogicalDisc> result = new List<WmiLogicalDisc>();
            foreach (ManagementBaseObject memory in mosQueryCollection)
                result.Add( new WmiLogicalDisc( new WmiObject( memory ) ) );
            return result;
        }

        private WmiLogicalDisc (WmiObject wmiObject)
        {
            Caption = wmiObject.GetValue( "Caption" );
            Name = wmiObject.GetValue( "Name" );
            DeviceID = wmiObject.GetValue( "DeviceID" );
            Description = wmiObject.GetValue( "Description" );
            
            uint? driveTypeCode = wmiObject.GetValue<uint?>( "DriveType" );
            if (driveTypeCode != null)
                DriveType = DiscTypes.TryGetValue( (uint)driveTypeCode, out string driveType ) ? driveType : "Unknown";

            FileSystem = wmiObject.GetValue( "FileSystem" );
            Size = wmiObject.GetValue<ulong?>( "Size" );
            FreeSpace = wmiObject.GetValue<ulong?>( "FreeSpace" );
            ProviderName = wmiObject.GetValue( "ProviderName" );
            VolumeName = wmiObject.GetValue( "VolumeName" );
            VolumeSerialNumber = wmiObject.GetValue( "VolumeSerialNumber" );
        }
    }
}
