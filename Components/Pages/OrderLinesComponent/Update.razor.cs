using CoreTest.Models;
using CoreTest.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CoreTest.Components.Pages.OrderLinesComponent;

public  class UpdateBase : ComponentBase
{
    
    [Inject] private IOrderLineService Service { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; }
  
    [Inject] private ISnackbar Snackbar { get; set; }
    public MudForm? _form;
   
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    
    public void Cancel() => MudDialog.Cancel();
    
    [Parameter] public OrderLine orderLine { get; set; }  
    [Parameter] public EventCallback OnOrderUpdate { get; set; }
    protected async Task HandleSubmit()
    {
            await Service.UpdateOrderLineAsync(orderLine);
            await OnOrderUpdate.InvokeAsync();
            MudDialog.Close();
    
    }
}