﻿//
// GotoDeclarationHandler.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.FindInFiles;
using MonoDevelop.Ide.TypeSystem;
using MonoDevelop.Projects;
using MonoDevelop.JavaScript;

namespace MonoDevelop.JavaScript
{
	public class GotoDeclarationHandler : CommandHandler
	{
		protected override void Run (object dataItem)
		{
			if (dataItem != null)
				reportDeclarations (dataItem.ToString ());
		}

		static void reportDeclarations (string term)
		{
			var currentProject = IdeApp.ProjectOperations.CurrentSelectedProject;
			using (var monitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor (true, true)) {
				IProjectContent context = TypeSystemService.GetProjectContext (currentProject);
				var jsProjectContent = context as JSUpdateableProjectContent;
				if (jsProjectContent != null) {
					monitor.ReportResults (FindReferencesByProject (jsProjectContent, term));
				}
			}
		}

		public static IEnumerable<SearchResult> FindReferencesByProject (JSUpdateableProjectContent project, string query)
		{
			var results = new List<SearchResult> ();
			foreach (var item in project.DocumentsCache) {
				recurseJSAst (query, item.SimpleAst, ref results);
			}
			return results;
		}

		static void recurseJSAst (string query, SimpleJSAst ast, ref List<SearchResult> results)
		{
			foreach (var node in ast.AstNodes) {
				var varData = node as JSVariableDeclaration;
				if (varData != null && !string.IsNullOrWhiteSpace (varData.Name) && varData.Name == query) {
					results.Add (createSearchResult(varData.Filename, varData.SourceCodePosition.StartLine, varData.SourceCodePosition.StartColumn, varData.Name));
					continue;
				}
		
				var funcData = node as JSFunctionStatement;
				if (funcData != null && funcData.Name == query && funcData.Name == query) {
					results.Add (createSearchResult(funcData.Filename, funcData.SourceCodePosition.StartLine, funcData.SourceCodePosition.StartColumn, funcData.Name));
					continue;
				}
			}
		}
			
		static SearchResult createSearchResult(string filename, int beginLine, int beginColumn, string name)
		{
			var provider = new MonoDevelop.Ide.FindInFiles.FileProvider (filename);
			var doc = new Mono.TextEditor.TextDocument ();
			doc.Text = provider.ReadString ();
			int position = doc.LocationToOffset (beginLine, beginColumn);
			while (position + name.Length < doc.TextLength) {
				if (doc.GetTextAt (position, name.Length) == name)
					break;
				position++;
			}

			return new SearchResult (provider, position, name.Length);
		}
	}
}