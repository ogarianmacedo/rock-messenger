using RockMessenger.App.Models;
using RockMessenger.App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RockMessenger.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();

            BtnEntrar.Clicked += async (sender, args) =>
            {
                string email = Email.Text;
                string senha = Senha.Text;

                if (email == null && senha == null)
                {
                    await DisplayAlert("Atenção!", "Preencha os campos!", "OK");
                } 
                else
                {
                    Usuario usuario = new Usuario { Email = email, Senha = senha };

                    Mensagem.Text = string.Empty;
                    BtnEntrar.IsEnabled = false;
                    Carregando.IsRunning = true;

                    await RockMessengerService.GetInstance().Login(usuario);
                }
            };
        }

        public void SetMensagem(string msg)
        {
            Mensagem.Text = msg;
            BtnEntrar.IsEnabled = true;
            Carregando.IsRunning = false;

            DisplayAlert("Atenção!", msg, "OK");
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            ((Principal)App.Current.MainPage).CurrentPage = ((Principal)App.Current.MainPage).Children[1];
        }
    }
}