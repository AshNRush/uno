## Sharing Content

The data transfer manager allows sharing content from your application using the OS sharing dialog. To check whether sharing is supported at runtime, use `IsSupported()` method:

```
if (DataTransferManager.IsSupported())
{
    // Sharing is supported    
}
```

Currently, the following types of content can be shared:

| Type of content   | Android | iOS | macOS | WASM | Tizen |
|-------------------|---------|-----|-------|------| ----- |
| Text              | ✅      | ✅ | ✅    | ✅  | ✅    |
| Uri               | ✅      | ✅ | ✅    | ✅  |✅     |
| File              | ❌      | ❌ | ❌    | ❌  |❌     |   

To set up the `DataTransferManager` use the following snippet:

```
var dataTransferManager = DataTransferManager.GetForCurrentView();
dataTransferManager.DataRequested += DataRequested;
dataTransferManager.ShowShareUI();
```

Make sure to unregister the `DataRequested` event after finished sharing.

In the `DataRequested` method, you can set the data to be shared in the `DataRequestedEventArgs`:

```
private void DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
{        
    args.Request.Data.Properties.Title = "Sharing dialog title";
    args.Request.Data.Properties.Description = "Description";

    args.Request.Data.SetText("Text to share");
    args.Request.Data.SetWebLink(new Uri("https://platform.uno/"));
}
```

If you need to prepare the data asynchronously, you can use a deferral:

```
private async void DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
{        
    var deferral = args.Request.GetDeferral();

    args.Request.Data.Properties.Title = "Sharing dialog title";
    args.Request.Data.Properties.Description = "Description";

    var text = await SomeOperationAsync();
    args.Request.Data.SetText(text);
    
    deferral.Complete();
}
```

To control the location where the sharing dialog shows up on iOS and macOS, use the `ShowShareUI(ShareUIOptions)` overload. `ShareUIOptions.SelectionRect` denotes the area the user is interacting with and will be taken into account by the OS. On iOS, you can also specify `ShareUIOptions.Theme` to make the dialog dark/light based on your app's preference. On other Uno targets, these properties have no effect.

On Tizen, the `"http://tizen.org/privilege/appmanager.launch` privilege must be declared in the application manifest to allow sharing.