using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marble.Core.Declaration;
using Marble.Generator.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ControllerDescriptor = Marble.Generator.Models.ControllerDescriptor;
using ProcedureDescriptor = Marble.Generator.Models.ProcedureDescriptor;

namespace Marble.Generator
{
    public class ControllerExplorer
    {
        private readonly ClassFileFinder classFileFinder;

        private readonly string marbleControllerAttributeName =
            nameof(MarbleControllerAttribute).Replace("Attribute", string.Empty);

        private readonly string marbleProcedureAttributeName =
            nameof(MarbleProcedureAttribute).Replace("Attribute", string.Empty);

        private readonly string projectPath;

        public ControllerExplorer(string projectPath)
        {
            this.projectPath = projectPath;
            this.classFileFinder = new ClassFileFinder(this.projectPath);
        }

        public IEnumerable<ControllerDescriptor> Find()
        {
            var classFiles = this.classFileFinder.Find();

            return classFiles
                .Select(this.GetControllerDescriptorIfExists)
                .Where(descriptor => descriptor != null)
                .ToList();
        }

        private ControllerDescriptor GetControllerDescriptorIfExists(string filePath)
        {
            var fileContent = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(fileContent);
            var root = tree.GetCompilationUnitRoot();
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var classDeclarationSyntax in classes)
            foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (attributeSyntax.Name.ToString() == this.marbleControllerAttributeName)
                {
                    var namespaceName = (classDeclarationSyntax.Parent as NamespaceDeclarationSyntax).Name.ToString();

                    return new ControllerDescriptor
                    {
                        ClassName = classDeclarationSyntax.Identifier.Text,
                        Name = $"{namespaceName}.{classDeclarationSyntax.Identifier.Text}",
                        ProcedureDescriptors = this.FindProcedures(classDeclarationSyntax)
                    };
                }
            }

            return null;
        }

        private IEnumerable<ProcedureDescriptor> FindProcedures(ClassDeclarationSyntax declarationSyntax)
        {
            var procedureDescriptor = new List<ProcedureDescriptor>();
            var methods = declarationSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var methodDeclarationSyntax in methods)
            foreach (var attributeListSyntax in methodDeclarationSyntax.AttributeLists)
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (attributeSyntax.Name.ToString() != this.marbleProcedureAttributeName)
                {
                    continue;
                }

                var isPublic = methodDeclarationSyntax.Modifiers.Any(token => token.Text == "public");

                if (!isPublic)
                {
                    continue;
                }

                var returnType = methodDeclarationSyntax.ReturnType.ToString();
                var arguments = methodDeclarationSyntax.ParameterList.Parameters
                    .Select(parameterListParameter => new ProcedureArgument
                    {
                        Name = parameterListParameter.Identifier.ToString(),
                        Type = parameterListParameter.Type.ToString()
                    }).ToList();

                procedureDescriptor.Add(new ProcedureDescriptor
                {
                    Name = methodDeclarationSyntax.Identifier.Text,
                    ReturnType = returnType,
                    Arguments = arguments
                });
            }

            return procedureDescriptor;
        }
    }
}