﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Динамические.действия
{
    // есть планирование а есть уже готовое реагирование
    // реаагирование это получение команды, поиск подходящей реакции и запуск реакции
    // эта схема реагирование тоже описывается планом
    // 
    public class Планирование
    {

        void Тест()
        {
            var Реагирование = new ПоследовательностьДействий();
            Реагирование.СписокТипизированныхПеременных = new List<ТипизированнаяПеременная>();

            Реагирование.СписокДействий = new List<План>
            {
              //Получить команду, сохранить ее в переменную текущая команда
              //
            };

            //вставка группы действий внутрь другого, например проверка на пусто перед использованием
            // 
            // созданиеокружения 
            // 

        }
    }
}
