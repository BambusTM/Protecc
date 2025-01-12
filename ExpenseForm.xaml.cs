using System;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Protecc.Models;

namespace Protecc
{
    public partial class ExpenseForm : ContentView
    {
        private readonly string _filePath;
        private ExpenseData _currentData;

        public ExpenseForm()
        {
            InitializeComponent();

            _filePath = Path.Combine(FileSystem.AppDataDirectory, "expenseData.json");
        }

        private async void NextPage(object sender, EventArgs e)
        {
            if (Application.Current.MainPage is NavigationPage navigationPage)
            {
                await navigationPage.Navigation.PushAsync(new ConfirmPage());
            }
        }

        private void OnExpenseChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotal();
        }

        private async void SaveData(object sender, EventArgs e)
        {
            _currentData ??= new ExpenseData();

            // Update _currentData fields
            _currentData.FoodTotal = ParseTotal(FoodFromEntry.Text, FoodToEntry.Text);
            _currentData.RentTotal = ParseTotal(RentEntry.Text, RentFromEntry.Text, RentToEntry.Text);
            _currentData.Tax = ParseDecimal(TaxEntry.Text);
            _currentData.SubscriptionTotal = ParseDecimal(FunSubEntry.Text) +
                                             ParseDecimal(HealthSubEntry.Text) +
                                             ParseDecimal(MoreSubEntry.Text);
            _currentData.CarTotal = ParseDecimal(FuelCarEntry.Text) +
                                    ParseDecimal(MaintainCarEntry.Text) +
                                    ParseDecimal(RepairCarEntry.Text);
            _currentData.InsuranceTotal = ParseDecimal(HealthInsuranceEntry.Text) +
                                          ParseDecimal(LiabilityInsuranceEntry.Text) +
                                          ParseDecimal(CarInsuranceEntry.Text);
            _currentData.ShoppingTotal = ParseDecimal(ClothesShopEntry.Text) +
                                         ParseDecimal(HauseholdShopEntry.Text) +
                                         ParseDecimal(MoreShopEntry.Text);
            _currentData.Other = ParseDecimal(OtherExpensesEntry.Text);

            // Calculate total expenses
            _currentData.TotalExpense = (_currentData.FoodTotal = _currentData.FoodTotal * 12) +
                                        _currentData.RentTotal +
                                        _currentData.Tax +
                                        _currentData.SubscriptionTotal +
                                        _currentData.CarTotal +
                                        _currentData.InsuranceTotal +
                                        _currentData.ShoppingTotal +
                                        _currentData.Other;

            // Update total label
            TotalExpenseLabel.Text = $"{_currentData.TotalExpense:F2} CHF / Jahr";

            // Save data to file
            try
            {
                var json = JsonSerializer.Serialize(_currentData, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, json);
                Console.WriteLine($"Data saved successfully to: {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        private decimal ParseDecimal(string input)
        {
            return decimal.TryParse(input, out var result) ? result : 0;
        }

        private decimal ParseTotal(string singleValue, string fromValue = null, string toValue = null)
        {
            if (!string.IsNullOrWhiteSpace(singleValue))
            {
                return ParseDecimal(singleValue);
            }
            else if (!string.IsNullOrWhiteSpace(fromValue) && !string.IsNullOrWhiteSpace(toValue))
            {
                return (ParseDecimal(fromValue) + ParseDecimal(toValue)) / 2;
            }

            return 0;
        }

        private void UpdateTotal()
        {
            try
            {
                decimal foodTotal = ParseTotal(FoodFromEntry.Text, FoodToEntry.Text);
                decimal rentTotal = ParseTotal(RentEntry.Text, RentFromEntry.Text, RentToEntry.Text);
                decimal tax = ParseDecimal(TaxEntry.Text);
                decimal subscriptionTotal = ParseDecimal(FunSubEntry.Text) +
                                            ParseDecimal(HealthSubEntry.Text) +
                                            ParseDecimal(MoreSubEntry.Text);
                decimal carTotal = ParseDecimal(FuelCarEntry.Text) +
                                   ParseDecimal(MaintainCarEntry.Text) +
                                   ParseDecimal(RepairCarEntry.Text);
                decimal insuranceTotal = ParseDecimal(HealthInsuranceEntry.Text) +
                                         ParseDecimal(LiabilityInsuranceEntry.Text) +
                                         ParseDecimal(CarInsuranceEntry.Text);
                decimal shoppingTotal = ParseDecimal(ClothesShopEntry.Text) +
                                        ParseDecimal(HauseholdShopEntry.Text) +
                                        ParseDecimal(MoreShopEntry.Text);
                decimal other = ParseDecimal(OtherExpensesEntry.Text);

                decimal total = foodTotal + rentTotal + tax + subscriptionTotal + carTotal + insuranceTotal + shoppingTotal + other;

                TotalExpenseLabel.Text = $"{total:F2} CHF / Jahr";
            }
            catch
            {
                TotalExpenseLabel.Text = "Error in calculation";
            }
        }
    }
}
