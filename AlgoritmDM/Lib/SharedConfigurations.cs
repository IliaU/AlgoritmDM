using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Список конфигураций которые будет создавать польщзователь
    /// </summary>
    public sealed class SharedConfigurations : ConfigurationBase.ConfigurationBaseList.SharedConfigurationBaseList
    {
        private static SharedConfigurations MyObj;

        /// <summary>
        /// Событие возникновения добавления конфигурации
        /// </summary>
        public event EventHandler<EventSharedConfigurations> onConfigurationsLstAddingSharedConfigurationsLst;
        /// <summary>
        /// Событие добавления конфигурации
        /// </summary>
        public event EventHandler<EventConfigurationList> onConfigurationsLstAddedConfigurationsLst;
        /// <summary>
        /// Событие возникновения удаления конфигурации
        /// </summary>
        public event EventHandler<EventSharedConfigurations> onConfigurationsLstListDeletingConfigurationsLst;
        /// <summary>
        /// Событие удаления конфигурации
        /// </summary>
        public event EventHandler<EventConfigurationList> onConfigurationsLstListDeletedConfigurationsLst;
        /// <summary>
        /// Событие возникновения изменения данных конфигурации
        /// </summary>
        public event EventHandler<EventSharedConfigurations> onConfigurationsLstListUpdatingConfigurationsLst;
        /// <summary>
        /// Событие изменения данных конфигурации
        /// </summary>
        public event EventHandler<EventConfigurationList> onConfigurationsLstListUpdatedConfigurationsLst;


        /// <summary>
        /// Получение экземпляра нашего синглетон списка пользователей
        /// </summary>
        /// <returns></returns>
        public static SharedConfigurations GetInstatnce()
        {
            if (MyObj == null) MyObj = new SharedConfigurations();
            return MyObj;
        }

        /// <summary>
        /// внутренний конструктор нашего листа
        /// </summary>
        private SharedConfigurations()
        {
            
        }

        /// <summary>
        /// Вытаскивает конфигурацию по её индексу в эом контернере
        /// </summary>
        /// <param name="i">Индекс</param>
        /// <returns>Конфигурация</returns>
        public ConfigurationList this[int i]
        {
            get 
            {
                ConfigurationList rez = null;
                lock (MyObj)
                {
                    rez = ((ConfigurationList)base.getConfigurationListComponent(i));
                }
                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает конфигурацию по её имени в эом контернере
        /// </summary>
        /// <param name="i">ConfigurationName</param>
        /// <returns>Конфигурация</returns>
        public ConfigurationList this[string s]
        {
            get 
            {
                ConfigurationList rez = null;
                lock (MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (base.getConfigurationListComponent(i).ConfigurationName == s)
                        {
                            rez = ((ConfigurationList)base.getConfigurationListComponent(i));
                            break;
                        }
                    }
                }
                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает конфигурацию по её имени
        /// </summary>
        /// <param name="s">Имя</param>
        /// <returns>Универсальный провайдер</returns>
        public ConfigurationList GetConfigurationList(string s)
        {
            try
            {
                ConfigurationList rez = null;
                lock (MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (((ConfigurationList)base.getConfigurationListComponent(i)).ConfigurationName == s)
                        {
                            rez = ((ConfigurationList)base.getConfigurationListComponent(i));
                            break;
                        }
                    }
                }
                if (rez == null) throw new ApplicationException(string.Format("Конфигурация с именем {0} в системе не существует.", s));
                else return rez;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Добавление новой конфигурации
        /// </summary>
        /// <param name="newCnfL">Конфигурация которую нужно добавить в общий список</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <param name="HashExeption">Записывать сообщения в лог или нет</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Add(ConfigurationList newCnfL, bool HashExeption, bool WriteLog)
        {
            EventSharedConfigurations MyArg = new EventSharedConfigurations((ConfigurationList)newCnfL);
            if (onConfigurationsLstAddingSharedConfigurationsLst != null) onConfigurationsLstAddingSharedConfigurationsLst.Invoke(this, MyArg);

            // Если мы одобрили добавление
            if (MyArg.Action)
            {
                bool rez = false;
                try
                {
                    lock (MyObj)
                    {
                        rez = base.Add(newCnfL, HashExeption);
                    }

                    // Если добавление конфигурации прошло успешно.
                    if (rez)
                    {
                        if (onConfigurationsLstAddedConfigurationsLst != null) onConfigurationsLstAddedConfigurationsLst.Invoke(this, new EventConfigurationList(MyArg.CfgL, null));
                        if (WriteLog) Com.Log.EventSave(string.Format("Добавилась новая конфигурация с именем {0} в общий список доступных конфигураций.", newCnfL.ConfigurationName), GetType().Name + ".Add", EventEn.Message);
                    }
                    else if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при добавлении новой конфигурации с именем {0} в общий список доступных конфигураций", newCnfL.ConfigurationName), GetType().Name + ".Add", EventEn.Error);
                }
                catch (Exception ex)
                {
                    if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при добавлении новой конфигурации с именем {0} в общий список доступных конфигураций ({1})",  newCnfL.ConfigurationName, ex.Message), GetType().Name + ".Add", EventEn.Error);
                    throw;
                }
                return rez;
            }
            return false;
        }
        /// <summary>
        /// Добавление новой конфигурации
        /// </summary>
        /// <param name="newCnfL">Конфигурация которую нужно добавить в общий список</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Add(ConfigurationList newCnfL, bool HashExeption)
        {
            return Add(newCnfL, true, true);
        }
        /// <summary>
        /// Добавление новой конфигурации
        /// </summary>
        /// <param name="newCnfL">Конфигурация которую нужно добавить в общий список</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Add(ConfigurationList newCnfL)
        {
            return Add(newCnfL, true);
        }

        /// <summary>
        /// Удаление конфигурации
        /// </summary>
        /// <param name="delCnfL">Конфигурация котороую нужно удалить из списка</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <param name="WriteLog">Записывать сообщения в лог или нет</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Remove(ConfigurationList delCnfL, bool HashExeption, bool WriteLog)
        {
            EventSharedConfigurations MyArg = new EventSharedConfigurations((ConfigurationList)delCnfL);
            if (onConfigurationsLstListDeletingConfigurationsLst != null) onConfigurationsLstListDeletingConfigurationsLst.Invoke(this, MyArg);

            // Если мы одобрили удаление
            if (MyArg.Action)
            {
                bool rez = false;
                try
                {
                    lock (MyObj)
                    {
                        rez = base.Remove(delCnfL, HashExeption);
                    }

                    // Если удаление пользователя прошло успешно.
                    if (rez)
                    {
                        if (onConfigurationsLstListDeletedConfigurationsLst != null) onConfigurationsLstListDeletedConfigurationsLst.Invoke(this, new EventConfigurationList(MyArg.CfgL, null));
                        if (WriteLog) Com.Log.EventSave(string.Format("Удалили конфигурацию с именем {0} из общего списока доступных конфигураций", delCnfL.ConfigurationName), GetType().Name + ".Remove", EventEn.Message);
                    }
                    else if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при удалении конфигурации с именем {0} из общего списока доступных конфигураций", delCnfL.ConfigurationName), GetType().Name + ".Remove", EventEn.Error);
                }
                catch (Exception ex)
                {
                    if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при удалении конфигурации с именем {0} из общего списока доступных конфигураций ({1})", delCnfL.ConfigurationName, ex.Message), GetType().Name + ".Remove", EventEn.Error);
                    throw;
                }
                return rez;
            }
            return false;
        }
        /// <summary>
        /// Удаление конфигурации
        /// </summary>
        /// <param name="delCnfL">Конфигурация котороую нужно удалить из списка</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Remove(ConfigurationList delCnfL, bool HashExeption)
        {
            return Remove(delCnfL, true, true);
        }
        /// <summary>
        /// Удаление конфигурации
        /// </summary>
        /// <param name="delCnfL">Конфигурация котороую нужно удалить из списка</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Remove(ConfigurationList delCnfL)
        {
            return Remove(delCnfL, true);
        }

        /// <summary>
        /// Обновление конфигурации в общем списке. Ключём удаления является имя конфигурации
        /// </summary>
        /// <param name="updCnfL">Конфигурация которую нужно обновить</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <param name="WriteLog">Записывать сообщения в лог или нет</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Update(ConfigurationList updCnfL, bool HashExeption, bool WriteLog)
        {
            EventSharedConfigurations MyArg = new EventSharedConfigurations((ConfigurationList)updCnfL);
            if (onConfigurationsLstListUpdatingConfigurationsLst != null) onConfigurationsLstListUpdatingConfigurationsLst.Invoke(this, MyArg);

            // Если мы одобрили обновлени
            if (MyArg.Action)
            {
                bool rez = false;
                try
                {
                    lock (MyObj)
                    {
                        rez = base.Update(updCnfL, HashExeption);
                    }

                    // Если обновление данных пользователя прошло успешно.
                    if (rez)
                    {
                        if (onConfigurationsLstListUpdatedConfigurationsLst != null) onConfigurationsLstListUpdatedConfigurationsLst.Invoke(this, new EventConfigurationList(MyArg.CfgL, null));
                        if (WriteLog) Com.Log.EventSave(string.Format("Обновление конфигурацию с именем {0} из общего списока доступных конфигураций", updCnfL.ConfigurationName), GetType().Name + ".Update", EventEn.Message);
                    }
                    else if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при обновлении конфигурации с именем {0} из общего списока доступных конфигураций", updCnfL.ConfigurationName), GetType().Name + ".Update", EventEn.Error);
                }
                catch (Exception ex)
                {
                    if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при обновлении конфигурации с именем {0} из общего списока доступных конфигураций ({1})", updCnfL.ConfigurationName, ex.Message), GetType().Name + ".Update", EventEn.Error);
                    throw;
                }
                return rez;
            }
            return false;
        }
        /// <summary>
        /// Обновление конфигурации в общем списке. Ключём удаления является имя конфигурации
        /// </summary>
        /// <param name="updCnfL">Конфигурация которую нужно обновить</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Update(ConfigurationList updCnfL, bool HashExeption)
        {
            return Update(updCnfL, true, true);
        }
        /// <summary>
        /// Обновление конфигурации в общем списке. Ключём удаления является имя конфигурации
        /// </summary>
        /// <param name="updCnfL">Конфигурация которую нужно обновить</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Update(ConfigurationList updCnfL)
        {
            return Update(updCnfL, true);
        }

    }
}
