using System;
using Adatea.classe;

namespace Adatea
{
    public partial class AddAppointment : ContentPage
    {
        public AddAppointment()
        {
            InitializeComponent();
        }
        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            var bdd = new Bdd();

            int idCommercial = SessionManager.CurrentUserId;

            if (idCommercial > 0) 
            {
                var newAppointment = new Appointment
                {
                    ID_Client = int.TryParse(IdClient.Text, out int idClient) ? idClient : 0, 
                    ID_Commercial = idCommercial,
                    Date_Rdv = DateEntry.Date,
                    Time_Rdv = TimeEntry.Time,
                    Location = LocationEntry.Text
                };

                bool success = await bdd.AddAppointmentAsync(newAppointment);

                if (success)
                {
                    MessagingCenter.Send<AddAppointment, Appointment>(this, "AppointmentAdded", newAppointment);
                    await DisplayAlert("Succès", "Le rendez-vous a été ajouté avec succès.", "OK");
                }

                else
                {
                    await DisplayAlert("Erreur", "Une erreur est survenue lors de l'ajout du rendez-vous.", "OK");
                }
            }
            else
            {
                // Si aucun utilisateur n'est connecté ou si l'ID n'est pas valide, afficher une erreur ou rediriger vers la page de connexion.
                await DisplayAlert("Erreur", "Aucun commercial connecté. Veuillez vous connecter pour ajouter un rendez-vous.", "OK");
            }
        }

    }
}
