namespace Shiron.DemoProject;

/// <summary>
/// A class that attempts to orchestrate order in a chaotic universe, much like a cat trying to herd humans.
/// It was created by <see cref="https://github.com/iamshiron"/> on a quest to bring structured documentation to the wild west of codebases.
/// Here is another link using href syntax: <see href="https://github.com/iamshiron">Shiron's GitHub</see>.
/// </summary>
/// <remarks>
/// <para>
/// This class serves as the foundational block for the <see cref="Shiron.DemoProject"/> namespace.
/// It exists in a superposition of states, balancing internal truth (<see cref="Value"/>) with external perception (<see cref="Name"/>).
/// </para>
/// <list type="bullet">
/// <item>
/// <description> existential dread management.</description>
/// </item>
/// <item>
/// <description>Async capabilities reminiscent of waiting for a toaster to pop.</description>
/// </item>
/// </list>
/// </remarks>
public class DemoClass {
    /// <summary>
    /// Represents a nested layer of logic, hidden away like that one embarrassing memory from high school.
    /// </summary>
    public class NestedClass {
        /// <summary>
        /// Executes the nested logic, screaming into the void.
        /// </summary>
        public void NestedMethod() {
            Console.WriteLine("Hello from NestedClass!");
        }
    }

    /// <summary>
    /// Defines the illusion of choice.
    /// <list type="bullet">
    /// <item>
    /// OptionA: The safe path.
    /// </item>
    /// <item>
    /// OptionB: The adventurous path.
    /// </item>
    /// <item>
    /// OptionC: The chaotic path.
    /// </item>
    /// </list>
    /// </summary>
    public enum DemoInnerEnum {
        /// <summary>
        /// The path of least resistance.
        /// </summary>
        OptionA,
        /// <summary>
        /// The road less traveled, which usually just means it has more potholes.
        /// </summary>
        OptionB,
        /// <summary>
        /// The option you pick when you want to watch the world <strong>burn</strong>.
        /// </summary>
        OptionC
    }

    /// <summary>
    /// Gets or sets the name of the entity.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the identity.
    /// Defaults to <see cref="string.Empty"/>, symbolizing the tabula rasa of the soul before life writes its messy code upon it.
    /// </value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the current accumulation of arbitrary worth.
    /// </summary>
    /// <remarks>
    /// This value is private-set because self-worth should come from within, not from external setters.
    /// </remarks>
    public int Value { get; private set; } = 0;

