/**
 * Conexão e Reconexão com o SignalR - Hub
 */
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/RockMessengerHub")
    .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
    .build();

function ConnectionStart() {
    connection.start().then(function () {
        HabilitarLogin();
        HabilitarCadastro();
        HabilitarMensagens();

        console.info("Conectado!");
    }).catch(function (erro) {
        if (connection.state == 0) {
            console.error(erro.toString());
            setTimeout(ConnectionStart, 5000);
        }
    });
}

connection.onclose(async () => { await ConnectionStart(); });

/**
 * Global 
 */
var nomeGrupo = "";

/**
 *  Cadastro de usuário
 */
function HabilitarCadastro() {
    var formCadastro = document.getElementById("form-cadastro");

    if (formCadastro != null) {
        var btnCadastrar = document.getElementById("btn-cadastrar");

        btnCadastrar.addEventListener("click", function () {
            var nome = document.getElementById("nome").value;
            var email = document.getElementById("email").value;
            var senha = document.getElementById("senha").value;

            if (nome == '' || email == '' || senha == '') {
                toastr.info('Preencha os campos corretamente!', "Atenção!");
                return false;
            }

            var usuario = {
                Nome: nome,
                Email: email,
                Senha: senha
            };

            connection.invoke("Cadastrar", usuario).then(function () {
                //console.info("Cadastrado com sucesso!");
            }).catch(function (erro) {
                console.error(erro.toString());
            });
        });
    }

    connection.on("ReceberCadastro", function (sucesso, usuario, msg) {
        if (sucesso) {
            document.getElementById("nome").value = "";
            document.getElementById("email").value = "";
            document.getElementById("senha").value = "";
        }

        toastr.info(msg, "Atenção!");
    });
}

/**
 *  Login de usuário
 */
function HabilitarLogin() {
    var formLogin = document.getElementById("form-login");

    if (formLogin != null) {
        if (GetUsuarioLogado() != null) {
            window.location.href = "/Home/Mensagens";
        }

        var btnEntrar = document.getElementById("btn-entrar");

        btnEntrar.addEventListener("click", function () {
            var email = document.getElementById("email").value;
            var senha = document.getElementById("senha").value;

            if (email == '' || senha == '') {
                toastr.info('Preencha os campos corretamente!', "Atenção!");
                return false;
            }

            var usuario = {
                Email: email,
                Senha: senha
            };

            connection.invoke("Login", usuario).then(function () {
                //console.info("");
            }).catch(function (erro) {
                console.error(erro.toString());
            });;
        });
    }

    connection.on("ReceberLogin", function (sucesso, usuario, msg) {
        if (sucesso) {
            SetUsuarioLogado(usuario);
            window.location.href = "/Home/Mensagens";
        } else {
            toastr.info(msg, "Atenção!");
            return false;
        }
    });
}

/**
 * Validações do usuário logado
 */
var telaMensagens = document.getElementById("tela-mensagens");
if (telaMensagens != null) {
    if (GetUsuarioLogado() == null) {
        window.location.href = "/Home/Login";
    }
}

/**
 *  Habilitação da tela de mensagens
 */
function HabilitarMensagens() {
    var telaMensagens = document.getElementById("tela-mensagens");
    if (telaMensagens != null) {
        MonitorarConnectionId();
        MonitorarListaUsuarios();
        EnviarReceberMensagens();
        AbrirGrupo();
    }
}

/**
 * Cria grupo de mensagens
 */
function AbrirGrupo() {
    connection.on("AbrirGrupo", function (nomeGrupoMsg, mensagens) {
        nomeGrupo = nomeGrupoMsg;

        var containerMensagens = document.querySelector(".container-message");
        containerMensagens.innerHTML = "";

        var mensagemHTML = "";
        for (i = 0; i < mensagens.length; i++) {
            mensagemHTML += '<article class="message ' + (mensagens[i].Usuario.Id == GetUsuarioLogado().Id ? "is-pulled-right" : "is-pulled-left") + ' column is-9">' +
                                '<div class="message-header has-background-light">' +
                                    '<div class="media">' +
                                        '<div class="media-left">' +
                                            '<figure class="image is-24x24">' +
                                                '<img class="is-rounded" src="../img/rock-messenger.png">' +
                                            '</figure>' +
                                        '</div>' +
                                        '<div class="media-content">' +
                                            '<p class="title is-6">' + (mensagens[i].Usuario.Id == GetUsuarioLogado().Id ? "Eu" : mensagens[i].Usuario.Nome) + '</p>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="message-body">' +
                                    mensagens[i].Texto +
                                '</div >' +
                            '</article>';
        }

        containerMensagens.innerHTML += mensagemHTML;

        document.querySelector(".footer").style.display = "block";

        RolaMensagens();
    });
}

/**
 * Envia e recebe as mensagens 
 */
