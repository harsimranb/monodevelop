//
// MemberReference.cs
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
using Mono.TextEditor.Highlighting;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.TextEditor;
using MonoDevelop.Ide.FindInFiles;

namespace Refactoring
{
	[Flags]
	public enum ReferenceUsageType {
		Unknown = 0,
		Read    = 1,
		Write   = 2,
		ReadWrite = Read | Write
	}

	public class MemberReference : Mono.TextEditor.SearchResult
	{
		public override FileProvider FileProvider {
			get {
				return new FileProvider (FileName);
			}
		}

		public override  string FileName {
			get {
				return Region.FileName;
			}
		}

		public ReferenceUsageType ReferenceUsageType { get; set; }
		public object EntityOrVariable { get; private set;}
		public DomRegion Region { get; private set;}

		public MemberReference (string name, DomRegion region, int offset, int length) : base (offset, length)
		{
			if (string.IsNullOrWhiteSpace (name))
				throw new System.ArgumentNullException ("name");
			EntityOrVariable = name;
			Region = region;
		}

		public string GetName ()
		{
			if (EntityOrVariable is IEntity) {
				return ((IEntity)EntityOrVariable).Name;
			} 
			if (EntityOrVariable is ITypeParameter) {
				return ((ITypeParameter)EntityOrVariable).Name;
			} 
			if (EntityOrVariable is INamespace) {
				return ((INamespace)EntityOrVariable).Name;
			} 
			return ((IVariable)EntityOrVariable).Name;
		}

		public override AmbientColor GetBackgroundMarkerColor (ColorScheme style)
		{
			return (ReferenceUsageType & ReferenceUsageType.Write) != 0 ?
				style.ChangingUsagesRectangle :
				style.UsagesRectangle;
		}
	}
}

