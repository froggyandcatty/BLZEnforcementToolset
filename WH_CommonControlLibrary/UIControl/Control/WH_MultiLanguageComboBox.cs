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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WH_CommonControlLibrary.Functionality.MultiLanguage;

namespace WH_CommonControlLibrary.UIControl.Control
{
    /// <summary>
    /// 多语言选择下拉列表
    /// </summary>
    public class WH_MultiLanguageComboBox : ComboBox
    {
        #region 属性
        private WH_MultiLanguageSupport LanguageSupport;
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public WH_MultiLanguageComboBox()
        {
            LanguageSupport = new WH_MultiLanguageSupport();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 初始化多语言支持
        /// </summary>
        /// <param name="languageFilePath"></param>
        public void InitMultiLanguage(string languageFilePath)
        {
            LanguageSupport.InitializeMultiLanguageSupport(languageFilePath, this);
        }

        /// <summary>
        /// 获取语音资源字典
        /// </summary>
        /// <param name="name">本地化名称</param>
        /// <returns>语音资源字典</returns>
        public ResourceDictionary GetLanguageResourceDictionary(string culture)
        {
            return LanguageSupport.GetLanguageResourceDictionary(culture);
        }
        #endregion
    }
}
