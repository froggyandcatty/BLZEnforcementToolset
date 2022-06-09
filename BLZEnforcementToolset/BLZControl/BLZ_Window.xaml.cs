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
using System.Windows.Shapes;

using EnumLanguage = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumLanguage;
using EnumGameRegion = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumGameRegion;

namespace BLZEnforcementToolset.BLZControl
{
    /// <summary>
    /// BLZ_Window.xaml 的交互逻辑
    /// </summary>
    public partial class BLZ_Window : Window
    {
        #region 声明
        /// <summary>
        /// 窗口类型
        /// </summary>
        #endregion

        #region 属性
        /// <summary>
        /// 标题文本依赖项属性
        /// </summary>
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(BLZ_Window));
        /// <summary>
        /// 标题文本依赖项
        /// </summary>
        public string Caption
        {
            set { SetValue(CaptionProperty, value); }
            get { return (string)GetValue(CaptionProperty); }
        }
        /// <summary>
        /// 内容文本依赖项属性
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(BLZ_Window));
        /// <summary>
        /// 内容文本依赖项
        /// </summary>
        public string Text
        {
            set { SetValue(TextProperty, value); }
            get { return (string)GetValue(TextProperty); }
        }
        /// <summary>
        /// 窗口类型文本依赖项属性
        /// </summary>
        public static readonly DependencyProperty WindowTypeProperty = DependencyProperty.Register("WindowType", typeof(MessageBoxImage), typeof(BLZ_Window));
        /// <summary>
        /// 窗口类型依赖项
        /// </summary>
        public MessageBoxImage WindowType
        {
            set
            {
                SetValue(WindowTypeProperty, value);
                switch (value)
                {
                    case MessageBoxImage.None:
                        Image_ICON.Visibility = Visibility.Collapsed;
                        TextBlock_BLZInfoPannelCaption.HaloForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FFCC"));
                        break;
                    case MessageBoxImage.Warning:
                        Image_ICON.Visibility = Visibility.Visible;
                        Image_ICON.Source = Application.Current.Resources["IMAGE_Waning"] as BitmapImage;
                        TextBlock_BLZInfoPannelCaption.HaloForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF990000"));
                        break;
                    case MessageBoxImage.Information:
                        Image_ICON.Visibility = Visibility.Visible;
                        Image_ICON.Source = Application.Current.Resources["IMAGE_Complete"] as BitmapImage;
                        TextBlock_BLZInfoPannelCaption.HaloForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FFCC"));
                        break;
                    default:
                        break;
                }
            }
            get { return (MessageBoxImage)GetValue(WindowTypeProperty); }
        }
        /// <summary>
        /// 按钮类型文本依赖项属性
        /// </summary>
        public static readonly DependencyProperty ButtonTypeProperty = DependencyProperty.Register("ButtonType", typeof(MessageBoxButton), typeof(BLZ_Window));
        /// <summary>
        /// 按钮类型依赖项
        /// </summary>
        public MessageBoxButton ButtonType
        {
            set
            {
                SetValue(ButtonTypeProperty, value);
                switch (value)
                {
                    case MessageBoxButton.OK:
                        Button_BLZYES.Visibility = Visibility.Collapsed;
                        Button_BLZNO.Visibility = Visibility.Collapsed;
                        Button_BLZOK.Visibility = Visibility.Visible;
                        break;
                    case MessageBoxButton.YesNo:
                        Button_BLZYES.Visibility = Visibility.Visible;
                        Button_BLZNO.Visibility = Visibility.Visible;
                        Button_BLZOK.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
            }
            get { return (MessageBoxButton)GetValue(ButtonTypeProperty); }
        }

        /// <summary>
        /// 当前语言
        /// </summary>
        public static EnumLanguage SoftwareLanguage { set; get; }

        /// <summary>
        /// 静态返回结果
        /// </summary>
        public static bool StaticDialogResult { set; get; }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZ_Window()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caption">标题</param>
        /// <param name="text">文本</param>
        /// <param name="windowType">窗口类型</param>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="yesText">Yes按钮文本</param>
        /// <param name="noText">No按钮文本</param>
        /// <param name="okText">OK按钮文本</param>
        public BLZ_Window(string text, string caption, MessageBoxButton buttonType, MessageBoxImage windowType)
        {
            InitializeComponent();
            Caption = caption;
            Text = text;
            WindowType = windowType;
            ButtonType = buttonType;
            ResourceDictionary_WindowLanguage.MergedDictionaries.Clear();
            if (SoftwareLanguage == EnumLanguage.enUS)
            {
                ResourceDictionary_WindowLanguage.MergedDictionaries.Add(BLZEnforcement_BankEmailTool.BLZFont);
            }
            ResourceDictionary_WindowLanguage.MergedDictionaries.Add(BLZEnforcement_BankEmailTool.DictUILanguages[SoftwareLanguage]);
        }
        #endregion

        #region 方法
        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="messageBoxText">一个 System.String，用于指定要显示的文本。</param>
        /// <param name="caption">一个 System.String，用于指定要显示的标题栏标题。</param>
        /// <param name="button">一个 System.Windows.MessageBoxButton 值，用于指定要显示哪个按钮或哪些按钮。</param>
        /// <param name="icon">一个 System.Windows.MessageBoxImage 值，用于指定要显示的图标。</param>
        /// <returns>点击结果</returns>
        public static MessageBoxResult Show (string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                BLZ_Window messageBox = new BLZ_Window(messageBoxText, caption, button, icon)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                StaticDialogResult = messageBox.ShowDialog() == true;
            }));
            if (button != MessageBoxButton.OK)
            {
                return StaticDialogResult ? MessageBoxResult.Yes : MessageBoxResult.No;
            }
            else
            {
                return MessageBoxResult.OK;
            }
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="messageBoxText">一个 System.String，用于指定要显示的文本。</param>
        /// <param name="caption">一个 System.String，用于指定要显示的标题栏标题。</param>
        /// <param name="button">一个 System.Windows.MessageBoxButton 值，用于指定要显示哪个按钮或哪些按钮。</param>
        /// <returns>点击结果</returns>
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            return Show(messageBoxText, caption, button, MessageBoxImage.None);
        }
        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="messageBoxText">一个 System.String，用于指定要显示的文本。</param>
        /// <param name="caption">一个 System.String，用于指定要显示的标题栏标题。</param>
        /// <returns>点击结果</returns>
        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return Show(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 拖拽事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        /// <summary>
        /// 点击OK事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_BLZOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        /// <summary>
        /// 点击Yes事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_BLZYES_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        /// <summary>
        /// 点击NO事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_BLZNO_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        #endregion

    }
}
