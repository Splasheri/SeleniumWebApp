using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WpfApp1
{
    //  ConsoleInterface - класс для взаимодействия с cmd
    //
    //  ConsoleInterface - конструктор, выполняющий проверку количества аргументов и вызывающий метод Execute
    //                   Параметры: args - аргументы командной строки   
    //
    //  CheckArguments   - проверка количества аргументов. отправляет ошибку если их меньше 4 или больше 6
    //  Execute          - по очереди выполняет действие TryPerformChoise для всех аргументов 
    //                   затем формирует список последних параметров(возможно языков) и выполняет для них LanguageClick                    
    //                   в конце концов выполняется консольный  DisplayJobNumber
    //
    // WaitForExit       - ожидает пользовательского ввода, затем закрывает браузер и освобождает память
    class ConsoleInterface
    {
        WebActions webActions;
        string[] arguments;

        internal ConsoleInterface(string[] args)
        {
            webActions = new WebActions();

            arguments = args;
            try
            {
                CheckArguments();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Execute();

        }

        private void CheckArguments()
        {
            if (arguments.Length>6 || arguments.Length < 4)
            {
                throw new ArgumentException("Wrong number of parameters");
            }
        }

        private void Execute()
        {
            if (!webActions.TryPerformChoise(WebActions.Selectors.country, arguments[0]))
            {
                webActions.TryPerformChoise(WebActions.Selectors.country, "All countries");
            }

            webActions.TryPerformChoise(WebActions.Selectors.city, arguments[1]);
            webActions.TryPerformChoise(WebActions.Selectors.department, arguments[2]);

            List<string> perhapsLanguages = new List<string>();

            for (int i = 3; i < arguments.Length; i++)
            {
                perhapsLanguages.Add(arguments[i]);
            }

            webActions.LanguageClick(perhapsLanguages);

            webActions.DisplayJobNumber(Console.Out);
        }

        internal void WaitForExit()
        {
            Console.ReadLine();
            webActions.CloseBrowser();
        }
    }
}
