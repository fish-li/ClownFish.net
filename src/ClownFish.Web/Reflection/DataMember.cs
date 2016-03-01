using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ClownFish.Base.Reflection;


namespace ClownFish.Web.Reflection
{
	internal abstract class DataMember
	{
		public abstract object GetValue(object obj);
		public abstract void SetValue(object obj, object val);
		public abstract Type Type { get; }
		public abstract string Name { get; }
		public abstract bool Ignore { get; }
	}

	internal sealed class PropertyMember : DataMember
	{
		private bool _ignore;
		private PropertyInfo _pi;

		public PropertyMember(PropertyInfo pi)
		{
			if( pi == null )
				throw new ArgumentNullException("pi");
			_pi = pi;
			_ignore = pi.GetMyAttribute<HttpValueIgnoreAttribute>(false) != null;
		}

		public override object GetValue(object obj)
		{
			//return _pi.GetValue(obj, null);
			return _pi.FastGetValue(obj);
		}

		public override void SetValue(object obj, object val)
		{
			//_pi.SetValue(obj, val, null);
			_pi.FastSetValue(obj, val);
		}

		public override Type Type
		{
			get { return _pi.PropertyType; }
		}

		public override string Name
		{
			get { return _pi.Name; }
		}

		public override bool Ignore
		{
			get { return _ignore; }
		}
	}


	internal sealed class FieldMember : DataMember
	{
		private bool _ignore;
		private FieldInfo _field;

		public FieldMember(FieldInfo fi)
		{
			if( fi == null )
				throw new ArgumentNullException("fi");
			_field = fi;
			_ignore = fi.GetMyAttribute<HttpValueIgnoreAttribute>() != null;
		}

		public override object GetValue(object obj)
		{
			//return _field.GetValue(obj);
			return _field.FastGetValue(obj);
		}

		public override void SetValue(object obj, object val)
		{
			//_field.SetValue(obj, val);
			_field.FastSetField(obj, val);
		}

		public override Type Type
		{
			get { return _field.FieldType; }
		}

		public override string Name
		{
			get { return _field.Name; }
		}

		public override bool Ignore
		{
			get { return _ignore; }
		}
	}



}
