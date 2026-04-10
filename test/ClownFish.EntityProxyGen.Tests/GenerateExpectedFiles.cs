using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.EntityProxyGen.Tests
{
    [TestClass]
    public class GenerateExpectedFiles
    {
        internal static readonly string ModelsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Models");
        internal static readonly string GeneratedDir = Path.Combine(ModelsDir, "_Generated");

        [TestMethod]
        public void CreateAllExpectedFiles()
        {
            Directory.CreateDirectory(GeneratedDir);
            Directory.GetFiles(GeneratedDir, "*", SearchOption.TopDirectoryOnly)
                .ToList()
                .ForEach(f => File.Delete(f));

            string[] modelFiles = Directory.GetFiles(ModelsDir, "*.cs", SearchOption.TopDirectoryOnly);

            foreach (var file in modelFiles)
            {
                var text = File.ReadAllText(file);
                var syntaxTree = CSharpSyntaxTree.ParseText(text);

                var refs = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => false == a.IsDynamic && false == string.IsNullOrEmpty(a.Location))
                    .Select(a => MetadataReference.CreateFromFile(a.Location))
                    .Distinct()
                    .ToList();

                CSharpCompilation compilation = CSharpCompilation.Create("GenTemp")
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddReferences(refs)
                    .AddSyntaxTrees(syntaxTree);

                SemanticModel model = compilation.GetSemanticModel(syntaxTree);
                SyntaxNode root = syntaxTree.GetRoot();
                ClassDeclarationSyntax classDecl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
                if (classDecl == null)
                    continue;

                var symbol = model.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
                if (symbol == null)
                    continue;


                string generated = Gen1.GenerateForEntity(symbol, true);

                string expectedPath = Path.Combine(GeneratedDir, symbol.Name + ".proxy.expected.cs");
                File.WriteAllText(expectedPath, generated, Encoding.UTF8);
            }

            // succeed
            //Assert.IsTrue(true);
        }
    }
}
