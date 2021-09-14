using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Com.Scenariy.NakopDM
{
    /// <summary>
    /// Класс для хранения порога при котором мы должны срабатывать
    /// </summary>
    public class PorogPoint
    {
        /// <summary>
        /// Порог при котором нам нужноуказать процент скидки
        /// </summary>
        public decimal Porog { get; private set; }

        /// <summary>
        /// Процент скидки
        /// </summary>
        public decimal Procent { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Porog">Порог при котором нам нужноуказать процент скидки</param>
        /// <param name="Procent">Процент скидки</param>
        public PorogPoint(decimal Porog, decimal Procent)
        {
            this.Porog = Porog;
            this.Procent = Procent;
        }
    }
}
