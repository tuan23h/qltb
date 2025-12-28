using System.Windows;

namespace qltb
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries_V2.Init();
            base.OnStartup(e);
        }
    }
}
