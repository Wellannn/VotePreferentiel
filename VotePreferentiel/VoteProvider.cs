using System.Text.Json;

namespace VotePreferentiel;

public sealed class VoteProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _votesUrl;

    public VoteProvider(HttpClient httpClient, string votesUrl)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _votesUrl = string.IsNullOrWhiteSpace(votesUrl)
            ? throw new ArgumentException("L'URL des votes est obligatoire.", nameof(votesUrl))
            : votesUrl;
    }

    public async Task<VoteModel> GetVotesAsync(CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(_votesUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using Stream content = await response.Content.ReadAsStreamAsync(cancellationToken);
        using JsonDocument document = await JsonDocument.ParseAsync(content, cancellationToken: cancellationToken);

        VoteModel? voteModel = document.RootElement.ValueKind switch
        {
            JsonValueKind.Object => document.RootElement.Deserialize<VoteModel>(),
            _ => null
        };

        return voteModel is null
            ? throw new InvalidOperationException("Le fichier JSON des votes a un format invalide.")
            : voteModel;
    }
}
