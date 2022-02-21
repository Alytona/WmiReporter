using System.IO;
using System.Management;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WmiReporter
{
    [DataContract]
    internal class WmiMotherboard
    {
        private readonly static DataContractJsonSerializer _serializer = new DataContractJsonSerializer( typeof( WmiMotherboard ) );

        [DataMember( Order = 0 )] public string SKU { get; private set; }
        [DataMember( Order = 1 )] public string Name { get; private set; }
        [DataMember( Order = 2 )] public string Model { get; private set; }
        [DataMember( Order = 3 )] public string Caption { get; private set; }
        [DataMember( Order = 4 )] public string Product { get; private set; }
        [DataMember( Order = 5 )] public string Version { get; private set; }
        [DataMember( Order = 6 )] public string PartNumber { get; private set; }
        [DataMember( Order = 7 )] public string Description { get; private set; }
        [DataMember( Order = 8 )] public string Manufacturer { get; private set; }
        [DataMember( Order = 9 )] public string SerialNumber { get; private set; }
        public string JsonKey => PartNumber ?? "" + Manufacturer ?? "" + SerialNumber ?? "" + Product ?? "";
        public static WmiMotherboard Query ()
        {
            ManagementObjectCollection mosQueryCollection = new ManagementObjectSearcher(
                "SELECT Caption, Description, Manufacturer, Model, Name, PartNumber, Product, SerialNumber, SKU, Version FROM Win32_BaseBoard" ).Get();

            foreach (ManagementBaseObject mboard in mosQueryCollection)
                return new WmiMotherboard( new WmiObject( mboard ) );
            return null;
        }
        public string GetJson ()
        {
            MemoryStream dataStream = new MemoryStream();
            _serializer.WriteObject( dataStream, this );

            string json = Encoding.UTF8.GetString( dataStream.ToArray() );
            return json;
        }

        private WmiMotherboard (WmiObject wmiObject)
        {
            Caption = wmiObject.GetValue( "Caption" );
            Description = wmiObject.GetValue( "Description" );
            Manufacturer = wmiObject.GetValue( "Manufacturer" );
            Name = wmiObject.GetValue( "Name" );
            PartNumber = wmiObject.GetValue( "PartNumber" );
            Model = wmiObject.GetValue( "Model" );
            Product = wmiObject.GetValue( "Product" );
            SerialNumber = wmiObject.GetValue( "SerialNumber" );
            SKU = wmiObject.GetValue( "SKU" );
            Version = wmiObject.GetValue( "Version" );
        }
    }
}
