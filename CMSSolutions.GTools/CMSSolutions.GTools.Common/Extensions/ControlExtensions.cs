using System.ComponentModel;
using System.Windows.Forms;

namespace CMSSolutions.GTools.Common.Extensions
{
    public static class ControlExtensions
    {
        //FROM: http://dnpextensions.codeplex.com
        /// <summary>
        ///   Returns <c>true</c> if target control is in design mode or one of the target's parent is in design mode.
        ///   Othervise returns <c>false</c>.
        /// </summary>
        /// <param name = "target">Target control. Can not be null.</param>
        /// <example>
        ///   bool designMode = this.button1.IsInWinDesignMode();
        /// </example>
        /// <remarks>
        ///   The design mode is set only to direct controls in desgined control/form.
        ///   However the child controls in controls/usercontrols does not flag for "my parent is in design mode".
        ///   The solution is traversion upon parents of target control.
        ///
        ///   Contributed by tencokacistromy, http://www.codeplex.com/site/users/view/tencokacistromy
        /// </remarks>
        public static bool IsInWinDesignMode(this Control target)
        {
            bool ret = false;

            Control ctl = target;
            while (false == object.ReferenceEquals(ctl, null))
            {
                ISite site = ctl.Site;
                if (false == object.ReferenceEquals(site, null))
                {
                    if (site.DesignMode)
                    {
                        ret = true;
                        break;
                    }
                }
                ctl = ctl.Parent;
            }

            return ret;
        }
    }
}