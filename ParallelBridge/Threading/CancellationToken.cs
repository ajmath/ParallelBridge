//
// CancellationToken.cs
//
// Author:
//       Jérémie "Garuma" Laval <jeremie.laval@gmail.com>
//
// Copyright (c) 2009 Jérémie "Garuma" Laval
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
using System.Threading;

namespace Mono.Threading
{
	[System.Diagnostics.DebuggerDisplay ("IsCancellationRequested = {IsCancellationRequested}")]
	public struct CancellationToken
	{
		bool canBeCanceled;
		bool initialized;
		CancellationTokenSource source;

		public CancellationToken (bool canceled)
			: this ()
		{
			initialized = true;
			canBeCanceled = canceled;
			// This is correctly set later if token originates from a Source
			source = canceled ? CancellationTokenSource.CanceledSource : CancellationTokenSource.NoneSource;
		}

		public static CancellationToken None {
			get {
				return CancellationTokenSource.NoneSource.Token;
			}
		}

		public CancellationTokenRegistration Register (Action callback)
		{
			return Register (callback, false);
		}

		public CancellationTokenRegistration Register (Action callback, bool useSynchronizationContext)
		{
			if (callback == null)
				throw new ArgumentNullException ("callback");

			return Source.Register (callback, useSynchronizationContext);
		}

		public CancellationTokenRegistration Register (Action<object> callback, object state)
		{
			return Register (callback, state, false);
		}

		public CancellationTokenRegistration Register (Action<object> callback, object state, bool useSynchronizationContext)
		{
			if (callback == null)
				throw new ArgumentNullException ("callback");

			return Register (() => callback (state), useSynchronizationContext);
		}

		public void ThrowIfCancellationRequested ()
		{
			if (initialized && Source.IsCancellationRequested)
				throw new OperationCanceledException (this);
		}

		public bool Equals (CancellationToken other)
		{
			return this.Source == other.Source;
		}

		public override bool Equals (object other)
		{
			return (other is CancellationToken) ? Equals ((CancellationToken)other) : false;
		}

		public override int GetHashCode ()
		{
			return Source.GetHashCode ();
		}

		public static bool operator == (CancellationToken left, CancellationToken right)
		{
			return left.Equals (right);
		}

		public static bool operator != (CancellationToken left, CancellationToken right)
		{
			return !left.Equals (right);
		}

		public bool CanBeCanceled {
			get {
				return canBeCanceled;
			}
		}

		public bool IsCancellationRequested {
			get {
				return initialized && Source.IsCancellationRequested;
			}
		}

		public WaitHandle WaitHandle {
			get {
				return Source.WaitHandle;
			}
		}

		internal CancellationTokenSource Source {
			get {
				if (!initialized)
					CorrectlyInitialize ();
				return source;
			}
			set {
				source = value;
			}
		}

		void CorrectlyInitialize ()
		{
			Source = CancellationTokenSource.NoneSource;
			initialized = true;
		}
	}
}
