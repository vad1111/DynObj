using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Динамические;

namespace ИИ
{
    public class ДобавитьВХранилище : Активность
    {
        public override void ПолучиСообщение(Связь связь, РАМОбъект сообщение)
        {
            var строка = (СтрокаЮникода) сообщение ;
            СтрокаЮникода.Создать(строка);
            ОкноХранилища.окноХранилища.ОбновитьОкно();

        }
    }
}
