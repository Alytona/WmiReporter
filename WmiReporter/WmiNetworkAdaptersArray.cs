using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WmiReporter
{
    internal class WmiNetworkAdaptersArray
    {
        private readonly static DataContractJsonSerializer _dataSerializer = new DataContractJsonSerializer( typeof( WmiNetworkAdapter ) );

        public List<WmiNetworkAdapter> NetworkAdapters
        {
            get; private set;
        }

        public static WmiNetworkAdaptersArray Query ()
        {
            return new WmiNetworkAdaptersArray( WmiNetworkAdapter.Query() );
        }

        public string GetJson ()
        {
            StringBuilder json = new StringBuilder( "{" );
            for (int i = 0; i < NetworkAdapters.Count; i++)
            {
                MemoryStream dataStream = new MemoryStream();
                _dataSerializer.WriteObject( dataStream, NetworkAdapters[i] );
                if (i > 0)
                    json.Append( "," );

                string jsonElement = Encoding.UTF8.GetString( dataStream.ToArray() );
                json.Append( $"\"{i}\":{jsonElement}" );
            }
            json.Append( "}" );
            return json.ToString();
        }

        private WmiNetworkAdaptersArray (IEnumerable<WmiNetworkAdapter> networkAdapters)
        {
            NetworkAdapters = new List<WmiNetworkAdapter>( networkAdapters );
        }
    }
}
