﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Динамические
{
    // это группа - обстоятельства и действие.

    public class КармическоеДействие
    {
        private ПримитивИлиАдрес Обстоятельства; //Список произошедших событий 
        private План Реакция; // когда появляются подходящие обстоятельства срабатывает реакция

    }



    // накопитель или сумматор считает сколько входящих связей активно
    // руское слово - 2 входящих, если они поступают одновременно или утвержденной задержкой яркость увеличивается до максимума и посылается сообщение
    // похоже на ЛОгическое И
    // возможен порядок поступления сначала поступает первое
    // возможна еще подписка на промежуточные состояния счетчика
    // нужно добавить сюда еще количество тактов, которое будет считать время между появлением двух слов (сигналов)
    // если есть встроенный процесс яркость можно постепенно уменьшать, уменьшая ожидания
    // если требуемое количество пропусков =0 
    public class Накопитель : Активность // Накопитель Событий, если надо определить одновременное появление событий (расширенное логическое И)(узел шаблона 
    {
        private List<Активность> СписокАктивностей; // "русская" "прописная" "буква" активность активируется если они появляются все в произвольном порядке
        private List<Активность> ПришедшиеАктивности;
        private int счетчик;
        private int разрешенныйОтступ; //= Разрешенная пауза не сбрасывает счетчик, пока не поступит это количество пустых тактов 
        // так можно реализовать отслеживание перестановки букв 
        public override void ПолучиСообщение(Связь связь, ПримитивИлиАдрес сообщение) // это сообщение посылает либо активность представляющая ее части, либо пустой такт 
        {
            var поступившаяАктивность = сообщение as Активность;
            // если поступившая есть в списке и она еще неприходила
            if (СписокАктивностей.Contains(поступившаяАктивность))
            {
                if (!ПришедшиеАктивности.Contains(поступившаяАктивность)) // если она ранее не приходила
                {
                    ПришедшиеАктивности.Add(поступившаяАктивность);
                    if (ПришедшиеАктивности.Count == СписокОтКогоПолучить.Список.Count)
                    {
                        ОтослатьСообщениеВсемИсходящим("все пришли");
                        ПришедшиеАктивности.Clear();
                    }
                }
                else // если уже ранее приходила //если придет два раза "русская" (дребезг) 
                {
                    //если текущий пауза меньше разрешенной
                    // вариант 1 устранение дребезга = "одна  или несколько одинаковвых слов
                    //ничего не делать

                    //вариант 2 точное совпадение, не допускаются повторы
                    ПришедшиеАктивности.Clear();
                }
            }
            else //если пришла неподходящая активность - пустой такт 
            {
                //если текущий пауза меньше разрешенной
                ПришедшиеАктивности.Clear();
            }

            //if (поступившаяАктивность != null) //если это не пустой такт
            //{

            //}


        }
    }
}
