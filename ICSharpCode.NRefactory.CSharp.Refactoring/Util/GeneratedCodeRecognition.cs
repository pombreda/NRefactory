﻿//
// GeneratedCodeRecognition.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2015 Xamarin Inc. (http://xamarin.com)
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
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Threading;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace ICSharpCode.NRefactory6.CSharp
{
	static class GeneratedCodeRecognition
	{
		public static bool IsFromGeneratedCode (this SemanticModel semanticModel, CancellationToken cancellationToken)
		{
			return IsFileNameForGeneratedCode (semanticModel.SyntaxTree.FilePath) || ContainsAutogeneratedComment (semanticModel.SyntaxTree, cancellationToken);
		}

		public static bool IsFromGeneratedCode (this SyntaxNodeAnalysisContext context)
		{
			return IsFromGeneratedCode (context.SemanticModel, context.CancellationToken);
		}

		public static bool IsFileNameForGeneratedCode (string fileName)
		{
			if (fileName.StartsWith ("TemporaryGeneratedFile_", StringComparison.OrdinalIgnoreCase)) {
				return true;
			}

			string extension = Path.GetExtension (fileName);
			if (extension != string.Empty) {
				fileName = Path.GetFileNameWithoutExtension (fileName);

				if (fileName.EndsWith ("AssemblyInfo", StringComparison.OrdinalIgnoreCase) ||
					fileName.EndsWith (".designer", StringComparison.OrdinalIgnoreCase) ||
					fileName.EndsWith (".generated", StringComparison.OrdinalIgnoreCase) ||
					fileName.EndsWith (".g", StringComparison.OrdinalIgnoreCase) ||
					fileName.EndsWith (".g.i", StringComparison.OrdinalIgnoreCase) ||
					fileName.EndsWith (".AssemblyAttributes", StringComparison.OrdinalIgnoreCase)) {
					return true;
				}
			}

			return false;
		}

		static bool ContainsAutogeneratedComment (SyntaxTree tree, CancellationToken cancellationToken = default(CancellationToken))
		{
			var root = tree.GetRoot (cancellationToken);
			if (root == null)
				return false;
			var firstToken = root.GetFirstToken ();
			if (!firstToken.HasLeadingTrivia)
				return false;
			foreach (var trivia in firstToken.LeadingTrivia.Where (t => t.IsKind (SyntaxKind.SingleLineCommentTrivia)).Take (2)) {
				var str = trivia.ToString ();
				if (str == "// This file has been generated by the GUI designer. Do not modify.")
					return true;
				if (str == "// <auto-generated>" || str == "// <autogenerated>")
					return true;
			}
			return false;
		}
	}
}