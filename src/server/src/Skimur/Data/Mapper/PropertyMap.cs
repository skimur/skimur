using System.Reflection;

namespace Skimur.Data.Mapper
{
    public class PropertyMap
    {
        public PropertyMap(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            ColumnName = PropertyInfo.Name;
        }

        public PropertyInfo PropertyInfo { get; private set; }
        
        public string Name
        {
            get { return PropertyInfo.Name; }
        }
        
        public string ColumnName { get; private set; }
        
        public KeyType KeyType { get; private set; }
        
        public PropertyMap Column(string columnName)
        {
            ColumnName = columnName;
            return this;
        }
        
        public PropertyMap Key(KeyType keyType)
        {
            KeyType = keyType;
            return this;
        } 
    }
}
