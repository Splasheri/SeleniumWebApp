using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        //импорт библиотеки с winapi методом подключения консоли к приложению
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);

        //если приложение запущено с аргументами(через командную строку) - окно не открывается
        private void Application_Startup(object sender, StartupEventArgs e)
        {            
            if (e.Args.Count()==0)
            {
                MainWindow wnd = new MainWindow();
                wnd.Show();
            }
            else
            {
                AttachConsole(-1);
                ConsoleInterface consoleInterface = new ConsoleInterface(e.Args);
                consoleInterface.WaitForExit();                
            }
        }
    }
}
