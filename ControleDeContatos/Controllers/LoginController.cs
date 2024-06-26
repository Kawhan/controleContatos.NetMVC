﻿using ControleDeContatos.Helper;
using ControleDeContatos.Models;
using ControleDeContatos.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeContatos.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISessao _sessao;
        private readonly IEmail _email;

        public LoginController(IUsuarioRepository usuarioRepository, ISessao sessao, IEmail email)
        {
            _usuarioRepository = usuarioRepository;
            _sessao = sessao;
            _email = email;

        }

        public IActionResult Index()
        {
            // Se usuário estiver logado redirecionar para a home
            if (_sessao.BuscarSessaoDoUsuario() != null) 
            {
                TempData["MensagemError"] = "Você já está logado no sistema";
                return RedirectToAction("Index", "Home");
            } 

            return View();
        }

        public IActionResult RedefinirSenha()
        {
            return View();
        }
         
        public IActionResult Sair()
        {
            _sessao.RemoverSessaoUsuario();

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public IActionResult Entrar(LoginModel loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuario =  _usuarioRepository.BuscarPorLogin(loginModel.Login);

                    if (usuario != null)
                    {
                        if (usuario.SenhaValida(loginModel.Senha))
                        {
                            _sessao.CriarSessaoDoUsuario(usuario);
                            TempData["MensagemSucesso"] = "Login realizado com sucesso";
                            return RedirectToAction("Index", "Home");
                        }
                        TempData["MensagemError"] = $"A Senha do usuário é inválida, tente novamente.";
                    }
                    TempData["MensagemError"] = $"Usuário e/ou senha inválido(s). Por favor, tente novamente.";
                    
                }
                return View("Index");
            }
            catch (Exception err)
            {
                TempData["MensagemError"] = $"Ops, não conseguimos realizar o login, tente novamente! detalhe do erro: {err.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult EnviarLinkParaRedefinirSenha(RedefinirSenhaModel redefinirSenhaModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuario = _usuarioRepository.BuscarPorEmailELogin(redefinirSenhaModel.Email, redefinirSenhaModel.Login);

                    if (usuario != null)
                    {
                        string novaSenha = usuario.GerarNovaSenha();
                        string mensagem = $"Sua nova senha é: {novaSenha}";


                        bool emailEnviado = _email.Enviar(usuario.Email, "Sistema de Contatos - Nova Senha", mensagem);

                        if (emailEnviado)
                        {
                            _usuarioRepository.Atualizar(usuario);
                            TempData["MensagemSucesso"] = "Enviamos para o seu e-mail cadastrado uma nova senha";
                        } else
                        {
                            TempData["MensagemError"] = "Não conseguimos enviar o e-mail. Por favor, tente novamente";
                        }

                        return RedirectToAction("Index", "Login");
                    }
                    TempData["MensagemError"] = "Não conseguimos redefinir sua senha. Por favor, verifique os dados informados";

                }
                return View("Index");
            }
            catch (Exception err)
            {
                TempData["MensagemError"] = $"Ops, não conseguimos redefinir sua senha, tente novamente! detalhe do erro: {err.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
