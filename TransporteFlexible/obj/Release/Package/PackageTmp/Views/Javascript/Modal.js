function ShowPopup(title, body, redireccion) {
    $("#defaultPopUp .modal-title").html(title);
    $("#defaultPopUp .modal-body").html(body);
    $("#defaultPopUp").modal("show");
    $('#defaultPopUp').modal({ backdrop: 'static', keyboard: true });
    if (redireccion || 0 !== redireccion.length) {

        $("#defaultPopUp").click(function () {
            window.location.href = redireccion;
        });
    }
}