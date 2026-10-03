using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using ValidacaoFacialWeb.Models; // <- ajuste para o namespace onde estão suas classes de reconhecimento

namespace TesteFacial.Controllers
{
    public class ReconhecimentoController : Controller
    {
        // A biblioteca não é thread-safe: serializa as análises entre requisições.
        private static readonly object _trava = new();

        private readonly ServicoReconhecimento _servico;

        public ReconhecimentoController(ServicoReconhecimento servico)
        {
            _servico = servico;
        }

        // GET /Reconhecimento
        public IActionResult Index()
        {
            return View();
        }

        // POST /Reconhecimento/Identificar
        [HttpPost]
        public IActionResult Identificar([FromBody] IdentificarRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Imagem))
                return BadRequest(new { erro = "Imagem não enviada." });

            try
            {
                // Remove o prefixo "data:image/jpeg;base64,"
                var base64 = req.Imagem.Contains(',') ? req.Imagem.Split(',')[1] : req.Imagem;
                var bytes = Convert.FromBase64String(base64);

                using var ms = new MemoryStream(bytes);
                using var bmp = new Bitmap(ms);

                PessoaCadastrada? pessoa;
                lock (_trava)
                {
                    pessoa = _servico.Identificar(bmp);
                }

                if (pessoa == null)
                    return Json(new { identificado = false });

                return Json(new { identificado = true, id = pessoa.Id, nome = pessoa.Nome });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }

    public class IdentificarRequest
    {
        public string Imagem { get; set; } = "";
    }
}