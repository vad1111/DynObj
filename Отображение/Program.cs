using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Динамические;

namespace Отображение
{
    static class Program
    {
        private static readonly Dictionary<string, object> кол = new Dictionary<string, object>();
        private static Form1 f;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] имяФайла)
        {
            if (имяФайла.Length != 0)
            {
                try
                {
                    Хранилище.Восстановить(имяФайла[0]);
                }
                catch
                {
                    MessageBox.Show(@" Не удалось восстановить диспетчер");
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            var контекстноеМеню = new ContextMenu();
            var menu1 = new MenuItem();
            menu1.Text = @"Добавить визуальный элемент";
            menu1.Click += menu1_Click;
            кол["меню1"] = menu1;
            контекстноеМеню.MenuItems.Add(menu1);
            f = new Form1();
            f.ContextMenu = контекстноеМеню;

            Application.Run(f);
        }

        private static void menu1_Click(object sender, EventArgs e)
        {
            var список = new ListBox();
            список.Items.Add("строка");
            var контекстноеМеню = new ContextMenu();
            var menu1 = new MenuItem();
            menu1.Text = @"Добавить элемент";
            //menu1.Click += menu1_Click;
            контекстноеМеню.MenuItems.Add(menu1);
            список.ContextMenu = контекстноеМеню;
            var м1 = (MenuItem)кол["меню1"];
           
            //w.ContextMenu=null;

            контекстноеМеню.MenuItems.Add((MenuItem) кол["меню1"]);
            f.Controls.Add(список);
            //w.Content = список;
        }
    }
}
