using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Log.Logging;

/// <summary>
/// 用于为一段代码片段增加日志监控功能
/// </summary>
public sealed class CodeSnippetContext : BasePipelineContext, IDisposable
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public CodeSnippetContext(Type executorType, string operName, long performanceThresholdMs = 100)
    {
        if( executorType == null )
            throw new ArgumentNullException(nameof(executorType));

        this.PerformanceThresholdMs = performanceThresholdMs;

        this.CreateOprLogScope(executorType, operName);
    }

    private void CreateOprLogScope(Type executorType, string operName)
    {
        OprLogScope scope = OprLogScope.Start();
        this.SetOprLogScope(scope);

        OprLog log = scope.OprLog;
        log.RootId = this.ProcessId;
        log.OprKind = "task";
        log.AppKind = 1;

        log.Module = executorType.Namespace;
        log.Controller = executorType.Name;
        log.Action = "Execute";
        log.OprName = operName.HasValue() ? operName : (executorType.Name + "/Execute");
        log.Url = log.OprName + "/" + this.ProcessId;
    }


    void IDisposable.Dispose()
    {
        this.End();

        if( this.OprLogScope.IsNull == false ) {
            // 记录日志(OprLog + InvokeLog)
            this.SaveLog();
            this.DisposeOprLogScope();
        }
    }


    private void SaveLog()
    {
        OprLogScope scope = this.OprLogScope;
        scope.SetException(this.LastException);
        try {
            scope.SaveOprLog(this);
        }
        catch( Exception ex ) {
            Console2.Error("CodeSnippetContext.SaveLog() ERROR.", ex);
        }
    }

}