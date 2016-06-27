using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Centauro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Administrador : Page
    {
        

        public Administrador()
        {
            this.InitializeComponent();
            //Navigate to the Default Page Temperatura Refrigerador
            ContentFrame.Navigate(typeof(TemperaturaRefrigerador));
            //Select the element 
            BtnTempRefri.IsChecked = true;
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

                        
            

            

           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var usuario = (string)e.Parameter;
            if (usuario == "Tecnico")
            {
                BtnTecnico.Visibility = Visibility.Collapsed;
            }
            else
            {
                BtnTecnico.Visibility = Visibility.Visible;
            }

        }



        private void HamburguerButton_Click(object sender, RoutedEventArgs e)
        {
            Split.IsPaneOpen = !Split.IsPaneOpen;
            HamburguerButton.IsChecked = false;
        }

        



        private void BtnTempDesh_Click(object sender, RoutedEventArgs e)
        {
            //Navigate to the Default Page Temperatura Refrigerador
            ContentFrame.Navigate(typeof(TemperaturaDeshidratadora));
            //Select the element 
            BtnTempDesh.IsChecked = true;
        }

        private void BtnHistFallas_Click(object sender, RoutedEventArgs e)
        {
            //Navigate to the Default Page Temperatura Refrigerador
            ContentFrame.Navigate(typeof(HistorialFallas));
            //Select the element 
            BtnHistFallas.IsChecked = true;
        }

        /*private void BtnLoteCarner_Click(object sender, RoutedEventArgs e)
        {
            //Navigate to the Default Page Temperatura Refrigerador
            ContentFrame.Navigate(typeof(LoteCarne));
            //Select the element 
            BtnLoteCarner.IsChecked = true;
        }*/

        /*private void BtnProveedor_Click(object sender, RoutedEventArgs e)
        {
            //Navigate to the Default Page Temperatura Refrigerador
            ContentFrame.Navigate(typeof(Proveedor));
            //Select the element 
            BtnProveedor.IsChecked = true;
        }*/

        private void BtnTecnico_Click(object sender, RoutedEventArgs e)
        {
            //Navigate to the Default Page Temperatura Refrigerador
            ContentFrame.Navigate(typeof(Tecnico));
            //Select the element 
            BtnTecnico.IsChecked = true;
        }

        private void BtnTempRefri_Click(object sender, RoutedEventArgs e)
        {
            //Navigate to the Default Page Temperatura Refrigerador
            ContentFrame.Navigate(typeof(TemperaturaRefrigerador));
            //Select the element 
            BtnTempRefri.IsChecked = true;
        }

        private async void logOut_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("¿Esta seguro de cerrar sesión?");
            dialog.Title = "¿Esta seguro?";
            dialog.Commands.Add(new UICommand { Label = "Si", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "No", Id = 1 });
            var res = await dialog.ShowAsync();

            if ((int)res.Id == 0)
            {
                this.Frame.Navigate(typeof(MainPage));
            }

        }

        /*private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }

            BtnGoBack.IsChecked = false;
        }*/
    }

   
}
