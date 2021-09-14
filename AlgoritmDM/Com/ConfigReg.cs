using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32;
using System.Windows.Forms;
using AlgoritmDM.Lib;

namespace AlgoritmDM.Com
{
    /// <summary>
    /// Класс для работы с реестром
    /// </summary>
    public partial class ConfigReg
    {
        #region Private Param
        /// <summary>
        /// Корневая ветка для хронения данных привязанных к пользователю
        /// </summary>
        private static string RootFoldCurUsr = "SOFTWARE";

        /// <summary>
        /// Корневая папка  для хранения информации
        /// </summary>
        private static string ProgramnName = ".AlgoritmDM";

        /// <summary>
        /// Объект для реализации блокировки в многопоточном режиме
        /// </summary>
        private static object MylockReg;

        /// <summary>
        /// Режим работы из реестра в контексте текущего пользователя
        /// </summary>
        private static ModeEn _Mode = ModeEn.Normal;

        /// <summary>
        /// Имя магазина
        /// </summary>
        private static string _ShopName;

        #endregion

        #region Public Param
        /// <summary>
        /// Режим работы
        /// </summary>
        public static ModeEn Mode
        {
            get
            {
                return _Mode;
            }
            private set { }
        }

        /// <summary>
        /// Имя магазина
        /// </summary>
        public static string ShopName
        {
            get
            {
                return _ShopName;
            }
            set 
            {
                SetupShopName(value);
            }
        }
        #endregion

        #region Public metod
        #endregion

        #region Private metod
        /// <summary>
        /// Конструктор для работы с реестром
        /// </summary>
        public ConfigReg()
        {
            try
            {
                // Если это первая загрузка класса то инициируем его
                if (MylockReg == null)
                {
                    MylockReg = this;

                    lock (MylockReg)
                    {
                        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RootFoldCurUsr, true))
                        {
                            key.CreateSubKey(ProgramnName);
                        }
                    }

                    LoadReg();
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при чтении конфигурации с ошибкой: " + ex.Message);
                Com.Log Log = new Com.Log("AlgoritmDM.txt");
                Log.EventSave(ae.Message, MylockReg.GetType().Name + ".ConfigReg()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Чтение реестра
        /// </summary>
        private static void LoadReg()
        {
            try
            {
                lock (MylockReg)
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(string.Format(@"{0}\{1}", RootFoldCurUsr, ProgramnName), true))
                    {
                        foreach (string item in key.GetValueNames())
                        {
                            if (item == "Mode") _Mode = EventConverter.Convert(key.GetValue(item).ToString(), _Mode);
                            if (item == "ShopName") _ShopName = key.GetValue(item).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при чтении конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, MylockReg.GetType().Name + ".LoadReg()", EventEn.Error);
                throw ae;
            }


        }

        /// <summary>
        /// Установка нового имени магазина
        /// </summary>
        /// <param name="ShopName">Новое имя магазина</param>
        private static void SetupShopName(string ShopName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ShopName))
                {
                    MessageBox.Show("Не указано имя папки, будет использоваться корневая папка.");

                    lock (MylockReg)
                    {
                        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(string.Format(@"{0}\{1}", RootFoldCurUsr, ProgramnName), true))
                        {
                            foreach (string item in key.GetValueNames())
                            {
                                if (item == "ShopName") key.DeleteValue("ShopName");
                                _ShopName = null;
                            }
                        }
                    }
                }
                else
                {
                    lock (MylockReg)
                    {
                        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(string.Format(@"{0}\{1}", RootFoldCurUsr, ProgramnName), true))
                        {
                            key.SetValue("ShopName", ShopName);
                            _ShopName = ShopName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при сохранении нового имени магазина: " + ex.Message);
                Log.EventSave(ae.Message, MylockReg.GetType().Name + ".SetupShopName()", EventEn.Error);
                throw ae;
            }
        }

        #endregion
    }
}
