using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ParcareAuto.Models;

namespace ParcareAuto;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<Automobil> automobile = new();
    private readonly ObservableCollection<Automobil> automobileFiltrate = new();
    private readonly string[] tipuri = { "Autoturism", "SUV", "Microbuz", "Electric" };
    private bool actualizareSelectie;

    public MainWindow()
    {
        InitializeComponent();
        AdaugaDateInitiale();
        AutomobileGrid.ItemsSource = automobile;
        AutomobileList.ItemsSource = automobileFiltrate;
        TipNouCombo.ItemsSource = tipuri;
        TipNouCombo.SelectedIndex = 0;
        OraText.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        TipFiltruCombo.ItemsSource = tipuri;
        TipFiltruCombo.SelectedIndex = 0;
    }

    private void AdaugaDateInitiale()
    {
        automobile.Add(CreeazaAutomobil("ABC 123", "Dacia", "Autoturism", 2, 10));
        automobile.Add(CreeazaAutomobil("DEF 456", "Toyota", "SUV", 5, 15));
        automobile.Add(CreeazaAutomobil("GHI 789", "Mercedes", "Microbuz", 8, 20));
        automobile.Add(CreeazaAutomobil("JKL 101", "Tesla", "Electric", 1, 12));
        automobile.Add(CreeazaAutomobil("MNO 202", "Skoda", "Autoturism", 4, 10));
        automobile.Add(CreeazaAutomobil("PQR 303", "BMW", "SUV", 7, 18));
        automobile.Add(CreeazaAutomobil("STU 404", "Ford", "Microbuz", 6, 20));
        automobile.Add(CreeazaAutomobil("VWX 505", "Nissan", "Electric", 3, 12));
    }

    private static Automobil CreeazaAutomobil(string numar, string marca, string tip, int ore, decimal tarif)
    {
        return new Automobil
        {
            NumarInmatriculare = numar, Marca = marca, Tip = tip,
            OraIntrare = DateTime.Now.AddHours(-ore), OreStationare = ore, TarifOra = tarif
        };
    }

    private void TipFiltruCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (actualizareSelectie) return;
        FiltreazaAutomobile();
    }

    private void FiltreazaAutomobile()
    {
        actualizareSelectie = true;
        automobileFiltrate.Clear();
        foreach (Automobil automobil in automobile)
            if (automobil.Tip == TipFiltruCombo.SelectedItem as string)
                automobileFiltrate.Add(automobil);
        actualizareSelectie = false;
        AutomobileList.SelectedIndex = -1;
        AfiseazaAutomobil(null);
    }

    private void AutomobileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (actualizareSelectie) return;
        AfiseazaAutomobil(AutomobileList.SelectedItem as Automobil);
    }

    private void AfiseazaAutomobil(Automobil? automobil)
    {
        DetaliiPanel.DataContext = automobil;
        DetaliiPanel.Visibility = automobil == null ? Visibility.Collapsed : Visibility.Visible;
        SelectieMesaj.Visibility = automobil == null ? Visibility.Visible : Visibility.Collapsed;
        EliminaButton.IsEnabled = automobil != null;
        actualizareSelectie = true;
        AutomobileGrid.SelectedItem = automobil;
        actualizareSelectie = false;
    }

    private void AutomobileGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (actualizareSelectie || AutomobileGrid.SelectedItem is not Automobil automobil) return;
        SelecteazaAutomobil(automobil);
    }

    private void SelecteazaAutomobil(Automobil automobil)
    {
        TipFiltruCombo.SelectedItem = automobil.Tip;
        AutomobileList.SelectedItem = automobil;
        AfiseazaAutomobil(automobil);
    }

    private void AdaugaButton_Click(object sender, RoutedEventArgs e)
    {
        string numar = NumarText.Text.Trim().ToUpperInvariant();
        if (numar.Length == 0)
        {
            ValidareText.Text = "Numarul de inmatriculare nu poate fi gol.";
            return;
        }
        if (!int.TryParse(OreText.Text, out int ore) || ore <= 0)
        {
            ValidareText.Text = "Numarul orelor trebuie sa fie un numar intreg pozitiv.";
            return;
        }
        // Sunt acceptate atat virgula, cat si punctul pentru zecimale.
        if (!decimal.TryParse(TarifText.Text.Trim().Replace(',', '.'),
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture, out decimal tarif) || tarif <= 0)
        {
            ValidareText.Text = "Tariful trebuie sa fie un numar mai mare decat 0.";
            return;
        }
        if (!DateTime.TryParseExact(OraText.Text.Trim(), "dd.MM.yyyy HH:mm",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime ora))
        {
            ValidareText.Text = "Ora intrarii trebuie sa aiba formatul zz.ll.aaaa hh:mm.";
            return;
        }
        if (TipNouCombo.SelectedItem is not string tip)
        {
            ValidareText.Text = "Selectati tipul automobilului.";
            return;
        }
        // Evitam doua automobile cu acelasi numar in parcare.
        foreach (Automobil existent in automobile)
            if (string.Equals(existent.NumarInmatriculare, numar, StringComparison.OrdinalIgnoreCase))
            {
                ValidareText.Text = "Exista deja un automobil cu acest numar.";
                return;
            }

        Automobil automobil = new()
        {
            NumarInmatriculare = numar, Marca = MarcaText.Text.Trim(), Tip = tip,
            OraIntrare = ora, OreStationare = ore, TarifOra = tarif
        };
        automobile.Add(automobil);
        FiltreazaAutomobile();
        SelecteazaAutomobil(automobil);
        ValidareText.Text = "";
        NumarText.Clear();
        MarcaText.Clear();
        OreText.Clear();
        TarifText.Clear();
        OraText.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
    }

    private void EliminaButton_Click(object sender, RoutedEventArgs e)
    {
        if (AutomobileList.SelectedItem is not Automobil automobil) return;
        MessageBoxResult raspuns = MessageBox.Show(this,
            $"Doriti sa eliminati automobilul {automobil.NumarInmatriculare}?",
            "Confirmare eliminare", MessageBoxButton.YesNo, MessageBoxImage.Question,
            MessageBoxResult.No);
        if (raspuns != MessageBoxResult.Yes) return;

        automobile.Remove(automobil);
        FiltreazaAutomobile();
    }
}
