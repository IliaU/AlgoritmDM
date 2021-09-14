using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Com.Data.Lib;

namespace AlgoritmDM.Lib
{
    public class EventConfigurationList : EventArgs
    {
        /// <summary>
        /// Конфигурация
        /// </summary>
        public ConfigurationList CfgL { get; private set; }

        /// <summary>
        /// Обрабатывать или нет
        /// </summary>
        public Boolean Action = true;

        /// <summary>
        /// Фильтр по клинету
        /// </summary>
        public CustomerBase Cst { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CfgL">Конфигурация</param>
        /// <param name="Cst">Фильтр по клинету по которому идёт работа</param>
        public EventConfigurationList(ConfigurationList CfgL, CustomerBase Cst)
        {
            this.CfgL = CfgL;
            this.Cst = Cst;
        }
    }
}
