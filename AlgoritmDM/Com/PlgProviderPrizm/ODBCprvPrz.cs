﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using AlgoritmDM.Com.PlgProviderPrizm.Lib;
using AlgoritmDM.Lib;
using System.Data.Odbc;
using System.Data;

namespace AlgoritmDM.Com.PlgProviderPrizm
{
    /// <summary>
    /// ODBC провайдер для работы с призмом
    /// </summary>
    public sealed class ODBCprvPrz:ProviderPrizm, ProviderPrizmInterface
    {
        #region Private Param
        private string ServerVersion;
        public string DriverOdbc;
        #endregion

        #region Puplic Param
        // Билдер строки подключения
        public OdbcConnectionStringBuilder Ocsb = new OdbcConnectionStringBuilder();
        #endregion

        #region Puplic metod
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ConnectionString">Строка подключения</param>
        public ODBCprvPrz(string ConnectionString):base("ODBCprvPrz", ConnectionString)
        {
            try
            {
                /*
                //Генерим ячейку элемент меню для отображения информации по плагину
                using (ToolStripMenuItem InfoToolStripMenuItem = new ToolStripMenuItem(this.GetType().Name))
                {
                    InfoToolStripMenuItem.Text = "Провайдер для работы с базой через OLEDB";
                    InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
                    //InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                    //InfoToolStripMenuItem.Image = (Image)(new Icon(Type.GetType("Reminder.Common.PLUGIN.DwMonPlg.DwMonInfo"), "DwMon.ico").ToBitmap()); // для нормальной раьботы ресурс должен быть внедрённый
                    InfoToolStripMenuItem.Click += new EventHandler(InfoToolStripMenuItem_Click);

                    // Настраиваем компонент
                    base.InfoToolStripMenuItem = InfoToolStripMenuItem;
                }
                */

                // Тестируем подключение и получаем информациию по версии базе данных
                if (!string.IsNullOrWhiteSpace(ConnectionString))
                {
                    TestConnection(ConnectionString, false, true);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при инициализации конструктора с ошибкой: ({0})", ex.Message));
                base.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }
        //
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="PlugInType">Тип плагина</param>
        public ODBCprvPrz() :this (null)
        {
            try
            {
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при инициализации конструктора с ошибкой: ({0})", ex.Message));
                Com.Log.EventSave(ae.Message, GetType().Name, EventEn.Error, true, true);
                throw ae;
            }
        }

        /// <summary>
        /// Печать строки подключения с маскировкой секретных данных
        /// </summary>
        /// <returns>Строка подклюения с замасированной секретной информацией</returns>
        public override string PrintConnectionString()
        {
            try
            {
                if (base.ConnectionString != null && base.ConnectionString.Trim() != string.Empty)
                {
                    this.Ocsb = new OdbcConnectionStringBuilder(base.ConnectionString);
                    object P;
                    this.Ocsb.TryGetValue("Pwd", out P);
                    string Pssword = P.ToString();

                    return base.ConnectionString.Replace(Pssword, "*****");
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при выполнении метода: ({0})", ex.Message));
                base.EventSave(ae.Message, string.Format("{0}.PrintConnectionString", GetType().Name), EventEn.Error);
                throw ae;
            }

            return null;
        }

        /// <summary>
        /// Проверка валидности подключения
        /// </summary>
        /// <param name="ConnectionString">Строка подключения которую нужно проверить</param>
        /// <returns>Возврощает результат проверки</returns>
        private bool TestConnection(string ConnectionString, bool VisibleError, bool Save)
        {
            try
            {
                // Если мы работаем в режиме без базы данных то на выход
                if (Com.Config.Mode == ModeEn.NotDB) return true;

                string tmpServerVersion;
                string tmpDriver;

                // Проверка подключения
                using (OdbcConnection con = new OdbcConnection(ConnectionString))
                {
                    con.Open();
                    tmpDriver = con.Driver;
                    tmpServerVersion = con.ServerVersion; // Если не упали, значит подключение создано верно, тогда сохраняем переданные параметры
                }


                // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                switch (tmpDriver)
                {
                    //case "SQLSRV32.DLL":
                    case "SQORA32.DLL":
                    case "SQORA64.DLL":
                        // Оракловая логика
                        break;
                    case "myodbc8a.dll":
                    case "myodbc8w.dll":
                        // MySql логика
                        break;
                    default:
                        throw new ApplicationException("Извините. Мы не умеем работать с драйвером: " + tmpDriver);
                }

                // Если не упали значит можно сохранить текущую версию
                if (Save)
                {
                    this.DriverOdbc = tmpDriver;
                    this.ServerVersion = tmpServerVersion; // Сохраняем версию базы
                }

                return true;
            }
            catch (Exception ex)
            {
                // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                if (VisibleError || Com.Config.Trace) base.EventSave(ex.Message, "testConnection", EventEn.Error);

                // Отображаем ошибку если это нужно
                if (VisibleError) MessageBox.Show(ex.Message);
                return false;
            }
        }
        //
        /// <summary>
        /// Проверка валидности подключения
        /// </summary>
        /// <param name="ConnectionString">Строка подключения которую нужно проверить</param>
        /// <returns>Возврощает результат проверки</returns>
        public override bool TestConnection(string ConnectionString, bool VisibleError)
        {
            return TestConnection(ConnectionString, VisibleError, false);
        }
        #endregion

        #region Public metod for  ProviderPrizmInterface
        /// <summary>
        /// Вызов менюшки для настройки строки подключения конкретно для этого типа провайдера
        /// </summary>
        /// <returns>Что решил пользователь после вызова сохранить или не сохранить True означает сохранить</returns>
        public bool SetupProviderPrizm()
        {
            try
            {
                bool rez = false;
                using (ODBC.FSetupConnectDB Frm = new ODBC.FSetupConnectDB(this))
                {
                    DialogResult drez = Frm.ShowDialog();

                    // Если пользователь сохраняет новую строку подключения то сохраняем её в нашем объекте
                    if (drez == DialogResult.Yes)
                    {
                        base.SetupConnectionStringAndVersionDB(Frm.ConnectionString, this.ServerVersion, this.DriverOdbc);
                        rez = true;
                    }
                }
                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при выполнении метода: ({0})", ex.Message));
                base.EventSave(ae.Message, string.Format("{0}.SetupProviderPrizm", GetType().Name), EventEn.Error);
                throw ae;
            }
        }
        #endregion

        #region Puplic Custom metod
        /// <summary>
        /// Установка параметров через форму провайдера(плагина)
        /// </summary>
        /// <param name="DSN">DSN</param>
        /// <param name="Login">Логин</param>
        /// <param name="Password">Пароль</param>
        /// <param name="VisibleError">Выкидывать сообщения при неудачных попытках подключения</param>
        /// <param name="WriteLog">Писать в лог информацию о проверке побключения или нет</param>
        /// <param name="InstalConnect">Установить текущую строку подключения в билдере или нет</param>
        public string InstalProvider(string DSN, string Login, string Password, bool VisibleError, bool WriteLog, bool InstalConnect)
        {
            OdbcConnectionStringBuilder OcsbTmp = new OdbcConnectionStringBuilder();
            OcsbTmp.Dsn = DSN;
            OcsbTmp.Add("Uid", Login);
            OcsbTmp.Add("Pwd", Password);

            try
            {
                if (TestConnection(OcsbTmp.ConnectionString, VisibleError, true))
                {
                    if (InstalConnect) this.Ocsb = OcsbTmp;
                    return OcsbTmp.ConnectionString;
                }
                else return null;
            }
            catch (Exception)
            {
                if (WriteLog) base.EventSave("Не удалось создать подключение: " + DSN, this.ToString(), EventEn.Error);
                throw;
            }
        }
        #endregion


        #region Private metod



        #endregion

        #region Private metod MySql
        #endregion
    }
}
