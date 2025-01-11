using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Protecc
{
    public partial class ExpenseForm : ContentView
    {
        // Bindable properties for toggling visibility
        public static readonly BindableProperty IsAutoExpandedProperty =
            BindableProperty.Create(nameof(IsAutoExpanded), typeof(bool), typeof(ExpenseForm), false);

        public static readonly BindableProperty IsInsuranceExpandedProperty =
            BindableProperty.Create(nameof(IsInsuranceExpanded), typeof(bool), typeof(ExpenseForm), false);

        // Properties
        public bool IsAutoExpanded
        {
            get => (bool)GetValue(IsAutoExpandedProperty);
            set => SetValue(IsAutoExpandedProperty, value);
        }

        public bool IsInsuranceExpanded
        {
            get => (bool)GetValue(IsInsuranceExpandedProperty);
            set => SetValue(IsInsuranceExpandedProperty, value);
        }

        // Commands for toggling sections
        public ICommand ToggleAutoCommand { get; }
        public ICommand ToggleInsuranceCommand { get; }

        public ExpenseForm()
        {
            InitializeComponent();

            // Initialize commands
            ToggleAutoCommand = new Command(() =>
            {
                IsAutoExpanded = !IsAutoExpanded;
            });

            ToggleInsuranceCommand = new Command(() =>
            {
                IsInsuranceExpanded = !IsInsuranceExpanded;
            });

            // Set BindingContext for data bindings
            BindingContext = this;
        }

        // Handles changes in expense inputs and updates the total
        private void OnExpenseChanged(object sender, TextChangedEventArgs e)
        {
            decimal carFuel = ParseDecimal(CarFuelEntry.Text);
            decimal carMaintenance = ParseDecimal(CarMaintenanceEntry.Text);
            decimal carRepair = ParseDecimal(CarRepairEntry.Text);

            decimal healthInsurance = ParseDecimal(HealthInsuranceEntry.Text);
            decimal liabilityInsurance = ParseDecimal(LiabilityInsuranceEntry.Text);
            decimal carInsurance = ParseDecimal(CarInsuranceEntry.Text);

            decimal otherExpenses = ParseDecimal(OtherExpensesEntry.Text);

            // Calculate total expenses
            decimal totalExpenses = carFuel + carMaintenance + carRepair +
                                    healthInsurance + liabilityInsurance + carInsurance +
                                    otherExpenses;

            // Update total label
            TotalExpenseLabel.Text = $"{totalExpenses:F2} CHF/m";
        }

        // Helper method to parse decimal values
        private decimal ParseDecimal(string input)
        {
            return decimal.TryParse(input, out var result) ? result : 0;
        }
    }
}
