using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Lib;

namespace AlgoritmDM.Com
{
    /// <summary>
    /// Управление справочником причин скидок
    /// </summary>
    public static class DiscReasonFarm
    {
        /// <summary>
        /// Причины скидок зарегистрированные в системе
        /// </summary>
        public static DiscReasonList List = DiscReasonList.GetInstatnce();
    }
}
