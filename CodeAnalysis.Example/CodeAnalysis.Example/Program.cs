using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        [SwaggerOperation(""SomethingOrOther"")]
        [Route(""MyRoute""]
        static string Main([KamTest][FromHeader(Name = ""kamtest"")] string[] args, [FromHeader] int testParam)
        {
            Console.WriteLine(""My Test"");
        }

        static string Test([FromBody(Name=""kam"")] string a, int b)
        {
           return ""1"";
        }
    }
}");
            var node = (CompilationUnitSyntax) tree.GetRoot();
            
            //var newSourceCode = new MyRewriter().Visit(node).ToFullString();

            var newSourceCode = new MyMethodAndParameterWriter().Visit(node).ToFullString();

            Console.WriteLine($"{newSourceCode}");

            Console.ReadLine();
        }
    }
}
