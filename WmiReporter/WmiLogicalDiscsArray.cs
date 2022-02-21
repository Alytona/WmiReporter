using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WmiReporter
{
    internal class WmiLogicalDiscsArray
    {
        private readonly static DataContractJsonSerializer _dataSerializer = new DataContractJsonSerializer( typeof( WmiLogicalDisc ) );

        public List<WmiLogicalDisc> LogicalDiscs
        {
            get; private set;
        }

        public static WmiLogicalDiscsArray Query ()
        {
            return new WmiLogicalDiscsArray( WmiLogicalDisc.Query() );
        }

        public string GetJson ()
        {
            StringBuilder json = new StringBuilder( "{" );
            for (int i = 0; i < LogicalDiscs.Count; i++)
            {
                MemoryStream dataStream = new MemoryStream();
                _dataSerializer.WriteObject( dataStream, LogicalDiscs[i] );
                if (i > 0)
                    json.Append( "," );

                string jsonElement = Encoding.UTF8.GetString( dataStream.ToArray() );
                json.Append( $"\"{i}\":{jsonElement}" );
            }
            json.Append( "}" );
            return json.ToString();
        }

        private WmiLogicalDiscsArray (IEnumerable<WmiLogicalDisc> discsArray)
        {
            LogicalDiscs = new List<WmiLogicalDisc>( discsArray );
        }
    }
}
