using BenchmarkDotNet.Attributes;

[MemoryDiagnoser(false)]
public class MethodGroupBenchmarks
{
    private static readonly List<int> Ages = Enumerable.Range(0, 100).ToList();

    static Func<int, bool> AgeFilterFunc = x => x > 50;

    static Predicate<int> AgeFilterPredicate = x => x > 50;

    static bool AgeFilterMetod(int x)
    {
        return x > 50;
    }

    [Benchmark]
    public int Sum_Explicit()
    {
        return Ages.Where(x => AgeFilterMetod(x)).Sum();
    }

    [Benchmark]
    public int Sum_MethodGroup()
    {
        return Ages.Where(AgeFilterMetod).Sum();
    }

    [Benchmark]
    public int Sum_Func()
    {
        return Ages.Where(AgeFilterFunc).Sum();
    }

    [Benchmark]
    public int Sum_Predicate()
    {
        return Ages.Where(x => AgeFilterPredicate(x)).Sum();
    }
}