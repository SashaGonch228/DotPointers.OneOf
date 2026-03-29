using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DotPointers.OneOf
{
	[GenerateOneOf(new string[] { "Value", "Error" }, false, OneOfLayoutKind.Composition)]
	public readonly partial struct Result<T> : IOneOf<T, string>
	{
		public readonly void EnsureSuccess()
		{
			if (IsError) { throw new Exception(Error); }
		}

		public readonly TResult Map<TResult>(Func<T, TResult> OnValue)
		{
			EnsureSuccess();
			return OnValue.Invoke(Value);
		}
	}

	[GenerateOneOf(new string[] { "Value", "Error" }, false, OneOfLayoutKind.Composition)]
	public readonly partial struct Result<T, TException> : IOneOf<T, TException> where TException : Exception
	{
		public readonly void EnsureSuccess()
		{
			if (IsError) { throw Error; }
		}

		public readonly TResult Map<TResult>(Func<T, TResult> OnValue)
		{
			EnsureSuccess();
			return OnValue.Invoke(Value);
		}

		public static Result<T, Exception> Try(Func<T> func)
		{
			try { return func(); }
			catch (Exception ex) { return ex; }
		}
	}

	public class LazyResult<T, E> where E : Exception
	{
		private readonly Func<Result<T, E>> _factory;
		private Result<T, E>? _cache;

		public LazyResult(Func<Result<T, E>> factory) => _factory = factory;

		public Result<T, E> GetResult() => _cache ??= _factory();

		public LazyResult<TNext, E> Map<TNext>(Func<T, TNext> selector)
		{
			return new LazyResult<TNext, E>(() => GetResult().Map(selector));
		}

		public TResult Match<TResult>(Func<T, TResult> onValue, Func<E, TResult> onError)
		{
			var res = GetResult();
			return res.Match(onValue, onError);
		}
	}

	public readonly struct AsyncResult<T, E> where E : Exception
	{
		private readonly ValueTask<Result<T, E>> _task;

		public AsyncResult(ValueTask<Result<T, E>> task) => _task = task;
		public AsyncResult(Result<T, E> result) => _task = new ValueTask<Result<T, E>>(result);

		public ValueTaskAwaiter<Result<T, E>> GetAwaiter() => _task.GetAwaiter();

		public AsyncResult<TNext, E> Map<TNext>(Func<T, TNext> selector)
		{
			async ValueTask<Result<TNext, E>> Next(ValueTask<Result<T, E>> t)
			{
				var res = await t;
				return res.IsValue ? new Result<TNext, E>(selector(res.Value)) : res.Error;
			}
			return new AsyncResult<TNext, E>(Next(_task));
		}

		public AsyncResult<TNext, E> Bind<TNext>(Func<T, Result<TNext, E>> selector)
		{
			async ValueTask<Result<TNext, E>> Next(ValueTask<Result<T, E>> t)
			{
				var res = await t;
				return res.IsValue ? selector(res.Value) : res.Error;
			}
			return new AsyncResult<TNext, E>(Next(_task));
		}
	}

	public static class AsyncResultExtensions
	{
		public static AsyncResult<T, E> ToAsync<T, E>(this Task<Result<T, E>> task) where E : Exception => new(new ValueTask<Result<T, E>>(task));
		public static AsyncResult<T, E> ToAsync<T, E>(this Result<T, E> result) where E : Exception => new(result);
	}
}