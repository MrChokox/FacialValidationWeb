namespace ValidacaoFacialWeb.Models
{
    public interface IPessoaRepository
    {
        IEnumerable<PessoaCadastrada> ObterTodas();
    }
}
