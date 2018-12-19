using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Example
{
    public static class AttributeSyntaxExtension
    {
        public static string GetValue(this AttributeSyntax syntax)
        {
            return syntax.ArgumentList.Arguments.Select((s => s.Expression.ToFullString().Trim('"'))).First();
        }
    }
}