using ACL.Recom.Client.Entidades;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACL.Recom.Client
{
    public class RecomClient
    {
        private readonly HttpClient httpClient;
        private readonly OAuthHttpHandler oAuthHttpHandler;

        public RecomClient(string clienteId, string clienteSenha, string urlApi, string urlAutenticacao, object claims, string loginUsuario = null)
        {
            if (string.IsNullOrEmpty(clienteId))
                throw new ArgumentNullException(nameof(clienteId));

            if (string.IsNullOrEmpty(clienteSenha))
                throw new ArgumentNullException(nameof(clienteSenha));

            if (string.IsNullOrEmpty(urlApi))
                throw new ArgumentNullException(nameof(urlApi));

            if (string.IsNullOrEmpty(urlAutenticacao))
                throw new ArgumentNullException(nameof(urlAutenticacao));

            oAuthHttpHandler = new OAuthHttpHandler(
                clienteId,
                clienteSenha,
                urlAutenticacao, 
                claims, 
                loginUsuario);

            httpClient = new HttpClient(oAuthHttpHandler)
            {
                BaseAddress = new Uri(urlApi)
            };
        }

        public Task<string> ObterAccessToken(CancellationToken cancellationToken)
        {
            return oAuthHttpHandler.ObterAcessTokenAsync(cancellationToken);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        /// <summary>
        /// Obra como prestadora de serviço. Gera incremento no saldo dedutível.
        /// </summary>
        /// <param name="nfse">NFS-e</param>
        /// <returns></returns>
        public Task EnviarNFSeTomadorAsync(NFSeTomador nfse)
        {
            return httpClient.PostAsJsonAsync($"api/nfses/incrementar", nfse);
        }

        /// <summary>
        /// Obra como tomadora de serviço. Amortiza o saldo dedutível.
        /// </summary>
        /// <param name="nfse">NFS-e</param>
        /// <returns></returns>
        public Task EnviarNFSePrestadorAsync(NFSePrestador nfse)
        {
            return httpClient.PostAsJsonAsync($"api/nfses/amortizar", nfse);
        }

        /// <summary>
        /// Estorna o incremento de saldo dedutível. Ao cancelar uma NFS-e onde a obra é o tomador de serviço.
        /// </summary>
        /// <param name="cnpj">CNPJ do prestador de serviço (Emitente).</param>
        /// <param name="numero">Número da NFS-e</param>
        /// <returns></returns>
        public Task EstornarIncrementoAsync(string cnpj, string numero)
        {
            return httpClient.DeleteAsync($"api/nfses/estornarIncremento/{cnpj}/{numero}");
        }

        /// <summary>
        /// Estorna a amortização de saldo dedutível. Ao cancelar uma NFS-e onde a obra é o prestador de serviço.
        /// </summary>
        /// <param name="cnpj">CNPJ do prestador de serviço (Emitente).</param>
        /// <param name="numero">Número da NFS-e</param>
        /// <returns></returns>
        public Task EstornarAmortizacaoAsync(string cnpj, string numero)
        {
            return httpClient.DeleteAsync($"api/nfses/estornarAmortizacao/{cnpj}/{numero}");
        }

        /// <summary>
        /// Verifica se a obra existe
        /// </summary>
        /// <param name="codigoObra">Código da obra</param>
        /// <returns></returns>
        public async Task<bool> ObraExisteAsync(string codigoObra)
        {
            var response = await httpClient.GetAsync($"api/obras/{codigoObra}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            return true;
        }

        /// <summary>
        /// Obtem o valor máximo dedutível para o valor da NFS-e
        /// </summary>
        /// <param name="codigoObra">Código da obra</param>
        /// <param name="valorNFSe">Valor da NFS-e</param>
        /// <returns></returns>
        public Task<double> ObterValorDedutivelAsync(string codigoObra, double valorNFSe)
        {
            return httpClient.GetJsonAsync<double>($"api/contacorrente/saldo/saque/{codigoObra}/{valorNFSe}");
        }

        /// <summary>
        /// Envia uma nova construtora para o Recom
        /// </summary>
        /// <param name="construtora">Dados da contrutora</param>
        /// <returns></returns>
        public Task CadastrarConstrutoraAsync(Construtora construtora)
        {
            return httpClient.PostAsJsonAsync($"api/construtoras", construtora);
        }

        /// <summary>
        /// Envia uma nova obra para o Recom
        /// </summary>
        /// <param name="obra">Dados da obra</param>
        /// <returns></returns>
        public Task CadastrarObraAsync(Obra obra)
        {
            return httpClient.PostAsJsonAsync($"api/obras", obra);
        }

        /// <summary>
        /// Envia o usuário para o Recom e qual construtora ele tem acesso
        /// </summary>
        /// <param name="login">Login do usuário</param>
        /// <param name="cnpjConstrutora">CNPJ da construtora</param>
        /// <param name="codigoMunicipio">Código IBGE do município</param>
        /// <returns></returns>
        public Task EnviarUsuarioAsync(string login, string cnpjConstrutora, string codigoMunicipio)
        {
            var usuario = new Usuario
            {
                Login = login,
                RaizCnpjConstrutora = cnpjConstrutora,
                CodigoMunicipio = codigoMunicipio
            };

            return httpClient.PostAsJsonAsync<Usuario>($"api/usuarios", usuario);
        }

        /// <summary>
        /// Remove a permissão de acesso do usuário a construtora
        /// </summary>
        /// <param name="login">Login do usuário</param>
        /// <param name="cnpjConstrutora">CNPJ da construtora</param>
        /// <returns></returns>
        public Task RemoverPermissaoDoUsuarioAsync(string login, string cnpjConstrutora)
        {
            return httpClient.DeleteAsync($"api/usuarios/{login}/{cnpjConstrutora}");
        }
    }
}
