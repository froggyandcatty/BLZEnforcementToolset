using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using BLZOnLineConfigDataSetStruct;
using EnumLanguage = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumLanguage;
using EnumGameRegion = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumGameRegion;
using System.IO;
using System.Linq;
using ICSharpCode.AvalonEdit;

namespace BLZEnforcementToolset
{
    /// <summary>
    /// 游戏配置信息
    /// </summary>
    public class GameMapConfigInfoItem : ListBoxItem
    {
        #region 属性字段
        /// <summary>
        /// 配置URL依赖项属性
        /// </summary>
        public static DependencyProperty ConfigUrlProperty = DependencyProperty.Register("ConfigUrl", typeof(string), typeof(GameMapConfigInfoItem), new PropertyMetadata(""));

        /// <summary>
        /// 配置URL依赖项
        /// </summary>
        public string ConfigUrl { set => SetValue(ConfigUrlProperty, value); get => (string)GetValue(ConfigUrlProperty); }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public GameMapConfigInfoItem() : base()
        {
            Content = "新建";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="address">配置地址</param>
        /// <param name="name">配置名称</param>
        public GameMapConfigInfoItem(string address, string name) : base()
        {
            ConfigUrl = address;
            Content = name;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="row">数据行</param>
        public GameMapConfigInfoItem(DataRow row)
        {
            Content = row[BLZ_EmailAndMapConfigDataSet.ColumnName_MapConfig_MapName] as string;
            ConfigUrl = row[BLZ_EmailAndMapConfigDataSet.ColumnName_MapConfig_ConfigAddress] as string;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加到数据表
        /// </summary>
        /// <param name="dataSet">邮箱及地图数据</param>
        public void AddDataRow(BLZ_EmailAndMapConfigDataSet dataSet)
        {
            dataSet.AddNewMap(Content as string, ConfigUrl);
        }
        #endregion

    }

    /// <summary>
    /// 版本配置信息
    /// </summary>
    public class SoftwareConfigVersionItem : ComboBoxItem
    {
        #region 属性字段

        /// <summary>
        /// 主版本号依赖项属性
        /// </summary>
        public static DependencyProperty MajorVersionProperty = DependencyProperty.Register("MajorVersion", typeof(int), typeof(SoftwareConfigVersionItem), new PropertyMetadata(0));

        /// <summary>
        /// 主版本号依赖
        /// </summary>
        public int MajorVersion { set => SetValue(MajorVersionProperty, value); get => (int)GetValue(MajorVersionProperty); }

        /// <summary>
        /// 次版本号依赖项属性
        /// </summary>
        public static DependencyProperty MinorVersionProperty = DependencyProperty.Register("MinorVersion", typeof(int), typeof(SoftwareConfigVersionItem), new PropertyMetadata(0));

        /// <summary>
        /// 次版本号依赖
        /// </summary>
        public int MinorVersion { set => SetValue(MinorVersionProperty, value); get => (int)GetValue(MinorVersionProperty); }

        /// <summary>
        /// 次版本号依赖项属性
        /// </summary>
        public static DependencyProperty BuildVersionProperty = DependencyProperty.Register("BuildVersion", typeof(int), typeof(SoftwareConfigVersionItem), new PropertyMetadata(0));

        /// <summary>
        /// 修订版本号依赖
        /// </summary>
        public int BuildVersion { set => SetValue(BuildVersionProperty, value); get => (int)GetValue(BuildVersionProperty); }

        /// <summary>
        /// 修订版本号依赖项属性
        /// </summary>
        public static DependencyProperty RevisedVersionProperty = DependencyProperty.Register("RevisedVersion", typeof(int), typeof(SoftwareConfigVersionItem), new PropertyMetadata(0));

        /// <summary>
        /// 次版本号依赖
        /// </summary>
        public int RevisedVersion { set => SetValue(RevisedVersionProperty, value); get => (int)GetValue(RevisedVersionProperty); }

        /// <summary>
        /// 版本字符串依赖项属性
        /// </summary>
        public static DependencyProperty VersionStringProperty = DependencyProperty.Register("VersionString", typeof(string), typeof(SoftwareConfigVersionItem), new PropertyMetadata(""));

        /// <summary>
        /// 版本字符串依赖项
        /// </summary>
        public string VersionString
        {
            get
            {
                SetValue(VersionStringProperty, BLZEnforcement_BankEmailTool.GetVersionString(MajorVersion, MinorVersion, BuildVersion, RevisedVersion));
                return (string)GetValue(VersionStringProperty);
            }
        }
        
        /// <summary>
        /// 下载地址依赖项属性
        /// </summary>
        public static DependencyProperty DownloadLinkProperty = DependencyProperty.Register("DownloadLink", typeof(string), typeof(SoftwareConfigVersionItem), new PropertyMetadata(""));

        /// <summary>
        /// 下载地址依赖项
        /// </summary>
        public string DownloadLink { private set; get; } = "https://github.com/froggyandcatty/BLZEnforcementToolset/releases";

        /// <summary>
        /// 配置地址依赖项属性
        /// </summary>
        public static DependencyProperty ConfigLinkProperty = DependencyProperty.Register("ConfigLink", typeof(string), typeof(SoftwareConfigVersionItem), new PropertyMetadata(""));

        /// <summary>
        /// 下载地址依赖项
        /// </summary>
        public string ConfigLink
        {
            get
            {
                SetValue(ConfigLinkProperty, BLZEnforcement_BankEmailTool.Const_BaseAddress + BLZEnforcement_BankEmailTool.Const_ConfigFolder + BLZEnforcement_BankEmailTool.Const_FilePrefix + VersionString + ".cfg");
                return (string)GetValue(ConfigLinkProperty);
            }
        }

        /// <summary>
        /// 更新日志
        /// </summary>
        public Dictionary<EnumLanguage, string> ChangeLog { private set; get; } = new Dictionary<EnumLanguage, string>();

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SoftwareConfigVersionItem () : base()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            MajorVersion = version.Major;
            MinorVersion = version.Minor;
            BuildVersion = version.Build;
            RevisedVersion = version.Revision;
            Content = VersionString;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="row">数据行</param>
        public SoftwareConfigVersionItem(DataRow row)
        {
            MajorVersion = (int)row[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_MajorVersion];
            MinorVersion = (int)row[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_MinorVersion];
            BuildVersion = (int)row[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_BuildVersion];
            RevisedVersion = (int)row[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_RevisedVersion];
            foreach (DataRow select in row.GetChildRows(BLZ_SoftwareConfigDataSet.RelationName_VersionString_LanguageConfig))
            {
                string changeLog = select[BLZ_SoftwareConfigDataSet.ColumnName_LanguageConfig_SoftwareChangeLog] as string;
                if (!string.IsNullOrEmpty(changeLog))
                {
                    ChangeLog.Add((EnumLanguage)select[BLZ_SoftwareConfigDataSet.ColumnName_LanguageConfig_LanguageId], changeLog);
                }
            }
            Content = VersionString;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 添加到数据表
        /// </summary>
        /// <param name="tableVersion">版本数据表</param>
        /// <param name="tableLanguage">语言数据表</param>
        public void AddDataRow(BLZ_SoftwareConfigDataSet dataSet)
        {
            dataSet.AddNewVersionInfo(VersionString, MajorVersion, MinorVersion, BuildVersion, RevisedVersion, DownloadLink, ConfigLink);
            foreach (var select in ChangeLog)
            {
                dataSet.AddNewLanguage(select.Key, VersionString, select.Value);
            }
        }

        /// <summary>
        /// 设置版本
        /// </summary>
        /// <param name="major">主要版本号</param>
        /// <param name="minor">次要版本号</param>
        /// <param name="build">修订版本号</param>
        /// <param name="revised">编译版本号</param>
        public void SetVersion(int major, int minor, int build, int revised)
        {
            MajorVersion = major;
            MinorVersion = minor;
            BuildVersion = build;
            RevisedVersion = revised;
            Content = VersionString;
        }

        /// <summary>
        /// 设置语言的更新日志
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="text">日志内容</param>
        public void SetChangeLog(EnumLanguage language, string text)
        {
            ChangeLog[language] = text;
        }

        /// <summary>
        /// 设置软件下载地址
        /// </summary>
        /// <param name="address">地址</param>
        public void SetSoftwareDownLoadLinke(string address)
        {
            DownloadLink = address;
        }

        #endregion

    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BLZBankEmailSeverConfigManager_Window : Window
    {
        #region 属性字段

        #region 属性

        /// <summary>
        /// 实例
        /// </summary>
        public static BLZBankEmailSeverConfigManager_Window Instance { set; get; }

        /// <summary>
        /// 允许修改地图配置
        /// </summary>
        private bool IsDisableConfigMapConfig { set; get; } = false;

        /// <summary>
        /// 运行修改版本配置
        /// </summary>
        private bool IsDisableModifyVersionConfig { set; get; } = false;

        #endregion

        #region 字段
        private readonly FileInfo VersionInfoConfigFile = new FileInfo("BLZBankEmailSeverConfigManager_VersionInfo.xml");
        private readonly FileInfo EmailInfoConfigFile   = new FileInfo("BLZBankEmailSeverConfigManager_EmailInfo.xml");
        private readonly FileInfo VersionCfgFileName    = new FileInfo(BLZEnforcement_BankEmailTool.Const_VersionInfo);
        #endregion

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZBankEmailSeverConfigManager_Window()
        {
            InitializeComponent();
            Title = "防卫局Bank右键配置管理器 V" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Instance = this;
#if !DEBUG
            try
#endif
            {
                BLZ_SoftwareConfigDataSet softwareData = null;
                BLZ_EmailAndMapConfigDataSet emailData = null;
                string[] pargs = Environment.GetCommandLineArgs();
                if (pargs.Count() > 2 && pargs[1].ToLower() == "-url")
                {
                    softwareData = BLZ_SoftwareConfigDataSet.DataSetDeserializeDecompress(pargs[2]);
                    emailData = BLZ_EmailAndMapConfigDataSet.DataSetDeserializeDecompress(pargs[3]);
                }
                else
                {
                    if (VersionInfoConfigFile.Exists)
                    {
                        softwareData = new BLZ_SoftwareConfigDataSet();
                        softwareData.ReadXml(VersionInfoConfigFile.FullName);
                        
                    }
                    if (EmailInfoConfigFile.Exists)
                    {
                        emailData = new BLZ_EmailAndMapConfigDataSet();
                        emailData.ReadXml(EmailInfoConfigFile.FullName);
                    }
                }
                if (softwareData != null && softwareData != null)
                {
                    RestoreUIFromConfig(softwareData, emailData);
                }
            }
#if !DEBUG
            catch (Exception err)
            {
                MessageBox.Show(CurrentLanguage["MSG_LoadUIConfigFailed_Text"] + "\n" + err.Message, CurrentLanguage["MSG_LoadUIConfigFailed_Caption"].ToString(), MessageBoxButton.OK);
            }
#endif
        }
        #endregion

        #region 方法

        /// <summary>
        /// 从配置还原UI
        /// </summary>
        /// <param name="softwareData">软件配置</param>
        private void RestoreUIFromConfig(BLZ_SoftwareConfigDataSet softwareData, BLZ_EmailAndMapConfigDataSet emailData)
        {
            if (softwareData != null)
            {

                foreach (DataRow row in softwareData.Tables[BLZ_SoftwareConfigDataSet.TableName_VersionInfo].Rows)
                {
                    ComboBox_SoftwareVersion.Items.Add(new SoftwareConfigVersionItem(row));
                }
            }
            if (emailData != null)
            {

                foreach (DataRow row in emailData.Tables[BLZ_EmailAndMapConfigDataSet.TableName_MapConfig].Rows)
                {
                    ListBox_MapList.Items.Add(new GameMapConfigInfoItem(row));
                }
                foreach (BLZLanguageConfig select in TabControl_LanguageConfig.Items)
                {
                    EnumLanguage lang = (EnumLanguage)select.Tag;
                    DataRow row = emailData.Tables[BLZ_EmailAndMapConfigDataSet.TableName_EmailLanguageConfig].Rows.Find(lang);
                    select.TextBox_EmailSubject.Text = row[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailLanguageConfig_EmailSubject] as string;
                    select.TextEditor_EmailBody.Text = row[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailLanguageConfig_EmailBody] as string;
                    select.CheckBox_IsBodyHtml.IsChecked = (bool)row[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailLanguageConfig_IsBodyHtml];
                    if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item && item.ChangeLog.ContainsKey(lang))
                    {
                        select.TextEditor_SoftwareLog.Text = item.ChangeLog[lang];
                    }
                }
                foreach (BLZRegionInfoConfig select in TabControl_RegionConfig.Items)
                {
                    EnumGameRegion region = (EnumGameRegion)select.Tag;
                    DataRow row = emailData.Tables[BLZ_EmailAndMapConfigDataSet.TableName_EmailGameRegionConfig].Rows.Find(region);
                    select.TextBox_EmailSmtpHost.Text = row[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_EmailSmtpHost] as string;
                    select.RegexTextBox_EmailSmtpPort.Text = row[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_EmailSmtpPort].ToString();
                    select.TextBox_EmailSmtpUser.Text = row[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_EmailSmtpUser] as string;
                    select.TextBox_EmailSmtpPassword.Text = row[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_EmailSmtpPassword] as string;
                    select.TextBox_EmailSmtpFrom.Text = row[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_EmailSmtpFrom] as string;
                    select.TextBox_GameBankNamePrefix.Text = row[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_GameBankNamePrefix] as string;
                    select.TextBox_AuthorPlayerHandle.Text = row[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_AuthorPlayerHandle] as string;
                }
                TextBox_BLZGameCacheConfig.Text = emailData.Tables[BLZ_EmailAndMapConfigDataSet.TableName_Config].Rows[0][BLZ_EmailAndMapConfigDataSet.ColumnName_Config_GameCacheConfig] as string;
            }
        }

        /// <summary>
        /// 从UI读取配置
        /// </summary>
        /// <param name="emailData">邮箱与地图配置</param>
        /// <returns>生成成功</returns>
        private bool GetConfigFromUI(BLZ_SoftwareConfigDataSet softwareData, BLZ_EmailAndMapConfigDataSet emailData)
        {
            foreach (BLZLanguageConfig select in TabControl_LanguageConfig.Items)
            {
                EnumLanguage lang = (EnumLanguage)select.Tag;
                emailData.AddNewLanguage(lang, select.TextBox_EmailSubject.Text, select.TextEditor_EmailBody.Text, select.CheckBox_IsBodyHtml.IsChecked == true);
                if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item && item != null)
                {
                    item.ChangeLog[lang] = select.TextEditor_SoftwareLog.Text;
                }
            }
            foreach (BLZRegionInfoConfig select in TabControl_RegionConfig.Items)
            {
                EnumGameRegion region = (EnumGameRegion)select.Tag;
                int port = 0;
                if (!string.IsNullOrEmpty(select.RegexTextBox_EmailSmtpPort.Text))
                {
                    try
                    {
                        port = int.Parse(select.RegexTextBox_EmailSmtpPort.Text);
                    }
                    catch
                    {
                        MessageBox.Show("端口号解析异常（" + Enum.GetName(region.GetType(), region) + ")", "错误");
                        return false;
                    }
                }
                emailData.AddNewRegion(region, select.TextBox_EmailSmtpHost.Text, port, select.TextBox_EmailSmtpUser.Text, select.TextBox_EmailSmtpPassword.Text, select.TextBox_EmailSmtpFrom.Text, select.TextBox_GameBankNamePrefix.Text, select.TextBox_AuthorPlayerHandle.Text);
                emailData.SetBLZGameCacheConfig(TextBox_BLZGameCacheConfig.Text);
            }
            foreach (SoftwareConfigVersionItem select in ComboBox_SoftwareVersion.Items)
            {
                select.AddDataRow(softwareData);
            }
            foreach (GameMapConfigInfoItem select in ListBox_MapList.Items)
            {
                select.AddDataRow(emailData);
            }
            return true;
        }

        /// <summary>
        /// 选择第一个可见的地图
        /// </summary>
        private void SelectFirstVisibleMapItem()
        {
            foreach (GameMapConfigInfoItem select in ListBox_MapList.Items)
            {
                if (select.Visibility == Visibility.Visible)
                {
                    ListBox_MapList.SelectedItem = select;
                    return;
                }
            }
        }

        /// <summary>
        /// 刷新地图配置筛选
        /// </summary>
        private void RefreshMapConfigFilter()
        {
            foreach (GameMapConfigInfoItem select in ListBox_MapList.Items)
            {
                if (select.Content.ToString().Contains(TextBox_MapSearch.Text))
                {
                    select.Visibility = Visibility.Visible;
                }
                else
                {
                    select.Visibility = Visibility.Collapsed;
                }
            }

            if (ListBox_MapList.SelectedItem == null)
            {
                SelectFirstVisibleMapItem();
                return;
            }

            GameMapConfigInfoItem item = ListBox_MapList.SelectedItem as GameMapConfigInfoItem;
            if (item.Visibility != Visibility.Visible)
            {
                SelectFirstVisibleMapItem();
            }
        }

        #endregion

        #region 控件方法

        /// <summary>
        /// 显示地图配置
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_MapConfigShow_Click(object sender, RoutedEventArgs e)
        {
            SC2GameCacheSeverConfigManager manager = new SC2GameCacheSeverConfigManager(@"http://froggyandcatty.com/sc2software/BLZconfig/BlzEnforcement.cfg");
            manager.ShowDialog();
        }

        /// <summary>
        /// 新建地图配置
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_MapConfigCreate_Click(object sender, RoutedEventArgs e)
        {
            GameMapConfigInfoItem item = new GameMapConfigInfoItem();
            ListBox_MapList.Items.Add(item);
            item.IsSelected = true;
        }
        
        /// <summary>
        /// 删除地图配置
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_MapConfigDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_MapList.SelectedItem != null) ListBox_MapList.Items.Remove(ListBox_MapList.SelectedItem);
            RefreshMapConfigFilter();

        }

        /// <summary>
        /// 选择切换
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void ListBox_MapGameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                if (e.AddedItems[0] is GameMapConfigInfoItem item)
                {
                    IsDisableConfigMapConfig = true;
                    TextBox_MapName.Text = item.Content.ToString();
                    TextBox_MapConfigUrl.Text = item.ConfigUrl;
                    IsDisableConfigMapConfig = false;
                    return;
                }
            }
            IsDisableConfigMapConfig = true;
            TextBox_MapName.Text = "";
            TextBox_MapConfigUrl.Text = "";
            IsDisableConfigMapConfig = false;
        }

