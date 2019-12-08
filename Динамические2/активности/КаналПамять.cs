using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Динамические.активности
{
    public class КаналПамять: Канал
    {
        public КаналПамять()
        {
            ПередайИсходящееСообщение = ПередайСообщениеВПоток;
            //поток = new MemoryStream(100);// буфер 100 байт
        }

        private MemoryStream ВходящийПоток;
        private MemoryStream ИсходящийПоток;

        public override void Регистрация()
        {
            ВходящийПоток = new MemoryStream(100);//это возможность подключиться удаленному каналу
        }

        public override bool УстановитьСвязь()
        {
            var каналУдСвязи =(КаналПамять) ((Связь) АдресУдаленнойСвязи.АдресВКучеПамяти()).Канал;
            if (каналУдСвязи.ИсходящийПоток == null)
            {
                ВходящийПоток = new MemoryStream(100);// буфер 100 байт
                каналУдСвязи.ИсходящийПоток = ВходящийПоток; // Дуплекс и один буфер (как в карте неизвестно)
                // если удаленный Исходящий не пуст, то оборвется существующая связь 
                ИсходящийПоток = каналУдСвязи.ВходящийПоток;
                return true;
            }
            else
            {
                return true;
            }
        }

        public override РАМОбъект ДайВходящееСообщение()
        {
            return Создать(ВходящийПоток);
        }

        public void ПередайСообщениеВПоток(РАМОбъект сообщение)
        {
            сообщение.СохранисьВ(ИсходящийПоток);
        }

        public override void Стоп()
        {
            
        }

    }
}
