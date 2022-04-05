﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmDM.Com.PlgProviderPrizm.Lib;
using AlgoritmDM.Lib;

namespace AlgoritmDM.Com
{
    /// <summary>
    /// Основной класс который будем использовать для работы с Prizm
    /// </summary>
    public class ProviderPrizm:ProviderPrizmBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="PlugInType">Тип плагина</param>
        /// <param name="ConnectionString">Строка подключения</param>
        public ProviderPrizm(string PlugInType, string ConnectionString):base(PlugInType, ConnectionString)
        {
            try
            {
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при инициализации конструктора с ошибкой: ({0})", ex.Message));
                Com.Log.EventSave(ae.Message, GetType().Name, EventEn.Error, true, true);
                throw ae;
            }
        }

        /// <summary>
        /// Печать строки подключения с маскировкой секретных данных
        /// </summary>
        /// <returns>Строка подклюения с замасированной секретной информацией</returns>
        public virtual string PrintConnectionString()
        {
            throw new ApplicationException("Не реализован метод PrintConnectionString");
        }

        /// <summary>
        /// Проверка валидности подключения
        /// </summary>
        /// <param name="ConnectionString">Строка подключения которую нужно проверить</param>
        /// <returns>Возврощает результат проверки</returns>
        public virtual bool TestConnection(string ConnectionString, bool VisibleError)
        {
            throw new ApplicationException("Не реализован метод TestConnection");
        }

        /// <summary>
        /// Выкачивание чеков
        /// </summary>
        /// <param name="FilCustSid">Фильтр для получения данных по конкретному клиенту. null значит пользователь выгребает по всем клиентам</param>
        /// <returns>Список чеков из второго источника</returns>
        public virtual List<Com.Data.Check> GetCheck(long? FilCustSid)
        {
            throw new ApplicationException("Не реализован метод GetCheck");
        }
    }
}
