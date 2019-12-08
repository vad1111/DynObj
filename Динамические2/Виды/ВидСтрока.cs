using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Динамические;

namespace Виды
{
    public class ВидСтрока : ОбщийВид
    {
        public TextBox ТекстБокс = new TextBox();
        //public TextBlock ТекстБлок = new TextBlock();
        public СтрокаЮникода строка => (СтрокаЮникода) _объект ;
        private ContextMenu ТекстБоксМеню;
            
        public ВидСтрока()
        {
            ТекстБокс.BorderThickness = new Thickness(0);

            PreviewMouseLeftButtonDown += ВидСтрока_PreviewMouseLeftButtonDown;
            ТекстБоксМеню = ТекстБокс.ContextMenu;
            ТекстБокс.ContextMenu = null;

            var связь = new Binding() {Source = this, Path = new PropertyPath("Background") };
            ТекстБокс.SetBinding(BackgroundProperty, связь);
            ТекстБокс.Focusable = false;

        }

        public override void Фокус()
        {
            ТекстБокс.Focus();
        }

        public override void ОбновитьВид()
        {

        }
        public override List<MenuItem> ДайКонекстноеМЕнюКоллекцию()
        {
            var СписокМеню = base.ДайКонекстноеМЕнюКоллекцию();

            var item = new MenuItem {Header = "Копировать"};
            item.Click += РеакцияКопировать;
            item.IsEnabled = ТекстБокс.SelectedText != "";
            if(ТекстБокс.SelectedText != "") СписокМеню.Add(item);
            item = new MenuItem { Header = "Вырезать" };
            item.Click += (o, args) => ТекстБокс.Cut(); 
            item.IsEnabled = ТекстБокс.SelectedText != "";
            if (ТекстБокс.SelectedText != "") СписокМеню.Add(item);

            item = new MenuItem() {Header = "Вставить"};
            item.Click += (o, args) => ТекстБокс.Paste();
            item.IsEnabled = Clipboard.ContainsText();
            СписокМеню.Add(item);

            return СписокМеню;
        }

        private void РеакцияКопировать(object sender, RoutedEventArgs routedEventArgs)
        {
            ТекстБокс.Copy();
           
        }

        private void ВидСтрока_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == ТекстБокс)
            {
                ВидРодитель.ПроизошлоСобытие("леваяКнопкаМыши", this);
                
            }
        }

        public override void ДобавьСодержание(РАМОбъект объект)
        {
            _объект = объект;
            ТекстБокс.Text = строка.Значение;
            //ТекстБлок.Text = строка.Значение;
            Content = ТекстБокс;
        }

     

        public void НачатьРедактирование()
        {
            Content = ТекстБокс;
        }

        public void ЗакончитьРедактирование()
        {
            строка.Значение = ТекстБокс.Text;
           
        }
    }
}
