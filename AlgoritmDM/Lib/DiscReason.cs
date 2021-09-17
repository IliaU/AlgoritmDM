using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Причина скидки
    /// </summary>
    public class DiscReason : DiscReasonBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="DiscReasonId">Идентификатор причины скидки</param>
        /// <param name="DiscReasonName">Строковое иписания причины скидки</param>
        public DiscReason(Int64 DiscReasonId, string DiscReasonName)
        {
            base.InitBaseObject(DiscReasonId, DiscReasonName);
        }
    }
}
