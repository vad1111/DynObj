using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Динамические;

namespace Динамические2
{
    //возможно это даже структуры
    //
    // возможно просто надо расширить существующие типы
    // это примитивы в  раме без собственного адреса их адрес в куче

    public class Суперпримитивы
    {
        public static Dictionary<Type,byte> КодыТипов= new Dictionary<Type, byte>()
        {
            {typeof(Целое), 1},
            {typeof(АдресЦелое), 2},
            {typeof(АдресСтрокаЮникода), 3},

        };
        public virtual void Сохранить( Адрес куда) { } // это может быть Stream, BinaryReader, out список байт
        public virtual void Сохранить(Суперпримитивы вочто) { }
        public virtual void Восстановить( Адрес откуда, Адрес куда){ }
        public virtual int ДлинаБит() => 8;
    }

    public class Бит : Суперпримитивы
    {
        public byte Значение; //0 или 1
        public override int ДлинаБит() => 1;
    }
    public class Байт : Суперпримитивы // это группаБит фиксированой длины
    {
        public sbyte Значение;
        public override int ДлинаБит() => 8;
        public Бит this[byte индекс]
        {
            get => new Бит(){Значение = (byte)(Значение >> индекс & 1) };
            set
            {
                if(value.Значение==1)
                Значение = (sbyte) (Значение | (sbyte) (1 << value.Значение));
                else
                    Значение = (sbyte)(Значение & (sbyte)~(1 << value.Значение));

            }
        }

        public string ВСтроку()
        {
            string с = "";
            for (int i = ДлинаБит()-1; i > -1; i--)
            {
                с += Значение >> i & 1;
            }
            return с;
        }
    }

    //это значение только в памяти
    public class Целое: Суперпримитивы // это группаБит
    {
        public int Значение;
        public override int ДлинаБит() => 32;
    }
    public class ЦелоеРазделяемоеЗащита  // 
    {
        public class ДанныеСподписью
        {
            public object ОткрытыйКлюч;
            public object Владелец;
            public object Данные;
            public object Подпись;

            public bool ПроверитьДанные()
            {
                var хеш = Расшифровать(ОткрытыйКлюч, Подпись);
                var расчетныйхеш = Расчитать(Данные, Владелец);
                if (хеш == расчетныйхеш) return true;
                return false;
            }

            public static long Расчитать(object данные, object владелец)
            {
                throw new NotImplementedException();
            }

            public  object Подписать( object закКлюч)
            {
                var хеш = Расчитать(Данные, Владелец);
                Подпись = Зашифровать(хеш, закКлюч);
                throw new NotImplementedException();
            }

            private object Зашифровать(object хэш, object закКлюч)
            {
                throw new NotImplementedException();
            }

            public static long Расшифровать(object открытыйКлюч, object подпись)
            {
                throw new NotImplementedException();
            }
        }
        public object ОткрытыйКлюч;
        public object закрытыйКлюч;
        public object Владелец;
        private int _значение;
        private int счетчик;
        private DateTime Времязменения; // на другом компе время может ытть несинхрон
        private bool замокПростой;


        public int Значение
        {
            get { return _значение; }
            set
            {
                if (замокПростой) return;
                замокПростой = true;
                _значение = value;
                счетчик++;

                замокПростой = false;
            }
        }

        public ДанныеСподписью ДайДанныеСподписью()
        {
            var данные= new ДанныеСподписью()
            {
                Владелец = Владелец, Данные = (_значение, счетчик),ОткрытыйКлюч = ОткрытыйКлюч,
            };
            данные.Подписать(закрытыйКлюч);
            return данные;
        }

       

