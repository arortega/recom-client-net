using System;

namespace ACL.Recom.Client.Entidades
{
    public class Obra
    {
        public string RaizCnpjConstrutora { get; set; }
        public string Cnpj { get; set; }
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public Endereco Endereco { get; set; }
        public string Descricao { get; set; }
        public string CodigoMunicipio { get; set; }
        public Instituicao Construtor { get; set; }
        public Instituicao Tomador { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataTermino { get; set; }
        public Deducao Deducao { get; set; }
        public ObraFiscal ObraFiscal { get; set; }
    }

    public enum Deducao
    {
        Estimada = 0,
        ValorReal = 1
    }

    public enum Zona
    {
        Urbana,
        Rural
    }

    public enum TipoObra
    {
        Construcao,
        Reforma,
        Demolicao
    }

    public class ObraFiscal
    {
        public Zona Local { get; set; }
        public TipoObra TipoObra { get; set; }
        public string Cei { get; set; }
        public string Art { get; set; }
        public string IndicacaoFiscal { get; set; }
        public string Inscricao { get; set; }
        public bool His { get; set; }
        public bool Hpmcmv { get; set; }
        public string CertificadoHis { get; set; }
        public string Alvara { get; set; }
        public ResponsavelTecnico ResponsavelTecnico { get; set; }
    }

    public class ResponsavelTecnico
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Fone { get; set; }
        public string Email { get; set; }
        public string Crea { get; set; }
    }
}
