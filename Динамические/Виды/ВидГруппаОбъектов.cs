using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Динамические;
using Динамические.Виды;
using Динамические.Примитивы;

namespace Виды
{
   
    //  
    /// <summary>
    /// Логика взаимодействия для ВидГруппаОбъектов.xaml
    /// </summary>
    /// 
    

    public class ВидГруппаОбъектов : ОбщийВид
    {
        public ГруппаОбъектов Группа => (ГруппаОбъектов) Объект;
        public StackPanel Панель = new StackPanel();
        ScrollViewer скрол = new  ScrollViewer();
        public UIElementCollection СписокВидов => Панель.Children;
        public ОбщийВид this[ int индекс] => (ОбщийВид) Панель.Children[индекс];
        public ОбщийВид ВыбранныйЭлемент => Группа.Список.Count>=0?(ОбщийВид) Панель.Children[_номерВыбранногоЭлемента+1]: ВидПустогоСписка;
        public bool ЭтоПустаяГруппа=>Группа.Список.Count==0;
        public ОбщийВид ВидПустогоСписка;

        protected int _номерВыбранногоЭлемента=-1;    // может быть -1, если Группа пуста

        public ВидГруппаОбъектов()
        {
            //ВидПустогоСписка = new ВидКурсорВставки
            //{
            //    Тип = ВидКурсорВставки.ТипКурсора.Вертикальный,
            //    ВидРодитель = this
            //};
            ВидПустогоСписка = new ВидСимвола() { ВидРодитель = this };
            ВидПустогоСписка.ДобавьСодержание(new Символ('+'));

            Focusable = true;

            Content = скрол;

            Панель.CanVerticallyScroll = true;
            //Панель.ScrollOwner = скрол;

            скрол.CanContentScroll = true;
            скрол.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            скрол.Content = Панель;

            //MouseLeftButtonUp += ВидГруппаОбъектов_MouseLeftButtonUp;
            //KeyUp += ВидГруппаОбъектов_KeyUp;
            //MouseRightButtonUp += РеакцияПраваяКнопкаМышиВВерх;
            //PreviewMouseLeftButtonDown += ВидГруппаОбъектов_MouseLeftButtonDown;
        }

        public int НомерВыбранногоЭлемента
        {
            get { return _номерВыбранногоЭлемента; }
            set
            {
                var предыдущий = _номерВыбранногоЭлемента;
                if (value <= -1 )
                {
                    _номерВыбранногоЭлемента = -1;
                }
                else if ( value >= СписокВидов.Count-1)
                {
                    _номерВыбранногоЭлемента = СписокВидов.Count - 2;
                }
                else
                {
                     _номерВыбранногоЭлемента = value;
                }
                ПодкраситьВыбранный(_номерВыбранногоЭлемента,предыдущий);
            }
        }

        private bool _редактируемый;

        public bool Редактируемый
        {
            get
            {
                return _редактируемый;
            }
            set
            {
                _редактируемый=value;
                if (value)
                {
                    if (Клавиатура.ГруппаВФокусе != null) Клавиатура.ГруппаВФокусе.Редактируемый = false;
                    KeyUp += ВидГруппаОбъектов_KeyUp;
                    Клавиатура.ГруппаВФокусе = this;
                    // сделать : Добавить курсор
                    ДобавитьКурсор(_номерВыбранногоЭлемента);
                }
                else {
                    KeyUp -= ВидГруппаОбъектов_KeyUp;
                    // сделать: убрать курсор
                    УбратьКурсор(_номерВыбранногоЭлемента);
                }
            }
        }

        public virtual void ПодкраситьВыбранный(int новый, int старый)
        {
            if(СписокВидов.Count==0) return;

            УбратьКурсор(старый);
            ДобавитьКурсор(новый);
            
        }
        public virtual void УбратьКурсор(int старый)
        {
              if (старый>=-1 && старый < СписокВидов.Count)
            {
                this[старый+1].Background = Brushes.White;
                //this[старый].IsEnabled = false;
            }
        }
        public virtual void ДобавитьКурсор(int новый)
        {
             if (новый>=-1 && новый < СписокВидов.Count)
            { 
                this[новый+1].Background = Brushes.LightBlue;

                //this[новый].IsEnabled = true;
                //if (this[новый].Focusable)
                //this[новый].Фокус();
            }
        }

