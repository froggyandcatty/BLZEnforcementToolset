using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// BLZ_ScrollBar 滚动条
    /// </summary>
    public class BLZ_ScrollBar : ScrollBar
    {
        #region 属性

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        static BLZ_ScrollBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BLZ_ScrollBar), new FrameworkPropertyMetadata(typeof(BLZ_ScrollBar)));
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZ_ScrollBar()
        {
            Style = Application.Current.Resources["BLZ_ScrollBarStyle"] as Style;
        }
        #endregion

        #region 方法

        #endregion
    }
}
