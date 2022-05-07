using System;
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
        /// <param name="VisibleError">Обрабатывать ошибки или нет</param>
        /// <param name="Save">Сохранить результаты проверки в текущем провайдере или нет</param>
        /// <returns>Возврощает результат проверки</returns>
        public virtual bool TestConnection(string ConnectionString, bool VisibleError, bool Save)
        {
            throw new ApplicationException("Не реализован метод TestConnection");
        }

        /// <summary>
        /// Проверка валидности подключения
        /// </summary>
        /// <param name="ConnectionString">Строка подключения которую нужно проверить</param>
        /// <param name="VisibleError">Обрабатывать ошибки или нет</param>
        /// <returns>Возврощает результат проверки</returns>
        public virtual bool TestConnection(string ConnectionString, bool VisibleError)
        {
            throw new ApplicationException("Не реализован метод TestConnection");
        }

        /// <summary>
        /// Обновление в списке пользователей значений дефолтных для подтягивания данных из второй типа призм
        /// </summary>
        public virtual void UpdateCustomerDefaultCallOffSc()
        {
            throw new ApplicationException("Не реализован метод UpdateCustomerDefaultCallOffSc");
        }
    }
}
