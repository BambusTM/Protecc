using System;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
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
            ValidateInput();

            UpdateTotal();
        }

        private void ValidateInput()
        {
            SetValidationColor(FoodFromEntry.Text, FoodFromValidation);
            SetValidationColor(FoodToEntry.Text, FoodToValidation);

            SetValidationColor(RentEntry.Text, RentValidation);
            SetValidationColor(RentFromEntry.Text, RentFromValidation);
            SetValidationColor(RentToEntry.Text, RenToValidation);

            SetValidationColor(FunSubEntry.Text, FunSubValidation);
            SetValidationColor(HealthSubEntry.Text, HealthSubValidation);
            SetValidationColor(MoreSubEntry.Text, MoreSubValidation);

            SetValidationColor(FuelCarEntry.Text, FuelCarValidation);
            SetValidationColor(MaintainCarEntry.Text, MaintainCarValidation);
            SetValidationColor(RepairCarEntry.Text, RepairCarValidation);

            SetValidationColor(HealthInsuranceEntry.Text, HealtInsuranceValidation);
            SetValidationColor(LiabilityInsuranceEntry.Text, LiabilityInsuranceValidation);
            SetValidationColor(CarInsuranceEntry.Text, CarInsuranceValidation);

            SetValidationColor(ClothesShopEntry.Text, ClothesShopValidation);
            SetValidationColor(HauseholdShopEntry.Text, HouseholdShopValidation);
            SetValidationColor(MoreShopEntry.Text, MoreShopValidation);

            SetValidationColor(OtherExpensesEntry.Text, OtherEValidation);
        }

        private void SetValidationColor(string input, BoxView validationBox)
        {
            if (string.IsNullOrWhiteSpace(input) || !IsValidNumber(input))
            {
                validationBox.Color = Colors.Red;
            }
            else
            {
                validationBox.Color = Colors.Green;
            }
        }

        private bool IsValidNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true; // Allow null or empty inputs

            return decimal.TryParse(input, out var result) && result >= 0 && HasMaxTwoDecimalPlaces(input);
        }

        private bool HasMaxTwoDecimalPlaces(string input)
        {
            int decimalIndex = input.IndexOf('.');
            if (decimalIndex == -1)
                return true;

            string decimalPart = input.Substring(decimalIndex + 1);
            return decimalPart.Length <= 2;
        }

        private async void SaveData(object sender, EventArgs e)
        {
            // Validate inputs first
            if (!ValidateInputs())
            {
                // Display error message if inputs are invalid
                await Application.Current.MainPage.DisplayAlert("Fehler",
                    "Bitte stellen Sie sicher, dass alle Eingaben gültige Zahlen enthalten.", "OK");
                return; // Don't proceed with saving
            }

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
            _currentData.TotalExpense = (_currentData.FoodTotal * 12) +
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
                // Show success message
                await Application.Current.MainPage.DisplayAlert("Erfolg", "Die Daten wurden erfolgreich gespeichert.",
                    "OK");
            }
            catch (Exception ex)
            {
                // Show error message in case of any issue while saving
                await Application.Current.MainPage.DisplayAlert("Fehler",
                    $"Fehler beim Speichern der Daten: {ex.Message}", "OK");
            }
        }

        private decimal ParseDecimal(string input)
        {
            return decimal.TryParse(input, out var result) ? result : -1; // Invalid input returns -1
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

        private bool ValidateInputs()
        {
            // Validate all numeric fields
            string[] inputs =
            {
                FoodFromEntry.Text, FoodToEntry.Text, RentEntry.Text,
                RentFromEntry.Text, RentToEntry.Text, TaxEntry.Text,
                FunSubEntry.Text, HealthSubEntry.Text, MoreSubEntry.Text,
                FuelCarEntry.Text, MaintainCarEntry.Text, RepairCarEntry.Text,
                HealthInsuranceEntry.Text, LiabilityInsuranceEntry.Text, CarInsuranceEntry.Text,
                ClothesShopEntry.Text, HauseholdShopEntry.Text, MoreShopEntry.Text,
                OtherExpensesEntry.Text
            };

            foreach (var input in inputs)
            {
                if (!string.IsNullOrWhiteSpace(input) && ParseDecimal(input) == -1)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateTotal()
        {
            if (!ValidateInputs())
            {
                TotalExpenseLabel.Text = "Ungültige Eingabe";
                return;
            }

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

                decimal total = foodTotal * 12 + rentTotal + tax + subscriptionTotal + carTotal + insuranceTotal +
                                shoppingTotal + other;

                TotalExpenseLabel.Text = $"{total:F2} CHF / Jahr";
            }
            catch
            {
                TotalExpenseLabel.Text = "Fehler bei der Berechnung";
            }
        }
    }
}