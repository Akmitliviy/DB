using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScaffoldDB_WPF.Context;
using ScaffoldDB_WPF.Migrations;

namespace ScaffoldDB_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MyDbContext _context;
    public MainWindow()
    {
        InitializeComponent();
        _context = new MyDbContext(); // Replace with your DbContext class
        LoadClients();
    }

    private void LoadClients()
    {
        ClientsDataGrid.ItemsSource = _context.Clients.ToList();
    }
    private void AddClient_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var newClient = new Client
            {
                Email = ClientEmailTextBox.Text,
                FirstName = ClientFirstNameTextBox.Text,
                LastName = "DefaultLastName", // Add other fields
                PhoneNumber = "DefaultPhone", // Example default
                BirthDate = DateOnly.FromDateTime(DateTime.Now) // Example default
            };

            _context.Clients.Add(newClient);
            _context.SaveChanges();
            LoadClients();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }
    
    private void DeleteClient_Click(object sender, RoutedEventArgs e)
    {
        if (ClientsDataGrid.SelectedItem is Client selectedClient)
        {
            _context.Clients.Remove(selectedClient);
            _context.SaveChanges();
            LoadClients();
        }
        else
        {
            MessageBox.Show("Select a client to delete.");
        }
    }

    private void RefreshClients_Click(object sender, RoutedEventArgs e)
    {
        LoadClients();
    }

    

}