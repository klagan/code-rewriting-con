using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using SQLitePCL;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalysis.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(
                @"using System;
using System.Collections;
using System.Linq;
using System.Text;
 
namespace HelloWorld
{
    class Program
    {
        static void Main([KamTest][FromHeader(Name = ""kamtest"")] string[] args, [FromHeader] int testParam)
        {
            Console.WriteLine(""Hello, World!"");
        }

static void Test([FromBody(Name=""kam"")] string a, int b)
{
}
    }
}");

            var node = (CompilationUnitSyntax) tree.GetRoot();

            new KamWalker().Visit(node);

            var g = new KamWriter();
            var gg = g.Visit(node);
            var ggg = gg.ToFullString();


            NameSyntax name = IdentifierName("FromHeader");

            var tt = node.ToFullString();

            var root = (CompilationUnitSyntax)tree.GetRoot();

            var oldUsing = root.Usings[1];
            var newUsing = oldUsing.WithName(name);


            var k = "j";



            var firstMember = root.Members[0];
            var helloWorldDeclaration = (NamespaceDeclarationSyntax)firstMember;
            var programDeclaration = (ClassDeclarationSyntax)helloWorldDeclaration.Members[0];
            var mainDeclaration = (MethodDeclarationSyntax)programDeclaration.Members[0];
            var argsParameter = mainDeclaration.ParameterList.Parameters[0];


            var classes = from classDeclaration in root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                select classDeclaration;

            var t = classes.ToList()[0];

                      var firstParameters = from methodDeclaration in root.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                where methodDeclaration.Identifier.ValueText == "Main"
                select methodDeclaration.ParameterList.Parameters.First();

            var argsParameter2 = firstParameters.Single();

            var kam = "kam";

            //// Attempt to set the version of MSBuild.
            //var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            //var instance = visualStudioInstances.Length == 1
            //    // If there is only one instance of MSBuild on this machine, set that as the one to use.
            //    ? visualStudioInstances[0]
            //    // Handle selecting the version of MSBuild you want to use.
            //    : SelectVisualStudioInstance(visualStudioInstances);

            //Console.WriteLine($"Using MSBuild at '{instance.MSBuildPath}' to load projects.");

            //// NOTE: Be sure to register an instance with the MSBuildLocator 
            ////       before calling MSBuildWorkspace.Create()
            ////       otherwise, MSBuildWorkspace won't MEF compose.
            //MSBuildLocator.RegisterInstance(instance);

            //using (var workspace = MSBuildWorkspace.Create())
            //{
            //    // Print message for WorkspaceFailed event to help diagnosing project load failures.
            //    workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);

            //    var solutionPath = args[0];
            //    Console.WriteLine($"Loading solution '{solutionPath}'");

            //    // Attach progress reporter so we print projects as they are loaded.
            //    var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
            //    Console.WriteLine($"Finished loading solution '{solutionPath}'");

            //    // TODO: Do analysis on the projects in the loaded solution
            //}
        }

        private static VisualStudioInstance SelectVisualStudioInstance(VisualStudioInstance[] visualStudioInstances)
        {
            Console.WriteLine("Multiple installs of MSBuild detected please select one:");
            for (int i = 0; i < visualStudioInstances.Length; i++)
            {
                Console.WriteLine($"Instance {i + 1}");
                Console.WriteLine($"    Name: {visualStudioInstances[i].Name}");
                Console.WriteLine($"    Version: {visualStudioInstances[i].Version}");
                Console.WriteLine($"    MSBuild Path: {visualStudioInstances[i].MSBuildPath}");
            }

            while (true)
            {
                var userResponse = Console.ReadLine();
                if (int.TryParse(userResponse, out int instanceNumber) &&
                    instanceNumber > 0 &&
                    instanceNumber <= visualStudioInstances.Length)
                {
                    return visualStudioInstances[instanceNumber - 1];
                }
                Console.WriteLine("Input not accepted, try again.");
            }
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress loadProgress)
            {
                var projectDisplay = Path.GetFileName(loadProgress.FilePath);
                if (loadProgress.TargetFramework != null)
                {
                    projectDisplay += $" ({loadProgress.TargetFramework})";
                }

                Console.WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
            }
        }
    }

    public class KamWalker : CSharpSyntaxWalker
    {
        public override void VisitParameterList(ParameterListSyntax node)
        {
            base.VisitParameterList(node);
        }
    }

    public class KamWriter : CSharpSyntaxRewriter
    {
        //public override SyntaxNode VisitParameter(ParameterSyntax node)
        //{
        //    var paramName = node.Identifier.ToFullString();
        //    var name = ParseName("MyAttribute");
        //    var arguments = ParseAttributeArgumentList($"(Name = \"{paramName}\")");
        //    var attribute = Attribute(name, arguments);

        //    var attributeList = new SeparatedSyntaxList<AttributeSyntax>();
        //    attributeList = attributeList.Add(attribute);
        //    var list = AttributeList(attributeList);

        //    if (node.AttributeLists.Any())
        //    {

        //        foreach (var a in node.AttributeLists)
        //        {

        //        }

        //        return node.ReplaceNode(node.AttributeLists[0], list);
        //    }

            
        
        //    return base.VisitParameter(node);
        //}


        public override SyntaxNode VisitAttribute(AttributeSyntax node)
        {
            SyntaxNode parent = node;

            while (parent.Kind() != SyntaxKind.Parameter)
            {
                parent = parent.Parent;
            }

            if (node.Name.ToFullString() != "FromHeader")
            {
                return base.VisitAttribute(node);
            }

            if (!(parent is ParameterSyntax)) return base.VisitAttribute(node);
        
            var parameterName = (parent as ParameterSyntax).Identifier.ToFullString();

            var name = ParseName("MyAttribute");
            var arguments = ParseAttributeArgumentList($"(Name = \"{parameterName}\")");
            var attribute = Attribute(name, arguments);

            //var attributeList = new SeparatedSyntaxList<AttributeSyntax>();
            //attributeList = attributeList.Add(attribute);
            //var list = AttributeList(attributeList);

            node = node.ReplaceNode(node, attribute);
            
            return base.VisitAttribute(node);

        }
    }
}
