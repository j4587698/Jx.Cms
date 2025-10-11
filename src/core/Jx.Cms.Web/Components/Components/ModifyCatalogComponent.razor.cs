using BootstrapBlazor.Components;
using Jx.Cms.DbContext.Entities.Article;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Web.Components.Components;

public partial class ModifyCatalogComponent : ComponentBase, IResultDialog
{
    private CatalogEntity _catalogEntity;

    private IEnumerable<SelectedItem> _selectedItems;

    [CascadingParameter(Name = "BodyContext")]
    private object CatalogId { get; set; }

    public Task OnClose(DialogResult result)
    {
        return Task.CompletedTask;
    }


    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        var catalogs = CatalogService.GetAllCatalogs();
        if (CatalogId is int catalogId)
        {
            _catalogEntity = CatalogService.FindCatalogById(catalogId);
            if (_catalogEntity != null) catalogs = catalogs.Where(x => x.Id != _catalogEntity.Id).ToList();
        }
        else
        {
            _catalogEntity = new CatalogEntity();
        }

        _selectedItems = catalogs.Select(x => new SelectedItem(x.Id.ToString(), x.Name));
    }
}