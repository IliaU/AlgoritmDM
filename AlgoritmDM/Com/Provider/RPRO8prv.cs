using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using AlgoritmDM.Com.Data;
using AlgoritmDM.Com.Data.Lib;
using AlgoritmDM.Com.Provider.Lib;
using AlgoritmDM.Lib;
using System.Threading;

using System.IO;

namespace AlgoritmDM.Com.Provider
{
    /// <summary>
    /// Провайдер для работы с базой RPro8
    /// </summary>
    public sealed class RPRO8prv : ProviderBase, ProviderI, ProviderTransferI
    {
        #region Private Param
            private string ServerVersion;
            private object MyLock = new object();
        #endregion

        #region Puplic Param
            // Путь к базе данных
            //public string PatchDB;
        #endregion

        #region Puplic metod

            /// <summary>
            /// Контруктор
            /// </summary>
            /// <param name="ConnectionString">Строка подключения</param>
            public RPRO8prv(string ConnectionString)
            {
                try
                {
                    //Генерим ячейку элемент меню для отображения информации по плагину
                    using (ToolStripMenuItem InfoToolStripMenuItem = new ToolStripMenuItem(this.GetType().Name))
                    {
                        InfoToolStripMenuItem.Text = "Провайдер для работы с базой RPro 8";
                        InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
                        //InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                        //InfoToolStripMenuItem.Image = (Image)(new Icon(Type.GetType("Reminder.Common.PLUGIN.DwMonPlg.DwMonInfo"), "DwMon.ico").ToBitmap()); // для нормальной раьботы ресурс должен быть внедрённый
                        //                        InfoToolStripMenuItem.Click += new EventHandler(InfoToolStripMenuItem_Click);

                        // Настраиваем компонент
                        base.SetupProviderBase(this.GetType(), InfoToolStripMenuItem, ConnectionString);
                    }
                    // Тестируем подключение и получаем информациию по версии базе данных
                    if (ConnectionString != null && ConnectionString.Trim() != string.Empty)
                    {
                        if (testConnection(ConnectionString, false))
                        {
                            // Устанавливаем в базовом классе строку подключения (котрая не меняется) и версию источника, чтобы не нужно было делать проверки коннектов
                            base.SetupConnectionStringAndVersionDB(ConnectionString, this.ServerVersion);
                        }
                    }
                }
                catch (Exception ex) { base.UPovider.EventSave(ex.Message, "RPRO8prv", EventEn.Error); }
            }

            /// <summary>
            /// Печать строки подключения с маскировкой секретных данных
            /// </summary>
            /// <returns>Строка подклюения с замасированной секретной информацией</returns>
            public override string PrintConnectionString()
            {
                try
                {
                    return base.ConnectionString;
                }
                catch (Exception ex) { base.UPovider.EventSave(ex.Message, "PrintConnectionString", EventEn.Error); }

                return null;
            }

            /// <summary>
            /// Процедура вызывающая настройку подключения
            /// </summary>
            /// <returns>Возвращаем значение диалога</returns>
            public bool SetupConnectDB()
            {
                bool rez = false;

                using (RPRO8.FSetupConnectDB Frm = new RPRO8.FSetupConnectDB(this))
                {
                    DialogResult drez = Frm.ShowDialog();

                    // Если пользователь сохраняет новую строку подключения то сохраняем её в нашем объекте
                    if (drez == DialogResult.Yes)
                    {
                        base.SetupConnectionStringAndVersionDB(Frm.ConnectionString, this.ServerVersion);
                        rez = true;
                    }
                }

                return rez;
            }

            /// <summary>
            /// Установка параметров через форму провайдера(плагина)
            /// </summary>
            /// <param name="PatchDB">Путь к каталогу базы данных</param>
            /// <param name="VisibleError">Выкидывать сообщения при неудачных попытках подключения</param>
            /// <param name="WriteLog">Писать в лог информацию о проверке побключения или нет</param>
            /// <param name="InstalConnect">Установить текущую строку подключения в билдере или нет</param>
            public string InstalProvider(string PatchDB, bool VisibleError, bool WriteLog, bool InstalConnect)
            {
                try
                {
                    if (testConnection(PatchDB, VisibleError))
                    {
                        if (InstalConnect) base.SetupConnectionStringAndVersionDB(PatchDB, this.ServerVersion);
                        return PatchDB;
                    }
                    else return null;
                }
                catch (Exception)
                {
                    if (WriteLog) Log.EventSave("Не удалось создать подключение: " + PatchDB, this.ToString(), EventEn.Error);
                    throw;
                }
            }

