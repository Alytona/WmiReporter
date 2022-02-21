using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WmiReporter
{
    internal class WmiMemoryArray
    {
        private readonly static DataContractJsonSerializer _itemsSerializer = new DataContractJsonSerializer( typeof( WmiMemory ) );

        public uint? MaxCapacity { get; private set; }
        public ulong MemSum { get; private set; }
        public ushort? MemoryDevices { get; private set; }
        public List <WmiMemory> MemoryArray { get; private set; }
        public string JsonKey => string.Join( "", MemoryArray.Select( (item) => item.JsonKey ).ToArray() );
        public static WmiMemoryArray Query ()
        {
            ManagementObjectCollection mosQueryCollection
                = new ManagementObjectSearcher( "SELECT MaxCapacity, MemoryDevices FROM Win32_PhysicalMemoryArray" ).Get();

            if (mosQueryCollection.Count == 0) {
                return new WmiMemoryArray( WmiMemory.Query() );
            }

            foreach (ManagementBaseObject memory in mosQueryCollection)
                return new WmiMemoryArray( new WmiObject( memory ), WmiMemory.Query() );
            return null;
        }

        public string GetJson ()
        {
            if (MaxCapacity == null && MemoryDevices == null && MemoryArray.Count == 0)
                return string.Empty;

            StringBuilder json = new StringBuilder( "{" );
            if (MaxCapacity != null && MemoryDevices != null) {
                json.Append( $"\"MaxCapacity\":{MaxCapacity}," );
                json.Append( $"\"MemSum\":{MemSum}," );
                json.Append( $"\"MemoryDevices\":{MemoryDevices}," );
            }
            for (int i = 0; i < MemoryArray.Count; i++)
            {
                MemoryStream dataStream = new MemoryStream();
                _itemsSerializer.WriteObject( dataStream, MemoryArray[i] );
                if (i > 0)
                    json.Append( "," );
                json.Append( $"\"{i}\":{Encoding.UTF8.GetString( dataStream.ToArray() )}" );
            }
            json.Append( "}" );
            return json.ToString();
        }

        private WmiMemoryArray (IEnumerable<WmiMemory> memoryArray)
        {
            MemoryArray = new List<WmiMemory> ( memoryArray );
            foreach (WmiMemory memory in MemoryArray)
                MemSum += memory.Capacity ?? 0;
        }
        private WmiMemoryArray (WmiObject wmiObject, IEnumerable<WmiMemory> memoryArray)
        {
            MaxCapacity = wmiObject.GetValue<uint?>( "MaxCapacity" );
            MemoryDevices = wmiObject.GetValue<ushort?>( "MemoryDevices" );

            MemoryArray = new List<WmiMemory>( memoryArray );
            foreach (WmiMemory memory in MemoryArray)
                MemSum += memory.Capacity ?? 0;
        }
    }
}
