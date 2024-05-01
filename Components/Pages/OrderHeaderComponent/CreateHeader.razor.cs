using CoreTest.Models;
using CoreTest.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CoreTest.Components.Pages.OrderHeaderComponent;

public class CreateBase : ComponentBase
{
    [Inject] private IOrderHeaderService Service { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Parameter]
    public string? Id { get; set; }
    
    [Parameter]
    public IEnumerable<OrderType> _OrderTypes { get; set; }
    
    [Parameter]
    public IEnumerable<OrderStatus> _OrderStatus { get; set; }
    
    [Parameter] public EventCallback OnOrderHeaderSubmitted { get; set; }
    
    public MudForm? _form;
    
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    
    public void Submit() => MudDialog.Close(DialogResult.Ok(true));
    public void Cancel() => MudDialog.Cancel();
    
    
    public OrderHeader CurrentOrder { get; set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var orderHeader = await Service.GetOrderHeaderIdAsync(Id);
            if (Id != null)
            {
                CurrentOrder = orderHeader;
            }
        }
    }

    protected async Task HandleSubmit()
    {
            await Service.AddOrderHeaderAsync(CurrentOrder);
            await OnOrderHeaderSubmitted.InvokeAsync();
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