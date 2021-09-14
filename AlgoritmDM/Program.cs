using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Threading;

//using System.IO;
using System.Diagnostics;

namespace AlgoritmDM
{
    static class Program
    {
        /// <summary>
        /// Флаг для работы сборщика муссора
        /// </summary>
        private static bool RunGC=true;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args) // -h -i
        {
            // Проверка на то чтобы не запускался ещё один экземпляр нашего приложения
            if (Process.GetProcesses().Count(x => x.ProcessName == Process.GetCurrentProcess().ProcessName) > 1)
            {
                MessageBox.Show(String.Format("Приложение с именем {0}, уже работает на этом компьютере.",Process.GetCurrentProcess().ProcessName));
                Process.GetCurrentProcess().Kill();
            }


            //int iyt = Com.RegFarm.GegUsr.RegItems.Count;
            //Com.RegFarm.GegUsr.RegItems.Add(new Lib.Reg.Reg(

            bool Ishelp = false;
            //string AutoStart = null;
            bool IsInterfase = true;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == @"-h" || args[i] == @"-H" || args[i] == @"-?" || args[i] == @"\h" || args[i] == @"\H" || args[i] == @"\?" || args[i] == @"/h" || args[i] == @"/H" || args[i] == @"/?") Ishelp = true;
                //if (args[i] == @"-s" || args[i] == @"\s" || args[i] == @"/s" || args[i] == @"-S" || args[i] == @"\S" || args[i] == @"/S") { i++; AutoStart = args[i]; }
                if (args[i] == @"-i" || args[i] == @"\i" || args[i] == @"/i" || args[i] == @"-I" || args[i] == @"\I" || args[i] == @"/I") IsInterfase = false;
            }

            try
            {                
                // Проверка, если пользователь вызвал справку, то запускать прогу не надо
                if (Ishelp)
                {
                    Console.WriteLine(@"-s ShortName (Name Task Auto Run)");
                    Console.WriteLine(@"-i (not run interface)");
                }
                else
                {
                    // Проверка по процессам, чтобы приложение было в единственном экземпляре.
                    //bool oneOnlyProg;
                    //Mutex m = new Mutex(true, Application.ProductName, out oneOnlyProg);
                    //if (oneOnlyProg == true)

                    // Инициализвция классов
                    Com.ConfigReg ConfReg = new Com.ConfigReg();
                    Com.Log Log = new Com.Log("AlgoritmDM.txt");
                    Com.Config Conf = new Com.Config("AlgoritmDM.xml");

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    // Проверка валидности лицензии
                    DialogResult rez = DialogResult.Yes;
                    while (!Com.Lic.isValid && rez == DialogResult.Yes)
                    {
                        if (!IsInterfase)
                        {
                            rez = DialogResult.No;
                            Console.WriteLine("У вас есть проблемы с лицензией. Обратитесь к вендору.");
                            if (!Ishelp) Com.Log.EventSave("У вас есть проблемы с лицензией. Обратитесь к вендору.", "Main", Lib.EventEn.Message);
                        }
                        else
                        {
                            Application.Run(new FLic());
                            //
                            if (!Com.Lic.isValid) rez = MessageBox.Show("Для запуска программы необходимо ввести лицензию, вы хотите это сделать?", "Лицензирование", MessageBoxButtons.YesNo);
                        }
                    }
                    // Проверка валидности лиценции
                    if (Com.Lic.isValid)
                    {
                        // Проверка режима с интерфейсом или нет
                        if (IsInterfase)
                        {
                            // Авторизуем пользователя
                            DialogResult rezExit = DialogResult.None;
                            while (Com.UserFarm.CurrentUser == null && rezExit == DialogResult.None)
                            {
                                // Пользователь авторизируется
                                using (FLogon Frm = new FLogon())
                                {
                                    rezExit = Frm.ShowDialog();

                                    // Если пользователь с админской записью существует, то даём возможность авторизоваться
                                    if (rezExit != DialogResult.OK && !Com.UserFarm.HashRoleUsers(Lib.RoleEn.Admin))
                                    {
                                        rezExit = MessageBox.Show("В системе нет ни одного администратора. Закрыть программу или войти под админской учёткой?", "Авторизация", MessageBoxButtons.OKCancel);
                                    }
                                }
                            }

                            // Если мы авторизованны то запускаем приложение
                            if (Com.UserFarm.CurrentUser != null)
                            {
                                // Асинхронный запуск процесса
                                Thread thr = new Thread(GarbColRun);
                                //thr = new Thread(new ParameterizedThreadStart(Run)); //Запуск с параметрами   
                                thr.Name = "AE_Thr_GC";
                                thr.IsBackground = true;
                                thr.Start();

                                Application.Run(new FStart());

                                // Ожидаем завершения работы сборщика мусора
                                RunGC = false;
                                thr.Join();
                            }
                        }
                        else
                        {
                            // Авторизуемся под системной учёткой
                            Lib.User consUsr = new Lib.User("Console", "123456", "Console", Lib.RoleEn.Admin);
                            Com.UserFarm.List.Add(consUsr, true, false);
                            Com.UserFarm.SetupCurrentUser(consUsr, "123456");

                            // Если мы авторизованны то запускаем приложение
                            if (Com.UserFarm.CurrentUser != null)
                            {
                                // Асинхронный запуск процесса
                                Thread thr = new Thread(GarbColRun);
                                //thr = new Thread(new ParameterizedThreadStart(Run)); //Запуск с параметрами   
                                thr.Name = "AE_Thr_GC";
                                thr.IsBackground = true;
                                thr.Start();

                                if (Com.ConfigurationFarm.CurrentCnfList == null)
                                {
                                    Com.Log.EventSave("не опрелелена текущая конфигурация.", "Main", Lib.EventEn.FatalError);
                                }
                                else
                                {
                                    // Запускаем чтение списка клиентов
                                    Com.CustomerFarm.List.onProcessedCustomerList += new EventHandler<Lib.EventCustomerListAsicRez>(List_onProcessedCustomerList);
                                    Com.ConfigurationFarm.CurrentCnfList.onProcessedCalculateDM += new EventHandler<Lib.EventConfigurationListAsicRez>(CurrentCnfList_onProcessedCalculateDM);
                                    Com.ConfigurationFarm.CurrentCnfList.onProcessedCalculateDMProgressBar += new EventHandler<Lib.EventConfigurationListProcessingProgerssBar>(CurrentCnfList_onProcessedCalculateDMProgressBar);
                                    Com.ConfigurationFarm.CurrentCnfList.onProcessedCalculateTotal += new EventHandler<Lib.EventConfigurationListAsicRez>(CurrentCnfList_onProcessedCalculateTotal);
                                    Com.ConfigurationFarm.CurrentCnfList.onProcessedCalculateTotalProgressBar += new EventHandler<Lib.EventConfigurationListProcessingProgerssBar>(CurrentCnfList_onProcessedCalculateTotalProgressBar);
                                    Com.CustomerFarm.List.onProcessedAployDMCalkMaxDiscPerc += new EventHandler<Lib.EventCustomerListAsicRez>(List_onProcessedAployDMCalkMaxDiscPerc);
                                    Com.CustomerFarm.List.onProcessedAployDMCalkMaxDiscPercProgressBar += new EventHandler<Lib.EventCustomerListProcessingProgerssBar>(List_onProcessedAployDMCalkMaxDiscPercProgressBar);
                                    Com.CustomerFarm.List.ProcessingCustomers();
                                }
                                // Ожидаем завершения работы сборщика мусора
                                thr.Join();
                            }

                        }
                    }
                }
                // Тестируем список пользователей
                //Com.UserFarm.List.Add(new Lib.User("H","h","H", Lib.RoleEn.Admin));
                //foreach (Lib.User item in Com.UserFarm.List){}

                // Тестируем шифрование
                //string str = Com.Lic.InCode("1234ABC000845wferhyuy534tewgryyu7684regtrh7886756634t4gfy67865v35435465g5buvc45v4564563");
                //string stro = Com.Lic.DeCode(str);
                //bool f = Com.Lic.isValid;
                //string ActivN = Com.Lic.GetActivNumber();
                //string lic = Com.Lic.GetLicNumber(ActivN, "20180101", "Инфа", false);
                //Com.Lic.IsValidLicKey(lic);
                //Com.Lic.RegNewKey(lic,ActivN);
                //f = Com.Lic.isValid;

                // Тест провайдера
                //Lib.UProvider prv = new Lib.UProvider("ODBCprv", "cnstr");
                //List<string> str= Lib.UProvider.ListProviderName();

                // Тест универсального сценария
                //Lib.UScenariy scn = new Lib.UScenariy("NakopDMscn", "Тестовый сценарий", null);
                //List<string> strScenariyList = Lib.UScenariy.ListScenariyName();                

                // Тестируем список сценариев
                //Com.ScenariyFarm.List.Add(new Lib.UScenariy("NakopDMscn", "Тестовый сценарий", null));
                //foreach (Lib.UScenariy item in Com.ScenariyFarm.List) { }

                // Тестирование конфигураций лучше создавать не новые сценарии а брать их из списка существующих сценариев, Для того чтобы не получилось что в конфигурации сценарий есть а в списке доступных сценариев его нет
                //Lib.Configuration cnfItem = new Lib.Configuration() { ActionRows = true, UScn = new Lib.UScenariy("NakopDMscn", "Тестовый сценарий", null) };
                //Lib.ConfigurationList cnfItemL = new Lib.ConfigurationList("День гранённого стакана");
                //cnfItemL.Add(cnfItem);
                //Com.ConfigurationFarm.ShdConfigurations.Add(cnfItemL);
                //foreach (Lib.ConfigurationList itemL in Com.ConfigurationFarm.ShdConfigurations)
                //{
                //    foreach (Lib.Configuration item in itemL)
                //    {}
                //}

                // Тест запуска процессинга расчётов скидок
                //Com.ScenariyFarm.List.Add(new Lib.UScenariy("NakopDMscn", "Тестовый сценарий", null), true, false);
                //Lib.ConfigurationList cnfItemL = new Lib.ConfigurationList("День гранённого стакана");
                //cnfItemL.Add(new Lib.Configuration() { UScn = Com.ScenariyFarm.List[0] }, true, false);
                //Com.ConfigurationFarm.SetupCurrentSharedConfigurations(cnfItemL);
                //Com.ConfigurationFarm.CurrentCnfList.ProcessingCalculateDM();


                // Тест трансфера строк чеков от провайдера через несколько сценариев
                //Lib.ConfigurationList cnfItemL = new Lib.ConfigurationList("День гранённого стакана");
                //cnfItemL.Add(new Lib.Configuration() { UScn = new Lib.UScenariy("NakopDMscn", "Тестовый сценарий 1", null) }, true, false);
                //cnfItemL.Add(new Lib.Configuration() { UScn = new Lib.UScenariy("NakopDMscn", "Тестовый сценарий 2", null) }, true, false);
                //cnfItemL.ProcessingCalculateDM();

                // Тест получения списка пользователей
                //Com.CustomerFarm.List.ProcessingCustomers();

                // Тестирование процессинга всех объектов и подсчёт итоговой скидки
                //Lib.ConfigurationList cnfItemL = new Lib.ConfigurationList("День гранённого стакана");
                //cnfItemL.Add(new Lib.Configuration() { UScn = Com.ScenariyFarm.List[0] }, true, false);
                //Com.ConfigurationFarm.SetupCurrentSharedConfigurations(cnfItemL);
                //Com.CustomerFarm.List.ProcessingCustomers();
                //System.Threading.Thread.Sleep(6000);
                //Com.ConfigurationFarm.CurrentCnfList.ProcessClearCalcMaxDiscPerc();
                //Com.ConfigurationFarm.CurrentCnfList.ProcessingCalculateDM();
                //Com.ConfigurationFarm.CurrentCnfList.ProcessCalcTotalMaxDiscPerc();
                //int i = Com.CustomerFarm.List[0].CalkMaxDiscPerc;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (!Ishelp) Com.Log.EventSave("Упали с ошибкой: " + ex.Message, "Main", Lib.EventEn.FatalError);
            }
            finally
            {
                if (!Ishelp) Com.Log.EventSave("Завершили работу программы.", "Main", Lib.EventEn.Message);
            }

        }

        // Завершилось чтение спарвичника клинетов
        private static void List_onProcessedCustomerList(object sender, Lib.EventCustomerListAsicRez e)
        {
            if (e.ex != null)
            {
                Com.Log.EventSave("Упали с ошибкой при получении спавочника клиентов: " + e.ex.Message, "Main", Lib.EventEn.Error);
            }
            else
            {
                Com.ConfigurationFarm.CurrentCnfList.ProcessingCalculateDM();
            }
        }

        // Завершилось выкачивание чеков по текущей конфигурации
        private static void CurrentCnfList_onProcessedCalculateDM(object sender, Lib.EventConfigurationListAsicRez e)
        {
            if (e.ex != null)
            {
                Com.Log.EventSave("Упали с ошибкой при получении чеков: " + e.ex.Message, "Main", Lib.EventEn.Error);
            }
            else
            {
                Com.ConfigurationFarm.CurrentCnfList.ProcessCalcTotalMaxDiscPerc();
            }
        }

        // Закончился подсчёт итоговой скидки
        private static void CurrentCnfList_onProcessedCalculateTotal(object sender, Lib.EventConfigurationListAsicRez e)
        {
            if (e.ex != null)
            {
                Com.Log.EventSave("Упали с ошибкой при просчёте итоговой скидки: " + e.ex.Message, "Main", Lib.EventEn.Error);
            }
            else
            {
                Com.CustomerFarm.List.ProcessAployDMCalkScn(null);
            }
        }

        // Окончание примененя скидок
        private static void List_onProcessedAployDMCalkMaxDiscPerc(object sender, Lib.EventCustomerListAsicRez e)
        {
            if (e.ex != null)
            {
                Com.Log.EventSave("Упали с ошибкой при приминении скодок: " + e.ex.Message, "Main", Lib.EventEn.Error);
            }

            RunGC = false;
        }

        // Прогресс бар по выкачиванию чеков
        private static void CurrentCnfList_onProcessedCalculateDMProgressBar(object sender, Lib.EventConfigurationListProcessingProgerssBar e)
        {
            try
            {
                Com.Log.EventSave(string.Format("Просмотр чеков: {0}%", e.CurrentStep * 100 / e.CurrentStep), "Main", Lib.EventEn.Message);
            }
            catch (Exception) {}
        }

        // Прогресс бар по подсчёту итоговой скидки
        private static void CurrentCnfList_onProcessedCalculateTotalProgressBar(object sender, Lib.EventConfigurationListProcessingProgerssBar e)
        {
            try
            {
                Com.Log.EventSave(string.Format("Подсчёт итоговой скидки: {0}%", e.CurrentStep * 100 / e.CurrentStep), "Main", Lib.EventEn.Message);
            }
            catch (Exception) { }
        }

        // Прогресс бар по применению скидок
        private static void List_onProcessedAployDMCalkMaxDiscPercProgressBar(object sender, Lib.EventCustomerListProcessingProgerssBar e)
        {
            try
            {
                Com.Log.EventSave(string.Format("Применение скидки в базе: {0}%", e.CurrentStep * 100 / e.CurrentStep), "Main", Lib.EventEn.Message);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Асинхронный процесс сборщика мусора
        /// </summary>
        private static void GarbColRun()
        {
            while (RunGC)
            {
                Thread.Sleep(5000);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }
}
