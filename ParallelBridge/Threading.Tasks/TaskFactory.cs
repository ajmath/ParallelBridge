// 
// TaskFactory.cs
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

namespace Mono.Threading.Tasks
{
	
	public class TaskFactory
	{
		TaskScheduler scheduler;
		TaskCreationOptions creationOptions;
		TaskContinuationOptions continuationOptions;
		CancellationToken cancellationToken;
		
		#region ctors
		public TaskFactory ()
			: this (CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Current)
		{
		}

		public TaskFactory (CancellationToken cancellationToken)
			: this (cancellationToken, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Current)
		{	
		}

		public TaskFactory (TaskScheduler scheduler)
			: this (CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, scheduler)
		{	
		}
		
		public TaskFactory (TaskCreationOptions creationOptions, TaskContinuationOptions continuationOptions)
			: this (CancellationToken.None, creationOptions, continuationOptions, TaskScheduler.Current)
		{	
		}
		
		public TaskFactory (CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskContinuationOptions continuationOptions,
		                    TaskScheduler scheduler)
		{
			this.cancellationToken = cancellationToken;
			this.scheduler = scheduler;
			this.creationOptions = creationOptions;
			this.continuationOptions = continuationOptions;

			CheckContinuationOptions (continuationOptions);
		}
		#endregion

		internal static void CheckContinuationOptions (TaskContinuationOptions continuationOptions)
		{
			if ((continuationOptions & (TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion)) != 0)
				throw new ArgumentOutOfRangeException ("continuationOptions", "msg");

			const TaskContinuationOptions long_running = TaskContinuationOptions.LongRunning | TaskContinuationOptions.ExecuteSynchronously;
			if ((continuationOptions & long_running) == long_running)
				throw new ArgumentOutOfRangeException ("continuationOptions", "Synchronous continuations cannot be long running");
		}
		
		#region StartNew for Task
		public Task StartNew (Action action)
		{
			return StartNew (action, cancellationToken, creationOptions, scheduler);
		}
		
		public Task StartNew (Action action, CancellationToken cancellationToken)
		{
			return StartNew (action, cancellationToken, creationOptions, scheduler);
		}
		
		public Task StartNew (Action action, TaskCreationOptions creationOptions)
		{
			return StartNew (action, cancellationToken, creationOptions, scheduler);
		}
		
		public Task StartNew (Action<object> action, object state)
		{
			return StartNew (action, state, cancellationToken, creationOptions, scheduler);
		}
		
		public Task StartNew (Action<object> action, object state, CancellationToken cancellationToken)
		{
			return StartNew (action, state, cancellationToken, creationOptions, scheduler);
		}
		
		public Task StartNew (Action<object> action, object state, TaskCreationOptions creationOptions)
		{
			return StartNew (action, state, cancellationToken, creationOptions, scheduler);
		}
		
