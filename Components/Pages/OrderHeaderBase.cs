using CoreTest.Components.Pages.OrderHeaderComponent;
using CoreTest.Components.Pages.OrderLinesComponent;
using CoreTest.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CoreTest.Components.Pages;

public partial class OrderHeaderBase
{
    #region OrderHeaderCrud

    public void AddNewOrder()
    {
        var onOrderHeaderSubmittedCallback = EventCallback.Factory.Create(this, OnOrderHeaderSubmitted);
        var parameters = new DialogParameters<CreateHeader>();
        parameters.Add("OnOrderHeaderSubmitted", onOrderHeaderSubmittedCallback);

        var options = new DialogOptions { CloseOnEscapeKey = true };
        DialogService.Show<CreateHeader>("Add Order", parameters, options);
    }

    protected void UpdateOrderHeader(OrderHeader header)
    {
        var onOrderHeaderUpdateCallback = EventCallback.Factory.Create(this, OnOrderHeaderUpdate);
        var parameters = new DialogParameters<UpdateHeader>();
        parameters.Add(x => x.orderHeader, header);
        parameters.Add("OnOrderHeaderUpdate", onOrderHeaderUpdateCallback);

        var options = new DialogOptions { CloseOnEscapeKey = true };
        DialogService.Show<UpdateHeader>("Update Order Header", parameters, options);
    }
    
    protected async Task DeleteOrderHeader(string id)
    {
        var result = await DialogService.ShowMessageBox(
            "Confirm",
            "Are you sure you want to delete this order Header?",
            yesText: "Yes", cancelText: "Cancel");

        if (result == true)
        {
            await Service.DeleteOrderHeaderAsync(id) ;
            OrderHeaders = await Service.GetAllOrderHeadersAsync() ?? [];
            Snackbar.Add("Order Header was deleted successfully.", Severity.Success);
        }
    }

    #endregion

    #region OrderLineLogic

    protected void AddNewOrderLine()
    {
        var onOrderSubmittedCallback = EventCallback.Factory.Create(this, OnOrderSubmitted);
        var parameters = new DialogParameters<Create>();
        parameters.Add(x=>x.Id, HeaderId);
        parameters.Add("OnOrderSubmitted", onOrderSubmittedCallback);
            
        var options = new DialogOptions { CloseOnEscapeKey = true };
        DialogService.Show<Create>("Add Order Line", parameters, options);
    }

    protected void UpdateOrderLine(OrderLine line) 
    {
        var onOrderUpdateCallback = EventCallback.Factory.Create(this, OnOrderUpdate);
        var parameters = new DialogParameters<Update>();
        parameters.Add(x=>x.orderLine, line);
        parameters.Add("OnOrderUpdate", onOrderUpdateCallback);

        var options = new DialogOptions { CloseOnEscapeKey = true };
        DialogService.Show<Update>("Update Order Line", parameters, options);
            
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

    #endregion
}