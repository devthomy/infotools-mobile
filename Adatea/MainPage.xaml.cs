using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Adatea.classe;
using Adatea.ViewModels;
using System.Diagnostics; // Ajout du namespace System.Diagnostics pour utiliser Debug

namespace Adatea
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private ObservableCollection<Appointment> _appointments = new ObservableCollection<Appointment>();

        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set
            {
                _appointments = value;
                OnPropertyChanged();
            }
        }

        private Appointment _selectedAppointment;
        public Appointment SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                if (_selectedAppointment != value)
                {
                    _selectedAppointment = value;
                    OnPropertyChanged();
                }
            }
        }


        public ICommand DeleteAppointmentCommand { get; private set; }
        public ICommand ModifyAppointmentCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            MessagingCenter.Subscribe<LoginViewModel>(this, "UserLoggedOut", (sender) => {
                ClearAppointments();
            });
            MessagingCenter.Subscribe<LoginViewModel>(this, "UserLoggedIn", (sender) => {
                LoadAppointments();
            });

            MessagingCenter.Subscribe<AddAppointment, Appointment>(this, "AppointmentAdded", async (sender, appointment) =>
            {
                await LoadAppointments();
            });

            ModifyAppointmentCommand = new Command<Appointment>((appointment) =>
            {
                SelectedAppointment = appointment;
                EditAppointmentSection.IsVisible = true;
            });


            DeleteAppointmentCommand = new Command<Appointment>(async (Appointment appointment) =>
            {
                bool isUserSure = await DisplayAlert("Confirmation", "Voulez-vous vraiment supprimer ce rendez-vous ?", "Oui", "Non");
                if (isUserSure)
                {
                    var success = await new Bdd().DeleteAppointmentAsync(appointment.ID_Rdv);
                    if (success)
                    {
                        Appointments.Remove(appointment);
                        await DisplayAlert("Succès", "Rendez-vous supprimé avec succès.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Impossible de supprimer le rendez-vous.", "OK");
                    }
                }
            });
        }

        private void ClearAppointments()
        {
            Appointments.Clear();
        }

        private int GetLoggedInCommercialId()
        {
            var id = SessionManager.CurrentUserId;
            Debug.WriteLine($"Current Commercial ID: {id}"); 
            return id;
        }

        private async Task LoadAppointments()
        {
            Debug.WriteLine("Starting to load appointments...");

            ClearAppointments();

            int commercialId = GetLoggedInCommercialId();
            Debug.WriteLine(commercialId);

            if (commercialId <= 0)
            {
                Debug.WriteLine("Invalid commercial ID, stopping load.");
                return;
            }

            try
            {
                // Charger les rendez-vous directement depuis Bdd
                var appointmentsFromDatabase = await new Bdd().LoadRendezVousAsync(commercialId);
                foreach (var appointment in appointmentsFromDatabase)
                {
                    // Vérifier si l'appointment est null ou si certaines propriétés sont null avant de l'ajouter à la collection
                    if (appointment != null && appointment.ID_Client != null)
                    {
                        Appointments.Add(appointment);
                    }
                }

                Debug.WriteLine($"Loaded {appointmentsFromDatabase.Count} appointments.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while loading appointments: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        #region MODIFIER

        private void OnModifyClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Appointment appointment)
            {
                SelectedAppointment = appointment;
                EditAppointmentSection.IsVisible = true;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (SelectedAppointment != null)
            {
                bool success = await new Bdd().UpdateAppointmentAsync(SelectedAppointment);
                if (success)
                {
                    await DisplayAlert("Succès", "Le rendez-vous a été mis à jour.", "OK");
                    EditAppointmentSection.IsVisible = false;
                    await LoadAppointments();
                }
                else
                {
                    await DisplayAlert("Erreur", "La mise à jour a échoué.", "OK");
                }
            }
        }

        void OnCloseClicked(object sender, EventArgs e)
        {
            EditAppointmentSection.IsVisible = false;
        }

        #endregion



    }
}
