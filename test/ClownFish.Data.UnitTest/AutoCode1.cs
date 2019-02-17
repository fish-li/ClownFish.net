
// ClownFish.Data dynamically generate code
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using ClownFish.Data;

//[assembly: ClownFish.Data.EntityProxyAssembly]
//[assembly: System.Reflection.AssemblyTitle("ClownFish.Data.ProxyGen")]
//[assembly: System.Reflection.AssemblyDescription("")]
//[assembly: System.Reflection.AssemblyConfiguration("")]
//[assembly: System.Reflection.AssemblyCompany("")]
//[assembly: System.Reflection.AssemblyProduct("ClownFish.Data.ProxyGen")]
//[assembly: System.Reflection.AssemblyCopyright("Copyright © ClownFish 2016")]
//[assembly: System.Reflection.AssemblyTrademark("")]
//[assembly: System.Reflection.AssemblyCulture("")]
//[assembly: System.Runtime.InteropServices.ComVisible(false)]
//[assembly: System.Runtime.InteropServices.Guid("a54fdfa3-acf3-46a8-8f55-ffcbf2bcdaee")]
//[assembly: System.Reflection.AssemblyVersion("1.0.0.0")]
//[assembly: System.Reflection.AssemblyFileVersion("1.2019.217.1853")]


namespace ClownFish.Data.GeneratorCode
{
    [Serializable]
    public sealed class ModelX_4248716FE17677FE75F78A4863A35ADC_Proxy : ClownFish.Data.Test.ModelX, IEntityProxy
    {

        private bool[] _x_changeFlags = new bool[19];
        private ClownFish.Data.Test.ModelX _x_realEntity;

        Entity IEntityProxy.RealEntity { get { return _x_realEntity; } }
        private ClownFish.Data.Test.ModelX _X__Entity {
            get {
                if( _x_realEntity == null ) _x_realEntity = new ClownFish.Data.Test.ModelX();
                return _x_realEntity;
            }
        }

        void IEntityProxy.Init(Entity entity)
        {
            _x_realEntity = (ClownFish.Data.Test.ModelX)entity;
        }

        void IEntityProxy.ClearChangeFlags()
        {
            for( int i = 0; i < _x_changeFlags.Length; i++ )
                _x_changeFlags[i] = false;
        }
        #region Properties

        public override int RowId {
            get { return _x_realEntity.RowId; }
            set { _x_changeFlags[0] = true; _X__Entity.RowId = value; }
        }
        public override int IntField {
            get { return _x_realEntity.IntField; }
            set { _x_changeFlags[1] = true; _X__Entity.IntField = value; }
        }
        public override DateTime TimeField {
            get { return _x_realEntity.TimeField; }
            set { _x_changeFlags[2] = true; _X__Entity.TimeField = value; }
        }
        public override decimal MoneyField {
            get { return _x_realEntity.MoneyField; }
            set { _x_changeFlags[3] = true; _X__Entity.MoneyField = value; }
        }
        public override string StringField {
            get { return _x_realEntity.StringField; }
            set { _x_changeFlags[4] = true; _X__Entity.StringField = value; }
        }
        // ignore BoolField
        public override Guid GuidField {
            get { return _x_realEntity.GuidField; }
            set { _x_changeFlags[6] = true; _X__Entity.GuidField = value; }
        }
        public override int? IntNullField {
            get { return _x_realEntity.IntNullField; }
            set { _x_changeFlags[7] = true; _X__Entity.IntNullField = value; }
        }
        public override decimal? MoneyNullField {
            get { return _x_realEntity.MoneyNullField; }
            set { _x_changeFlags[8] = true; _X__Entity.MoneyNullField = value; }
        }
        // ignore GuidNullField
        public override short? ShortField {
            get { return _x_realEntity.ShortField; }
            set { _x_changeFlags[13] = true; _X__Entity.ShortField = value; }
        }
        public override System.Char Char1 {
            get { return _x_realEntity.Char1; }
            set { _x_changeFlags[14] = true; _X__Entity.Char1 = value; }
        }
        public override System.Nullable<System.Char> Char2 {
            get { return _x_realEntity.Char2; }
            set { _x_changeFlags[15] = true; _X__Entity.Char2 = value; }
        }
        public override byte[] Image {
            get { return _x_realEntity.Image; }
            set { _x_changeFlags[16] = true; _X__Entity.Image = value; }
        }
        public override Guid AutoGuid {
            get { return _x_realEntity.AutoGuid; }
            set { _x_changeFlags[17] = true; _X__Entity.AutoGuid = value; }
        }
        public override byte[] TimeStamp {
            get { return _x_realEntity.TimeStamp; }
            set { _x_changeFlags[18] = true; _X__Entity.TimeStamp = value; }
        }
        #endregion

