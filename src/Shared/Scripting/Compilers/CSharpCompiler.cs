// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Aura.Shared.Util;
using CSScriptLibrary;
using System.Text.RegularExpressions;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Aura.Shared.Scripting.Compilers
{
	public class CSharpCompilerException : Exception
	{
		public CSharpCompilerException(System.CodeDom.Compiler.CompilerErrorCollection errors)
		{
			this.Data["Errors"] = errors;
		}
	}

	/// <summary>
	/// C# compiler, utilizing CSScript
	/// </summary>
	public class CSharpCompiler : Compiler
	{
		private Dictionary<Type, Assembly> _typeAsms = new Dictionary<Type, Assembly>();

		public override Assembly Compile(string path, string outPath, bool cache)
		{
			Assembly asm = null;
			try
			{
				// Get from cache?
				if (this.ExistsAndUpToDate(path, outPath) && cache)
				{
					asm = Assembly.LoadFrom(outPath);
				}
				else
				{
					// Precompile script to a temp file
					var precompiled = this.PreCompile(File.ReadAllText(path));
					var tmpFileName = Path.GetTempFileName();
					File.WriteAllText(tmpFileName, precompiled);

					var asmPath = Assembly.GetEntryAssembly().Location;
					var asmDir = Path.GetDirectoryName(asmPath);

					var provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp"); ;
					var parameters = new System.CodeDom.Compiler.CompilerParameters();
					parameters.ReferencedAssemblies.Add("System.dll");
					parameters.ReferencedAssemblies.Add("System.Core.dll");
					parameters.ReferencedAssemblies.Add("System.Data.dll");
					parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
					parameters.ReferencedAssemblies.Add("System.Xml.dll");
					parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
					parameters.ReferencedAssemblies.Add(Path.Combine(asmDir, "Data.dll"));
					parameters.ReferencedAssemblies.Add(Path.Combine(asmDir, "Mabi.dll"));
					parameters.ReferencedAssemblies.Add(Path.Combine(asmDir, "Shared.dll"));
					parameters.ReferencedAssemblies.Add(asmPath);
					parameters.GenerateExecutable = false;
					parameters.GenerateInMemory = false;
					parameters.OutputAssembly = tmpFileName + ".compiled";
					parameters.TreatWarningsAsErrors = false;
					parameters.WarningLevel = 0;

#if DEBUG
					parameters.IncludeDebugInformation = true;
#else
					parameters.IncludeDebugInformation = false;
#endif

					// Reference required asms
					foreach (var type in _typeAsms)
					{
						if (Regex.IsMatch(precompiled, @":\s*" + type.Key.Name))
							parameters.ReferencedAssemblies.Add(type.Value.Location);

						//foreach (var asmName in type.Value.GetReferencedAssemblies())
						//{
						//	var location = Assembly.ReflectionOnlyLoad(asmName.FullName).Location;
						//	if (!parameters.ReferencedAssemblies.Contains(location))
						//		parameters.ReferencedAssemblies.Add(location);
						//}
					}
					// Compile

					var results = provider.CompileAssemblyFromFile(parameters, tmpFileName);
					if (results.Errors.Count != 0)
						throw new CSharpCompilerException(results.Errors);

					asm = results.CompiledAssembly;

					this.SaveAssembly(asm, outPath);
				}

				// Remember where the types came from
				this.CacheTypeAsms(asm);
			}
			catch (CSharpCompilerException ex)
			{
				var errors = ex.Data["Errors"] as System.CodeDom.Compiler.CompilerErrorCollection;
				var newExs = new CompilerErrorsException();

				foreach (System.CodeDom.Compiler.CompilerError err in errors)
				{
					var newEx = new CompilerError(path, err.Line, err.Column, err.ErrorText, err.IsWarning);
					newExs.Errors.Add(newEx);
				}

				throw newExs;
			}
			catch (UnauthorizedAccessException)
			{
				// Thrown if file can't be copied. Happens if script was
				// initially loaded from cache.
				// TODO: Also thrown if CS-Script can't create the file,
				//   ie under Linux, if /tmp/CSSCRIPT isn't writeable.
				//   Handle that somehow?
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}

			return asm;
		}

		/// <summary>
		/// Logs which assembly contains which type.
		/// </summary>
		/// <param name="asm"></param>
		private void CacheTypeAsms(Assembly asm)
		{
			foreach (var type in asm.GetTypes())
			{
				_typeAsms[type] = asm;
			}
		}
	}
}
