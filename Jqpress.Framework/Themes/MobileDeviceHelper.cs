using System.Web;

namespace Jqpress.Framework.Themes
{
    public class MobileDeviceHelper : IMobileDeviceHelper
    {
        #region Implementation of IMobileDeviceHelper

        /// <summary>
        /// Returns a value indicating whether request is made by a mobile device
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Result</returns>
        public bool IsMobileDevice(HttpContextBase httpContext)
        {
            return false;
        }

        /// <summary>
        /// Returns a value indicating whether mobile devices support is enabled
        /// </summary>
        public bool MobileDevicesSupported()
        {
            return false;
        }

        /// <summary>
        /// Returns a value indicating whether current customer prefer to use full desktop version (even request is made by a mobile device)
        /// </summary>
        public bool CustomerDontUseMobileVersion()
        {
            return true;
        }

        #endregion
    }
}
