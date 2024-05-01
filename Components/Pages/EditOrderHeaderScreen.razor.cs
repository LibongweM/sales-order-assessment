using CoreTest.Models;
using CoreTest.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CoreTest.Components.Pages;

public class EditOrderHeaderScreenBase : ComponentBase
{
    [Inject] private IOrderHeaderService Service { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; }
    
    [Parameter]
    public string? Id { get; set; }
    
    [Parameter]
    public IEnumerable<OrderType> _OrderTypes { get; set; }
    
    [Parameter]
    public IEnumerable<OrderStatus> _OrderStatus { get; set; }
    
    public MudForm? _form;
    
    // public OrderType enumOrderType { get; set; }
    // public OrderStatus enumOrderStatus { get; set; }
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
        if (Id is not null)
        {
            await Service.UpdateOrderHeaderAsync(CurrentOrder);
            Navigation.NavigateTo("/orders");
            
        }
        else
        {
            await Service.AddOrderHeaderAsync(CurrentOrder);
            Navigation.NavigateTo("/orders");
            
        }
    }
    
    
    
    
    public MudTimePicker? TimePicket { get; set; } = new MudTimePicker();

    public DateTime? DateS { get; set; }
    
 

    public TimeSpan? TimeS { get; set; }

    public void OpenTimer()
    {

        TimePicket.Open();
    }

    public void AddTimeToDatetime()
    {
        if (TimeS != null)
        {
            TimeSpan nonNullableTimeSpan = TimeS ?? TimeSpan.Zero;
            DateS = DateS?.Add(nonNullableTimeSpan);
        }
    }

    public TimeSpan SelectedTime { get; set; }

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
            else
            {
                // Handle the case where time is not selected
                // You can set a default time or handle it based on your requirements
            }
        }
        else
        {
            // Handle the case where CurrentOrder.OrderCreated is null
            // You can set a default date or handle it based on your requirements
        }
    }
   

    
}