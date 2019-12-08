﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Динамические.Группы
{

    // Это к системе распознавания 

    // это АдресВгруппе только вместо Целого Здесь массив флагов или бит
    // если Описание заменить на Значение 
    // это еще похоже свойство гет
    // При смене приложения элемент, может менять адрес группы на локальный
    // тогда в элменте появиться два адреса, удаленный адрес группы, и локальный
    // как обобщение адресов может быть много
    // если группа создана и поддерживается тртьей стороной (владелец),то этот адрес будет УНомером

    public class ЭлементПеречисления
    {
        public ГруппаПеречисления Группа;
        public byte[] флаги;// биты лучше набору флагов это аналог номера в Адресе Группы
        public СтрокаЮникода Описание => Группа.ДайОписание(this);// для флагов это будет список Строк

        public bool Совпадает(ЭлементПеречисления другойЭлемент)
        {
            if(Группа!=другойЭлемент.Группа) return false;
            if(флаги.Length!= другойЭлемент.флаги.Length) return false;
            // длины могут не совпадать
            for (int и = 0; и < флаги.Length; и++)
            {
                if (флаги[и] != другойЭлемент.флаги[и]) return false;
            }
            return true;
        }

        // у текущего элемента заполнены теже флаги что и другого плюс есть еще
        // аналог включения как в множестве
        public bool Включает(ЭлементПеречисления другойЭлемент)
        {
            if (Группа != другойЭлемент.Группа) return false;
            for (int и = 0; и < флаги.Length; и++)
            {
                if (другойЭлемент.флаги[и]!=0 && флаги[и] != другойЭлемент.флаги[и]) return false;
            }
            return true;
        }
        //public static bool operator ==(ЭлементПеречисления с1, ЭлементПеречисления с2)
        //{
        //    return с1.Совпадает(с2);
        //}
        //public static bool operator !=(ЭлементПеречисления с1, ЭлементПеречисления с2)
        //{
        //    return !с1.Совпадает(с2);
        //}

        

    }

    //Эта Группа отличается от ГруппыОбъектов, что нумерация от 0 и дальше
    // а имеет пропуски т.е. это словарь, Cловарь<Слово,ПримитивИлиАдрес>  у меня 
    // это словарь<ЭлементПереч, строка>
    // эТа группа может содержать элемнты разной размерности?
    // сеть активностей не упорядочена в отличии от списка или матрицы
    // какая размерность у активности , может быть дробной 
    // элемент матрицы иммеет 2 адреса на группу строки и группу столбца
    // или на 8 соседних элементов или 4 если не считать диагонали
    // соты это 6-и гранники и имеют 6 связей 
    public class ГруппаПеречисления
    {
        public List<ЭлементПеречисления> СписокЭлементов;
        public List<СтрокаЮникода> СписокОписанийЭлементов;

        public bool Добавить(ЭлементПеречисления элемент, СтрокаЮникода описание)
        {
            if (Содержит(элемент)) return false;
            СписокЭлементов.Add(элемент);
            СписокОписанийЭлементов.Add(описание);
            return true;
        }

        public bool Содержит(ЭлементПеречисления элемент)
        {
            foreach (var эл in СписокЭлементов)
            {
                if (эл.Совпадает(элемент)) return false;
            }
            return true;
        }

        internal СтрокаЮникода ДайОписание(ЭлементПеречисления элементПеречисления)
        {
            var  номер = СписокЭлементов.IndexOf(элементПеречисления);
            return СписокОписанийЭлементов[номер];
        }

        // не делает проверку на отсутсиве описания
        public ЭлементПеречисления ДайЭлементПоОписанию(СтрокаЮникода описание)
        {
            var номер = СписокОписанийЭлементов.FindIndex(оп => оп == описание);
            return СписокЭлементов[номер];
        }
    }

    // каждый ненулевой флаг имеет одно название
    public class ГруппаФлагов
    {
        public List<СтрокаЮникода> СписокОписанийЭлементов;

        public ЭлементПеречисления ДайЭлементПоОписанию(СтрокаЮникода описание)
        {
            var номер = СписокОписанийЭлементов.FindIndex(оп => оп == описание);
            var элемент = new ЭлементПеречисления {флаги = new byte[СписокОписанийЭлементов.Count]};
            элемент.флаги[номер] = 1;
            return элемент;
        }

        public ЭлементПеречисления ДайЭлементПоСпискуОписаний(СтрокаЮникода[] описания)
        {
            var элемент = new ЭлементПеречисления { флаги = new byte[СписокОписанийЭлементов.Count] };
            foreach (var описание in описания)
            {
                var номер = СписокОписанийЭлементов.FindIndex(оп => оп == описание);
                if(номер>=0)
                элемент.флаги[номер] = 1;
            }
            return элемент;
        }
    }

}
