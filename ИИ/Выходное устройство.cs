using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Динамические;

namespace ИИ
{
    
    public class Выходное_устройство : Активность
    {
        //Список исходящих 
        Текстовый_Вывод окно = new Текстовый_Вывод();
        private string строка;
        public override void Запуск()
        {
            if (Application.Current != null && Application.Current.MainWindow != null) окно.Owner = Application.Current.MainWindow;
            окно.Show();
        }

        public override void ПолучиСообщение(Связь связь, РАМОбъект сообщение)
        {
             строка = (СтрокаЮникода) сообщение;

            if( окно.Dispatcher.Thread == Thread.CurrentThread)
            { Обработать();}//отправить сообщение 
            else
            {
                окно.Dispatcher.Invoke( Обработать ); //для вывода из другого потока
            }
        }

        void Обработать()
        {
            окно.TextBlockВывод.Text = строка;
        }

        public override void Закрыть()
        {
            окно.Close();
        }

        //public override ПримитивИлиАдрес Восстановить(BinaryReader читатель)
        //{
        //    return base.Восстановить(читатель);

        //}
    }
    
}