            /// <summary>
            /// Выкачивание чеков
            /// </summary>
            /// <param name="FuncTarget">Функция которой передать строчку из чека</param>
            /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
            /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
            /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
            /// <param name="FilCustSid">Фильтр для получения данных по конкретному клиенту. null значит пользователь выгребает по всем клиентам</param>
            /// <returns>Успех обработки функции</returns>
            public override bool getCheck(Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate, long? FilCustSid)
            {
                try
                {
                    // Если мы работаем в режиме без базы то выводим тестовые записи
                    if (Com.Config.Mode == ModeEn.NotDB) return this.getCheckNotDB(FuncTarget, CnfL, NextScenary, FirstDate, FilCustSid);
                    else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) return this.getCheckNotDB(FuncTarget, CnfL, NextScenary, FirstDate, FilCustSid);
                    else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                    else
                    {
                        if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.ConnectionString);

                        return getCheckRPro(FuncTarget, CnfL, NextScenary, FirstDate, FilCustSid);
                    }
                    //return true;
                }
                catch (Exception ex)
                {
                    // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                    if (Com.Config.Trace) base.EventSave(ex.Message, "getCheck", EventEn.Error);

                    // Отображаем ошибку если это нужно
                    MessageBox.Show(ex.Message);

                    return false;
                }
            }

            /// <summary>
            /// Заполнение справочника текущих пользователей
            /// </summary>
            /// <param name="FuncTarget">Функция котороая юудет заполнять справочник</param>
            /// <returns>Успех обработки функции</returns>
            public override bool getCustumers(Func<Customer, bool> FuncTarget)
            {
                try
                {
                    // Если мы работаем в режиме без базы то выводим тестовые записи  NotDB
                    if (Com.Config.Mode == ModeEn.NotDB) return this.getCustumersNotDB(FuncTarget);
                    else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) return this.getCustumersNotDB(FuncTarget);
                    else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                    else
                    {
                        if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.ConnectionString);

                        return getCustumersRPro(FuncTarget);
                    }
                    //return true;
                }
                catch (Exception ex)
                {
                    // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                    if (Com.Config.Trace) base.EventSave(ex.Message, "getCustumers", EventEn.Error);

                    // Отображаем ошибку если это нужно
                    MessageBox.Show(ex.Message);

                    return false;
                }
            }

            /// <summary>
            /// Заполнение справочника причин скидок
            /// </summary>
            /// <returns>Успех обработки функции</returns>
            public override bool getDiscReasons()
            {
                try
                {
                    // Если мы работаем в режиме без базы то выводим тестовые записи
                    if (Com.Config.Mode == ModeEn.NotDB) return this.getDiscReasonsNotDB();
                    else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) return this.getDiscReasonsNotDB();
                    else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                    else
                    {
                        if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.ConnectionString);

                        return this.getDiscReasonsNotDB();
                    }
                    //return true;
                }
                catch (Exception ex)
                {
                    // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                    if (Com.Config.Trace) base.EventSave(ex.Message, "getDiscReasons", EventEn.Error);

                    // Отображаем ошибку если это нужно
                    MessageBox.Show(ex.Message);

                    return false;
                }
            }

            /// <summary>
            /// Установка расчитанной скидки в базе у конкртеного клиента
            /// </summary>
            /// <param name="Cst">Клиент</param>
            /// <param name="CalkMaxDiscPerc">Процент скидки который мы устанавливаем</param>
            /// <returns>Успех обработки функции</returns>
            public override bool AployDMCalkMaxDiscPerc(CustomerBase Cst, decimal CalkMaxDiscPerc)
            {
                try
                {
                    // Если мы работаем в режиме без базы то выводим тестовые записи
                    if (Com.Config.Mode == ModeEn.NotDB) return this.AployDMCalkMaxDiscPercNotDB(Cst, CalkMaxDiscPerc);
                    else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) return this.AployDMCalkMaxDiscPercNotDB(Cst, CalkMaxDiscPerc);
                    else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                    else
                    {
                        if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.ConnectionString);

                        return AployDMCalkMaxDiscRPro(Cst, CalkMaxDiscPerc);
                    }
                    //return true;
                }
                catch (Exception ex)
                {
                    // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                    if (Com.Config.Trace) base.EventSave(ex.Message, "AployDMCalkMaxDiscPerc", EventEn.Error);

                    // Отображаем ошибку если это нужно
                    MessageBox.Show(ex.Message);

                    return false;
                }
            }

            /// <summary>
            /// Установка бонусного бала в базе у конкртеного клиента
            /// </summary>
            /// <param name="Cst">Клиент</param>
            /// <param name="CalkStoreCredit">Бонусный бал который мы устанавливаем</param>
            /// <param name="CalcScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
            /// <param name="OldCalkStoreCredit">Старый бонусный бал который мы устанавливаем</param>
            /// <param name="OldCalcScPerc">Старый процент по которому считался бонусный бал который мы устанавливаем</param>
            /// <returns>Успех обработки функции</returns>
            public override bool AployDMCalkStoreCredit(CustomerBase Cst, decimal CalkStoreCredit, decimal CalcScPerc, decimal OldCalkStoreCredit, decimal OldCalcScPerc)
            {
                try
                {
                    // Если мы работаем в режиме без базы то выводим тестовые записи
                    if (Com.Config.Mode == ModeEn.NotDB) return this.AployDMCalkStoreCreditNotDB(Cst, CalkStoreCredit, CalcScPerc);
                    else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) return this.AployDMCalkStoreCreditNotDB(Cst, CalkStoreCredit, CalcScPerc);
                    else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                    else
                    {
                        if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.ConnectionString);

                        return AployDMCalkStoreCreditRPro(Cst, CalkStoreCredit, CalcScPerc);
                    }
                    //return true;
                }
                catch (Exception ex)
                {
                    // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                    if (Com.Config.Trace) base.EventSave(ex.Message, "AployDMCalkStoreCredit", EventEn.Error);

                    // Отображаем ошибку если это нужно
                    MessageBox.Show(ex.Message);

                    return false;
                }
            }

            /// <summary>
            /// Объединение клиентов
            /// </summary>
            /// <param name="MergeClientMain">Основной клиент</param>
            /// <param name="MergeClientDonors">Клинеты доноры</param>
            public void MergeClient(Customer MergeClientMain, List<Customer> MergeClientDonors)
            {
               
            }

        #endregion

        #region Private metod
            // Пользователь вызвал меню информации по провайдеру
            private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
            {
                //using (ODBC.FInfo Frm = new ODBC.FInfo(this))
                //{
                //    Frm.ShowDialog();
                //}
            }

            /// <summary>
            /// Проверка валидности подключения
            /// </summary>
            /// <param name="ConnectionString">Строка подключения которую нужно проверить</param>
            /// <returns>Возврощает результат проверки</returns>
            private bool testConnection(string ConnectionString, bool VisibleError)
            {
                try
                {
                    // Если мы работаем в режиме без базы данных то на выход
                    if (Com.Config.Mode == ModeEn.NotDB) return true;

                    string tmpServerVersion = "8";

                    /*
                    // Проверка подключения
                    using (OdbcConnection con = new OdbcConnection(ConnectionString))
                    {
                        con.Open();
                        tmpDriver = con.Driver;
                        tmpServerVersion = con.ServerVersion; // Если не упали, значит подключение создано верно, тогда сохраняем переданные параметры
                    }*/

                    this.ServerVersion = tmpServerVersion; // Сохраняем версию базы

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
        #endregion

        #region Private metod For RPro
            /// <summary>
            /// Выкачивание чеков
            /// </summary>
            /// <param name="FuncTarget">Функция которой передать строчку из чека</param>
            /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
            /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
            /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
            /// <param name="FilCustSid">Фильтр для получения данных по конкретному клиенту. null значит пользователь выгребает по всем клиентам</param>
            /// <returns>Успех обработки функции</returns>
            private bool getCheckRPro(Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate, long? FilCustSid)
            {
                //string FileName = @"ME??????.Dat";  192
                string FileName = @"SA??????.Dat";  //216

                try
                {
                    bool rez = true;

                    lock (MyLock)
                    {
                        DateTime? tmpFirstDate = null;
                        DateTime? tmpCreatedDate = null;

                        // Получаем массив байт для каждой строки
                        foreach (Byte[] item in GetRowsFromFile(FileName, 216, 1512))
                        {
                            int InvcType = (item.Length == 1512 ? 0 : 1); // 0 продажа, 1 возврат  1301
                            int tmpInvcNo = GetIntForByte4(item, 7);
                            decimal? Qty = GetDecimalForByte4Delit3(item, 1080 + (InvcType * 216) + 147);
                            string AUP = GetStringForByte(item, 1449 + (InvcType * 216));
                            int npor = GetIntForByte2(item, 1301 + (InvcType * 216)); // порядковый номер строки


                            //string Address1 = GetStringForByte(item, 108);
                            //string Phone1 = GetStringForByte(item, 215);
                            //string EmailAddr = GetStringForByte(item, 789);
                            //decimal? MaxDiscPerc = GetDecimalForByte2Delit2(item, 869);
                            //long? CustId = GetLongForByte8(item, 900);
                            //long? CustSid = GetLongForByte8(item, 908);
                            //if (CustSid == null) throw new ApplicationException(string.Format("Не смогли получить ключь клиента при парсинге из файла"));
                            tmpCreatedDate = GetDateTimeForByte8(item, 15);
                            //DateTime? LastSaleDate = GetDateTimeForByte8(item, 932);

                            if (tmpCreatedDate != null && Qty != 0)
                            {
                                //Check nChk = new Check(InvcType, tmpInvcNo, (DateTime)tmpCreatedDate, tmpAlu, tmpDescription1, tmpDescription2, tmpSiz, (decimal)Qty, tmpCustSid, tmpStoreNo, tmpDiscReasonId, tmpItemSid, tmpOrigPrice, tmpPrice, tmpUsrDiscPerc);

                                // Запоминаем первую дату для коректной работы прогресс бара
                                if (tmpFirstDate == null) tmpFirstDate = tmpCreatedDate;

                                // Передаём добытый чек обработчику
                                //if (FuncTarget != null) rez = FuncTarget(nChk, CnfL, NextScenary, tmpFirstDate);

                                // Проверяем необходимость продолжения дальнейшей работы
                                //if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи чека {0} за дату {1} с штрих кодом {2} обработчику Func<Check, ConfigurationList, int, bool>", tmpInvcNo, tmpCreatedDate, tmpAlu));
                            }
                        }
                    }
                    return rez;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCheckRPro", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(FileName, GetType().Name + ".getCheckRPro", EventEn.Dump);
                    throw;
                }
            }

            /// <summary>
            /// Заполнение справочника текущих пользователей
            /// </summary>
            /// <param name="FuncTarget">Функция котороая юудет заполнять справочник</param>
            /// <returns>Успех обработки функции</returns>
            private bool getCustumersRPro(Func<Customer, bool> FuncTarget)
            {
                //return true;
                string FileName = @"Client.Dat";

                try
                {
                    bool rez = true;

                    lock (MyLock)
                    {
                        // Получаем массив байт для каждой строки
                        foreach (Byte[] item in GetRowsFromFile(FileName, 1008))
                        {
                            string FirstName = GetStringForByte(item, 46);
                            string LastName = GetStringForByte(item, 77);
                            string Address1 = GetStringForByte(item, 108);
                            string Phone1 = GetStringForByte(item, 215);
                            string EmailAddr = GetStringForByte(item, 789);
                            decimal? MaxDiscPerc = GetDecimalForByte2Delit2(item, 869);
                            decimal? StoreCredit = null;// GetDecimalForByte2Delit2(item, 869);
                            decimal? ScPerc = null;// GetDecimalForByte2Delit2(item, 869);
                            long? CustId = GetLongForByte8(item, 900);
                            long? CustSid = GetLongForByte8(item, 908);
                            if (CustSid == null) throw new ApplicationException(string.Format("Не смогли получить ключь клиента при парсинге из файла"));
                            //DateTime? FstSaleDate = GetDateTimeForByte8(item, 932);
                            DateTime? LastSaleDate = GetDateTimeForByte8(item, 932);

                            if (CustSid != 0)
                            {
                                // Создаём тестового пользователя и передаём его обработчику. Затем проверяем результат если всё ок то не выводим ошибку
                                Customer nCust = new Customer((long)CustSid, LastName, FirstName, (CustId == null ? null : ((long)CustId).ToString()), (MaxDiscPerc == null ? 0 : (decimal)MaxDiscPerc), (StoreCredit == null ? 0 : (decimal)StoreCredit), (ScPerc == null ? 0 : (decimal)ScPerc), Phone1, Address1, null, LastSaleDate, EmailAddr, 1);
                                if (rez && FuncTarget != null) rez = FuncTarget(nCust);
                                if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи пользователя сид {0} обработчику Func<Customer, bool>", nCust.CustSid));
                            }
                        }
                    }
                    return rez;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCustumersRPro", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(FileName, GetType().Name + ".getCustumersRPro", EventEn.Dump);
                    throw;
                }
            }

            /// <summary>
            /// Установка расчитанной скидки в базе у конкртеного клиента
            /// </summary>
            /// <param name="Cst">Клиент</param>
            /// <param name="CalkMaxDiscPerc">Процент скидки который мы устанавливаем</param>
            /// <returns>Успех обработки функции</returns>
            private bool AployDMCalkMaxDiscRPro(CustomerBase Cst, decimal CalkMaxDiscPerc)
            {
                string CommandSql = string.Format("Update CMS.customer Set MAX_DISC_PERC={0} Where cust_sid={1}", CalkMaxDiscPerc.ToString().Replace(",", "."), Cst.CustSid);

                try
                {
                    bool rez = false;

                    /*
                    // Закрывать конект не нужно он будет закрыт деструктором
                    using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                    {
                        con.Open();

                        using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                        {
                            int dr = com.ExecuteNonQuery();

                            // Проверяем кол-во обновлённых строк
                            if (dr > 0)
                            {
                                rez = true;
                            }
                            else
                            {
                                throw new ApplicationException("Количество строк которое обновилось в базе менее 1.");
                            }
                        }
                    }*/

                    return rez;
                }
                //catch (OdbcException ex)
                //{
                //    base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscRPro", EventEn.Error);
                //    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkMaxDiscRPro", EventEn.Dump);
                //    throw;
                //}
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscRPro", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkMaxDiscRPro", EventEn.Dump);
                    throw;
                }
            }

            /// <summary>
            /// Установка бонусного бала в базе у конкртеного клиента
            /// </summary>
            /// <param name="Cst">Клиент</param>
            /// <param name="CalkStoreCredit">Бонусный бал который мы устанавливаем</param>
            /// <param name="CalcScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
            /// <returns>Успех обработки функции</returns>
            private bool AployDMCalkStoreCreditRPro(CustomerBase Cst, decimal CalkStoreCredit, decimal CalcScPerc)
            {
                string CommandSql = string.Format("Update CMS.customer Set Store_Credit={0} Where cust_sid={1}", CalkStoreCredit.ToString().Replace(",", "."), Cst.CustSid);

                try
                {
                    bool rez = false;

                    /*
                    // Закрывать конект не нужно он будет закрыт деструктором
                    using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                    {
                        con.Open();

                        using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                        {
                            int dr = com.ExecuteNonQuery();

                            // Проверяем кол-во обновлённых строк
                            if (dr > 0)
                            {
                                rez = true;
                            }
                            else
                            {
                                throw new ApplicationException("Количество строк которое обновилось в базе менее 1.");
                            }
                        }
                    }*/

                    return rez;
                }
                //catch (OdbcException ex)
                //{
                //    base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscRPro", EventEn.Error);
                //    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditRPro", EventEn.Dump);
                //    throw;
                //}
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscRPro", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditRPro", EventEn.Dump);
                    throw;
                }
            }

        #endregion

        #region Private metod For Mode NotDB
            /// <summary>
            /// Выкачивание чеков
            /// </summary>
            /// <param name="FuncTarget">Функция которой передать строчку из чека</param>
            /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
            /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
            /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
            /// <param name="FilCustSid">Фильтр для получения данных по конкретному клиенту. null значит пользователь выгребает по всем клиентам</param>
            /// <returns>Успех обработки функции</returns>
            private bool getCheckNotDB(Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate, long? FilCustSid)
            {
                try
                {
                    bool rez = true;
                    Check nChk = null;
                    FirstDate = DateTime.Now.AddDays(-4);

                    // Создаём тестовый чеки и передаём их обработчику. Затем проверяем результат если всё ок то не выводим ошибку
                    if (FilCustSid == null || (long)FilCustSid == 1)
                    {
                        nChk = new Check(100001, 1, 1, 1, DateTime.Now.AddDays(-4), DateTime.Now.AddDays(-4), "12345", "Описание 1", "Описаине 2", "XXL", 42, ((long)1), 1, 4, ((long)12344224), 7, 7, 0);
                        if (rez && FuncTarget != null) rez = FuncTarget(nChk, CnfL, NextScenary, FirstDate);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи чека продукт {0} обработчику Func<Check, ConfigurationList, int, bool>", nChk.ItemSid));

                        // имитация долгой работы, чтобы можно было увидеть пользователю визуально, что сейчас идёт обращение в базу данных
                        Thread.Sleep(2500);
                    }

                    if (FilCustSid == null || (long)FilCustSid == 2)
                    {
                        nChk = new Check(100001, 1, 1, 1, DateTime.Now.AddDays(-4), DateTime.Now.AddDays(-4), "12346", "Описание t 1", "Описаине  2", "XLL", 43, ((long)2), 1, 4, ((long)12344224), 8, 8, 0);
                        if (rez && FuncTarget != null) rez = FuncTarget(nChk, CnfL, NextScenary, FirstDate);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи чека продукт {0} обработчику Func<Check, ConfigurationList, int, bool>", nChk.ItemSid));

                        Thread.Sleep(2500);
                    }

                    if (FilCustSid == null || (long)FilCustSid == 2)
                    {
                        nChk = new Check(100001,1, 1, 1, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-2), "12347", "Описание t 3", "Описаине  2", "XL", 44, ((long)2), 1, 4, ((long)12344224), 9, 9, 0);
                        if (rez && FuncTarget != null) rez = FuncTarget(nChk, CnfL, NextScenary, FirstDate);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи чека продукт {0} обработчику Func<Check, ConfigurationList, int, bool>", nChk.ItemSid));

                        Thread.Sleep(2500);
                    }

                    if (FilCustSid == null)
                    {
                        nChk = new Check(100001,1, 1, 1, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1), "12347", "Описание t 3", "Описаине  2", "XL", 44, null, 1, 4, ((long)12344224), 6, 6, 0);
                        if (rez && FuncTarget != null) rez = FuncTarget(nChk, CnfL, NextScenary, FirstDate);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи чека продукт {0} обработчику Func<Check, ConfigurationList, int, bool>", nChk.ItemSid));

                        // имитация долгой работы, чтобы можно было увидеть пользователю визуально, что сейчас идёт обращение в базу данных
                        Thread.Sleep(2500);
                    }

                    // Передаём результат операции
                    return rez;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCheckNotDB", EventEn.Error);
                    throw;
                }
            }

            /// <summary>
            /// Заполнение справочника текущих пользователей
            /// </summary>
            /// <param name="FuncTarget">Функция котороая юудет заполнять справочник</param>
            /// <returns>Успех обработки функции</returns>
            private bool getCustumersNotDB(Func<Customer, bool> FuncTarget)
            {
                try
                {
                    bool rez = true;
                    Customer nCust = null;

                    // Создаём тестового пользователя и передаём его обработчику. Затем проверяем результат если всё ок то не выводим ошибку
                    nCust = new Customer(1, "Илья Михайлович", "Погодин", "100001", 5, 100, 0, "9163253757", "Москва...", DateTime.Now, null, "ilia82@mail.ru", 1);
                    if (rez && FuncTarget != null) rez = FuncTarget(nCust);
                    if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи пользователя сид {0} обработчику Func<Customer, bool>", nCust.CustSid));
                    //
                    nCust = new Customer(2, "Константин", "Чудаков", "100002", 3, 50, 0, "91632", "Москва...", null, null, null, 1);
                    if (rez && FuncTarget != null) rez = FuncTarget(nCust);
                    if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи пользователя сид {0} обработчику Func<Customer, bool>", nCust.CustSid));

                    // имитация долгой работы, чтобы можно было увидеть пользователю визуально, что сейчас идёт обращение в базу данных
                    Thread.Sleep(5000);

                    // Передаём результат операции
                    return rez;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCustumersNotDB", EventEn.Error);
                    throw;
                }
            }

            /// <summary>
            /// Заполнение справочника причин скидок
            /// </summary>
            /// <returns>Успех обработки функции</returns>
            private bool getDiscReasonsNotDB()
            {
                try
                {
                    bool rez = true;
                    DiscReason nDscReas = null;

                    lock (MyLock)
                    {
                        // Создаём тестовую причину скидки. Затем проверяем результат если всё ок то не выводим ошибку
                        nDscReas = new DiscReason(1, "Qty");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                        //
                        nDscReas = new DiscReason(2, "Pkg");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                        //
                        nDscReas = new DiscReason(3, "Cust");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                        //
                        nDscReas = new DiscReason(4, "Promo");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                        //
                        nDscReas = new DiscReason(5, "GROUP");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                        //
                        nDscReas = new DiscReason(6, "EMPLOY");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                        //
                        nDscReas = new DiscReason(7, "SALE");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                        //
                        nDscReas = new DiscReason(8, "VIP");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                        //
                        nDscReas = new DiscReason(9, "DAMAGE");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                        //
                        nDscReas = new DiscReason(10, "COUPON");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                        //
                        nDscReas = new DiscReason(11, "Lty");
                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);
                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи новой причины скидки id {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));

                    }

                    // Передаём результат операции
                    return rez;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getDiscReasonsNotDB", EventEn.Error);
                    throw;
                }
            }

            /// <summary>
            /// Установка расчитанной скидки в базе у конкртеного клиента
            /// </summary>
            /// <param name="Cst">Клиент</param>
            /// <param name="CalkMaxDiscPerc">Процент скидки который мы устанавливаем</param>
            /// <returns>Успех обработки функции</returns>
            private bool AployDMCalkMaxDiscPercNotDB(CustomerBase Cst, decimal CalkMaxDiscPerc)
            {
                try
                {
                    bool rez = true;

                    // имитация долгой работы, чтобы можно было увидеть пользователю визуально, что сейчас идёт обращение в базу данных
                    Thread.Sleep(2000);

                    // Передаём результат операции
                    return rez;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercNotDB", EventEn.Error);
                    throw;
                }
            }

            /// <summary>
            /// Установка бонусного бала в базе у конкртеного клиента
            /// </summary>
            /// <param name="Cst">Клиент</param>
            /// <param name="CalkStoreCredit">Бонусный бал который мы устанавливаем</param>
            /// <param name="CalcScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
            /// <returns>Успех обработки функции</returns>
            private bool AployDMCalkStoreCreditNotDB(CustomerBase Cst, decimal CalkStoreCredit, decimal CalcScPerc)
            {
                try
                {
                    bool rez = true;

                    // имитация долгой работы, чтобы можно было увидеть пользователю визуально, что сейчас идёт обращение в базу данных
                    Thread.Sleep(2000);

                    // Передаём результат операции
                    return rez;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkStoreCreditNotDB", EventEn.Error);
                    throw;
                }
            }

        #endregion

        #region Специальные функции парсинга
            /// <summary>
            /// Чтение строк из файла
            /// </summary>
            /// <param name="FileName">Шаблон файла</param>
            /// <param name="TopLen">Длина заголовка</param>
            /// <param name="RowLen">Длина строки</param>
            /// <returns>Массив байт</returns>
            private IEnumerable<byte[]> GetRowsFromFile(string FileName, int TopLen, int RowLen)
            {
                yield break;  // В 10 студии не компилится по этому всё что ниже казоментил и нарисовал заглушку 
                /*
                // Иногда длина строки разная в зависимотсти от данных которые там хранятся
                int tmpRowLen = RowLen;
                lock (MyLock)
                {
                    string[] fileSource;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(this.ConnectionString)) throw new ApplicationException(string.Format("Нет подключения к базе данных."));
                        if (string.IsNullOrWhiteSpace(FileName)) throw new ApplicationException(string.Format("Не указан файл откуда читать строки."));
                        fileSource = Directory.GetFiles(this.ConnectionString, FileName);

                        if (fileSource == null || fileSource.Length == 0) throw new ApplicationException(string.Format("Нет файлов отвечающих условию: {0}", FileName));
                    }
                    catch (IOException ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при чтени данных из файла {0}. {1}", FileName, ex.Message), GetType().Name + ".GetRowsFromFile", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(FileName, GetType().Name + ".GetRowsFromFile", EventEn.Dump);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при чтени данных из файла {0}. {1}", FileName, ex.Message), GetType().Name + ".GetRowsFromFile", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(FileName, GetType().Name + ".GetRowsFromFile", EventEn.Dump);
                        throw;
                    }

                    // Пробегаем по списку файлов
                    foreach (string item in fileSource)
                    {
                        // прочитали весь файл
                        byte[] fileB = File.ReadAllBytes(item);
                        bool flagfirst = true;

                        if (fileB.Length <= TopLen)
                        {
                            // Проверка заголовка
                            if (flagfirst) { CheckFirstReadPage(fileB, TopLen, true); flagfirst = false; }
                            else yield return fileB;
                        }
                        else
                        {
                            int startPos = 0;
                            // Бегаем по строкам
                            while (fileB.Length > startPos)
                            {
                                byte[] rez = null;

                                // Переменные для работы с чеками
                                int DocType = 0;  // Байт по которому мы определяем что это возврат или продажа 72 - Возврат | 122 - Продажа 
                                int rowcount = 0; // Кол-во строк одна строка 216 байт

                                // длина массива которую нужно вернуть
                                if (flagfirst)
                                {
                                    if (fileB.Length - startPos >= TopLen) rez = new byte[TopLen];
                                    else rez = new byte[fileB.Length - TopLen];
                                }
                                else
                                {
                                    // Есть файлы где длина строки может быть разной
                                    if (FileName == @"SA??????.Dat")   // Если это чеки
                                    {
                                        
                                        //  216 байт заголовок строк
                                        //  648 (216*3) кол-во строк какая то инфа по строкам
                                        //  864(216*4) что-то от заголовка при возврате 1080 (216*5)
                                        //  648 (216*3) кол-во строк какая то инфа по строкам
                                        
                                        DocType = fileB[startPos + 372];  // Байт по которому мы определяем что это возврат или продажа 72 - Возврат | 122 - Продажа 
                                        rowcount = BitConverter.ToInt32(fileB, startPos + 210); // Кол-во строк одна строка 216 байт

                                        if (DocType == 72) tmpRowLen = 216 + (rowcount * 216 * 2) + (216 * 5);  //72 - Возврат
                                        else tmpRowLen = 216 + (rowcount * 216 * 2) + (216 * 4);                //122 - Продажа 
                                    }

                                    if (fileB.Length - startPos >= tmpRowLen) rez = new byte[tmpRowLen];
                                    else rez = new byte[fileB.Length - tmpRowLen];

                                }

                                // Обработка чеков требует специального подхода
                                if (!flagfirst && FileName == @"SA??????.Dat")   // Если это чеки
                                {
                                    // Создаём промежуточный буфер чтобы возвращать построчно
                                    byte[] rezChk = null;
                                    if (DocType == 72) rezChk = new byte[216 + (216 * 2) + (216 * 5)]; //72 - Возврат
                                    else rezChk = new byte[216 + (216 * 2) + (216 * 4)]; //72 - Продажа

                                    // Парсим документ на строки
                                    for (int i = 0; i < rowcount; i++)
                                    {
                                        // Заполняем массив заголовка первые 216 байт
                                        for (int t1 = 0; t1 < 216; t1++)
                                        {
                                            rezChk[t1] = fileB[startPos + t1];
                                        }
                                        // Заполняем массив заголовка вторые 864(216*4) для продаж 1080(216*5) для возвратов байт
                                        for (int t2 = 0; t2 < (DocType == 72 ? 1080 : 864); t2++)
                                        {
                                            rezChk[216 + t2] = fileB[startPos + 216 + (rowcount * 216) + t2];
                                        }
                                        // Заполняем массив первыми данными по выбранной строке
                                        for (int r1 = 0; r1 < 216; r1++)
                                        {
                                            rezChk[216 + (DocType == 72 ? 1080 : 864) + r1] = fileB[startPos + 216 + (216 * i) + r1];
                                        }
                                        // Заполняем массив вторыми данными по выбранной строке
                                        for (int r2 = 0; r2 < 216; r2++)
                                        {
                                            int ttt = startPos + 216 + (216 * rowcount) + (DocType == 72 ? 1080 : 864) + (216 * i) + r2;
                                            rezChk[216 + (DocType == 72 ? 1080 : 864) + 216 + r2] = fileB[startPos + 216 + (216 * rowcount) + (DocType == 72 ? 1080 : 864) + (216 * i) + r2];
                                        }

                                        yield return rezChk;
                                    }
                                }
                                else
                                {
                                    // Заполняем массив
                                    for (int i = 0; i < rez.Length; i++)
                                    {
                                        rez[i] = fileB[startPos + i];
                                    }
                                }

                                // передвигаем указатель
                                if (flagfirst) startPos = startPos + TopLen;
                                else startPos = startPos + tmpRowLen;

                                // Проверка заголовка
                                if (flagfirst) { CheckFirstReadPage(rez, TopLen, true); flagfirst = false; }
                                else
                                {
                                    if (FileName != @"SA??????.Dat")   // Если это  не чеки
                                    {
                                        yield return rez;
                                    }
                                }
                            }
                        }
                    }
                    yield break;
                }
            */}
            /// <summary>
            /// Чтение строк из файла
            /// </summary>
            /// <param name="FileName">Шаблон файла</param>
            /// <param name="TopLen">Длина заголовка</param>
            /// <returns>Массив байт</returns>
            private IEnumerable<byte[]> GetRowsFromFile(string FileName, int TopLen)
            {
                // Если длина строк не указана значит она ровна заголовку
                return GetRowsFromFile(FileName, TopLen, TopLen);
            }
            /// <summary>
            /// Проверка заголовка страниц
            /// </summary>
            /// <param name="b">массив первых байт</param>
            /// <param name="RowLen">Длина строки которую нужно проверить в заголовке</param>
            /// <param name="HashException">выводить ошибки</param>
            /// <returns>Результат проверки</returns>
            private bool CheckFirstReadPage(byte[] b, int RowLen, bool HashException)
            {
                bool rez = false;
                try
                {
                    if (b == null) throw new ApplicationException("Нет массива данных в котором нужно сделать проверку заголовка.");
                    if (b.Length < 14) throw new ApplicationException("Массив коткий не сможем сделать проверку.");
                    if (b[0] != 255 || b[1] != 255 || b[2] != 255 || b[3] != 255) throw new ApplicationException("Скорее всего файл не того формата.");
                    if ((b[13] * 256) + b[12] != RowLen) throw new ApplicationException(string.Format("В заголовке файла указана длина строки {0} байт, а вы парсите по {1} байт.", (b[13] * 256) + b[12], RowLen));
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".CheckFirstReadPage", EventEn.Error);
                    if (HashException) throw;
                }
                return rez;
            }

            /// <summary>
            /// Получаем строку из байт
            /// </summary>
            /// <param name="RowB">Массив байт</param>
            /// <param name="StartPozition">Начальная позиция строки</param>
            /// <returns>Строка которая получается в текущей кодировке из массива байт</returns>
            private string GetStringForByte(byte[] RowB, int StartPozition)
            {
                string rez = null;
                try
                {
                    int len = RowB[StartPozition];
                    byte[] tmpb = new byte[len];
                    for (int i = 0; i < len; i++)
                    {
                        tmpb[i] = RowB[StartPozition + i + 1];
                    }
                    rez = Encoding.Default.GetString(tmpb);
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetStringForByte", EventEn.Error);
                    throw;
                }
                return rez;
            }

            /// <summary>
            /// Получаем long обработав 8 байт от начала которое мы указали
            /// </summary>
            /// <param name="RowB">Массив байт</param
            /// <param name="StartPozition">Начальная позиция</param>
            /// <returns></returns>
            private long? GetLongForByte8(byte[] RowB, int StartPozition)
            {
                long? rez = null;
                try
                {
                    byte[] tmpb = new byte[8];
                    for (int i = 0; i < tmpb.Length; i++)
                    {
                        tmpb[i] = RowB[StartPozition + i];
                    }

                    Int64 ii = BitConverter.ToInt64(tmpb, 0);
                    rez = (long?)ii;

                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetLongForByte8", EventEn.Error);
                    throw;
                }
                return rez;
            }

            /// <summary>
            /// Получаем число из 4 байт
            /// </summary>
            /// <param name="RowB">Массив байт</param
            /// <param name="StartPozition">Начальная позиция</param>
            /// <returns></returns>
            private int GetIntForByte4(byte[] RowB, int StartPozition)
            {
                int rez;
                try
                {
                    byte[] tmpb = new byte[4];
                    for (int i = 0; i < tmpb.Length; i++)
                    {
                        tmpb[i] = RowB[StartPozition + i];
                    }

                    rez = BitConverter.ToInt32(tmpb, 0);
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetLongForByte4", EventEn.Error);
                    throw;
                }
                return rez;
            }

            /// <summary>
            /// Получаем число из 4 байт
            /// </summary>
            /// <param name="RowB">Массив байт</param
            /// <param name="StartPozition">Начальная позиция</param>
            /// <returns></returns>
            private int GetIntForByte2(byte[] RowB, int StartPozition)
            {
                int rez;
                try
                {
                    byte[] tmpb = new byte[2];
                    for (int i = 0; i < tmpb.Length; i++)
                    {
                        tmpb[i] = RowB[StartPozition + i];
                    }

                    rez = BitConverter.ToInt16(tmpb, 0);
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetLongForByte2", EventEn.Error);
                    throw;
                }
                return rez;
            }

            /// <summary>
            /// Получить десимал из массива из 2х байт, где 2 знака в кконце фиксированы для разделителя
            /// </summary>
            /// <param name="RowB">Массив байт</param>
            /// <param name="StartPozition">Начальная позиция</param>
            /// <returns>Возвращает значение полученное из байтов</returns>
            private decimal? GetDecimalForByte2Delit2(byte[] RowB, int StartPozition)
            {
                decimal? rez = null;
                try
                {
                    byte[] tmpb = new byte[2];
                    for (int i = 0; i < tmpb.Length; i++)
                    {
                        tmpb[i] = RowB[StartPozition + i];
                    }

                    Int16 ii = BitConverter.ToInt16(tmpb, 0);
                    rez = ((decimal)ii) / 100;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetDecimalForByte2Delit2", EventEn.Error);
                    throw;
                }
                return rez;
            }

            /// <summary>
            /// Получить десимал из массива из 2х байт, где 2 знака в кконце фиксированы для разделителя
            /// </summary>
            /// <param name="RowB">Массив байт</param>
            /// <param name="StartPozition">Начальная позиция</param>
            /// <returns>Возвращает значение полученное из байтов</returns>
            private decimal? GetDecimalForByte4Delit3(byte[] RowB, int StartPozition)
            {
                decimal? rez = null;
                try
                {
                    byte[] tmpb = new byte[4];
                    for (int i = 0; i < tmpb.Length; i++)
                    {
                        tmpb[i] = RowB[StartPozition + i];
                    }

                    Int32 ii = BitConverter.ToInt32(tmpb, 0);
                    rez = ((decimal)ii) / 1000;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetDecimalForByte2Delit2", EventEn.Error);
                    throw;
                }
                return rez;
            }

            /// <summary>
            /// Получаем дату время из 8 батов
            /// </summary>
            /// <param name="RowB">Массив байт</param>
            /// <param name="StartPozition">Начальная позиция</param>
            /// <returns>Возвращает значение полученное из байтов</returns>
            private DateTime? GetDateTimeForByte8(byte[] RowB, int StartPozition)
            {
                DateTime? rez = null;
                try
                {
                    byte[] tmpb = new byte[2];
                    for (int i = 0; i < tmpb.Length; i++)
                    {
                        tmpb[i] = RowB[StartPozition + i];
                    }
                    Int16 yyyy = BitConverter.ToInt16(tmpb, 0);
                    Int16 mm = RowB[StartPozition + 2];
                    Int16 dd = RowB[StartPozition + 3];
                    Int16 h = RowB[StartPozition + 4];
                    Int16 mi = RowB[StartPozition + 5];
                    Int16 ss = RowB[StartPozition + 6];

                    if (yyyy != 0 && mm != 0 && dd != 0)
                    {
                        string tmp = string.Format("{0}.{1}.{2} {3}:{4}:{5}", dd.ToString("D2"), mm.ToString("D2"), yyyy.ToString("D4"), h.ToString(), mi.ToString("D2"), ss.ToString("D2"));
                        try
                        {
                            rez = DateTime.Parse(tmp);
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetDateTimeForByte8", EventEn.Error);
                    throw;
                }
                return rez;
            }

        #endregion
    }
}