        public override void Фокус()
        {
            Focus();
            Редактируемый = true;
        }


        //private void ВидГруппаОбъектов_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    // найти элемент над которым произошло
        //    // или перенести это общий вид он смотрит если родитель группа, то меняет текущий
        //}

        public override void ПроизошлоСобытие(string v, ОбщийВид вид)
        {
            if (v == "леваяКнопкаМыши")
            {
                var номер = Панель.Children.IndexOf(вид);
                НомерВыбранногоЭлемента = номер;
            }
            //base.ПроизошлоСобытие(v,this);
            if (ВидРодитель != null) ВидРодитель.ПроизошлоСобытие(v, this);
        }

        private void ВидГруппаОбъектов_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //найти какой объект послал
            var откого =  e.Source;
            

        }

        private void РеакцияПраваяКнопкаМышиВВерх(object sender, MouseButtonEventArgs e)
        {
            //var контекстноеМеню = new ContextMenu();
           
            //var заголовок = new MenuItem { Header = "Это ГруппаОбъектов (адрес " + Группа.НомерВГруппе + ") в ", IsEnabled = true };
            //if (ВидРодитель != null)
            //{
            //    var менюРодителя = ВидРодитель.ДайКонекстноеМеню();
            //    контекстноеМеню.Items.Add(менюРодителя);
            //}

            //контекстноеМеню.Items.Add(заголовок);

            //var команда = new MenuItem {Header = "Добавить"};
            //команда.Click += КомандаДобавить_Click;
            //контекстноеМеню.Items.Add(команда);

            
            ////e.Handled = true;
            //ContextMenu = контекстноеМеню;
        }

        public override List<MenuItem> ДайКонекстноеМЕнюКоллекцию()
        {
            //var СписокМеню = new List<MenuItem>();
            var СписокМеню = base.ДайКонекстноеМЕнюКоллекцию();
            //var своеМеню = new MenuItem { Header = "Это ГруппаОбъектов (адрес " + Группа.СобственныйАдресПримитива + ") в ", IsEnabled = true };

            //СписокМеню.Add(своеМеню);

            //var менюРодителя = ВидРодитель.ДайКонекстноеМЕнюКоллекцию();
            //foreach (var подменю in менюРодителя)
            //{
            //    своеМеню.Items.Add(подменю);
            //}
            var команда = new MenuItem { Header = "Добавить" };
            команда.Click += КомандаДобавить_Click;
            СписокМеню.Add(команда);

            return СписокМеню;
        }

        public override MenuItem ДайКонекстноеМеню()
        {

            var меню = new MenuItem { Header = "ГруппаОбъектов", IsEnabled = true };
            меню.Items.Add(new MenuItem { Header = "Это ГруппаОбъектов (адрес " + Группа.СобственныйАдресПримитива + ")", IsEnabled = false });
            if (ВидРодитель != null)
            {
                var менюРодителя = ВидРодитель.ДайКонекстноеМеню();
                меню.Items.Add(менюРодителя);
            }

            var команда = new MenuItem { Header = "Добавить" };
            команда.Click += КомандаДобавить_Click;
            меню.Items.Add(команда);
            
            return меню;
        }

        private void КомандаДобавить_Click(object sender, RoutedEventArgs e)
        {
            var вставляемыйОбъект = new Пустота();
            Группа.Добавить( вставляемыйОбъект);

            var вид = ВыбратьВид(вставляемыйОбъект);  
            СписокВидов.Add( вид);
        }

