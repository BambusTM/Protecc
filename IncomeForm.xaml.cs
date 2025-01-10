using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Protecc.Models;

namespace Protecc
{
    public partial class IncomeForm : ContentView
    {
        private readonly string _filePath;

        public IncomeForm()
        {
            InitializeComponent();

            _filePath = Path.Combine(FileSystem.AppDataDirectory, "incomeData.json");

            LoadData();
        }

        private void OnIncomeChanged(object sender, TextChangedEventArgs e)
        {
            decimal lohn = ParseDecimal(LohnEntry.Text);
            decimal nebenerwerb = ParseDecimal(NebenerwerbEntry.Text);
            decimal sonstige = ParseDecimal(SonstigeEntry.Text);
            decimal fromValue = ParseDecimal(FromEntry.Text);
            decimal toValue = ParseDecimal(ToEntry.Text);

            decimal average = (fromValue != 0 && toValue != 0)
                ? (fromValue + toValue) / 2
                : fromValue + toValue;

            decimal gesamtwert = lohn + nebenerwerb + sonstige + average;

            GesamtWertLabel.Text = gesamtwert.ToString("F2") + " CHF";

            Console.WriteLine($"Saving to: {_filePath}");

            SaveData(new IncomeData
            {
                Lohn = lohn,
                Nebenerwerb = nebenerwerb,
                Sonstige = sonstige,
                FromValue = fromValue,
                ToValue = toValue,
                Gesamtwert = gesamtwert
            });
        }

        private decimal ParseDecimal(string input)
        {
            return decimal.TryParse(input, out var result) ? result : 0;
        }

        private void SaveData(IncomeData data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    var data = JsonSerializer.Deserialize<IncomeData>(json);

                    if (data != null)
                    {
                        LohnEntry.Text = data.Lohn.ToString();
                        NebenerwerbEntry.Text = data.Nebenerwerb.ToString();
                        SonstigeEntry.Text = data.Sonstige.ToString();
                        FromEntry.Text = data.FromValue.ToString();
                        ToEntry.Text = data.ToValue.ToString();
                        GesamtWertLabel.Text = data.Gesamtwert.ToString("F2") + " CHF";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
    }
}
