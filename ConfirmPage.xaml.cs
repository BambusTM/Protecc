using System;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Protecc.Models;

namespace Protecc;

public partial class ConfirmPage : ContentPage
{
    public ConfirmPage()
    {
        InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);

        LoadAndDisplayData();
    }

    private async void PreviousPage(object sender, EventArgs e)
    {
        if (Application.Current.MainPage is NavigationPage navigationPage)
        {
            await navigationPage.Navigation.PushAsync(new AnalyzisPage());
        }
    }

    private void LoadAndDisplayData()
    {
        try
        {
            // File paths
            string expenseFilePath =
                "/Users/yoru/Library/Containers/com.companyname.protecc/Data/Library/expenseData.json";
            string incomeFilePath =
                "/Users/yoru/Library/Containers/com.companyname.protecc/Data/Library/incomeData.json";

            // Read JSON files
            string expenseJson = File.ReadAllText(expenseFilePath);
            string incomeJson = File.ReadAllText(incomeFilePath);

            // Parse JSON data
            var expenseData = JsonConvert.DeserializeObject<ExpenseData>(expenseJson);
            var incomeData = JsonConvert.DeserializeObject<IncomeData>(incomeJson);

            // Populate Income Expander
            var incomeLayout = this.FindByName<VerticalStackLayout>("IncomeContentLayout");
            incomeLayout.Children.Clear();
            foreach (var property in typeof(IncomeData).GetProperties())
            {
                incomeLayout.Children.Add(new Label
                {
                    Text = $"{property.Name}: {property.GetValue(incomeData)} CHF / Jahr",
                    FontSize = 18,
                    TextColor = (Color)Application.Current.Resources["White"]
                });
            }

            // Populate Expense Expander
            var expenseLayout = this.FindByName<VerticalStackLayout>("ExpenseContentLayout");
            expenseLayout.Children.Clear();
            foreach (var property in typeof(ExpenseData).GetProperties())
            {
                expenseLayout.Children.Add(new Label
                {
                    Text = $"{property.Name}: {property.GetValue(expenseData)} CHF / Jahr",
                    FontSize = 18,
                    TextColor = (Color)Application.Current.Resources["White"]
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        var selectedDate = e.NewDate;
        var oneYearLater = selectedDate.AddYears(1);

        var dateRangeLabel = this.FindByName<Label>("DateRangeLabel");
        dateRangeLabel.Text = $"{selectedDate:dd.MM.yyyy} - {oneYearLater:dd.MM.yyyy}";
    }


    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            var profileNameEntry = this.FindByName<Entry>("ProfileNameEntry");
            var startDatePicker = this.FindByName<DatePicker>("StartDatePicker");
            var dateRangeLabel = this.FindByName<Label>("DateRangeLabel");

            if (string.IsNullOrWhiteSpace(profileNameEntry.Text))
            {
                await DisplayAlert("Fehler", "Bitte geben Sie einen Profilnamen ein.", "OK");
                return;
            }

            if (startDatePicker.Date == null)
            {
                await DisplayAlert("Fehler", "Bitte wählen Sie ein Startdatum aus.", "OK");
                return;
            }

            var endDate = startDatePicker.Date.AddYears(1).ToString("dd.MM.yyyy");

            var profileData = new ProfileData
            {
                ProfileName = profileNameEntry.Text,
                StartDate = startDatePicker.Date.ToString("dd.MM.yyyy"),
                EndDate = endDate,
                IncomeFileRef = "/Users/yoru/Library/Containers/com.companyname.protecc/Data/Library/incomeData.json",
                ExpenseFileReg = "/Users/yoru/Library/Containers/com.companyname.protecc/Data/Library/expenseData.json"
            };

            string profileFilePath =
                "/Users/yoru/Library/Containers/com.companyname.protecc/Data/Library/profileData.json";

            string profileJson = JsonConvert.SerializeObject(profileData, Formatting.Indented);

            File.WriteAllText(profileFilePath, profileJson);

            // Popup
            await DisplayAlert("Erfolg", "Profil erfolgreich gespeichert.", "OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving profile: {ex.Message}");
            await DisplayAlert("Fehler", "Beim Speichern des Profils ist ein Fehler aufgetreten.", "OK");
        }

        if (Application.Current.MainPage is NavigationPage navigationPage)
        {
            await navigationPage.Navigation.PushAsync(new MainPage());
        }
    }
}
