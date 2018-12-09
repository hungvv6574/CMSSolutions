using CMSSolutions.GTools.Common.Models;
namespace CMSSolutions.GTools.Common.Data
{
    public interface IFieldTypeConverter<T>
    {
        FieldType GetScaffolderFieldType(T providerFieldType);

        T GetDataProviderFieldType(FieldType fieldType);
    }
}