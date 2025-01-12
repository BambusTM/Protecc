using System;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;
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

        private async void SaveData(object sender, EventArgs e)
        {
            _currentData ??= new IncomeData();

            _currentData.Salary = !string.IsNullOrWhiteSpace(SalaryEntry.Text)
                ? ParseDecimal(SalaryEntry.Text)
                : _currentData.Salary;

            _currentData.Sideline = !string.IsNullOrWhiteSpace(SidelineEntry.Text)
                ? ParseDecimal(SidelineEntry.Text)
                : _currentData.Sideline;

            _currentData.Other = !string.IsNullOrWhiteSpace(OtherEntry.Text)
                ? ParseDecimal(OtherEntry.Text)
                : _currentData.Other;

            _currentData.FromValue = !string.IsNullOrWhiteSpace(FromEntry.Text)
                ? ParseDecimal(FromEntry.Text)
                : _currentData.FromValue;

            _currentData.ToValue = !string.IsNullOrWhiteSpace(ToEntry.Text)
                ? ParseDecimal(ToEntry.Text)
                : _currentData.ToValue;

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
