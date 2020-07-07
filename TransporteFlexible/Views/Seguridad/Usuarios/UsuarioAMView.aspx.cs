using Common.FactoryMensaje;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
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
                Usuario user = ObtenerUsuario(userId);
                if (user != null)
                {
                    PopularDatosUsuario(user);
                    if (user.Persona != null)
                    {
                        PopularDatosDePersona(user.Persona);

                        if (user.Persona.Emails != null)
                        {
                            PopularEmailsGridView(user.Persona.Emails);
                        }
                        
                    }
                    

                }
                else
                {
                    Mensaje msj = MessageFactory.CrearMensaje("MS09");
                    MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
                }
            }
        }

        private void PopularDatosDePersona(Persona persona)
        {
            _tbNombre.Text = persona.Nombre;
            _tbApellido.Text = persona.Apellido;
            _tbDni.Text = persona.DNI;
            _tbCuil.Text = persona.NumeroCuil;
            _ddlEsCuit.SelectedValue = persona.EsCuit.ToString();
            Usuario usuarioCreacion = ObtenerUsuario((int)persona.UsuarioCreacion);
            _lblUsuarioCreacionPersona.Text = usuarioCreacion != null ? usuarioCreacion.NombreUsuario : "";
            _lblFechaCreacionPersona.Text = persona.FechaCreacion.ToString("MM/dd/yyyy H:mm");
            Usuario usuarioModificacion = ObtenerUsuario((int)persona.UsuarioModificacion);
            _lbUsuarioModificacionPersona.Text = usuarioModificacion != null ? usuarioModificacion.NombreUsuario : "";
            _lblFechamodificacionPersona.Text = persona.FechaModificacion.ToString("MM/dd/yyyy H:mm");
        }

        private void PopularDatosUsuario(Usuario user)
        {
            // DATOS DE USUARIO
            _tbNombreUsuario.Text = user.NombreUsuario;
            _tbIntentos.Text = user.Intentos.ToString();
            _ddlActivo.SelectedValue = user.Activo.ToString();
            Usuario usuarioCreacion = ObtenerUsuario((int)user.UsuarioCreacion);
            _lblUsuarioCreación.Text = usuarioCreacion != null ? usuarioCreacion.NombreUsuario : "";
            _lblFechaCreación.Text = user.FechaCreacion.ToString("MM/dd/yyyy H:mm");
            Usuario usuarioModificacion = ObtenerUsuario((int)user.UsuarioModificacion);
            _lblUsuarioModificación.Text = usuarioModificacion != null ? usuarioModificacion.NombreUsuario : "";
            _lblFechaModificacion.Text = user.FechaModificacion.ToString("MM/dd/yyyy H:mm");
            // DATOS DE USUARIO
        }

        private void PopularEmailsGridView(List<Email> emails)
        {
            WUCEmails.emails = emails;
        }

        private void PopularTelefonosGridView(List<Telefono> telefonos)
        {
            WUCTelefonos.Telefonos = telefonos;
        }


        private Usuario ObtenerUsuario(int userId)
        {
            UsuarioManager _usuarioMgr = new UsuarioManager();
            Mensaje msj = _usuarioMgr.ObtenerUsuariosNegocioDesencriptados(userId, null, null);
            if (msj.CodigoMensaje != "OK")
            {
                MensajesHelper.ProcesarMensajeGenerico(GetType(), msj, Page);
            }
            else
            {
                List<Usuario> usuarios = (List<Usuario>)msj.Resultado;
                if (usuarios.Count == 1)
                {
                    return usuarios.Single();
                }
            }
            return null;
        }
    }
}