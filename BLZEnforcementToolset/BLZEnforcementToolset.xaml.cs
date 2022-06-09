using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Data;
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
using System.IO.Compression;
using System.Xml.Linq;
using System.Reflection;
using System.Net.Mail;
using System.Net;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using BLZEnforcementToolset.BLZControl;
using SC2OnLineConfigDataSetStruct;

using EnumLanguage = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumLanguage;
using EnumGameRegion = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumGameRegion;
using BLZOnLineConfigDataSetStruct;

namespace BLZEnforcementToolset
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BLZEnforcement_BankEmailTool : Window
    {
        #region 声明
        /// <summary>
        /// 配置类
        /// </summary>
        private class ToolConfig
        {
            #region 常量
            private const string ConfigElement = "Config";
            private const string ConfigEmail = "SendEmail";
            private const string ConfigCharacter = "SelectCharacter";
            private const string ConfigRegion = "SelectRegion";
            #endregion

            #region 属性

            /// <summary>
            /// 发送邮箱
            /// </summary>
            public string SendEmail { set; get; }
            /// <summary>
            /// 选择角色
            /// </summary>
            public string SelectCharacter { set; get; }

            /// <summary>
            /// 选择分区
            /// </summary>
            public EnumGameRegion SelectRegion { set; get; }

            /// <summary>
            /// 实例
            /// </summary>
            public static ToolConfig Instance { private set; get; }

            #endregion

            #region 构造函数

            /// <summary>
            /// 静态构造函数
            /// </summary>
            static ToolConfig()
            {
                Instance = new ToolConfig();
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            public ToolConfig()
            {
                SendEmail = "";
                SelectCharacter = "";
                SelectRegion = EnumGameRegion.China;

                LoadConfig();
            }
            #endregion

            #region 方法
            /// <summary>
            /// 读取配置
            /// </summary>
            public void LoadConfig()
            {
                if (!System.IO.File.Exists(Const_ToolConfigFileName))
                {
                    return;
                }
                try
                {
                    XDocument cfg = XDocument.Load(Const_ToolConfigFileName);
                    SendEmail = cfg.Root.Attribute(ConfigEmail).Value;
                    SelectCharacter = cfg.Root.Attribute(ConfigCharacter).Value;
                    SelectRegion = (SC2_MapInfoDataSet.EnumGameRegion)Enum.Parse(typeof(SC2_MapInfoDataSet.EnumGameRegion), cfg.Root.Attribute(ConfigRegion).Value);
                }
                catch
                {
                    SendEmail = "";
                    SelectCharacter = "";
                }
            }

            public void SaveConfig()
            {
                XDocument cfg = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(ConfigElement,
                    new XAttribute(ConfigEmail, SendEmail),
                    new XAttribute(ConfigCharacter, SelectCharacter),
                    new XAttribute(ConfigRegion, Enum.GetName(typeof(SC2_MapInfoDataSet.EnumGameRegion), SelectRegion))
                        )
                    );
                cfg.Save(Const_ToolConfigFileName, SaveOptions.None);
            }
            #endregion
        }

        /// <summary>
        /// 发送邮件参数
        /// </summary>
        private class SendMailArgus
        {
            #region 属性
            /// <summary>
            /// 发件人
            /// </summary>
            public string From { set; get; }
            /// <summary>
            /// 收件人
            /// </summary>
            public string To { set; get; }
            /// <summary>
            /// 主题
            /// </summary>
            public string Subject { set; get; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Body { set; get; }
            /// <summary>
            /// 使用Html
            /// </summary>
            public bool IsHtml { set; get; }
            /// <summary>
            /// SMTP服务器
            /// </summary>
            public string Host { set; get; }
            /// <summary>
            /// SMTP服务器端口
            /// </summary>
            public int Port { set; get; }
            /// <summary>
            /// 发信账号
            /// </summary>
            public string User { set; get; }
            /// <summary>
            /// 发信密码
            /// </summary>
            public string Password { set; get; }
            /// <summary>
            /// 存档目录
            /// </summary>
            public DirectoryInfo BankDir { set; get; }
            /// <summary>
            /// 角色快捷方式
            /// </summary>
            public string CharachterLnkPath { set; get; }

            public string BankFilePrefix { get; set; }
            #endregion

            #region 构造函数
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="sendTo">收件人</param>
            /// <param name="title">邮件主题</param>
            public SendMailArgus(string sendTo, string lnkPath, DirectoryInfo bankDir, DataRow regionConfig, DataRow languageConfig, string bankFilePrefix)
            {
                From = regionConfig[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_EmailSmtpFrom] as string;
                To = sendTo;
                Subject = languageConfig[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailLanguageConfig_EmailSubject] as string;
                Body = languageConfig[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailLanguageConfig_EmailBody] as string;
                IsHtml = (bool)languageConfig[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailLanguageConfig_IsBodyHtml];
                Host = regionConfig[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_EmailSmtpHost] as string;
                Port = (int)regionConfig[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_EmailSmtpPort];
                User = regionConfig[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_EmailSmtpUser] as string;
                Password = regionConfig[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_EmailSmtpPassword] as string;
                CharachterLnkPath = lnkPath;
                BankDir = bankDir;
                BankFilePrefix = bankFilePrefix;
            }
            #endregion

            #region 方法

            #endregion
        }

        /// <summary>
        /// 文件缓存
        /// </summary>
        private class CacheFile
        {
            #region 属性
            /// <summary>
            /// 下载地址
            /// </summary>
            public string URL { private set; get; }
            /// <summary>
            /// 文件路径
            /// </summary>
            public string Path { private set; get; }
            /// <summary>
            /// 效验SHA
            /// </summary>
            public Byte[] SHA { private set; get; }
            /// <summary>
            /// 文件大小
            /// </summary>
            public long Size { private set; get; }
            /// <summary>
            /// 下载结果
            /// </summary>
            public bool IsSuccess { set; get; }
            /// <summary>
            /// 下载错误信息
            /// </summary>
            public string ErrorMsg { set; get; }
            #endregion

            #region 构造函数
            public CacheFile(DataRow row)
            {
                URL = row[SC2_MapInfoDataSet.ColumnName_CacheFile_URL] as string;
                Path = row[SC2_MapInfoDataSet.ColumnName_CacheFile_Path] as string;
                SHA = row[SC2_MapInfoDataSet.ColumnName_CacheFile_SHA] as Byte[];
                Size = (long)row[SC2_MapInfoDataSet.ColumnName_CacheFile_FileSize];
                IsSuccess = false;
                ErrorMsg = "";
            }
            #endregion

            #region 方法
            /// <summary>
            /// 验证是否需要下载
            /// </summary>
            /// <returns>是否需要下载</returns>
            public bool IsNeedDownload()
            {
                try
                {
                    FileInfo file = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path);
                    if (!file.Exists)
                    {
                        return true;
                    }
                    FileStream fs = new FileStream(file.FullName, FileMode.Open);
                    System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                    Byte[] shaBytes = sha.ComputeHash(fs);
                    fs.Close();
                    return !shaBytes.SequenceEqual(SHA);
                }
                catch
                {
                    return false;
                }
            }
            #endregion
        }

        /// <summary>
        /// 加载在线配置步骤
        /// </summary>
        public enum EnumLoadingOnlineConfigStep
        {
            Init,
            Email,
            Version,
            Maps,
        }
        #endregion

        #region 常量
        private const string Const_DataDir = "Data";
        private const string Const_BankDir = "Bank";
        private const string Const_BankExtension = ".SC2Bank";
        private const string Const_ToolConfigFileName = "Config.cfg";
        private const string Const_AttachmentFileName = "BLZEnforcementToolset.zip";
        private const string Const_NotExitLangUri = "pack://application:,,,/Language/enUS.xaml";
        private const int Const_ModeSwitchHight_On = 484;
        private const int Const_ModeSwitchHight_Off = 521;
#if DEBUG
        public const string Const_BaseAddress = "http://froggyandcatty.com/sc2software/d/";
#else
        public const string Const_BaseAddress = "http://froggyandcatty.com/sc2software/r/";
#endif
        public const string Const_ConfigFolder = "Config/";
        public const string Const_FilePrefix = "GCABLZ_";
        public const string Const_VersionInfo = "VersionInfo.cfg";
        #endregion

        #region 字段属性

        #region 字段
        private ResourceDictionary CurrentLanguage;
        private readonly Regex EmailRegex = new Regex(@"^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", RegexOptions.Compiled);

        private Dictionary<object, EnumLanguage> DictComboBoxItemLanguage = new Dictionary<object, EnumLanguage>();
        private Dictionary<object, Dictionary<object, string>> DictMultiLanguageText = new Dictionary<object, Dictionary<object, string>>();
        private Dictionary<object, string> DictCharacterItemToPath = new Dictionary<object, string>();
        private Dictionary<EnumGameRegion, string> DictAuthorHandle = new Dictionary<EnumGameRegion, string>();
        private Dictionary<EnumGameRegion, string> DictBankName = new Dictionary<EnumGameRegion, string>();

        #endregion

        #region 属性

        /// <summary>
        /// 版本配置地址
        /// </summary>
        public string VersionConfigLink { get; } = Const_BaseAddress + Const_VersionInfo;

        /// <summary>
        /// 主窗体实例
        /// </summary>
        public static BLZEnforcement_BankEmailTool Instance { private set; get; }

        /// <summary>
        /// 地图配置信息
        /// </summary>
        public Dictionary<string, SC2_MapInfoDataSet> MapConfigInfos { private set; get; }

        public SC2_MapInfoDataSet BLZMapConfigInfo { private set; get; }

        /// <summary>
        /// 邮箱配置数据
        /// </summary>
        public BLZ_EmailAndMapConfigDataSet EmailConfigInfo { private set; get; }

        /// <summary>
        /// 软件配置数据
        /// </summary>
        public BLZ_SoftwareConfigDataSet SoftwareConfigInfo { private set; get; }

        /// <summary>
        /// 当前版本配置数据
        /// </summary>
        public DataRow CurrrentVersionConfigData { private set; get; }

        /// <summary>
        /// 版本
        /// </summary>
        public static string SofteareVersion { private set; get; }

        /// <summary>
        /// 字体
        /// </summary>
        public static ResourceDictionary BLZFont { private set; get; }

        /// <summary>
        /// 语言
        /// </summary>
        public static Dictionary<EnumLanguage, ResourceDictionary> DictUILanguages { private set; get; }

        /// <summary>
        /// 加载在线配置步骤
        /// </summary>
        public static EnumLoadingOnlineConfigStep LodingStem { private set; get; }

        #endregion

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZEnforcement_BankEmailTool()
        {
            LodingStem = EnumLoadingOnlineConfigStep.Init;
            MapConfigInfos = null;
            Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            SofteareVersion = "V" + ver.Major + "." + ver.Minor;
            BLZFont = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/Themes/enUSFonts.xaml"),
            };
            DictUILanguages = new Dictionary<EnumLanguage, ResourceDictionary>();
            Instance = this;
            InitializeComponent();
            bool useDefault = true;
            List<string> noSupportLang = new List<string>();
            foreach (EnumLanguage select in Enum.GetValues(typeof(EnumLanguage)))
            {
                string languageName = Enum.GetName(typeof(EnumLanguage), select);
                string fileName = "Language\\" + languageName + ".xaml";
                FileInfo file = new FileInfo(fileName);
                ResourceDictionary language = new ResourceDictionary();
                string itemName;
                if (file.Exists)
                {
                    language.Source = new Uri(file.FullName);
                    itemName = language["LanguageName"] as string;
                }
                else
                {
                    switch (select)
                    {
                        case EnumLanguage.zhCN:
                            language.Source = new Uri("pack://application:,,,/" + fileName);
                            itemName = language["LanguageName"] as string;
                            break;
                        case EnumLanguage.zhTW:
                            language.Source = new Uri("pack://application:,,,/" + fileName);
                            itemName = language["LanguageName"] as string;
                            break;
                        case EnumLanguage.enUS:
                            language.Source = new Uri("pack://application:,,,/" + fileName);
                            itemName = language["LanguageName"] as string;
                            break;
                        default:
                            language.Source = new Uri(Const_NotExitLangUri);
                            itemName = Enum.GetName(typeof(EnumLanguage), select) + "(NotSupport)";
                            noSupportLang.Add(itemName);
                            break;
                    }
                }
                DictComboBoxItemLanguage.Add(itemName, select);
                DictUILanguages.Add(select, language);
                DictMultiLanguageText.Add(itemName, new Dictionary<object, string>());
                if (!noSupportLang.Contains(itemName)) ComboBox_BLZLanguage.Items.Add(itemName);
                if (CultureInfo.CurrentCulture.LCID == (int)select)
                {
                    ComboBox_BLZLanguage.SelectedItem = itemName;
                    useDefault = false;
                }
            }
            foreach (string select in noSupportLang)
            {
                ComboBox_BLZLanguage.Items.Add(select);
            }
            if (useDefault)
            {
                ComboBox_BLZLanguage.SelectedItem = DictUILanguages[EnumLanguage.enUS]["LanguageName"];
            }
            foreach (ComboBoxItem select in ComboBox_BLZLocation.Items)
            {
                EnumGameRegion region = (EnumGameRegion)select.Tag;
                if (region == ToolConfig.Instance.SelectRegion)
                {
                    ComboBox_BLZLocation.SelectedItem = select;
                }
            }
            SetMultiLanguageText(TextBlock_BLZDownloadInfo, "UI_BLZTextBlock_DownloadInfoDefault_Text");
            SetMultiLanguageText(TextBlock_BLZInfoPannelText, "UI_BLZTextBlock_InfoPannelDefault_Text");
            TextBox_BLZEmail.Text = ToolConfig.Instance.SendEmail;
            RefreshMultiLanguageText(ComboBox_BLZLanguage.SelectedItem);
            LoadOnlineConfigStart();
        }
        #endregion

        #region 方法

        #region 通用        

        /// <summary>
        /// 获取版本字符串
        /// </summary>
        /// <param name="major">主要版本号</param>
        /// <param name="minor">次要版本号</param>
        /// <param name="build">编译版本号</param>
        /// <param name="revised">修订版本号</param>
        /// <returns>版本字符串</returns>
        public static string GetVersionString(int major, int minor, int build, int revised)
        {
            return "V" + major + "." + minor + "." + build + "." + revised;
        }

        /// <summary>
        /// 获取当前版本字符串
        /// </summary>
        /// <returns>版本字符串</returns>
        public string GetVersionString()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return GetVersionString(version.Major, version.Minor, version.Build, version.Revision);
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveConfig()
        {
            ToolConfig.Instance.SendEmail = TextBox_BLZEmail.Text;
            ToolConfig.Instance.SelectRegion = (SC2_MapInfoDataSet.EnumGameRegion)(ComboBox_BLZLocation.SelectedItem as ComboBoxItem).Tag;
            if (!(ComboBox_BLZAccount.SelectedItem is ComboBoxItem))
            {
                ToolConfig.Instance.SelectCharacter = "";
            }
            else
            {
                ToolConfig.Instance.SelectCharacter = (ComboBox_BLZAccount.SelectedItem as ComboBoxItem).Content.ToString();
            }
            ToolConfig.Instance.SaveConfig();
        }

        /// <summary>
        /// 是否新软件版本
        /// </summary>
        /// <param name="select">当前</param>
        /// <param name="compare">对比</param>
        /// <returns>比较结果</returns>
        private bool IsNewerSoftwareVersion(DataRow select, DataRow compare)
        {
            int selectVer;
            int compareVer;
            selectVer = (int)select[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_MajorVersion];
            compareVer = (int)compare[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_MajorVersion];
            if (selectVer == compareVer)
            {
                selectVer = (int)select[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_MinorVersion];
                compareVer = (int)compare[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_MinorVersion];
                if (selectVer == compareVer)
                {
                    selectVer = (int)select[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_BuildVersion];
                    compareVer = (int)compare[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_BuildVersion];
                    if (selectVer == compareVer)
                    {
                        selectVer = (int)select[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_RevisedVersion];
                        compareVer = (int)compare[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_RevisedVersion];
                    }
                }
            }
            return selectVer > compareVer;
        }

        #endregion

        #region 存档邮件

        /// <summary>
        /// 通过角色快捷方式获取存档路径
        /// </summary>
        /// <param name="path">角色快捷方式路径</param>
        /// <returns>存档目录</returns>
        private DirectoryInfo GetBankDirectoryFormCharacterLnk(string path)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);
            EnumGameRegion region = (EnumGameRegion)(ComboBox_BLZLocation.SelectedItem as ComboBoxItem).Tag;
            return new DirectoryInfo(shortcut.TargetPath + "\\Banks\\" + DictAuthorHandle[region]);

        }

        /// <summary>
        /// 读取星际争霸2用户
        /// </summary>
        private void LoadSC2User()
        {
            if (MapConfigInfos == null) return;
            ComboBox_BLZAccount.Items.Clear();
            DictCharacterItemToPath.Clear();
            DirectoryInfo docFolder = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\StarCraft II");
            if (docFolder == null)
            {
                BLZ_Window.Show(CurrentLanguage["MSG_FindSC2FolderFailed_Text"].ToString(), CurrentLanguage["MSG_FindSC2FolderFailed_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            List<FileInfo> charactersLink = docFolder.GetFiles("*.lnk").ToList();
            if (charactersLink.Count == 0)
            {
                BLZ_Window.Show(CurrentLanguage["MSG_FindCharactersFailed_Text"].ToString(), CurrentLanguage["MSG_FindCharactersFailed_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            WshShell shell = new WshShell();
            bool matchCharacter = false;
            foreach (FileInfo select in charactersLink)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(select.FullName);
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(select.FullName);
                EnumGameRegion region = (EnumGameRegion)(ComboBox_BLZLocation.SelectedItem as ComboBoxItem).Tag;
                string path = shortcut.TargetPath + "\\Banks\\" + DictAuthorHandle[region];
                if (int.Parse(System.IO.Path.GetFileNameWithoutExtension(path).Substring(0, 1)) != (int)(region))
                {
                    continue;
                }
                if (!(Directory.Exists(path) || System.IO.File.Exists(path + "\\" + DictBankName[region] + Const_BankExtension) || System.IO.File.Exists(path + DictBankName[region] + "PTR" + Const_BankExtension))) continue;
                ComboBox_BLZAccount.Items.Add(name);
                DictCharacterItemToPath[name] = select.FullName;
                if (name == ToolConfig.Instance.SelectCharacter)
                {
                    matchCharacter = true;
                    ComboBox_BLZAccount.SelectedItem = select.FullName;
                }
            }
            CheckSendEmailButtonEnable();
            if (!matchCharacter) ComboBox_BLZAccount.SelectedIndex = 0;
        }

        /// <summary>
        /// 检测是否允许发送邮件按钮
        /// </summary>
        void CheckSendEmailButtonEnable()
        {
            Button_BLZSendMail.IsEnabled = MapConfigInfos != null && ComboBox_BLZAccount.Items.Count != 0 && EmailRegex.IsMatch(TextBox_BLZEmail.Text);
        }

        /// <summary>   
        /// 压缩文件   
        /// </summary>   
        /// <param name="fileNames">要打包的文件列表</param>  
        /// <param name="basePatch">压缩基础目录</param>    
        /// <param name="GzipFileName">目标文件名</param>   
        /// <param name="CompressionLevel">压缩品质级别（0~9）</param>   
        private static void Compress(List<FileInfo> fileNames, string basePatch, string zipFileName, int compressionLevel)
        {
            string replacePath;
            if (basePatch.Last() == '\\')
            {
                replacePath = basePatch;
            }
            else
            {
                replacePath = basePatch + "\\";
            }
            ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(zipFileName));
            try
            {
                s.SetLevel(compressionLevel);   //0 - store only to 9 - means best compression   
                foreach (FileInfo file in fileNames)
                {
                    FileStream fs = null;
                    try
                    {
                        fs = file.Open(FileMode.Open, FileAccess.ReadWrite);
                    }
                    catch
                    {
                        continue;
                    }
                    //  方法二，将文件分批读入缓冲区   
                    byte[] data = new byte[2048];
                    int size = 2048;
                    ZipEntry entry = new ZipEntry(file.FullName.Replace(replacePath, string.Empty))
                    {
                        DateTime = (file.CreationTime > file.LastWriteTime ? file.LastWriteTime : file.CreationTime)
                    };
                    s.PutNextEntry(entry);
                    while (true)
                    {
                        size = fs.Read(data, 0, size);
                        if (size <= 0) break;
                        s.Write(data, 0, size);
                    }
                    fs.Close();
                }
            }
            finally
            {
                s.Finish();
                s.Close();
            }
        }

        /// <summary>
        /// 预处理邮件附件
        /// </summary>
        /// <returns>结果</returns>
        private static bool PrepareEmailAttachment(DirectoryInfo bankDir, string characterLnk, string bankFilePrefix)
        {
            try
            {
                List<FileInfo> compressFiles = new List<FileInfo>();
                string path;

                DirectoryInfo dataDir = new DirectoryInfo(Const_DataDir);
                if (dataDir.Exists)
                {
                    dataDir.Delete(true);
                }
                dataDir.Create();
                DirectoryInfo bakcupDir = new DirectoryInfo(dataDir.FullName + "\\" + Const_BankDir);
                bakcupDir.Create();
                foreach (FileInfo select in bankDir.GetFiles("*" + Const_BankExtension))
                {
                    if (select.Name.StartsWith(bankFilePrefix))
                    {
                        FileInfo bank = new FileInfo(bakcupDir.FullName + select.FullName.Replace(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\StarCraft II", ""));
                        if (!bank.Directory.Exists)
                        {
                            bank.Directory.Create();
                        }
                        select.CopyTo(bank.FullName, true);
                        compressFiles.Add(bank);
                    }
                }
                DirectoryInfo languageDir = new DirectoryInfo(dataDir.FullName + "\\Language");
                foreach (EnumLanguage select in Enum.GetValues(typeof(EnumLanguage)))
                {
                    string languageName = Enum.GetName(typeof(EnumLanguage), select);
                    string fileName = "Language\\" + languageName + ".xaml";
                    FileInfo file = new FileInfo(fileName);
                    if (!file.Exists) continue;
                    if (!languageDir.Exists)
                    {
                        languageDir.Create();
                    }
                    path = languageDir.FullName + "\\" + file.Name;
                    file.CopyTo(path, true);
                    compressFiles.Add(new FileInfo(path));
                }
                FileInfo lnk = new FileInfo(characterLnk);
                path = bakcupDir.FullName + "\\" + lnk.Name;
                lnk.CopyTo(path, true);
                compressFiles.Add(new FileInfo(path));
                FileInfo cfg = new FileInfo(Const_ToolConfigFileName);
                path = dataDir.FullName + "\\" + cfg.Name;
                cfg.CopyTo(path, true);
                compressFiles.Add(new FileInfo(path));
                FileInfo app = new FileInfo(Assembly.GetEntryAssembly().Location);
                path = dataDir.FullName + "\\" + app.Name;
                app.CopyTo(path, true);
                compressFiles.Add(new FileInfo(path));
                FileInfo dll = new FileInfo("ICSharpCode.SharpZipLib.dll");
                path = dataDir.FullName + "\\" + dll.Name;
                dll.CopyTo(path, true);
                compressFiles.Add(new FileInfo(path));
                Compress(compressFiles, dataDir.FullName, Const_AttachmentFileName, 9);
                return true;
            }
            catch
            {
                BLZ_Window.Show(Instance.CurrentLanguage["MSG_PrepareEmailAttachment_Text"].ToString(), Instance.CurrentLanguage["MSG_PrepareEmailAttachment_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <returns>结果</returns>
        private static bool SendEmail(SendMailArgus argu)
        {
            MailAddress fromEmail = new MailAddress(argu.From);
            MailAddress toEmail = new MailAddress(argu.To);
            MailMessage mail = new MailMessage(fromEmail, toEmail);
            mail.Attachments.Add(new Attachment(Const_AttachmentFileName));
            mail.Subject = argu.Subject;
            mail.Body = argu.Body;
            mail.IsBodyHtml = argu.IsHtml;
            mail.BodyEncoding = Encoding.UTF8;
            mail.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient
            {
                Host = argu.Host,
                Port = argu.Port,
                Credentials = new NetworkCredential(argu.User, argu.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };
            try
            {
                client.Send(mail);
                BLZ_Window.Show(Instance.CurrentLanguage["MSG_SendMailSuccess_Text"].ToString(), Instance.CurrentLanguage["MSG_SendMailSuccess_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception e)
            {
                BLZ_Window.Show(Instance.CurrentLanguage["MSG_SendMailFailed_Text"].ToString() + e.Message, Instance.CurrentLanguage["MSG_SendMailFailed_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            finally
            {
                ////释放资源
                mail.Dispose();
            }
        }

        /// <summary>
        /// 启动发送邮件线程
        /// </summary>
        public void SendMailThreadStart()
        {
            //Button_BLZSendMail.IsEnabled = false;
            //Instance.SaveConfig();
            //EnumGameRegion region = (EnumGameRegion)(ComboBox_BLZLocation.SelectedItem as ComboBoxItem).Tag;
            //DataRow regionConfig = MapConfigInfos.Tables[SC2_MapInfoDataSet.TableName_GameRegionConfig].Rows.Find(region);
            //DataRow languageConfig = MapConfigInfos.Tables[SC2_MapInfoDataSet.TableName_LanguageConfig].Rows.Find(DictComboBoxItemLanguage[ComboBox_BLZLanguage.SelectedItem]);
            //Task task = new Task(SendMailThreadDo, new SendMailArgus(TextBox_BLZEmail.Text, DictCharacterItemToPath[ComboBox_BLZAccount.SelectedItem], GetBankDirectoryFormCharacterLnk(DictCharacterItemToPath[ComboBox_BLZAccount.SelectedItem]), regionConfig, languageConfig, DictBankName[region]));
            //task.Start();
        }

        /// <summary>
        /// 发送邮件线程运行
        /// </summary>
        /// <param name="argu">参数</param>   
        public void SendMailThreadDo(object argu)
        {
            SendMailArgus arguData = argu as SendMailArgus;
            if (PrepareEmailAttachment(arguData.BankDir, arguData.CharachterLnkPath, arguData.BankFilePrefix))
            {
                SendEmail(arguData);

            }
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                       (ThreadStart)delegate ()
                       {
                           Button_BLZSendMail.IsEnabled = true;
                       });
        }

        #endregion

        #region 软件配置

        /// <summary>
        /// 加载在线配置
        /// </summary>
        public void LoadOnlineConfigStart()
        {
            Task task = new Task(LoadOnlineConfigDo);
            task.Start();
        }

        /// <summary>
        /// 加载在线配置
        /// </summary>
        public void LoadOnlineConfigDo()
        {
#if Debug
            try
#endif
            {
                // 版本配置
                BLZ_SoftwareConfigDataSet softwareData = BLZ_SoftwareConfigDataSet.DataSetDeserializeDecompress(VersionConfigLink);

                // 查找版本配置
                DataRow versionConfig = null;
#if DEBUG
                foreach (DataRow select in softwareData.Tables[BLZ_SoftwareConfigDataSet.TableName_VersionInfo].Rows)
                {
                    if (versionConfig == null)
                    {
                        versionConfig = select;
                    }
                    else
                    {
                        if (IsNewerSoftwareVersion(versionConfig, select))
                        {
                            versionConfig = select;
                        }
                    }
                }

#else
                lastVersion = softwareData.Tables[BLZ_SoftwareConfigDataSet.TableName_VersionInfo].Rows.Find(GetVersionString());
#endif
                if (versionConfig == null)
                {
                    BLZ_Window.Show(CurrentLanguage["MSG_DownloadVersionFileFailed_Text"].ToString(), CurrentLanguage["MSG_DownloadVersionFileFailed_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.None);
                    Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                (ThreadStart)delegate ()
                                {
                                    Close();
                                });
                    return;
                }

                // 邮件配置
                string emailConfigURL = versionConfig[BLZ_SoftwareConfigDataSet.ColumnName_VersionInfo_ConfigAddress] as string;
                BLZ_EmailAndMapConfigDataSet emailData = BLZ_EmailAndMapConfigDataSet.DataSetDeserializeDecompress(emailConfigURL);

                if (emailConfigURL == null)
                {
                    BLZ_Window.Show(CurrentLanguage["MSG_DownloadVersionFileFailed_Text"].ToString(), CurrentLanguage["MSG_DownloadVersionFileFailed_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.None);
                    Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                (ThreadStart)delegate ()
                                {
                                    Close();
                                });
                    return;
                }

                string BLZMapConfigURL = emailData.Tables[BLZ_EmailAndMapConfigDataSet.TableName_Config].Rows[0][BLZ_EmailAndMapConfigDataSet.ColumnName_Config_GameCacheConfig] as string;
                SC2_MapInfoDataSet blzCacheData = SC2_MapInfoDataSet.DataSetDeserializeDecompress(BLZMapConfigURL);

                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                               (ThreadStart)delegate ()
                               {
                                   SoftwareConfigInfo = softwareData;
                                   CurrrentVersionConfigData = versionConfig;
                                   EmailConfigInfo = emailData;
                                   BLZMapConfigInfo = blzCacheData;
                                   LoadVersionsConfigFinish();
                               });


                foreach (DataRow select in emailData.Tables[BLZ_EmailAndMapConfigDataSet.TableName_MapConfig].Rows)
                {
                    try
                    {
                        string link = select[BLZ_EmailAndMapConfigDataSet.ColumnName_MapConfig_ConfigAddress] as string;
                        SC2_MapInfoDataSet mapData = SC2_MapInfoDataSet.DataSetDeserializeDecompress(link);
                        Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                               (ThreadStart)delegate ()
                               {
                                   MapConfigInfos.Add(link, mapData);
                                   LoadSingleMapConfigFinish();
                               });
                    }
                    catch
                    {
                        continue;
                    }
                }
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                              (ThreadStart)delegate ()
                              {
                                  LoadAllMapConfigFinish();
                              });
            }
#if Debug
            catch (Exception err)
            {
                BLZ_Window.Show(CurrentLanguage["MSG_SyncConfigFailed_Text"].ToString() + " \r\n" + err.Message, CurrentLanguage["MSG_SyncConfigFailed_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.None);
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                (ThreadStart)delegate ()
                                {
                                    SetMultiLanguageText(TextBlock_BLZDownloadInfo, "UI_BLZTextBlock_DownloadInfoSyncFailed_Text");
                                    RefreshMultiLanguageText(ComboBox_BLZLanguage.SelectedItem);
                                });
            }
#endif
        }
        
        /// <summary>
        /// 加载版本配置
        /// </summary>
        public void LoadVersionsConfigFinish()
        {
            CheckSendEmailButtonEnable();
            Button_BLZDownload.IsEnabled = true;
            Button_BLZStart.IsEnabled = true;
            Button_BLZRefresh.IsEnabled = true;
            SetMultiLanguageText(TextBlock_BLZDownloadInfo, "UI_BLZTextBlock_DownloadInfoSyncComplete_Text");
            object currentLanguageName = ComboBox_BLZLanguage.SelectedItem;
            DataRow [] langConfigOfVersion = CurrrentVersionConfigData.GetChildRows(BLZ_SoftwareConfigDataSet.RelationName_VersionString_LanguageConfig);
            foreach (DataRow select in langConfigOfVersion)
            {
                EnumLanguage language = (EnumLanguage)select[BLZ_SoftwareConfigDataSet.ColumnName_LanguageConfig_LanguageId];
                object languageName = DictUILanguages[language]["LanguageName"];
                string changeLog = select[BLZ_SoftwareConfigDataSet.ColumnName_LanguageConfig_SoftwareChangeLog] as string;

                SetMultiLanguageText(DictUILanguages[language]["LanguageName"], TextBlock_BLZInfoPannelText, changeLog);
                if (languageName == currentLanguageName)
                {
                    TextBlock_BLZInfoPannelText.Text = changeLog;
                }
            }
            DataTable regionTable = EmailConfigInfo.Tables[BLZ_EmailAndMapConfigDataSet.TableName_EmailGameRegionConfig];
            foreach (DataRow select in regionTable.Rows)
            {
                EnumGameRegion region = (EnumGameRegion)select[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_RegionIndex];
                DictAuthorHandle[region] = select[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_AuthorPlayerHandle] as string;
                DictBankName[region] = select[BLZ_EmailAndMapConfigDataSet.ColumnName_EmailGameRegion_GameBankNamePrefix] as string;
            }
            LoadSC2User();
            RefreshMultiLanguageText(ComboBox_BLZLanguage.SelectedItem);
        }
                
        /// <summary>
        /// 加载单个地图配置完成
        /// </summary>
        public void LoadSingleMapConfigFinish()
        {

        }


        /// <summary>
        /// 加载全部地图配置完成
        /// </summary>
        public void LoadAllMapConfigFinish()
        {

        }

        #endregion

        #region UI

        /// <summary>
        /// 设置控件关联的多语言文本
        /// </summary>
        /// <param name="language">语言，为空时设置全部语言的对应控件文本</param>
        /// <param name="control">控件</param>
        /// <param name="text">文本</param>
        private void SetMultiLanguageText(object language, object control, string text)
        {
            if (control == null)
            {
                throw new Exception("Null control in set multilanguage text!");
            }
            if (language == null)
            {
                foreach (var select in DictMultiLanguageText)
                {
                    select.Value[control] = text;
                }
            }
            else
            {
                if (!DictMultiLanguageText.ContainsKey(language))
                {
                    throw new Exception("No language exist on set multilanguage text!");
                }
                DictMultiLanguageText[language][control] = text;
            }
        }

        /// <summary>
        /// 设置控件关联的多语言文本
        /// </summary>
        /// <param name="language">语言，为空时设置全部语言的对应控件文本</param>
        /// <param name="control">控件</param>
        /// <param name="key">文本</param>
        private void SetMultiLanguageText(object control, string key)
        {
            if (control == null)
            {
                throw new Exception("Null control in set multilanguage text!");
            }
            foreach (var select in DictMultiLanguageText)
            {
                var a = DictUILanguages[DictComboBoxItemLanguage[select.Key]][key];
                select.Value[control] = DictUILanguages[DictComboBoxItemLanguage[select.Key]][key].ToString();
            }
        }

        /// <summary>
        /// 获取控件关联的多语言文本
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="control">控件</param>
        /// <returns>文本内容</returns>
        private string GetMultiLanguageText(object language, object control)
        {
            if (!DictMultiLanguageText.ContainsKey(language) || !DictMultiLanguageText[language].ContainsKey(control))
            {
                return "";
            }
            else
            {
                return DictMultiLanguageText[language][control];
            }
        }

        /// <summary>
        /// 刷新多语言文本
        /// </summary>
        /// <param name="language">语言</param>
        private void RefreshMultiLanguageText(object language)
        {
            TextBlock_BLZInfoPannelText.Text = GetMultiLanguageText(language, TextBlock_BLZInfoPannelText);
            TextBlock_BLZDownloadInfo.Text = GetMultiLanguageText(language, TextBlock_BLZDownloadInfo);
        }

        /// <summary>
        /// 设置软件模式
        /// </summary>
        /// <param name="isBLZ">是否为防卫局模式</param>
        private void SetSoftwareMode(bool isBLZ)
        {
            if (isBLZ)
            {
                Canvas.SetTop(Image_ModeSwitch, Const_ModeSwitchHight_On);
                Image_Light.Source = Application.Current.Resources["IMAGE_SignalLightGreen"] as BitmapImage;
                Image_ArcadeMode_Background.Visibility = Visibility.Hidden;
                Image_BLZBanner.Source = Application.Current.Resources["IMAGE_TitleImage"] as BitmapImage;
                TextBlock_Banner.Visibility = Visibility.Visible;
                TextBlock_Account.Visibility = Visibility.Visible;
                ComboBox_BLZAccount.Visibility = Visibility.Visible;
                TextBlock_Email.Visibility = Visibility.Visible;
                TextBox_BLZEmail.Visibility = Visibility.Visible;
                Button_BLZRefresh.Visibility = Visibility.Visible;
                Button_BLZRestore.Visibility = Visibility.Visible;
                Button_BLZSendMail.Visibility = Visibility.Visible;
                TextBlock_ArcadeGame.Visibility = Visibility.Hidden;
                ComboBox_ArcadeGame.Visibility = Visibility.Hidden;
            }
            else
            {
                Canvas.SetTop(Image_ModeSwitch, Const_ModeSwitchHight_Off);
                Image_Light.Source = Application.Current.Resources["IMAGE_SignalLightinitializing"] as BitmapImage;
                Image_ArcadeMode_Background.Visibility = Visibility.Visible;
                Image_BLZBanner.Source = Application.Current.Resources["IMAGE_TitleImage02"] as BitmapImage;
                TextBlock_Banner.Visibility = Visibility.Hidden;
                TextBlock_Account.Visibility = Visibility.Hidden;
                ComboBox_BLZAccount.Visibility = Visibility.Hidden;
                TextBlock_Email.Visibility = Visibility.Hidden;
                TextBox_BLZEmail.Visibility = Visibility.Hidden;
                Button_BLZRefresh.Visibility = Visibility.Hidden;
                Button_BLZRestore.Visibility = Visibility.Hidden;
                Button_BLZSendMail.Visibility = Visibility.Hidden;
                TextBlock_ArcadeGame.Visibility = Visibility.Visible;
                ComboBox_ArcadeGame.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region 缓存下载

        /// <summary>
        /// 递归复制目录
        /// </summary>
        /// <param name="toCopy">被复制的目录</param>
        /// <param name="newDir">复制到的基目录</param>
        /// <param name="baseDir">当前基目录</param>
        ///  <returns>复制结果</returns>
        private bool CopyDirectory(DirectoryInfo toCopy, DirectoryInfo newDir, DirectoryInfo baseDir)
        {
            try
            {
                DirectoryInfo copy = new DirectoryInfo(toCopy.FullName.Replace(baseDir.FullName, newDir.FullName));
                if (!copy.Exists) copy.Create();
                foreach (FileInfo select in toCopy.GetFiles())
                {
                    FileInfo newFile = new FileInfo(select.FullName.Replace(baseDir.FullName, newDir.FullName));
                    select.CopyTo(newFile.FullName, true);
                }
                bool result = true;
                foreach (DirectoryInfo select in toCopy.GetDirectories())
                {
                    if (!CopyDirectory(select, newDir, baseDir))
                    {
                        result = false;
                    }
                }
                return result;
            }
            catch
            {
                BLZ_Window.Show(CurrentLanguage["MSG_CopyFileFailed_Text"].ToString() + " (" + toCopy.FullName + ")", CurrentLanguage["MSG_CopyFileFailed_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        /// <summary>
        /// http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="path">文件存放地址，包含文件名</param>
        /// <param name="fileSize">当前文件大小</param>
        /// <param name="downloadSize">已下载文件大小</param>
        /// <param name="index">下载文件序号</param>
        /// <param name="count">下载文件总数</param>
        private void HttpDownload(string url, string path, long fileSize, long downloadSize, int index, int count, List<CacheFile> cacheFiles)
        {
            string tempPath = System.IO.Path.GetDirectoryName(path) + @"\temp";
            DirectoryInfo tempDir = new DirectoryInfo(tempPath);
            if (!tempDir.Exists) tempDir.Create();
            string tempFile = tempPath + @"\" + System.IO.Path.GetFileName(path) + ".temp"; //临时文件
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);    //存在则删除
            }
            FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Proxy = null;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            //Stream stream = new FileStream(tempFile, FileMode.Create);
            byte[] bArr = new byte[1024];
            SetDownloadInfo(0, fileSize, downloadSize, index, count);
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            int downSize = size;
            while (size > 0)
            {
                SetDownloadInfo(downSize, fileSize, downloadSize, index, count);
                //stream.Write(bArr, 0, size);
                fs.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
                downSize += size;
            }
            //stream.Close();
            fs.Close();
            responseStream.Close();
            System.IO.File.Move(tempFile, path);
            tempDir.Delete();
            if (index == count)
            {
                DownloadCacheFileEnd(cacheFiles);
            }
        }

        /// <summary>
        /// 下载缓存文件
        /// </summary>
        /// <param name="cacheFiles">下载文件</param>
        private void DownloadCacheFileStart(object cacheFiles)
        {
            int index = 1;
            List<CacheFile> list = cacheFiles as List<CacheFile>;
            long downloadSize = 0;
            foreach (CacheFile select in list)
            {
                CacheFile cacheFile = select;
                DownloadCacheFileDo(ref cacheFile, downloadSize, index, list.Count, list);
                downloadSize += select.Size;
                index++;
            }

        }

        /// <summary>
        /// 下载缓存文件
        /// </summary>
        /// <param name="file">下载文件</param>
        /// <param name="downloadSize">已下载文件大小</param>
        /// <param name="index">下载文件序号</param>
        /// <param name="count">下载文件总数</param>
        private void DownloadCacheFileDo(ref CacheFile file, long downloadSize, int index, int count, List<CacheFile> cacheFiles)
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + file.Path;
            file.IsSuccess = true;
            try
            {
                HttpDownload(file.URL, filePath, file.Size, downloadSize, index, count, cacheFiles);
            }
            catch (Exception err)
            {
                file.IsSuccess = false;
                file.ErrorMsg = err.Message;
            }
        }

        /// <summary>
        /// 下载缓存文件
        /// </summary>
        /// <param name="cacheFiles">下载文件</param>
        private void DownloadCacheFileEnd(List<CacheFile> cacheFiles)
        {
            int failedCount = 0;
            string failedMsg = "";
            foreach (CacheFile select in cacheFiles)
            {
                if (!select.IsSuccess)
                {
                    failedCount++;
                    failedMsg += "\r\n" + select.ErrorMsg;
                }
            }
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (failedCount == 0)
                {
                    TextBlock_BLZDownloadInfo.Text = CurrentLanguage["UI_BLZTextBlock_DownloadInfoDownloadComplete_Text"] as string;
                    BLZ_Window.Show(CurrentLanguage["MSG_DownloadCacheFileSuccess_Text"].ToString(), CurrentLanguage["MSG_DownloadCacheFileSuccess_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    TextBlock_BLZDownloadInfo.Text = CurrentLanguage["UI_BLZTextBlock_DownloadInfoDownloadFailde_Text"] as string;
                    BLZ_Window.Show(CurrentLanguage["MSG_DownloadCacheFileFailed_Text1"].ToString() + failedCount + CurrentLanguage["MSG_DownloadCacheFileFailed_Text2"].ToString() + "\r\n" + failedMsg, CurrentLanguage["MSG_DownloadCacheFileFailed_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.None);
                }
                Button_BLZDownload.IsEnabled = true;
            }));
        }

        /// <summary>
        /// 设置下载信息
        /// </summary>
        /// <param name="info">下载信息</param>
        /// <param name="currentSize">当前文件下载大小</param>
        /// <param name="fileSize">当前文件大小</param>
        /// <param name="downloadSize">已下载文件大小</param>
        /// <param name="index">下载文件序号</param>
        /// <param name="count">下载文件总数</param>
        private void SetDownloadInfo(long currentSize, long fileSize, long downloadSize, int index, int count)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                TextBlock_BLZDownloadInfo.Text = (currentSize / 1024) + "K/" + fileSize / 1024 + "K (" + index + "/" + count + ")";
                ProgressBar_BLZDownload.Value = ((currentSize + downloadSize) / 1024);
            }));
        }

        #endregion

        #endregion

        #region 控件事件
        /// <summary>
        /// 语言选择事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void ComboBox_BLZLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string itemName = ComboBox_BLZLanguage.SelectedItem as string;
            ResourceDictionary_WindowLanguage.MergedDictionaries.Clear();
            EnumLanguage language = DictComboBoxItemLanguage[itemName];
            if (language == EnumLanguage.enUS)
            {
                ResourceDictionary_WindowLanguage.MergedDictionaries.Add(BLZFont);
            }
            CurrentLanguage = DictUILanguages[language];
            ResourceDictionary_WindowLanguage.MergedDictionaries.Add(CurrentLanguage);
            BLZ_AboutWindow.SoftwareLanguage = language;
            BLZ_Window.SoftwareLanguage = language;
            Title = ResourceDictionary_WindowLanguage["WindowTitleText"].ToString() + " v" + SofteareVersion;
            RefreshMultiLanguageText(itemName);
        }

        /// <summary>
        /// 刷新角色
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_BLZRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadSC2User();
        }

        /// <summary>
        /// 邮箱地址失去焦点
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void TextBox_BLZEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckSendEmailButtonEnable();
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveConfig();
        }

        /// <summary>
        /// 游戏区域切换
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void ComboBox_BLZLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSC2User();
        }

        /// <summary>
        /// 点击发送事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_BLZSendMail_Click(object sender, RoutedEventArgs e)
        {
            SendMailThreadStart();
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// 退出程序事件
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 点击启动
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_BLZStart_Click(object sender, RoutedEventArgs e)
        {
            //EnumGameRegion region = (EnumGameRegion)(ComboBox_BLZLocation.SelectedItem as ComboBoxItem).Tag;
            //DataRow regionRow = MapConfigInfos.Tables[SC2_MapInfoDataSet.TableName_GameRegionConfig].Rows.Find(region);
            //System.Diagnostics.Process.Start(regionRow[SC2_MapInfoDataSet.ColumnName_GameRegion_StartGameLink].ToString());
        }

        /// <summary>
        /// 还原存档
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_BLZRestore_Click(object sender, RoutedEventArgs e)
        {
            if (BLZ_Window.Show(CurrentLanguage["MSG_RestoreStart_Text"].ToString(), CurrentLanguage["MSG_RestoreStart_Caption"].ToString(), MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                DirectoryInfo bankDir = new DirectoryInfo(Const_BankDir);
                if (!bankDir.Exists)
                {
                    BLZ_Window.Show(CurrentLanguage["MSG_FindOutBankFiles_Text"].ToString(), CurrentLanguage["MSG_FindOutBankFiles_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                DirectoryInfo sc2Dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\StarCraft II");
                if (CopyDirectory(bankDir, sc2Dir, bankDir))
                {
                    BLZ_Window.Show(CurrentLanguage["MSG_RestoreComplete_Text"].ToString(), CurrentLanguage["MSG_RestoreComplete_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    BLZ_Window.Show(CurrentLanguage["MSG_RestoreCompleteWithFailed_Text"].ToString(), CurrentLanguage["MSG_RestoreComplete_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception err)
            {
                BLZ_Window.Show(CurrentLanguage["MSG_RestoreCompleteWithFailed_Text"].ToString() + "\r\n" + err.Message, CurrentLanguage["MSG_RestoreComplete_Caption"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        /// <summary>
        /// 下载缓存
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_BLZDownload_Click(object sender, RoutedEventArgs e)
        {
            //if (BLZ_Window.Show(CurrentLanguage["MSG_DownloadCacheFileStart_Text"].ToString(), CurrentLanguage["MSG_DownloadCacheFileStart_Caption"].ToString(), MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            //{
            //    return;
            //}
            //List<CacheFile> cacheFiles = new List<CacheFile>();
            //Button_BLZDownload.IsEnabled = false;
            //DataRow regionRow = MapConfigInfos.Tables[SC2_MapInfoDataSet.TableName_GameRegionConfig].Rows.Find((ComboBox_BLZLocation.SelectedItem as ComboBoxItem).Tag);
            //long fullSize = 0;
            //foreach (DataRow select in regionRow.GetChildRows(SC2_MapInfoDataSet.RelationName_CacheFileRegionIndex))
            //{
            //    CacheFile cache = new CacheFile(select);
            //    if (cache.IsNeedDownload())
            //    {
            //        cacheFiles.Add(cache);
            //        fullSize += cache.Size;
            //    }
            //}
            //if (cacheFiles.Count != 0)
            //{
            //    ProgressBar_BLZDownload.Value = 0;
            //    ProgressBar_BLZDownload.Maximum = (fullSize / 1024);
            //    Task task = new Task(DownloadCacheFileStart, cacheFiles);
            //    task.Start();
            //}
            //else
            //{
            //    DownloadCacheFileEnd(cacheFiles);
            //}
        }

        /// <summary>
        /// 显示关于窗口
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_BLZHelp_Click(object sender, RoutedEventArgs e)
        {
            BLZ_AboutWindow about = new BLZ_AboutWindow();
            about.ShowDialog();
        }


        /// <summary>
        /// 鼠标进入
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void ToggleButton_BLZShowPullDownPage_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (ToggleButton_BLZShowPullDownPage.IsChecked == false)
            //{
            //    Canvas_BLZInfoPannel.Visibility = Visibility.Visible;
            //}
            Canvas_BLZInfoPannel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void ToggleButton_BLZShowPullDownPage_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (ToggleButton_BLZShowPullDownPage.IsChecked == false)
            //{
            //    Canvas_BLZInfoPannel.Visibility = Visibility.Hidden;
            //}
            Canvas_BLZInfoPannel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 鼠标进入
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Canvas_BLZInfoPannel_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (ToggleButton_BLZShowPullDownPage.IsChecked == false)
            //{
            //    Canvas_BLZInfoPannel.Visibility = Visibility.Visible;
            //}
            Canvas_BLZInfoPannel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Canvas_BLZInfoPannel_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (ToggleButton_BLZShowPullDownPage.IsChecked == false)
            //{
            //    Canvas_BLZInfoPannel.Visibility = Visibility.Hidden;
            //}
            Canvas_BLZInfoPannel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 切换开关
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void ToggleButton_BLZModeSwitch_Click(object sender, RoutedEventArgs e)
        {
            bool isOpen = ToggleButton_BLZModeSwitch.IsChecked == true;
            SetSoftwareMode(isOpen);
        }

        /// <summary>
        /// 进入按钮范围
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void ToggleButton_BLZModeSwitch_MouseEnter(object sender, MouseEventArgs e)
        {
            Image_ModeSwitch.Source = Application.Current.Resources["IMAGE_ModeSwitch_On"] as BitmapImage;
        }

        /// <summary>
        /// 离开按钮范围
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void ToggleButton_BLZModeSwitch_MouseLeave(object sender, MouseEventArgs e)
        {
            Image_ModeSwitch.Source = Application.Current.Resources["IMAGE_ModeSwitch"] as BitmapImage;
        }
        #endregion
    }
}
