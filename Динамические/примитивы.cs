using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Динамические
{
    //Простейшие объекты

   // Примитивы 

    public  class ПримитивИлиАдрес{
        public Адрес СобственныйАдресПримитива; // это обобщение просто номера в группе, может быть имя , адрес:порт
        //public List<Адрес> СписокСобственныхАдресов; // группа может быть зарегистрирована в разных группах
        public static List<РеакцияНаКоманду> РеакцииНаКоманды; // здесь зарлняются имя команды и Исполнение
        public object Вид; //это типа ToString()- добавить ссылку на сборку
        //public Словарь ДополнительныеПоля; // некоторые поля могут дублировать друг друга, нужна методика их выявления, сравнение структур , сравнение по обобщению
        //public event Func<ПримитивИлиАдрес,ПримитивИлиАдрес> РеакцииНаСобытия ;
        public List<Команда> ДайКоманды
        {
            get
            {
                var списокКоманд = new List<Команда>();
                foreach (var реакция in РеакцииНаКоманды)
                {
                    списокКоманд.Add(реакция.ШаблонКоманды);
                }

                return списокКоманд;
            }
        }

        public virtual ПримитивИлиАдрес ВыполнитьКоманду(Команда команда)
        {
            var реакция = РеакцииНаКоманды.Find(реакц => реакц.Имя.Значение == команда.Имя.Значение);

            return реакция.Выполнить(this,команда); // выполнение на конкретном экземпляре 
        }

        public static ПримитивИлиАдрес Создать(Type тип)
        {
            return (ПримитивИлиАдрес)тип.GetConstructor(Type.EmptyTypes).Invoke(null);
        }
        /// <summary>
        /// Создает все примитивы и адреса из универсального потока
        /// </summary>
        /// <param name="поток"></param>
        /// <returns></returns>
        public static ПримитивИлиАдрес Создать(Stream поток)
        {
            var номерТипа = поток.ReadByte();
            var тип = Хранилище.Типы[номерТипа];
            ПримитивИлиАдрес примитив = (ПримитивИлиАдрес)тип.GetConstructor(Type.EmptyTypes).Invoke(null);

            примитив.Восстановить(поток);
            return примитив;
        }
        
        public virtual int Длина()
        {
            return 1; // это длина кода Примитива в байтах
        }
        public virtual void СохранисьВ(BinaryWriter писатель)
        {
            писатель.Write(Хранилище.кодыТипов[this.GetType()]); // первое всегда сохраняется код типа
        }

        public virtual void СохранисьВ(Stream поток)
        {
            поток.WriteByte(Хранилище.кодыТипов[this.GetType()]);
        }
        public virtual  ПримитивИлиАдрес  Восстановить(BinaryReader читатель) // восстанавливает пустой примитив или адрес
        {
            var номерТипа = читатель.ReadByte();
            var тип = Хранилище.Типы[номерТипа];
            ПримитивИлиАдрес примитив = (ПримитивИлиАдрес)тип.GetConstructor(Type.EmptyTypes).Invoke(null);
            примитив.Восстановить(читатель);
            return примитив;
        }

        /// <summary>
        /// В производном классе возвращается сам объект (this) восстанавливается только значения 
        /// используется в стат методt Соpдать из универсального потока 
        /// </summary>
        /// <param name="поток"></param>
        /// <returns></returns>
        public  virtual ПримитивИлиАдрес Восстановить(Stream поток) //универсальный поток
        {
            var номерТипа = поток.ReadByte();
            var тип = Хранилище.Типы[номерТипа];
            ПримитивИлиАдрес примитив = (ПримитивИлиАдрес)тип.GetConstructor(Type.EmptyTypes).Invoke(null);

            return this;
        }
        public virtual byte[] СохранитьВБайты()
        {
            List<byte> байты = new List<byte>(); //сохраняется код типа
            байты.Add(Хранилище.кодыТипов[this.GetType()]);

            return байты.ToArray();
        }

        public virtual ПримитивИлиАдрес ВостанвитьИзБайтов(byte[] байты)
        {
            var номерТипа = байты[0];
            var тип = Хранилище.Типы[номерТипа];
            ПримитивИлиАдрес примитив = (ПримитивИлиАдрес)тип.GetConstructor(Type.EmptyTypes).Invoke(null);
            return примитив;
        }


        public virtual ПримитивИлиАдрес Копировать()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Примитив : ПримитивИлиАдрес
    {
        //у притивов должны быть только операции сложения , преобразования из строки и обратно
        // тИп или номер Типа в Хранилище
        public virtual Строка ВСтроку()
        {
            return new Строка(ToString());
        }

        public Адрес АдресТипа()
        {
            return new АдресВХранилище() {НомерВХранилище = Хранилище.кодыТипов[this.GetType()]};
        }
        
    }

    public class Пустота : Примитив // можно добавить длину пустоте
    {
        internal static readonly ПримитивИлиАдрес Статик;

        public override ПримитивИлиАдрес Восстановить(BinaryReader читатель)
        {
           return this;
        }

        public override string ToString()
        {
            return  "пустота";
        }
        
    }
    // Типы ПРимитив - Число (Целое - целое фиксировано1 длины - ) дробное, действительное, комплескное и т.д.
    // целое является числом и только, можно скрыть, 
    //имеет подкатегории фикированое (1.2.4.8 байт), нефиксированной длины
    // двоичное, воьмиричное, десятичное, - это форма представления по основанию, отсылка к группе символов
    // 
   
    
    public class ЦелоеЧисло : Примитив
    {
        public static Строка Описание = new Строка() { Значение = "целое число со знаком, длина 4 байта" };

        static ЦелоеЧисло()
        {
            РеакцииНаКоманды = new List<РеакцияНаКоманду>();
            var ком = new РеакцияНаКоманду() { Имя = "ДайЗначение", ШаблонКоманды = new Команда() { Имя = new Строка("ДайЗначение") }, Инструкция = typeof(ЦелоеЧисло).GetMethod("ДайЗначение") };
            РеакцииНаКоманды.Add(ком); // В кленте выполнение команды замениться установление связи и отсылку команды серверу
            ком = new РеакцияНаКоманду() { Имя = "НаЗначение", Инструкция = typeof(ЦелоеЧисло).GetMethod("НаЗначение") };
            РеакцииНаКоманды.Add(ком);
        }
        public ЦелоеЧисло()
        {
        }
        public ЦелоеЧисло(int число)
        {
            Значение = число;
        }
        public override int Длина()
        {
            return base.Длина()+4;
        }

        public static ЦелоеЧисло Создать()
        {
            var этот= new ЦелоеЧисло();
            var адрес=Хранилище.Добавить(этот);
            return этот;
        }
        public int Значение;
        public override ПримитивИлиАдрес Копировать()
        {
            var  копия = new ЦелоеЧисло();
            копия.Значение = Значение;
            копия.СобственныйАдресПримитива = СобственныйАдресПримитива;
            return копия ;
        }

        //public override object ДайЗначение()
        //{
        //    return Значение;
        //}

        //public override object НаЗначение(object значение)
        //{
        //    if (значение is int)
        //    {
        //        Значение = (int) значение; // проверка чтобы знчение было целым + преобразование типов 
        //        return true;
        //    }
                
        //    return false;
        //}

        public override void СохранисьВ(BinaryWriter писатель)
        {
            писатель.Write(Значение);
        }

        public override void СохранисьВ(Stream поток)
        {
            base.СохранисьВ(поток);
            var буфер = BitConverter.GetBytes(Значение);
            поток.Write(буфер,0, 4);

        }

        public override byte[] СохранитьВБайты()
        {
            var байты = base.СохранитьВБайты().ToList();
            байты.AddRange(BitConverter.GetBytes(Значение));
            return байты.ToArray();

        }

        public void СохранитьВпоток(Stream поток)
        {
            поток.Write(СохранитьВБайты(),0,Длина());
        }

        public override ПримитивИлиАдрес Восстановить(BinaryReader читатель)
        {
            Значение = читатель.ReadInt32();
            return this;
        }

        public override ПримитивИлиАдрес Восстановить(Stream поток)
        {
            //var  примитив = (ЦелоеЧисло) base.Восстановить(поток);
            byte[] буфер = new byte[4];
            поток.Read(буфер, 0, 4); //0 - смещение в буфере
            Значение = BitConverter.ToInt32(буфер, 0);
            return this;
        }

        public ПримитивИлиАдрес ВостановитьИзПотока(Stream поток)
        {
            byte[] буфер= new byte[Длина()];
            поток.Read(буфер, 0, Длина()); //0 - смещение в буфере
            var примитив = ВостанвитьИзБайтов(буфер);
            return примитив;
        }

        public override ПримитивИлиАдрес ВостанвитьИзБайтов(byte[] байты) //байты вместе с кодом
        {
            var примитив = (ЦелоеЧисло)base.ВостанвитьИзБайтов(байты);
            примитив.Значение = BitConverter.ToInt32(байты, 1);
            return примитив;
        }
        public static ЦелоеЧисло operator +(ЦелоеЧисло с1, ЦелоеЧисло с2)
        {
            return new ЦелоеЧисло(с1.Значение + с2.Значение);
        }
        public static ЦелоеЧисло operator -(ЦелоеЧисло с1, ЦелоеЧисло с2)
        {
            return new ЦелоеЧисло(с1.Значение - с2.Значение);
        }
    }

  

    public class Строка                     :Примитив
    {
        
        public static Строка Создать(string строка)
        {
            var этот= new Строка(){ Значение = строка};
            Хранилище.Добавить(этот);
            return этот;
           
        }
        public Строка() { }
        public Строка(string строка)
        {
            Значение = строка;
        }

        public override int Длина() // проблема все зависит от кодировки сохранения
        {
            return base.Длина()+Значение.Length*2; //2 байта на символ сохранение в UTF8
        }

        public static string Описание = "строка";

        public string Значение; //здесь значение всегда в Unicode

        public override void СохранисьВ(BinaryWriter писатель)
         {
            писатель.Write(Значение);
        }

        public override void СохранисьВ(Stream поток)
        {
            base.СохранисьВ(поток);
            var байтыЗначение = Encoding.Unicode.GetBytes(Значение);
            var длина = (ushort) байтыЗначение.Length; // макс длина 65000

            var буфер = BitConverter.GetBytes(длина);
            поток.Write(буфер, 0, 2);
            поток.Write(байтыЗначение,0,длина);

        }

        public override byte[] СохранитьВБайты()
        {
            var байтыОсновы= base.СохранитьВБайты(); //запись кода типа
             
            var байтыЗначение = Encoding.Unicode.GetBytes(Значение);
            int длина = байтыЗначение.Length;
            var длинавБайтах = BitConverter.GetBytes(длина); // запись длины в виде массива байтов

            byte[] байтыРезультат= new byte[1+длинавБайтах.Length+ байтыЗначение.Length];

            байтыРезультат[0] = байтыОсновы[0];
            длинавБайтах.CopyTo(байтыРезультат,1);
            байтыЗначение.CopyTo(байтыРезультат,1+длинавБайтах.Length);
            return байтыРезультат;


        }

        public override ПримитивИлиАдрес ВостанвитьИзБайтов(byte[] байты)
        {
            var примитив=(Строка) base.ВостанвитьИзБайтов(байты);
            var длина = BitConverter.ToInt32(байты, 1);
            примитив.Значение = Encoding.Unicode.GetString(байты, 1, байты.Length - 1);
            return примитив;

        }

        public override ПримитивИлиАдрес Восстановить(Stream поток)
        {
            
            byte[] буфер = new byte[2];
            поток.Read(буфер, 0, 2);
            var длина= BitConverter.ToUInt16(буфер, 0);
            буфер = new byte[длина];
            поток.Read(буфер, 0, длина);
            Значение = Encoding.Unicode.GetString(буфер, 0, длина);
            return this;
        }

        public override ПримитивИлиАдрес Восстановить(BinaryReader читатель)
        {
            Значение = читатель.ReadString();
            return this;
        }
        public override string ToString()
        {
            return СобственныйАдресПримитива+": "+Значение;
        }

        public static Строка operator +(Строка с1, Строка с2)
        {
            return new Строка(с1.Значение + с2.Значение);
        }
        public static bool operator ==(Строка с1, Строка с2)
        {
            return с1.Значение == с2.Значение;
        }
        public static bool operator !=(Строка с1, Строка с2)
        {
            return с1.Значение != с2.Значение;
        }

        public static implicit operator Строка(string c)
        {
            return new Строка(c);
        }
        public static implicit operator string(Строка c)
        {
            return c.Значение;
        }
    }

    

    
}
