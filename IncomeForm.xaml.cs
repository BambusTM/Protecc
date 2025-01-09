namespace Protecc;

public partial class IncomeForm : ContentView
{
    public IncomeForm()
    {
        InitializeComponent();
    }
    
    private void OnIncomeChanged(object sender, TextChangedEventArgs e)
    {
        decimal lohn = ParseDecimal(LohnEntry.Text);
        decimal nebenerwerb = ParseDecimal(NebenerwerbEntry.Text);
        decimal sonstige = ParseDecimal(SonstigeEntry.Text);
        decimal fromValue = ParseDecimal(FromEntry.Text);
        decimal toValue = ParseDecimal(ToEntry.Text);

        decimal average;
        
        if (fromValue != 0 && toValue != 0)
        {
            average = (fromValue + toValue) / 2;
        }
        else
        {
            average = fromValue + toValue;
        }

        // total
        decimal gesamtwert = lohn + nebenerwerb + sonstige + average;

        GesamtWertLabel.Text = gesamtwert.ToString("F2") + " CHF";
    }

    private decimal ParseDecimal(string input)
    {
        if (decimal.TryParse(input, out var result))
            return result;
        return 0;
    }
}