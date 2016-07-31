using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
    /// <summary>
    /// 定义实体代理应该实现的接口（此接口仅供框架内部使用）
    /// </summary>
    public interface IEntityProxy
    {
		/// <summary>
		/// 设置代理要包装的实体对象
		/// </summary>
		/// <param name="entity"></param>
		void Init(Entity entity);

		/// <summary>
		/// 获取与代理对象关联的原实体对象
		/// </summary>
		Entity RealEntity { get; }


		/// <summary>
		/// 获取哪些字段发生了改变，返回对应的字段名称列表
		/// </summary>
		/// <returns></returns>
		string[] GetChangeNames();

		/// <summary>
		/// 获取哪些字段发生了改变，返回对应的字段值列表
		/// </summary>
		/// <returns></returns>
		object[] GetChangeValues();

		/// <summary>
		/// 获取实体的主键信息：字段名，字段值
		/// </summary>
		/// <returns></returns>
		Tuple<string, object> GetRowKey();

		/// <summary>
		/// 清除代理实体中所有属性的修改标记
		/// </summary>
		void ClearChangeFlags();

	}
}
