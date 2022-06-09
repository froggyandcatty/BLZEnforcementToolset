using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WH_CommonControlLibrary.UIControl.UIWindow
{
    /// <summary>
    /// Interaction logic for WH_ProgressWindow.xaml
    /// </summary>
    public partial class WH_ProgressWindow : Window
    {
        #region 声明

        /// <summary>
        /// 按下停止按钮回调委托
        /// </summary>
        /// <param name="window">停止的窗口对象</param>
        /// <param name="inParams">停止参数</param>
        public delegate void Delegate_ProgressWindowStop(WH_ProgressWindow window, object inParams);

        #endregion

        #region 属性

        /// <summary>
        /// 停止按钮按下回调
        /// </summary>
        public Delegate_ProgressWindowStop StopCallBackFunc { set; get; }

        /// <summary>
        /// 停止按钮按下参数
        /// </summary>
        public object StopCallBackParams { set; get; }

        /// <summary>
        /// 是否已经停止
        /// </summary>
        public bool IsStop { set; get; } = false;

        #endregion

        #region 构造函数

        /// <summary>
        /// 显示进度条窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="primary">主要文本</param>
        /// <param name="secondaryLeft">左对齐次要文本</param>
        /// <param name="secondaryRight">右对齐次要文本</param>
        /// <param name="stop">停止</param>
        /// <param name="maximum">最大值</param>
        /// <param name="stopCallBack">停止回调</param>
        /// <param name="stopParams">停止回调参数</param>
        /// <returns>新窗口对象</returns>
        public static WH_ProgressWindow Show(string title, string primary, string secondaryLeft, string secondaryRight, string stop, double maximum, Delegate_ProgressWindowStop stopCallBack, object stopParams)
        {
            WH_ProgressWindow window = new WH_ProgressWindow(title, primary, secondaryLeft, secondaryRight, stop, maximum, stopCallBack, stopParams);
            window.Show();
            return window;
        }

        /// <summary>
        /// 显示进度条窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="primary">主要文本</param>
        /// <param name="secondaryLeft">左对齐次要文本</param>
        /// <param name="secondaryRight">右对齐次要文本</param>
        /// <param name="stop">停止</param>
        /// <param name="maximum">最大值</param>
        /// <param name="stopCallBack">停止回调</param>
        /// <param name="stopParams">停止回调参数</param>
        /// <returns>新窗口对象</returns>
        public static WH_ProgressWindow ShowDialog(string title, string primary, string secondaryLeft, string secondaryRight, string stop, double maximum, Delegate_ProgressWindowStop stopCallBack, object stopParams)
        {
            WH_ProgressWindow window = new WH_ProgressWindow(title, primary, secondaryLeft, secondaryRight, stop, maximum, stopCallBack, stopParams);
            window.ShowDialog();
            return window;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WH_ProgressWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="primary">主要文本</param>
        /// <param name="secondaryLeft">左对齐次要文本</param>
        /// <param name="secondaryRight">右对齐次要文本</param>
        /// <param name="stop">停止</param>
        /// <param name="maximum">最大值</param>
        /// <param name="stopCallBack">停止回调</param>
        /// <param name="stopParams">停止回调参数</param>
        public WH_ProgressWindow(string title, string primary, string secondaryLeft, string secondaryRight, string stop, double maximum, Delegate_ProgressWindowStop stopCallBack, object stopParams)
        {
            InitializeComponent();
            Title = title;
            TextBlock_Primary.Text = primary;
            TextBlock_SecodaryLeft.Text = secondaryLeft;
            TextBlock_SecodaryRight.Text = secondaryRight;
            Button_Stop.Content = stop;
            ProgressBar_Main.Maximum = maximum;
            ProgressBar_Main.Value = 0;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="text">文本内容</param>
        public void SetTitle(string text)
        {
            Title = text;
        }

        /// <summary>
        /// 设置首要文本
        /// </summary>
        /// <param name="text">文本内容</param>
        public void SetPrimaryText(string text)
        {
            TextBlock_Primary.Text = text;
        }

        /// <summary>
        /// 设置左对齐次要文本
        /// </summary>
        /// <param name="text">文本内容</param>
        public void SetSecondaryLeftText(string text)
        {
            TextBlock_SecodaryLeft.Text = text;
        }

        /// <summary>
        /// 设置右对齐次要文本
        /// </summary>
        /// <param name="text">文本内容</param>
        public void SetSecondaryRightText(string text)
        {
            TextBlock_SecodaryRight.Text = text;
        }
               
        /// <summary>
        /// 停止按钮文本
        /// </summary>
        /// <param name="text">文本内容</param>
        public void SetStopButtonText(string text)
        {
            Button_Stop.Content = text;
        }

        /// <summary>
        /// 设置进度条当前值
        /// </summary>
        /// <param name="value">值</param>
        public void SetProgressValue(double value)
        {
            ProgressBar_Main.Value = value;
        }

        /// <summary>
        /// 设置进度条最大值
        /// </summary>
        /// <param name="value">值</param>
        public void SetMaxPrecent(double value)
        {
            ProgressBar_Main.Maximum = value;
        }

        #endregion

        #region 控件方法

        /// <summary>
        /// 按下Stop按钮事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            IsStop = true;
            StopCallBackFunc?.Invoke(this, StopCallBackParams);
        }

        #endregion
    }
}
