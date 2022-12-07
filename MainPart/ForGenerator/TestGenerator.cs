﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace MainPart.ForGenerator
{
    public class TestGenerator
    {
        readonly List<UsingDirectiveSyntax> _usings = new List<UsingDirectiveSyntax>();
        readonly List<NamespaceDeclarationSyntax> _namespaces = new List<NamespaceDeclarationSyntax>();
        readonly List<ClassDeclarationSyntax> _classes = new List<ClassDeclarationSyntax>();

        private string _fileName; 


        public TestGenerator(string csFileString, string nameFile)
        {
            _fileName = nameFile;
            SyntaxTree tree = CSharpSyntaxTree.ParseText(csFileString);

            _usings = tree.GetRoot()
            .DescendantNodes()
            .OfType<UsingDirectiveSyntax>().ToList();

            _namespaces = tree.GetRoot()
            .DescendantNodes()
            .OfType<NamespaceDeclarationSyntax>().ToList();

            _classes = tree.GetRoot()
           .DescendantNodes()
           .OfType<ClassDeclarationSyntax>().Where(c => c.Modifiers.Where(c => c.IsKind(SyntaxKind.PublicKeyword)).Any()).ToList();
        }

        public Dictionary<string, string> GenerateFiles()
        {
            var testFileStrings = new Dictionary<string, string>();
            
            foreach (var c in _classes)
            {
                var root = CompilationUnit()
                  .WithUsings(new SyntaxList<UsingDirectiveSyntax>(_usings)
                      .Add(UsingDirective(QualifiedName(IdentifierName("NUnit"), IdentifierName("Framework"))))
                      .Add(UsingDirective(_namespaces[0].Name))
                      )
                  .NormalizeWhitespace();
                      
                var namespaceDeclaration = NamespaceDeclaration(QualifiedName(IdentifierName(_namespaces[0].Name.ToString()), IdentifierName("Tests")));
                var classDeclaration = ClassDeclaration(Identifier(c.Identifier.Text + "Test"))
                                        .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                        .AddAttributeLists(AttributeList(SingletonSeparatedList<AttributeSyntax>(Attribute(IdentifierName("TestFixture")))));
                var methods = c
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>().Where(m => m.Modifiers.Where(m => m.IsKind(SyntaxKind.PublicKeyword)).Any()).ToList();

                foreach(var method in methods)
                {
                    var syntax = ParseStatement("Assert.Fail(\"autogenerated\");");

                    classDeclaration = classDeclaration
                        .AddMembers(MethodDeclaration(ParseTypeName("void"), Identifier(method.Identifier.Text + "Test"))
                            .AddModifiers(Token(SyntaxKind.PublicKeyword))
                            .WithBody(SyntaxFactory.Block(syntax))
                            .AddAttributeLists(AttributeList(SingletonSeparatedList<AttributeSyntax>(Attribute(IdentifierName("Test"))))));
                        

                }

                namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclaration);
                root = root.AddMembers(namespaceDeclaration);

                testFileStrings.Add(classDeclaration.Identifier.Text, root.NormalizeWhitespace().ToFullString());
            }

            return testFileStrings;
        }
        
    }
}
