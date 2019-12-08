using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Виды;
using Динамические.Примитивы;

namespace Динамические.Виды
{
    public class ВидСимвола: ОбщийВид
    {
        public TextBlock ТекстБлок = new TextBlock();
        private Символ символ=>(Символ)Объект;

        public ВидСимвола()
        {
            Content = ТекстБлок;
            
        }

        public override void ДобавьСодержание(РАМОбъект объект)
        {
            var с = объект as Символ;
            if(с==null) return;
            _объект = с;

            ТекстБлок.Text = символ.Значение.ToString();
            var связь = new Binding() { Source = ВидРодитель, Path = new PropertyPath("Background") };
            this.SetBinding(BackgroundProperty, связь);

        }

       
        public override void Фокус()
        {
            //ВидРодитель.Фокус();
        }
    }
}
