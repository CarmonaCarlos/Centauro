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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Centauro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TemperaturaRefrigerador : Page
    {
        IMobileServiceTable<Refrigerador> RefriTable = App.MobileService.GetTable<Refrigerador>();
        MobileServiceCollection<Refrigerador, Refrigerador> refrigeradores;

        IMobileServiceTable<Temp_Refri> TempRefriTable = App.MobileService.GetTable<Temp_Refri>();
        MobileServiceCollection<Temp_Refri, Temp_Refri> temperaturas;

        IMobileServiceTable<Falla_Refri> FallaTableRefri = App.MobileService.GetTable<Falla_Refri>();
        MobileServiceCollection<Falla_Refri, Falla_Refri> fallaRefri;

        public TemperaturaRefrigerador()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //LoadChartContents();
            LoadComboBox();
            
        }


        private async void LoadChartContents(DateTime date)
        {
            
            MobileServiceInvalidOperationException exception = null;
            string idRefrigerador;
            object selectedItem = comboBox.SelectedValue.ToString();
            idRefrigerador = selectedItem.ToString();
            
           
            try
                {
                    ProgressBarBefore();
                    temperaturas = await TempRefriTable
                        .Where(Tempe => Tempe.Fecha == date.ToString("yyyy-MM-dd") && Tempe.idRefrigerador == idRefrigerador)
                        .ToCollectionAsync();
                    if (temperaturas.Count >= 1)
                    {
                        foreach (var c in temperaturas)
                        {
                            string[] hora = c.Hora.Split(' ');
                            c.Hora = hora[1];
                        }

                        CategoryAxis categoryAxis = new CategoryAxis() { Orientation = AxisOrientation.X, Location = AxisLocation.Bottom, AxisLabelStyle = App.Current.Resources["VerticalAxisStyle"] as Style };
                        (LineSeries.Series[0] as LineSeries).IndependentAxis = categoryAxis;
                        (LineSeries.Series[0] as LineSeries).ItemsSource = temperaturas;
                        LineSeries.Visibility = Visibility.Visible;

                        ProgressBarAfter();
                    }
                    else
                    {
                        LineSeries.Visibility = Visibility.Collapsed;
                        MessageDialog mensaje = new MessageDialog("No existe registro para esta fecha.", "No existe registro");
                        await mensaje.ShowAsync();
                        ProgressBarAfter();
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

                // (LineSeries.Series[0] as LineSeries).ItemsSource = financialStuffList;


            

        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {

            DateTime date = DatePickerToChart.Date.DateTime;
            LoadChartContents(date);
        }

        public async void LoadComboBox()
        {
            MobileServiceInvalidOperationException exception = null;

            try
            {
                ProgressBarBefore();
                refrigeradores = await RefriTable
                    .Select(Refri => Refri)
                    .ToCollectionAsync();
                comboBox.ItemsSource = refrigeradores;
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

            if (comboBox.SelectedItem == null)
            {
                BtnActualizar.IsEnabled = false;
                DatePickerToChart.IsEnabled = false;


            }
            else
            {
                BtnActualizar.IsEnabled = true;
                DatePickerToChart.IsEnabled = true;
            }
        }

        private async void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //LoadChartContents();
            MobileServiceInvalidOperationException exception = null;
            string idRefrigerador;  
            object selectedItem = comboBox.SelectedValue.ToString();
            idRefrigerador = selectedItem.ToString();
            try
            {
                ProgressBarBefore();
                DateTime date = DatePickerToChart.Date.DateTime;
                LoadChartContents(date);
                IMobileServiceTableQuery<Refrigerador> query = RefriTable.Where(Refri => Refri.idRefrigerador == idRefrigerador);
                var res = await query.ToListAsync();

                fallaRefri = await FallaTableRefri
                    .Select(Falla => Falla)
                    .Where(Falla => Falla.idRefrigerador == idRefrigerador /*&& Falla.Fecha == date.ToString("yyyy-MM-dd") /*&& Falla.Atendido == "0"*/)
                    .ToCollectionAsync();


                string cantFallas = fallaRefri.Count().ToString();

                var item = res.First();
                string marca = item.Marca;
                string modelo = item.Modelo;
                string sn = item.No_Serie;
                textBlockMarca.Text = marca;
                textBlockModelo.Text = modelo;
                textBlockNoSerie.Text = sn;
                textBlockTotalAlertas.Text = cantFallas;
                ProgressBarAfter();

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
            disableElementTemperaturaRefri();
            LoadingBar.IsEnabled = true;
            LoadingBar.Visibility = Visibility.Visible;
        }

        public void ProgressBarAfter()
        {
            EnableElementTemperaturaRefri();
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        private void DatePickerToChart_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            DateTime date = DatePickerToChart.Date.DateTime;
            /*string idRefrigerador;
            if (comboBox.SelectedItem != null)
            {
                object selectedItem = comboBox.SelectedValue.ToString();
                idRefrigerador = selectedItem.ToString();
            }
            else
            {
                MessageDialog mensaje = new MessageDialog("Seleccione un refrigerador.", "No selecciono refrigerador");
                await mensaje.ShowAsync();
                idRefrigerador = "NOREF";
            }*/
            LoadChartContents(date);
        }

        public void disableElementTemperaturaRefri()
        {
            comboBox.IsEnabled = false;
            DatePickerToChart.IsEnabled = false;
            BtnActualizar.IsEnabled = false;
        }
        public void EnableElementTemperaturaRefri()
        {
            comboBox.IsEnabled = true;
            DatePickerToChart.IsEnabled = true;
            BtnActualizar.IsEnabled = true;
        }





    }
}
