using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DotPointers.OneOf
{
	[GenerateOneOf(new string[] { "Value", "Error" }, false, OneOfLayoutKind.Composition)]
	public readonly partial struct Result<T> : IOneOf<T, Exception>
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

		public static Result<T> Try(Func<T> func)
		{
			try { return func(); }
			catch (Exception ex) { return ex; }
		}
	}

	public class LazyResult<T>
	{
		private readonly Func<Result<T>> _factory;
		private Result<T>? _cache;

		public LazyResult(Func<Result<T>> factory) => _factory = factory;

		public Result<T> GetResult() => _cache ??= _factory();

		public LazyResult<TNext> Map<TNext>(Func<T, TNext> selector)
		{
			return new LazyResult<TNext>(() => GetResult().Map(selector));
		}

		public TResult Match<TResult>(Func<T, TResult> onValue, Func<Exception, TResult> onError)
		{
			var res = GetResult();
			return res.Match(onValue, onError);
		}
	}

	public readonly struct AsyncResult<T>
	{
		private readonly ValueTask<Result<T>> _task;

		public AsyncResult(ValueTask<Result<T>> task) => _task = task;
		public AsyncResult(Result<T> result) => _task = new ValueTask<Result<T>>(result);

		public ValueTaskAwaiter<Result<T>> GetAwaiter() => _task.GetAwaiter();

		public AsyncResult<TNext> Map<TNext>(Func<T, TNext> selector)
		{
			async ValueTask<Result<TNext>> Next(ValueTask<Result<T>> t)
			{
				var res = await t;
				return res.IsValue ? new Result<TNext>(selector(res.Value)) : res.Error;
			}
			return new AsyncResult<TNext>(Next(_task));
		}

		public AsyncResult<TNext> Bind<TNext>(Func<T, Result<TNext>> selector)
		{
			async ValueTask<Result<TNext>> Next(ValueTask<Result<T>> t)
			{
				var res = await t;
				return res.IsValue ? selector(res.Value) : res.Error;
			}
			return new AsyncResult<TNext>(Next(_task));
		}
	}

	public static class AsyncResultExtensions
	{
		public static AsyncResult<T> ToAsync<T>(this Task<Result<T>> task) => new(new ValueTask<Result<T>>(task));
		public static AsyncResult<T> ToAsync<T>(this Result<T> result) => new(result);
	}
}