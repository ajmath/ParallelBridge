//
// System.OperationCanceledException.cs
//
// Authors:
//   Zoltan Varga  <vargaz@freemail.hu>
//   Jérémie Laval <jeremie.laval@gmail.com>
//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Threading;
using Mono.Threading;

namespace Mono
{
	[Serializable]
	[ComVisible (true)]
	public class OperationCanceledException : SystemException
	{
		const int Result = unchecked ((int)0x8013153b);
		CancellationToken? token;

		// Constructors
		public OperationCanceledException ()
			: base ("The operation was canceled.")
		{
			HResult = Result;
		}

		public OperationCanceledException (string message)
			: base (message)
		{
			HResult = Result;
		}

		public OperationCanceledException (string message, Exception innerException)
			: base (message, innerException)
		{
			HResult = Result;
		}

		protected OperationCanceledException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}
		
		public OperationCanceledException (CancellationToken token)
			: this ()
		{
			this.token = token;
		}
		
		public OperationCanceledException (string message, CancellationToken token)
			: this (message)
		{
			this.token = token;
		}
		
		public OperationCanceledException (string message, Exception innerException, CancellationToken token)
			: base (message, innerException)
		{
			this.token = token;
		}
		
		public CancellationToken CancellationToken {
			get {
				if (token == null)
					return CancellationToken.None;
				return token.Value;
			}
			private set {
				token = value;
			}
		}
	}
}
