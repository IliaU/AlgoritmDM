using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Список причин скидок
    /// </summary>
    public class DiscReasonList:DiscReasonBase.DiscReasonListBase
    {
        private static DiscReasonList MyObj;


        /// <summary>
        /// Ссылка на асинхронный процесс   
        /// </summary>
        private Thread thr;

        /// <summary>
        /// Событие возникновения начала получения списка клиентов
        /// </summary>
        public event EventHandler<EventDiscReasonList> onProcessingDiscReasonList;
        /// <summary>
        /// Событие окончания получения списка клиентов
        /// </summary>
        public event EventHandler<EventDiscReasonListAsicRez> onProcessedDiscReasonList;

        /// <summary>
        /// Вытаскивает причину скидки по Index
        /// </summary>
        /// <param name="index">Index клиента</param>
        /// <returns>Причина скидки</returns>
        public DiscReason this[int index]
        {
            get { return ((DiscReason)base.getCustComponent(index)); }
            private set { }
        }

        /// <summary>
        /// Вытаскивает причину скидки по DiscReasonName
        /// </summary>
        /// <param name="DscReasName">Index клиента</param>
        /// <returns>Причина скидки</returns>
        public DiscReason this[string DscReasName]
        {
            get
            {
                for (int i = 0; i < base.Count; i++)
                {
                    if (base.getCustComponent(i).DiscReasonName == DscReasName) return ((DiscReason)base.getCustComponent(i));
                }
                return null;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает причину скидки по DiscReasonId
        /// </summary>
        /// <param name="DscReasId">Номер карточки</param>
        /// <returns>УПричина скидки</returns>
        public DiscReason GetDiscReason(int DscReasId)
        {
            try
            {
                for (int i = 0; i < base.Count; i++)
                {
                    if (((DiscReason)base.getCustComponent(i)).DiscReasonId == DscReasId) return ((DiscReason)base.getCustComponent(i));
                }
                throw new ApplicationException(string.Format("Причины скидки с даким DiscReasonId {0} в системе не существует.", DscReasId));
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Получение экземпляра нашего синглетон списка причин скидок
        /// </summary>
        /// <returns></returns>
        public static DiscReasonList GetInstatnce()
        {
            if (MyObj == null) MyObj = new DiscReasonList();
            return MyObj;
        }

        /// <summary>
        /// внутренний конструктор нашего листа
        /// </summary>
        private DiscReasonList()
        {

        }


        /// <summary>
        /// Запуск процесса получения списка причин скидок из базы
        /// </summary>
        public void ProcessingDiscReason()
        {
            try
            {
                Com.Log.EventSave(string.Format("Запуск процесса получения причин скидок из базы"), this.GetType().Name + ".ProcessingDiscReason", EventEn.Message);

                EventDiscReasonList myArg = new EventDiscReasonList(this);

                // Обрабатываем события начала получения клиентов
                if (onProcessingDiscReasonList != null) onProcessingDiscReasonList.Invoke(this, myArg);

                // Если поьзователь не отменил то запускаем процесс асинхронно
                if (myArg.Action)
                {
                    // Асинхронный запуск процесса
                    this.thr = new Thread(ProcessingDiscReasonRun);
                    //this.thr = new Thread(new ParameterizedThreadStart(Run)); //Запуск с параметрами   
                    this.thr.Name = "AE_Thr_ProcessingDiscReason";
                    this.thr.IsBackground = true;
                    this.thr.Start();
                }

            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessingDiscReason", EventEn.Error, true, true);
                throw;
            }
        }
        //
        /// <summary>
        /// Асинхронный запуск нашего процесса
        /// </summary>
        private void ProcessingDiscReasonRun()
        {
            ApplicationException RezEx = null;
            try
            {
                bool rez = false;
                

                // Создаём объект для доступа к интерфейсу ProviderTransferI
                UProvider.Transfer TrfSource = new UProvider.Transfer(Com.ProviderFarm.CurrentPrv, this);
                base.Clear(true);
                rez = TrfSource.PrvTI.getDiscReasons();

                Com.Log.EventSave(string.Format("Завершён процесс получения списка причин скидок из базы с результатом: {0}", rez.ToString()), this.GetType().Name + ".ProcessingDiscReasonRun", EventEn.Message);
            }
            catch (Exception ex)
            {
                RezEx = new ApplicationException(ex.Message);
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessingDiscReasonRun", EventEn.Error);
                throw;
            }
            finally
            {
                EventDiscReasonListAsicRez myArg = new EventDiscReasonListAsicRez(this, RezEx);

                // Обрабатываем события окончания процесса просмотра чеков сценариями
                if (onProcessedDiscReasonList != null) onProcessedDiscReasonList.Invoke(this, myArg);
            }
        }

    }
}
