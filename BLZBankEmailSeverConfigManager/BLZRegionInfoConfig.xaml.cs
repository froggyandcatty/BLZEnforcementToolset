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

using EnumLanguage = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumLanguage;
namespace BLZEnforcementToolset
{
    /// <summary>
    /// RegionInfoConfig.xaml 的交互逻辑
    /// </summary>
    public partial class BLZRegionInfoConfig : TabItem
    {
        #region 属性字段
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZRegionInfoConfig()
        {
            InitializeComponent();
        }
        #endregion

        #region 事件响应
        /// <summary>
        /// 清理全部日志按钮
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应参数</param>
        private void Button_CleanAllChangeLog_Click(object sender, RoutedEventArgs e)
        {
        }
        #endregion
    }
}
