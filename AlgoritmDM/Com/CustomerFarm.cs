using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Com.Data.Lib;

namespace AlgoritmDM.Com
{
    /// <summary>
    /// Управлнеия клиентами
    /// </summary>
    public static class CustomerFarm
    {
        /// <summary>
        /// Клиенты зарегистрированные в системе
        /// </summary>
        public static CustomerList List = CustomerList.GetInstatnce();
    }
}
