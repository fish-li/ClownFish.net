using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.Linq;

namespace ClownFish.Data
{
	/// <summary>
	/// 实体方法的工厂类型，供框架内部使用
	/// </summary>
	public sealed class EntityMethodFactory
	{
		internal EntityMethodFactory() { }

		internal DbContext Context { get; set; }

		/// <summary>
		/// 创建与实体相关的EntityTable实例，开始数据库操作
		/// </summary>
		/// <typeparam name="T">实体的类型参数</typeparam>
		/// <returns>与实体相关的EntityTable实例</returns>
		public EntityTable<T> From<T>() where T : Entity, new()
		{
			return new EntityTable<T>() { Context = this.Context };
		}

		/// <summary>
		/// 开始LINQ查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public EntityQuery<T> Query<T>() where T : Entity, new()
		{
			EntityLinqProvider provider = new EntityLinqProvider() { Context = this.Context };
			return new EntityQuery<T>(provider);
		}


		/// <summary>
		/// 创建与实体相关的代理对象，并指示实体进入编辑状态，
		/// 请基本此方法的返回值来修改实体的属性，而不要直接修改原实体对象。
		/// 例如：var product = Entity.BeginEdit(product);
		/// 注意：Insert/Delete/Update操作必须基本此方法的返回值对象才能调用。
		/// </summary>
		/// <returns>与实体相关的代理对象</returns>
		public T BeginEdit<T>(T entity) where T : Entity, new()
		{
			if( entity == null )
				throw new ArgumentNullException(nameof(entity));

			if( entity is IEntityProxy )
				throw new ArgumentException("BeginEdit方法只接收实体对象，不允许操作代理对象。");

			return (T)entity.GetProxy(this.Context);
		}
	}
}
