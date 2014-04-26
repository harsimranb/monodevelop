using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoDevelop.Ide.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.JavaScript.TextEditor;

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
                if (node is Jurassic.Compiler.FunctionStatement)
                {
                    var function = node as Jurassic.Compiler.FunctionStatement;
                    if (function.SourceSpan != null)
                    {
                        var region = new DomRegion(fileName,
                            function.SourceSpan.StartLine,
                            function.SourceSpan.StartColumn,
                            function.SourceSpan.EndLine,
                            function.SourceSpan.EndColumn);

                        foldings.Add(new FoldingRegion(region));
                    }

                    setFoldings(function.BodyRoot.ChildNodes);
                }
                else if (node is Jurassic.Compiler.BlockStatement)
                {
                    var blockStatement = node as Jurassic.Compiler.BlockStatement;
                    if (blockStatement.SourceSpan != null)
                    {
                        var region = new DomRegion(fileName,
                            blockStatement.SourceSpan.StartLine,
                            blockStatement.SourceSpan.StartColumn,
                            blockStatement.SourceSpan.EndLine,
                            blockStatement.SourceSpan.EndColumn);

                        foldings.Add(new FoldingRegion(region));
                    }

                    setFoldings(blockStatement.Statements);
                }
                else if (node is Jurassic.Compiler.IfStatement)
                {
                    var ifStatement = node as Jurassic.Compiler.IfStatement;
                    if (ifStatement.SourceSpan != null)
                    {
                        var region = new DomRegion(fileName,
                            ifStatement.SourceSpan.StartLine,
                            ifStatement.SourceSpan.StartColumn,
                            ifStatement.SourceSpan.EndLine,
                            ifStatement.SourceSpan.EndColumn);

                        foldings.Add(new FoldingRegion(region));
                    }

                    setFoldings(ifStatement.ChildNodes);
                }
                else if (node is Jurassic.Compiler.FunctionExpression)
                {
                    var functionExpression = node as Jurassic.Compiler.FunctionExpression;
                    if (functionExpression.SourceSpan != null)
                    {
                        var region = new DomRegion(fileName,
                            functionExpression.SourceSpan.StartLine,
                            functionExpression.SourceSpan.StartColumn,
                            functionExpression.SourceSpan.EndLine,
                            functionExpression.SourceSpan.EndColumn);

                        foldings.Add(new FoldingRegion(region));
                    }

                    setFoldings(functionExpression.BodyRoot.ChildNodes);
                }
                else
                { 
                    setFoldings(node.ChildNodes);
                }
            }
        }

        #endregion
    }
}
