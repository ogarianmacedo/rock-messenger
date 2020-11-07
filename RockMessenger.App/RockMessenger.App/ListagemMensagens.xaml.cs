using RockMessenger.App.Models;
using RockMessenger.App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class ListagemMensagens : ContentPage
    {
        private Usuario _usuario { get; set; }
        private string _nomeGrupo { get; set; }

        public ListagemMensagens()
        {
            InitializeComponent();

            BtnEnviar.Clicked += async (sender, args) =>
            {
                var mensagem = Mensagem.Text;

                if (mensagem != null)
                {
                    await RockMessengerService.GetInstance().EnviarMensagem(
                        UsuarioManager.GetUsuarioLogado(),
                        mensagem,
                        _nomeGrupo
                    );

                    Mensagem.Text = string.Empty;
                }
                else
                {
                    await DisplayAlert("Atenção!", "Preencha o campo mensagem!", "OK");
                }
            };
        }

        public void SetUsuario(Usuario usuario)
        {
            _usuario = usuario;

            Title = usuario.Nome.FirstCharWordsToUpper();

            var emailUsuarioLogado = UsuarioManager.GetUsuarioLogado().Email;
            var emailUsuarioSelecionado = usuario.Email;

            Task.Run(async() => { await RockMessengerService.GetInstance().CriarAbrirGrupo(emailUsuarioLogado, emailUsuarioSelecionado); });
        }

        public void SetScrollOnBottom()
        {
            var ultimoItemMensagem = ListViewMensagens.ItemsSource.Cast<object>().LastOrDefault();
            ListViewMensagens.ScrollTo(
                ultimoItemMensagem,
                ScrollToPosition.MakeVisible,
                true
            );
        }

        public void SetNomeGrupo(string nomeGrupo)
        {
            _nomeGrupo = nomeGrupo;
        }

        public string GetNomeGrupo()
        {
            return _nomeGrupo;
        }
    }

    public class ListagemMensagensViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Mensagem> _mensagens;
        public ObservableCollection<Mensagem> Mensagens
        {
            get
            {
                return _mensagens;
            }
            set
            {
                _mensagens = value;
                NotifyPropertyChanged(nameof(Mensagens));
            }
        }

        public ListagemMensagensViewModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MensagemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EsquerdaTemplate { get; set; }
        public DataTemplate DireitaTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            Usuario usuarioLogado = UsuarioManager.GetUsuarioLogado();
            return ((Mensagem)item).Usuario.Id == usuarioLogado.Id ? DireitaTemplate : EsquerdaTemplate;
        }
    }
}