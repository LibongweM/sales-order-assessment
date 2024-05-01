using CoreTest.Components.Pages.OrderHeaderComponent;
using CoreTest.Components.Pages.OrderLinesComponent;
using CoreTest.Models;
using CoreTest.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Create = CoreTest.Components.Pages.OrderLinesComponent.Create;


namespace CoreTest.Components.Pages
{
    public partial class OrderHeaderBase : ComponentBase
    {
        [Inject] private IOrderHeaderService Service { get; set; } = default!;
        [Inject] private IOrderLineService OrderLineService { get; set; } = default!;

        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;
        
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        protected List<OrderHeader> OrderHeaders = [];

        protected List<OrderLine> OrderLines = [];

        protected MudTable<OrderHeader> MudTable;
        
        public MudTable<OrderLine> OrderLineTable;
        
        public string? SearchOrderNumber { get; set; }
        public string? SearchOrderType { get; set; }
        protected DateTime? FromDate { get; set; }
        protected DateTime? ToDate { get; set; }
        public string? SearchProductCode { get; set; }
        
        private int _selectedRowNumber = -1;

        protected string? HeaderId { get; private set; } 

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }
        
        private async Task LoadData()
        {
            OrderHeaders = await Service.GetAllOrderHeadersAsync() ?? [];
        }
        
        public void RowClickEvent(TableRowClickEventArgs<OrderHeader> tableRowClickEventArgs)
        {
            HeaderId=tableRowClickEventArgs.Item.Id;
            OrderLines = tableRowClickEventArgs.Item.OrderLines;
        }

        protected string SelectedRowClassFunc(OrderHeader element, int rowNumber)
        {
            if (_selectedRowNumber == rowNumber)
            {
                _selectedRowNumber = -1;
                return string.Empty;
            }

            if (MudTable.SelectedItem != null && MudTable.SelectedItem.Equals(element))
            {
                return "selected";
            }
            
            return string.Empty;
        }

        #region Snackbars

        private async Task OnOrderHeaderSubmitted()
        {
            OrderHeaders = await Service.GetAllOrderHeadersAsync() ?? [];
            Snackbar.Add("Order Header was added successfully.", Severity.Success);
            
        }

        private async Task OnOrderHeaderUpdate()
        {
            if (HeaderId != null)
            {
                OrderHeaders = await Service.GetAllOrderHeadersAsync() ?? [];
                Snackbar.Add("Order Header was updated successfully.", Severity.Success);
              
            }
        }

        private async Task OnOrderUpdate()
        {
            if (HeaderId != null)
            {
                OrderLines = await OrderLineService.GetOrderLineByHeaderAsync(HeaderId) ?? [];
                Snackbar.Add("Order line was updated successfully.", Severity.Success);
              
            }
        }

        private async Task OnOrderSubmitted()
        {
            if (HeaderId != null)
            {
                OrderLines = await OrderLineService.GetOrderLineByHeaderAsync(HeaderId) ?? [];
                Snackbar.Add("Order line was added successfully.", Severity.Success);
              
            }
        }

        #endregion

        #region SearchLogic

        protected async Task ClearOrderHeaderSearch()
        {
            SearchOrderNumber = string.Empty;
            SearchOrderType = string.Empty;
            FromDate = null;
            ToDate = null;
            
            await LoadData();
        }
        
        public async Task ClearOrderLineSearch()
        {
            SearchProductCode = string.Empty;
            if (HeaderId != null) OrderLines = await OrderLineService.GetOrderLineByHeaderAsync(HeaderId);
        }
        
        public async Task SearchOrders()
        {
            await LoadData();
            
            var filteredOrderHeaders = OrderHeaders;
            
            if (!string.IsNullOrEmpty(SearchOrderNumber))
            {
                filteredOrderHeaders = filteredOrderHeaders.Where(oH => oH.OrderNumber.Contains(SearchOrderNumber, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            
            if(!string.IsNullOrEmpty(SearchOrderType))
            {
                filteredOrderHeaders = filteredOrderHeaders.Where(oH => oH.OrderType == (OrderType) Enum.Parse(typeof(OrderType), SearchOrderType)).ToList();
            }
            
            if(!string.IsNullOrEmpty(FromDate.ToString()) && !string.IsNullOrEmpty(ToDate.ToString()))
            {
                filteredOrderHeaders = filteredOrderHeaders.Where(oH => (oH.OrderCreated >= FromDate) && oH.OrderCreated <= ToDate).ToList();
            }
            
            OrderHeaders = filteredOrderHeaders;
        }
        
        public async Task SearchOrderLines()
        {
            if (HeaderId != null) OrderLines = await OrderLineService.GetOrderLineByHeaderAsync(HeaderId);
            
            var filteredOrderLines = OrderLines;
            
            if (!string.IsNullOrEmpty(SearchProductCode))
            {
                filteredOrderLines = OrderLines
                    .Where(line =>
                        line.ProductCode.Contains(SearchProductCode, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            
            OrderLines = filteredOrderLines;
        }

        #endregion
    }
}
