using RockMessenger.App.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RockMessenger.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Principal();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            Task.Run(async() => { await RockMessengerService.GetInstance().Sair(UsuarioManager.GetUsuarioLogado()); });
        }

        protected override void OnResume()
        {
            Task.Run(async () => { await RockMessengerService.GetInstance().Entrar(UsuarioManager.GetUsuarioLogado()); });
        }
    }
}
