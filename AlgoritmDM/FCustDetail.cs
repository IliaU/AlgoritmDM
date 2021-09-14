using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AlgoritmDM.Lib;

namespace AlgoritmDM
{
    public partial class FCustDetail : Form
    {
        private UProvider.CustDetailCheck CstDetChk;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CstDetChk">Детали по чекам клиента</param>
        public FCustDetail(UProvider.CustDetailCheck CstDetChk)
        {
            this.CstDetChk = CstDetChk;
            InitializeComponent();
            this.dgDetail.DataSource = this.CstDetChk.dtRez; 
        }
    }
}