        /// <summary>
        /// 所选名称项切换
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void TextBox_MapName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsDisableConfigMapConfig || ListBox_MapList.SelectedItem == null) return;
            GameMapConfigInfoItem item = ListBox_MapList.SelectedItem as GameMapConfigInfoItem;
            item.Content = TextBox_MapName.Text;
        }

        /// <summary>
        /// 所选项地址切换
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void TextBox_MapConfigUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsDisableConfigMapConfig || ListBox_MapList.SelectedItem == null) return;
            GameMapConfigInfoItem item = ListBox_MapList.SelectedItem as GameMapConfigInfoItem;
            item.ConfigUrl = TextBox_MapConfigUrl.Text;
        }

        /// <summary>
        /// 筛选文字变化
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void TextBox_MapSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshMapConfigFilter();
        }

        /// <summary>
        /// 新建版本
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_VersionNew_Click(object sender, RoutedEventArgs e)
        {
            IsDisableModifyVersionConfig = true;
            SoftwareConfigVersionItem item = new SoftwareConfigVersionItem();
            foreach (SoftwareConfigVersionItem select in ComboBox_SoftwareVersion.Items)
            {
                if (select.VersionString == item.VersionString)
                {
                    MessageBox.Show("当前版本已经存在！", "错误");
                    IsDisableModifyVersionConfig = false;
                    return;
                }
            }
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem current)
            {
                foreach (KeyValuePair<EnumLanguage, string> select in current.ChangeLog)
                {
                    item.ChangeLog[select.Key] = select.Value;
                }
            }
            ComboBox_SoftwareVersion.Items.Add(item);
            IsDisableModifyVersionConfig = false;
            item.IsSelected = true;
        }

        /// <summary>
        /// 删除版本
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_VersionDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_SoftwareVersion.SelectedItem != null)
            {
                ComboBox_SoftwareVersion.Items.Remove(ComboBox_SoftwareVersion.SelectedItem);
                ComboBox_SoftwareVersion.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 版本选择切换
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void ComboBox_SoftwareVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsDisableModifyVersionConfig = true;

            if (e.AddedItems.Count != 0)
            {
                if (e.AddedItems[0] is SoftwareConfigVersionItem item)
                {
                    TextBox_VersionDownloadLink.Text = item.DownloadLink;
                    TextBox_VersionConfigLink.Text = item.ConfigLink;
                    TextBox_VersionMajorVersion.Text = item.MajorVersion.ToString();
                    TextBox_VersionMinorVersion.Text = item.MinorVersion.ToString();
                    TextBox_VersionBuildVersion.Text = item.BuildVersion.ToString();
                    TextBox_VersionRevisedVersion.Text = item.RevisedVersion.ToString();
                    foreach (BLZLanguageConfig select in TabControl_LanguageConfig.Items)
                    {
                        EnumLanguage language = (EnumLanguage)select.Tag;
                        if (item.ChangeLog.ContainsKey(language))
                        {
                            select.TextEditor_SoftwareLog.Text = item.ChangeLog[language];
                        }
                        else
                        {
                            select.TextEditor_SoftwareLog.Text = "";
                        }
                        select.TextEditor_SoftwareLog.IsReadOnly = false;
                    }
                    IsDisableModifyVersionConfig = false;
                    return;
                }
            }
            // 没有可选项
            TextBox_VersionDownloadLink.Text = "";
            TextBox_VersionConfigLink.Text = "";
            TextBox_VersionMajorVersion.Text = "";
            TextBox_VersionMinorVersion.Text = "";
            TextBox_VersionBuildVersion.Text = "";
            TextBox_VersionRevisedVersion.Text = "";
            foreach (BLZLanguageConfig select in TabControl_LanguageConfig.Items)
            {
                EnumLanguage language = (EnumLanguage)select.Tag;
                select.TextEditor_SoftwareLog.Text = "";
                select.TextEditor_SoftwareLog.IsReadOnly = true;
            }
            IsDisableModifyVersionConfig = false;
        }

        /// <summary>
        /// 点击生成按钮
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_Generate_Click(object sender, RoutedEventArgs e)
        {
            BLZ_SoftwareConfigDataSet softwareData = new BLZ_SoftwareConfigDataSet();
            BLZ_EmailAndMapConfigDataSet emailData = new BLZ_EmailAndMapConfigDataSet();
            if (GetConfigFromUI(softwareData, emailData))
            {
                softwareData.DataSetSerializerCompression(VersionCfgFileName);
                FileInfo cfg;
                cfg = EmailInfoConfigFile;
                if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
                {
                    cfg = new FileInfo(cfg.DirectoryName + "\\"  + BLZEnforcement_BankEmailTool.Const_FilePrefix + item.VersionString + ".cfg");
                }
                emailData.DataSetSerializerCompression(cfg);
                MessageBox.Show("生成成功！", "通知");
            }
            else
            {
                MessageBox.Show("保存配置失败！", "错误");
            }
        }

        /// <summary>
        /// 关闭软件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("是否保存当前的UI的数据？如果没有使用-url参数加载在线配置，则会在下次启动软件时自动加载。", "确定", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No) return;
            BLZ_SoftwareConfigDataSet softwareData = new BLZ_SoftwareConfigDataSet();
            BLZ_EmailAndMapConfigDataSet emailData = new BLZ_EmailAndMapConfigDataSet();
            if (GetConfigFromUI(softwareData, emailData))
            {
                softwareData.WriteXml(VersionInfoConfigFile.FullName);
                emailData.WriteXml(EmailInfoConfigFile.FullName);
            }
            else
            {
                MessageBox.Show("保存配置失败！", "错误");
            }
        }

        /// <summary>
        /// 日志变化事件 CN
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_zhCN_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if(ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.zhCN, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 TW
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_zhTW_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.zhTW, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 US
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_enUS_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.enUS, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 DE
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_deDE_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.deDE, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 MX
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_esMX_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.esMX, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 ES
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_esES_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.esES, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 FR
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_frFR_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.frFR, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 IT
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_itIT_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.itIT, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 PL
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_plPL_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.plPL, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 BR
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_ptBR_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.ptBR, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 RU
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_ruRU_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.ruRU, editor.Text);
                }
            }
        }

        /// <summary>
        /// 日志变化事件 KR
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void LanguageInfo_koKR_VersionLogTextChangeHandler(object sender, EventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                if (sender is TextEditor editor)
                {
                    item.SetChangeLog(EnumLanguage.koKR, editor.Text);
                }
            }
        }

        /// <summary>
        /// 下载地址变化
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void TextBox_VersionDownloadLink_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsDisableModifyVersionConfig) return;
            if (ComboBox_SoftwareVersion.SelectedItem is SoftwareConfigVersionItem item)
            {
                item.SetSoftwareDownLoadLinke(TextBox_VersionDownloadLink.Text);
            }
        }

        #endregion
    }
}
