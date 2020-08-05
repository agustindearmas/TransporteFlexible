<%@ Page Title="Bienvenido" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TransporteFlexible._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Header -->
    <header class="masthead">
        <div class="container d-flex h-100 align-items-center">
            <div class="mx-auto text-center">
                <h1 class="mx-auto my-0 text-uppercase">T-FLEX</h1>
                <h2 class="text-white-50 mx-auto mt-2 mb-5">Conectamos dadores de carga con transportistas.</h2>
                <a href="#registro" class="btn btn-primary js-scroll-trigger">Registrate</a><br />
                <br />
                <a href="/Views/Seguridad/Ingreso.aspx" class="btn btn-primary js-scroll-trigger">Log In</a>
            </div>
        </div>
    </header>

    <!-- About Section -->
    <section id="about" class="about-section text-center">
        <div class="container">
            <div class="row">
                <div class="col-lg-8 mx-auto">
                    <h2 class="text-white mb-4">Transporte Flexible</h2>
                    <p class="text-white-50">
                        Transporte flexible es un emprendimiento dedicado a conectar dadores de carga con transportistas, por medio de un sistema conocido como T-FLEX.              
                        T-FLEX es un sistema (cliente servidor de acceso WEB) que permite la independización y autogestión del trabajo del transportista, y la posibilidad de elegir 
                        al dador de carga el precio, el seguro y el transportista que más se ajuste a sus necesidades de transporte. Es un tipo de negocio B2B que presta sus servicio a
                        pequeñas y medianas empresas dadoras de carga y transportistas
                    </p>
                </div>
            </div>
            <img src="img/inside.jpg" class="img-fluid" alt="">
        </div>
    </section>

    <!-- Projects Section -->
    <section id="registro" class="projects-section bg-light">
        <div class="container">

            <!-- Featured Project Row -->
            <div class="row align-items-center no-gutters mb-4 mb-lg-5">
                <div class="col-xl-8 col-lg-7">
                    <img class="img-fluid mb-3 mb-lg-0" src="img/clark.jpg" alt="">
                </div>
                <div class="col-xl-4 col-lg-5">
                    <div class="featured-text text-center text-lg-left">
                        <a class="h4" href="/Views/Seguridad/Registro.aspx?perfil=2">Dador de Carga</a>
                        <p class="text-black-50 mb-0">Registrate como Dador de Carga y encontra el transporte que necesitas</p>
                    </div>
                </div>
            </div>

            <!-- Project One Row -->
            <div class="row justify-content-center no-gutters mb-5 mb-lg-0">
                <div class="col-lg-6">
                    <img class="img-fluid" src="img/driver.jpg" alt="">
                </div>
                <div class="col-lg-6">
                    <div class="bg-black text-center h-100 project">
                        <div class="d-flex h-100">
                            <div class="project-text w-100 my-auto text-center text-lg-left">
                                <a class="text-white h4" href="/Views/Seguridad/Registro.aspx?perfil=4">Conductor/a Independiente</a>
                                <p class="mb-0 text-white-50">Registrate como conductor y comenza a disfrutar de nuestros servicios</p>
                                <hr class="d-none d-lg-block mb-0 ml-0">
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Project Two Row -->
            <div class="row justify-content-center no-gutters">
                <div class="col-lg-6">
                    <img class="img-fluid" src="img/varios.jpg" alt="">
                </div>
                <div class="col-lg-6 order-lg-first">
                    <div class="bg-black text-center h-100 project">
                        <div class="d-flex h-100">
                            <div class="project-text w-100 my-auto text-center text-lg-right">
                                <a class="text-white h4" href="/Views/Seguridad/Registro.aspx?perfil=3">Transportista Independiente</a>
                                <p class="mb-0 text-white-50">Registrate como Transportista privado, carga tu comboy y comenza a disfrutar de nuestros servicios</p>
                                <hr class="d-none d-lg-block mb-0 mr-0">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Signup Section -->
    <section id="signup" class="signup-section">
        <div class="">
            <div class="row">
                <div class="col-md-10 col-lg-8 mx-auto text-center">

                    <i class="far fa-paper-plane fa-2x mb-2 text-white"></i>
                    <h2 class="text-white mb-5">Subscribe to receive updates!</h2>

                    <div class="form-inline d-flex">
                        <input type="email" class="form-control flex-fill mr-0 mr-sm-2 mb-3 mb-sm-0" id="inputEmail" placeholder="Enter email address...">
                        <button type="submit" class="btn btn-primary mx-auto">Subscribe</button>
                    </div>

                </div>
            </div>
        </div>
    </section>

    <!-- Contact Section -->
    <section class="contact-section bg-black">
        <div class="container">

            <div class="row">

                <div class="col-md-4 mb-3 mb-md-0">
                    <div class="card py-4 h-100">
                        <div class="card-body text-center">
                            <i class="fas fa-map-marked-alt text-primary mb-2"></i>
                            <h4 class="text-uppercase m-0">Address</h4>
                            <hr class="my-4">
                            <div class="small text-black-50">4923 Market Street, Orlando FL</div>
                        </div>
                    </div>
                </div>

                <div class="col-md-4 mb-3 mb-md-0">
                    <div class="card py-4 h-100">
                        <div class="card-body text-center">
                            <i class="fas fa-envelope text-primary mb-2"></i>
                            <h4 class="text-uppercase m-0">Email</h4>
                            <a href="Properties/">Properties/</a>
                            <hr class="my-4">
                            <div class="small text-black-50">
                                <a href="#">hello@yourdomain.com</a>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-md-4 mb-3 mb-md-0">
                    <div class="card py-4 h-100">
                        <div class="card-body text-center">
                            <i class="fas fa-mobile-alt text-primary mb-2"></i>
                            <h4 class="text-uppercase m-0">Phone</h4>
                            <hr class="my-4">
                            <div class="small text-black-50">+1 (555) 902-8832</div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="social d-flex justify-content-center">
                <a href="#" class="mx-2">
                    <i class="fab fa-twitter"></i>
                </a>
                <a href="#" class="mx-2">
                    <i class="fab fa-facebook-f"></i>
                </a>
                <a href="#" class="mx-2">
                    <i class="fab fa-github"></i>
                </a>
            </div>

        </div>
    </section>

</asp:Content>
