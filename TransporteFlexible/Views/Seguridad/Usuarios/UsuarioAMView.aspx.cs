using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using Negocio.Managers.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad.Usuarios
{
    public partial class UsuarioAMView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageLoad();
            }
        }

        private void PageLoad()
        {
            if (Request.QueryString.HasKeys() &&
                            int.TryParse(Request.QueryString.Get("id"), out int userId))
            {
                User user = ObtenerUsuario(userId);
                if (user != null)
                {
                    PopularDatosUsuario(user);
                    if (user.Persona != null)
                    {
                        Session[SV.EditingPersonId.GD()] = user.Persona.Id;
                        PopularDatosDePersona(user.Persona);

                        if (user.Persona.Emails != null)
                        {
                            WUCEmails.Emails = user.Persona.Emails;
                        }

                        if (user.Persona.Telefonos != null)
                        {
                            WUCTelefonos.Phones = user.Persona.Telefonos;
                        }

                        if (user.Persona.Addresses != null)
                        {
                            WUCAddress.Addresses = user.Persona.Addresses;
                        }
                        else
                        {
                            WUCAddress.Addresses = new List<Address>();
                        }
                    }
                }
                else
                {
                    Message msj = MessageFactory.GetMessage("MS09");
                    MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
                }
            }
        }

        private void PopularDatosDePersona(Persona persona)
        {
            // DATOS DE PERSONA
            _tbNombre.Text = persona.Nombre;
            _tbApellido.Text = persona.Apellido;
            _tbDni.Text = persona.DNI;
            _tbCuil.Text = persona.NumeroCuil;
            _ddlEsCuit.SelectedValue = persona.EsCuit.ToString();
            User usuarioCreacion = ObtenerUsuario((int)persona.UsuarioCreacion);
            _lblUsuarioCreacionPersona.Text = usuarioCreacion != null ? usuarioCreacion.NombreUsuario : "";
            _lblFechaCreacionPersona.Text = persona.FechaCreacion.ToString("MM/dd/yyyy H:mm");
            User usuarioModificacion = ObtenerUsuario((int)persona.UsuarioModificacion);
            _lbUsuarioModificacionPersona.Text = usuarioModificacion != null ? usuarioModificacion.NombreUsuario : "";
            _lblFechamodificacionPersona.Text = persona.FechaModificacion.ToString("MM/dd/yyyy H:mm");
            // DATOS DE PERSONA
        }

        private void PopularDatosUsuario(User user)
        {
            // DATOS DE USUARIO
            _tbNombreUsuario.Text = user.NombreUsuario;
            _tbIntentos.Text = user.Intentos.ToString();
            _ddlActivo.SelectedValue = user.Activo.ToString();
            User usuarioCreacion = ObtenerUsuario((int)user.UsuarioCreacion);
            _lblUsuarioCreación.Text = usuarioCreacion != null ? usuarioCreacion.NombreUsuario : "";
            _lblFechaCreación.Text = user.FechaCreacion.ToString("MM/dd/yyyy H:mm");
            User usuarioModificacion = ObtenerUsuario((int)user.UsuarioModificacion);
            _lblUsuarioModificación.Text = usuarioModificacion != null ? usuarioModificacion.NombreUsuario : "";
            _lblFechaModificacion.Text = user.FechaModificacion.ToString("MM/dd/yyyy H:mm");
            // DATOS DE USUARIO
        }

        private User ObtenerUsuario(int userId)
        {
            UserManager _usuarioMgr = new UserManager();
            Message msj = _usuarioMgr.ObtenerUsuariosNegocioDesencriptados(userId, null, null);
            if (msj.CodigoMensaje != "OK")
            {
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
            else
            {
                List<User> usuarios = (List<User>)msj.Resultado;
                if (usuarios.Count == 1)
                {
                    return usuarios.Single();
                }
            }
            return null;
        }
    }
}