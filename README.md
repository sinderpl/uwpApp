#  Appointment Application

###Introduction
I have created a simple CRUD based Appointment making application in
Windows Universal Platform. It features a MVVM based architecture and utilises a lot of the newest features of the platform.

###MVVM
The app features the following components: #### MVVVM
1. MainPage.xaml - As the view
2. MainPage.xaml.cs - As the view model
3. DataModel/ TodoItem.cs - As the model

###Binding
The application manipulates the Windows 10 built in Calendar feature by offering the ability to 
create custom appointments with values input by the user #### (X:Bind) feature. < /br>

The code below binds the Name to the button allowing for the button to be referenced in the ViewModel,
the Click side calls the corresponding method when the button is pressed.
	 <Button x:Name="ButtonSearch" Click="ButtonSearch_Click" />
< /br>
Here is another example of Binding, this time a value in one of the models is binded to the textbox in order to show it onscreen.
	<TextBlock Name="ItemText" Text="{Binding dataString}
< /br>


