using RockMessenger.App.Models;
using RockMessenger.App.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RockMessenger.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListagemUsuarios : ContentPage
    {
        public ListagemUsuarios()
        {
            InitializeComponent();

            BtnSair.Clicked += async (sender, args) =>
            {
                await RockMessengerService.GetInstance().Sair(UsuarioManager.GetUsuarioLogado());
                UsuarioManager.DelUsuarioLogado();

                App.Current.MainPage = new Principal();
            };

            Listagem.ItemTapped += (sender, args) =>
            {
                Usuario usuario = (Usuario)args.Item;

                var listagemMensagens = new ListagemMensagens();
                listagemMensagens.SetUsuario(usuario);

                Navigation.PushAsync(listagemMensagens);
            };

            Task.Run(async() => { await RockMessengerService.GetInstance().ObterListaUsuarios(); });
        }
    }

    public class ListagemUsuariosViewModel : INotifyPropertyChanged
    {
        private List<Usuario> _usuarios;
        public List<Usuario> Usuarios { 
            get {
                return _usuarios;
            }
            set {
                _usuarios = value;
                NotifyPropertyChanged(nameof(Usuarios));
            }
        }

        public ListagemUsuariosViewModel()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}