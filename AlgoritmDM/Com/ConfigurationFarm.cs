using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Lib;

namespace AlgoritmDM.Com
{
    /// <summary>
    /// Класс управления конфигурациями
    /// </summary>
    public static class ConfigurationFarm
    {
        /// <summary>
        /// Событие возникновения изменения текущей конфигурации
        /// </summary>
        public static event EventHandler<EventConfigurationList> onСhengingCurrentCnfList;
        /// <summary>
        /// Событие изменения текущей конфигурации
        /// </summary>
        public static event EventHandler<EventConfigurationList> onСhengedCurrentCnfList;
        
        
        /// <summary>
        /// Список всех доступных конфигураций
        /// </summary>
        public static SharedConfigurations ShdConfigurations = SharedConfigurations.GetInstatnce();

        public static ConfigurationParams ParamsOfScenatiy = ConfigurationParams.GetInstatnce();

        /// <summary>
        /// Текущая конфигурация с которой работает система
        /// </summary>
        public static ConfigurationList CurrentCnfList;

        /// <summary>
        /// Установка текущей конфигурации в которой будет работать система
        /// </summary>
        /// <param name="CnfList">Конфигурация которая будет работать в системе</param>
        public static void SetupCurrentSharedConfigurations(ConfigurationList CnfList)
        {
            SetupCurrentSharedConfigurations(CnfList, true);
        }

        /// <summary>
        /// Установка текущей конфигурации в которой будет работать система
        /// </summary>
        /// <param name="CnfList">Конфигурация которая будет работать в системе</param>
        /// <param name="WriteLog">Писать в лог наши события</param>
        public static void SetupCurrentSharedConfigurations(ConfigurationList CnfList, bool WriteLog)
        {
            EventConfigurationList MyArg = new EventConfigurationList(CnfList, null);
            if (onСhengingCurrentCnfList != null) onСhengingCurrentCnfList.Invoke(CnfList, MyArg);

            // Если мы одобрили удаление
            if (MyArg.Action)
            {
                //bool rez = false;
                try
                {
                    CurrentCnfList = CnfList;

                    if (onСhengedCurrentCnfList != null) onСhengedCurrentCnfList.Invoke(CnfList, MyArg);
                    if (WriteLog) Com.Log.EventSave(string.Format("Пользователь изменил текущую конфигурацию на: {0}", CnfList.ConfigurationName), "ConfigurationFarm.SetupCurrentSharedConfigurations", EventEn.Message);
                }
                catch (Exception ex)
                {
                    if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при изменении текущей конфигурации {0} на конфигурацию {1} ({2})", (CurrentCnfList == null ? "null" : CurrentCnfList.ConfigurationName), CnfList.ConfigurationName, ex.Message), "ConfigurationFarm.SetupCurrentSharedConfigurations", EventEn.Error);
                    throw;
                }
                //return rez;
            }
            //return false;            
        }

    }
}
