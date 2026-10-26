namespace VotePreferentiel;

public sealed class VoteCounter : IVoteCounter
{
    public HashSet<string> GetCandidates(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        HashSet<string> candidates = new(StringComparer.Ordinal);
        foreach (Vote ballot in voteModel.Votes)
        {
            foreach (string candidate in ballot.Choices)
            {
                if (!string.IsNullOrWhiteSpace(candidate))
                {
                    candidates.Add(candidate);
                }
            }
        }

        return candidates;
    }

    public string GetWinner(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        HashSet<string> remainingCandidates = GetCandidates(voteModel);
        if (remainingCandidates.Count == 0)
        {
            throw new InvalidOperationException("Aucun candidat valide n'a été trouvé.");
        }

        while (remainingCandidates.Count > 1)
        {
            List<string> currentChoices = GetCurrentChoices(voteModel, remainingCandidates);
            IDictionary<string, int> counts = CountFirstChoices(currentChoices);
            foreach (string candidate in remainingCandidates)
            {
                counts.TryAdd(candidate, 0);
            }

            string[] eliminated = GetCandidatesWithLeastVotes(counts);
            if (eliminated.Length == remainingCandidates.Count)
            {
                throw new InvalidOperationException(
                    "Impossible de départager les candidats : ils sont tous à égalité.");
            }

            foreach (string candidate in eliminated)
            {
                remainingCandidates.Remove(candidate);
            }
        }

        return remainingCandidates.Single();
    }

    public List<string> GetAllFirstChoices(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        return voteModel.Votes.Select(ballot => GetFirstValidChoice(ballot)).ToList();
    }

    public IDictionary<string, int> CountFirstChoices(List<string> firstChoices)
    {
        ArgumentNullException.ThrowIfNull(firstChoices);

        return firstChoices
            .GroupBy(choice => choice, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
    }

    public string[] GetCandidatesWithLeastVotes(IDictionary<string, int> firstChoiceCounts)
    {
        ArgumentNullException.ThrowIfNull(firstChoiceCounts);
        if (firstChoiceCounts.Count == 0)
        {
            throw new InvalidOperationException("Aucun candidat à départager.");
        }

        int minimum = firstChoiceCounts.Values.Min();
        return firstChoiceCounts
            .Where(pair => pair.Value == minimum)
            .Select(pair => pair.Key)
            .ToArray();
    }

    private static List<string> GetCurrentChoices(
        VoteModel voteModel,
        HashSet<string> remainingCandidates)
    {
        List<string> choices = [];
        foreach (Vote ballot in voteModel.Votes)
        {
            string? choice = ballot.Choices.FirstOrDefault(candidate =>
                !string.IsNullOrWhiteSpace(candidate) && remainingCandidates.Contains(candidate));

            if (choice is null)
            {
                throw new InvalidOperationException(
                    "Un bulletin ne contient aucun choix encore en lice.");
            }

            choices.Add(choice);
        }

        return choices;
    }

    private static string GetFirstValidChoice(Vote ballot)
    {
        string? choice = ballot.Choices.FirstOrDefault(candidate => !string.IsNullOrWhiteSpace(candidate));
        return choice ?? throw new InvalidOperationException(
            "Un bulletin ne contient aucun choix valide.");
    }
}
