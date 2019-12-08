using System.Windows;
using Динамические;

namespace ИИ
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class ОкноХранилища : Window
    {
        public static ОкноХранилища окноХранилища ;
        public ОкноХранилища()
        {
            окноХранилища = this;
            InitializeComponent();
            ListBoxХранилище.ItemsSource = Хранилище.Память;
        }

        public void ОбновитьОкно()
        {
            ListBoxХранилище.ItemsSource = null;
            ListBoxХранилище.ItemsSource = Хранилище.Память;
        }

        private void MenuItem_OnClick_Добавить(object sender, RoutedEventArgs e)
        {
            СтрокаЮникода.Создать("проба");
            ОбновитьОкно();

        }

        private void MenuItemClickСохранить(object sender, RoutedEventArgs e)
        {
           
           Хранилище.Сохранить();
        }

        private void MenuItem_OnClick_Востановить(object sender, RoutedEventArgs e)
        {
            Хранилище.Восстановить(Хранилище.ИмяФайла);
            ОбновитьОкно();
        }

        private void MenuItem_OnClick_Удалить(object sender, RoutedEventArgs e)
        {
            var элемент = ListBoxХранилище.SelectedItem as РАМОбъект;
            
            if(элемент== null) return;
            if(элемент is Активность) ((Активность)элемент).Закрыть();
            var номер = ((АдресВХранилище) элемент.СобственныйАдресПримитива).НомерВХранилище;
            Хранилище.Память[номер] = new Пустота(){СобственныйАдресПримитива = элемент.СобственныйАдресПримитива };
            ОбновитьОкно();

            //var listBoxItem = ListBoxХранилище.ItemContainerGenerator.ContainerFromItem(элемент);
        }

        private void MenuItem_OnClick_Обновить(object sender, RoutedEventArgs e)
        {
            ОбновитьОкно();
        }
    }
}
