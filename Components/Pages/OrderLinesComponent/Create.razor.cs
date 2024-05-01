using CoreTest.Models;
using CoreTest.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CoreTest.Components.Pages.OrderLinesComponent;

public  class CreateBase : ComponentBase
{
    [Inject] private IOrderLineService Service { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    
    [Parameter]
    public string? Id { get; set; }
    
    [Parameter]
    public IEnumerable<ProductType> _ProductTypes { get; set; }
    
  [Parameter] public EventCallback OnOrderSubmitted { get; set; }
    
    public MudForm? _form;
   
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    
    public void Cancel() => MudDialog.Cancel();
    
    
    public OrderLine CurrentOrder { get; set; } = new();

    protected async Task HandleSubmit()
    {
        if (Id is not null)
        {
            await Service.CreateOrderLineAsync(Id, CurrentOrder);
            await OnOrderSubmitted.InvokeAsync();
            MudDialog.Close();
            Snackbar.Add("Order line was added successfully.", Severity.Success);
        }
    }
    

}