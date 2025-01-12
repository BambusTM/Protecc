using System;
using Microsoft.Maui.Controls;

namespace Protecc;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void NavToAnalyzis(object sender, EventArgs e)
    {
        if (Application.Current.MainPage is NavigationPage navigationPage)
        {
            await navigationPage.Navigation.PushAsync(new AnalyzisPage());
        }
    }
    
    
}