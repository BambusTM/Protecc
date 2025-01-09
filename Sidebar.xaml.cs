using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protecc;

public partial class Sidebar : ContentView
{
    public Sidebar()
    {
        InitializeComponent();
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        if (Application.Current.MainPage is NavigationPage navigationPage)
        {
            await navigationPage.Navigation.PopToRootAsync();
        }
    }

    private async void OnMainClicked(object sender, EventArgs e)
    {
        if (Application.Current.MainPage is NavigationPage navigationPage)
        {
            await navigationPage.Navigation.PushAsync(new MainPage());
        }
    }

    private async void OnInfoClicked(object sender, EventArgs e)
    {
        if (Application.Current.MainPage is NavigationPage navigationPage)
        {
            await navigationPage.Navigation.PushAsync(new InfoPage());
        }
    }
}