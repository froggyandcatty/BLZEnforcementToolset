using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BLZEnforcementToolset.BLZControl
{
    /// <summary>
    /// BLZ_ProgressBar 进度条
    /// </summary>
    public class BLZ_ProgressBar : ProgressBar
    {
        #region 属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        static BLZ_ProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BLZ_ProgressBar), new FrameworkPropertyMetadata(typeof(BLZ_ProgressBar)));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZ_ProgressBar()
        {
            Style = Application.Current.Resources["BLZ_ProgressBarStyle"] as Style;
        }
        #endregion

        #region 方法

        #endregion
    }
}
