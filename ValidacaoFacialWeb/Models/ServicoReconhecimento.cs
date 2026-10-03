using FaceRecognitionDotNet;
using System.Drawing;
using ValidacaoFacialWeb.Models;

namespace ValidacaoFacialWeb.Models
{
    public class ServicoReconhecimento : IDisposable
    {
        private class FaceCadastrada
        {
            public PessoaCadastrada Pessoa { get; }
            public FaceEncoding Encoding { get; }

            public FaceCadastrada(PessoaCadastrada pessoa, FaceEncoding encoding)
            {
                Pessoa = pessoa;
                Encoding = encoding;
            }
        }

        private readonly FaceRecognition _fr;
        private readonly List<FaceCadastrada> _base = new();

        // Quanto menor, mais rígido. 0.6 é o padrão da biblioteca; 0.5 é mais seguro.
        public double Tolerancia { get; set; } = 0.5;

        public int TotalCarregado => _base.Count;

        public ServicoReconhecimento(string pastaModelos, IPessoaRepository repositorio)
        {
            _fr = FaceRecognition.Create(pastaModelos);
            CarregarBase(repositorio);
        }

        private void CarregarBase(IPessoaRepository repo)
        {
            foreach (var pessoa in repo.ObterTodas())
            {
                using var img = FaceRecognition.LoadImageFile(pessoa.CaminhoFoto);
                var encodings = _fr.FaceEncodings(img).ToList();

                if (encodings.Count > 0)
                    _base.Add(new FaceCadastrada(pessoa, encodings[0]));
            }
        }

        /// <summary>Retorna a pessoa identificada ou null.</summary>
        public PessoaCadastrada? Identificar(Bitmap frame)
        {
            using var img = FaceRecognition.LoadImage(frame);
            var encodings = _fr.FaceEncodings(img).ToList();

            try
            {
                foreach (var candidato in encodings)
                {
                    PessoaCadastrada? melhor = null;
                    double melhorDist = double.MaxValue;

                    foreach (var item in _base)
                    {
                        double d = FaceRecognition.FaceDistance(item.Encoding, candidato);
                        if (d < melhorDist)
                        {
                            melhorDist = d;
                            melhor = item.Pessoa;
                        }
                    }

                    if (melhor != null && melhorDist <= Tolerancia)
                        return melhor;
                }

                return null;
            }
            finally
            {
                foreach (var e in encodings)
                    e.Dispose();
            }
        }

        public void Dispose()
        {
            foreach (var item in _base)
                item.Encoding.Dispose();

            _fr?.Dispose();
        }
    }
}