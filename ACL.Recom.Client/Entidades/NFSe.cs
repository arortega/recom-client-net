using System;

namespace ACL.Recom.Client.Entidades
{
    public abstract class NFSe
    {
        public DateTime DataEmissao { get; set; }
        public string Descricao { get; set; }
        public Instituicao Emitente { get; set; }
        public string Numero { get; set; }
        public string NumeroRanfs { get; set; }
        public Servico Servico { get; set; }
        public Instituicao Tomador { get; set; }
        public double Valor { get; set; }
        public string UsuarioExterno { get; set; }
        public string CodigoObra { get; set; }
    }

    public class NFSePrestador : NFSe
    {
        public double ValorDeducoes { get; set; }
    }

    public class NFSeTomador : NFSe
    {
    }
}
