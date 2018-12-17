using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Example
{
    /// <summary>
    /// http://roslynquoter.azurewebsites.net/
    /// </summary>
    public class MyRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var mainBody =
                SyntaxFactory
                    .ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal("hello, world")))
                    .WithReturnKeyword(SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(new[] {SyntaxFactory.LineFeed, SyntaxFactory.Whitespace(" ")}),
                        SyntaxKind.ReturnKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space))).WithSemicolonToken(
                        SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.SemicolonToken,
                            SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)));

            var block = SyntaxFactory.Block(mainBody);

            var newNode = node.WithBody(block);

            return base.VisitMethodDeclaration(newNode);
        }


        public override SyntaxNode VisitAttribute(AttributeSyntax node)
        {
            var kindWhitelist = new List<SyntaxKind>
            {
                SyntaxKind.AttributeArgument,
                SyntaxKind.Attribute,
                SyntaxKind.AttributeList,
                SyntaxKind.AttributeArgumentList,
                SyntaxKind.Parameter
            };

            SyntaxNode parent = node;

            while (parent.Kind() != SyntaxKind.Parameter)
            {
                parent = parent.Parent;

                if (!kindWhitelist.Contains(parent.Kind()))
                {
                    return base.VisitAttribute(node);
                }
            }

            if (node.Name.ToFullString() != "FromHeader")
            {
                return base.VisitAttribute(node);
            }

            if (!(parent is ParameterSyntax)) return base.VisitAttribute(node);
        
            var parameterName = (parent as ParameterSyntax).Identifier.ToFullString();

            var name = SyntaxFactory.ParseName("MyAttribute");
            var arguments = SyntaxFactory.ParseAttributeArgumentList($"(Name = \"{parameterName}\")");
            var attribute = SyntaxFactory.Attribute(name, arguments);

            //var attributeList = new SeparatedSyntaxList<AttributeSyntax>();
            //attributeList = attributeList.Add(attribute);
            //var list = AttributeList(attributeList);

            node = node.ReplaceNode(node, attribute);
            
            return base.VisitAttribute(node);
        }
    }
}