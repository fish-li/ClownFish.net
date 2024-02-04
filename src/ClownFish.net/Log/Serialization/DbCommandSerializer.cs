namespace ClownFish.Log;

internal static class DbCommandSerializer
{
#if NET6_0_OR_GREATER
    public static string ToLoggingText(this DbBatch dbBatch)
    {
        if( (dbBatch?.BatchCommands?.Count ?? 0) == 0 )
            return string.Empty;

        DbBatchCommand command = dbBatch.BatchCommands.First();

        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.AppendLineRN(command.CommandText.SubstringN(LoggingLimit.SQL.CommandTextMaxLen));

            sb.Append("RecordsAffected: ").AppendLineRN(dbBatch.BatchCommands.Count.ToString());

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }
#endif

    public static string ToLoggingText(this DbCommand command)
    {
        if( command == null )
            return string.Empty;

        StringBuilder sb = StringBuilderPool.Get();
        try {

            //if( command.Transaction != null )
            //    sb.AppendLineRN($"[Transaction-Isolation-Level: {command.Transaction.IsolationLevel}]");

            sb.AppendLineRN(command.CommandText.SubstringN(LoggingLimit.SQL.CommandTextMaxLen));


            DbParameterCollection parameters = command.Parameters;
            if( parameters.IsNullOrEmpty() == false ) {

                // SQL和参数之间增加一个特殊的分隔行
                sb.AppendLineRN(TextUtils.StepDetailSeparatedLine3);

                for( int i = 0; i < parameters.Count; i++ ) {
                    if( i < LoggingLimit.SQL.ParametersMaxCount ) {
                        DbParameter parameter = parameters[i];

                        string name = parameter.ParameterName;
                        string dbType = parameter.DbType.ToString();
                        string value = GetParamValue(parameter.Value);

                        sb.AppendLineRN($"{name}({dbType})={value}");
                    }
                    else {
                        string name = "#####";
                        string dbType = System.Data.DbType.String.ToString();
                        string value = "参数太多，已被截断...，参数数量：" + parameters.Count.ToString();
                        sb.AppendLineRN($"{name}({dbType})={value}");
                        break;
                    }
                }
            }

            return sb.ToString().TrimEnd();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    internal static string GetParamValue(object value)
    {
        if( value == null || value == DBNull.Value )
            return "NULL";

        if( value is string text )
            return text.SubstringN(LoggingLimit.SQL.ParamValueMaxLen);

        if( value is DateTime time )
            return time.ToTime23String();

        if( value is long value64 )
            return value64.ToString();

        if( value is int value32 )
            return value32.ToString();

        if( value is decimal m )
            return m.ToString("F4");

        if( value is double d )
            return d.ToString("F4");

        if( value is float f )
            return f.ToString("F4");

        return value.ToString();
    }

}
