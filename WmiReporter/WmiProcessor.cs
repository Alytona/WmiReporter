using System.IO;
using System.Management;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WmiReporter
{
    [DataContract]
    internal class WmiProcessor
    {
        private readonly static DataContractJsonSerializer _serializer = new DataContractJsonSerializer( typeof( WmiProcessor ) );

        [DataMember] public string Name { get; private set; }
        [DataMember] public string SocketDesignation { get; private set; }
        [DataMember] public uint? NumberOfCores { get; private set; }
        [DataMember] public uint? NumberOfLogicalProcessors { get; private set; }
        public string JsonKey => Name ?? "";
        public static WmiProcessor Query ()
        {
            ManagementObjectCollection mosQueryCollection = new ManagementObjectSearcher( "SELECT Name, NumberOfCores, " +
                "SocketDesignation, NumberOfLogicalProcessors FROM Win32_Processor" ).Get();

            foreach (ManagementBaseObject processor in mosQueryCollection)
                return new WmiProcessor( new WmiObject( processor ) );
            return null;
        }
        public string GetJson ()
        {
            MemoryStream dataStream = new MemoryStream();
            _serializer.WriteObject( dataStream, this );

            string json = Encoding.UTF8.GetString( dataStream.ToArray() );
            return json;
        }

        private WmiProcessor (WmiObject wmiObject)
        {
            Name = wmiObject.GetValue( "Name" );
            SocketDesignation = wmiObject.GetValue( "SocketDesignation" );
            NumberOfCores = wmiObject.GetValue<uint?>( "NumberOfCores" );
            NumberOfLogicalProcessors = wmiObject.GetValue<uint?>( "NumberOfLogicalProcessors" );
        }
    }
}
