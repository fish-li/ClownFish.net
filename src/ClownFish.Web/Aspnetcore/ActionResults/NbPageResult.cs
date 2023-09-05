using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ClownFish.Web.AspnetCore.ActionResults;

/// <summary>
/// PageResult
/// </summary>
public sealed class NbPageResult : ViewResult
{
    private static readonly EmptyModelMetadataProvider s_provider = new EmptyModelMetadataProvider();
    private static readonly ModelStateDictionary s_dictionary = new ModelStateDictionary();

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="viewPath"></param>
    /// <param name="viewModel"></param>
    public NbPageResult(string viewPath, object viewModel = null)
    {
        if( viewPath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(viewPath));

        var viewData = new ViewDataDictionary(s_provider, s_dictionary);
        viewData.Model = viewModel;

        ViewName = viewPath;
        ViewData = viewData;
        TempData = null;
    }
}
