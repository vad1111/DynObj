﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Виды;
using Динамические;
using Динамические.активности;
using Динамические.Виды;
using Динамические.Примитивы;

namespace ИИ
{
    public class Запуск
    {
        private static ОкноХранилища w;
        private static readonly Dictionary<string, object> кол = new Dictionary<string, object>();
        //[LoaderOptimization(LoaderOptimization.MultiDomain)]
        [STAThread]
        public static void Main(string[] имяФайла)
        {
            var conv = new KeyConverter();
            var cult = CultureInfo.GetCultureInfo("ru");
            var s = conv.ConvertToString(null, CultureInfo.GetCultureInfo("ru"), Key.A);
            //var sru = 
            //var k = conv.ConvertFromString("ф");

            Хранилище.Типы.Add(typeof(Выходное_устройство));
            Хранилище.Типы.Add(typeof(Входное_устройство));
            Хранилище.Типы.Add(typeof(АПереадресация));
            Хранилище.Типы.Add(typeof(АссинхроннаяПереадресация));
            Хранилище.Типы.Add(typeof(ДобавитьВХранилище));
            Хранилище.ПересчитатьКодыТипов();

           //ДисковоеХранилище.Тест();

            if (имяФайла.Length != 0)
            {
                try
                {
                    Хранилище.Восстановить(имяФайла[0]);
                }
                catch
                {
                    MessageBox.Show(" Не удалось восстановить диспетчер");
                }
            }
            else
            {
                //var сервер = new Сервер_хранилища();
               

                var адресРегистратра = Хранилище.Добавить(new РегистраторАдресов());// регистратор по  0 адресу
                СтрокаЮникода.Создать("место хранения");
                СтрокаЮникода.Создать("Имя");
                Хранилище.Добавить(new АдресВХранилище());

                var словарь = new СловарьПлохой();
                словарь.Добавить(new СтрокаЮникода() { Значение = "ключ" }, new СтрокаЮникода() { Значение = "значение" });
                словарь.Добавить(new СтрокаЮникода() { Значение = "ключ2" }, new СтрокаЮникода() { Значение = "значение2" });
                Хранилище.Добавить(словарь);

                var группа = new ГруппаОбъектов();
                Хранилище.Добавить(группа);

                группа.Добавить(new АдресВХранилище() { НомерВХранилище = 1 });
                группа.Добавить(new АдресВХранилище() { НомерВХранилище = 2 });
                группа.Добавить(new СтрокаЮникода(){Значение = "строка добавленая в группу"});

                //var тспсервер = new ТСПСвязьВходящая();
                //тспсервер.Старт();



                var гр = new ГруппаОбъектов();
                Хранилище.Добавить(гр);

                var Группа2 = new ГруппаАдресовИзСловаря();
                Группа2.Перечисление = Клавиатура.ЮНИКОДСимволКодыРусскихБуквПрописные;
                гр.Добавить(Группа2);

                гр.Добавить(new АдресВХранилище() { НомерВХранилище = 1 });
                гр.Добавить(new АдресВГруппе() { НомерВГруппе = 12 });
                гр.Добавить(new СтрокаЮникода() { Значение = "строка из группы" });
                гр.Добавить(new Пустота());
                var гр2 = new ГруппаОбъектов();
                гр.Добавить(гр2);
                гр2.Добавить(new СтрокаЮникода() { Значение = "строка из группы" });
                гр2.Добавить(new Пустота());
                гр2.Добавить(new СтрокаЮникода() { Значение = "строка из группы" });
                

                Dictionary<Type, Type> видыОбъектов = new Dictionary<Type, Type>()
                {
                    [typeof(ГруппаОбъектов)]= typeof(ВидГруппаОбъектов),
                    [typeof(СтрокаЮникода)] = typeof(ВидСтрока),
                    [typeof(Символ)] = typeof(ВидСимвола),
                    [typeof(ГруппаАдресовИзСловаря)] = typeof(ВидГруппаГоризонтальная),
                    [typeof(ПроксиОбъекта)] = typeof(ВидЗначениеАдреса)
                };

                var окноГруппы = new ВидОкно();
                окноГруппы.ВыборВидов = видыОбъектов; // объекты должны иметь свободные слоты, которые добавляются из справочника по мере надобности, ненужные не создаются похоже на список переменных в блоке кода
                окноГруппы.ДобавьСодержание(гр);

                окноГруппы.Показать();

                

                var вывод = new Выходное_устройство() {Имя = new СтрокаЮникода("Окно Вывода полученого текста")};
                var адресВывод = Хранилище.Добавить(вывод);
                var адресСвязиВывод =вывод.ДобавьСвязь(new Связь() {РольУдаленного = "поставщик", ТипСвязи = "входящая", ИмяСвязи = "Связь входящая выводного окна"});
                вывод.Запуск();

                var переадресация = new АссинхроннаяПереадресация();
                var адреспере = Хранилище.Добавить(переадресация);
                var адресСвязиПере = переадресация.ДобавьСвязь(new Связь() { РольУдаленного = "поставщик", ТипСвязи = "входящая"});
                переадресация.ДобавьСвязь(new Связь() { РольУдаленного = "получатель", ТипСвязи = "исходящая", });


                var адресдВХ = Хранилище.Добавить(new ДобавитьВХранилище());
                

                var ввод = new Входное_устройство();
                
                 var  адресввода= Хранилище.Добавить(ввод);

                ввод.Запуск();



                var удалЦелое1 = new Активность();
                




            }


           

            var a = new Application();

            w = new ОкноХранилища();

            var ff = new DependencyObject();
            ff.SetValue(Control.BackgroundProperty, Brushes.LightBlue);
            var ht = ff.GetValue(Control.BackgroundProperty);

            a.Run(w);

            // после завершение приложеня


            

        }
        public static void ИнициализацияСловарейЯзыков()
        {
            // это безымянные группы 
            var ЮНИКОДСимволКодыРусскихБукв = new ГруппаТипизированная() { ТипХранящихсяОбъектов = typeof(Символ) };

            Хранилище.Добавить(ЮНИКОДСимволКодыРусскихБукв);
            //Клавиатура.ЮНИКОДСимволКодыРусскихБукв = ЮНИКОДСимволКодыРусскихБукв;

            var ЮНИКОДРусскихБукв = new ГруппаОбъектов();

            Хранилище.Добавить(ЮНИКОДРусскихБукв);
            //Клавиатура.ЮНИКОДРусскихБукв = ЮНИКОДРусскихБукв;

            for (var с = 'а'; с <= 'я'; с++)
            {
                ЮНИКОДСимволКодыРусскихБукв.Добавить(new Символ(с));
                ЮНИКОДРусскихБукв.Добавить(new ЦелоеЧисло(с));
            }


            //------------------------------------
            var актБуквы = new Активность() { Имя = "русские буквы" }; // группа всех букв русских например
            var адресАктБуквы = Хранилище.Добавить(актБуквы);
            var связь0 = new СвязьПамять
            {
                ИмяСвязи = "1, а",// номер в алфавите, изображение
                ТипСвязи = "имеет члена"
            };
            актБуквы.ДобавьСвязь(связь0);

            // активность представляет конкретную букву
            // имеет код юникода, изображения возможно прямые, возможно через код Юникода
            // является членом группы Русские буквы  это Группа словосочетания 
            // Активности могут быть не активными, ничего не делать только указывать пути 
            // Активности группы 
            var актБуква = new Активность() { Имя = "буква а" };
            var адресАктБуква = Хранилище.Добавить(актБуква);


            var связь11 = new СвязьПамять
            {
                ИмяСвязи = "член группы",
                ТипСвязи = "это", //или является членом группы
                АдресУдаленнойСвязи = связь0.АдресСобственный
            };
            актБуква.ДобавьСвязь(связь11);

            var связь10 = new СвязьПамять
            {
                ИмяСвязи = "Код Юникода",
                ТипСвязи = "имеет" // соответсвует
            };
            var адрессвязи1 = актБуква.ДобавьСвязь(связь10);

            // имеет связь с конкретной буквой, связь с группой "Коды Юникода"
            var актКод = new АктивностьЗначение() { Значение = new Символ('а') };
            актКод.Имя = "Код ЮНИКОДА а"; // это ответ на вопрос Что это? совпадает с связью с группой

            var связь2 = new СвязьПамять();
            связь2.ИмяСвязи = "Буква";
            связь2.ТипСвязи = "чей"; //чей это код
            var адрессвязи2 = актКод.ДобавьСвязь(связь2);
            связь2.АдресУдаленнойСвязи = адрессвязи1;
        }
      
    }

    public class Список
    {
        public static List<string> Память = new List<string>(){ "лороро", "лороро"}; 
    }
}