        protected virtual void ВидГруппаОбъектов_KeyUp(object sender, KeyEventArgs событие)
        {
            if (событие.Key == Key.Up)
            {
                if (_номерВыбранногоЭлемента != -1)
                {
                    НомерВыбранногоЭлемента--;
                }
            }

            if (событие.Key == Key.Down)
            {
                if (_номерВыбранногоЭлемента < Группа.Список.Count-1)
                {
                    НомерВыбранногоЭлемента++;
                }
                
            }
            if (событие.Key == Key.Enter)
            {
                var группа = this[_номерВыбранногоЭлемента+1] as ВидГруппаОбъектов;
                if (группа != null)
                {
                    группа.Фокус();
                    //группа.Редактируемый=true;
                    //группа.ВыбранныйЭлемент.Фокус();
                }
            }
            if (событие.Key == Key.Escape)
            {
                var родитель = ВидРодитель as ВидГруппаОбъектов;
                if (родитель != null)
                {
                    родитель.Фокус();
                }
            }

            if (событие.Key == Key.Insert)
            {
                // здесь выбирается какой элемент вставить 

                var вставляемыйОбъект = new Пустота(); // тутможно вытавить выбор из Списка видов 

                if (НомерВыбранногоЭлемента == -1) _номерВыбранногоЭлемента = 0;
                Группа.Вставить(_номерВыбранногоЭлемента, вставляемыйОбъект);

                var вид = ВыбратьВид(вставляемыйОбъект); // нужно различать вставляемые группы и невставляемые

                //if(ЭтоПустаяГруппа) СписокВидов.Clear();

                Панель.Children.Insert(_номерВыбранногоЭлемента + 1, вид);
                for (int и = _номерВыбранногоЭлемента + 2; и < Группа.Список.Count+1; и++)
                {
                    var вид2 = (ОбщийВид)СписокВидов[и];
                    вид2.ОбновитьВид();
                }
                НомерВыбранногоЭлемента++;
                //Focus();

                событие.Handled = true;//событие обработано Запрет на дальнейшую обработку, можно обойти
            }

            if (событие.Key == Key.Delete)
            {
                if (НомерВыбранногоЭлемента>=-1
                     && НомерВыбранногоЭлемента < Группа.Список.Count - 1 && !ЭтоПустаяГруппа)
                {
                    Группа.Удалить(НомерВыбранногоЭлемента+1);
                    СписокВидов.RemoveAt(НомерВыбранногоЭлемента+2);

                    //for (int и = НомерВыбранногоЭлемента+2; и < Группа.Список.Count+1; и++)
                    //{
                    //    var вид = (ОбщийВид) СписокВидов[и];
                    //    вид.ОбновитьВид();
                    //}


                }
                //if (ЭтоПустаяГруппа) СписокВидов.Add(ВидПустогоСписка);
            }
            
        }

        public override void ОбновитьВид()
        {
            
        }

        public override void ДобавьСодержание(ПримитивИлиАдрес объект)
        {
            
            _объект=объект;

            Панель.Children.Clear();
            Панель.Children.Add(ВидПустогоСписка);

            Панель.Margin = new Thickness(20, 0, 0, 0);

            if (!ЭтоПустаяГруппа)
            {
                for (var и = 0; и < Группа.Список.Count; и++)
                {
                    var объект2 = Группа[и];
                    var вид = ВыбратьВид(объект2);
                    объект2.Вид = вид;

                    //если это будет группа то появиться новый список, что не всегда хорошо, если он не большой меньше 10 и есть место

                    // расчитать размер вида 

                    //var размер= вид.RenderSize;
                    //var размерВизуал = VisualTreeHelper.GetContentBounds(вид);
                    //var размерВсехПотомков = VisualTreeHelper.GetDescendantBounds(вид);

                    Панель.Children.Add(вид);
                    _номерВыбранногоЭлемента = 0;
                   
                }

            }
            else
            {
            }
            ПодкраситьВыбранный(_номерВыбранногоЭлемента, _номерВыбранногоЭлемента);
        }






        public double Высота => Height;
        public double Ширина => Width;

        public void Расположение()
        {

        }
    }

}
