using CoreTest.Models;
using CoreTest.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CoreTest.Components.Pages.OrderHeaderComponent;

public class UpdateHeaderBase : ComponentBase
{
    
    [Inject] private IOrderHeaderService Service { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    public MudForm? _form;
   
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    public void Cancel() => MudDialog.Cancel();
    
    public OrderHeader CurrentOrder { get; set; } = new(); 
    [Parameter] public OrderHeader orderHeader { get; set; }  
    [Parameter] public EventCallback OnOrderHeaderUpdate { get; set; }

    protected async Task HandleSubmit()
    {
        await Service.UpdateOrderHeaderAsync(orderHeader);
        await OnOrderHeaderUpdate.InvokeAsync();
        MudDialog.Close();
   
    }
    
    public MudTimePicker? TimePicket { get; set; } = new MudTimePicker();
   
    public async Task UpdateTimeForOrderCreated()
    {
        if (CurrentOrder.OrderCreated.HasValue)
        {
            if (TimePicket.Time.HasValue)
            {
                TimeSpan selectedTime = TimePicket.Time.Value;
                DateTime orderWithTime = CurrentOrder.OrderCreated.Value.Date + selectedTime;
                CurrentOrder.OrderCreated = orderWithTime;
            }
        }
        
    }
}