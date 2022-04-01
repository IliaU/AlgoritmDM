using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmDM.Lib;
using System.Reflection;

namespace AlgoritmDM.Lib
{
    public class EventProviderPrizmFarm : EventArgs
    {
        /// <summary>
        /// UProvider
        /// </summary>
        public Com.ProviderPrizm PrvPrizm { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="PrvPrizm">Провайдер для призма (ProviderPrizm)</param>
        public EventProviderPrizmFarm(Com.ProviderPrizm PrvPrizm)
        {
            this.PrvPrizm = PrvPrizm;
        }
    }
}
