using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32;

namespace AlgoritmDM.Lib.Reg
{
    /// <summary>
    /// Корневой класс реестра
    /// </summary>
    public class RootReg: Reg
    {
        /// <summary>
        /// Объект для блокировки в многопоточном режиме
        /// </summary>
        private object _myLock = null;

        /// <summary>
        /// Ссылка на объект реестра с которым мы будем работать
        /// </summary>
        private RegistryKey _WinReg;

        /// <summary>
        /// Объект для блокировки в многопоточном режиме
        /// </summary>
        protected override object myLock()
        {
            return this._myLock;
        }

        /// <summary>
        /// Корневой объект реестра
        /// </summary>
        protected override RegistryKey WinReg()
        {
 	         return this._WinReg;
        }

        /// <summary>
        /// Полный путь к объекту
        /// </summary>
        /// <returns>Полный путь к объекту</returns>
        protected override string FullPath()
        {
            return "Software";
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="WinReg">Корневой элемент реестра</param>
        /// <param name="RegName">Имя корневого элемента</param>
        public RootReg(RegistryKey WinReg, string RegName)
        {
            if (WinReg == null) throw new ApplicationException("Корневой элемент реестра не указан.");
            else this._WinReg = WinReg;

            if (string.IsNullOrWhiteSpace(RegName)) throw new ApplicationException("Имя корневого элемента не указано.");
            else base._RegName = RegName;

            this._myLock = new object();

            base.Save();
        }
    }
}
