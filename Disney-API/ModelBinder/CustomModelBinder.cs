using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Disney_API.ModelBinder
{
    public class CustomModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            string model;
            if (bindingContext.ValueProvider.GetValue("name").FirstOrDefault() != null)
            {

                model = bindingContext.ValueProvider.GetValue("name").FirstOrDefault() ?? "";

            }
            else
            {
                //set the default value.
                model = "Default Value";
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }

    public class MyCustomBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            // specify the parameter your binder operates on
            if (context.Metadata.ParameterName == "param2")
            {
                return new BinderTypeModelBinder(typeof(CustomModelBinder));
            }
            return null;
        }
    }
}
