using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockMessenger.App.Models
{
    public class Mensagem
    {
        public int Id { get; set; }
        public string NomeGrupo { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public string UsuarioJson { get; set; }
        public string Texto { get; set; }
        public DateTime? DataCriacao { get; set; }
    }
}
