using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using AlgoritmDM.Com.PlgProviderPrizm.Lib;
using AlgoritmDM.Com.Data;
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
        /// <param name="VisibleError">Обрабатывать ошибки или нет</param>
        /// <param name="Save">Сохранить результаты проверки в текущем провайдере или нет</param>
        /// <returns>Возврощает результат проверки</returns>
        public override bool TestConnection(string ConnectionString, bool VisibleError, bool Save)
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
                    base.Driver = tmpDriver;
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
        /// <param name="VisibleError">Обрабатывать ошибки или нет</param>
        /// <returns>Возврощает результат проверки</returns>
        public override bool TestConnection(string ConnectionString, bool VisibleError)
        {
            return TestConnection(ConnectionString, VisibleError, false);
        }

        /// <summary>
        /// Выкачивание чеков
        /// </summary>
        /// <param name="FilCustSid">Фильтр для получения данных по конкретному клиенту. null значит пользователь выгребает по всем клиентам</param>
        /// <returns>Список чеков из второго источника</returns>
        public override List<Com.Data.Check> GetCheck(long? FilCustSid)
        {
            try
            {
                // Если мы работаем в режиме без базы то выводим тестовые записи
                if (string.IsNullOrWhiteSpace(this.ServerVersion))
                {
                    throw new ApplicationException("Нет подключение к базе данных." + this.Driver);
                }
                else
                {
                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        // case "SQORA32.DLL":
                        // case "SQORA64.DLL":
                        //     return AployDMCalkStoreCreditORA(Cst, CalkStoreCredit, CalcScPerc, OldCalkStoreCredit, OldCalcScPerc);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return GetCheckMySql(FilCustSid);
                        default:
                            throw new ApplicationException("Извините. Мы не умеем работать с драйвером: " + this.Driver);
                            //break;
                    }
                }
                //return true;
            }
            catch (Exception ex)
            {
                // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                if (Com.Config.Trace) base.EventSave(ex.Message, "GetCheck", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Заполнение справочника текущих пользователей
        /// </summary>
        /// <param name="FuncTarget">Функция котороая юудет заполнять справочник</param>
        /// <returns>Успех обработки функции</returns>
        public override List<Com.Data.Customer> GetCustumers()
        {
            try
            {
                // Если мы работаем в режиме без базы то выводим тестовые записи
                if (string.IsNullOrWhiteSpace(this.ServerVersion))
                {
                    throw new ApplicationException("Нет подключение к базе данных." + this.Driver);
                }
                else
                {
                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        // case "SQORA32.DLL":
                        // case "SQORA64.DLL":
                        //     return AployDMCalkStoreCreditORA(Cst, CalkStoreCredit, CalcScPerc, OldCalkStoreCredit, OldCalcScPerc);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return GetCustumersMySql();
                        default:
                            throw new ApplicationException("Извините. Мы не умеем работать с драйвером: " + this.Driver);
                            //break;
                    }
                }
                //return true;
            }
            catch (Exception ex)
            {
                // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                if (Com.Config.Trace) base.EventSave(ex.Message, "GetCustumers", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return null;
            }
        }
        #endregion

        #region Public metod for ProviderPrizmInterface
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

        #region Private metod MySql
        /// <summary>
        /// Выкачивание чеков
        /// </summary>
        /// <param name="FilCustSid">Фильтр для получения данных по конкретному клиенту. null значит пользователь выгребает по всем клиентам</param>
        /// <returns>Список чеков из второго источника</returns>
        private List<Com.Data.Check> GetCheckMySql(long? FilCustSid)
        {
            // Вне зависимости есть чеки или нет, для тех у кого есть бонусы которые должны примениться спустя указанные период, они должны примениться
            //this.setApplayNextStoreCgreditMySql();

            string CommandSql = @"select  i.`sid` As INVC_SID, i.`tender_type` As invc_type, i.`doc_no` As invc_no, ii.`item_pos` As Item_Pos, i.`created_datetime` As created_date, i.`post_date` As post_date, inv.alu, inv.description1, inv.DESCRIPTION2,
       inv.`item_size` As siz, ii.`qty` As QTY, i.`bt_cuid` As cust_sid, i.`STORE_NO`, r.`sid` as disc_reason_id, 
       iid.`sid` As ITEM_SID, iid.`prev_price` AS ORIG_PRICE, iid.`new_price` As PRICE, iid.`new_disc_perc` As USR_DISC_PERC
    from `rpsods`.`document` i 
     inner join `rpsods`.`document_item` ii on i.SID=ii.`doc_sid`
     inner join `rpsods`.`invn_sbs_item` inv on ii.invn_sbs_item_sid=inv.sid
     inner join `rpsods`.`document_item_disc` iid on ii.sid=iid.doc_item_sid
     inner join `rpsods`.`pref_reason` r On iid.`disc_reason`=r.`name` and r.reason_type = 10
     inner join `rpsods`.`customer` s on i.`bt_cuid`=s.`sid`
    Where i.`bt_cuid` is not null " + (FilCustSid == null ? "" : @"
        and i.`bt_cuid`=" + ((long)FilCustSid).ToString() + @"
    ") + @"Order by i.`post_date`, i.`doc_no`, ii.`item_pos`";

            // Если указана какая-то спецефическая обработка чеков
            if (!string.IsNullOrWhiteSpace(Com.Config.SpecificProcessBonus))
            {
                switch (Com.Config.SpecificProcessBonus)
                {
                    case "BonusDM":
                        CommandSql = @"select  i.`sid` As INVC_SID, i.`tender_type` As invc_type, i.`doc_no` As invc_no, ii.`item_pos` As Item_Pos, i.`created_datetime` As created_date, i.`post_date` As post_date, inv.alu, inv.description1, inv.DESCRIPTION2,
       inv.`item_size` As siz, ii.`qty` As QTY, i.`bt_cuid` As cust_sid, i.`STORE_NO`, r.`sid` as disc_reason_id, 
       iid.`sid` As ITEM_SID, iid.`prev_price` AS ORIG_PRICE, iid.`new_price` As PRICE, iid.`new_disc_perc` As USR_DISC_PERC
    from `rpsods`.`document` i 
     left join `rpsods`.`document_item` ii on i.SID=ii.`doc_sid`
     left join `rpsods`.`invn_sbs_item` inv on ii.invn_sbs_item_sid=inv.sid
     left join `rpsods`.`document_item_disc` iid on ii.sid=iid.doc_item_sid
     left join `rpsods`.`pref_reason` r On iid.`disc_reason`=r.`name` and r.reason_type = 10
     inner join `rpsods`.`customer` s on i.`bt_cuid`=s.`sid`
    Where i.`bt_cuid` is not null " + (FilCustSid == null ? "" : @"
        and i.`bt_cuid`=" + ((long)FilCustSid).ToString() + @"
    ") + @"Order by i.`post_date`, i.`doc_no`, ii.`item_pos`";
                        break;
                    default:
                        break;
                }
            }

            //CommandSql = "Select 1 invc_type, 1 invc_no, sysdate created_date, '12345' alu, 'Оп 1' description1, 'Оп 2' DESCRIPTION2, 'XXL' siz, 42 QTY, 1 cust_sid, 1 STORE_NO, 4 disc_reason_id, 12344224 ITEM_SID, 7 ORIG_PRICE, 7 PRICE, 5.45 USR_DISC_PERC From Dual";

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCheckMySql", EventEn.Dump);

                List<Com.Data.Check> rez = new List<Data.Check>();

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (OdbcDataReader dr = com.ExecuteReader())
                        {

                            if (dr.HasRows)
                            {
                                // Получаем схему таблицы
                                //DataTable tt = dr.GetSchemaTable();

                                //foreach (DataRow item in tt.Rows)
                                //{
                                //    DataColumn ncol = new DataColumn(item["ColumnName"].ToString(), Type.GetType(item["DataType"].ToString()));
                                //ncol.SetOrdinal(int.Parse(item["ColumnOrdinal"].ToString()));
                                //ncol.MaxLength = (int.Parse(item["ColumnSize"].ToString()) < 300 ? 300 : int.Parse(item["ColumnSize"].ToString()));
                                //rez.Columns.Add(ncol);
                                //}


                                DateTime? tmpFirstDate = null;

                                // пробегаем по строкам
                                while (dr.Read())
                                {

                                    try
                                    {
                                        Int64 tmpInvcSid = -1;
                                        int tmpInvcType = -1;
                                        int tmpInvcNo = -1;
                                        int tmpItemPos = -1;
                                        DateTime tmpCreatedDate = DateTime.Now;
                                        DateTime tmpPostDate = DateTime.Now;
                                        string tmpAlu = null;
                                        string tmpDescription1 = null;
                                        string tmpDescription2 = null;
                                        string tmpSiz = null;
                                        decimal tmpQty = -1;
                                        long? tmpCustSid = null;
                                        int tmpStoreNo = -1;
                                        long tmpDiscReasonId = 0;
                                        long tmpItemSid = -1;
                                        decimal tmpOrigPrice = 0;
                                        decimal tmpPrice = 0;
                                        decimal tmpUsrDiscPerc = 0;
                                        for (int i = 0; i < dr.FieldCount; i++)
                                        {
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("INVC_SID").ToUpper()) tmpInvcSid = Int64.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("invc_type").ToUpper()) tmpInvcType = int.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("invc_no").ToUpper()) tmpInvcNo = int.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("Item_Pos").ToUpper()) tmpItemPos = int.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("created_date").ToUpper()) tmpCreatedDate = dr.GetDateTime(i);
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("post_date").ToUpper()) tmpPostDate = dr.GetDateTime(i);
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("alu").ToUpper()) tmpAlu = dr.GetValue(i).ToString();
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("description1").ToUpper()) tmpDescription1 = dr.GetValue(i).ToString();
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION2").ToUpper()) tmpDescription2 = dr.GetValue(i).ToString();
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("siz").ToUpper()) tmpSiz = dr.GetValue(i).ToString();
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("QTY").ToUpper()) tmpQty = decimal.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = long.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("STORE_NO").ToUpper()) tmpStoreNo = int.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("disc_reason_id").ToUpper()) tmpDiscReasonId = long.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ITEM_SID").ToUpper()) tmpItemSid = long.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ORIG_PRICE").ToUpper()) tmpOrigPrice = decimal.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("PRICE").ToUpper()) tmpPrice = decimal.Parse(dr.GetValue(i).ToString().Replace(".", Com.Config.TekDelitel).Replace(",", Com.Config.TekDelitel));
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("USR_DISC_PERC").ToUpper()) tmpUsrDiscPerc = decimal.Parse(dr.GetValue(i).ToString().Replace(".", Com.Config.TekDelitel).Replace(",", Com.Config.TekDelitel));
                                        }

                                        Check nChk = new Check(EnSourceType.Prizm, tmpInvcSid, tmpInvcType, tmpInvcNo, tmpItemPos, tmpCreatedDate, tmpPostDate, tmpAlu, tmpDescription1, tmpDescription2, tmpSiz, tmpQty, tmpCustSid, tmpStoreNo, tmpDiscReasonId, tmpItemSid, tmpOrigPrice, tmpPrice, tmpUsrDiscPerc);
                                        rez.Add(nChk);
                                    }
                                    catch (Exception ex)
                                    {

                                        if (Com.Config.Trace)
                                        {
                                            string tmpMes = null;

                                            for (int i = 0; i < dr.FieldCount; i++)
                                            {
                                                if (tmpMes != null) tmpMes += Environment.NewLine;
                                                else tmpMes = "";

                                                tmpMes += dr.GetName(i) + " = " + dr.GetValue(i).ToString();
                                            }

                                            base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0} \n\r На обработке строки: \n\r{1}", ex.Message, tmpMes), GetType().Name + ".getCheckMySql", EventEn.Dump);
                                        }
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCheckMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCheckMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCheckMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCheckMySql", EventEn.Dump);
                throw;
            }
        }
        //
        /// <summary>
        /// Обновление по отложенным записям связанным с бонусом, которые ещё не начислились
        /// </summary>
        private void setApplayNextStoreCgreditMySql()
        {
            // Пробегаем по доступным сценариям
            int Delay_Period = 0;
            foreach (UScenariy item in Com.ScenariyFarm.List)
            {
                if (item.ScenariyInType.Name == "BonusDMscn")
                {
                    AlgoritmDM.Com.Scenariy.BonusDMscn bdm = (AlgoritmDM.Com.Scenariy.BonusDMscn)item.getScenariyPlugIn();
                    Delay_Period = bdm.Delay_Period;
                }
            }

            string CommandSql = string.Format(@"Select INVC_SID, CUST_SID, POST_DATE, INVC_NO, ITEM_POS, NEXT_STORE_CREDIT As NEXT_STORE_CREDIT
From `aks`.`invc_sc_down`
Where (APPLAY_NEXT_STORE_CREDIT is null or APPLAY_NEXT_STORE_CREDIT=0)
    and POST_DATE<=date(sysdate()-{0})", Delay_Period);


            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setApplayNextStoreCgreditMySql", EventEn.Dump);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    // Читаем строчки которые нужно обработать
                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (OdbcDataReader dr = com.ExecuteReader())
                        {
                            // Если есть что обрабатывать
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    Int64 tmpInvcSid = -1;
                                    Int64 tmpCustSid = -1;
                                    DateTime tmpPosDate = DateTime.Now;
                                    int tmpInvcNo = -1;
                                    int tmpItemPos = -1;
                                    string tmpNextStoreCredit = "0";

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("INVC_SID").ToUpper()) tmpInvcSid = Int64.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = Int64.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("POST_DATE").ToUpper()) tmpPosDate = dr.GetDateTime(i);
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("INVC_NO").ToUpper()) tmpInvcNo = int.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ITEM_POS").ToUpper()) tmpItemPos = int.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NEXT_STORE_CREDIT").ToUpper()) tmpNextStoreCredit = dr.GetValue(i).ToString().Replace(",", ".");
                                    }

                                    string CommandSql2 = String.Format(@"#SELECT STR_TO_DATE('2014,2,28 19,30,05', '%Y,%m,%d %H,%i,%s');
#Select *
Update `aks`.`invc_sc_down` Set 
    STORE_CREDIT = STORE_CREDIT+{4}, 
    NEXT_STORE_CREDIT = Case When NEXT_STORE_CREDIT is null then 0 else NEXT_STORE_CREDIT end,
    APPLAY_NEXT_STORE_CREDIT = Case When INVC_SID = {5} Then 1 else APPLAY_NEXT_STORE_CREDIT End 
#From `aks`.`invc_sc_down` 
Where (CUST_SID={0} and POST_DATE>STR_TO_DATE('{1}','%d.%m.%Y'))
    or (CUST_SID={0} and POST_DATE=STR_TO_DATE('{1}','%d.%m.%Y') and invc_no>{2})
    or (CUST_SID={0} and POST_DATE=STR_TO_DATE('{1}','%d.%m.%Y') and invc_no={2} and Item_Pos>={3}) ", tmpCustSid, tmpPosDate.Day.ToString().PadLeft(2, '0') + "." + tmpPosDate.Month.ToString().PadLeft(2, '0') + "." + tmpPosDate.Year.ToString(), tmpInvcNo, tmpItemPos, tmpNextStoreCredit, tmpInvcSid);

                                    // обновляем информацию по этому чеку и по всем последующим
                                    using (OdbcCommand com2 = new OdbcCommand(CommandSql2, con))
                                    {
                                        com2.CommandTimeout = 900;  // 15 минут
                                        int dr2 = com2.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка по отложенным записям связанным с бонусом, которые ещё не начислились после {1} дней. {0}", ex.Message, Delay_Period), GetType().Name + ".setApplayNextStoreCgreditMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setApplayNextStoreCgreditMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка по отложенным записям связанным с бонусом, которые ещё не начислились после {1} дней. {0}", ex.Message, Delay_Period), GetType().Name + ".setApplayNextStoreCgreditMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setApplayNextStoreCgreditMySql", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Заполнение справочника текущих пользователей
        /// </summary>
        /// <param name="FuncTarget">Функция котороая юудет заполнять справочник</param>
        /// <returns>Успех обработки функции</returns>
        public List<Com.Data.Customer> GetCustumersMySql()
        {
            string CommandSql = null;
            if (string.IsNullOrWhiteSpace(Com.Config.CustomerCountryList))      // Если код региона не определён
            {
                if (string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList)  // Если префикс телефона не определён
                    || (!string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList) && Com.Config.CustomerPrefixPhoneList.IndexOf(",") > -1)) // Или если определено несколько префиксов телефона
                {
                    CommandSql = @"select s.sid As cust_sid, s.first_name, s.last_name, s.cust_id, s.`max_disc_perc` MAX_DISC_PERC, s.`store_credit` STORE_CREDIT,
  ap.`phone_no` As phone, a.`address_1` As address1, s.`first_sale_date` As FST_SALE_DATE, s.`last_sale_date` As lst_sale_date, ae.`email_address` As EMAIL_ADDR, p.SC_PERC
from `rpsods`.`customer` s
    left join `rpsods`.`customer_address` a on s.sid=a.cust_sid and a.seq_no=1
    left join `rpsods`.`customer_phone` ap on s.sid=ap.cust_sid and ap.seq_no=1
    left join `rpsods`.`customer_email` ae on s.sid=ae.cust_sid and ae.seq_no=1
    left join `aks`.`cust_sc_param` p on s.sid=p.cust_sid";
                }
                else
                {
                    CommandSql = @"select s.sid As cust_sid, s.first_name, s.last_name, s.cust_id, s.`max_disc_perc` MAX_DISC_PERC, s.`store_credit` STORE_CREDIT,
  ap.`phone_no` As phone, a.`address_1` As address1, s.`first_sale_date` As FST_SALE_DATE, s.`last_sale_date` As lst_sale_date, ae.`email_address` As EMAIL_ADDR, p.SC_PERC
from `rpsods`.`customer` s
    left join `rpsods`.`customer_address` a on s.sid=a.cust_sid and a.seq_no=1
    left join `rpsods`.`customer_phone` ap on s.sid=ap.cust_sid and ap.seq_no=1
    left join `rpsods`.`customer_email` ae on s.sid=ae.cust_sid and ae.seq_no=1
    left join `aks`.`cust_sc_param` p on s.sid=p.cust_sid
Where ap.`phone_no` like '" + Com.Config.CustomerPrefixPhoneList.Trim() + @"%'";
                }
            }
            else    // Если код региона указан
            {
                if (string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList)  // Если префикс телефона не определён
                    || (!string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList) && Com.Config.CustomerPrefixPhoneList.IndexOf(",") > -1)) // Или если определено несколько префиксов телефона
                {
                    CommandSql = @"select s.sid As cust_sid, s.first_name, s.last_name, s.cust_id, s.`max_disc_perc` MAX_DISC_PERC, s.`store_credit` STORE_CREDIT,
  ap.`phone_no` As phone, a.`address_1` As address1, s.`first_sale_date` As FST_SALE_DATE, s.`last_sale_date` As lst_sale_date, ae.`email_address` As EMAIL_ADDR, p.SC_PERC
from `rpsods`.`customer` s
    left join `rpsods`.`customer_address` a on s.sid=a.cust_sid and a.seq_no=1
    left join `rpsods`.`customer_phone` ap on s.sid=ap.cust_sid and ap.seq_no=1
    left join `rpsods`.`customer_email` ae on s.sid=ae.cust_sid and ae.seq_no=1
    left join `aks`.`cust_sc_param` p on s.sid=p.cust_sid
Where coalesce(a.`country_sid`,0) in (" + Com.Config.CustomerCountryList + @")";
                }
                else
                {
                    CommandSql = @"select s.sid As cust_sid, s.first_name, s.last_name, s.cust_id, s.`max_disc_perc` MAX_DISC_PERC, s.`store_credit` STORE_CREDIT,
  ap.`phone_no` As phone, a.`address_1` As address1, s.`first_sale_date` As FST_SALE_DATE, s.`last_sale_date` As lst_sale_date, ae.`email_address` As EMAIL_ADDR, p.SC_PERC
from `rpsods`.`customer` s
    left join `rpsods`.`customer_address` a on s.sid=a.cust_sid and a.seq_no=1
    left join `rpsods`.`customer_phone` ap on s.sid=ap.cust_sid and ap.seq_no=1
    left join `rpsods`.`customer_email` ae on s.sid=ae.cust_sid and ae.seq_no=1
    left join `aks`.`cust_sc_param` p on s.sid=p.cust_sid
Where coalesce(a.`country_sid`,0) in (" + Com.Config.CustomerCountryList + @")
    and ap.`phone_no` like '" + Com.Config.CustomerPrefixPhoneList.Trim() + @"%'";
                }
            }


            //CommandSql = "Select 1 cust_sid, 'Илья Михайлович' first_name, 'Погодин' last_name, '100001' cust_id, 5.5 MAX_DISC_PERC, '9163253757' phone1, 'Москва...' address1, sysdate FST_SALE_DATE, sysdate lst_sale_date, 'ilia82@mail.ru' EMAIL_ADDR From Dual union Select 2 cust_sid, 'Константин' first_name, 'Чудаков' last_name, '100002' cust_id, 3.3 MAX_DISC_PERC, '91632' phone1, 'Москва...' address1, sysdate FST_SALE_DATE, sysdate lst_sale_date, null EMAIL_ADDR From Dual";

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCustumersMySql", EventEn.Dump);

                List<Com.Data.Customer> rez = new List<Customer>();

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (OdbcDataReader dr = com.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                // Получаем схему таблицы
                                //DataTable tt = dr.GetSchemaTable();

                                //foreach (DataRow item in tt.Rows)
                                //{
                                //    DataColumn ncol = new DataColumn(item["ColumnName"].ToString(), Type.GetType(item["DataType"].ToString()));
                                //ncol.SetOrdinal(int.Parse(item["ColumnOrdinal"].ToString()));
                                //ncol.MaxLength = (int.Parse(item["ColumnSize"].ToString()) < 300 ? 300 : int.Parse(item["ColumnSize"].ToString()));
                                //rez.Columns.Add(ncol);
                                //}

                                // пробегаем по строкам
                                while (dr.Read())
                                {

                                    long tmpCustSid = -1;
                                    string tmpFirstName = null;
                                    string tmpLastName = null;
                                    string tmpCustId = null;
                                    decimal tmpMaxDiscPerc = 0;
                                    decimal tmpStoreCredit = 0;
                                    decimal tmpScPerc = 0;
                                    string tmpPhone1 = null;
                                    string tmpAddress1 = null;
                                    DateTime? tmpFstSaleDate = null;
                                    DateTime? tmpLstSaleDate = null;
                                    string tmpEmailAddr = null;
                                    int Active = 0;

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = long.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("first_name").ToUpper()) tmpFirstName = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("last_name").ToUpper()) tmpLastName = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_id").ToUpper()) tmpCustId = dr.GetValue(i).ToString();
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("MAX_DISC_PERC").ToUpper()) tmpMaxDiscPerc = dr.GetDecimal(i); }
                                        catch { }
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("STORE_CREDIT").ToUpper()) tmpStoreCredit = dr.GetDecimal(i); }
                                        catch { }
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("SC_PERC").ToUpper()) tmpScPerc = dr.GetDecimal(i); }
                                        catch { }
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("phone").ToUpper()) tmpPhone1 = dr.GetString(i);
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("address1").ToUpper()) tmpAddress1 = dr.GetString(i);
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("FST_SALE_DATE").ToUpper()) tmpFstSaleDate = dr.GetDateTime(i); }
                                        catch { }
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("lst_sale_date").ToUpper()) tmpLstSaleDate = dr.GetDateTime(i); }
                                        catch { }
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("EMAIL_ADDR").ToUpper()) tmpEmailAddr = dr.GetString(i);
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ACTIVE").ToUpper()) Active = dr.GetInt32(i); }
                                        catch { }
                                    }
                                    Customer nCust = new Customer(EnSourceType.Retail, 0, tmpCustSid, tmpFirstName, tmpLastName, tmpCustId, tmpMaxDiscPerc, tmpStoreCredit, tmpScPerc, tmpPhone1, tmpAddress1, tmpFstSaleDate, tmpLstSaleDate, tmpEmailAddr, Active);

                                    // Проверяем если в конфиге определено несколько префиксов к телефону то нужно их проверить и передать только если они найдены
                                    setActiveCustomerMySql(nCust);           // Предварительно проверяем активность клиента и если он не активный, принудительно выставляем флаг активности
                                    rez.Add(nCust);
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCustumersMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetCustumersMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCustumersMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetCustumersMySql", EventEn.Dump);
                throw;
            }
        }
        //
        /// <summary>
        /// Проверка статуса клиента active если 0, то нужно выставить 1
        /// </summary>
        /// <param name="Cst">Клиент у которого нужно сделать проверку на активность и если он не активный, принудительно выставить её.</param>
        private void setActiveCustomerMySql(Customer Cst)
        {
            if (Cst.Active == 0)
            {
                string CommandSql = string.Format("Update `rpsods`.`customer` Set ACTIVE=1 Where sid={0} and ACTIVE=0", Cst.CustSid);

                try
                {
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setActiveCustomerMySql", EventEn.Dump);

                    // Закрывать конект не нужно он будет закрыт деструктором
                    using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                    {
                        con.Open();

                        using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                        {
                            com.CommandTimeout = 900;  // 15 минут
                            int dr = com.ExecuteNonQuery();
                        }
                    }
                }
                catch (OdbcException ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".setActiveCustomerMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setActiveCustomerMySql", EventEn.Dump);
                    throw;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".setActiveCustomerMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setActiveCustomerMySql", EventEn.Dump);
                    throw;
                }
            }
        }

        #endregion
    }
}
