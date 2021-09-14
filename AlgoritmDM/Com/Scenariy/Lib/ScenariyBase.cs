using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Collections;
using System.Xml;
using AlgoritmDM.Com.Data;
using AlgoritmDM.Com.Data.Lib;
using AlgoritmDM.Lib;


namespace AlgoritmDM.Com.Scenariy.Lib
{
    /// <summary>
    /// Базовый класс для реализации сценариев
    /// </summary>
    public abstract class ScenariyBase : EventArgs, ScenariyTransferI
    {
        /// <summary>
        /// Получаем элемент меню для получения информации с описанием работы по этому сценарию
        /// </summary>
        public ToolStripMenuItem InfoToolStripMenuItem { get; private set; }

        /// <summary>
        /// Контекстное меню для правки этого типа сценария
        /// </summary>
        public ToolStripMenuItem TSMItemGonfig { get; private set; }

        /// <summary>
        /// Индекс элемента в списке конфигурации
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Тип сценария
        /// </summary>
        public Type ScenariyInType { get; private set; }

        /// <summary>
        /// Имя сценария
        /// </summary>
        public string ScenariyName { get; private set; }

        /// <summary>
        /// Ссылка на универсальный сценарий
        /// </summary>
        public UScenariy UScenariy { get; private set; }

        /// <summary>
        /// XML элемент из файла конфигурации
        /// </summary>
        protected XmlElement XmlNode;

        /// <summary>
        /// Объект для хранения личных данных сценария в контексте любого клиента
        /// </summary>
        protected CustomerBase.AccessForScenary AccessScnDataForCustomer { get; private set; }

        /// <summary>
        /// Элемент через который наследуемые классы будут иметь доступ к свой ветке в конфигурационном файле
        /// </summary>
        protected Com.Config.AceessForDoc AceessForDocXML;

        /// <summary>
        /// Установка галавного компонента который должны
        /// </summary>
        /// <param name="t">Тип сценария</param>
        /// <param name="ScenariyName">Имя сценария с которым мы потом будем работать</param>
        /// <param name="InfoToolStripMenuItem">"Контекстное меню для получения информации с описанием работы по этому сценарию</param>
        /// <param name="TSMItemGonfig">Контекстное меню для правки этого типа сценария</param>
        /// <param name="XmlNode">XML элемент из файла конфигурации</param>
        protected void SetupScenaryBase(Type t, string ScenariyName, ToolStripMenuItem InfoToolStripMenuItem, ToolStripMenuItem TSMItemGonfig, XmlElement XmlNode)
        {
            this.ScenariyInType = t;
            this.ScenariyName = ScenariyName;
            this.InfoToolStripMenuItem=InfoToolStripMenuItem;
            this.TSMItemGonfig = TSMItemGonfig;
            this.XmlNode = XmlNode;

            this.AceessForDocXML = new Config.AceessForDoc();
            this.AccessScnDataForCustomer = new CustomerBase.AccessForScenary(this);
        }

        /// <summary>
        /// Сохранение изменений
        /// </summary>
        public virtual void Save()
        {
            if (this.XmlNode == null) this.EventSave(string.Format("Нет ссылки на корневой элемент сценария.\r\nСохранить в конфигурационный файл не удалось.\r\n\r\nСначала добавьте сценарий в список сценариев приложения, поcле этого этот метод станет доступным."), GetType().Name + ".Save", EventEn.Error, true, true); 
        }

        /// <summary>
        /// Метод для записи информации в лог
        /// </summary>
        /// <param name="Message">Сообщение</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        /// <param name="isLog">Писать в лог или нет</param>
        /// <param name="Show">Отобразить сообщение пользователю или нет</param>
        public void EventSave(string Message, string Source, EventEn evn, bool isLog, bool Show)
        {
            Log.EventSave(Message, Source + " (" + this.ScenariyInType.Name + " - " + this.ScenariyName + ")", evn);
        }
        //
        /// <summary>
        /// Метод для записи информации в лог
        /// </summary>
        /// <param name="Message">Сообщение</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        public void EventSave(string Message, string Source, EventEn evn)
        {
            EventSave(Message,Source,evn, true, false);
        }

        /// <summary>
        /// Получаем дату в формате YYYYMMDD до которой залицензирован этот сценарий
        /// </summary>
        /// <returns>До какой даты данный модуль залицензирован, если 0 то лицензия просрочена</returns>
        public int ValidLicData()
        {
            int rez=0;

            try
            {
                // Пробегаем по всем лицензиям
                foreach (Com.LicLib.onLicEventKey kitem in Com.Lic.LicKey)
                {
                    foreach (string mitem in kitem.ScnFullNameList)
                    {
                        if (this.ScenariyInType.Name == mitem.Trim())
                        {
                            if (rez < kitem.ValidToYYYYMMDD) rez = kitem.ValidToYYYYMMDD;
                        }
                    }
                }
            }
            catch (Exception){ }

            return rez;
        }

        /// <summary>
        /// Прокачивание чека по цепочке сценариев
        /// </summary>
        /// <param name="FuncTarget">Функция которой передать строчку из чека</param>
        /// <param name="Chk">Чек который был на входе</param>
        /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
        /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
        /// <returns>Успех обработки функции</returns>
        public virtual bool transfCheck(Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget, Check Chk, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate)
        {
            throw new ApplicationException(string.Format("В сценарии с именем {0} ({1}) не реализовано переопределение метода: transfCheck()", this.ScenariyName, this.ScenariyInType.Name));
        }

