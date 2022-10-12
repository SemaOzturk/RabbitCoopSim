using Redzen.Numerics.Distributions.Float;
using Redzen.Random;

namespace Wabbit;

public class RabbitRandomizerService : IRabbitRandomizerService
{
    private readonly Random _random = new();
    public int GetPregnancyDuration()
    {
        return _random.Next(28, 32);
    }

    public Gender GetGender()
    {
        return _random.Next(2) == 1 ? Gender.Male : Gender.Female;
    }

    public int GetBirthCount()
    {
        BoxMullerGaussianSampler gaussianBirthSampler = new(7,2);
        var round = (int)Math.Round(gaussianBirthSampler.Sample());
        if (round < 0)
        {
            return 0;
        }
        return round;
    }

    public int GetMaxAge()
    {
        BoxMullerGaussianSampler gaussianDeathAgeSampler = new(8,1.2f);
        var round = (int)Math.Round(gaussianDeathAgeSampler.Sample());
        if (round < 0)
        {
            return 0;
        }
        return round;
    }
}