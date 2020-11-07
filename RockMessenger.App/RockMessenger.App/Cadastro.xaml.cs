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
    public partial class Cadastro : ContentPage
    {
        public Cadastro()
        {
            InitializeComponent();

            BtnCadastrar.Clicked += async (sender, args) =>
            {
                string nome = Nome.Text;
                string email = Email.Text;
                string senha = Senha.Text;

                if (nome == null && email == null && senha == null)
                {
                    await DisplayAlert("Atenção!", "Preencha os campos do cadastro!", "OK");
                }
                else
                {
                    Usuario usuario = new Usuario()
                    {
                        Nome = nome,
                        Email = email,
                        Senha = senha
                    };

                    Mensagem.Text = string.Empty;
                    BtnCadastrar.IsEnabled = false;
                    Carregando.IsRunning = true;

                    await RockMessengerService.GetInstance().Cadastrar(usuario);
                }
            };
        }

        public void SetMensagem(string msg, bool isErro)
        {
            Mensagem.TextColor = (isErro) ? Color.Red : Color.White;
            Mensagem.Text = msg;
            BtnCadastrar.IsEnabled = true;
            Carregando.IsRunning = false;

            DisplayAlert("Atenção!", msg, "OK");

            if (!isErro)
            {
                Nome.Text = string.Empty;
                Email.Text = string.Empty;
                Senha.Text = string.Empty;
            }
        }
    }
}