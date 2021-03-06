﻿//
// JavaScriptEditorExtension.cs
//
// Author:
//       Harsimran Bath <harsimranbath@gmail.com>
//
// Copyright (c) 2014 Harsimran
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
using MonoDevelop.DesignerSupport;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoDevelop.Core;
using MonoDevelop.Ide.CodeCompletion;
using Gtk;
using MonoDevelop.Projects;
using MonoDevelop.Ide.TypeSystem;

namespace MonoDevelop.JavaScript
{
	class JavaScriptEditorExtension : CompletionTextEditorExtension, IOutlinedDocument
	{

		#region Variables

		bool disposed;
		bool refreshingOutline = false;
		MonoDevelop.Ide.Gui.Components.PadTreeView outlineTreeView;
		Gtk.TreeStore outlineTreeStore;
		readonly Gdk.Color normal = new Gdk.Color (0x00, 0x00, 0x00);

		#endregion

		#region Properties

		public JavaScriptParsedDocument ParsedDoc { get; private set; }

		#endregion

		#region Initialization and Events

		public JavaScriptEditorExtension ()
		{
		}

		public override void Initialize ()
		{
			base.Initialize ();
			Document.DocumentParsed += HandleDocumentParsed;
			HandleDocumentParsed (this, EventArgs.Empty);
		}

		void HandleDocumentParsed (object sender, EventArgs e)
		{
			ParsedDoc = (JavaScriptParsedDocument)Document.ParsedDocument;
			if (ParsedDoc != null)
				refreshOutline ();
		}

		public override void Dispose ()
		{
			if (disposed)
				return;
			disposed = true;
			base.Dispose ();
		}

		#endregion

		#region Document Outline Implementation

		public Gtk.Widget GetOutlineWidget ()
		{
			if (outlineTreeView != null)
				return outlineTreeView;

			outlineTreeStore = new Gtk.TreeStore (typeof(object));
			outlineTreeView = new MonoDevelop.Ide.Gui.Components.PadTreeView (outlineTreeStore);
			outlineTreeView.Realized += delegate {
				refillOutlineStore ();
			};

			outlineTreeView.TextRenderer.Xpad = 0;
			outlineTreeView.TextRenderer.Ypad = 0;

			var pixRenderer = new CellRendererPixbuf ();
			pixRenderer.Xpad = 0;
			pixRenderer.Ypad = 0;

			var treeCol = new TreeViewColumn ();
			treeCol.PackStart (pixRenderer, false);

			treeCol.SetCellDataFunc (pixRenderer, new TreeCellDataFunc (outlineTreeIconFunc));
			treeCol.PackStart (outlineTreeView.TextRenderer, true);

			treeCol.SetCellDataFunc (outlineTreeView.TextRenderer, new TreeCellDataFunc (outlineTreeTextFunc));
			outlineTreeView.AppendColumn (treeCol);

			outlineTreeView.HeadersVisible = false;

			outlineTreeView.Selection.Changed += delegate {
				Gtk.TreeIter iter;
				if (!outlineTreeView.Selection.GetSelected (out iter))
					return;
				selectSegment (outlineTreeStore.GetValue (iter, 0));
			};

			refillOutlineStore ();
			var sw = new MonoDevelop.Components.CompactScrolledWindow ();

			sw.Add (outlineTreeView);
			sw.ShowAll ();
			return sw;
		}

		public IEnumerable<Gtk.Widget> GetToolbarWidgets ()
		{
			//var widgets = new List<Gtk.Widget>();

			//var searchTextBox = new Gtk.TextView();
			//searchTextBox.Buffer.Text = "Search...";
			//searchTextBox.WidthRequest = 300;
			//widgets.Add(searchTextBox);

			//return widgets;

			return null;
		}

		public void ReleaseOutlineWidget ()
		{
			if (outlineTreeView == null)
				return;

			Gtk.ScrolledWindow w = (Gtk.ScrolledWindow)outlineTreeView.Parent;
			w.Destroy ();
			outlineTreeView.Destroy ();
			outlineTreeStore.Dispose ();
			outlineTreeStore = null;
			outlineTreeView = null;
		}

		#endregion

		#region Completion TextEditor Implementation

		public override string CompletionLanguage { get { return "JavaScript"; } }

		public override bool CanRunCompletionCommand ()
		{
			return true;
		}

		public override bool CanRunParameterCompletionCommand ()
		{
			return false;
		}

		public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar, ref int triggerWordLength)
		{
			if (char.IsLetterOrDigit (completionChar) || completionChar == '_') {
				if (completionContext.TriggerOffset > 1 &&
				    char.IsLetterOrDigit (document.Editor.GetCharAt (completionContext.TriggerOffset - 2)))
					return null;
				triggerWordLength = 1;
			} else
				return null;

			if (Ide.IdeApp.ProjectOperations.CurrentSelectedProject == null)
				return null;

			var wrapper = TypeSystemService.GetProjectContentWrapper (IdeApp.ProjectOperations.CurrentSelectedProject).GetExtensionObject<JSUpdateableProjectContent> ();
			List<JavaScriptDocumentCache> parsedDocuments = wrapper.DocumentsCache;
			//List<SimpleJSAst> parsedDocuments = JSTypeSystemService.GetAllJSAstsForProject (Ide.IdeApp.ProjectOperations.CurrentSelectedProject.ItemId);

			// TODO: Find the current scope.
			var dataList = new CompletionDataList ();
			foreach (JavaScriptDocumentCache parsedDocumentAstNodes in parsedDocuments) {
				if (parsedDocumentAstNodes != null && parsedDocumentAstNodes.SimpleAst != null)
					dataList.AddRange (buildCodeCompletionList (parsedDocumentAstNodes.SimpleAst.AstNodes));
			}
			return dataList;
		}

