using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DotPointers.OneOf.Types
{
	[GenerateOneOf(["Value", "Error"], false)]
	public readonly partial struct Result<T> : IOneOf<T, Exception>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void EnsureSuccess()
		{
			if (IsError) { throw Error; }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly TResult Map<TResult>(Func<T, TResult> OnValue)
		{
			EnsureSuccess();
			return OnValue.Invoke(Value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public LazyResult(Func<Result<T>> factory) => _factory = factory;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> GetResult() => _cache ??= _factory();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public LazyResult<TNext> Map<TNext>(Func<T, TNext> selector)
		{
			return new LazyResult<TNext>(() => GetResult().Map(selector));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TResult Match<TResult>(Func<T, TResult> onValue, Func<Exception, TResult> onError)
		{
			var res = GetResult();
			return res.Match(onValue, onError);
		}
	}

	public readonly struct AsyncResult<T>
	{
		private readonly ValueTask<Result<T>> _task;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AsyncResult(ValueTask<Result<T>> task) => _task = task;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AsyncResult(Result<T> result) => _task = new ValueTask<Result<T>>(result);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ValueTaskAwaiter<Result<T>> GetAwaiter() => _task.GetAwaiter();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AsyncResult<TNext> Map<TNext>(Func<T, TNext> selector)
		{
			async ValueTask<Result<TNext>> Next(ValueTask<Result<T>> t)
			{
				var res = await t;
				return res.IsValue ? new Result<TNext>(selector(res.Value)) : res.Error;
			}
			return new AsyncResult<TNext>(Next(_task));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AsyncResult<T> ToAsync<T>(this Task<Result<T>> task) => new(new ValueTask<Result<T>>(task));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AsyncResult<T> ToAsync<T>(this Result<T> result) => new(result);
	}
}