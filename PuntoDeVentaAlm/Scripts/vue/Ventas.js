
var cliente = new Vue({ 
    el:'#datosCliente',
    data: {
        cliente: [],
        //direccion:[],
        idCliente:-1,
        nombre:"",
        rfc:"",
        lCredito:0,
        dCredito:0
    },
    methods:{
        defineCliente: function(cliente,token){
            this.cliente = cliente;
            this.actualizaModelosCliente();
            this.defineDomicilio(token);
        },
        defineDomicilio: function(token){
            urlDomicilio = "/Clientes/buscaDireccion";
            dataD = {
                __RequestVerificationToken: token,
                idCliente:this.cliente.value.id
            };
            this.$http.post(
                urlDomicilio,
                dataD,
                { emulateJSON: true , responseType: "json"}
                ).then(
                    response =>{  
                        //console.log("%O",response.body[0]);
                        this.direccion = response.body[0];
                        // console.log(this.direccion.correo);
                        // console.log(this.direccion["correo"]);
                    }, response => {
                        console.log("erros al recuperar direccion");
                    }
                );
        },
        pasarCliente: function()
        {
            modalCliente.actualizaModeloCliente(this.cliente);
        },
        actualizaModelosCliente: function()
        {
            this.idCliente = this.cliente.value.id;
            this.nombre = this.cliente.label;
            this.rfc = this.cliente.value.rfc;
            this.lCredito = this.cliente.value.lCredito;
            this.dCredito = this.cliente.value.dCredito;
        },
        actualizaObjetoCliente: function()
        {
            this.cliente.value.id = this.idCliente;
            this.cliente.value.nombre = this.nombre;
            this.cliente.value.rfc = this.rfc;
            this.cliente.value.lCredito = this.lCredito;
            this.cliente.value.dCredito = this.dCredito;
        }
    }
});

var modalCliente = new Vue({
    el:'#modalCliente',
    data:{
        idClienteM:0,
        nombreM:"",
        rfcM:"",
        lCredito:0,
        dCredito:0,
        cliente:[],
        domicilio:[]
    },
    methods:{
        actualizaModeloCliente: function(cliente)
        {
            this.cliente = cliente;

        }
    }
})

$(document).ready(function () {
    var form = $('#__AjaxAntiForgeryFormCliente');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $("#idCliente").autocomplete({
        source: function (request, response) {
            var url2 = "/Clientes/buscaCliente";
            var data2 = $("#idCliente").val();
            $.ajax({
                type: "POST",
                url: url2,
                dataType: "json",
                data: {
                    __RequestVerificationToken: token,
                    term : data2
                },
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 2,
        select: function(event,ui){
            cliente.defineCliente(ui.item,token);
        }
    });
});