using CoreTest.Models;
using CoreTest.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CoreTest.Components.Pages.TrackingOrders;

public class AddOrderBase : ComponentBase
{
    [Inject] private IOrderHeaderService Service { get; set; } = default!;
    [Parameter] public EventCallback<OrderHeader> OnOrderSubmitted { get; set; }
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    
    public void Cancel() => MudDialog.Cancel();

    [Parameter] public List<OrderHeader> Orders { get; set; } = [];
    
    public string? SelectedOrderNumber { get; set; }
    protected async Task HandleSubmit()
    {
        if (string.IsNullOrEmpty(SelectedOrderNumber)) return;

        var selectedOrder = Orders.First(o => o.OrderNumber.Equals(SelectedOrderNumber));
        
        MudDialog.Close(DialogResult.Ok(true));
        
        await OnOrderSubmitted.InvokeAsync(selectedOrder);
    }
}