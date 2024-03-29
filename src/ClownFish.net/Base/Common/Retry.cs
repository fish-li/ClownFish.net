﻿namespace ClownFish.Base;

/// <summary>
/// 用于执行重试任务的工具类
/// </summary>
public sealed class Retry
{
    /// <summary>
    /// 重试策略的默认值
    /// </summary>
    public static class Default
    {
        // ### 注意 注意 注意 ###
        // 下面二个默认值不能从配置文件中读取，原因可以查看 RetryFile 中的注释代码

        /// <summary>
        /// 重试次数，默认值：2
        /// </summary>
        public static int Count = 2;

        /// <summary>
        /// 重试的间隔时间，单位：毫秒，默认值：1000ms
        /// </summary>
        public static int WaitMillisecond = 1000;
    }


    private int _retryCount;

    private List<Func<Exception, bool>> _filterList = null;
    private List<Action<Exception, int>> _callbakList = null;


    /// <summary>
    /// 根据默认值，创建一个Retry实例
    /// </summary>
    /// <returns></returns>
    public static Retry Create()
    {
        return new Retry { Count = Default.Count, Milliseconds = Default.WaitMillisecond };
    }

    /// <summary>
    /// 创建RetryOption实例
    /// </summary>
    /// <param name="count">重试次数，0表示不重试</param>
    /// <param name="milliseconds">重试的间隔时间，单位：毫秒</param>
    /// <returns></returns>
    public static Retry Create(int count, int milliseconds = 1000)
    {
        return new Retry { Count = count, Milliseconds = milliseconds };
    }



    /// <summary>
    /// 重试次数
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 二次重试之间的间隔毫秒。
    /// 如果不指定，默认 1 秒。
    /// </summary>
    public int Milliseconds { get; set; }

    /// <summary>
    /// 实际执行的次数
    /// </summary>
    internal int ExecuteCount { get; private set; }


    /// <summary>
    /// 设置仅重试哪些类型的异常。
    /// 允许多次调用（以OR方式处理）。
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <returns></returns>
    public Retry Filter<TException>() where TException : Exception
    {
        if( _filterList == null )
            _filterList = new List<Func<Exception, bool>>();


        _filterList.Add(
            (Exception ex) => {
                TException tex = ex as TException;
                if( tex == null )
                    return false;

                return true;
            });

        return this;
    }

    /// <summary>
    /// 设置仅重试哪些类型的异常，并允许根据特殊的异常再进一步判断。
    /// 允许多次调用（以OR方式处理）。
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <param name="func">委托返回 TRUE，表示操作需要重试</param>
    /// <returns></returns>
    public Retry Filter<TException>(Func<TException, bool> func) where TException : Exception
    {
        if( func == null )
            throw new ArgumentNullException(nameof(func));

        if( _filterList == null )
            _filterList = new List<Func<Exception, bool>>();


        _filterList.Add(
            (Exception ex) => {
                TException tex = ex as TException;
                if( tex == null )
                    return false;

                return func(tex);
            });

        return this;
    }

    /// <summary>
    /// 注册当异常发生时需要执行的回调委托
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public Retry OnException(Action<Exception, int> callback)
    {
        if( callback == null )
            throw new ArgumentNullException(nameof(callback));

        if( _callbakList == null )
            _callbakList = new List<Action<Exception, int>>();

        _callbakList.Add(callback);
        return this;
    }






    /// <summary>
    /// 执行重试任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
    public T Run<T>(Func<T> func)
    {
        if( func == null )
            throw new ArgumentNullException(nameof(func));

        // 重试次数设置不正确（或者不需要重试），直接调用（不做异常处理）
        if( this.Count <= 0 ) {
            this.ExecuteCount++;
            return func();
        }



        while( true ) {
            try {
                this.ExecuteCount++;
                return func();
            }
            catch( Exception ex ) {
                if( CheckCount(ex) == false ) {
                    // 如果超过重试次数，就抛出本次捕获的异常，结束整个重试机制
                    throw;
                }
            }
        }
    }


    /// <summary>
    /// 执行重试任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<T> RunAsync<T>(Func<Task<T>> func)
    {
        if( func == null )
            throw new ArgumentNullException(nameof(func));

        // 重试次数设置不正确，直接调用（不做异常处理）
        if( this.Count <= 0 ) {
            this.ExecuteCount++;
            return await func();
        }


        while( true ) {
            try {
                this.ExecuteCount++;
                return await func();
            }
            catch( Exception ex ) {
                if( CheckCount(ex) == false ) {
                    // 如果超过重试次数，就抛出本次捕获的异常，结束整个重试机制
                    throw;
                }
            }
        }
    }


    private bool CheckFilter(Exception ex)
    {
        // 如果没有定义过滤条件，就认为需要重试
        if( _filterList == null || _filterList.Count == 0 )
            return true;


        foreach( var func in _filterList ) {

            // 只要满足一个过滤条件就认为是有效的异常，需要执行重试
            if( func(ex) ) {
                return true;
            }
        }

        // 所有过滤条件都不满足
        return false;
    }

    private bool CheckCount(Exception ex)
    {
        _retryCount++;

        // 执行异常回调
        if( _callbakList != null ) {
            foreach( var cb in _callbakList )
                cb(ex, _retryCount);
        }


        // 如果不满足过滤条件，就直接跳出，最终也不会被重试
        if( CheckFilter(ex) == false ) {
            return false;
        }


        // 如果在重试次数之内，就启动重试机制
        if( _retryCount <= this.Count ) {

            // 为了保证重试有效，先暂停，等待外部环境变化
            if( this.Milliseconds > 0 )
                System.Threading.Thread.Sleep(this.Milliseconds);


            return true;
        }
        else {
            return false;
        }
    }


}
