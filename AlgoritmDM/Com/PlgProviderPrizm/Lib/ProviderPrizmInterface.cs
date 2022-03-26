using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmDM.Com.PlgProviderPrizm.Lib
{
    /// <summary>
    /// Стандартный интерфейс который используется из системы и обычным пользователям не доступен
    /// </summary>
    public interface ProviderPrizmInterface
    {
        /// <summary>
        /// Вызов менюшки для настройки строки подключения конкретно для этого типа провайдера
        /// </summary>
        /// <returns>Что решил пользователь после вызова сохранить или не сохранить True означает сохранить</returns>
        bool SetupProviderPrizm();
    }
}
