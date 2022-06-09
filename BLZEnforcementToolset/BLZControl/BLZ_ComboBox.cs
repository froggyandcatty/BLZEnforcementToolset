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
    /// 对象到字符串转换器
    /// </summary>
    public class ObjectToStringConverter : IValueConverter
    {
        /// <summary>
        /// 转换函数
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">本地化</param>
        /// <returns>转换结果</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? "": value.ToString();
        }

        /// <summary>
        /// 逆向转换函数
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">本地化</param>
        /// <returns>转换结果</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("不能换回");
        }
    }
    
    /// <summary>
    /// BLZ_ComboBox 下拉框
    /// </summary>
    public class BLZ_ComboBox : ComboBox
    {
        #region 属性

        /// <summary>
        /// 显示滚动条依赖项属性
        /// </summary>
        public static readonly DependencyProperty ScrollVisibilityProperty = DependencyProperty.Register("ScrollVisibility", typeof(Visibility), typeof(BLZ_ComboBox), new PropertyMetadata(Visibility.Hidden));
        /// <summary>
        /// 显示滚动条依赖项
        /// </summary>
        public Visibility ScrollVisibility
        {
            set { SetValue(ScrollVisibilityProperty, value); }
            get { return (Visibility)GetValue(ScrollVisibilityProperty); }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        static BLZ_ComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BLZ_ComboBox), new FrameworkPropertyMetadata(typeof(BLZ_ComboBox)));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZ_ComboBox()
        {
            Style = Application.Current.Resources["BLZ_ComboBoxStyle"] as Style;
        }

        #endregion

        #region 方法

        #endregion
    }
}
