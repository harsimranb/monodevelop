﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoDevelop.Ide.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.JavaScript.TextEditor;
using ICSharpCode.NRefactory;
using MonoDevelop.JavaScript.Factories;
using MonoDevelop.JavaScript.Extentions;

namespace MonoDevelop.JavaScript.Parser
{
    public class JavaScriptParsedDocument : ParsedDocument
    {
        #region Variables

        string fileName;
        private System.IO.TextReader content;
        private IList<Error> errors;
        private IList<FoldingRegion> foldings;

        #endregion

        #region Properties

        public override string FileName
        {
            get
            {
                return fileName;
            }
        }

        public IEnumerable<Jurassic.Compiler.JSAstNode> AstNodes { get; set; }

        public override IList<Error> Errors
        {
            get
            {
                return errors;
            }
        }

        public override IEnumerable<FoldingRegion> Foldings
        {
            get
            {
                return foldings;
            }
        }

        #endregion

        #region Constructor

        public JavaScriptParsedDocument(string fileName, System.IO.TextReader content)
            : base(fileName)
        {
            this.fileName = fileName;
            this.content = content;

            errors = new List<Error>();
            foldings = new List<FoldingRegion>();

            parse();
        }

        #endregion

        #region Private Methods

        void parse()
        {
            string fileContent = content.ReadToEnd();

            var scriptEngine = new Jurassic.ScriptEngine();
            var scriptSource = new Jurassic.StringScriptSource(fileContent);
            var lexar = new Jurassic.Compiler.Lexer(scriptEngine, scriptSource);
            var scope = scriptEngine.CreateGlobalScope();
            var compilerOptions = new Jurassic.Compiler.CompilerOptions();
            var parser = new Jurassic.Compiler.Parser(scriptEngine, lexar, scope, compilerOptions, Jurassic.Compiler.CodeContext.Global);
            
            Jurassic.Compiler.Statement parserResult = null;
            try
            {
                parserResult = parser.Parse();
            }
            catch (Jurassic.JavaScriptException jsException)
            {
                errors.Add(ErrorFactory.CreateError(jsException.Message, jsException.LineNumber));
            }
            catch (Exception ex)
            {
                errors.Add(ErrorFactory.CreateError(ex.Message));
            }

            if (parserResult != null)
            {
                errors = parser.Errors;
                AstNodes = parserResult.ChildNodes;
                setFoldings(AstNodes);
            }
        }

        private void setFoldings(IEnumerable<Jurassic.Compiler.JSAstNode> astNodes)
        {
            if (astNodes == null)
                return;

            foreach (var node in astNodes)
            {
                var expressionStatement = node as Jurassic.Compiler.ExpressionStatement;
                if (expressionStatement != null)
                {
                    if (expressionStatement.SourceSpan != null)
                    {
                        if (expressionStatement.SourceSpan.EndLine > expressionStatement.SourceSpan.StartLine + 2)
                        {
                            var region = DomRegionFactory.CreateDomRegion(fileName, expressionStatement.SourceSpan);
                            foldings.Add(new FoldingRegion(region));
                        }
                    }

                    setFoldings(expressionStatement.ChildNodes);

                    continue;
                }

                var function = node as Jurassic.Compiler.FunctionStatement;
                if (function != null)
                {
                    if (function.SourceSpan != null)
                    {
                        var region = DomRegionFactory.CreateDomRegion(fileName, function.SourceSpan);
                        foldings.Add(new FoldingRegion(function.BuildFunctionSignature(), region));
                    }

                    setFoldings(function.BodyRoot.ChildNodes);

                    continue;
                }

                var blockStatement = node as Jurassic.Compiler.BlockStatement;
                if (blockStatement != null)
                {
                    if (blockStatement.SourceSpan != null)
                    {
                        var region = DomRegionFactory.CreateDomRegion(fileName, blockStatement.SourceSpan);
                        foldings.Add(new FoldingRegion(region));
                    }

                    setFoldings(blockStatement.Statements);

                    continue;
                }

                var ifStatement = node as Jurassic.Compiler.IfStatement;
                if (ifStatement != null)
                {
                    if (ifStatement.SourceSpan != null)
                    {
                        var region = DomRegionFactory.CreateDomRegion(fileName, ifStatement.SourceSpan);
                        foldings.Add(new FoldingRegion(region));
                    }
                    if (ifStatement.ElseClause != null)
                    {
                        if (ifStatement.ElseClause.SourceSpan != null)
                        {
                            var region = DomRegionFactory.CreateDomRegion(fileName, ifStatement.ElseClause.SourceSpan);
                            foldings.Add(new FoldingRegion(region));
                        }

                        setFoldings(ifStatement.ElseClause.ChildNodes);
                    }

                    setFoldings(ifStatement.ChildNodes);

                    continue;
                }

                var functionExpression = node as Jurassic.Compiler.FunctionExpression;
                if (functionExpression != null)
                {
                    if (functionExpression.SourceSpan != null)
                    {
                        if (functionExpression.SourceSpan.EndLine > functionExpression.SourceSpan.StartLine + 2)
                        {
                            var region = DomRegionFactory.CreateDomRegion(fileName, functionExpression.SourceSpan);
                            foldings.Add(new FoldingRegion(functionExpression.BuildFunctionSignature(), region));
                        }
                    }

                    setFoldings(functionExpression.BodyRoot.ChildNodes);

                    continue;
                }

                setFoldings(node.ChildNodes);
            }
        }

        #endregion
    }
}
