namespace Shiron.DemoProject;

public class DemoClass {
    public class NestedClass {
        public void NestedMethod() {
            Console.WriteLine("Hello from NestedClass!");
        }
    }

    public enum DemoInnerEnum {
        OptionA,
        OptionB,
        OptionC
    }

    public string Name { get; set; } = string.Empty;
    public int Value { get; private set; } = 0;

    private readonly int _magicNumber;

    public DemoClass(int magicNumber) {
        _magicNumber = magicNumber;
    }
    public DemoClass() {
        _magicNumber = 42;
    }

    public void IncrementValue(int amount) {
        Value += amount;
    }
    public void IncrementValue() {
        Value += 1;
    }
    public void IncrementValue(long amount) {
        Value += (int) amount;
    }

    public void PrintInfo() {
        Console.WriteLine($"Name: {Name}, Value: {Value}, MagicNumber: {_magicNumber}");
    }

    public static void StaticMethodExample() {
        Console.WriteLine("This is a static method.");
    }
    public async Task<string> AsyncMethodExample(int delayMilliseconds) {
        await Task.Delay(delayMilliseconds);
        return "Async method completed.";
    }

    public void LambdaMethod(Func<int, int> transformer) {
        int original = 10;
        int transformed = transformer(original);
        Console.WriteLine($"Original: {original}, Transformed: {transformed}");
    }
}

public interface IDemoInterface {
    void InterfaceMethod();
}
public sealed class DemoSealedClass : IDemoInterface {
    public void InterfaceMethod() {
        Console.WriteLine("Interface method implemented.");
    }
}

public enum DemoEnum {
    FirstOption,
    SecondOption,
    ThirdOption
}
