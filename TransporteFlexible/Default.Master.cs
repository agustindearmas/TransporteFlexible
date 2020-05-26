using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using TransporteFlexible.Enums;

namespace TransporteFlexible
{
    public partial class Default : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            _lblUsuario.Text = Session["NombreUsuario"].ToString();
            ArmarMenu();
        }
        private void ArmarMenu()
        {
            List<int> permisos = (List<int>)Session["Permisos"];

            foreach (var permiso in permisos)
            {
                switch (permiso)
                {
                    case (int)PermisosEnum.LeerRolesPerfiles:
                        CrearItemDeMenu(PermisosEnum.LeerRolesPerfiles.GetDescription(), "fa-tags", "");
                        break;
                    case (int)PermisosEnum.LeerUsuariosAdministrativos:
                        CrearItemDeMenu(PermisosEnum.LeerUsuariosAdministrativos.GetDescription(), "fa-users", "../Seguridad/Usuarios/UsuarioView.aspx");
                        break;
                    case (int)PermisosEnum.LeerPermisos:
                        CrearItemDeMenu(PermisosEnum.LeerPermisos.GetDescription(), "fa-lock", "");
                        break;
                    case (int)PermisosEnum.LeerBitacora:
                        CrearItemDeMenu(PermisosEnum.LeerBitacora.GetDescription(), "fa-table", "../Seguridad/BitacoraView.aspx");
                        break;
                    case (int)PermisosEnum.LeerBasedeDatos:
                        CrearItemDeMenu(PermisosEnum.LeerBasedeDatos.GetDescription(), "fa-database", "../Seguridad/BaseDeDatos.aspx");
                        break;
                    case (int)PermisosEnum.LeerCargas:
                        CrearItemDeMenu(PermisosEnum.LeerCargas.GetDescription(), "fa-truck-loading", "");
                        break;
                    case (int)PermisosEnum.LeerViajes:
                        CrearItemDeMenu(PermisosEnum.LeerViajes.GetDescription(), "fa-route", "");
                        break;
                    case (int)PermisosEnum.LeerReputacion:
                        CrearItemDeMenu(PermisosEnum.LeerReputacion.GetDescription(), "fa-retweet", "");
                        break;
                    case (int)PermisosEnum.LeerConductores:
                        CrearItemDeMenu(PermisosEnum.LeerConductores.GetDescription(), "fa-user-friends", "");
                        break;
                    case (int)PermisosEnum.LeerVehiculos:
                        CrearItemDeMenu(PermisosEnum.LeerVehiculos.GetDescription(), "fa-truck", "");
                        break;
                    case (int)PermisosEnum.LeerOfertas:
                        CrearItemDeMenu(PermisosEnum.LeerOfertas.GetDescription(), "fa-money-check-alt", "");
                        break;
                }
            }
        }

        private void CrearItemDeMenu(string menuDescripcion, string nombreIcono, string href)
        {
            HtmlGenericControl control;
            HtmlGenericControl li = new HtmlGenericControl("li");
            li.Attributes.Add("class", "nav-item");
            _navMenu.Controls.Add(li);
            control = CrearControl(menuDescripcion, nombreIcono, href);
            li.Controls.Add(control);
        }

        private HtmlGenericControl CrearControl(string descripcionEnlace, string nombreIcono, string href)
        {
            HtmlGenericControl span = new HtmlGenericControl("span")
            {
                InnerText = " " + descripcionEnlace
            };
            HtmlGenericControl i = new HtmlGenericControl("i");
            i.Attributes.Add("class", "fas fa-fw " + nombreIcono);
            HtmlGenericControl a = new HtmlGenericControl("a");
            a.Attributes.Add("class", "nav-link");
            a.Attributes.Add("href", href);
            a.Controls.Add(i);
            a.Controls.Add(span);
            return a;
        }

        protected void _btnLogout_Click(object sender, EventArgs e)
        {
            Session["UsuarioLogueado"] = null;
            Session["NombreUsuario"] = null;
            Session["Permisos"] = null;
            Response.Redirect("/");
        }
    }
}