        public void СинхронЗначение(int счетч, int значНовое)
        {
            if(счетч<=счетчик) return;
            Значение = значНовое;
        }
    }
    // целое общее несколько хозяев, у всех храняться все подписи
    // необходимо несколько подписей.(все 2\3 1\2)
    // запрос всем хозявам на внесение изменений
    // прием запроса и блокировка на запросы
    // хозяеа подписывают и присылают инициатору данные с подписью
    // инициатор собирает все подписи, если их необходимое количество меняет значение
    // сообщает всем и пересылает все подписи
    //либо каждые хозяин подписывает и раасылает всем свою подпись кроме инициатора
    //
    public class ЦелоеСсылка // это ссылка прокси
    {
        public Целое АдресВРАМЗначения;
        public bool РазрЧтение;
        public bool РазрЗапись;
        public int Значение
        {
            get
            { if( РазрЧтение)
                return АдресВРАМЗначения.Значение;
              else  throw  new Exception("нет разрешения на чтение");
            }
            set { if (РазрЗапись) АдресВРАМЗначения.Значение=value; else throw new Exception("нет разрешения на запись"); }
        }
    }
    public struct ЦелоеСсылкаСКэшем // это ссылка прокси
    {
        private int _значение;
        public Целое АдресВРАМЗначения;
        private bool считано;
        private bool синхронизировано;
        public bool всегдасохранять;
        public bool РазрЧтение;
        public bool РазрЗапись;
        public int Значение
        {
            get { return считано ? _значение : Считать(); }
            set { _значение = value; if (РазрЗапись && всегдасохранять) СохранитьКэш(); }
        }
        private int Считать()
        {
            try
            {
                _значение = АдресВРАМЗначения.Значение; // здесь копирование т.к. это структура
                считано = синхронизировано = true;
                return _значение;
            }
            catch (Exception e) { считано = синхронизировано = false; }

            return 0;
        }
        public bool СохранитьКэш()
        {
            if (!РазрЗапись) return false;

            try
            {
                АдресВРАМЗначения.Значение = _значение;
                синхронизировано = true;
            }
            catch (Exception e) { синхронизировано = false; }

            return синхронизировано;
        }
    }


    //это значение только на диске
    public class ЦелоеНаДиске // это ссылка прокси
    {
        public string файл;
        public bool РазрЧтение;
        public bool РазрЗапись;
        public int Значение
        {
            get => new BinaryReader(File.OpenRead(файл)).ReadInt32();
            set { if(РазрЗапись) new BinaryWriter(File.OpenWrite(файл)).Write(value);}
        }

    }

    //это значение только на диске
    public class ЦелоеНаДискеДубли // это ссылка прокси
    {
        public string[] файлы;
        public bool РазрЧтение;
        public bool РазрЗапись;
        public int Значение
        {
            get
            {
                var ис= new Dictionary<int,int>();
                foreach (var файл in файлы)
                {
                    var значение=new BinaryReader(File.OpenRead(файл)).ReadInt32();
                    if (ис.ContainsKey(значение)) ис[значение]++;
                    ис[значение] = 1;
                }
               
                if( ис.Count==1)
                    return ис.GetEnumerator().Current.Key;
                else throw new Exception("несовпадений"+ис.Count);
            }
            set { if (РазрЗапись) new BinaryWriter(File.OpenWrite(файлы[0])).Write(value); }
        }
    }
    //это значение только на диске в монопольом доступе
    public class ЦелоеНаДискеСКэшем 
    {
        public string файл;
        private int _РАМЗначение;
        private bool считано;
        private bool синхронизировано;
        public bool всегдасохранять; //можно это ввыкинуть добавить разные пведения
        public bool РазрЧтение;
        public bool РазрЗапись;

        public int Значение
        {
            get { return считано?_РАМЗначение : Считать(); }
            set { _РАМЗначение = value; if(РазрЗапись && всегдасохранять) СохранитьКэш(); }
        } 

        private int Считать()
        {
            try{
                _РАМЗначение = new BinaryReader(File.OpenRead(файл)).ReadInt32();
                считано = синхронизировано = true;
                return _РАМЗначение;
            }
            catch (Exception e) { считано= синхронизировано = false; }

            return 0;
        }
       
