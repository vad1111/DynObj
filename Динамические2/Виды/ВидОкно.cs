using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Динамические;

namespace Виды
{

    // Окно , за окном находиться пейезаж, т.е оно прокручивается, за окном всегда есть рама
    // Рама , или картина не прокручивается, размеры ее можно меенять 
    // внутрение объекты могут иметь фиксированый размер, могут расширяться до границ контейнера, сжиматься до размеров содержимого 
    // разделять пространство между соседними объектами (элементы группы делят пространство) группа может разделять, пространство
    // список теор имеет бесконечное вниз пространство , но в его раму влазят только несколько, группа содержит элементы которым запрещено расширяться вниз
    // 
    public class ВидОкно : ОбщийВид
    {
        public Window Окно = new Window();
        //public Window Фон = new Window();
        public Мышь Мышь;

        public static class WindowsServices
        {
            const int WS_EX_TRANSPARENT = 0x00000020;
            const int GWL_EXSTYLE = (-20);

            [DllImport("user32.dll")]
            static extern int GetWindowLong(IntPtr hwnd, int index);

            [DllImport("user32.dll")]
            static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

            public static void SetWindowExTransparent(IntPtr hwnd)
            {
                var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            }
        }
        public ВидОкно()
        {
            Мышь = Мышь.Мыши;
            Мышь.Окно = this;
            Мышь.Заполнить();
            //Мышь.ПодписатьсяНасобытие(this, "любое"); // обычно сюда посылают делегат, чтобы сама мышь покапалась во внутренностях, хотя можно получать его обратно
            Focusable = true;
            Окно.Focusable = true;
            Окно.AddHandler(UIElement.PreviewMouseRightButtonUpEvent, new MouseButtonEventHandler(ВидОкно_PreviewMouseRightButtonUp), true);
            Окно.AddHandler(UIElement.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(ВидОкно_PreviewMouseLeftButtonUp), true);
            //Окно.PreviewMouseRightButtonUp += ВидОкно_PreviewMouseRightButtonUp;
            //Окно.MouseRightButtonDown += ВидОкно_PreviewMouseRightButtonUp;

            //Фон.AllowsTransparency = true;
            //Фон.ShowInTaskbar = false;
            //Фон.Left = 0;
            //Фон.Top = 0;
            //Фон.Height = SystemParameters.MaximizedPrimaryScreenHeight;
            //Фон.Width = SystemParameters.PrimaryScreenWidth;
            //Фон.Background = new SolidColorBrush(new Color() { A = 100, B = 255, G = 255, R = 255 });
            ////Фон.Opacity = 0.25;
            //Фон.WindowStyle = WindowStyle.None;
            //Фон.SourceInitialized += (sender, e) =>
            //{
            //    var hwnd = new WindowInteropHelper(Фон).Handle;
            //    WindowsServices.SetWindowExTransparent(hwnd);
            //};

            //Окно.Topmost = true;
            //Окно.TaskbarItemInfo.Overlay

        }
        


        public override void ДобавьСодержание(РАМОбъект объект)
        {
            var вид = ВыбратьВид(объект);

            var видГруппа = вид as ВидГруппаОбъектов;
            if (видГруппа != null)
            {
                видГруппа.Редактируемый = true;
            }

            вид.Focus();

            Окно.Content = вид;

        }

        public ОбщийВид НайдиВерхнийОбщийВид(object надКем)
        {
            var предок = (DependencyObject) надКем;
            ОбщийВид предоквид;

            метка:
            предоквид = предок as ОбщийВид;
            if (предоквид!=null) { return предоквид; }

            if (предок is Window) return this;

            //var лог = LogicalTreeHelper.GetParent(предок);
            предок = VisualTreeHelper.GetParent(предок);
            //предок = (FrameworkElement)предок.Parent;

            goto метка;

        }

        private void ВидОкно_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var надКем = НайдиВерхнийОбщийВид(e.OriginalSource);
            надКем.АктивироватьСебяВГруппе();
        }

        private void ВидОкно_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

            var надКем = НайдиВерхнийОбщийВид( e.OriginalSource) ; 
            // после удаления всех элементов из группы Группы невидно, но она есть и ее меню должно работать
            if (надКем == this) надКем = (ОбщийВид) Content;

            var контекстноеМеню = new ContextMenu();
           
            var СписокМеню = надКем.ДайКонекстноеМЕнюКоллекцию();
            foreach (var подменю in СписокМеню)
            {
                контекстноеМеню.Items.Add(подменю);
            }

            Окно.ContextMenu = контекстноеМеню;
        }

        public override MenuItem ДайКонекстноеМеню()
        {
            return new MenuItem { Header = "Это Окно", IsEnabled = false };
        }

        public override List<MenuItem> ДайКонекстноеМЕнюКоллекцию()
        {
            var Список = new List<MenuItem>();
            Список.Add(new MenuItem { Header = "Это Окно", IsEnabled = false });
            return Список ;
        }

        public override object ОбработайСобытие(object Откого, object параметры)
        {
            Action метод = (Action)параметры;
            метод.Invoke();

            if (Откого is Мышь)
            {

            }
            return null;
        }

        

        public void Показать()
        {

            //Фон.Show();
            Окно.Show();
            //Фон.Owner = Окно;

        }
    }
}
