using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Класс реализующий конфигурацию
    /// </summary>
    public sealed class Configuration: ConfigurationBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ConfigurationName">Имя конфигурации</param>
        public Configuration()
        {
            // Инициируем базовый класс
            base.InitialConfiguration();
        }
        //
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ActionRows">Передавать строки чеков следующему сценарию для расчёта скидок</param>
        /// <param name="UScn">Универсальный сценарий который будет использоваться для просчёта скидок</param>
        /// <param name="SaleActionOut">Какое действие применить на выходе сценария. Откуда взять скидку из предыдущего сценария или из текущего или ещё как-то.</param>
        public Configuration(bool ActionRows, UScenariy UScn, CongigurationActionSaleEn SaleActionOut) :this()
        {
            base.ActionRows = ActionRows;
            base.UScn = UScn;
            base.SaleActionOut = SaleActionOut;
        }
    }
}
