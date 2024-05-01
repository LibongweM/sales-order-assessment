using System.Text.Json;
using System.Text.Json.Serialization;
using CoreTest.Components.Pages.TrackingOrders;
using CoreTest.Models;
using CoreTest.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace CoreTest.Components.Pages;

public  class TrackingOrdersBase : ComponentBase
{
	[Inject]
	protected IOrderHeaderService OrderHeaderService { get; set; }
	[Inject] private IDialogService DialogService { get; set; }
	[Inject] private IJSRuntime JsRuntime { get; set; }
	[Inject] private ISnackbar Snackbar { get; set; }
	
	protected MudDropContainer<OrderHeader> DropContainer;
	
	protected List<OrderHeader> OrderHeaders { get; private set; } = [];
	protected List<OrderHeader> DbOderHeaders { get; set; }

	protected readonly List<KanBanSections> Sections =
	[
		new KanBanSections("New Orders"),
		new KanBanSections("In Progress Orders"),
		new KanBanSections("Complete Orders"),
		new KanBanSections("Order Dispatched")
	];

	protected class KanBanSections(string name)
	{
		public string Name { get; } = name;
	}

	protected override async Task OnInitializedAsync()
	{
		DbOderHeaders = await OrderHeaderService.GetAllOrderHeadersWithoutLinesAsync();
	}

	private void AddOrder(OrderHeader orderHeader)
	{
		OrderHeaders.Add(orderHeader);
		DropContainer.Refresh();
	}

	protected async Task UpdateOrderHeader(MudItemDropInfo<OrderHeader> info)
	{
		// update order header status
		if (info.Item == null) return;
		info.Item.OrderStatus = GetOrderStatus(info.DropzoneIdentifier);
		await OrderHeaderService.UpdateOrderHeaderAsync(info.Item);
		DropContainer.Refresh();
	}

	protected void AddNewOrder()
	{
		var usableOrders = DbOderHeaders.Where(item => OrderHeaders.All(item2 => item2.Id != item.Id)).ToList();
		var onOrderSubmittedCallback = EventCallback.Factory.Create<OrderHeader>(this, AddOrder);
		var parameters = new DialogParameters
		{
			{ "OnOrderSubmitted", onOrderSubmittedCallback }, 
			{"Orders", usableOrders},
		};
		DialogService.Show<AddOrder>("Add Order", parameters);
	}
	
	protected Task DownloadAsJson()
	{
		JsonSerializerOptions jsonSerializerOptions = 
		new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
		
		var options = new JsonSerializerOptions(jsonSerializerOptions) 
		{ 
			WriteIndented = true
		};
		var json = JsonSerializer.Serialize(OrderHeaders, options);
		return FileUtil.DownloadAsJson("orders.json", json, JsRuntime);
	}
	
	protected async Task UploadFiles(IBrowserFile file)
	{
	    if (!file.ContentType.Equals("application/json", StringComparison.CurrentCultureIgnoreCase))
	    {
	        Snackbar.Add("Oooopss! Please upload a JSON file", Severity.Error);
	        return;
	    }

	    try
	    {
		    await using var stream = file.OpenReadStream();
	        var reader = new StreamReader(stream);
	        var fileContent = await reader.ReadToEndAsync();
	        
	        if(string.IsNullOrWhiteSpace(fileContent))
	        {
		        Snackbar.Add("Oooopss! The file is empty", Severity.Error);
		        return;
	        }
	        
	        fileContent = JsonSerializer.Deserialize<string>(fileContent);
	        
	        if(string.IsNullOrWhiteSpace(fileContent))
	        {
		        Snackbar.Add("Oooopss! The file is empty", Severity.Error);
		        return;
	        }

	        var orderHeaders = JsonSerializer.Deserialize<List<OrderHeader>>(fileContent);

	        if (orderHeaders == null)
	        {
		        Snackbar.Add("Oooopss! The file could not be deserialized into a list of OrderHeader objects", Severity.Error);
		        return;
	        };

	        foreach (var orderHeader in orderHeaders)
	        {
		        OrderHeaders.Add(orderHeader);
	        }
	        DropContainer.Refresh();
	        StateHasChanged();
	    }
	    catch (JsonException e)
	    {
	       Snackbar.Add($"Oooopss! {e.Message}", Severity.Error);
	    }
	}

	private static OrderStatus GetOrderStatus(string identifier)
	{
		return identifier switch
		{
			"New Orders" => OrderStatus.New,
			"In Progress Orders" => OrderStatus.InProgress,
			"Complete Orders" => OrderStatus.Complete,
			"Order Dispatched" => OrderStatus.Dispatched,
			_ => OrderStatus.New
		};
	}

	protected static string GetOrderStatusAsString(OrderStatus status)
	{
		return status switch
		{
			OrderStatus.Complete => "Complete Orders",
			OrderStatus.Dispatched => "Order Dispatched",
			OrderStatus.New => "New Orders",
			_ => "In Progress Orders"
		};
	}
}

public static class FileUtil
{
	public static async Task DownloadAsJson<T>(string filename, T data, IJSRuntime JsRuntime)
	{
		var json = JsonSerializer.Serialize(data);
		await JsRuntime.InvokeVoidAsync("downloadAsJson", filename, json);
	}
}
