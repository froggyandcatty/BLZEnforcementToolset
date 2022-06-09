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
    /// BLZ_TextBox 文本框
    /// </summary>
    public class BLZ_TextBox : TextBox
    {
        #region 属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        static BLZ_TextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BLZ_TextBox), new FrameworkPropertyMetadata(typeof(BLZ_TextBox)));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZ_TextBox()
        {
            Style = Application.Current.Resources["BLZ_TextBoxStyle"] as Style;
        }
        #endregion

        #region 方法

        #endregion
    }
}
