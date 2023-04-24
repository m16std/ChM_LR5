namespace WpfApplication1
{
    using System.Windows;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Main _main = new Main();
            TextBox1.Text = _main.some_pice_of_shit;
        }
    }
}