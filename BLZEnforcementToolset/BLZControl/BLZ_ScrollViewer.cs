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
    /// BLZ_ScrollViewer 滚动面板
    /// </summary>
    public class BLZ_ScrollViewer : ScrollViewer
    {
        #region 属性

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        static BLZ_ScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BLZ_ScrollViewer), new FrameworkPropertyMetadata(typeof(BLZ_ScrollViewer)));
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZ_ScrollViewer()
        {
            Style = Application.Current.Resources["BLZ_ScrollViewerStyle"] as Style;
        }
        #endregion

        #region 方法

        #endregion
    }
}
