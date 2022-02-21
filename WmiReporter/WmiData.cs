using System;
using System.Text;
using System.Security.Cryptography;

namespace WmiReporter
{
    internal class WmiData
    {
        public WmiProcessor Processor { get; }
        public WmiMemoryArray Memory { get; }
        public WmiMotherboard Motherboard { get; }
        public WmiBios Bios { get; }
        public WmiComputerSystem ComputerSystem { get; }
        public WmiOperatingSystem OperatingSystem { get; }
        public WmiDiskDrivesArray DiskDrives { get; }
        public WmiLogicalDiscsArray LogicalDiscs { get; }
        public WmiNetworkAdaptersArray NetworkAdapters { get; }

        public WmiData ()
        {
            Processor = WmiProcessor.Query();
            Memory = WmiMemoryArray.Query();
            Motherboard = WmiMotherboard.Query();
            Bios = WmiBios.Query();
            ComputerSystem = WmiComputerSystem.Query();
            OperatingSystem = WmiOperatingSystem.Query();
            DiskDrives = WmiDiskDrivesArray.Query();
            LogicalDiscs = WmiLogicalDiscsArray.Query();
            NetworkAdapters = WmiNetworkAdaptersArray.Query();
        }

        public string GetJson ()
        {
            StringBuilder json = new StringBuilder( "{" );
            json.Append( "\"mb\":" );
            json.Append( Motherboard.GetJson() );
            json.Append( ",\"os\":" );
            json.Append( OperatingSystem.GetJson() );
            json.Append( ",\"net\":" );
            json.Append( NetworkAdapters.GetJson() );
            json.Append( ",\"sys\":" );
            json.Append( ComputerSystem.GetJson() );
            json.Append( ",\"bios\":" );
            json.Append( Bios.GetJson() );
            json.Append( ",\"disk\":" );
            json.Append( LogicalDiscs.GetJson() );
            json.Append( ",\"proc\":" );
            json.Append( Processor.GetJson() );
            json.Append( ",\"status\": 1" );
            json.Append( ",\"diskdev\":" );
            json.Append( DiskDrives.GetJson() );

            if (Memory != null) {
                string memoryJson = Memory.GetJson();
                if (memoryJson.Length > 0) {
                    json.Append( ",\"memory\":" );
                    json.Append( memoryJson );
                }
            }

            StringBuilder jsonKey = new StringBuilder();
            jsonKey.Append( Processor.JsonKey );
            if (Memory != null)
                jsonKey.Append( Memory.JsonKey );
            jsonKey.Append( Motherboard.JsonKey );
            jsonKey.Append( DiskDrives.JsonKey );

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash( Encoding.ASCII.GetBytes( jsonKey.ToString() ) );
            string hashString = BitConverter.ToString( hash ).Replace( "-", "" ).ToLower();
            json.Append( $",\"json_key\": \"{hashString}\"" );
            json.Append( "}" );

            return json.ToString();
        }
    }
}
