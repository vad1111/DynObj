﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Динамические
{

    public class Команда : Примитив // чистый набор данных, нет методов
    {

        public ЦелоеЧисло НомерКоманды; //ускоряет поиск команды в списке команд объекта, стоит разделить Команду по номеру и команду по имени  
        public СтрокаЮникода Имя;
        public ГруппаОбъектов Параметры; // тут нет описания параметров имя, тип, in-out-ref параметра
        public List<object> Параметры2;

        public Команда() { }

        public Команда(string имя, params РАМОбъект[] параметры)
        {
            Имя = new СтрокаЮникода(имя); Параметры = new ГруппаОбъектов();
            foreach (var пар in параметры) Параметры.Добавить(пар);
        }
        public override void СохранисьВ(Stream поток)
        {
            base.СохранисьВ(поток);
            Имя.СохранисьВ(поток);
            Параметры.СохранисьВ(поток);
        }

        public override РАМОбъект Восстановить(Stream поток)
        {
            Имя = (СтрокаЮникода)РАМОбъект.Создать(поток);
            Параметры = (ГруппаОбъектов)РАМОбъект.Создать(поток);
            return this;
        }

        
    }

    public class РеакцияНаКоманду : Примитив
    {
        public object Объект;
        public СтрокаЮникода Имя;
        public ГруппаОбъектов ОписаниеПараметры;
        public Команда ШаблонКоманды; //описание команды 
       
        public MethodInfo Инструкция; // сдесь может быть объект типа Действие, хотя Действие это описние действия - рецепт, действием рецепт становиться в момент выполнения и до пояления результата

        public РАМОбъект Выполнить(object объект, Команда команда)
        {
            //проверка что данные команды совпадает с описнием или без этого 
            return (РАМОбъект) Инструкция.Invoke(объект, команда.Параметры.Список.ToArray());
        }

    }


    public class КомандаПоНомеру : Примитив
    {
        public ЦелоеЧисло НомерКоманды; //ускоряет поиск команды в списке команд объекта, стоит разделить Команду по номеру и команду по имени  
        public ГруппаОбъектов Параметры;

        public override void СохранисьВ(Stream поток)
        {
            base.СохранисьВ(поток);
            НомерКоманды.СохранисьВ(поток); //теперь нужны два режима работы активности которые работают с двумя типами команд (лучше первое согласование, переадчик имеет списоки производит настройку
            //у себя, выбор из списка и отсылку уже по номеру, другой сценарий - осылка по имени , обратный результат содкержит уже номер команды (параллельная настройка)
            // второе самое быстрое обмениваться не командами а просто уже данными т.е. произошло соединение сразу отправил цифру(значение), закрыл соединение - это только 
            // такое возможно только для односторонней связи (один только получает другой только передает)
            // возможны два варианта 1. Инициатитор-клиент (тот кто устанавливает связь) - получатель , Слушатель -передатчик (получение по запросу)
            //                       2. Инициатор-клиент  - передатчик, Слушатель - получатель   ( слушатель зависимый объект, подписчик на информацию 
            // сайт содержит и разметку и список команд себе же и еще скрипты для выполнения самим клиентам (которые никак не контролируются клиентом
            // а
            Параметры.СохранисьВ(поток);
        }

        public override РАМОбъект Восстановить(Stream поток)
        {
            Параметры = (ГруппаОбъектов)РАМОбъект.Создать(поток);
            return this;
        }
        

    }

  

}
