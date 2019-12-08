using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Динамические;

namespace Виды
{
    public class ВыборВидов1
    {
        public Dictionary<Type, Type> ВыборВидов;
        public ОбщийВид ВыбратьВид(ПримитивИлиАдрес объект)
        {
            // если это группа то выбрать свернутый вариант
            var тип = ВыборВидов.ContainsKey(объект.GetType()) ? ВыборВидов[объект.GetType()] : typeof(ОбщийВид);
            var вид = (ОбщийВид)тип.GetConstructor(Type.EmptyTypes).Invoke(null);

            if (тип == typeof(ВидГруппаОбъектов)) ((ВидГруппаОбъектов)вид).ВыборВидов = ВыборВидов;

            вид.Объект = объект;

            return вид;
        }
    }
}
