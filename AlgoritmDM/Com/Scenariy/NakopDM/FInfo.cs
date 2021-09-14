using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlgoritmDM.Com.Scenariy.NakopDM
{
    public partial class FInfo : Form
    {
        /// <summary>
        /// Типоризированный сценарий с которым мы работаем
        /// </summary>
        private NakopDMscn MainNDM;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="MainNDM">Ссылка на наш типоризированный объект который мы будем настраивать</param>
        public FInfo(NakopDMscn MainNDM)
        {
            this.MainNDM = MainNDM;
            InitializeComponent();
        }
    }
}
