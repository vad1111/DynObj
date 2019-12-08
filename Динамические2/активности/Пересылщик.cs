﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Динамические
{
    //пересыльщик может играть роль доверенного объекта 
    public class Пересылщик : Активность // можно использовать как переадресацию
    {
        // будет выглядеть как от него - Реальный посыльный стирается
        // ответ получить посыльному не возможно
        // можно добавить в сообщение поле отправить ответ по адресу
        public override void ПолучиСообщение(Связь связь, РАМОбъект сообщение)
        {
            // без проверки на входящие объекты // это экземпляры типа
            var сооб = сообщение as Сообщение;
            сооб.ОтКого = СобственныйАдресПримитива ;
            ((Связь) сооб.Кому.АдресВКучеПамяти()).ПередайСообщение(сооб);

            //ОтослатьСообщениеВсемИсходящим(сооб);
        }
    }

    public class АПереадресация : Активность // синхронная переадресацияя может сразу нескольким объектам
    {
        // будет выглядеть как от него - Реальный посыльный стирается
        // ответ получить посыльному не возможно
        // можно добавить в сообщение поле отправить ответ по адресу
        
        public override void ПолучиСообщение(Связь связь,РАМОбъект сообщение)
        {
            // без проверки на входящие объекты // это экземпляры типа
            foreach (var Связь in СписокКомуОтдать.Список)
            {
                 //((Связь)Связь).ПередайСообщение(сообщение);
            }
        }
    }

    // это еще расширение сигнала, увеличение мощности , если количество исходящих связей ограничено (255 например), ставиться этот ретротранслятор
    // если связь только одна это становиться переадресацией 
    // это специализированый пересылщик, нет нужды одновременно делать анализ сообщения и его рассылку
    //
    public class АссинхроннаяПереадресация : Активность // можно использовать как переадресацию
    {
        // 
        // процессор сразу уходит из активности
        // 
        public override void ПолучиСообщение(Связь связь, РАМОбъект сообщение)
        {
          
        }

        private void начало(object сообщение)
        {
            foreach (var адреспереадресации in СписокКомуОтдать.Список)
            {
                //((Связь)адреспереадресации).ПередайСообщение(сообщение);
            }
        }
    }

    // можно расщеплять на группы (слоги) 
    public class РасщеплениеСтрокиНаСимволы : Активность //последовательное  (возможно паралельное первый символ первой активности, второй - второй активности )
    {
        public override void ПолучиСообщение(Связь связь, РАМОбъект сообщение)
        {
            string строка = (СтрокаЮникода) сообщение;
            for (int i = 0; i < строка.Length; i++)
            {
                ОтослатьСообщениеВсемИсходящим(строка[i].ToString());
            }
        }
    }
    public class ВыщеплениеСимволаИзстроки : Активность //последовательное  (возможно паралельное первый символ первой активности, второй - второй активности )
    {
        public int номерСимвола;
        public override void ПолучиСообщение(Связь связь, РАМОбъект сообщение)
        {
            string строка = (СтрокаЮникода)сообщение;
            
                ОтослатьСообщениеВсемИсходящим(строка[номерСимвола].ToString());
        }

        public static object ДайСписокРасщепительСтроки(int количество)
        {
            var расщепитель= new List<ВыщеплениеСимволаИзстроки>();
            for (int и = 0; и < количество; и++)
            {
                расщепитель.Add(new ВыщеплениеСимволаИзстроки(){номерСимвола = и});
            }
            return расщепитель;
        }
    }

   // в исходящие добавить СписокРасщепителей, к каждому выщепителю добавить свой анализатор получиться одномерный анализатор 
    // таким можно делить строку на слова, ряд светящихся пробел не светиться, анализатор может на букву давать 1 на остальное 0 
    // как найти слово тогда слово тогда надо расщепить словарьВторойПозиции (буква) = Список слов, ДАЛЬШЕ КАждому слову посылается сигнал с количеством букв в слове. Срабатывает только один или много
    // с другими окончаниями более длинные слова (радио, радиола)
    // мы имее 7 списков выбираемое слово будет иметь светимость= 7 



    /// <summary>
    /// Если слов миллион распозновать так каждое слово, на начало слова регистрируется сразу млн. активностей, а потом все меньше и меньше  
    /// </summary>
    public class ХранительРяда : Активность // как узнать слово, зависит как оно сохранено, простеший вариант список
                                            // если последовательно загораются активность хранителей букв 
    {
        public List<Активность> РядБукв; // это вариант ЧЕТКОГО хранения слова, для НЕЧЕТКОГО это не подайдет "рожь"
                                            // получается все слова будут привязаны ко всем своим буквам
                                            // лучше привязываться только к ожидаемым буквам
                                            // нужно иметь список всех активностей 
        private int номерОжидания;
        private Активность фокус;
        //public override object ПолучиСообщениеСОтветом(object сообщение) //привязан к букве
        //{
        //    РеакцияНаБукву( сообщение);
        //    var фокус = (Активность) сообщение;
        //    //найти в фокусе требуемую активность
        //    bool НужнаяАктивностьАктивна = true;
        //    if (НужнаяАктивностьАктивна) // фокус присылает да если ожидание оправдалось, или нет и исключчает его из списка текущих ожиданий 
        //    {
        //        номерОжидания++; 
        //        if (номерОжидания == РядБукв.Count)
        //        {
        //            // конец ожиданиям
        //            номерОжидания = 0; // ошибка надо ждать еще пробела или любого кроме буквы иначе появяться однокоренные слова 
        //            ОтослатьСообщениеВсемИсходящим("слово");
        //        }
        //        var ожидаемаяАктивность = РядБукв[номерОжидания];
        //        фокус.СписокКомуОтдать.Добавить(this); // ссылку на ожидаемую Активность 
        //        //продолжаем ожидание
        //        return true;
        //    }
        //    // прерываем ожидания
        //    номерОжидания = 0;
        //    return false; // этот шаблон отключается и ждет начала нового слова или подключается к ожиданию начала нового слова
        //}

        private void РеакцияНаБукву(object сообщение)
        {
            var ответ = (bool) сообщение;
            if (ответ == true)
            {
                номерОжидания++;
                РядБукв[номерОжидания].СписокКомуОтдать.Добавить(this); //ожидание следущей буквы или не буквы (конца слова)
            }
            else
            {
                номерОжидания = 0;
                РядБукв[номерОжидания].СписокКомуОтдать.Добавить(this);// ожидание начало нового слова
            }
        }
    }

    //public class АктивностьСписок : Активность //еще Фильтр , еще Создатель, Распознователь
    //                                            // если это фильтр, то это становиться концепцией, например "русская буква"
    //{
    //    // получить символ отправить получателям, если нет положительного отклика
    //    // создать нового получателя
    //    //private List<string> Варианты;
    //    public override object ПолучиСообщениеСОтветом(object сообщение) //это функция запоминания 
    //    {
    //        var буква = (string)сообщение;

    //        //if (Варианты.Contains(буква)) return true; // ЭТО вариант с внутренним хранением, но здесь исчезают концепции букв

    //        //нужно пройти все чтобы Исходящие активности сработали=  или активировали исходящие или их сбросили 
    //        for (int и = 2; и < СписокИсходящих.Список.Count; и++) //можно распаралелить
    //        {
    //            var ответ = ((Связь) СписокИсходящих[и]).ПередайСообщениеСОтветом(буква);
    //            if (ответ.GetType() != typeof(Пустота))
    //            {
    //                ИсхАктивность(0).ПолучиСообщениеСОтветом(Имя.Значение); // посылка во внтуренний голос ( если голос открыт отправить на вывод)
    //                                                                        // во внутр голосе появиться сначала "а" потом "буква" 
    //                return ИсхАктивность(и);
    //            }
    //        }
    //        //нет ответов - неивестный символ 
    //        // реакция на неизвестность
    //        // или игнорировать - получается фильтр букв ( см. ниже), например ограничим количество связей

            
    //        // обучение
    //        // спросить "что это?"
    //        // если ответ "это буква ххх" 
    //        // если ответ "это буква" проверить со своей реакцией если успешно создать активность Буква и привязать к ней ответ ("это буква"+ Буква)
    //        // если нет 
    //        var ответУчителя = (string) ИсхАктивность(0).ПолучиСообщениеСОтветом("что это?"); // "это что?"
    //        string[] словаОтвета = ответУчителя.Split(' ');
    //        if (ответУчителя.Contains(this.Имя.Значение))
    //        {
    //            var новаяАктивность = new ХранительБукв(){Буква = буква};
    //            новаяАктивность.СписокИсходящих.Добавить(new Активность(){Имя = new Строка(){Значение = словаОтвета[2]}}); // Эта активность выдающая "это буква"+Буква
    //            // эту активность можно переслать во внутренний голос, а тот уже решит произносить его или нет, а можно сразу в наружный 
    //            СписокИсходящих.Добавить(новаяАктивность);
    //            return новаяАктивность;
    //        }
    //        // или создать новую активность в предположении что это буква
    //        //
            
            

    //        return false; 
    //    }
    //}

    // объекты объединяются если у них есть общие цели, интересы. 
    // для этого объекты должны найти таких
    // 1. создается группа где формулируется цель, объект который имеет цель, должен начать искать группу и присоединиться
    // один из вариантов прямой обмен сообщениями, "ты имеешь цель?" ответ "да" резонанс, отсылаешь число - получаешь его обратно 
    // справочник цель- группа

    //верхний  уровень обучения АктивностьСписок("это") это распознователь там есть все, у него в списке "буква" "знаки препинания" и т.д.
    // хотя с начала будет только "русская буква а" из слов ответа исключается "это" 
    // появление "русская буква б" приводит к тому что сравниваються наборы слов, происходит расщепление на "русская буква" и два "а" и "б"
    // как только появляется "английская буква" происходит выделение "буква" и два "английская" "русская"
    // !!!хотя верхний уровень должно быть слово "ЧТО" - это имя всех существительных
    // "буква" это - "что", "английский" "это" "какой" "буква имеет гафический символ" буква сопоставлена цифра из кодовой таблицы
    // "английская" это что, не что а какая
 
    // другая последовательность
    // ХранительБукв добавляется не в исходящие, а во Входящие, И в ХранительБукв  во Входящие добавляется Расщепитель 

    // ускорение доступа к данным это создание справочников, появление символа относит его к буквам и ищет уже там 


    public class ХранительБукв : Активность //это может быть не буква а некий ряд букв 
    {
        public string Буква;
        //public override ПримитивИлиАдрес ПолучиСообщениеСОтветом(ПримитивИлиАдрес сообщение)
        //{
        //    if (Буква == (string) сообщение)
        //    {
        //        // сделать что-то
        //        // послать на вывод "это "+Буква
        //        ОтослатьСообщениеВсемИсходящим(true); // абстрагирование от поллученого сообщения, можно получить например имя в том числе активность "буква"
        //        return true;
        //    }
        //    ОтослатьСообщениеВсемИсходящим(false); // ожидание не сработало 
        //    СписокКомуОтдать.Список.Clear();// ожидающие шаблоны отключаться
        //    return false;
        //}
    }

    //эта активность фильтрует поступающие символы !!!это частный случай хранителя букв
    public class РусскаяПрописнаяБуква : Активность
    {
        //public override object ПолучиСообщениеСОтветом(object сообщение)
        //{
        //   var буква = (char) сообщение ; 
        //    if (буква == 'а') // если ( символ является маленькой прописной буквой ) буква = переменна
        //    {
                
        //        ОтослатьСообщениеВсемИсходящим(сообщение);return true;
        //    }
        //    return false;
        //}
    }

    // как создать эту активность путем определения 
    // а это рпб, рпб = русская прописная буква
    //я это рпб 
    // вопрос я это рпб? да
    // что такое а? а это рпб , сокращенно рпб, еще сокращенее буква
    // а это буква 
    // а это прописная буква 
    // а(анг) это прописная буква
    // а относиться к группе букв, а имеет свойство(какая)  ( язык, может быть еще не определен) = русский, а имеет свойство(какая) (без названия, размер) = прописная 
    // когда таких определений много
    // выгоднее иметь один объект рпб и одну ссылку от каждой буквы, хотя изначально, существует куча ссылок от каждой буквы 
    // как это обнаружить?


    /* переадресация это разновидность однонаправленной связи - посредник который ничего не делает 
     * 
     * можно создать отдельный объект активность, роль которого соединять только две другие активности 
     * этот объект хранит адреса двух объектов и представляет стандартный набор функций, разрешений 
     * связь может измениться и стать переключателем, пускать или не пускать сигнал
     * 
     * связи могут временно рваться (не отвечать) например на другом компе. Необходимо тогда вести статистику надежности или искать нового поставщика готового или создать нового
     * т.е. наличие только адреса подрядчика недостаточно, наличие более сложной структуры позволит абстагироваться от сложного адреса и заменить его коротким, вести справочник
    */
    
    // проблема мусора, если активность удаляется и имеет связи с объектами, объекты остаются бесхозными и просто жрут ресурсы
    // как их выявить и не удалить еще нужные ресурсы ? 
    // есть активности которые только посылают сигналы о них никто не знает (ввод текста напримера)но у них есть подпичики
    // есть активности которые пассивные они не имеют связей (сервер) или имеют временные связи (типа сеанс связи)
    // для них сценарий должен быть подсчет активности (количество обращений или время последнего обращения, после чего самоликвидация, или принудительная ликвидация)
    // 
    // сценарий для тех у кого есть подписчики, подсчет количества подписчиков, ликвидация после обнуления подписчиков (появился новый вариант активности, все переключились)
    // если он еще сервер, то сценарий сервера с временем жизни

}
