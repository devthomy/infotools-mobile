using System.Windows.Input;
using Adatea.classe;
using Microsoft.Maui.Controls;

namespace Adatea
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new Adatea.ViewModels.LoginViewModel();
        }

    }

}
