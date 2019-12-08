using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Динамические
{
    public class Число : Примитив
    {
    }

    public class ЦелоеУниверсальное : Число
    {
        public byte ДлинаЧисла; //количество байт, изначально =0, int= 4 
        public bool Положительное = true;
        public byte[] Число;
        public static ЦелоеУниверсальное Сложить(ЦелоеУниверсальное сл1, ЦелоеУниверсальное сл2)
        {
            var результат = new ЦелоеУниверсальное();

            return результат;
        }

    }
}