        //можно несклько раз менять РАМ, потом сохранить, риск потери данных
        public bool СохранитьКэш()
        {
            if (!РазрЗапись) return false;

            try { new BinaryWriter(File.OpenWrite(файл)).Write(_РАМЗначение);
                синхронизировано = true;
            }
            catch(Exception e) { синхронизировано = false;}
            
            return синхронизировано;
        }
    }



    //это значение только в сети
    public class ЦелоеВСети  // это группаБит
    {
        public ТСПСвязьИсходящая адресВсети;
        public int Значение
        {
            get => new BinaryReader(адресВсети.поток).ReadInt32();
            set => new BinaryWriter(адресВсети.поток).Write(value);
        }
    }

    public class ТСПКлиент
    {
        public TcpClient клиент;
        public BinaryWriter потокЗапись;
        public BinaryReader потокЧтение;
        public bool ОфрмленаПодписка;
    }
    public class ЦелоеВСетиСервер  // 
    {
        public ТСПСвязьВходящая Сервер;
        public Целое Значение;
        public List<ТСПКлиент> Клиенты;

        public Dictionary<byte, Action> Реакции;
        
        public void Запуск()
        {
            Сервер.Старт(); // здесь создаются клиенты

        }

        public void ОчередьОбработкиКоиентов()
        {
            while (true)
            {
                foreach (var клиент in Клиенты)
                {
                    if (клиент.клиент.Available != 0)
                    {
                        var сообщение = клиент.потокЧтение.ReadByte();
                        //если сообщение закончено?
                        var вОчередь = (клиент, сообщение);// реакция не здесь а единой очереди
                        ОчередьВопросов.Enqueue(вОчередь);
                    }
                }
            }
        }

        public Queue<(ТСПКлиент, byte)> ОчередьВопросов;

        public void ОтветыНаВопросы()
        {
            while (true)
            {
                //if (ОчередьВопросов.Count != 0)
                var вопрос = ОчередьВопросов.Dequeue();
                var реакц = Реакции[вопрос.Item2];
                реакц();
            }
        }

        public void Протокол(ТСПКлиент клиент)
        {
            var работает = true;

            void ДайЗначение()
            {
                клиент.потокЗапись.Write(Значение.Значение);
            }

            void НаЗначение()
            {
                Значение.Значение = клиент.потокЧтение.ReadInt32();
                клиент.потокЗапись.Write("ОК");
                ОповеститьВсехПодписчиков();
            }
            void Откл()
            { клиент.клиент.Close();
                работает = false;
            }
            Реакции = new Dictionary<byte, Action>()
            {
                {0, ДайЗначение},
                {1, Откл },
                {2, НаЗначение }
            };

            while (работает)
            {
                var сообщение = клиент.потокЧтение.ReadByte();
                var вОчередь = (клиент, сообщение);// реакция не здесь а единой очереди
                var реакц = Реакции[сообщение];
                реакц();
            }
            

        }

        private void ОповеститьВсехПодписчиков()
        {
            throw new NotImplementedException();
        }
    }

    public class РасширениеУНП
    {
        public ЦелоеВСетиСервер Сервер;
        public ТСПКлиент Клиент;
        public Guid УНП; //уникальный номер простраснства, по нему можно узнать ресурс типа магнет-ссылка
        public void ДайУНН()
        { Клиент.потокЗапись.Write(УНП.ToByteArray());}

        public void Инит()
        {
            Сервер.Реакции[2] = ДайУНН;
        }
    }


    public class Адрес: Суперпримитивы
    { }
    public class АдресОтносительный : Суперпримитивы
    { }

    public class АдресЦелое : АдресОтносительный
    {
        public int Значение;
    }

    public class АдресСтрокаЮникода : АдресОтносительный
    {
        public string Значение;
    }

}
