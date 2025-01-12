using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                // Path for profile data
                string profileFilePath =
                    "/Users/yoru/Library/Containers/com.companyname.protecc/Data/Library/profileData.json";
                
                // Read the profile JSON file
                string profileJson = File.ReadAllText(profileFilePath);
                
                // Debugging: Check if profile data was read correctly
                Console.WriteLine($"Profile JSON: {profileJson}");

                // Deserialize the profile data
                Profile = JsonConvert.DeserializeObject<ProfileData>(profileJson);

                // Debugging: Check if Profile data was successfully deserialized
                if (Profile == null)
                {
                    Console.WriteLine("Profile data is null or could not be parsed.");
                    return;
                }
                
                Console.WriteLine($"Profile data loaded: {Profile.ProfileName}");

                // Use paths from Profile to load income and expense data
                string incomeFilePath = Profile.IncomeFileRef;
                string expenseFilePath = Profile.ExpenseFileReg;

                // Debugging: Check the paths used for income and expense files
                Console.WriteLine($"Income file path: {incomeFilePath}");
                Console.WriteLine($"Expense file path: {expenseFilePath}");

                // Read the income JSON file
                string incomeJson = File.ReadAllText(incomeFilePath);
                
                // Debugging: Check if income data was read correctly
                Console.WriteLine($"Income JSON: {incomeJson}");

                // Deserialize the income data
                Income = JsonConvert.DeserializeObject<IncomeData>(incomeJson);

                // Debugging: Check if Income data was successfully deserialized
                if (Income == null)
                {
                    Console.WriteLine("Income data is null or could not be parsed.");
                    return;
                }
                
                Console.WriteLine($"Income data loaded: Salary = {Income.Salary}, Sideline = {Income.Sideline}");

                // Read the expense JSON file
                string expenseJson = File.ReadAllText(expenseFilePath);
                
                // Debugging: Check if expense data was read correctly
                Console.WriteLine($"Expense JSON: {expenseJson}");

                // Deserialize the expense data
                Expense = JsonConvert.DeserializeObject<ExpenseData>(expenseJson);

                // Debugging: Check if Expense data was successfully deserialized
                if (Expense == null)
                {
                    Console.WriteLine("Expense data is null or could not be parsed.");
                    return;
                }

                Console.WriteLine($"Expense data loaded: FoodTotal = {Expense.FoodTotal}, RentTotal = {Expense.RentTotal}");

                // Log for debugging purposes
                Console.WriteLine("Data loaded successfully:");
                Console.WriteLine($"Profile: {Profile.ProfileName}");
                Console.WriteLine($"Income Total: {Income?.TotalIncome} CHF");
                Console.WriteLine($"Expense Total: {Expense?.TotalExpense} CHF");

                // Use MainThread to update UI elements on the main thread
                MainThread.BeginInvokeOnMainThread(() => { DisplayExpenseComparisonChart(); });
            }
            catch (Exception ex)
            {
                // Catch any errors and log them
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

        // Debugging: Check if the StackLayout for the chart was found
        if (expenseChartLayout == null)
        {
            Console.WriteLine("ExpenseChartLayout not found.");
            return;
        }

        expenseChartLayout.Children.Clear();

        expenseChartLayout.Children.Add(new Label
        {
            Text = "Ausgaben im Vergleich",
            FontSize = 32,
            BackgroundColor = Colors.Red,
            HorizontalOptions = LayoutOptions.Start
        });

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

        // Debugging: Print out expense values
        Console.WriteLine("Expenses for chart:");
        foreach (var expense in expenses)
        {
            Console.WriteLine($"{expense.Label}: {expense.Value:F2} CHF");
        }

        decimal maxExpense = expenses.Max(e => e.Value);

        // Debugging: Check the maximum expense value
        Console.WriteLine($"Max Expense Value: {maxExpense:F2}");

        foreach (var expense in expenses)
        {
            expenseChartLayout.Children.Add(new Label
            {
                Text = $"{expense.Label}: {expense.Value:F2} CHF",
                FontSize = 18,
                TextColor = (Color)Application.Current.Resources["Secondary"]
            });

            expenseChartLayout.Children.Add(new BoxView
            {
                HeightRequest = 20,
                WidthRequest = maxExpense > 0 ? (double)(expense.Value / maxExpense) * 300 : 0,
                BackgroundColor = Colors.Red,
                HorizontalOptions = LayoutOptions.Start
            });

            // Debugging: Check if BoxView is being added correctly
            Console.WriteLine($"Added bar for {expense.Label} with width {(maxExpense > 0 ? (expense.Value / maxExpense) * 300 : 0)}");
        }
    }
}
