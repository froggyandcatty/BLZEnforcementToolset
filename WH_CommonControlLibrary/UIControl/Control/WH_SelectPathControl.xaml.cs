using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WH_CommonControlLibrary.UIControl.Control
{
    /// <summary>
    /// 路由事件参数
    /// </summary>
    public class TextChangeRoutedEventArgs : RoutedEventArgs
    {
        public TextChangeRoutedEventArgs(RoutedEvent routedEvent, object source, string text) : base(routedEvent, source) { }

        public string Text { set; get; }
    }

    /// <summary>
    /// WH_SelectPathControl.xaml 的交互逻辑
    /// </summary>
    public partial class WH_SelectPathControl : UserControl
    {
        #region 定义
        /// <summary>
        /// 控件类型
        /// </summary>
        public enum EnumControlType
        {
            /// <summary>
            /// 读取文件
            /// </summary>
            LoadFile,
            /// <summary>
            /// 保存文件
            /// </summary>
            SaveFile,
            /// <summary>
            /// 选择路径
            /// </summary>
            SelectPath,
        }

        /// <summary>
        /// 事件委托
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        public delegate void TextChangeRoutedEventHandler(object sender, TextChangeRoutedEventArgs e);
        #endregion

        #region 属性
        /// <summary>
        /// 控件类型依赖项
        /// </summary>
        public static readonly DependencyProperty ControlTypeProperty = DependencyProperty.Register("ControlType", typeof(EnumControlType), typeof(WH_SelectPathControl));
        /// <summary>
        /// 控件类型属性
        /// </summary>
        public EnumControlType ControlType
        {
            set { SetValue(ControlTypeProperty, value); }
            get { return (EnumControlType)GetValue(ControlTypeProperty); }
        }

        /// <summary>
        /// 文件类型依赖项
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(WH_SelectPathControl));
        /// <summary>
        /// 文件类型属性
        /// </summary>
        public string Filter
        {
            set { SetValue(FilterProperty, value); }
            get { return (string)GetValue(FilterProperty); }
        }

        /// <summary>
        /// 窗口标题依赖项
        /// </summary>
        public static readonly DependencyProperty TitleDescriptionProperty = DependencyProperty.Register("TitleDescription", typeof(string), typeof(WH_SelectPathControl));
        /// <summary>
        /// 窗口标题属性
        /// </summary>
        public string TitleDescription
        {
            set { SetValue(TitleDescriptionProperty, value); }
            get { return (string)GetValue(TitleDescriptionProperty); }
        }

        /// <summary>
        /// 默认目录依赖项
        /// </summary>
        public static readonly DependencyProperty DefaultDirectoryProperty = DependencyProperty.Register("DefaultDirectory", typeof(string), typeof(WH_SelectPathControl));
        /// <summary>
        /// 窗口标题属性
        /// </summary>
        public string DefaultDirectory
        {
            set { SetValue(DefaultDirectoryProperty, value); }
            get { return (string)GetValue(DefaultDirectoryProperty); }
        }

        /// <summary>
        /// 按钮文本依赖项
        /// </summary>
        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register("ButtonContent", typeof(string), typeof(WH_SelectPathControl));
        /// <summary>
        /// 按钮文本属性
        /// </summary>
        public string ButtonContent
        {
            set { SetValue(ButtonContentProperty, value); }
            get { return (string)GetValue(ButtonContentProperty); }
        }

        /// <summary>
        /// 按钮文本依赖项
        /// </summary>
        public static readonly DependencyProperty PathTextProperty = DependencyProperty.Register("PathText", typeof(string), typeof(WH_SelectPathControl));
        /// <summary>
        /// 按钮文本属性
        /// </summary>
        public string PathText
        {
            set { TextBox_Path.Text = value; SetValue(PathTextProperty, value); }
            get { SetValue(PathTextProperty, TextBox_Path.Text); return (string)GetValue(PathTextProperty); }
        }

        /// <summary>
        /// 按钮文本依赖项
        /// </summary>
        public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register("ButtonWidth", typeof(double), typeof(WH_SelectPathControl));
        /// <summary>
        /// 按钮文本属性
        /// </summary>
        public double ButtonWidth
        {
            set { SetValue(ButtonWidthProperty, value); }
            get { return (double)GetValue(ButtonWidthProperty); }
        }

        /// <summary>
        /// 已选择依赖项
        /// </summary>
        public static readonly DependencyProperty IsHaveSelectedProperty = DependencyProperty.Register("IsHaveSelected", typeof(bool?), typeof(WH_SelectPathControl));
        /// <summary>
        /// 已选择本属性
        /// </summary>
        public bool? IsHaveSelected
        {
            private set { SetValue(IsHaveSelectedProperty, value); }
            get { return (bool?)GetValue(IsHaveSelectedProperty); }
        }

        /// <summary>
        /// 只读依赖项
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(WH_SelectPathControl));
        /// <summary>
        /// 只读择本属性
        /// </summary>
        public bool IsReadOnly
        {
            set
            {
                SetValue(IsReadOnlyProperty, value);
                TextBox_Path.IsReadOnly = value;
                Button_Select.IsEnabled = !value;
            }
            get { return (bool)GetValue(IsReadOnlyProperty); }
        }

        /// <summary>
        /// 文本变化路由事件依赖项
        /// </summary>
        public static readonly RoutedEvent TextChangeRoutedEvent = EventManager.RegisterRoutedEvent("TextChange", RoutingStrategy.Bubble, typeof(EventHandler<TextChangeRoutedEventArgs>), typeof(WH_SelectPathControl));

        /// <summary>
        /// 文本变化路由事件依赖项属性
        /// </summary>
        public event RoutedEventHandler TextChangeHandler
        {
            add { this.AddHandler(TextChangeRoutedEvent, value); }
            remove { this.RemoveHandler(TextChangeRoutedEvent, value); }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public WH_SelectPathControl()
        {
            InitializeComponent();
            ButtonWidth = double.NaN;
            IsHaveSelected = false;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 打开文件浏览器选择文件
        /// </summary>
        /// <param name="textBox">设置路径的TextBox控件</param>
        /// <param name="filter">文件类型筛选字符串</param>
        /// <param name="title">标题</param>
        /// <param name="defaultPath">默认路径</param>
        /// <returns>OpenFileDialog</returns>
        static System.Windows.Forms.OpenFileDialog OpenFileDialogGetOpenFile(TextBox textBox, string filter, string title, string defaultPath)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            if (File.Exists(textBox.Text))
            {
                fileDialog.InitialDirectory = new FileInfo(textBox.Text).DirectoryName;
            }
            if (Directory.Exists(textBox.Text))
            {
                fileDialog.InitialDirectory = textBox.Text;
            }
            else if (defaultPath != null && Directory.Exists(defaultPath))
            {
                fileDialog.InitialDirectory = defaultPath;
            }
            else
            {
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            }
            fileDialog.Filter = filter;
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = title;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox.Text = fileDialog.FileName;
                return fileDialog;
            }
            else
            {
                textBox.Text = "";
                return null;
            }
        }

        /// <summary>
        /// 保存文件选择路径
        /// </summary>
        /// <param name="textBox">设置路径的TextBox控件</param>
        /// <param name="filter">文件类型筛选字符串</param>
        /// <param name="title">标题</param>
        /// <param name="defaultPath">默认路径</param>
        /// <returns>SaveFileDialog</returns>
        static System.Windows.Forms.SaveFileDialog OpenFileDialogGetSavePath(TextBox textBox, string filter, string title, string defaultPath)
        {
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();

            if (File.Exists(textBox.Text))
            {
                fileDialog.InitialDirectory = new FileInfo(textBox.Text).DirectoryName;
            }
            else if (defaultPath != null && Directory.Exists(defaultPath))
            {
                fileDialog.InitialDirectory = defaultPath;
            }
            else
            {
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            }

            fileDialog.Filter = filter;
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = title;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox.Text = fileDialog.FileName;
                return fileDialog;
            }
            else
            {
                textBox.Text = "";
                return null;
            }
        }
        /// <summary>
        /// 选择文件夹路径
        /// </summary>
        /// <param name="textBox">设置路径的TextBox控件</param>
        /// <param name="description">打开描述</param>
        /// <param name="defaultPath">默认路径</param>
        /// <returns>FolderBrowserDialog</returns>
        static System.Windows.Forms.FolderBrowserDialog OpenDirectoryDialogGetFolder(TextBox textBox, string description, string defaultPath)
        {
            System.Windows.Forms.FolderBrowserDialog dirDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (Directory.Exists(textBox.Text))
            {
                dirDialog.SelectedPath = new FileInfo(textBox.Text).DirectoryName;
            }
            else if (defaultPath != null && Directory.Exists(defaultPath))
            {
                dirDialog.SelectedPath = defaultPath;
            }
            else
            {
                dirDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            }
            dirDialog.ShowNewFolderButton = true;
            dirDialog.Description = description;
            if (dirDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox.Text = dirDialog.SelectedPath;
                return dirDialog;
            }
            else
            {
                textBox.Text = "";
                return null;
            }
        }
        #endregion

        #region 控件方法
        /// <summary>
        /// 点击选择按钮事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            switch (ControlType)
            {
                case EnumControlType.LoadFile:
                    OpenFileDialogGetOpenFile(TextBox_Path, Filter, TitleDescription, DefaultDirectory);
                    break;
                case EnumControlType.SaveFile:
                    OpenFileDialogGetSavePath(TextBox_Path, Filter, TitleDescription, DefaultDirectory);
                    break;
                case EnumControlType.SelectPath:
                    OpenDirectoryDialogGetFolder(TextBox_Path, TitleDescription, DefaultDirectory);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 路径文本变化事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void TextBox_Path_TextChanged(object sender, TextChangedEventArgs e)
        {
            string path = TextBox_Path.Text;
            IsHaveSelected = File.Exists(path) || Directory.Exists(path);
            TextChangeRoutedEventArgs args = new TextChangeRoutedEventArgs(TextChangeRoutedEvent, this, path);
            this.RaiseEvent(args);
        }

        #endregion

    }
}
