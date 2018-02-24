using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace esHelper.Common
{
    public class PageUtil
    {
        public static void SetDefaultCursor()
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }

        public static void SetLoadingCursor()
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Wait, 1);
        }

        public static void ShowMsg(string message)
        {
            SetDefaultCursor();
            (new MessageDialog(message)).ShowAsync();
        }
    }
}
