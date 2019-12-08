using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Динамические
{
    //все примитивы это списки байт 
    public class СписокБайт : Примитив
    {
        public List<byte> Список;
        public override int Длина()
        {
            return Список.Count;
        }
        

        public override void СохранисьВ(BinaryWriter писатель)
        {
            писатель.Write(Длина());
            писатель.Write(Список.ToArray());
        }

        public override ПримитивИлиАдрес Восстановить(BinaryReader читатель)
        {
            var длина = читатель.ReadInt32(); // адрес 4 байта
            
            Список = читатель.ReadBytes(длина).ToList();

            return this;
        }
    }

    public abstract class ШаблонБайт
    {
        public abstract byte ПроверитьСовпадение(byte значение);
    }

    public class ШаблонИлиБайт : ШаблонБайт//узел выбора - группировка = концепция = условие ИЛИ
    {
        public string Имя; 
        public List<ШаблонУсловиеСовпаденияБайт> ДоступныеВарианты;
        public override byte ПроверитьСовпадение(byte значение)
        {
            foreach (var вар in ДоступныеВарианты)
            {
                if (вар.ПроверитьСовпадение(значение)==1)
                {
                    return 1;
                }
            }
            return 0;
        }

    }


    public class ШаблонУсловиеСовпаденияБайт :ШаблонБайт
    {
        public string Имя; 
        public byte ДоступныеВарианты;
        public override byte ПроверитьСовпадение(byte значение)
        {
            if (значение == ДоступныеВарианты) return 1;
            return 0;
        }

    }
}
