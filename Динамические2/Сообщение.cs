using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Динамические
{
    // внутреннее сообщение
    public class Сообщение: РАМОбъект
    {
        public Адрес ОтКого;
        public Адрес Кому;
        public int НомерВнутреннейКоманды;
        public СтрокаЮникода ИмяВнутреннейКоманды; // на всякий случай, если список команд не согласован
        public object ПараметрыКоманды;
    }
}
