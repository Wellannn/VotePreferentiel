using VotePreferentiel;
using VotePreferentiel.Worker;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

string votesUrl = builder.Configuration["VotesUrl"]
    ?? throw new InvalidOperationException(
        "La configuration 'VotesUrl' est obligatoire.");

builder.Services.AddHttpClient("Votes", client =>
{
    client.Timeout = TimeSpan.FromMinutes(2);
});
builder.Services.AddTransient<VoteProvider>(serviceProvider =>
    new VoteProvider(
        serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("Votes"),
        votesUrl));
builder.Services.AddSingleton<IVoteCounter, VoteCounter>();
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();
