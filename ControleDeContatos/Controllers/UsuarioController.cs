using ControleDeContatos.Filters;
using ControleDeContatos.Models;
using ControleDeContatos.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeContatos.Controllers
{
    [PaginaRestritaSomenteAdmin]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IContatoRepository _contatoRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository, IContatoRepository contatoRepository)
        {
            _usuarioRepository = usuarioRepository;
            _contatoRepository = contatoRepository;
        }

        public IActionResult Index()
        {
            List<UsuarioModel> usuarios = _usuarioRepository.BuscarTodos();
            return View(usuarios);
        }

        public IActionResult Criar()
        {
            return View();
        }

        public IActionResult ApagarConfirmacao(int id)
        {
            UsuarioModel usuario = _usuarioRepository.ListarPorId(id);
            return View(usuario);
        }

        public IActionResult Editar(int id)
        {
            UsuarioModel usuario = _usuarioRepository.ListarPorId(id);
            return View(usuario);
        }

       

        [HttpPost]
        public IActionResult Criar(UsuarioModel usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _usuarioRepository.Adicionar(usuario);
                    TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso";
                    return RedirectToAction("Index");
                }

                return View(usuario);
            }
            catch (Exception err)
            {
                TempData["MensagemError"] = $"Ops, não conseguimos cadastrar seu Usuário, tente novamente! detalhe do erro: {err.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Apagar(int id)
        {
            try
            {
                bool apagado = _usuarioRepository.Apagar(id);
                if (apagado)
                {
                    TempData["MensagemSucesso"] = "Usuário apagado com sucesso";
                }
                else
                {
                    TempData["MensagemError"] = $"Ops, não conseguimos apagar seu usuário, tente novamente!";
                }
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                TempData["MensagemError"] = $"Ops, não conseguimos apagar seu usuário, tente novamente! detalhe do erro: {err.Message}";
                return RedirectToAction("Index");
            }

        }

        public IActionResult ListarContatosPorUsuarioId(int id)
        {
            List<ContatoModel> contatos = _contatoRepository.BuscarTodos(id);
            return PartialView("_ContatosUsuario", contatos);
        }

        [HttpPost]
        public IActionResult Editar(UsuarioSemSenhaModel usuarioSemSenhaModel)
        {
            try
            {
                UsuarioModel usuario = null;
                if (ModelState.IsValid)
                {
                    usuario = new UsuarioModel()
                    {
                        Id = usuarioSemSenhaModel.Id,
                        Nome = usuarioSemSenhaModel.Nome,
                        Login = usuarioSemSenhaModel.Login,
                        Email = usuarioSemSenhaModel.Email,
                        Pefil = usuarioSemSenhaModel.Pefil
                    };

                    _usuarioRepository.Atualizar(usuario);
                    TempData["MensagemSucesso"] = "Usuário alterado com sucesso";
                    return RedirectToAction("Index");
                }

                return View(usuario);
            }
            catch (Exception err)
            {

                TempData["MensagemError"] = $"Ops, não conseguimos alterar seu usuário, tente novamente! detalhe do erro: {err.Message}";
                return RedirectToAction("Index");
            }


        }

    }
}
