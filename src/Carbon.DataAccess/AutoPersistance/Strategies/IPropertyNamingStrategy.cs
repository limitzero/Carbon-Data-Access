using System;

namespace Carbon.Repository.AutoPersistance.Strategies
{
    /// <summary>
    /// Contract that will allow for custom naming strategy for 
    /// column names that are created in the *.hbm.xml file.
    /// </summary>
    public interface IColumnNamingStrategy
    {
        /// <summary>
        /// This will create the column name based on the current 
        /// property type.
        /// </summary>
        /// <param name="propertyName">Current name of the property on the class.</param>
        /// <param name="propertyType">Current property type to inact the naming strategy.</param>
        /// <returns></returns>
        string Execute(string propertyName, Type propertyType);
    }
}