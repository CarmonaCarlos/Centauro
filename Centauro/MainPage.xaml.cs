using Centauro.DataModel;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Centauro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IMobileServiceTable<Usuario> UsuarioTable = App.MobileService.GetTable<Usuario>();
        MobileServiceCollection<Usuario, Usuario> usuarios;
        MobileServiceInvalidOperationException exception = null;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            
            

            try
            {
                ProgressBarBefore();
                usuarios = await UsuarioTable
                    .Select(Usuarios => Usuarios)
                    .Where(Usuarios => Usuarios.id == textBoxUser.Text && Usuarios.contrasena == passwordBox.Password.ToString())
                    .ToCollectionAsync();
                ProgressBarAfter();                
                if(usuarios.Count >= 1)
                {
                    var rsUsuario = usuarios.First();
                    
                    if(rsUsuario.Tipo == "Tecnico")
                    {
                        Frame.Navigate(typeof(Administrador), "Tecnico");
                    }
                    else
                    {
                        Frame.Navigate(typeof(Administrador), "Administrador");
                    }
                }
                else
                {
                    MessageDialog mensaje = new MessageDialog("Usuario o contraseña incorrectos.", "Credenciales invalidas");
                    await mensaje.ShowAsync();
                }



            }
            catch (MobileServiceInvalidOperationException ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error!").ShowAsync();
            }


        }

        public void ProgressBarBefore()
        {
            LoadingBar.IsEnabled = true;
            LoadingBar.Visibility = Visibility.Visible;
            button.IsEnabled = false;
            textBoxUser.IsEnabled = false;
            passwordBox.IsEnabled = false;
        }

        public void ProgressBarAfter()
        {
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
            button.IsEnabled = true;
            textBoxUser.IsEnabled = true;
            passwordBox.IsEnabled = true;
        }

        private async void passwordBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                try
                {
                    ProgressBarBefore();
                    usuarios = await UsuarioTable
                        .Select(Usuarios => Usuarios)
                        .Where(Usuarios => Usuarios.id == textBoxUser.Text && Usuarios.contrasena == passwordBox.Password.ToString())
                        .ToCollectionAsync();
                    ProgressBarAfter();
                    if (usuarios.Count >= 1)
                    {
                        var rsUsuario = usuarios.First();

                        if (rsUsuario.Tipo == "Tecnico")
                        {
                            Frame.Navigate(typeof(Administrador), "Tecnico");
                        }
                        else
                        {
                            Frame.Navigate(typeof(Administrador), "Administrador");
                        }
                    }
                    else
                    {
                        MessageDialog mensaje = new MessageDialog("Usuario o contraseña incorrectos.", "Credenciales invalidas");
                        await mensaje.ShowAsync();
                    }



                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    exception = ex;
                }

                if (exception != null)
                {
                    await new MessageDialog(exception.Message, "Error!").ShowAsync();
                }

            }
        }
    }
}
