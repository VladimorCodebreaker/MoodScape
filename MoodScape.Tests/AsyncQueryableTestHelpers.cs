using System;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace MoodScape.Tests;

public static class AsyncQueryableTestHelpers
{
    public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> enumerable)
    {
        return new AsyncQueryable<T>(enumerable);
    }

    private class AsyncQueryable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncQueryable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public AsyncQueryable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);
    }

    private class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;

        public AsyncEnumerator(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }

        public T Current => enumerator.Current;

        public ValueTask DisposeAsync()
        {
            enumerator.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(enumerator.MoveNext());
        }
    }

    private class AsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryProvider queryProvider;

        public AsyncQueryProvider(IQueryProvider queryProvider)
        {
            this.queryProvider = queryProvider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new AsyncQueryable<T>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncQueryable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return queryProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return queryProvider.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                                    .GetMethod(
                                        name: nameof(IQueryProvider.Execute),
                                        genericParameterCount: 1,
                                        types: new[] { typeof(Expression) })
                                    .MakeGenericMethod(expectedResultType)
                                    .Invoke(this, new object[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                                        ?.MakeGenericMethod(expectedResultType)
                                        .Invoke(null, new[] { executionResult });
        }
    }
}

