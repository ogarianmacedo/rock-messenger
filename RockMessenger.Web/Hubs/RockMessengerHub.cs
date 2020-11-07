using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RockMessenger.WebApi.Database;
using RockMessenger.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockMessenger.Web.Hubs
{
    public class RockMessengerHub : Hub
    {
        private Context _context;

        public RockMessengerHub(Context context)
        {
            _context = context;
        }

        public async Task Cadastrar(Usuario usuario)
        {
            bool isUser = _context.Usuarios.Where(u => u.Email == usuario.Email).Count() > 0;
            
            if (isUser)
            {
                await Clients.Caller.SendAsync(
                    "ReceberCadastro", 
                    false, 
                    null, 
                    "E-mail já cadastrado!"
                );
            }
            else
            {
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                await Clients.Caller.SendAsync(
                   "ReceberCadastro",
                   true,
                   usuario,
                   "Cadastrado com sucesso!"
               );
            }
        }

        public async Task Login(Usuario usuario)
        {
            Usuario usuarioDB = _context.Usuarios.FirstOrDefault(u => u.Email == usuario.Email && u.Senha == usuario.Senha);

            if (usuarioDB == null)
            {
                await Clients.Caller.SendAsync(
                    "ReceberLogin",
                    false,
                    null,
                    "E-mail ou senha incorretos!"
                );
            }
            else
            {
                await Clients.Caller.SendAsync(
                   "ReceberLogin",
                   true,
                   usuarioDB,
                   ""
                );

                usuarioDB.IsOnline = true;

                _context.Usuarios.Update(usuarioDB);
                _context.SaveChanges();

                await NotificarMudancaListaUsuarios();
            }
        }

        public async Task Logout(Usuario usuario)
        {
            var usuarioDB = _context.Usuarios.Find(usuario.Id);
            usuarioDB.IsOnline = false;

            _context.Usuarios.Update(usuarioDB);
            _context.SaveChanges();

            await DelConnectionIdUsuario(usuarioDB);
            await NotificarMudancaListaUsuarios();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.ConnectionId.Contains(Context.ConnectionId));

            if (usuario != null)
            {
                await DelConnectionIdUsuario(usuario);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddConnectionIdUsuario(Usuario usuario)
        {
            var connectionIdCurrent = Context.ConnectionId;
            List<string> connectionsId = null;

            Usuario usuarioDB = _context.Usuarios.Find(usuario.Id);

            if (usuarioDB.ConnectionId == null)
            {
                connectionsId = new List<string>();
                connectionsId.Add(connectionIdCurrent);
            }
            else
            {
                connectionsId = JsonConvert.DeserializeObject<List<string>>(usuarioDB.ConnectionId);

                if (!connectionsId.Contains(connectionIdCurrent))
                {
                    connectionsId.Add(connectionIdCurrent);
                }
            }

            usuarioDB.IsOnline = true;
            usuarioDB.ConnectionId = JsonConvert.SerializeObject(connectionsId);

            _context.Usuarios.Update(usuarioDB);
            _context.SaveChanges();

            await NotificarMudancaListaUsuarios();

            var grupos = _context.Grupos.Where(g => g.Usuarios.Contains(usuarioDB.Email));
            foreach (var connectionId in connectionsId)
            {
                foreach (var grupo in grupos)
                {
                    await Groups.AddToGroupAsync(connectionId, grupo.Nome);
                }
            }
        }

        public async Task DelConnectionIdUsuario(Usuario usuario)
        {
            Usuario usuarioDB = _context.Usuarios.Find(usuario.Id);

            List<string> connectionsId = null;

            if (usuarioDB.ConnectionId.Length > 0)
            {
                var connectionIdCurrent = Context.ConnectionId;

                connectionsId = JsonConvert.DeserializeObject<List<string>>(usuarioDB.ConnectionId);
                if (connectionsId.Contains(connectionIdCurrent))
                {
                    connectionsId.Remove(connectionIdCurrent);
                }

                usuarioDB.ConnectionId = JsonConvert.SerializeObject(connectionsId);
                if (connectionsId.Count <= 0)
                {
                    usuarioDB.IsOnline = false;
                }

                _context.Usuarios.Update(usuarioDB);
                _context.SaveChanges();

                await NotificarMudancaListaUsuarios();

                var grupos = _context.Grupos.Where(g => g.Usuarios.Contains(usuarioDB.Email));
                foreach (var connectionId in connectionsId)
                {
                    foreach (var grupo in grupos)
                    {
                        await Groups.RemoveFromGroupAsync(connectionId, grupo.Nome);
                    }
                }
            }
        }

        public async Task ObterListaUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            await Clients.Caller.SendAsync("ReceberListaUsuarios", usuarios);
        }

        public async Task NotificarMudancaListaUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            await Clients.All.SendAsync("ReceberListaUsuarios", usuarios);
        }

        public async Task CriarAbrirGrupo(string emailUsuarioLogado, string emailUsuarioSelecionado)
        {
            string nomeGrupo = CriarNomeGrupo(emailUsuarioLogado, emailUsuarioSelecionado);

            Grupo grupoDB = _context.Grupos.FirstOrDefault(g => g.Nome == nomeGrupo);

            if (grupoDB == null)
            {
                grupoDB = new Grupo();
                grupoDB.Nome = nomeGrupo;
                grupoDB.Usuarios = JsonConvert.SerializeObject(new List<string>()
                {
                    emailUsuarioLogado,
                    emailUsuarioSelecionado
                });

                _context.Grupos.Add(grupoDB);
                _context.SaveChanges();
            }

            List<string> emails = JsonConvert.DeserializeObject<List<string>>(grupoDB.Usuarios);
            List<Usuario> usuarios = new List<Usuario>()
            {
                _context.Usuarios.First(u => u.Email == emails[0]),
                _context.Usuarios.First(u => u.Email == emails[1])
            };

            foreach (var usuario in usuarios)
            {
                if(usuario.ConnectionId == null)
                {
                    usuario.ConnectionId = "[]";
                }

                var connectionsId = JsonConvert.DeserializeObject<List<string>>(usuario.ConnectionId);
                foreach (var connectionId in connectionsId)
                {
                    await Groups.AddToGroupAsync(connectionId, nomeGrupo);
                }
            }

            var mensagens = _context.Mensagens
                .Where(m => m.NomeGrupo == nomeGrupo)
                .OrderBy(m => m.DataCriacao)
                .ToList();

            for (int i =0; i < mensagens.Count; i++)
            {
                mensagens[i].Usuario = JsonConvert.DeserializeObject<Usuario>(mensagens[i].UsuarioJson);
            }

            await Clients.Caller.SendAsync("AbrirGrupo", nomeGrupo, mensagens);
        }

        public async Task EnviarMensagem(Usuario usuario, string msg, string nomeGrupo)
        {
            Grupo grupo = _context.Grupos.FirstOrDefault(g => g.Nome == nomeGrupo);
            
            if (!grupo.Usuarios.Contains(usuario.Email))
            {
                throw new Exception("Usuário não pertence ao grupo!");
            }

            Mensagem mensagem = new Mensagem();
            mensagem.NomeGrupo = nomeGrupo;
            mensagem.Texto = msg;
            mensagem.UsuarioId = usuario.Id;
            mensagem.UsuarioJson = JsonConvert.SerializeObject(usuario);
            mensagem.Usuario = usuario;
            mensagem.DataCriacao = DateTime.Now;

            _context.Mensagens.Add(mensagem);
            _context.SaveChanges();

            await Clients.Group(nomeGrupo).SendAsync("ReceberMensagem", mensagem, nomeGrupo);
        }

        private string CriarNomeGrupo(string emailUsuarioLogado, string emailUsuarioSelecionado)
        {
            List<string> lista = new List<string>() { emailUsuarioLogado, emailUsuarioSelecionado };
            var listaOrdenada = lista.OrderBy(u => u).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var item in listaOrdenada)
            {
                sb.Append(item);
            }

            return sb.ToString();
        }
    }
}
