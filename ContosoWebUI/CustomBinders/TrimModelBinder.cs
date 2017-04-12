using System.ComponentModel;
using System.Web.Mvc;

namespace ContosoWebUI.CustomBinders
{
    public class TrimModelBinder : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                if (value != null)
                {
                    value = ((string)value).Trim();
                }
            }

            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
            base.OnModelUpdated(controllerContext, bindingContext);
        }
    }
}