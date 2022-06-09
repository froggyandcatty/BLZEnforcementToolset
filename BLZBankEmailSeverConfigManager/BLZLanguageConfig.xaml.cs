using ICSharpCode.AvalonEdit.Highlighting;
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

using EnumLanguage = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumLanguage;

namespace BLZEnforcementToolset
{
    /// <summary>
    /// LanguageConfig.xaml 的交互逻辑
    /// </summary>
    public partial class BLZLanguageConfig : TabItem
    {
        #region 属性字段
        /// <summary>
        /// 文本变化路由事件依赖项
        /// </summary>
        public static readonly EventHandler VersionLogTextChangeRoutedEvent;

        /// <summary>
        /// 文本变化路由事件依赖项属性
        /// </summary>
        public event EventHandler VersionLogTextChangeHandler
        {
            add { _VersionLogTextChangeHandler += value; }
            remove { _VersionLogTextChangeHandler -= value; }
        }
        private EventHandler _VersionLogTextChangeHandler;

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZLanguageConfig()
        {
            InitializeComponent();
        }
        #endregion

        #region 控件事件
        /// <summary>
        /// Html模式切换事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void CheckBox_IsBodyHtml_CheckChange(object sender, RoutedEventArgs e)
        {
            if (CheckBox_IsBodyHtml.IsChecked == true)
            {
                TextEditor_EmailBody.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
            }
            else
            {
                TextEditor_EmailBody.SyntaxHighlighting = null;
            }
        }

        /// <summary>
        /// 软件日志文本变化
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void TextEditor_SoftwareLog_TextChanged(object sender, EventArgs e)
        {
            _VersionLogTextChangeHandler?.Invoke(sender, e);
        }
        #endregion
    }
}
