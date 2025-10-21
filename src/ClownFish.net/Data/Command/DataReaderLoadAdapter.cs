namespace ClownFish.Data;

internal class DataReaderLoadAdapter : DataAdapter
{
#if NETCOREAPP
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL2026: loadAdapter.FillLoadOption")]
#endif
    public static void FillTable(DataTable table, IDataReader reader, int maxRecords)
    {
        DataReaderLoadAdapter loadAdapter = new DataReaderLoadAdapter();
        loadAdapter.FillLoadOption = LoadOption.PreserveChanges;
        loadAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

        loadAdapter.FillFromReader(new DataTable[1] { table }, reader, 0, maxRecords);
    }

    internal int FillFromReader(DataTable[] dataTables, IDataReader dataReader, int startRecord, int maxRecords)
    {
        return Fill(dataTables, dataReader, startRecord, maxRecords);
    }
}
