using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using AlgoritmDM.Com.Data.Lib;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Базовый класс для конфигурации
    /// </summary>
    public abstract class ConfigurationBase : EventArgs
    {
        /// <summary>
        /// Индекс элемента в списке
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Передавать строки чеков следующему сценарию для расчёта скидок
        /// </summary>
        public bool ActionRows = true;

        /// <summary>
        /// Универсальный сценарий который будет использоваться для просчёта скидок
        /// </summary>
        public UScenariy UScn;

        /// <summary>
        /// Какое действие применить на выходе сценария. Откуда взять скидку из предыдущего сценария или из текущего или ещё как-то.
        /// </summary>
        public CongigurationActionSaleEn SaleActionOut = CongigurationActionSaleEn.Last;

        /// <summary>
        /// Инициализация параметров базового класса
        /// </summary>
        protected void InitialConfiguration()
        {
            this.Index = -1;
        }

        /// <summary>
        /// Базовый класс для компонента списка эелементов конфигурации
        /// </summary>
        public abstract class ConfigurationBaseList : IEnumerable
        {
            /// <summary>
            /// Внутренний список 
            /// </summary>
            private List<ConfigurationBase> CfgL = new List<ConfigurationBase>();

            /// <summary>
            /// Индекс элемента в списке
            /// </summary>
            public int Index { get; private set; }

            /// <summary>
            /// Имя конфигурации
            /// </summary>
            public string ConfigurationName { get; private set; }

            /// <summary>
            /// Количчество объектов в контейнере
            /// </summary>
            public int Count 
            { 
                get 
                {
                    int rez;
                    lock (CfgL) 
                    { 
                        rez=CfgL.Count; 
                    }
                    return rez;
                } 
                private set { } 
            }

            /// <summary>
            /// Объект через который конфигурация будет иметь доступ к закрытым полям клиентов
            /// </summary>
            public CustomerBase.AccessForConfigurationList AccessCustomer;

            /// <summary>
            /// Настройка базового компонента
            /// </summary>
            /// <param name="ConfigurationName">Имя конфигурации. Например день гранённого стакана.</param>
            protected void SetupConfigurationBaseList(string ConfigurationName)
            {
                lock (CfgL)
                {
                    this.ConfigurationName = ConfigurationName;
                    this.Index = -1;
                    this.AccessCustomer = new CustomerBase.AccessForConfigurationList(this);
                }
            }

            /// <summary>
            /// Добавление нового элемента в конфигурацию
            /// </summary>
            /// <param name="newCfg">Элемент кофигурации которой нужно добавить в список</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Add(ConfigurationBase newCfg, bool HashExeption)
            {
                bool rez = false;

                try
                {
                    lock (this.CfgL)
                    {
                        // Проверка на наличие этого элемента в списке
                        foreach (ConfigurationBase item in this.CfgL)
                        {
                            if (item.UScn.ScenariyName == newCfg.UScn.ScenariyName)
                            {
                                throw new ApplicationException(string.Format("Элемент с таким именем: {0} уже существует в списке {1}", newCfg.UScn.ScenariyName, ConfigurationName));
                            }
                        }

                        newCfg.Index = CfgL.Count;
                        this.CfgL.Add(newCfg);
                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось добавить элемент конфигурации в конфигурацию {0} произошла ошибка: {1}", this.ConfigurationName, ex.Message));
                }
                return rez;
            }

            /// <summary>
            /// Удаление элемента в конфигурации
            /// </summary>
            /// <param name="delCfg">Элемент кофигурации которой нужно удалить из списка</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Remove(ConfigurationBase delCfg, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (this.CfgL)
                    {
                        int delIndex = delCfg.Index;
                        this.CfgL.RemoveAt(delIndex);

                        for (int i = delIndex; i < this.CfgL.Count; i++)
                        {
                            this.CfgL[i].Index = i;
                        }

                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось удалить элемент конфигурации в списке {0} произошла ошибка: {1}", this.ConfigurationName, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Обновление данных элемента конфигурации.
            /// </summary>
            /// <param name="IndexId">Индекс элемента который нужно обновить</param>
            /// <param name="updCfg">Пользователь у которого нужно изменить данные</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Update(int IndexId, ConfigurationBase updCfg, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (this.CfgL)
                    {

                        if (IndexId >= this.CfgL.Count)
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить данные элемента конфигурации в списке {0}. Элемента с таким индексом не сужествует.", this.ConfigurationName));
                        }
                        else
                        {
                            updCfg.Index = IndexId;
                            this.CfgL[IndexId] = updCfg;

                            rez = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить данные элемента конфигурации в списке {0} произошла ошибка: {1}", this.ConfigurationName, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Получение компонента по его ID
            /// </summary>
            /// <param name="i">Введите идентификатор</param>
            /// <returns></returns>
            protected ConfigurationBase getConfigurationComponent(int i) 
            {
                ConfigurationBase rez=null;
                lock (CfgL)
                {
                    rez = this.CfgL[i];
                }
                return rez;
            }

            /// <summary>
            /// Для обращения по индексатору
            /// </summary>
            /// <returns>Возвращаем стандарнтый индексатор</returns>
            public IEnumerator GetEnumerator()
            {
                IEnumerator rez = null;
                lock (CfgL)
                {
                    rez=this.CfgL.GetEnumerator();
                }
                return rez;
            }


            /// <summary>
            /// Базовый класс для компонента списка всех конфигураций
            /// </summary>
            public abstract class SharedConfigurationBaseList : IEnumerable
            {
                /// <summary>
                /// Внутренний список 
                /// </summary>
                private static List<ConfigurationBaseList> SharedCfgL = new List<ConfigurationBaseList>();

                /// <summary>
                /// Количчество объектов в контейнере
                /// </summary>
                public int Count 
                { 
                    get 
                    {
                        int rez;
                        lock (SharedCfgL)
                        {
                            rez = SharedCfgL.Count;
                        }
                        return rez;
                    } 
                    private set { } 
                }

                /// <summary>
                /// Добавление конфигурации в общий список
                /// </summary>
                /// <param name="newCfgL">Конфигурация котрую нужно добавить в список</param>
                /// <param name="HashExeption">C отображением исключений</param>
                /// <returns>Результат операции (Успех или нет)</returns>
                protected bool Add(ConfigurationBaseList newCfgL, bool HashExeption)
                {
                    bool rez = false;

                    try
                    {
                        lock (SharedCfgL)
                        {
                            newCfgL.Index = SharedCfgL.Count;
                            SharedCfgL.Add(newCfgL);

                            rez = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (HashExeption) throw new ApplicationException(string.Format("Не удалось добавить конфигурацию с именем {0} произошла ошибка: {1}", newCfgL.ConfigurationName, ex.Message));
                    }
                    return rez;
                }

                /// <summary>
                /// Удаление конфигурации в общем списке
                /// </summary>
                /// <param name="delCfgL">Конфигурация которую нужно удалить из списка</param>
                /// <param name="HashExeption">C отображением исключений</param>
                /// <returns>Результат операции (Успех или нет)</returns>
                protected bool Remove(ConfigurationBaseList delCfgL, bool HashExeption)
                {
                    bool rez = false;
                    try
                    {
                        lock (SharedCfgL)
                        {
                            int delIndex = delCfgL.Index;
                            SharedCfgL.RemoveAt(delIndex);

                            for (int i = delIndex; i < SharedCfgL.Count; i++)
                            {
                                SharedCfgL[i].Index = i;
                            }

                            rez = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (HashExeption) throw new ApplicationException(string.Format("Не удалось удалить конфигурацию с именем {0} произошла ошибка: {1}", delCfgL.ConfigurationName, ex.Message));
                    }

                    return rez;
                }

                /// <summary>
                /// Обновление конфигурации в общем списке.
                /// </summary>
                /// <param name="updCfgL">Конфигурация которую нужно поставить на указанный индекс</param>
                /// <param name="HashExeption">C отображением исключений</param>
                /// <returns>Результат операции (Успех или нет)</returns>
                protected bool Update(ConfigurationBaseList updCfgL, bool HashExeption)
                {
                    bool rez = false;
                    try
                    {
                        lock (SharedCfgL)
                        {
                            int ubpIndex = -1;
                            for (int i = 0; i < SharedCfgL.Count; i++)
                            {
                                if (SharedCfgL[i].ConfigurationName == updCfgL.ConfigurationName) ubpIndex = i;
                            }

                            if (ubpIndex == -1)
                            {
                                if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить конфигурацию с именем {0} в общем списке.", updCfgL.ConfigurationName));
                            }
                            else
                            {
                                updCfgL.Index = ubpIndex;
                                SharedCfgL[ubpIndex] = updCfgL;

                                rez = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить конфигурацию с именем {0} в общем списке. Произошла ошибка: {1}", updCfgL.ConfigurationName, ex.Message));
                    }

                    return rez;
                }

                /// <summary>
                /// Получение компонента по его ID
                /// </summary>
                /// <param name="i">Введите идентификатор</param>
                /// <returns></returns>
                protected ConfigurationBaseList getConfigurationListComponent(int i) 
                {
                    ConfigurationBaseList rez=null;
                    lock (SharedCfgL)
                    {
                        rez = SharedCfgL[i];
                    }
                    return rez;
                }

                /// <summary>
                /// Для обращения по индексатору
                /// </summary>
                /// <returns>Возвращаем стандарнтый индексатор</returns>
                public IEnumerator GetEnumerator()
                {
                    IEnumerator rez = null;
                    lock (SharedCfgL)
                    {
                        rez = SharedCfgL.GetEnumerator();
                    }
                    return rez;
                }
            }
        }
    }
}
