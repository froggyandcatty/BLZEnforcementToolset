using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BLZEnforcementToolset.BLZControl
{
    public class BLZ_ModeSwitch : ToggleButton
    {

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        static BLZ_ModeSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BLZ_ModeSwitch), new FrameworkPropertyMetadata(typeof(BLZ_ModeSwitch)));
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public BLZ_ModeSwitch()
        {
            Style = Application.Current.Resources["BLZ_ModeSwitchStyle"] as Style;
        }

        #endregion
    }
}
