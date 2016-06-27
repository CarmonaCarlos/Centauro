using Centauro.DataModel;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Centauro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 



    public sealed partial class HistorialFallas : Page
    {
        IMobileServiceTable<Refrigerador> RefriTable = App.MobileService.GetTable<Refrigerador>();
        MobileServiceCollection<Refrigerador, Refrigerador> refrigeradores;

        IMobileServiceTable<Deshidratadora> DeshTable = App.MobileService.GetTable<Deshidratadora>();
        MobileServiceCollection<Deshidratadora, Deshidratadora> desidratadoras;

        IMobileServiceTable<Falla_Refri> FallaTableRefri = App.MobileService.GetTable<Falla_Refri>();
        MobileServiceCollection<Falla_Refri, Falla_Refri> fallaRefri;

        IMobileServiceTable<Falla_Desh> FallaTableDesh = App.MobileService.GetTable<Falla_Desh>();
        MobileServiceCollection<Falla_Desh, Falla_Desh> fallaDesh;

        IMobileServiceTable<Usuario> UsuarioTable = App.MobileService.GetTable<Usuario>();
        MobileServiceCollection<Usuario, Usuario> usuarios;



        MobileServiceInvalidOperationException exception = null;



        public HistorialFallas()
        {
            this.InitializeComponent();
            comboBoxEstado.SelectedIndex = 0;
            comboBoxDispositivo.SelectedIndex = 0;
            disableChangeStatus();
        }

        public async void LoadComboBox()
        {
            //Cargar el combobox idDipositivo 


            object selectedItemEstado = comboBoxEstado.SelectedValue.ToString();
            object selectedItemDispositivo = comboBoxDispositivo.SelectedValue.ToString();
            textBlock2.Text = "Refrigerador:";
            if (comboBoxDispositivo.SelectedIndex == 0) //comboBoxDispositivo == 0 -> Refrigerador
            {
                comboBoxIdDispositivo.DisplayMemberPath = "idRefrigerador";
                comboBoxIdDispositivo.SelectedValuePath = "idRefrigerador";

                try
                {
                    ProgressBarBefore();
                    refrigeradores = await RefriTable
                        .Select(Refri => Refri)
                        .ToCollectionAsync();
                    comboBoxIdDispositivo.ItemsSource = refrigeradores;
                    ProgressBarAfter();



                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    exception = ex;
                }
                if (exception != null)
                {
                    await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                }

            }
            else
            {
                textBlock2.Text = "Deshidratadora:";
                comboBoxIdDispositivo.DisplayMemberPath = "idDeshidratadora";
                comboBoxIdDispositivo.SelectedValuePath = "idDeshidratadora";

                try
                {
                    ProgressBarBefore();
                    desidratadoras = await DeshTable
                        .Select(Refri => Refri)
                        .ToCollectionAsync();
                    comboBoxIdDispositivo.ItemsSource = desidratadoras;
                    ProgressBarAfter();
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    exception = ex;
                }

                if (exception != null)
                {
                    await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                }

            }

            if (comboBoxIdDispositivo.SelectedIndex != 0)
            {
                comboBoxIdDispositivo.SelectedIndex = 0;
            }



        }


        public void ProgressBarBefore()
        {
            //disableElementTemperaturaRefri();
            LoadingBar.IsEnabled = true;
            LoadingBar.Visibility = Visibility.Visible;
        }

        public void ProgressBarAfter()
        {
            //EnableElementTemperaturaRefri();
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }



        private  void comboBoxDispositivo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadComboBox();       
            
        }

        private void radioButton_Click(object sender, RoutedEventArgs e)
        {
            dpFecha.IsEnabled = false;
        }

        private void radioButton1_Click(object sender, RoutedEventArgs e)
        {
            dpFecha.IsEnabled = true;
        }

        private async void comboBoxIdDispositivo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

            //Habilidar radiobutton
            radioButtonTodo.IsEnabled = true;
            radioButtonFecha.IsEnabled = true;
            //radioButtonTodo.IsChecked = true;


            

            //Verificar que combobox idDispositivo sea diferente de nulo 
            if (comboBoxIdDispositivo.SelectedItem != null)
            {
                object selectedItemDispositivo = comboBoxIdDispositivo.SelectedValue.ToString();
                string idDispositivo = selectedItemDispositivo.ToString();
                DateTime date = dpFecha.Date.DateTime;
                GridDetalles.Visibility = Visibility.Collapsed;
                //Verificar que radioBUtton esta activo Todo o Fecha 
                if (radioButtonTodo.IsChecked == true)
                {
                    //RadioButton Todo esta activo 

                    // index 0 en comboboxEstado = atendido, Index 1 en comboboxEstado = No atendido
                    // index 0 en comboboxIdDispositivo = Refrigerador, Index 1 en comboboxIdDispositivo = Deshidratadora
                    if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 0)
                    {
                        //Index de comboboxEstado esta en 0 y es Atendido 
                        //Se selecciono Refrigerador en combobox dispositivo 
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaRefri = await FallaTableRefri
                                .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "1")
                                .ToCollectionAsync();
                            if (fallaRefri.Count >= 1)
                            {
                                listView.ItemsSource = fallaRefri;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro.", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();

                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }


                    }
                    else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 0)
                    {
                        
                        //Index de comboboxEstado esta en 1 y es No Atendido 
                        //Se selecciono Refrigerador en combobox dispositivo
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaRefri = await FallaTableRefri.Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "0")
                            .ToCollectionAsync();

                           
                            if(fallaRefri.Count >= 1)
                            {
                                listView.ItemsSource = fallaRefri;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro.", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();

                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }
                        

                    }
                    else if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 1)
                    {
                           
                            
                            //Index de comboboxEstado esta en 0 y es Atendido 
                            //Se selecciono Deshidratadora en combobox dispositivo
                            try
                            {
                                //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                                ProgressBarBefore();
                                fallaDesh = await FallaTableDesh
                                    .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "1")
                                    .ToCollectionAsync();
                                if (fallaDesh.Count >= 1)
                                {
                                    listView.ItemsSource = fallaDesh;
                                    ProgressBarAfter();
                                    listView.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    MessageDialog mensaje = new MessageDialog("No existe registro.", "No existe registro");
                                    await mensaje.ShowAsync();
                                    listView.Visibility = Visibility.Collapsed;
                                    ProgressBarAfter();

                            }
                            }
                            catch (MobileServiceInvalidOperationException ex)
                            {
                                exception = ex;
                            }

                            if (exception != null)
                            {
                                await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                            }

                   }
                   else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 1)
                   { 


                            //Index de comboboxEstado esta en 1 y es No Atendido 
                            //Se selecciono Deshidratadora en combobox dispositivo
                            try
                            {
                                //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                                ProgressBarBefore();
                                fallaDesh = await FallaTableDesh
                                    .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "0")
                                    .ToCollectionAsync();
                                if (fallaDesh.Count >= 1)
                                {
                                    listView.Visibility = Visibility.Visible;                                
                                    listView.ItemsSource = fallaDesh;
                                    ProgressBarAfter();
                                }
                                else
                                {
                                    MessageDialog mensaje = new MessageDialog("No existe registro.", "No existe registro");
                                    await mensaje.ShowAsync();
                                    listView.Visibility = Visibility.Collapsed;
                                    ProgressBarAfter();

                                }
                            }
                            catch (MobileServiceInvalidOperationException ex)
                            {
                                exception = ex;
                            }

                            if (exception != null)
                            {
                                await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                            }

                   }


                }
                else
                {
                    //RadioButton Fecha esta activo 


                    // index 0 en comboboxEstado = atendido, Index 1 en comboboxEstado = No atendido
                    // index 0 en comboboxIdDispositivo = Refrigerador, Index 1 en comboboxIdDispositivo = Deshidratadora
                    if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 0)
                    {
                        //Index de comboboxEstado esta en 0 y es Atendido 
                        //Se selecciono Refrigerador en combobox dispositivo 
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaRefri = await FallaTableRefri
                                .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "1" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                                .ToCollectionAsync();
                            if (fallaRefri.Count >= 1)
                            {
                                listView.ItemsSource = fallaRefri;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();

                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }


                        }
                        else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 0)
                        {
                        

                            //Index de comboboxEstado esta en 1 y es No Atendido 
                            //Se selecciono Refrigerador en combobox dispositivo
                            try
                            {
                                //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                                ProgressBarBefore();
                                fallaRefri = await FallaTableRefri
                                .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "0" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                                .ToCollectionAsync();
                                if (fallaRefri.Count >= 1)
                                {
                                    listView.ItemsSource = fallaRefri;
                                    ProgressBarAfter();
                                    listView.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                                    await mensaje.ShowAsync();
                                    listView.Visibility = Visibility.Collapsed;
                                    ProgressBarAfter();

                                }
                            }
                            catch (MobileServiceInvalidOperationException ex)
                            {
                                exception = ex;
                            }

                            if (exception != null)
                            {
                                await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                            }

                        }
                        else if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 1)
                        {

                            
                            //Index de comboboxEstado esta en 0 y es Atendido 
                            //Se selecciono Deshidratadora en combobox dispositivo
                            try
                            {
                                //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                                ProgressBarBefore();
                                fallaDesh = await FallaTableDesh
                                .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "1" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                                .ToCollectionAsync();
                                if (fallaDesh.Count >= 1)
                                {
                                    listView.ItemsSource = fallaDesh;
                                    ProgressBarAfter();
                                    listView.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                                    await mensaje.ShowAsync();
                                    listView.Visibility = Visibility.Collapsed;
                                    ProgressBarAfter();

                                }
                            }
                            catch (MobileServiceInvalidOperationException ex)
                            {
                                exception = ex;
                            }

                            if (exception != null)
                            {
                                await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                            }

                        }
                        else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 1)
                        {
                            

                            //Index de comboboxEstado esta en 1 y es No Atendido 
                            //Se selecciono Deshidratadora en combobox dispositivo
                            try
                            {
                                //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                                ProgressBarBefore();
                                fallaDesh = await FallaTableDesh
                                .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "0" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                                .ToCollectionAsync();
                                if (fallaDesh.Count >= 1)
                                {
                                    listView.ItemsSource = fallaDesh;
                                    ProgressBarAfter();
                                    listView.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                                    await mensaje.ShowAsync();
                                    listView.Visibility = Visibility.Collapsed;
                                    ProgressBarAfter();

                                }
                            }
                            catch (MobileServiceInvalidOperationException ex)
                            {
                                exception = ex;
                            }

                            if (exception != null)
                            {
                             await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                            }
                        }
                    }
                }              
        }

        
                    
        private async void radioButtonTodo_Checked(object sender, RoutedEventArgs e)
        {
            


            if (comboBoxIdDispositivo.SelectedItem != null)  //Comprobar que el ComboBox Id dispositivo sea diferente de nulo 
            {
                GridDetalles.Visibility = Visibility.Collapsed;
                object selectedItemDispositivo = comboBoxIdDispositivo.SelectedValue.ToString();
                string idDispositivo = selectedItemDispositivo.ToString();
                // index 0 en comboboxEstado = atendido, Index 1 en comboboxEstado = No atendido
                // index 0 en comboboxIdDispositivo = Refrigerador, Index 1 en comboboxIdDispositivo = Deshidratadora
                if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 0)
                {
                    //Index de comboboxEstado esta en 0 y es Atendido 
                    //Se selecciono Refrigerador en combobox dispositivo 
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaRefri = await FallaTableRefri
                            .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "1")
                            .ToCollectionAsync();
                        if (fallaRefri.Count >= 1)
                        {
                            listView.ItemsSource = fallaRefri;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }


                }
                else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 0)
                {
                    //Index de comboboxEstado esta en 1 y es No Atendido 
                    //Se selecciono Refrigerador en combobox dispositivo
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaRefri = await FallaTableRefri
                            .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "0")
                            .ToCollectionAsync();
                        if (fallaRefri.Count >= 1)
                        {
                            listView.ItemsSource = fallaRefri;
                            ProgressBarAfter();
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }

                }
                else if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 1)
                {
                    //Index de comboboxEstado esta en 0 y es Atendido 
                    //Se selecciono Deshidratadora en combobox dispositivo
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaDesh = await FallaTableDesh
                            .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "1")
                            .ToCollectionAsync();
                        if (fallaDesh.Count >= 1)
                        {
                            listView.ItemsSource = fallaDesh;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }

                }
                else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 1)
                {
                    //Index de comboboxEstado esta en 1 y es No Atendido 
                    //Se selecciono Deshidratadora en combobox dispositivo
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaDesh = await FallaTableDesh
                            .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "0")
                            .ToCollectionAsync();
                        if (fallaDesh.Count >= 1)
                        {
                            listView.ItemsSource = fallaDesh;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }

                }


            }
        }

        private async void comboBoxEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            //Verificar que combobox idDispositivo sea diferente de nulo 
            if (comboBoxIdDispositivo.SelectedItem != null)
            {
                GridDetalles.Visibility = Visibility.Collapsed;
                object selectedItemDispositivo = comboBoxIdDispositivo.SelectedValue.ToString();
                string idDispositivo = selectedItemDispositivo.ToString();
                DateTime date = dpFecha.Date.DateTime;

                //Verificar que radioBUtton esta activo Todo o Fecha 
                if (radioButtonTodo.IsChecked == true)
                {
                    //RadioButton Todo esta activo 

                    // index 0 en comboboxEstado = atendido, Index 1 en comboboxEstado = No atendido
                    // index 0 en comboboxIdDispositivo = Refrigerador, Index 1 en comboboxIdDispositivo = Deshidratadora
                    if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 0)
                    {
                        //Index de comboboxEstado esta en 0 y es Atendido 
                        //Se selecciono Refrigerador en combobox dispositivo 
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaRefri = await FallaTableRefri
                                .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "1")
                                .ToCollectionAsync();
                            if (fallaRefri.Count >= 1)
                            {
                                listView.ItemsSource = fallaRefri;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro.", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();

                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }


                    }
                    else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 0)
                    {
                        //Index de comboboxEstado esta en 1 y es No Atendido 
                        //Se selecciono Refrigerador en combobox dispositivo
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaRefri = await FallaTableRefri
                                .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "0")
                                .ToCollectionAsync();
                            if (fallaRefri.Count >= 1)
                            {
                                listView.ItemsSource = fallaRefri;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro.", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();

                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }

                    }
                    else if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 1)
                    {
                        //Index de comboboxEstado esta en 0 y es Atendido 
                        //Se selecciono Deshidratadora en combobox dispositivo
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaDesh = await FallaTableDesh
                                .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "1")
                                .ToCollectionAsync();
                            if (fallaDesh.Count >= 1)
                            {
                                listView.ItemsSource = fallaDesh;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro.", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();

                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }

                    }
                    else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 1)
                    {
                        //Index de comboboxEstado esta en 1 y es No Atendido 
                        //Se selecciono Deshidratadora en combobox dispositivo
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaDesh = await FallaTableDesh
                                .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "0")
                                .ToCollectionAsync();
                            if (fallaDesh.Count >= 1)
                            {
                                listView.ItemsSource = fallaDesh;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro.", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();


                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }

                    }


                }
                else
                {
                    //RadioButton Fecha esta activo 


                    // index 0 en comboboxEstado = atendido, Index 1 en comboboxEstado = No atendido
                    // index 0 en comboboxIdDispositivo = Refrigerador, Index 1 en comboboxIdDispositivo = Deshidratadora
                    if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 0)
                    {
                        //Index de comboboxEstado esta en 0 y es Atendido 
                        //Se selecciono Refrigerador en combobox dispositivo 
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaRefri = await FallaTableRefri
                                .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "1" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                                .ToCollectionAsync();
                            if (fallaRefri.Count >= 1)
                            {
                                listView.ItemsSource = fallaRefri;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();

                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }


                    }
                    else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 0)
                    {
                        //Index de comboboxEstado esta en 1 y es No Atendido 
                        //Se selecciono Refrigerador en combobox dispositivo
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaRefri = await FallaTableRefri
                                .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "0" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                                .ToCollectionAsync();
                            if (fallaRefri.Count >= 1)
                            {
                                listView.ItemsSource = fallaRefri;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();

                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }

                    }
                    else if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 1)
                    {
                        //Index de comboboxEstado esta en 0 y es Atendido 
                        //Se selecciono Deshidratadora en combobox dispositivo
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaDesh = await FallaTableDesh
                                .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "1" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                                .ToCollectionAsync();
                            if (fallaDesh.Count >= 1)
                            {
                                listView.ItemsSource = fallaDesh;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();

                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }

                    }
                    else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 1)
                    {
                        //Index de comboboxEstado esta en 1 y es No Atendido 
                        //Se selecciono Deshidratadora en combobox dispositivo
                        try
                        {
                            //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                            ProgressBarBefore();
                            fallaDesh = await FallaTableDesh
                                .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "0" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                                .ToCollectionAsync();
                            if (fallaDesh.Count >= 1)
                            {
                                listView.ItemsSource = fallaDesh;
                                ProgressBarAfter();
                                listView.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                                await mensaje.ShowAsync();
                                listView.Visibility = Visibility.Collapsed;
                                ProgressBarAfter();

                            }
                        }
                        catch (MobileServiceInvalidOperationException ex)
                        {
                            exception = ex;
                        }

                        if (exception != null)
                        {
                            await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                        }
                    }
                }
            }
        }

        private async void dpFecha_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            
            
            

            if (comboBoxIdDispositivo.SelectedItem != null)  //Comprobar que el ComboBox Id dispositivo sea diferente de nulo 
            {
                DateTime date = dpFecha.Date.DateTime;
                object selectedItemDispositivo = comboBoxIdDispositivo.SelectedValue.ToString();
                string idDispositivo = selectedItemDispositivo.ToString();
                GridDetalles.Visibility = Visibility.Collapsed;
                // index 0 en comboboxEstado = atendido, Index 1 en comboboxEstado = No atendido
                // index 0 en comboboxIdDispositivo = Refrigerador, Index 1 en comboboxIdDispositivo = Deshidratadora
                if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 0) 
                {
                    //Index de comboboxEstado esta en 0 y es Atendido 
                    //Se selecciono Refrigerador en combobox dispositivo 
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaRefri = await FallaTableRefri
                            .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "1" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                            .ToCollectionAsync();
                        if (fallaRefri.Count >= 1)
                        {
                            listView.ItemsSource = fallaRefri;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }


                }
                else if(comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 0)
                {
                    //Index de comboboxEstado esta en 1 y es No Atendido 
                    //Se selecciono Refrigerador en combobox dispositivo
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaRefri = await FallaTableRefri
                            .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "0" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                            .ToCollectionAsync();
                        if (fallaRefri.Count >= 1)
                        {
                            listView.ItemsSource = fallaRefri;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }

                }else if(comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 1)
                {
                    //Index de comboboxEstado esta en 0 y es Atendido 
                    //Se selecciono Deshidratadora en combobox dispositivo
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaDesh = await FallaTableDesh
                            .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "1" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                            .ToCollectionAsync();
                        if (fallaDesh.Count >= 1)
                        {
                            listView.ItemsSource = fallaDesh;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }

                }
                else if(comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 1)
                {
                    //Index de comboboxEstado esta en 1 y es No Atendido 
                    //Se selecciono Deshidratadora en combobox dispositivo
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaDesh = await FallaTableDesh
                            .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "0" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                            .ToCollectionAsync();
                        if (fallaDesh.Count >= 1)
                        {
                            listView.ItemsSource = fallaDesh;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }

                }


            }
            

        }

        private async void radioButtonFecha_Checked(object sender, RoutedEventArgs e)
        {
            


            if (comboBoxIdDispositivo.SelectedItem != null)  //Comprobar que el ComboBox Id dispositivo sea diferente de nulo 
            {
                GridDetalles.Visibility = Visibility.Collapsed;
                object selectedItemDispositivo = comboBoxIdDispositivo.SelectedValue.ToString();
                String idDispositivo = selectedItemDispositivo.ToString();
                DateTime date = dpFecha.Date.DateTime;
                // index 0 en comboboxEstado = atendido, Index 1 en comboboxEstado = No atendido
                // index 0 en comboboxIdDispositivo = Refrigerador, Index 1 en comboboxIdDispositivo = Deshidratadora
                if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 0)
                {
                    //Index de comboboxEstado esta en 0 y es Atendido 
                    //Se selecciono Refrigerador en combobox dispositivo 
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaRefri = await FallaTableRefri
                            .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "1" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                            .ToCollectionAsync();
                        if (fallaRefri.Count >= 1)
                        {
                            listView.ItemsSource = fallaRefri;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }


                }
                else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 0)
                {
                    //Index de comboboxEstado esta en 1 y es No Atendido 
                    //Se selecciono Refrigerador en combobox dispositivo
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaRefri = await FallaTableRefri
                            .Where(Falla => Falla.idRefrigerador == idDispositivo && Falla.Atendido == "0" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                            .ToCollectionAsync();
                        if (fallaRefri.Count >= 1)
                        {
                            listView.ItemsSource = fallaRefri;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }

                }
                else if (comboBoxEstado.SelectedIndex == 0 && comboBoxDispositivo.SelectedIndex == 1)
                {
                    //Index de comboboxEstado esta en 0 y es Atendido 
                    //Se selecciono Deshidratadora en combobox dispositivo
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaDesh = await FallaTableDesh
                            .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "1" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                            .ToCollectionAsync();
                        if (fallaDesh.Count >= 1)
                        {
                            listView.ItemsSource = fallaDesh;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }

                }
                else if (comboBoxEstado.SelectedIndex == 1 && comboBoxDispositivo.SelectedIndex == 1)
                {
                    //Index de comboboxEstado esta en 1 y es No Atendido 
                    //Se selecciono Deshidratadora en combobox dispositivo
                    try
                    {
                        //En la base de datos un 1 representa Atendido y un 0 Representa a No Atendido 
                        ProgressBarBefore();
                        fallaDesh = await FallaTableDesh
                            .Where(Falla => Falla.idDeshidratadora == idDispositivo && Falla.Atendido == "0" && Falla.Fecha == date.ToString("yyyy-MM-dd"))
                            .ToCollectionAsync();
                        if (fallaDesh.Count >= 1)
                        {
                            listView.ItemsSource = fallaDesh;
                            ProgressBarAfter();
                            listView.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha", "No existe registro");
                            await mensaje.ShowAsync();
                            listView.Visibility = Visibility.Collapsed;
                            ProgressBarAfter();

                        }
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        await new MessageDialog(exception.Message, "Error al cargar!").ShowAsync();
                    }

                }


            }
        }

        private async void ListBoxItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            
            GridDetalles.Visibility = Visibility.Visible;

              


            //Obtener index del combobox dispositivo Index 0 Representa un Refrigerador, 1 Represeta una Deshidratadora

            if (comboBoxDispositivo.SelectedIndex == 0)
            {

                ListBoxItem lbi = (ListBoxItem)sender;
                
                //listView.SelectedIndex = ;
                Falla_Refri item = lbi.DataContext as Falla_Refri;
                string idFalla = item.idFalla.ToString();


                //Esta seleccionado un Refrigerador
                IMobileServiceTableQuery<Falla_Refri> query = FallaTableRefri.Where(Falla => Falla.idFalla == idFalla);
                var res = await query.ToListAsync();
                var falla = res.First();
                string idFallaTB = falla.idFalla;
                string estadoTB = falla.Atendido;
                string temperaturaTB = falla.Temperatura_Refri;
                string observacionesTB = falla.Observacion;
                string idTecnicoTB = falla.idTecnico;
                string idRefriTB = falla.idRefrigerador;

                if(estadoTB == "0")
                {
                    estadoTB = "No Atendido";
                }
                else
                {
                    estadoTB = "Atendido";
                }

                if(observacionesTB == null || observacionesTB == "")
                {
                    observacionesTB = "Sin Observaciones";
                }

                if(idTecnicoTB == null)
                {
                    idTecnicoTB = "Aún no se ha atendido por ningun tecnico"; 
                }

                textBlockIdFalla.Text = idFallaTB;
                textBlockEstado.Text = estadoTB;
                textBlockTemperatura.Text = temperaturaTB;
                textBlockObservaciones.Text = observacionesTB;
                textBlockIdTecnico.Text = idTecnicoTB;
                textBlockIdDispositivo.Text = idRefriTB;


            }
            else
            {
                //Esta seleccionado una Deshidratadora
                //Selecciono una Deshidratadora
                ListBoxItem lbi = (ListBoxItem)sender;
                /*int pruba = lbi.TabIndex;
                listView.SelectedIndex = pruba;*/
                Falla_Desh item = lbi.DataContext as Falla_Desh;
                string idFalla = item.idFalla.ToString();
                IMobileServiceTableQuery<Falla_Desh> query = FallaTableDesh.Where(Falla => Falla.idFalla == idFalla);
                var res = await query.ToListAsync();
                var falla = res.First();
                string idFallaTB = falla.idFalla;
                string estadoTB = falla.Atendido;
                string temperaturaTB = falla.Temperatura_Desh;
                string observacionesTB = falla.Observacion;
                string idTecnicoTB = falla.idTecnico;
                string idRefriTB = falla.idDeshidratadora;

                if (estadoTB == "0")
                {
                    estadoTB = "No Atendido";
                }
                else
                {
                    estadoTB = "Atendido";
                }

                if (observacionesTB == null || observacionesTB == "")
                {
                    observacionesTB = "Sin Observaciones";
                }

                if (idTecnicoTB == null)
                {
                    idTecnicoTB = "Aún no se ha atendido por ningun tecnico";
                }

                textBlockIdFalla.Text = idFallaTB;
                textBlockEstado.Text = estadoTB;
                textBlockTemperatura.Text = temperaturaTB;
                textBlockObservaciones.Text = observacionesTB;
                textBlockIdTecnico.Text = idTecnicoTB;
                textBlockIdDispositivo.Text = idRefriTB;

            }




        }

        private async void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            enableChangeStatus();
            gridDetallesTecnico.Visibility = Visibility.Visible;
            //Cargar combobox id tecnico 
            if(comboBoxIdDispositivo.SelectedItem != null)
            {
                try
                {
                    ProgressBarBefore();
                    usuarios = await UsuarioTable
                        .Select(Usuario => Usuario)
                        .Where(Usuario => Usuario.Tipo == "Tecnico")
                        .ToCollectionAsync();
                    comboBoxidTecnico.ItemsSource = usuarios;                  
                    ProgressBarAfter();
                    comboBoxidTecnico.DisplayMemberPath = "id";
                    comboBoxidTecnico.SelectedValuePath = "id";
                    comboBoxidTecnico.SelectedIndex = 0; 
                    if(comboBoxEstado.SelectedIndex == 0)
                    {
                        //Selecciono atendido 
                        comboBoxCambiarEstado.SelectedIndex = 0;
                    }
                    else
                    {
                        //Selecciono no atendido 
                        comboBoxCambiarEstado.SelectedIndex = 1; 
                    }
                    



                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    exception = ex;
                }
                if (exception != null)
                {
                    await new MessageDialog(exception.Message, "Error al cargar a los Técnicos!").ShowAsync();
                }

            }

            if(comboBoxidTecnico.SelectedItem != null)
            {
                ProgressBarBefore();
                try
                {
                    usuarios = await UsuarioTable
                        .Select(Usuario => Usuario)
                        .Where(Usuario => Usuario.id == comboBoxidTecnico.SelectedValue.ToString())
                        .ToCollectionAsync();
                    ProgressBarAfter();

                    if (usuarios.Count() == 1)
                    {
                        var tecnico = usuarios.First();                        
                        textBlockNombre.Text = tecnico.Nombre;
                        textBlock15Apaterno.Text = tecnico.APaterno;
                        textBlock15Amaterno.Text = tecnico.AMaterno;
                        textBlock15Telefono.Text = tecnico.telefono;
                        textBlock15Correo.Text = tecnico.correo;
                        textBlock15Direccion.Text = tecnico.direccion;

                    }
                }catch(MobileServiceInvalidOperationException ex)
                {
                    exception = ex;
                }

                if(exception != null)
                {
                    await new MessageDialog(exception.Message, "Error!").ShowAsync();
                }
                
                
                gridDetallesTecnico.Visibility = Visibility.Visible;

            }
        }


        private void disableChangeStatus()
        {
            comboBoxidTecnico.IsEnabled = false;
            comboBoxCambiarEstado.IsEnabled = false;
            textBoxObservaciones.IsEnabled = false;
            buttonCancelar.IsEnabled = false;
            buttonGuardar.IsEnabled = false;
        }

        private void enableChangeStatus()
        {
            comboBoxidTecnico.IsEnabled = true;
            comboBoxCambiarEstado.IsEnabled = true;
            textBoxObservaciones.IsEnabled = true;
            buttonCancelar.IsEnabled = true;
            buttonGuardar.IsEnabled = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            disableChangeStatus();
            cleanCambiarEstado();
            gridDetallesTecnico.Visibility = Visibility.Collapsed;
            
        }

        public void cleanCambiarEstado()
        {
            textBoxObservaciones.Text = "";
            comboBoxidTecnico.Items.Remove(comboBoxidTecnico.SelectedIndex);
        }

        private async void comboBoxidTecnico_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxidTecnico.SelectedItem != null)
            {
                ProgressBarBefore();
                try
                {
                    usuarios = await UsuarioTable
                        .Select(Usuario => Usuario)
                        .Where(Usuario => Usuario.id == comboBoxidTecnico.SelectedValue.ToString())
                        .ToCollectionAsync();
                    ProgressBarAfter();

                    if (usuarios.Count() == 1)
                    {
                        var tecnico = usuarios.First();
                        textBlockNombre.Text = tecnico.Nombre;
                        textBlock15Apaterno.Text = tecnico.APaterno;
                        textBlock15Amaterno.Text = tecnico.AMaterno;
                        textBlock15Telefono.Text = tecnico.telefono;
                        textBlock15Correo.Text = tecnico.correo;
                        textBlock15Direccion.Text = tecnico.direccion;

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


                gridDetallesTecnico.Visibility = Visibility.Visible;
            }
        }

        private void buttonCancelar_Click(object sender, RoutedEventArgs e)
        {
            checkBox.IsChecked = false;
        }

        private async void buttonGuardar_Click(object sender, RoutedEventArgs e)
        {
                      
            if(comboBoxDispositivo.SelectedIndex == 0)
            {
                string Atendido;
                string observacion = textBoxObservaciones.Text;
                if(comboBoxCambiarEstado.SelectedIndex == 0)
                {
                    Atendido = "1";
                }
                else
                {
                    Atendido = "0";
                }
                
                try
                {
                    string idFalla = textBlockIdFalla.Text;
                    IMobileServiceTableQuery<Falla_Refri> query = FallaTableRefri.Select(Falla => Falla).Where(Falla => Falla.idFalla == idFalla);
                    var res = await query.ToListAsync();
                    
                    if(res.Count() >= 1)
                    {
                        var falla = res.First();
                        string id = falla.id;
                        JObject jo = new JObject();
                        jo.Add("id", id);
                        jo.Add("idTecnico", comboBoxidTecnico.SelectedValue.ToString());
                        jo.Add("Atendido", Atendido);
                        jo.Add("Observacion", observacion);
                        var inserted = await FallaTableRefri.UpdateAsync(jo);
                    }
                    
                    

                }
                catch(MobileServiceInvalidOperationException ex)
                {
                    exception = ex;
                }

                if(exception!= null)
                {
                    await new MessageDialog(exception.Message, "Error!").ShowAsync();
                }
                else
                {
                    MessageDialog mensaje = new MessageDialog("Se actualizo la información correctamente.", "Actualización correcta");
                    await mensaje.ShowAsync();
                    // Recargar de nuevo 
                    checkBox.IsChecked = false;
                    if(comboBoxCambiarEstado.SelectedIndex == 0)
                    {
                        comboBoxEstado.SelectedIndex = 0;
                    }
                    else
                    {
                        comboBoxEstado.SelectedIndex = 1;
                    }
                }

            }
            else
            {
                string Atendido;
                string observacion = textBoxObservaciones.Text;
                if (comboBoxCambiarEstado.SelectedIndex == 0)
                {
                    Atendido = "1";
                }
                else
                {
                    Atendido = "0";
                }

                try
                {
                    string idFalla = textBlockIdFalla.Text;
                    IMobileServiceTableQuery<Falla_Desh> query = FallaTableDesh.Where(Falla => Falla.idFalla == idFalla);
                    var res = await query.ToListAsync();
                    if (res.Count() >= 1)
                    {
                        var falla = res.First();
                        string id = falla.id;
                        JObject jo = new JObject();
                        jo.Add("id", id);
                        jo.Add("idTecnico", comboBoxidTecnico.SelectedValue.ToString());
                        jo.Add("Atendido", Atendido);
                        jo.Add("Observacion", observacion);
                        var inserted = await FallaTableDesh.UpdateAsync(jo);

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
                else
                {
                    MessageDialog mensaje = new MessageDialog("Se actualizo la información correctamente.", "Actualización correcta");
                    await mensaje.ShowAsync();
                    // Recargar de nuevo 
                    checkBox.IsChecked = false;
                    if (comboBoxCambiarEstado.SelectedIndex == 0)
                    {
                        comboBoxEstado.SelectedIndex = 0;
                    }
                    else
                    {
                        comboBoxEstado.SelectedIndex = 1;
                    }
                }
            }
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkBox.IsChecked = false;
        }
    }
}
