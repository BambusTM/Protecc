using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microcharts;
using Newtonsoft.Json;
using Protecc.Models;
using Microsoft.Maui.Controls;
using SkiaSharp;

namespace Protecc;

public partial class MainPage : ContentPage
{
    // Properties to hold loaded data
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

                // Read and deserialize profile data
                string profileJson = File.ReadAllText(profileFilePath);
                Profile = JsonConvert.DeserializeObject<ProfileData>(profileJson);

                if (Profile == null)
                {
                    Console.WriteLine("Profile data is null or could not be parsed.");
                    return;
                }

                // Use paths from Profile to load other data
                string incomeFilePath = Profile.IncomeFileRef;
                string expenseFilePath = Profile.ExpenseFileReg;

                // Read and deserialize income data
                string incomeJson = File.ReadAllText(incomeFilePath);
                Income = JsonConvert.DeserializeObject<IncomeData>(incomeJson);

                // Read and deserialize expense data
                string expenseJson = File.ReadAllText(expenseFilePath);
                Expense = JsonConvert.DeserializeObject<ExpenseData>(expenseJson);

                // Log for debugging purposes
                Console.WriteLine("Data loaded successfully:");
                Console.WriteLine($"Profile: {Profile.ProfileName}");
                Console.WriteLine($"Income Total: {Income?.TotalIncome} CHF");
                Console.WriteLine($"Expense Total: {Expense?.TotalExpense} CHF");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        });
    }

    private void DisplayExpenseChart()
    {
        if (Expense == null)
        {
            Console.WriteLine("Expense data is not loaded.");
            return;
        }

        // Prepare chart entries
        var entries = new List<ChartEntry>
        {
            new ChartEntry((float)Expense.FoodTotal)
            {
                Label = "Essen",
                ValueLabel = $"{Expense.FoodTotal:F2} CHF",
                Color = SKColor.Parse("#FF6384")
            },
            new ChartEntry((float)Expense.RentTotal)
            {
                Label = "Miete",
                ValueLabel = $"{Expense.RentTotal:F2} CHF",
                Color = SKColor.Parse("#36A2EB")
            },
            new ChartEntry((float)Expense.Tax)
            {
                Label = "Steuer",
                ValueLabel = $"{Expense.Tax:F2} CHF",
                Color = SKColor.Parse("#FFCE56")
            },
            new ChartEntry((float)Expense.SubscriptionTotal)
            {
                Label = "Abos",
                ValueLabel = $"{Expense.SubscriptionTotal:F2} CHF",
                Color = SKColor.Parse("#4BC0C0")
            },
            new ChartEntry((float)Expense.CarTotal)
            {
                Label = "Auto",
                ValueLabel = $"{Expense.CarTotal:F2} CHF",
                Color = SKColor.Parse("#9966FF")
            },
            new ChartEntry((float)Expense.InsuranceTotal)
            {
                Label = "Versicherung",
                ValueLabel = $"{Expense.InsuranceTotal:F2} CHF",
                Color = SKColor.Parse("#FF9F40")
            },
            new ChartEntry((float)Expense.ShoppingTotal)
            {
                Label = "Shopping",
                ValueLabel = $"{Expense.ShoppingTotal:F2} CHF",
                Color = SKColor.Parse("#E7E9ED")
            },
            new ChartEntry((float)Expense.Other)
            {
                Label = "Sonstige",
                ValueLabel = $"{Expense.Other:F2} CHF",
                Color = SKColor.Parse("#C9CBCF")
            }
        };
    }
}