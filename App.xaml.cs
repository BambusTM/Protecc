using Microsoft.Maui.Controls;

namespace Protecc;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new HomePage());
    }
}