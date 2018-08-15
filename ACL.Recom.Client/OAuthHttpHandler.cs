using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace ACL.Recom.Client
{
    internal class OAuthHttpHandler : DelegatingHandler
    {
        private readonly string _clienteId;
        private readonly string _clienteSenha;
        private readonly string _urlAutenticacao;
        private readonly string _loginUsuario;
        private readonly object _claims;

        private string _accessToken;

        public OAuthHttpHandler(string clienteId, string clienteSenha, string urlAutenticacao, object claims, string loginUsuario = null)
        {
            _clienteId = clienteId;
            _clienteSenha = clienteSenha;
            _urlAutenticacao = urlAutenticacao;
            _loginUsuario = loginUsuario;
            _claims = claims;

            InnerHandler = new HttpClientHandler();
        }

        public async Task<string> ObterAcessTokenAsync(CancellationToken cancellationToken)
        {
            if (_accessToken == null)
                _accessToken = await RequisitarToken(cancellationToken);

            return _accessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization == null)
            {
                if (_accessToken == null)
                    _accessToken = await RequisitarToken(cancellationToken);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> RequisitarToken(CancellationToken cancellationToken)
        {
            DiscoveryResponse disco;
            using (var discoClient = new DiscoveryClient(_urlAutenticacao))
            {
                discoClient.Policy.RequireHttps = _urlAutenticacao.ToLower().StartsWith("https");
                disco = await discoClient.GetAsync(cancellationToken);
                if (disco.IsError)
                    throw disco.Exception;
            }

            using (var tokenClient = new TokenClient(disco.TokenEndpoint, _clienteId, _clienteSenha))
            {
                if (!string.IsNullOrEmpty(_loginUsuario) && _claims != null)
                {
                    var responseToken = await tokenClient.RequestResourceOwnerPasswordAsync(_loginUsuario, "123456", "recom", _claims);

                    if (responseToken.IsError)
                        throw new UnauthorizedAccessException();

                    var handler = new JwtSecurityTokenHandler();
                    var securityToken = handler.ReadToken(responseToken.AccessToken) as JwtSecurityToken;

                    return responseToken.AccessToken;
                }

                var response = await tokenClient.RequestClientCredentialsAsync(
                    scope: "recom",
                    cancellationToken: cancellationToken
                    );

                return response.AccessToken;
            }
        }
    }
}