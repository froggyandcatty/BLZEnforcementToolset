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
    /// BLZ_Button 按钮
    /// </summary>
    public class BLZ_Button : Button
    {
        #region 属性
        /// <summary>
        /// 通用图片依赖项属性
        /// </summary>
        public static readonly DependencyProperty NormalImageProperty = DependencyProperty.Register("NormalImage", typeof(BitmapImage), typeof(BLZ_Button));
        /// <summary>
        /// 通用图片依赖项
        /// </summary>
        public BitmapImage NormalImage
        {
            set { SetValue(NormalImageProperty, value); }
            get { return (BitmapImage)GetValue(NormalImageProperty); }
        }
        /// <summary>
        /// 高亮图片依赖项属性
        /// </summary>
        public static readonly DependencyProperty HoverImageProperty = DependencyProperty.Register("HoverImage", typeof(BitmapImage), typeof(BLZ_Button));
        /// <summary>
        /// 高亮图片依赖项
        /// </summary>
        public BitmapImage HoverImage
        {
            set { SetValue(HoverImageProperty, value); }
            get { return (BitmapImage)GetValue(HoverImageProperty); }
        }
        /// <summary>
        /// 按下图片依赖项属性
        /// </summary>
        public static readonly DependencyProperty PressImageProperty = DependencyProperty.Register("PressImage", typeof(BitmapImage), typeof(BLZ_Button));
        /// <summary>
        /// 按下图片依赖项
        /// </summary>
        public BitmapImage PressImage
        {
            set { SetValue(PressImageProperty, value); }
            get { return (BitmapImage)GetValue(PressImageProperty); }
        }
        /// <summary>
        /// 禁用图片依赖项属性
        /// </summary>
        public static readonly DependencyProperty DisableImageProperty = DependencyProperty.Register("DisableImage", typeof(BitmapImage), typeof(BLZ_Button));
        /// <summary>
        /// 禁用图片依赖项
        /// </summary>
        public BitmapImage DisableImage
        {
            set { SetValue(DisableImageProperty, value); }
            get { return (BitmapImage)GetValue(DisableImageProperty); }
        }
        /// <summary>
        /// 选定图片依赖项属性
        /// </summary>
        public static readonly DependencyProperty CheckedImageProperty = DependencyProperty.Register("CheckedImage", typeof(BitmapImage), typeof(BLZ_Button));
        /// <summary>
        /// 选定图片依赖项
        /// </summary>
        public BitmapImage CheckedImage
        {
            set { SetValue(CheckedImageProperty, value); }
            get { return (BitmapImage)GetValue(CheckedImageProperty); }
        }
        
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        static BLZ_Button()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BLZ_Button), new FrameworkPropertyMetadata(typeof(BLZ_Button)));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZ_Button() : base()
        {
            Style = Application.Current.Resources["BLZ_ButtonStyle"] as Style;
        }

        #endregion

        #region 方法
        #endregion
    }
}
