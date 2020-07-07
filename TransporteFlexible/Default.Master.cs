using Common.Enums.Seguridad;
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
            _lblUsuario.Text = Session[SV.NombreUsuario.GD()].ToString();
            ArmarMenu();
        }
        private void ArmarMenu()
        {
            // ESTOS PERMISOS DEBERIAMOS IR A BUSCARLOS A LA BASE 
            List<int> permisos = (List<int>)Session[SV.Permisos.GD()];

            foreach (var permiso in permisos)
            {
                switch (permiso)
                {
                    case (int)PermisosEnum.LeerRolesPerfiles:
                        CrearItemDeMenu(PermisosEnum.LeerRolesPerfiles.GD(), "fa-tags", "");
                        break;
                    case (int)PermisosEnum.LeerUsuariosAdministrativos:
                        CrearItemDeMenu(PermisosEnum.LeerUsuariosAdministrativos.GD(), "fa-users", ViewsEnum.Usuario.GD());
                        break;
                    case (int)PermisosEnum.LeerPermisos:
                        CrearItemDeMenu(PermisosEnum.LeerPermisos.GD(), "fa-lock", "");
                        break;
                    case (int)PermisosEnum.LeerBitacora:
                        CrearItemDeMenu(PermisosEnum.LeerBitacora.GD(), "fa-table", ViewsEnum.Bitacora.GD());
                        break;
                    case (int)PermisosEnum.LeerBasedeDatos:
                        CrearItemDeMenu(PermisosEnum.LeerBasedeDatos.GD(), "fa-database", ViewsEnum.BaseDeDatos.GD());
                        break;
                    case (int)PermisosEnum.LeerCargas:
                        CrearItemDeMenu(PermisosEnum.LeerCargas.GD(), "fa-truck-loading", "");
                        break;
                    case (int)PermisosEnum.LeerViajes:
                        CrearItemDeMenu(PermisosEnum.LeerViajes.GD(), "fa-route", "");
                        break;
                    case (int)PermisosEnum.LeerReputacion:
                        CrearItemDeMenu(PermisosEnum.LeerReputacion.GD(), "fa-retweet", "");
                        break;
                    case (int)PermisosEnum.LeerConductores:
                        CrearItemDeMenu(PermisosEnum.LeerConductores.GD(), "fa-user-friends", "");
                        break;
                    case (int)PermisosEnum.LeerVehiculos:
                        CrearItemDeMenu(PermisosEnum.LeerVehiculos.GD(), "fa-truck", "");
                        break;
                    case (int)PermisosEnum.LeerOfertas:
                        CrearItemDeMenu(PermisosEnum.LeerOfertas.GD(), "fa-money-check-alt", "");
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
            Session[SV.UsuarioLogueado.GD()] = null;
            Session[SV.NombreUsuario.GD()] = null;
            Session[SV.Permisos.GD()] = null;
            Response.Redirect("/");
        }
    }
}