		public Task StartNew (Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
		{
			Task t = new Task (action, cancellationToken, creationOptions);
			t.Start (scheduler);

			return t;
		}
		
		public Task StartNew (Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions,
		                      TaskScheduler scheduler)
		{
			Task t = new Task (action, state, cancellationToken, creationOptions);
			t.Start (scheduler);
			
			return t;
		}
		#endregion
		
		#region StartNew for Task<TResult>	
		public Task<TResult> StartNew<TResult> (Func<TResult> function)
		{
			return StartNew<TResult> (function, cancellationToken, creationOptions, scheduler);
		}
		
		public Task<TResult> StartNew<TResult> (Func<TResult> function, TaskCreationOptions creationOptions)
		{
			return StartNew<TResult> (function, cancellationToken, creationOptions, scheduler);

		}
		
		public Task<TResult> StartNew<TResult> (Func<TResult> function, CancellationToken cancellationToken)
		{
			return StartNew<TResult> (function, cancellationToken, creationOptions, scheduler);
		}
		
		public Task<TResult> StartNew<TResult> (Func<TResult> function,
		                                        CancellationToken cancellationToken,
		                                        TaskCreationOptions creationOptions,
		                                        TaskScheduler scheduler)
		{
			return StartNew<TResult> ((o) => function (), null, cancellationToken, creationOptions, scheduler);
		}
		
		public Task<TResult> StartNew<TResult> (Func<object, TResult> function, object state)
		{
			return StartNew<TResult> (function, state, cancellationToken, creationOptions, scheduler);
		}
		
		public Task<TResult> StartNew<TResult> (Func<object, TResult> function, object state, CancellationToken cancellationToken)
		{
			return StartNew<TResult> (function, state, cancellationToken, creationOptions, scheduler);
		}
		
		public Task<TResult> StartNew<TResult> (Func<object, TResult> function, object state, TaskCreationOptions creationOptions)
		{
			return StartNew<TResult> (function, state, cancellationToken, creationOptions, scheduler);
		}
		
		public Task<TResult> StartNew<TResult> (Func<object, TResult> function, object state,
		                                        CancellationToken cancellationToken,
		                                        TaskCreationOptions creationOptions,
		                                        TaskScheduler scheduler)
		{
			Task<TResult> t = new Task<TResult> (function, state, cancellationToken, creationOptions);
			t.Start (scheduler);
			
			return t;
		}
		#endregion
		
		#region Continue
		
		public Task ContinueWhenAny (Task[] tasks, Action<Task> continuationAction)
		{
			return ContinueWhenAny (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task ContinueWhenAny (Task[] tasks, Action<Task> continuationAction, CancellationToken cancellationToken)
		{
			return ContinueWhenAny (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task ContinueWhenAny (Task[] tasks, Action<Task> continuationAction, TaskContinuationOptions continuationOptions)
		{
			return ContinueWhenAny (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}

		public Task ContinueWhenAny (Task[] tasks,
		                             Action<Task> continuationAction,
		                             CancellationToken cancellationToken,
		                             TaskContinuationOptions continuationOptions,
		                             TaskScheduler scheduler)
		{
			var ourTasks = (Task[])tasks.Clone ();
			AtomicBoolean trigger = new AtomicBoolean ();
			Task commonContinuation = new Task (null);
			
			foreach (Task t in ourTasks) {
				Task cont = new Task ((o) => continuationAction ((Task)o), t, cancellationToken, creationOptions, t);
				t.ContinueWithCore (cont, continuationOptions, scheduler, trigger.TrySet);
				cont.ContinueWithCore (commonContinuation, TaskContinuationOptions.None, scheduler);
			}
			
			return commonContinuation;
		}
		
		public Task ContinueWhenAny<TAntecedentResult> (Task<TAntecedentResult>[] tasks,
		                                                Action<Task<TAntecedentResult>> continuationAction)
		{
			return ContinueWhenAny (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task ContinueWhenAny<TAntecedentResult> (Task<TAntecedentResult>[] tasks,
		                                                Action<Task<TAntecedentResult>> continuationAction,
		                                                CancellationToken cancellationToken)
		{
			return ContinueWhenAny (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task ContinueWhenAny<TAntecedentResult> (Task<TAntecedentResult>[] tasks,
		                                                Action<Task<TAntecedentResult>> continuationAction,
		                                                TaskContinuationOptions continuationOptions)
		{
			return ContinueWhenAny (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}

		public Task ContinueWhenAny<TAntecedentResult> (Task<TAntecedentResult>[] tasks,
		                                                Action<Task<TAntecedentResult>> continuationAction,
		                                                CancellationToken cancellationToken,
		                                                TaskContinuationOptions continuationOptions,
		                                                TaskScheduler scheduler)
		{
			return ContinueWhenAny ((Task[]) tasks,
			                        (o) => continuationAction ((Task<TAntecedentResult>)o),
			                        cancellationToken, continuationOptions, scheduler);
		}

		public Task<TResult> ContinueWhenAny<TResult> (Task[] tasks, Func<Task, TResult> continuationFunction)
		{
			return ContinueWhenAny (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}

		public Task<TResult> ContinueWhenAny<TResult> (Task[] tasks,
		                                               Func<Task, TResult> continuationFunction,
		                                               CancellationToken cancellationToken)
		{
			return ContinueWhenAny (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}

		public Task<TResult> ContinueWhenAny<TResult> (Task[] tasks,
		                                               Func<Task, TResult> continuationFunction,
		                                               TaskContinuationOptions continuationOptions)
		{
			return ContinueWhenAny (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}

		public Task<TResult> ContinueWhenAny<TResult> (Task[] tasks,
		                                               Func<Task, TResult> continuationFunction,
		                                               CancellationToken cancellationToken,
		                                               TaskContinuationOptions continuationOptions,
		                                               TaskScheduler scheduler)
		{
			var ourTasks = (Task[])tasks.Clone ();
			AtomicBoolean trigger = new AtomicBoolean ();
			TaskCompletionSource<TResult> source = new TaskCompletionSource<TResult> ();

			foreach (Task t in ourTasks) {
				Task cont = new Task ((o) => source.SetResult (continuationFunction ((Task)o)), t, cancellationToken, creationOptions, t);
				t.ContinueWithCore (cont, continuationOptions, scheduler, trigger.TrySet);
			}

			return source.Task;
		}

		public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult> (Task<TAntecedentResult>[] tasks,
		                                                                  Func<Task<TAntecedentResult>, TResult> continuationFunction)
		{
			return ContinueWhenAny (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}

		public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult> (Task<TAntecedentResult>[] tasks,
		                                                                  Func<Task<TAntecedentResult>, TResult> continuationFunction,
		                                                                  CancellationToken cancellationToken)
		{
			return ContinueWhenAny (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}

		public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult> (Task<TAntecedentResult>[] tasks,
		                                                                  Func<Task<TAntecedentResult>, TResult> continuationFunction,
		                                                                  TaskContinuationOptions continuationOptions)
		{
			return ContinueWhenAny (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}

		public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult> (Task<TAntecedentResult>[] tasks,
		                                                                  Func<Task<TAntecedentResult>, TResult> continuationFunction,
		                                                                  CancellationToken cancellationToken,
		                                                                  TaskContinuationOptions continuationOptions,
		                                                                  TaskScheduler scheduler)
		{
			return ContinueWhenAny<TResult> ((Task[])tasks,
			                                 (t) => continuationFunction((Task<TAntecedentResult>)t),
			                                 cancellationToken,
			                                 continuationOptions,
			                                 scheduler);
		}
		
		public Task ContinueWhenAll (Task[] tasks, Action<Task[]> continuationAction)
		{
			return ContinueWhenAll (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task ContinueWhenAll (Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken)
		{
			return ContinueWhenAll (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task ContinueWhenAll (Task[] tasks, Action<Task[]> continuationAction,
		                             TaskContinuationOptions continuationOptions)
		{
			return ContinueWhenAll (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task ContinueWhenAll (Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken,
		                             TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
		{
			var ourTasks = (Task[])tasks.Clone ();
			CountdownEvent evt = new CountdownEvent (ourTasks.Length);
			Task cont = new Task ((o) => continuationAction ((Task[])o), ourTasks, cancellationToken, creationOptions);
			
			foreach (Task t in ourTasks)
				t.ContinueWithCore (cont, continuationOptions, scheduler, evt.Signal);
			
			return cont;
		}
		
		public Task ContinueWhenAll<TAntecedentResult> (Task<TAntecedentResult>[] tasks,
		                                                Action<Task<TAntecedentResult>[]> continuationAction)
		{
			return ContinueWhenAll (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task ContinueWhenAll<TAntecedentResult> (Task<TAntecedentResult>[] tasks,
		                                                Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken)
		{
			return ContinueWhenAll (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task ContinueWhenAll<TAntecedentResult> (Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction,
		                                                TaskContinuationOptions continuationOptions)
		{
			return ContinueWhenAll (tasks, continuationAction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task ContinueWhenAll<TAntecedentResult> (Task<TAntecedentResult>[] tasks, 
		                                                Action<Task<TAntecedentResult>[]> continuationAction,
		                                                CancellationToken cancellationToken, TaskContinuationOptions continuationOptions,
		                                                TaskScheduler scheduler)
		{
			return ContinueWhenAll ((Task[]) tasks, (o) => continuationAction (tasks), cancellationToken,
			                        continuationOptions, scheduler);
		}
		
		public Task<TResult> ContinueWhenAll<TResult> (Task[] tasks, Func<Task[], TResult> continuationFunction)
		{
			return ContinueWhenAll<TResult> (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task<TResult> ContinueWhenAll<TResult> (Task[] tasks, Func<Task[], TResult> continuationFunction,
		                                               TaskContinuationOptions continuationOptions)
		{
			return ContinueWhenAll<TResult> (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task<TResult> ContinueWhenAll<TResult> (Task[] tasks, Func<Task[], TResult> continuationFunction,
		                                               CancellationToken cancellationToken)
		{
			return ContinueWhenAll<TResult> (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task<TResult> ContinueWhenAll<TResult> (Task[] tasks, Func<Task[], TResult> continuationFunction,
		                                               CancellationToken cancellationToken,
		                                               TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
		{
			var ourTasks = (Task[])tasks.Clone ();
			CountdownEvent evt = new CountdownEvent (ourTasks.Length);
			Task<TResult> cont = new Task<TResult> ((o) => continuationFunction ((Task[])o), ourTasks, cancellationToken, creationOptions);
			
			foreach (Task t in ourTasks)
				t.ContinueWithCore (cont, continuationOptions, scheduler, evt.Signal);
			
			return cont;
		}
		
		public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult> (Task<TAntecedentResult>[] tasks,
		                                                                  Func<Task<TAntecedentResult>[], TResult> continuationFunction)
		{
			return ContinueWhenAll<TAntecedentResult, TResult> (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult> (Task<TAntecedentResult>[] tasks, 
		                                                                  Func<Task<TAntecedentResult>[], TResult> continuationFunction,
		                                                                  TaskContinuationOptions continuationOptions)
		{
			return ContinueWhenAll<TAntecedentResult, TResult> (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult> (Task<TAntecedentResult>[] tasks,
		                                                                  Func<Task<TAntecedentResult>[], TResult> continuationFunction,
		                                                                  CancellationToken cancellationToken)
		{
			return ContinueWhenAll<TAntecedentResult, TResult> (tasks, continuationFunction, cancellationToken, continuationOptions, scheduler);
		}
		
		public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult> (Task<TAntecedentResult>[] tasks, 
		                                                                  Func<Task<TAntecedentResult>[], TResult> continuationFunction,
		                                                                  CancellationToken cancellationToken,
		                                                                  TaskContinuationOptions continuationOptions,
		                                                                  TaskScheduler scheduler)
		{
			return ContinueWhenAll<TResult> ((Task[]) tasks,
			                                 (o) => continuationFunction (tasks),
			                                 cancellationToken,
			                                 continuationOptions, scheduler);
		}

		#endregion
		
		#region FromAsync
		public Task FromAsync (IAsyncResult asyncResult, Action<IAsyncResult> endMethod)
		{
			return FromAsync (asyncResult, endMethod, creationOptions, scheduler);
		}
		
		public Task FromAsync (IAsyncResult asyncResult, Action<IAsyncResult> endMethod,
		                       TaskCreationOptions creationOptions)
		{
			return FromAsync (asyncResult, endMethod, creationOptions, scheduler);
		}
		
		public Task FromAsync (IAsyncResult asyncResult, Action<IAsyncResult> endMethod,
		                       TaskCreationOptions creationOptions, TaskScheduler scheduler)
		{
			return FromAsync<object> (asyncResult, (ar) => { endMethod (ar); return null; }, creationOptions, scheduler);
		}
		
		public Task<TResult> FromAsync<TResult> (IAsyncResult asyncResult, Func<IAsyncResult, TResult> endMethod)
		{
			return FromAsync<TResult> (asyncResult, endMethod, creationOptions, scheduler);
		}
		
		public Task<TResult> FromAsync<TResult> (IAsyncResult asyncResult, Func<IAsyncResult, TResult> endMethod,
		                                         TaskCreationOptions creationOptions)
		{
			return FromAsync<TResult> (asyncResult, endMethod, creationOptions, scheduler);
		}
		
		public Task<TResult> FromAsync<TResult> (IAsyncResult asyncResult, Func<IAsyncResult, TResult> endMethod,
		                                         TaskCreationOptions creationOptions, TaskScheduler scheduler)
		{
			var completionSource = new TaskCompletionSource<TResult> ();

			ThreadPool.RegisterWaitForSingleObject (asyncResult.AsyncWaitHandle,
			                                        (o, b) => {
				                                        try {
					                                        completionSource.SetResult (endMethod (asyncResult));
				                                        } catch (Exception e) {
					                                        completionSource.SetException (e);
				                                        }
			                                        },
			                                        null,
			                                        -1,
			                                        true);

			return completionSource.Task;
		}
		
		public Task FromAsync (Func<AsyncCallback, Object, IAsyncResult> beginMethod,
		                       Action<IAsyncResult> endMethod,
		                       object state)
		{
			return FromAsync (beginMethod, endMethod, state, creationOptions);
		}
		
		public Task FromAsync (Func<AsyncCallback, Object, IAsyncResult> beginMethod,
		                       Action<IAsyncResult> endMethod,
		                       object state, TaskCreationOptions creationOptions)
		{
			return FromAsync (beginMethod, (ar) => { endMethod (ar); return (object)null; }, state, creationOptions);
		}
		
		public Task FromAsync<TArg1> (Func<TArg1, AsyncCallback, Object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod,
		                              TArg1 arg1, object state)
		{
			return FromAsync (beginMethod, endMethod, arg1, state, creationOptions);
		}
		
		public Task FromAsync<TArg1> (Func<TArg1, AsyncCallback, Object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod,
		                              TArg1 arg1, object state, TaskCreationOptions creationOptions)
		{
			return FromAsync (beginMethod, (ar) => { endMethod (ar); return (object)null; }, arg1, state, creationOptions);
		}
		
		public Task FromAsync<TArg1, TArg2> (Func<TArg1, TArg2, AsyncCallback, Object, IAsyncResult> beginMethod,
		                                     Action<IAsyncResult> endMethod,
		                                     TArg1 arg1, TArg2 arg2, object state)
		{
			return FromAsync (beginMethod, endMethod, arg1, arg2, state, creationOptions);
		}
		
		public Task FromAsync<TArg1, TArg2> (Func<TArg1, TArg2, AsyncCallback, Object, IAsyncResult> beginMethod,
		                                     Action<IAsyncResult> endMethod,
		                                     TArg1 arg1, TArg2 arg2, object state, TaskCreationOptions creationOptions)
		{
			return FromAsync (beginMethod, (ar) => { endMethod (ar); return (object)null; }, arg1, arg2, state, creationOptions);
		}
		
		public Task FromAsync<TArg1, TArg2, TArg3> (Func<TArg1, TArg2, TArg3, AsyncCallback, Object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod,
		                                            TArg1 arg1, TArg2 arg2, TArg3 arg3, object state)
		{
			return FromAsync (beginMethod, endMethod, arg1, arg2, arg3, state, creationOptions);
		}
		
		public Task FromAsync<TArg1, TArg2, TArg3> (Func<TArg1, TArg2, TArg3, AsyncCallback, Object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod,
		                                            TArg1 arg1, TArg2 arg2, TArg3 arg3, object state, TaskCreationOptions creationOptions)
		{
			return FromAsync (beginMethod, (ar) => { endMethod (ar); return (object)null; }, arg1, arg2, arg3, state, creationOptions);
		}		
		
		public Task<TResult> FromAsync<TResult> (Func<AsyncCallback, Object, IAsyncResult> beginMethod,
		                                         Func<IAsyncResult, TResult> endMethod,
		                                         object state)
		{
			return FromAsync (beginMethod, endMethod, state, creationOptions);
		}
		
		public Task<TResult> FromAsync<TResult> (Func<AsyncCallback, Object, IAsyncResult> beginMethod,
		                                         Func<IAsyncResult, TResult> endMethod,
		                                         object state, TaskCreationOptions creationOptions)
		{
			var completionSource = new TaskCompletionSource<TResult> (creationOptions);
			beginMethod ((ar) => {
				try {
					completionSource.SetResult (endMethod (ar));
				} catch (Exception e) {
					completionSource.SetException (e);
				}
			}, state);

			return completionSource.Task;
		}
		
		public Task<TResult> FromAsync<TArg1, TResult> (Func<TArg1, AsyncCallback, Object, IAsyncResult> beginMethod,
		                                                Func<IAsyncResult, TResult> endMethod,
		                                                TArg1 arg1, object state)
		{
			return FromAsync (beginMethod, endMethod, arg1, state, creationOptions);
		}
		
		public Task<TResult> FromAsync<TArg1, TResult> (Func<TArg1, AsyncCallback, Object, IAsyncResult> beginMethod,
		                                                Func<IAsyncResult, TResult> endMethod,
		                                                TArg1 arg1, object state, TaskCreationOptions creationOptions)
		{
			var completionSource = new TaskCompletionSource<TResult> (creationOptions);
			beginMethod (arg1, (ar) => {
				try {
					completionSource.SetResult (endMethod (ar));
				} catch (Exception e) {
					completionSource.SetException (e);
				}
			}, state);

			return completionSource.Task;
		}

		public Task<TResult> FromAsync<TArg1, TArg2, TResult> (Func<TArg1, TArg2, AsyncCallback, Object, IAsyncResult> beginMethod,
		                                                       Func<IAsyncResult, TResult> endMethod,
		                                                       TArg1 arg1, TArg2 arg2, object state)
		{
			return FromAsync (beginMethod, endMethod, arg1, arg2, state, creationOptions);
		}
		
		public Task<TResult> FromAsync<TArg1, TArg2, TResult> (Func<TArg1, TArg2, AsyncCallback, Object, IAsyncResult> beginMethod,
		                                                       Func<IAsyncResult, TResult> endMethod,
		                                                       TArg1 arg1, TArg2 arg2, object state, TaskCreationOptions creationOptions)
		{
			var completionSource = new TaskCompletionSource<TResult> (creationOptions);
			beginMethod (arg1, arg2, (ar) => {
				try {
					completionSource.SetResult (endMethod (ar));
				} catch (Exception e) {
					completionSource.SetException (e);
				}
			}, state);

			return completionSource.Task;
		}
		
		public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult> (Func<TArg1, TArg2, TArg3, AsyncCallback, Object, IAsyncResult> beginMethod,
		                                                              Func<IAsyncResult, TResult> endMethod,
		                                                              TArg1 arg1, TArg2 arg2, TArg3 arg3, object state)
		{
			return FromAsync (beginMethod, endMethod, arg1, arg2, arg3, state, creationOptions);
		}
		
		public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult> (Func<TArg1, TArg2, TArg3, AsyncCallback, Object, IAsyncResult> beginMethod,
		                                                              Func<IAsyncResult, TResult> endMethod,
		                                                              TArg1 arg1, TArg2 arg2, TArg3 arg3, object state,
		                                                              TaskCreationOptions creationOptions)
		{
			var completionSource = new TaskCompletionSource<TResult> (creationOptions);
			beginMethod (arg1, arg2, arg3, (ar) => {
				try {
					completionSource.SetResult (endMethod (ar));
				} catch (Exception e) {
					completionSource.SetException (e);
				}
			}, state);

			return completionSource.Task;
		}
		#endregion
		
		public TaskScheduler Scheduler {
			get {
				return scheduler;
			}
		}
		
		public TaskContinuationOptions ContinuationOptions {
			get {
				return continuationOptions;
			}
		}
		
		public TaskCreationOptions CreationOptions {
			get {
				return creationOptions;
			}
		}
		
		public CancellationToken CancellationToken {
			get {
				return cancellationToken;
			}
		}
	}
}

