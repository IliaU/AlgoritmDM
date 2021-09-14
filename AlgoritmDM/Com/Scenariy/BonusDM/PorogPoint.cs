using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Com.Scenariy.BonusDM
{
    /// <summary>
    /// Класс для хранения порога при котором мы должны срабатывать
    /// </summary>
    public class PorogPoint
    {
        /// <summary>
        /// Порог при котором нам нужноуказать процент бонусов
        /// </summary>
        public decimal Porog { get; private set; }

        /// <summary>
        /// Процент бонусов
        /// </summary>
        public decimal Procent { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Porog">Порог при котором нам нужно указать процент бонусов</param>
        /// <param name="Procent">Процент бонусов</param>
        public PorogPoint(decimal Porog, decimal Procent)
        {
            this.Porog = Porog;
            this.Procent = Procent;
        }
    }
}
