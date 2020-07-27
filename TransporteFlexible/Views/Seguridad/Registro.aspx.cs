using Common.DTO.Shared;
using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad
{
    public partial class Registro : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.DataBind();
        }

        protected void BtnRegistrarme_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && ValidarCampos())
            {
                Button _btnRegistrarme = (Button)sender;
                _btnRegistrarme.Enabled = false;
                UserManager _usrManager = new UserManager();
                string rolId = Request.QueryString["perfil"];
                if (!string.IsNullOrWhiteSpace(rolId))
                {
                    RegistroDto registro = new RegistroDto
                    {
                        Nombre = _tbNombre.Text,
                        Apellido = _tbApellido.Text,
                        DNI = _tbDni.Text,
                        CUIL = _tbCuil.Text,
                        EsCuit = _cbEsCuit.Checked,
                        Email = _tbEmail.Text,
                        Telefono = _tbTelefono.Text,
                        NombreUsuario = _tbNombreUsuario.Text,
                        Contraseña = _tbContraseña.Text,
                        RepetirContraseña = _tbRepetirContraseña.Text,
                        Rol = rolId
                    };

                    Message msj = _usrManager.RegisterNewBusinessUser(registro);
                    MensajesHelper.ProcesarMensajeGenerico(this.GetType(), msj, Page);
                    _btnRegistrarme.Enabled = true;
                }
                else
                {
                    Message msj = MessageFactory.GetMessage("MS36", ViewsEnum.Default.GD());
                    MensajesHelper.ProcesarMensajeGenerico(this.GetType(), msj, Page);                    
                }
            }            
        }

        private bool ValidarCampos()
        {
            return !string.IsNullOrEmpty(_tbNombre.Text) && !string.IsNullOrEmpty(_tbApellido.Text) &&
                   !string.IsNullOrEmpty(_tbDni.Text) && !string.IsNullOrEmpty(_tbCuil.Text) &&
                   !string.IsNullOrEmpty(_tbEmail.Text) && !string.IsNullOrEmpty(_tbTelefono.Text) &&
                   !string.IsNullOrEmpty(_tbNombreUsuario.Text) && !string.IsNullOrEmpty(_tbContraseña.Text) &&
                   !string.IsNullOrEmpty(_tbRepetirContraseña.Text);
        }
    }
}