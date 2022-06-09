using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace WH_CommonControlLibrary.Functionality.MultiLanguage
{
    /// <summary>
    /// 多语言支持类
    /// </summary>
    public class WH_MultiLanguageSupport
    {
        #region 属性
        /// <summary>
        /// 多语言名字列表
        /// </summary>
        public Dictionary<string, ResourceDictionary> UILanguages { get; set; }
        #endregion

        #region 构造函数
        public WH_MultiLanguageSupport()
        {
            UILanguages = new Dictionary<string, ResourceDictionary>();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化多语言支持
        /// </summary>
        /// <param name="languageFilePath">语音文件路径</param>
        /// <param name="comboBox">多语言选择控件</param>
        public void InitializeMultiLanguageSupport(string languageFilePath, ComboBox comboBox)
        {
            ResourceDictionary defaultLanguage = new ResourceDictionary
            {
                Source = new Uri(languageFilePath + @"Default.xaml", UriKind.Relative)
            };
            string defaultName = defaultLanguage["LanguageName"] as string;
            UILanguages.Add(defaultName, defaultLanguage);
            comboBox.Items.Add(defaultName);
            comboBox.SelectedItem = defaultName;
            if (Directory.Exists(languageFilePath))
            {
                foreach (FileInfo select in Directory.GetFiles(languageFilePath).Select(r => new FileInfo(r)))
                {
                    if (select.Extension == ".xaml")
                    {
                        try
                        {
                            string coltureName = select.Name.Substring(0, select.Name.Length - select.Extension.Length);
                            CultureInfo colture = new CultureInfo(coltureName);
                            ResourceDictionary language = new ResourceDictionary
                            {
                                Source = new Uri(select.FullName)
                            };
                            string itemName = language["LanguageName"] as string;
                            UILanguages.Add(itemName, language);
                            comboBox.Items.Add(itemName);
                            if (CultureInfo.CurrentCulture.Name == coltureName)
                            {
                                comboBox.SelectedItem = itemName;
                            }
                        }
                        catch (Exception)
                        {
                            
                            continue;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 初始化多语言支持
        /// </summary>
        /// <param name="languageFilePath">语音文件路径</param>
        /// <param name="comboBox">多语言选择控件</param>
        public string InitializeMultiLanguageSupport(string languageFilePath, ref List<string> languageColture)
        {
            string selectItem;
            ResourceDictionary defaultLanguage = new ResourceDictionary
            {
                Source = new Uri(languageFilePath + @"Default.xaml", UriKind.Relative)
            };
            string defaultName = defaultLanguage["LanguageName"] as string;
            UILanguages.Add(defaultName, defaultLanguage);
            languageColture.Add(defaultName);
            selectItem = defaultName;
            if (Directory.Exists(languageFilePath))
            {
                foreach (FileInfo select in Directory.GetFiles(languageFilePath).Select(r => new FileInfo(r)))
                {
                    if (select.Extension == ".xaml")
                    {
                        try
                        {
                            string coltureName = select.Name.Substring(0, select.Name.Length - select.Extension.Length);
                            CultureInfo colture = new CultureInfo(coltureName);
                            ResourceDictionary language = new ResourceDictionary
                            {
                                Source = new Uri(select.FullName)
                            };
                            string itemName = language["LanguageName"] as string;
                            UILanguages.Add(itemName, language);
                            languageColture.Add(itemName);
                            if (CultureInfo.CurrentCulture.Name == coltureName)
                            {
                                selectItem = itemName;
                            }
                        }
                        catch (Exception)
                        {

                            continue;
                        }
                    }
                }
            }
            return selectItem;
        }
        /// <summary>
        /// 获取语音资源字典
        /// </summary>
        /// <param name="name">本地化名称</param>
        /// <returns>语音资源字典</returns>
        public ResourceDictionary GetLanguageResourceDictionary(string culture)
        {
            return UILanguages[culture];
        }
        #endregion
    }
}
