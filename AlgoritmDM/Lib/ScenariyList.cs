using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using AlgoritmDM.Com.Scenariy.Lib;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Список сценариев достуаных в системе
    /// </summary>
    public sealed class ScenariyList : ScenariyBase.ScenaryBaseList
    {
        private static ScenariyList MyObj;

        /// <summary>
        /// Событие возникновения добавления сценария
        /// </summary>
        public event EventHandler<EventScenariy> onScenariyListAddingScenariy;
        /// <summary>
        /// Событие добавления сценария
        /// </summary>
        public event EventHandler<ScenariyBase> onScenariyListAddedScenariy;
        /// <summary>
        /// Событие возникновения удаления сценария
        /// </summary>
        public event EventHandler<EventScenariy> onScenariyListDeletingScenariy;
        /// <summary>
        /// Событие удаления сценария
        /// </summary>
        public event EventHandler<ScenariyBase> onScenariyListDeletedScenariy;
        /// <summary>
        /// Событие возникновения изменения данных сценария
        /// </summary>
        public event EventHandler<EventScenariy> onScenariyListUpdatingScenariy;
        /// <summary>
        /// Событие изменения данных сценария
        /// </summary>
        public event EventHandler<ScenariyBase> onScenariyListUpdatedScenariy;

        /// <summary>
        /// Получение экземпляра нашего синглетон списка сценариев
        /// </summary>
        /// <returns></returns>
        public static ScenariyList GetInstatnce()
        {
            if (MyObj == null) MyObj = new ScenariyList();
            return MyObj;
        }

        /// <summary>
        /// Внутренний конструктор нашего листа
        /// </summary>
        private ScenariyList()
        {
            
        }

        /// <summary>
        /// Вытаскивает сценарий по его индексу в эом контернере
        /// </summary>
        /// <param name="i">Индекс</param>
        /// <returns>Универсальный сценарий</returns>
        public UScenariy this[int i]
        {
            get 
            {
                UScenariy rez;
                lock (MyObj)
                {
                    rez=base.getScenaryComponent(i).UScenariy;
                }
                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает сценарий по его имени в эом контернере
        /// </summary>
        /// <param name="s">ScenariyName</param>
        /// <returns>Универсальный сценарий</returns>
        public UScenariy this[string s]
        {
            get 
            {
                UScenariy rez=null;
                lock (MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (base.getScenaryComponent(i).ScenariyName == s) return base.getScenaryComponent(i).UScenariy;
                    }
                }

                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает сценарий по его имени
        /// </summary>
        /// <<param name="s">Имя</param>
        /// <returns>Универсальный сценарий</returns>
        public UScenariy GetUScenariy (string s)
        {
            try
            {
                UScenariy rez = null;
                lock (MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (base.getScenaryComponent(i).ScenariyName == s) return base.getScenaryComponent(i).UScenariy;
                    }
                }
                if(rez==null) throw new ApplicationException(string.Format("Сценария с именем {0} в системе не существует.", s));
                return rez;
	        }
	        catch (Exception){throw;}
        }

        /// <summary>
        /// Добавление нового сценария
        /// </summary>
        /// <param name="newScenariy">Сценарий который нужно добавить в список</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <param name="HashExeption">Записывать сообщения в лог или нет</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Add(UScenariy newScenariy, bool HashExeption, bool WriteLog)
        {
            EventScenariy MyArg = new EventScenariy(newScenariy);
            if (onScenariyListAddingScenariy != null) onScenariyListAddingScenariy.Invoke(this, MyArg);

            // Если мы одобрили добавление
            if (MyArg.Action)
            {
                bool rez = false;
                try
                {
                    lock (MyObj)
                    {
                        rez = base.Add(newScenariy.getScenariyPlugIn(), HashExeption);
                    }

                    // Если добавление сценария прошло успешно.
                    if (rez)
                    {
                        if (onScenariyListAddedScenariy != null) onScenariyListAddedScenariy.Invoke(this, newScenariy.getScenariyPlugIn());
                        if (WriteLog) Com.Log.EventSave(string.Format("Добавился новый сценарий: ScenariyName {0}, ScenariyInType {1}", newScenariy.ScenariyName, newScenariy.ScenariyInType.Name), GetType().Name + ".Add", EventEn.Message);
                    }
                    else if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при добавлении нового сценария: ScenariyName {0}, ScenariyInType {1}", newScenariy.ScenariyName, newScenariy.ScenariyInType.Name), GetType().Name + ".Add", EventEn.Error);
                }
                catch (Exception ex)
                {
                    if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при добавлении нового сценария: ScenariyName {0}, ScenariyInType {1} ({2})", newScenariy.ScenariyName, newScenariy.ScenariyInType.Name, ex.Message), GetType().Name + ".Add", EventEn.Error);
                    throw;
                }
                return rez;
            }
            return false;
        }
        /// <summary>
        /// Добавление нового сценария
        /// </summary>
        /// <param name="newScenariy">Сценарий который нужно добавить в список</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Add(UScenariy newScenariy, bool HashExeption)
        {
            return Add(newScenariy, true, true);
        }
        /// <summary>
        /// Добавление нового сценария
        /// </summary>
        /// <param name="newScenariy">Сценарий который нужно добавить в список</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Add(UScenariy newScenariy)
        {
            return Add(newScenariy, true);
        }

        /// <summary>
        /// Удаление сценария
        /// </summary>
        /// <param name="delScenariy">Сценарий которого нужно удалить из списка</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <param name="WriteLog">Записывать сообщения в лог или нет</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Remove(UScenariy delScenariy, bool HashExeption, bool WriteLog)
        {
            EventScenariy MyArg = new EventScenariy(delScenariy);
            if (onScenariyListDeletingScenariy != null) onScenariyListDeletingScenariy.Invoke(this, MyArg);

            // Если мы одобрили удаление
            if (MyArg.Action)
            {
                bool rez = false;
                try
                {
                    lock (MyObj)
                    {
                        rez = base.Remove(delScenariy.getScenariyPlugIn(), HashExeption);
                    }

                    // Если удаление пользователя прошло успешно.
                    if (rez)
                    {
                        if (onScenariyListDeletedScenariy != null) onScenariyListDeletedScenariy.Invoke(this, delScenariy.getScenariyPlugIn());
                        if (WriteLog) Com.Log.EventSave(string.Format("Удалили сценарий: ScenariyName {0}, ScenariyInType {1}", delScenariy.ScenariyName, delScenariy.ScenariyInType.Name), GetType().Name + ".Remove", EventEn.Message);
                    }
                    else if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при удалении сценария: ScenariyName {0}, ScenariyInType {1}", delScenariy.ScenariyName, delScenariy.ScenariyInType.Name), GetType().Name + ".Remove", EventEn.Error);
                }
                catch (Exception ex)
                {
                    if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при удалении сценария: ScenariyName {0}, ScenariyInType {1} ({2})", delScenariy.ScenariyName, delScenariy.ScenariyInType.Name, ex.Message), GetType().Name + ".Remove", EventEn.Error);
                    throw;
                }
                return rez;
            }
            return false;
        }
        /// <summary>
        /// Удаление сценария
        /// </summary>
        /// <param name="delScenariy">Сценарий которого нужно удалить из списка</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Remove(UScenariy delScenariy, bool HashExeption)
        {
            return Remove(delScenariy, true, true);
        }
        /// <summary>
        /// Удаление сценария
        /// </summary>
        /// <param name="delScenariy">Сценарий которого нужно удалить из списка</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Remove(UScenariy delScenariy)
        {
            return Remove(delScenariy, true);
        }

        /// <summary>
        /// Обновление сценария
        /// </summary>
        /// <param name="updScenariy">Сценарий у которого нужно обновить данные в списке</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <param name="WriteLog">Записывать сообщения в лог или нет</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Update(UScenariy updScenariy, bool HashExeption, bool WriteLog)
        {
            EventScenariy MyArg = new EventScenariy(updScenariy);
            if (onScenariyListUpdatingScenariy != null) onScenariyListUpdatingScenariy.Invoke(this, MyArg);

            // Если мы одобрили обновлени
            if (MyArg.Action)
            {
                bool rez = false;
                try
                {
                    lock (MyObj)
                    {
                        rez = base.Update(updScenariy.getScenariyPlugIn(), HashExeption);
                    }

                    // Если обновление данных пользователя прошло успешно.
                    if (rez)
                    {
                        if (onScenariyListUpdatedScenariy != null) onScenariyListUpdatedScenariy.Invoke(this, updScenariy.getScenariyPlugIn());
                        if (WriteLog) Com.Log.EventSave(string.Format("Обновление данных сценария: ScenariyName {0}, ScenariyInType {1}", updScenariy.ScenariyName, updScenariy.ScenariyInType), GetType().Name + ".Update", EventEn.Message);
                    }
                    else if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при обновлении данных сценария: ScenariyName {0}, ScenariyInType {1}", updScenariy.ScenariyName, updScenariy.ScenariyInType), GetType().Name + ".Update", EventEn.Error);
                }
                catch (Exception ex)
                {
                    if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при обновлении данных сценария: ScenariyName {0}, ScenariyInType {1} ({2})", updScenariy.ScenariyName, updScenariy.ScenariyInType, ex.Message), GetType().Name + ".Update", EventEn.Error);
                    throw;
                }
                return rez;
            }
            return false;
        }
        /// <summary>
        /// Обновление сценария
        /// </summary>
        /// <param name="updScenariy">Сценарий у которого нужно обновить данные в списке</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Update(UScenariy updScenariy, bool HashExeption)
        {
            return Update(updScenariy, true, true);
        }
        /// <summary>
        /// Обновление сценария
        /// </summary>
        /// <param name="updScenariy">Сценарий у которого нужно обновить данные в списке</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Update(UScenariy updScenariy)
        {
            return Update(updScenariy, true);
        }

    }
}
