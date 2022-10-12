using System.Collections.Concurrent;
using System.Text.Json;

namespace Wabbit;

public class CoopController
{
    private static readonly Random Random = new Random();
    private readonly List<Rabbit> _rabbits;
    private readonly ICollection<Rabbit> _newBorns;
    private CoopInfo? _coopInfo;

    public CoopController(IEnumerable<Rabbit> rabbits)
    {
        _rabbits = new List<Rabbit>();
        _newBorns = new List<Rabbit>();
        AddRabbits(rabbits);
        // RefreshCoopInfo();
    }

    private void AddRabbit(Rabbit rabbit)
    {
        _newBorns.Add(rabbit);
        rabbit.OnBirth += AddRabbits;
        rabbit.OnDeath += RemoveRabbit;
    }

    private void RemoveRabbit(Rabbit rabbit)
    {
        _rabbits.Remove(rabbit);
        rabbit.OnBirth -= AddRabbits;
        rabbit.OnDeath -= RemoveRabbit;
    }

    private void AddRabbits(IEnumerable<Rabbit> rabbits)
    {
        foreach (var rabbit in rabbits)
        {
            AddRabbit(rabbit);
        }
    }

    public void SimulateDay()
    {
        var genderGrouped = _rabbits.GroupBy(x => x.Gender);
        var genderDictionary = genderGrouped.ToDictionary(x => (int) x.Key, rabbits => rabbits.ToList());
        var tasks = _rabbits.Select(x => Task.Run(() => AgeRabbit(x, genderDictionary)));
        Task.WaitAll(tasks.ToArray());
        SimulatedTime.Instance.AddDays(1);
        _rabbits.AddRange(_newBorns);
        _newBorns.Clear();
        RefreshCoopInfo();
    }

    private static void AgeRabbit(Rabbit rabbit, Dictionary<int, List<Rabbit>> genderDictionary)
    {
        rabbit.Age();
        var oppositeGenderedRabbits = genderDictionary[(int) rabbit.Gender * -1];
        var randomOppositeGenderedRabbit = oppositeGenderedRabbits[Random.Next(oppositeGenderedRabbits.Count)];
        rabbit.Mate(randomOppositeGenderedRabbit);
    }

    private void RefreshCoopInfo()
    {
        _coopInfo = new CoopInfo();
        for (var index = 0; index < _rabbits.Count; index++)
        {
            var rabbit = _rabbits[index];
            if (rabbit == null)
            {
                continue;
            }

            _coopInfo.TotalAge += rabbit.BiologicalAge;
            _coopInfo.TotalRabbits++;
            if (rabbit.Gender == Gender.Male) _coopInfo.MaleRabbits++;
            else _coopInfo.FemaleRabbits++;
            if (rabbit.BiologicalAge > _coopInfo.MaxAge) _coopInfo.MaxAge = rabbit.BiologicalAge;
        }

        _coopInfo.AverageAge = _coopInfo.TotalAge / _coopInfo.TotalRabbits;
        _coopInfo.CurrentDate = SimulatedTime.Instance.Now;
    }

    public override string ToString()
    {
        if (_coopInfo != null) return JsonSerializer.Serialize(_coopInfo);
        return "No data";
    }

    private class CoopInfo
    {
        public int TotalRabbits { get; set; }
        public int MaleRabbits { get; set; }
        public int FemaleRabbits { get; set; }
        public double TotalAge { get; set; }
        public double AverageAge { get; set; }
        public double MaxAge { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}