        string[] IEntityProxy.GetChangeNames()
        {
            List<string> list = new List<string>(19);
            if( _x_changeFlags[0] ) list.Add("rid");
            if( _x_changeFlags[1] ) list.Add("intA");
            if( _x_changeFlags[2] ) list.Add("timeA");
            if( _x_changeFlags[3] ) list.Add("moneyA");
            if( _x_changeFlags[4] ) list.Add("stringA");
            if( _x_changeFlags[6] ) list.Add("guidA");
            if( _x_changeFlags[7] ) list.Add("intB");
            if( _x_changeFlags[8] ) list.Add("moneyB");
            if( _x_changeFlags[13] ) list.Add("shortB");
            if( _x_changeFlags[14] ) list.Add("charA");
            if( _x_changeFlags[15] ) list.Add("charB");
            if( _x_changeFlags[16] ) list.Add("img");
            if( _x_changeFlags[17] ) list.Add("g2");
            if( _x_changeFlags[18] ) list.Add("ts");
            return list.ToArray();
        }
        object[] IEntityProxy.GetChangeValues()
        {
            List<object> list = new List<object>(19);
            if( _x_changeFlags[0] ) list.Add(this.RowId);
            if( _x_changeFlags[1] ) list.Add(this.IntField);
            if( _x_changeFlags[2] ) list.Add(this.TimeField);
            if( _x_changeFlags[3] ) list.Add(this.MoneyField);
            if( _x_changeFlags[4] ) list.Add(this.StringField);
            if( _x_changeFlags[6] ) list.Add(this.GuidField);

            if( _x_changeFlags[7] ) {
                if( this.IntNullField.HasValue ) list.Add(this.IntNullField.Value);
                else list.Add(null);
            }

            if( _x_changeFlags[8] ) {
                if( this.MoneyNullField.HasValue ) list.Add(this.MoneyNullField.Value);
                else list.Add(null);
            }

            if( _x_changeFlags[13] ) {
                if( this.ShortField.HasValue ) list.Add(this.ShortField.Value);
                else list.Add(null);
            }
            if( _x_changeFlags[14] ) list.Add(this.Char1);

            if( _x_changeFlags[15] ) {
                if( this.Char2.HasValue ) list.Add(this.Char2.Value);
                else list.Add(null);
            }
            if( _x_changeFlags[16] ) list.Add(this.Image);
            if( _x_changeFlags[17] ) list.Add(this.AutoGuid);
            if( _x_changeFlags[18] ) list.Add(this.TimeStamp);
            return list.ToArray();
        }
        Tuple<string, object> IEntityProxy.GetRowKey()
        {
            return new Tuple<string, object>("rid", this.RowId);
        }

        [XmlAttribute]
        public bool[] XEntityDataChangeFlags { get { return _x_changeFlags; } set { _x_changeFlags = value; } }
    }
    [EntityAddition(ProxyType = typeof(ModelX_4248716FE17677FE75F78A4863A35ADC_Proxy))]
    public sealed class ModelX_4248716FE17677FE75F78A4863A35ADC_Loader : BaseDataLoader<ClownFish.Data.Test.ModelX>, IDataLoader<ClownFish.Data.Test.ModelX>
    {

