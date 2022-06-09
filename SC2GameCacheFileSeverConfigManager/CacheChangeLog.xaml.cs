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
    /// ChangeLog.xaml 的交互逻辑
    /// </summary>
    public partial class CacheChangeLog : TabItem
    {
        #region 属性字段
        /// <summary>
        /// 当前语言依赖项属性
        /// </summary>
        public static DependencyProperty EnumCurrentLanguageProperty = DependencyProperty.Register("EnumCurrentLanguage", typeof(EnumLanguage), typeof(CacheChangeLog));

        /// <summary>
        /// 当前语言依赖项
        /// </summary>
        public EnumLanguage EnumCurrentLanguage
        {
            set
            {
                SetValue(EnumCurrentLanguageProperty, value);
                ResourceDictionary_TabItemLanguage.MergedDictionaries.Clear();
                CurrentLanguage = SC2GameCacheSeverConfigManager.DictUILanguages[value];
                ResourceDictionary_TabItemLanguage.MergedDictionaries.Add(CurrentLanguage);
            }
            get
            {
                return ((EnumLanguage)GetValue(EnumCurrentLanguageProperty));
            }
        }
        /// <summary>
        /// 当前语言
        /// </summary>
        private ResourceDictionary CurrentLanguage;
        #endregion
        public CacheChangeLog()
        {
            InitializeComponent();
        }
    }
}
