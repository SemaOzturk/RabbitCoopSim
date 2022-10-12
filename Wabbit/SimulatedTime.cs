namespace Wabbit;

public class SimulatedTime
{
    public static SimulatedTime Instance = new SimulatedTime();
    public DateTime Now { get; private set; } = DateTime.MinValue;
    public DateTime Add(TimeSpan timeSpan)
    {
        Now = Now.Add(timeSpan);
        return Now;
    }
    public DateTime AddDays(double days)
    {
        Now = Now.AddDays(days);
        return Now;
    }
}