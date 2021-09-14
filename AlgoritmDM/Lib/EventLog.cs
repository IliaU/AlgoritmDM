using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventLog :EventArgs
    {
        public String Message { get; private set; }
        public String Source { get; private set; }
        public EventEn Evn { get; private set; }
        public bool isLog;
        public bool Show;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Message">Сообщение</param>
        public EventLog(String Message, String Source, EventEn evn, bool isLog, bool Show)
        {
            this.Message = Message;
            this.Source = Source;
            this.Evn = evn;
            this.isLog = isLog;
            this.Show = Show;
        }
    }
}
