using System.IO;
using System.Management;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WmiReporter
{
    [DataContract]
    internal class WmiBios
    {
        private readonly static DataContractJsonSerializer _serializer = new DataContractJsonSerializer( typeof( WmiBios ) );

        [DataMember] public string Manufacturer { get; private set; }
        [DataMember] public string Name { get; private set; }
        [DataMember] public string Version { get; private set; }
        [DataMember] public string SMBIOSBIOSVersion { get; private set; }
        public static WmiBios Query ()
        {
            ManagementObjectCollection mosQueryCollection = new ManagementObjectSearcher(
                "SELECT Manufacturer, Name, Version, SMBIOSBIOSVersion FROM Win32_BIOS" ).Get();

            foreach (ManagementBaseObject mboard in mosQueryCollection)
                return new WmiBios( new WmiObject( mboard ) );
            return null;
        }
        public string GetJson ()
        {
            MemoryStream dataStream = new MemoryStream();
            _serializer.WriteObject( dataStream, this );

            string json = Encoding.UTF8.GetString( dataStream.ToArray() );
            return json;
        }

        private WmiBios (WmiObject wmiObject)
        {
            Manufacturer = wmiObject.GetValue( "Manufacturer" );
            Name = wmiObject.GetValue( "Name" );
            Version = wmiObject.GetValue( "Version" );
            SMBIOSBIOSVersion = wmiObject.GetValue( "SMBIOSBIOSVersion" );
        }
    }
}
