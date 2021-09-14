using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Lib;

namespace AlgoritmDM.Com
{
    /// <summary>
    /// Управление сценариями и конфигурациями сценариев
    /// </summary>
    public static class ScenariyFarm
    {
        /// <summary>
        /// Сценарии зарегистрированные в сисиетме
        /// </summary>
        public static ScenariyList List = ScenariyList.GetInstatnce();

        /// <summary>
        /// Получаем список доступных сценариев
        /// </summary>
        /// <returns>Список имён доступных провайдеров</returns>
        public static List<string> ListScenariyName()
        {
            return UScenariy.ListScenariyName();
        }
    }
}
