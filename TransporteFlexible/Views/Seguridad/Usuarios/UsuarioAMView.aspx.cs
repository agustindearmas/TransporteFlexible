using Common.DTO.Shared;
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
using System.Web.UI.WebControls;
using TransporteFlexible.Helper;
using TransporteFlexible.Mensajes;

namespace TransporteFlexible.Views.Seguridad.Usuarios
{
    public partial class UsuarioAMView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[SV.LoggedUserName.GD()] != null)
            {
                if (SecurityHelper.CheckPermissions(6, Session[SV.Permissions.GD()]) && SecurityHelper.CheckPermissions(8, Session[SV.Permissions.GD()]))
                {
                    if (!IsPostBack)
                    {
                        PageLoad();
                    }
                }
                else
                {
                    Message msj = MessageFactory.GetMessage("MS39", "/");
                    MessageHelper.ProcessMessage(GetType(), msj, Page);
                }
            }
            else
            {
                Response.Redirect(ViewsEnum.SessionExpired.GD());
            }
        }

        private void PageLoad()
        {
            if (Request.QueryString.HasKeys())
            {
                if (int.TryParse(Request.QueryString.Get("id"), out int userId))
                {
                    if (userId != 0)
                    {
                        BuildEditView(userId);
                    }
                    else
                    {
                        BuildAddView();
                    }
                }
            }
        }

        private void BuildAddView()
        {
            TitleLBL.Text = "<span class='fa fa-user'></span> Nuevo Usuario";
            PersonCardLBL.Text = "Información de Usuario";

            AuditPersonFieldsPNL.Visible = false;
            UserFieldsPNL.Visible = false;
            ContactInfoPNL.Visible = false;
            PersonButtonsPNL.Visible = false;
            ContactAddPNL.Visible = true;
            UserInfoPNL.Visible = true;
            UserAddButtonsPNL.Visible = true;

            PopulateDLLs(RolDLL);
        }

        private void PopulateDLLs(DropDownList DLL)
        {
            RolManager _rolMgr = new RolManager();
            List<Rol> roles = _rolMgr.Retrieve(null);
            DLL.DataSource = roles;
            DLL.DataTextField = "Descripcion";
            DLL.DataValueField = "Id";
            DLL.DataBind();
            
        }

        private void BuildEditView(int userId)
        {
            PopulateDLLs(RoleEditDLL);
            User user = ObtenerUsuario(userId);
            if (user != null)
            {
                PopularDatosUsuario(user);
                if (user.Persona != null)
                {
                    Session[SV.EditingPersonId.GD()] = user.Persona.Id;
                    PopularDatosDePersona(user.Persona);
                    ContactInfoPNL.Visible = true;
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
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
        }

        private void PopularDatosUsuario(User user)
        {
            // DATOS DE USUARIO
            UserIdHF.Value = user.Id.ToString();
            UserNameTitleTB.Text = user.NombreUsuario;
            UserNameTB.Text = user.NombreUsuario;
            RoleEditDLL.SelectedValue = user.Roles.FirstOrDefault() != null ? user.Roles[0].Id.ToString() : "1";
            BlockDLL.SelectedValue = user.Activo.ToString();
            User usuarioCreacion = ObtenerUsuario((int)user.UsuarioCreacion);
            CreationUserNameLBL.Text = usuarioCreacion != null ? usuarioCreacion.NombreUsuario : "";
            CreationDateLBL.Text = user.FechaCreacion.ToString("MM/dd/yyyy H:mm");
            User usuarioModificacion = ObtenerUsuario((int)user.UsuarioModificacion);
            ModifyUserNameLBL.Text = usuarioModificacion != null ? usuarioModificacion.NombreUsuario : "";
            ModificationDateLBL.Text = user.FechaModificacion.ToString("MM/dd/yyyy H:mm");
            // DATOS DE USUARIO
        }

        private void PopularDatosDePersona(Persona person)
        {
            // DATOS DE PERSONA
            PersonIdHF.Value = person.Id.ToString();
            PersonNameTB.Text = person.Nombre;
            PersonLastNameTB.Text = person.Apellido;
            PersonDniTB.Text = person.DNI;
            PersonCuilTB.Text = person.NumeroCuil;
            PersonCuitDLL.SelectedValue = person.EsCuit.ToString();
            User usuarioCreacion = ObtenerUsuario((int)person.UsuarioCreacion);
            PersonCreationUserLBL.Text = usuarioCreacion != null ? usuarioCreacion.NombreUsuario : "";
            PersonCreationDateLBL.Text = person.FechaCreacion.ToString("MM/dd/yyyy H:mm");
            User usuarioModificacion = ObtenerUsuario((int)person.UsuarioModificacion);
            PersonModificationUserLBL.Text = usuarioModificacion != null ? usuarioModificacion.NombreUsuario : "";
            PersonModificationDateLBL.Text = person.FechaModificacion.ToString("MM/dd/yyyy H:mm");
            // DATOS DE PERSONA
        }

        private User ObtenerUsuario(int userId)
        {
            UserManager _usuarioMgr = new UserManager();
            Message msj = _usuarioMgr.GetUserById(userId);
            if (msj.CodigoMensaje != "OK")
            {
                MessageHelper.ProcessMessage(GetType(), msj, Page);
            }
            else
            {
                User user = msj.Resultado as User;
                return user;
            }
            return null;
        }

        protected void SaveUserButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (bool.TryParse(BlockDLL.SelectedValue, out bool blocked))
                {
                    if (int.TryParse(UserIdHF.Value, out int userId))
                    {
                        if (int.TryParse(RoleEditDLL.SelectedValue, out int roleId))
                        {
                            UserManager _userMgr = new UserManager();
                            Message msj = _userMgr.UpdateUser(userId, UserNameTB.Text, roleId, blocked, (int)Session[SV.LoggedUserId.GD()]);
                            MessageHelper.ProcessMessage(GetType(), msj, Page);
                            PageLoad();
                        }
                    }
                }

            }
        }

        protected void SavePersonButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(PersonNameTB.Text))
                {
                    if (!string.IsNullOrWhiteSpace(PersonLastNameTB.Text))
                    {
                        if (!string.IsNullOrWhiteSpace(PersonDniTB.Text))
                        {
                            if (!string.IsNullOrWhiteSpace(PersonCuilTB.Text))
                            {
                                if (bool.TryParse(PersonCuitDLL.SelectedValue, out bool IsCuit))
                                {
                                    if (int.TryParse(PersonIdHF.Value, out int PersonId))
                                    {
                                        PersonManager _personMgr = new PersonManager();
                                        Message msj = _personMgr.UpdatePerson(PersonId, PersonNameTB.Text, PersonLastNameTB.Text,
                                            PersonDniTB.Text, PersonCuilTB.Text, IsCuit, (int)Session[SV.LoggedUserId.GD()]);
                                        MessageHelper.ProcessMessage(GetType(), msj, Page);
                                        PageLoad();
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        protected void CancelUserButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(ViewsEnum.Usuario.GD());
        }

        protected void CancelPersonButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(ViewsEnum.Usuario.GD());
        }

        protected void AddNewUserButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && ValidateNewUserFields())
            {
                LinkButton addUserButton = sender as LinkButton;
                addUserButton.Enabled = false;
                RegistroDto RegisterDto = new RegistroDto
                {
                    Nombre = PersonNameTB.Text,
                    Apellido = PersonLastNameTB.Text,
                    DNI = PersonDniTB.Text,
                    CUIL = PersonCuilTB.Text,
                    EsCuit = bool.Parse(PersonCuitDLL.SelectedValue),
                    Contraseña = null,
                    RepetirContraseña = null,
                    Email = PersonEmailTB.Text,
                    Telefono = PersonPhoneTB.Text,
                    NombreUsuario = UserNameAddTB.Text,
                    Rol = RolDLL.SelectedValue,
                    AutomaticRegister = false
                };
                UserManager _usrManager = new UserManager();
                Message msj = _usrManager.RegisterNewUser(RegisterDto);
                MessageHelper.ProcessMessage(this.GetType(), msj, Page);
                addUserButton.Enabled = true;
            }
        }

        private bool ValidateNewUserFields()
        {
            return !string.IsNullOrWhiteSpace(PersonNameTB.Text) && !string.IsNullOrWhiteSpace(PersonLastNameTB.Text) &&
                   !string.IsNullOrWhiteSpace(PersonDniTB.Text) && !string.IsNullOrWhiteSpace(PersonCuilTB.Text) &&
                   !string.IsNullOrWhiteSpace(PersonEmailTB.Text) && !string.IsNullOrWhiteSpace(PersonPhoneTB.Text) &&
                   !string.IsNullOrWhiteSpace(UserNameAddTB.Text);
        }

        protected void CancelAddNewUserButton_Click(object sender, EventArgs e)
        {
            PageLoad();
        }
    }
}