using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Planact.App.Views
{
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplashScreen
    public sealed partial class Splash : UserControl
    {
        public Splash(SplashScreen splashScreen)
        {
            this.InitializeComponent();
        }
    }
}