        public override int[] CreateIndex(object dataSource)
        {
            return DataLoaderHelper.CreateNameMapIndex(dataSource, 19
           , new KeyValuePair<int, string>(0, "rid")
           , new KeyValuePair<int, string>(1, "intA")
           , new KeyValuePair<int, string>(2, "timeA")
           , new KeyValuePair<int, string>(3, "moneyA")
           , new KeyValuePair<int, string>(4, "stringA")
           , new KeyValuePair<int, string>(5, "boolA")
           , new KeyValuePair<int, string>(6, "guidA")
           , new KeyValuePair<int, string>(7, "intB")
           , new KeyValuePair<int, string>(8, "moneyB")
           , new KeyValuePair<int, string>(9, "guidB")
           , new KeyValuePair<int, string>(13, "shortB")
           , new KeyValuePair<int, string>(14, "charA")
           , new KeyValuePair<int, string>(15, "charB")
           , new KeyValuePair<int, string>(16, "img")
           , new KeyValuePair<int, string>(17, "g2")
           , new KeyValuePair<int, string>(18, "ts")
           );
        }
        public override void LoadFromDataReader(DbDataReader reader, int[] colIndex, ClownFish.Data.Test.ModelX m)
        {
            if( colIndex[0] >= 0 ) {
                m.RowId = reader.GetInt32(colIndex[0]);
            }
            if( colIndex[1] >= 0 ) {
                m.IntField = reader.GetInt32(colIndex[1]);
            }
            if( colIndex[2] >= 0 ) {
                m.TimeField = reader.GetDateTime(colIndex[2]);
            }
            if( colIndex[3] >= 0 ) {
                m.MoneyField = reader.GetDecimal(colIndex[3]);
            }
            if( colIndex[4] >= 0 ) {

                object val = reader.GetValue(colIndex[4]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.StringField = (string)(val);
            }
            if( colIndex[5] >= 0 ) {
                m.BoolField = reader.GetBoolean(colIndex[5]);
            }
            if( colIndex[6] >= 0 ) {
                m.GuidField = reader.GetGuid(colIndex[6]);
            }
            if( colIndex[7] >= 0 ) {

                object val = reader.GetValue(colIndex[7]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.IntNullField = (int)(val);
            }
            if( colIndex[8] >= 0 ) {

                object val = reader.GetValue(colIndex[8]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.MoneyNullField = (decimal)(val);
            }
            if( colIndex[9] >= 0 ) {

                object val = reader.GetValue(colIndex[9]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.GuidNullField = (Guid)(val);
            }
            if( colIndex[13] >= 0 ) {

                object val = reader.GetValue(colIndex[13]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.ShortField = (short)(val);
            }
            if( colIndex[14] >= 0 ) {
                m.Char1 = reader.GetString(colIndex[14])[0];
            }
            if( colIndex[15] >= 0 ) {

                object val = reader.GetValue(colIndex[15]);
                if( val != null && DBNull.Value.Equals(val) == false ) {
                    string str = val.ToString();
                    if( str.Length > 0 )
                        m.Char2 = str[0];
                }
            }
            if( colIndex[16] >= 0 ) {

                object val = reader.GetValue(colIndex[16]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Image = (byte[])(val);
            }
            if( colIndex[17] >= 0 ) {
                m.AutoGuid = reader.GetGuid(colIndex[17]);
            }
            if( colIndex[18] >= 0 ) {

                object val = reader.GetValue(colIndex[18]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.TimeStamp = (byte[])(val);
            }
        }
        public override void LoadFromDataRow(DataRow row, int[] colIndex, ClownFish.Data.Test.ModelX m)
        {
            if( colIndex[0] >= 0 ) {
                m.RowId = (int)(row[colIndex[0]]);
            }
            if( colIndex[1] >= 0 ) {
                m.IntField = (int)(row[colIndex[1]]);
            }
            if( colIndex[2] >= 0 ) {
                m.TimeField = (DateTime)(row[colIndex[2]]);
            }
            if( colIndex[3] >= 0 ) {
                m.MoneyField = (decimal)(row[colIndex[3]]);
            }
            if( colIndex[4] >= 0 ) {

                object val = row[colIndex[4]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.StringField = (string)(val);
            }
            if( colIndex[5] >= 0 ) {
                m.BoolField = (bool)(row[colIndex[5]]);
            }
            if( colIndex[6] >= 0 ) {
                m.GuidField = (Guid)(row[colIndex[6]]);
            }
            if( colIndex[7] >= 0 ) {

                object val = row[colIndex[7]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.IntNullField = (int)(val);
            }
            if( colIndex[8] >= 0 ) {

                object val = row[colIndex[8]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.MoneyNullField = (decimal)(val);
            }
            if( colIndex[9] >= 0 ) {

                object val = row[colIndex[9]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.GuidNullField = (Guid)(val);
            }
            if( colIndex[13] >= 0 ) {

                object val = row[colIndex[13]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.ShortField = (short)(val);
            }
            if( colIndex[14] >= 0 ) {
                m.Char1 = (System.Char)(row[colIndex[14]]);
            }
            if( colIndex[15] >= 0 ) {

                object val = row[colIndex[15]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Char2 = (System.Char)(val);
            }
            if( colIndex[16] >= 0 ) {

                object val = row[colIndex[16]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Image = (byte[])(val);
            }
            if( colIndex[17] >= 0 ) {
                m.AutoGuid = (Guid)(row[colIndex[17]]);
            }
            if( colIndex[18] >= 0 ) {

                object val = row[colIndex[18]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.TimeStamp = (byte[])(val);
            }
        }
    }
}
namespace ClownFish.Data.GeneratorCode
{
    public sealed class Model11_DA4D43D4B9219C8FD956A61DB33D18FD_Proxy : ClownFish.Data.UnitTest._DEMO.Model11, IEntityProxy
    {

        private bool[] _x_changeFlags = new bool[2];
        private ClownFish.Data.UnitTest._DEMO.Model11 _x_realEntity;

        Entity IEntityProxy.RealEntity { get { return _x_realEntity; } }
        private ClownFish.Data.UnitTest._DEMO.Model11 _X__Entity {
            get {
                if( _x_realEntity == null ) _x_realEntity = new ClownFish.Data.UnitTest._DEMO.Model11();
                return _x_realEntity;
            }
        }

        void IEntityProxy.Init(Entity entity)
        {
            _x_realEntity = (ClownFish.Data.UnitTest._DEMO.Model11)entity;
        }

        void IEntityProxy.ClearChangeFlags()
        {
            for( int i = 0; i < _x_changeFlags.Length; i++ )
                _x_changeFlags[i] = false;
        }
        #region Properties

        public override int IntValue {
            get { return _x_realEntity.IntValue; }
            set { _x_changeFlags[0] = true; _X__Entity.IntValue = value; }
        }
        public override string StrValue {
            get { return _x_realEntity.StrValue; }
            set { _x_changeFlags[1] = true; _X__Entity.StrValue = value; }
        }
        #endregion

        string[] IEntityProxy.GetChangeNames()
        {
            List<string> list = new List<string>(2);
            if( _x_changeFlags[0] ) list.Add("IntValue");
            if( _x_changeFlags[1] ) list.Add("StrValue");
            return list.ToArray();
        }
        object[] IEntityProxy.GetChangeValues()
        {
            List<object> list = new List<object>(2);
            if( _x_changeFlags[0] ) list.Add(this.IntValue);
            if( _x_changeFlags[1] ) list.Add(this.StrValue);
            return list.ToArray();
        }
        Tuple<string, object> IEntityProxy.GetRowKey()
        {
            throw new InvalidOperationException("实体没有属性被指定为【唯一】主键（ [DbColumn(PrimaryKey=true)] ），不能执行Update操作");
        }

        [XmlAttribute]
        public bool[] XEntityDataChangeFlags { get { return _x_changeFlags; } set { _x_changeFlags = value; } }
    }
    [EntityAddition(ProxyType = typeof(Model11_DA4D43D4B9219C8FD956A61DB33D18FD_Proxy))]
    public sealed class Model11_DA4D43D4B9219C8FD956A61DB33D18FD_Loader : BaseDataLoader<ClownFish.Data.UnitTest._DEMO.Model11>, IDataLoader<ClownFish.Data.UnitTest._DEMO.Model11>
    {

        public override int[] CreateIndex(object dataSource)
        {
            return DataLoaderHelper.CreateNameMapIndex(dataSource, 2
           , new KeyValuePair<int, string>(0, "IntValue")
           , new KeyValuePair<int, string>(1, "StrValue")
           );
        }
        public override void LoadFromDataReader(DbDataReader reader, int[] colIndex, ClownFish.Data.UnitTest._DEMO.Model11 m)
        {
            if( colIndex[0] >= 0 ) {
                m.IntValue = reader.GetInt32(colIndex[0]);
            }
            if( colIndex[1] >= 0 ) {

                object val = reader.GetValue(colIndex[1]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.StrValue = (string)(val);
            }
        }
        public override void LoadFromDataRow(DataRow row, int[] colIndex, ClownFish.Data.UnitTest._DEMO.Model11 m)
        {
            if( colIndex[0] >= 0 ) {
                m.IntValue = (int)(row[colIndex[0]]);
            }
            if( colIndex[1] >= 0 ) {

                object val = row[colIndex[1]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.StrValue = (string)(val);
            }
        }
    }
}
namespace ClownFish.Data.GeneratorCode
{
    [Serializable]
    public sealed class Customer_43883FCEC5889D4267FC1B47BA84FFB1_Proxy : ClownFish.Data.UnitTest.Models.Customer, IEntityProxy
    {

        private bool[] _x_changeFlags = new bool[6];
        private ClownFish.Data.UnitTest.Models.Customer _x_realEntity;

        Entity IEntityProxy.RealEntity { get { return _x_realEntity; } }
        private ClownFish.Data.UnitTest.Models.Customer _X__Entity {
            get {
                if( _x_realEntity == null ) _x_realEntity = new ClownFish.Data.UnitTest.Models.Customer();
                return _x_realEntity;
            }
        }

        void IEntityProxy.Init(Entity entity)
        {
            _x_realEntity = (ClownFish.Data.UnitTest.Models.Customer)entity;
        }

        void IEntityProxy.ClearChangeFlags()
        {
            for( int i = 0; i < _x_changeFlags.Length; i++ )
                _x_changeFlags[i] = false;
        }
        #region Properties

        public override int CustomerID {
            get { return _x_realEntity.CustomerID; }
            set { _x_changeFlags[0] = true; _X__Entity.CustomerID = value; }
        }
        public override string CustomerName {
            get { return _x_realEntity.CustomerName; }
            set { _x_changeFlags[1] = true; _X__Entity.CustomerName = value; }
        }
        public override string ContactName {
            get { return _x_realEntity.ContactName; }
            set { _x_changeFlags[2] = true; _X__Entity.ContactName = value; }
        }
        public override string Address {
            get { return _x_realEntity.Address; }
            set { _x_changeFlags[3] = true; _X__Entity.Address = value; }
        }
        public override string PostalCode {
            get { return _x_realEntity.PostalCode; }
            set { _x_changeFlags[4] = true; _X__Entity.PostalCode = value; }
        }
        public override string Tel {
            get { return _x_realEntity.Tel; }
            set { _x_changeFlags[5] = true; _X__Entity.Tel = value; }
        }
        #endregion

        string[] IEntityProxy.GetChangeNames()
        {
            List<string> list = new List<string>(6);
            if( _x_changeFlags[0] ) list.Add("CustomerID");
            if( _x_changeFlags[1] ) list.Add("CustomerName");
            if( _x_changeFlags[2] ) list.Add("ContactName");
            if( _x_changeFlags[3] ) list.Add("Address");
            if( _x_changeFlags[4] ) list.Add("PostalCode");
            if( _x_changeFlags[5] ) list.Add("Tel");
            return list.ToArray();
        }
        object[] IEntityProxy.GetChangeValues()
        {
            List<object> list = new List<object>(6);
            if( _x_changeFlags[0] ) list.Add(this.CustomerID);
            if( _x_changeFlags[1] ) list.Add(this.CustomerName);
            if( _x_changeFlags[2] ) list.Add(this.ContactName);
            if( _x_changeFlags[3] ) list.Add(this.Address);
            if( _x_changeFlags[4] ) list.Add(this.PostalCode);
            if( _x_changeFlags[5] ) list.Add(this.Tel);
            return list.ToArray();
        }
        Tuple<string, object> IEntityProxy.GetRowKey()
        {
            return new Tuple<string, object>("CustomerID", this.CustomerID);
        }

        [XmlAttribute]
        public bool[] XEntityDataChangeFlags { get { return _x_changeFlags; } set { _x_changeFlags = value; } }
    }
    [EntityAddition(ProxyType = typeof(Customer_43883FCEC5889D4267FC1B47BA84FFB1_Proxy))]
    public sealed class Customer_43883FCEC5889D4267FC1B47BA84FFB1_Loader : BaseDataLoader<ClownFish.Data.UnitTest.Models.Customer>, IDataLoader<ClownFish.Data.UnitTest.Models.Customer>
    {

        public override int[] CreateIndex(object dataSource)
        {
            return DataLoaderHelper.CreateNameMapIndex(dataSource, 6
           , new KeyValuePair<int, string>(0, "CustomerID")
           , new KeyValuePair<int, string>(1, "CustomerName")
           , new KeyValuePair<int, string>(2, "ContactName")
           , new KeyValuePair<int, string>(3, "Address")
           , new KeyValuePair<int, string>(4, "PostalCode")
           , new KeyValuePair<int, string>(5, "Tel")
           );
        }
        public override void LoadFromDataReader(DbDataReader reader, int[] colIndex, ClownFish.Data.UnitTest.Models.Customer m)
        {
            if( colIndex[0] >= 0 ) {
                m.CustomerID = reader.GetInt32(colIndex[0]);
            }
            if( colIndex[1] >= 0 ) {

                object val = reader.GetValue(colIndex[1]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.CustomerName = (string)(val);
            }
            if( colIndex[2] >= 0 ) {

                object val = reader.GetValue(colIndex[2]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.ContactName = (string)(val);
            }
            if( colIndex[3] >= 0 ) {

                object val = reader.GetValue(colIndex[3]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Address = (string)(val);
            }
            if( colIndex[4] >= 0 ) {

                object val = reader.GetValue(colIndex[4]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.PostalCode = (string)(val);
            }
            if( colIndex[5] >= 0 ) {

                object val = reader.GetValue(colIndex[5]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Tel = (string)(val);
            }
        }
        public override void LoadFromDataRow(DataRow row, int[] colIndex, ClownFish.Data.UnitTest.Models.Customer m)
        {
            if( colIndex[0] >= 0 ) {
                m.CustomerID = (int)(row[colIndex[0]]);
            }
            if( colIndex[1] >= 0 ) {

                object val = row[colIndex[1]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.CustomerName = (string)(val);
            }
            if( colIndex[2] >= 0 ) {

                object val = row[colIndex[2]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.ContactName = (string)(val);
            }
            if( colIndex[3] >= 0 ) {

                object val = row[colIndex[3]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Address = (string)(val);
            }
            if( colIndex[4] >= 0 ) {

                object val = row[colIndex[4]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.PostalCode = (string)(val);
            }
            if( colIndex[5] >= 0 ) {

                object val = row[colIndex[5]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Tel = (string)(val);
            }
        }
    }
}
namespace ClownFish.Data.GeneratorCode
{
    [Serializable]
    public sealed class Product_22504B0AA59DC41CEDC8925382100BEA_Proxy : ClownFish.Data.UnitTest.Models.Product, IEntityProxy
    {

        private bool[] _x_changeFlags = new bool[7];
        private ClownFish.Data.UnitTest.Models.Product _x_realEntity;

        Entity IEntityProxy.RealEntity { get { return _x_realEntity; } }
        private ClownFish.Data.UnitTest.Models.Product _X__Entity {
            get {
                if( _x_realEntity == null ) _x_realEntity = new ClownFish.Data.UnitTest.Models.Product();
                return _x_realEntity;
            }
        }

        void IEntityProxy.Init(Entity entity)
        {
            _x_realEntity = (ClownFish.Data.UnitTest.Models.Product)entity;
        }

        void IEntityProxy.ClearChangeFlags()
        {
            for( int i = 0; i < _x_changeFlags.Length; i++ )
                _x_changeFlags[i] = false;
        }
        #region Properties

        public override int ProductID {
            get { return _x_realEntity.ProductID; }
            set { _x_changeFlags[0] = true; _X__Entity.ProductID = value; }
        }
        public override string ProductName {
            get { return _x_realEntity.ProductName; }
            set { _x_changeFlags[1] = true; _X__Entity.ProductName = value; }
        }
        public override int CategoryID {
            get { return _x_realEntity.CategoryID; }
            set { _x_changeFlags[2] = true; _X__Entity.CategoryID = value; }
        }
        public override string Unit {
            get { return _x_realEntity.Unit; }
            set { _x_changeFlags[3] = true; _X__Entity.Unit = value; }
        }
        public override decimal UnitPrice {
            get { return _x_realEntity.UnitPrice; }
            set { _x_changeFlags[4] = true; _X__Entity.UnitPrice = value; }
        }
        public override string Remark {
            get { return _x_realEntity.Remark; }
            set { _x_changeFlags[5] = true; _X__Entity.Remark = value; }
        }
        public override int Quantity {
            get { return _x_realEntity.Quantity; }
            set { _x_changeFlags[6] = true; _X__Entity.Quantity = value; }
        }
        #endregion

        string[] IEntityProxy.GetChangeNames()
        {
            List<string> list = new List<string>(7);
            if( _x_changeFlags[0] ) list.Add("ProductID");
            if( _x_changeFlags[1] ) list.Add("ProductName");
            if( _x_changeFlags[2] ) list.Add("CategoryID");
            if( _x_changeFlags[3] ) list.Add("Unit");
            if( _x_changeFlags[4] ) list.Add("UnitPrice");
            if( _x_changeFlags[5] ) list.Add("Remark");
            if( _x_changeFlags[6] ) list.Add("Quantity");
            return list.ToArray();
        }
        object[] IEntityProxy.GetChangeValues()
        {
            List<object> list = new List<object>(7);
            if( _x_changeFlags[0] ) list.Add(this.ProductID);
            if( _x_changeFlags[1] ) list.Add(this.ProductName);
            if( _x_changeFlags[2] ) list.Add(this.CategoryID);
            if( _x_changeFlags[3] ) list.Add(this.Unit);
            if( _x_changeFlags[4] ) list.Add(this.UnitPrice);
            if( _x_changeFlags[5] ) list.Add(this.Remark);
            if( _x_changeFlags[6] ) list.Add(this.Quantity);
            return list.ToArray();
        }
        Tuple<string, object> IEntityProxy.GetRowKey()
        {
            return new Tuple<string, object>("ProductID", this.ProductID);
        }

        [XmlAttribute]
        public bool[] XEntityDataChangeFlags { get { return _x_changeFlags; } set { _x_changeFlags = value; } }
    }
    [EntityAddition(ProxyType = typeof(Product_22504B0AA59DC41CEDC8925382100BEA_Proxy))]
    public sealed class Product_22504B0AA59DC41CEDC8925382100BEA_Loader : BaseDataLoader<ClownFish.Data.UnitTest.Models.Product>, IDataLoader<ClownFish.Data.UnitTest.Models.Product>
    {

        public override int[] CreateIndex(object dataSource)
        {
            return DataLoaderHelper.CreateNameMapIndex(dataSource, 7
           , new KeyValuePair<int, string>(0, "ProductID")
           , new KeyValuePair<int, string>(1, "ProductName")
           , new KeyValuePair<int, string>(2, "CategoryID")
           , new KeyValuePair<int, string>(3, "Unit")
           , new KeyValuePair<int, string>(4, "UnitPrice")
           , new KeyValuePair<int, string>(5, "Remark")
           , new KeyValuePair<int, string>(6, "Quantity")
           );
        }
        public override void LoadFromDataReader(DbDataReader reader, int[] colIndex, ClownFish.Data.UnitTest.Models.Product m)
        {
            if( colIndex[0] >= 0 ) {
                m.ProductID = reader.GetInt32(colIndex[0]);
            }
            if( colIndex[1] >= 0 ) {

                object val = reader.GetValue(colIndex[1]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.ProductName = (string)(val);
            }
            if( colIndex[2] >= 0 ) {
                m.CategoryID = reader.GetInt32(colIndex[2]);
            }
            if( colIndex[3] >= 0 ) {

                object val = reader.GetValue(colIndex[3]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Unit = (string)(val);
            }
            if( colIndex[4] >= 0 ) {
                m.UnitPrice = reader.GetDecimal(colIndex[4]);
            }
            if( colIndex[5] >= 0 ) {

                object val = reader.GetValue(colIndex[5]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Remark = (string)(val);
            }
            if( colIndex[6] >= 0 ) {
                m.Quantity = reader.GetInt32(colIndex[6]);
            }
        }
        public override void LoadFromDataRow(DataRow row, int[] colIndex, ClownFish.Data.UnitTest.Models.Product m)
        {
            if( colIndex[0] >= 0 ) {
                m.ProductID = (int)(row[colIndex[0]]);
            }
            if( colIndex[1] >= 0 ) {

                object val = row[colIndex[1]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.ProductName = (string)(val);
            }
            if( colIndex[2] >= 0 ) {
                m.CategoryID = (int)(row[colIndex[2]]);
            }
            if( colIndex[3] >= 0 ) {

                object val = row[colIndex[3]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Unit = (string)(val);
            }
            if( colIndex[4] >= 0 ) {
                m.UnitPrice = (decimal)(row[colIndex[4]]);
            }
            if( colIndex[5] >= 0 ) {

                object val = row[colIndex[5]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Remark = (string)(val);
            }
            if( colIndex[6] >= 0 ) {
                m.Quantity = (int)(row[colIndex[6]]);
            }
        }
    }
}
namespace ClownFish.Data.GeneratorCode
{
    public sealed class Product2_2CDE0FF0CBA13546F9F70C152D12AE32_Proxy : ClownFish.Data.UnitTest.Models.Product2, IEntityProxy
    {

        private bool[] _x_changeFlags = new bool[7];
        private ClownFish.Data.UnitTest.Models.Product2 _x_realEntity;

        Entity IEntityProxy.RealEntity { get { return _x_realEntity; } }
        private ClownFish.Data.UnitTest.Models.Product2 _X__Entity {
            get {
                if( _x_realEntity == null ) _x_realEntity = new ClownFish.Data.UnitTest.Models.Product2();
                return _x_realEntity;
            }
        }

        void IEntityProxy.Init(Entity entity)
        {
            _x_realEntity = (ClownFish.Data.UnitTest.Models.Product2)entity;
        }

        void IEntityProxy.ClearChangeFlags()
        {
            for( int i = 0; i < _x_changeFlags.Length; i++ )
                _x_changeFlags[i] = false;
        }
        #region Properties

        public override int PId {
            get { return _x_realEntity.PId; }
            set { _x_changeFlags[0] = true; _X__Entity.PId = value; }
        }
        public override string PName {
            get { return _x_realEntity.PName; }
            set { _x_changeFlags[1] = true; _X__Entity.PName = value; }
        }
        public override int CID {
            get { return _x_realEntity.CID; }
            set { _x_changeFlags[2] = true; _X__Entity.CID = value; }
        }
        public override string Unt {
            get { return _x_realEntity.Unt; }
            set { _x_changeFlags[3] = true; _X__Entity.Unt = value; }
        }
        public override decimal UPrice {
            get { return _x_realEntity.UPrice; }
            set { _x_changeFlags[4] = true; _X__Entity.UPrice = value; }
        }
        public override string Remark2 {
            get { return _x_realEntity.Remark2; }
            set { _x_changeFlags[5] = true; _X__Entity.Remark2 = value; }
        }
        public override int Quantity2 {
            get { return _x_realEntity.Quantity2; }
            set { _x_changeFlags[6] = true; _X__Entity.Quantity2 = value; }
        }
        #endregion

        string[] IEntityProxy.GetChangeNames()
        {
            List<string> list = new List<string>(7);
            if( _x_changeFlags[0] ) list.Add("ProductID");
            if( _x_changeFlags[1] ) list.Add("ProductName");
            if( _x_changeFlags[2] ) list.Add("CategoryID");
            if( _x_changeFlags[3] ) list.Add("Unit");
            if( _x_changeFlags[4] ) list.Add("UnitPrice");
            if( _x_changeFlags[5] ) list.Add("Remark");
            if( _x_changeFlags[6] ) list.Add("Quantity");
            return list.ToArray();
        }
        object[] IEntityProxy.GetChangeValues()
        {
            List<object> list = new List<object>(7);
            if( _x_changeFlags[0] ) list.Add(this.PId);
            if( _x_changeFlags[1] ) list.Add(this.PName);
            if( _x_changeFlags[2] ) list.Add(this.CID);
            if( _x_changeFlags[3] ) list.Add(this.Unt);
            if( _x_changeFlags[4] ) list.Add(this.UPrice);
            if( _x_changeFlags[5] ) list.Add(this.Remark2);
            if( _x_changeFlags[6] ) list.Add(this.Quantity2);
            return list.ToArray();
        }
        Tuple<string, object> IEntityProxy.GetRowKey()
        {
            return new Tuple<string, object>("ProductID", this.PId);
        }

        [XmlAttribute]
        public bool[] XEntityDataChangeFlags { get { return _x_changeFlags; } set { _x_changeFlags = value; } }
    }
    [EntityAddition(ProxyType = typeof(Product2_2CDE0FF0CBA13546F9F70C152D12AE32_Proxy))]
    public sealed class Product2_2CDE0FF0CBA13546F9F70C152D12AE32_Loader : BaseDataLoader<ClownFish.Data.UnitTest.Models.Product2>, IDataLoader<ClownFish.Data.UnitTest.Models.Product2>
    {

        public override int[] CreateIndex(object dataSource)
        {
            return DataLoaderHelper.CreateNameMapIndex(dataSource, 7
           , new KeyValuePair<int, string>(0, "ProductID")
           , new KeyValuePair<int, string>(1, "ProductName")
           , new KeyValuePair<int, string>(2, "CategoryID")
           , new KeyValuePair<int, string>(3, "Unit")
           , new KeyValuePair<int, string>(4, "UnitPrice")
           , new KeyValuePair<int, string>(5, "Remark")
           , new KeyValuePair<int, string>(6, "Quantity")
           );
        }
        public override void LoadFromDataReader(DbDataReader reader, int[] colIndex, ClownFish.Data.UnitTest.Models.Product2 m)
        {
            if( colIndex[0] >= 0 ) {
                m.PId = reader.GetInt32(colIndex[0]);
            }
            if( colIndex[1] >= 0 ) {

                object val = reader.GetValue(colIndex[1]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.PName = (string)(val);
            }
            if( colIndex[2] >= 0 ) {
                m.CID = reader.GetInt32(colIndex[2]);
            }
            if( colIndex[3] >= 0 ) {

                object val = reader.GetValue(colIndex[3]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Unt = (string)(val);
            }
            if( colIndex[4] >= 0 ) {
                m.UPrice = reader.GetDecimal(colIndex[4]);
            }
            if( colIndex[5] >= 0 ) {

                object val = reader.GetValue(colIndex[5]);
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Remark2 = (string)(val);
            }
            if( colIndex[6] >= 0 ) {
                m.Quantity2 = reader.GetInt32(colIndex[6]);
            }
        }
        public override void LoadFromDataRow(DataRow row, int[] colIndex, ClownFish.Data.UnitTest.Models.Product2 m)
        {
            if( colIndex[0] >= 0 ) {
                m.PId = (int)(row[colIndex[0]]);
            }
            if( colIndex[1] >= 0 ) {

                object val = row[colIndex[1]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.PName = (string)(val);
            }
            if( colIndex[2] >= 0 ) {
                m.CID = (int)(row[colIndex[2]]);
            }
            if( colIndex[3] >= 0 ) {

                object val = row[colIndex[3]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Unt = (string)(val);
            }
            if( colIndex[4] >= 0 ) {
                m.UPrice = (decimal)(row[colIndex[4]]);
            }
            if( colIndex[5] >= 0 ) {

                object val = row[colIndex[5]];
                if( val != null && DBNull.Value.Equals(val) == false )
                    m.Remark2 = (string)(val);
            }
            if( colIndex[6] >= 0 ) {
                m.Quantity2 = (int)(row[colIndex[6]]);
            }
        }
    }
}
