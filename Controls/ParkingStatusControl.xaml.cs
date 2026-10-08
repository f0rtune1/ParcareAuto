using System.Windows;
using System.Windows.Controls;

namespace ParcareAuto.Controls;

public partial class ParkingStatusControl : UserControl
{
    public static readonly DependencyProperty OreStationareProperty =
        DependencyProperty.Register(nameof(OreStationare), typeof(int),
            typeof(ParkingStatusControl),
            new PropertyMetadata(0, OnOreStationareChanged));

    public int OreStationare
    {
        get => (int)GetValue(OreStationareProperty);
        set => SetValue(OreStationareProperty, value);
    }

    public ParkingStatusControl()
    {
        InitializeComponent();
        ActualizeazaStatus();
    }

    private static void OnOreStationareChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        ((ParkingStatusControl)d).ActualizeazaStatus();
    }

    private void ActualizeazaStatus()
    {
        if (TextStatus == null) return;

        if (OreStationare <= 2)
            TextStatus.Text = "Stationare scurta";
        else if (OreStationare <= 6)
            TextStatus.Text = "Stationare normala";
        else
            TextStatus.Text = "Stationare indelungata";
    }
}
