using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protecc;

public partial class InfoPage : ContentPage
{
    public InfoPage()
    {
        InitializeComponent();
        
        NavigationPage.SetHasNavigationBar(this, false);
    }
}