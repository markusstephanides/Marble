namespace Marble.Generator.Workers
{
    public class ControllerExplorer
    {
        // private readonly ClassFileFinder classFileFinder;
        //
        // private readonly string marbleControllerAttributeName =
        //     nameof(MarbleControllerAttribute).Replace("Attribute", string.Empty);
        //
        // private readonly string marbleProcedureAttributeName =
        //     nameof(MarbleProcedureAttribute).Replace("Attribute", string.Empty);
        //
        // private readonly string projectPath;
        //
        // public ControllerExplorer(string projectPath)
        // {
        //     this.projectPath = projectPath;
        //     this.classFileFinder = new ClassFileFinder(this.projectPath);
        // }
        //
        // public IEnumerable<ControllerDescriptor> Find()
        // {
        //     var classFiles = this.classFileFinder.Find();
        //
        //     return classFiles
        //         .Select(this.GetControllerDescriptorIfExists)
        //         .Where(descriptor => descriptor != null)
        //         .ToList();
        // }
        //
        // private ControllerDescriptor GetControllerDescriptorIfExists(string filePath)
        // {
        //     var fileContent = File.ReadAllText(filePath);
        //     var tree = CSharpSyntaxTree.ParseText(fileContent);
        //     var root = tree.GetCompilationUnitRoot();
        //     var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        //
        //     // foreach (var classDeclarationSyntax in classes)
        //     // foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        //     // foreach (var attributeSyntax in attributeListSyntax.Attributes)
        //     // {
        //     //     if (attributeSyntax.Name.ToString() == this.marbleControllerAttributeName)
        //     //     {
        //     //         // argumentlist null
        //     //         var nameArgument = attributeSyntax.ArgumentList?
        //     //             .Arguments.FirstOrDefault(argument => argument?.NameEquals?.Name.Identifier.Text ==
        //     //                                                   nameof(MarbleControllerAttribute.Name));
        //     //         var controllerName = nameArgument?.Expression.ToString() ??
        //     //                              ControllerName.FromClassName(classDeclarationSyntax.Identifier.Text);
        //     //
        //     //         return new ControllerDescriptor
        //     //         {
        //     //             ClassName = controllerName,
        //     //             Name = controllerName,
        //     //             ProcedureDescriptors = this.FindProcedures(classDeclarationSyntax)
        //     //         };
        //     //     }
        //     // }
        //
        //     return null;
        // }
        //
        // private IEnumerable<ProcedureDescriptor> FindProcedures(ClassDeclarationSyntax declarationSyntax)
        // {
        //     var procedureDescriptor = new List<ProcedureDescriptor>();
        //     var methods = declarationSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>();
        //
        //     // foreach (var methodDeclarationSyntax in methods)
        //     // foreach (var attributeListSyntax in methodDeclarationSyntax.AttributeLists)
        //     // foreach (var attributeSyntax in attributeListSyntax.Attributes)
        //     // {
        //     //     if (attributeSyntax.Name.ToString() != this.marbleProcedureAttributeName ||
        //     //         methodDeclarationSyntax.Modifiers.All(token => token.Text != "public"))
        //     //     {
        //     //         continue;
        //     //     }
        //     //
        //     //     var arguments = methodDeclarationSyntax.ParameterList.Parameters
        //     //         .Select(parameterListParameter => new ProcedureArgument
        //     //         {
        //     //             Name = parameterListParameter.Identifier.ToString(),
        //     //             Type = parameterListParameter.Type.ToString()
        //     //         }).ToList();
        //     //
        //     //     var nameArgument = attributeSyntax.ArgumentList?
        //     //         .Arguments.FirstOrDefault(argument => argument?.NameEquals?.Name.Identifier.Text ==
        //     //                                               nameof(MarbleProcedureAttribute.Name));
        //     //     var procedureName = nameArgument?.Expression.ToString() ??
        //     //                         ProcedureName.FromMethodName(methodDeclarationSyntax.Identifier.Text);
        //     //
        //     //     procedureDescriptor.Add(new ProcedureDescriptor
        //     //     {
        //     //         Name = procedureName,
        //     //         MethodName = methodDeclarationSyntax.Identifier.Text,
        //     //         ReturnType = this.AdjustReturnType(methodDeclarationSyntax.ReturnType.ToString()),
        //     //         Arguments = arguments
        //     //     });
        //     // }
        //
        //     return procedureDescriptor;
        // }
        //
        // private string AdjustReturnType(string originalReturnType)
        // {
        //     if (originalReturnType == "void" || originalReturnType == "Task")
        //     {
        //         return null;
        //     }
        //
        //     var pattern = @"Task<(.*)>";
        //     var options = RegexOptions.Singleline;
        //     var matches = Regex.Matches(originalReturnType, pattern, options);
        //
        //     return matches.Count > 0 ? matches[0].Groups[1].Value : originalReturnType;
        // }
    }
}