using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserClient.HolePunching;

namespace UserClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HolePunchingUdpSocket holePunchingSocket;

        public MainWindow()
        {
            InitializeComponent();

            holePunchingSocket = new HolePunchingUdpSocket();
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex("^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}$");  // IP address match check
            if (regex.IsMatch(textBoxIp.Text) == false)
            {
                MessageBox.Show("Invalid IP address",
                                "Invalid Value",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (Int32.TryParse(textBoxPort.Text, out int port) == false)
            {
                MessageBox.Show("Invalid port number",
                                "Invalid Value",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (holePunchingSocket.Connect(textBoxIp.Text, port) == false)
            {
                MessageBox.Show("Failed to connect socket. Check the IP address and port is valid",
                                "Connection Failed",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (holePunchingSocket.Send("Hello World!") == false)
            {
                MessageBox.Show("Failed to send data to target",
                                "Failed",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
        }

        private void TextBoxNumberValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsNumber(e.Text[0]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            holePunchingSocket.Dispose();
        }

        private void buttonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            holePunchingSocket.Disconnect();
        }
    }
}