        /// <summary>
        /// Базовый класс универсального сценария, он имеет доступ к закрытым базовым объектам сценариев
        /// </summary>
        public class UScenaryBase
        {
            /// <summary>
            /// Установка в базовом сценарии ссылки на универсальный сценарий, для того чтобы при написании нового плагина можно было вызывать функции универсального сценария через базовый клас
            /// </summary>
            /// <param name="Bscn">Сценарий в котором мы устанавливаем ссылку</param>
            /// <param name="Uscn">Универсальный сценарий на который мы указываем</param>
            protected void UScenarySetupForScenaryBase(ScenariyBase Bscn, UScenariy Uscn)
            {
                Bscn.UScenariy = Uscn;
            }
        }

        /// <summary>
        /// Класс для получения доступа к базовым объектам сценариев
        /// </summary>
        public class AccessForBaseScenary
        {
            private ScenariyBase Bscn;

            /// <summary>
            /// Конструктор для получения доступа к внутренним классам нашего сценария
            /// </summary>
            /// <param name="Bscn">Базовый класс сценария к которому нужно получить доступ</param>
            public AccessForBaseScenary(ScenariyBase Bscn)
            {
                this.Bscn = Bscn;
            }

            /// <summary>
            /// Установка ссылки на xml элемент которым может пользоваться сценарий
            /// </summary>
            /// <param name="XmlNode">Ссылка на xml элемент</param>
            public void setXmlNodeForBaseScenary(XmlElement XmlNode)
            {
                this.Bscn.XmlNode = XmlNode;
                this.Bscn.Save();                       // Запускаем созранение элементов внутри нашего xml файла
                //this.Bscn.AceessForDocXML.SaveDoc();    // Сохраняем изменения в файл
            }
        }

        /// <summary>
        /// Базовый класс для списка сценариев
        /// </summary>
        public abstract class ScenaryBaseList : IEnumerable
        {
            /// <summary>
            /// Внутренний список 
            /// </summary>
            private static List<UScenariy> ScenaryL = new List<UScenariy>();

            /// <summary>
            /// Количчество объектов в контейнере
            /// </summary>
            public int Count 
            { 
                get 
                {
                    int rez;
                    lock (ScenaryL)
                    {
                        rez=ScenaryL.Count;
                    }
                    return rez;
                } 
                private set { } 
            }

            /// <summary>
            /// Добавление нового сценария
            /// </summary>
            /// <param name="newScenariy">Сценарий которого нужно добавить в список</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Add(ScenariyBase newScenariy, bool HashExeption)
            {
                bool rez = false;

                try
                {
                    lock (ScenaryL)
                    {
                        bool flag = false;

                        foreach (UScenariy item in ScenaryL)
                        {
                            if (item.ScenariyName == newScenariy.ScenariyName) flag = true;
                        }

                        if (!flag)
                        {
                            newScenariy.Index = ScenaryL.Count;
                            ScenaryL.Add(newScenariy.UScenariy);
                            rez = true;
                        }
                        else
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Сценарий с таким именем {0} уже существует.", newScenariy.ScenariyName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось добавить сценарий с именем {0} произошла ошибка: {1}", newScenariy.ScenariyName, ex.Message));
                }
                return rez;
            }

            /// <summary>
            /// Удаление сценария
            /// </summary>
            /// <param name="delScenariy">Сценарий которого нужно удалить из списка</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Remove(ScenariyBase delScenariy, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (ScenaryL)
                    {
                        int delIndex = delScenariy.Index;
                        ScenaryL.RemoveAt(delIndex);

                        for (int i = delIndex; i < ScenaryL.Count; i++)
                        {
                            ScenaryL[i].getScenariyPlugIn().Index = i;
                        }

                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось удалить сценарий с именем {0} произошла ошибка: {1}", delScenariy.ScenariyName, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Обновление данных сценария. Ключом является ScenaryТфьу он не обнавляется
            /// </summary>
            /// <param name="updScenariy">Сценарий у которого нужно изменить данные</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Update(ScenariyBase updScenariy, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (ScenaryL)
                    {
                        int ubpIndex = -1;
                        for (int i = 0; i < ScenaryL.Count; i++)
                        {
                            if (ScenaryL[i].ScenariyName == updScenariy.ScenariyName) ubpIndex = i;
                        }

                        if (ubpIndex == -1)
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить данные сценария {0} с таким именем сценария в текущем спискене найдено.", updScenariy.ScenariyName));
                        }
                        else
                        {
                            //ScenaryL[ubpIndex].Password = updScenary.Password;
                            rez = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить данные сценария {0} произошла ошибка: {1}", updScenariy.ScenariyName, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Получение компонента по его ID
            /// </summary>
            /// <param name="i">Введите идентификатор</param>
            /// <returns></returns>
            protected ScenariyBase getScenaryComponent(int i) 
            {
                ScenariyBase rez;
                lock (ScenaryL)
                {
                    rez=ScenaryL[i].getScenariyPlugIn();
                }
                return rez;
            }

            /// <summary>
            /// Для обращения по индексатору
            /// </summary>
            /// <returns>Возвращаем стандарнтый индексатор</returns>
            public IEnumerator GetEnumerator()
            {
                IEnumerator rez;
                lock (ScenaryL)
                {
                    rez = ScenaryL.GetEnumerator();
                }
                return rez;
            }
        }
    }
}
