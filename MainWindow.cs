using ChartDirector;
using System.IO.Ports;
using System.Windows;
using System.Windows.Threading;
using System.Globalization;

namespace WpfAppChartDirectorLinearMeter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort? serialPort;
        private bool isConnected = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateMeter(double voltage)
        {
            // Linear Meter für 0-5V Bereich
            LinearMeter m = new LinearMeter(70, 240, 0xeeeeee, 0xcccccc);
            m.setRoundedFrame(Chart.Transparent);
            m.setThickFrame(3);
            m.setMeter(28, 18, 20, 205);
            m.setScale(0, 5, 0.5);

            // Farbverlauf von Blau (0V) über Grün (2.5V) zu Rot (5V)
            double[] smoothColorScale = {
                0, 0x0000ff,      // 0V: Blau
                1.25, 0x00bbbb,   // 1.25V: Cyan
                2.5, 0x00ff00,    // 2.5V: Grün
                3.75, 0xffff00,   // 3.75V: Gelb
                5, 0xff0000       // 5V: Rot
            };
            m.addColorScale(smoothColorScale);
            m.addPointer(voltage, 0x0000cc);
            WPFChartViewer1.Chart = m;

            VoltageText.Text = $"{voltage:F3} V";
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            UpdateMeter(0);
            RefreshPorts_Click(this, new RoutedEventArgs());
        }

        private void RefreshPorts_Click(object sender, RoutedEventArgs e)
        {
            PortCombo.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                PortCombo.Items.Add(port);
            }
            if (PortCombo.Items.Count > 0)
            {
                PortCombo.SelectedIndex = 0;
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                if (PortCombo.SelectedItem == null)
                {
                    MessageBox.Show("Bitte wählen Sie einen Port aus.", "Fehler",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                try
                {
                    string portName = PortCombo.SelectedItem.ToString()!;
                    serialPort = new SerialPort(portName, 9600);
                    serialPort.NewLine = "\n";
                    serialPort.DataReceived += SerialPort_DataReceived;
                    serialPort.Open();

                    isConnected = true;
                    ConnectButton.Content = "Disconnect";
                    PortCombo.IsEnabled = false;
                    RefreshPortsButton.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Verbinden: {ex.Message}", "Fehler",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                DisconnectPort();
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    string data = serialPort.ReadLine().Trim();
                    if (double.TryParse(data, NumberStyles.Float,
                        CultureInfo.InvariantCulture, out double voltage))
                    {
                        Dispatcher.Invoke(() => UpdateMeter(voltage));
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Fehler beim Lesen: {ex.Message}", "Fehler",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    DisconnectPort();
                });
            }
        }

        private void DisconnectPort()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.Dispose();
                serialPort = null;
            }

            isConnected = false;
            ConnectButton.Content = "Connect";
            PortCombo.IsEnabled = true;
            RefreshPortsButton.IsEnabled = true;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            DisconnectPort();
            base.OnClosing(e);
        }
    }
}