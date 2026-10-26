using VotePreferentiel;

namespace VotePreferentiel.Worker;

public sealed class Worker(
    VoteProvider voteProvider,
    IVoteCounter voteCounter,
    ILogger<Worker> logger,
    IHostApplicationLifetime applicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            VoteModel votes = await voteProvider.GetVotesAsync(stoppingToken);
            string winner = voteCounter.GetWinner(votes);
            logger.LogInformation("La gagnante ou le gagnant est : {Winner}", winner);
            Console.WriteLine($"La gagnante ou le gagnant est : {winner}");
            applicationLifetime.StopApplication();
        }
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException)
        {
            logger.LogError(exception, "Le calcul du résultat de l'élection a échoué.");
            throw;
        }
    }
}
