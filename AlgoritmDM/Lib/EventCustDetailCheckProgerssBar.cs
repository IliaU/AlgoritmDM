using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventCustDetailCheckProgerssBar : EventArgs
    {
        /// <summary>
        /// Объект представляющий результат
        /// </summary>
        public UProvider.CustDetailCheck CstDetailChk { get; private set; }

        /// <summary>
        /// Всего шагов
        /// </summary>
        public int AllStep { get; private set; }

        /// <summary>
        ///  Текущий шаг
        /// </summary>
        public int CurrentStep { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CfgL">Объект представляющий результат</param>
        public EventCustDetailCheckProgerssBar(UProvider.CustDetailCheck CstDetailChk, int AllStep, int CurrentStep)
        {
            this.CstDetailChk = CstDetailChk;
            this.AllStep = AllStep;
            this.CurrentStep = CurrentStep;
        }
    }
}
