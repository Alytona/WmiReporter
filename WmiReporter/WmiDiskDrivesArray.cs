using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WmiReporter
{
    internal class WmiDiskDrivesArray
    {
        private readonly static DataContractJsonSerializer _dataSerializer = new DataContractJsonSerializer( typeof( WmiDiskDrive ) );

        public List<WmiDiskDrive> DiskDrives { get; private set; }

        public string JsonKey => string.Join( "", DiskDrives.Select( (item) => item.JsonKey ).ToArray() );

        public static WmiDiskDrivesArray Query ()  => new WmiDiskDrivesArray( WmiDiskDrive.Query() );

        public string GetJson ()
        {
            StringBuilder json = new StringBuilder("{");
            for (int i = 0; i < DiskDrives.Count; i++) 
            {
                MemoryStream dataStream = new MemoryStream();
                _dataSerializer.WriteObject( dataStream, DiskDrives[i] );
                if (i > 0)
                    json.Append( "," );

                string jsonElement = Encoding.UTF8.GetString( dataStream.ToArray() );
                json.Append( $"\"{i}\":{jsonElement}" );
            }
            json.Append( "}" );
            return json.ToString();
        }

        private WmiDiskDrivesArray (IEnumerable<WmiDiskDrive> diskDrivesArray)
        {
            DiskDrives = new List<WmiDiskDrive>( diskDrivesArray );
        }
    }
}
