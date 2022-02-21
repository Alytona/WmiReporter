using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WmiReporter
{
    [DataContract]
    internal class WmiOperatingSystem
    {
        private readonly static DataContractJsonSerializer _serializer = new DataContractJsonSerializer( typeof( WmiOperatingSystem ) );

        public readonly static Dictionary<uint, string> OSLanguages
            = new Dictionary<uint, string>() {
            { 9, "English" },
            { 1033, "English US" },
            { 1036, "French" },
            { 1040, "Italian" },
            { 1041, "Japanese" },
            { 1049, "Russian" },
            { 1058, "Ukrainian" },
            { 1059, "Belarusian" }
        };

        [DataMember] public string Manufacturer { get; private set; }       // v (string)
        [DataMember] public string Caption { get; private set; }            // CIM_ManagedSystemElement (string)
        [DataMember] public string BuildNumber { get; private set; }        // v (string)
        [DataMember] public string OSArchitecture { get; private set; }     // В Windows 7 и в Windows XP нет
        [DataMember] public string Version { get; private set; }            // v (string)
        [DataMember] public string OSLanguage { get; private set; }         // v (uint32)
        [DataMember] public string CSName { get; private set; }             // CIM_OperatingSystem (string)
        [DataMember] public string Description { get; private set; }        // v (string)
        [DataMember] public string Organization { get; private set; }       // v (string)
        [DataMember] public ulong? FreePhysicalMemory { get; private set; } // CIM_OperatingSystem (uint64)
        [DataMember] public string InstallDate { get; private set; }        // CIM_ManagedSystemElement (datetime)

        private static string queryFields = "Manufacturer, Caption, BuildNumber, Version, OSLanguage, CSName, Description, Organization, FreePhysicalMemory, InstallDate";

        public static WmiOperatingSystem Query ()
        {
            try {
                return Query( "SELECT " + queryFields + ", OSArchitecture FROM Win32_OperatingSystem" );
            }
            catch (ManagementException) {
                return Query( "SELECT " + queryFields + " FROM Win32_OperatingSystem" );
            }
        }
        private static WmiOperatingSystem Query (string query)
        {
            ManagementObjectCollection mosQueryCollection = new ManagementObjectSearcher( query ).Get();
            foreach (ManagementBaseObject os in mosQueryCollection)
                return new WmiOperatingSystem( new WmiObject( os ) );
            return null;
        }

        public string GetJson ()
        {
            MemoryStream dataStream = new MemoryStream();
            _serializer.WriteObject( dataStream, this );

            string json = Encoding.UTF8.GetString( dataStream.ToArray() );
            return json;
        }

        private WmiOperatingSystem (WmiObject wmiObject)
        {
            Manufacturer = wmiObject.GetValue( "Manufacturer" );
            Caption = wmiObject.GetValue( "Caption" );
            BuildNumber = wmiObject.GetValue( "BuildNumber" );
            OSArchitecture = wmiObject.GetValue( "OSArchitecture" );
            Version = wmiObject.GetValue( "Version" );

            uint? osLanguageCode = wmiObject.GetValue<uint?>( "OSLanguage" );
            if (osLanguageCode != null) {
                OSLanguage = OSLanguages.TryGetValue( (uint)osLanguageCode, out string osLanguage ) ? osLanguage : "Unknown";
            }

            CSName = wmiObject.GetValue( "CSName" );
            Description = wmiObject.GetValue( "Description" );
            Organization = wmiObject.GetValue( "Organization" );
            FreePhysicalMemory = wmiObject.GetValue<ulong?>( "FreePhysicalMemory" );

            string iDate = wmiObject.GetValue( "InstallDate" );
            InstallDate = $"{iDate.Substring( 0, 4 )}-{iDate.Substring( 4, 2 )}-{iDate.Substring( 6, 2 )} {iDate.Substring( 8, 2 )}:{iDate.Substring( 10, 2 )}:{iDate.Substring( 12, 2 )}";
        }
    }
}
