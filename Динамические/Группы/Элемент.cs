﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Динамические
{
    // это копия или ссылка или значение по адресу 
    // это результат запроса из базе данных
    // только Ключ здесь адрес
    // это копия объекта в памяти в случае хранения данных на диске
    // точнее это ссылка на реальный объект
    // когда объект восстанавливается откуда -либо нужно указывать что это копия или заместитель
    // Это Образ объекта и одновременно методы воздействия на реальный объект
    public class ПроксиОбъекта : ПримитивИлиАдрес //еще название элемент, Агент хранитель ,Адресная капсула, Кэш объекта в памяти (т.е. в другой группе, если примитива нет, то получается читсый адрес 
    {
        public ПримитивИлиАдрес Примитив; // это ссылка на реальный объект, из примитива можно удалить адрес
        public Адрес АдресПримитива; // здесь может быть относительный и абсолютный адрес это адрес источника хранения 
        public ГруппаОбъектов ЛокальнаяГруппаВладелец => (ГруппаОбъектов)АдресПримитива.АдресРегистратора.АдресВКуче();

        public ПримитивИлиАдрес УдаленныйПримитив => Примитив == null ? Примитив = АдресПримитива.АдресВКуче() : Примитив;
// получается чистая ссылка возможна замена на вызов к удаленным серверам
        public bool ДляЧтения; // режимы для чтения, записи, чтения-записи,
        public bool ДляЗаписи;

        // добавка метода обратного вызова для подписки обновления 
        public void ИзменитьИздалека(ПримитивИлиАдрес новоеЗначение) // методы обратного вызова принадлежат потоку вызывающего явно что-то не так сов сей концепцией ООП этот метод должен быть в группе
        {
            Примитив = новоеЗначение;
            // сообщить о событии подписантам 
        }
        public void ИзменитьИзнутри(ПримитивИлиАдрес новоеЗначение)
        {
            Примитив = новоеЗначение;
            // сообщить об Изменении группе
            // найти связь с ролью хранитель или посредник хранителя 
            var связь = АдресПримитива.СоздатьСвязь(); // это для активностей 
            связь.ПередайСообщениеСОтветом(новоеЗначение);

            var группа = (ГруппаОбъектов) АдресПримитива.АдресРегистратора.АдресВКуче();
            группа.Заменить(АдресПримитива, новоеЗначение);
        }
        // собственный адрес для обратного вызова присваивается новым владельцем и сообщается источнику хранения 
        public override ПримитивИлиАдрес Восстановить(Stream поток)// где восстановить
        {
            var адресГдеВостановить = СобственныйАдресПримитива.СоздатьСвязь().АдресУдаленнойСвязи;// пусть эта связь доступна Как параметр
            var адресУдаленный= (АдресУниверсальный) Создать(поток);// это адрес локальный АдресВХрналище, например
           
            var текущийадрес= new АдресУниверсальный();
            текущийадрес.АдресРегистратора = адресГдеВостановить;
            текущийадрес.СобственныйАдресПримитива = адресГдеВостановить;
            текущийадрес.КомандаОбъекту = new Команда("ДайЭлемент", адресУдаленный);
            АдресПримитива = текущийадрес;
            
            // это код для Активности или
            var канал = АдресПримитива.ТипКанала();

            var группа = АдресПримитива.АдресРегистратора;// это адрес группы

            return base.Восстановить(поток);
        }
    }

    


}
