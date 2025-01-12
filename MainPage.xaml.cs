using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Newtonsoft.Json;
using Protecc.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Protecc;

public partial class MainPage : ContentPage
{
    public ProfileData Profile { get; private set; }
    public IncomeData Income { get; private set; }
    public ExpenseData Expense { get; private set; }

    public MainPage()
    {
        InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);

        LoadData();
    }

    private async void LoadData()
    {
        await Task.Run(() =>
        {
            try
            {
                string profileFilePath =
                    "/Users/yoru/Library/Containers/com.companyname.protecc/Data/Library/profileData.json";

                string profileJson = File.ReadAllText(profileFilePath);

                Console.WriteLine($"Profile JSON: {profileJson}");

                Profile = JsonConvert.DeserializeObject<ProfileData>(profileJson);

                if (Profile == null)
                {
                    Console.WriteLine("Profile data is null or could not be parsed.");
                    return;
                }

                Console.WriteLine($"Profile data loaded: {Profile.ProfileName}");

                string incomeFilePath = Profile.IncomeFileRef;
                string expenseFilePath = Profile.ExpenseFileReg;

                Console.WriteLine($"Income file path: {incomeFilePath}");
                Console.WriteLine($"Expense file path: {expenseFilePath}");

                string incomeJson = File.ReadAllText(incomeFilePath);

                Console.WriteLine($"Income JSON: {incomeJson}");

                Income = JsonConvert.DeserializeObject<IncomeData>(incomeJson);

                if (Income == null)
                {
                    Console.WriteLine("Income data is null or could not be parsed.");
                    return;
                }

                Console.WriteLine($"Income data loaded: Salary = {Income.Salary}, Sideline = {Income.Sideline}");

                string expenseJson = File.ReadAllText(expenseFilePath);

                Console.WriteLine($"Expense JSON: {expenseJson}");

                Expense = JsonConvert.DeserializeObject<ExpenseData>(expenseJson);

                if (Expense == null)
                {
                    Console.WriteLine("Expense data is null or could not be parsed.");
                    return;
                }

                Console.WriteLine(
                    $"Expense data loaded: FoodTotal = {Expense.FoodTotal}, RentTotal = {Expense.RentTotal}");

                Console.WriteLine("Data loaded successfully:");
                Console.WriteLine($"Profile: {Profile.ProfileName}");
                Console.WriteLine($"Income Total: {Income?.TotalIncome} CHF");
                Console.WriteLine($"Expense Total: {Expense?.TotalExpense} CHF");

                MainThread.BeginInvokeOnMainThread(() => { DisplayExpenseComparisonChart(); });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        });
    }

    private void DisplayExpenseComparisonChart()
    {
        if (Expense == null)
        {
            Console.WriteLine("Expense data is null, cannot generate chart.");
            return;
        }

        var expenseChartLayout = this.FindByName<StackLayout>("ExpenseChartLayout");

        if (expenseChartLayout == null)
        {
            Console.WriteLine("ExpenseChartLayout not found.");
            return;
        }

        expenseChartLayout.Children.Clear();

        // Title for the chart
        expenseChartLayout.Children.Add(new Label
        {
            Text = "Ausgaben im Vergleich",
            FontSize = 32,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Start,
            TextColor = (Color)Application.Current.Resources["Secondary"]
        });

        // Define the expenses and their values
        var expenses = new (string Label, decimal Value)[]
        {
            ("Lebensmittel", Expense.FoodTotal),
            ("Miete", Expense.RentTotal),
            ("Steuern", Expense.Tax),
            ("Abos", Expense.SubscriptionTotal),
            ("Auto", Expense.CarTotal),
            ("Versicherungen", Expense.InsuranceTotal),
            ("Shopping", Expense.ShoppingTotal),
            ("Andere", Expense.Other)
        };

        // Log the expenses for debugging
        Console.WriteLine("Expenses for chart:");
        foreach (var expense in expenses)
        {
            Console.WriteLine($"{expense.Label}: {expense.Value:F2} CHF");
        }

        // Calculate the maximum expense for proportional sizing
        decimal maxExpense = expenses.Max(e => e.Value);
        Console.WriteLine($"Max Expense Value: {maxExpense:F2}");

        // Loop through the expenses and create bars for the chart
        foreach (var expense in expenses)
        {
            // Add the label for the expense
            expenseChartLayout.Children.Add(new Label
            {
                Text = $"{expense.Label}: {expense.Value:F2} CHF",
                FontSize = 18,
                TextColor = (Color)Application.Current.Resources["Gray600"]
            });

            var bar = new BoxView
            {
                HeightRequest = 20,
                WidthRequest =
                    maxExpense > 0
                        ? (double)(expense.Value / maxExpense) * 300
                        : 0,
                BackgroundColor = (Color)Application.Current.Resources["PrimaryDark"],
                HorizontalOptions = LayoutOptions.Start
            };

            // Add the bar to the layout
            expenseChartLayout.Children.Add(bar);
        }
    }
}