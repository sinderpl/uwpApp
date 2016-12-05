/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=717898
 */
//#define OFFLINE_SYNC_ENABLED

using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Core;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;  // offline sync
using Microsoft.WindowsAzure.MobileServices.Sync;         // offline sync
#endif

namespace CalendarApplication
{


    public sealed partial class MainPage : Page
    {
        
        private MobileServiceCollection<TodoItem, TodoItem> items;
        private string appointmentIdGlobal = "";
#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<TodoItem> todoTable = App.MobileService.GetSyncTable<TodoItem>(); // offline sync
#else
        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();
#endif

        public MainPage()
        {
            this.InitializeComponent();
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
#if OFFLINE_SYNC_ENABLED
            await InitLocalStoreAsync(); // offline sync
#endif
            ButtonRefresh_Click(this, null);
        }

        private async Task InsertTodoItem(TodoItem todoItem)
        {
            // This code inserts a new TodoItem into the database. After the operation completes
            // and the mobile app backend has assigned an id, the item is added to the CollectionView.
            //await App.MobileService.GetTable<TodoItem>().InsertAsync(todoItem);
            //var td = new TodoItem();
            await todoTable.InsertAsync(todoItem);
            items.Add(todoItem);

#if OFFLINE_SYNC_ENABLED
            await App.MobileService.SyncContext.PushAsync(); // offline sync
#endif
        }
        private async Task RefreshTodoItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems.
                items = await todoTable
                    .Where(todoItem => todoItem.Complete == false)
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                //ListItems.ItemsSource = items;
                this.AddApointment.IsEnabled = true;
            }
        }

        private async Task UpdateCheckedTodoItem(TodoItem item)
        {
            // This code takes a freshly completed TodoItem and updates the database.
			// After the MobileService client responds, the item is removed from the list.
            await todoTable.UpdateAsync(item);
            items.Remove(item);
            //ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);

#if OFFLINE_SYNC_ENABLED
            await App.MobileService.SyncContext.PushAsync(); // offline sync
#endif
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            ButtonRefresh.IsEnabled = false;

#if OFFLINE_SYNC_ENABLED
            await SyncAsync(); // offline sync
#endif
            await RefreshTodoItems();

            ButtonRefresh.IsEnabled = true;
        }

        // Code adapted from https://msdn.microsoft.com/en-us/windows/uwp/contacts-and-calendar/managing-appointments
        private async Task Create_Appointment(string time, string dateVar,string name ,TimeSpan start, TimeSpan end, Rect rect)
        {
            //function vars
            TimeSpan appDur = end - start;
            var appD = appDur.TotalMinutes;

            //Convert the date and time back into DateTime objects
            var startTime = DateTime.ParseExact(time, "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

            var date = DateTime.ParseExact(dateVar, "MM dd yyyy", new System.Globalization.CultureInfo("id-ID"), System.Globalization.DateTimeStyles.None);
            var timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            
            //appointment var intializing and setting    
            var appointment = new Windows.ApplicationModel.Appointments.Appointment();
            appointment.Subject = name;

            //Adapted from http://stackoverflow.com/questions/18919530/convert-string-to-time
            appointment.StartTime = new DateTimeOffset(date.Year, date.Month, date.Day, startTime.Hour, startTime.Minute, 0 ,timeZoneOffset);
            System.Diagnostics.Debug.Write(appointment.StartTime);
            appointment.Duration = TimeSpan.FromMinutes(appD);
            appointment.Reminder = TimeSpan.FromMinutes(15);


            // ShowAddAppointmentAsync returns an appointment id if the appointment given was added to the user's calendar.
            // This value should be stored in app data and roamed so that the appointment can be replaced or removed in the future.
            // An empty string return value indicates that the user canceled the operation before the appointment was added.

            //Extra measure to ensure the blogID is returned correctly
            MessageDialog dialog = new MessageDialog("If you press accept, please do not close the empty window. \nIt will close itself once the operation is complete.");

            //Add the appointment to the user calendar and ensure the ID is returned
            await dialog.ShowAsync();
            String appointmentId = await Windows.ApplicationModel.Appointments.AppointmentManager.ShowAddAppointmentAsync(
                                   appointment, rect, Windows.UI.Popups.Placement.Default);
            //Might be redundant
            if (appointmentId != String.Empty)
            {
                appointmentIdGlobal = appointmentId;
            }
        }

        //Adapted from https://social.msdn.microsoft.com/Forums/en-US/f5feb8b2-0555-403e-a1f9-967ccf970c7a/how-can-i-transform-rectangle-to-rect-and-vice-versa?forum=winappswithcsharp
        public static Rect GetElementRect(FrameworkElement element)
        {
            //This is necessary to open the pop up to add the appointment
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        //Adapted from https://blogs.windows.com/buildingapps/2014/03/13/build-apps-that-connect-with-people-and-calendar-part-2-appointments/#lSzUlU8pZfca36uS.97
        //Show the calendar
        private async void Show_Click(object sender, RoutedEventArgs e)
        { 
            var dateToShow = new DateTimeOffset(2014, 2, 25, 18, 32, 0, 0,
                    TimeSpan.FromHours(-8));
            DateTime thisDay = DateTime.Today;
            var duration = TimeSpan.FromHours(1);
            await Windows.ApplicationModel.Appointments.AppointmentManager.ShowTimeFrameAsync(
                thisDay, duration);
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            //Convert date to a string value
            String appDate = "";
            if (AppointmentDate.Date.HasValue)
            {
                appDate = AppointmentDate.Date.Value.ToString("MM dd yyyy");
            }
            var rect = GetElementRect(sender as FrameworkElement);

            //Create appointment locally, await because we need to wait for the appointment id to return
            await Create_Appointment(AppointmentTimeStart.Time.ToString(), appDate, textBox.Text, AppointmentTimeStart.Time, AppointmentTimeEnd.Time, rect);

            //Create a new todo item
            var todoItem = new TodoItem { Text = textBox.Text, appointmentDate = appDate, appointmentTime = AppointmentTimeStart.Time.ToString(), appointmentTimeEnd = AppointmentTimeEnd.Time.ToString(), appointmentID = appointmentIdGlobal}; 
            
            //Clear necessary variables
            textBox.Text = "";
            appointmentIdGlobal = "";
            //Insert into the azure db
            await InsertTodoItem(todoItem);
        }

        private async void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            await UpdateCheckedTodoItem(item);
        }

        private void TextInput_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter) {
                AddApointment.Focus(FocusState.Programmatic);
            }
        }

        #region Offline sync
#if OFFLINE_SYNC_ENABLED
        private async Task InitLocalStoreAsync()
        {
           if (!App.MobileService.SyncContext.IsInitialized)
           {
               var store = new MobileServiceSQLiteStore("localstore.db");
               store.DefineTable<TodoItem>();
               await App.MobileService.SyncContext.InitializeAsync(store);
           }

           await SyncAsync();
        }

        private async Task SyncAsync()
        {
           await App.MobileService.SyncContext.PushAsync();
           await todoTable.PullAsync("todoItems", todoTable.CreateQuery());
        }
#endif
        #endregion**/
    }
}
