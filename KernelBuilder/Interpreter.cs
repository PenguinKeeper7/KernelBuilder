using System.Text;

/*
 * The majority of this PHP interpreter file was created by AI.
 * I have no clue what half of it does but it works extremely well and is thoroughly tested
 * -Penguin
 */

namespace KernelBuilder
{
    // --- Expression AST definitions ---
    public interface IExpression { }

    // Represents a variable like $plain (stored as plain)
    public class Variable : IExpression
    {
        public string Name;
        public Variable(string name) { Name = name; }
        public override string ToString() => "$" + Name;
    }

    // Represents a function call like md5(...) or sha1(...)
    public class FunctionCall : IExpression
    {
        public string FunctionName;
        public List<IExpression> Arguments = new List<IExpression>();
        public string OutputId; // Assigned during instruction generation

        public FunctionCall(string name)
        {
            FunctionName = name;
        }

        public override string ToString() => $"{FunctionName}({string.Join(", ", Arguments)})";
    }

    // Represents a concatenation operation: expressions separated by "."
    public class Concat : IExpression
    {
        public List<IExpression> Parts = new List<IExpression>();
        public override string ToString() => string.Join(" . ", Parts);
    }

    // --- Recursive Descent Parser ---
    public class Parser
    {
        private readonly string input;
        private int pos;

        public Parser(string input)
        {
            this.input = input;
            pos = 0;
        }

        // Entry point to parse an expression.
        public IExpression ParseExpression()
        {
            return ParseConcat();
        }

        // Parses concatenated terms using the '.' operator.
        private IExpression ParseConcat()
        {
            IExpression expr = ParseTerm();
            List<IExpression> parts = new List<IExpression> { expr };

            while (Match('.'))
            {
                IExpression next = ParseTerm();
                parts.Add(next);
            }

            if (parts.Count == 1)
                return parts[0];
            else
            {
                Concat concat = new Concat();
                concat.Parts.AddRange(parts);
                return concat;
            }
        }

        // Parses a single term (variable, function call, or a parenthesized expression)
        private IExpression ParseTerm()
        {
            SkipWhitespace();
            if (pos >= input.Length)
                throw new Exception("Unexpected end of input.");

            char current = Current();
            if (current == '$')
            {
                return ParseVariable();
            }
            else if (Char.IsLetter(current))
            {
                return ParseFunctionCall();
            }
            else if (current == '(')
            {
                Consume('(');
                IExpression expr = ParseExpression();
                Consume(')');
                return expr;
            }
            throw new Exception("Unexpected character '" + current + "' at position " + pos);
        }

        // Parses a variable of the form $varName.
        private Variable ParseVariable()
        {
            Consume('$');
            StringBuilder sb = new StringBuilder();
            while (pos < input.Length && (Char.IsLetterOrDigit(Current()) || Current() == '_'))
            {
                sb.Append(Current());
                pos++;
            }
            return new Variable(sb.ToString());
        }

        // Parses a function call of the form funcName(expression)
        private FunctionCall ParseFunctionCall()
        {
            StringBuilder sb = new StringBuilder();
            // Modified: include digits (and underscores) in the function name.
            while (pos < input.Length && (Char.IsLetterOrDigit(Current()) || Current() == '_'))
            {
                sb.Append(Current());
                pos++;
            }
            string funcName = sb.ToString();
            FunctionCall fc = new FunctionCall(funcName);
            SkipWhitespace();
            if (pos < input.Length && Current() == '(')
            {
                Consume('(');
                // In this example we assume a single argument that may be a concatenation.
                IExpression arg = ParseExpression();
                fc.Arguments.Add(arg);
                Consume(')');
            }
            return fc;
        }

        // Helper method to skip whitespace characters.
        private void SkipWhitespace()
        {
            while (pos < input.Length && Char.IsWhiteSpace(Current()))
                pos++;
        }

        // Returns the current character.
        private char Current() => input[pos];

        // Checks if the current character matches 'expected' and advances the position if so.
        private bool Match(char expected)
        {
            SkipWhitespace();
            if (pos < input.Length && input[pos] == expected)
            {
                pos++;
                return true;
            }
            return false;
        }

        // Consumes the expected character; throws an exception if not found.
        private void Consume(char expected)
        {
            SkipWhitespace();
            if (pos < input.Length && input[pos] == expected)
                pos++;
            else
                throw new Exception("Expected '" + expected + "' at position " + pos);
        }
    }

    // --- Instruction Generator ---
    public class InstructionGenerator
    {
        private int idCounter = 1;
        private List<string> instructions = new List<string>();

        // Recursively generate instructions for an expression.
        // Returns a string representing either a variable name or a function call output identifier.
        public string Generate(IExpression expr)
        {
            if (expr is Variable variable)
            {
                return variable.Name;
            }
            else if (expr is Concat concat)
            {
                List<string> parts = new List<string>();
                foreach (var part in concat.Parts)
                {
                    parts.Add(Generate(part));
                }
                // The concatenation itself is not a function call so return a combined string.
                return string.Join(", ", parts);
            }
            else if (expr is FunctionCall fc)
            {
                List<string> argOutputs = new List<string>();
                foreach (var arg in fc.Arguments)
                {
                    if (arg is Concat concatArg)
                    {
                        foreach (var part in concatArg.Parts)
                            argOutputs.Add(Generate(part));
                    }
                    else
                    {
                        argOutputs.Add(Generate(arg));
                    }
                }

                // Create a unique output id based on the function name.
                string currentId = fc.FunctionName.ToUpper() + "-" + idCounter++;
                fc.OutputId = currentId;

                // Emit an instruction for each input argument.
                foreach (var argument in argOutputs)
                {
                    string inst = currentId + " - " + argument;
                    instructions.Add(inst);
                }
                return currentId;
            }
            return "";
        }

        public List<string> GetInstructions() => instructions;
    }

    public class Interpreter
    {
        public static List<string> GenerateInstructions(string algorithm)
        {
            Parser parser = new Parser(algorithm);
            IExpression expr = parser.ParseExpression();

            InstructionGenerator generator = new InstructionGenerator();
            string result = generator.Generate(expr);

            List<string> instructions = generator.GetInstructions();

            return instructions;
        }
    }
}
