using System.IO;
using ValidacaoFacialWeb.Models;

namespace ValidacaoFacialWeb.Models
{
    public class PessoaRepositoryPasta : IPessoaRepository
    {
        private readonly string _pasta;

        public PessoaRepositoryPasta(string pasta)
        {
            _pasta = pasta;
        }

        public IEnumerable<PessoaCadastrada> ObterTodas()
        {
            if (!Directory.Exists(_pasta))
                yield break;

            var extensoes = new[] { ".jpg", ".jpeg", ".png", ".bmp" };

            foreach (var arq in Directory.GetFiles(_pasta)
                         .Where(f => extensoes.Contains(Path.GetExtension(f).ToLower())))
            {
                var nomeArq = Path.GetFileNameWithoutExtension(arq); // "1_Joao Silva"
                var partes = nomeArq.Split('_', 2);

                int id = partes.Length == 2 && int.TryParse(partes[0], out var i) ? i : 0;
                string nome = partes.Length == 2 ? partes[1] : nomeArq;

                yield return new PessoaCadastrada
                {
                    Id = id,
                    Nome = nome,
                    CaminhoFoto = arq
                };
            }
        }
    }
}