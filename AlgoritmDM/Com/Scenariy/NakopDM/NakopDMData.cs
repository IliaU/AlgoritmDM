using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Com.Scenariy.Lib;

namespace AlgoritmDM.Com.Scenariy.NakopDM 
{
    public class NakopDMData : ScenariyDataBase
    {
        /// <summary>
        /// Всего куплено
        /// </summary>
        public decimal TotalBuy;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ScnB">Ссылка на сценарий которому принадлежит этот элемент</param>
        public NakopDMData(ScenariyBase ScnB)
        {
            base.SetupBase(ScnB);
        }
    }
}
