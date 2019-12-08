using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ИИ
{
    /// <summary>
    /// Логика взаимодействия для Текстовый_Ввод.xaml
    /// </summary>
    /// тоже можно отнести к ативностям
    public partial class Текстовый_Ввод : Window
    {
        public Текстовый_Ввод()
        {
            InitializeComponent();
        }

        public Входное_устройство Активность; // относится к группе исходящих адресов 

        private void TextBoxТекст_KeyUp(object sender, KeyEventArgs e)
        {
            var текст = TextBoxТекст.Text;
            if (e.Key == Key.Enter)
            {
                Активность.ОтошлиСообщения(текст);
                TextBoxТекст.Text = "";
            }
            
        }

        private void TextBoxТекст_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //foreach (var textChange in e.Changes)
            //{
            //    //textChange.Offset
            //}
            //var текст = TextBoxТекст.Text;
            //Активность.ОтошлиСообщения(текст);
            
        }
    }
}