		public override ParameterDataProvider HandleParameterCompletion (CodeCompletionContext completionContext, char completionChar)
		{
			return null;
		}

		#endregion

		#region Private Methods

		void outlineTreeIconFunc (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			var pixRenderer = (CellRendererPixbuf)cell;
			object o = model.GetValue (iter, 0);

			if (o is JSFunctionStatement) {
				pixRenderer.Pixbuf = ImageService.GetPixbuf (MonoDevelop.Ide.Gui.Stock.Method, IconSize.Menu);
			} else if (o is JSVariableDeclaration) {
				pixRenderer.Pixbuf = ImageService.GetPixbuf (MonoDevelop.Ide.Gui.Stock.Field, IconSize.Menu);
			} else {
				throw new ArgumentException (string.Format ("Type {0} is not supported in JavaScript Outline.", o.GetType ().Name));
			}
		}

		void outlineTreeTextFunc (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			var txtRenderer = (CellRendererText)cell;
			object o = model.GetValue (iter, 0);

			var functionStatement = o as JSFunctionStatement;
			if (functionStatement != null) {
				txtRenderer.Text = functionStatement.FunctionSignature;
				return;
			} 

			var varDeclaration = o as JSVariableDeclaration;
			if (varDeclaration != null) {
				txtRenderer.Text = varDeclaration.Name;
				return;
			}

			throw new ArgumentException (string.Format ("Type {0} is not supported in JavaScript Outline.", o.GetType ().Name));
		}

		bool refillOutlineStoreIdleHandler ()
		{
			refreshingOutline = false;
			refillOutlineStore ();
			return false;
		}

		void refreshOutline ()
		{
			if (refreshingOutline || outlineTreeView == null)
				return;
			refreshingOutline = true;
			GLib.Timeout.Add (3000, refillOutlineStoreIdleHandler);
		}

		void refillOutlineStore ()
		{
			DispatchService.AssertGuiThread ();
			Gdk.Threads.Enter ();
			refreshingOutline = false;
			if (outlineTreeStore == null || !outlineTreeView.IsRealized)
				return;
			outlineTreeStore.Clear ();

			if (ParsedDoc != null) {
				DateTime start = DateTime.Now;
				refillOutlineStore (ParsedDoc, outlineTreeStore);
				outlineTreeView.ExpandAll ();
				outlineTreeView.ExpandAll ();
				LoggingService.LogDebug ("Built outline in {0}ms", (DateTime.Now - start).Milliseconds);
			}

			Gdk.Threads.Leave ();
		}

		void refillOutlineStore (JavaScriptParsedDocument doc, Gtk.TreeStore store)
		{
			if (doc == null)
				return;

			buildTreeChildren (store, Gtk.TreeIter.Zero, doc.SimpleAst.AstNodes);
		}

		void buildTreeChildren (Gtk.TreeStore store, Gtk.TreeIter parent, IEnumerable<JSStatement> nodes)
		{
			if (nodes == null)
				return;

			foreach (JSStatement node in nodes) {
				Gtk.TreeIter childIter = default (Gtk.TreeIter);
				var variableDeclaration = node as JSVariableDeclaration;
				if (variableDeclaration != null) {
					if (!parent.Equals (Gtk.TreeIter.Zero))
						store.AppendValues (parent, variableDeclaration);
					else
						store.AppendValues (variableDeclaration);
					continue;
				}

				var functionStatement = node as JSFunctionStatement;
				if (functionStatement != null) {
					if (!parent.Equals (Gtk.TreeIter.Zero))
						childIter = store.AppendValues (parent, functionStatement);
					else
						childIter = store.AppendValues (functionStatement);
					buildTreeChildren (store, childIter, functionStatement.ChildNodes);
					continue;
				}

				buildTreeChildren (store, parent, node.ChildNodes);
			}
		}

		void selectSegment (object node)
		{
			int line = 0, column = 0;

			if (node is JSStatement) {
				line = (node as JSStatement).SourceCodePosition.StartLine;
				column = (node as JSStatement).SourceCodePosition.StartColumn;
			} else {
				return;
			}

			int s = Editor.Document.LocationToOffset (line, column);
			if (s > -1) {
				Editor.Caret.Offset = s;
				Editor.CenterTo (s);
			}
		}

		CompletionDataList buildCodeCompletionList (IEnumerable<JSStatement> nodes)
		{
			// TODO: Store all functions, variables in project, similar C Binding
			if (nodes == null)
				return new CompletionDataList ();

			var completionList = new CompletionDataList ();

			foreach (JSStatement node in nodes) {
				var variableDeclaration = node as JSVariableDeclaration;
				if (variableDeclaration != null) {
					completionList.Add (new CompletionData (variableDeclaration));
					completionList.AddRange (buildCodeCompletionList (variableDeclaration.ChildNodes));
					continue;
				}

				var functionStatement = node as JSFunctionStatement;
				if (functionStatement != null) {
					completionList.Add (new CompletionData (functionStatement));
					completionList.AddRange (buildCodeCompletionList (functionStatement.ChildNodes));
					continue;
				}

				completionList.AddRange (buildCodeCompletionList (node.ChildNodes));
			}

			return completionList;
		}

		#endregion

	}
}
