namespace Silabaco
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Pages.Setting), typeof(Pages.Setting));
        }
    }
}