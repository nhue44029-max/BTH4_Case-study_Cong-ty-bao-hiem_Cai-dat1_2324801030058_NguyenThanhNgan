using System.Globalization;

namespace PricingService.Domain;

// The tariff format supplied with the exercise uses numbers, answer names, comparisons,
// arithmetic, parentheses and conditional expressions. Reject unknown syntax explicitly.
internal sealed class RuleExpression
{
    private readonly string source;
    private readonly IReadOnlyDictionary<string, object> answers;
    private int position;

    private RuleExpression(string source, IReadOnlyDictionary<string, object> answers)
    {
        this.source = source;
        this.answers = answers;
    }

    public static object Evaluate(string source, IReadOnlyDictionary<string, object> answers)
    {
        var parser = new RuleExpression(source, answers);
        var result = parser.Conditional();
        parser.SkipWhitespace();
        if (parser.position != source.Length)
            throw new FormatException($"Unexpected tariff expression at position {parser.position}: {source}");
        return result;
    }

    private object Conditional()
    {
        var condition = Comparison();
        if (!Take('?')) return condition;
        var whenTrue = Conditional();
        Require(':');
        var whenFalse = Conditional();
        return Convert.ToBoolean(condition) ? whenTrue : whenFalse;
    }

    private object Comparison()
    {
        var left = Sum();
        if (Take("==")) return Equals(left, Sum());
        if (Take('>')) return Convert.ToDecimal(left) > Convert.ToDecimal(Sum());
        return left;
    }

    private object Sum()
    {
        var value = Product();
        while (Take('+')) value = Convert.ToDecimal(value) + Convert.ToDecimal(Product());
        return value;
    }

    private object Product()
    {
        var value = Primary();
        while (Take('*')) value = Convert.ToDecimal(value) * Convert.ToDecimal(Primary());
        return value;
    }

    private object Primary()
    {
        SkipWhitespace();
        if (Take('('))
        {
            var value = Conditional();
            Require(')');
            return value;
        }
        if (Take('"'))
        {
            var start = position;
            while (position < source.Length && source[position] != '"') position++;
            var value = source[start..position];
            Require('"');
            return value;
        }
        var tokenStart = position;
        while (position < source.Length && (char.IsLetterOrDigit(source[position]) || source[position] is '_' or '.')) position++;
        if (tokenStart == position) throw new FormatException($"Expected tariff value at position {position}: {source}");
        var token = source[tokenStart..position];
        if (decimal.TryParse(token.TrimEnd('M'), NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
            return number;
        if (!answers.TryGetValue(token, out var answer)) throw new KeyNotFoundException($"Missing answer {token}.");
        return answer;
    }

    private bool Take(char character)
    {
        SkipWhitespace();
        if (position >= source.Length || source[position] != character) return false;
        position++;
        return true;
    }

    private bool Take(string text)
    {
        SkipWhitespace();
        if (!source.AsSpan(position).StartsWith(text, StringComparison.Ordinal)) return false;
        position += text.Length;
        return true;
    }

    private void Require(char character)
    {
        if (!Take(character)) throw new FormatException($"Expected '{character}' at position {position}: {source}");
    }

    private void SkipWhitespace()
    {
        while (position < source.Length && char.IsWhiteSpace(source[position])) position++;
    }
}
