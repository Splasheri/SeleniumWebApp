using System;
using System.Collections.Generic;
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
using OpenQA.Selenium;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        WebActions webActions;

        //при запуске
        public MainWindow()
        {
            InitializeComponent();
            
            webActions = new WebActions(
                new List<ListBox>()
                { CountryList, CityList, FunctionList },
                LanguageList,
                JobList,
                JobCounter
                );

        }

        //закрытие окна
        private void Window_Closed(object sender, EventArgs e)
        {
            webActions.CloseBrowser();
        }

        //выбор одного из первых трех параметров
        private void AnyClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            webActions.PerformChoice(
                (WebActions.Selectors)Enum.Parse(
                    typeof(WebActions.Selectors),
                    button.Tag as string
                )
            );
        }

        //открытие страницы вакансии
        private void OpenJobLink_Click(object sender, RoutedEventArgs e)
        {
            webActions.OpenVacancyPage();
        }

        //выбор языка
        private void LanguageChoise_Click(object sender, RoutedEventArgs e)
        {
            webActions.LanguageClick();
        }

        //выполнение задачи из задания
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            webActions.CompleteTask();
        }
    }    
}