function EnviarReceberMensagens() {
    var btnEnviar = document.getElementById("btn-enviar");

    btnEnviar.addEventListener("click", function () {
        var mensagem = document.getElementById("mensagem").value;

        if (mensagem != '' && mensagem != null) {
            connection.invoke("EnviarMensagem", GetUsuarioLogado(), mensagem, nomeGrupo).then(function () {
                //console.info("");
                document.getElementById("mensagem").value = "";
            }).catch(function (erro) {
                console.error(erro.toString());
            });
        } else {
            toastr.info("Preencha o campo mensagem!", "Atenção!");
            return false;
        }
    });

    connection.on("ReceberMensagem", function (mensagem, nomeGrupoMsg) {
        if (nomeGrupoMsg == nomeGrupo) {
            var containerMensagens = document.querySelector(".container-message");

            var mensagemHTML = '<article class="message ' + (mensagem.Usuario.Id == GetUsuarioLogado().Id ? "is-pulled-right" : "is-pulled-left") + ' column is-9">' +
                                    '<div class="message-header has-background-light">' +
                                        '<div class="media">' +
                                            '<div class="media-left">' +
                                                '<figure class="image is-24x24">' +
                                                    '<img class="is-rounded" src="../img/rock-messenger.png">' +
                                                '</figure>' +
                                            '</div>' +
                                            '<div class="media-content">' +
                                                '<p class="title is-6">' + (mensagem.Usuario.Id == GetUsuarioLogado().Id ? "Eu" : mensagem.Usuario.Nome) + '</p>' +
                                            '</div>' +
                                        '</div>' +
                                    '</div>' +
                                    '<div class="message-body">' +
                                        mensagem.Texto +
                                    '</div >' + 
                                '</article>';
               
            containerMensagens.innerHTML += mensagemHTML;

            RolaMensagens();
        }
    });
}

/**
 * Monitora e recupera lista de usuários 
 */
function MonitorarListaUsuarios() {
    connection.invoke("ObterListaUsuarios").then(function () {
        //console.info("");
    }).catch(function (erro) {
        console.error(erro.toString());
    });

    connection.on("ReceberListaUsuarios", function (usuarios) {
        var html = "";

        for (i = 0; i < usuarios.length; i++) {
            if (usuarios[i].Id != GetUsuarioLogado().Id) {
                html += '<li class="mbottom">' +
                            '<div class="media">' +
                                '<div class="media-left">' +
                                    '<figure class="image is-32x32">' +
                                        '<img class="is-rounded" src="../img/rock-messenger.png" alt="Placeholder image">' +
                                    '</figure>' +
                                '</div>' +
                                '<div class="media-content">' +
                                    '<p class="title is-5 has-text-white">' +
                                        usuarios[i].Nome.split(" ")[0] +
                                        ' ' +
                    (usuarios[i].IsOnline ? '<span class="tag tag-radius is-success is-light">online</span>' : '<span class="tag tag-radius is-danger is-light">offline</span>') +
                                    '</p>' +
                                    '<p class="email subtitle is-6 has-text-grey">' + usuarios[i].Email + '</p>' +
                                '</div>' +
                            '</div>' +
                        '</li>';
            }
        }

        document.getElementById("usuarios").innerHTML = html;

        var usuariosMenu = document.getElementById("usuarios").querySelectorAll(".media");

        for (i = 0; i < usuariosMenu.length; i++) {
            usuariosMenu[i].addEventListener("click", function (event) {
                var componente = event.target || event.srcElement;

                var emailUsuarioLogado = GetUsuarioLogado().Email;
                var emailUsuarioSelecionado = componente.parentElement.querySelector(".email").innerText;

                connection.invoke("CriarAbrirGrupo", emailUsuarioLogado, emailUsuarioSelecionado).then(function () {
                    //console.info("");
                }).catch(function (erro) {
                    console.error(erro.toString());
                });
            });
        }
    });
}

/**
 * Monitora conexão para incluir connectionId ao usuário logado
 */
function MonitorarConnectionId() {
    var telaMensagens = document.getElementById("tela-mensagens");
    if (telaMensagens != null) {

        connection.invoke("AddConnectionIdUsuario", GetUsuarioLogado()).then(function () {
            //console.info("");
        }).catch(function (erro) {
            console.error(erro.toString());
        });

        var btnSair = document.getElementById("btn-sair");
        btnSair.addEventListener("click", function () {
            connection.invoke("Logout", GetUsuarioLogado()).then(function () {
                //console.info("");
                DelUsuarioLogado();
                window.location.href = "/Home/Login";
            }).catch(function (erro) {
                console.error(erro.toString());
            });
        });
    }
}

function RolaMensagens() {
    var el = document.querySelector('.container-message');
    el.style.overflowY = "scroll";
    el.scrollTop = el.scrollHeight;
}

function GetUsuarioLogado() {
    return JSON.parse(sessionStorage.getItem("Logado"));
}

function SetUsuarioLogado(usuario) {
    sessionStorage.setItem("Logado", JSON.stringify(usuario));
}

function DelUsuarioLogado() {
    sessionStorage.removeItem("Logado");
}

/**
 * Starta a conexão com o Hub
 */  
ConnectionStart();