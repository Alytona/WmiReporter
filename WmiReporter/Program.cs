using System;
using System.IO;
using System.Net;
using System.Text;

namespace WmiReporter
{
    public static class WmiReporter
    {
        public static void Main ( string[] args )
        {
            Console.WriteLine();
            Console.WriteLine( " *** Alytona, 2022 г. ***" );
            Console.WriteLine();
            Console.WriteLine( " Утилита для сбора информации об оборудовании." );
            Console.WriteLine( " Версия 1.05" );
            Console.WriteLine();

            try
            {
                Uri uri = new Uri( args.Length > 0 ? args[0] : "http://dbs.asu.net/wmi/wmi_add.php" );
                Report( uri );
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine( " " + ex.ToString() );
            }
#if DEBUG            
            Console.WriteLine();
            Console.WriteLine( " Нажмите любую клавишу." );
            Console.ReadKey( true );
#endif
        }

        internal static void Report ( Uri uri )
        {
            WmiData data = new WmiData();

            string dataString = data.GetJson();

            Console.WriteLine( " Данные успешно собраны." );
            Console.WriteLine( dataString );

            byte[] dataToSend = Encoding.UTF8.GetBytes( "json=" + Uri.EscapeDataString( dataString ) );

            Console.WriteLine();
            Console.WriteLine( $" Отправка данных ({uri.AbsoluteUri})." );
            sendData( uri, dataToSend );
        }

        private static void sendData (Uri uri, byte[] data)
        {
            var request = (System.Net.HttpWebRequest)WebRequest.Create( uri );
            request.ProtocolVersion = HttpVersion.Version11;
            request.KeepAlive = true;   // Использовать ранее установленное соединение, вместо того, чтобы каждый раз устанавливать новое
            request.Pipelined = true;
            request.Timeout = 3000;
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Proxy = null;

            using (Stream sw = request.GetRequestStream())
            {
                sw.Write( data, 0, data.Length );
            }

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                Console.WriteLine( $" Сервер вернул ошибку {response.StatusCode}." );
            else
            {
                Console.WriteLine( " Успешно отправлено." );
                if (response.ContentLength > 0)
                {
                    Console.WriteLine( $" Получены данные, размер - {response.ContentLength}." );

                    byte[] responseData = new byte[response.ContentLength];
                    Stream responseStream = response.GetResponseStream();
                    responseStream.Read( responseData, 0, (int)response.ContentLength );
                    string responseString = Encoding.UTF8.GetString( responseData );

                    Console.WriteLine( " Ответ:" );
                    Console.WriteLine( responseString );
                    Console.WriteLine( " ===" );
                }
            }
        }
    }
}