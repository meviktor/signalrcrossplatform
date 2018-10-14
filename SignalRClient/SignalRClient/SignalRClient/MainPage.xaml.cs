using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SignalRClient
{
    public partial class MainPage : ContentPage
    {
        MessageController controller;

        private ObservableCollection<Person> deserializedPersons;
        private ObservableCollection<string> rawPesonJsons;
        private ObservableCollection<string> connectionLogs;

        public MainPage()
        {
            InitializeComponent();

            genderPicker.ItemsSource = new List<string> { "Female", "Male", "Undetermined" };
            genderPicker.SelectedIndex = 0;
            ageLabel.Text = ageStepper.Value.ToString();

            marriedPicker.ItemsSource = new List<string> { "is married", "is not married" };
            genderPicker.SelectedIndex = 0;

            deserializedPersons = new ObservableCollection<Person>();
            rawPesonJsons = new ObservableCollection<string>();
            connectionLogs = new ObservableCollection<string>();

            rawJsonPersonListView.ItemsSource = rawPesonJsons;
            deserializedPersonListView.ItemsSource = deserializedPersons;
            connectionLogsListView.ItemsSource = connectionLogs;

            controller = MessageController.Instance();

            clientIdLabel.Text = controller.clientID.ToString();

            controller.connectionNewsReceived += DisplayReceivedConnectionNews;
            controller.personDataRecieved += DisplayNewPersonDataInApp;
        }

        private void ageStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            ageLabel.Text = e.NewValue.ToString();
        }

        private void sendButton_Clicked(object sender, EventArgs e)
        {
            if (IsValidInput())
            {
                Person p = new Person
                {
                    FirstName = firstNameEntry.Text,
                    LastName = lastNameEntry.Text,
                    Age = (int)ageStepper.Value,
                    Gender = (string)genderPicker.SelectedItem,
                    IsMarried = (string) genderPicker.SelectedItem == "is married" ? true : false,
                    SentFromClient = controller.clientID
                };
                string personData = JsonConvert.SerializeObject(p);
                controller.SendNewPerson(personData);
            }
        }

        private bool IsValidInput()
        {
            return !string.IsNullOrWhiteSpace(firstNameEntry.Text) && !string.IsNullOrWhiteSpace(lastNameEntry.Text);
        }

        private void DisplayReceivedConnectionNews(string message, bool isNewConnection)
        {
            connectionLogs.Insert(0, message);
        }

        private void DisplayNewPersonDataInApp(string personData, int senderClientID)
        {
            rawPesonJsons.Insert(0, personData);
            Person newPerson = JsonConvert.DeserializeObject<Person>(personData);
            deserializedPersons.Insert(0, newPerson);
            connectionLogs.Insert(0, string.Format("New person data received from client with the id: {0}", senderClientID));
        }
    }
}
