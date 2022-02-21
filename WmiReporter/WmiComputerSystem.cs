using System.IO;
using System.Management;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WmiReporter
{
    [DataContract]
    internal class WmiComputerSystem
    {
        private readonly static DataContractJsonSerializer _serializer = new DataContractJsonSerializer( typeof( WmiComputerSystem ) );

        [DataMember] public string Name { get; private set; }
        [DataMember] public string UserName { get; private set; }
        [DataMember] public bool? PartOfDomain { get; private set; }
        [DataMember] public string Workgroup { get; private set; }
        public static WmiComputerSystem Query ()
        {
            ManagementObjectCollection mosQueryCollection = new ManagementObjectSearcher(
                "SELECT Name, UserName, PartOfDomain, Workgroup FROM Win32_ComputerSystem" ).Get();

            foreach (ManagementBaseObject mboard in mosQueryCollection)
                return new WmiComputerSystem( new WmiObject( mboard ) );
            return null;
        }
        public string GetJson ()
        {
            MemoryStream dataStream = new MemoryStream();
            _serializer.WriteObject( dataStream, this );

            string json = Encoding.UTF8.GetString( dataStream.ToArray() );
            return json;
        }

        private WmiComputerSystem (WmiObject wmiObject)
        {
            Name = wmiObject.GetValue( "Name" );
            UserName = wmiObject.GetValue( "UserName" )?.Replace( '\\', '/' );
            PartOfDomain = wmiObject.GetValue<bool?>( "PartOfDomain" );
            Workgroup = wmiObject.GetValue( "Workgroup" );
        }
    }
}
