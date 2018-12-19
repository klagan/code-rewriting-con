using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Example
{
    public class MyMethodAndParameterWriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            AttributeSyntax route, operation;

            foreach (var list in node.AttributeLists)
            {
                foreach (var attribute in list.Attributes)
                {
                    if (attribute.Name.ToFullString() == "Route")
                    {
                        route = attribute;
                    }

                    if (attribute.Name.ToFullString() == "SwaggerOperation")
                    {
                        operation = attribute;
                    }
                }
            }

            var parameters = node.ParameterList;

            var newParams = SyntaxFactory.ParameterList();

            foreach (var param in parameters.Parameters)
            {
                var parameterName = param.Identifier.ToFullString();
                
                if (parameterName == "args")
                {
                    var fullList = new List<AttributeListSyntax>();

                    var attributeList = new SeparatedSyntaxList<AttributeSyntax>();
                    var name = SyntaxFactory.ParseName("MyAttribute");
                    var arguments = SyntaxFactory.ParseAttributeArgumentList($"(Name = \"{parameterName}\")");
                    var attribute = SyntaxFactory.Attribute(name, arguments);
                    attributeList = attributeList.Add(attribute);
                    
                    fullList.Add(SyntaxFactory.AttributeList(attributeList).WithTrailingTrivia(SyntaxFactory.Space));

                    attributeList = new SeparatedSyntaxList<AttributeSyntax>();
                    name = SyntaxFactory.ParseName("MyOtherAttribute");
                    arguments = SyntaxFactory.ParseAttributeArgumentList($"(\"{parameterName}\")");
                    attribute = SyntaxFactory.Attribute(name, arguments);
                    attributeList = attributeList.Add(attribute);
                    
                    fullList.Add(SyntaxFactory.AttributeList(attributeList));
                    
                    var newParam = param.WithAttributeLists(new SyntaxList<AttributeListSyntax>(fullList));
                    
                    newParams = newParams.AddParameters(newParam.WithLeadingTrivia(SyntaxFactory.Space));
                }

                if (parameterName == "testParam")
                {
                    var fullList = new List<AttributeListSyntax>();

                    var attributeList = new SeparatedSyntaxList<AttributeSyntax>();
                    var name = SyntaxFactory.ParseName("MyTest");
                    var arguments = SyntaxFactory.ParseAttributeArgumentList($"(Name = \"{parameterName}\")");
                    var attribute = SyntaxFactory.Attribute(name, arguments);
                    attributeList = attributeList.Add(attribute);

                    fullList.Add(SyntaxFactory.AttributeList(attributeList));

                    var newParam = param.WithAttributeLists(new SyntaxList<AttributeListSyntax>(fullList));

                    newParams = newParams.AddParameters(newParam.WithLeadingTrivia(SyntaxFactory.Space));
                }
            }

            node = node.WithParameterList(newParams);

            return base.VisitMethodDeclaration(node);
        }
    }
}