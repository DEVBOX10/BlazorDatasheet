using BlazorDatasheet.Formats;
using BlazorDatasheet.Interfaces;
using BlazorDatasheet.Render;

namespace BlazorDatasheet.ObjectEditor;

public class ObjectPropertyDefinition<T>
{
    public string PropertyName { get; set; }
    public string Heading { get; set; }
    public string Type { get; set; } = "text";
    public CellFormat? Format { get; set; }
    public bool IsReadOnly { get; set; }
    internal List<ConditionalFormat> ConditionalFormats { get; set; }
    internal List<IDataValidator> Validators { get; set; }

    internal ObjectPropertyDefinition(string propName, string type)
    {
        PropertyName = propName;
        Type = type;
        ConditionalFormats = new List<ConditionalFormat>();
        Validators = new List<IDataValidator>();
    }

    public void ApplyConditionalFormat(ConditionalFormat conditionalFormat)
    {
        ConditionalFormats.Add(conditionalFormat);
    }

    public void UseDataValidator(IDataValidator validator)
    {
        Validators.Add(validator);
    }
}