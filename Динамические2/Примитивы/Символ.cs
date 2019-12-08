using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Динамические.Примитивы
{
    public class Символ: Примитив
    {
        public char Значение;
        public Символ() { }
        public Символ(char символ)
        {
            Значение = символ;
        }

        public override string ToString()
        {
            return Значение.ToString();
        }
    }
}
