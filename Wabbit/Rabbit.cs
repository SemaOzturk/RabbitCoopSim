namespace Wabbit;

public class Rabbit
{
    private readonly IRabbitRandomizerService _randomizerService;

    public Rabbit(IRabbitRandomizerService randomizerService, Gender gender)
    {
        _randomizerService = randomizerService;
        Gender = gender;
        DeathDate = SimulatedTime.Instance.Now.AddYears(_randomizerService.GetMaxAge());
    }

    public DateTime BirtDate { get; set; } = SimulatedTime.Instance.Now;
    public Gender Gender { get; }
    public double BiologicalAge => ((SimulatedTime.Instance.Now - BirtDate).TotalDays / 365);
    public DateTime? PregnancyDate { get; set; }
    public DateTime DeathDate { get; set; }
    public bool IsPregnant => PregnancyDate.HasValue;
    public int? PregnancyDuration { get; set; }
    public bool IsDead { get; set; }
    public event Action<IEnumerable<Rabbit>>? OnBirth;
    public event Action<Rabbit>? OnDeath;

    private bool CanBecomePregnant()
    {
        return Gender == Gender.Female && !IsPregnant && SimulatedTime.Instance.Now - BirtDate > TimeSpan.FromDays(180);
    }

    public void Mate(Rabbit other)
    {
        if (Gender == other.Gender || IsPregnant || other.IsPregnant) return;
        var female = Gender == Gender.Female ? this : other;
        if (female.CanBecomePregnant())
        {
            female.PregnancyDate = SimulatedTime.Instance.Now;
            female.PregnancyDuration = _randomizerService.GetPregnancyDuration();   
        }
    }

    public void Age()
    {
        var now = SimulatedTime.Instance.Now;
        if (IsPregnant && PregnancyDate.HasValue)
        {
            var duration = (now - PregnancyDate.Value).Days;
            if (duration >= PregnancyDuration)
            {
                GiveBirth();
            }
        }

        if (now >= DeathDate)
        {
            IsDead = true;
            OnDeath?.Invoke(this);
        }
    }

    private void GiveBirth()
    {
        PregnancyDate = null;
        PregnancyDuration = null;
        var birthCount = _randomizerService.GetBirthCount();
        OnBirth?.Invoke(Enumerable.Range(0, birthCount)
            .Select(_ => new Rabbit(_randomizerService, _randomizerService.GetGender())));
    }
}

public enum Gender
{
    Male = -1,
    Female = 1
}