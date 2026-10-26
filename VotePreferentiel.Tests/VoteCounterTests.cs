using VotePreferentiel;

namespace VotePreferentiel.Tests;

public sealed class VoteCounterTests
{
    private readonly VoteCounter _counter = new();

    [Test]
    public async Task ReturnsChloeForTheExample()
    {
        VoteModel votes = Model(
            ["Alice", "Bilal", "Chloé"],
            ["Alice", "Chloé", "Bilal"],
            ["Bilal", "Chloé", "Alice"],
            ["Chloé", "Bilal", "Alice"],
            ["Chloé", "Alice", "Bilal"]);

        await Assert.That(_counter.GetWinner(votes)).IsEqualTo("Chloé");
    }

    [Test]
    public async Task ReturnsTheOnlyCandidate()
    {
        await Assert.That(_counter.GetWinner(Model(["Alice"]))).IsEqualTo("Alice");
    }

    [Test]
    public async Task TransfersPreferencesAfterEliminations()
    {
        List<List<string>> ballots = [];
        ballots.AddRange(Enumerable.Repeat(new List<string> { "Alice", "Bilal", "Chloé", "Diane" }, 4));
        ballots.AddRange(Enumerable.Repeat(new List<string> { "Bilal", "Chloé", "Alice", "Diane" }, 3));
        ballots.AddRange(Enumerable.Repeat(new List<string> { "Chloé", "Alice", "Bilal", "Diane" }, 3));
        ballots.Add(["Diane", "Chloé", "Bilal", "Alice"]);

        await Assert.That(_counter.GetWinner(Model(ballots.ToArray()))).IsEqualTo("Chloé");
    }

    [Test]
    public async Task EliminatesTiedCandidatesSimultaneously()
    {
        VoteModel votes = Model(
            ["Alice", "Chloé", "Bilal"],
            ["Bilal", "Chloé", "Alice"],
            ["Chloé", "Alice", "Bilal"],
            ["Chloé", "Bilal", "Alice"]);

        await Assert.That(_counter.GetWinner(votes)).IsEqualTo("Chloé");
    }

    [Test]
    public async Task RejectsEmptyBallot()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Task.Run(() => _counter.GetWinner(Model([]))));
    }

    private static VoteModel Model(params List<string>[] ballots) =>
        new() { Votes = ballots.Select(choices => new Vote { Choices = choices }).ToList() };
}
