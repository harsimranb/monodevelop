//
// FindAllReferencesHandler.cs
//
// Author:
//       Harsimran Bath <harsimranb@outlook.com>
//
// Copyright (c) 2014 Harsimran Bath
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using System.Threading;
using MonoDevelop.Core;
using MonoDevelop.Ide.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Ide.FindInFiles;
using System.Collections;
using MonoDevelop.Projects;
using System.Collections.Generic;
using System.IO;
using Mono.TextEditor;

namespace MonoDevelop.JavaScript
{
//	public class FindAllReferencesHandler : CommandHandler
//	{
//		public static void FindRefs (object obj)
//		{
//			var monitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor (true, true);
//			var solution = IdeApp.ProjectOperations.CurrentSelectedSolution;
//			ThreadPool.QueueUserWorkItem (delegate {
//				try {
//					foreach (var project in solution.GetAllProjects ()) {
//						IProjectContent context = TypeSystemService.GetProjectContext (project);
//						var jsProjectContent = context as JSUpdateableProjectContent;
//						if (jsProjectContent != null) {
//							//monitor.ReportResults (FindReferencesByProject (jsProjectContent, obj.ToString ()));
//						}
//					}
//				} catch (Exception ex) {
//					if (monitor != null)
//						monitor.ReportError ("Error finding references", ex);
//					else
//						LoggingService.LogError ("Error finding references", ex);
//				} finally {
//					if (monitor != null)
//						monitor.Dispose ();
//				}
//			});
//		}
//
//		protected override void Run (object data)
//		{
//			var doc = IdeApp.Workbench.ActiveDocument;
//			if (doc == null || doc.FileName == FilePath.Null)
//				return;
//			ResolveResult resolveResoult;
//			object item = CurrentRefactoryOperationsHandler.GetItem (doc, out resolveResoult);
//			if (item != null)
//				FindRefs (item);
//		}
//
//		public static IEnumerable<SearchResult> FindReferencesByProject (Project project, string query)
//		{
//			var results = new List<SearchResult> ();
//
//			foreach (var file in project.Files) {
//				if (Path.GetExtension (file.Name) != "js")
//					continue;
//
//				string text;
//
//				try {
//					if  (!File.Exists (file))
//						continue;
//					text = Mono.TextEditor.Utils.TextFileUtility.ReadAllText (file.Name);
//				} catch (Exception e) {
//					LoggingService.LogError ("Exception while file reading.", e);
//					continue;
//				}
//
//				using (var editor = TextEditorData.CreateImmutable (text)) {
//					editor.Document.FileName = file;
//					var unit = new  JavaScriptParsedDocument (file.Name, text);
//					if (unit == null)
//						continue;
//
//					if (parsedFile == null) {
//						// for fallback purposes - should never happen.
//						parsedFile = unit.ToTypeSystem ();
//						content = content.AddOrUpdateFiles (parsedFile);
//						compilation = content.CreateCompilation ();
//					}
//					foreach (var scope in scopes) {
//						refFinder.FindReferencesInFile (
//							scope,
//							parsedFile,
//							unit,
//							compilation,
//							(astNode, result) => {
//								var newRef = GetReference (project, result, astNode, unit, file, editor);
//								if (newRef == null || refs.Any (r => r.FileName == newRef.FileName && r.Region == newRef.Region))
//									return;
//								refs.Add (newRef);
//							},
//							CancellationToken.None
//						);
//					}
//				}
//			}
//		}
//
//		static void recurseJSAst (string query, SimpleJSAst ast, Project project, ref List<SearchResult> results)
//		{
//			foreach (var node in ast.AstNodes) {
//				var varData = node as JSVariableDeclaration;
//				if(varData != null && varData.Name) {
//					int offset = varData.SourceCodePosition.
//					results.Add (new SearchResult(new FileProvider (varData.Filename, project)));
//					continue;
//				}
//
//				var funcData = node as JSFunctionStatement;
//				if(funcData != null && funcData.Name == query) {
//					results.Add (new SearchResult(new FileProvider (funData.FileName, project), funData,));
//					continue;
//				}
//			}
//		}
//	}
}

