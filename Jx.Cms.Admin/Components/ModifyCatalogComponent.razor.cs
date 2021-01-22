using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Jx.Cms.Entities.Article;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Admin.Components
{
    public partial class ModifyCatalogComponent:ComponentBase, IResultDialog
    {
        [CascadingParameter(Name = "BodyContext")]
        private object CatalogId { get; set; }

        private CatalogEntity _catalogEntity;

        private IEnumerable<SelectedItem> _selectedItems;
        

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            List<CatalogEntity> catalogs = CatalogService.GetAllCatalogs();
            if (CatalogId is int catalogId)
            {
                 _catalogEntity = CatalogService.FindCatalogById(catalogId);
                if (_catalogEntity != null)
                {
                    catalogs = catalogs.Where(x => x.Id != _catalogEntity.Id).ToList();
                }
            }
            else
            {
                _catalogEntity = new CatalogEntity();
            }

            _selectedItems = catalogs.Select(x => new SelectedItem(x.Id.ToString(), x.Name));
        }

        public Task OnClose(DialogResult result)
        {
            return Task.CompletedTask;
        }
    }
}