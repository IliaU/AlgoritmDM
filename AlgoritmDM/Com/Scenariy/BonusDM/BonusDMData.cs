using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Com.Scenariy.Lib;

namespace AlgoritmDM.Com.Scenariy.BonusDM
{
    public class BonusDMData : ScenariyDataBase
    {
        /// <summary>
        /// Всего куплено
        /// </summary>
        public decimal TotalBuy;

        /// <summary>
        /// Сумма чека за которую заплотили не бонусами
        /// </summary>
        public decimal SumCurentChek;

        /// <summary>
        /// Итоговый процент для клинета
        /// </summary>
        public decimal TotalPrc;

        /// <summary>
        /// Итоговый бонус, который нужно выставить клинету
        /// </summary>
        public decimal TotalStoreCredit;

        /// <summary>
        /// Порог через который прошёл клиент
        /// </summary>
        public decimal ActivePorog;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ScnB">Ссылка на сценарий которому принадлежит этот элемент</param>
        public BonusDMData(ScenariyBase ScnB)
        {
            base.SetupBase(ScnB);
        }
    }
}
