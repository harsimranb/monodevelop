﻿using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{
	/// <summary>
	/// Represents the unit of compilation.
	/// </summary>
	public abstract class MethodGenerator
	{
		/// <summary>
		/// Creates a new MethodGenerator instance.
		/// </summary>
		/// <param name="engine"> The script engine. </param>
		/// <param name="scope"> The initial scope. </param>
		/// <param name="source"> The source of javascript code. </param>
		/// <param name="options"> Options that influence the compiler. </param>
		protected MethodGenerator (ScriptEngine engine, ScriptSource source, CompilerOptions options)
		{
			if (engine == null)
				throw new ArgumentNullException ("engine");
			if (source == null)
				throw new ArgumentNullException ("source");
			if (options == null)
				throw new ArgumentNullException ("options");
			Engine = engine;
			Source = source;
			Options = options;
			StrictMode = Options.ForceStrictMode;
		}

		/// <summary>
		/// Gets a reference to the script engine.
		/// </summary>
		public ScriptEngine Engine {
			get;
			private set;
		}

		/// <summary>
		/// Gets a reference to any compiler options.
		/// </summary>
		public CompilerOptions Options {
			get;
			private set;
		}

		/// <summary>
		/// Gets the source of javascript code.
		/// </summary>
		public ScriptSource Source {
			get;
			private set;
		}

		/// <summary>
		/// Gets a value that indicates whether strict mode is enabled.
		/// </summary>
		public bool StrictMode {
			get;
			protected set;
		}

		/// <summary>
		/// Gets the root node of the abstract syntax tree.  This will be <c>null</c> until Parse()
		/// is called.
		/// </summary>
		public Statement AbstractSyntaxTree {
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets optimization information.
		/// </summary>
		public MethodOptimizationHints MethodOptimizationHints {
			get;
			set;
		}

		/// <summary>
		/// Gets a name for the generated method.
		/// </summary>
		/// <returns> A name for the generated method. </returns>
		protected abstract string GetMethodName ();

		/// <summary>
		/// Gets a name for the function, as it appears in the stack trace.
		/// </summary>
		/// <returns> A name for the function, as it appears in the stack trace, or <c>null</c> if
		/// this generator is generating code in the global scope. </returns>
		protected virtual string GetStackName ()
		{
			return null;
		}

		/// <summary>
		/// Gets an array of types - one for each parameter accepted by the method generated by
		/// this context.
		/// </summary>
		/// <returns> An array of parameter types. </returns>
		protected virtual Type[] GetParameterTypes ()
		{
			return new [] {
				typeof(ScriptEngine),   // The script engine.
				typeof(Scope),          // The scope.
				typeof(object),         // The "this" object.
			};
		}

		/// <summary>
		/// Optimizes the abstract syntax tree.
		/// </summary>
		public void Optimize ()
		{
		}
	}

}