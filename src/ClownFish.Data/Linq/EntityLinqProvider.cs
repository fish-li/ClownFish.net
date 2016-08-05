using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.Linq
{
	/// <summary>
	/// 实体的LINQ查询的提供者
	/// </summary>
	public sealed class EntityLinqProvider : IQueryProvider
	{
		internal DbContext Context { get; set; }

		internal bool WithNoLock { get; set; }

		/// <summary>
		/// 构造一个 System.Linq.IQueryable 对象，该对象可计算指定表达式目录树所表示的查询。
		/// </summary>
		/// <param name="expression">表示 LINQ 查询的表达式目录树。</param>
		/// <returns></returns>
		public IQueryable CreateQuery(Expression expression)
		{
			// 不实现弱类型版本
			throw new NotImplementedException();
		}

		/// <summary>
		/// 构造一个 System.Linq.IQueryable`1 对象，该对象可计算指定表达式目录树所表示的查询。
		/// </summary>
		/// <typeparam name="TElement">返回的 System.Linq.IQueryable`1 的元素的类型</typeparam>
		/// <param name="expression">表示 LINQ 查询的表达式目录树。</param>
		/// <returns></returns>
		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new EntityQuery<TElement>(this, expression);
		}

		/// <summary>
		/// 执行指定表达式目录树所表示的查询。
		/// </summary>
		/// <param name="expression">表示 LINQ 查询的表达式目录树。</param>
		/// <returns></returns>
		public object Execute(Expression expression)
		{
			// 不实现弱类型版本
			throw new NotImplementedException();
		}

		/// <summary>
		/// 执行指定表达式目录树所表示的强类型查询。
		/// </summary>
		/// <typeparam name="TResult">执行查询所生成的值的类型</typeparam>
		/// <param name="expression">表示 LINQ 查询的表达式目录树。</param>
		/// <returns></returns>
		public TResult Execute<TResult>(Expression expression)
		{
			LinqParser sqlParser = new LinqParser();
			sqlParser.WithNoLock = this.WithNoLock;
			sqlParser.Context = this.Context;

			object result = sqlParser.Translator(expression);

			return (TResult)(object)result;
		}





	}
}
