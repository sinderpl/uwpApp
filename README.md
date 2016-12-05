#  Appointment Application

###Introduction
I have created a simple CRUD based Appointment making application in
Windows Universal Platform. It features a MVVM based architecture and utilises a lot of the newest features of the platform.

###MVVM
The app features the following components:
1. MainPage.xaml - As the view
2. MainPage.xaml.cs - As the view model
3. DataModel TodoItem.cs - As the model

###Binding
The application manipulates the Windows 10 built in Calendar feature by offering the ability to 
create custom appointments with values input by the user **(X:Bind)** feature.

The code below binds the Name to the button allowing for the button to be referenced in the ViewModel,
the Click side calls the corresponding method when the button is pressed.
```XAML
	 <Button x:Name="ButtonSearch" Click="ButtonSearch_Click" />
```
Here is another example of Binding, this time a value in one of the models is binded to the textbox in order to show it onscreen.
```XAML	
	<TextBlock Name="ItemText" Text="{Binding dataString}>
```

###Conditional Operators
The following is a simple print statement that represents a use of the **Conditional Operators** feature.
```C#
	System.Diagnostics.Debug.Write((removed) ?  "success" : "Failed to remove");
```

###Calendar
I have implemented the calendar feature  by allowing the calendar to be opened with a single user click.
The application is synchronized with the calendar allowing for the creation and deletion of appointments
in the actual user calendar. In the following code I insert an appointment into the calendar and await the return of an ID value.
```C#
String appointmentId = await Windows.ApplicationModel.Appointments.AppointmentManager.ShowAddAppointmentAsync(
                                   appointment, rect, Windows.UI.Popups.Placement.Default);
```

###Async & Await
Async and Await are used widely throught the application to allow for a smoother user experience.
They allow for the methods to be executed on another thread, keeping the interface runnning while the tasks execute 
in the background.
In the following code I also show the Search by Date feature I implemented which allows for the user to query for all 
appointments on a given day.
```C#
private async Task SearchItemsByDate()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
            //Convert date to a string value
            String appDate = "";
            if (AppointmentDate.Date.HasValue)
            {
                appDate = AppointmentDate.Date.Value.ToString("MM dd yyyy");
            }
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems.
                items = await todoTable
                    .Where(todoItem => todoItem.appointmentDate == appDate)
                    .ToCollectionAsync();
            }
```

###Azure
The application is synced with the Azure cloud services, saving all the appointments in the cloud.
It also pulls them down whenever it is needed, mainly during the application initiation.
```C#
        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();
```



