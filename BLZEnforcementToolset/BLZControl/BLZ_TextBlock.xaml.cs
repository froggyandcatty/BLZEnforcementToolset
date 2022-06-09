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
    /// BLZ_TextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class BLZ_TextBlock : UserControl
    {
        #region 属性
        /// <summary>
        /// 文本内容依赖项属性
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(BLZ_TextBlock));
        /// <summary>
        /// 文本内容依赖项
        /// </summary>
        public string Text
        {
            set { SetValue(TextProperty, value); }
            get { return (string)GetValue(TextProperty); }
        }
        /// <summary>
        /// 光晕色依赖项属性
        /// </summary>
        public static readonly DependencyProperty HaloForegroundProperty = DependencyProperty.Register("HaloForeground", typeof(Brush), typeof(BLZ_TextBlock));
        /// <summary>
        /// 光晕色依赖项
        /// </summary>
        public Brush HaloForeground
        {
            set { SetValue(HaloForegroundProperty, value); }
            get { return (Brush)GetValue(HaloForegroundProperty); }
        }
        /// <summary>
        /// 光晕半径依赖项属性
        /// </summary>
        public static readonly DependencyProperty HaloRadiusProperty = DependencyProperty.Register("HaloRadius", typeof(int), typeof(BLZ_TextBlock));
        /// <summary>
        /// 光晕色依赖项
        /// </summary>
        public int HaloRadius
        {
            set { SetValue(HaloRadiusProperty, value); }
            get { return (int)GetValue(HaloRadiusProperty); }
        }
        /// <summary>
        /// 光晕半径依赖项属性
        /// </summary>
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(BLZ_TextBlock));
        /// <summary>
        /// 光晕色依赖项
        /// </summary>
        public TextWrapping TextWrapping
        {
            set { SetValue(TextWrappingProperty, value); }
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
        }
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZ_TextBlock()
        {
            InitializeComponent();
            
        }
        #endregion

        #region 方法

        #endregion
    }
}