    /// <summary>
    /// A constant that governs the laws of this specific instance's reality.
    /// </summary>
    private readonly int _magicNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="DemoClass"/> with a specific magic number.
    /// </summary>
    /// <param name="magicNumber">The initial seed value. Choose wisely, for it determines the flavor of your entropy.</param>
    public DemoClass(int magicNumber) {
        _magicNumber = magicNumber;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DemoClass"/> with the answer to life, the universe, and everything.
    /// </summary>
    public DemoClass() {
        _magicNumber = 42;
    }

    /// <summary>
    /// Increments the <see cref="Value"/> by a specific amount.
    /// </summary>
    /// <param name="amount">The quantity of "stuff" to add to the pile.</param>
    public void IncrementValue(int amount) {
        Value += amount;
    }

    /// <summary>
    /// Increments the <see cref="Value"/> by 1.
    /// </summary>
    /// <remarks>
    /// A small step for the code, but... well, it's just a small step. Don't overthink it.
    /// </remarks>
    public void IncrementValue() {
        Value += 1;
    }

    /// <summary>
    /// Increments the <see cref="Value"/> using a long integer.
    /// </summary>
    /// <param name="amount">A burden potentially too heavy for a mere integer to bear.</param>
    /// <exception cref="OverflowException">
    /// Thrown if <paramref name="amount"/> exceeds the integer limits, proving that even code has a breaking point.
    /// </exception>
    /// <seealso cref="IncrementValue(int)"/>
    public void IncrementValue(long amount) {
        Value += (int) amount;
    }

    /// <summary>
    /// Screams the current state of the object into the console void.
    /// </summary>
    /// <example>
    /// <code>
    /// var demo = new DemoClass(100);
    /// demo.PrintInfo(); // If a tree falls in the console and no one is reading logs, does it make a sound?
    /// </code>
    /// </example>
    public void PrintInfo() {
        Console.WriteLine($"Name: {Name}, Value: {Value}, MagicNumber: {_magicNumber}");
    }

    /// <summary>
    /// A static method, floating freely without the baggage of an instance.
    /// </summary>
    public static void StaticMethodExample() {
        Console.WriteLine("This is a static method.");
    }

    /// <summary>
    /// Asynchronously contemplates existence after a specified delay.
    /// </summary>
    /// <param name="delayMilliseconds">The time to procrastinate, in milliseconds.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, eventually yielding a string when it feels like it.</returns>
    /// <remarks>
    /// Time is an illusion, but this delay is very real.
    /// </remarks>
    public async Task<string> AsyncMethodExample(int delayMilliseconds) {
        await Task.Delay(delayMilliseconds);
        return "Async method completed.";
    }

    /// <summary>
    /// Applies a transformation function, mutating the essence of a value.
    /// </summary>
    /// <param name="transformer">The agent of change.</param>
    public void LambdaMethod(Func<int, int> transformer) {
        int original = 10;
        int transformed = transformer(original);
        Console.WriteLine($"Original: {original}, Transformed: {transformed}");
    }

    /// <summary>
    /// A method that has been deprecated, like a VHS tape in the age of streaming.
    /// </summary>
    /// <remarks>
    /// Use <see cref="NewMethod"/> instead, because progress marches on, whether we like it or not.
    /// </remarks>
    [Obsolete("This method is obsolete. Use NewMethod instead.")]
    public void DeprecatedMethod() {
        Console.WriteLine("This method is deprecated. Use NewMethod instead.");
    }
    public void NewMethod() {
        Console.WriteLine("This is the new and improved method.");
    }
}

/// <summary>
/// Defines a contract, a binding promise that cannot be broken (unless you throw a NotImplementedException).
/// </summary>
public interface IDemoInterface {
    /// <summary>
    /// The thing that must be done.
    /// </summary>
    void InterfaceMethod();
}

/// <summary>
/// A sealed implementation of <see cref="IDemoInterface"/>.
/// </summary>
/// <remarks>
/// This class is sealed, hermitically closed off from the dangers of inheritance. It is complete. It is final.
/// </remarks>
public sealed class DemoSealedClass : IDemoInterface {
    /// <inheritdoc />
    public void InterfaceMethod() {
        Console.WriteLine("Interface method implemented.");
    }
}

/// <summary>
/// An enumeration of life choices.
/// </summary>
/// <remarks>
/// Choosing an option here is like choosing a path in life; each has its own consequences, none of which can be undone.
/// </remarks>
public enum DemoEnum {
    /// <summary>
    /// The vanilla option. Safe, reliable, boring.
    /// </summary>
    FirstOption,
    /// <summary>
    /// The spicy option.
    /// </summary>
    SecondOption,
    /// <summary>
    /// The option that makes you question why you started this enumeration in the first place.
    /// </summary>
    ThirdOption
}

/// <summary>
/// A "Kitchen Sink" class used to stress-test the <see cref="Shiron.Docs.Engine.Extractor"/> logic.
/// </summary>
/// <remarks>
/// This class demonstrates complex XML documentation features.
/// <para>
/// It supports <strong>bold</strong> (if your parser allows raw HTML) or <c>inline code</c> mixed with text.
/// </para>
/// <list type="bullet">
///     <item>
///         <term>Robustness</term>
///         <description>Ensures the <see cref="XmlNodeToTokens"/> method doesn't crash.</description>
///     </item>
///     <item>
///         <term>Recursion</term>
///         <description>Handles nested tags like <c>code</c> inside lists.</description>
///     </item>
/// </list>
/// </remarks>
/// <seealso cref="System.String"/>
public class XmlStressTest {
    /// <summary>
    /// Processes data with extreme prejudice.
    /// </summary>
    /// <typeparam name="T">The type of data to process. Must implement <see cref="IDisposable"/>.</typeparam>
    /// <param name="input">
    /// The input data.
    /// <para>
    /// If null, this method throws an <see cref="ArgumentNullException"/>.
    /// </para>
    /// </param>
    /// <param name="mode">
    /// The execution mode.
    /// <list type="number">
    ///     <item>0: Fast mode (unsafe)</item>
    ///     <item>1: Safe mode (slow)</item>
    /// </list>
    /// </param>
    /// <returns>
    /// A generic wrapper around <paramref name="input"/> containing the results.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown if the <paramref name="mode"/> is invalid.</exception>
    /// <example>
    /// Here is how you use this method:
    /// <code>
    /// var processor = new XmlStressTest();
    /// var result = processor.Process&lt;int&gt;(42, 1);
    /// Console.WriteLine(result);
    /// </code>
    /// </example>
    public T Process<T>(T input, int mode) where T : IDisposable {
        return input;
    }

    /// <summary>
    /// Gets or sets the configuration level.
    /// </summary>
    /// <value>
    /// An integer representing the level. Default is <c>10</c>.
    /// </value>
    public int Level { get; set; }

    /// <summary>
    /// Renders a <b>chaotic <i>symphony <u>of <c>styles</c></u></i></b>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is a test for <b><i><u><s>MAXIMUM OVERKILL</s></u></i></b> styling.
    /// </para>
    /// <para>
    /// It verifies that:
    /// <list type="bullet">
    ///     <item>
    ///         <b>Bold</b> can contain <i>Italic</i> which can contain <c>Code</c>.
    ///     </item>
    ///     <item>
    ///         <s>Strikethrough</s> can silence a <see cref="System.Console.WriteLine()"/> link.
    ///     </item>
    ///     <item>
    ///         <u>Underline</u> can hold a <b>Bold <i>Italic</i></b> truth.
    ///     </item>
    /// </list>
    /// </para>
    /// <para>
    /// Corrected nesting: <b><i>Proper <u>nesting</u></i></b> works fine.
    /// </para>
    /// </remarks>
    public void RenderChaos() {
    }
}
