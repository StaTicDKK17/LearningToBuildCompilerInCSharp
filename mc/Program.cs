using mc.CodeAnalysis.Binding;
using mc.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis;

namespace Minsk;

// EXAMPLE OF A SYNTAX TREE OF A SIMPLE EXPRESSION
// 1 + 2 * 3
//      +
//     / \
//    1   *
//       / \
//      2   3

internal class Program
{
    internal static void Main()
    {
        var showTree = false; 

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                return;

            if (line == "#showTree")
            {
                showTree = !showTree;
                Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
                continue;
            }
            else if (line == "#cls")
            {
                Console.Clear();
                continue;
            }
            

            var syntaxTree = SyntaxTree.Parse(line);
            var binder = new Binder();
            var boundExpression = binder.BindExpression(syntaxTree.Root);

            IReadOnlyList<string> diagnostics = syntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();

            if (showTree)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                PrettyPrint(syntaxTree.Root);
                Console.ResetColor();
            }

            if (!diagnostics.Any())
            {
                var e = new Evaluator(boundExpression);
                var result = e.Evalate();
                Console.WriteLine(result);
                
            } else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                foreach (var diagnostic in syntaxTree.Diagnostics)
                    Console.WriteLine(diagnostic);

                Console.ResetColor();
            }
        }
    }

    static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        Console.Write(indent);
        Console.Write(marker);

        Console.Write(node.Kind);

        if (node is SyntaxToken t && t.Value != null)
        {
            Console.Write(" ");
            Console.Write(t.Value);
        }

        Console.WriteLine();

        indent += isLast ? "   " : "|    ";

        var lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
            PrettyPrint(child, indent, child == lastChild);
    }
}