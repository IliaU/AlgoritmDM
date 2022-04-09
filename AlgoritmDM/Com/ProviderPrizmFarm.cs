using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmDM.Lib;
using System.Reflection;

namespace AlgoritmDM.Com
{
    /// <summary>
    /// Ферма для работы с провайдером для призма
    /// </summary>
    public sealed class ProviderPrizmFarm
    {
        /// <summary>
        /// Внутренний список доступных плагинов, чтобы каждый раз не пересчитывать
        /// </summary>
        private static List<string> ListProviderPrizmName;

        /// <summary>
        /// Текущий провайдер
        /// </summary>
        public static ProviderPrizm CurProviderPrizm = null;

        /// <summary>
        /// Событие изменения текущего провайдера Prizm
        /// </summary>
        public static event EventHandler<EventProviderPrizmFarm> onEventSetup;

        /// <summary>
        /// Конструктор
        /// </summary>
        public ProviderPrizmFarm()
        {
            try
            {
                if (ListProviderPrizmName == null)
                {
                    Log.EventSave("Загружается драйвер для даботы с Prizm", GetType().Name, EventEn.Message, true, false);

                    // Если списка документов ещё нет то создаём его
                    GetListProviderPrizmName();
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при инициализации конструктора с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error, true, true);
                throw ae;
            }
        }

        /// <summary>
        /// Получить списко доступных Local
        /// </summary>
        /// <returns>Список доступных документов</returns>
        public static List<string> GetListProviderPrizmName()
        {
            // Если список ещё не получали то получаем его
            if (ListProviderPrizmName == null)
            {
                ListProviderPrizmName = new List<string>();

                Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "AlgoritmDM.Com.PlgProviderPrizm").ToArray();

                foreach (Type item in typelist)
                {
                    // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подкрузить
                    bool flagI = false;
                    foreach (Type i in item.GetInterfaces())
                    {
                        if (i.FullName == "AlgoritmDM.Com.PlgProviderPrizm.Lib.ProviderPrizmInterface")
                        {
                            flagI = true;
                            break;
                        }
                    }
                    if (!flagI) continue;


                    // Проверяем что наш клас наследует ProviderPrizm
                    bool flagB = false;
                    if (item.BaseType.Name == "ProviderPrizm")
                    {
                        flagB = true;
                    }
                    if (!flagB) continue;

                    // Проверяем конструктор нашего класса  
                    bool flag = false;
                    bool flag0 = false;
                    string nameConstructor;
                    foreach (ConstructorInfo ctor in item.GetConstructors())
                    {
                        nameConstructor = item.Name;

                        // получаем параметры конструктора  
                        ParameterInfo[] parameters = ctor.GetParameters();

                        // если в этом конструктаре 1 параметров то проверяем тип и имя параметра 
                        if (parameters.Length == 1)
                        {
                            bool flag2 = true;
                            if (parameters[0].ParameterType.Name != "String" || parameters[0].Name != "ConnectionString") flag = false;
                            flag = flag2;
                        }

                        // Проверяем конструктор для создания документа пустого по умолчанию
                        if (parameters.Length == 0) flag0 = true;
                    }
                    if (!flag) continue;
                    if (!flag0) continue;

                    ListProviderPrizmName.Add(item.Name);
                }
            }

            return ListProviderPrizmName;
        }

        /// <summary>
        /// Создание пустого ProviderPrizm
        /// </summary>
        /// <param name="PlugInType">Имя плагина определяющего тип ProviderPrizm который создаём</param>
        /// <returns>Возвращаем ProviderPrizm</returns>
        public static ProviderPrizm CreateNewProviderPrizm(string PlugInType)
        {
            // Если списка ProviderPrizm ещё нет то создаём его
            GetListProviderPrizmName();

            ProviderPrizm rez = null;

            // Проверяем наличие существование этого типа документа
            foreach (string item in ListProviderPrizmName)
            {
                if (item.ToUpper() == PlugInType.Trim().ToUpper())
                {
                    Type myType = Type.GetType("AlgoritmDM.Com.PlgProviderPrizm." + item, false, true);

                    // Создаём экземпляр объекта
                    rez = (ProviderPrizm)Activator.CreateInstance(myType);

                    break;
                }
            }

            return rez;
        }

        /// <summary>
        /// Создание пустого ProviderPrizm
        /// </summary>
        /// <param name="PlugInType">Имя плагина определяющего тип ProviderPrizm который создаём</param>
        /// <param name="ConnectionString">Строка подключения</param>
        /// <returns>Возвращаем ProviderPrizm</returns>
        public static ProviderPrizm CreateNewProviderPrizm(string PlugInType, string ConnectionString)
        {
            // Если списка ProviderPrizm ещё нет то создаём его
            GetListProviderPrizmName();

            ProviderPrizm rez = null;

            // Если не задан тип провайдера для призма то передаём null
            if (string.IsNullOrEmpty(PlugInType)) return rez;

            // Проверяем наличие существование этого типа документа
            foreach (string item in ListProviderPrizmName)
            {
                if (item.ToUpper() == PlugInType.Trim().ToUpper())
                {
                    Type myType = Type.GetType("AlgoritmDM.Com.PlgProviderPrizm." + item, false, true);

                    // Создаём экземпляр объекта  
                    object[] targ = { ConnectionString };
                    rez = (ProviderPrizm)Activator.CreateInstance(myType, targ);

                    break;
                }
            }

            return rez;
        }

        /// <summary>
        /// Сохранение текущего провайдера
        /// </summary>
        /// <param name="PrvPrizm">Провайдер который надо установить как текущий</param>
        public static void SetupCurrentProvider(ProviderPrizm PrvPrizm)
        {
            try
            {
                // Собственно обработка события
                EventProviderPrizmFarm myArg = new EventProviderPrizmFarm(PrvPrizm);
                if (onEventSetup != null)
                {
                    onEventSetup.Invoke(PrvPrizm, myArg);
                }

                // Логируем изменение подключения
                PrvPrizm.TestConnection(PrvPrizm.ConnectionString, false);
                CurProviderPrizm = PrvPrizm;
                if (PrvPrizm!=null) Log.EventSave(string.Format("Пользователь настроил новое подключениe Prizm: {0} ({1})", PrvPrizm.PrintConnectionString(), PrvPrizm.PlugInType), "ProviderPrizmFarm.SetupCurrentProvider", EventEn.Message, true, false);
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format("Ошибка при сохранении подключекния Prizm: {0} ({1} - {2})", ex.Message, PrvPrizm.PrintConnectionString(), PrvPrizm.PlugInType), "ProviderPrizmFarm.SetupCurrentProvider", EventEn.Error, true, true);
            }
            
        }
    }
}
