using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Динамические;

namespace ИИ
{
    public class Коллекция : ObservableCollection<РАМОбъект>
    {
        public  new void Add(РАМОбъект примитив)
        {
            ((ObservableCollection<РАМОбъект>)this).Add(примитив);
            // сделать что-то
        }
        new public int Count { get { return ((ObservableCollection<РАМОбъект>) this).Count; } } 
    }
}
