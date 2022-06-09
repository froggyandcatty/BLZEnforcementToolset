using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WH_CommonControlLibrary.UIControl.Control
{
    /// <summary>
    /// 正则检测模式
    /// </summary>
    public enum EnumRegexCheckMode
    {
        /// <summary>
        /// 焦点变化
        /// </summary>
        FocusChange,
        /// <summary>
        /// 文本变化
        /// </summary>
        TextChange,
    }
    /// <summary>
    /// 正则表达式文本框
    /// </summary>
    public class WH_RegexTextBox : TextBox
    {

        #region 属性字段
        /// <summary>
        /// 正则表达式依赖项
        /// </summary>
        public static readonly DependencyProperty RegexExpressionProperty = DependencyProperty.Register("RegexExpression", typeof(string), typeof(WH_RegexTextBox), new PropertyMetadata(""));

        /// <summary>
        /// 正则表达式属性
        /// </summary>
        public string RegexExpression { set => SetValue(RegexExpressionProperty, value); get => (string)GetValue(RegexExpressionProperty); }

        /// <summary>
        /// 正则表达式效验结果依赖项
        /// </summary>
        public static readonly DependencyProperty IsPassRegexCheckProperty = DependencyProperty.Register("IsPassRegexCheck", typeof(bool), typeof(WH_RegexTextBox), new PropertyMetadata(true));

        /// <summary>
        /// 正则表达式效验结果属性
        /// </summary>
        public bool IsPassRegexCheck { private set => SetValue(IsPassRegexCheckProperty, value); get => (bool)GetValue(IsPassRegexCheckProperty); }

        /// <summary>
        /// 正则表达式效验失败颜色依赖项
        /// </summary>
        public static readonly DependencyProperty RegexCheckFailureColorProperty = DependencyProperty.Register("RegexCheckFailureColor", typeof(Brush), typeof(WH_RegexTextBox), new PropertyMetadata(Brushes.Red));

        /// <summary>
        /// 正则表达式效验失败颜色属性
        /// </summary>
        public Brush RegexCheckFailureColor { set => SetValue(RegexCheckFailureColorProperty, value); get => (Brush)GetValue(RegexCheckFailureColorProperty); }

        /// <summary>
        /// 正则表达式效验通过颜色依赖项
        /// </summary>
        public static readonly DependencyProperty RegexCheckPassColorProperty = DependencyProperty.Register("RegexCheckPassColor", typeof(Brush), typeof(WH_RegexTextBox), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// 正则表达式效验通过颜色属性
        /// </summary>
        public Brush RegexCheckPassColor { set => SetValue(RegexCheckPassColorProperty, value); get => (Brush)GetValue(RegexCheckPassColorProperty); }


        /// <summary>
        /// 正则表达式效检测模式依赖项
        /// </summary>
        public static readonly DependencyProperty RegexCheckModeProperty = DependencyProperty.Register("RegexCheckMode", typeof(EnumRegexCheckMode), typeof(WH_RegexTextBox), new PropertyMetadata(EnumRegexCheckMode.FocusChange));

        /// <summary>
        /// 正则表达式效检测模式属性
        /// </summary>
        public EnumRegexCheckMode RegexCheckMode { set => SetValue(RegexCheckModeProperty, value); get => (EnumRegexCheckMode)GetValue(RegexCheckModeProperty); }

        /// <summary>
        /// 正则表达式效验输入允许依赖项
        /// </summary>
        public static readonly DependencyProperty EnableRegexInputCheckProperty = DependencyProperty.Register("EnableRegexInputCheck", typeof(bool), typeof(WH_RegexTextBox), new PropertyMetadata(false));

        /// <summary>
        /// 正则表达式效验输入允许属性
        /// </summary>
        public bool EnableRegexInputCheck { set => SetValue(EnableRegexInputCheckProperty, value); get => (bool)GetValue(EnableRegexInputCheckProperty); }

        /// <summary>
        /// 上一次的文本
        /// </summary>
        private string LastString { set; get; }
        private bool InternalTextChange { set; get; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public WH_RegexTextBox()
        {
            LastString = Text;
            InternalTextChange = false;
            LostFocus += WH_RegexTextBox_LostFocus;
            GotFocus += WH_RegexTextBox_GotFocus;
            TextChanged += WH_RegexTextBox_TextChanged;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 验证文本符合正则表达式
        /// </summary>
        private void CheckTextWithRegex()
        {
            if (string.IsNullOrEmpty(RegexExpression) || string.IsNullOrEmpty(Text))
            {
                IsPassRegexCheck = true;
            }
            else
            {
                IsPassRegexCheck = Regex.Match(Text, RegexExpression).Value == Text;
            }
        }

        #endregion

        #region 事件响应

        /// <summary>
        /// 失去焦点事件响应
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void WH_RegexTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RegexCheckMode != EnumRegexCheckMode.FocusChange) return;
            CheckTextWithRegex();
            if (!IsPassRegexCheck)
            {
                Foreground = RegexCheckFailureColor;
                
            }
            else
            {
                Foreground = RegexCheckPassColor;
            }
        }

        /// <summary>
        /// 获得点事件响应
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void WH_RegexTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (RegexCheckMode != EnumRegexCheckMode.FocusChange) return;
            Foreground = RegexCheckPassColor;
        }

        /// <summary>
        /// 文本变化事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void WH_RegexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (InternalTextChange) return;
            CheckTextWithRegex();
            if (EnableRegexInputCheck)
            {
                if (!IsPassRegexCheck)
                {
                    IsPassRegexCheck = true;
                    InternalTextChange = true;
                    Text = LastString;
                    InternalTextChange = false;
                }
            }
            else if (RegexCheckMode == EnumRegexCheckMode.TextChange)
            {
                if (!IsPassRegexCheck)
                {
                    Foreground = RegexCheckFailureColor;

                }
                else
                {
                    Foreground = RegexCheckPassColor;
                }
            }
            LastString = Text;
        }

        #endregion
    }
}
