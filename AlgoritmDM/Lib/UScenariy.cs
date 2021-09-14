using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Reflection;
using System.Xml;
using AlgoritmDM.Com.Scenariy.Lib;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Универсальный сценарий
    /// </summary>
    public class UScenariy :ScenariyBase.UScenaryBase, ScenariyI
    {
        /// <summary>
        /// Базовый сценарий
        /// </summary>
        private ScenariyBase ScnB;

        /// <summary>
        /// Интерфейс сценария
        /// </summary>
        private ScenariyI ScnI;

        /// <summary>
        /// Имя сценария
        /// </summary>
        public string ScenariyName
        {
            get { return (this.ScnB == null ? null : this.ScnB.ScenariyName); }
            private set { }
        }

        /// <summary>
        /// Тип  сценария
        /// </summary>
        public Type ScenariyInType
        {
            get { return (this.ScnB == null ? null : this.ScnB.ScenariyInType); }
            private set { }
        }

        /// <summary>
        /// Конструктор по созданию универсального плагина
        /// </summary>
        /// <param name="ScnFullName">Имя с которым хотим работать. (Имя класса которое подгружать)</param>
        /// <param name="ScenariyName">Имя сценария которым потом будет оперировать пользователь</param>
        /// <param name="XmlNode">Ссылка на корневой элемент сценария в настроечном файле</param>
        public UScenariy(string ScnFullName, string ScenariyName, XmlElement XmlNode)
        {
            if (ScnFullName == null || ScnFullName.Trim() == string.Empty) throw new ApplicationException(string.Format("Не можем создать сценарий указанного типа: ({0})", ScnFullName == null ? "" : ScnFullName.Trim()));

            // Получаем инфу о класса 1 параметр полный путь например "EducationAnyProvider.Provider.MSSQL.MsSqlProvider", 2 параметр пропускать или не пропускать ошибки сейчас пропускаем, а третий учитывать или нет регистр из первого параметра
            //, если первый параметр нужно взять из другой зборки то сначала её загружаем Assembly asm = Assembly.LoadFrom("MyApp.exe"); а потом тоже самое только первый параметр кажется будет так "Reminder.Common.PLUGIN.MonitoringSetNedost, РЕШЕНИЕ" 
            Type myType = Type.GetType("AlgoritmDM.Com.Scenariy." + ScnFullName.Trim(), false, true);

            // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подкрузить
            bool flagI = false;
            foreach (Type i in myType.GetInterfaces())
            {
                if (i.FullName == "AlgoritmDM.Com.Scenariy.Lib.ScenariyI") flagI = true;
            }
            if (!flagI) throw new ApplicationException("Класс который вы грузите не реализовывает интерфейс (ScenariyI)");

            // Проверяем что наш клас наследует ScenariyBase 
            bool flagB = false;
            foreach (MemberInfo mi in myType.GetMembers())
            {
                if (mi.DeclaringType.FullName == "AlgoritmDM.Com.Scenariy.Lib.ScenariyBase") flagB = true;
            }
            if (!flagB) throw new ApplicationException("Класс который вы грузите не наследует от класса ScenariyBase");


            // Проверяем конструктор нашего класса  
            bool flag = false;
            string nameConstructor;
            foreach (ConstructorInfo ctor in myType.GetConstructors())
            {
                nameConstructor = myType.Name;

                // получаем параметры конструктора  
                ParameterInfo[] parameters = ctor.GetParameters();

                // если в этом конструктаре 1 параметр то проверяем тип и имя параметра  
                if (parameters.Length == 2)
                {
                    if (parameters[0].ParameterType.Name == "String" && parameters[0].Name == "ScenariyName"
                        && parameters[1].ParameterType.Name == "XmlElement" && parameters[1].Name == "XmlNode") flag = true;
                }
            }
            if (!flag) throw new ApplicationException("Класс который вы грузите не имеет конструктора (string ScenariyName)");
            
            // Создаём экземпляр объекта  
            object[] targ = { ScenariyName, XmlNode};
            object obj = Activator.CreateInstance(myType, targ);
            this.ScnB = (ScenariyBase)obj;
            this.ScnI = (ScenariyI)obj;

            base.UScenarySetupForScenaryBase(this.ScnB, this);
        }

        /// <summary>
        /// Метод для записи информации в лог
        /// </summary>
        /// <param name="Message">Сообщение</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        public void EventSave(string Message, string Source, EventEn evn)
        {
            this.ScnB.EventSave(Message, Source, evn);
        }

        /// <summary>
        /// Получаем дату в формате YYYYMMDD до которой залицензирован этот сценарий
        /// </summary>
        /// <returns>До какой даты данный модуль залицензирован, если 0 то лицензия просрочена</returns>
        public int ValidLicData()
        {
            return ScnB.ValidLicData();
        }

        /// <summary>
        /// Получение объекта реализованного как плагин
        /// </summary>
        /// <returns></returns>
        public ScenariyBase getScenariyPlugIn()
        {
            return this.ScnB;
        }

        /// <summary>
        /// Получаем список доступных сценариев
        /// </summary>
        /// <returns>Список имён доступных провайдеров</returns>
        public static List<string> ListScenariyName()
        {
            List<string> ScenariyName = new List<string>();

            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "AlgoritmDM.Com.Scenariy").ToArray();


            foreach (Type item in typelist)
            {
                // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подкрузить
                bool flagI = false;
                foreach (Type i in item.GetInterfaces())
                {
                    if (i.FullName == "AlgoritmDM.Com.Scenariy.Lib.ScenariyI") flagI = true;
                }
                if (!flagI) continue;

                // Проверяем что наш клас наследует ScenariyBase 
                bool flagB = false;
                foreach (MemberInfo mi in item.GetMembers())
                {
                    if (mi.DeclaringType.FullName == "AlgoritmDM.Com.Scenariy.Lib.ScenariyBase") flagB = true;
                }
                if (!flagB) continue;


                // Проверяем конструктор нашего класса  
                bool flag = false;
                string nameConstructor;
                foreach (ConstructorInfo ctor in item.GetConstructors())
                {
                    nameConstructor = item.Name;

                    // получаем параметры конструктора  
                    ParameterInfo[] parameters = ctor.GetParameters();

                    // если в этом конструктаре 1 параметр то проверяем тип и имя параметра  
                    if (parameters.Length == 2)
                    {
                        if (parameters[0].ParameterType.Name == "String" && parameters[0].Name == "ScenariyName"
                            && parameters[1].ParameterType.Name == "XmlElement" && parameters[1].Name == "XmlNode") flag = true;
                    }
                }
                if (!flag) continue;

                ScenariyName.Add(item.Name);
            }

            return ScenariyName;
        }

        /// <summary>
        /// Проверка наличия данного типа сценариев
        /// </summary>
        /// <param name="ScnFullName"></param>
        /// <returns></returns>
        public static bool HashScnFullName(string ScnFullName)
        {
            bool rez = false;
            foreach (string item in ListScenariyName())
            {
                if (item == ScnFullName)
                {
                    rez = true;
                    break;
                }
            }
            return rez;
        }

        /// <summary>
        /// Сохранение изменений
        /// </summary>
        public virtual void Save()
        {
            this.ScnB.Save();
        }

        /// <summary>
        /// Класс для выкачивания или закачивания денных из провайдера
        /// </summary>
        public sealed class Transfer
        {
            private UScenariy Uscn;
            private ConfigurationList CnfL;
            private int NextScenary;
            private DateTime? FirstDate;

            /// <summary>
            /// Получаем интерфейс для работы с данными
            /// </summary>
            public ScenariyTransferI PcnTI { get { return (ScenariyTransferI)this.Uscn.ScnB; } private set { } }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="Uprv">Универсальный сценарий с которым мы потом будем работать</param>
            /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
            /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
            /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
            public Transfer(UScenariy Uscn, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate)
            {
                this.Uscn = Uscn;
                this.CnfL = CnfL;
                this.NextScenary = NextScenary;
                this.FirstDate = FirstDate;
            }
        }

    }
}
