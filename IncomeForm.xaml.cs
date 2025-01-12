using System;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using Protecc.Models;

namespace Protecc
{
    public partial class IncomeForm : ContentView
    {
        private readonly string _filePath;
        private IncomeData _currentData;

        public IncomeForm()
        {
            InitializeComponent();

            _filePath = Path.Combine(FileSystem.AppDataDirectory, "incomeData.json");

            LoadData();
        }

        private async void PreviousPage(object sender, EventArgs e)
        {
            if (Application.Current.MainPage is NavigationPage navigationPage)
            {
                await navigationPage.Navigation.PushAsync(new HomePage());
            }
        }

        private void OnIncomeChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();

            decimal salary = ParseDecimal(SalaryEntry.Text);
            decimal sideline = ParseDecimal(SidelineEntry.Text);
            decimal other = ParseDecimal(OtherEntry.Text);
            decimal fromValue = ParseDecimal(FromEntry.Text);
            decimal toValue = ParseDecimal(ToEntry.Text);

            decimal average = (fromValue != 0 && toValue != 0)
                ? (fromValue + toValue) / 2
                : fromValue + toValue;

            decimal total = salary + sideline + other + average;

            TotalLabel.Text = total.ToString("F2") + " CHF";
        }

        private void ValidateInput()
        {
            SetValidationColor(SalaryEntry.Text, SalaryValidation);
            SetValidationColor(SidelineEntry.Text, SidelineValidation);
            SetValidationColor(OtherEntry.Text, OtherValidation);
            SetValidationColor(FromEntry.Text, FromValidation);
            SetValidationColor(ToEntry.Text, ToValidation);
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
            // Validate all inputs before saving
            if (!IsValidNumber(SalaryEntry.Text) ||
                !IsValidNumber(SidelineEntry.Text) ||
                !IsValidNumber(OtherEntry.Text) ||
                !IsValidNumber(FromEntry.Text) ||
                !IsValidNumber(ToEntry.Text))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ungültige Eingabe",
                    "Bitte überprüfen Sie alle Felder. Nur positive Zahlen mit bis zu zwei Dezimalstellen sind erlaubt.",
                    "OK");
                return;
            }

            _currentData ??= new IncomeData();

            _currentData.Salary = ParseDecimal(SalaryEntry.Text);
            _currentData.Sideline = ParseDecimal(SidelineEntry.Text);
            _currentData.Other = ParseDecimal(OtherEntry.Text);
            _currentData.FromValue = ParseDecimal(FromEntry.Text);
            _currentData.ToValue = ParseDecimal(ToEntry.Text);

            decimal average = (_currentData.FromValue != 0 && _currentData.ToValue != 0)
                ? (_currentData.FromValue + _currentData.ToValue) / 2
                : _currentData.FromValue + _currentData.ToValue;

            _currentData.TotalIncome = _currentData.Salary +
                                       _currentData.Sideline +
                                       _currentData.Other +
                                       average;

            TotalLabel.Text = _currentData.TotalIncome.ToString("F2") + " CHF";

            // Save to file
            try
            {
                var json = JsonSerializer.Serialize(_currentData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
                Console.WriteLine($"Data saved successfully to: {_filePath}");

                // Show confirmation toast
                await Application.Current.MainPage.DisplayAlert("Erfolg", "Daten wurden erfolgreich gespeichert.",
                    "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Fehler", "Daten konnten nicht gespeichert werden.",
                    "OK");
            }
        }

        private decimal ParseDecimal(string input)
        {
            return decimal.TryParse(input, out var result) ? result : 0;
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    _currentData = JsonSerializer.Deserialize<IncomeData>(json);

                    if (_currentData != null)
                    {
                        // Populate fields with existing data
                        SalaryEntry.Text = _currentData.Salary.ToString();
                        SidelineEntry.Text = _currentData.Sideline.ToString();
                        OtherEntry.Text = _currentData.Other.ToString();
                        FromEntry.Text = _currentData.FromValue.ToString();
                        ToEntry.Text = _currentData.ToValue.ToString();
                        TotalLabel.Text = _currentData.TotalIncome.ToString("F2") + " CHF";
                    }
                }
                else
                {
                    _currentData = new IncomeData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                _currentData = new IncomeData();
            }
        }
    }
}