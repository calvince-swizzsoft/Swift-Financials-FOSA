using System;
using System.Reflection;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    /// <summary>
    /// Easy access to common Assembly attributes.
    /// </summary>
    public class AssemblyAttributes
    {
        readonly Assembly _assembly;

        public AssemblyAttributes()
            : this(Assembly.GetCallingAssembly())
        { }

        public AssemblyAttributes(Assembly assembly)
        {
            _assembly = assembly;
        }

        public string Title
        {
            get { return GetValue<AssemblyTitleAttribute>(a => a.Title); }
        }

        public string Product
        {
            get { return GetValue<AssemblyProductAttribute>(a => a.Product); }
        }

        public string Copyright
        {
            get { return GetValue<AssemblyCopyrightAttribute>(a => a.Copyright); }
        }

        public string Company
        {
            get { return GetValue<AssemblyCompanyAttribute>(a => a.Company); }
        }

        public string Description
        {
            get { return GetValue<AssemblyDescriptionAttribute>(a => a.Description); }
        }

        public string Trademark
        {
            get { return GetValue<AssemblyTrademarkAttribute>(a => a.Trademark); }
        }

        public string Configuration
        {
            get { return GetValue<AssemblyConfigurationAttribute>(a => a.Configuration); }
        }

        public string Version
        {
            get
            {
#if !SILVERLIGHT
                return _assembly.GetName().Version.ToString();
#else
                return _assembly.FullName.Split(',')[1].Split('=')[1]; // workaround for silverlight
#endif
            }
        }

        public string FileVersion
        {
            get { return GetValue<AssemblyFileVersionAttribute>(a => a.Version); }
        }

        public string InformationalVersion
        {
            get { return GetValue<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion); }
        }

        /// <summary>
        /// Returns the value of attribute T or String.Empty if no value is available.
        /// </summary>
        string GetValue<T>(Func<T, string> getValue) where T : Attribute
        {
            T a = (T)Attribute.GetCustomAttribute(_assembly, typeof(T));

            return a == null ? "" : getValue(a);
        }

    }
}
