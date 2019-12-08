using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Виды;

namespace Динамические.Виды
{
    public class ВидЗначениеАдреса: ОбщийВид
    {
        private ПроксиОбъекта элемент => (ПроксиОбъекта) Объект;
        public override void ДобавьСодержание(РАМОбъект объект)
        {
            var эл = объект as ПроксиОбъекта;
            if(эл==null) return;
            _объект = эл;
            var значение = эл.УдаленныйПримитив;

            var вид = ВыбратьВид(значение);
            Content = вид;

        }

        public override void ОбновитьВид()
        {
        }

       
    }
}
