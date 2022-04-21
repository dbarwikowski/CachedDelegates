# CachedDelegates

After watching Nick Chapsas video
"[The C# 11 fix you didn’t know you needed](https://www.youtube.com/watch?v=5Lit18RJYp8)"
I was motivated to do some more tests.

What if instead of passing method name I would pass Predicate of Func stored in the same class?

```csharp
private static Func<int, bool> AgeFilter_Func = x => x > 50;
private static Predicate<int> AgeFilter_Predicate = x => x > 50;
```

## Here are my results:

|          Method |     Mean |    Error |   StdDev |   Median | Allocated |
|---------------- |---------:|---------:|---------:|---------:|----------:|
|    Sum_Explicit | 703.3 ns | 13.97 ns | 17.66 ns | 698.6 ns |      72 B |
| Sum_MethodGroup | 760.0 ns | 14.50 ns | 13.56 ns | 759.1 ns |     136 B |
|        Sum_Func | 730.3 ns | 21.35 ns | 62.63 ns | 702.3 ns |      72 B |
|   Sum_Predicate | 785.1 ns | 15.69 ns | 16.11 ns | 783.0 ns |      72 B |
