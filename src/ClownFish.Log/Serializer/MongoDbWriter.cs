using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ClownFish.Log.Configuration;
using System.Runtime.CompilerServices;

namespace ClownFish.Log.Serializer
{
#if _MongoDB_

	/// <summary>
	/// 将日志记录到MongoDb的写入器
	/// </summary>
	public sealed class MongoDbWriter : ILogWriter
	{
		internal class MongoDbSetting
		{
			public string ConnectionString { get; set; }
			public string Database { get; set; }

			internal static MongoDbSetting Create(string connectionString)
			{
				if( string.IsNullOrEmpty(connectionString) )
					throw new ArgumentNullException("connectionString");

				MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder(connectionString);

				MongoDbSetting setting = new MongoDbSetting();
				setting.Database = mongoUrlBuilder.DatabaseName;
				setting.ConnectionString = connectionString;
				return setting;
			}
		}

		/// <summary>
		/// 配置文件中的连接设置
		/// </summary>
		private static MongoDbSetting s_configSetting;

		/// <summary>
		/// 当前实例的连接设置
		/// </summary>
		private MongoDbSetting _currentSetting;
		
		/// <summary>
		/// 可用于当前实例的连接设置
		/// </summary>
		private MongoDbSetting CrrentSetting
		{
			get { return _currentSetting ?? s_configSetting; }
		}


		#region ILogWriter 成员

		/// <summary>
		/// 从配置文件中初始化
		/// 注意：仅供框架调用，不需要在代码中调用。
		/// </summary>
		public void Init()
		{
			MongoDbWriterConfig config = WriterFactory.Config.Writers.MongoDb;

			s_configSetting = MongoDbSetting.Create(config.ConnectionString);
		}

		/// <summary>
		/// 设置默认的连接字符串。
		/// 注意：默认情况下并不需要调用这个方法，除非需要直接使用MongoDbWriter
		/// </summary>
		/// <param name="connectionString"></param>
		public void SetConnectionString(string connectionString)
		{
			_currentSetting = MongoDbSetting.Create(connectionString);
		}


		private IMongoDatabase GetMongoDatabase()
		{
			MongoClient mongoClient = new MongoClient(CrrentSetting.ConnectionString);

			return mongoClient.GetDatabase(CrrentSetting.Database);
		}




		private IMongoCollection<T> GetCollection<T>() where T : Model.BaseInfo
		{
			IMongoDatabase mongoDatabase = this.GetMongoDatabase();

			string colllectionName = typeof(T).Name;

			return mongoDatabase.GetCollection<T>(colllectionName);
		}

		/// <summary>
		/// 写入单条日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="info"></param>
		public void Write<T>(T info) where T : Model.BaseInfo
		{
			if( info == null )
				return;

			IMongoCollection<T> mongoCollection = this.GetCollection<T>();
			mongoCollection.InsertOne(info);
		}

		/// <summary>
		/// 批量写入日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public void Write<T>(List<T> list) where T : Model.BaseInfo
		{
			if( list == null || list.Count == 0 )
				return;


			IMongoCollection<T> mongoCollection = this.GetCollection<T>();
			mongoCollection.InsertMany(list);
		}

		/// <summary>
		/// 根据日志ID获取单条日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="guid"></param>
		/// <returns></returns>
		public T Get<T>(Guid guid) where T : Model.BaseInfo
		{
			IMongoCollection<T> mongoCollection = this.GetCollection<T>();
			return mongoCollection.AsQueryable().FirstOrDefault(item => item.InfoGuid == guid);
		}


		/// <summary>
		/// 根据指定的一段时间获取对应的日志记录
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="guidArray"></param>
		public void Delete<T>(params Guid[] guidArray) where T : Model.BaseInfo
		{
			if( guidArray == null || guidArray.Length == 0 )
				return;


			IMongoCollection<T> mongoCollection = this.GetCollection<T>();
			mongoCollection.DeleteMany(new FilterDefinitionBuilder<T>().In(item => item.InfoGuid, guidArray));
		}

		/// <summary>
		/// 根据指定的一段时间获取对应的日志记录
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public List<T> GetList<T>(DateTime t1, DateTime t2) where T : Model.BaseInfo
		{
			Expression<Func<T, bool>> func = x => x.Time >= t1 && x.Time < t2;
			return this.GetList(func);
		}

		#endregion


		/// <summary>
		/// 日志查询
		/// </summary>
		/// <param name="func">lambda表达式</param>
		/// <returns>查询结果</returns>
		public List<T> GetList<T>(Expression<Func<T, bool>> func) where T : Model.BaseInfo
		{
			if( func == null )
				throw new ArgumentNullException("func");

			IMongoCollection<T> mongoCollection = this.GetCollection<T>();
			return mongoCollection.AsQueryable()
							.Where(func)
							.OrderByDescending(m => m.Time)
							.ToList();
		}


		/// <summary>
		/// 日志分页查询
		/// </summary>
		/// <param name="pageIndex">页码</param>
		/// <param name="pageSize">分页大小</param>
		/// <param name="func">lambda表达式</param>
		/// <param name="totalCount">总数</param>
		/// <returns>查询结果</returns>
		public List<T> GetPageList<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, out int totalCount) 
			where T : Model.BaseInfo
		{
			IMongoCollection<T> mongoCollection = this.GetCollection<T>();

			IMongoQueryable<T> mongoQueryable = mongoCollection.AsQueryable();

			if( func != null )
				mongoQueryable = mongoQueryable.Where(func); //.AsQueryable();

			totalCount = mongoQueryable.Count();

			return mongoQueryable
							.OrderByDescending(m => m.Time)
							.Skip((pageIndex) * pageSize).Take(pageSize)
							.ToList();

		}

		
	}


#else

	/// <summary>
	/// MongoDbWriter的空实现
	/// </summary>
	public sealed class MongoDbWriter : NullWriter { }

#endif
}
