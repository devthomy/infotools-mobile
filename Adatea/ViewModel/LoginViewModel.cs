using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Adatea.classe;
using System.Threading.Tasks;

namespace Adatea.ViewModels
{
    // Définit le ViewModel pour la gestion de la connexion.
    public class LoginViewModel : INotifyPropertyChanged
    {
        // Déclaration des variables privées pour le ViewModel.
        private Bdd _database = new Bdd(); // Instance de la base de données.
        private bool _isLoggedIn = false; // Indique si un utilisateur est connecté.
        private string _email; // Stocke l'email de l'utilisateur.
        private string _password; // Stocke le mot de passe de l'utilisateur.
        private Commercial _currentUser; // Stocke les informations sur l'utilisateur actuellement connecté.

        // Implémentation de l'interface INotifyPropertyChanged pour notifier les changements de propriété.
        public event PropertyChangedEventHandler PropertyChanged;

        // Propriété Email avec notification de changement.
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        // Propriété Password avec notification de changement.
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        // Propriété CurrentUser avec notification de changement.
        public Commercial CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UserDisplayName));
            }
        }

        // Propriété calculée pour afficher le nom complet de l'utilisateur.
        public string UserDisplayName => CurrentUser != null ? $"{CurrentUser.Firstname} {CurrentUser.Lastname}" : string.Empty;

        // Propriété IsLoggedIn avec notification de changement. Indique si l'utilisateur est connecté.
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoggedIn));
            }
        }

        // Propriété calculée basée sur IsLoggedIn pour indiquer si l'utilisateur n'est pas connecté.
        public bool IsNotLoggedIn => !IsLoggedIn;

        // Commandes pour gérer les actions de connexion et déconnexion.
        public ICommand LoginCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        // Constructeur qui initialise les commandes.
        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await ExecuteLoginCommand());
            LogoutCommand = new Command(ExecuteLogoutCommand);
        }

        // Méthode asynchrone exécutée lors de la connexion. Vérifie les identifiants et met à jour l'état de connexion.
        private async Task ExecuteLoginCommand()
        {
            var commercial = await _database.GetCommercialByEmailAndPasswordAsync(Email, Password);
            if (commercial != null)
            {
                // Utilisateur trouvé, mise à jour de l'état de connexion et envoi d'un message de connexion réussie.
                CurrentUser = commercial;
                IsLoggedIn = true;
                SessionManager.CurrentUserId = commercial.ID_Commercial;
                MessagingCenter.Send(this, "UserLoggedIn");
            }
            else
            {
                // Aucun utilisateur trouvé avec les identifiants fournis, afficher une alerte.
                await Application.Current.MainPage.DisplayAlert("Erreur", "Nom d'utilisateur ou mot de passe incorrect.", "OK");
                // Réinitialisation de l'état de connexion
                SessionManager.CurrentUserId = -1;
            }
        }


        // Méthode exécutée lors de la déconnexion. Réinitialise l'état de connexion et notifie les autres parties de l'application.
        private void ExecuteLogoutCommand()
        {
            IsLoggedIn = false;
            CurrentUser = null; // Réinitialiser l'utilisateur actuel.

            // Réinitialiser l'ID de l'utilisateur dans SessionManager.
            SessionManager.CurrentUserId = -1;

            // Envoyer une notification de déconnexion.
            MessagingCenter.Send(this, "UserLoggedOut");
        }

        // Méthode pour notifier les changements de propriété.
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
