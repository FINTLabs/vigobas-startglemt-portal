using System;
using System.Collections.Generic;
using System.Configuration;
//using N-Log;
using Vigo.Bas.ManagementAgent.Log;

namespace VigoBAS_Start.Configuration
{
    public class PasswordValidationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public PasswordValidationInstanceCollection Instances
        {
            get { return (PasswordValidationInstanceCollection)this[""]; }
            set { this[""] = value; }
        }
    }
    public class PasswordValidationInstanceCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PasswordValidationInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PasswordValidationInstanceElement)element).Name;
        }
    }

    public class PasswordValidationInstanceElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("regex", IsRequired = true)]
        public string Regex
        {
            get { return (string)base["regex"]; }
            set { base["regex"] = value; }
        }

        [ConfigurationProperty("errorMessage", IsRequired = true)]
        public string ErrorMessage
        {
            get { return (string)base["errorMessage"]; }
            set { base["errorMessage"] = value; }
        }

        [ConfigurationProperty("description", IsRequired = true)]
        public string Description
        {
            get { return (string)base["description"]; }
            set { base["description"] = value; }
        }
    }

    public class PasswordConfigurationHelper
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static List<string> GetPasswordRules()
        {
            var rules = new List<string>();
            try
            {
                var validationRules = ConfigurationManager.GetSection("passwordValidation") as PasswordValidationSection;
                if (validationRules == null)
                {
                    return rules;
                }
                foreach (PasswordValidationInstanceElement rule in validationRules.Instances)
                {
                    rules.Add(rule.Description);
                }
            }
            catch (Exception e)
            {
                //Logger.Error(e);
                Logger.Log.Error(e);
                return rules;
            }
            return rules;
        }
    }
}