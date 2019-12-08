using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Динамические;

namespace  Виды
{
    public class ОбщийВид : UserControl
    {
        public ОбщийВид ВидРодитель;
        public Dictionary<Type, Type> ВыборВидов;
        protected РАМОбъект _объект;

        public РАМОбъект Объект
        {
            get { return _объект; }
            set { ДобавьСодержание(value); }
        }

        public ОбщийВид()
        {
            
            //MouseLeftButtonUp += ОбщийВид_MouseLeftButtonUp;
            Background = Brushes.White;
            //Focusable = true;
        }
        

        public void АктивироватьСебяВГруппе()// это посылка сообщения родителю
        {
            if(ВидРодитель==null) return;

            var видР = ВидРодитель as ВидГруппаОбъектов;

            if (видР != null)
            {
                if (!видР.ЭтоПустаяГруппа )
                {
                    if (Объект.СобственныйАдресПримитива != null)
                        видР.НомерВыбранногоЭлемента = ((АдресВГруппе)Объект.СобственныйАдресПримитива).НомерВГруппе;
                    else видР.НомерВыбранногоЭлемента = -1;
                }

                видР.АктивироватьСебяВГруппе();
            }
            else
            {
                ВидРодитель.АктивироватьСебяВГруппе();
            }
            
            Фокус();
        }
        public ОбщийВид ВыбратьВид(РАМОбъект объект)
        {
            // если это группа то выбрать свернутый вариант
            var тип = ВыборВидов.ContainsKey(объект.GetType()) ? ВыборВидов[объект.GetType()] : typeof(ОбщийВид);
            var вид = (ОбщийВид)тип.GetConstructor(Type.EmptyTypes).Invoke(null);

            вид.ВыборВидов = ВыборВидов;
            вид.ВидРодитель = this;

            вид.ДобавьСодержание(объект);
            return вид;
        }

        public virtual void Фокус()
        {
            //Focus();
        }

        private void ОбщийВид_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var видРодителя = ВидРодитель as ВидГруппаОбъектов;
            //if (видРодителя != null)
            //{
            //    видРодителя.НомерВыбранногоЭлемента = ((АдресВГруппе) Объект.СобственныйАдресПримитива).НомерВГруппе;

            //    видРодителя.Редактируемый = true;

            //}
            //e.Handled = true; //вниз не передается
        }

        public virtual void ДобавьСодержание(РАМОбъект объект)
        {
            _объект = объект;

            Focusable = true;

            BorderThickness = new Thickness(4);
            BorderBrush = Brushes.Azure;

            //MouseRightButtonUp += ПраваяКнопкаМышиВВерх;
            //AddHandler(UIElement.PreviewMouseRightButtonUpEvent, new MouseButtonEventHandler(ПраваяКнопкаМышиВВерх), true);
            //Content = null;

            Content = Объект;
        }

        public virtual void ОбновитьВид()
        {
            Content = null;
            Content = Объект;
        }

        private void ПраваяКнопкаМышиВВерх(object sender, MouseButtonEventArgs e)
        {
            var контекстноеМеню = new ContextMenu();
            
        }

        public virtual List<MenuItem> ДайКонекстноеМЕнюКоллекцию()
        {
            var СписокМеню = new List<MenuItem>();

            var своеМеню = new MenuItem
            {
                Header = "Объект: " + Объект.GetType().Name + " (адрес:" + Объект.СобственныйАдресПримитива + ")"
            };
            СписокМеню.Add(своеМеню);

            var заголовок = new TextBlock() {Text = "Свойства"};
            var менюЗаголовка = new MenuItem {Header = заголовок, IsEnabled = false};
            СписокМеню.Add(менюЗаголовка);


            var менюРодитель = new MenuItem
            {
                Header = "Родитель: "
            };
            СписокМеню.Add(менюРодитель);
            var менюРодителя = ВидРодитель.ДайКонекстноеМЕнюКоллекцию();
            foreach (var подменю in менюРодителя)
            {
                менюРодитель.Items.Add(подменю);
            }
            var МенюВид = new MenuItem
            {
                Header = "Вид: "+ GetType().Name
            };
            var МенюСвойстваВида = new MenuItem
            {
                Header = "Свойства Вида" 
            };
            МенюВид.Items.Add(МенюСвойстваВида);
            СписокМеню.Add(МенюВид);

            return СписокМеню;

        }

        public virtual void ПроизошлоСобытие(string v, ОбщийВид ПредокОткого) // обработка событиий от предков или от родителей 
        {
            ;
        }

        public virtual object ОбработайСобытие(object Откого, object параметры) //обработка событий от дивайсов
        {
            // проверить от кого пришло событие мышь, клава
            // если мышь , посмотреть тип события 
            // выбрать метод, запустить метод
            // события от клавы и мыши могут пересекаться что-то придет раньше например нажатие CTRL

            return null;
        }

        public virtual MenuItem ДайКонекстноеМеню()
        {
            var заголовок = new MenuItem { Header = Объект.GetType().Name };
            return заголовок;
        }

       
        public virtual object РасчитатьРазмер()
        {
            return null;

        }
    }
}
