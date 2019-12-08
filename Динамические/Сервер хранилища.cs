using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Динамические
{
    public class Сервер_хранилища
    {
        public Хранилище Хран;
        private TcpListener Слушатель;
        public IPAddress IPАдрес = IPAddress.Parse("127.0.0.1");
        public int порт=101;


        public void ЗапуститьСервер()
        {
            Слушатель = new TcpListener(new IPEndPoint(IPАдрес, порт));
            Слушатель.Start();

            // Buffer for reading data
            //Byte[] буферBytes = new Byte[256];
            //String data = null;

            while (true)
            {
                
                TcpClient клиент = Слушатель.AcceptTcpClient();

                // Код после подключения

                var адресПодключившегося = (IPEndPoint) клиент.Client.RemoteEndPoint ;
                var IP = адресПодключившегося.Address; // здесь можно отфильтровать входящие соединения


                
                NetworkStream поток = клиент.GetStream();
                var читатель = new BinaryReader(поток);

                // протокол обмена сообщениями
                var команда = читатель.ReadString();
                var числоПараметров = читатель.ReadByte();
                object[] параметры = new object[числоПараметров];
                for (int j = 0; j < числоПараметров; j++)
                {
                    var номерТипа = читатель.ReadByte();
                    var тип = Динамические.Хранилище.Типы[номерТипа];
                    ПримитивИлиАдрес примитив = (ПримитивИлиАдрес) тип.GetConstructor(Type.EmptyTypes).Invoke(null);
                    примитив.Восстановить(читатель);

                    параметры[j] = примитив;
                }
                
                object результат = typeof(Хранилище).InvokeMember(команда, BindingFlags.InvokeMethod, null, Хран, параметры);

                var писатель = new BinaryWriter(поток);

                ((ПримитивИлиАдрес) результат).СохранисьВ(писатель);


                //byte[] результатВбаты =
                //поток.Write(результатВбаты,0,результатВбаты.Length);




                //int i;

                //// Loop to receive all the data sent by the client.
                //while ((i = поток.Read(буферBytes, 0, буферBytes.Length)) != 0)
                //{

                //    // Translate data bytes to a ASCII string.
                //    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                //    Console.WriteLine("Received: {0}", data);

                //    // Process the data sent by the client.
                //    data = data.ToUpper();

                //    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                //    // Send back a response.
                //    поток.Write(msg, 0, msg.Length);
                //}

                // Shutdown and end connection
                клиент.Close();
            }
        }
    }
}
