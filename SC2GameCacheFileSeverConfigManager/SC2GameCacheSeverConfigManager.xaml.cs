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
using WH_CommonControlLibrary.Functionality.MultiLanguage;
using System.IO;
using System.Globalization;

using EnumLanguage = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumLanguage;
using EnumGameRegion = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumGameRegion;
using System.Diagnostics;
using SC2OnLineConfigDataSetStruct;
using Newtonsoft.Json.Linq;
using ICSharpCode.AvalonEdit;
using System.Data;
using System.Net;

namespace BLZEnforcementToolset
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SC2GameCacheSeverConfigManager : Window
    {
        #region 属性字段
        #region 属性
        /// <summary>
        /// 实例
        /// </summary>
        public static SC2GameCacheSeverConfigManager Instance { private set; get; }
        /// <summary>
        /// 语言字典
        /// </summary>
        public static Dictionary<EnumLanguage, ResourceDictionary> DictUILanguages { set; get; }

        /// <summary>
        /// 语言依赖项属性
        /// </summary>
        public static DependencyProperty EnumCurrentLanguageProperty = DependencyProperty.Register("EnumCurrentLanguage", typeof(EnumLanguage), typeof(SC2GameCacheSeverConfigManager));

        /// <summary>
        /// 当前语言依赖项
        /// </summary>
        public EnumLanguage EnumCurrentLanguage
        {
            set
            {
                SetValue(EnumCurrentLanguageProperty, value);
                ResourceDictionary_WindowLanguage.MergedDictionaries.Clear();
                CurrentLanguage = DictUILanguages[value];
                ResourceDictionary_WindowLanguage.MergedDictionaries.Add(CurrentLanguage);
                RegionInfoConfig_RegionNorthAmerica.EnumCurrentLanguage = value;
                RegionInfoConfig_RegionEuropeAndRussia.EnumCurrentLanguage = value;
                RegionInfoConfig_RegionKoreaAndTaiWan.EnumCurrentLanguage = value;
                RegionInfoConfig_RegionChina.EnumCurrentLanguage = value;
            }
            get
            {
                return ((EnumLanguage)GetValue(EnumCurrentLanguageProperty));
            }
        }
        /// <summary>
        /// 只读依赖项
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(SC2GameCacheSeverConfigManager));
        /// <summary>
        /// 只读依赖项属性
        /// </summary>
        public bool IsReadOnly
        {
            set
            {
                SetValue(IsReadOnlyProperty, value);
                SetReadOnly(value);
            }
            get
            {
                return (bool) GetValue(IsReadOnlyProperty);
            }
        }
        #endregion

        #region 字段
        private ResourceDictionary CurrentLanguage;
        private Dictionary<object, EnumLanguage> DictComboBoxItemLanguage = new Dictionary<object, EnumLanguage>();
        private readonly FileInfo ToolInfoConfigFile = new FileInfo("SC2GameCacheSeverConfigManager.cfg");
        #endregion
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SC2GameCacheSeverConfigManager()
        {
            InitializeComponent();
            DictUILanguages = new Dictionary<EnumLanguage, ResourceDictionary>();
            Instance = this;
            bool useDefault = true;
            #region 多语言配置
            foreach (EnumLanguage select in Enum.GetValues(typeof(EnumLanguage)))
            {
                string languageName = Enum.GetName(typeof(EnumLanguage), select);
                string fileName = "Language\\" + languageName + ".xaml";
                FileInfo file = new FileInfo(fileName);
                ResourceDictionary language = new ResourceDictionary();
                if (file.Exists)
                {
                    language.Source = new Uri(file.FullName);
                }
                else
                {
                    switch (select)
                    {
                        case EnumLanguage.zhCN:
                            language.Source = new Uri("pack://application:,,,/" + fileName);
                            break;
                        //case EnumLanguage.zhTW:
                        //    language.Source = new Uri("pack://application:,,,/" + fileName);
                        //    break;
                        case EnumLanguage.enUS:
                            language.Source = new Uri("pack://application:,,,/" + fileName);
                            break;
                        default:
                            continue;
                    }
                }
                string itemName = language["LanguageName"] as string;
                DictComboBoxItemLanguage.Add(itemName, select);
                DictUILanguages.Add(select, language);
                ComboBox_MultiLanguage.Items.Add(itemName);
                if (CultureInfo.CurrentCulture.LCID == (int)select)
                {
                    ComboBox_MultiLanguage.SelectedItem = itemName;
                    useDefault = false;
                }
            }
            if (useDefault)
            {
                ComboBox_MultiLanguage.SelectedItem = DictUILanguages[EnumLanguage.enUS]["LanguageName"];
            }
            #endregion
            #region MyRegion
#if !DEBUG
            try
#endif
            {
                SC2_MapInfoDataSet mapInfo = null;
                string[] pargs = Environment.GetCommandLineArgs();
                if (pargs.Count() > 2 && pargs[1].ToLower() == "-url")
                {
                    mapInfo = SC2_MapInfoDataSet.DataSetDeserializeDecompress(pargs[2]);
                }
                else if (File.Exists(System.IO.Path.GetFileNameWithoutExtension(ToolInfoConfigFile.FullName) + ".xml"))
                {
                    mapInfo = new SC2_MapInfoDataSet();
                    mapInfo.ReadXml(System.IO.Path.GetFileNameWithoutExtension(ToolInfoConfigFile.FullName) + ".xml");
                }
                if (mapInfo != null)
                {
                    RestoreUIFromConfig(mapInfo);
                }
            }
#if !DEBUG
            catch (Exception err)
            {
                MessageBox.Show(CurrentLanguage["MSG_LoadUIConfigFailed_Text"] + "\n" + err.Message, CurrentLanguage["MSG_LoadUIConfigFailed_Caption"].ToString(), MessageBoxButton.OK);
            }
#endif
            #endregion
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configUrl">配置文件</param>
        public SC2GameCacheSeverConfigManager(string configUrl)
        {
            InitializeComponent();
            IsReadOnly = true;

            DictUILanguages = new Dictionary<EnumLanguage, ResourceDictionary>();
            Instance = this;
            bool useDefault = true;
            #region 多语言配置
            foreach (EnumLanguage select in Enum.GetValues(typeof(EnumLanguage)))
            {
                string languageName = Enum.GetName(typeof(EnumLanguage), select);
                string fileName = "Language\\" + languageName + ".xaml";
                FileInfo file = new FileInfo(fileName);
                ResourceDictionary language = new ResourceDictionary();
                if (file.Exists)
                {
                    language.Source = new Uri(file.FullName);
                }
                else
                {
                    switch (select)
                    {
                        case EnumLanguage.zhCN:
                            language.Source = new Uri("pack://application:,,,/SC2GameCacheSeverConfigManager;component/" + fileName);
                            break;
                        //case EnumLanguage.zhTW:
                        //    language.Source = new Uri("pack://application:,,,/SC2GameCacheSeverConfigManager;" + fileName);
                        //    break;
                        case EnumLanguage.enUS:
                            language.Source = new Uri("pack://application:,,,/SC2GameCacheSeverConfigManager;component/" + fileName);
                            break;
                        default:
                            continue;
                    }
                }
                string itemName = language["LanguageName"] as string;
                DictComboBoxItemLanguage.Add(itemName, select);
                DictUILanguages.Add(select, language);
                ComboBox_MultiLanguage.Items.Add(itemName);
                if (CultureInfo.CurrentCulture.LCID == (int)select)
                {
                    ComboBox_MultiLanguage.SelectedItem = itemName;
                    useDefault = false;
                }
            }
            if (useDefault)
            {
                ComboBox_MultiLanguage.SelectedItem = DictUILanguages[EnumLanguage.enUS]["LanguageName"];
            }
            #endregion
            #region MyRegion
#if !DEBUG
            try
#endif
            {
                SC2_MapInfoDataSet mapInfo = SC2_MapInfoDataSet.DataSetDeserializeDecompress(configUrl);
                if (mapInfo != null)
                {
                    RestoreUIFromConfig(mapInfo);
                }
            }
#if !DEBUG
            catch (Exception err)
            {
                MessageBox.Show(CurrentLanguage["MSG_LoadUIConfigFailed_Text"] + "\n" + err.Message, CurrentLanguage["MSG_LoadUIConfigFailed_Caption"].ToString(), MessageBoxButton.OK);
            }
#endif
            #endregion
        }
        #endregion

        #region 方法

        /// <summary>
        /// 设置只读
        /// </summary>
        /// <param name="isReadOnly">只读</param>
        private void SetReadOnly(bool isReadOnly)
        {
            foreach (CacheRegionInfoConfig region in TabControl_RegionConfig.Items)
            {
                region.TextBox_SatrtLink.IsReadOnly = isReadOnly;
                region.SelectPatchControl_ReplayPath.IsReadOnly = isReadOnly;
                region.Button_CleanAllChangeLog.IsEnabled = !isReadOnly;
                foreach (CacheChangeLog lang in region.TabControl_ChangeLogWithLanguage.Items)
                {
                    lang.TextBox_GameName.IsReadOnly = isReadOnly;
                    lang.TextEditor_ChangLog.IsReadOnly = isReadOnly;
                }
            }
            TextEditor_Log.IsReadOnly = isReadOnly;
            Button_Generation.IsEnabled = !isReadOnly;
            Button_UpdateReplayLib.IsEnabled = !isReadOnly;
        }

        /// <summary>
        /// 新增日志信息
        /// </summary>
        /// <param name="msg">消息</param>
        private void AddLogMsg(string operate, string msg)
        {
            TextEditor_Log.Text = "[" + operate + "]:\r\n" + msg + "\r\n" + TextEditor_Log.Text;
        }

        /// <summary>
        /// 从DataSet还原UI
        /// </summary>
        /// <param name="dataSet">DataSet</param>
        private void RestoreUIFromConfig(SC2_MapInfoDataSet dataSet)
        {
            DataTable regionConfig = dataSet.Tables[SC2_MapInfoDataSet.TableName_GameRegionConfig];
            DataTable languageConfig = dataSet.Tables[SC2_MapInfoDataSet.TableName_LanguageConfig];

            foreach (CacheRegionInfoConfig select in TabControl_RegionConfig.Items)
            {
                EnumGameRegion region = (EnumGameRegion)select.Tag;
                DataRow regionRow = regionConfig.Rows.Find(region);
                if (regionRow == null) continue;
                select.SelectPatchControl_ReplayPath.PathText = regionRow[SC2_MapInfoDataSet.ColumnName_GameRegion_FilePath] as string;
                select.TextBox_SatrtLink.Text = regionRow[SC2_MapInfoDataSet.ColumnName_GameRegion_StartGameLink] as string;
                List<DataRow> changeLogList = regionRow.GetChildRows(SC2_MapInfoDataSet.RelationName_ChangeLogRegionIndex).ToList();
                foreach (CacheChangeLog item in select.TabControl_ChangeLogWithLanguage.Items)
                {
                    EnumLanguage language = (EnumLanguage)item.Tag;
                    List<DataRow> changeLogRowList = changeLogList.Where(r => (EnumLanguage)r[SC2_MapInfoDataSet.ColumnName_LanguageConifg_LanguageId] == language).ToList();
                    if (changeLogRowList.Count == 0) continue;
                    DataRow changLogRow = changeLogRowList.First();
                    if (changLogRow == null) continue;
                    item.TextBox_GameName.Text = changLogRow[SC2_MapInfoDataSet.ColumnName_LanguageConifg_GameName] as string;
                    item.TextEditor_ChangLog.Text = changLogRow[SC2_MapInfoDataSet.ColumnName_LanguageConifg_ChangeLog] as string;
                }
            }
        }
        
        /// <summary>
        /// 从UI获取配置
        /// </summary>
        /// <param name="isGen">是否生成录像数据</param>
        /// <returns>生成数据表</returns>
        private SC2_MapInfoDataSet GetConfigFromUI(bool isGen)
        {
            Process process = new Process();//创建进程对象
            SC2_MapInfoDataSet mapInfo = new SC2_MapInfoDataSet();
#if !DEBUG
            try
#endif
            {
                foreach (CacheRegionInfoConfig select in TabControl_RegionConfig.Items)
                {
                    EnumGameRegion region = (EnumGameRegion)select.Tag;
                    mapInfo.AddNewRegion(region, select.TextBox_SatrtLink.Text, isGen ? "" : select.SelectPatchControl_ReplayPath.PathText);
                    if (isGen)
                    {
                        if (isGen && !File.Exists(select.SelectPatchControl_ReplayPath.PathText)) continue;
                        FileInfo replay = new FileInfo(select.SelectPatchControl_ReplayPath.PathText);
                        if (!replay.Exists)
                        {
                            MessageBox.Show(CurrentLanguage["MSG_ReplayFileNotExist_Text"] + "\n" + replay.FullName, CurrentLanguage["MSG_ReplayFileNotExist_Caption"].ToString(), MessageBoxButton.OK);
                            return null;
                        }

                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                        process.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                        process.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                        process.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                        process.StartInfo.CreateNoWindow = true;//不显示程序窗口
                        process.Start();//启动程序

                        //向cmd窗口发送输入信息
                        string msg = "py -2 SC2ReplayDetails.py \"" + replay.FullName + "\"& exit";
                        process.StandardInput.WriteLine(msg);

                        process.StandardInput.AutoFlush = true;

                        // 跳过无用的内容
                        AddLogMsg(Button_Generation.Content as string, process.StandardOutput.ReadLine());
                        AddLogMsg(Button_Generation.Content as string, process.StandardOutput.ReadLine());
                        AddLogMsg(Button_Generation.Content as string, process.StandardOutput.ReadLine());
                        AddLogMsg(Button_Generation.Content as string, process.StandardOutput.ReadLine());
                        string details = "";
                        while (!process.StandardOutput.EndOfStream)
                        {
                            string line = process.StandardOutput.ReadLine();
                            details += line + "\r\n";
                            if (line.Contains("]"))
                            {
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(details))
                        {
                            continue;
                        }

                        details += "}";
                        AddLogMsg(Button_Generation.Content as string, details);
                        JObject json = JObject.Parse(details);
                        if (json != null)
                        {
                            if (json["m_cacheHandles"] is JArray cacheHandles)
                            {
                                foreach (var cache in cacheHandles)
                                {
                                    if (cache is JToken cacheToken)
                                    {
                                        string url = cacheToken.ToString();
                                        if (!mapInfo.AddNewCacheFile(region, url, CurrentLanguage))
                                        {
                                            MessageBox.Show(CurrentLanguage["MSG_CacheFileNotExist_Text"] + "\n" + url, CurrentLanguage["MSG_CacheFileNotExist_Caption"].ToString(), MessageBoxButton.OK);
                                            return null;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(CurrentLanguage["MSG_AnalysiReplayFailed_Text"] + "\n" + replay.FullName, CurrentLanguage["MSG_AnalysiReplayFailed_Caption"].ToString(), MessageBoxButton.OK);
                            return null;
                        }
                    }
                    foreach (CacheChangeLog item in select.TabControl_ChangeLogWithLanguage.Items)
                    {
                        EnumLanguage language = (EnumLanguage)item.Tag;
                        mapInfo.AddNewLanguage(mapInfo.Tables[SC2_MapInfoDataSet.TableName_LanguageConfig].Rows.Count, language, region, item.TextBox_GameName.Text, item.TextEditor_ChangLog.Text);
                    }
                }
            }
#if !DEBUG
            catch (Exception err)
            {
                if (process != null)
                    process.Close();
                MessageBox.Show(CurrentLanguage["MSG_GenerateFailed_Text"] + "\n" + err.Message, CurrentLanguage["MSG_GenerateFailed_Caption"].ToString(), MessageBoxButton.OK);
                return null;
            }
#endif
            return mapInfo;
        }
        #endregion

        #region 控件事件
        /// <summary>
        /// 切换语言事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void ComboBox_MultiLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string itemName = ComboBox_MultiLanguage.SelectedItem as string;
            EnumCurrentLanguage = DictComboBoxItemLanguage[itemName];
        }

        /// <summary>
        /// 生成按钮点击事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_Generation_Click(object sender, RoutedEventArgs e)
        {
            SC2_MapInfoDataSet mapInfo = GetConfigFromUI(true);
            if (mapInfo != null)
            {
                mapInfo.DataSetSerializerCompression(ToolInfoConfigFile);
                MessageBox.Show(CurrentLanguage["MSG_GenerateSuccess_Text"].ToString(), CurrentLanguage["MSG_GenerateSuccess_Caption"].ToString(), MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 关闭软件事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Window_SC2GameCacheSeverConfigManager_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(CurrentLanguage["MSG_ClosingSaveConfig_Text"].ToString(), CurrentLanguage["MSG_ClosingSaveConfig_Caption"].ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No) return;
            SC2_MapInfoDataSet mapInfo = GetConfigFromUI(false);
            if (mapInfo != null)
            {
                mapInfo.WriteXml(System.IO.Path.GetFileNameWithoutExtension(ToolInfoConfigFile.FullName) + ".xml");
            }
        }
        /// <summary>
        /// 更新SC2录像Python库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_UpdateReplayLib_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();//创建进程对象
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            process.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            process.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            process.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            process.StartInfo.CreateNoWindow = true;//不显示程序窗口
            process.Start();//启动程序

            //向cmd窗口发送输入信息
            process.StandardInput.WriteLine("py -2 -m pip install s2protocol");
            process.StandardInput.WriteLine("py -2 -m pip install --upgrade s2protocol&exit");
            string details = process.StandardOutput.ReadToEnd();
            process.StandardInput.AutoFlush = true;
            AddLogMsg(Button_UpdateReplayLib.Content as string, details);
        }
        #endregion

    }
}
