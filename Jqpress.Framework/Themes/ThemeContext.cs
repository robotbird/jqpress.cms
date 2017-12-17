using System;

namespace Jqpress.Framework.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private static ThemeContext themeContext;
        private ThemeContext(){}
        #region Implementation of IThemeContext

        /// <summary>
        /// Get or set current theme for desktops
        /// </summary>
        public string WorkingDesktopTheme { get; set; }
        /// <summary>
        /// 模板样式文件
        /// </summary>
        public string ThemeCss { get; set; }

        /// <summary>
        /// Get current theme for mobile (e.g. Mobile)
        /// </summary>
        public string WorkingMobileTheme
        {
            get
            {
                string theme = "";
                return theme;
            }
        }

        #endregion

        public static ThemeContext GetInstance() 
        {
            if (themeContext == null) 
            {
                themeContext = new ThemeContext();
            }
            return themeContext;
        }
    }
}
