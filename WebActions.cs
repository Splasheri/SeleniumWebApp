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
    // Класс WebActions содержащий в себе большую часть логики всего приложения
    // Перечисление Selectors помогает поддерживать универсальность методов FillItemList и PerformChoise
    // Для контейнеров и кнопок, связанных с выбором языком созданы отдельные методы, так как у формы выбора языка сильно отличается html-код и присутствует множественный выбор
    //
    // Набор констант                                   - css-селекторы, XPath выражения и ссылки    
    //
    // dataContainers, languageContainer, jobContainer  - ссылки на listBox'ы, распологающиеся на форме
    //
    // jobNumber                                        - label, хранящий в себе количество доступных вакансий
    //
    // WebActions(containers...)                        - конструктор, в котором задаются ссылки на необходимые элементы формы
    //
    // WebActions()                                     - перегруженный конструктор для консольной версии
    //
    // StartBrowser                                     - метод, открывающий браузер, разворачивающий его на весь экран и открывающий ссылку на https://careers.veeam.com/
    //
    // CloseBrowset                                     - метод, вызывающийся в конце работы приложения, закрывающий браузер и освобождающий память под переменную Browser
    //
    // UpdateContainers                                 - метод, обновляющий данные во всех listBox
    //
    // FillItemList                                     - метод, заполняющий listBox с странами, городами и должностями. 
    //                                                  Параметры: selector указывает на то, какой именно это listBox; currentList - ссылка на сам listBox
    //
    // FillLanguageList                                 - метод, заполняющий listBox с языками.
    //
    // PerformChoice                                    - метод, выполняющий выбор фильтра
    //                                                  Параметры: selector указывает на то, какой именно это фильтр
    //
    // PerformChoice(Selectors selector, string item)   - перегрузка метода для использования с cmd
    //                                                  Параметры: item  - значение, по которому выполняется фильтрация
    // TryPerformChoice(Selectors selector, string item)- метод для использования с cmd, проверяет существование item, затем вызывает PerformChoise
    //                                                  Параметры: item  - значение, по которому выполняется фильтрация
    //
    // LanguageClick                                    - метод, выполняющий фильтрацию по выбранным языкам
    //
    // LanguageClick(List<string> languages)            - перегрузка метода для использования с cmd
    //                                                  Параметры: languages - значения, по котороым выполняется фильтрация
    //
    // DisplayJobNumber                                 - запись в jobNumber количества доступных вакансий и выполняющая заполение jobContainer
    //
    // DisplayJobNumber(System.IO.TextWriter console)   - перегрузка, выполняющая вывод в консоль количества доступных вакансий
    //                                                  Параметры: console - ссылка на Console.out
    //
    // FillJobList                                      - заполнение listBox названиями вакансий и ссылками на них. Название записывается в content, а ссылка - в тэг
    //
    // OpenVacancyPage                                  - метод, открывающий в браузере ссылку на выбранную вакансию
    //
    // OpenVacancyPage(object sender, RoutedEventArgs e)- перегрузка для использования с двойным кликом по вакансии в списке
    //
    // CompleteTask                                     - выполнение тестового задания

    public class WebActions
    {
        public enum Selectors
        {
            country = 0,
            city = 1,
            department = 2
        };

        private const string LANGUAGE_SELECTOR  = "#language";
        private const string NUM_OF_JOBS        = ".vacancies-blocks h3";
        private const string APPLY_BUTTON       = ".selecter-fieldset-submit";
        private const string LOAD_MORE_BUTTON   = ".load-more-button:not(.hide)";
        private const string VACANCIES_URL      = ".vacancies-blocks-item-header";
        private const string VACANCIES_TEXT     = ".vacancies-blocks-item-header h4";
        private const string NUM_OF_LANGUAGES   = "//label/input[not(@disabled)]/parent::label";
        private const string TASK_URL                = "https://careers.veeam.com/";

        private IWebDriver Browser;
        private readonly List<ListBox> dataContainers;
        private readonly ListBox languageContainer;
        private readonly ListBox jobContainer;
        private readonly Label jobNumber;

        internal WebActions(List<ListBox> _dataContainers, ListBox _languageContainer, ListBox _jobContainer, Label _jobNumber)
        {
            dataContainers = _dataContainers;
            languageContainer = _languageContainer;
            jobNumber = _jobNumber;
            jobContainer = _jobContainer;
            StartBrowser();
        }

        internal WebActions()
        {
            dataContainers = new List<ListBox>();
            languageContainer = null;
            jobNumber = null;
            jobContainer = null;
            StartBrowser();
        }

        internal  void StartBrowser()
        {
            Browser = new OpenQA.Selenium.Chrome.ChromeDriver();
            Browser.Manage().Window.Maximize();
            Browser.Navigate().GoToUrl(TASK_URL);

            UpdateContainers();
        }

        internal  void CloseBrowser()
        {
            Browser.Close();
            Browser.Dispose();
        }

        internal  void UpdateContainers()
        {
            foreach (var listBox in dataContainers)
            {
                FillItemList(
                    (WebActions.Selectors)dataContainers.IndexOf(listBox),
                    listBox);
            }
            FillLanguageList();
            DisplayJobNumber();
        }

        private   void FillItemList(Selectors selector, ListBox currentList)
        {
            if (currentList!=null)
            {
                currentList.Items.Clear();

                List<IWebElement> values = Browser.FindElements(By.CssSelector("#" + selector.ToString() + "-element  .closed:not(.disabled) span.selecter-item:not(.disabled)")).ToList();

                foreach (var item in values)
                {
                    currentList.Items.Add(new ListBoxItem().Content = item.GetAttribute("textContent"));
                }

                currentList.SelectedIndex = 0;
            }
        }

        private   void FillLanguageList()
        {
            if (languageContainer!=null)
            {
                languageContainer.Items.Clear();

                List<IWebElement> values = Browser.FindElements(By.XPath(NUM_OF_LANGUAGES)).ToList();

                foreach (var item in values)
                {
                    languageContainer.Items.Add(new ListBoxItem().Content = item.GetAttribute("textContent").Replace(" ", "").Replace("\r\n", ""));
                }

                languageContainer.SelectedIndex = 0;
            }
        }

        internal  void PerformChoice(Selectors selector)
        {

            if (Browser.Url != TASK_URL)
            {
                Browser.Navigate().GoToUrl(TASK_URL);
            }
            else
            {
                var item = dataContainers[(int)selector].SelectedValue as string;
                if (item != null)
                {
                    Browser.FindElement(By.CssSelector("#" + selector.ToString() + "-element div.closed")).Click();
                    Browser.FindElement(By.XPath("//div[contains(@class, 'scroller-content')]/span[contains(text(), '" + item + "')]")).Click();

                    System.Threading.Thread.Sleep(2000);

                    UpdateContainers();
                }
            }
        }

        private  void PerformChoice(Selectors selector, string item)
        {
            if (Browser.FindElements(By.CssSelector("#" + selector.ToString() + "-element div.closed:not(.disabled)")).ToArray().Length>0)
            {
                Browser.FindElement(By.CssSelector("#" + selector.ToString() + "-element div.closed:not(.disabled)")).Click();
                Browser.FindElement(By.XPath("//div[contains(@class, 'scroller-content')]/span[contains(text(), '" + item + "')]")).Click();

                System.Threading.Thread.Sleep(2000);

                UpdateContainers();
            }
        }

        internal  bool TryPerformChoise(Selectors selector, string item)
        {
            if (Browser.FindElements(By.XPath("//div[contains(@class, 'scroller-content')]/span[contains(text(), '" + item + "')]")).ToList().Count==0)
            {
                return false;
            }
            else
            {
                PerformChoice(selector,item);
                return true;
            }
        }

        internal  void LanguageClick()
        {
            if (Browser.Url != TASK_URL)
            {
                Browser.Navigate().GoToUrl(TASK_URL);
            }
            else
            {
                if (languageContainer.SelectedItems.Count != 0)
                {
                    Browser.FindElement(By.CssSelector("#language")).Click();
                    foreach (var item in languageContainer.SelectedItems)
                    {
                        Browser.FindElement(By.XPath("//label[contains(., '" + item.ToString() + "')]/span")).Click();
                    }
                    Browser.FindElement(By.CssSelector(".selecter-fieldset-submit")).Click();

                    System.Threading.Thread.Sleep(2000);

                    UpdateContainers();
                }
            }
        }

        internal void LanguageClick(List<string> languages)
        {
            Browser.FindElement(By.CssSelector(LANGUAGE_SELECTOR)).Click();
            foreach (var item in languages)
            {
                if (Browser.FindElements(By.XPath("//label[contains(., '" + item.ToString() + "')]/span")).ToList().Count>0)
                {
                    Browser.FindElement(By.XPath("//label[contains(., '" + item.ToString() + "')]/span")).Click();
                }
            }
            Browser.FindElement(By.CssSelector(APPLY_BUTTON)).Click();

            System.Threading.Thread.Sleep(2000);

            UpdateContainers();
        }

        private   void DisplayJobNumber()
        {
            if (jobNumber!=null)
            {
                string numOfJobs = Browser.FindElement(By.CssSelector(NUM_OF_JOBS)).GetAttribute("textContent");
                jobNumber.Content = numOfJobs.Substring(0, numOfJobs.IndexOf(" ")) + " jobs";
                if (Browser.FindElements(By.CssSelector(LOAD_MORE_BUTTON)).ToList().Count == 1)
                {
                    Browser.FindElement(By.CssSelector(LOAD_MORE_BUTTON)).Click();
                    System.Threading.Thread.Sleep(2000);
                }
                FillJobList();
            }
        }

        internal   void DisplayJobNumber(System.IO.TextWriter console)
        {
            string numOfJobs = Browser.FindElement(By.CssSelector(NUM_OF_JOBS)).GetAttribute("textContent");
            console.WriteLine(numOfJobs.Substring(0, numOfJobs.IndexOf(" ")) + " jobs");                        
        }

        private   void FillJobList()
        {
            jobContainer.Items.Clear();

            List<IWebElement> vacancies = Browser.FindElements(By.CssSelector(VACANCIES_TEXT)).ToList();
            List<string> names = new List<string>();

            foreach (var job in vacancies)
            {
                names.Add(job.GetAttribute("textContent"));
            }

            vacancies.Clear();
            vacancies = Browser.FindElements(By.CssSelector(VACANCIES_URL)).ToList();

            for (int i = 0; i < vacancies.Count; i++)
            {
                ListBoxItem newItem = new ListBoxItem();
                newItem.Content = names[i];
                newItem.Tag = vacancies[i].GetAttribute("href");
                newItem.MouseDoubleClick += OpenVacancyPage;
                jobContainer.Items.Add(newItem);
            }

            jobContainer.SelectedItem = 0;
        }

        internal  void OpenVacancyPage()
        {
            Browser.Navigate().GoToUrl(((ListBoxItem)(jobContainer.SelectedItem)).Tag as string);
        }

        private   void OpenVacancyPage(object sender, RoutedEventArgs e)
        {
            OpenVacancyPage();
        }

        internal  void CompleteTask()
        {
            PerformChoice(Selectors.country, "Romania");
            LanguageClick(new List<string>() { "English" });
        }

    }
}
