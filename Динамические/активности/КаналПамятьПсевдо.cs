﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Динамические.активности
{

    // чтобы изолировать эти объекты, сейчас можно отдавать команды удаленному каналу
    // минус, что адрес входящего буфера можно передавать кому угодно и кто угодно может туда писать и читать
    // ну поток ТСП тоже можно передать кому угодно, так что тут защиты нет ни там ни там

        // это регистратор скрывает адреса в Куче каналов друг от друга 
        // Адрес в Куче из Адреса можно узнать только внутри регистратора

        // АдресВКуче может отдавать не адрес канала а  АдресВкуче ВходящегоСообщения
        // или Метод Получи(сообщение)
    public class Регистратор: ПримитивИлиАдрес
    {
        private Dictionary<int, КаналПамятьПсевдо> Каналы;
        private Dictionary<КаналПамятьПсевдо, List<КаналПамятьПсевдо>> СписокСвязейКанала;

        public bool Зарегистрируй(КаналПамятьПсевдо канал, АдресВГруппе адрес)
        {
            Каналы[адрес.НомерВГруппе] = канал;

            return true;
        }

        // установить связь
        public bool УстановитьСвязь(АдресВГруппе адресСКем, АдресВГруппе свойАдрес)
        {
            if (Каналы.ContainsKey(адресСКем.НомерВГруппе))
            {
                
                if (Каналы[адресСКем.НомерВГруппе].ЗапросСвязи(свойАдрес))
                {
                    return true;
                }
                
            }
            return false;
        }

        // этот метод скрывыает удрес Канала в КУче 
        public bool ПередайОбъект(АдресВГруппе адрес, ПримитивИлиАдрес объкет)
        {
            if (адрес != null)
            {
                if (Каналы.ContainsKey(адрес.НомерВГруппе))
                {
                    Каналы[адрес.НомерВГруппе].Получи(объкет);
                    return true;
                }
                
            }

            return false;
        }

    }

    public class КаналПамятьПсевдо : Канал
    {
        private ПримитивИлиАдрес _ВходящееСообщение;// это собственное хранилище ссобщений
        private ПримитивИлиАдрес _исходящееСообщение; // может отсутствовать
        private ПримитивИлиАдрес _адресУдаленногоИсходящегоСообщения;//это чужое 
        private ПримитивИлиАдрес _адресУдаленногоВходящегоСообщения;// это чужое
        private КаналПамятьПсевдо _удаленныйКанал; // может отсутсвовать если есть УдаленныйМетод
        private Thread _текущийПоток;
        //public Action<ПримитивИлиАдрес> Получи; // вместо входящего сообщения . Может предоставляться выбор методов
        private Action<ПримитивИлиАдрес> УдаленнаяКомандаПолучи;

        public КаналПамятьПсевдо()
        {
            Получи = Связь.ПолучиСообщение; // это напрямую в связь и активность без депонирования прямее ПередатьНапрямую
            Получи = ОставитьиУйти;// только это просто, остальное трудно с потоками
            ДайВходящее = ДайВходящееСообщение;
            ПередайИсходящееСообщение = ПередайСообщениеКомандой;
            //Получи.DynamicInvoke();
        }

        // внешние команды , но настройка внутренняя путем присвоения делегату Получи
        void ПередатьНапрямую(ПримитивИлиАдрес сообщение)
        {
            Связь.ПолучиСообщение(сообщение);
        }

        // забор сообщения собственным потоком
        void ОставитьиУйти(ПримитивИлиАдрес сообщение)// неизвестно когда заберут, ответ ждать путем опроса
                                                      // прерыванием или не ждать
        {           
            _ВходящееСообщение = сообщение;
            // тогда ДайВходящее == ДайВходящееОпросом()
        }
        void ОставитьИСообщитьВТекущемПотоке(ПримитивИлиАдрес объект) // с прерыванием ожидания
        {
            _ВходящееСообщение = объект;
            Связь.СигналОПоступленииСообщения();// здесь происходит аренда потока, Связь или Активность сама решает
            // связь  должна взять входящее сооьщение передать в активность
            // активность должна быстро что-то сделать, (возможно вызвать передатьИсходящее=ОставитьИ уйти)
            // и вернуть поток
            // либо запустить обработку входящих в собственном потоке
        }
        void ОставитьИСообщитьВНовомПотоке(ПримитивИлиАдрес объект) // текст команды содержит программу
        {
            _ВходящееСообщение = объект;
            Task.Run(Связь.СигналОПоступленииСообщения);
        }

        // это связано с командой ДайВходящееОжиданиеСпрерыванием()
        void ОставитьИПрерватьОжиданиеСообщения(ПримитивИлиАдрес объект) // текст команды содержит программу
        {
            _ВходящееСообщение = объект;
            ПрерватьОжиданиеПотока();
        }
        

        // данный сценарий нужен когда неизвестно нужно Вам сообщение или нет, забирать или нет решать ВАм
        // Есть категория активностей,типа данных температуры, которые можно только скопировать, стереть невозможно
        // данные для многократного забора
        void Забери()
        {
            Связь.СигналОПоступленииСообщения(); // это для забора из адреса исходщего сообщения вызови метод Забрать()
                                                 // вставка своих методов
             // ЗабратьСообщение можно путем 1.простого копирования 2. забор адреса и очистка удаленногоИсходящего 
            //3.запуска удаленного метода
        }

        ПримитивИлиАдрес ДайВходящееОжиданиеОпросом()
        {
            //var началоОжидания = DateTime.Now;
            //var конецОжидания = началоОжидания + TimeSpan.FromMilliseconds(ВремяОжиданияПриема);
            var количествоЦиклов = ВремяОжиданияПриема/Связь.ИнтервалОпросаВходящих;
            var текущийЦикл = 0;
            while (текущийЦикл<=количествоЦиклов)
            {
                if (_ВходящееСообщение!=null)
                {
                    return ВходящееСообщение;
                }
                Thread.Sleep(Связь.ИнтервалОпросаВходящих);
                текущийЦикл++;
            }
            return null;
        }

        ПримитивИлиАдрес ДайВходящееОжиданиеСпрерыванием()
        {
            _текущийПоток = Thread.CurrentThread;

            while (true)
            {
                 if (_ВходящееСообщение != null)
                {
                    return ВходящееСообщение;
                }
                 try
                {
                    Thread.Sleep(0); // блокировать поток
                }
                catch (ThreadInterruptedException) { }
            }
            return null;
        }


        private ПримитивИлиАдрес ВходящееСообщение
        {
            get
            {
                var c = _ВходящееСообщение;
                _ВходящееСообщение = null;
                return c;
            }
            set { Получи(value); }
        }

        private ПримитивИлиАдрес ИсходящееСообщение
        {
            get
            {
                var c = _исходящееСообщение;
                // создать переменную типа адрес ,куда копировать значение из переменной с именем
                // перемещение объекта= копирование значение в новое место и стирание в старом
                // на самом деле лучше иметь метод ( выходная переменная, так будет повторное присваивание, 
                // лишняя временная переменная
                // зачем нужны методы? для процессора это рецепт как достичь требуемого результата
                // метод с с возратом адреса результата (это некорый объект)
                //в С№ есть Универсальное хранилище, где создаются все объекты, Домены или контексты -это хранилища
                // временных объектов в памяти
                // название ДайИсходящееСообщение неверен
                // переменные хранят (адреса объектов в куче)=(значение)
                // переменные это временные объекты для хранения  примитивов, адресов,
                //     использумые в процессе выполнения метода 
                _исходящееСообщение = null;
                return c;
            }
            set { _исходящееСообщение = value; //переписывает значение
            }
        }

        private КаналПамятьПсевдо УдаленныйКанал
        {
            get { return _удаленныйКанал; }
            set
            {
                if (_удаленныйКанал == null)
                {
                     _удаленныйКанал = value; 
                    ПрерватьОжиданиеПотока();
                }
                else
                {
                    if (_удаленныйКанал != value)
                    {
                        // новое подключение 
                        _удаленныйКанал.СообщениеРазорвиСоединение();
                        // если получено нет, то проверить приоритеты каналов
                        // если новый канал более важный
                        // разорвать принудительно 
                        _удаленныйКанал = value; // здесь рвется старое соединение и устанавливается новое 
                        //иначе отказать в сооединении
                        ПрерватьОжиданиеПотока();
                    }
                    else
                        return;
                }
               
               
            }
        }

        public override void Регистрация()// связь уже зарегистрирована в активности, но могут быть другие 
        {
            
        }

        public override bool УстановитьСвязь()
        {
            try
            {
                _удаленныйКанал = (КаналПамятьПсевдо) ((Связь) АдресУдаленнойСвязи.АдресВКуче()).Канал;
                _удаленныйКанал.УдаленныйКанал = this;

                УдаленнаяКомандаПолучи = _удаленныйКанал.Получи;// это сразу получается из адреса
                ПрерватьОжиданиеПотока(); // порядок прерывания ожидания сначала далеко потом у себя
                return true;
            }
            catch // адрес может быть еще не зарегистрирован
            {
            }
            return false;

        }


        public override void ОжидатьПодключение()
        {
            _текущийПоток = Thread.CurrentThread;
            try
            {
                Thread.Sleep(0); // блокировать поток

            }
            catch (ThreadInterruptedException)
            {
               
            }
        }
        void ПрерватьОжиданиеПотока()
        {
            if (_текущийПоток != null) // прервать ожидание 
                {
                    var поток = _текущийПоток;
                    _текущийПоток = null;
                    поток.Interrupt();
                }
        }

        public override bool СоединениеУстановлено { get { return УдаленныйКанал != null; } set {} }

        public void РазорватьСоединениеПринудительно()
        {
            УдаленныйКанал.УдаленныйКанал = null; //стереть ссылку на себя
            УдаленныйКанал = null;
        }
        public void СообщениеРазорвиСоединение() // это команда от другого канала
        {
            // здесь может быть код безопасного разрыва, забрать например сообщения
            УдаленныйКанал = null;
        }

        public override int ДоступноБайт()
        {
            if (ВходящееСообщение != null) return ВходящееСообщение.Длина();
            return 0;
            // сть еще вариант проверки что на удаленных исодящих
        }

        public override ПримитивИлиАдрес ДайВходящееСообщение()
        {
            
            return ВходящееСообщение;
        }

        public ПримитивИлиАдрес ЗабратьСообщение()
        {
            _ВходящееСообщение = УдаленныйКанал.ИсходящееСообщение;// прямое присвоение без вызова сигнала для связи
            return ВходящееСообщение;
        }


        private void ПередайСообщениеКомандой(ПримитивИлиАдрес сообщение)
        {
            if (СоединениеУстановлено)
            {
                УдаленнаяКомандаПолучи(сообщение); 
            }
        }

        private void ПередайСообщениеПрисваиванием(ПримитивИлиАдрес сообщение)
        {
            
            if (СоединениеУстановлено)
            {
                УдаленныйКанал.ВходящееСообщение = сообщение;
            }
                 
        }
        public void ВыложитьВОткрытыйДоступ (ПримитивИлиАдрес сообщение)
        {
            //оставить у себя чтобы забрали
            ИсходящееСообщение = сообщение;
            //  УдаленныйКАнал.Забери()
            
        }

        public bool ЗапросСвязи(Адрес АдресУдаленный)
        {
            if (АдресУдаленный == АдресУдаленнойСвязи)
            {
                return true;
            }
            if (Связь.АдресУдаленнойСвязи == null) // точнее любой
            {
                Связь.АдресУдаленнойСвязи = АдресУдаленный;
                return true;
            }
            return false;
        }
    }
}
