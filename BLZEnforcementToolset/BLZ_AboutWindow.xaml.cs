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

namespace BLZEnforcementToolset
{
    /// <summary>
    /// BLZ_Window.xaml 的交互逻辑
    /// </summary>
    public partial class BLZ_AboutWindow : Window
    {

        #region 属性
        /// <summary>
        /// 当前语言
        /// </summary>
        public static EnumLanguage SoftwareLanguage {set; get;}

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZ_AboutWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResourceDictionary_WindowLanguage.MergedDictionaries.Clear();
            if (SoftwareLanguage == EnumLanguage.enUS)
            {
                ResourceDictionary_WindowLanguage.MergedDictionaries.Add(BLZEnforcement_BankEmailTool.BLZFont);
            }
            ResourceDictionary_WindowLanguage.MergedDictionaries.Add(BLZEnforcement_BankEmailTool.DictUILanguages[SoftwareLanguage]);
            if (SoftwareLanguage != EnumLanguage.zhCN)
            {
                Image_Alipay.Visibility = Visibility.Hidden;
                Image_Paypal.Visibility = Visibility.Visible;
                TextBlock_Alipay.Visibility = Visibility.Hidden;
                Button_BLZDonate.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region 方法
       
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
        /// 点击联系邮箱
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Hyperlink_EmailClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:whimsyduke@163.com?subject= &body= ");
        }
        /// <summary>
        /// 点击捐赠按钮
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_BLZDonate_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.me/Froggyandcatty");
        }
        /// <summary>
        /// 点击更新页地址
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/froggyandcatty/BLZEnforcementToolset/releases");
        }
        #endregion
    }
}
