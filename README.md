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

## Sum_Explicit

### C#10

```csharp
static bool AgeFilterMetod(int x)
{
    return x > 50;
}

public int Sum_Explicit()
{
    return Ages.Where(x => AgeFilterMetod(x)).Sum();
}
```

### Lowered

```csharp
private sealed class <>c
{
    public static readonly <>c <>9 = new <>c();

    public static Func<int, bool> <>9__2_0;

    internal bool <Sum_Explicit>b__2_0(int x)
    {
        return AgeFilterMetod(x);
    }
}

public int Sum_Explicit()
{
    return Enumerable.Sum(Enumerable.Where(Ages, <>c.<>9__2_0 ?? (<>c.<>9__2_0 = new Func<int, bool>(<>c.<>9.<Sum_Explicit>b__2_0))));
}
```

## Sum_MethodGroup

### C#10
```csharp
public int Sum_MethodGroup()
{
    return Ages.Where(AgeFilterMetod).Sum();
}
```
### Lowered
```csharp
public int Sum_MethodGroup()
{
    return Enumerable.Sum(Enumerable.Where(Ages, new Func<int, bool>(AgeFilterMetod)));
}
```



## Sum_Func

### C#10
```csharp
static Func<int, bool> AgeFilterFunc = x => x > 50;

public int Sum_Func()
{
    return Ages.Where(AgeFilterFunc).Sum();
}
```
### Lowered
```csharp
private sealed class <>c
{
    public static readonly <>c <>9 = new <>c();

    internal bool <.cctor>b__4_0(int x)
    {
        return x > 50;
    }
}

private static Func<int, bool> AgeFilterFunc = new Func<int, bool>(<>c.<>9.<.cctor>b__4_0);

public int Sum_Func()
{
    return Enumerable.Sum(Enumerable.Where(Ages, AgeFilterFunc));
}
```



## Sum_Predicate

### C#10
```csharp
static Predicate<int> AgeFilterPredicate = x => x > 50;

public int Sum_Predicate()
{
    return Ages.Where(x => AgeFilterPredicate(x)).Sum();
}
```
### Lowered
```csharp
private sealed class <>c
{
    public static readonly <>c <>9 = new <>c();

    public static Func<int, bool> <>9__2_0;

    internal bool <Sum_Predicate>b__2_0(int x)
    {
        return AgeFilterPredicate(x);
    }

    internal bool <.cctor>b__4_0(int x)
    {
        return x > 50;
    }
}

private static Predicate<int> AgeFilterPredicate = new Predicate<int>(<>c.<>9.<.cctor>b__4_0);

public int Sum_Predicate()
{
    return Enumerable.Sum(Enumerable.Where(Ages, <>c.<>9__2_0 ?? (<>c.<>9__2_0 = new Func<int, bool>(<>c.<>9.<Sum_Predicate>b__2_0))));
}
```

## Summary

Use Func. Don't use predicate? Or at least use lambda (`Where(x => AgeFilter(x))`)
