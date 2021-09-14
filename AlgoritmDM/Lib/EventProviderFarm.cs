using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventProviderFarm : EventArgs
    {
        /// <summary>
        /// UProvider
        /// </summary>
        public UProvider Uprv { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Uprv">Универсальный провайдер UProvider</param>
        public EventProviderFarm(UProvider Uprv)
        {
            this.Uprv = Uprv;
        }
    }
}
