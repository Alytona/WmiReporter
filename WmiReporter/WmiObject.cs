using System.Management;

namespace WmiReporter
{
    internal class WmiObject
    {
        private readonly ManagementBaseObject _managementObject;

        public PropertyDataCollection Properties => _managementObject.Properties;

        public WmiObject (ManagementBaseObject managementObject)
        {
            _managementObject = managementObject;
        }

        public T GetValue<T> (string name)
        {
            return (T)GetPropertyValue( name );
        }
        public string GetValue (string name)
        {
            return ((string)GetPropertyValue( name ))?.Trim();
        }

        private object GetPropertyValue (string name)
        {
            try
            {
                return _managementObject[name];
            }
            catch (ManagementException)
            {
                return null;
            }
        }
    }
}
