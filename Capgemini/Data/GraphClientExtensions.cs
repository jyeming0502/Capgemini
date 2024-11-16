using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Authentication.WebAssembly.Msal.Models;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using IAccessTokenProvider =
    Microsoft.AspNetCore.Components.WebAssembly.Authentication.IAccessTokenProvider;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Adds services and implements methods to use Microsoft Graph SDK.
/// </summary>
internal static class GraphClientExtensions
{
    /// <summary>
    /// Extension method for adding the Microsoft Graph SDK to IServiceCollection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="scopes">The MS Graph scopes to request</param>
    /// <returns></returns>
    public static IServiceCollection AddMicrosoftGraphClient(this IServiceCollection services, string? baseUrl, List<string>? scopes)
    {
        if (string.IsNullOrEmpty(baseUrl) || scopes is null)
        {
            return services;
        }

        services.Configure<RemoteAuthenticationOptions<MsalProviderOptions>>(options =>
        {
            scopes?.ForEach((scope) =>
            {
                options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
            });
        });

        //var interactiveBrowserCredentialOptions = new InteractiveBrowserCredentialOptions();
        //var interactiveBrowserCredential = new InteractiveBrowserCredential(interactiveBrowserCredentialOptions);
        services.AddScoped<IAuthenticationProvider, GraphAuthenticationProvider>();
        //services.AddScoped<IHttpProvider, HttpClientHttpProvider>(sp => new HttpClientHttpProvider(new HttpClient()));
        services.AddScoped(sp =>
            {
                return new GraphServiceClient(
                new HttpClient(),
                  sp.GetRequiredService<IAuthenticationProvider>(),
                  baseUrl);
            }
        );
        //services.AddScoped(sp => new GraphServiceClient(interactiveBrowserCredential));

        return services;
    }

    /// <summary>
    /// Implements IAuthenticationProvider interface.
    /// Tries to get an access token for Microsoft Graph.
    /// </summary>
    /// 
    private class GraphAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IConfiguration config;

        public GraphAuthenticationProvider(IAccessTokenProvider tokenProvider,
            IConfiguration config)
        {
            TokenProvider = tokenProvider;
            this.config = config;
        }

        public IAccessTokenProvider TokenProvider { get; }

        public async Task AuthenticateRequestAsync(RequestInformation request,
            Dictionary<string, object>? additionalAuthenticationContext = null,
            CancellationToken cancellationToken = default)
        {
            var result = await TokenProvider.RequestAccessToken(new AccessTokenRequestOptions()
            {
                Scopes = config.GetSection("MicrosoftGraph:Scopes").Get<string[]>()
            });

            if (result.TryGetToken(out var token))
            {
                request.Headers.Add("Authorization",
                    $"{CoreConstants.Headers.Bearer} {token.Value}");
            }
        }
    }

    //private class HttpClientHttpProvider : IHttpProvider
    //{
    //    private readonly HttpClient _client;

    //    public HttpClientHttpProvider(HttpClient client)
    //    {
    //        _client = client;
    //    }

    //    public ISerializer Serializer { get; } = new Serializer();

    //    public TimeSpan OverallTimeout { get; set; } = TimeSpan.FromSeconds(300);

    //    public void Dispose()
    //    {
    //    }

    //    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    //    {
    //        return _client.SendAsync(request);
    //    }

    //    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    //    {
    //        return _client.SendAsync(request, completionOption, cancellationToken);
    //    }
    //}
}
