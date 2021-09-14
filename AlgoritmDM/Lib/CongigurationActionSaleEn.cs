using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Действия сценария по отношению к предыдущему занчению скидки. Определяет кукую скитку применять пришетшую на вход сценария или другую
    /// </summary>
    public enum CongigurationActionSaleEn
    {
        First, Last, Max, Min, Avg
    }
}
