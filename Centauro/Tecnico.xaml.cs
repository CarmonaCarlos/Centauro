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
    public sealed partial class Tecnico : Page
    {
        IMobileServiceTable<Usuario> UsuariosTable = App.MobileService.GetTable<Usuario>();
        MobileServiceCollection<Usuario, Usuario> usuarios;
        MobileServiceInvalidOperationException exception = null;

        public Tecnico()
        {
            this.InitializeComponent();
            disableTexBox();
            //radioButtonBusqueda.IsChecked = true;
        }





        private void limpiarTexBoxRegistroEditar()
        {
            textBoxNombreUsuario.Text = "";
            textBoxNombre.Text = "";
            textBoxAPatrno.Text = "";
            textBoxAMaterno.Text = "";
            textBoxTelefono.Text = "";
            textBoxCorreo.Text = "";
            passwordBox.Password = "";
            textBoxDireccion.Text = "";
            dpFechaNac.Date = DateTime.Now;
        }

        public void enableRegistro()
        {
            textBoxNombre.IsEnabled = true;
            textBoxAPatrno.IsEnabled = true;
            textBoxAMaterno.IsEnabled = true;
            dpFechaNac.IsEnabled = true;
            textBoxTelefono.IsEnabled = true;
            textBoxCorreo.IsEnabled = true;
            textBoxDireccion.IsEnabled = true;
            buttonCancelar.IsEnabled = true;
            buttonGuardar.IsEnabled = true;
            passwordBox.IsEnabled = true;
        }

        private async void radioButtonBuscar_Checked(object sender, RoutedEventArgs e)
        {
            disableTexBox();
            comboBoxIdTecnicoEditar.IsEnabled = true;
            buttonEliminar.Visibility = Visibility.Collapsed;
            textBlockTitleNewEdit.Text = "Busqueda de Técnico";
            //enableRegistro();

            comboBoxIdTecnicoEditar.DisplayMemberPath = "id";
            comboBoxIdTecnicoEditar.SelectedValuePath = "id";

            try
            {
                ProgressBarBefore();
                usuarios = await UsuariosTable
                    .Select(Usuarios => Usuarios)
                    .Where(Usuarios => Usuarios.Tipo == "Tecnico")
                    .ToCollectionAsync();
                comboBoxIdTecnicoEditar.ItemsSource = usuarios;
                ProgressBarAfter();
                if(usuarios.Count >= 1)
                {
                    comboBoxIdTecnicoEditar.SelectedIndex = 0;
                }
                else
                {
                    MessageDialog mensaje = new MessageDialog("No existen tecnicos registrados.", "Sin registros");
                    await mensaje.ShowAsync();

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

        private void radioButtonRegistrar_Checked(object sender, RoutedEventArgs e)
        {

            //RadioButton Registar nuevo tecnico 
            enableTexBox();
            buttonGuardar.Content = "Guardar";
            comboBoxIdTecnicoEditar.IsEnabled = false;
            buttonEliminar.Visibility = Visibility.Collapsed;
            limpiarTexBoxRegistroEditar();
            textBlockTitleNewEdit.Text = "Registrar nuevo Técnico";


        }

        public void ProgressBarBefore()
        {
 
            LoadingBar.IsEnabled = true;
            LoadingBar.Visibility = Visibility.Visible;
        }

        public void ProgressBarAfter()
        {
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        private async void radioButtonEditar_Checked(object sender, RoutedEventArgs e)
        {
            enableTexBox();
            comboBoxIdTecnicoEditar.IsEnabled = true;
            buttonGuardar.Content = "Actualizar";
            textBlockTitleNewEdit.Text = "Editar técnico";
            buttonEliminar.Visibility = Visibility.Collapsed;
            limpiarTexBoxRegistroEditar();
            comboBoxIdTecnicoEditar.DisplayMemberPath = "id";
            comboBoxIdTecnicoEditar.SelectedValuePath = "id";
            textBoxNombreUsuario.IsEnabled = false;

            try
            {
                ProgressBarBefore();
                usuarios = await UsuariosTable
                    .Select(Tecnicos => Tecnicos)
                    .Where(Tecnicos => Tecnicos.Tipo == "Tecnico")
                    .ToCollectionAsync();
                comboBoxIdTecnicoEditar.ItemsSource = usuarios;
                ProgressBarAfter();
                if (usuarios.Count >= 1)
                {
                    comboBoxIdTecnicoEditar.SelectedIndex = 0;
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

        private async void radioButtonEliminar_Checked(object sender, RoutedEventArgs e)
        {
            disableTexBox();
            comboBoxIdTecnicoEditar.IsEnabled = true;
            buttonEliminar.Visibility = Visibility.Visible;
            textBlockTitleNewEdit.Text = "Eliminar Técnico";
            comboBoxIdTecnicoEditar.DisplayMemberPath = "id";
            comboBoxIdTecnicoEditar.SelectedValuePath = "id";


            try
            {
                ProgressBarBefore();
                usuarios = await UsuariosTable
                    .Select(Tecnicos => Tecnicos)
                    .Where(Tecnicos => Tecnicos.Tipo == "Tecnico")
                    .ToCollectionAsync();
                comboBoxIdTecnicoEditar.ItemsSource = usuarios;
                ProgressBarAfter();
                if (usuarios.Count >= 1)
                {
                    comboBoxIdTecnicoEditar.SelectedIndex = 0;
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

        private async void comboBoxIdTecnicoEditar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Cargar detalles del tecnico 
            if(comboBoxIdTecnicoEditar.SelectedItem != null)
            {
                string idTecnico;
                object selectedItem = comboBoxIdTecnicoEditar.SelectedValue.ToString();
                idTecnico = selectedItem.ToString();
                try
                {
                    ProgressBarBefore();
                    IMobileServiceTableQuery<Usuario> query = UsuariosTable.Where(Tecnico => Tecnico.id == idTecnico);
                    var res = await query.ToListAsync();
                    var item = res.First();
                    string id = item.id;
                    string nombre = item.Nombre;
                    string appaterno = item.APaterno;
                    string apmaterno = item.AMaterno;
                    string fechaNacimiento = item.fechaNacimiento;
                    string correo = item.correo;
                    string telefono = item.telefono;
                    string direccion = item.direccion;
                    string contrasena = item.contrasena;
                    textBoxNombreUsuario.Text = id;
                    textBoxNombre.Text = nombre;
                    textBoxAPatrno.Text = appaterno;
                    textBoxAMaterno.Text = apmaterno;
                    dpFechaNac.Date = DateTime.Parse(fechaNacimiento);
                    //textBox.Text = fechaNacimiento.ToString();
                    textBoxCorreo.Text = correo;
                    textBoxTelefono.Text = telefono;
                    textBoxDireccion.Text = direccion;
                    passwordBox.Password = contrasena;
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

            
        
        }


        /*private async void comboBoxIdTecnicoEditar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           /* // Cargar TextBox 

            if (comboBoxIdTecnicoEditar.SelectedItem != null)
            {
                string idTecnico;
                object selectedItem = comboBoxIdTecnicoEditar.SelectedValue.ToString();
                idTecnico = selectedItem.ToString();
                try
                {
                    ProgressBarBefore();
                    IMobileServiceTableQuery<Tecnicos> query = TecnicosTable.Where(Tecnico => Tecnico.idTecnico == idTecnico);
                    var res = await query.ToListAsync();
                    var item = res.First();
                    string nombre = item.Nombre;
                    string appaterno = item.APaterno;
                    string apmaterno = item.AMaterno;
                    string edad = item.edad;
                    string correo = item.correo;
                    string telefono = item.telefono;
                    string direccion = item.direccion;
                    string contrasena = item.contrasena;
                    textBoxNombre.Text = nombre;
                    textBoxAPatrno.Text = appaterno;
                    textBoxAMaterno.Text = apmaterno;
                    //textBox.Text = edad;
                    textBoxCorreo.Text = correo;
                    textBoxTelefono.Text = telefono;
                    textBoxDireccion.Text = direccion;
                    textBoxContrasena.Text = contrasena;
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

            }*/



        //}

        private void buttonCancelar_Click(object sender, RoutedEventArgs e)
        {
            limpiarTexBoxRegistroEditar();
            disableTexBox();
            radioButtonEditar.IsChecked = false;
            radioButtonRegistro.IsChecked = false;
        }

        private async void buttonEliminar_Click(object sender, RoutedEventArgs e)
        {
          if(comboBoxIdTecnicoEditar.SelectedItem != null)
            {

                string idTecnico;
                object selectedItem = comboBoxIdTecnicoEditar.SelectedValue.ToString();
                idTecnico = selectedItem.ToString();

                var dialog = new MessageDialog("¿Esta seguro de eliminar al usuario?");
                dialog.Title = "¿Esta seguro?";
                dialog.Commands.Add(new UICommand { Label = "Si", Id = 0 });
                dialog.Commands.Add(new UICommand { Label = "No", Id = 1 });
                var result = await dialog.ShowAsync();

                if ((int)result.Id == 0)
                {
                    try
                    {
                        IMobileServiceTableQuery<Usuario> query = UsuariosTable.Where(Tecnico => Tecnico.id == idTecnico);
                        var res = await query.ToListAsync();
                        var item = res.First();
                        string idEliminar = item.id;
                        JObject tecnicoDelete = new JObject();
                        tecnicoDelete.Add("id", idEliminar);
                        await UsuariosTable.DeleteAsync(tecnicoDelete);
                        radioButtonEliminar.IsChecked = false;
                        buttonEliminar.Visibility = Visibility.Collapsed;
                        comboBoxIdTecnicoEditar.IsEnabled = false;

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
                        MessageDialog mensaje = new MessageDialog("Se elimino correctamente.", "Eliminación correcta");
                        await mensaje.ShowAsync();
                        limpiarTexBoxRegistroEditar();
                        disableTexBox();
                        radioButtonEliminar.IsChecked = false;

                    }
                }

                
            }
            
        }

        private async void buttonGuardar_Click(object sender, RoutedEventArgs e)
        {
            
            if (radioButtonEditar.IsChecked == true)
            {
                int validar = await validarCampos();
                if (validar == 1)
                {
                    //Actualizar registro 
                    if (comboBoxIdTecnicoEditar.SelectedItem != null)
                    {

                        string idTecnico;
                        object selectedItem = comboBoxIdTecnicoEditar.SelectedValue.ToString();
                        idTecnico = selectedItem.ToString();

                        var dialog = new MessageDialog("¿Esta seguro de modificar al usuario?");
                        dialog.Title = "¿Esta seguro?";
                        dialog.Commands.Add(new UICommand { Label = "Si", Id = 0 });
                        dialog.Commands.Add(new UICommand { Label = "No", Id = 1 });
                        var result = await dialog.ShowAsync();

                        if ((int)result.Id == 0)
                        {

                            try
                            {
                                IMobileServiceTableQuery<Usuario> query = UsuariosTable.Where(Usuario => Usuario.id == idTecnico);
                                var res = await query.ToListAsync();
                                var item = res.First();
                                string idActualizar = item.id;
                                JObject update = new JObject();
                                update.Add("id", idActualizar);
                                update.Add("Nombre", textBoxNombre.Text);
                                update.Add("APaterno", textBoxAPatrno.Text);
                                update.Add("AMaterno", textBoxAMaterno.Text);
                                update.Add("Contrasena", passwordBox.Password);
                                update.Add("Telefono", textBoxTelefono.Text);
                                update.Add("Correo", textBoxCorreo.Text);
                                update.Add("Direccion", textBoxDireccion.Text);
                                update.Add("FechaNacimiento", dpFechaNac.Date);
                                var inserted = await UsuariosTable.UpdateAsync(update);


                                //verificar si se modifico 


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
                                MessageDialog mensaje = new MessageDialog("Se actualizo correctamente.", "Actualización correcta");
                                await mensaje.ShowAsync();
                                limpiarTexBoxRegistroEditar();
                                disableTexBox();
                                radioButtonEditar.IsChecked = false;

                            }

                        }

                        
                    }

                }
                

                
                

            }

            else if (radioButtonRegistro.IsChecked == true)
            {
                int validar = await validarCampos();
                if(validar == 1)
                {
                    try
                    {
                        /*IMobileServiceTableQuery<Tecnicos> query = UsuariosTable.Select(Usuario => Usuario);
                        var res = await query.ToListAsync();
                        int aux = res.Count;
                        int contador = aux++;*/
                        //string contadorInsert = contador.ToString();
                        JObject jo = new JObject();
                        jo.Add("id", textBoxNombreUsuario.Text);
                        jo.Add("Contrasena", passwordBox.Password);
                        jo.Add("Nombre", textBoxNombre.Text);
                        jo.Add("APaterno", textBoxAPatrno.Text);
                        jo.Add("AMaterno", textBoxAMaterno.Text);
                        jo.Add("fechaNacimiento", dpFechaNac.Date);
                        jo.Add("Telefono", textBoxTelefono.Text);
                        jo.Add("Correo", textBoxCorreo.Text);
                        jo.Add("Direccion", textBoxDireccion.Text);
                        jo.Add("Tipo", "Tecnico");
                        var inserted = await UsuariosTable.InsertAsync(jo);

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
                        MessageDialog mensaje = new MessageDialog("Se guardo correctamente la información.", "Se guardo correctamente");
                        await mensaje.ShowAsync();
                        limpiarTexBoxRegistroEditar();
                        disableTexBox();
                        radioButtonRegistro.IsChecked = false;

                    }

                }

            }

                
                        
        }

        private async System.Threading.Tasks.Task<int> validarCampos()
        {
            int resultado = 0;

            if (textBoxNombreUsuario.Text == "")
            {
                MessageDialog mensaje = new MessageDialog("El campo Nombre de usuario no puede estar vacio.", "Vacio");
                await mensaje.ShowAsync();
            }else if (textBoxNombre.Text == "")
            {
                MessageDialog mensaje = new MessageDialog("El campo Nombre no puede estar vacio.", "Vacio");
                await mensaje.ShowAsync();
            }
            else if (textBoxAPatrno.Text == "")
            {
                MessageDialog mensaje = new MessageDialog("El campo Apellido Paterno no puede estar vacio.", "Vacio");
                await mensaje.ShowAsync();
            }else if(textBoxAMaterno.Text == "")
            {
                MessageDialog mensaje = new MessageDialog("El campo Apellido Materno no puede estar vacio.", "Vacio");
                await mensaje.ShowAsync();
            }else if(textBoxTelefono.Text == "")
            {
                MessageDialog mensaje = new MessageDialog("El campo Telefono no puede estar vacio.", "Vacio");
                await mensaje.ShowAsync();
            }else if(textBoxCorreo.Text == "")
            {
                MessageDialog mensaje = new MessageDialog("El campo Correo no puede estar vacio.", "Vacio");
                await mensaje.ShowAsync();
            }else if(textBoxDireccion.Text == "")
            {
                MessageDialog mensaje = new MessageDialog("El campo Dirección no puede estar vacio.", "Vacio");
                await mensaje.ShowAsync();

            }else if(passwordBox.Password == "")
            {
                MessageDialog mensaje = new MessageDialog("El campo Contraseña no puede estar vacio.", "Vacio");
                await mensaje.ShowAsync();

            }
            else
            {
                resultado = 1;
            }
            return resultado;

        }

        public void disableTexBox()
        {
            comboBoxIdTecnicoEditar.IsEnabled = false;
            textBoxNombreUsuario.IsEnabled = false;
            textBoxNombre.IsEnabled = false;
            textBoxAPatrno.IsEnabled = false;
            textBoxAMaterno.IsEnabled = false;
            dpFechaNac.IsEnabled = false;
            textBoxCorreo.IsEnabled = false;
            textBoxTelefono.IsEnabled = false;
            textBoxDireccion.IsEnabled = false;
            passwordBox.IsEnabled = false;
            buttonCancelar.IsEnabled = false;
            buttonGuardar.IsEnabled = false;
        }

        public void enableTexBox()
        {
            comboBoxIdTecnicoEditar.IsEnabled = true;
            textBoxNombreUsuario.IsEnabled = true;
            textBoxNombre.IsEnabled = true;
            textBoxAPatrno.IsEnabled = true;
            textBoxAMaterno.IsEnabled = true;
            dpFechaNac.IsEnabled = true;
            textBoxCorreo.IsEnabled = true;
            textBoxTelefono.IsEnabled = true;
            textBoxDireccion.IsEnabled = true;
            passwordBox.IsEnabled = true;
            buttonCancelar.IsEnabled = true;
            buttonGuardar.IsEnabled = true;
        }
    }
}
