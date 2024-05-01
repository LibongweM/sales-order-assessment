using CoreTest.Components.Pages.OrderHeaderComponent;
using CoreTest.Components.Pages.OrderLinesComponent;
using CoreTest.Models;
using CoreTest.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Create = CoreTest.Components.Pages.OrderLinesComponent.Create;


namespace CoreTest.Components.Pages
{
    public class OrderHeaderBase : ComponentBase
    {
        [Inject] private IOrderHeaderService Service { get; set; } = default!;
        [Inject] private IOrderLineService OrderLineService { get; set; } = default!;
        
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        
        [Inject] private NavigationManager Navigation { get; set; }
        
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        
        public List<OrderHeader> orderHeaders = new List<OrderHeader>();
        
        
        public List<OrderLine> orderLines = new List<OrderLine>();
        
        public MudTable<OrderHeader> mudTable;
        
        public MudTable<OrderLine> orderLineTable;
        
        public string SearchOrderHeader { get; set; }
        public string SearchOrderType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SearchProductCode { get; set; }
        
        private int selectedRowNumber = -1;

        protected string? HeaderId { get; set; }
        
        [Parameter] public OrderLine orderLine { get; set; }  
        

        protected override async Task OnInitializedAsync()
        {
            orderHeaders = await Service.GetAllOrderHeadersAsync() ?? [];
            
        }
        
        // public void AddNewOrder()
        // {
        //     var options = new DialogOptions { CloseOnEscapeKey = true };
        //     DialogService.Show<CreateHeader>("Add Order", options);
        //     Snackbar.Add("Order was added successfully.", Severity.Success);
        // }
        
        public void AddNewOrderLine()
        {
            var onOrderSubmittedCallback = EventCallback.Factory.Create(this, OnOrderSubmitted);
            var parameters = new DialogParameters<Create>();
            parameters.Add(x=>x.Id, HeaderId);
            parameters.Add("OnOrderSubmitted", onOrderSubmittedCallback);
            
            var options = new DialogOptions { CloseOnEscapeKey = true };
            DialogService.Show<Create>("Add Order Line", parameters, options);
           
        }
        public void AddNewOrder()
        {
            var onOrderHeaderSubmittedCallback = EventCallback.Factory.Create(this, OnOrderHeaderSubmitted);
            var parameters = new DialogParameters<CreateHeader>();
            parameters.Add("OnOrderHeaderSubmitted", onOrderHeaderSubmittedCallback);

            var options = new DialogOptions { CloseOnEscapeKey = true };
            DialogService.Show<CreateHeader>("Add Order", parameters, options);
        }

        public void UpdateOrderHeader(OrderHeader header)
        {
            var onOrderHeaderUpdateCallback = EventCallback.Factory.Create(this, OnOrderHeaderUpdate);
            var parameters = new DialogParameters<UpdateHeader>();
            parameters.Add(x => x.orderHeader, header);
            parameters.Add("OnOrderHeaderUpdate", onOrderHeaderUpdateCallback);

            var options = new DialogOptions { CloseOnEscapeKey = true };
            DialogService.Show<UpdateHeader>("Update Order Header", parameters, options);
        }
        
        protected async Task OnOrderSubmitted()
        {
            if (HeaderId != null)
            {
                orderLines = await OrderLineService.GetOrderLineByHeaderAsync(HeaderId) ?? [];
                Snackbar.Add("Order line was added successfully.", Severity.Success);
              
            }
        }
        
        protected async Task OnOrderHeaderSubmitted()
        {
            orderHeaders = await Service.GetAllOrderHeadersAsync() ?? [];
            Snackbar.Add("Order Header was added successfully.", Severity.Success);
            
        }

        protected async Task OnOrderHeaderUpdate()
        {
            if (HeaderId != null)
            {
                orderHeaders = await Service.GetAllOrderHeadersAsync() ?? [];
                Snackbar.Add("Order Header was updated successfully.", Severity.Success);
              
            }
        }
        protected async Task OnOrderUpdate()
        {
            if (HeaderId != null)
            {
                orderLines = await OrderLineService.GetOrderLineByHeaderAsync(HeaderId) ?? [];
                Snackbar.Add("Order line was updated successfully.", Severity.Success);
              
            }
        }
        public async Task UpdateOrderLine(OrderLine line) 
        {
            var onOrderUpdateCallback = EventCallback.Factory.Create(this, OnOrderUpdate);
            var parameters = new DialogParameters<Update>();
            parameters.Add(x=>x.orderLine, line);
             parameters.Add("OnOrderUpdate", onOrderUpdateCallback);

            var options = new DialogOptions { CloseOnEscapeKey = true };
            DialogService.Show<Update>("Update Order Line", parameters, options);
            
        }
        
 
        
        // public async Task UpdateOrderHeader(OrderHeader header) 
        // {
        //     var parameters = new DialogParameters<UpdateHeader>();
        //     parameters.Add(x=>x.orderHeader, header);
        //     
        //     var options = new DialogOptions { CloseOnEscapeKey = true };
        //     DialogService.Show<UpdateHeader>("Update Order Header", parameters, options);
        //     Snackbar.Add("Order Header was updated successfully.", Severity.Success);
        // }
        
        protected async Task DeleteOrderHeader(string id)
        {
            var result = await DialogService.ShowMessageBox(
                "Confirm",
                "Are you sure you want to delete this order Header?",
                yesText: "Yes", cancelText: "Cancel");

            if (result == true)
            {
                await Service.DeleteOrderHeaderAsync(id) ;
                orderHeaders = await Service.GetAllOrderHeadersAsync() ?? [];
                Snackbar.Add("Order Header was deleted successfully.", Severity.Success);
            }
          //  orderHeaders = await Service.GetAllOrderHeadersAsync() ?? [];
            
        }
      
        
        protected async Task DeleteOrderLine(string id)
        {
            var result = await DialogService.ShowMessageBox(
                "Confirm",
                "Are you sure you want to delete this order line?",
                yesText: "Yes", cancelText: "Cancel");

            if (result == true)
            {
                await OrderLineService.DeleteOrderLineAsync(id);
                Snackbar.Add("Order line was deleted successfully.", Severity.Success);
            }
        }
        public void RowClickEvent(TableRowClickEventArgs<OrderHeader> tableRowClickEventArgs)
        {
            HeaderId=tableRowClickEventArgs.Item.Id;
            orderLines = tableRowClickEventArgs.Item.OrderLines;
        }

        public string SelectedRowClassFunc(OrderHeader element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                return string.Empty;
            }

            if (mudTable.SelectedItem != null && mudTable.SelectedItem.Equals(element))
            {
                return "selected";
            }
            
            return string.Empty;
        }
        


        public void SearchOrders()
        {
            orderHeaders = orderHeaders
                .Where(order =>
                    (string.IsNullOrEmpty(SearchOrderHeader) || order.OrderNumber.IndexOf(SearchOrderHeader, StringComparison.OrdinalIgnoreCase) >= 0)
                    && (string.IsNullOrEmpty(SearchOrderType) || order.OrderType.ToString().IndexOf(SearchOrderType, StringComparison.OrdinalIgnoreCase) >= 0)
                    && (!FromDate.HasValue || order.OrderCreated >= FromDate.Value.Date)
                    && (!ToDate.HasValue || order.OrderCreated <= ToDate.Value.Date))
                .ToList();
            
        }
        
        public void SearchOrderLines()
        {
            if (!string.IsNullOrEmpty(SearchProductCode))
            {
                orderLines = orderLines
                    .Where(line =>
                        line.ProductCode.Contains(SearchProductCode, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

    }
}
